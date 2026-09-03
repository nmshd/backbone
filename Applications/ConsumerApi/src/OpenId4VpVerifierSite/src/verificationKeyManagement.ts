import { Kms, TypedArrayEncoder } from "@credo-ts/core";
import { ed25519 } from "@noble/curves/ed25519.js";
import { secp256k1 } from "@noble/curves/secp256k1.js";
import { sha256 } from "@noble/hashes/sha2.js";
import type { AgentContext } from "@credo-ts/core";

const supportedAlgorithms = [
  "RS256",
  "RS384",
  "RS512",
  "PS256",
  "PS384",
  "PS512",
  "ES256",
  "ES384",
  "ES512",
  "ES256K",
  "EdDSA",
  "Ed25519"
] as const;

type SupportedAlgorithm = (typeof supportedAlgorithms)[number];

export class BrowserVerificationKeyManagementService implements Kms.KeyManagementService {
  public readonly backend = "browser-verification";

  public isOperationSupported(_agentContext: AgentContext, operation: Kms.KmsOperation) {
    return operation.operation === "randomBytes" || (operation.operation === "verify" && supportedAlgorithms.includes(operation.algorithm as SupportedAlgorithm));
  }

  public async getPublicKey() {
    return null;
  }

  public async createKey(): Promise<never> {
    throw new Kms.KeyManagementError("The browser verification backend cannot create keys.");
  }

  public async importKey(): Promise<never> {
    throw new Kms.KeyManagementError("The browser verification backend cannot import keys.");
  }

  public async deleteKey() {
    return false;
  }

  public async sign(): Promise<never> {
    throw new Kms.KeyManagementError("The browser verification backend cannot sign data.");
  }

  public async verify(_agentContext: AgentContext, options: Kms.KmsVerifyOptions): Promise<Kms.KmsVerifyReturn> {
    const publicJwk = options.key.publicJwk;

    if (!publicJwk) {
      throw new Kms.KeyManagementError("Only public JWK based verification is supported in the browser.");
    }

    Kms.assertAllowedSigningAlgForKey(publicJwk, options.algorithm);
    Kms.assertKeyAllowsVerify(publicJwk);

    const verified = await this.verifyWithJwk(publicJwk, options.algorithm as SupportedAlgorithm, options.data, options.signature);

    if (!verified) {
      return { verified: false };
    }

    return {
      publicJwk: Kms.publicJwkFromPrivateJwk(publicJwk),
      verified: true
    };
  }

  public async encrypt(): Promise<never> {
    throw new Kms.KeyManagementError("The browser verification backend cannot encrypt data.");
  }

  public async decrypt(): Promise<never> {
    throw new Kms.KeyManagementError("The browser verification backend cannot decrypt data.");
  }

  public randomBytes(_agentContext: AgentContext, options: Kms.KmsRandomBytesOptions) {
    const bytes = new Uint8Array(options.length);
    crypto.getRandomValues(bytes);
    return bytes;
  }

  private async verifyWithJwk(jwk: Kms.KmsJwkPublic, algorithm: SupportedAlgorithm, data: Uint8Array, signature: Uint8Array) {
    if (jwk.kty === "EC" && jwk.crv === "secp256k1") {
      const publicKey = new Uint8Array([4, ...TypedArrayEncoder.fromBase64Url(jwk.x), ...TypedArrayEncoder.fromBase64Url(jwk.y)]);
      return secp256k1.verify(signature, sha256(data), publicKey, { lowS: false });
    }

    if (jwk.kty === "OKP" && jwk.crv === "Ed25519") {
      return ed25519.verify(signature, data, TypedArrayEncoder.fromBase64Url(jwk.x));
    }

    const key = await crypto.subtle.importKey("jwk", sanitizePublicJwk(jwk), webCryptoImportAlgorithm(algorithm, jwk), false, ["verify"]);
    const verifyAlgorithm = webCryptoVerifyAlgorithm(algorithm);
    const verified = await crypto.subtle.verify(verifyAlgorithm, key, toBrowserBytes(signature), toBrowserBytes(data));

    if (verified || jwk.kty !== "EC") {
      return verified;
    }

    return crypto.subtle.verify(verifyAlgorithm, key, toBrowserBytes(Kms.rawEcSignatureToDer(signature, jwk.crv)), toBrowserBytes(data));
  }
}

function toBrowserBytes(bytes: Uint8Array): Uint8Array<ArrayBuffer> {
  return new Uint8Array(bytes);
}

function sanitizePublicJwk(jwk: Kms.KmsJwkPublic): JsonWebKey {
  const { alg: _alg, key_ops: _keyOps, use: _use, ...sanitizedJwk } = jwk;
  return sanitizedJwk as JsonWebKey;
}

function webCryptoImportAlgorithm(algorithm: SupportedAlgorithm, jwk: Kms.KmsJwkPublic): EcKeyImportParams | RsaHashedImportParams {
  if (jwk.kty === "EC") {
    return {
      name: "ECDSA",
      namedCurve: jwk.crv === "P-521" ? "P-521" : jwk.crv === "P-384" ? "P-384" : "P-256"
    };
  }

  if (algorithm.startsWith("PS")) {
    return {
      hash: { name: hashForAlgorithm(algorithm) },
      name: "RSA-PSS"
    };
  }

  return {
    hash: { name: hashForAlgorithm(algorithm) },
    name: "RSASSA-PKCS1-v1_5"
  };
}

function webCryptoVerifyAlgorithm(algorithm: SupportedAlgorithm): EcdsaParams | RsaPssParams | RsaHashedImportParams {
  if (algorithm.startsWith("ES")) {
    return {
      hash: { name: hashForAlgorithm(algorithm) },
      name: "ECDSA"
    };
  }

  if (algorithm.startsWith("PS")) {
    return {
      hash: { name: hashForAlgorithm(algorithm) },
      name: "RSA-PSS",
      saltLength: Number.parseInt(algorithm.slice(2), 10) / 8
    };
  }

  return {
    hash: { name: hashForAlgorithm(algorithm) },
    name: "RSASSA-PKCS1-v1_5"
  };
}

function hashForAlgorithm(algorithm: SupportedAlgorithm): "SHA-256" | "SHA-384" | "SHA-512" {
  if (algorithm.endsWith("384")) {
    return "SHA-384";
  }

  if (algorithm.endsWith("512")) {
    return "SHA-512";
  }

  return "SHA-256";
}
