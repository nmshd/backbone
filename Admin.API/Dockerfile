#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["Admin.API/Admin.API.csproj", "Admin.API/"]
COPY ["Modules/Devices/src/Devices.Application/Devices.Application.csproj", "Modules/Devices/src/Devices.Application/"]
COPY ["BuildingBlocks/src/BuildingBlocks.Application.Abstractions/BuildingBlocks.Application.Abstractions.csproj", "BuildingBlocks/src/BuildingBlocks.Application.Abstractions/"]
COPY ["BuildingBlocks/src/DevelopmentKit.Identity/DevelopmentKit.Identity.csproj", "BuildingBlocks/src/DevelopmentKit.Identity/"]
COPY ["BuildingBlocks/src/BuildingBlocks.Domain/BuildingBlocks.Domain.csproj", "BuildingBlocks/src/BuildingBlocks.Domain/"]
COPY ["BuildingBlocks/src/Tooling/Tooling.csproj", "BuildingBlocks/src/Tooling/"]
COPY ["BuildingBlocks/src/BuildingBlocks.Application/BuildingBlocks.Application.csproj", "BuildingBlocks/src/BuildingBlocks.Application/"]
COPY ["BuildingBlocks/src/Crypto/Crypto.csproj", "BuildingBlocks/src/Crypto/"]
COPY ["Modules/Devices/src/Devices.Domain/Devices.Domain.csproj", "Modules/Devices/src/Devices.Domain/"]
COPY ["Modules/Devices/src/Devices.Infrastructure/Devices.Infrastructure.csproj", "Modules/Devices/src/Devices.Infrastructure/"]
COPY ["BuildingBlocks/src/BuildingBlocks.Infrastructure/BuildingBlocks.Infrastructure.csproj", "BuildingBlocks/src/BuildingBlocks.Infrastructure/"]
COPY ["Backbone.Infrastructure/Backbone.Infrastructure.csproj", "Backbone.Infrastructure/"]
RUN dotnet restore "Admin.API/Admin.API.csproj"
COPY . .
WORKDIR "/src/Admin.API"
RUN dotnet build "Admin.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Admin.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Admin.API.dll"]