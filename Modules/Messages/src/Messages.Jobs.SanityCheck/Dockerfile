FROM mcr.microsoft.com/dotnet/runtime:7.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src

COPY ["Modules/Messages/src/Messages.Jobs.SanityCheck/Messages.Jobs.SanityCheck.csproj", "Modules/Messages/src/Messages.Jobs.SanityCheck/"]
COPY ["BuildingBlocks/src/BuildingBlocks.Application.Abstractions/BuildingBlocks.Application.Abstractions.csproj", "BuildingBlocks/src/BuildingBlocks.Application.Abstractions/"]
COPY ["BuildingBlocks/src/DevelopmentKit.Identity/DevelopmentKit.Identity.csproj", "BuildingBlocks/src/DevelopmentKit.Identity/"]
COPY ["BuildingBlocks/src/StronglyTypedIds/StronglyTypedIds.csproj", "BuildingBlocks/src/StronglyTypedIds/"]
COPY ["BuildingBlocks/src/Tooling/Tooling.csproj", "BuildingBlocks/src/Tooling/"]
COPY ["Modules/Messages/src/Messages.Application/Messages.Application.csproj", "Modules/Messages/src/Messages.Application/"]
COPY ["BuildingBlocks/src/BuildingBlocks.Application/BuildingBlocks.Application.csproj", "BuildingBlocks/src/BuildingBlocks.Application/"]
COPY ["Modules/Messages/src/Messages.Common/Messages.Common.csproj", "Modules/Messages/src/Messages.Common/"]
COPY ["Modules/Messages/src/Messages.Domain/Messages.Domain.csproj", "Modules/Messages/src/Messages.Domain/"]
COPY ["Modules/Messages/src/Messages.Infrastructure/Messages.Infrastructure.csproj", "Modules/Messages/src/Messages.Infrastructure/"]
COPY ["BuildingBlocks/src/BuildingBlocks.Infrastructure/BuildingBlocks.Infrastructure.csproj", "BuildingBlocks/src/BuildingBlocks.Infrastructure/"]

RUN dotnet restore "Modules/Messages/src/Messages.Jobs.SanityCheck/Messages.Jobs.SanityCheck.csproj"

COPY . .

WORKDIR "/src/Modules/Messages/src/Messages.Jobs.SanityCheck"

RUN dotnet build "Messages.Jobs.SanityCheck.csproj" --configuration Release --output /app/build --no-restore

FROM build AS publish
RUN dotnet publish /property:UseAppHost=false /property:WarningLevel=0 --configuration Release --output /app/publish --no-restore "Messages.Jobs.SanityCheck.csproj"

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Messages.Jobs.SanityCheck.dll"]