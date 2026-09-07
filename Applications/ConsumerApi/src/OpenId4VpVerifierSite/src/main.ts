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

type AppState = {
  credential: CredentialDisplay;
  error?: string;
  isLoading?: boolean;
  isValid: boolean;
  isClosed?: boolean;
};

const credentialDisplayFields = ["createdAt", "expiresAt", "issuer", "portrait", "publicKey", "title"] as const;

const rootElement = document.querySelector<HTMLDivElement>("#openid4vp-verifier-root");

if (rootElement) {
  document.body.classList.add("openid4vp-verifier-visible");
  rootElement.innerHTML = renderVerifier({
    credential: {},
    isLoading: true,
    isValid: false
  });

  void initialize(rootElement);
}

async function initialize(appElement: HTMLElement) {
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

  appElement.innerHTML = renderVerifier({
    credential,
    error: result.error,
    isValid: result.isValid
  });

  document.querySelector<HTMLButtonElement>("[data-close]")?.addEventListener("click", () => {
    window.close();

    window.setTimeout(() => {
      appElement.innerHTML = renderVerifier({
        credential,
        isClosed: true,
        isValid: result.isValid
      });
    }, 120);
  });
}

function showOnboarding() {
  document.body.classList.remove("openid4vp-verifier-visible");
  rootElement?.replaceChildren();
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

  return merged as CredentialDisplay;
}

function renderVerifier(state: AppState) {
  if (state.isClosed) {
    return `
      <main class="openid4vp-verifier verifier is-closed" aria-label="OpenID4VP Nachweisprüfung geschlossen">
        <section class="closed-view" aria-live="polite">
          <p>Sie können dieses Fenster jetzt schließen.</p>
        </section>
      </main>
    `;
  }

  const statusClass = state.isLoading ? "is-loading" : state.isValid ? "is-valid" : "is-invalid";
  const statusText = state.isValid ? "ist gültig." : "ist ungültig.";
  const title = state.credential.title ? escapeHtml(state.credential.title) : undefined;
  const portrait = state.credential.portrait ? `<img class="pass-portrait" src="${escapeAttribute(state.credential.portrait)}" alt="" />` : "";

  return `
    <main class="openid4vp-verifier verifier ${statusClass}" aria-label="OpenID4VP Nachweisprüfung">
      <p class="intro">
        Ein Nachweis wurde Ihnen präsentiert.<br />
        Überprüfen Sie die Gültigkeit!
      </p>

      ${state.isLoading ? "" : renderCredential(state.credential, portrait, title)}

      <section class="result" aria-live="polite">
        ${
          state.isLoading
            ? `<p>Der Nachweis<br />wird geprüft.</p>${renderLoader()}`
            : `<p>Der präsentierte Nachweis<br />${statusText}</p>${state.isValid ? renderCheckmark() : renderCross()}`
        }
      </section>

      ${state.error ? `<p class="sr-only">Prüfhinweis: ${escapeHtml(state.error)}</p>` : ""}

      ${state.isLoading ? "" : `<button class="close-button" type="button" data-close>Hinweis schließen</button>`}
    </main>
  `;
}

function renderLoader() {
  return `<div class="status-mark loading-mark" aria-hidden="true"></div>`;
}

function renderCheckmark() {
  return `
    <div class="status-mark" aria-hidden="true">
      <svg viewBox="0 0 120 120">
        <path d="M36 62.5 52.5 79 86 40" fill="none" stroke="currentColor" stroke-width="7" />
      </svg>
    </div>
  `;
}

function renderCross() {
  return `
    <div class="status-mark" aria-hidden="true">
      <svg viewBox="0 0 120 120">
        <path d="m43 43 34 34M77 43 43 77" fill="none" stroke="currentColor" stroke-width="7" />
      </svg>
    </div>
  `;
}

function escapeHtml(value: string) {
  return value
    .replaceAll("&", "&amp;")
    .replaceAll("<", "&lt;")
    .replaceAll(">", "&gt;")
    .replaceAll('"', "&quot;")
    .replaceAll("'", "&#039;");
}

function escapeAttribute(value: string) {
  return escapeHtml(value).replaceAll("`", "&#096;");
}

function renderCredential(credential: CredentialDisplay, portrait: string, title?: string) {
  const detailRows = [
    detailRow("Aussteller", credential.issuer),
    detailRow("Erstellt", credential.createdAt),
    detailRow("Gültig bis", credential.expiresAt),
    detailRow("Public Key", credential.publicKey)
  ].join("");
  const card =
    title || portrait
      ? `<section class="pass-card" aria-label="${title ?? "Nachweis"}">
          <div class="pass-logo" aria-hidden="true">
            <span></span><span></span><span></span>
          </div>
          ${portrait}
          ${title ? `<h1>${title}</h1>` : ""}
        </section>`
      : "";

  return `
    ${card}

    ${
      detailRows
        ? `<section class="details" aria-label="Nachweisdetails">
            ${credential.issuer ? verifiedBy(credential.issuer) : ""}
            <dl>${detailRows}</dl>
          </section>`
        : ""
    }
  `;
}

function verifiedBy(issuer: string) {
  return `
    <div class="verified-by">
      <svg viewBox="0 0 24 24" aria-hidden="true">
        <path d="M12 3 5 6v5c0 4.5 2.8 8.7 7 10 4.2-1.3 7-5.5 7-10V6l-7-3Z" fill="none" stroke="currentColor" stroke-width="1.8" />
        <path d="m9 12 2 2 4-5" fill="none" stroke="currentColor" stroke-width="1.8" stroke-linecap="round" stroke-linejoin="round" />
      </svg>
      <span>verifiziert durch ${escapeHtml(issuer)}</span>
    </div>
  `;
}

function detailRow(label: string, value?: string) {
  return typeof value === "string" && value.trim()
    ? `<div>
        <dt>${escapeHtml(label)}</dt>
        <dd>${escapeHtml(value)}</dd>
      </div>`
    : "";
}
