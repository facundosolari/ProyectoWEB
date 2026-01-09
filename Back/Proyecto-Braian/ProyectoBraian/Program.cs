using Application.Extensions;
using Infrastructure.Extensions;

try
{
    var builder = WebApplication.CreateBuilder(args);

    // ----------------------------
    // BLOQUE DE SERVICIOS
    try
    {
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

        var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
        builder.WebHost.UseUrls($"http://0.0.0.0:{port}");
    }
    catch (Exception ex)
    {
        Console.WriteLine("❌ Error en configuración de servicios:");
        Console.WriteLine(ex.Message);
        Console.WriteLine(ex.StackTrace);
    }

    var app = builder.Build();

    // ----------------------------
    // BLOQUE DE MIDDLEWARE
    try
    {
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "Proyecto API v1");
            c.RoutePrefix = "api-docs";
        });

        app.UseCors("AllowFrontend");
        app.UseStaticFiles();

        app.UseAuthentication();
        app.UseAuthorization();
    }
    catch (Exception ex)
    {
        Console.WriteLine("❌ Error en configuración de middleware:");
        Console.WriteLine(ex.Message);
        Console.WriteLine(ex.StackTrace);
    }

    // ----------------------------
    // RUTAS
    app.MapGet("/health", (ILogger<Program> log) =>
    {
        log.LogInformation("Healthcheck requested at {time}", DateTime.UtcNow);
        return Results.Ok("Healthy");
    });

    app.MapControllers();

    // ----------------------------
    try
    {
        app.Run();
    }
    catch (Exception ex)
    {
        Console.WriteLine("❌ Error al iniciar la app:");
        Console.WriteLine(ex.Message);
        Console.WriteLine(ex.StackTrace);
    }
}
catch (Exception ex)
{
    Console.WriteLine("💀 ERROR CRÍTICO EN ARRANQUE:");
    Console.WriteLine(ex.Message);
    Console.WriteLine(ex.StackTrace);
}

// Loggear el error crítico

// Environment.Exit(1); // Termina el proceso con código 1


// ----------------------------
// Abrir Swagger automáticamente solo en local
/*
if (port == 5000)
{
    var url = $"http://localhost:{port}/api-docs";
    try
    {
        var psi = new System.Diagnostics.ProcessStartInfo
        {
            FileName = url,
            UseShellExecute = true
        };
        System.Diagnostics.Process.Start(psi);
    }
    catch (Exception ex)
    {
        Console.WriteLine("No se pudo abrir el navegador automáticamente: " + ex.Message);
    }
}
*/

// ----------------------------
// Ejecutar
