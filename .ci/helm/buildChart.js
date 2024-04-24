#!/usr/bin/env node

import { $ } from "zx";
import { getRequiredEnvVar } from "../lib.js";

const version = getRequiredEnvVar("VERSION");

await $`helm dependency update helm`;

// replace <<app_version>> with the value of `version` in all Chart.yaml files in helm folder
await $`find helm -name Chart.yaml -exec sed -i -e 's/__appVersion__/${version}/g' {} +`;

await $`helm package --version ${version} helm`;
