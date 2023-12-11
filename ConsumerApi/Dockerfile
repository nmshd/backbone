# Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env
WORKDIR /src
COPY . .

RUN dotnet restore "ConsumerApi/ConsumerApi.csproj"

RUN dotnet publish /property:WarningLevel=0 --configuration Release --output /app --no-restore "ConsumerApi/ConsumerApi.csproj"

RUN dotnet publish --configuration Release --output /app/health "HealthCheck/HealthCheck.csproj"

# Run
FROM mcr.microsoft.com/dotnet/runtime-deps:8.0.0-jammy-chiseled-extra
EXPOSE 8080
WORKDIR /app

# ENV Logging__Console__FormatterName=
# ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=0
# RUN apk add icu-libs
COPY --from=build-env /app .

LABEL org.opencontainers.image.source = "https://github.com/nmshd/backbone"

ENV ASPNETCORE_URLS=http://0.0.0.0:8080

HEALTHCHECK --interval=30s --timeout=3s --start-period=5s --retries=3 CMD [ "dotnet", "/app/health/Backbone.HealthCheck.dll", "--", "http://localhost:8080/health" ]

USER $APP_UID

ENTRYPOINT ["dotnet", "Backbone.ConsumerApi.dll"]
