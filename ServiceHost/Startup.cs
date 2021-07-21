using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using NLog;
using Persistence.Context;
using Persistence.Mappers;
using ServiceHost.Config;
using ServiceHost.ExceptionHandlers;
using ServiceHost.Extensions;
using System;
using System.Diagnostics.CodeAnalysis;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;

namespace ServiceHost
{
    [ExcludeFromCodeCoverage]
    public class Startup
    {
        private const string FeedServiceName = "FeedService";
        private const int BytesPerMb = 1024 * 1024;

        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            MapConfig.Configure();

            services.Configure<KestrelServerOptions>(Configuration.GetSection("Kestrel"));

            var httpClient = new HttpClient();
            services.AddSingleton(httpClient);
            var itemArray = GetCorsOrigin();
            if (itemArray.Any())
                services.AddCors(options =>
                {
                    options.AddDefaultPolicy(builder =>
                    {
                        builder.WithOrigins(itemArray).AllowAnyHeader().AllowAnyMethod();
                    });
                });

            services
                .AddMvc(options => options.EnableEndpointRouting = false)
                .AddMetrics()
                .SetCompatibilityVersion(CompatibilityVersion.Version_3_0)
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                });

       

            var httpContextAccessor = new HttpContextAccessor();
            services.AddSingleton<IHttpContextAccessor>(httpContextAccessor);

            var logger = LogManager.GetLogger(FeedServiceName);
            services.AddSingleton<ILogger>(logger);

            logger.Log(LogLevel.Info, "Service starting");
            
            var memoryCache = new MemoryCache(new MemoryCacheOptions());
            services.AddSingleton<IMemoryCache>(memoryCache);

            var jwtConfig = GetJwtConfig();
            AddAuthenticationToServices(services, jwtConfig);

            AddSwaggerConfigurationToServices(services, Configuration["RootUrl"]);

            services.AddSingleton(Configuration);

            var dbContextOptions = new DbContextOptionsBuilder<FeedContext>()
                .UseInMemoryDatabase(databaseName: "FeedInMemory")
                .Options;
            services.AddSingleton(dbContextOptions);
            services.AddDbContext<FeedContext>();

            services.ConfigureModelServices();
            services.ConfigurePersistenceServices();
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseExceptionHandler(exApp =>
                exApp.Run(ExceptionHandler.HandleExceptionRequest()));
            app.UseHsts();

            app.UseAuthentication();
            var itemArray = GetCorsOrigin();
            if (itemArray.Any()) app.UseCors();

            app.UseMvc();

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("./swagger/v1/swagger.json", FeedServiceName);
                c.RoutePrefix = string.Empty;
            });
        }

        private static void AddAuthenticationToServices(IServiceCollection services, JwtConfig jwtConfig)
        {
            var sharedKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtConfig.Key));
            services
                .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ClockSkew = TimeSpan.FromMinutes(5),
                        IssuerSigningKey = sharedKey,
                        RequireSignedTokens = true,
                        RequireExpirationTime = true,
                        ValidateLifetime = true,
                        ValidateAudience = true,
                        ValidAudience = jwtConfig.Audience,
                        ValidateIssuer = true,
                        ValidIssuer = jwtConfig.Issuer
                    };

                    var jwtHandler = options.SecurityTokenValidators.FirstOrDefault() as JwtSecurityTokenHandler;
                    jwtHandler?.InboundClaimTypeMap.Clear();
                });
        }

        private JwtConfig GetJwtConfig()
        {
            return new()
            {
                Audience = Configuration["Jwt:Audience"],
                Issuer = Configuration["Jwt:Issuer"],
                Key = Configuration["Jwt:Key"],
                SecondsToExpire = Convert.ToInt32(Configuration["Jwt:SecondsToExpire"]),
                CacheSecondsToExpire = Convert.ToInt32(Configuration["Jwt:CacheSecondsToExpire"])
            };
        }

        private string[] GetCorsOrigin()
        {
            var myArraySection = Configuration.GetSection("CorsOrigins");
            var itemArray = myArraySection.GetChildren().Select(c => c.Value).ToArray();
            return itemArray;
        }

        private static void AddSwaggerConfigurationToServices(IServiceCollection services, string configurationRootUrl)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = FeedServiceName,
                    Version = "v1"
                });
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description = "Please insert JWT with Bearer into field",
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new string[] { }
                    }
                });

                if (!string.IsNullOrEmpty(configurationRootUrl))
                {
                    c.AddServer(new OpenApiServer
                    {
                        Url = configurationRootUrl
                    });
                }
            });
        }
    }
}