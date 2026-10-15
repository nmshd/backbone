FROM dhi.io/dotnet:10.0.402-sdk@sha256:2d58c985492e9b619314487fc3ea6e4b92eb322a03e70c6c2c3e1ea26b26461d AS build-env

ARG VERSION

WORKDIR /src

COPY . .

RUN dotnet restore /p:ContinuousIntegrationBuild=true "Applications/ConsumerApi/src/ConsumerApi.csproj"
RUN dotnet restore /p:ContinuousIntegrationBuild=true "Applications/HealthCheck/src/HealthCheck.csproj"
RUN dotnet publish /p:ContinuousIntegrationBuild=true --configuration Release --output /app/publish --no-restore "Applications/ConsumerApi/src/ConsumerApi.csproj"
RUN dotnet publish /p:ContinuousIntegrationBuild=true --configuration Release --output /app/publish/health --no-restore "Applications/HealthCheck/src/HealthCheck.csproj"

FROM dhi.io/aspnetcore:10.0.13-debian13@sha256:fd3c2052991fc4094bc852df6813f190a88be41e0430c4771c27b4540eb6030e

LABEL org.opencontainers.image.source="https://github.com/nmshd/backbone"
HEALTHCHECK --interval=30s --timeout=3s --start-period=10s --retries=3 CMD [ "dotnet", "/app/health/Backbone.HealthCheck.dll", "--", "http://localhost:8080/health" ]
EXPOSE 8080

ENV ASPNETCORE_URLS=http://0.0.0.0:8080
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=0

WORKDIR /app

COPY --from=build-env /app/publish .

ENTRYPOINT ["dotnet", "Backbone.ConsumerApi.dll"]
