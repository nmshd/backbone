import "reflect-metadata";
import "@fontsource/work-sans/latin-400.css";
import "@fontsource/work-sans/latin-500.css";
import "@fontsource/work-sans/latin-600.css";
import "./styles.css";
import { tryLoadVerifiablePresentationTokenContent } from "./referenceContent";
import { type VerificationDisplay, validatePresentedCredential } from "./verification";

type CredentialDisplay = VerificationDisplay & {
  backgroundColor?: string;
  logo?: string;
  textColor?: string;
};

type VerificationStatus = "loading" | "valid" | "invalid";

type VerifierElements = {
  closeButton: HTMLButtonElement;
  closedView: HTMLElement;
  content: HTMLElement;
  credentialCard: HTMLElement;
  credentialLogo: HTMLImageElement;
  additionalClaims: HTMLElement;
  details: HTMLElement;
  detailRows: Record<DetailField, HTMLElement | undefined>;
  detailValues: Record<DetailField, HTMLElement | undefined>;
  error: HTMLElement;
  invalidMark: HTMLElement;
  loadingMark: HTMLElement;
  portrait: HTMLImageElement;
  resultText: HTMLElement;
  root: HTMLElement;
  title: HTMLElement;
  validMark: HTMLElement;
  verifiedBy: HTMLElement;
  verifiedByIssuer: HTMLElement;
};

const credentialDisplayFields = ["backgroundColor", "createdAt", "expiresAt", "issuer", "logo", "portrait", "publicKey", "textColor", "title"] as const;
const detailFields = ["createdAt", "expiresAt", "issuer", "publicKey"] as const;
type DetailField = (typeof detailFields)[number];
const rootElement = document.querySelector<HTMLElement>("#openid4vp-verifier-root");
const verifierElements = rootElement ? getVerifierElements(rootElement) : undefined;

if (rootElement && verifierElements) {
  document.body.classList.add("openid4vp-verifier-visible");
  setStatus(verifierElements, "loading");

  void initialize(rootElement, verifierElements);
}

async function initialize(appElement: HTMLElement, elements: VerifierElements) {
  const tokenContent = await tryLoadVerifiablePresentationTokenContent({
    referenceId: appElement.dataset.referenceId,
    relationshipTemplateEndpointTemplate: appElement.dataset.relationshipTemplateEndpointTemplate,
    tokenEndpointTemplate: appElement.dataset.tokenEndpointTemplate
  });

  if (!tokenContent) {
    showOnboarding();
    return;
  }

  const result = await validatePresentedCredential(tokenContent.value, {
    expectedAudience: "defaultPresentationAudience",
    expectedNonce: appElement.dataset.referenceId ?? ""
  });
  const credential = mergeCredentialDisplay(result.credential, credentialDisplayFromTokenContent(tokenContent.displayInformation));

  setCredential(elements, credential);
  setStatus(elements, result.isValid ? "valid" : "invalid", result.error);

  elements.closeButton.addEventListener("click", () => {
    window.close();

    window.setTimeout(() => {
      showClosedView(elements);
    }, 120);
  });
}

function showOnboarding() {
  document.body.classList.remove("openid4vp-verifier-visible");
  rootElement?.replaceChildren();
}

function showClosedView(elements: VerifierElements) {
  elements.content.hidden = true;
  elements.closedView.hidden = false;
}

function setStatus(elements: VerifierElements, status: VerificationStatus, error?: string) {
  elements.root.classList.toggle("is-loading", status === "loading");
  elements.root.classList.toggle("is-valid", status === "valid");
  elements.root.classList.toggle("is-invalid", status === "invalid");

  elements.resultText.textContent = status === "loading" ? "Der Nachweis\nwird geprüft." : `Der präsentierte Nachweis\n${status === "valid" ? "ist gültig." : "ist ungültig."}`;
  elements.loadingMark.hidden = status !== "loading";
  elements.validMark.hidden = status !== "valid";
  elements.invalidMark.hidden = status !== "invalid";
  elements.closeButton.hidden = status === "loading";

  if (error) {
    elements.error.textContent = `Prüfhinweis: ${error}`;
    elements.error.hidden = false;
  } else {
    elements.error.textContent = "";
    elements.error.hidden = true;
  }
}

function setCredential(elements: VerifierElements, credential: CredentialDisplay) {
  setText(elements.title, credential.title);
  elements.credentialCard.hidden = !credential.title && !credential.portrait && !credential.logo;
  elements.credentialCard.style.backgroundColor = credential.backgroundColor ?? "";
  elements.credentialCard.style.color = credential.textColor ?? "";

  setImage(elements.credentialLogo, credential.logo);
  setImage(elements.portrait, credential.portrait);

  let hasDetailRow = false;
  for (const field of detailFields) {
    const row = elements.detailRows[field];
    const value = elements.detailValues[field];
    const fieldValue = credential[field];

    if (!row || !value) {
      continue;
    }

    const hasValue = setText(value, fieldValue);
    row.hidden = !hasValue;
    hasDetailRow ||= hasValue;
  }

  elements.additionalClaims.replaceChildren();
  for (const claim of credential.claims ?? []) {
    const row = document.createElement("div");
    const label = document.createElement("dt");
    const value = document.createElement("dd");
    label.textContent = claim.label;
    value.textContent = claim.value;
    row.append(label, value);
    elements.additionalClaims.append(row);
    hasDetailRow = true;
  }

  if (credential.issuer) {
    elements.verifiedByIssuer.textContent = credential.issuer;
    elements.verifiedBy.hidden = false;
  } else {
    elements.verifiedByIssuer.textContent = "";
    elements.verifiedBy.hidden = true;
  }

  elements.details.hidden = !hasDetailRow;
}

function mergeCredentialDisplay(...sources: Array<Partial<CredentialDisplay>>): CredentialDisplay {
  const merged: Partial<CredentialDisplay> = {};

  for (const source of sources) {
    for (const field of credentialDisplayFields) {
      const value = source[field];

      if (typeof value === "string" && value.trim()) {
        merged[field] = value;
      }
    }

    if (source.claims?.length) {
      merged.claims = source.claims;
    }
  }

  return merged;
}

function credentialDisplayFromTokenContent(displayInformation?: Array<Record<string, unknown>>): CredentialDisplay {
  const display = selectDisplayInformation(displayInformation);
  if (!display) {
    return {};
  }

  const logo = display.logo;
  return {
    backgroundColor: stringValue(display.background_color),
    logo: typeof logo === "string" ? logo : isRecord(logo) ? stringValue(logo.uri) : undefined,
    textColor: stringValue(display.text_color),
    title: stringValue(display.name)
  };
}

function selectDisplayInformation(displayInformation?: Array<Record<string, unknown>>) {
  if (!displayInformation?.length) {
    return undefined;
  }

  const languages = navigator.languages.map((language) => language.toLowerCase());
  return (
    displayInformation.find((entry) => {
      const locale = stringValue(entry.locale)?.toLowerCase();
      return locale ? languages.includes(locale) : false;
    }) ??
    displayInformation.find((entry) => {
      const language = stringValue(entry.locale)?.split("-")[0].toLowerCase();
      return language ? languages.some((candidate) => candidate.split("-")[0] === language) : false;
    }) ??
    displayInformation[0]
  );
}

function stringValue(value: unknown) {
  return typeof value === "string" && value.trim() ? value : undefined;
}

function isRecord(value: unknown): value is Record<string, unknown> {
  return typeof value === "object" && value !== null && !Array.isArray(value);
}

function setImage(element: HTMLImageElement, source?: string) {
  if (source) {
    element.onload = () => {
      element.hidden = false;
    };
    element.onerror = () => {
      element.hidden = true;
    };
    element.src = source;
    element.hidden = false;
  } else {
    element.removeAttribute("src");
    element.hidden = true;
  }
}

function setText(element: HTMLElement, value?: string) {
  const hasValue = typeof value === "string" && value.trim().length > 0;
  element.textContent = hasValue ? value : "";
  element.hidden = !hasValue;

  return hasValue;
}

function getVerifierElements(root: HTMLElement): VerifierElements | undefined {
  const credentialCard = query<HTMLElement>(root, "[data-credential-card]");
  const credentialLogo = query<HTMLImageElement>(root, "[data-credential-logo]");
  const additionalClaims = query<HTMLElement>(root, "[data-additional-claims]");
  const details = query<HTMLElement>(root, "[data-details]");
  const closeButton = query<HTMLButtonElement>(root, "[data-close]");
  if (!credentialCard || !credentialLogo || !additionalClaims || !details || !closeButton) {
    return undefined;
  }

  const detailRows = {
    createdAt: query<HTMLElement>(root, '[data-detail-row="createdAt"]'),
    expiresAt: query<HTMLElement>(root, '[data-detail-row="expiresAt"]'),
    issuer: query<HTMLElement>(root, '[data-detail-row="issuer"]'),
    publicKey: query<HTMLElement>(root, '[data-detail-row="publicKey"]')
  };
  const detailValues = {
    createdAt: query<HTMLElement>(root, '[data-detail-value="createdAt"]'),
    expiresAt: query<HTMLElement>(root, '[data-detail-value="expiresAt"]'),
    issuer: query<HTMLElement>(root, '[data-detail-value="issuer"]'),
    publicKey: query<HTMLElement>(root, '[data-detail-value="publicKey"]')
  };

  const elements = {
    additionalClaims,
    closeButton,
    closedView: query<HTMLElement>(root, "[data-closed-view]"),
    content: query<HTMLElement>(root, "[data-verifier-content]"),
    credentialCard,
    credentialLogo,
    details,
    detailRows,
    detailValues,
    error: query<HTMLElement>(root, "[data-error]"),
    invalidMark: query<HTMLElement>(root, "[data-invalid-mark]"),
    loadingMark: query<HTMLElement>(root, "[data-loading-mark]"),
    portrait: query<HTMLImageElement>(root, "[data-credential-portrait]"),
    resultText: query<HTMLElement>(root, "[data-result-text]"),
    root: query<HTMLElement>(root, "[data-verifier]"),
    title: query<HTMLElement>(root, "[data-credential-title]"),
    validMark: query<HTMLElement>(root, "[data-valid-mark]"),
    verifiedBy: query<HTMLElement>(root, "[data-verified-by]"),
    verifiedByIssuer: query<HTMLElement>(root, "[data-verified-by-issuer]")
  };

  return Object.values(elements).every(Boolean) ? (elements as VerifierElements) : undefined;
}

function query<T extends Element>(root: ParentNode, selector: string) {
  return root.querySelector<T>(selector) ?? undefined;
}
