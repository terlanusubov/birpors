using Birpors.Application.Interfaces;
using Birpors.Domain.Entities;
using Birpors.Domain.Enums;
using Birpors.MVC.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Birpors.MVC.Controllers
{
    [Route("giris", Name = "login")]
    public class AccountController : Controller
    {
        private readonly IApplicationDbContext _context;
        public AccountController(IApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Login()
        {
            //var user = new User()
            //{
            //    Name = "Admin",
            //    Surname = "Admin",
            //    UserStatusId = (byte)UserStatusEnum.Active,
            //    Created = DateTime.Now,
            //    Updated = DateTime.Now,
            //    Email = "admin@birpors.az",
            //    MasterKey = Guid.NewGuid().ToString(),
            //    Phone = "0505000000",
            //    UserRoleId = (byte)UserRoleEnum.Admin
            //};


            //var pass = "12345678";
            //using (SHA256 sha256 = SHA256.Create())
            //{
            //    var buffer = Encoding.UTF8.GetBytes(pass);

            //    var hash = sha256.ComputeHash(buffer);

            //    user.Password = hash;
            //}

            //await _context.Users.AddAsync(user);
            //await _context.SaveChanges();

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginModel model)
        {
            var user = await _context.Users.Where(c => c.Email == model.Email).FirstOrDefaultAsync();
            if (user == null) { return RedirectToRoute("login"); }


            if (user.UserRoleId != (byte)UserRoleEnum.Admin) { return RedirectToRoute("login"); }

            using(SHA256 sha256 = SHA256.Create())
            {
                var buffer = Encoding.UTF8.GetBytes(model.Password);

                var hash = sha256.ComputeHash(buffer);

                if (user.Password.SequenceEqual(hash))
                {
                    var claims = new List<Claim>
        {
            new Claim("userId", user.Id.ToString()),
            new Claim("Name", user.Name),
            new Claim("Surname", user.Surname),
            new Claim(ClaimTypes.Role, "Admin")
        };

                    var claimsIdentity = new ClaimsIdentity(
                        claims, CookieAuthenticationDefaults.AuthenticationScheme);

                    var authProperties = new AuthenticationProperties
                    {
                        //AllowRefresh = <bool>,
                        // Refreshing the authentication session should be allowed.

                        //ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(10),
                        // The time at which the authentication ticket expires. A 
                        // value set here overrides the ExpireTimeSpan option of 
                        // CookieAuthenticationOptions set with AddCookie.

                        //IsPersistent = true,
                        // Whether the authentication session is persisted across 
                        // multiple requests. When used with cookies, controls
                        // whether the cookie's lifetime is absolute (matching the
                        // lifetime of the authentication ticket) or session-based.

                        //IssuedUtc = <DateTimeOffset>,
                        // The time at which the authentication ticket was issued.

                        //RedirectUri = <string>
                        // The full path or absolute URI to be used as an http 
                        // redirect response value.
                    };

                    await HttpContext.SignInAsync(
                        CookieAuthenticationDefaults.AuthenticationScheme,
                        new ClaimsPrincipal(claimsIdentity),
                        authProperties);
                    return RedirectToRoute("main");
                }
            }

         


            return RedirectToRoute("login");
        }

        [Route("cixis")]
        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            return RedirectToRoute("main");
        }
    }
}
