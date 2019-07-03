using AuthService.CORE.EF;
using AuthService.CORE.Interfaces;
using AuthService.CORE.Services;
using AuthService.DATA.Enteties;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.Swagger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthService.API.Configurations
{
    public static class ServicesConfiguration
    {
        public static IServiceCollection AddCorsConfiguration(this IServiceCollection services) =>
            services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", new Microsoft.AspNetCore.Cors.Infrastructure.CorsPolicyBuilder()
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowAnyOrigin()
                    .AllowCredentials()
                    .Build());
            });

        public static IServiceCollection AddSwagger(this IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("HugeHeart", new Info { Title = "Huge Heart Auth Service", Version = "1.0" });
            });

            return services;
        }


        public static IServiceCollection AddServices(this IServiceCollection services)
        {
            services
                .AddTransient<IJwtGenerator, JwtGenerator>()
                .AddTransient<IAuthService, AuthService.CORE.Services.AuthService>();

            return services;
        }

        public static IServiceCollection ConfigureIdentity(this IServiceCollection services)
        {
            services.AddIdentity<User, IdentityRole<Guid>>(o =>
            {
                o.Password.RequireNonAlphanumeric = false;
                o.Password.RequireUppercase = false;
                o.Password.RequireLowercase = false;
            })
                .AddEntityFrameworkStores<AuthServiceContext>();

            return services;
        }

    }
}
