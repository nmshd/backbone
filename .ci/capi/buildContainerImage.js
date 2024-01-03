#!/usr/bin/env node

import { $ } from "zx";
import { getRequiredEnvVar } from "../lib.js";

const tag = getRequiredEnvVar("TAG");

await $`docker build --file ./ConsumerApi/Dockerfile --tag ghcr.io/nmshd/backbone-consumer-api:${tag} .`;
