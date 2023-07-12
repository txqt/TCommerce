using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Linq.Dynamic.Core;
using System;

namespace T.WebApi.Extensions
{
    public static class IQueryableExtensions
    {
        public static IQueryable<T> SearchByString<T>(this IQueryable<T> items, string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return items;

            var lowerCaseSearchTerm = searchTerm.Trim().ToLower();
            var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            var searchQuery = new StringBuilder();

            foreach (var property in properties)
            {
                var propertyType = property.PropertyType;
                if (propertyType == typeof(string))
                {
                    searchQuery.Append($"{property.Name}.ToLower().Contains(\"{lowerCaseSearchTerm}\") || ");
                }
            }

            var searchQueryString = searchQuery.ToString().TrimEnd(' ', '|', '|', ' ');
            if (string.IsNullOrWhiteSpace(searchQueryString))
                return items;

            return items.Where(searchQueryString);
        }



        public static IQueryable<T> FindByIntId<T>(this IQueryable<T> entities, int id)
        {
            if (id < 0)
                return entities;

            var parameter = Expression.Parameter(typeof(T), "entity");
            var property = Expression.Property(parameter, "Id");
            var equals = Expression.Equal(property, Expression.Constant(id));
            var lambda = Expression.Lambda<Func<T, bool>>(equals, parameter);

            return entities.Where(lambda);
        }


        //public static IQueryable<T> Sort<T>(this IQueryable<T> entities, string orderByQueryString)
        //{
        //    if (string.IsNullOrWhiteSpace(orderByQueryString))
        //        return entities.OrderBy(e => e);

        //    var orderParams = orderByQueryString.Trim().Split(',');
        //    var propertyInfos = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
        //    var orderQueryBuilder = new StringBuilder();

        //    foreach (var param in orderParams)
        //    {
        //        if (string.IsNullOrWhiteSpace(param))
        //            continue;

        //        var propertyFromQueryName = param.Split(" ")[0];
        //        var objectProperty = propertyInfos.FirstOrDefault(pi =>
        //            pi.Name.Equals(propertyFromQueryName, StringComparison.InvariantCultureIgnoreCase));

        //        if (objectProperty == null)
        //            continue;

        //        var direction = param.EndsWith(" desc") ? "descending" : "ascending";
        //        orderQueryBuilder.Append($"{objectProperty.Name} {direction}, ");
        //    }

        //    var orderQuery = orderQueryBuilder.ToString().TrimEnd(',', ' ');
        //    if (string.IsNullOrWhiteSpace(orderQuery))
        //        return entities.OrderBy(e => e);

        //    return entities.OrderBy(orderQuery);
        //}
        public static IQueryable<T> Sort<T>(this IQueryable<T> entities, string orderByQueryString)
        {
            if (string.IsNullOrWhiteSpace(orderByQueryString))
                return entities.OrderBy(e => 1); // Sắp xếp mặc định nếu không có tham số sắp xếp

            var orderParams = orderByQueryString.Trim().Split(',');
            var query = entities.Expression;

            foreach (var param in orderParams)
            {
                if (string.IsNullOrWhiteSpace(param))
                    continue;

                var propertyArray = param.Trim().Split(' ');
                var propertyName = propertyArray[0];
                var descending = propertyArray.Length > 1 && propertyArray[1].ToLower() == "desc";

                var parameter = Expression.Parameter(typeof(T), "entity");
                var property = Expression.Property(parameter, propertyName);
                var selector = Expression.Lambda(property, parameter);

                query = Expression.Call(typeof(Queryable), descending ? "OrderByDescending" : "OrderBy", new[] { typeof(T), property.Type },
                    query, Expression.Quote(selector));
            }

            return entities.Provider.CreateQuery<T>(query);
        }

    }

}
