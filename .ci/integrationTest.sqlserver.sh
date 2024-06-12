#!/bin/bash
set -e

dockerCompose() {
    docker compose -f ./.ci/docker-compose.test.yml -f ./.ci/docker-compose.test.sqlserver.yml "$@"
}

{
  dockerCompose down;
  dockerCompose up -d --no-build
} &
{
  dotnet restore "Backbone.sln";
  dotnet build --no-restore "Backbone.sln"
}
wait

export CONSUMER_API_BASE_ADDRESS="http://localhost:5000"
export ADMIN_API_BASE_ADDRESS="http://localhost:5173"

dotnet test --no-restore --no-build --filter "Category=Integration&TestCategory!~ignore" --logger "GitHubActions;summary.includePassedTests=true" "Backbone.sln"
