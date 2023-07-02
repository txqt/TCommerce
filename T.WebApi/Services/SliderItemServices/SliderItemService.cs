using Microsoft.EntityFrameworkCore;
using T.Library.Model;
using T.Library.Model.BannerItem;
using T.Library.Model.Common;
using T.Library.Model.Response;
using T.Library.Model.Slider;
using T.WebApi.Database.ConfigurationDatabase;
using T.WebApi.Extensions;
using T.WebApi.Services.ProductServices;

namespace T.WebApi.Services.SliderItemServices
{
    public interface ISliderItemService
    {
        Task<List<SliderItem>> GetAllSliderItemAsync();
        Task<ServiceResponse<bool>> CreateAsync(SaveSliderItemRequest saveSliderItemRequest);
        Task<ServiceResponse<bool>> DeleteAllSliderItemAsync();
    }
    public class SliderItemService : ISliderItemService
    {
        private readonly DatabaseContext _context;
        private readonly IProductService _productService;
        public SliderItemService(DatabaseContext context, IProductService productService)
        {
            _context = context;
            _productService = productService;
        }

        public async Task<ServiceResponse<bool>> CreateAsync(SaveSliderItemRequest request)
        {
            if (!(await DeleteAllSliderItemAsync()).Success)
            {
                return new ServiceErrorResponse<bool>("Delete old slider was failed");
            }

            var sliderItem = new SliderItem();
            var product = new List<Product>();

            switch (request.SliderItemSelectOption)
            {
                case SliderItemSelectOption.Newest:
                    product = await _productService.GetAllNewestProduct();
                    break;
                default:
                    product = await _productService.GetRandomProduct();
                    break;
            }

            if(product.Count <= 0)
            {
                product = await _productService.GetRandomProduct();
            }

            _context.DbccCheckIdent<SliderItem>(0);

            try
            {
                for (int i = 0; i < request.SliderItemQuantity && i < product.Count(); i++)
                {
                    sliderItem = new SliderItem()
                    {
                        Name = product[i].Name,
                        Price = product[i].Price.ToString(),
                        ImgPath = await _productService.GetFirstImagePathByProductId(product[i].Id),
                        ProductId = product[i].Id
                    };
                    await _context.SliderItem.AddAsync(sliderItem);
                    await _context.SaveChangesAsync();
                }
                return new ServiceSuccessResponse<bool>();
            }
            catch
            {
                return new ServiceErrorResponse<bool>("Create slider item failed");
            }

        }

        public async Task<ServiceResponse<bool>> DeleteAllSliderItemAsync()
        {
            var records = _context.SliderItem.ToList();

            if (records.Count <= 0)
            {
                return new ServiceSuccessResponse<bool>();
            }

            _context.SliderItem.RemoveRange(records);

            int affectedRows = await _context.SaveChangesAsync();

            if (affectedRows > 0)
            {
                return new ServiceSuccessResponse<bool>();
            }
            else
            {
                return new ServiceErrorResponse<bool>();
            }

        }

        public async Task<List<SliderItem>> GetAllSliderItemAsync()
        {
            using (_context)
            {
                return await _context.SliderItem.ToListAsync();
            }
        }
    }
}
