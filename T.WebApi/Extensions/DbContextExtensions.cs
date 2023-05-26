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
                var existingPropertyValue = property.GetGetter().GetClrValue(existingRecord);
                var updatedPropertyValue = property.GetGetter().GetClrValue(updatedRecord);

                if (!Equals(existingPropertyValue, updatedPropertyValue))
                {
                    return false;
                }
            }

            return true;
        }

    }

}
