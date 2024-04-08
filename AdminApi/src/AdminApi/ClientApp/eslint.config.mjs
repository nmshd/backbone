import globals from "globals";

export default [
    ({ files: ["./src/**/*.ts"], languageOptions: { sourceType: "module" } },
    {
        languageOptions: {
            globals: {
                ...globals.es2020,
                ...globals.node
            },
            parserOptions: {
                allowAutomaticSingleRunInference: true,
                cacheLifetime: {
                    glob: "Infinity"
                },
                project: ["tsconfig.json", "packages/*/tsconfig.json", "packages/scope-manager/tsconfig.build.json", "packages/scope-manager/tsconfig.spec.json"],
                tsconfigRootDir: ".",
                warnOnUnsupportedTypeScriptVersion: false
            }
        }
    })
];
