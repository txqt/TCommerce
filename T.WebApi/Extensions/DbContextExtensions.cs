using Microsoft.EntityFrameworkCore;

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
                var originalValue = entry.OriginalValues[property.Name];
                var updatedValue = entry.CurrentValues[property.Name];

                if (!Equals(originalValue, updatedValue))
                {
                    return false;
                }
            }

            return true;
        }
    }

}
