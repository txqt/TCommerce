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
            return context.Database.ExecuteSqlRawAsync(
                $"SET IDENTITY_INSERT {entityType.GetSchema()}.{entityType.GetTableName()} {value}");
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
