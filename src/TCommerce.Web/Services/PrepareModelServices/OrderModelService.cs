using TCommerce.Core.Interface;
using TCommerce.Core.Models.Orders;
using TCommerce.Core.Models.ViewsModel;
using TCommerce.Web.Models;

namespace TCommerce.Web.Services.PrepareModelServices
{
    public interface IOrderModelService
    {
        Task<OrderDetailsModel> PrepareOrderDetailsModelAsync(Order order);
    }
    public class OrderModelService : IOrderModelService
    {
        private readonly IOrderService _orderService;
        private readonly IUserService _userService;
        private readonly IPaymentService _paymentService;
        private readonly IProductService _productService;
        private readonly IUrlRecordService _urlRecordService;
        private readonly IPictureService _pictureService;

        public OrderModelService(IPictureService pictureService, IUrlRecordService urlRecordService, IProductService productService, IPaymentService paymentService, IUserService userService, IOrderService orderService)
        {
            _pictureService = pictureService;
            _urlRecordService = urlRecordService;
            _productService = productService;
            _paymentService = paymentService;
            _userService = userService;
            _orderService = orderService;
        }

        public async Task<OrderDetailsModel> PrepareOrderDetailsModelAsync(Order order)
        {
            ArgumentNullException.ThrowIfNull(order);
            var model = new OrderDetailsModel
            {
                Id = order.Id,
                CreatedOn = order.CreatedOnUtc,
                OrderStatus = order.OrderStatus.ToString(),
                PaymentMethod = order.PaymentStatus.ToString(),
                IsReOrderAllowed = true
            };

            //payment method
            var customer = await _userService.GetUserById(order.UserId);
            var paymentMethod = _paymentService.GetPaymentMethodBySystemName(order.PaymentMethodSystemName);
            model.PaymentMethod = paymentMethod.Name;


            //discount (applied to order total)
            model.OrderTotalDiscount = order.OrderDiscount.ToString();
            model.OrderTotalDiscountValue = order.OrderDiscount;

            model.OrderSubtotal = order.OrderSubtotalInclTax.ToString();
            model.OrderSubtotalValue = order.OrderSubtotalInclTax;

            //total
            model.OrderTotal = order.OrderTotal.ToString();
            model.OrderTotalValue = order.OrderTotal;

            //order notes
            foreach (var orderNote in (await _orderService.GetOrderNotesByOrderIdAsync(order.Id, true))
                     .OrderByDescending(on => on.CreatedOnUtc)
                     .ToList())
            {
                model.OrderNotes.Add(new OrderDetailsModel.OrderNote
                {
                    Id = orderNote.Id,
                    Note = orderNote.Note,
                    CreatedOn = orderNote.CreatedOnUtc,
                });
            }

            //purchased products
            model.ShowSku = true;
            model.ShowProductThumbnail = true;

            var orderItems = await _orderService.GetOrderItemsByOrderIdAsync(order.Id);

            foreach (var orderItem in orderItems)
            {
                var product = await _productService.GetByIdAsync(orderItem.ProductId);

                var orderItemModel = new OrderDetailsModel.OrderItemModel
                {
                    Id = orderItem.Id,
                    Sku = product.Sku,
                    ProductId = product.Id,
                    ProductName = product.Name,
                    ProductSeName = await _urlRecordService.GetSeNameAsync(product),
                    Price = orderItem.Price.ToString(),
                    PriceValue = orderItem.Price,
                    Quantity = orderItem.Quantity,
                    AttributeInfo = orderItem.AttributeInfo,
                    SubTotal = (orderItem.Quantity * orderItem.Price).ToString(),
                    SubTotalValue = (orderItem.Quantity * orderItem.Price)
                };
                model.Items.Add(orderItemModel);

                var allPictureMappings = await _productService.GetProductPicturesByProductIdAsync(orderItem.ProductId);
                if (allPictureMappings.Any())
                {
                    var pictureId = allPictureMappings.FirstOrDefault().PictureId;
                    if (pictureId > 0)
                    {
                        var picture = await _pictureService.GetPictureByIdAsync(pictureId);
                        orderItemModel.Picture = new PictureModel()
                        {
                            TitleAttribute = picture.TitleAttribute,
                            AltAttribute = picture.AltAttribute,
                            ImageUrl = picture.UrlPath
                        };
                    }
                }
            }

            return model;
        }
    }
}
