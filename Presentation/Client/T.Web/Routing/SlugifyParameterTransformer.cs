
using System.Text.RegularExpressions;

namespace T.Web.Routing
{
    public class SlugifyParameterTransformer : IOutboundParameterTransformer
    {
#pragma warning disable CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.
        public string? TransformOutbound(object? value)
#pragma warning restore CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.
        {
            if (value == null) { return null; }

            return Regex.Replace(value.ToString()!,
                                 "([a-z])([A-Z])",
                                 "$1-$2",
                                 RegexOptions.CultureInvariant,
                                 TimeSpan.FromMilliseconds(100)).ToLowerInvariant();
        }
    }
}
