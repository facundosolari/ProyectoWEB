# =========================
# Etapa 1: Build
# =========================
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copiar solución y proyectos
COPY Proyecto-Braian.sln ./
COPY Domain/Domain.csproj Domain/
COPY Application/Application.csproj Application/
COPY Infrastructure/Infrastructure.csproj Infrastructure/
COPY ProyectoBraian/ProyectoBraian.csproj ProyectoBraian/

# Restaurar paquetes NuGet
RUN dotnet restore ProyectoBraian/ProyectoBraian.csproj

# Copiar todo el código
COPY . .

# Publicar la app en Release
RUN dotnet publish ProyectoBraian/ProyectoBraian.csproj -c Debug -o /app/publish

# =========================
# Etapa 2: Runtime
# =========================
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

# Copiar la app publicada desde la etapa de build
COPY --from=build /app/publish .

# Configuración de Railway: escuchar en el puerto dinámico
ENV ASPNETCORE_URLS=http://+:8080

# Comando de arranque
ENTRYPOINT ["dotnet", "ProyectoBraian.dll"]