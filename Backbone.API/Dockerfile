#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["Backbone.API/Backbone.API.csproj", "Backbone.API/"]
COPY ["Backbone.Infrastructure/Backbone.Infrastructure.csproj", "Backbone.Infrastructure/"]
COPY ["BuildingBlocks/BuildingBlocks.Application.Abstractions/BuildingBlocks.Application.Abstractions/BuildingBlocks.Application.Abstractions.csproj", "BuildingBlocks/BuildingBlocks.Application.Abstractions/BuildingBlocks.Application.Abstractions/"]
COPY ["BuildingBlocks/DevelopmentKit.Identity/DevelopmentKit.Identity/DevelopmentKit.Identity.csproj", "BuildingBlocks/DevelopmentKit.Identity/DevelopmentKit.Identity/"]
COPY ["BuildingBlocks/StronglyTypedIds/StronglyTypedIds/StronglyTypedIds.csproj", "BuildingBlocks/StronglyTypedIds/StronglyTypedIds/"]
COPY ["BuildingBlocks/Tooling/Tooling/Tooling.csproj", "BuildingBlocks/Tooling/Tooling/"]
COPY ["BuildingBlocks/BuildingBlocks.Infrastructure/BuildingBlocks.Infrastructure/BuildingBlocks.Infrastructure.csproj", "BuildingBlocks/BuildingBlocks.Infrastructure/BuildingBlocks.Infrastructure/"]
COPY ["BuildingBlocks/BuildingBlocks.API/BuildingBlocks.API/BuildingBlocks.API.csproj", "BuildingBlocks/BuildingBlocks.API/BuildingBlocks.API/"]
COPY ["BuildingBlocks/BuildingBlocks.Application/BuildingBlocks.Application/BuildingBlocks.Application.csproj", "BuildingBlocks/BuildingBlocks.Application/BuildingBlocks.Application/"]
COPY ["Challenges/src/Challenges.Infrastructure/Challenges.Infrastructure.csproj", "Challenges/src/Challenges.Infrastructure/"]
COPY ["Challenges/src/Challenges.Application/Challenges.Application.csproj", "Challenges/src/Challenges.Application/"]
COPY ["Challenges/src/Challenges.Domain/Challenges.Domain.csproj", "Challenges/src/Challenges.Domain/"]
RUN dotnet restore "Backbone.API/Backbone.API.csproj"
COPY . .
WORKDIR "/src/Backbone.API"
RUN dotnet build "Backbone.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Backbone.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Backbone.API.dll"]