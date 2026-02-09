FROM mcr.microsoft.com/dotnet/sdk:10.0@sha256:6ba533cc61a5d8c5e7d4b3a3e33e2ddc2efef200b112e4d658303516bfd24255 AS build

ARG VERSION

WORKDIR /src

COPY ["Directory.Build.props", "."]
COPY ["Modules/Directory.Build.props", "Modules/"]

COPY ["Directory.Packages.props", "."]

COPY ["Applications/AdminCli/src/AdminCli/AdminCli.csproj", "Applications/AdminCli/src/AdminCli/packages.lock.json*", "Applications/AdminCli/src/AdminCli/"]

COPY ["Applications/AdminApi/src/AdminApi.Infrastructure/AdminApi.Infrastructure.csproj", "Applications/AdminApi/src/AdminApi.Infrastructure/packages.lock.json*", "Applications/AdminApi/src/AdminApi.Infrastructure/"]
COPY ["Applications/AdminApi/src/AdminApi.Infrastructure.Database.Postgres/AdminApi.Infrastructure.Database.Postgres.csproj", "Applications/AdminApi/src/AdminApi.Infrastructure.Database.Postgres/packages.lock.json*", "Applications/AdminApi/src/AdminApi.Infrastructure.Database.Postgres/"]
COPY ["Applications/AdminApi/src/AdminApi.Infrastructure.Database.SqlServer/AdminApi.Infrastructure.Database.SqlServer.csproj", "Applications/AdminApi/src/AdminApi.Infrastructure.Database.SqlServer/packages.lock.json*", "Applications/AdminApi/src/AdminApi.Infrastructure.Database.SqlServer/"]


COPY ["BuildingBlocks/src/BuildingBlocks.Domain/BuildingBlocks.Domain.csproj", "BuildingBlocks/src/BuildingBlocks.Domain/packages.lock.json*", "BuildingBlocks/src/BuildingBlocks.Domain/"]
COPY ["BuildingBlocks/src/BuildingBlocks.Infrastructure/BuildingBlocks.Infrastructure.csproj", "BuildingBlocks/src/BuildingBlocks.Infrastructure/packages.lock.json*", "BuildingBlocks/src/BuildingBlocks.Infrastructure/"]
COPY ["BuildingBlocks/src/BuildingBlocks.API/BuildingBlocks.API.csproj", "BuildingBlocks/src/BuildingBlocks.API/packages.lock.json*", "BuildingBlocks/src/BuildingBlocks.API/"]
COPY ["BuildingBlocks/src/BuildingBlocks.Application.Abstractions/BuildingBlocks.Application.Abstractions.csproj", "BuildingBlocks/src/BuildingBlocks.Application.Abstractions/packages.lock.json*", "BuildingBlocks/src/BuildingBlocks.Application.Abstractions/"]
COPY ["BuildingBlocks/src/BuildingBlocks.Application/BuildingBlocks.Application.csproj", "BuildingBlocks/src/BuildingBlocks.Application/packages.lock.json*", "BuildingBlocks/src/BuildingBlocks.Application/"]
COPY ["BuildingBlocks/src/BuildingBlocks.Module/BuildingBlocks.Module.csproj", "BuildingBlocks/src/BuildingBlocks.Module/packages.lock.json*", "BuildingBlocks/src/BuildingBlocks.Module/"]
COPY ["BuildingBlocks/src/DevelopmentKit.Identity/DevelopmentKit.Identity.csproj", "BuildingBlocks/src/DevelopmentKit.Identity/packages.lock.json*", "BuildingBlocks/src/DevelopmentKit.Identity/"]
COPY ["BuildingBlocks/src/StronglyTypedIds/StronglyTypedIds.csproj", "BuildingBlocks/src/StronglyTypedIds/packages.lock.json*", "BuildingBlocks/src/StronglyTypedIds/"]
COPY ["BuildingBlocks/src/Tooling/Tooling.csproj", "BuildingBlocks/src/Tooling/packages.lock.json*", "BuildingBlocks/src/Tooling/"]
COPY ["BuildingBlocks/src/Crypto/Crypto.csproj", "BuildingBlocks/src/Crypto/packages.lock.json*", "BuildingBlocks/src/Crypto/"]

COPY ["Common/src/Common.Infrastructure/Common.Infrastructure.csproj", "Common/src/Common.Infrastructure/packages.lock.json*", "Common/src/Common.Infrastructure/"]

COPY ["Modules/Announcements/src/Announcements.Application/Announcements.Application.csproj", "Modules/Announcements/src/Announcements.Application/packages.lock.json*", "Modules/Announcements/src/Announcements.Application/"]
COPY ["Modules/Announcements/src/Announcements.Infrastructure/Announcements.Infrastructure.csproj", "Modules/Announcements/src/Announcements.Infrastructure/packages.lock.json*", "Modules/Announcements/src/Announcements.Infrastructure/"]
COPY ["Modules/Announcements/src/Announcements.Infrastructure.Database.Postgres/Announcements.Infrastructure.Database.Postgres.csproj", "Modules/Announcements/src/Announcements.Infrastructure.Database.Postgres/packages.lock.json*", "Modules/Announcements/src/Announcements.Infrastructure.Database.Postgres/"]
COPY ["Modules/Announcements/src/Announcements.Infrastructure.Database.SqlServer/Announcements.Infrastructure.Database.SqlServer.csproj", "Modules/Announcements/src/Announcements.Infrastructure.Database.SqlServer/packages.lock.json*", "Modules/Announcements/src/Announcements.Infrastructure.Database.SqlServer/"]
COPY ["Modules/Announcements/src/Announcements.Domain/Announcements.Domain.csproj", "Modules/Announcements/src/Announcements.Domain/packages.lock.json*", "Modules/Announcements/src/Announcements.Domain/"]
COPY ["Modules/Announcements/src/Announcements.Module/Announcements.Module.csproj", "Modules/Announcements/src/Announcements.Module/packages.lock.json*", "Modules/Announcements/src/Announcements.Module/"]

COPY ["Modules/Devices/src/Devices.Application/Devices.Application.csproj", "Modules/Devices/src/Devices.Application/packages.lock.json*", "Modules/Devices/src/Devices.Application/"]
COPY ["Modules/Devices/src/Devices.Infrastructure/Devices.Infrastructure.csproj", "Modules/Devices/src/Devices.Infrastructure/packages.lock.json*", "Modules/Devices/src/Devices.Infrastructure/"]
COPY ["Modules/Devices/src/Devices.Domain/Devices.Domain.csproj", "Modules/Devices/src/Devices.Domain/packages.lock.json*", "Modules/Devices/src/Devices.Domain/"]
COPY ["Modules/Devices/src/Devices.Module/Devices.Module.csproj", "Modules/Devices/src/Devices.Module/packages.lock.json*", "Modules/Devices/src/Devices.Module/"]
COPY ["Modules/Devices/src/Devices.Infrastructure.Database.Postgres/Devices.Infrastructure.Database.Postgres.csproj", "Modules/Devices/src/Devices.Infrastructure.Database.Postgres/packages.lock.json*", "Modules/Devices/src/Devices.Infrastructure.Database.Postgres/"]
COPY ["Modules/Devices/src/Devices.Infrastructure.Database.SqlServer/Devices.Infrastructure.Database.SqlServer.csproj", "Modules/Devices/src/Devices.Infrastructure.Database.SqlServer/packages.lock.json*", "Modules/Devices/src/Devices.Infrastructure.Database.SqlServer/"]

COPY ["Modules/Tokens/src/Tokens.Application/Tokens.Application.csproj", "Modules/Tokens/src/Tokens.Application/packages.lock.json*", "Modules/Tokens/src/Tokens.Application/"]
COPY ["Modules/Tokens/src/Tokens.Infrastructure/Tokens.Infrastructure.csproj", "Modules/Tokens/src/Tokens.Infrastructure/packages.lock.json*", "Modules/Tokens/src/Tokens.Infrastructure/"]
COPY ["Modules/Tokens/src/Tokens.Domain/Tokens.Domain.csproj", "Modules/Tokens/src/Tokens.Domain/packages.lock.json*", "Modules/Tokens/src/Tokens.Domain/"]
COPY ["Modules/Tokens/src/Tokens.Module/Tokens.Module.csproj", "Modules/Tokens/src/Tokens.Module/packages.lock.json*", "Modules/Tokens/src/Tokens.Module/"]
COPY ["Modules/Tokens/src/Tokens.Infrastructure.Database.Postgres/Tokens.Infrastructure.Database.Postgres.csproj", "Modules/Tokens/src/Tokens.Infrastructure.Database.Postgres/packages.lock.json*", "Modules/Tokens/src/Tokens.Infrastructure.Database.Postgres/"]
COPY ["Modules/Tokens/src/Tokens.Infrastructure.Database.SqlServer/Tokens.Infrastructure.Database.SqlServer.csproj", "Modules/Tokens/src/Tokens.Infrastructure.Database.SqlServer/packages.lock.json*", "Modules/Tokens/src/Tokens.Infrastructure.Database.SqlServer/"]


RUN dotnet restore /p:ContinuousIntegrationBuild=true "Applications/AdminCli/src/AdminCli/AdminCli.csproj"

COPY . .

RUN dotnet publish /p:ContinuousIntegrationBuild=true --no-restore -c Release -o /app/publish "Applications/AdminCli/src/AdminCli/AdminCli.csproj"

# Create a simple wrapper script to call the AdminCli. This simplifies execution, because users do not have to write "dotnet /app/Backbone.AdminCli.dll <command>" every time.
# Instead they can just call "backbone <command>"
RUN echo '#!/bin/bash\nexec dotnet /app/Backbone.AdminCli.dll "$@"' > /app/publish/backbone && chmod +x /app/publish/backbone

FROM mcr.microsoft.com/dotnet/aspnet:10.0-noble@sha256:7feda9a96737a8c268cc7636c92d0efda3512ef019e3c409dd97c7c9ae9e2bdb
RUN apt-get update && apt-get install -y libgssapi-krb5-2 && rm -rf /var/lib/apt/lists/*

WORKDIR /app

COPY --from=build /app/publish/ .

ENV PATH="$PATH:/app"

LABEL org.opencontainers.image.source="https://github.com/nmshd/backbone"
