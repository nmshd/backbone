import type { OpenId4VpAuthorizationResponsePayload } from "@credo-ts/openid4vc";
import {
  Agent,
  ClaimFormat,
  ConsoleLogger,
  DependencyManager,
  DidsModule,
  InjectionSymbols,
  JwkDidResolver,
  KeyDidResolver,
  Kms,
  LogLevel,
  WebDidResolver,
  W3cJsonLdVerifiableCredential,
  W3cJsonLdVerifiablePresentation
} from "@credo-ts/core";
import { EventEmitter } from "events";
import { BrowserVerificationKeyManagementService } from "./verificationKeyManagement";
import { BrowserFileSystem, InMemoryStorageService } from "./verificationStorage";

type JsonObject = Record<string, unknown>;

export type VerificationDisplay = {
  createdAt?: string;
  expiresAt?: string;
  issuer?: string;
  portrait?: string;
  publicKey?: string;
  title?: string;
};

export type VerificationOutcome = {
  credential: VerificationDisplay;
  error?: string;
  isValid: boolean;
};

type VerifierAgent = Agent<{
  dids: DidsModule;
  kms: Kms.KeyManagementModule;
}>;

let agentPromise: Promise<VerifierAgent> | undefined;

export async function verifyPresentedCredential(): Promise<VerificationOutcome> {
  try {
    const response = readAuthorizationResponseFromUrl();
    const tokens = extractVpTokens(response);
    const expectedNonce = getStringParam("nonce") ?? getStringParam("expected_nonce") ?? sessionStorage.getItem("openid4vpVerifier.nonce") ?? undefined;
    const expectedAudience = getStringParam("audience") ?? getStringParam("client_id") ?? window.location.origin;

    if (tokens.length === 0) {
      return invalid("No vp_token parameter was found in the OpenID4VP response.");
    }

    const agent = await getVerifierAgent();
    const verifiedArtifacts = [];

    for (const token of tokens) {
      verifiedArtifacts.push(await verifyToken(agent, token, { expectedAudience, expectedNonce }));
    }

    const isValid = verifiedArtifacts.length > 0 && verifiedArtifacts.every((artifact) => artifact.isValid);
    const firstCredential = verifiedArtifacts.map((artifact) => artifact.display).find(Boolean) ?? {};
    const firstError = verifiedArtifacts.find((artifact) => !artifact.isValid)?.error;

    return {
      credential: firstCredential,
      error: firstError,
      isValid
    };
  } catch (error) {
    return invalid(error instanceof Error ? error.message : "The credential could not be verified.");
  }
}

function readAuthorizationResponseFromUrl(): Partial<OpenId4VpAuthorizationResponsePayload> {
  const params = new URLSearchParams(window.location.search);
  const fragment = window.location.hash.startsWith("#") ? window.location.hash.slice(1) : window.location.hash;
  const fragmentParams = new URLSearchParams(fragment);

  for (const [key, value] of fragmentParams.entries()) {
    if (!params.has(key)) {
      params.set(key, value);
    }
  }

  const response: JsonObject = {};
  for (const [key, value] of params.entries()) {
    response[key] = tryParseJson(value);
  }

  return response as Partial<OpenId4VpAuthorizationResponsePayload>;
}

function extractVpTokens(response: Partial<OpenId4VpAuthorizationResponsePayload>) {
  const candidates = [response.vp_token, (response as JsonObject).presentation, (response as JsonObject).verifiablePresentation];
  return candidates.flatMap(normalizeToken).filter((token) => token !== undefined);
}

function normalizeToken(value: unknown): Array<string | JsonObject> {
  if (!value) {
    return [];
  }

  if (Array.isArray(value)) {
    return value.flatMap(normalizeToken);
  }

  if (typeof value === "string") {
    const parsed = tryParseJson(value);
    if (parsed !== value) {
      return normalizeToken(parsed);
    }

    return [value];
  }

  if (typeof value === "object") {
    return [value as JsonObject];
  }

  return [];
}

async function verifyToken(
  agent: VerifierAgent,
  token: string | JsonObject,
  context: { expectedAudience: string; expectedNonce?: string }
): Promise<{ display?: VerificationDisplay; error?: string; isValid: boolean }> {
  if (typeof token === "string") {
    if (isSdJwt(token)) {
      return verifySdJwtCredential(agent, token, context);
    }

    if (isJwt(token)) {
      return verifyJwtArtifact(agent, token, context);
    }

    return { error: "The vp_token format is not supported.", isValid: false };
  }

  return verifyJsonLdArtifact(agent, token, context);
}

async function verifySdJwtCredential(
  agent: VerifierAgent,
  token: string,
  context: { expectedAudience: string; expectedNonce?: string }
): Promise<{ display?: VerificationDisplay; error?: string; isValid: boolean }> {
  const result = await agent.sdJwtVc.verify({
    compactSdJwtVc: token,
    fetchTypeMetadata: false,
    keyBinding: context.expectedNonce
      ? {
          audience: context.expectedAudience,
          nonce: context.expectedNonce
        }
      : undefined
  });

  return {
    display: extractDisplayFromObject(result.sdJwtVc?.prettyClaims ?? result.sdJwtVc?.payload),
    error: result.isValid ? undefined : result.error.message,
    isValid: result.isValid
  };
}

async function verifyJwtArtifact(
  agent: VerifierAgent,
  token: string,
  context: { expectedAudience: string; expectedNonce?: string }
): Promise<{ display?: VerificationDisplay; error?: string; isValid: boolean }> {
  const payload = decodeJwtPayload(token);
  const embeddedCredentials = extractEmbeddedCredentials(payload);

  if (looksLikePresentation(payload)) {
    const challenge = context.expectedNonce;
    const presentationResult = challenge
      ? await tryVerify(() =>
          agent.w3cCredentials.verifyPresentation({
            challenge,
            domain: context.expectedAudience,
            presentation: token,
            verifyCredentialStatus: false
          })
        )
      : undefined;

    if (challenge && presentationResult?.isValid === false) {
      return {
        display: extractDisplayFromObject(payload),
        error: presentationResult.error,
        isValid: false
      };
    }

    if (embeddedCredentials.length === 0) {
      return {
        display: extractDisplayFromObject(payload),
        error: "The presentation did not contain a verifiable credential.",
        isValid: false
      };
    }

    const credentialResults = await Promise.all(embeddedCredentials.map((credential) => verifyToken(agent, credential, context)));

    return {
      display: credentialResults.map((result) => result.display).find(Boolean) ?? extractDisplayFromObject(payload),
      error: credentialResults.find((result) => !result.isValid)?.error,
      isValid: credentialResults.every((result) => result.isValid)
    };
  }

  const credentialResult = await tryVerify(() =>
    agent.w3cCredentials.verifyCredential({
      credential: token,
      verifyCredentialStatus: false
    })
  );

  if (credentialResult.isValid) {
    return {
      display: extractDisplayFromObject(payload),
      isValid: true
    };
  }

  const v2CredentialResult = await tryVerify(() =>
    agent.w3cV2Credentials.verifyCredential({
      credential: token
    })
  );

  return {
    display: extractDisplayFromObject(payload),
    error: v2CredentialResult.isValid ? undefined : credentialResult.error ?? v2CredentialResult.error,
    isValid: v2CredentialResult.isValid
  };
}

async function verifyJsonLdArtifact(
  agent: VerifierAgent,
  token: JsonObject,
  context: { expectedAudience: string; expectedNonce?: string }
): Promise<{ display?: VerificationDisplay; error?: string; isValid: boolean }> {
  if (looksLikePresentation(token)) {
    const embeddedCredentials = extractEmbeddedCredentials(token);

    if (embeddedCredentials.length > 0) {
      const credentialResults = await Promise.all(embeddedCredentials.map((credential) => verifyToken(agent, credential, context)));
      return {
        display: credentialResults.map((result) => result.display).find(Boolean) ?? extractDisplayFromObject(token),
        error: credentialResults.find((result) => !result.isValid)?.error,
        isValid: credentialResults.every((result) => result.isValid)
      };
    }

    if (!context.expectedNonce) {
      return {
        display: extractDisplayFromObject(token),
        error: "A challenge is required to verify a JSON-LD presentation.",
        isValid: false
      };
    }

    const challenge = context.expectedNonce;
    const result = await tryVerify(() =>
      agent.w3cCredentials.verifyPresentation({
        challenge,
        domain: context.expectedAudience,
        presentation: new W3cJsonLdVerifiablePresentation(token as never),
        verifyCredentialStatus: false
      })
    );

    return {
      display: extractDisplayFromObject(token),
      error: result.isValid ? undefined : result.error,
      isValid: result.isValid
    };
  }

  const result = await tryVerify(() =>
    agent.w3cCredentials.verifyCredential({
      credential: W3cJsonLdVerifiableCredential.fromJson(token),
      verifyCredentialStatus: false
    })
  );

  return {
    display: extractDisplayFromObject(token),
    error: result.isValid ? undefined : result.error,
    isValid: result.isValid
  };
}

async function tryVerify(verify: () => Promise<{ isValid?: boolean; verified?: boolean }>) {
  try {
    const result = await verify();
    return { isValid: result.isValid === true || result.verified === true };
  } catch (error) {
    return {
      error: error instanceof Error ? error.message : "The signature could not be verified.",
      isValid: false
    };
  }
}

async function getVerifierAgent() {
  agentPromise ??= createVerifierAgent();
  return agentPromise;
}

async function createVerifierAgent() {
  const dependencyManager = new DependencyManager();
  dependencyManager.registerInstance(InjectionSymbols.StorageService, new InMemoryStorageService());

  const agent = new Agent({
    config: {
      allowInsecureHttpUrls: window.location.protocol === "http:",
      autoUpdateStorageOnStartup: true,
      logger: new ConsoleLogger(LogLevel.Error)
    },
    dependencies: {
      EventEmitterClass: EventEmitter,
      FileSystem: BrowserFileSystem,
      WebSocketClass: window.WebSocket as never,
      fetch: window.fetch.bind(window)
    },
    modules: {
      dids: new DidsModule({
        resolvers: [new KeyDidResolver(), new JwkDidResolver(), new WebDidResolver()]
      }),
      kms: new Kms.KeyManagementModule({
        backends: [new BrowserVerificationKeyManagementService()]
      })
    }
  }, dependencyManager);

  await agent.initialize();
  return agent;
}

function extractEmbeddedCredentials(input: unknown): Array<string | JsonObject> {
  const presentation = unwrapPresentation(input);
  const rawCredentials = getPath(presentation, ["verifiableCredential"]) ?? getPath(presentation, ["vp", "verifiableCredential"]);

  return normalizeToken(rawCredentials);
}

function unwrapPresentation(input: unknown) {
  if (!isObject(input)) {
    return input;
  }

  return input.vp ?? input.presentation ?? input.verifiablePresentation ?? input;
}

function looksLikePresentation(input: unknown) {
  const presentation = unwrapPresentation(input);

  if (!isObject(presentation)) {
    return false;
  }

  const type = presentation.type;
  return Boolean(
    "verifiableCredential" in presentation ||
      (Array.isArray(type) && type.includes("VerifiablePresentation")) ||
      type === "VerifiablePresentation" ||
      "vp" in presentation
  );
}

function extractDisplayFromObject(input: unknown): VerificationDisplay {
  const credential = unwrapCredential(input);
  const credentialSubject = getObjectPath(credential, ["credentialSubject"]) ?? getObjectPath(credential, ["vc", "credentialSubject"]);
  const claims = [credentialSubject, getObjectPath(credential, ["vc"]), credential].filter(isObject);

  return {
    createdAt: formatDate(firstString(claims, ["issuanceDate", "validFrom", "nbf", "iat"])),
    expiresAt: formatDate(firstString(claims, ["expirationDate", "validUntil", "exp"])),
    issuer: issuerName(credential),
    portrait: firstString(claims, ["portrait", "photo", "picture", "image"]),
    publicKey: formatPublicKey(firstString([credential], ["kid", "iss", "id"])),
    title: credentialTitle(credential)
  };
}

function unwrapCredential(input: unknown): JsonObject {
  if (typeof input === "string" && isJwt(input)) {
    return decodeJwtPayload(input);
  }

  if (!isObject(input)) {
    return {};
  }

  if (isObject(input.vc)) {
    return input.vc;
  }

  if (isObject(input.credential)) {
    return input.credential;
  }

  return input;
}

function credentialTitle(credential: JsonObject) {
  const name = getStringPath(credential, ["name"]) ?? getStringPath(credential, ["vc", "name"]);
  if (name) {
    return name;
  }

  const type = credential.type ?? getPath(credential, ["vc", "type"]);
  const lastType = Array.isArray(type) ? type.filter((entry) => entry !== "VerifiableCredential").at(-1) : type;
  if (typeof lastType === "string") {
    return splitCamelCase(lastType).replaceAll("_", " ");
  }

  const vct = getStringPath(credential, ["vct"]);
  if (vct) {
    return splitCamelCase(vct.split(/[/:#]/).at(-1) ?? vct);
  }

  return undefined;
}

function issuerName(credential: JsonObject) {
  const issuer = credential.issuer ?? getPath(credential, ["vc", "issuer"]) ?? credential.iss;

  if (typeof issuer === "string") {
    return readableIssuer(issuer);
  }

  if (isObject(issuer)) {
    return getStringPath(issuer, ["name"]) ?? getStringPath(issuer, ["id"]) ?? undefined;
  }

  return undefined;
}

function firstString(objects: JsonObject[], keys: string[]) {
  for (const object of objects) {
    for (const key of keys) {
      const value = object[key];
      if (typeof value === "string" && value.trim()) {
        return value;
      }

      if (typeof value === "number") {
        return value.toString();
      }
    }
  }

  return undefined;
}

function formatDate(value?: string) {
  if (!value) {
    return undefined;
  }

  if (/^\d+$/.test(value)) {
    const date = new Date(Number(value) * 1000);
    return Number.isNaN(date.getTime()) ? undefined : new Intl.DateTimeFormat("de-DE").format(date);
  }

  const date = new Date(value);
  return Number.isNaN(date.getTime()) ? value : new Intl.DateTimeFormat("de-DE").format(date);
}

function formatPublicKey(value?: string) {
  if (!value) {
    return undefined;
  }

  const strippedValue = value.split("#").at(-1)?.replace(/[^a-zA-Z0-9]/g, "") ?? value;
  const numericValue = strippedValue.replace(/\D/g, "");

  if (numericValue.length >= 20) {
    return numericValue.slice(0, 20).match(/.{1,4}/g)?.join("-") ?? numericValue;
  }

  return strippedValue.length > 24 ? `${strippedValue.slice(0, 4)}-${strippedValue.slice(-16).match(/.{1,4}/g)?.join("-")}` : value;
}

function readableIssuer(value: string) {
  if (value.startsWith("did:")) {
    return value;
  }

  try {
    const url = new URL(value);
    return url.hostname
      .replace(/^www\./, "")
      .split(".")
      .slice(0, -1)
      .map(capitalize)
      .join(" ");
  } catch {
    return value;
  }
}

function splitCamelCase(value: string) {
  return value.replace(/([a-z])([A-Z])/g, "$1 $2").replace(/[-_]+/g, " ");
}

function capitalize(value: string) {
  return value.charAt(0).toUpperCase() + value.slice(1);
}

function decodeJwtPayload(jwt: string): JsonObject {
  const payload = jwt.split(".")[1];
  if (!payload) {
    return {};
  }

  return JSON.parse(new TextDecoder().decode(base64UrlToBytes(payload))) as JsonObject;
}

function getStringParam(key: string) {
  const searchParams = new URLSearchParams(window.location.search);
  const fragmentParams = new URLSearchParams(window.location.hash.startsWith("#") ? window.location.hash.slice(1) : window.location.hash);
  return searchParams.get(key) ?? fragmentParams.get(key);
}

function getObjectPath(object: JsonObject, path: string[]) {
  const value = getPath(object, path);
  return isObject(value) ? value : undefined;
}

function getStringPath(object: JsonObject, path: string[]) {
  const value = getPath(object, path);
  return typeof value === "string" && value.trim() ? value : undefined;
}

function getPath(object: unknown, path: string[]) {
  let current = object;

  for (const segment of path) {
    if (!isObject(current)) {
      return undefined;
    }

    current = current[segment];
  }

  return current;
}

function tryParseJson(value: string): unknown {
  try {
    return JSON.parse(value);
  } catch {
    return value;
  }
}

function base64UrlToBytes(value: string) {
  const base64 = value.replaceAll("-", "+").replaceAll("_", "/").padEnd(Math.ceil(value.length / 4) * 4, "=");
  return Uint8Array.from(atob(base64), (character) => character.charCodeAt(0));
}

function isJwt(value: string) {
  return value.split(".").length === 3;
}

function isSdJwt(value: string) {
  return value.includes("~");
}

function isObject(value: unknown): value is JsonObject {
  return typeof value === "object" && value !== null && !Array.isArray(value);
}

function invalid(error: string): VerificationOutcome {
  return {
    credential: {},
    error,
    isValid: false
  };
}

void ClaimFormat.JwtVc;
