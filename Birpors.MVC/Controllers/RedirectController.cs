using System;
using Microsoft.AspNetCore.Mvc;

namespace Birpors.MVC.Controllers
{
    public class RedirectController : Controller
    {
        public RedirectController()
        {
        }

        public IActionResult Redirect()
        {
            return RedirectToRoute("main");
        }
    }
}

