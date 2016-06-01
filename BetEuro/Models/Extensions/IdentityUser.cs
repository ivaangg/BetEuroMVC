using System.Security.Claims;
using System.Security.Principal;

namespace BetEuro.Models.Extensions
{
    public static class IdentityExtensions
    {
        public static bool GetPaidVal(this IIdentity identity)
        {
            var claim = ((ClaimsIdentity)identity).FindFirst("Paid");
            // Test for null to avoid issues during local testing
            return (claim == null) ? false : (claim.Value == "1" ? true : false);
        }

        public static bool GetPiwoVal(this IIdentity identity)
        {
            var claim = ((ClaimsIdentity)identity).FindFirst("Piwo");
            // Test for null to avoid issues during local testing
            return (claim == null) ? false : (claim.Value == "1" ? true : false);
        }
    }
}