FROM mcr.microsoft.com/dotnet/runtime:8.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["Directory.Build.props", "."]
COPY ["Modules/Synchronization/src/Synchronization.Jobs.SanityCheck/Synchronization.Jobs.SanityCheck.csproj", "Modules/Synchronization/src/Synchronization.Jobs.SanityCheck/"]
COPY ["BuildingBlocks/src/BuildingBlocks.Application.Abstractions/BuildingBlocks.Application.Abstractions.csproj", "BuildingBlocks/src/BuildingBlocks.Application.Abstractions/"]
COPY ["BuildingBlocks/src/BuildingBlocks.Domain/BuildingBlocks.Domain.csproj", "BuildingBlocks/src/BuildingBlocks.Domain/"]
COPY ["BuildingBlocks/src/DevelopmentKit.Identity/DevelopmentKit.Identity.csproj", "BuildingBlocks/src/DevelopmentKit.Identity/"]
COPY ["BuildingBlocks/src/StronglyTypedIds/StronglyTypedIds.csproj", "BuildingBlocks/src/StronglyTypedIds/"]
COPY ["BuildingBlocks/src/Tooling/Tooling.csproj", "BuildingBlocks/src/Tooling/"]
COPY ["Modules/Synchronization/src/Synchronization.Application/Synchronization.Application.csproj", "Modules/Synchronization/src/Synchronization.Application/"]
COPY ["BuildingBlocks/src/BuildingBlocks.Application/BuildingBlocks.Application.csproj", "BuildingBlocks/src/BuildingBlocks.Application/"]
COPY ["Modules/Synchronization/src/Synchronization.Domain/Synchronization.Domain.csproj", "Modules/Synchronization/src/Synchronization.Domain/"]
COPY ["Modules/Synchronization/src/Synchronization.Infrastructure/Synchronization.Infrastructure.csproj", "Modules/Synchronization/src/Synchronization.Infrastructure/"]
COPY ["BuildingBlocks/src/BuildingBlocks.Infrastructure/BuildingBlocks.Infrastructure.csproj", "BuildingBlocks/src/BuildingBlocks.Infrastructure/"]
COPY ["Common/src/Common.Infrastructure/Common.Infrastructure.csproj", "Common/src/Common.Infrastructure/"]

RUN dotnet restore "Modules/Synchronization/src/Synchronization.Jobs.SanityCheck/Synchronization.Jobs.SanityCheck.csproj"

COPY . .

WORKDIR "/src/Modules/Synchronization/src/Synchronization.Jobs.SanityCheck"

RUN dotnet build "Synchronization.Jobs.SanityCheck.csproj" --configuration Release --output /app/build --no-restore

FROM build AS publish
RUN dotnet publish /property:UseAppHost=false /property:WarningLevel=0 --configuration Release --output /app/publish --no-restore "Synchronization.Jobs.SanityCheck.csproj"

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Backbone.Modules.Synchronization.Jobs.SanityCheck.dll"]
