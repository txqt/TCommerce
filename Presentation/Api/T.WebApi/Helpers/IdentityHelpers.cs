using Microsoft.EntityFrameworkCore;
using T.WebApi.Database;

namespace T.WebApi.Helpers
{
    public static class IdentityHelpers
    {
        public static Task EnableIdentityInsert<T>(this DatabaseContext context) => SetIdentityInsert<T>(context, enable: true);
        public static Task DisableIdentityInsert<T>(this DatabaseContext context) => SetIdentityInsert<T>(context, enable: false);

        private static Task SetIdentityInsert<T>(DatabaseContext context, bool enable)
        {
            var entityType = context.Model.FindEntityType(typeof(T));
            var value = enable ? "ON" : "OFF";

            string tableName = entityType?.GetTableName()!;
            string schemaName = entityType?.GetSchema()!;

            if (tableName != null && schemaName != null)
            {
                string sql = $"SET IDENTITY_INSERT [{schemaName}].[{tableName}] {value}";
                return context.Database.ExecuteSqlRawAsync(sql);
            }
            else
            {
                // Handle the case where entityType or its properties are null
                // or entityType doesn't map to a table
                throw new InvalidOperationException("Entity type is not valid.");
            }
        }


        public static void SaveChangesWithIdentityInsert<T>(this DatabaseContext context)
        {
            using var transaction = context.Database.BeginTransaction();
            context.EnableIdentityInsert<T>();
            context.SaveChanges();
            context.DisableIdentityInsert<T>();
            transaction.Commit();
        }

    }
}
