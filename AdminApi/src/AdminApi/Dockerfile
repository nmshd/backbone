#### Build chisel binary ####
################################################################################################################
# The following lines are required to install libicu, which itself is required for the SQL Server client to work 
# (see https://github.com/dotnet/dotnet-docker/discussions/4527).
# They are copied from https://github.com/ubuntu-rocks/dotnet/issues/21
################################################################################################################
FROM golang:1.20 as chisel

RUN git clone --depth 1 -b main https://github.com/canonical/chisel /opt/chisel
WORKDIR /opt/chisel
RUN go build ./cmd/chisel

#### Build application ####
FROM mcr.microsoft.com/dotnet/sdk:8.0-jammy AS build-env

# use chisel to install libicu
RUN apt-get update \
    && apt-get install -y fdupes \
    && rm -rf /var/lib/apt/lists/*

COPY --from=chisel /opt/chisel/chisel /usr/bin/
COPY --from=mcr.microsoft.com/dotnet/nightly/runtime:8.0-jammy-chiseled / /runtime-ref

RUN mkdir /rootfs \
    && chisel cut --release "ubuntu-22.04" --root /rootfs \
    libicu70_libs \
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

# install nodejs
RUN apt install --yes ca-certificates gnupg && \
    mkdir -p /etc/apt/keyrings && \
    curl -fsSL https://deb.nodesource.com/gpgkey/nodesource-repo.gpg.key | gpg --dearmor -o /etc/apt/keyrings/nodesource.gpg && \
    echo "deb [signed-by=/etc/apt/keyrings/nodesource.gpg] https://deb.nodesource.com/node_18.x nodistro main" | tee /etc/apt/sources.list.d/nodesource.list && \
    apt update && \
    apt install --yes nodejs

RUN npm i -g @angular/cli

WORKDIR /src
COPY ["Directory.Build.props", "."]
COPY ["Modules/Directory.Build.props", "Modules/"]
COPY ["AdminApi/src/AdminApi/AdminApi.csproj", "AdminApi/src/AdminApi/"]
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
COPY ["Modules/Challenges/src/Challenges.Application/Challenges.Application.csproj", "Modules/Challenges/src/Challenges.Application/"]
COPY ["Modules/Challenges/src/Challenges.Domain/Challenges.Domain.csproj", "Modules/Challenges/src/Challenges.Domain/"]
COPY ["Modules/Challenges/src/Challenges.Infrastructure/Challenges.Infrastructure.csproj", "Modules/Challenges/src/Challenges.Infrastructure/"]
COPY ["Infrastructure/Infrastructure.csproj", "Infrastructure/"]
COPY ["Modules/Quotas/src/Quotas.Application/Quotas.Application.csproj", "Modules/Quotas/src/Quotas.Application/"]
COPY ["Modules/Quotas/src/Quotas.Domain/Quotas.Domain.csproj", "Modules/Quotas/src/Quotas.Domain/"]
COPY ["Modules/Quotas/src/Quotas.Infrastructure/Quotas.Infrastructure.csproj", "Modules/Quotas/src/Quotas.Infrastructure/"]
COPY ["AdminApi/src/AdminApi.Infrastructure.Database.Postgres/AdminApi.Infrastructure.Database.Postgres.csproj", "AdminApi/src/AdminApi.Infrastructure.Database.Postgres/"]
COPY ["AdminApi/src/AdminApi.Infrastructure/AdminApi.Infrastructure.csproj", "AdminApi/src/AdminApi.Infrastructure/"]
COPY ["AdminApi/src/AdminApi.Infrastructure.Database.SqlServer/AdminApi.Infrastructure.Database.SqlServer.csproj", "AdminApi/src/AdminApi.Infrastructure.Database.SqlServer/"]
RUN dotnet restore "AdminApi/src/AdminApi/AdminApi.csproj"

COPY . .

RUN dotnet publish /p:UseAppHost=false --no-restore --configuration Release --output /app/publish "/src/AdminApi/src/AdminApi/AdminApi.csproj"
RUN dotnet publish --configuration Release --output /app/publish/health "HealthCheck/HealthCheck.csproj"

### Final ####
FROM mcr.microsoft.com/dotnet/aspnet:8.0.0-jammy-chiseled

LABEL org.opencontainers.image.source = "https://github.com/nmshd/backbone"
HEALTHCHECK --interval=30s --timeout=3s --start-period=5s --retries=3 CMD [ "dotnet", "/app/health/Backbone.HealthCheck.dll", "--", "http://localhost:8080/health" ]
EXPOSE 8080

ENV ASPNETCORE_URLS=http://0.0.0.0:8080
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=0

WORKDIR /app

COPY --from=build-env /rootfs /
COPY --from=build-env /app/publish .

USER $APP_UID

ENTRYPOINT ["dotnet", "Backbone.AdminApi.dll"]
