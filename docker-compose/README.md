# Local development with Docker

The local Docker stack runs the Admin API, Consumer API, Event Handler Service,
and SSE Server with `dotnet watch`. The APIs remain available on their
established host ports:

- Consumer API: `http://localhost:8081`
- Admin API: `http://localhost:8082`
- SSE Server: `http://localhost:8083`

The repository is mounted into the development containers. Each service uses a
separate NuGet cache inside the shared Docker artifacts volume, so parallel
restores cannot corrupt one another. C# changes do not require an image rebuild
and do not create container build outputs in the workspace.

The repository's `appsettings.override.json` uses Docker Compose service names
for PostgreSQL, RabbitMQ, Azurite, Seq, OpenTelemetry, and the SSE Server.
Backend applications that use this file must therefore run in the Compose
network.

## Command line

From the repository root, start the watch stack with:

```shell
docker compose \
  -f docker-compose/compose.yml \
  up --detach --build \
  admin-api consumer-api event-handler-service sse-server
```

Stop it without deleting persistent volumes:

```shell
docker compose \
  -f docker-compose/compose.yml \
  down
```

Supported C# edits are applied to the running process automatically. For a
change that cannot be hot-reloaded, `dotnet watch` restarts the affected
application process. Any debugger attached to that process must then be
attached again.

## Rider

Start the `docker-compose (with Postgres)` configuration with **Run**. It loads
`compose.yml` and starts all four backend services and their infrastructure
dependencies.

Rider's native **Debug** action for Docker Compose cannot be used with these
services: it requires a `.dll` or `.exe` in the container command line, whereas
`dotnet watch` must be started with a `.csproj`. To debug a service while
retaining Hot Reload and automatic restarts, use **Run | Attach to Process**,
select the Docker target, and choose the child process whose executable has the
corresponding name:

- `Backbone.AdminApi`
- `Backbone.ConsumerApi`
- `Backbone.EventHandlerService`
- `Backbone.SseServer`

Do not attach to the `dotnet watch` parent process. Reattach after a change that
causes `dotnet watch` to restart the application.

## VS Code

Install the recommended Microsoft C# extension. Select
`Docker Watch: Debug Backbone` and press **F5**. The compound configuration
starts the stack and attaches directly to the matching `Backbone.*` child
process in each container.

Use `Start Backbone` and `Stop Backbone` when no debugger is needed. Individual
`Attach ...` configurations can be used to reattach a single service after an
automatic restart.

## On-demand tools

The tools profile contains applications that should not run whenever the main
stack starts. The base command for each invocation is:

```shell
docker compose \
  -f docker-compose/compose.yml \
  --profile tools run --rm SERVICE [ARGUMENTS]
```

Examples:

```shell
# Admin CLI
docker compose -f docker-compose/compose.yml \
  --profile tools run --rm admin-cli client list

# Database migrations
docker compose -f docker-compose/compose.yml \
  --profile tools run --rm database-migrator

# Housekeeping
docker compose -f docker-compose/compose.yml \
  --profile tools run --rm housekeeper

# Identity deletion workers
docker compose -f docker-compose/compose.yml \
  --profile tools run --rm identity-deletion-job --Worker ActualDeletionWorker
docker compose -f docker-compose/compose.yml \
  --profile tools run --rm identity-deletion-job --Worker SendGracePeriodRemindersWorker
```

Equivalent tasks are available in VS Code. The Admin CLI task prompts for its
arguments.
