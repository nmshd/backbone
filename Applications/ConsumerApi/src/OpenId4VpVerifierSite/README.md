# OpenID4VP Verifier Site

This Vite application builds the static OpenID4VP verifier page that is served by the Consumer API from `/openid4vp-verifier/`.

## Build

```sh
npm install
npm run build
```

The build output is written to `../wwwroot/openid4vp-verifier`.

## Input

The page reads an OpenID4VP authorization response from query or fragment parameters. It currently supports `vp_token`, `presentation`, or `verifiablePresentation`.

Optional validation parameters:

- `nonce` or `expected_nonce`: expected presentation challenge.
- `audience` or `client_id`: expected presentation audience. If omitted, the current origin is used.

The implementation performs a minimal browser-side validation using `@credo-ts/core` and `@credo-ts/openid4vc` types. Credential status checks are disabled for this first version.
