using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using MediatR.Pipeline;

namespace Birpors.Application.Behaviours
{
    public class RequestExceptionBehavior<TRequest, TResponse,TException> : IRequestExceptionHandler<TRequest, TResponse, TException> where TException : Exception
    {
        public async Task Handle(TRequest request, TException exception, RequestExceptionHandlerState<TResponse> state, CancellationToken cancellationToken)
        {
           // throw new NotImplementedException();
        }
    }
}