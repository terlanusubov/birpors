using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using Serilog;

namespace Birpors.Application.Behaviours
{
    public class RequestLoggerBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    {
        private readonly HttpContext _httpContext;
        public RequestLoggerBehavior(IHttpContextAccessor httpContextAccessor)
        {
            _httpContext = httpContextAccessor.HttpContext;
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            //Request

            var ipAddress = _httpContext.Connection.RemoteIpAddress?.ToString();

            Log.Information("Request started {0} time and {1} ip address", DateTime.Now, ipAddress);

            Log.Information($"Handling {typeof(TRequest).Name}");

            Type myType = request.GetType();

            IList<PropertyInfo> props = new List<PropertyInfo>(myType.GetProperties());

            foreach (PropertyInfo prop in props)
            {
                object propValue = prop.GetValue(request, null);
                Log.Information("{Property} : {@Value}", prop.Name, propValue);
            }

            var response = await next();

            //Response
            Log.Information($"Handled {typeof(TResponse).Name}");

            return await next();

        }
    }
}