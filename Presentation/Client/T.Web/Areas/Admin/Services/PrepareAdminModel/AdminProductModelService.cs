using AutoMapper;
using T.Library.Model;
using T.Web.Areas.Admin.Models;
using T.Web.Services.ProductService;
using Microsoft.AspNetCore.Mvc.Rendering;
using T.Web.Services.CategoryService;
using T.Library.Model.Common;
using T.Library.Model.Interface;
using T.Web.Areas.Admin.Models.SearchModel;
using T.Web.Services.PrepareModelServices.PrepareAdminModel;
using T.Library.Model.ViewsModel;
using T.Web.Services.UrlRecordService;

namespace T.Web.Areas.Admin.Services.PrepareModel
{
    public interface IAdminProductModelService
    {
        Task<ProductAttributeMappingModel> PrepareProductAttributeMappingModelAsync(ProductAttributeMappingModel model,
            Product product, ProductAttributeMapping productAttributeMapping);
        Task<List<ProductAttributeMappingModel>> PrepareProductAttributeMappingListModelAsync(
            Product product);
        Task<List<ProductAttributeValueModel>> PrepareProductAttributeValueListModelAsync(ProductAttributeMapping productAttributeMapping);
        Task<ProductAttributeValueModel> PrepareProductAttributeValueModelAsync(ProductAttributeValueModel model,
            ProductAttributeMapping productAttributeMapping, ProductAttributeValue productAttributeValue);
        Task<List<ProductPictureModel>> PrepareProductPictureModelAsync(Product product);
        Task<ProductCategoryModel> PrepareProductCategoryMappingModelAsync(ProductCategoryModel model,
            Product product, ProductCategory productCategory);
        Task<ProductSearchModel> PrepareProductSearchModelModelAsync(ProductSearchModel model);
        Task<RelatedProductSearchModel> PrepareRelatedProductSearchModel(RelatedProductSearchModel model);
        Task<ProductEditModel> PrepareProductEditModelModelAsync(Product product, ProductEditModel model);
    }
    public class AdminProductModelService : IAdminProductModelService
    {
        private readonly IProductAttributeCommon _productAttributeService;
        private readonly IMapper _mapper;
        private readonly IProductService _productService;
        private readonly ICategoryServiceCommon _categoryService;
        private readonly IBaseAdminModelService _baseAdminModelService;
        private readonly IUrlRecordService _urlRecordService;
        public AdminProductModelService(IProductAttributeCommon productAttributeService,
            IMapper mapper, IProductService productService, ICategoryServiceCommon categoryService, IBaseAdminModelService baseAdminModelService, IUrlRecordService urlRecordService)
        {
            _productAttributeService = productAttributeService;
            _mapper = mapper;
            _productService = productService;
            _categoryService = categoryService;
            _baseAdminModelService = baseAdminModelService;
            _urlRecordService = urlRecordService;
        }

        public async Task<List<ProductAttributeMappingModel>> PrepareProductAttributeMappingListModelAsync(Product product)
        {
            ArgumentNullException.ThrowIfNull(product);

            //get product attribute mappings

            var result = (await _productAttributeService
                                .GetProductAttributesMappingByProductIdAsync(product.Id));

            var pamList = _mapper.Map<List<ProductAttributeMappingModel>>(result);


            return pamList;
        }

        public async Task<ProductAttributeMappingModel> PrepareProductAttributeMappingModelAsync(ProductAttributeMappingModel model, Product product, ProductAttributeMapping productAttributeMapping)
        {
            if (productAttributeMapping != null)
            {
                //fill in model values from the entity
                model ??= new ProductAttributeMappingModel
                {
                    Id = productAttributeMapping.Id
                };
                _mapper.Map(productAttributeMapping, model);
                model.ProductAttributeName = (await _productAttributeService.GetProductAttributeByIdAsync(productAttributeMapping.ProductAttributeId)).Name;
                model.ProductAttributeId = productAttributeMapping.ProductAttributeId;
                model.TextPrompt = productAttributeMapping.TextPrompt;
                model.IsRequired = productAttributeMapping.IsRequired;
                model.DisplayOrder = productAttributeMapping.DisplayOrder;
                model.ValidationMinLength = productAttributeMapping.ValidationMinLength;
                model.ValidationMaxLength = productAttributeMapping.ValidationMaxLength;
                model.ValidationFileAllowedExtensions = productAttributeMapping.ValidationFileAllowedExtensions;
                model.ValidationFileMaximumSize = productAttributeMapping.ValidationFileMaximumSize;
                model.DefaultValue = productAttributeMapping.DefaultValue;
            }

            model.ProductId = product.Id;

            //prepare available product attributes
            model.AvailableProductAttributes = (await _productAttributeService.GetAllProductAttributeAsync()).Select(productAttribute => new SelectListItem
            {
                Text = productAttribute.Name,
                Value = productAttribute.Id.ToString()
            }).ToList();

            return model;
        }

        public async Task<List<ProductAttributeValueModel>> PrepareProductAttributeValueListModelAsync(ProductAttributeMapping productAttributeMapping)
        {
            ArgumentNullException.ThrowIfNull(productAttributeMapping);

            //get product attribute mappings

            var result = (await _productAttributeService
                                .GetProductAttributeValuesByMappingIdAsync(productAttributeMapping.Id));

            var pamList = _mapper.Map<List<ProductAttributeValueModel>>(result);


            return pamList;
        }

        public async Task<ProductAttributeValueModel> PrepareProductAttributeValueModelAsync(ProductAttributeValueModel model,
            ProductAttributeMapping productAttributeMapping, ProductAttributeValue productAttributeValue)
        {
            ArgumentNullException.ThrowIfNull(productAttributeMapping);

            if (productAttributeValue != null)
            {
                //fill in model values from the entity
                model ??= new ProductAttributeValueModel
                {
                    ProductAttributeMappingId = productAttributeValue.ProductAttributeMappingId,
                    Name = productAttributeValue.Name,
                    ColorSquaresRgb = productAttributeValue.ColorSquaresRgb,
                    PriceAdjustment = productAttributeValue.PriceAdjustment,
                    PriceAdjustmentUsePercentage = productAttributeValue.PriceAdjustmentUsePercentage,
                    WeightAdjustment = productAttributeValue.WeightAdjustment,
                    Cost = productAttributeValue.Cost,
                    CustomerEntersQty = productAttributeValue.CustomerEntersQty,
                    Quantity = productAttributeValue.Quantity,
                    IsPreSelected = productAttributeValue.IsPreSelected,
                    DisplayOrder = productAttributeValue.DisplayOrder,
                    PictureId = productAttributeValue.PictureId
                };

            }

            model.ProductAttributeMappingId = productAttributeMapping.Id;

            //set default values for the new model
            if (productAttributeValue == null)
                model.Quantity = 1;

            //prepare picture models
            var productPictures = (await _productService.GetProductPicturesByProductIdAsync(productAttributeMapping.ProductId));
            model.ProductPictureModels = productPictures.Select(productPicture => new ProductPictureModel
            {
                Id = productPicture.Id,
                ProductId = productPicture.ProductId,
                PictureId = productPicture.PictureId,
                PictureUrl = productPicture.Picture.UrlPath,
                DisplayOrder = productPicture.DisplayOrder
            }).ToList();

            return model;
        }

        public async Task<ProductCategoryModel> PrepareProductCategoryMappingModelAsync(ProductCategoryModel model,
            Product product, ProductCategory productCategory)
        {
            if (productCategory != null)
            {
                //fill in model values from the entity
                model ??= new ProductCategoryModel
                {
                    Id = productCategory.Id
                };
                _mapper.Map(productCategory, model);
                model.CategoryName = (await _categoryService.GetCategoryByIdAsync(productCategory.CategoryId)).Name;
                model.CategoryId = productCategory.CategoryId;
                model.IsFeaturedProduct = productCategory.IsFeaturedProduct;
                model.DisplayOrder = productCategory.DisplayOrder;
            }

            model.ProductId = product.Id;

            //prepare available product attributes
            model.AvailableCategories = (await _categoryService.GetAllCategoryAsync()).Select(productAttribute => new SelectListItem
            {
                Text = productAttribute.Name,
                Value = productAttribute.Id.ToString()
            }).ToList();

            return model;
        }

        public async Task<List<ProductPictureModel>> PrepareProductPictureModelAsync(Product product)
        {
            ArgumentNullException.ThrowIfNull(product);

            var model = new List<ProductPictureModel>();

            var productPictures = (await _productService.GetProductPicturesByProductIdAsync(product.Id));

            if (productPictures != null)
            {
                model = productPictures.Select(productPicture => new ProductPictureModel
                {
                    Id = productPicture.Id,
                    ProductId = productPicture.ProductId,
                    PictureId = productPicture.PictureId,
                    PictureUrl = productPicture.Picture.UrlPath,
                    DisplayOrder = productPicture.DisplayOrder
                }).ToList();
            }

            return model;
        }

        public async Task<ProductSearchModel> PrepareProductSearchModelModelAsync(ProductSearchModel model)
        {
            await _baseAdminModelService.PrepareSelectListCategoryAsync(model.AvailableCategories);
            await _baseAdminModelService.PrepareSelectListManufactureAsync(model.AvailableManufacturers);
            return model;
        }

        public async Task<ProductEditModel> PrepareProductEditModelModelAsync(Product product, ProductEditModel model)
        {
            if(product is not null)
            {
                model ??= new ProductEditModel()
                {
                    Id = product.Id,
                    Name = string.Empty
                };

                _mapper.Map(product, model);
                model.SeName = await _urlRecordService.GetSeNameAsync(product);
            }


            await _baseAdminModelService.PrepareSelectListCategoryAsync(model.AvailableCategories);

            return model;
        }

        public async Task<RelatedProductSearchModel> PrepareRelatedProductSearchModel(RelatedProductSearchModel model)
        {
            await _baseAdminModelService.PrepareSelectListCategoryAsync(model.AvailableCategories);
            await _baseAdminModelService.PrepareSelectListManufactureAsync(model.AvailableManufacturers);
            return model;
        }
    }
}
