FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /src
COPY ["Directory.Build.props", "."]
COPY ["Modules/Directory.Build.props", "Modules/"]
COPY ["AdminUi/src/AdminUi/AdminUi.csproj", "AdminUi/src/AdminUi/"]
COPY ["BuildingBlocks/src/BuildingBlocks.API/BuildingBlocks.API.csproj", "BuildingBlocks/src/BuildingBlocks.API/"]
COPY ["Modules/Devices/src/Devices.Domain/Devices.Domain.csproj", "Modules/Devices/src/Devices.Domain/"]
COPY ["BuildingBlocks/src/BuildingBlocks.Domain/BuildingBlocks.Domain.csproj", "BuildingBlocks/src/BuildingBlocks.Domain/"]
COPY ["BuildingBlocks/src/Tooling/Tooling.csproj", "BuildingBlocks/src/Tooling/"]
COPY ["BuildingBlocks/src/DevelopmentKit.Identity/DevelopmentKit.Identity.csproj", "BuildingBlocks/src/DevelopmentKit.Identity/"]
COPY ["Modules/Devices/src/Devices.Infrastructure/Devices.Infrastructure.csproj", "Modules/Devices/src/Devices.Infrastructure/"]
COPY ["BuildingBlocks/src/BuildingBlocks.Infrastructure/BuildingBlocks.Infrastructure.csproj", "BuildingBlocks/src/BuildingBlocks.Infrastructure/"]
COPY ["BuildingBlocks/src/BuildingBlocks.Application.Abstractions/BuildingBlocks.Application.Abstractions.csproj", "BuildingBlocks/src/BuildingBlocks.Application.Abstractions/"]
COPY ["Modules/Devices/src/Devices.Application/Devices.Application.csproj", "Modules/Devices/src/Devices.Application/"]
COPY ["BuildingBlocks/src/BuildingBlocks.Application/BuildingBlocks.Application.csproj", "BuildingBlocks/src/BuildingBlocks.Application/"]
COPY ["Common/src/Common.Infrastructure/Common.Infrastructure.csproj", "Common/src/Common.Infrastructure/"]
COPY ["BuildingBlocks/src/Crypto/Crypto.csproj", "BuildingBlocks/src/Crypto/"]
COPY ["Infrastructure/Infrastructure.csproj", "Infrastructure/"]
COPY ["Modules/Quotas/src/Quotas.Application/Quotas.Application.csproj", "Modules/Quotas/src/Quotas.Application/"]
COPY ["Modules/Quotas/src/Quotas.Domain/Quotas.Domain.csproj", "Modules/Quotas/src/Quotas.Domain/"]
COPY ["Modules/Quotas/src/Quotas.Infrastructure/Quotas.Infrastructure.csproj", "Modules/Quotas/src/Quotas.Infrastructure/"]
COPY ["AdminUi/src/AdminUi.Infrastructure.Database.Postgres/AdminUi.Infrastructure.Database.Postgres.csproj", "AdminUi/src/AdminUi.Infrastructure.Database.Postgres/"]
COPY ["AdminUi/src/AdminUi.Infrastructure/AdminUi.Infrastructure.csproj", "AdminUi/src/AdminUi.Infrastructure/"]
COPY ["AdminUi/src/AdminUi.Infrastructure.Database.SqlServer/AdminUi.Infrastructure.Database.SqlServer.csproj", "AdminUi/src/AdminUi.Infrastructure.Database.SqlServer/"]
RUN dotnet restore "AdminUi/src/AdminUi/AdminUi.csproj"
COPY . .
WORKDIR "/src/AdminUi/src/AdminUi"
RUN dotnet build /property:WarningLevel=0 --configuration Release --output /app/build --no-restore "AdminUi.csproj"

FROM build AS publish
RUN dotnet publish /property:WarningLevel=0 /p:UseAppHost=false --no-restore --configuration Release --output /app/publish "AdminUi.csproj"

FROM base AS final
ENV ASPNETCORE_PORT=80
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Backbone.AdminUi.dll"]
