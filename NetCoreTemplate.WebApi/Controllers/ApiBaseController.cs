using System.Collections.Generic;
using System.Linq;
using System.Net;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using NetCoreTemplate.Application.Exceptions;
using Microsoft.Extensions.DependencyInjection;
using NetCoreTemplate.WebApi.Filters.ExceptionResult;

namespace NetCoreTemplate.WebApi.Controllers {
  [Route("api/[controller]")]
  public abstract class ApiBaseController : Controller {
    private IMediator _mediator;

    protected IMediator Mediator => _mediator ?? (_mediator = HttpContext.RequestServices.GetService<IMediator>());

    public override void OnActionExecuting(ActionExecutingContext context) {
      if (!context.ModelState.IsValid) {
        if (!ModelState.IsValid) {
          var errorList = ModelState.ToDictionary(
              kvp => kvp.Key,
              kvp => kvp.Value.Errors.Select(e =>
                  string.IsNullOrEmpty(e.ErrorMessage)
                      ? e.Exception?.GetBaseException().Message
                      : e.ErrorMessage).ToArray()
          );

          throw new ApiException<IDictionary<string, string[]>>("Invalid request", errorList);
        }
      }

      base.OnActionExecuting(context);
    }
  }
}