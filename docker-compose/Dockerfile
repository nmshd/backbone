FROM mcr.microsoft.com/dotnet/sdk:10.0@sha256:fe0fd4e93bd2fbb47c883b7a6369e5c8d3db8bf01cdc5981f6ee35fbdf3ff4fb

RUN apt-get update \
    && apt-get install --yes --no-install-recommends curl unzip \
    && curl --fail --silent --show-error --location https://aka.ms/getvsdbgsh --output /tmp/getvsdbg.sh \
    && bash /tmp/getvsdbg.sh -v latest -l /vsdbg \
    && rm --recursive --force /var/lib/apt/lists/* /tmp/getvsdbg.sh \
    && mkdir --parents /artifacts /src

ENV ASPNETCORE_ENVIRONMENT=Local \
    ASPNETCORE_URLS=http://0.0.0.0:8080 \
    DOTNET_ENVIRONMENT=Local \
    DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=0

WORKDIR /src

ENTRYPOINT ["dotnet", "watch"]
