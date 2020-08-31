using System.Threading;
using System.Threading.Tasks;
using MediatR.Pipeline;
using Serilog;
using Serilog.Events;

namespace NetCoreTemplate.Application.Infrastructure {
  public class RequestLogger<TRequest> : IRequestPreProcessor<TRequest> {

    public Task Process(TRequest request, CancellationToken cancellationToken) {
      var name = typeof(TRequest).Name;

      Log.Information($"NetCoreTemplate Request: {name} @{request}");

      return Task.CompletedTask;
    }
  }
}
