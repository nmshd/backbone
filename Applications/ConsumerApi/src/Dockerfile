FROM dhi.io/dotnet:10.0.403-sdk@sha256:eb58013cd929c44300e8d7f80652011c6d1abdb3dcf30305e622ead82c29ed3f AS build-env

ARG VERSION

WORKDIR /src

COPY . .

RUN dotnet restore /p:ContinuousIntegrationBuild=true "Applications/ConsumerApi/src/ConsumerApi.csproj"
RUN dotnet restore /p:ContinuousIntegrationBuild=true "Applications/HealthCheck/src/HealthCheck.csproj"
RUN dotnet publish /p:ContinuousIntegrationBuild=true --configuration Release --output /app/publish --no-restore "Applications/ConsumerApi/src/ConsumerApi.csproj"
RUN dotnet publish /p:ContinuousIntegrationBuild=true --configuration Release --output /app/publish/health --no-restore "Applications/HealthCheck/src/HealthCheck.csproj"

FROM dhi.io/aspnetcore:10.0.14-debian13@sha256:4c9bb4f36fb1969dce36c1605d64d3fa0913fe11aecadd02abf371f0d117a258

LABEL org.opencontainers.image.source="https://github.com/nmshd/backbone"
HEALTHCHECK --interval=30s --timeout=3s --start-period=10s --retries=3 CMD [ "dotnet", "/app/health/Backbone.HealthCheck.dll", "--", "http://localhost:8080/health" ]
EXPOSE 8080

ENV ASPNETCORE_URLS=http://0.0.0.0:8080
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=0

WORKDIR /app

COPY --from=build-env /app/publish .

ENTRYPOINT ["dotnet", "Backbone.ConsumerApi.dll"]
