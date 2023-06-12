#!/usr/bin/env -S npx ts-node --esm -T
import { $ } from "zx";
import { getRequiredEnvVar } from "../lib.js";

const tag = getRequiredEnvVar("TAG");

await $`docker build --file ./AdminUI/Dockerfile --tag ghcr.io/nmshd/backbone-admin-ui:${tag} ./AdminUI`;
