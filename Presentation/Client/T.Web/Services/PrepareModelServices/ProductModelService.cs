using AutoMapper;
using T.Library.Model;
using T.Library.Model.Interface;
using T.Library.Model.ViewsModel;
using T.Web.Areas.Admin.Models;
using T.Web.Models;
using T.Web.Services.PictureServices;
using T.Web.Services.ProductService;
using T.Web.Services.UrlRecordService;

namespace T.Web.Services.PrepareModelServices
{
    public interface IProductModelService
    {
        public Task<ProductBoxModel> PrepareProductBoxModel(Product product, ProductBoxModel model);
    }
    public class ProductModelService : IProductModelService
    {
        private readonly IProductService _productService;
        private readonly IUrlRecordService _urlRecordService;
        private readonly IPictureService _pictureService;
        private readonly ICategoryServiceCommon _categoryService;
        private readonly IMapper _mapper;

        public ProductModelService(IMapper mapper, IProductService productService, IUrlRecordService urlRecordService, ICategoryServiceCommon categoryService, IPictureService pictureService)
        {
            _mapper = mapper;
            _productService = productService;
            _urlRecordService = urlRecordService;
            _categoryService = categoryService;
            _pictureService = pictureService;
        }

        public async Task<ProductBoxModel> PrepareProductBoxModel(Product product, ProductBoxModel model)
        {
            if (product is not null)
            {
                model ??= new ProductBoxModel()
                {
                    Id = product.Id,
                };

                model.ProductName = product.Name;
                model.ProductSeName = await _urlRecordService.GetSeNameAsync(product);
                model.ProductImage = await GetProductImageAsync(product.Id);
                model.DisableBuyButton = product.DisableBuyButton;
                model.DisableWishlistButton = product.DisableWishlistButton;
                model.Categories = await GetCategoryOfProducts(product.Id);
                model.ProductPrice = new ProductBoxModel.ProductPriceModel()
                {
                    Price = product.Price.ToString("N0"),
                    OldPrice = product.OldPrice.ToString("N0"),
                    PriceValue = product.Price,
                    OldPriceValue = product.OldPrice
                };
                model.ShortDescription = product.ShortDescription;
            }

            return model;
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
