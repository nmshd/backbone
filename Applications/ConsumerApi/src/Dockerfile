FROM dhi.io/dotnet:10.0.101@sha256:7b8537ac8b4a954e35033d3bbf62a7f8e760b097fecaf09c9fd723444c25a3d3 AS build-env

ARG VERSION

WORKDIR /src

COPY . .

RUN dotnet restore /p:ContinuousIntegrationBuild=true "Applications/ConsumerApi/src/ConsumerApi.csproj"
RUN dotnet restore /p:ContinuousIntegrationBuild=true "Applications/HealthCheck/src/HealthCheck.csproj"
RUN dotnet publish /p:ContinuousIntegrationBuild=true --configuration Release --output /app/publish --no-restore "Applications/ConsumerApi/src/ConsumerApi.csproj"
RUN dotnet publish /p:ContinuousIntegrationBuild=true --configuration Release --output /app/publish/health --no-restore "Applications/HealthCheck/src/HealthCheck.csproj"

FROM dhi.io/aspnetcore:10.0.1-alpine3.22@sha256:467fbea286437c7345fd5612dbb975e8debae801ab5d93398b5db69fa163d4d8

LABEL org.opencontainers.image.source="https://github.com/nmshd/backbone"
HEALTHCHECK --interval=30s --timeout=3s --start-period=10s --retries=3 CMD [ "dotnet", "/app/health/Backbone.HealthCheck.dll", "--", "http://localhost:8080/health" ]
EXPOSE 8080

ENV ASPNETCORE_URLS=http://0.0.0.0:8080
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=0

WORKDIR /app

COPY --from=build-env /app/publish .

ENTRYPOINT ["dotnet", "Backbone.ConsumerApi.dll"]
