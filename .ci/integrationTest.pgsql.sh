#!/bin/bash
set -e

docker compose -f ./.ci/docker-compose.test.yml -f ./.ci/docker-compose.test.postgres.yml up -d --build
dotnet restore "Backbone.sln"
dotnet build /property:WarningLevel=0 --no-restore "Backbone.sln"
dotnet test --no-restore --no-build --filter "Category=Integration&TestCategory!~ignore" "Backbone.sln"
docker compose -f ./.ci/docker-compose.test.yml -f ./.ci/docker-compose.test.postgres.yml down
