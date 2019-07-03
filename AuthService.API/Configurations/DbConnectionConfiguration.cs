using AuthService.CORE.EF;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthService.API.Configurations
{
    public static class DbConnectionConfiguration
    {
        public static IServiceCollection AddConnectionProvider(this IServiceCollection services, IConfiguration conf)
        {
            services.AddDbContext<AuthServiceContext>(opt =>
                opt.UseSqlServer(conf.GetConnectionString("Auth"), b => b.MigrationsAssembly("AuthService.API"))
               // .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking));
               );

            return services;
        }

    }
}
