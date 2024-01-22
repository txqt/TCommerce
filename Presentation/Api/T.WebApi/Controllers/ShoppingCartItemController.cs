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
using static T.Library.Model.ViewsModel.ShoppingCartItemModel;

namespace T.WebApi.Controllers
{
    [Route("api/shopping-cart-item")]
    [ApiController]
    public class ShoppingCartItemController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly IMapper _mapper;
        private readonly IUserServiceCommon _userServiceCommon;
        private readonly IProductAttributeConverter _productAttributeConverter;
        private readonly IShoppingCartService _shoppingCartService;
        private readonly IProductAttributeService _productAttribute;

        public ShoppingCartItemController(IProductService productService, IMapper mapper, IUserServiceCommon userServiceCommon, IProductAttributeConverter productAttributeConverter, IShoppingCartService shoppingCartService, IProductAttributeService productAttribute)
        {
            _productService = productService;
            _mapper = mapper;
            _userServiceCommon = userServiceCommon;
            _productAttributeConverter = productAttributeConverter;
            _shoppingCartService = shoppingCartService;
            _productAttribute = productAttribute;
        }

        [HttpGet("me")]
        [ProducesResponseType(typeof(ShoppingCartItemModel), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
        [CheckPermission]
        public async Task<IActionResult> GetCurrentShoppingCart()
        {
            var customer = await _userServiceCommon.GetCurrentUser();

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
            var newShoppingCartItem = _mapper.Map<ShoppingCartItem>(shoppingCartItemModel);

            // We know that the product id and customer id will be provided because they are required by the validator.
            // TODO: validate
            var product = await _productService.GetByIdAsync(newShoppingCartItem.ProductId);

            if (product == null)
            {
                return NotFound();
            }

            var productAttributeMapping = await _productAttribute.GetProductAttributesMappingByProductIdAsync(product.Id);

            var validateCart = await _shoppingCartService.ValidateCart(productAttributeMapping, shoppingCartItemModel.Attributes);
            if (validateCart.Count > 0)
            {
                return BadRequest(new ServiceErrorResponse<bool>() { Message = string.Join(",", validateCart)});
            }

            var customer = await _userServiceCommon.GetCurrentUser();

            if (customer == null || customer.Id != newShoppingCartItem.UserId)
            {
                return Unauthorized();
            }

            var attributesJson = await _productAttributeConverter.ConvertToJsonAsync(shoppingCartItemModel.Attributes, product.Id);

            newShoppingCartItem.AttributeJson = attributesJson;
            newShoppingCartItem.ShoppingCartType = ShoppingCartType.ShoppingCart;
            await SaveItemAsync(newShoppingCartItem);

            var hasShoppingCartItems = !_shoppingCartService.IsUserShoppingCartEmpty(customer);
            if (hasShoppingCartItems != customer.HasShoppingCartItems)
            {
                customer.HasShoppingCartItems = hasShoppingCartItems;
                await _userServiceCommon.UpdateUserAsync(customer);
            }

            return Ok(new ServiceSuccessResponse<bool>());
        }

        private async Task<List<ShoppingCartItemModel>> LoadCurrentShoppingCartItems(ShoppingCartType shoppingCartType, UserModel user)
        {
            var updatedShoppingCart = await _shoppingCartService.GetShoppingCartAsync(user, shoppingCartType);

            var model = _mapper.Map<List<ShoppingCartItemModel>>(updatedShoppingCart);

            if(updatedShoppingCart.Count > 0)
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
        protected async Task SaveItemAsync(ShoppingCartItem shoppingCartItem)
        {
            var user = await _userServiceCommon.GetCurrentUser();

            var updatedShoppingCart = await _shoppingCartService.GetShoppingCartAsync(user, shoppingCartItem.ShoppingCartType);

            var model = _mapper.Map<List<ShoppingCartItemModel>>(updatedShoppingCart);

            if(model.Count > 0)
            {
                var existCI = updatedShoppingCart.Where(x => x.ShoppingCartType == ShoppingCartType.ShoppingCart).FirstOrDefault(x => x.AttributeJson == shoppingCartItem.AttributeJson);
                if (existCI is not null)
                {
                    existCI.Quantity += shoppingCartItem.Quantity;
                    await _shoppingCartService.UpdateAsync(existCI);
                }
                else
                {
                    await _shoppingCartService.CreateAsync(shoppingCartItem);
                }
            }
            else
            {
                await _shoppingCartService.CreateAsync(shoppingCartItem);
            }
        }
    }
}
