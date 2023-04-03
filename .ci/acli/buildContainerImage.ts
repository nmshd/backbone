#!/usr/bin/env -S npx ts-node --esm -T
import { $ } from "zx";
import { getRequiredEnvVar } from "../lib.js";

const tag = getRequiredEnvVar("TAG");

await $`docker build --file ./Modules/Devices/src/Devices.AdminCli/Dockerfile --tag ghcr.io/nmshd/backbone-admin-cli:${tag} .`;
