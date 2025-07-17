# Build
FROM mcr.microsoft.com/dotnet/sdk:9.0-alpine@sha256:329088c6945f917af6492ebe58b74c9720e1cf7e8e2bcbf62e04ab3232e18e7c AS build-env
WORKDIR /src

ARG VERSION

COPY . .
RUN dotnet restore /p:ContinuousIntegrationBuild=true "Challenges.Jobs.Cleanup/Challenges.Jobs.Cleanup.csproj"

WORKDIR "/src/Challenges.Jobs.Cleanup"
RUN dotnet publish /p:ContinuousIntegrationBuild=true --configuration Release --output /app --no-restore

# Run	
FROM mcr.microsoft.com/dotnet/aspnet:9.0-alpine@sha256:89a7a398c5acaa773642cfabd6456f33e29687c1529abfaf068929ff9991cb66
WORKDIR /app
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=0
RUN apk add icu-libs
COPY --from=build-env /app .

LABEL org.opencontainers.image.source="https://github.com/nmshd/backbone"

ENTRYPOINT ["dotnet", "Backbone.Modules.Challenges.Jobs.Cleanup.dll"]
