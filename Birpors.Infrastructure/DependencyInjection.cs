using Birpors.Application.Interfaces;
using Birpors.Infrastructure.Persistance;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Birpors.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<IApplicationDbContext, ApplicationDbContext>(options =>
            {
                options.UseMySql(configuration["Database:ConnectionString"], ServerVersion.AutoDetect(configuration["Database:ConnectionString"]));
            });

            return services;
        }
    }
}
