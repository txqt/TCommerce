using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using T.Library.Model.Common;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace T.WebApi.Helpers
{
    public static partial class CacheKeysDefault<TEntity> where TEntity : BaseEntity
    {
        /// <summary>
        /// Gets an entity type name used in cache keys
        /// </summary>
        public static string EntityTypeName => typeof(TEntity).Name.ToLowerInvariant();

        /// <summary>
        /// Gets a key pattern to clear cache
        /// </summary>
        public static string Prefix => $"T.{EntityTypeName}.";

        /// <summary>
        /// Gets a key pattern to clear cache
        /// </summary>
        public static string ByIdPrefix => $"T.{EntityTypeName}.byid.";

        /// <summary>
        /// Gets a key pattern to clear cache
        /// </summary>
        public static string ByIdsPrefix => $"T.{EntityTypeName}.byids.";

        /// <summary>
        /// Gets a key pattern to clear cache
        /// </summary>
        public static string AllPrefix => $"T.{EntityTypeName}.all.";

        //public static string GetKeyFromIQueryable(IQueryable query)
        //{
        //    var keyBuilder = new StringBuilder(query.ToString());
        //    var queryParamVisitor = new QueryParameterVisitor(keyBuilder);
        //    queryParamVisitor.GetQueryParameters(query.Expression);

        //    // Loại bỏ các thông tin về datetime
        //    var regex = new Regex(@"\d{1,2}/\d{1,2}/\d{4} \d{1,2}:\d{2}:\d{2} (AM|PM)");
        //    var cleanKey = regex.Replace(keyBuilder.ToString(), "");

        //    var assemblyQualifiedName = typeof(TEntity).AssemblyQualifiedName;

        //    // Tạo key mới chỉ với các thông tin quan trọng
        //    var newKeyBuilder = new StringBuilder();
        //    newKeyBuilder.Append(cleanKey);
        //    newKeyBuilder.Append("\n\r");
        //    newKeyBuilder.Append(assemblyQualifiedName);

        //    return newKeyBuilder.ToString();
        //}
    }
}
