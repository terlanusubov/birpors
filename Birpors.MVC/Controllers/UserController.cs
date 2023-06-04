using Azure.Core;
using Birpors.Application.CQRS.Admin.Users.Queries.GetUserRoles;
using Birpors.Application.CQRS.Admin.Users.Queries.GetUsers;
using Birpors.Application.Interfaces;
using Birpors.Domain.DTOs;
using Birpors.Domain.DTOs.Admin;
using Birpors.Domain.Entities;
using Birpors.Domain.Enums;
using Birpors.MVC.ViewModels;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Birpors.MVC.Controllers
{
    [Route("istifadeciler", Name = "users")]
    [Authorize(Roles = "Admin")]
    public class UserController : BaseController
    {
        private readonly IApplicationDbContext _context;
        private readonly IConfiguration _configuration;
        public UserController(IApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public async Task<IActionResult> List(GetUsersRequest model, GetUserRolesRequest model2)
        {
            ViewBag.PageTitle = "İstifadəçilər";
            UserViewModel viewmodel = new UserViewModel();
            var response = await Mediator.Send(new GetUsersQuery(model));

            var response_userrole = await Mediator.Send(new GetUserRolesQuery(model2));

            viewmodel.Users = response;
            viewmodel.UserRoles = response_userrole;

            return View(viewmodel);
        }

        [Route("{userId}", Name = "userDetail")]
        public async Task<IActionResult> Detail(int userId)
        {
            var user = await _context.Users.Include(c => c.UserRole).Where(c => c.Id == userId).Select(c => new Domain.DTOs.UserDto
            {
                Id = c.Id,
                StatusId = c.UserStatusId,
                Surname = c.Surname,
                Email = c.Email,
                Name = c.Name,
                RoleId = c.UserRoleId,
                RoleName = c.UserRole.Name,
                Created = (DateTime)c.Created,
                Phone = c.Phone
            }).FirstOrDefaultAsync();


            if (user == null)
                return RedirectToRoute("users");

            var vm = new UserDetailVm();
            vm.UserDetail = user;
            return View(vm);
        }


        [Route("status/{userId}")]
        [HttpPost]
        public async Task<JsonResult> ChangeStatus(int userId)
        {
            var user = await _context.Users.Where(c => c.Id == userId).FirstOrDefaultAsync();
            if (user == null)
                return Json(new
                {
                    status = 400
                });

            user.UserStatusId = user.UserStatusId == (byte)UserStatusEnum.Active ? (byte)UserStatusEnum.Deactive : (byte)UserStatusEnum.Active;

            if (user.UserRoleId == (byte)UserRoleEnum.Cook)
            {
                var kitchen = await _context.Kitchens.Where(c => c.UserId == user.Id).FirstOrDefaultAsync();

                if (kitchen == null)
                    return Json(new
                    {
                        status = 400
                    });


                kitchen.KitchenStatusId = user.UserStatusId == (byte)UserStatusEnum.Active ? (byte)KitchenStatusEnum.Active : (byte)KitchenStatusEnum.Deactive;

                await _context.SaveChanges();


            }

            return Json(new
            {
                status = 200
            });
        }

        [Route("{userId}/sifarisler")]
        [HttpGet]
        public async Task<JsonResult> UserOrders(int userId)
        {
            var user = await _context.Users.Include(c => c.UserRole).Where(c => c.Id == userId).FirstOrDefaultAsync();


            if (user == null)
            {
                return Json(new
                {
                    status = 400
                });
            }


            var orders = await _context.Orders.Include(c => c.Cook).Include(c => c.User).Where(c => (user.UserRoleId == (byte)UserRoleEnum.User ? c.UserId == userId : c.CookId == userId)).Select(c => new OrderDto
            {
                Id = c.Id,
                CookName = c.Cook.Name,
                CookSurname = c.Cook.Surname,
                CookUserId = c.CookId,
                OrderDate = c.OrderDate,
                OrderStatusId = c.OrderStatusId,
                TransactionId = c.TransactionId,
                DeliveryDate = c.DeliverDate,
                Rating = c.Rating,
                PaymentTypeId = c.PaymentTypeId,
                OrderUserName = c.User.Name,
                OrderUserSurname = c.User.Surname,
                DeliverPrice = c.DeliverPrice,
                TotalPrice = c.TotalPrice
            }).ToListAsync();

            return Json(new
            {
                status = 200,
                data = orders
            });

        }

        [Route("{userId}/sifarisler/{orderId}")]
        [HttpGet]
        public async Task<JsonResult> UserOrderDetails(int userId, int orderId)
        {
            var user = await _context.Users.Include(c => c.UserRole).Where(c => c.Id == userId).FirstOrDefaultAsync();


            if (user == null)
            {
                return Json(new
                {
                    status = 400
                });
            }

            var baseProductImagePath = _configuration["Files:Foods"];

            var order = await _context.Orders
                .Include(c => c.Cook)
                .Include(c => c.User)
                                           .Include(c => c.UserAddress)
                                           .Include(c => c.OrderDetails)
                                               .ThenInclude(c => c.KitchenFood)
                                               .ThenInclude(c => c.KitchenFoodPhotos)
                                               .Where(c => c.Id == orderId)
                                                   .FirstOrDefaultAsync();

            if (order == null)
                return Json(new
                {
                    status = 400
                });

            var orderDetails = order.OrderDetails.OrderByDescending(c=>c.Order.OrderDate).Select(c => new OrderInvoiceDto
            {
                OrderId = c.OrderId,
                Count = c.Count,
                DiscountPercentage = c.DiscountPercentage,
                FoodName = c.KitchenFood.Name,
                Image = baseProductImagePath + c.KitchenFood.KitchenFoodPhotos.Where(c => c.IsMain).Select(c => c.Image).FirstOrDefault(),
                Ingredients = c.KitchenFood.Ingredients,
                ProductId = c.KitchenFoodId,
                Note = c.Note,
                Price = c.Price,
                TotalPrice = c.TotalPrice,
                UserAddress = order.UserAddress.AddressDescription,
                UserAddressId = order.UserAddressId,
                CookSurname = order.Cook.Surname,
                OrderUserSurname = order.User.Surname,
                CookName = order.Cook.Name,
                OrderUserName = order.User.Name,
                CookUserId = (int)order.CookId,
                OrderDate = order.OrderDate,
                DeliveryDate = order.DeliverDate
            }).ToList();

            return Json(new
            {
                status = 200,
                data = orderDetails
            });

        }


        [Route("{userId}/metbex")]
        [HttpGet]
        public async Task<JsonResult> CookKitchen(int userId)
        {
            var user = await _context.Users.Include(c => c.UserRole).Where(c => c.Id == userId && c.UserRoleId == (int)UserRoleEnum.Cook).FirstOrDefaultAsync();
            if (user == null)
                return Json(new
                {
                    status = 400
                });

            string kitchenImageBasePath = _configuration["Files:Kitchens"];
            string foodImageBasePath = _configuration["Files:Foods"];

            var kitchen = await _context.Kitchens
                                            .Include(c => c.KitchenPhotos)
                                                .Include(c => c.KitchenStatus)
                                                    .Include(c => c.KitchenFoods)
                                                    .ThenInclude(c => c.KitchenFoodPhotos)
                                                    .Include(c => c.KitchenFoods)
                                                    .ThenInclude(c => c.KitchenFoodStatus)
                                                    .Where(c => c.UserId == userId)
                                                        .Select(c => new KitchenDto
                                                        {
                                                            Created = (DateTime)c.Created,
                                                            KitchenId = c.Id,
                                                            KitchenStatusId = c.KitchenStatusId,
                                                            KitchenStatusName = c.KitchenStatus.Name,
                                                            KitchenImages = c.KitchenPhotos.Select(a => kitchenImageBasePath + a.Image).ToList(),
                                                            KitchenDetails = c.KitchenFoods.Select(a => new KitchenDetailDto
                                                            {
                                                                KitchenDetailId = a.Id,
                                                                CategoryId = a.CategoryId,
                                                                CategoryName = a.Category.Name,
                                                                KitchenFoodStatusId = a.KitchenFoodStatusId,
                                                                KitchenFoodStatusName = a.KitchenFoodStatus.Name,
                                                                Created = DateTime.Now,
                                                                DiscountPercentage = a.DiscountPercentage == null ? 0 : (decimal)a.DiscountPercentage,
                                                                Kalori = a.Kalori,
                                                                KitchenId = a.KitchenId,
                                                                MainImage = a.KitchenFoodPhotos.Where(a => a.IsMain).Select(a => foodImageBasePath + a.Image).FirstOrDefault(),
                                                                RatedPeopleCount = a.RatedPeopleCount,
                                                                Name = a.Name,
                                                                Price = a.Price,
                                                                Rating = a.Rating
                                                            }).ToList()
                                                        }).FirstOrDefaultAsync();


            return Json(new
            {
                status = 200,
                data = kitchen
            });

        }
        //[Route("{userId}", Name = "userDetail")]
        //public async Task<IActionResult> UserDetail(int userId)
        //{
        //    var user = await _context.Users.Include(c => c.UserRole).Where(c => c.Id == userId && c.UserRoleId == (int)UserRoleEnum.User).Select(c => new UserDto
        //    {
        //        Id = c.Id,
        //        StatusId = c.UserStatusId,
        //        Surname = c.Surname,
        //        Email = c.Email,
        //        Name = c.Name,
        //        RoleId = c.UserRoleId,
        //        RoleName = c.UserRole.Name,
        //        Created = (DateTime)c.Created,
        //        Phone = c.Phone
        //    }).FirstOrDefaultAsync();


        //    if (user == null)
        //    {
        //        return RedirectToAction("List", "User");
        //    }


        //    var orders = await _context.Orders.Include(c => c.Cook).Include(c => c.User).Where(c => c.UserId == userId).Select(c => new OrderDto
        //    {
        //        Id = c.Id,
        //        CookName = c.Cook.Name,
        //        CookSurname = c.Cook.Surname,
        //        CookUserId = c.CookId,
        //        OrderDate = c.OrderDate,
        //        OrderStatusId = c.OrderStatusId,
        //        TransactionId = c.TransactionId,
        //        DeliveryDate = c.DeliverDate,
        //        Rating = c.Rating,
        //        PaymentTypeId = c.PaymentTypeId
        //    }).ToListAsync();

        //    var userStatuses = await _context.UserStatuses.ToListAsync();

        //    UserDetailVm vm = new UserDetailVm();

        //    vm.UserDetail = user;
        //    vm.Orders = orders;
        //    vm.UserStatuses = userStatuses;

        //    return View(vm);
        //}

        //[Route("sifaris/{orderId}", Name = "userOrder")]
        //public async Task<IActionResult> DetailUserOrders(int orderId)
        //{
        //    var orders = await _context.OrderDetails
        //                                    .Include(c => c.Order)
        //                                    .ThenInclude(c => c.UserAddress)
        //                                    .Include(c => c.KitchenFood)
        //                                    .ThenInclude(c => c.KitchenFoodPhotos)
        //                                    .Where(c => c.OrderId == orderId)
        //                                    .ToListAsync();

        //    return View(orders);
        //}

        //[Route("aspaz", Name = "cookDetail")]
        //public async Task<IActionResult> CookDetail(int userId)
        //{
        //    string foodImageBasePath = _configuration["Files:Foods"];
        //    string kitchenImageBasePath = _configuration["Files:Kitchens"];
        //    CookDetailVm vm = new CookDetailVm();

        //    var userDetail = await _context.Users.Include(c => c.UserRole).Where(c => c.Id == userId && c.UserRoleId == (int)UserRoleEnum.Cook).Select(c => new UserDto
        //    {
        //        Id = c.Id,
        //        StatusId = c.UserStatusId,
        //        Surname = c.Surname,
        //        Email = c.Email,
        //        Name = c.Name,
        //        RoleId = c.UserRoleId,
        //        RoleName = c.UserRole.Name,
        //        Created = (DateTime)c.Created,
        //        Phone = c.Phone,
        //        Balance = c.Kitchens.FirstOrDefault().Balance
        //    }).FirstOrDefaultAsync();

        //    if (userDetail == null)
        //    {
        //        return RedirectToAction("List", "User");
        //    }

        //    vm.UserDetail = userDetail;
        //    var kitchen = await _context.Kitchens
        //                                    .Include(c => c.KitchenPhotos)
        //                                        .Include(c => c.KitchenStatus)
        //                                            .Include(c => c.KitchenFoods)
        //                                            .ThenInclude(c => c.KitchenFoodPhotos)
        //                                            .Include(c => c.KitchenFoods)
        //                                            .ThenInclude(c => c.KitchenFoodStatus)
        //                                            .Where(c => c.UserId == userDetail.Id)
        //                                                .Select(c => new KitchenDto
        //                                                {
        //                                                    Created = (DateTime)c.Created,
        //                                                    KitchenId = c.Id,
        //                                                    KitchenStatusId = c.KitchenStatusId,
        //                                                    KitchenStatusName = c.KitchenStatus.Name,
        //                                                    KitchenImages = c.KitchenPhotos.Select(a => kitchenImageBasePath + a.Image).ToList(),
        //                                                    KitchenDetails = c.KitchenFoods.Select(a => new KitchenDetailDto
        //                                                    {
        //                                                        KitchenDetailId = a.Id,
        //                                                        CategoryId = a.CategoryId,
        //                                                        CategoryName = a.Category.Name,
        //                                                        KitchenFoodStatusId = a.KitchenFoodStatusId,
        //                                                        KitchenFoodStatusName = a.KitchenFoodStatus.Name,
        //                                                        Created = DateTime.Now,
        //                                                        DiscountPercentage = a.DiscountPercentage == null ? 0 : (decimal)a.DiscountPercentage,
        //                                                        Kalori = a.Kalori,
        //                                                        KitchenId = a.KitchenId,
        //                                                        MainImage = a.KitchenFoodPhotos.Where(a => a.IsMain).Select(a => foodImageBasePath + a.Image).FirstOrDefault(),
        //                                                        RatedPeopleCount = a.RatedPeopleCount,
        //                                                        Name = a.Name,
        //                                                        Price = a.Price,
        //                                                        Rating = a.Rating
        //                                                    }).ToList()
        //                                                }).FirstOrDefaultAsync();

        //    if (kitchen == null)
        //    {
        //        return RedirectToAction("List", "User");
        //    }
        //    vm.Kitchen = kitchen;

        //    var orders = await _context.Orders
        //                                    .Include(c => c.User)
        //                                    .Include(c => c.Cook)
        //                                    .Where(c => c.CookId == userDetail.Id)
        //                                        .Select(c => new OrderDto
        //                                        {
        //                                            Id = c.Id,
        //                                            CookName = userDetail.Name,
        //                                            CookSurname = userDetail.Surname,
        //                                            CookUserId = userDetail.Id,
        //                                            OrderDate = c.OrderDate,
        //                                            OrderStatusId = c.OrderStatusId,
        //                                            TransactionId = c.TransactionId,
        //                                            DeliveryDate = c.DeliverDate,
        //                                            Rating = c.Rating,
        //                                            PaymentTypeId = c.PaymentTypeId,
        //                                            OrderUserName = c.User.Name,
        //                                            OrderUserSurname = c.User.Surname,
        //                                            TotalPrice = c.TotalPrice,
        //                                            DeliverPrice = c.DeliverPrice
        //                                        }).OrderByDescending(c => c.Id)
        //                                         .ToListAsync();


        //    vm.Orders = orders;

        //    var userStatuses = await _context.UserStatuses.ToListAsync();
        //    vm.UserStatuses = userStatuses;

        //    vm.KitchenStatuses = await _context.KitchenStatuses.ToListAsync();

        //    decimal totalInCome = vm.Orders.Sum(c => c.TotalPrice);

        //    vm.OrderCount = orders.Count();

        //    vm.InCome = totalInCome - (totalInCome * 20 / 100);

        //    vm.Balance = userDetail.Balance;

        //    vm.Comission = 20;

        //    return View(vm);
        //}

        ////public async Task<IActionResult> Edit()
        ////{
        ////    return View();
        ////}



        //[HttpPost]
        //public async Task<IActionResult> UpdateKitchen(byte statusId, int userId)
        //{
        //    var user = await _context.Users.Where(c => c.Id == userId && c.UserRoleId == (int)UserRoleEnum.Cook).FirstOrDefaultAsync();
        //    if (user == null)
        //        return RedirectToAction("List", "User");

        //    var kitchen = await _context.Kitchens.Where(c => c.UserId == user.Id).FirstOrDefaultAsync();

        //    if (kitchen == null)
        //        return RedirectToAction("List", "User");


        //    kitchen.KitchenStatusId = statusId;

        //    await _context.SaveChanges();


        //    return RedirectToAction("CookDetail", "User", new { userId = userId });

        //}


        //[HttpPost]
        //public async Task<IActionResult> UpdateUser(byte statusId, int userId)
        //{
        //    var user = await _context.Users.Where(c => c.Id == userId).FirstOrDefaultAsync();
        //    if (user == null)
        //        return RedirectToAction("List", "User");

        //    user.UserStatusId = statusId;
        //    await _context.SaveChanges();

        //    if (user.UserRoleId == (int)UserRoleEnum.User)
        //        return RedirectToAction("UserDetail", "User", new { userId = userId });
        //    else
        //        return RedirectToAction("CookDetail", "User", new { userId = userId });

        //}

        //[Route("/gozleyenler", Name = "waiters")]
        //[HttpGet]
        //public async Task<JsonResult> GetWaitingCooks()
        //{
        //    var users = await _context.Users.Where(c => c.UserRoleId == (int)UserRoleEnum.Cook && c.UserStatusId == (int)UserStatusEnum.Active && c.Kitchens.FirstOrDefault().KitchenStatusId == (int)KitchenStatusEnum.Waiting)
        //        .Select(c => new UserDto
        //        {
        //            Id = c.Id,
        //            Name = c.Name,
        //            Surname = c.Surname,
        //            Created = (DateTime)c.Created
        //        })
        //        .ToListAsync();


        //    var count = users.Count();


        //    return Json(new
        //    {
        //        status = 200,
        //        data = users,
        //        count = count
        //    });
        //}
    }
}
