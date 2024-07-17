using System.Linq.Expressions;
using System;

public static class GetFieldNameHelper
{
    public static string GetPropertyName<T>(Expression<Func<T, object>> propertyExpression)
    {
        var memberExpression = GetMemberExpression(propertyExpression);
        return memberExpression.Member.Name;
    }

    public static MemberExpression GetMemberExpression<T>(Expression<Func<T, object>> propertyExpression)
    {
        var memberExpression = propertyExpression.Body as MemberExpression;

        if (memberExpression == null)
        {
            // Nếu expression không phải là MemberExpression (ví dụ: Convert(expression))
            // Thì thử lấy MemberExpression từ UnaryExpression
            var unaryExpression = propertyExpression.Body as UnaryExpression;
            if (unaryExpression?.Operand is MemberExpression operand)
            {
                memberExpression = operand;
            }
        }

        if (memberExpression != null)
        {
            return memberExpression;
        }

        throw new ArgumentException("Invalid expression", nameof(propertyExpression));
    }
}


