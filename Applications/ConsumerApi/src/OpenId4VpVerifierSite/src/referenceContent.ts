import { CoreBuffer, CryptoCipher, CryptoEncryption, CryptoSecretKey } from "@nmshd/crypto";

type ApiEnvelope = {
  result?: unknown;
};

export type TokenContentVerifiablePresentation = {
  "@type": "TokenContentVerifiablePresentation";
  displayInformation?: Array<Record<string, unknown>>;
  type?: string;
  value: unknown;
};

type ReferenceContentConfig = {
  referenceId?: string;
  relationshipTemplateEndpointTemplate?: string;
  tokenEndpointTemplate?: string;
};

type ParsedReferenceHash = {
  algorithm: number;
  key: string;
};

const XCHACHA20_POLY1305_ALGORITHM = 3;

export async function tryLoadVerifiablePresentationTokenContent(
  config: ReferenceContentConfig
): Promise<TokenContentVerifiablePresentation | undefined> {
  try {
    const referenceId = config.referenceId?.trim();
    const parsedReference = parseReferenceHash(window.location.hash);

    if (!referenceId || !parsedReference || parsedReference.algorithm !== XCHACHA20_POLY1305_ALGORITHM) {
      return undefined;
    }

    const encryptedContent = await tryFetchEncryptedContent(referenceId, config);
    if (!encryptedContent) {
      return undefined;
    }

    const decryptedContent = await decryptTokenContent(encryptedContent, parsedReference);
    return isTokenContentVerifiablePresentation(decryptedContent) ? decryptedContent : undefined;
  } catch {
    return undefined;
  }
}

function parseReferenceHash(hash: string): ParsedReferenceHash | undefined {
  const hashValue = hash.startsWith("#") ? hash.slice(1) : hash;
  const candidates = [hashValue, hashValue.endsWith(".") ? hashValue.slice(0, -1) : undefined].filter(Boolean) as string[];

  for (const candidate of candidates) {
    try {
      const [algorithm, key] = CoreBuffer.fromBase64URL(candidate).toUtf8().split("|");
      const parsedAlgorithm = Number.parseInt(algorithm, 10);

      if (Number.isNaN(parsedAlgorithm) || !key) {
        continue;
      }

      return {
        algorithm: parsedAlgorithm,
        key
      };
    } catch {
      continue;
    }
  }

  return undefined;
}

async function tryFetchEncryptedContent(referenceId: string, config: ReferenceContentConfig) {
  for (const endpoint of endpointsForReferenceId(referenceId, config)) {
    try {
      const response = await fetch(endpoint, {
        credentials: "same-origin",
        headers: {
          Accept: "application/json"
        }
      });

      if (!response.ok) {
        continue;
      }

      const body = (await response.json()) as ApiEnvelope;
      const content = extractEncryptedContent(body);

      if (content) {
        return content;
      }
    } catch {
      continue;
    }
  }

  return undefined;
}

function endpointsForReferenceId(referenceId: string, config: ReferenceContentConfig) {
  const tokenEndpoint = applyEndpointTemplate(config.tokenEndpointTemplate, referenceId);
  const relationshipTemplateEndpoint = applyEndpointTemplate(config.relationshipTemplateEndpointTemplate, referenceId);

  if (referenceId.startsWith("RLT")) {
    return [relationshipTemplateEndpoint, tokenEndpoint].filter(Boolean) as string[];
  }

  return [tokenEndpoint, relationshipTemplateEndpoint].filter(Boolean) as string[];
}

function applyEndpointTemplate(template: string | undefined, referenceId: string) {
  return template?.replace("{id}", encodeURIComponent(referenceId));
}

function extractEncryptedContent(body: ApiEnvelope): string | undefined {
  const candidates = [body.result, getPath(body.result, ["value"]), body].filter(isRecord);

  for (const candidate of candidates) {
    const content = candidate.content;
    if (typeof content === "string" && content.trim()) {
      return content;
    }
  }

  return undefined;
}

async function decryptTokenContent(encryptedContent: string, reference: ParsedReferenceHash) {
  const secretKey = CryptoSecretKey.from({
    algorithm: XCHACHA20_POLY1305_ALGORITHM,
    secretKey: CoreBuffer.fromBase64URL(reference.key)
  });
  const cipher = CryptoCipher.fromBase64(encryptedContent);
  const plaintext = await CryptoEncryption.decrypt(cipher, secretKey);

  return JSON.parse(plaintext.toUtf8()) as unknown;
}

function isTokenContentVerifiablePresentation(value: unknown): value is TokenContentVerifiablePresentation {
  return isRecord(value) && value["@type"] === "TokenContentVerifiablePresentation" && "value" in value;
}

function getPath(object: unknown, path: string[]) {
  let current = object;

  for (const segment of path) {
    if (!isRecord(current)) {
      return undefined;
    }

    current = current[segment];
  }

  return current;
}

function isRecord(value: unknown): value is Record<string, unknown> {
  return typeof value === "object" && value !== null && !Array.isArray(value);
}
