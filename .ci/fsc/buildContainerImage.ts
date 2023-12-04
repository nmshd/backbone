#!/usr/bin/env -S npx ts-node --esm -T

import { $ } from "zx";
import { getRequiredEnvVar, toCamelCase } from "../lib.js";

const tag = getRequiredEnvVar("TAG");
const moduleName = getRequiredEnvVar("MODULE");
const projectSuffix = $.env["PROJECT_SUFFIX"] ? `.${$.env["PROJECT_SUFFIX"]}` : "";

const moduleNameCamelCase = toCamelCase(moduleName);

await $`docker build --file ./Modules/${moduleName}/src/${moduleName}.Jobs.SanityCheck${projectSuffix}/Dockerfile --tag ghcr.io/nmshd/backbone-${moduleNameCamelCase}-sanity-check:${tag} .`;
