#!/usr/bin/env node

import { $ } from "zx";
import { getRequiredEnvVar } from "../lib.js";

const version = getRequiredEnvVar("VERSION");

await $`helm dependency update Applications/helm`;

await $`helm package --version ${version} --app-version ${version} Applications/helm`;
