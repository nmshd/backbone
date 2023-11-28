FROM mcr.microsoft.com/dotnet/runtime:8.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["Directory.Build.props", "."]
COPY ["Modules/Tokens/src/Tokens.Jobs.SanityCheck/Tokens.Jobs.SanityCheck.csproj", "Modules/Tokens/src/Tokens.Jobs.SanityCheck/"]
COPY ["BuildingBlocks/src/BuildingBlocks.Application.Abstractions/BuildingBlocks.Application.Abstractions.csproj", "BuildingBlocks/src/BuildingBlocks.Application.Abstractions/"]
COPY ["BuildingBlocks/src/BuildingBlocks.Domain/BuildingBlocks.Domain.csproj", "BuildingBlocks/src/BuildingBlocks.Domain/"]
COPY ["BuildingBlocks/src/DevelopmentKit.Identity/DevelopmentKit.Identity.csproj", "BuildingBlocks/src/DevelopmentKit.Identity/"]
COPY ["BuildingBlocks/src/StronglyTypedIds/StronglyTypedIds.csproj", "BuildingBlocks/src/StronglyTypedIds/"]
COPY ["BuildingBlocks/src/Tooling/Tooling.csproj", "BuildingBlocks/src/Tooling/"]
COPY ["Modules/Tokens/src/Tokens.Application/Tokens.Application.csproj", "Modules/Tokens/src/Tokens.Application/"]
COPY ["BuildingBlocks/src/BuildingBlocks.Application/BuildingBlocks.Application.csproj", "BuildingBlocks/src/BuildingBlocks.Application/"]
COPY ["Modules/Tokens/src/Tokens.Domain/Tokens.Domain.csproj", "Modules/Tokens/src/Tokens.Domain/"]
COPY ["Modules/Tokens/src/Tokens.Infrastructure/Tokens.Infrastructure.csproj", "Modules/Tokens/src/Tokens.Infrastructure/"]
COPY ["BuildingBlocks/src/BuildingBlocks.Infrastructure/BuildingBlocks.Infrastructure.csproj", "BuildingBlocks/src/BuildingBlocks.Infrastructure/"]
COPY ["Common/src/Common.Infrastructure/Common.Infrastructure.csproj", "Common/src/Common.Infrastructure/"]

RUN dotnet restore "Modules/Tokens/src/Tokens.Jobs.SanityCheck/Tokens.Jobs.SanityCheck.csproj"

COPY . .

WORKDIR "/src/Modules/Tokens/src/Tokens.Jobs.SanityCheck"

RUN dotnet build "Tokens.Jobs.SanityCheck.csproj" --configuration Release --output /app/build --no-restore

FROM build AS publish
RUN dotnet publish /property:UseAppHost=false /property:WarningLevel=0 --configuration Release --output /app/publish --no-restore "Tokens.Jobs.SanityCheck.csproj"

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Backbone.Modules.Tokens.Jobs.SanityCheck.dll"]
