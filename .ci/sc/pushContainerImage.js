#!/usr/bin/env node

import { $ } from "zx";
import { getRequiredEnvVar, toCamelCase } from "../lib.js";

const tag = getRequiredEnvVar("TAG");
const moduleName = getRequiredEnvVar("MODULE");

const moduleNameCamelCase = toCamelCase(moduleName);

await $`docker push ghcr.io/nmshd/backbone-${moduleNameCamelCase}-sanity-check:${tag}`;
