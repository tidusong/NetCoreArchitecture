using Microsoft.AspNetCore.Identity;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using NetCoreTemplate.Persistence.Data;
using NetCoreTemplate.Application.Interfaces.Auth;
using Microsoft.EntityFrameworkCore;
using NetCoreTemplate.Application.Exceptions;
using MediatR;
using System.Linq;
using NetCoreTemplate.Domain.Entities;

namespace NetCoreTemplate.Application.Auth.Queries.Login {
  public class LoginQueryHandler : IRequestHandler<LoginQuery, LoginResponseViewModel> {
    private readonly UserManager<AppUser> _userManager;
    private readonly IJwtFactory _jwtFactory;
    private readonly ITokenFactory _tokenFactory;
    private readonly IMapper _mapper;
    private readonly NetCoreTemplateDbContext _context;

    public LoginQueryHandler(UserManager<AppUser> userManager, IJwtFactory jwtFactory,
      ITokenFactory tokenFactory, IMapper mapper, NetCoreTemplateDbContext context) {
      _userManager = userManager;
      _jwtFactory = jwtFactory;
      _tokenFactory = tokenFactory;
      _mapper = mapper;
      _context = context;
    }

    public async Task<LoginResponseViewModel> Handle(LoginQuery request, CancellationToken cancellationToken) {
      var appUser = await _userManager.FindByEmailAsync(request.Email);
      var user = appUser == null ? null : _mapper.Map(appUser, await _context.User.Where(u => u.IdentityId.Equals(appUser.Id)).FirstOrDefaultAsync());
      if (user != null) {
        if (await _userManager.CheckPasswordAsync(appUser, request.Password)) {
          var jwtToken = await _jwtFactory.GenerateEncodedToken(user.IdentityId, user.Email);
          var refreshToken = _tokenFactory.GenerateToken();
          user.AddRefreshToken(refreshToken, user.Id, request.RemoteIpAddress);
          _context.Entry(user).State = EntityState.Modified;
          await _context.SaveChangesAsync();

          return new LoginResponseViewModel(jwtToken, refreshToken, true);
        }
      }

      throw new InvalidTokenException();
    }
  }
}
