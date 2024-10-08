﻿using AutoMapper;
using Microsoft.AspNetCore.Mvc.Rendering;
using TCommerce.Services.PrepareModelServices.PrepareAdminModel;
using TCommerce.Core.Models.Catalogs;
using TCommerce.Core.Interface;
using TCommerce.Web.Areas.Admin.Models.Catalog;

namespace TCommerce.Web.Areas.Admin.Services.PrepareModel
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
        Task<ProductModel> PrepareProductModelModelAsync(Product product, ProductModel model);
    }
    public class AdminProductModelService : IAdminProductModelService
    {
        private readonly IProductAttributeService _productAttributeService;
        private readonly IMapper _mapper;
        private readonly IProductService _productService;
        private readonly ICategoryService _categoryService;
        private readonly IBaseAdminModelService _baseAdminModelService;
        private readonly IUrlRecordService _urlRecordService;
        private readonly IPictureService _pictureService;
        public AdminProductModelService(IProductAttributeService productAttributeService,
            IMapper mapper, IProductService productService, ICategoryService categoryService, IBaseAdminModelService baseAdminModelService, IUrlRecordService urlRecordService, IPictureService pictureService)
        {
            _productAttributeService = productAttributeService;
            _mapper = mapper;
            _productService = productService;
            _categoryService = categoryService;
            _baseAdminModelService = baseAdminModelService;
            _urlRecordService = urlRecordService;
            _pictureService = pictureService;
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

            if (productPictures != null)
            {
                foreach (var productPicture in productPictures)
                {
                    model.ProductPictureModels.Add(new ProductPictureModel
                    {
                        Id = productPicture.Id,
                        ProductId = productPicture.ProductId,
                        PictureId = productPicture.PictureId,
                        PictureUrl = (await _pictureService.GetPictureByIdAsync(productPicture.PictureId)).UrlPath,
                        DisplayOrder = productPicture.DisplayOrder
                    });
                }
            }
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
                foreach (var productPicture in productPictures)
                {
                    model.Add(new ProductPictureModel
                    {
                        Id = productPicture.Id,
                        ProductId = productPicture.ProductId,
                        PictureId = productPicture.PictureId,
                        PictureUrl = (await _pictureService.GetPictureByIdAsync(productPicture.PictureId)).UrlPath,
                        DisplayOrder = productPicture.DisplayOrder
                    });
                }
            }

            return model;
        }

        public async Task<ProductSearchModel> PrepareProductSearchModelModelAsync(ProductSearchModel model)
        {
            await _baseAdminModelService.PrepareSelectListCategoryAsync(model.AvailableCategories);
            await _baseAdminModelService.PrepareSelectListManufactureAsync(model.AvailableManufacturers);
            return model;
        }

        public async Task<ProductModel> PrepareProductModelModelAsync(Product product, ProductModel model)
        {
            if(product is not null)
            {
                model ??= new ProductModel()
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
