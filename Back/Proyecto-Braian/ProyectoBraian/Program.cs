using Application.Extensions;
using Infrastructure.Extensions;

var builder = WebApplication.CreateBuilder(args);

// ----------------------------
// SERVICIOS
// ----------------------------
builder.Services.AddControllers();

// CORS para frontend (cookies necesitan AllowCredentials)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend",
        policy => policy
            .WithOrigins(
                "http://localhost:5173", // desarrollo
                "https://cobacha-60edd35ba-facundosolaris-projects.vercel.app", // prod Vercel
                "https://proyectoweb.up.railway.app" // prod Railway
            )
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials() // muy importante para cookies HttpOnly
    );
});

// Infraestructura y servicios
builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddApplicationServices();
builder.Services.AddHttpContextAccessor();

// JWT Authentication
builder.Services.AddJwtAuthentication(builder.Configuration);

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// ----------------------------
// PIPELINE DE MIDDLEWARE
// ----------------------------
/*
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
*/

app.UseSwagger();
app.UseSwaggerUI();

// ✅ Aplicar CORS ANTES de los controladores y autorización
app.UseCors("AllowFrontend");

app.UseStaticFiles();

// HTTPS
app.UseHttpsRedirection();

// Autenticación y autorización
app.UseAuthentication();
app.UseAuthorization();

// Mapear controladores

app.MapGet("/", () => "API online 🚀");
app.MapControllers();

// ----------------------------
// Escuchar en el puerto dinámico de Railway
// ----------------------------
var port = Environment.GetEnvironmentVariable("PORT") ?? "5000";
app.Urls.Add($"http://*:{port}");
Console.WriteLine($"Listening on port {port}");

// Ejecutar app
app.Run();
