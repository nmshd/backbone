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
