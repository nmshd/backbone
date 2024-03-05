#!/bin/bash

dotnet restore "Backbone.sln"`;
dotnet build  /property:WarningLevel=0 --no-restore "Backbone.sln"
dotnet test --no-restore --no-build --filter "Category!=Integration" --logger "GitHubActions;summary.includePassedTests=true;summary.includeSkippedTests=true" "Backbone.sln"
