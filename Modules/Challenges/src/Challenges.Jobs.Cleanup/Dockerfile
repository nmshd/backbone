# Build
FROM mcr.microsoft.com/dotnet/sdk:9.0-alpine@sha256:33be1326b4a2602d08e145cf7e4a8db4b243db3cac3bdec42e91aef930656080 AS build-env
WORKDIR /src

ARG VERSION

COPY . .
RUN dotnet restore /p:ContinuousIntegrationBuild=true "Challenges.Jobs.Cleanup/Challenges.Jobs.Cleanup.csproj"

WORKDIR "/src/Challenges.Jobs.Cleanup"
RUN dotnet publish /p:ContinuousIntegrationBuild=true --configuration Release --output /app --no-restore

# Run	
FROM mcr.microsoft.com/dotnet/aspnet:9.0-alpine@sha256:3fce6771d84422e2396c77267865df61174a3e503c049f1fe242224c012fde65
WORKDIR /app
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=0
RUN apk add icu-libs
COPY --from=build-env /app .

LABEL org.opencontainers.image.source="https://github.com/nmshd/backbone"

ENTRYPOINT ["dotnet", "Backbone.Modules.Challenges.Jobs.Cleanup.dll"]
