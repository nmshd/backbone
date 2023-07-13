#!/usr/bin/env -S npx ts-node --esm

import { $ } from "zx";

await $`dotnet restore "Backbone.sln" && dotnet build  /property:WarningLevel=0 --no-restore "Backbone.sln"`;
await $`docker compose -f ./docker-compose/docker-compose.test.yml up consumer-api admin-ui seed-mssql seed-client -d`;
await $`dotnet test --no-restore --no-build "Backbone.sln" --filter "Category=Integration&TestCategory!~ignore"`;
await $`docker compose -f ./docker-compose/docker-compose.test.yml down`;