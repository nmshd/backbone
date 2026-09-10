FROM node:24-bookworm-slim@sha256:ba849c60be29959425b8734d57b8b4b7d56f98edd9504c9af091d5281095a71e AS openid4vp-verifier-build-env

WORKDIR /src/Applications/ConsumerApi/src/OpenId4VpVerifierSite

COPY ["Applications/ConsumerApi/src/OpenId4VpVerifierSite/package.json", "Applications/ConsumerApi/src/OpenId4VpVerifierSite/package-lock.json", "./"]
RUN npm ci

COPY ["Applications/ConsumerApi/src/OpenId4VpVerifierSite/", "./"]
RUN npm run build

FROM dhi.io/dotnet:10.0.302-sdk@sha256:e18fceb745383b13f9c334a22c4c17943770129a5601033d3dfc1beff2cec17e AS build-env

ARG VERSION

WORKDIR /src

COPY . .
COPY --from=openid4vp-verifier-build-env /src/Applications/ConsumerApi/src/OpenId4VpVerifierSite/dist/openid4vp-verifier ./Applications/ConsumerApi/src/wwwroot/openid4vp-verifier

RUN dotnet restore /p:ContinuousIntegrationBuild=true "Applications/ConsumerApi/src/ConsumerApi.csproj"
RUN dotnet restore /p:ContinuousIntegrationBuild=true "Applications/HealthCheck/src/HealthCheck.csproj"
RUN dotnet publish /p:ContinuousIntegrationBuild=true /p:SkipOpenId4VpVerifierBuild=true --configuration Release --output /app/publish --no-restore "Applications/ConsumerApi/src/ConsumerApi.csproj"
RUN dotnet publish /p:ContinuousIntegrationBuild=true --configuration Release --output /app/publish/health --no-restore "Applications/HealthCheck/src/HealthCheck.csproj"

FROM dhi.io/aspnetcore:10.0.10-debian13@sha256:36f56fe5ec5c2ac63d1451b43eb6ef00f981c9454159e3a45087a2b9071d405d

LABEL org.opencontainers.image.source="https://github.com/nmshd/backbone"
HEALTHCHECK --interval=30s --timeout=3s --start-period=10s --retries=3 CMD [ "dotnet", "/app/health/Backbone.HealthCheck.dll", "--", "http://localhost:8080/health" ]
EXPOSE 8080

ENV ASPNETCORE_URLS=http://0.0.0.0:8080
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=0

WORKDIR /app

COPY --from=build-env /app/publish .

ENTRYPOINT ["dotnet", "Backbone.ConsumerApi.dll"]
