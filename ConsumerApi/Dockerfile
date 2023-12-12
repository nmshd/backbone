#### Build chisel binary ####
FROM golang:1.20 as chisel

RUN git clone --depth 1 -b main https://github.com/canonical/chisel /opt/chisel
WORKDIR /opt/chisel
RUN go build ./cmd/chisel

### Build application ####
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env

# copy chisel binary
COPY --from=chisel /opt/chisel/chisel /usr/bin/
# use chisel to install libicu
RUN mkdir /rootfs \
    && chisel cut --release "ubuntu-22.04" --root /rootfs \
    libicu70_libs

WORKDIR /src
COPY . .

RUN dotnet restore "ConsumerApi/ConsumerApi.csproj"
RUN dotnet publish /property:WarningLevel=0 --configuration Release --output /app/publish --no-restore "ConsumerApi/ConsumerApi.csproj"
RUN dotnet publish --configuration Release --output /app/publish/health "HealthCheck/HealthCheck.csproj"

### Final ####
FROM mcr.microsoft.com/dotnet/aspnet:8.0.0-jammy-chiseled

LABEL org.opencontainers.image.source = "https://github.com/nmshd/backbone"
HEALTHCHECK --interval=30s --timeout=3s --start-period=5s --retries=3 CMD [ "dotnet", "/app/publish/health/Backbone.HealthCheck.dll", "--", "http://localhost:8080/health" ]
EXPOSE 8080

ENV ASPNETCORE_URLS=http://0.0.0.0:8080
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=0

WORKDIR /app

COPY --from=build-env /rootfs /
COPY --from=build-env /app/publish .

USER $APP_UID

ENTRYPOINT ["dotnet", "Backbone.ConsumerApi.dll"]
