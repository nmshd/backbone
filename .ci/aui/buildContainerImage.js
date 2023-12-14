#!/usr/bin/env node

import { $ } from "zx";
import { getRequiredEnvVar } from "../lib.js";

const tag = getRequiredEnvVar("TAG");

await $`docker build --file ./AdminUi/src/AdminUi/Dockerfile --tag ghcr.io/nmshd/backbone-admin-ui:${tag} .`;
