using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using NetCoreTemplate.Application.Auth.Commands.Register;
using NetCoreTemplate.WebApi.Filters;
using System;
using Autofac.Extensions.DependencyInjection;
using Autofac;
using NetCoreTemplate.Common;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Microsoft.AspNetCore.ResponseCompression;
using System.IO.Compression;
using Serilog;
using Swashbuckle.AspNetCore.Swagger;
using AutoMapper;
using AutofacSerilogIntegration;
using NetCoreTemplate.Common.Auth;
using System.Collections.Generic;
using NetCoreTemplate.Persistence.Data;
using NetCoreTemplate.Persistence.Identity;
using NetCoreTemplate.Domain.Entities;

namespace NetCoreTemplate.WebApi {
  public class Startup {
    private readonly IConfiguration _configuration;

    public Startup(IConfiguration configuration, IHostingEnvironment hostingEnvironment) {
      _configuration = configuration;

      Log.Information($"Constructing for environment: {hostingEnvironment.EnvironmentName}");
    }

    protected IContainer ApplicationContainer { get; private set; }

    // This method gets called by the runtime. Use this method to add services to the container.
    public IServiceProvider ConfigureServices(IServiceCollection services) {
      Log.Information("Starting: Configure Services");

      services.AddOptions();

      #region Config Jwt
      var authSettings = _configuration.GetSection(nameof(AuthSettings));
      services.Configure<AuthSettings>(authSettings);

      var signingKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(authSettings[nameof(AuthSettings.SecretKey)]));

      var jwtAppSettingsOptions = _configuration.GetSection(nameof(JwtIssuerOptions));

      services.Configure<JwtIssuerOptions>(options => {
        options.Issuer = jwtAppSettingsOptions[nameof(JwtIssuerOptions.Issuer)];
        options.Audience = jwtAppSettingsOptions[nameof(JwtIssuerOptions.Audience)];
        options.SigningCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);
      });

      var tokenValidationParameters = new TokenValidationParameters {
        ValidateIssuer = true,
        ValidIssuer = jwtAppSettingsOptions[nameof(JwtIssuerOptions.Issuer)],

        ValidateAudience = true,
        ValidAudience = jwtAppSettingsOptions[nameof(JwtIssuerOptions.Audience)],

        ValidateIssuerSigningKey = true,
        IssuerSigningKey = signingKey,

        RequireExpirationTime = false,
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero
      };

      services.AddAuthentication(options => {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
      })
      .AddJwtBearer(configureOptions => {
        configureOptions.ClaimsIssuer = jwtAppSettingsOptions[nameof(JwtIssuerOptions.Issuer)];
        configureOptions.TokenValidationParameters = tokenValidationParameters;
        configureOptions.SaveToken = true;

        configureOptions.Events = new JwtBearerEvents {
          OnAuthenticationFailed = context => {
            if (context.Exception.GetType() == typeof(SecurityTokenExpiredException)) {
              context.Response.Headers.Add("Token-Expired", "true");
            }
            return Task.CompletedTask;
          }
        };
      });
      #endregion

      // User claim policy
      services.AddAuthorization(options => {
        options.AddPolicy("ApiUser", policy => policy.RequireClaim(Constants.Strings.JwtClaimIdentifiers.Rol,
          Constants.Strings.JwtClaims.ApiAccess));
      });

      #region Add identity
      var identityBuilder = services.AddIdentityCore<AppUser>(o => {
        o.Password.RequireDigit = false;
        o.Password.RequireLowercase = false;
        o.Password.RequireUppercase = false;
        o.Password.RequireNonAlphanumeric = false;
        o.Password.RequiredLength = 6;
      });

      identityBuilder = new IdentityBuilder(identityBuilder.UserType, typeof(IdentityRole), identityBuilder.Services);
      identityBuilder.AddEntityFrameworkStores<NetCoreTemplateIdentityDbContext>().AddDefaultTokenProviders();
      #endregion

      services.AddAutoMapper();

      //services.AddAntiforgery(options => options.HeaderName = "X-XSRF-TOKEN");
      services.AddRouting(options => options.LowercaseUrls = true);

      #region Config CORS
      services.AddCors((options => options.AddPolicy("AllowAllOrigins",
        builder => {
          builder.AllowAnyOrigin()
                .AllowAnyHeader()
                .AllowAnyMethod()
                .SetPreflightMaxAge(TimeSpan.FromSeconds(2250));
        })));
      #endregion

      services.AddDbContext<NetCoreTemplateDbContext>(options =>
          options.UseSqlServer(_configuration.GetConnectionString("NetCoreTemplateDatabase"), b => b.MigrationsAssembly("NetCoreTemplate.Persistence")));
      services.AddDbContext<NetCoreTemplateIdentityDbContext>(options =>
          options.UseSqlServer(_configuration.GetConnectionString("NetCoreTemplateDatabase"), b => b.MigrationsAssembly("NetCoreTemplate.Persistence")));

      #region AddMvc
      services.AddMvc(o => {
        o.Filters.Add(typeof(GlobalExceptionFilter));
        o.ModelValidatorProviders.Clear();

        var policy = new AuthorizationPolicyBuilder()
          .RequireAuthenticatedUser()
          .Build();
        o.Filters.Add(new AuthorizeFilter(policy));
      })
      .AddJsonOptions(options => {
        var settings = options.SerializerSettings;

        var camelCasePropertyNamesContractResolver = new CamelCasePropertyNamesContractResolver();

        settings.ContractResolver = camelCasePropertyNamesContractResolver;
        settings.Converters = new JsonConverter[] {
                      new IsoDateTimeConverter(),
                      new StringEnumConverter(new DefaultNamingStrategy(), true)
        };
      })
      .SetCompatibilityVersion(CompatibilityVersion.Version_2_1)
      .AddFluentValidation(fv => fv.RegisterValidatorsFromAssemblyContaining<RegisterCommandValidator>());
      #endregion

      #region Config Compression
      services.Configure<GzipCompressionProviderOptions>(options => options.Level = CompressionLevel.Fastest);
      services.AddResponseCompression(options => { options.Providers.Add<GzipCompressionProvider>(); });
      #endregion

      #region Config Swagger
      services.AddSwaggerGen(c => {
        c.SwaggerDoc("v1", new Info {
          Version = "v1",
          Title = "Net Core Template API",
          Description = "Net Core Template API"
        });
        c.AddSecurityDefinition("Bearer", new ApiKeyScheme {
          In = "header",
          Description = "Please insert Jwt with Bearer into field",
          Name = "Authorization",
          Type = "apiKey"
        });

        c.AddSecurityRequirement(new Dictionary<string, IEnumerable<string>> {
          { "Bearer", new string[] { } }
        });
        c.OperationFilter<FormFileOperationFilter>();
      });
      #endregion

      services.AddMemoryCache();

      #region Autofac
      var autofacBuilder = new ContainerBuilder();

      autofacBuilder.Register(ctx => _configuration).As<IConfiguration>();
      //autofacBuilder.RegisterModule(new CommonModule());
      autofacBuilder.RegisterModule(new ApiModule());
      autofacBuilder.RegisterLogger();

      autofacBuilder.Populate(services);

      ApplicationContainer = autofacBuilder.Build();

      var provider = new AutofacServiceProvider(ApplicationContainer);
      #endregion

      Log.Information("Completing: Configure Services");

      return provider;
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IHostingEnvironment env, IConfiguration configuration, IApplicationLifetime appLifetime) {
      Log.Information("Starting: Configure");


      app.UseAuthentication();
      app.UseFileServer();
      app.UseStaticFiles();
      app.UseCors("AllowAllOrigins");

      app.UseSwagger();
      app.UseSwaggerUI(c => {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Net Core Template API V1");
      });

      app.UseResponseCompression();

      app.UseMvc();

      Log.Information("Completing: Configure");
    }
  }
}
