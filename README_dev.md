# Prerequisites

1. Download and install [Docker Desktop](https://www.docker.com/products/docker-desktop)
2. Download and install the latest version of the [.NET SDK](https://dotnet.microsoft.com/en-us/download)

# How to run

Start the infrastructure and the four backend services with hot reload from the
root directory of the repository:

```bash
docker compose \
  -f ./docker-compose/compose.yml \
  up --detach --build \
  admin-api consumer-api event-handler-service sse-server
```

In Rider, start the `docker-compose (with Postgres)` configuration with **Run**
and attach to the desired `Backbone.*` child process via **Run | Attach to
Process**. In VS Code, use the `Start Backbone` task or the `Docker Watch: Debug
Backbone` debug configuration.

See [docker-compose/README.md](docker-compose/README.md) for debugging,
hot-reload behavior, shutdown, and the on-demand tools.
