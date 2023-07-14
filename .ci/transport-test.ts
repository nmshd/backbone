#!/usr/bin/env -S npx ts-node --esm

import { $ } from "zx";

await $`docker compose -f ./backbone/docker-compose/docker-compose.test.yml up test-consumer-api test-seed-mssql test-seed-client -d`
await $`npm install`

$.env['NMSHD_TEST_BASEURL'] = "http://localhost:5000";
$.env['NMSHD_TEST_CLIENTID'] = "test";
$.env['NMSHD_TEST_CLIENTSECRET'] = "test";
await $`npm run test:local:node:lokijs`

await $`docker compose -f ./backbone/docker-compose/docker-compose.test.yml down`
