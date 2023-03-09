#!/bin/bash
set -e
set -u
set -x
 
dotnet restore "Backbone.sln"
dotnet build  /property:WarningLevel=0 --no-restore "Backbone.sln"
dotnet test --no-restore --no-build "Backbone.sln" --filter "Category!=Integration"
