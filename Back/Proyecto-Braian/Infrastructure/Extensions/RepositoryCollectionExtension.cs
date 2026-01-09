using Application.Interfaces;
using Domain.Interfaces;
using Infrastructure.Context;
using Infrastructure.Data;
using Infrastructure.ThirdServices;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Infrastructure.Extensions
{
    public static class ServiceCollectionExtension
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<DataBaseContext>(options =>
            {
                // Intentar usar variables de entorno (Railway) primero
                var host = Environment.GetEnvironmentVariable("MYSQLHOST") ?? configuration["ConnectionStrings:Host"] ?? "localhost";
                var db = Environment.GetEnvironmentVariable("MYSQLDATABASE") ?? configuration["ConnectionStrings:Database"] ?? "proyecto_braian";
                var user = Environment.GetEnvironmentVariable("MYSQLUSER") ?? configuration["ConnectionStrings:User"] ?? "root";
                var password = Environment.GetEnvironmentVariable("MYSQL_ROOT_PASSWORD") ?? configuration["ConnectionStrings:Password"] ?? "solariDev135!";
                var port = Environment.GetEnvironmentVariable("MYSQLPORT") ?? configuration["ConnectionStrings:Port"] ?? "3306";

                Console.WriteLine($"MYSQLHOST={host}");
                Console.WriteLine($"MYSQLDATABASE={db}");
                Console.WriteLine($"MYSQLUSER={user}");
                Console.WriteLine($"MYSQLPORT={port}");

                // Construir connection string
                var connectionString = $"Server={host};Database={db};User={user};Password={password};Port={port};";

                options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
            });

            // ----------------------------
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