using Xunit;
using Moq;
using AutoMapper;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TCommerce.Web.Areas.Admin.Controllers;
using TCommerce.Services.DiscountServices;
using TCommerce.Web.Areas.Admin.Services.PrepareAdminModel;
using TCommerce.Core.Interface;
using TCommerce.Core.Models.Discounts;
using TCommerce.Core.Models.Paging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Logging;
using TCommerce.Web.Areas.Admin.Models.Discounts;

namespace TCommerce.Tests.TCommerce.Web.Areas.Admin.Controllers
{
    public class DiscountControllerTests
    {
        private readonly Mock<IDiscountService> _discountServiceMock;
        private readonly Mock<IAdminDiscountModelService> _discountModelServiceMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<IProductService> _productServiceMock;
        private readonly Mock<ICategoryService> _categoryServiceMock;
        private readonly Mock<IManufacturerService> _manufacturerServiceMock;
        private readonly DiscountController _controller;

        public DiscountControllerTests()
        {
            _discountServiceMock = new Mock<IDiscountService>();
            _discountModelServiceMock = new Mock<IAdminDiscountModelService>();
            _mapperMock = new Mock<IMapper>();
            _productServiceMock = new Mock<IProductService>();
            _categoryServiceMock = new Mock<ICategoryService>();
            _manufacturerServiceMock = new Mock<IManufacturerService>();
            _controller = new DiscountController(
                _discountServiceMock.Object,
                _discountModelServiceMock.Object,
                _mapperMock.Object,
                _productServiceMock.Object,
                _categoryServiceMock.Object,
                _manufacturerServiceMock.Object
            );
        }

        [Fact]
        public async Task Index_ReturnsViewResult_WithDiscountSearchModel()
        {
            // Arrange
            var searchModel = new DiscountSearchModel();
            _discountModelServiceMock.Setup(service => service.PrepareDiscountSearchModelModelAsync(It.IsAny<DiscountSearchModel>()))
                .ReturnsAsync(searchModel);

            // Act
            var result = await _controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<DiscountSearchModel>(viewResult.Model);
            Assert.Equal(searchModel, model);
        }

        // Kiểm thử cho CreateDiscount GET
        [Fact]
        public async Task CreateDiscount_Get_ReturnsViewResult_WithDiscountModel()
        {
            // Arrange
            var discountModel = new DiscountModel();
            _discountModelServiceMock.Setup(service => service.PrepareDiscountModelAsync(It.IsAny<DiscountModel>(), null))
                .ReturnsAsync(discountModel);

            // Act
            var result = await _controller.CreateDiscount();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<DiscountModel>(viewResult.Model);
            Assert.Equal(discountModel, model);
        }

        // Kiểm thử cho CreateDiscount POST
        [Fact]
        public async Task CreateDiscount_Post_InvalidModelState_ReturnsViewResult()
        {
            // Arrange
            var discountModel = new DiscountModel();
            _controller.ModelState.AddModelError("Name", "Required");
            _discountModelServiceMock.Setup(service => service.PrepareDiscountModelAsync(discountModel, null))
                .ReturnsAsync(discountModel);

            // Act
            var result = await _controller.CreateDiscount(discountModel, false);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<DiscountModel>(viewResult.Model);
            Assert.NotNull(model);
        }

        [Fact]
        public async Task CreateDiscount_Post_ValidModelState_ReturnsRedirectToIndex()
        {
            // Arrange
            var discountModel = new DiscountModel { Id = 1 };
            var discount = new Discount();
            _mapperMock.Setup(mapper => mapper.Map<Discount>(It.IsAny<DiscountModel>())).Returns(discount);
            _discountServiceMock.Setup(service => service.CreateDiscountAsync(discount)).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.CreateDiscount(discountModel, false);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal(nameof(_controller.Index), redirectResult.ActionName);
        }

        [Fact]
        public async Task CreateDiscount_Post_ValidModelState_ContinueEditing_ReturnsRedirectToEdit()
        {
            // Arrange
            var discountModel = new DiscountModel { Id = 1 };
            var discount = new Discount { Id = 1 };
            _mapperMock.Setup(mapper => mapper.Map<Discount>(It.IsAny<DiscountModel>())).Returns(discount);
            _discountServiceMock.Setup(service => service.CreateDiscountAsync(discount)).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.CreateDiscount(discountModel, true);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("EditDiscount", redirectResult.ActionName);
            Assert.Equal(discount.Id, redirectResult.RouteValues["id"]);
        }
    }
}
