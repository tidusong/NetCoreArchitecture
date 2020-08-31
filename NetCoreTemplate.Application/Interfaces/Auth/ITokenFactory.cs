using System;
using System.Collections.Generic;
using System.Text;

namespace NetCoreTemplate.Application.Interfaces.Auth {
  public interface ITokenFactory {
    string GenerateToken(int size = 32);
  }
}
