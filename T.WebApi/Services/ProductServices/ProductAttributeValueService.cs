using Microsoft.EntityFrameworkCore;
using T.Library.Model;
using T.Library.Model.Response;
using T.WebApi.Database.ConfigurationDatabase;
using T.WebApi.Extensions;

namespace T.WebApi.Services.ProductServices
{
    public interface IProductAttributeValueService
    {
        Task<ServiceResponse<ProductAttributeValue>> GetProductAttributeValuesByIdAsync(int id);
        Task<ServiceResponse<bool>> AddOrUpdateProductAttributeValue(ProductAttributeValue productAttributeValue);
        Task<ServiceResponse<bool>> DeleteProductAttributeValue(int id);
    }
    public class ProductAttributeValueService : IProductAttributeValueService
    {
        private readonly DatabaseContext _context;
        public ProductAttributeValueService(DatabaseContext context)
        {
            _context = context;
        }

        public async Task<ServiceResponse<bool>> AddOrUpdateProductAttributeValue(ProductAttributeValue productAttributeValue)
        {
            var pavTable = await _context.ProductAttributeValue
                    .Where(pav => pav.ProductAttributeMappingId == productAttributeValue.ProductAttributeMappingId && pav.Id == productAttributeValue.Id)
                    .FirstOrDefaultAsync();

            if (pavTable != null)
            {
                pavTable.ProductAttributeMappingId = productAttributeValue.ProductAttributeMappingId;
                pavTable.Name = productAttributeValue.Name;
                pavTable.ColorSquaresRgb = productAttributeValue.ColorSquaresRgb;
                pavTable.PriceAdjustment = productAttributeValue.PriceAdjustment;
                pavTable.PriceAdjustmentUsePercentage = productAttributeValue.PriceAdjustmentUsePercentage;
                pavTable.WeightAdjustment = productAttributeValue.WeightAdjustment;
                pavTable.Cost = productAttributeValue.Cost;
                pavTable.CustomerEntersQty = productAttributeValue.CustomerEntersQty;
                pavTable.Quantity = productAttributeValue.Quantity;
                pavTable.IsPreSelected = productAttributeValue.IsPreSelected;
                pavTable.DisplayOrder = productAttributeValue.DisplayOrder;
                pavTable.PictureId = productAttributeValue.PictureId;

                if (_context.IsRecordUnchanged(pavTable, productAttributeValue))
                {
                    return new ServiceErrorResponse<bool>("Data is unchanged");
                }
            }
            else
            {
                var _productAttributeValue = new ProductAttributeValue()
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

                _context.ProductAttributeValue.Add(_productAttributeValue);
            }

            var result = await _context.SaveChangesAsync();
            if (result == 0)
            {
                return new ServiceErrorResponse<bool>("Add or edit product mapping failed");
            }
            return new ServiceSuccessResponse<bool>();
        }

        public async Task<ServiceResponse<bool>> DeleteProductAttributeValue(int id)
        {
            try
            {
                var pav = await _context.ProductAttributeValue.Where(x => x.Id == id).FirstOrDefaultAsync();
                var result = _context.ProductAttributeValue.Remove(pav);
                await _context.SaveChangesAsync();
                return new ServiceSuccessResponse<bool>();
            }
            catch (Exception ex)
            {
                return new ServiceErrorResponse<bool>(ex.Message);
            }
        }

        public async Task<ServiceResponse<ProductAttributeValue>> GetProductAttributeValuesByIdAsync(int id)
        {
            using (_context)
            {
                var pav = await _context.ProductAttributeValue.Where(x => x.Id == id).FirstOrDefaultAsync();
                return new ServiceSuccessResponse<ProductAttributeValue>(pav);
            }
        }
    }
}
