# Build
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build-env
WORKDIR /src
COPY . .

RUN dotnet restore "Backbone.API/Backbone.API.csproj"

RUN dotnet publish /property:WarningLevel=0 --configuration Release --output /app --no-restore "Backbone.API/Backbone.API.csproj"

# Run	
FROM mcr.microsoft.com/dotnet/aspnet:7.0.3-alpine3.16
EXPOSE 80
WORKDIR /app
ENV Logging__Console__FormatterName=
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=0
RUN apk add icu-libs
COPY --from=build-env /app .

LABEL org.opencontainers.image.source = "https://github.com/nmshd/bkb-backbone"

ENTRYPOINT ["dotnet", "Backbone.API.dll"]
