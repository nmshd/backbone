#!/bin/bash

dotnet restore /p:ContinuousIntegrationBuild=true "Backbone.slnx"
dotnet build /p:ContinuousIntegrationBuild=true --no-restore "Backbone.slnx"
dotnet test /p:ContinuousIntegrationBuild=true --no-restore --no-build --ignore-exit-code 8 --filter-not-trait "Category=Integration" --solution "Backbone.slnx"
