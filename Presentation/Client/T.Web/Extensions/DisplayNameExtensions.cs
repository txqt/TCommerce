using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Globalization;
using System.Linq.Expressions;
using System.Reflection;

public static class DisplayNameExtensions
{
    public static T GetAttribute<T>(this MemberInfo member, bool isRequired)
    where T : Attribute
    {
        var attribute = member.GetCustomAttributes(typeof(T), false).SingleOrDefault();

        if (attribute == null && isRequired)
        {
            throw new ArgumentException(
                string.Format(
                    CultureInfo.InvariantCulture,
                    "The {0} attribute must be defined on member {1}",
                    typeof(T).Name,
                    member.Name));
        }

        return (T)attribute;
    }

    public static string GetPropertyDisplayName<T>(Expression<Func<T, object>> propertyExpression)
    {
        var memberInfo = GetPropertyInformation(propertyExpression.Body);
        if (memberInfo == null)
        {
            throw new ArgumentException(
                "No property reference expression was found.",
                "propertyExpression");
        }

        var displayNameAttribute = memberInfo.GetAttribute<DisplayNameAttribute>(false);
        var displayAttribute = memberInfo.GetAttribute<DisplayAttribute>(false);

        if (displayNameAttribute != null)
        {
            return displayNameAttribute.DisplayName;
        }
        if (displayAttribute != null)
        {
            return displayAttribute.Name;
        }

        return memberInfo.Name;
    }

    public static MemberInfo GetPropertyInformation(Expression propertyExpression)
    {
        Debug.Assert(propertyExpression != null, "propertyExpression != null");
        MemberExpression memberExpr = propertyExpression as MemberExpression;
        if (memberExpr == null)
        {
            UnaryExpression unaryExpr = propertyExpression as UnaryExpression;
            if (unaryExpr != null && unaryExpr.NodeType == ExpressionType.Convert)
            {
                memberExpr = unaryExpr.Operand as MemberExpression;
            }
        }

        if (memberExpr != null && memberExpr.Member.MemberType == MemberTypes.Property)
        {
            return memberExpr.Member;
        }

        return null;
    }
}
