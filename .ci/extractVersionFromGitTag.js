#!/usr/bin/env node

import { echo } from "zx";
import { getRequiredEnvVar } from "./lib.js";

const gitTag = getRequiredEnvVar("GIT_TAG");

// const semVerRegex =
//   /(0|[1-9]\d*)\.(0|[1-9]\d*)\.(0|[1-9]\d*)(?:-((?:0|[1-9]\d*|\d*[a-zA-Z-][0-9a-zA-Z-]*)(?:\.(?:0|[1-9]\d*|\d*[a-zA-Z-][0-9a-zA-Z-]*))*))?(?:\+([0-9a-zA-Z-]+(?:\.[0-9a-zA-Z-]+)*))?/;
// const gitTagRegex = new RegExp(`^.+/(${semVerRegex.source})$`);
// const dockerTag = gitTagRegex.exec(gitTag)?.[1];

const splittedGitTag = gitTag.split("/");
const dockerTag = splittedGitTag.at(-1);

echo(dockerTag ?? "");
