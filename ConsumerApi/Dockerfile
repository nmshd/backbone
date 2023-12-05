# Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env
WORKDIR /src
COPY . .

RUN dotnet restore "ConsumerApi/ConsumerApi.csproj"

RUN dotnet publish /property:WarningLevel=0 --configuration Release --output /app --no-restore "ConsumerApi/ConsumerApi.csproj"

# Run
FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine3.18
EXPOSE 80
WORKDIR /app

ENV ASPNETCORE_URLS=http://0.0.0.0:80

ENV Logging__Console__FormatterName=
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=0
RUN apk add icu-libs
COPY --from=build-env /app .

LABEL org.opencontainers.image.source = "https://github.com/nmshd/backbone"

ENTRYPOINT ["dotnet", "Backbone.ConsumerApi.dll"]
