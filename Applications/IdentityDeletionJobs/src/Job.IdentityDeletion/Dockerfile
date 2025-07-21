FROM mcr.microsoft.com/dotnet/aspnet:9.0-alpine@sha256:89a7a398c5acaa773642cfabd6456f33e29687c1529abfaf068929ff9991cb66 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:9.0-alpine@sha256:a4eb48407ea8a1a4af92ee6630ec91af216365fdf45e7f08e1b5f4ce602407f4 AS build
WORKDIR /src

COPY ["Directory.Build.props", "."]
COPY ["Applications/IdentityDeletionJobs/src/Job.IdentityDeletion/Job.IdentityDeletion.csproj", "Applications/IdentityDeletionJobs/src/Job.IdentityDeletion/"]
COPY ["BuildingBlocks/src/BuildingBlocks.API/BuildingBlocks.API.csproj", "BuildingBlocks/src/BuildingBlocks.API/"]
COPY ["BuildingBlocks/src/BuildingBlocks.Domain/BuildingBlocks.Domain.csproj", "BuildingBlocks/src/BuildingBlocks.Domain/"]
COPY ["BuildingBlocks/src/Tooling/Tooling.csproj", "BuildingBlocks/src/Tooling/"]
COPY ["BuildingBlocks/src/DevelopmentKit.Identity/DevelopmentKit.Identity.csproj", "BuildingBlocks/src/DevelopmentKit.Identity/"]
COPY ["BuildingBlocks/src/BuildingBlocks.Infrastructure/BuildingBlocks.Infrastructure.csproj", "BuildingBlocks/src/BuildingBlocks.Infrastructure/"]
COPY ["BuildingBlocks/src/BuildingBlocks.Application.Abstractions/BuildingBlocks.Application.Abstractions.csproj", "BuildingBlocks/src/BuildingBlocks.Application.Abstractions/"]
COPY ["BuildingBlocks/src/BuildingBlocks.Application/BuildingBlocks.Application.csproj", "BuildingBlocks/src/BuildingBlocks.Application/"]
COPY ["BuildingBlocks/src/BuildingBlocks.Module/BuildingBlocks.Module.csproj", "BuildingBlocks/src/BuildingBlocks.Module/"]
COPY ["Common/src/Common.Infrastructure/Common.Infrastructure.csproj", "Common/src/Common.Infrastructure/"]
COPY ["BuildingBlocks/src/Crypto/Crypto.csproj", "BuildingBlocks/src/Crypto/"]
COPY ["Modules/Announcements/src/Announcements.Domain/Announcements.Domain.csproj", "Modules/Announcements/src/Announcements.Domain/"]
COPY ["Modules/Announcements/src/Announcements.Application/Announcements.Application.csproj", "Modules/Announcements/src/Announcements.Application/"]	
COPY ["Modules/Announcements/src/Announcements.Module/Announcements.Module.csproj", "Modules/Announcements/src/Announcements.Module/"]
COPY ["Modules/Announcements/src/Announcements.Infrastructure.Database.Postgres/Announcements.Infrastructure.Database.Postgres.csproj", "Modules/Announcements/src/Announcements.Infrastructure.Database.Postgres/"]
COPY ["Modules/Announcements/src/Announcements.Infrastructure/Announcements.Infrastructure.csproj", "Modules/Announcements/src/Announcements.Infrastructure/"]
COPY ["Modules/Announcements/src/Announcements.Infrastructure.Database.SqlServer/Announcements.Infrastructure.Database.SqlServer.csproj", "Modules/Announcements/src/Announcements.Infrastructure.Database.SqlServer/"]
COPY ["Modules/Challenges/src/Challenges.Application/Challenges.Application.csproj", "Modules/Challenges/src/Challenges.Application/"]
COPY ["Modules/Challenges/src/Challenges.Domain/Challenges.Domain.csproj", "Modules/Challenges/src/Challenges.Domain/"]
COPY ["Modules/Challenges/src/Challenges.Module/Challenges.Module.csproj", "Modules/Challenges/src/Challenges.Module/"]
COPY ["Modules/Challenges/src/Challenges.Infrastructure.Database.Postgres/Challenges.Infrastructure.Database.Postgres.csproj", "Modules/Challenges/src/Challenges.Infrastructure.Database.Postgres/"]
COPY ["Modules/Challenges/src/Challenges.Infrastructure/Challenges.Infrastructure.csproj", "Modules/Challenges/src/Challenges.Infrastructure/"]
COPY ["Modules/Challenges/src/Challenges.Infrastructure.Database.SqlServer/Challenges.Infrastructure.Database.SqlServer.csproj", "Modules/Challenges/src/Challenges.Infrastructure.Database.SqlServer/"]
COPY ["Modules/Devices/src/Devices.Domain/Devices.Domain.csproj", "Modules/Devices/src/Devices.Domain/"]
COPY ["Modules/Devices/src/Devices.Infrastructure/Devices.Infrastructure.csproj", "Modules/Devices/src/Devices.Infrastructure/"]
COPY ["Modules/Devices/src/Devices.Application/Devices.Application.csproj", "Modules/Devices/src/Devices.Application/"]
COPY ["Modules/Devices/src/Devices.Module/Devices.Module.csproj", "Modules/Devices/src/Devices.Module/"]
COPY ["Modules/Devices/src/Devices.Infrastructure.Database.Postgres/Devices.Infrastructure.Database.Postgres.csproj", "Modules/Devices/src/Devices.Infrastructure.Database.Postgres/"]
COPY ["Modules/Devices/src/Devices.Infrastructure.Database.SqlServer/Devices.Infrastructure.Database.SqlServer.csproj", "Modules/Devices/src/Devices.Infrastructure.Database.SqlServer/"]
COPY ["Modules/Files/src/Files.Application/Files.Application.csproj", "Modules/Files/src/Files.Application/"]
COPY ["Modules/Files/src/Files.Domain/Files.Domain.csproj", "Modules/Files/src/Files.Domain/"]
COPY ["Modules/Files/src/Files.Module/Files.Module.csproj", "Modules/Files/src/Files.Module/"]
COPY ["Modules/Files/src/Files.Infrastructure.Database.Postgres/Files.Infrastructure.Database.Postgres.csproj", "Modules/Files/src/Files.Infrastructure.Database.Postgres/"]
COPY ["Modules/Files/src/Files.Infrastructure/Files.Infrastructure.csproj", "Modules/Files/src/Files.Infrastructure/"]
COPY ["Modules/Files/src/Files.Infrastructure.Database.SqlServer/Files.Infrastructure.Database.SqlServer.csproj", "Modules/Files/src/Files.Infrastructure.Database.SqlServer/"]
COPY ["Modules/Messages/src/Messages.Application/Messages.Application.csproj", "Modules/Messages/src/Messages.Application/"]
COPY ["Modules/Messages/src/Messages.Domain/Messages.Domain.csproj", "Modules/Messages/src/Messages.Domain/"]
COPY ["Modules/Messages/src/Messages.Module/Messages.Module.csproj", "Modules/Messages/src/Messages.Module/"]
COPY ["Modules/Messages/src/Messages.Infrastructure.Database.Postgres/Messages.Infrastructure.Database.Postgres.csproj", "Modules/Messages/src/Messages.Infrastructure.Database.Postgres/"]
COPY ["Modules/Messages/src/Messages.Infrastructure/Messages.Infrastructure.csproj", "Modules/Messages/src/Messages.Infrastructure/"]
COPY ["Modules/Messages/src/Messages.Infrastructure.Database.SqlServer/Messages.Infrastructure.Database.SqlServer.csproj", "Modules/Messages/src/Messages.Infrastructure.Database.SqlServer/"]
COPY ["Modules/Quotas/src/Quotas.Application/Quotas.Application.csproj", "Modules/Quotas/src/Quotas.Application/"]
COPY ["Modules/Quotas/src/Quotas.Domain/Quotas.Domain.csproj", "Modules/Quotas/src/Quotas.Domain/"]
COPY ["Modules/Quotas/src/Quotas.Module/Quotas.Module.csproj", "Modules/Quotas/src/Quotas.Module/"]
COPY ["Modules/Quotas/src/Quotas.Infrastructure.Database.Postgres/Quotas.Infrastructure.Database.Postgres.csproj", "Modules/Quotas/src/Quotas.Infrastructure.Database.Postgres/"]
COPY ["Modules/Quotas/src/Quotas.Infrastructure/Quotas.Infrastructure.csproj", "Modules/Quotas/src/Quotas.Infrastructure/"]
COPY ["Modules/Quotas/src/Quotas.Infrastructure.Database.SqlServer/Quotas.Infrastructure.Database.SqlServer.csproj", "Modules/Quotas/src/Quotas.Infrastructure.Database.SqlServer/"]
COPY ["Modules/Relationships/src/Relationships.Application/Relationships.Application.csproj", "Modules/Relationships/src/Relationships.Application/"]
COPY ["Modules/Relationships/src/Relationships.Common/Relationships.Common.csproj", "Modules/Relationships/src/Relationships.Common/"]
COPY ["Modules/Relationships/src/Relationships.Domain/Relationships.Domain.csproj", "Modules/Relationships/src/Relationships.Domain/"]
COPY ["Modules/Relationships/src/Relationships.Module/Relationships.Module.csproj", "Modules/Relationships/src/Relationships.Module/"]
COPY ["Modules/Relationships/src/Relationships.Infrastructure.Database.Postgres/Relationships.Infrastructure.Database.Postgres.csproj", "Modules/Relationships/src/Relationships.Infrastructure.Database.Postgres/"]
COPY ["Modules/Relationships/src/Relationships.Infrastructure/Relationships.Infrastructure.csproj", "Modules/Relationships/src/Relationships.Infrastructure/"]
COPY ["Modules/Relationships/src/Relationships.Infrastructure.Database.SqlServer/Relationships.Infrastructure.Database.SqlServer.csproj", "Modules/Relationships/src/Relationships.Infrastructure.Database.SqlServer/"]
COPY ["Modules/Synchronization/src/Synchronization.Application/Synchronization.Application.csproj", "Modules/Synchronization/src/Synchronization.Application/"]
COPY ["Modules/Synchronization/src/Synchronization.Domain/Synchronization.Domain.csproj", "Modules/Synchronization/src/Synchronization.Domain/"]
COPY ["Modules/Synchronization/src/Synchronization.Module/Synchronization.Module.csproj", "Modules/Synchronization/src/Synchronization.Module/"]
COPY ["Modules/Synchronization/src/Synchronization.Infrastructure.Database.Postgres/Synchronization.Infrastructure.Database.Postgres.csproj", "Modules/Synchronization/src/Synchronization.Infrastructure.Database.Postgres/"]
COPY ["Modules/Synchronization/src/Synchronization.Infrastructure/Synchronization.Infrastructure.csproj", "Modules/Synchronization/src/Synchronization.Infrastructure/"]
COPY ["Modules/Synchronization/src/Synchronization.Infrastructure.Database.SqlServer/Synchronization.Infrastructure.Database.SqlServer.csproj", "Modules/Synchronization/src/Synchronization.Infrastructure.Database.SqlServer/"]
COPY ["Modules/Tokens/src/Tokens.Application/Tokens.Application.csproj", "Modules/Tokens/src/Tokens.Application/"]
COPY ["Modules/Tokens/src/Tokens.Domain/Tokens.Domain.csproj", "Modules/Tokens/src/Tokens.Domain/"]
COPY ["Modules/Tokens/src/Tokens.Module/Tokens.Module.csproj", "Modules/Tokens/src/Tokens.Module/"]
COPY ["Modules/Tokens/src/Tokens.Infrastructure.Database.Postgres/Tokens.Infrastructure.Database.Postgres.csproj", "Modules/Tokens/src/Tokens.Infrastructure.Database.Postgres/"]
COPY ["Modules/Tokens/src/Tokens.Infrastructure/Tokens.Infrastructure.csproj", "Modules/Tokens/src/Tokens.Infrastructure/"]
COPY ["Modules/Tokens/src/Tokens.Infrastructure.Database.SqlServer/Tokens.Infrastructure.Database.SqlServer.csproj", "Modules/Tokens/src/Tokens.Infrastructure.Database.SqlServer/"]

RUN dotnet restore /p:ContinuousIntegrationBuild=true "Applications/IdentityDeletionJobs/src/Job.IdentityDeletion/Job.IdentityDeletion.csproj"

COPY . .

ARG VERSION

WORKDIR "/src/Applications/IdentityDeletionJobs/src/Job.IdentityDeletion"
RUN dotnet build /p:ContinuousIntegrationBuild=true "Job.IdentityDeletion.csproj" -c Release --output /app/build --no-restore

FROM build AS publish
RUN dotnet publish /p:ContinuousIntegrationBuild=true -c Release --output /app/publish --no-restore "Job.IdentityDeletion.csproj" 

FROM base AS final

RUN apk add icu-libs
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=0

WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Backbone.Job.IdentityDeletion.dll"]
