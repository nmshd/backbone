#!/bin/bash

dotnet format /p:ContinuousIntegrationBuild=true --verify-no-changes
