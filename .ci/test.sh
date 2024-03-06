#!/bin/bash

dotnet restore "Backbone.sln"
dotnet build --no-restore "Backbone.sln"
dotnet test --no-restore --no-build --filter "Category!=Integration" --logger "GitHubActions" "Backbone.sln"
