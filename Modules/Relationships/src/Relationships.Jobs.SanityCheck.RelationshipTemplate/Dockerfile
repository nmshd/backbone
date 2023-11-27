FROM mcr.microsoft.com/dotnet/runtime:8.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["Directory.Build.props", "."]
COPY ["Modules/Relationships/src/Relationships.Jobs.SanityCheck.RelationshipTemplate/Relationships.Jobs.SanityCheck.RelationshipTemplate.csproj", "Modules/Relationships/src/Relationships.Jobs.SanityCheck.RelationshipTemplate/"]
COPY ["BuildingBlocks/src/BuildingBlocks.Application.Abstractions/BuildingBlocks.Application.Abstractions.csproj", "BuildingBlocks/src/BuildingBlocks.Application.Abstractions/"]
COPY ["BuildingBlocks/src/BuildingBlocks.Domain/BuildingBlocks.Domain.csproj", "BuildingBlocks/src/BuildingBlocks.Domain/"]
COPY ["BuildingBlocks/src/DevelopmentKit.Identity/DevelopmentKit.Identity.csproj", "BuildingBlocks/src/DevelopmentKit.Identity/"]
COPY ["BuildingBlocks/src/StronglyTypedIds/StronglyTypedIds.csproj", "BuildingBlocks/src/StronglyTypedIds/"]
COPY ["BuildingBlocks/src/Tooling/Tooling.csproj", "BuildingBlocks/src/Tooling/"]
COPY ["Modules/Relationships/src/Relationships.Application/Relationships.Application.csproj", "Modules/Relationships/src/Relationships.Application/"]
COPY ["BuildingBlocks/src/BuildingBlocks.Application/BuildingBlocks.Application.csproj", "BuildingBlocks/src/BuildingBlocks.Application/"]
COPY ["Modules/Relationships/src/Relationships.Common/Relationships.Common.csproj", "Modules/Relationships/src/Relationships.Common/"]
COPY ["Modules/Relationships/src/Relationships.Domain/Relationships.Domain.csproj", "Modules/Relationships/src/Relationships.Domain/"]
COPY ["Modules/Relationships/src/Relationships.Infrastructure/Relationships.Infrastructure.csproj", "Modules/Relationships/src/Relationships.Infrastructure/"]
COPY ["BuildingBlocks/src/BuildingBlocks.Infrastructure/BuildingBlocks.Infrastructure.csproj", "BuildingBlocks/src/BuildingBlocks.Infrastructure/"]
COPY ["Common/src/Common.Infrastructure/Common.Infrastructure.csproj", "Common/src/Common.Infrastructure/"]

RUN dotnet restore "Modules/Relationships/src/Relationships.Jobs.SanityCheck.RelationshipTemplate/Relationships.Jobs.SanityCheck.RelationshipTemplate.csproj"

COPY . .

WORKDIR "/src/Modules/Relationships/src/Relationships.Jobs.SanityCheck.RelationshipTemplate"

RUN dotnet build "Relationships.Jobs.SanityCheck.RelationshipTemplate.csproj" --configuration Release --output /app/build --no-restore

FROM build AS publish
RUN dotnet publish /property:UseAppHost=false /property:WarningLevel=0 --configuration Release --output /app/publish --no-restore "Relationships.Jobs.SanityCheck.RelationshipTemplate.csproj"

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Backbone.Modules.Relationships.Jobs.SanityCheck.RelationshipTemplate.dll"]
