FROM mcr.microsoft.com/dotnet/runtime:9.0@sha256:9563e814250fef5dde24cd68916878ef5faa8f993bf99a06db1c947f23217996 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:9.0@sha256:faa2daf2b72cbe787ee1882d9651fa4ef3e938ee56792b8324516f5a448f3abe AS build
WORKDIR /src

COPY ["Directory.Build.props", "."]
COPY ["Applications/AdminCli/src/AdminCli/AdminCli.csproj", "Applications/AdminCli/src/AdminCli/"]

COPY ["Applications/AdminApi/src/AdminApi.Infrastructure/AdminApi.Infrastructure.csproj", "Applications/AdminApi/src/AdminApi.Infrastructure/"]
COPY ["Applications/AdminApi/src/AdminApi.Infrastructure.Database.Postgres/AdminApi.Infrastructure.Database.Postgres.csproj", "Applications/AdminApi/src/AdminApi.Infrastructure.Database.Postgres/"]
COPY ["Applications/AdminApi/src/AdminApi.Infrastructure.Database.SqlServer/AdminApi.Infrastructure.Database.SqlServer.csproj", "Applications/AdminApi/src/AdminApi.Infrastructure.Database.SqlServer/"]


COPY ["BuildingBlocks/src/BuildingBlocks.Domain/BuildingBlocks.Domain.csproj", "BuildingBlocks/src/BuildingBlocks.Domain/"]
COPY ["BuildingBlocks/src/BuildingBlocks.Infrastructure/BuildingBlocks.Infrastructure.csproj", "BuildingBlocks/src/BuildingBlocks.Infrastructure/"]
COPY ["BuildingBlocks/src/BuildingBlocks.API/BuildingBlocks.API.csproj", "BuildingBlocks/src/BuildingBlocks.API/"]
COPY ["BuildingBlocks/src/BuildingBlocks.Application.Abstractions/BuildingBlocks.Application.Abstractions.csproj", "BuildingBlocks/src/BuildingBlocks.Application.Abstractions/"]
COPY ["BuildingBlocks/src/BuildingBlocks.Application/BuildingBlocks.Application.csproj", "BuildingBlocks/src/BuildingBlocks.Application/"]
COPY ["BuildingBlocks/src/BuildingBlocks.Module/BuildingBlocks.Module.csproj", "BuildingBlocks/src/BuildingBlocks.Module/"]
COPY ["BuildingBlocks/src/DevelopmentKit.Identity/DevelopmentKit.Identity.csproj", "BuildingBlocks/src/DevelopmentKit.Identity/"]
COPY ["BuildingBlocks/src/StronglyTypedIds/StronglyTypedIds.csproj", "BuildingBlocks/src/StronglyTypedIds/"]
COPY ["BuildingBlocks/src/Tooling/Tooling.csproj", "BuildingBlocks/src/Tooling/"]
COPY ["BuildingBlocks/src/Crypto/Crypto.csproj", "BuildingBlocks/src/Crypto/"]

COPY ["Common/src/Common.Infrastructure/Common.Infrastructure.csproj", "Common/src/Common.Infrastructure/"]

COPY ["Modules/Announcements/src/Announcements.Application/Announcements.Application.csproj", "Modules/Announcements/src/Announcements.Application/"]
COPY ["Modules/Announcements/src/Announcements.Infrastructure/Announcements.Infrastructure.csproj", "Modules/Announcements/src/Announcements.Infrastructure/"]
COPY ["Modules/Announcements/src/Announcements.Infrastructure.Database.Postgres/Announcements.Infrastructure.Database.Postgres.csproj", "Modules/Announcements/src/Announcements.Infrastructure.Database.Postgres/"]
COPY ["Modules/Announcements/src/Announcements.Infrastructure.Database.SqlServer/Announcements.Infrastructure.Database.SqlServer.csproj", "Modules/Announcements/src/Announcements.Infrastructure.Database.SqlServer/"]
COPY ["Modules/Announcements/src/Announcements.Domain/Announcements.Domain.csproj", "Modules/Announcements/src/Announcements.Domain/"]
COPY ["Modules/Announcements/src/Announcements.Module/Announcements.Module.csproj", "Modules/Announcements/src/Announcements.Module/"]

COPY ["Modules/Devices/src/Devices.Application/Devices.Application.csproj", "Modules/Devices/src/Devices.Application/"]
COPY ["Modules/Devices/src/Devices.Infrastructure/Devices.Infrastructure.csproj", "Modules/Devices/src/Devices.Infrastructure/"]
COPY ["Modules/Devices/src/Devices.Domain/Devices.Domain.csproj", "Modules/Devices/src/Devices.Domain/"]
COPY ["Modules/Devices/src/Devices.Module/Devices.Module.csproj", "Modules/Devices/src/Devices.Module/"]
COPY ["Modules/Devices/src/Devices.Infrastructure.Database.Postgres/Devices.Infrastructure.Database.Postgres.csproj", "Modules/Devices/src/Devices.Infrastructure.Database.Postgres/"]
COPY ["Modules/Devices/src/Devices.Infrastructure.Database.SqlServer/Devices.Infrastructure.Database.SqlServer.csproj", "Modules/Devices/src/Devices.Infrastructure.Database.SqlServer/"]

COPY ["Modules/Tokens/src/Tokens.Application/Tokens.Application.csproj", "Modules/Tokens/src/Tokens.Application/"]
COPY ["Modules/Tokens/src/Tokens.Infrastructure/Tokens.Infrastructure.csproj", "Modules/Tokens/src/Tokens.Infrastructure/"]
COPY ["Modules/Tokens/src/Tokens.Domain/Tokens.Domain.csproj", "Modules/Tokens/src/Tokens.Domain/"]
COPY ["Modules/Tokens/src/Tokens.Module/Tokens.Module.csproj", "Modules/Tokens/src/Tokens.Module/"]
COPY ["Modules/Tokens/src/Tokens.Infrastructure.Database.Postgres/Tokens.Infrastructure.Database.Postgres.csproj", "Modules/Tokens/src/Tokens.Infrastructure.Database.Postgres/"]
COPY ["Modules/Tokens/src/Tokens.Infrastructure.Database.SqlServer/Tokens.Infrastructure.Database.SqlServer.csproj", "Modules/Tokens/src/Tokens.Infrastructure.Database.SqlServer/"]


RUN dotnet restore /p:ContinuousIntegrationBuild=true "Applications/AdminCli/src/AdminCli/AdminCli.csproj"

COPY . .

ARG VERSION

WORKDIR "/src/Applications/AdminCli/src/AdminCli"
RUN dotnet publish /p:ContinuousIntegrationBuild=true --no-restore -c Release -o /app/publish "AdminCli.csproj"

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish/Backbone.AdminCli ./backbone

# Test
COPY Applications/AdminCli/src/AdminCli/appsettings.json .

ENV PATH="$PATH:/app"

LABEL org.opencontainers.image.source="https://github.com/nmshd/backbone"
