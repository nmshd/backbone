#!/usr/bin/env zx

process.env.FORCE_COLOR = 3;

await $`npm ci`;
await nothrow($`npx eslint --ext ts ./src`);
await nothrow($`npx prettier --check .`);
await nothrow($`npx license-check`);
await nothrow($`npx better-npm-audit audit`);
