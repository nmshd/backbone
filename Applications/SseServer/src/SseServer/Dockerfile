FROM dhi.io/dotnet:11.0.100-sdk@sha256:d7e0803b6e53a5b709e3e455559d997e8dbd18f5117ab6f3b41751bb47a86cb3 AS build-env

ARG VERSION

WORKDIR /src

COPY . .

RUN dotnet restore /p:ContinuousIntegrationBuild=true "Applications/SseServer/src/SseServer/SseServer.csproj"
RUN dotnet restore /p:ContinuousIntegrationBuild=true "Applications/HealthCheck/src/HealthCheck.csproj"
RUN dotnet publish /p:ContinuousIntegrationBuild=true --configuration Release --output /app/publish --no-restore "Applications/SseServer/src/SseServer/SseServer.csproj"
RUN dotnet publish /p:ContinuousIntegrationBuild=true --configuration Release --output /app/publish/health --no-restore "Applications/HealthCheck/src/HealthCheck.csproj"

FROM dhi.io/aspnetcore:11.0.0-debian13@sha256:6bb1c585896870b9d56c1b065707e1a00968a042e44e60906f0d130988ae8e7b

LABEL org.opencontainers.image.source="https://github.com/nmshd/backbone"
HEALTHCHECK --interval=30s --timeout=3s --start-period=10s --retries=3 CMD [ "dotnet", "/app/health/Backbone.HealthCheck.dll", "--", "http://localhost:8080/health" ]
EXPOSE 8080

ENV ASPNETCORE_URLS=http://0.0.0.0:8080
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=0

WORKDIR /app

COPY --from=build-env /app/publish .

ENTRYPOINT ["dotnet", "Backbone.SseServer.dll"]
