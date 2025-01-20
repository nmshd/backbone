#!/bin/bash

dotnet restore /p:ContinuousIntegrationBuild=true ./Backbone.sln
dotnet format --no-restore --verify-no-changes ./Backbone.sln
