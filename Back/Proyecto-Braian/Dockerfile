FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 as build
ARG BUILD_CONFIGURATION=Release
WORKDIR /ProyectoBraian
COPY ["ProyectoBraian.Application.csproj", "ProyectoBraian.Application"]
COPY ["ProyectoBraian.Infrastructure.csproj", "ProyectoBraian.Infrastructure"]
COPY ["ProyectoBraian.Domain.csproj", "ProyectoBraian.Domain"]
COPY ["ProyectoBraian.ProyectoBraian.csproj", "ProyectoBraian.ProyectoBraian"]
RUN dotnet restore "ProyectoBraian.Application/Application.csproj"
RUN dotnet restore "ProyectoBraian.Infrastructure/Infrastructure.csproj"
RUN dotnet restore "ProyectoBraian.ProyectoBraian/ProyectoBraian.csproj"
RUN dotnet restore "ProyectoBraian.Domain/Domain.csproj"
COPY . .
WORKDIR "/ProyectoBraian/ProyectoBraian"
RUN dotnet build "ProyectoBraian.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "ProyectoBraian.csproj" -c $BUILD_CONFIGURATION -o /app/publish = false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT [ "dotnet", "ProyectoBraian.dll"]