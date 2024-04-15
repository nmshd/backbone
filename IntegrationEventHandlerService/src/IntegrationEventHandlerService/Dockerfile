FROM mcr.microsoft.com/dotnet/runtime:8.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["IntegrationEventHandlerService/src/IntegrationEventHandlerService/IntegrationEventHandlerService.csproj", "IntegrationEventHandlerService/src/IntegrationEventHandlerService/"]

RUN dotnet restore "IntegrationEventHandlerService/src/IntegrationEventHandlerService/IntegrationEventHandlerService.csproj"

COPY . .

WORKDIR "/src/IntegrationEventHandlerService/src/IntegrationEventHandlerService"
RUN dotnet build "./IntegrationEventHandlerService.csproj" -c Release -o /app/build --no-restore

FROM build AS publish
RUN dotnet publish -c Release -o /app/publish --no-restore "IntegrationEventHandlerService.csproj"

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Backbone.IntegrationEventHandlerService.dll"]
