using Microsoft.Extensions.Options;
using NetCoreTemplate.Application.Interfaces.Auth;
using NetCoreTemplate.Common.Auth;
using NetCoreTemplate.Common.Models;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace NetCoreTemplate.Application.Auth {
  public sealed class JwtFactory : IJwtFactory {
    private readonly IJwtTokenHandler _jwtTokenHandler;
    private readonly JwtIssuerOptions _jwtOptions;

    public JwtFactory(IJwtTokenHandler jwtTokenHandler, IOptions<JwtIssuerOptions> jwtOptions) {
      _jwtTokenHandler = jwtTokenHandler;
      _jwtOptions = jwtOptions.Value;
      ThrowIfInvalidOptions(_jwtOptions);
    }

    public async Task<AccessToken> GenerateEncodedToken(string id, string email) {
      var identity = GenerateClaimsIdentity(id, email);

      var claims = new[] {
        new Claim(JwtRegisteredClaimNames.Sub, email),
        new Claim(JwtRegisteredClaimNames.Jti, await _jwtOptions.JtiGenerator()),
        new Claim(JwtRegisteredClaimNames.Iat, ToUnixEpochDate(_jwtOptions.IssueAt).ToString(), ClaimValueTypes.Integer64),
        identity.FindFirst(Constants.Strings.JwtClaimIdentifiers.Rol),
        identity.FindFirst(Constants.Strings.JwtClaimIdentifiers.Id)
      };

      var jwt = new JwtSecurityToken(
        _jwtOptions.Issuer,
        _jwtOptions.Audience,
        claims,
        _jwtOptions.NotBefore,
        _jwtOptions.Expiration,
        _jwtOptions.SigningCredentials);

      return new AccessToken(_jwtTokenHandler.WriteToken(jwt), (int)_jwtOptions.ValidFor.TotalSeconds);
    }

    private static ClaimsIdentity GenerateClaimsIdentity(string id, string email) {
      return new ClaimsIdentity(new GenericIdentity(email, "Token"), new[] {
        new Claim(Constants.Strings.JwtClaimIdentifiers.Id, id),
        new Claim(Constants.Strings.JwtClaimIdentifiers.Rol, Constants.Strings.JwtClaims.ApiAccess)
      });
    }

    private static long ToUnixEpochDate(DateTime date)
      => (long)Math.Round((date.ToUniversalTime() -
        new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero))
        .TotalSeconds);

    private static void ThrowIfInvalidOptions(JwtIssuerOptions options) {
      if (options == null) throw new ArgumentNullException(nameof(options));

      if (options.ValidFor <= TimeSpan.Zero) {
        throw new ArgumentException("Must be a non-zero TimeSpan.", nameof(JwtIssuerOptions.ValidFor));
      }

      if (options.SigningCredentials == null) {
        throw new ArgumentNullException(nameof(JwtIssuerOptions.SigningCredentials));
      }

      if (options.JtiGenerator == null) {
        throw new ArgumentNullException(nameof(JwtIssuerOptions.JtiGenerator));
      }
    }
  }
}
