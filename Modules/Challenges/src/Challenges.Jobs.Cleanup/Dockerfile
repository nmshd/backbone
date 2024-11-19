# Build
FROM mcr.microsoft.com/dotnet/sdk:9.0-alpine AS build-env
WORKDIR /src

ARG VERSION

COPY . .
RUN dotnet restore /p:ContinuousIntegrationBuild=true "Challenges.Jobs.Cleanup/Challenges.Jobs.Cleanup.csproj"

WORKDIR "/src/Challenges.Jobs.Cleanup"
RUN dotnet publish /p:ContinuousIntegrationBuild=true --configuration Release --output /app --no-restore

# Run	
FROM mcr.microsoft.com/dotnet/aspnet:9.0-alpine
WORKDIR /app
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=0
RUN apk add icu-libs
COPY --from=build-env /app .

LABEL org.opencontainers.image.source="https://github.com/nmshd/backbone"

ENTRYPOINT ["dotnet", "Backbone.Modules.Challenges.Jobs.Cleanup.dll"]
