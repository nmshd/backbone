FROM mcr.microsoft.com/dotnet/runtime:9.0@sha256:d067453026c556115c214e81d4f93adaa190487fad6d7ef4b539eab6f8103d2e AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:9.0@sha256:c5b188baf837b0180a14f988815b9cc7a55b836dd6cbe1b6e6523cf3098faaa8 AS build
WORKDIR /src

COPY ["Directory.Build.props", "."]
COPY ["Applications/AdminCli/src/AdminCli/AdminCli.csproj", "Applications/AdminCli/src/AdminCli/"]
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

COPY ["Modules/Files/src/Files.Application/Files.Application.csproj", "Modules/Files/src/Files.Application/"]
COPY ["Modules/Files/src/Files.Infrastructure/Files.Infrastructure.csproj", "Modules/Files/src/Files.Infrastructure/"]
COPY ["Modules/Files/src/Files.Domain/Files.Domain.csproj", "Modules/Files/src/Files.Domain/"]
COPY ["Modules/Files/src/Files.Module/Files.Module.csproj", "Modules/Files/src/Files.Module/"]
COPY ["Modules/Files/src/Files.Infrastructure.Database.Postgres/Files.Infrastructure.Database.Postgres.csproj", "Modules/Files/src/Files.Infrastructure.Database.Postgres/"]
COPY ["Modules/Files/src/Files.Infrastructure.Database.SqlServer/Files.Infrastructure.Database.SqlServer.csproj", "Modules/Files/src/Files.Infrastructure.Database.SqlServer/"]

COPY ["Modules/Messages/src/Messages.Application/Messages.Application.csproj", "Modules/Messages/src/Messages.Application/"]
COPY ["Modules/Messages/src/Messages.Infrastructure/Messages.Infrastructure.csproj", "Modules/Messages/src/Messages.Infrastructure/"]
COPY ["Modules/Messages/src/Messages.Common/Messages.Common.csproj", "Modules/Messages/src/Messages.Common/"]
COPY ["Modules/Messages/src/Messages.Domain/Messages.Domain.csproj", "Modules/Messages/src/Messages.Domain/"]
COPY ["Modules/Messages/src/Messages.Module/Messages.Module.csproj", "Modules/Messages/src/Messages.Module/"]
COPY ["Modules/Messages/src/Messages.Infrastructure.Database.Postgres/Messages.Infrastructure.Database.Postgres.csproj", "Modules/Messages/src/Messages.Infrastructure.Database.Postgres/"]
COPY ["Modules/Messages/src/Messages.Infrastructure.Database.SqlServer/Messages.Infrastructure.Database.SqlServer.csproj", "Modules/Messages/src/Messages.Infrastructure.Database.SqlServer/"]

COPY ["Modules/Relationships/src/Relationships.Application/Relationships.Application.csproj", "Modules/Relationships/src/Relationships.Application/"]
COPY ["Modules/Relationships/src/Relationships.Infrastructure/Relationships.Infrastructure.csproj", "Modules/Relationships/src/Relationships.Infrastructure/"]
COPY ["Modules/Relationships/src/Relationships.Common/Relationships.Common.csproj", "Modules/Relationships/src/Relationships.Common/"]
COPY ["Modules/Relationships/src/Relationships.Domain/Relationships.Domain.csproj", "Modules/Relationships/src/Relationships.Domain/"]
COPY ["Modules/Relationships/src/Relationships.Module/Relationships.Module.csproj", "Modules/Relationships/src/Relationships.Module/"]
COPY ["Modules/Relationships/src/Relationships.Infrastructure.Database.Postgres/Relationships.Infrastructure.Database.Postgres.csproj", "Modules/Relationships/src/Relationships.Infrastructure.Database.Postgres/"]
COPY ["Modules/Relationships/src/Relationships.Infrastructure.Database.SqlServer/Relationships.Infrastructure.Database.SqlServer.csproj", "Modules/Relationships/src/Relationships.Infrastructure.Database.SqlServer/"]

COPY ["Modules/Synchronization/src/Synchronization.Application/Synchronization.Application.csproj", "Modules/Synchronization/src/Synchronization.Application/"]
COPY ["Modules/Synchronization/src/Synchronization.Infrastructure/Synchronization.Infrastructure.csproj", "Modules/Synchronization/src/Synchronization.Infrastructure/"]
COPY ["Modules/Synchronization/src/Synchronization.Domain/Synchronization.Domain.csproj", "Modules/Synchronization/src/Synchronization.Domain/"]
COPY ["Modules/Synchronization/src/Synchronization.Module/Synchronization.Module.csproj", "Modules/Synchronization/src/Synchronization.Module/"]
COPY ["Modules/Synchronization/src/Synchronization.Infrastructure.Database.Postgres/Synchronization.Infrastructure.Database.Postgres.csproj", "Modules/Synchronization/src/Synchronization.Infrastructure.Database.Postgres/"]
COPY ["Modules/Synchronization/src/Synchronization.Infrastructure.Database.SqlServer/Synchronization.Infrastructure.Database.SqlServer.csproj", "Modules/Synchronization/src/Synchronization.Infrastructure.Database.SqlServer/"]

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
