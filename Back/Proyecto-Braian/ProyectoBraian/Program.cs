using Application.Extensions;
using Infrastructure.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Puerto dinámico de Railway
var portStr = Environment.GetEnvironmentVariable("PORT");
if (!int.TryParse(portStr, out var port))
{
    throw new Exception("PORT environment variable is not set or invalid.");
}

builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(port);
});

// Servicios
builder.Services.AddControllers();
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

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Logging.ClearProviders();
builder.Logging.AddConsole();

var app = builder.Build();

// Middleware
app.UseSwagger();
app.UseSwaggerUI();

app.UseCors("AllowFrontend");

app.UseStaticFiles();

app.UseAuthentication();
app.UseAuthorization();

// Endpoints básicos
app.MapGet("/health", (ILogger<Program> log) =>
{
    log.LogInformation("Healthcheck requested at {time}", DateTime.UtcNow);
    return Results.Ok("Healthy");
});

app.MapGet("/", (ILogger<Program> log) =>
{
    log.LogInformation("Root endpoint requested at {time}", DateTime.UtcNow);
    return Results.Ok("API is running");
});

// Mapear controladores
app.MapControllers();

// Ejecutar
app.Run();