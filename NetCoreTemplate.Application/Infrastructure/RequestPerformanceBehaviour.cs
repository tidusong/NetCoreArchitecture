using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Serilog;

namespace NetCoreTemplate.Application.Infrastructure {
  public class RequestPerformanceBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> {
    private readonly Stopwatch _timer;

    public RequestPerformanceBehaviour() {
      _timer = new Stopwatch();
    }

    public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next) {
      _timer.Start();

      var response = await next();

      _timer.Stop();

      if (_timer.ElapsedMilliseconds > 500) {
        var name = typeof(TRequest).Name;

        Log.Warning($"NetCoreTemplate Long Running Request: {name} ({_timer.ElapsedMilliseconds} milliseconds) @{request}");
      }

      return response;
    }
  }
}
