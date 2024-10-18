#!/bin/bash

dotnet restore /p:ContinuousIntegrationBuild=true ../BackBone.sln
dotnet format --no-restore --verify-no-changes ../BackBone.sln
