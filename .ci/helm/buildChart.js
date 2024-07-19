#!/usr/bin/env node

import { $ } from "zx";
import { getRequiredEnvVar } from "../lib.js";

const version = getRequiredEnvVar("VERSION");

await $`helm dependency update Executables/helm`;

await $`helm package --version ${version} --app-version ${version} Executables/helm`;
