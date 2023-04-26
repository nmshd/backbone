#!/usr/bin/env -S npx ts-node --esm -T
import { $ } from "zx";
import { getRequiredEnvVar } from "../lib.js";

const tag = getRequiredEnvVar("TAG");

await $`helm push backbone-helm-chart-${tag}.tgz oci://ghcr.io/nmshd`;
