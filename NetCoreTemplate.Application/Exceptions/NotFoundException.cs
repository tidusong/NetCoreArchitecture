using System;
using System.Net;

namespace NetCoreTemplate.Application.Exceptions {
  public class NotFoundException : ApiException {
    public NotFoundException(string name, object key)
        : base(HttpStatusCode.NotFound, $"Entity \"{name}\" ({key}) was not found.") { }

        public override string GetContent() {
            return null;
        }
    }
}