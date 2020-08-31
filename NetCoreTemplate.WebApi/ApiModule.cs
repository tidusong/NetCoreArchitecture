using Autofac;
using Microsoft.AspNetCore.Http;
using MediatR;
using MediatR.Pipeline;
using NetCoreTemplate.Application.Auth.Commands.Register;
using System.Reflection;
using NetCoreTemplate.Application.Infrastructure;
using NetCoreTemplate.Application.Users.Queries.GetUserDetail;
using NetCoreTemplate.Application.Interfaces.Auth;
using NetCoreTemplate.Application.Auth;

namespace NetCoreTemplate.WebApi {
  public class ApiModule : Autofac.Module {
    protected override void Load(ContainerBuilder builder) {
      builder.RegisterAssemblyTypes(ThisAssembly).AsImplementedInterfaces();
      builder.RegisterType<HttpContextAccessor>().As<IHttpContextAccessor>().SingleInstance();

      builder.RegisterType<JwtFactory>().As<IJwtFactory>().SingleInstance();
      builder.RegisterType<JwtTokenHandler>().As<IJwtTokenHandler>().SingleInstance();
      builder.RegisterType<TokenFactory>().As<ITokenFactory>().SingleInstance();
      builder.RegisterType<JwtTokenValidator>().As<IJwtTokenValidator>().SingleInstance();

      #region MediatR
      builder.RegisterAssemblyTypes(typeof(IMediator).GetTypeInfo().Assembly).AsImplementedInterfaces();

      var mediatrOpenTypes = new[]
        {
            typeof(IRequestHandler<,>),
            typeof(IRequestHandler<>)
        };

      foreach (var mediatrOpenType in mediatrOpenTypes) {
        // Register all command handler in the same assembly as WriteLogMessageCommandHandler
        builder
            .RegisterAssemblyTypes(typeof(RegisterCommandHandler).GetTypeInfo().Assembly)
            .AsClosedTypesOf(mediatrOpenType)
            .AsImplementedInterfaces();

        // Register all QueryHandlers in the same assembly as GetExternalLoginQueryHandler
        builder
            .RegisterAssemblyTypes(typeof(GetUserDetailQueryHandler).GetTypeInfo().Assembly)
            .AsClosedTypesOf(mediatrOpenType)
            .AsImplementedInterfaces();
      }

      builder.RegisterGeneric(typeof(RequestPostProcessorBehavior<,>)).As(typeof(IPipelineBehavior<,>));
      builder.RegisterGeneric(typeof(RequestPreProcessorBehavior<,>)).As(typeof(IPipelineBehavior<,>));
      builder.RegisterGeneric(typeof(RequestLogger<>)).As(typeof(IRequestPreProcessor<>));

      builder.Register<ServiceFactory>(ctx => {
        var c = ctx.Resolve<IComponentContext>();
        return t => {
          object o;
          return c.TryResolve(t, out o) ? o : null;
        };
      });
      #endregion
    }
  }
}
