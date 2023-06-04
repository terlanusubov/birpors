using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Birpors.MVC.Controllers
{
    public class BaseController : Controller
    {
        private IMediator _mediator;

        public IMediator Mediator { get => _mediator ??= HttpContext.RequestServices.GetService<IMediator>(); }

    }
}