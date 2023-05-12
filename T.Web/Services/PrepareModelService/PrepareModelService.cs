using AutoMapper;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Text.Json;
using T.Library.Model;
using T.Web.Areas.Admin.Models;
using T.Web.Services.ProductService;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace T.Web.Services.PrepareModel
{
    public interface IPrepareModelService
    {
        Task<ProductAttributeMappingModel> PrepareProductAttributeMappingModelAsync(ProductAttributeMappingModel model,
            Product product, ProductAttributeMapping productAttributeMapping);
        Task<List<ProductAttributeMappingModel>> PrepareProductAttributeMappingListModelAsync(
            Product product);
        Task<List<ProductAttributeValueModel>> PrepareProductAttributeValueListModelAsync(ProductAttributeMapping productAttributeMapping);
        Task<ProductAttributeValueModel> PrepareProductAttributeValueModelAsync(ProductAttributeValueModel model,
            ProductAttributeMapping productAttributeMapping, ProductAttributeValue productAttributeValue);
        Task<List<ProductPictureModel>> PrepareProductPictureModelAsync(Product product);
    }
    public class PrepareModelService : IPrepareModelService
    {
        private readonly IProductAttributeService _productAttributeService;
        private readonly IProductAttributeMappingService _productAttributeMappingService;
        private readonly IMapper _mapper;
        private readonly IProductService _productService;
        public PrepareModelService(IProductAttributeService productAttributeService, IProductAttributeMappingService productAttributeMappingService,
            IMapper mapper, IProductService productService)
        {
            _productAttributeService = productAttributeService;
            _productAttributeMappingService = productAttributeMappingService;
            _mapper = mapper;
            _productService = productService;
        }
        public async Task<List<ProductAttributeMappingModel>> PrepareProductAttributeMappingListModelAsync(Product product)
        {
            if (product == null)
                throw new ArgumentNullException(nameof(product));

            //get product attribute mappings

            var result = (await _productAttributeMappingService
                                .GetProductAttributeMappingByProductId(product.Id)).Data;

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
                model.ProductAttributeName = (await _productAttributeService.Get(productAttributeMapping.ProductAttributeId)).Data.Name;
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
            model.AvailableProductAttributes = (await _productAttributeService.GetAll()).Select(productAttribute => new SelectListItem
            {
                Text = productAttribute.Name,
                Value = productAttribute.Id.ToString()
            }).ToList();

            return model;
        }

        public async Task<List<ProductAttributeValueModel>> PrepareProductAttributeValueListModelAsync(ProductAttributeMapping productAttributeMapping)
        {
            if (productAttributeMapping == null)
                throw new ArgumentNullException(nameof(productAttributeMapping));

            //get product attribute mappings

            var result = (await _productAttributeMappingService
                                .GetProductAttributeValuesAsync(productAttributeMapping.Id)).Data;

            var pamList = _mapper.Map<List<ProductAttributeValueModel>>(result);


            return pamList;
        }

        public async Task<ProductAttributeValueModel> PrepareProductAttributeValueModelAsync(ProductAttributeValueModel model, ProductAttributeMapping productAttributeMapping, ProductAttributeValue productAttributeValue)
        {
            if (productAttributeMapping == null)
                throw new ArgumentNullException(nameof(productAttributeMapping));

            if (productAttributeValue != null)
            {
                //fill in model values from the entity
                model ??= new ProductAttributeValueModel
                {
                    ProductAttributeMappingId = productAttributeValue.ProductAttributeMappingId,
                    AssociatedProductId = productAttributeValue.AssociatedProductId,
                    Name = productAttributeValue.Name,
                    ColorSquaresRgb = productAttributeValue.ColorSquaresRgb,
                    ImageSquaresPictureId = productAttributeValue.ImageSquaresPictureId,
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

                model.AssociatedProductName = ((await _productService.Get(productAttributeValue.AssociatedProductId))).Data?.Name;

            }

            model.ProductAttributeMappingId = productAttributeMapping.Id;

            //set default values for the new model
            if (productAttributeValue == null)
                model.Quantity = 1;

            //prepare picture models
            var productPictures = (await _productService.GetProductPicturesByProductIdAsync(productAttributeMapping.ProductId)).Data;
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

        public async Task<List<ProductPictureModel>> PrepareProductPictureModelAsync(Product product)
        {
            if (product == null)
                throw new ArgumentNullException(nameof(product));

            var model = new List<ProductPictureModel>();

            var productPictures = (await _productService.GetProductPicturesByProductIdAsync(product.Id)).Data;

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
    }
}
