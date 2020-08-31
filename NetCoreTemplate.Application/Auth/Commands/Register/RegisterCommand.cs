using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace NetCoreTemplate.Application.Auth.Commands.Register {
  public class RegisterCommand : IRequest<int> {
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
  }
}
