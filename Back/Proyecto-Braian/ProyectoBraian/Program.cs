using Application.Extensions;
using Infrastructure.Context;
using Infrastructure.Extensions;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// 1. CONFIGURACIÓN DEL PUERTO (Vital para Railway)
// Leemos la variable PORT que asigna Railway, si no existe usamos 8080
var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";

// Forzamos a Kestrel (el servidor de .NET) a escuchar en todas las interfaces (0.0.0.0)
builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(int.Parse(port));
});

try
{
    Console.WriteLine($"--- 🏗️ INICIANDO CONFIGURACIÓN DE SERVICIOS (Puerto: {port}) ---");

    // 2. REGISTRO DE SERVICIOS BÁSICOS
    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    // 3. CONFIGURACIÓN DE CORS
    builder.Services.AddCors(options =>
    {
        options.AddPolicy("AllowFrontend", policy =>
            policy.WithOrigins(
                    "http://localhost:5173",
                    "https://cobacha-60edd35ba-facundosolaris-projects.vercel.app",
                    "https://proyectoweb.up.railway.app"
                )
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials());
    });

    // 4. SERVICIOS DE CAPAS (Infraestructura y Aplicación)
    Console.WriteLine("🔌 Registrando servicios de infraestructura...");
    builder.Services.AddInfrastructureServices(builder.Configuration);

    builder.Services.AddApplicationServices();
    builder.Services.AddHttpContextAccessor();
    builder.Services.AddJwtAuthentication(builder.Configuration);
    builder.Services.AddHealthChecks();

    var app = builder.Build();

    // ---------------------------------------------------------
    // MIDDLEWARE - EL ORDEN ES IMPORTANTE
    // ---------------------------------------------------------

    // Healthcheck rápido para que Railway no mate el contenedor
    app.MapHealthChecks("/health");

    // Swagger habilitado en TODOS los entornos para pruebas en la nube
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Proyecto API v1");
        c.RoutePrefix = string.Empty; // Swagger cargará en la raíz: https://dominio.com/
    });

    app.UseCors("AllowFrontend");

    // No usamos HttpsRedirection en Railway porque ellos manejan el SSL externamente
    if (app.Environment.IsDevelopment())
    {
        app.UseHttpsRedirection();
    }

    app.UseStaticFiles();
    app.UseAuthentication();
    app.UseAuthorization();
    app.MapControllers();

    Console.WriteLine($"🚀 APLICACIÓN LISTA Y ESCUCHANDO EN: {port}");
    app.Run();
}
catch (Exception ex)
{
    Console.WriteLine("💀 ERROR CRÍTICO EN EL ARRANQUE:");
    Console.WriteLine(ex.Message);
    if (ex.InnerException != null)
        Console.WriteLine($"Inner Exception: {ex.InnerException.Message}");

    Console.WriteLine(ex.StackTrace);
    throw;
}