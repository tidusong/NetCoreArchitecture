using System;
using Microsoft.AspNetCore.Mvc;

namespace NetCoreTemplate.WebApi.Filters.ExceptionResult {
  public interface IExceptionResultBuilder {
    IActionResult Build(Exception exception);
  }
}