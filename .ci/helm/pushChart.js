#!/usr/bin/env node

import { $ } from "zx";
import { getRequiredEnvVar } from "../lib.js";

const version = getRequiredEnvVar("VERSION");

await $`helm push backbone-helm-chart-${version}.tgz oci://ghcr.io/nmshd`;
