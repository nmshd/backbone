#!/usr/bin/env node

import { $ } from "zx";
import { getRequiredEnvVar } from "../lib.js";

const tag = getRequiredEnvVar("TAG");

const platforms = process.env.PLATFORMS ?? "linux/amd64,linux/arm64";
const push = process.env.PUSH === "1" ? ["--push", "--provenance=true", "--sbom=true"] : "";

await $`docker buildx build --file ./Applications/IdentityDeletionJobs/src/Job.IdentityDeletion/Dockerfile --tag ghcr.io/nmshd/backbone-identity-deletion-jobs:${tag} --platform ${platforms} --build-arg VERSION=${tag} ${push} .`;
