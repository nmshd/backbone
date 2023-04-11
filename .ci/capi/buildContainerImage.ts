#!/usr/bin/env -S npx ts-node --esm -T
import { $ } from "zx";
import { getRequiredEnvVar } from "../lib.js";

const tag = getRequiredEnvVar("TAG");

await $`docker build --file ./Backbone.API/Dockerfile --tag ghcr.io/nmshd/backbone:${tag} --tag ghcr.io/nmshd/backbone-consumer-api:${tag} .`;
