FROM mcr.microsoft.com/dotnet/runtime:9.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY ["Directory.Build.props", "."]
COPY ["Applications/FilesSanityCheck/src/FilesSanityCheck/FilesSanityCheck.csproj", "Applications/FilesSanityCheck/src/FilesSanityCheck/"]
COPY ["BuildingBlocks/src/BuildingBlocks.Application.Abstractions/BuildingBlocks.Application.Abstractions.csproj", "BuildingBlocks/src/BuildingBlocks.Application.Abstractions/"]
COPY ["BuildingBlocks/src/BuildingBlocks.Domain/BuildingBlocks.Domain.csproj", "BuildingBlocks/src/BuildingBlocks.Domain/"]
COPY ["BuildingBlocks/src/DevelopmentKit.Identity/DevelopmentKit.Identity.csproj", "BuildingBlocks/src/DevelopmentKit.Identity/"]
COPY ["BuildingBlocks/src/StronglyTypedIds/StronglyTypedIds.csproj", "BuildingBlocks/src/StronglyTypedIds/"]
COPY ["BuildingBlocks/src/Tooling/Tooling.csproj", "BuildingBlocks/src/Tooling/"]
COPY ["Modules/Files/src/Files.Application/Files.Application.csproj", "Modules/Files/src/Files.Application/"]
COPY ["BuildingBlocks/src/BuildingBlocks.Application/BuildingBlocks.Application.csproj", "BuildingBlocks/src/BuildingBlocks.Application/"]
COPY ["Modules/Files/src/Files.Domain/Files.Domain.csproj", "Modules/Files/src/Files.Domain/"]
COPY ["Modules/Files/src/Files.Infrastructure/Files.Infrastructure.csproj", "Modules/Files/src/Files.Infrastructure/"]
COPY ["BuildingBlocks/src/BuildingBlocks.Infrastructure/BuildingBlocks.Infrastructure.csproj", "BuildingBlocks/src/BuildingBlocks.Infrastructure/"]
COPY ["Common/src/Common.Infrastructure/Common.Infrastructure.csproj", "Common/src/Common.Infrastructure/"]

RUN dotnet restore /p:ContinuousIntegrationBuild=true "Applications/FilesSanityCheck/src/FilesSanityCheck/FilesSanityCheck.csproj"

COPY . .

ARG VERSION

WORKDIR "/src/Applications/FilesSanityCheck/src/FilesSanityCheck"
RUN dotnet build /p:ContinuousIntegrationBuild=true "FilesSanityCheck.csproj" --configuration Release --output /app/build --no-restore

FROM build AS publish
RUN dotnet publish /p:ContinuousIntegrationBuild=true /property:UseAppHost=false --configuration Release --output /app/publish --no-restore "FilesSanityCheck.csproj" 

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Backbone.Modules.Files.Jobs.SanityCheck.dll"]
