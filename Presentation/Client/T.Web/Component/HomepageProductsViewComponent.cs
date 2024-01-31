using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;
using T.Library.Model;
using T.Library.Model.Catalogs;
using T.Library.Model.Interface;
using T.Library.Model.Security;
using T.Library.Model.ViewsModel;
using T.Web.Models;
using T.Web.Services.PictureServices;
using T.Web.Services.ProductService;
using T.Web.Services.UrlRecordService;
using static T.Web.Models.HomePageModel;

namespace T.Web.Component
{
    public class HomePageProductsViewComponent : ViewComponent
    {
        private readonly IProductService _productService;
        private readonly IUrlRecordService _urlRecordService;
        private readonly IPictureService _pictureService;
        private readonly IMapper _mapper;
        private readonly ICategoryService _categoryService;
        public HomePageProductsViewComponent(IProductService productService, IUrlRecordService urlRecordService, IPictureService pictureService, IMapper mapper, ICategoryService categoryService)
        {
            _productService = productService;
            _urlRecordService = urlRecordService;
            _pictureService = pictureService;
            _mapper = mapper;
            _categoryService = categoryService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var productsHomePage = await _productService.GetAllProductsDisplayedOnHomepageAsync();
            var homepagelist = new List<HomePageModel>()
            {
                new HomePageModel()
                {
                    Title = "Feartured",
                    ProductList = (await Task.WhenAll(productsHomePage.Select(async product => new ProductBoxModel
                    {
                        ProductName = product.Name,
                        ProductSeName = await _urlRecordService.GetSeNameAsync(product),
                        ProductImage = await GetProductImageAsync(product.Id),
                        DisableBuyButton = product.DisableBuyButton,
                        DisableWishlistButton = product.DisableWishlistButton,
                        Categories = await GetCategoryOfProducts(product.Id),
                        ProductPrice = new ProductBoxModel.ProductPriceModel()
                        {
                            Price = product.Price.ToString("N0"),
                            OldPrice = product.OldPrice.ToString("N0"),
                            PriceValue = product.Price,
                            OldPriceValue = product.OldPrice
                        },
                    }))).ToList()
                }
            };

            return View(homepagelist);
        }
        private async Task<PictureModel> GetProductImageAsync(int productId)
        {
            var productPictures = await _productService.GetProductPicturesByProductIdAsync(productId);

            if (productPictures != null && productPictures.Any())
            {
                var pictureId = productPictures.FirstOrDefault().PictureId;
                if (pictureId > 0)
                {
                    var picture = await _pictureService.GetPictureByIdAsync(pictureId);
                    return new PictureModel()
                    {
                        TitleAttribute = picture.TitleAttribute,
                        AltAttribute = picture.AltAttribute,
                        ImageUrl = picture.UrlPath
                    };
                }
            }

            return new PictureModel();
        }
        private async Task<List<ProductBoxModel.CategoryOfProduct>> GetCategoryOfProducts(int productId)
        {
            var productCategories = await _categoryService.GetProductCategoriesByProductIdAsync(productId);

            var categoryTasks = productCategories.Select(async pc =>
            {
                var category = await _categoryService.GetCategoryByIdAsync(pc.CategoryId);
                return new ProductBoxModel.CategoryOfProduct
                {
                    CategoryName = category?.Name,
                    SeName = await _urlRecordService.GetSeNameAsync(category)
                };
            });

            return (await Task.WhenAll(categoryTasks)).Where(category => category.CategoryName != null).ToList();
        }

    }
}
