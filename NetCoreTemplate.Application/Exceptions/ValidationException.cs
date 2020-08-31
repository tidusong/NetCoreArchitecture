using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using FluentValidation.Results;
using Newtonsoft.Json;

namespace NetCoreTemplate.Application.Exceptions {
  public class ValidationException : ApiException<List<ValidationFailure>> {
    public ValidationException(List<ValidationFailure> validationFailures)
        : base(HttpStatusCode.BadRequest, "One or more validation failures have occurred.", validationFailures ?? new List<ValidationFailure>()) {
      Failures = new Dictionary<string, string[]>();
      var propertyNames = validationFailures
          .Select(e => e.PropertyName)
          .Distinct();

      foreach (var propertyName in propertyNames) {
        var propertyFailures = validationFailures
            .Where(e => e.PropertyName == propertyName)
            .Select(e => e.ErrorMessage)
            .ToArray();

        Failures.Add(propertyName, propertyFailures);
      }
    }

    public override string GetContent() {
      return JsonConvert.SerializeObject(Content);
    }

    public IDictionary<string, string[]> Failures { get; }
  }
}