# OpenID4VP Verifier – Instructions for AI Agents

This file applies to the entire `OpenId4VpVerifierSite` directory. Keep it up to date with every change to the feature. If the architecture, validation rules, supported formats, integration points, build steps, tests, or design requirements change, update this file as part of the same change.

## Purpose and Flow

This Vite/TypeScript bundle adds browser-side verification of presented credentials to the Consumer API page `/r/{referenceId}`.

1. `referenceContent.ts` reads the key from the URL fragment. The fragment is base64url-encoded and contains `algorithm|key|forIdentity|passwordProtection`; currently, only algorithm `3` (`XCHACHA20_POLY1305`) is supported.
2. The encrypted content is loaded through the Consumer API's token or RelationshipTemplate endpoint and decrypted in the browser.
3. Only content with `@type: "TokenContentVerifiablePresentation"` is treated as a presentation. Otherwise, the regular onboarding page remains visible.
4. `verification.ts` validates `value`. The reference ID is used as the expected nonce, while the audience is currently hard-coded to `defaultPresentationAudience`.
5. `main.ts` connects the loading and validation logic to the existing DOM and displays the status and credential contents.

The URL fragment is not sent to the server by the browser. Do not move the decryption key it contains into query parameters or server-side requests.

## File Responsibilities

- `src/referenceContent.ts`: Parse the reference fragment, select API endpoints, and load and decrypt the encrypted content. Errors result in no VP content being returned.
- `src/verification.ts`: Validate and extract the credential data to be displayed. The public entry point is `validatePresentedCredential(presentation, options)`; input in, `VerificationOutcome` out, with no DOM dependency.
- `src/main.ts`: UI orchestration, DOM access, German text, mapping `PresentationValidationErrorCode` values to user-friendly messages, and merging optional `displayInformation`.
- `src/verificationKeyManagement.ts`: Browser KMS exclusively for signature verification with public JWKs. No key generation, private-key import, or signing.
- `src/verificationStorage.ts`: Ephemeral storage/filesystem adapters required to initialize Credo in the browser. No verifier data is stored persistently.
- `src/styles.css`: Verifier styles only; scope selectors beneath `.openid4vp-verifier` or `#openid4vp-verifier-root`.
- `test/verification.test.ts`: Real cryptographic unit tests of the validation logic without mocks.

## Validation Rules

Compact SD-JWT VCs with Key Binding as well as JWT- and JSON-LD-based VCs/VPs are currently supported to the extent that Credo can verify them. A successful status requires:

- at least one presentation or credential;
- the expected nonce and audience to be present;
- valid cryptographic signatures;
- JWT and JSON-LD credentials to be embedded in a request-bound Verifiable Presentation rather than supplied as standalone credentials;
- for SD-JWT, a valid issuer signature, valid disclosures, a valid holder key-binding signature, and a matching `sd_hash`;
- the nonce and audience to match the current verification process;
- a supported SD-JWT type and the required `vct` field;
- a validity period that has already started and has not yet expired;
- at least one embedded credential in a VP;
- every individual item to be valid when multiple presentations or credentials are supplied.

Signer keys are resolved from embedded JWKs, `x5c`, or supported DID URLs. The Credo configuration includes resolvers for `did:key`, `did:jwk`, and `did:web`.

Important intentional limitations:

- Credential status or revocation is not currently checked (`verifyCredentialStatus: false`).
- With `x5c`, the certificate's public key is used to verify the signature; neither the certificate chain nor trust in the issuer is validated.
- A valid signature alone therefore does not mean that the issuer is trusted from a business perspective.

Do not change these limitations casually. Security-relevant extensions require appropriate positive and negative tests.

## Error Handling

Validation results do not contain a free-text error message, but a value from `PresentationValidationErrorCode`. For new error cases, add:

1. a distinct enum value in `verification.ts`;
2. the correct mapping in the validation path;
3. a short, non-technical German UI message in `validationErrorMessages` in `main.ts`;
4. at least one unit test that asserts the exact error code.

Unknown errors must not be treated as valid and are mapped to `VerificationFailed`. Internal technical details, library errors, key material, and complete presentations must not appear in visible error messages.

## UI and Design Requirements

The markup is not located in this Vite project, but in `../Views/AppOnboarding/AppOnboarding.cshtml`. `main.ts` expects the `data-*` elements defined there. Change the markup, TypeScript queries, and CSS together when modifying this DOM contract. Do not create missing elements dynamically as a fallback; the verifier must not initialize when the DOM is incomplete.

All visible text, including error and accessibility text, must be in German. Error messages should be understandable and use minimal technical language. Use `textContent`, not `innerHTML`, for data from presentations.

All presented domain claims should be displayed. Technical metadata listed in `technicalClaimNames` and image fields listed in `imageClaimNames` are intentionally excluded or displayed separately. Hide missing values; do not add invented sample data or display fallbacks. The issuer also controls the “verifiziert durch” section. Optional `displayInformation` from the token can specify the title, logo, and colors.

Design source: [Frosch Wallet App in Figma](https://www.figma.com/design/D15DcZItr1P4lCa61vfOWN/Frosch-Wallet-App?node-id=73604-148644&m=dev). For design changes, use the Figma plugin and consider the desktop screens directly below the linked element; the first two screens there are only for the mobile app. The blue outer frame in Figma represents a smartphone and is not part of the web UI.

## Embedding and Build

- `vite.config.ts` generates fixed filenames under `dist/openid4vp-verifier/assets/verifier.{js,css}` with the base path `/openid4vp-verifier/`.
- `../ConsumerApi.csproj` runs `npm ci` and `npm run build` during regular builds and copies the result to `../wwwroot/openid4vp-verifier`.
- `../Dockerfile` builds the bundle in a dedicated Node stage and copies it into the Consumer API image.
- `../Views/AppOnboarding/AppOnboarding.cshtml` includes CSS and preloads and dynamically imports JavaScript through `IFileVersionProvider` with cache busting. The server-rendered verifier remains visible during normal loading, while import or initialization failures restore the onboarding page.
- `../../../../.github/workflows/test.yml` installs the dependencies and runs `npm test` in the unit-test job.

`node_modules/`, `dist/`, and `../wwwroot/openid4vp-verifier/` are generated or copied artifacts. Do not edit or commit them directly. Changes belong in `src/`, the Razor markup, or the build configuration. When dependencies change, `package-lock.json` must be updated together with `package.json`.

## Tests and Local Verification

Run at least the following commands in the verifier directory:

```sh
npm ci
npm run typecheck
npm test
npm run build
```

Tests for `validatePresentedCredential` should remain pure input/output tests without mocks. Generate signed test presentations with real test keys and inject a fixed time through `options.now` so that time checks are deterministic. When making changes, cover the success case as well as tampering, incorrect binding values, time boundaries, missing required data, and arrays containing partially invalid presentations.

When changing the markup or Consumer API embedding, also run the affected .NET integration tests or at least build `../ConsumerApi.csproj`. Do not confuse known warnings from transitive cryptography dependencies during the Vite build with errors, but investigate and document new warnings.
