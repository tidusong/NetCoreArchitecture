using System;
using System.Collections.Generic;
using System.Text;

namespace NetCoreTemplate.Common.Models {
  public sealed class LoginError {
    public string Code { get; }
    public string Description { get; }

    public LoginError(string code, string description) {
      Code = code;
      Description = description;
    }
  }
}
