using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace NetCoreTemplate.Application.Auth.Commands.Register {
  public class RegisterCommandValidator : AbstractValidator<RegisterCommand> {
    public RegisterCommandValidator() {
      RuleFor(x => x.Password).NotEmpty();
      RuleFor(x => x.Email).NotEmpty();
    }
  }
}
