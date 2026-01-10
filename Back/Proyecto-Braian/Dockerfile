# Etapa base: runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# Etapa de build
FROM mcr.microsoft.com/dotnet/sdk.Web:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /ProyectoBraian

# Copiar solo los archivos .csproj primero para aprovechar cache
COPY ["ProyectoBraian.Application.csproj", "ProyectoBraian.Application/"]
COPY ["ProyectoBraian.Infrastructure.csproj", "ProyectoBraian.Infrastructure/"]
COPY ["ProyectoBraian.Domain.csproj", "ProyectoBraian.Domain/"]
COPY ["Proyecto-Braian/ProyectoBraian/ProyectoBraian.csproj", "ProyectoBraian.ProyectoBraian/"]

# Restaurar paquetes
RUN dotnet restore "ProyectoBraian.Application/Application.csproj"
RUN dotnet restore "ProyectoBraian.Infrastructure/Infrastructure.csproj"
RUN dotnet restore "ProyectoBraian.ProyectoBraian/ProyectoBraian.csproj"
RUN dotnet restore "ProyectoBraian.Domain/Domain.csproj"

# Copiar todo el código
COPY . .

# Construir proyecto Web
WORKDIR "/ProyectoBraian/ProyectoBraian.ProyectoBraian"
RUN dotnet build "ProyectoBraian.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Publicar
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "ProyectoBraian.csproj" -c $BUILD_CONFIGURATION -o /app/publish

# Imagen final
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ProyectoBraian.dll"]