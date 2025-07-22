# Build
FROM mcr.microsoft.com/dotnet/sdk:9.0-alpine@sha256:2fe880002c458a6e95a3f8bb38b63c0f2e21ffefcb01c0223c4408cc91ad7d9d AS build-env
WORKDIR /src

ARG VERSION

COPY . .
RUN dotnet restore /p:ContinuousIntegrationBuild=true "Challenges.Jobs.Cleanup/Challenges.Jobs.Cleanup.csproj"

WORKDIR "/src/Challenges.Jobs.Cleanup"
RUN dotnet publish /p:ContinuousIntegrationBuild=true --configuration Release --output /app --no-restore

# Run	
FROM mcr.microsoft.com/dotnet/aspnet:9.0-alpine@sha256:91e477e260390e2fc18987e552daf7958491c2e247bf07ae3b876e4f629b6504
WORKDIR /app
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=0
RUN apk add icu-libs
COPY --from=build-env /app .

LABEL org.opencontainers.image.source="https://github.com/nmshd/backbone"

ENTRYPOINT ["dotnet", "Backbone.Modules.Challenges.Jobs.Cleanup.dll"]
