import { chalk, $ } from "zx";

export function getRequiredEnvVar(envVarName: string) {
  var envVar = $.env[envVarName];
  if (!envVar) {
    console.error(
      chalk.red(
        `The environment variable '${envVarName}' is required, but was not set.`
      )
    );
    process.exit(1);
  }
  return envVar;
}

// export function extractVersionFromGitTag(gitTag: string | undefined) {
//   if (!gitTag) return undefined;

//   const semVerRegex =
//     /(0|[1-9]\d*)\.(0|[1-9]\d*)\.(0|[1-9]\d*)(?:-((?:0|[1-9]\d*|\d*[a-zA-Z-][0-9a-zA-Z-]*)(?:\.(?:0|[1-9]\d*|\d*[a-zA-Z-][0-9a-zA-Z-]*))*))?(?:\+([0-9a-zA-Z-]+(?:\.[0-9a-zA-Z-]+)*))?/;
//   const gitTagRegex = new RegExp(`^.+/(${semVerRegex.source})$`);

//   const dockerTag = gitTagRegex.exec(gitTag)?.[1];
//   return dockerTag;
// }
