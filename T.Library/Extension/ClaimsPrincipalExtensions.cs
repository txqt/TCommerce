using System.Security.Claims;


internal static class ClaimsPrincipalExtensions
{
    internal static string GetEmail(this ClaimsPrincipal claimsPrincipal)
    => claimsPrincipal.FindFirstValue(ClaimTypes.Email);

    internal static string GetFirstName(this ClaimsPrincipal claimsPrincipal)
        => claimsPrincipal.FindFirstValue(ClaimTypes.Name);

    internal static string GetFullName(this ClaimsPrincipal claimsPrincipal)
            => claimsPrincipal.FindFirstValue(ClaimTypes.GivenName);

    internal static string GetLastName(this ClaimsPrincipal claimsPrincipal)
            => claimsPrincipal.FindFirstValue(ClaimTypes.Surname);

    internal static string GetPhoneNumber(this ClaimsPrincipal claimsPrincipal)
        => claimsPrincipal.FindFirstValue(ClaimTypes.MobilePhone);

    internal static string GetUserId(this ClaimsPrincipal claimsPrincipal)
       => claimsPrincipal.FindFirstValue(ClaimTypes.NameIdentifier);
    internal static string GetTypeOfClaim(this ClaimsPrincipal claimsPrincipal, string value)
       => claimsPrincipal.FindFirstValue(value).ToString();
}
