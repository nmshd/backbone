#!/bin/bash

echo "Shell"
pwd

dotnet restore /p:ContinuousIntegrationBuild=true "Backbone.sln"
dotnet build /p:ContinuousIntegrationBuild=true --no-restore "Backbone.sln"
dotnet test /p:ContinuousIntegrationBuild=true --no-restore --no-build --filter "Category!=Integration" --logger "GitHubActions;summary.includeNotFoundTests=false" "Backbone.sln"
