using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using MediatR.Pipeline;

namespace Birpors.Application.Behaviours
{
    public class RequestPreProcessorBehavior<TRequest> : IRequestPreProcessor<TRequest>
    {
        public async Task Process(TRequest request, CancellationToken cancellationToken)
        {
            
        }
    }
}