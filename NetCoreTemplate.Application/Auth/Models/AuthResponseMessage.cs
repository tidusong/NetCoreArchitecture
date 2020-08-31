using System;
using System.Collections.Generic;
using System.Text;

namespace NetCoreTemplate.Application.Auth.Models {
  public abstract class AuthResponseMessage {
    public bool Success { get; }
    public string Message { get; }

    protected AuthResponseMessage(bool success = false, string message = null) {
      Success = success;
      Message = message;
    }
  }
}
