using System;
using Autofac.Core;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using NetCoreTemplate.Application.Exceptions;
using NetCoreTemplate.WebApi.Filters.ExceptionResult;

namespace NetCoreTemplate.WebApi.Filters.ExceptionResult {
  public class ExceptionResultBuilder : IExceptionResultBuilder {
    private readonly IHostingEnvironment _hostingEnvironment;

    public ExceptionResultBuilder(IHostingEnvironment hostingEnvironment) {
      _hostingEnvironment = hostingEnvironment;
    }

    public IActionResult Build(Exception exception) {
            var stackTrace = "No stack trace available";

            if (!string.Equals(_hostingEnvironment.EnvironmentName, "Production", StringComparison.OrdinalIgnoreCase))
                stackTrace = exception.GetBaseException().StackTrace;
            var statusCode = 500;
            string content = null;
            var message = exception.GetBaseException().Message;

            var dependencyResolutionException = exception as DependencyResolutionException;
            if (dependencyResolutionException != null)
                message = $"Dependency Exception: Please ensure that classes implement the interface: {message}";

            var apiException = exception as ApiException;

            if (apiException != null) {
                statusCode = (int)apiException.StatusCode;
                content = apiException.GetContent();
                if (!string.IsNullOrEmpty(apiException.Message))
                    message = apiException.GetBaseException().Message;
            }

            return CreateActionResult(content, message, stackTrace, statusCode, exception);
        }

        protected virtual IActionResult CreateActionResult(string content, string message, string stackTrace,
            int statusCode, Exception exception) {
            var apiError = new ApiError {
                Error = content ?? message
            };

            if (!string.IsNullOrEmpty(stackTrace))
                apiError.StackTrace = stackTrace;

            var objectResult = new ObjectResult(apiError) {
                StatusCode = statusCode
            };
            var eventId = new Microsoft.Extensions.Logging.EventId(statusCode);

            Log.Error(exception, ($"{eventId.Name}, {message}"));

            return objectResult;
        }
    }
}
