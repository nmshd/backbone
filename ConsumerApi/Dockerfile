FROM golang:1.20 as chisel

RUN git clone --depth 1 -b main https://github.com/canonical/chisel /opt/chisel
WORKDIR /opt/chisel
RUN go build ./cmd/chisel

# Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env
WORKDIR /src
COPY . .

COPY --from=chisel /opt/chisel/chisel /usr/bin/
RUN mkdir /rootfs \
    && chisel cut --release "ubuntu-22.04" --root /rootfs \
    libicu70_libs

RUN ls

RUN dotnet restore "ConsumerApi/ConsumerApi.csproj"

RUN dotnet publish /property:WarningLevel=0 --configuration Release --output /app --no-restore "ConsumerApi/ConsumerApi.csproj"

RUN dotnet publish --configuration Release --output /app/health "HealthCheck/HealthCheck.csproj"

# Run
FROM mcr.microsoft.com/dotnet/aspnet:8.0.0-jammy-chiseled
EXPOSE 8080
WORKDIR /app

# ENV Logging__Console__FormatterName=
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=0
# RUN apk add icu-libs
COPY --from=build-env /rootfs /
COPY --from=build-env /app .

LABEL org.opencontainers.image.source = "https://github.com/nmshd/backbone"

ENV ASPNETCORE_URLS=http://0.0.0.0:8080

HEALTHCHECK --interval=30s --timeout=3s --start-period=5s --retries=3 CMD [ "dotnet", "/app/health/Backbone.HealthCheck.dll", "--", "http://localhost:8080/health" ]

USER $APP_UID

ENTRYPOINT ["dotnet", "Backbone.ConsumerApi.dll"]
