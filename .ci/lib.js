import { $, chalk } from "zx";

export function getRequiredEnvVar(envVarName) {
    var envVar = $.env[envVarName];
    if (!envVar) {
        console.error(chalk.red(`The environment variable '${envVarName}' is required, but was not set.`));
        process.exit(1);
    }
    return envVar;
}

export function toCamelCase(input) {
    const first = input.charAt(0).toLowerCase();
    const remaining = input.substring(1);
    const result = first + remaining;
    return result;
}
