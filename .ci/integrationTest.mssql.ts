#!/usr/bin/env -S npx ts-node --esm

import { $ } from "zx";

await $`docker compose -f ./docker-compose/docker-compose.test.mssql.yml -f ./docker-compose/docker-compose.test.yml up -d`;
await $`dotnet restore "Backbone.sln"`;
await $`dotnet build /property:WarningLevel=0 --no-restore "Backbone.sln"`;
await $`dotnet test --no-restore --no-build "Backbone.sln" --filter "Category=Integration&TestCategory!~ignore"`;
await $`docker compose -f ./docker-compose/docker-compose.test.mssql.yml -f ./docker-compose/docker-compose.test.yml down`;
