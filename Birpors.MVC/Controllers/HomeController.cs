using Birpors.Application.Interfaces;
using Birpors.Domain.Enums;
using Birpors.MVC.Models;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Birpors.MVC.Controllers
{
    public class HomeVm
    {
        public int Order { get; set; }
        public int Users { get; set; }
        public int Cooks { get; set; }
        public int InCome { get; set; }
    }

    [Route("ana-sehife", Name = "main")]
     [Authorize(Roles = "Admin")]
    public class HomeController : BaseController
    {
        private readonly IApplicationDbContext _context;
        public HomeController(IApplicationDbContext context)
        {
            _context = context;
        }


        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var homeVm = new HomeVm
            {
                Order = await _context.Orders.CountAsync(),
                Cooks = await _context.Users.CountAsync(c => c.UserRoleId == (byte)UserRoleEnum.Cook),
                InCome = 0,
                Users = await _context.Users.CountAsync(c => c.UserRoleId == (byte)UserRoleEnum.User)
            };
            return View(homeVm);
        }

        [Route("xeta", Name = "error")]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
