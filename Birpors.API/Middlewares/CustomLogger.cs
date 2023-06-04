using Birpors.Application.Interfaces;
using Birpors.Domain.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Birpors.API.Middlewares
{
    public class CustomLogger
    {
        private readonly RequestDelegate _next;

        public CustomLogger(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            httpContext.Request.EnableBuffering();

            var database = httpContext.RequestServices.GetService<IApplicationDbContext>();
            var bodyStream = httpContext.Request.Body;

            bool hasToken = httpContext.Request.Headers.TryGetValue("Authorization", out var token);

            MemoryStream memoryStream = new MemoryStream();


            await bodyStream.CopyToAsync(memoryStream);
            byte[] bodyBuffer = memoryStream.ToArray();
            await database.AppLogs.AddAsync(new Domain.Entities.AppLog()
            {
                LogText = Encoding.UTF8.GetString(bodyBuffer) + "token:" + (hasToken ? token.ToString() : "token yoxdur"),
                Created = DateTime.Now,
                LogType = (int)LogTypeEnum.Information
            });

            await database.SaveChanges();

            httpContext.Request.Body = memoryStream;

            httpContext.Request.Body.Position = 0;

            await _next(httpContext);

        }
    }

}
