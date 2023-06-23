//using System.Reflection;
//using System.Text;
//using T.Library.Model;
//using System.Linq.Dynamic.Core;

//namespace T.WebApi.Extensions
//{
//    public static class RepositoryExtensions
//    {
//        public static IQueryable<Product> Search(this IQueryable<Product> products, string searchTerm)
//        {
//            if (string.IsNullOrWhiteSpace(searchTerm))
//                return products;
//            var lowerCaseSearchTerm = searchTerm.Trim().ToLower();
//            return products.Where(p => p.Name.ToLower().Contains(lowerCaseSearchTerm) ||
//                                    p.ShortDescription.ToLower().Contains(lowerCaseSearchTerm) || 
//                                    p.FullDescription.ToLower().Contains(lowerCaseSearchTerm));
//        }
//        public static IQueryable<Product> FindProductByIdAsync(this IQueryable<Product> products, int productId)
//        {
//            if (productId < 0)
//                return products;

//            return products.Where(x => x.Deleted == false && x.Id == productId);
//        }

//        public static IQueryable<Product> Sort(this IQueryable<Product> products, string orderByQueryString)
//        {
//            if (string.IsNullOrWhiteSpace(orderByQueryString))
//                return products.OrderBy(e => e.Name);

//            var orderParams = orderByQueryString.Trim().Split(',');
//            var propertyInfos = typeof(Product).GetProperties(BindingFlags.Public | BindingFlags.Instance);
//            var orderQueryBuilder = new StringBuilder();

//            foreach (var param in orderParams)
//            {
//                if (string.IsNullOrWhiteSpace(param))
//                    continue;

//                var propertyFromQueryName = param.Split(" ")[0];
//                var objectProperty = propertyInfos.FirstOrDefault(pi => pi.Name.Equals(propertyFromQueryName, StringComparison.InvariantCultureIgnoreCase));

//                if (objectProperty == null)
//                    continue;

//                var direction = param.EndsWith(" desc") ? "descending" : "ascending";
//                orderQueryBuilder.Append($"{objectProperty.Name} {direction}, ");
//            }

//            var orderQuery = orderQueryBuilder.ToString().TrimEnd(',', ' ');
//            if (string.IsNullOrWhiteSpace(orderQuery))
//                return products.OrderBy(e => e.Name);

//            return products.OrderBy(orderQuery);
//        }
//    }
//}
