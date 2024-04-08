module.exports = [
    {
        ignores: ["/*", "!/src"],
        // extends: ["@js-soft/ts", "@js-soft/ts/mocha"],
        files: ["@js-soft/ts", "@js-soft/ts/mocha"],
        parserOptions: {
            project: ["tsconfig.json"]
        }
    }
];
