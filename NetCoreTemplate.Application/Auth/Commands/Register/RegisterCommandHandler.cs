using MediatR;
using Microsoft.AspNetCore.Identity;
using NetCoreTemplate.Application.Exceptions;
using NetCoreTemplate.Domain.Entities;
using NetCoreTemplate.Persistence.Data;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace NetCoreTemplate.Application.Auth.Commands.Register {
  public class RegisterCommandHandler : IRequestHandler<RegisterCommand, int> {
    private readonly UserManager<AppUser> _userManager;
    private NetCoreTemplateDbContext _context;

    public RegisterCommandHandler(UserManager<AppUser> userManager, NetCoreTemplateDbContext context) {
      _userManager = userManager;
      _context = context;
    }
    public async Task<int> Handle(RegisterCommand request, CancellationToken cancellationToken) {
      var appUser = new AppUser {
        UserName = request.Email,
        Email = request.Email
      };
      var identityResult = await _userManager.CreateAsync(appUser, request.Password);

      if (!identityResult.Succeeded) {
        throw new RegisterFailedException(identityResult.Errors);
      }

      var user = new User(request.FirstName, request.LastName, appUser.Id, appUser.Email);
      _context.User.Add(user);
      await _context.SaveChangesAsync();

      return user.Id;
    }
  }
}
