using System.IdentityModel.Tokens.Jwt;

namespace Core.Helpers
{
    public static class JwtReaderHelperClass
    {
        public static string? GetClaimValue(string jwt, string claimType)
        {
            if (string.IsNullOrWhiteSpace(jwt)) throw new ArgumentNullException(nameof(jwt));
            var handler = new JwtSecurityTokenHandler();
            if (!handler.CanReadToken(jwt)) return null;
            var token = handler.ReadJwtToken(jwt);
            return token.Claims.FirstOrDefault(c => c.Type == claimType)?.Value;
        }
    }
}
