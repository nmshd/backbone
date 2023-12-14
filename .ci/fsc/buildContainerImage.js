#!/usr/bin/env node

import { $ } from "zx";
import { getRequiredEnvVar } from "../lib.js";

const tag = getRequiredEnvVar("TAG");

await $`docker build --file ./Modules/Files/src/Files.Jobs.SanityCheck/Dockerfile --tag ghcr.io/nmshd/backbone-files-sanity-check:${tag} .`;
