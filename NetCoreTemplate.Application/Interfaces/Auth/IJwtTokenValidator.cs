using System.Security.Claims;

namespace NetCoreTemplate.Application.Interfaces.Auth {
  public interface IJwtTokenValidator {
    ClaimsPrincipal GetPrincipalFromToken(string token, string signingKey);
  }
}
