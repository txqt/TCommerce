using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.Net;
using T.Library.Model;
using T.Library.Model.Interface;
using T.Library.Model.Orders;
using T.Library.Model.Response;
using T.Library.Model.Users;
using T.Library.Model.ViewsModel;
using T.WebApi.Attribute;
using T.WebApi.Services.ProductServices;
using T.WebApi.Services.ShoppingCartServices;
using T.WebApi.Services.UserServices;
using static T.Library.Model.ViewsModel.ShoppingCartItemModel;

namespace T.WebApi.Controllers
{
    [Route("api/shopping-cart-items")]
    [ApiController]
    [CheckPermission]
    public class ShoppingCartItemController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly IMapper _mapper;
        private readonly IUserService _userService;
        private readonly IProductAttributeConverter _productAttributeConverter;
        private readonly IShoppingCartService _shoppingCartService;
        private readonly IProductAttributeService _productAttribute;

        public ShoppingCartItemController(IProductService productService, IMapper mapper, IUserService userService, IProductAttributeConverter productAttributeConverter, IShoppingCartService shoppingCartService, IProductAttributeService productAttribute)
        {
            _productService = productService;
            _mapper = mapper;
            _userService = userService;
            _productAttributeConverter = productAttributeConverter;
            _shoppingCartService = shoppingCartService;
            _productAttribute = productAttribute;
        }

        [HttpGet("me")]
        [ProducesResponseType(typeof(ShoppingCartItemModel), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
        public async Task<IActionResult> GetCurrentShoppingCart()
        {
            var customer = await _userService.GetCurrentUser();

            if (customer is null)
            {
                return NotFound();
            }

            var shoppingCartType = ShoppingCartType.ShoppingCart;

            // load current shopping cart and return it as result of request
            var shoppingCartsObject = await LoadCurrentShoppingCartItems(shoppingCartType, customer);

            return Ok(shoppingCartsObject);
        }

        [HttpPost("")]
        [CheckPermission]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        public async Task<IActionResult> CreateShoppingCartItem(
            ShoppingCartItemModel shoppingCartItemModel)
        {
            var shoppingCartItem = _mapper.Map<ShoppingCartItem>(shoppingCartItemModel);

            var product = await _productService.GetByIdAsync(shoppingCartItem.ProductId);

            if (product == null)
            {
                return NotFound();
            }
            var user = await _userService.GetCurrentUser();

            var attributesJson = await _productAttributeConverter.ConvertToJsonAsync(shoppingCartItemModel.Attributes, product.Id);
            shoppingCartItem.AttributeJson = attributesJson;

            var warnings = new List<string>();
            warnings.AddRange(await _shoppingCartService.GetShoppingCartItemWarningsAsync(user, shoppingCartItemModel.ShoppingCartType, product, attributesJson, shoppingCartItemModel.Quantity));

            if (warnings.Any())
            {
                return BadRequest(new ServiceErrorResponse<bool>() { Message = string.Join(",", warnings) });
            }

            await _shoppingCartService.AddToCartAsync(user, shoppingCartItem.ShoppingCartType, product, attributesJson, shoppingCartItem.Quantity);

            return Ok(new ServiceSuccessResponse<bool>());
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSelectedItemShoppingCart(int id)
        {
            var user = await _userService.GetCurrentUser();
            var cart = await _shoppingCartService.GetById(id);
            if (cart.UserId != user.Id)
            {
                return NotFound();
            }

            var result = await _shoppingCartService.DeleteAsync(id);
            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

        [HttpDelete("delete-list")]
        public async Task<IActionResult> DeleteBatchShoppingCart(List<int> ids)
        {
            var user = await _userService.GetCurrentUser();
            foreach (var id in ids)
            {
                var cart = await _shoppingCartService.GetById(id);
                if (cart.UserId != user.Id)
                {
                    return NotFound();
                }
            }

            var result = await _shoppingCartService.DeleteBatchAsync(ids);
            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

        private async Task<List<ShoppingCartItemModel>> LoadCurrentShoppingCartItems(ShoppingCartType shoppingCartType, UserModel user)
        {
            var updatedShoppingCart = await _shoppingCartService.GetShoppingCartAsync(user, shoppingCartType);

            var model = _mapper.Map<List<ShoppingCartItemModel>>(updatedShoppingCart);

            if (updatedShoppingCart.Count > 0)
            {
                foreach (var newShoppingCart in updatedShoppingCart)
                {
                    var product = await _productService.GetByIdAsync(newShoppingCart.ProductId);
                    var currentCartModel = model.Where(x => x.Id == newShoppingCart.Id).FirstOrDefault();
                    //currentCartModel.ProductModel = _mapper.Map<ProductModel>(product);
                    currentCartModel.Attributes = await _productAttributeConverter.ConvertToObject(newShoppingCart.AttributeJson);
                }
            }

            return model ??= null;
        }


        [HttpPut("batch")]
        [CheckPermission]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        public async Task<IActionResult> UpdateShoppingCartItems(
            List<ShoppingCartItemModel> shoppingCartItemModels)
        {
            var user = await _userService.GetCurrentUser();
            if (shoppingCartItemModels?.Count > 0)
            {
                foreach (var shoppingCartItemModel in shoppingCartItemModels)
                {
                    var product = await _productService.GetByIdAsync(shoppingCartItemModel.ProductId);

                    if (product == null)
                    {
                        return NotFound();
                    }

                    var attributesJson = await _productAttributeConverter.ConvertToJsonAsync(shoppingCartItemModel.Attributes, product.Id);

                    var warnings = new List<string>();
                    warnings.AddRange(await _shoppingCartService.GetShoppingCartItemWarningsAsync(user, shoppingCartItemModel.ShoppingCartType, product, attributesJson, shoppingCartItemModel.Quantity));

                    if (warnings.Any())
                    {
                        return BadRequest(new ServiceErrorResponse<bool>() { Message = string.Join(",", warnings) });
                    }

                    await _shoppingCartService.UpdateCartItemAsync(user, shoppingCartItemModel.Id, shoppingCartItemModel.ShoppingCartType, product, attributesJson, shoppingCartItemModel.Quantity);
                }
            }
            else
            {
                return NotFound();
            }

            return Ok(new ServiceSuccessResponse<bool>());
        }

        [HttpPut("")]
        [CheckPermission]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        public async Task<IActionResult> UpdateShoppingCartItem(
            ShoppingCartItemModel shoppingCartItemModel)
        {
            var product = await _productService.GetByIdAsync(shoppingCartItemModel.ProductId);

            if (product == null)
            {
                return NotFound();
            }

            var user = await _userService.GetCurrentUser();

            var attributesJson = await _productAttributeConverter.ConvertToJsonAsync(shoppingCartItemModel.Attributes, product.Id);

            var warnings = await _shoppingCartService.GetShoppingCartItemWarningsAsync(user, shoppingCartItemModel.ShoppingCartType, product, attributesJson, shoppingCartItemModel.Quantity);
            if (warnings.Any())
            {
                return BadRequest(new ServiceErrorResponse<bool>() { Message = string.Join(",", warnings) });
            }

            await _shoppingCartService.UpdateCartItemAsync(user, shoppingCartItemModel.Id, shoppingCartItemModel.ShoppingCartType, product, attributesJson, shoppingCartItemModel.Quantity);

            return Ok(new ServiceSuccessResponse<bool>());
        }

        [HttpGet("warnings")]
        public async Task<IActionResult> GetWarningsShoppingCart([FromQuery] List<ShoppingCartItemModel> shoppingCartItemModels)
        {
            var user = await _userService.GetCurrentUser();
            var warnings = new List<string>();
            if (shoppingCartItemModels is not null)
            {
                foreach (var item in shoppingCartItemModels)
                {
                    var product = await _productService.GetByIdAsync(item.ProductId);
                    var attributesJson = await _productAttributeConverter.ConvertToJsonAsync(item.Attributes, product.Id);
                    warnings.AddRange(await _shoppingCartService.GetShoppingCartItemWarningsAsync(user, item.ShoppingCartType, product, attributesJson, item.Quantity));
                }
            }
            return new JsonResult(warnings);
        }
        [HttpGet("warning")]
        public async Task<IActionResult> GetWarningShoppingCart([FromQuery] ShoppingCartItemModel shoppingCartItemModel)
        {
            var user = await _userService.GetCurrentUser();
            var warnings = new List<string>();
            if(shoppingCartItemModel is not null)
            {
                var product = await _productService.GetByIdAsync(shoppingCartItemModel.ProductId);
                var attributesJson = await _productAttributeConverter.ConvertToJsonAsync(shoppingCartItemModel.Attributes, product.Id);
                warnings.AddRange(await _shoppingCartService.GetShoppingCartItemWarningsAsync(user, shoppingCartItemModel.ShoppingCartType, product, attributesJson, shoppingCartItemModel.Quantity));
            }
            return new JsonResult(warnings);
        }
    }
}
