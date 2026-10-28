FROM mcr.microsoft.com/dotnet/sdk:10.0@sha256:de4fe90a98845d75910b9769662465c84de1a934665c431279d1382b2bddc02b

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
