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
                var connectionString =
                    Environment.GetEnvironmentVariable("DATABASE_URL")
                    ?? configuration.GetConnectionString("DefaultConnection");

                if (string.IsNullOrWhiteSpace(connectionString))
                    throw new Exception("Connection string no encontrada");

                options.UseMySql(
                    connectionString,
                    new MySqlServerVersion(new Version(8, 0, 36))
                );
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