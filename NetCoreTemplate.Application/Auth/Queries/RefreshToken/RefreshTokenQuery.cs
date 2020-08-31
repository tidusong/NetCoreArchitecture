using MediatR;
using NetCoreTemplate.Application.Auth.Queries.Login;

namespace NetCoreTemplate.Application.Auth.Queries.RefreshToken {
  public class RefreshTokenQuery : IRequest<LoginResponseViewModel> {
    public string AccessToken { get; set; }
    public string RefreshToken { get; set; }
  }
}
