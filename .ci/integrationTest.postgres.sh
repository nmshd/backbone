#!/bin/bash
set -e

dockerCompose() {
    docker compose -f ./.ci/docker-compose.test.yml -f ./.ci/docker-compose.test.postgres.yml "$@"
}

debugRun() {
    echo
    echo "---------------------------------------------------------------------------------------"
    echo
    echo "$@:"
    
    time $@
}

debugRun dockerCompose down
debugRun dockerCompose build
debugRun dockerCompose up -d
debugRun dotnet restore "Backbone.sln"
debugRun dotnet build --no-restore "Backbone.sln"
export CONSUMER_API_BASE_ADDRESS="http://localhost:5000"
export ADMIN_API_BASE_ADDRESS="http://localhost:5173"
debugRun dotnet test --no-restore --no-build --filter "Category=Integration&TestCategory!~ignore" --logger "GitHubActions;summary.includePassedTests=true;summary.includeSkippedTests=true" "Backbone.sln"
