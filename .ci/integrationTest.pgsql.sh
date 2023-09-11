#!/bin/bash
set -e

dockerCompose() {
    docker compose -f ./.ci/docker-compose.test.yml -f ./.ci/docker-compose.test.postgres.yml "$@"
}

dockerCompose down
dockerCompose build
dockerCompose up -d
dotnet restore "Backbone.sln"
dotnet build /property:WarningLevel=0 --no-restore "Backbone.sln"
dotnet test --no-restore --no-build --filter "Category=Integration&TestCategory!~ignore" "Backbone.sln"
