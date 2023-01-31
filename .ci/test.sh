#!/bin/bash
set -e
set -u
set -x
 
dotnet restore "Backbone.API/Backbone.API.csproj"
dotnet build --no-restore  "Backbone.API/Backbone.API.csproj"
dotnet test --no-restore --no-build "Backbone.sln"
