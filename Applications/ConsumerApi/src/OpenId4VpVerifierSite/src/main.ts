import "reflect-metadata";
import "./styles.css";
import { verifyPresentedCredential } from "./verification";

type CredentialDisplay = {
  createdAt: string;
  expiresAt: string;
  issuer: string;
  portrait?: string;
  publicKey: string;
  title: string;
};

type AppState = {
  credential: CredentialDisplay;
  error?: string;
  isLoading?: boolean;
  isValid: boolean;
};

const defaultCredential: CredentialDisplay = {
  createdAt: "21.11.2025",
  expiresAt: "20.11.2027",
  issuer: "Stadt Heidelberg",
  portrait: "/openid4vp-verifier/heidelberg-pass-portrait.png",
  publicKey: "8984-9874-6263-9485-9475",
  title: "Heidelberg-Pass"
};

const appElementCandidate = document.querySelector<HTMLDivElement>("#app");

if (!appElementCandidate) {
  throw new Error("Could not find app container.");
}

const appElement = appElementCandidate;

appElement.innerHTML = renderShell({
  credential: defaultCredential,
  isLoading: true,
  isValid: false
});

void initialize();

async function initialize() {
  const root = document.querySelector<HTMLElement>(".verifier");

  if (root) {
    root.dataset.loading = "true";
  }

  const result = await verifyPresentedCredential();
  const credential = {
    ...defaultCredential,
    ...result.credential
  };

  appElement.innerHTML = renderShell({
    credential,
    error: result.error,
    isValid: result.isValid
  });

  document.querySelector<HTMLButtonElement>("[data-close]")?.addEventListener("click", () => {
    window.close();

    if (!window.closed) {
      window.location.href = "/";
    }
  });
}

function renderShell(state: AppState) {
  const statusClass = state.isLoading ? "is-loading" : state.isValid ? "is-valid" : "is-invalid";
  const statusText = state.isValid ? "ist gültig." : "ist ungültig.";
  const title = escapeHtml(state.credential.title);
  const issuer = escapeHtml(state.credential.issuer);
  const publicKey = escapeHtml(state.credential.publicKey);
  const portrait = state.credential.portrait
    ? `<img class="pass-portrait" src="${escapeAttribute(state.credential.portrait)}" alt="" />`
    : `<div class="pass-portrait fallback-portrait" aria-hidden="true"></div>`;

  return `
    <main class="verifier ${statusClass}" aria-label="OpenID4VP Nachweisprüfung">
      <div class="phone-status" aria-hidden="true">
        <span>10:41</span>
        <span class="status-icons">
          <span class="signal"></span>
          <span class="wifi"></span>
          <span class="battery"></span>
        </span>
      </div>

      <p class="intro">
        Ein Nachweis wurde Ihnen präsentiert.<br />
        Überprüfen Sie die Gültigkeit!
      </p>

      <section class="pass-card" aria-label="${title}">
        <div class="pass-logo" aria-hidden="true">
          <span></span><span></span><span></span>
        </div>
        ${portrait}
        <h1>${title}</h1>
      </section>

      <section class="details" aria-label="Nachweisdetails">
        <div class="verified-by">
          <svg viewBox="0 0 24 24" aria-hidden="true">
            <path d="M12 3 5 6v5c0 4.5 2.8 8.7 7 10 4.2-1.3 7-5.5 7-10V6l-7-3Z" fill="none" stroke="currentColor" stroke-width="1.8" />
            <path d="m9 12 2 2 4-5" fill="none" stroke="currentColor" stroke-width="1.8" stroke-linecap="round" stroke-linejoin="round" />
          </svg>
          <span>verifiziert durch die Stadt Heidelberg</span>
        </div>
        <dl>
          <div>
            <dt>Aussteller</dt>
            <dd>${issuer}</dd>
          </div>
          <div>
            <dt>Erstellt</dt>
            <dd>${escapeHtml(state.credential.createdAt)}</dd>
          </div>
          <div>
            <dt>Gültig bis</dt>
            <dd>${escapeHtml(state.credential.expiresAt)}</dd>
          </div>
          <div>
            <dt>Public Key</dt>
            <dd>${publicKey}</dd>
          </div>
        </dl>
      </section>

      <section class="result" aria-live="polite">
        ${
          state.isLoading
            ? `<p>Der Nachweis<br />wird geprüft.</p>${renderLoader()}`
            : `<p>Der präsentierte Nachweis<br />${statusText}</p>${state.isValid ? renderCheckmark() : renderCross()}`
        }
      </section>

      ${state.error ? `<p class="sr-only">Prüfhinweis: ${escapeHtml(state.error)}</p>` : ""}

      <button class="close-button" type="button" data-close>Hinweis schließen</button>
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
