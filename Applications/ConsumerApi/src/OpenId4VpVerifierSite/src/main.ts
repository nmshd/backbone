import "reflect-metadata";
import "./styles.css";
import { tryLoadVerifiablePresentationTokenContent } from "./referenceContent";
import { verifyPresentedCredential } from "./verification";

type CredentialDisplay = {
  createdAt?: string;
  expiresAt?: string;
  issuer?: string;
  portrait?: string;
  publicKey?: string;
  title?: string;
};

type VerificationStatus = "loading" | "valid" | "invalid";

type VerifierElements = {
  closeButton: HTMLButtonElement;
  closedView: HTMLElement;
  content: HTMLElement;
  credentialCard: HTMLElement;
  details: HTMLElement;
  detailRows: Record<keyof CredentialDisplay, HTMLElement | undefined>;
  detailValues: Record<keyof CredentialDisplay, HTMLElement | undefined>;
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

const credentialDisplayFields = ["createdAt", "expiresAt", "issuer", "portrait", "publicKey", "title"] as const;
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

  const result = await verifyPresentedCredential(tokenContent.value, {
    expectedAudience: "defaultPresentationAudience",
    expectedNonce: appElement.dataset.referenceId
  });
  const credential = mergeCredentialDisplay(result.credential);

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
  elements.credentialCard.hidden = !credential.title && !credential.portrait;

  if (credential.portrait) {
    elements.portrait.src = credential.portrait;
    elements.portrait.hidden = false;
  } else {
    elements.portrait.removeAttribute("src");
    elements.portrait.hidden = true;
  }

  let hasDetailRow = false;
  for (const field of credentialDisplayFields) {
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
  }

  return merged;
}

function setText(element: HTMLElement, value?: string) {
  const hasValue = typeof value === "string" && value.trim().length > 0;
  element.textContent = hasValue ? value : "";
  element.hidden = !hasValue;

  return hasValue;
}

function getVerifierElements(root: HTMLElement): VerifierElements | undefined {
  const detailRows = {
    createdAt: query<HTMLElement>(root, '[data-detail-row="createdAt"]'),
    expiresAt: query<HTMLElement>(root, '[data-detail-row="expiresAt"]'),
    issuer: query<HTMLElement>(root, '[data-detail-row="issuer"]'),
    portrait: undefined,
    publicKey: query<HTMLElement>(root, '[data-detail-row="publicKey"]'),
    title: undefined
  };
  const detailValues = {
    createdAt: query<HTMLElement>(root, '[data-detail-value="createdAt"]'),
    expiresAt: query<HTMLElement>(root, '[data-detail-value="expiresAt"]'),
    issuer: query<HTMLElement>(root, '[data-detail-value="issuer"]'),
    portrait: undefined,
    publicKey: query<HTMLElement>(root, '[data-detail-value="publicKey"]'),
    title: undefined
  };

  const elements = {
    closeButton: query<HTMLButtonElement>(root, "[data-close]"),
    closedView: query<HTMLElement>(root, "[data-closed-view]"),
    content: query<HTMLElement>(root, "[data-verifier-content]"),
    credentialCard: query<HTMLElement>(root, "[data-credential-card]"),
    details: query<HTMLElement>(root, "[data-details]"),
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
