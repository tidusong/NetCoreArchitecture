using Microsoft.AspNetCore.Hosting;
using System;
using System.IO;
using Microsoft.Extensions.Configuration;
using System.Reflection;
using Serilog;
using Serilog.Events;
using Microsoft.AspNetCore;
using System.Net;
using Microsoft.Extensions.Logging;

namespace NetCoreTemplate.WebApi {
  public class Program {
    private static IConfiguration configuration { get; } = new ConfigurationBuilder()
      .SetBasePath(Directory.GetCurrentDirectory())
      .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
      .AddJsonFile("appsettings.Local.json", optional: true, reloadOnChange: true)
      .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json", optional: true, reloadOnChange: true)
      .AddEnvironmentVariables()
      .Build();
    public static int Main(string[] args) {
      var logPath = Path.Combine(Directory.GetCurrentDirectory(), "logs");
      if (!Directory.Exists(logPath)) {
        Directory.CreateDirectory(logPath);
      }

      Log.Logger = new LoggerConfiguration()
        .ReadFrom.Configuration(configuration)
        .CreateLogger();

      try {
        BuildWebHost(args).Run();
        return 0;
      } catch (Exception exception) {
        Log.Fatal(exception, "Site terminated.");
        return 1;
      } finally {
        Log.CloseAndFlush();
      }
    }

    public static IWebHost BuildWebHost(string[] args) {
      return WebHost.CreateDefaultBuilder(args)
        .UseKestrel()
        .UseContentRoot(Directory.GetCurrentDirectory())
        .ConfigureLogging((hostingContext, config) => {
          config.ClearProviders();
        })
        .UseConfiguration(configuration)
        .UseStartup<Startup>()
        .UseSerilog()
        .Build();
    }
  }
}
