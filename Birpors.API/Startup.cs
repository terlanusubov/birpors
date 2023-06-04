using Birpors.API.Chat;
using Birpors.API.Common;
using Birpors.API.Middlewares;
using Birpors.API.Sockets;
using Birpors.Application;
using Birpors.Infrastructure;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Connections;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.PlatformAbstractions;
using Microsoft.Extensions.Primitives;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Birpors.API
{

    public class CustomNegotiateMiddleware
    {
        private readonly RequestDelegate _next;

        public CustomNegotiateMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (context.Request.Path == "/chatHub/negotiate")
            {
                // Implement your custom negotiate logic here, for example:
                var connectionId = Guid.NewGuid().ToString();
                var hubUrl = "http://localhost:5000/chatHub?id=HZ0qNQ1m5AzKwGHmqGU0RA&access_token=eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJqdGkiOiI1MDFjMTI5ZC04ODFjLTRkYjQtODhhNS1iYWNjNWFmMGM1ZWYiLCJ1c2VySWQiOiIxMSIsInJvbGVJZCI6IjEwIiwibmJmIjoxNjgwNDY0NTA4LCJleHAiOjE2ODI1MzgxMDgsImlhdCI6MTY4MDQ2NDUwOCwiaXNzIjoiVGFybGFuIFVzdWJvdiIsImF1ZCI6IkJpcnBvcnMifQ.dP6s3R28UGtbrJZ7dj8Wku4qPDjgb6LUOSGTmph33IU";

                await context.Response.WriteAsync(JsonConvert.SerializeObject(new { connectionId, hubUrl }));
            }
            else
            {
                await _next(context);
            }
        }
    }

    public class Startup
    {


        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            services.AddControllers();

            services.AddApplication();

            services.AddInfrastructure(Configuration);

            var jwtConfig = new JwtConfig();
            Configuration.Bind(nameof(jwtConfig), jwtConfig);
            services.AddSingleton(jwtConfig);

            services.AddCors(options =>
            {

                options.AddPolicy(name: "front",
                                  policy =>
                                  {
                                      policy
                                      .AllowAnyOrigin()
                                      .AllowAnyHeader().AllowAnyMethod();
                                  });
            });

            var tokenValidationParameters = new TokenValidationParameters
            {
                ClockSkew = TimeSpan.Zero,
                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.ASCII.GetBytes(jwtConfig.Secret)
                ),
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidAudience = "Birpors",
                ValidIssuer = "Tarlan Usubov",
                ValidateIssuerSigningKey = true,
                RequireExpirationTime = true,
                ValidateLifetime = true,
            };

            services.AddSingleton<MyWebSocketManager>();
            services.AddSingleton(tokenValidationParameters);


            services
                .AddAuthentication(x =>
                {
                    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    x.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(x =>
                {
                    x.SaveToken = true;
                    x.TokenValidationParameters = tokenValidationParameters;
                    x.Events = new JwtBearerEvents
                    {
                        OnMessageReceived = context =>
                        {
                            var accessToken = context.Request.Query["access_token"];

                            // If the request is for our hub...
                            var path = context.HttpContext.Request.Path;
                            if (!string.IsNullOrEmpty(accessToken) &&
                                (path.StartsWithSegments("/chatHub")))
                            {
                                // Read the token out of the query string
                                context.Token = accessToken;
                            }

                            //if (!string.IsNullOrEmpty(accessToken) &&
                            //   (path.StartsWithSegments("/cookHub")))
                            //{
                            //    // Read the token out of the query string
                            //    context.Token = accessToken;
                            //}
                            return Task.CompletedTask;
                        }
                    };
                });

            services.AddSingleton<ChatClientManager>();

            services.AddSignalR(hubOptions =>
            {
                //hubOptions.SupportedProtocols =
                hubOptions.EnableDetailedErrors = true;
                hubOptions.KeepAliveInterval = TimeSpan.FromHours(1);
                hubOptions.HandshakeTimeout = TimeSpan.FromMinutes(10);
                hubOptions.ClientTimeoutInterval = TimeSpan.FromHours(1);
                hubOptions.MaximumParallelInvocationsPerClient = 2;
            });

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc(
                    "v1",
                    new OpenApiInfo
                    {
                        Title = "Birpors",
                        Version = "v1",
                        Description = "Birpors.",
                        Contact = new OpenApiContact
                        {
                            Name = "Birpors",
                            Email = "info@birpors.az",
                            Url = new Uri("https://birpors.az/"),
                        },
                    }
                );

                c.AddSecurityDefinition(
                    "Bearer",
                    new OpenApiSecurityScheme()
                    {
                        Description = "JWT Autentifikasiya üçün token daxil edin.",
                        Name = "Authorization",
                        In = ParameterLocation.Header,
                        Type = SecuritySchemeType.ApiKey
                    }
                );

                c.AddSecurityRequirement(
                    new OpenApiSecurityRequirement()
                    {
                        {
                            new OpenApiSecurityScheme
                            {
                                Reference = new OpenApiReference
                                {
                                    Type = ReferenceType.SecurityScheme,
                                    Id = "Bearer"
                                },
                                Scheme = "oauth2",
                                Name = "Bearer",
                                In = ParameterLocation.Header,
                            },
                            new List<string>()
                        }
                    }
                );
            });

            services.AddSwaggerGen();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)

        {

            app.UseDeveloperExceptionPage();

            app.UseCustomLogging();

            //app.UseHttpsRedirection();

            app.UseSwagger();

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Birpors API V1");
                c.RoutePrefix = string.Empty;
            });

            app.UseStaticFiles();

            app.UseRouting();

            //app.UseMiddleware<CustomNegotiateMiddleware>();
            app.UseCors("front");




            app.UseAuthorization();

            app.UseAuthentication();

            // app.UseWebSockets();


            app.UseEndpoints(endpoints =>
            {

                endpoints.MapHub<ChatHub>("/chatHub");

                endpoints.MapControllers();

            });
        }
    }
}

