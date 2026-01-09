using Application.Interfaces;
using Domain.Interfaces;
using Infrastructure.Context;
using Infrastructure.Data;
using Infrastructure.ThirdServices;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Extensions
{
    public static class ServiceCollectionExtension
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<DataBaseContext>(options =>
            {
                var host = Environment.GetEnvironmentVariable("MYSQLHOST");
                var db = Environment.GetEnvironmentVariable("MYSQLDATABASE");
                var user = Environment.GetEnvironmentVariable("MYSQLUSER");
                var password = Environment.GetEnvironmentVariable("MYSQL_ROOT_PASSWORD");
                var port = Environment.GetEnvironmentVariable("MYSQLPORT") ?? "3306";

                Console.WriteLine($"MYSQLHOST={host}");
                Console.WriteLine($"MYSQLDATABASE={db}");
                Console.WriteLine($"MYSQLUSER={user}");
                Console.WriteLine($"MYSQLPORT={port}");

                if (string.IsNullOrEmpty(host) || string.IsNullOrEmpty(db) ||
                    string.IsNullOrEmpty(user) || string.IsNullOrEmpty(password) ||
                    string.IsNullOrEmpty(port))
                {
                    throw new Exception("One or more MySQL environment variables are not set!");
                }

                var connectionString = $"Server={host};Database={db};User={user};Password={password};Port={port};";

                options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
            });

            // Repositorios
            services.AddScoped<IAuthenticationService, AuthenticationService>();
            services.AddScoped<IGoogleTokenValidator, GoogleTokenValidator>();
            services.AddScoped<IOrderMessageRepository, OrderMessageRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<IProductSizeRepository, ProductSizeRepository>();
            services.AddScoped<IOrderRepository, OrderRepository>();
            services.AddScoped<IOrderItemRepository, OrderItemRepository>();
            services.AddScoped<IAuditLogRepository, AuditLogRepository>();
            services.AddScoped<ICategoryRepository, CategoryRepository>();
            services.AddScoped<IDescuentoRepository, DescuentoRepository>();
            services.AddScoped<IReglaDescuentoRepository, ReglaDescuentoRepository>();

            return services;
        }
    }
}