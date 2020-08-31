using NetCoreTemplate.Common.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace NetCoreTemplate.Application.Interfaces.Auth {
  public interface IJwtFactory {
    Task<AccessToken> GenerateEncodedToken(string id, string email);
  }
}
