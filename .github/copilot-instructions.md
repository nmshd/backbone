# Copilot Instructions for Backbone Repository

## Repository Overview

**Enmeshed Backbone** is a comprehensive .NET-based cloud infrastructure platform that provides central services for the Enmeshed ecosystem. It consists of multiple services, libraries, feature modules, and a Flutter-based administrative UI.

-   **Repository Size**: Large monorepo with 100+ projects across multiple layers
-   **Primary Language**: C# (.NET 10.0)
-   **Secondary Language**: Dart (Flutter for Admin UI)
-   **Architecture Style**: Clean Architecture + CQRS + Domain-Driven Design
-   **Primary Target Runtime**: .NET 10.0 (net10.0)
-   **Database Support**: PostgreSQL and SQL Server
-   **Deployment**: Docker containers (8+ services)
-   **Framework**: ASP.NET Core 10 for APIs, Entity Framework Core 10 for data access

## Project Structure

```
Backbone/
├── Applications/              # 10 services
│   ├── AdminApi/             # Admin management API (REST)
│   ├── AdminCli/             # Admin command-line tool
│   ├── AdminUi/              # Admin UI (Flutter)
│   ├── ConsumerApi/          # Consumer-facing API (REST)
│   ├── DatabaseMigrator/     # EF Core migrations runner
│   ├── EventHandlerService/  # Event processing background service
│   ├── HealthCheck/          # Health check endpoint
│   ├── Housekeeper/          # Data cleanup background service
│   ├── IdentityDeletionJobs/ # Identity cleanup jobs
│   └── SseServer/            # Server-Sent Events service
├── BuildingBlocks/           # Shared libraries (API, Application, Domain, Infrastructure, SDK)
├── Common/                   # Common.Infrastructure library
├── Modules/                  # 11 feature modules (Announcements, Challenges, Devices, Files, Messages, etc.)
├── Sdks/                     # Generated SDKs for APIs
└── .github/workflows/        # CI/CD pipelines
```

## Key Dependencies & Versions

-   **.NET SDK**: 10.0.101 (pinned in `global.json`)
-   **Testing Framework**: xUnit 3 with Shouldly assertions, AutoFixture
-   **ORM**: Entity Framework Core 10.0.1
-   **API Framework**: ASP.NET Core 10, OData 9.4.1, Asp.Versioning 8.1.1
-   **BDD/Integration Tests**: Reqnroll (formerly SpecFlow)
-   **Database Drivers**: Npgsql 10.0.0, SQL Server provider 10.0.1
-   **Messaging**: Azure Service Bus, RabbitMQ
-   **Cloud Storage**: Azure Blob Storage, Google Cloud Storage, AWS S3
-   **Security**: OpenIddict 7.2.0, Autofac 9.0.0
-   **Telemetry**: Serilog, OpenTelemetry
-   **UI (Flutter)**: Flutter stable channel, Melos 7.3.0+
-   **Node.js**: 24.12.0 (for build scripts and CI)

## Build & Compilation

### Bootstrap (One-Time Setup)

**Always run this first when cloning or pulling major changes:**

```pwsh
# Create required Docker volumes for local development
docker volume create --name=postgres-volume
docker volume create --name=azure-storage-emulator-volume

# Restore NuGet packages and lock files (required with RestoreLockedMode)
dotnet restore /p:ContinuousIntegrationBuild=true "Backbone.slnx"
```

**Critical Settings**:

-   Central package management is enabled (`Directory.Packages.props`)
-   Locked mode is enforced in CI builds (`RestoreLockedMode=true`)
-   All projects use `net10.0` target framework
-   Implicit usings are enabled in all projects
-   Nullable reference types are enabled (TreatWarningsAsErrors=true)

### Build

**Standard build (required before running tests):**

```pwsh
# Restore dependencies
dotnet restore /p:ContinuousIntegrationBuild=true "Backbone.slnx"

# Compile the solution
dotnet build /p:ContinuousIntegrationBuild=true --no-restore "Backbone.slnx"
```

**Build Behavior**:

-   Solution file is `Backbone.slnx` (new .NET format)
-   Pre-build targets copy `appsettings.override.json` in Debug configuration
-   Build warnings are treated as errors (`TreatWarningsAsErrors=true`)
-   Expect ~19-25 seconds for full build on modern hardware

### Code Formatting & Style

**Always run before committing:**

```pwsh
# Check formatting (fails if changes needed)
dotnet format --no-restore --verify-no-changes

# Auto-fix formatting
dotnet format --no-restore
```

**Code Style Configuration**:

-   `.editorconfig` defines all C# conventions (4-space indentation, 200-character line limit)
-   C# 12 features enabled (file-scoped namespaces as suggestion, implicit usings)
-   Imports sorted, modifiers ordered: public, private, protected, internal, file, static, abstract, virtual, sealed, readonly, override, extern, unsafe, volatile, async, required
-   Single-line blocks preserved, space after cast disabled

## Testing

### Unit Tests (Fastest - Run First)

**Run all unit tests excluding integration tests:**

```pwsh
# Build first (required)
dotnet restore /p:ContinuousIntegrationBuild=true "Backbone.slnx"
dotnet build /p:ContinuousIntegrationBuild=true --no-restore "Backbone.slnx"

# Run unit tests only
dotnet test /p:ContinuousIntegrationBuild=true --no-restore --no-build `
  --filter-not-trait "Category=Integration" --solution "Backbone.slnx"
```

**Expected**:

-   Tests use xUnit 3 test framework
-   Assertions use Shouldly library
-   GitHubActionsTestLogger captures output
-   Unit tests should run in < 2 minutes
-   Exit code 8 is ignored (allowed, used for skipped tests)

### Integration Tests (Requires Docker)

**Prerequisites**: Docker running with postgres-volume created

```pwsh
# Run integration tests only
dotnet test /p:ContinuousIntegrationBuild=true --no-restore --no-build `
  --filter "Category=Integration" --solution "Backbone.slnx"
```

**Important**:

-   Integration tests use BDD with Reqnroll (.feature files)
-   Tests connect to PostgreSQL or SQL Server in Docker
-   Feature files have code-behind code that must be regenerated after edits
-   If code-behind files are out of sync, run `dotnet clean` then `dotnet build`

### Complete Test Pipeline (Replicates CI)

```pwsh
# This is what GitHub Actions runs
bash ./.ci/test.sh
```

Which does:

```pwsh
dotnet restore /p:ContinuousIntegrationBuild=true "Backbone.slnx"
dotnet build /p:ContinuousIntegrationBuild=true --no-restore "Backbone.slnx"
dotnet test /p:ContinuousIntegrationBuild=true --no-restore --no-build `
  --ignore-exit-code 8 --filter-not-trait "Category=Integration" --solution "Backbone.slnx"
```

## Admin UI (Flutter)

### Bootstrap

```bash
cd Applications/AdminUi

# Activate and bootstrap Melos (monorepo tool for Flutter)
dart pub global activate melos
melos bootstrap

# Generate translations (required if l10n files changed)
melos generate_translations
```

### Run Checks (Validates code in CI)

```bash
cd Applications/AdminUi

# This replicates the GitHub Actions check
dart pub global activate melos
melos bootstrap
melos generate_translations
flutter analyze
melos format
```

**Configuration**: Create `AdminUI/config.local.json` with your environment settings.

## Running Applications Locally

### Prerequisites

```bash
# Start Docker containers (PostgreSQL, storage emulator)
docker compose -f ./docker-compose/docker-compose.yml up -d
```

### Run Services (Each in separate terminal)

```pwsh
# Terminal 1: Consumer API (REST endpoint for users)
dotnet run --project ./Applications/ConsumerApi/src/ConsumerApi.csproj

# Terminal 2: Admin API (REST endpoint for administrators)
dotnet run --project ./Applications/AdminApi/src/AdminApi/AdminApi.csproj

# Terminal 3: Event Handler (background service for async events)
dotnet run --project ./Applications/EventHandlerService/src/EventHandlerService/EventHandlerService.csproj
```

Services expose OpenAPI/Swagger documentation at `/docs/{version}/openapi.json`.

## CI/CD Pipeline & Validation

The repository enforces strict quality gates on all pull requests:

### Pre-Commit Checks (Automated on PR)

**.github/workflows/check-pr.yml**:

-   Validates PR has required labels (breaking-change, bug, chore, ci, dependencies, documentation, enhancement, refactoring, test)

**.github/workflows/test.yml** (runs on every push to non-main branches):

1. **Admin UI Checks** (./.ci/aui/runChecks.sh) - Flutter analysis & formatting
2. **Formatting Check** (./.ci/checkFormatting.sh) - dotnet format verification
3. **Unit Tests** (./.ci/test.sh) - xUnit tests excluding integration tests
4. **Docker Image Builds** - Builds all 8 container images to verify Dockerfiles

All workflows use Ubuntu latest and cache NuGet/npm dependencies.

## Known Issues & Workarounds

### Reqnroll Code-Behind Files Warning

**Issue**: Build shows warning about old code-behind files in obj/ folder
**Workaround**: This is safe to ignore; files are auto-managed. If features don't compile, run: `dotnet clean` followed by `dotnet build`

### Database Migration Code-Behind

If you modify `.feature` files in ConsumerApi or AdminApi integration tests:

```pwsh
dotnet clean  # Remove old code-behind
dotnet build  # Regenerate from .feature files
```

### AdminUI Dart Pub Cache

If Flutter commands fail, clear cache:

```bash
flutter clean
dart pub cache clean
melos bootstrap
```

## Architecture Notes

-   **Clean Architecture**: Projects organized by layer (Domain, Application, Infrastructure, API)
-   **CQRS Pattern**: MediatR used for command/query handlers
-   **Domain Events**: Strong typing with IEvent, published via event handlers
-   **Module System**: Each feature module is independently testable with its own domain, application, and infrastructure layers
-   **Database Abstractions**: Support for PostgreSQL and SQL Server with database-specific implementations
-   **Strongly-Typed IDs**: Custom generated strongly-typed ID types prevent type confusion

## Important Files

| File                                | Purpose                                                                       |
| ----------------------------------- | ----------------------------------------------------------------------------- |
| `Backbone.slnx`                     | Main solution file (modern format)                                            |
| `Directory.Build.props`             | Global project settings (net10.0, implicit usings, nullable refs, code style) |
| `Directory.Packages.props`          | Central NuGet version management, locked mode for reproducible builds         |
| `global.json`                       | .NET SDK version pinning (10.0.101)                                           |
| `.editorconfig`                     | Code style rules (4-space indentation, line limits, C# conventions)           |
| `.ci/checkFormatting.sh`            | Format validation script                                                      |
| `.ci/test.sh`                       | Complete test pipeline script                                                 |
| `.ci/aui/runChecks.sh`              | Flutter UI validation script                                                  |
| `.github/workflows/test.yml`        | Main CI workflow                                                              |
| `appsettings.override.json`         | Environment-specific configuration (copied to Debug projects)                 |
| `docker-compose/docker-compose.yml` | Local infrastructure (Postgres, storage emulator)                             |

## Trust These Instructions

When implementing changes:

1. **Always follow the build sequence**: restore → build → test
2. **Always run format check**: before considering work complete
3. **Always use the Backbone.slnx solution file** - not individual projects
4. **Always use /p:ContinuousIntegrationBuild=true for CI-like builds** to verify compatibility
5. **Always run the complete ./.ci/test.sh script** before assuming tests pass

Only perform additional searches if:

-   Instructions mention "see documentation" or reference external sources
-   You need specific implementation details for a particular module
-   A command fails with an error not explained in these instructions
