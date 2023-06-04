using Birpors.Application.Interfaces;
using Birpors.Domain.Entities;
using Birpors.Domain.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Birpors.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SeedController : ControllerBase
    {
        private readonly IApplicationDbContext _context;
        public SeedController(IApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// SORĞU GÖNDƏRMƏYİN
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult<bool>> SeedAsync()
        {

            try
            {
                //Statuses

                if (!await _context.SubscriptionUserStatuses.AnyAsync())
                {
                    var sus1 = new SubscriptionUserStatus()
                    {
                        Id = 10,
                        Name = "Aktiv"
                    };

                    var sus2 = new SubscriptionUserStatus()
                    {
                        Id = 20,
                        Name = "Deaktiv"
                    };

                    await _context.SubscriptionUserStatuses.AddAsync(sus1);
                    await _context.SubscriptionUserStatuses.AddAsync(sus2);
                    await _context.SaveChanges();
                }

                if (!await _context.SubscriptionPlanStatuses.AnyAsync())
                {
                    var planStatus1 = new SubscriptionPlanStatus()
                    {
                        Id = 10,
                        Name = "Aktiv"
                    };

                    var planStatus2 = new SubscriptionPlanStatus()
                    {
                        Id = 20,
                        Name = "Deaktiv"
                    };


                    await _context.SubscriptionPlanStatuses.AddAsync(planStatus1);

                    await _context.SubscriptionPlanStatuses.AddAsync(planStatus2);

                    await _context.SaveChanges();
                }


                if (!await _context.UserDeviceStatuses.AnyAsync())
                {

                    var userDeviceStatuses = new List<UserDeviceStatus>
                {
                    new UserDeviceStatus
                    {
                        Id = 10,
                        Name = "Aktiv"
                    },
                    new UserDeviceStatus
                    {
                        Id = 20,
                        Name = "Deaktiv"
                    }
                };

                    await _context.UserDeviceStatuses.AddRangeAsync(userDeviceStatuses);
                    await _context.SaveChanges();

                }

                if (!await _context.KitchenStatuses.AnyAsync())
                {
                    var kitchenStatuses = new List<KitchenStatus>
                {
                    new KitchenStatus
                    {
                        Id = 10,
                        Name = "Aktiv",
                    },
                    new KitchenStatus
                    {
                        Id = 20,
                        Name = "Deaktiv"
                    },
                    new KitchenStatus
                    {
                        Id = 30,
                        Name = "Gözləmədə"
                    },
                    new KitchenStatus
                    {
                        Id = 40,
                        Name = "Bloklanmış"
                    }
                };

                    await _context.KitchenStatuses.AddRangeAsync(kitchenStatuses);
                    await _context.SaveChanges();
                }


                if (!(await _context.KitchenFoodStatuses.AnyAsync()))
                {
                    var kitchenFoodStatuses = new List<KitchenFoodStatus>
                {
                    new KitchenFoodStatus
                    {
                        Id = 10,
                        Name = "Aktiv"
                    }
                    ,
                    new KitchenFoodStatus
                    {
                        Id = 20,
                        Name = "Deaktiv"
                    }
                };



                    await _context.KitchenFoodStatuses.AddRangeAsync(kitchenFoodStatuses);
                    await _context.SaveChanges();



                }


                if (!await _context.UserStatuses.AnyAsync())
                {

                    var userStatuses = new List<UserStatus>
                {
                    new UserStatus
                    {
                        Id = 10,
                        Name = "Aktiv"
                    },
                     new UserStatus
                    {
                         Id = 20,
                        Name = "Deaktiv"
                    },
                       new UserStatus
                    {
                           Id = 30,
                        Name = "Bloklanmış"
                    }
                };

                    await _context.UserStatuses.AddRangeAsync(userStatuses);
                    await _context.SaveChanges();
                }


                if (!(await _context.Categories.AnyAsync()))
                {
                    var categories = new List<Category>
                            {

                                new Category
                                {
                                    Name = "Qazan"
                                },
                                new Category
                                {
                                    Name = "Sup"
                                },
                                new Category
                                {
                                    Name = "Kabab"
                                },
                                new Category
                                {
                                    Name = "Şirniyyat"
                                },
                                new Category
                                {
                                    Name = "Pizza"
                                },
                                new Category
                                {
                                    Name = "Içkilər"
                                }
                            };
                    await _context.Categories.AddRangeAsync(categories);
                    await _context.SaveChanges();
                }

                if (!(await _context.UserRoles.AnyAsync()))
                {
                    var userRoles = new List<UserRole>
                {
                    new UserRole
                    {
                        Id = 30,
                        Name = "Admin"
                    }
                    , new UserRole
                    {
                        Id = 10,
                        Name = "Istifadəçi"
                    }
                    ,
                    new UserRole
                    {
                        Id = 20,
                        Name = "Aşpaz"
                    }
                };

                    await _context.UserRoles.AddRangeAsync(userRoles);
                    await _context.SaveChanges();

                }

                if ( _context.Users.Count() == 1)
                {
                    var user = new User
                    {
                        Name = "Tərlan",
                        Surname = "Usubov",
                        UserStatusId = (int)UserStatusEnum.Active,
                        Created = DateTime.Now,
                        Email = "tarlan.usubov@gmail.com",
                        DefaultPaymentTypeId = (int)PaymentTypeEnum.Cash,
                        MasterKey = Guid.NewGuid().ToString(),
                        Phone = "994503401499",
                        Updated = DateTime.Now,
                        UserRoleId = (int)UserRoleEnum.User
                    };

                    using (HMACSHA256 hmac = new HMACSHA256(Encoding.UTF8.GetBytes(user.MasterKey)))
                    {
                        string password = String.Concat(user.Id.ToString(), user.MasterKey, "birporssecurepassword");
                        var passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
                        user.Password = passwordHash;

                    }

                    await _context.Users.AddAsync(user);
                    await _context.SaveChanges();


                    var userDevice = new UserDevice
                    {
                        UserDeviceStatusId = (int)UserDeviceStatusEnum.Deactive,
                        UserDeviceId = Guid.NewGuid().ToString(),
                        UserId = user.Id
                    };

                    await _context.UserDevices.AddAsync(userDevice);
                    await _context.SaveChanges();

                    var userAddress = new UserAddrese
                    {
                        Address = "Suraxani rayonu, Qaracuxur qesebesi",
                        AddressDescription = "Qaracuxur qesebesi , Zaur Serifov kucesi 4042/60",
                        Created = DateTime.Now,
                        IsMainAddress = true,
                        Latitude = "49.9902016",
                        Longitude = "40.4070686",
                        Updated = DateTime.Now,
                        UserId = user.Id
                    };

                    await _context.UserAddreses.AddAsync(userAddress);
                    await _context.SaveChanges();

                    var kitchen = new Kitchen
                    {
                        KitchenStatusId = 10,
                        Balance = 0,
                        Created = DateTime.Now,
                        Rating = 0,
                        Updated = DateTime.Now,
                        UserId = user.Id
                    };
                    await _context.Kitchens.AddAsync(kitchen);
                    await _context.SaveChanges();

                    var kitchenFoods = new List<KitchenFood>
                {
                    new KitchenFood
                    {
                        KitchenFoodStatusId = 10,
                        CategoryId = (await _context.Categories.ToListAsync())[0].Id,
                        Created = DateTime.Now,
                        DiscountPercentage = 0,
                        Ingredients = "Lorem Ipsum is simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry's standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book. It has survived not only five centuries, but also the leap into electronic typesetting, remaining essentially unchanged. It was popularised in the 1960s with the release of Letraset sheets containing Lorem Ipsum passages, and more recently with desktop publishing software like Aldus PageMaker including versions of Lorem Ipsum.",
                        Kalori = 100,
                        Name = "Lorem ipsum",
                        Price = 10,
                        RatedPeopleCount = 0,
                        Rating = 0,
                        Updated = DateTime.Now,
                        KitchenId = kitchen.Id
                    },
                    new KitchenFood
                    {
                        KitchenFoodStatusId = 10,
                        CategoryId = (await _context.Categories.ToListAsync())[0].Id,
                        Created = DateTime.Now,
                        DiscountPercentage = 0,
                        Ingredients = "Lorem Ipsum is simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry's standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book. It has survived not only five centuries, but also the leap into electronic typesetting, remaining essentially unchanged. It was popularised in the 1960s with the release of Letraset sheets containing Lorem Ipsum passages, and more recently with desktop publishing software like Aldus PageMaker including versions of Lorem Ipsum.",
                        Kalori = 100,
                        Name = "Lorem ipsum",
                        Price = 10,
                        RatedPeopleCount = 0,
                        Rating = 0,
                        Updated = DateTime.Now,
                        KitchenId = kitchen.Id
                    },
                    new KitchenFood
                    {
                        KitchenFoodStatusId = 10,
                        CategoryId = (await _context.Categories.ToListAsync())[0].Id,
                        Created = DateTime.Now,
                        DiscountPercentage = 0,
                        Ingredients = "Lorem Ipsum is simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry's standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book. It has survived not only five centuries, but also the leap into electronic typesetting, remaining essentially unchanged. It was popularised in the 1960s with the release of Letraset sheets containing Lorem Ipsum passages, and more recently with desktop publishing software like Aldus PageMaker including versions of Lorem Ipsum.",
                        Kalori = 100,
                        Name = "Lorem ipsum",
                        Price = 10,
                        RatedPeopleCount = 0,
                        Rating = 0,
                        Updated = DateTime.Now,
                        KitchenId = kitchen.Id
                    },
                    new KitchenFood
                    {
                        KitchenFoodStatusId = 10,
                        CategoryId = (await _context.Categories.ToListAsync())[0].Id,
                        Created = DateTime.Now,
                        DiscountPercentage = 0,
                        Ingredients = "Lorem Ipsum is simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry's standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book. It has survived not only five centuries, but also the leap into electronic typesetting, remaining essentially unchanged. It was popularised in the 1960s with the release of Letraset sheets containing Lorem Ipsum passages, and more recently with desktop publishing software like Aldus PageMaker including versions of Lorem Ipsum.",
                        Kalori = 100,
                        Name = "Lorem ipsum",
                        Price = 10,
                        RatedPeopleCount = 0,
                        Rating = 0,
                        Updated = DateTime.Now,
                        KitchenId = kitchen.Id
                    },
                    new KitchenFood
                    {
                        KitchenFoodStatusId = 10,
                        CategoryId = (await _context.Categories.ToListAsync())[0].Id,
                        Created = DateTime.Now,
                        DiscountPercentage = 0,
                        Ingredients = "Lorem Ipsum is simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry's standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book. It has survived not only five centuries, but also the leap into electronic typesetting, remaining essentially unchanged. It was popularised in the 1960s with the release of Letraset sheets containing Lorem Ipsum passages, and more recently with desktop publishing software like Aldus PageMaker including versions of Lorem Ipsum.",
                        Kalori = 100,
                        Name = "Lorem ipsum",
                        Price = 10,
                        RatedPeopleCount = 0,
                        Rating = 0,
                        Updated = DateTime.Now,
                        KitchenId = kitchen.Id
                    },
                    new KitchenFood
                    {
                        KitchenFoodStatusId = 10,
                        CategoryId = (await _context.Categories.ToListAsync())[1].Id,
                        Created = DateTime.Now,
                        DiscountPercentage = 0,
                        Ingredients = "Lorem Ipsum is simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry's standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book. It has survived not only five centuries, but also the leap into electronic typesetting, remaining essentially unchanged. It was popularised in the 1960s with the release of Letraset sheets containing Lorem Ipsum passages, and more recently with desktop publishing software like Aldus PageMaker including versions of Lorem Ipsum.",
                        Kalori = 100,
                        Name = "Lorem ipsum",
                        Price = 10,
                        RatedPeopleCount = 0,
                        Rating = 0,
                        Updated = DateTime.Now,
                        KitchenId = kitchen.Id
                    },
                    new KitchenFood
                    {
                        KitchenFoodStatusId = 10,
                        CategoryId = (await _context.Categories.ToListAsync())[1].Id,
                        Created = DateTime.Now,
                        DiscountPercentage = 0,
                        Ingredients = "Lorem Ipsum is simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry's standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book. It has survived not only five centuries, but also the leap into electronic typesetting, remaining essentially unchanged. It was popularised in the 1960s with the release of Letraset sheets containing Lorem Ipsum passages, and more recently with desktop publishing software like Aldus PageMaker including versions of Lorem Ipsum.",
                        Kalori = 100,
                        Name = "Lorem ipsum",
                        Price = 10,
                        RatedPeopleCount = 0,
                        Rating = 0,
                        Updated = DateTime.Now,
                        KitchenId = kitchen.Id
                    },
                    new KitchenFood
                    {
                        KitchenFoodStatusId = 10,
                        CategoryId = (await _context.Categories.ToListAsync())[1].Id,
                        Created = DateTime.Now,
                        DiscountPercentage = 0,
                        Ingredients = "Lorem Ipsum is simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry's standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book. It has survived not only five centuries, but also the leap into electronic typesetting, remaining essentially unchanged. It was popularised in the 1960s with the release of Letraset sheets containing Lorem Ipsum passages, and more recently with desktop publishing software like Aldus PageMaker including versions of Lorem Ipsum.",
                        Kalori = 100,
                        Name = "Lorem ipsum",
                        Price = 10,
                        RatedPeopleCount = 0,
                        Rating = 0,
                        Updated = DateTime.Now,
                        KitchenId = kitchen.Id
                    },
                    new KitchenFood
                    {
                        KitchenFoodStatusId = 10,
                        CategoryId = (await _context.Categories.ToListAsync())[1].Id,
                        Created = DateTime.Now,
                        DiscountPercentage = 0,
                        Ingredients = "Lorem Ipsum is simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry's standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book. It has survived not only five centuries, but also the leap into electronic typesetting, remaining essentially unchanged. It was popularised in the 1960s with the release of Letraset sheets containing Lorem Ipsum passages, and more recently with desktop publishing software like Aldus PageMaker including versions of Lorem Ipsum.",
                        Kalori = 100,
                        Name = "Lorem ipsum",
                        Price = 10,
                        RatedPeopleCount = 0,
                        Rating = 0,
                        Updated = DateTime.Now,
                        KitchenId = kitchen.Id
                    },
                    new KitchenFood
                    {
                        KitchenFoodStatusId = 10,
                        CategoryId = (await _context.Categories.ToListAsync())[1].Id,
                        Created = DateTime.Now,
                        DiscountPercentage = 0,
                        Ingredients = "Lorem Ipsum is simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry's standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book. It has survived not only five centuries, but also the leap into electronic typesetting, remaining essentially unchanged. It was popularised in the 1960s with the release of Letraset sheets containing Lorem Ipsum passages, and more recently with desktop publishing software like Aldus PageMaker including versions of Lorem Ipsum.",
                        Kalori = 100,
                        Name = "Lorem ipsum",
                        Price = 10,
                        RatedPeopleCount = 0,
                        Rating = 0,
                        Updated = DateTime.Now,
                        KitchenId = kitchen.Id
                    },
                    new KitchenFood
                    {
                        KitchenFoodStatusId = 10,
                        CategoryId = (await _context.Categories.ToListAsync())[2].Id,
                        Created = DateTime.Now,
                        DiscountPercentage = 0,
                        Ingredients = "Lorem Ipsum is simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry's standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book. It has survived not only five centuries, but also the leap into electronic typesetting, remaining essentially unchanged. It was popularised in the 1960s with the release of Letraset sheets containing Lorem Ipsum passages, and more recently with desktop publishing software like Aldus PageMaker including versions of Lorem Ipsum.",
                        Kalori = 100,
                        Name = "Lorem ipsum",
                        Price = 10,
                        RatedPeopleCount = 0,
                        Rating = 0,
                        Updated = DateTime.Now,
                        KitchenId = kitchen.Id
                    },
                     new KitchenFood
                    {
                        KitchenFoodStatusId = 10,
                        CategoryId = (await _context.Categories.ToListAsync())[2].Id,
                        Created = DateTime.Now,
                        DiscountPercentage = 0,
                        Ingredients = "Lorem Ipsum is simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry's standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book. It has survived not only five centuries, but also the leap into electronic typesetting, remaining essentially unchanged. It was popularised in the 1960s with the release of Letraset sheets containing Lorem Ipsum passages, and more recently with desktop publishing software like Aldus PageMaker including versions of Lorem Ipsum.",
                        Kalori = 100,
                        Name = "Lorem ipsum",
                        Price = 10,
                        RatedPeopleCount = 0,
                        Rating = 0,
                        Updated = DateTime.Now,
                        KitchenId = kitchen.Id
                    },
                      new KitchenFood
                    {
                        KitchenFoodStatusId = 10,
                        CategoryId = (await _context.Categories.ToListAsync())[2].Id,
                        Created = DateTime.Now,
                        DiscountPercentage = 0,
                        Ingredients = "Lorem Ipsum is simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry's standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book. It has survived not only five centuries, but also the leap into electronic typesetting, remaining essentially unchanged. It was popularised in the 1960s with the release of Letraset sheets containing Lorem Ipsum passages, and more recently with desktop publishing software like Aldus PageMaker including versions of Lorem Ipsum.",
                        Kalori = 100,
                        Name = "Lorem ipsum",
                        Price = 10,
                        RatedPeopleCount = 0,
                        Rating = 0,
                        Updated = DateTime.Now,
                        KitchenId = kitchen.Id
                    },
                       new KitchenFood
                    {
                        KitchenFoodStatusId = 10,
                        CategoryId = (await _context.Categories.ToListAsync())[2].Id,
                        Created = DateTime.Now,
                        DiscountPercentage = 0,
                        Ingredients = "Lorem Ipsum is simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry's standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book. It has survived not only five centuries, but also the leap into electronic typesetting, remaining essentially unchanged. It was popularised in the 1960s with the release of Letraset sheets containing Lorem Ipsum passages, and more recently with desktop publishing software like Aldus PageMaker including versions of Lorem Ipsum.",
                        Kalori = 100,
                        Name = "Lorem ipsum",
                        Price = 10,
                        RatedPeopleCount = 0,
                        Rating = 0,
                        Updated = DateTime.Now,
                        KitchenId = kitchen.Id
                    },
                        new KitchenFood
                    {
                        KitchenFoodStatusId = 10,
                        CategoryId = (await _context.Categories.ToListAsync())[2].Id,
                        Created = DateTime.Now,
                        DiscountPercentage = 0,
                        Ingredients = "Lorem Ipsum is simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry's standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book. It has survived not only five centuries, but also the leap into electronic typesetting, remaining essentially unchanged. It was popularised in the 1960s with the release of Letraset sheets containing Lorem Ipsum passages, and more recently with desktop publishing software like Aldus PageMaker including versions of Lorem Ipsum.",
                        Kalori = 100,
                        Name = "Lorem ipsum",
                        Price = 10,
                        RatedPeopleCount = 0,
                        Rating = 0,
                        Updated = DateTime.Now,
                        KitchenId = kitchen.Id
                    },
                         new KitchenFood
                    {
                        KitchenFoodStatusId = 10,
                        CategoryId = (await _context.Categories.ToListAsync())[2].Id,
                        Created = DateTime.Now,
                        DiscountPercentage = 0,
                        Ingredients = "Lorem Ipsum is simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry's standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book. It has survived not only five centuries, but also the leap into electronic typesetting, remaining essentially unchanged. It was popularised in the 1960s with the release of Letraset sheets containing Lorem Ipsum passages, and more recently with desktop publishing software like Aldus PageMaker including versions of Lorem Ipsum.",
                        Kalori = 100,
                        Name = "Lorem ipsum",
                        Price = 10,
                        RatedPeopleCount = 0,
                        Rating = 0,
                        Updated = DateTime.Now,
                        KitchenId = kitchen.Id
                    },
                          new KitchenFood
                    {
                        KitchenFoodStatusId = 10,
                        CategoryId = (await _context.Categories.ToListAsync())[2].Id,
                        Created = DateTime.Now,
                        DiscountPercentage = 0,
                        Ingredients = "Lorem Ipsum is simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry's standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book. It has survived not only five centuries, but also the leap into electronic typesetting, remaining essentially unchanged. It was popularised in the 1960s with the release of Letraset sheets containing Lorem Ipsum passages, and more recently with desktop publishing software like Aldus PageMaker including versions of Lorem Ipsum.",
                        Kalori = 100,
                        Name = "Lorem ipsum",
                        Price = 10,
                        RatedPeopleCount = 0,
                        Rating = 0,
                        Updated = DateTime.Now,
                        KitchenId = kitchen.Id
                    },
                            new KitchenFood
                    {
                        KitchenFoodStatusId = 10,
                        CategoryId = (await _context.Categories.ToListAsync())[3].Id,
                        Created = DateTime.Now,
                        DiscountPercentage = 0,
                        Ingredients = "Lorem Ipsum is simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry's standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book. It has survived not only five centuries, but also the leap into electronic typesetting, remaining essentially unchanged. It was popularised in the 1960s with the release of Letraset sheets containing Lorem Ipsum passages, and more recently with desktop publishing software like Aldus PageMaker including versions of Lorem Ipsum.",
                        Kalori = 100,
                        Name = "Lorem ipsum",
                        Price = 10,
                        RatedPeopleCount = 0,
                        Rating = 0,
                        Updated = DateTime.Now,
                        KitchenId = kitchen.Id
                    },
                              new KitchenFood
                    {
                        KitchenFoodStatusId = 10,
                        CategoryId = (await _context.Categories.ToListAsync())[3].Id,
                        Created = DateTime.Now,
                        DiscountPercentage = 0,
                        Ingredients = "Lorem Ipsum is simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry's standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book. It has survived not only five centuries, but also the leap into electronic typesetting, remaining essentially unchanged. It was popularised in the 1960s with the release of Letraset sheets containing Lorem Ipsum passages, and more recently with desktop publishing software like Aldus PageMaker including versions of Lorem Ipsum.",
                        Kalori = 100,
                        Name = "Lorem ipsum",
                        Price = 10,
                        RatedPeopleCount = 0,
                        Rating = 0,
                        Updated = DateTime.Now,
                        KitchenId = kitchen.Id
                    }
                              ,  new KitchenFood
                    {
                        KitchenFoodStatusId = 10,
                        CategoryId = (await _context.Categories.ToListAsync())[3].Id,
                        Created = DateTime.Now,
                        DiscountPercentage = 0,
                        Ingredients = "Lorem Ipsum is simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry's standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book. It has survived not only five centuries, but also the leap into electronic typesetting, remaining essentially unchanged. It was popularised in the 1960s with the release of Letraset sheets containing Lorem Ipsum passages, and more recently with desktop publishing software like Aldus PageMaker including versions of Lorem Ipsum.",
                        Kalori = 100,
                        Name = "Lorem ipsum",
                        Price = 10,
                        RatedPeopleCount = 0,
                        Rating = 0,
                        Updated = DateTime.Now,
                        KitchenId = kitchen.Id
                    }
                              ,  new KitchenFood
                    {
                        KitchenFoodStatusId = 10,
                        CategoryId = (await _context.Categories.ToListAsync())[3].Id,
                        Created = DateTime.Now,
                        DiscountPercentage = 0,
                        Ingredients = "Lorem Ipsum is simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry's standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book. It has survived not only five centuries, but also the leap into electronic typesetting, remaining essentially unchanged. It was popularised in the 1960s with the release of Letraset sheets containing Lorem Ipsum passages, and more recently with desktop publishing software like Aldus PageMaker including versions of Lorem Ipsum.",
                        Kalori = 100,
                        Name = "Lorem ipsum",
                        Price = 10,
                        RatedPeopleCount = 0,
                        Rating = 0,
                        Updated = DateTime.Now,
                        KitchenId = kitchen.Id
                    }
                              ,  new KitchenFood
                    {
                        KitchenFoodStatusId = 10,
                        CategoryId = (await _context.Categories.ToListAsync())[3].Id,
                        Created = DateTime.Now,
                        DiscountPercentage = 0,
                        Ingredients = "Lorem Ipsum is simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry's standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book. It has survived not only five centuries, but also the leap into electronic typesetting, remaining essentially unchanged. It was popularised in the 1960s with the release of Letraset sheets containing Lorem Ipsum passages, and more recently with desktop publishing software like Aldus PageMaker including versions of Lorem Ipsum.",
                        Kalori = 100,
                        Name = "Lorem ipsum",
                        Price = 10,
                        RatedPeopleCount = 0,
                        Rating = 0,
                        Updated = DateTime.Now,
                        KitchenId = kitchen.Id
                    },
                                new KitchenFood
                    {
                        KitchenFoodStatusId = 10,
                        CategoryId = (await _context.Categories.ToListAsync())[4].Id,
                        Created = DateTime.Now,
                        DiscountPercentage = 0,
                        Ingredients = "Lorem Ipsum is simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry's standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book. It has survived not only five centuries, but also the leap into electronic typesetting, remaining essentially unchanged. It was popularised in the 1960s with the release of Letraset sheets containing Lorem Ipsum passages, and more recently with desktop publishing software like Aldus PageMaker including versions of Lorem Ipsum.",
                        Kalori = 100,
                        Name = "Lorem ipsum",
                        Price = 10,
                        RatedPeopleCount = 0,
                        Rating = 0,
                        Updated = DateTime.Now,
                        KitchenId = kitchen.Id
                    },
                                        new KitchenFood
                    {
                        KitchenFoodStatusId = 10,
                        CategoryId = (await _context.Categories.ToListAsync())[4].Id,
                        Created = DateTime.Now,
                        DiscountPercentage = 0,
                        Ingredients = "Lorem Ipsum is simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry's standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book. It has survived not only five centuries, but also the leap into electronic typesetting, remaining essentially unchanged. It was popularised in the 1960s with the release of Letraset sheets containing Lorem Ipsum passages, and more recently with desktop publishing software like Aldus PageMaker including versions of Lorem Ipsum.",
                        Kalori = 100,
                        Name = "Lorem ipsum",
                        Price = 10,
                        RatedPeopleCount = 0,
                        Rating = 0,
                        Updated = DateTime.Now,
                        KitchenId = kitchen.Id
                    },
                                                new KitchenFood
                    {
                        KitchenFoodStatusId = 10,
                        CategoryId = (await _context.Categories.ToListAsync())[4].Id,
                        Created = DateTime.Now,
                        DiscountPercentage = 0,
                        Ingredients = "Lorem Ipsum is simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry's standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book. It has survived not only five centuries, but also the leap into electronic typesetting, remaining essentially unchanged. It was popularised in the 1960s with the release of Letraset sheets containing Lorem Ipsum passages, and more recently with desktop publishing software like Aldus PageMaker including versions of Lorem Ipsum.",
                        Kalori = 100,
                        Name = "Lorem ipsum",
                        Price = 10,
                        RatedPeopleCount = 0,
                        Rating = 0,
                        Updated = DateTime.Now,
                        KitchenId = kitchen.Id
                    },
                                                        new KitchenFood
                    {
                        KitchenFoodStatusId = 10,
                        CategoryId = (await _context.Categories.ToListAsync())[4].Id,
                        Created = DateTime.Now,
                        DiscountPercentage = 0,
                        Ingredients = "Lorem Ipsum is simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry's standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book. It has survived not only five centuries, but also the leap into electronic typesetting, remaining essentially unchanged. It was popularised in the 1960s with the release of Letraset sheets containing Lorem Ipsum passages, and more recently with desktop publishing software like Aldus PageMaker including versions of Lorem Ipsum.",
                        Kalori = 100,
                        Name = "Lorem ipsum",
                        Price = 10,
                        RatedPeopleCount = 0,
                        Rating = 0,
                        Updated = DateTime.Now,
                        KitchenId = kitchen.Id
                    },
                                                                new KitchenFood
                    {
                        KitchenFoodStatusId = 10,
                        CategoryId = (await _context.Categories.ToListAsync())[4].Id,
                        Created = DateTime.Now,
                        DiscountPercentage = 0,
                        Ingredients = "Lorem Ipsum is simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry's standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book. It has survived not only five centuries, but also the leap into electronic typesetting, remaining essentially unchanged. It was popularised in the 1960s with the release of Letraset sheets containing Lorem Ipsum passages, and more recently with desktop publishing software like Aldus PageMaker including versions of Lorem Ipsum.",
                        Kalori = 100,
                        Name = "Lorem ipsum",
                        Price = 10,
                        RatedPeopleCount = 0,
                        Rating = 0,
                        Updated = DateTime.Now,
                        KitchenId = kitchen.Id
                    },
                                                                        new KitchenFood
                    {
                        KitchenFoodStatusId = 10,
                        CategoryId = (await _context.Categories.ToListAsync())[5].Id,
                        Created = DateTime.Now,
                        DiscountPercentage = 0,
                        Ingredients = "Lorem Ipsum is simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry's standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book. It has survived not only five centuries, but also the leap into electronic typesetting, remaining essentially unchanged. It was popularised in the 1960s with the release of Letraset sheets containing Lorem Ipsum passages, and more recently with desktop publishing software like Aldus PageMaker including versions of Lorem Ipsum.",
                        Kalori = 100,
                        Name = "Lorem ipsum",
                        Price = 10,
                        RatedPeopleCount = 0,
                        Rating = 0,
                        Updated = DateTime.Now,
                        KitchenId = kitchen.Id
                    },
  new KitchenFood
                    {
                        KitchenFoodStatusId = 10,
                        CategoryId = (await _context.Categories.ToListAsync())[5].Id,
                        Created = DateTime.Now,
                        DiscountPercentage = 0,
                        Ingredients = "Lorem Ipsum is simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry's standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book. It has survived not only five centuries, but also the leap into electronic typesetting, remaining essentially unchanged. It was popularised in the 1960s with the release of Letraset sheets containing Lorem Ipsum passages, and more recently with desktop publishing software like Aldus PageMaker including versions of Lorem Ipsum.",
                        Kalori = 100,
                        Name = "Lorem ipsum",
                        Price = 10,
                        RatedPeopleCount = 0,
                        Rating = 0,
                        Updated = DateTime.Now,
                        KitchenId = kitchen.Id
                    },  new KitchenFood
                    {
                        KitchenFoodStatusId = 10,
                        CategoryId = (await _context.Categories.ToListAsync())[5].Id,
                        Created = DateTime.Now,
                        DiscountPercentage = 0,
                        Ingredients = "Lorem Ipsum is simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry's standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book. It has survived not only five centuries, but also the leap into electronic typesetting, remaining essentially unchanged. It was popularised in the 1960s with the release of Letraset sheets containing Lorem Ipsum passages, and more recently with desktop publishing software like Aldus PageMaker including versions of Lorem Ipsum.",
                        Kalori = 100,
                        Name = "Lorem ipsum",
                        Price = 10,
                        RatedPeopleCount = 0,
                        Rating = 0,
                        Updated = DateTime.Now,
                        KitchenId = kitchen.Id
                    },  new KitchenFood
                    {
                        KitchenFoodStatusId = 10,
                        CategoryId = (await _context.Categories.ToListAsync())[5].Id,
                        Created = DateTime.Now,
                        DiscountPercentage = 0,
                        Ingredients = "Lorem Ipsum is simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry's standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book. It has survived not only five centuries, but also the leap into electronic typesetting, remaining essentially unchanged. It was popularised in the 1960s with the release of Letraset sheets containing Lorem Ipsum passages, and more recently with desktop publishing software like Aldus PageMaker including versions of Lorem Ipsum.",
                        Kalori = 100,
                        Name = "Lorem ipsum",
                        Price = 10,
                        RatedPeopleCount = 0,
                        Rating = 0,
                        Updated = DateTime.Now,
                        KitchenId = kitchen.Id
                    },  new KitchenFood
                    {
                        KitchenFoodStatusId = 10,
                        CategoryId = (await _context.Categories.ToListAsync())[5].Id,
                        Created = DateTime.Now,
                        DiscountPercentage = 0,
                        Ingredients = "Lorem Ipsum is simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry's standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book. It has survived not only five centuries, but also the leap into electronic typesetting, remaining essentially unchanged. It was popularised in the 1960s with the release of Letraset sheets containing Lorem Ipsum passages, and more recently with desktop publishing software like Aldus PageMaker including versions of Lorem Ipsum.",
                        Kalori = 100,
                        Name = "Lorem ipsum",
                        Price = 10,
                        RatedPeopleCount = 0,
                        Rating = 0,
                        Updated = DateTime.Now,
                        KitchenId = kitchen.Id
                    },  new KitchenFood
                    {
                        KitchenFoodStatusId = 10,
                        CategoryId = (await _context.Categories.ToListAsync())[5].Id,
                        Created = DateTime.Now,
                        DiscountPercentage = 0,
                        Ingredients = "Lorem Ipsum is simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry's standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book. It has survived not only five centuries, but also the leap into electronic typesetting, remaining essentially unchanged. It was popularised in the 1960s with the release of Letraset sheets containing Lorem Ipsum passages, and more recently with desktop publishing software like Aldus PageMaker including versions of Lorem Ipsum.",
                        Kalori = 100,
                        Name = "Lorem ipsum",
                        Price = 10,
                        RatedPeopleCount = 0,
                        Rating = 0,
                        Updated = DateTime.Now,
                        KitchenId = kitchen.Id
                    },
   new KitchenFood
                    {
                        KitchenFoodStatusId = 10,
                        CategoryId = (await _context.Categories.ToListAsync())[5].Id,
                        Created = DateTime.Now,
                        DiscountPercentage = 0,
                        Ingredients = "Lorem Ipsum is simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry's standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book. It has survived not only five centuries, but also the leap into electronic typesetting, remaining essentially unchanged. It was popularised in the 1960s with the release of Letraset sheets containing Lorem Ipsum passages, and more recently with desktop publishing software like Aldus PageMaker including versions of Lorem Ipsum.",
                        Kalori = 100,
                        Name = "Lorem ipsum",
                        Price = 10,
                        RatedPeopleCount = 0,
                        Rating = 0,
                        Updated = DateTime.Now,
                        KitchenId = kitchen.Id
                    },
    new KitchenFood
                    {
                        KitchenFoodStatusId = 10,
                        CategoryId = (await _context.Categories.ToListAsync())[5].Id,
                        Created = DateTime.Now,
                        DiscountPercentage = 0,
                        Ingredients = "Lorem Ipsum is simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry's standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book. It has survived not only five centuries, but also the leap into electronic typesetting, remaining essentially unchanged. It was popularised in the 1960s with the release of Letraset sheets containing Lorem Ipsum passages, and more recently with desktop publishing software like Aldus PageMaker including versions of Lorem Ipsum.",
                        Kalori = 100,
                        Name = "Lorem ipsum",
                        Price = 10,
                        RatedPeopleCount = 0,
                        Rating = 0,
                        Updated = DateTime.Now,
                        KitchenId = kitchen.Id
                    },
     new KitchenFood
                    {
                        KitchenFoodStatusId = 10,
                        CategoryId = (await _context.Categories.ToListAsync())[5].Id,
                        Created = DateTime.Now,
                        DiscountPercentage = 0,
                        Ingredients = "Lorem Ipsum is simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry's standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book. It has survived not only five centuries, but also the leap into electronic typesetting, remaining essentially unchanged. It was popularised in the 1960s with the release of Letraset sheets containing Lorem Ipsum passages, and more recently with desktop publishing software like Aldus PageMaker including versions of Lorem Ipsum.",
                        Kalori = 100,
                        Name = "Lorem ipsum",
                        Price = 10,
                        RatedPeopleCount = 0,
                        Rating = 0,
                        Updated = DateTime.Now,
                        KitchenId = kitchen.Id
                    },
      new KitchenFood
                    {
                        KitchenFoodStatusId = 10,
                        CategoryId = (await _context.Categories.ToListAsync())[5].Id,
                        Created = DateTime.Now,
                        DiscountPercentage = 0,
                        Ingredients = "Lorem Ipsum is simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry's standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book. It has survived not only five centuries, but also the leap into electronic typesetting, remaining essentially unchanged. It was popularised in the 1960s with the release of Letraset sheets containing Lorem Ipsum passages, and more recently with desktop publishing software like Aldus PageMaker including versions of Lorem Ipsum.",
                        Kalori = 100,
                        Name = "Lorem ipsum",
                        Price = 10,
                        RatedPeopleCount = 0,
                        Rating = 0,
                        Updated = DateTime.Now,
                        KitchenId = kitchen.Id
                    },
                         new KitchenFood
                    {
                        KitchenFoodStatusId = 10,
                        CategoryId = (await _context.Categories.ToListAsync())[5].Id,
                        Created = DateTime.Now,
                        DiscountPercentage = 0,
                        Ingredients = "Lorem Ipsum is simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry's standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book. It has survived not only five centuries, but also the leap into electronic typesetting, remaining essentially unchanged. It was popularised in the 1960s with the release of Letraset sheets containing Lorem Ipsum passages, and more recently with desktop publishing software like Aldus PageMaker including versions of Lorem Ipsum.",
                        Kalori = 100,
                        Name = "Lorem ipsum",
                        Price = 10,
                        RatedPeopleCount = 0,
                        Rating = 0,
                        Updated = DateTime.Now,
                        KitchenId = kitchen.Id
                    }


                };

                    await _context.KitchenFoods.AddRangeAsync(kitchenFoods);
                    await _context.SaveChanges();
                }

                if (!(await _context.KitchenFoodPhotos.AnyAsync()))
                {
                    var kitchenFoods = await _context.KitchenFoods.ToListAsync();

                    foreach (var food in kitchenFoods)
                    {
                        var kitchenFoodPhoto = new KitchenFoodPhoto
                        {
                            Image = "food.jpg",
                            IsMain = true,
                            KitchenFoodId = food.Id
                        };

                        await _context.KitchenFoodPhotos.AddAsync(kitchenFoodPhoto);
                        await _context.SaveChanges();
                    }
                }


                return true;

            }

            catch
            {
                return false;
            }


        }


        /// <summary>
        /// SORĞU GÖNDƏRMƏYİN
        /// </summary>
        /// <returns></returns>

        [HttpGet("updatePassword")]
        public async Task<ActionResult<bool>> UpdatePassword()
        {
            var user = await _context.Users.FirstOrDefaultAsync();

            using (HMACSHA256 hmac = new HMACSHA256(Encoding.UTF8.GetBytes(user.MasterKey)))
            {
                string password = String.Concat(user.Id.ToString(), user.Created.ToString());
                var passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
                user.Password = passwordHash;

            }

            await _context.SaveChanges();
            return true;
        }
    }
}
