using Birpors.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Birpors.MVC.Controllers
{
    [Route("sifarisler", Name = "orders")]
    [Authorize(Roles = "Admin")]
    public class OrderController : Controller
    {
        private readonly IApplicationDbContext _context;

        public OrderController(IApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> List()
        {

            var orders = await _context.Orders
                .Include(c => c.Cook)
                .Include(c => c.User)
                .ToListAsync();

            return View(orders);
        }
    }
}
