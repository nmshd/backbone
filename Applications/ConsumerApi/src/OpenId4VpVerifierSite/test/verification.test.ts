import { DidJwk, Kms, TypedArrayEncoder, sdJwtVcHasher } from "@credo-ts/core";
import { ed25519 } from "@noble/curves/ed25519.js";
import { SDJwtInstance } from "@sd-jwt/core";
import { describe, expect, it } from "vitest";
import { PresentationValidationErrorCode, validatePresentedCredential } from "../src/verification";

const now = new Date("2030-01-01T12:00:00.000Z");
const nowInSeconds = Math.floor(now.getTime() / 1000);
const expectedAudience = "defaultPresentationAudience";
const expectedNonce = "TOK-unit-test-reference";
const issuerKey = createSigningKey(1);
const holderKey = createSigningKey(101);
const otherHolderKey = createSigningKey(201);

describe("validatePresentedCredential", () => {
  it("accepts a correctly signed and request-bound SD-JWT presentation", async () => {
    const presentation = await createPresentation();

    const result = await validate(presentation);

    expect(result.isValid).toBe(true);
    expect(result.errorCode).toBeUndefined();
    expect(result.credential.claims).toEqual(
      expect.arrayContaining([
        { label: "Vorname", value: "Maria" },
        { label: "Nachname", value: "Müller" }
      ])
    );
  });

  it("rejects a presentation whose issuer signature was changed", async () => {
    const presentation = await createPresentation();
    const tamperedPresentation = replacePart(presentation, 0, tamperJwtSignature);

    const result = await validate(tamperedPresentation);

    expect(result.isValid).toBe(false);
    expect(result.errorCode).toBe(PresentationValidationErrorCode.InvalidSignature);
  });

  it("rejects a presentation whose key-binding signature was changed", async () => {
    const presentation = await createPresentation();
    const parts = presentation.split("~");
    let keyBindingIndex = parts.length - 1;
    while (keyBindingIndex > 0 && parts[keyBindingIndex].split(".").length !== 3) {
      keyBindingIndex--;
    }
    const tamperedPresentation = replacePart(presentation, keyBindingIndex, tamperJwtSignature);

    const result = await validate(tamperedPresentation);

    expect(result.isValid).toBe(false);
    expect(result.errorCode).toBe(PresentationValidationErrorCode.InvalidSignature);
  });

  it("rejects a disclosed claim that no longer matches the digest signed by the issuer", async () => {
    const presentation = await createPresentation();
    const parts = presentation.split("~");
    const disclosure = JSON.parse(TypedArrayEncoder.toUtf8String(TypedArrayEncoder.fromBase64Url(parts[1]))) as [string, string, unknown];
    disclosure[2] = "Manipulated";
    parts[1] = TypedArrayEncoder.toBase64Url(TypedArrayEncoder.fromUtf8String(JSON.stringify(disclosure)));

    const result = await validate(parts.join("~"));

    expect(result.isValid).toBe(false);
    expect(result.errorCode).toBe(PresentationValidationErrorCode.InvalidPresentationBinding);
  });

  it("rejects an incorrect sd_hash even if the modified key-binding JWT has a valid holder signature", async () => {
    const presentation = await createPresentation();
    const parts = presentation.split("~");
    const keyBindingIndex = parts.length - 1;
    const [encodedHeader, encodedPayload] = parts[keyBindingIndex].split(".");
    const payload = JSON.parse(TypedArrayEncoder.toUtf8String(TypedArrayEncoder.fromBase64Url(encodedPayload))) as Record<string, unknown>;
    payload.sd_hash = "not-the-hash-of-this-presentation";
    const changedPayload = TypedArrayEncoder.toBase64Url(TypedArrayEncoder.fromUtf8String(JSON.stringify(payload)));
    const unsignedKeyBinding = `${encodedHeader}.${changedPayload}`;
    parts[keyBindingIndex] = `${unsignedKeyBinding}.${await holderKey.signer(unsignedKeyBinding)}`;

    const result = await validate(parts.join("~"));

    expect(result.isValid).toBe(false);
    expect(result.errorCode).toBe(PresentationValidationErrorCode.InvalidPresentationBinding);
  });

  it("rejects a key-binding JWT signed by a key other than the holder key in cnf", async () => {
    const presentation = await createPresentation({ presentationSigningKey: otherHolderKey });

    const result = await validate(presentation);

    expect(result.isValid).toBe(false);
    expect(result.errorCode).toBe(PresentationValidationErrorCode.InvalidSignature);
  });

  it("rejects an SD-JWT without a key-binding presentation", async () => {
    const credential = await createCredential();

    const result = await validate(credential);

    expect(result.isValid).toBe(false);
    expect(result.errorCode).toBe(PresentationValidationErrorCode.MissingKeyBinding);
  });

  it("rejects a correctly signed standalone JWT VC without presentation binding", async () => {
    const credential = await createStandaloneJwtCredential();

    const result = await validate(credential);

    expect(result.isValid).toBe(false);
    expect(result.errorCode).toBe(PresentationValidationErrorCode.MissingKeyBinding);
  });

  it("rejects a standalone JSON-LD VC without presentation binding", async () => {
    const result = await validate({
      "@context": ["https://www.w3.org/2018/credentials/v1"],
      credentialSubject: { id: "did:example:holder", name: "Maria Müller" },
      issuer: "did:example:issuer",
      proof: { type: "DataIntegrityProof" },
      type: ["VerifiableCredential"]
    });

    expect(result.isValid).toBe(false);
    expect(result.errorCode).toBe(PresentationValidationErrorCode.MissingKeyBinding);
  });

  it("rejects a presentation created for a different nonce", async () => {
    const presentation = await createPresentation({ nonce: "TOK-another-reference" });

    const result = await validate(presentation);

    expect(result.isValid).toBe(false);
    expect(result.errorCode).toBe(PresentationValidationErrorCode.NonceMismatch);
  });

  it("rejects a presentation created for a different audience", async () => {
    const presentation = await createPresentation({ audience: "another-audience" });

    const result = await validate(presentation);

    expect(result.isValid).toBe(false);
    expect(result.errorCode).toBe(PresentationValidationErrorCode.AudienceMismatch);
  });

  it("rejects an expired credential even when both signatures are valid", async () => {
    const presentation = await createPresentation({ expiresAt: nowInSeconds - 1 });

    const result = await validate(presentation);

    expect(result.isValid).toBe(false);
    expect(result.errorCode).toBe(PresentationValidationErrorCode.CredentialExpired);
  });

  it("rejects a credential whose validity period has not started", async () => {
    const presentation = await createPresentation({ notBefore: nowInSeconds + 1 });

    const result = await validate(presentation);

    expect(result.isValid).toBe(false);
    expect(result.errorCode).toBe(PresentationValidationErrorCode.CredentialNotYetValid);
  });

  it("rejects an unsupported SD-JWT typ header", async () => {
    const presentation = await createPresentation({ type: "JWT" });

    const result = await validate(presentation);

    expect(result.isValid).toBe(false);
    expect(result.errorCode).toBe(PresentationValidationErrorCode.UnsupportedSdJwtType);
  });

  it("rejects a cryptographically valid SD-JWT without a vct claim", async () => {
    const presentation = await createPresentation({ includeVct: false });

    const result = await validate(presentation);

    expect(result.isValid).toBe(false);
    expect(result.errorCode).toBe(PresentationValidationErrorCode.MissingCredentialType);
  });

  it("rejects the complete response when one of multiple presentations is invalid", async () => {
    const validPresentation = await createPresentation();
    const invalidPresentation = replacePart(await createPresentation(), 0, tamperJwtSignature);

    const result = await validate([validPresentation, invalidPresentation]);

    expect(result.isValid).toBe(false);
    expect(result.errorCode).toBe(PresentationValidationErrorCode.InvalidSignature);
  });

  it("rejects empty input", async () => {
    const result = await validatePresentedCredential(undefined, {
      expectedAudience,
      expectedNonce,
      now
    });

    expect(result).toEqual({
      credential: {},
      errorCode: PresentationValidationErrorCode.MissingPresentation,
      isValid: false
    });
  });

  it("rejects validation without a nonce before doing any cryptographic work", async () => {
    const presentation = await createPresentation();

    const result = await validatePresentedCredential(presentation, {
      expectedAudience,
      expectedNonce: "",
      now
    });

    expect(result.isValid).toBe(false);
    expect(result.errorCode).toBe(PresentationValidationErrorCode.MissingChallenge);
  });

  it("rejects validation without an audience before doing any cryptographic work", async () => {
    const presentation = await createPresentation();

    const result = await validatePresentedCredential(presentation, {
      expectedAudience: "",
      expectedNonce,
      now
    });

    expect(result.isValid).toBe(false);
    expect(result.errorCode).toBe(PresentationValidationErrorCode.MissingAudience);
  });
});

type SigningKey = {
  privateKey: Uint8Array;
  publicJwk: Kms.KmsJwkPublicOkp;
  signer: (data: string) => Promise<string>;
};

type PresentationOverrides = {
  audience?: string;
  expiresAt?: number;
  includeVct?: boolean;
  nonce?: string;
  notBefore?: number;
  presentationSigningKey?: SigningKey;
  type?: string;
};

async function validate(presentation: unknown) {
  return validatePresentedCredential(presentation, {
    expectedAudience,
    expectedNonce,
    now
  });
}

async function createPresentation(overrides: PresentationOverrides = {}) {
  const credential = await createCredential(overrides);
  const presenter = new SDJwtInstance({
    hasher: sdJwtVcHasher,
    kbSignAlg: "EdDSA",
    kbSigner: (overrides.presentationSigningKey ?? holderKey).signer
  });

  return presenter.present(
    credential,
    { GivenName: true, Surname: true },
    {
      kb: {
        payload: {
          aud: overrides.audience ?? expectedAudience,
          iat: nowInSeconds,
          nonce: overrides.nonce ?? expectedNonce
        }
      }
    }
  );
}

async function createCredential(overrides: PresentationOverrides = {}) {
  const issuerDid = DidJwk.fromPublicJwk(Kms.PublicJwk.fromPublicJwk(issuerKey.publicJwk));
  const issuer = new SDJwtInstance<TestCredentialPayload>({
    hasher: sdJwtVcHasher,
    saltGenerator: generateSalt,
    signAlg: "EdDSA",
    signer: issuerKey.signer
  });
  const payload: TestCredentialPayload = {
    GivenName: "Maria",
    Surname: "Müller",
    cnf: { jwk: holderKey.publicJwk },
    exp: overrides.expiresAt ?? nowInSeconds + 3600,
    iat: nowInSeconds - 60,
    iss: issuerDid.did,
    nbf: overrides.notBefore ?? nowInSeconds - 60
  };

  if (overrides.includeVct !== false) {
    payload.vct = "https://example.com/credentials/heidelberg-pass";
  }

  return issuer.issue(
    payload,
    { _sd: ["GivenName", "Surname"] },
    {
      header: {
        kid: issuerDid.verificationMethodId,
        typ: overrides.type ?? "dc+sd-jwt"
      }
    }
  );
}

async function createStandaloneJwtCredential() {
  const issuerDid = DidJwk.fromPublicJwk(Kms.PublicJwk.fromPublicJwk(issuerKey.publicJwk));
  const header = {
    alg: "EdDSA",
    kid: issuerDid.verificationMethodId,
    typ: "JWT"
  };
  const payload = {
    exp: nowInSeconds + 3600,
    iss: issuerDid.did,
    nbf: nowInSeconds - 60,
    sub: "did:example:holder",
    vc: {
      "@context": ["https://www.w3.org/2018/credentials/v1"],
      credentialSubject: {
        id: "did:example:holder",
        name: "Maria Müller"
      },
      type: ["VerifiableCredential", "IdentityCredential"]
    }
  };
  const encodedHeader = TypedArrayEncoder.toBase64Url(TypedArrayEncoder.fromUtf8String(JSON.stringify(header)));
  const encodedPayload = TypedArrayEncoder.toBase64Url(TypedArrayEncoder.fromUtf8String(JSON.stringify(payload)));
  const signingInput = `${encodedHeader}.${encodedPayload}`;

  return `${signingInput}.${await issuerKey.signer(signingInput)}`;
}

type TestCredentialPayload = {
  GivenName: string;
  Surname: string;
  cnf: { jwk: Kms.KmsJwkPublicOkp };
  exp: number;
  iat: number;
  iss: string;
  nbf: number;
  vct?: string;
};

function createSigningKey(seed: number): SigningKey {
  const privateKey = Uint8Array.from({ length: 32 }, (_value, index) => (seed + index) % 256);
  const publicJwk: Kms.KmsJwkPublicOkp = {
    alg: "EdDSA",
    crv: "Ed25519",
    key_ops: ["verify"],
    kty: "OKP",
    use: "sig",
    x: TypedArrayEncoder.toBase64Url(ed25519.getPublicKey(privateKey))
  };

  return {
    privateKey,
    publicJwk,
    signer: async (data) => TypedArrayEncoder.toBase64Url(ed25519.sign(TypedArrayEncoder.fromUtf8String(data), privateKey))
  };
}

function generateSalt(length: number) {
  const salt = new Uint8Array(length);
  crypto.getRandomValues(salt);
  return TypedArrayEncoder.toBase64Url(salt);
}

function replacePart(value: string, index: number, transform: (part: string) => string) {
  const parts = value.split("~");
  parts[index] = transform(parts[index]);
  return parts.join("~");
}

function tamperJwtSignature(jwt: string) {
  const parts = jwt.split(".");
  const signature = parts[2];
  parts[2] = `${signature.startsWith("A") ? "B" : "A"}${signature.slice(1)}`;
  return parts.join(".");
}
