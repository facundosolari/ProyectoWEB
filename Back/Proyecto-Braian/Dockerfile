# =========================
# Etapa 1: Build
# =========================
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copiar solo la solución y los proyectos para un restore rápido
COPY *.sln ./
COPY Domain/*.csproj ./Domain/
COPY Application/*.csproj ./Application/
COPY Infrastructure/*.csproj ./Infrastructure/
COPY ProyectoBraian/*.csproj ./ProyectoBraian/

# Restaurar dependencias
RUN dotnet restore "ProyectoBraian/ProyectoBraian.csproj"

# Copiar todo el código fuente
COPY . .

# Compilar la app en modo Release (produce la DLL final en /app/publish)
RUN dotnet publish "ProyectoBraian/ProyectoBraian.csproj" -c Release -o /app/publish

# =========================
# Etapa 2: Runtime
# =========================
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

# Comando de arranque
ENTRYPOINT ["dotnet", "ProyectoBraian.dll"]