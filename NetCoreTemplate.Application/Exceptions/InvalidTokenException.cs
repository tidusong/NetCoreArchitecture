using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace NetCoreTemplate.Application.Exceptions {
  public class InvalidTokenException : ApiException {
    public InvalidTokenException()
        : base(HttpStatusCode.Unauthorized, "Invalid token") { }

    public override string GetContent() {
      return null;
    }
  }
}
