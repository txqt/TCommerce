using Microsoft.EntityFrameworkCore;

namespace TCommerce.Data.Helpers
{
    public static class ApplicationDbContextExtensions
    {
        public static Task EnableIdentityInsert<T>(this ApplicationDbContext context) => SetIdentityInsert<T>(context, enable: true);
        public static Task DisableIdentityInsert<T>(this ApplicationDbContext context) => SetIdentityInsert<T>(context, enable: false);

        private static async Task SetIdentityInsert<T>(ApplicationDbContext context, bool enable)
        {
            var entityType = context.Model.FindEntityType(typeof(T));
            if (entityType == null)
            {
                throw new InvalidOperationException($"Entity type {typeof(T).Name} is not valid.");
            }

            var schemaName = entityType.GetSchema();
            var tableName = entityType.GetTableName();

            if (string.IsNullOrEmpty(tableName))
            {
                throw new InvalidOperationException($"Table name for entity type {typeof(T).Name} is not valid.");
            }

            var value = enable ? "ON" : "OFF";
            var sql = $"SET IDENTITY_INSERT [{schemaName ?? "dbo"}].[{tableName}] {value}";

            await context.Database.ExecuteSqlRawAsync(sql);
        }



        public static void SaveChangesWithIdentityInsert<T>(this ApplicationDbContext context)
        {
            using var transaction = context.Database.BeginTransaction();
            context.EnableIdentityInsert<T>();
            context.SaveChanges();
            context.DisableIdentityInsert<T>();
            transaction.Commit();
        }

    }
}
