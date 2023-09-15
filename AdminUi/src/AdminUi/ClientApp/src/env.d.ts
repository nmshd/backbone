/* eslint-disable @typescript-eslint/naming-convention */
interface ImportMeta {
    readonly env: ImportMetaEnv;
}

interface ImportMetaEnv {
    /**
     * Built-in environment variable.
     * @see Docs https://github.com/chihab/ngx-env#ng_app_env.
     */
    readonly NG_APP_ENV: string;
    // Add your environment variables below
    readonly ADMIN_API__BASE_URL: string;
    [key: string]: any;
}
