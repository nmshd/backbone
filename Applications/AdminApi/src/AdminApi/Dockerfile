#### Build chisel binary ####
################################################################################################################
# The following lines are required to install libicu, which itself is required for the SQL Server client to work 
# (see https://github.com/dotnet/dotnet-docker/discussions/4527).
# They are copied from https://github.com/ubuntu-rocks/dotnet/issues/21
################################################################################################################
FROM golang:1.24@sha256:81bf5927dc91aefb42e2bc3a5abdbe9bb3bae8ba8b107e2a4cf43ce3402534c6 AS chisel

RUN git clone --depth 1 -b main https://github.com/canonical/chisel /opt/chisel
WORKDIR /opt/chisel
RUN go build ./cmd/chisel

#### Build application ####
FROM mcr.microsoft.com/dotnet/sdk:9.0-noble@sha256:9f7bd4d010026e15a57d9cf876f2f7d08c3eeed6a0ea987b8c5ba8c75e68e948 AS build-env

# use chisel to install libicu
RUN apt-get update \
    && apt-get install -y fdupes \
    && rm -rf /var/lib/apt/lists/*

COPY --from=chisel /opt/chisel/chisel /usr/bin/
COPY --from=mcr.microsoft.com/dotnet/runtime:9.0-noble-chiseled@sha256:bd4288d187eac2d9753e4623e0466b9ceec2b340254a640858d3ebb1b25afbac / /runtime-ref

RUN mkdir /rootfs \
    && chisel cut --release "ubuntu-24.04" --root /rootfs \
    libicu74_libs \
    \
    # Remove duplicates from rootfs that exist in runtime-ref
    && fdupes /runtime-ref /rootfs -rdpN \
    \
    # Delete duplicate symlinks
    # Function to find and format symlinks w/o including root dir (format: /path/to/symlink /path/to/target)
    && getsymlinks() { find $1 -type l -printf '%p %l\n' | sed -n "s/^\\$1\\(.*\\)/\\1/p"; } \
    # Combine set of symlinks between rootfs and runtime-ref
    && (getsymlinks "/rootfs"; getsymlinks "/runtime-ref") \
    # Sort them
    | sort \
    # Find the duplicates
    | uniq -d \
    # Extract just the path to the symlink
    | cut -d' ' -f1 \
    # Prepend the rootfs directory to the paths
    | sed -e 's/^/\/rootfs/' \
    # Delete the files
    | xargs rm \
    \
    # Delete empty directories
    && find /rootfs -type d -empty -delete
################################################################################################################
# end of libicu-related stuff
################################################################################################################

RUN apt update && apt upgrade --yes

WORKDIR /src
COPY ["Directory.Build.props", "."]
COPY ["Modules/Directory.Build.props", "Modules/"]

COPY ["Applications/AdminApi/src/AdminApi/AdminApi.csproj", "Applications/AdminApi/src/AdminApi/"]
COPY ["Applications/AdminApi/src/AdminApi.Infrastructure.Database.Postgres/AdminApi.Infrastructure.Database.Postgres.csproj", "Applications/AdminApi/src/AdminApi.Infrastructure.Database.Postgres/"]
COPY ["Applications/AdminApi/src/AdminApi.Infrastructure/AdminApi.Infrastructure.csproj", "Applications/AdminApi/src/AdminApi.Infrastructure/"]
COPY ["Applications/AdminApi/src/AdminApi.Infrastructure.Database.SqlServer/AdminApi.Infrastructure.Database.SqlServer.csproj", "Applications/AdminApi/src/AdminApi.Infrastructure.Database.SqlServer/"]

COPY ["BuildingBlocks/src/BuildingBlocks.API/BuildingBlocks.API.csproj", "BuildingBlocks/src/BuildingBlocks.API/"]
COPY ["BuildingBlocks/src/BuildingBlocks.Domain/BuildingBlocks.Domain.csproj", "BuildingBlocks/src/BuildingBlocks.Domain/"]
COPY ["BuildingBlocks/src/BuildingBlocks.Module/BuildingBlocks.Module.csproj", "BuildingBlocks/src/BuildingBlocks.Module/"]
COPY ["BuildingBlocks/src/Tooling/Tooling.csproj", "BuildingBlocks/src/Tooling/"]
COPY ["BuildingBlocks/src/DevelopmentKit.Identity/DevelopmentKit.Identity.csproj", "BuildingBlocks/src/DevelopmentKit.Identity/"]
COPY ["BuildingBlocks/src/BuildingBlocks.Infrastructure/BuildingBlocks.Infrastructure.csproj", "BuildingBlocks/src/BuildingBlocks.Infrastructure/"]
COPY ["BuildingBlocks/src/BuildingBlocks.Application.Abstractions/BuildingBlocks.Application.Abstractions.csproj", "BuildingBlocks/src/BuildingBlocks.Application.Abstractions/"]
COPY ["BuildingBlocks/src/BuildingBlocks.Application/BuildingBlocks.Application.csproj", "BuildingBlocks/src/BuildingBlocks.Application/"]
COPY ["BuildingBlocks/src/Crypto/Crypto.csproj", "BuildingBlocks/src/Crypto/"]
COPY ["Common/src/Common.Infrastructure/Common.Infrastructure.csproj", "Common/src/Common.Infrastructure/"]

COPY ["Modules/Announcements/src/Announcements.Application/Announcements.Application.csproj", "Modules/Announcements/src/Announcements.Application/"]
COPY ["Modules/Announcements/src/Announcements.Domain/Announcements.Domain.csproj", "Modules/Announcements/src/Announcements.Domain/"]
COPY ["Modules/Announcements/src/Announcements.Infrastructure/Announcements.Infrastructure.csproj", "Modules/Announcements/src/Announcements.Infrastructure/"]
COPY ["Modules/Announcements/src/Announcements.Module/Announcements.Module.csproj", "Modules/Announcements/src/Announcements.Module/"]
COPY ["Modules/Announcements/src/Announcements.Infrastructure.Database.Postgres/Announcements.Infrastructure.Database.Postgres.csproj", "Modules/Announcements/src/Announcements.Infrastructure.Database.Postgres/"]
COPY ["Modules/Announcements/src/Announcements.Infrastructure.Database.SqlServer/Announcements.Infrastructure.Database.SqlServer.csproj", "Modules/Announcements/src/Announcements.Infrastructure.Database.SqlServer/"]

COPY ["Modules/Challenges/src/Challenges.Application/Challenges.Application.csproj", "Modules/Challenges/src/Challenges.Application/"]
COPY ["Modules/Challenges/src/Challenges.Domain/Challenges.Domain.csproj", "Modules/Challenges/src/Challenges.Domain/"]
COPY ["Modules/Challenges/src/Challenges.Infrastructure/Challenges.Infrastructure.csproj", "Modules/Challenges/src/Challenges.Infrastructure/"]
COPY ["Modules/Challenges/src/Challenges.Module/Challenges.Module.csproj", "Modules/Challenges/src/Challenges.Module/"]
COPY ["Modules/Challenges/src/Challenges.Infrastructure.Database.Postgres/Challenges.Infrastructure.Database.Postgres.csproj", "Modules/Challenges/src/Challenges.Infrastructure.Database.Postgres/"]
COPY ["Modules/Challenges/src/Challenges.Infrastructure.Database.SqlServer/Challenges.Infrastructure.Database.SqlServer.csproj", "Modules/Challenges/src/Challenges.Infrastructure.Database.SqlServer/"]

COPY ["Modules/Devices/src/Devices.Application/Devices.Application.csproj", "Modules/Devices/src/Devices.Application/"]
COPY ["Modules/Devices/src/Devices.Domain/Devices.Domain.csproj", "Modules/Devices/src/Devices.Domain/"]
COPY ["Modules/Devices/src/Devices.Infrastructure/Devices.Infrastructure.csproj", "Modules/Devices/src/Devices.Infrastructure/"]
COPY ["Modules/Devices/src/Devices.Module/Devices.Module.csproj", "Modules/Devices/src/Devices.Module/"]
COPY ["Modules/Devices/src/Devices.Infrastructure.Database.Postgres/Devices.Infrastructure.Database.Postgres.csproj", "Modules/Devices/src/Devices.Infrastructure.Database.Postgres/"]
COPY ["Modules/Devices/src/Devices.Infrastructure.Database.SqlServer/Devices.Infrastructure.Database.SqlServer.csproj", "Modules/Devices/src/Devices.Infrastructure.Database.SqlServer/"]

COPY ["Modules/Quotas/src/Quotas.Application/Quotas.Application.csproj", "Modules/Quotas/src/Quotas.Application/"]
COPY ["Modules/Quotas/src/Quotas.Domain/Quotas.Domain.csproj", "Modules/Quotas/src/Quotas.Domain/"]
COPY ["Modules/Quotas/src/Quotas.Infrastructure/Quotas.Infrastructure.csproj", "Modules/Quotas/src/Quotas.Infrastructure/"]
COPY ["Modules/Quotas/src/Quotas.Module/Quotas.Module.csproj", "Modules/Quotas/src/Quotas.Module/"]
COPY ["Modules/Quotas/src/Quotas.Infrastructure.Database.Postgres/Quotas.Infrastructure.Database.Postgres.csproj", "Modules/Quotas/src/Quotas.Infrastructure.Database.Postgres/"]
COPY ["Modules/Quotas/src/Quotas.Infrastructure.Database.SqlServer/Quotas.Infrastructure.Database.SqlServer.csproj", "Modules/Quotas/src/Quotas.Infrastructure.Database.SqlServer/"]

COPY ["Modules/Tokens/src/Tokens.Application/Tokens.Application.csproj", "Modules/Tokens/src/Tokens.Application/"]
COPY ["Modules/Tokens/src/Tokens.Domain/Tokens.Domain.csproj", "Modules/Tokens/src/Tokens.Domain/"]
COPY ["Modules/Tokens/src/Tokens.Infrastructure/Tokens.Infrastructure.csproj", "Modules/Tokens/src/Tokens.Infrastructure/"]
COPY ["Modules/Tokens/src/Tokens.Module/Tokens.Module.csproj", "Modules/Tokens/src/Tokens.Module/"]
COPY ["Modules/Tokens/src/Tokens.Infrastructure.Database.Postgres/Tokens.Infrastructure.Database.Postgres.csproj", "Modules/Tokens/src/Tokens.Infrastructure.Database.Postgres/"]
COPY ["Modules/Tokens/src/Tokens.Infrastructure.Database.SqlServer/Tokens.Infrastructure.Database.SqlServer.csproj", "Modules/Tokens/src/Tokens.Infrastructure.Database.SqlServer/"]

RUN dotnet restore /p:ContinuousIntegrationBuild=true "Applications/AdminApi/src/AdminApi/AdminApi.csproj"

COPY . .

ARG VERSION

RUN dotnet publish /p:ContinuousIntegrationBuild=true /p:UseAppHost=false --no-restore --configuration Release --output /app/publish "/src/Applications/AdminApi/src/AdminApi/AdminApi.csproj"
RUN dotnet publish /p:ContinuousIntegrationBuild=true --configuration Release --output /app/publish/health "/src/Applications/HealthCheck/src/HealthCheck.csproj"

#### Build Flutter Admin UI ####
FROM ghcr.io/cirruslabs/flutter:3.32.2@sha256:88415f5f8ce8087ccf6733b1952281b2f9e373b3c21d7498cfed719cfb9b7b81 AS flutter-build-env

COPY Applications/AdminUi /src
WORKDIR /src
RUN dart pub global activate melos
RUN dart pub global run melos:melos bootstrap
WORKDIR /src/apps/admin_ui
RUN flutter build web --output /build

#### Final #####
FROM mcr.microsoft.com/dotnet/aspnet:9.0-noble-chiseled@sha256:363d11f5c5979e7894c2eaaa878aed13946d52de68f5bef98d5bae11d2f242b1

LABEL org.opencontainers.image.source="https://github.com/nmshd/backbone"
HEALTHCHECK --interval=30s --timeout=3s --start-period=5s --retries=3 CMD [ "dotnet", "/app/health/Backbone.HealthCheck.dll", "--", "http://localhost:8080/health" ]
EXPOSE 8080

ENV ASPNETCORE_URLS=http://0.0.0.0:8080
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=0

WORKDIR /app

COPY --from=build-env /rootfs /
COPY --from=build-env /app/publish .
COPY --from=flutter-build-env /build /app/wwwroot

USER $APP_UID

ENTRYPOINT ["dotnet", "Backbone.AdminApi.dll"]
