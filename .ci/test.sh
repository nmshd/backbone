#!/bin/bash
set -e
set -u
set -x
 
dotnet restore "Backbone.sln"
dotnet build --no-restore "Backbone.sln"
dotnet test --no-restore --no-build "Backbone.sln"
