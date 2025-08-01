#### Build chisel binary ####
################################################################################################################
# The following lines are required to install libicu, which itself is required for the SQL Server client to work 
# (see https://github.com/dotnet/dotnet-docker/discussions/4527).
# They are copied from https://github.com/ubuntu-rocks/dotnet/issues/21
################################################################################################################
FROM golang:1.24@sha256:ef5b4be1f94b36c90385abd9b6b4f201723ae28e71acacb76d00687333c17282 AS chisel

RUN git clone --depth 1 -b main https://github.com/canonical/chisel /opt/chisel
WORKDIR /opt/chisel
RUN go build ./cmd/chisel

#### Build application ####
FROM mcr.microsoft.com/dotnet/sdk:9.0-noble@sha256:14fad15702d25fec7ff1532b762abd323fc7549c9b8a713b5871b88dc989053d AS build-env

# use chisel to install libicu
RUN apt-get update \
    && apt-get install -y fdupes \
    && rm -rf /var/lib/apt/lists/*

COPY --from=chisel /opt/chisel/chisel /usr/bin/
COPY --from=mcr.microsoft.com/dotnet/runtime:9.0-noble-chiseled@sha256:50c77c03cc6f0b56c435b14e4337f3be7c2be61ed7720dd487dc37e200a2791a / /runtime-ref

RUN mkdir /rootfs \
    && chisel cut --release "ubuntu-24.04" --root /rootfs \
    libicu74_libs \
    \
    # Remove duplicates from rootfs that exist in runtime-ref
    && fdupes /runtime-ref /rootfs -rdpN \
    \
    # Delete duplicate symlinks
    # Function to find and format symlinks w/o including root dir (format: /path/to/symlink /path/to/target)
    && getsymlinks() { find $1 -type l -printf '%p %l\n' | sed -n "s/^\\$1\\(.*\\)/\\1/p"; } \
    # Combine set of symlinks between rootfs and runtime-ref
    && (getsymlinks "/rootfs"; getsymlinks "/runtime-ref") \
    # Sort them
    | sort \
    # Find the duplicates
    | uniq -d \
    # Extract just the path to the symlink
    | cut -d' ' -f1 \
    # Prepend the rootfs directory to the paths
    | sed -e 's/^/\/rootfs/' \
    # Delete the files
    | xargs rm \
    \
    # Delete empty directories
    && find /rootfs -type d -empty -delete
################################################################################################################
# end of libicu-related stuff
################################################################################################################

WORKDIR /src
COPY . .

ARG VERSION

RUN dotnet restore /p:ContinuousIntegrationBuild=true "Applications/SseServer/src/SseServer/SseServer.csproj"
RUN dotnet publish /p:ContinuousIntegrationBuild=true --configuration Release --output /app/publish --no-restore "Applications/SseServer/src/SseServer/SseServer.csproj"
RUN dotnet publish /p:ContinuousIntegrationBuild=true --configuration Release --output /app/publish/health "Applications/HealthCheck/src/HealthCheck.csproj"

### Final ####
FROM mcr.microsoft.com/dotnet/aspnet:9.0-noble-chiseled@sha256:519d28b1f35e7060f5ae53d0e43e9522b59522dab51ecef6465af60b94b3ad92

LABEL org.opencontainers.image.source="https://github.com/nmshd/backbone"
HEALTHCHECK --interval=30s --timeout=3s --start-period=10s --retries=3 CMD [ "dotnet", "/app/health/Backbone.HealthCheck.dll", "--", "http://localhost:8080/health" ]
EXPOSE 8080

ENV ASPNETCORE_URLS=http://0.0.0.0:8080
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=0

WORKDIR /app

COPY --from=build-env /rootfs /
COPY --from=build-env /app/publish .

USER $APP_UID

ENTRYPOINT ["dotnet", "Backbone.SseServer.dll"]
