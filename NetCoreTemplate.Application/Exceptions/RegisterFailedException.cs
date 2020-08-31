using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace NetCoreTemplate.Application.Exceptions {
  public class RegisterFailedException : ApiException<IEnumerable<IdentityError>> {
    public RegisterFailedException(IEnumerable<IdentityError> identityErrors)
        : base(HttpStatusCode.BadRequest, "Register new user failed.", identityErrors ?? new List<IdentityError>()) {
      Failures = new Dictionary<string, string>();
      foreach (var error in identityErrors) {
        Failures.Add(error.Code, error.Description);
      }
    }

    public override string GetContent() {
      return JsonConvert.SerializeObject(Content);
    }

    public IDictionary<string, string> Failures { get; }
  }
}
