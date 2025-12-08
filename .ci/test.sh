#!/bin/bash

dotnet restore /p:ContinuousIntegrationBuild=true "Backbone.slnx"
dotnet build /p:ContinuousIntegrationBuild=true --no-restore "Backbone.slnx"
dotnet test /p:ContinuousIntegrationBuild=true --no-restore --no-build --filter "Category!=Integration" --solution "Backbone.slnx"
