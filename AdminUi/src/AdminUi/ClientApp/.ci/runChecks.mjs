#!/usr/bin/env zx

$.verbose = false;
$.nothrow = true;
process.env.FORCE_COLOR = 3;

console.log("Running checks...");
await $`npm ci`;

console.log("Running eslint check...");
try {
    var eslint = await $`npx eslint --ext ts ./src`;
    console.log(eslint.stdout);
} catch (err) {
    console.log(err.stdout);
    console.error(err.stderr);
}

console.log("Running prettier check...");
try {
    var prettier = await $`npx prettier --check .`;
    console.log(prettier.stdout);
} catch (err) {
    console.log(err.stdout);
    console.error(err.stderr);
}

console.log("Running license check...");
try {
    var licenseCheck = await $`npx license-check`;
    console.log(licenseCheck.stdout);
} catch (err) {
    console.log(err.stdout);
    console.error(err.stderr);
}

console.log("Running audit checks...");
try {
    var audit = await $`npx better-npm-audit audit`;
    console.log(audit.stdout);
} catch (err) {
    console.log(err.stdout);
    console.error(err.stderr);
}
