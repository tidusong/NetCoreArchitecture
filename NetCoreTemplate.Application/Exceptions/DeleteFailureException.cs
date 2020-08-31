using System;
using System.Net;

namespace NetCoreTemplate.Application.Exceptions {
  public class DeleteFailureException : ApiException {
    public DeleteFailureException(string name, object key, string message)
        : base($"Deletion of entity \"{name}\" ({key}) failed. {message}") {
    }

        public override string GetContent() {
            return null;
        }
    }
}