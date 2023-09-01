#!/usr/bin/env -S npx ts-node --esm

import { $ } from "zx";

await $`docker compose -f ./docker-compose/docker-compose.test.postgres.yml up test-consumer-api test-admin-ui test-seed-client -d`;
await $`dotnet restore "Backbone.sln"`;
await $`dotnet build /property:WarningLevel=0 --no-restore "Backbone.sln"`;
await $`dotnet test --no-restore --no-build "Backbone.sln" --filter "Category=Integration&TestCategory!~ignore"`;
await $`docker compose -f ./docker-compose/docker-compose.test.postgres.yml down`;
