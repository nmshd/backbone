#!/usr/bin/env node

import { $ } from "zx";
import { getRequiredEnvVar } from "../lib.js";

const version = getRequiredEnvVar("VERSION");

await $`Executables/helm dependency update helm`;

await $`Executables/helm package --version ${version} --app-version ${version} helm`;
