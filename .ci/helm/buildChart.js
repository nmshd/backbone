#!/usr/bin/env node

import { $ } from "zx";
import { getRequiredEnvVar } from "../lib.js";

const version = getRequiredEnvVar("VERSION");

await $`helm dependency update helm`;

// replace <<app_version>> in all files in helm/**/Chart.yaml with the value of `version`
await $`find helm -name Chart.yaml -exec sed -i -e 's/__app_version__/${version}/g' {} +`;

await $`helm package --version ${version} helm`;
