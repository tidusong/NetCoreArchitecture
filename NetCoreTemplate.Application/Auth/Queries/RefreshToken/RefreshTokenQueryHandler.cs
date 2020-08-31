using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using NetCoreTemplate.Application.Auth.Queries.Login;
using NetCoreTemplate.Application.Exceptions;
using NetCoreTemplate.Application.Interfaces.Auth;
using NetCoreTemplate.Common.Auth;
using NetCoreTemplate.Domain.Entities;
using NetCoreTemplate.Persistence.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace NetCoreTemplate.Application.Auth.Queries.RefreshToken {
  public class RefreshTokenQueryHandler : IRequestHandler<RefreshTokenQuery, LoginResponseViewModel> {
    private readonly UserManager<AppUser> _userManager;
    private readonly IJwtTokenValidator _jwtTokenValidator;
    private readonly AuthSettings _authSettings;
    private readonly NetCoreTemplateDbContext _context;
    private readonly IJwtFactory _jwtFactory;
    private readonly ITokenFactory _tokenFactory;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public RefreshTokenQueryHandler(UserManager<AppUser> userManager, IJwtTokenValidator jwtTokenValidator,
      IOptions<AuthSettings> authSettings, NetCoreTemplateDbContext context, IJwtFactory jwtFactory,
      ITokenFactory tokenFactory, IHttpContextAccessor httpContextAccessor) {
      _userManager = userManager;
      _jwtTokenValidator = jwtTokenValidator;
      _authSettings = authSettings.Value;
      _context = context;
      _jwtFactory = jwtFactory;
      _tokenFactory = tokenFactory;
      _httpContextAccessor = httpContextAccessor;
    }
    public async Task<LoginResponseViewModel> Handle(RefreshTokenQuery request, CancellationToken cancellationToken) {
      var cp = _jwtTokenValidator.GetPrincipalFromToken(request.AccessToken, _authSettings.SecretKey);

      if (cp != null) {
        var id = cp.Claims.First(c => c.Type == "id");
        var user = await _context.User.Include(u => u.RefreshTokens)
          .Where(u => u.IdentityId.Equals(id.Value)).FirstOrDefaultAsync();

        if (user.HasValidRefreshToken(request.RefreshToken)) {
          var jwtToken = await _jwtFactory.GenerateEncodedToken(user.IdentityId, user.Email);
          var refreshToken = _tokenFactory.GenerateToken();
          user.RemoveRefreshToken(request.RefreshToken);
          user.AddRefreshToken(refreshToken, user.Id, _httpContextAccessor.HttpContext.Connection.RemoteIpAddress.ToString());
          _context.Entry(user).State = EntityState.Modified;
          await _context.SaveChangesAsync();

          return new LoginResponseViewModel(jwtToken, refreshToken, true);
        }
      }

      throw new InvalidTokenException();
    }
  }
}
