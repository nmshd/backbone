#!/usr/bin/env -S npx ts-node --esm

import { $ } from "zx";

await $`docker compose -f ./docker-compose/docker-compose.test.yml -f ./docker-compose/docker-compose.test.mssql.yml up -d`;
await $`dotnet restore "Backbone.sln"`;
await $`dotnet build /property:WarningLevel=0 --no-restore "Backbone.sln"`;
await $`dotnet test --no-restore --no-build --filter "Category=Integration&TestCategory!~ignore" "Backbone.sln"`;
await $`docker compose -f ./docker-compose/docker-compose.test.yml -f ./docker-compose/docker-compose.test.mssql.yml down`;
