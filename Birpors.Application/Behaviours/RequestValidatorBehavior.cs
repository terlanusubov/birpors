using FluentValidation;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Birpors.Application.Behaviours
{
    public class RequestValidatorBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    {
        private readonly IEnumerable<IValidator<TRequest>> _validators;
        public RequestValidatorBehavior(IEnumerable<IValidator<TRequest>> validators)
        {
            _validators = validators;
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            var context = new ValidationContext<TRequest>(request);

            var failures = _validators.Select(x => x.Validate(context))
                                       .SelectMany(x => x.Errors)
                                       .Where(x => x != null)
                                       .ToList();

            Dictionary<string, string> errors = new Dictionary<string, string>();

            foreach (var error in failures)
            {
                errors?.Add(TakePropName(error.PropertyName), error.ErrorMessage);
            }

            if (failures.Count > 0)
                return await Task.FromResult((TResponse)typeof(TResponse).GetMethod("Error")?
                                                                                .Invoke(Activator.CreateInstance(typeof(TResponse), true), new object[] { errors, (int)HttpStatusCode.BadRequest, "Validasiya xətası baş verdi." }));

            return await next();
        }

        private string TakePropName(string fullPropName)
        {
            return fullPropName.Split(".").TakeLast(1).FirstOrDefault();
        }

    }
}
