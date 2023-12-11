#!/usr/bin/env node

import { $ } from "zx";
import { getRequiredEnvVar } from "../lib.js";

const tag = getRequiredEnvVar("TAG");

await $`docker push ghcr.io/nmshd/backbone-files-sanity-check:${tag}`;
