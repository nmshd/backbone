#!/usr/bin/env -S npx ts-node --esm -T

import { $ } from "zx";
import { getRequiredEnvVar } from "../lib.js";
import { readFile, writeFile, copyFile, rename, rm } from "fs/promises";

const version = getRequiredEnvVar("VERSION");

await $`helm dependency update helm`;
await $`helm package --version ${version} helm`;
