//using Microsoft.EntityFrameworkCore;
//using T.Library.Model;
//using T.Library.Model.BannerItem;
//using T.Library.Model.Common;
//using T.Library.Model.Response;
//using T.Library.Model.Slider;
//using T.WebApi.Database.ConfigurationDatabase;
//using T.WebApi.Extensions;
//using T.WebApi.Services.ProductServices;

//namespace T.WebApi.Services.SliderItemServices
//{
//    public interface ISliderItemService
//    {
//        Task<List<SlideShow>> GetAllSliderItemAsync();
//        Task<ServiceResponse<bool>> CreateAsync(SaveSliderItemRequest saveSliderItemRequest);
//        Task<ServiceResponse<bool>> DeleteAllSliderItemAsync();
//    }
//    public class SliderItemService : ISliderItemService
//    {
//        private readonly DatabaseContext _context;
//        private readonly IProductService _productService;
//        public SliderItemService(DatabaseContext context, IProductService productService)
//        {
//            _context = context;
//            _productService = productService;
//        }

//        public async Task<ServiceResponse<bool>> CreateAsync(SaveSliderItemRequest request)
//        {
//            if (!(await DeleteAllSliderItemAsync()).Success)
//            {
//                return new ServiceErrorResponse<bool>("Delete old slider was failed");
//            }
//            var message = "";
//            bool bHasNewestProduct = false;
//            var product = new List<Product>();

//            switch (request.SliderItemSelectOption)
//            {
//                case SliderItemSelectOption.Newest:
//                    product = await _productService.GetAllNewestProduct();
//                    bHasNewestProduct = product.Count() > 0;
//                    break;
//                default:
//                    product = await _productService.GetRandomProduct();
//                    break;
//            }

//            if(!bHasNewestProduct)
//            {
//                product = await _productService.GetRandomProduct();
//                message = "Bởi vì không có sản phẩm mới nhất nên sẽ thay bằng ngẫu nhiên.";
//            }

//            _context.DbccCheckIdent<SlideShow>(0);

//            if(await AddToDatabase(request.SliderItemQuantity, product))
//            {
//                message = "Slider đã thêm thành công !";
//                return new ServiceSuccessResponse<bool>() { Message = message};
//            }

//            message = "Something went wrong !";
//            return new ServiceErrorResponse<bool>(message);

//        }

//        public async Task<ServiceResponse<bool>> DeleteAllSliderItemAsync()
//        {
//            var records = _context.SliderItem.ToList();

//            if (records.Count <= 0)
//            {
//                return new ServiceSuccessResponse<bool>();
//            }

//            _context.SliderItem.RemoveRange(records);

//            int affectedRows = await _context.SaveChangesAsync();

//            if (affectedRows > 0)
//            {
//                return new ServiceSuccessResponse<bool>();
//            }
//            else
//            {
//                return new ServiceErrorResponse<bool>();
//            }

//        }

//        public async Task<List<SlideShow>> GetAllSliderItemAsync()
//        {
//            
//            {
//                return await _context.SliderItem.ToListAsync();
//            }
//        }

//        private async Task<bool> AddToDatabase(int quantity, List<Product> product)
//        {
//            _context.DbccCheckIdent<SlideShow>(0);

//            try
//            {
//                for (int i = 0; i < quantity && i < product.Count(); i++)
//                {
//                    var sliderItem = new SlideShow()
//                    {
//                        Title = product[i].Name,
//                        Price = product[i].Price,
//                        ImgPath = await _productService.GetFirstImagePathByProductId(product[i].Id),
//                        ProductId = product[i].Id
//                    };
//                    await _context.SliderItem.AddAsync(sliderItem);
//                    await _context.SaveChangesAsync();
//                }
//                return true;
//            }
//            catch
//            {
//                return false;
//            }
//        }
//    }
//}
