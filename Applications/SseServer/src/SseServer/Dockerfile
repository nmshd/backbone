FROM dhi.io/dotnet:10.0.403-sdk@sha256:cbec5ea09e27d615e872404ababdb15d921ad79ff876625def27aef13d90ca68 AS build-env

ARG VERSION

WORKDIR /src

COPY . .

RUN dotnet restore /p:ContinuousIntegrationBuild=true "Applications/SseServer/src/SseServer/SseServer.csproj"
RUN dotnet restore /p:ContinuousIntegrationBuild=true "Applications/HealthCheck/src/HealthCheck.csproj"
RUN dotnet publish /p:ContinuousIntegrationBuild=true --configuration Release --output /app/publish --no-restore "Applications/SseServer/src/SseServer/SseServer.csproj"
RUN dotnet publish /p:ContinuousIntegrationBuild=true --configuration Release --output /app/publish/health --no-restore "Applications/HealthCheck/src/HealthCheck.csproj"

FROM dhi.io/aspnetcore:10.0.14-debian13@sha256:ecf93dc794deabaca33ade3f7556088661b3c94fac5092f0174bce3bf38eff14

LABEL org.opencontainers.image.source="https://github.com/nmshd/backbone"
HEALTHCHECK --interval=30s --timeout=3s --start-period=10s --retries=3 CMD [ "dotnet", "/app/health/Backbone.HealthCheck.dll", "--", "http://localhost:8080/health" ]
EXPOSE 8080

ENV ASPNETCORE_URLS=http://0.0.0.0:8080
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=0

WORKDIR /app

COPY --from=build-env /app/publish .

ENTRYPOINT ["dotnet", "Backbone.SseServer.dll"]
