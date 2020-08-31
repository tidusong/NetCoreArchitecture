using NetCoreTemplate.Application.Auth.Models;
using NetCoreTemplate.Common.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace NetCoreTemplate.Application.Auth.Queries.Login {
  public class LoginResponseViewModel : AuthResponseMessage {
    public AccessToken AccessToken { get; }
    public string RefreshToken { get; }
    public IEnumerable<LoginError> Errors { get; }

    public LoginResponseViewModel(IEnumerable<LoginError> errors, bool success = false, string message = null) : base(success, message) {
      Errors = errors;
    }

    public LoginResponseViewModel(AccessToken accessToken, string refreshToken, bool success = false, string message = null) : base(success, message) {
      AccessToken = accessToken;
      RefreshToken = refreshToken;
    }
  }
}
