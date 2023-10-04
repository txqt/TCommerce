using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace T.WebApi.Extensions
{
    public static class DbContextExtensions
    {
        public static bool IsRecordUnchanged<TEntity>(this DbContext context, TEntity existingRecord, TEntity updatedRecord)
    where TEntity : class
        {
            var entry = context.Entry(existingRecord);
            var properties = entry.CurrentValues.Properties;

            foreach (var property in properties)
            {
                var existingPropertyValue = property.GetGetter().GetClrValue(existingRecord);
                var updatedPropertyValue = property.GetGetter().GetClrValue(updatedRecord);

                if (!Equals(existingPropertyValue, updatedPropertyValue))
                {
                    return false;
                }
            }

            return true;
        }
        public static void DbccCheckIdent<T>(this DbContext context, int? reseedTo = null) where T : class
        {
            var tableName = context.GetTableName<T>();
            var resetIdSql = $"DBCC CHECKIDENT('{tableName}', RESEED{(reseedTo != null ? "," + reseedTo : "")});";
            context.Database.ExecuteSqlRaw(resetIdSql);
        }

        public static string GetTableName<T>(this DbContext context) where T : class
        {
            var entityType = context.Model.FindEntityType(typeof(T));
            return entityType.GetTableName();
        }
    }

}
