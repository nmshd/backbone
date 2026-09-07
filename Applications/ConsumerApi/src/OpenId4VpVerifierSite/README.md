# OpenID4VP Verifier Site

This Vite application builds the static OpenID4VP verifier bundle that is embedded into the Consumer API `/r/{referenceId}` onboarding page.

## Build

```sh
npm install
npm run build
```

The build output is written to `../wwwroot/openid4vp-verifier` and exposes fixed asset names under `/openid4vp-verifier/assets/`.

## Input

The bundle reads the NMSHD reference fragment from the current `/r/{referenceId}` URL. The fragment is base64url encoded and contains `algorithm|key|forIdentity|passwordProtection`. For this first version, only algorithm `3` (`XCHACHA20_POLY1305`) is supported.

The referenced Token or RelationshipTemplate content is fetched from the Consumer API. If the decrypted JSON has `@type: "TokenContentVerifiablePresentation"`, its `value` is verified as a presented credential using the reference id as expected nonce and `defaultPresentationAudience` as expected audience. Otherwise, the original onboarding page is shown.

Optional validation parameters:

- `nonce` or `expected_nonce`: expected presentation challenge.
- `audience` or `client_id`: expected presentation audience. If omitted, the current origin is used.

The implementation performs a minimal browser-side validation using `@credo-ts/core` and `@credo-ts/openid4vc` types. NMSHD token content is decrypted with `@nmshd/crypto`. Credential status checks are disabled for this first version.
