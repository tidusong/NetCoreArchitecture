using FluentValidation;
using MediatR;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace NetCoreTemplate.Application.Infrastructure {
  public class RequestValidationBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest: IRequest<TResponse> {
    private readonly IEnumerable<IValidator<TRequest>> _validatores;

    public RequestValidationBehaviour(IEnumerable<IValidator<TRequest>> validators) {
      _validatores = validators;
    }

    public Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next) {
      var context = new ValidationContext(request);

      var failures = _validatores
        .Select(v => v.Validate(context))
        .SelectMany(result => result.Errors)
        .Where(f => f != null)
        .ToList();

      if (failures.Count != 0) {
        throw new Exceptions.ValidationException(failures);
      }

      return next();
    }
  }
}
