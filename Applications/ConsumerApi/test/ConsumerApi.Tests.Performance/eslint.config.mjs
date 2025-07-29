// @ts-check

import { configs } from "@js-soft/eslint-config-ts";
import { globalIgnores } from "eslint/config";
import tseslint from "typescript-eslint";

export default tseslint.config(globalIgnores(["**/dist", "**/node_modules"]), {
    extends: [configs.base],
    languageOptions: {
        parserOptions: {
            project: ["./tsconfig.json"]
        }
    },
    files: ["src/**/*.ts"],
    rules: {
        "no-console": "off",
        "@typescript-eslint/naming-convention": [
            "error",
            {
                selector: "variableLike",
                format: null,
                custom: {
                    regex: "^(@type|@version|@context|(\\$?[a-zA-Z0-9]+)([A-Z][a-z0-9]*)*)(\\.(@type|@version|@context|[a-z0-9]+([A-Z][a-z0-9\\s]*)*))*$",
                    match: true
                }
            }
        ]
    }
});
