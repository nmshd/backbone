#!/usr/bin/env -S npx ts-node --esm
import { $ } from "zx";
import { getRequiredEnvVar } from "../lib.js";

const tag = getRequiredEnvVar("TAG");

await $`docker push ghcr.io/nmshd/backbone-admin-api:${tag}`;
