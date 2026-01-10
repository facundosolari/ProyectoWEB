using Application.Extensions;
using Infrastructure.Context;
using Infrastructure.Extensions;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// 1. CONFIGURACIÓN DEL PUERTO (Crucial para Railway)
var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
builder.WebHost.UseUrls($"http://0.0.0.0:{port}");

var apiUrl = Environment.GetEnvironmentVariable("API_URL");
Console.WriteLine($"La API que se pasó al contenedor es: {apiUrl}");

try
{
    // ----------------------------
    // BLOQUE DE SERVICIOS
    // ----------------------------
    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    builder.Services.AddCors(options =>
    {
        options.AddPolicy("AllowFrontend",
            policy => policy
                .WithOrigins(
                    "http://localhost:5173",
                    "https://cobacha-60edd35ba-facundosolaris-projects.vercel.app",
                    "https://proyectoweb.up.railway.app"
                )
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials()
        );
    });

    builder.Services.AddInfrastructureServices(builder.Configuration);
    builder.Services.AddApplicationServices();
    builder.Services.AddHttpContextAccessor();
    builder.Services.AddJwtAuthentication(builder.Configuration);

    // Agregamos el servicio de HealthChecks nativo
    builder.Services.AddHealthChecks();

    var app = builder.Build();

    // ----------------------------
    // 1. RUTAS DE SALUD (PRIMERO QUE NADA)
    // ----------------------------
    // Importante: Antes que Auth para que Railway pueda entrar sin token
    app.MapGet("/health", (ILogger<Program> log) =>
    {
        log.LogInformation("Healthcheck manual solicitado");
        return Results.Ok(new { Status = "Healthy", Port = port });
    });

    app.MapHealthChecks("/health");

    // ----------------------------
    // 2. CONFIGURACIÓN DE SWAGGER
    // ----------------------------
    // Forzamos Swagger en producción para visualizar la API
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Proyecto API v1");
        c.RoutePrefix = string.Empty; // Carga Swagger en la raíz
    });

    // ----------------------------
    // 3. MIDDLEWARE DE SEGURIDAD Y RUTAS
    // ----------------------------
    app.UseCors("AllowFrontend");
    app.UseStaticFiles();

    app.UseAuthentication();
    app.UseAuthorization();

    app.MapControllers();

    Console.WriteLine($"🚀 App iniciando en el puerto: {port}...");
    app.Run();
}
catch (Exception ex)
{
    Console.WriteLine("💀 ERROR CRÍTICO EN ARRANQUE:");
    Console.WriteLine(ex.Message);
    if (ex.InnerException != null)
        Console.WriteLine($"Inner Exception: {ex.InnerException.Message}");

    Console.WriteLine(ex.StackTrace);
    throw;
}