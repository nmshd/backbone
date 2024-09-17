#!/bin/bash

dotnet restore /p:ContinuousIntegrationBuild=true
dotnet format --no-restore --verify-no-changes
