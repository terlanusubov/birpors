using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR.Pipeline;
using Newtonsoft.Json;
using Serilog;

namespace Birpors.Application.Behaviours
{
    public class RequestPostProcessorBehavior<TRequest,TResponse> : IRequestPostProcessor<TRequest,TResponse>
    {
        public async Task Process(TRequest request, TResponse response, CancellationToken cancellationToken)
        {
            var responseAsString = JsonConvert.SerializeObject(response);
            Log.Information("Response is : "+responseAsString);
            Log.Information("---Request Ended---");
        }
    }
}