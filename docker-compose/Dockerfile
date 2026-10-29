FROM mcr.microsoft.com/dotnet/sdk:11.0@sha256:3a9175b6c54b2f076817af1360f691c9994447b192faa48a1a1f112a516844c7

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
