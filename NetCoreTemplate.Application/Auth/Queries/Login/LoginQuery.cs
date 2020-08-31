using MediatR;

namespace NetCoreTemplate.Application.Auth.Queries.Login {
  public class LoginQuery : IRequest<LoginResponseViewModel> {
    public string Email { get; set; }
    public string Password { get; set; }
    public string RemoteIpAddress { get; set; }

    public LoginQuery(string email, string password, string remoteIpAddress) {
      Email = email;
      Password = password;
      RemoteIpAddress = remoteIpAddress;
    }
  }
}
