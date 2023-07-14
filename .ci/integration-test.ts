#!/usr/bin/env -S npx ts-node --esm

import { $ } from "zx";

await $`docker compose -f ./docker-compose/docker-compose.test.yml up test-consumer-api test-admin-ui test-seed-mssql test-seed-client -d`;
await $`dotnet test --no-restore --no-build "Backbone.sln" --filter "Category=Integration&TestCategory!~ignore"`;
await $`docker compose -f ./docker-compose/docker-compose.test.yml down`;