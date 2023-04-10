using Microsoft.EntityFrameworkCore;
using T.Library.Model;
using T.WebApi.Database.ConfigurationDatabase;
using T.WebApi.Extensions;

namespace T.WebApi.Services.AccountServices
{
    public interface IProductService
    {
        Task<PagedList<Product>> GetAll(ProductParameters productParameters);
    }
    public class ProductService : IProductService
    {
        private readonly DatabaseContext _context;

        public ProductService(DatabaseContext context)
        {
            this._context = context;
        }

        public async Task<PagedList<Product>> GetAll(ProductParameters productParameters)
        {
            using (_context)
            {
                var list_product = new List<Product>();

                list_product = await _context.Product
                    .Search(productParameters.searchText)
                    .Sort(productParameters.OrderBy)//sort by product coloumn 
                    .Include(x => x.ProductPictures)
                    .Where(x => x.Deleted == false)
                    .ToListAsync();



                list_product = list_product.DistinctBy(x => x.Id).ToList();
                //list_product.Shuffle();
                return PagedList<Product>
                            .ToPagedList(list_product, productParameters.PageNumber, productParameters.PageSize);
            }
        }
    }
}
