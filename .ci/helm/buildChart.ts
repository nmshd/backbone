#!/usr/bin/env -S npx ts-node --esm -T
import { $ } from "zx";
import { getRequiredEnvVar } from "../lib.js";
import { readFile, writeFile, copyFile, rename, rm } from "fs/promises";

const version = getRequiredEnvVar("VERSION");

const HELM_CHART_FILENAME = `helm/Chart.yaml`;
const HELM_CHART_VERSION_PLACEHOLDER = `<chartVersion>`;

await backupChart();
await fillVersionPlaceholderInChart();
await $`helm dependency update helm`;
await $`helm package helm`;
await restoreBackupOfChart();

async function backupChart() {
  await copyFile(HELM_CHART_FILENAME, `${HELM_CHART_FILENAME}.bak`);
}

async function fillVersionPlaceholderInChart() {
  const chartFile = (await readFile(HELM_CHART_FILENAME))
    .toString("utf-8")
    .replace(HELM_CHART_VERSION_PLACEHOLDER, version);

  await writeFile(HELM_CHART_FILENAME, chartFile);
}

async function restoreBackupOfChart() {
  await rm(HELM_CHART_FILENAME);
  await rename(`${HELM_CHART_FILENAME}.bak`, HELM_CHART_FILENAME);
}
