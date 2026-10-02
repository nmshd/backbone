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
  TypedArrayEncoder,
  WebDidResolver,
  W3cJsonLdVerifiableCredential,
  W3cJsonLdVerifiablePresentation,
  X509Certificate,
  getPublicJwkFromVerificationMethod,
  sdJwtVcHasher
} from "@credo-ts/core";
import { SDJwtInstance } from "@sd-jwt/core";
import { EventEmitter } from "events";
import { BrowserVerificationKeyManagementService } from "./verificationKeyManagement";
import { BrowserFileSystem, InMemoryStorageService } from "./verificationStorage";

type JsonObject = Record<string, unknown>;

export enum PresentationValidationErrorCode {
  AudienceMismatch = "audienceMismatch",
  CredentialExpired = "credentialExpired",
  CredentialNotYetValid = "credentialNotYetValid",
  InvalidDisclosure = "invalidDisclosure",
  InvalidPresentationBinding = "invalidPresentationBinding",
  InvalidSignature = "invalidSignature",
  MissingAudience = "missingAudience",
  MissingChallenge = "missingChallenge",
  MissingCredential = "missingCredential",
  MissingCredentialType = "missingCredentialType",
  MissingHolderVerificationKey = "missingHolderVerificationKey",
  MissingIssuerVerificationKey = "missingIssuerVerificationKey",
  MissingKeyBinding = "missingKeyBinding",
  MissingPresentation = "missingPresentation",
  NonceMismatch = "nonceMismatch",
  UnsupportedHolderDidUrl = "unsupportedHolderDidUrl",
  UnsupportedIssuerDidUrl = "unsupportedIssuerDidUrl",
  UnsupportedPresentationFormat = "unsupportedPresentationFormat",
  UnsupportedSdJwtType = "unsupportedSdJwtType",
  UnsupportedSignatureAlgorithm = "unsupportedSignatureAlgorithm",
  VerificationFailed = "verificationFailed"
}

class PresentationValidationError extends Error {
  public constructor(public readonly code: PresentationValidationErrorCode) {
    super(code);
  }
}

export type VerificationDisplay = {
  claims?: VerificationDisplayClaim[];
  createdAt?: string;
  expiresAt?: string;
  issuer?: string;
  portrait?: string;
  publicKey?: string;
  title?: string;
};

export type VerificationDisplayClaim = {
  label: string;
  value: string;
};

export type VerificationOutcome = {
  credential: VerificationDisplay;
  errorCode?: PresentationValidationErrorCode;
  isValid: boolean;
};

type ArtifactVerificationOutcome = {
  display?: VerificationDisplay;
  errorCode?: PresentationValidationErrorCode;
  isValid: boolean;
};

export type PresentationValidationOptions = {
  expectedAudience: string;
  expectedNonce: string;
  now?: Date;
};

type VerificationContext = {
  expectedAudience: string;
  expectedNonce: string;
  now: Date;
};

type VerifierAgent = Agent<{
  dids: DidsModule;
  kms: Kms.KeyManagementModule;
}>;

let agentPromise: Promise<VerifierAgent> | undefined;

export async function validatePresentedCredential(
  presentation: unknown,
  options: PresentationValidationOptions
): Promise<VerificationOutcome> {
  try {
    if (!options.expectedNonce) {
      return invalid(PresentationValidationErrorCode.MissingChallenge);
    }

    if (!options.expectedAudience) {
      return invalid(PresentationValidationErrorCode.MissingAudience);
    }

    const tokens = normalizeToken(presentation);

    if (tokens.length === 0) {
      return invalid(PresentationValidationErrorCode.MissingPresentation);
    }

    const agent = await getVerifierAgent();
    const verifiedArtifacts = [];
    const now = options.now ?? new Date();

    for (const token of tokens) {
      verifiedArtifacts.push(
        await verifyToken(agent, token, {
          expectedAudience: options.expectedAudience,
          expectedNonce: options.expectedNonce,
          now
        })
      );
    }

    const isValid = verifiedArtifacts.length > 0 && verifiedArtifacts.every((artifact) => artifact.isValid);
    const firstCredential = verifiedArtifacts.map((artifact) => artifact.display).find(Boolean) ?? {};
    const firstErrorCode = verifiedArtifacts.find((artifact) => !artifact.isValid)?.errorCode;

    return {
      credential: firstCredential,
      errorCode: firstErrorCode,
      isValid
    };
  } catch (error) {
    return invalid(error instanceof PresentationValidationError ? error.code : PresentationValidationErrorCode.VerificationFailed);
  }
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
  context: VerificationContext,
  isEmbeddedInVerifiedPresentation = false
): Promise<ArtifactVerificationOutcome> {
  if (typeof token === "string") {
    if (isSdJwt(token)) {
      return verifySdJwtCredential(agent, token, context);
    }

    if (isJwt(token)) {
      return verifyJwtArtifact(agent, token, context, isEmbeddedInVerifiedPresentation);
    }

    return { errorCode: PresentationValidationErrorCode.UnsupportedPresentationFormat, isValid: false };
  }

  return verifyJsonLdArtifact(agent, token, context, isEmbeddedInVerifiedPresentation);
}

async function verifySdJwtCredential(
  agent: VerifierAgent,
  token: string,
  context: VerificationContext
): Promise<ArtifactVerificationOutcome> {
  const sdJwtVc = agent.sdJwtVc.fromCompact(token);
  const display = extractDisplayFromObject(sdJwtVc.prettyClaims);

  if (!context.expectedNonce) {
    return {
      display,
      errorCode: PresentationValidationErrorCode.MissingChallenge,
      isValid: false
    };
  }

  try {
    if (sdJwtVc.header.typ !== "dc+sd-jwt" && sdJwtVc.header.typ !== "vc+sd-jwt") {
      throw new PresentationValidationError(PresentationValidationErrorCode.UnsupportedSdJwtType);
    }

    if (!sdJwtVc.kbJwt) {
      throw new PresentationValidationError(PresentationValidationErrorCode.MissingKeyBinding);
    }

    const issuerJwk = await resolveSdJwtIssuerJwk(agent, sdJwtVc.header, sdJwtVc.payload);
    const holderJwk = await resolveSdJwtHolderJwk(agent, sdJwtVc.payload);
    const issuerAlgorithm = signingAlgorithm(sdJwtVc.header);
    const holderAlgorithm = signingAlgorithm(sdJwtVc.kbJwt.header);
    const verifier = new SDJwtInstance({
      hasher: sdJwtVcHasher,
      kbVerifier: createJwtVerifier(agent, holderJwk, holderAlgorithm),
      verifier: createJwtVerifier(agent, issuerJwk, issuerAlgorithm)
    });

    await verifier.verify(token, {
      currentDate: Math.floor(context.now.getTime() / 1000),
      keyBindingNonce: context.expectedNonce,
      requiredClaimKeys: ["vct"]
    });

    const validationError = validateSdJwtPresentation(sdJwtVc.payload, sdJwtVc.kbJwt?.payload, context);

    return {
      display,
      errorCode: validationError,
      isValid: validationError === undefined
    };
  } catch (error) {
    return {
      display,
      errorCode: classifySdJwtError(error),
      isValid: false
    };
  }
}

function createJwtVerifier(agent: VerifierAgent, publicJwk: Kms.KmsJwkPublicAsymmetric, algorithm: Kms.KnownJwaSignatureAlgorithm) {
  return async (data: string, signature: string) => {
    const result = await agent.kms.verify({
      algorithm,
      data: TypedArrayEncoder.fromUtf8String(data),
      key: { publicJwk },
      signature: TypedArrayEncoder.fromBase64Url(signature)
    });

    return result.verified;
  };
}

function signingAlgorithm(header?: JsonObject): Kms.KnownJwaSignatureAlgorithm {
  const algorithm = header?.alg;

  if (
    typeof algorithm !== "string" ||
    !Object.values(Kms.KnownJwaSignatureAlgorithms).includes(algorithm as Kms.KnownJwaSignatureAlgorithm) ||
    algorithm.startsWith("HS")
  ) {
    throw new PresentationValidationError(PresentationValidationErrorCode.UnsupportedSignatureAlgorithm);
  }

  return algorithm as Kms.KnownJwaSignatureAlgorithm;
}

async function resolveSdJwtIssuerJwk(agent: VerifierAgent, header: JsonObject, payload: JsonObject): Promise<Kms.KmsJwkPublicAsymmetric> {
  const x5c = header.x5c;
  if (Array.isArray(x5c) && typeof x5c[0] === "string") {
    return X509Certificate.fromEncodedCertificate(x5c[0]).publicJwk.toJson();
  }

  return resolveDidJwk(agent, header.kid, payload.iss, "issuer");
}

async function resolveSdJwtHolderJwk(agent: VerifierAgent, payload: JsonObject): Promise<Kms.KmsJwkPublicAsymmetric> {
  const confirmation = isObject(payload.cnf) ? payload.cnf : undefined;

  if (confirmation?.jwk) {
    return Kms.PublicJwk.fromUnknown(confirmation.jwk).toJson();
  }

  return resolveDidJwk(agent, confirmation?.kid, undefined, "holder");
}

async function resolveDidJwk(agent: VerifierAgent, keyId: unknown, controller: unknown, role: "holder" | "issuer") {
  if (typeof keyId !== "string") {
    throw new PresentationValidationError(
      role === "issuer" ? PresentationValidationErrorCode.MissingIssuerVerificationKey : PresentationValidationErrorCode.MissingHolderVerificationKey
    );
  }

  const didUrl = keyId.startsWith("#") && typeof controller === "string" ? `${controller}${keyId}` : keyId;
  if (!didUrl.startsWith("did:")) {
    throw new PresentationValidationError(
      role === "issuer" ? PresentationValidationErrorCode.UnsupportedIssuerDidUrl : PresentationValidationErrorCode.UnsupportedHolderDidUrl
    );
  }

  const didDocument = await agent.dids.resolveDidDocument(didUrl);
  const verificationMethod = didDocument.dereferenceKey(didUrl, role === "issuer" ? ["assertionMethod", "verificationMethod"] : ["authentication", "verificationMethod"]);
  return getPublicJwkFromVerificationMethod(verificationMethod).toJson();
}

function validateSdJwtPresentation(
  credentialPayload: JsonObject,
  keyBindingPayload: JsonObject | undefined,
  context: VerificationContext
) {
  if (context.expectedNonce) {
    if (!keyBindingPayload) {
      return PresentationValidationErrorCode.MissingKeyBinding;
    }

    if (keyBindingPayload.nonce !== context.expectedNonce) {
      return PresentationValidationErrorCode.NonceMismatch;
    }

    const audience = keyBindingPayload.aud;
    const hasExpectedAudience =
      audience === context.expectedAudience || (Array.isArray(audience) && audience.includes(context.expectedAudience));

    if (!hasExpectedAudience) {
      return PresentationValidationErrorCode.AudienceMismatch;
    }
  }

  const now = context.now.getTime() / 1000;
  if (typeof credentialPayload.nbf === "number" && credentialPayload.nbf > now) {
    return PresentationValidationErrorCode.CredentialNotYetValid;
  }

  if (typeof credentialPayload.exp === "number" && credentialPayload.exp <= now) {
    return PresentationValidationErrorCode.CredentialExpired;
  }

  return undefined;
}

async function verifyJwtArtifact(
  agent: VerifierAgent,
  token: string,
  context: VerificationContext,
  isEmbeddedInVerifiedPresentation: boolean
): Promise<ArtifactVerificationOutcome> {
  const payload = decodeJwtPayload(token);
  const embeddedCredentials = extractEmbeddedCredentials(payload);

  if (looksLikePresentation(payload)) {
    const challenge = context.expectedNonce;
    if (!challenge) {
      return {
        display: extractDisplayFromObject(payload),
        errorCode: PresentationValidationErrorCode.MissingChallenge,
        isValid: false
      };
    }

    const presentationResult = await tryVerify(() =>
      agent.w3cCredentials.verifyPresentation({
        challenge,
        domain: context.expectedAudience,
        presentation: token,
        verifyCredentialStatus: false
      })
    );

    if (!presentationResult.isValid) {
      return {
        display: extractDisplayFromObject(payload),
        errorCode: presentationResult.errorCode,
        isValid: false
      };
    }

    if (embeddedCredentials.length === 0) {
      return {
        display: extractDisplayFromObject(payload),
        errorCode: PresentationValidationErrorCode.MissingCredential,
        isValid: false
      };
    }

    const credentialResults = await Promise.all(embeddedCredentials.map((credential) => verifyToken(agent, credential, context, true)));

    return {
      display: credentialResults.map((result) => result.display).find(Boolean) ?? extractDisplayFromObject(payload),
      errorCode: credentialResults.find((result) => !result.isValid)?.errorCode,
      isValid: credentialResults.every((result) => result.isValid)
    };
  }

  if (!isEmbeddedInVerifiedPresentation) {
    return {
      display: extractDisplayFromObject(payload),
      errorCode: PresentationValidationErrorCode.MissingKeyBinding,
      isValid: false
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
    errorCode: v2CredentialResult.isValid ? undefined : credentialResult.errorCode ?? v2CredentialResult.errorCode,
    isValid: v2CredentialResult.isValid
  };
}

async function verifyJsonLdArtifact(
  agent: VerifierAgent,
  token: JsonObject,
  context: VerificationContext,
  isEmbeddedInVerifiedPresentation: boolean
): Promise<ArtifactVerificationOutcome> {
  if (looksLikePresentation(token)) {
    if (!context.expectedNonce) {
      return {
        display: extractDisplayFromObject(token),
        errorCode: PresentationValidationErrorCode.MissingChallenge,
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

    if (!result.isValid) {
      return {
        display: extractDisplayFromObject(token),
        errorCode: result.errorCode,
        isValid: false
      };
    }

    const embeddedCredentials = extractEmbeddedCredentials(token);
    if (embeddedCredentials.length === 0) {
      return {
        display: extractDisplayFromObject(token),
        errorCode: PresentationValidationErrorCode.MissingCredential,
        isValid: false
      };
    }

    const credentialResults = await Promise.all(embeddedCredentials.map((credential) => verifyToken(agent, credential, context, true)));

    return {
      display: credentialResults.map((credentialResult) => credentialResult.display).find(Boolean) ?? extractDisplayFromObject(token),
      errorCode: credentialResults.find((credentialResult) => !credentialResult.isValid)?.errorCode,
      isValid: credentialResults.every((credentialResult) => credentialResult.isValid)
    };
  }

  if (!isEmbeddedInVerifiedPresentation) {
    return {
      display: extractDisplayFromObject(token),
      errorCode: PresentationValidationErrorCode.MissingKeyBinding,
      isValid: false
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
    errorCode: result.isValid ? undefined : result.errorCode,
    isValid: result.isValid
  };
}

function classifySdJwtError(error: unknown) {
  if (error instanceof PresentationValidationError) {
    return error.code;
  }

  const message = error instanceof Error ? error.message : "";

  if (/invalid jwt signature/i.test(message)) {
    return PresentationValidationErrorCode.InvalidSignature;
  }

  if (/invalid sd_hash/i.test(message)) {
    return PresentationValidationErrorCode.InvalidPresentationBinding;
  }

  if (/missing required claim keys.*vct/i.test(message)) {
    return PresentationValidationErrorCode.MissingCredentialType;
  }

  if (/jwt is expired/i.test(message)) {
    return PresentationValidationErrorCode.CredentialExpired;
  }

  if (/jwt is not yet valid/i.test(message)) {
    return PresentationValidationErrorCode.CredentialNotYetValid;
  }

  if (/invalid nonce/i.test(message)) {
    return PresentationValidationErrorCode.NonceMismatch;
  }

  if (/key binding jwt not exist/i.test(message)) {
    return PresentationValidationErrorCode.MissingKeyBinding;
  }

  if (/disclosure/i.test(message)) {
    return PresentationValidationErrorCode.InvalidDisclosure;
  }

  return PresentationValidationErrorCode.VerificationFailed;
}

async function tryVerify(verify: () => Promise<{ isValid?: boolean; verified?: boolean }>) {
  try {
    const result = await verify();
    const isValid = result.isValid === true || result.verified === true;
    return {
      errorCode: isValid ? undefined : PresentationValidationErrorCode.InvalidSignature,
      isValid
    };
  } catch {
    return {
      errorCode: PresentationValidationErrorCode.InvalidSignature,
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
      allowInsecureHttpUrls: globalThis.location?.protocol === "http:",
      autoUpdateStorageOnStartup: true,
      logger: new ConsoleLogger(LogLevel.Error)
    },
    dependencies: {
      EventEmitterClass: EventEmitter,
      FileSystem: BrowserFileSystem,
      WebSocketClass: globalThis.WebSocket as never,
      fetch: globalThis.fetch.bind(globalThis)
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
    claims: extractDisplayClaims(credential),
    createdAt: formatDate(firstString(claims, ["issuanceDate", "validFrom", "nbf", "iat"])),
    expiresAt: formatDate(firstString(claims, ["expirationDate", "validUntil", "exp"])),
    issuer: issuerName(credential),
    portrait: firstString(claims, ["portrait", "photo", "picture", "image"]),
    publicKey: formatPublicKey(firstString([credential], ["kid", "iss", "id"])),
    title: credentialTitle(credential)
  };
}

const technicalClaimNames = new Set([
  "@context",
  "_sd",
  "_sd_alg",
  "cnf",
  "credentialSubject",
  "exp",
  "iat",
  "iss",
  "issuanceDate",
  "nbf",
  "proof",
  "status",
  "type",
  "validFrom",
  "validUntil",
  "vc",
  "vct",
  "vp"
]);
const imageClaimNames = new Set(["image", "photo", "picture", "portrait"]);
const germanClaimLabels: Record<string, string> = {
  birthdate: "Geburtsdatum",
  city: "Stadt",
  country: "Land",
  dateofbirth: "Geburtsdatum",
  day: "Tag",
  email: "E-Mail-Adresse",
  familyname: "Nachname",
  firstname: "Vorname",
  givenname: "Vorname",
  houseno: "Hausnummer",
  lastname: "Nachname",
  month: "Monat",
  phonenumber: "Telefonnummer",
  recipient: "Empfänger",
  street: "Straße",
  streetaddress: "Anschrift",
  surname: "Nachname",
  year: "Jahr",
  zipcode: "Postleitzahl"
};

function extractDisplayClaims(credential: JsonObject): VerificationDisplayClaim[] {
  const credentialSubject = getObjectPath(credential, ["credentialSubject"]) ?? getObjectPath(credential, ["vc", "credentialSubject"]);
  const claimSource = credentialSubject ?? credential;
  const claims: VerificationDisplayClaim[] = [];

  for (const [name, value] of Object.entries(claimSource)) {
    if (technicalClaimNames.has(name) || imageClaimNames.has(name)) {
      continue;
    }

    appendDisplayClaims(claims, [name], value);
  }

  return claims;
}

function appendDisplayClaims(claims: VerificationDisplayClaim[], path: string[], value: unknown) {
  if (isObject(value)) {
    for (const [name, nestedValue] of Object.entries(value)) {
      if (name === "@type") {
        continue;
      }

      appendDisplayClaims(claims, [...path, name], nestedValue);
    }
    return;
  }

  if (Array.isArray(value) && value.some((entry) => isObject(entry) || Array.isArray(entry))) {
    value.forEach((entry, index) => appendDisplayClaims(claims, [...path, String(index + 1)], entry));
    return;
  }

  const formattedValue = formatClaimValue(value);
  if (formattedValue === undefined) {
    return;
  }

  claims.push({
    label: path.map(localizeClaimName).join(" · "),
    value: formattedValue
  });
}

function localizeClaimName(value: string) {
  const normalizedValue = value.toLowerCase().replace(/[^a-z0-9]/g, "");
  return germanClaimLabels[normalizedValue] ?? capitalize(splitCamelCase(value));
}

function formatClaimValue(value: unknown) {
  if (typeof value === "string") {
    return value.trim() || undefined;
  }

  if (typeof value === "boolean") {
    return value ? "Ja" : "Nein";
  }

  if (typeof value === "number") {
    return String(value);
  }

  if (Array.isArray(value)) {
    return value.map((entry) => String(entry)).join(", ");
  }

  return undefined;
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
    const hostnameParts = url.hostname.replace(/^www\./, "").split(".");
    const issuerParts = hostnameParts.length > 1 ? hostnameParts.slice(0, -1) : hostnameParts;
    return issuerParts.map(capitalize).join(" ");
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

function invalid(errorCode: PresentationValidationErrorCode): VerificationOutcome {
  return {
    credential: {},
    errorCode,
    isValid: false
  };
}

void ClaimFormat.JwtVc;
