#!/usr/bin/env zx

process.env.FORCE_COLOR = 3;

await $`npm ci`;
await $`npx eslint --ext ts ./src`;
await $`npx prettier --check .`;
await $`npx license-check`;
await $`npx better-npm-audit audit`;
