# OpenID4VP Verifier – Hinweise für AI Agents

Diese Datei gilt für den gesamten Ordner `OpenId4VpVerifierSite`. Halte sie bei jeder Änderung am Feature aktuell. Wenn sich Architektur, Validierungsregeln, unterstützte Formate, Integrationspunkte, Build-Schritte, Tests oder Designvorgaben ändern, aktualisiere diese Datei im selben Change.

## Zweck und Ablauf

Dieses Vite-/TypeScript-Bundle ergänzt die Consumer-API-Seite `/r/{referenceId}` um eine browserseitige Prüfung präsentierter Nachweise.

1. `referenceContent.ts` liest den Schlüssel aus dem URL-Fragment. Das Fragment ist base64url-kodiert und enthält `algorithm|key|forIdentity|passwordProtection`; aktuell wird nur Algorithmus `3` (`XCHACHA20_POLY1305`) unterstützt.
2. Der verschlüsselte Inhalt wird über den Token- oder RelationshipTemplate-Endpunkt der Consumer API geladen und im Browser entschlüsselt.
3. Nur Inhalte mit `@type: "TokenContentVerifiablePresentation"` werden als Präsentation behandelt. Andernfalls bleibt die reguläre Onboarding-Seite sichtbar.
4. `verification.ts` validiert `value`. Als erwartete Nonce wird die Reference-ID verwendet, als Audience derzeit fest `defaultPresentationAudience`.
5. `main.ts` verbindet Lade- und Validierungslogik mit dem vorhandenen DOM und zeigt Status sowie Credential-Inhalte an.

Das URL-Fragment wird vom Browser nicht an den Server übertragen. Verschiebe den darin enthaltenen Entschlüsselungsschlüssel nicht in Query-Parameter oder serverseitige Requests.

## Verantwortlichkeiten der Dateien

- `src/referenceContent.ts`: Referenz-Fragment parsen, API-Endpunkte auswählen, verschlüsselten Inhalt laden und entschlüsseln. Fehler führen dazu, dass kein VP-Inhalt zurückgegeben wird.
- `src/verification.ts`: Validierung und Extraktion der anzuzeigenden Credential-Daten. Der öffentliche Einstiegspunkt ist `validatePresentedCredential(presentation, options)`; Input rein, `VerificationOutcome` raus, ohne DOM-Abhängigkeit.
- `src/main.ts`: UI-Orchestrierung, DOM-Zugriff, deutsche Texte, Zuordnung von `PresentationValidationErrorCode` zu nutzerfreundlichen Meldungen und Zusammenführung optionaler `displayInformation`.
- `src/verificationKeyManagement.ts`: Browser-KMS für reine Signaturprüfung mit öffentlichen JWKs. Keine Schlüsselgenerierung, kein Import privater Schlüssel und kein Signieren.
- `src/verificationStorage.ts`: Flüchtige Storage-/Filesystem-Adapter, die Credo im Browser zum Initialisieren benötigt. Es werden keine Verifier-Daten dauerhaft gespeichert.
- `src/styles.css`: Ausschließlich Verifier-Styles; Selektoren unter `.openid4vp-verifier` beziehungsweise `#openid4vp-verifier-root` kapseln.
- `test/verification.test.ts`: Echte kryptografische Unit Tests der Validierungslogik ohne Mocks.

## Validierungsregeln

Unterstützt werden derzeit kompakte SD-JWT VCs mit Key Binding sowie JWT- und JSON-LD-basierte VCs/VPs, soweit Credo sie prüfen kann. Ein erfolgreicher Status erfordert:

- mindestens eine Präsentation beziehungsweise ein Credential;
- vorhandene erwartete Nonce und Audience;
- gültige kryptografische Signaturen;
- bei SD-JWT eine gültige Issuer-Signatur, gültige Disclosures, eine gültige Holder-Key-Binding-Signatur und einen passenden `sd_hash`;
- Übereinstimmung von Nonce und Audience mit dem aktuellen Prüfvorgang;
- einen unterstützten SD-JWT-Typ und das Pflichtfeld `vct`;
- einen bereits begonnenen und noch nicht abgelaufenen Gültigkeitszeitraum;
- bei einer VP mindestens ein eingebettetes Credential;
- bei mehreren Präsentationen beziehungsweise Credentials die Gültigkeit jedes einzelnen Elements.

Signer-Schlüssel werden aus eingebetteten JWKs, `x5c` oder unterstützten DID-URLs aufgelöst. Die Credo-Konfiguration enthält Resolver für `did:key`, `did:jwk` und `did:web`.

Wichtige bewusste Grenzen:

- Credential-Status beziehungsweise Widerruf wird aktuell nicht geprüft (`verifyCredentialStatus: false`).
- Bei `x5c` wird der öffentliche Schlüssel des Zertifikats für die Signaturprüfung verwendet; eine Zertifikatskette oder das Vertrauen in den Aussteller wird nicht validiert.
- Eine gültige Signatur allein bedeutet daher nicht, dass der Aussteller fachlich vertrauenswürdig ist.

Ändere diese Grenzen nicht beiläufig. Sicherheitsrelevante Erweiterungen benötigen passende positive und negative Tests.

## Fehlerbehandlung

Validierungsergebnisse enthalten keine Fehlermeldung als Freitext, sondern einen Wert aus `PresentationValidationErrorCode`. Ergänze für neue Fehlerfälle:

1. einen eindeutigen Enum-Wert in `verification.ts`;
2. eine korrekte Zuordnung im Validierungspfad;
3. eine kurze, nicht technische deutsche UI-Meldung in `validationErrorMessages` in `main.ts`;
4. mindestens einen Unit Test, der exakt den Error Code prüft.

Unbekannte Fehler dürfen nicht als gültig behandelt werden und werden auf `VerificationFailed` abgebildet. Interne technische Details, Bibliotheksfehler, Schlüsselmaterial und komplette Präsentationen gehören nicht in sichtbare Fehlermeldungen.

## UI- und Designvorgaben

Das Markup liegt nicht in diesem Vite-Projekt, sondern in `../Views/AppOnboarding/AppOnboarding.cshtml`. `main.ts` erwartet die dortigen `data-*`-Elemente. Ändere Markup, TypeScript-Abfragen und CSS gemeinsam, wenn dieser DOM-Vertrag angepasst wird. Erzeuge fehlende Elemente nicht dynamisch als Fallback; bei einem unvollständigen DOM soll der Verifier nicht initialisieren.

Alle sichtbaren Texte einschließlich Fehler- und Accessibility-Texte müssen auf Deutsch sein. Fehlermeldungen sollen verständlich und wenig technisch formuliert werden. Verwende `textContent`, nicht `innerHTML`, für Daten aus Präsentationen.

Alle präsentierten fachlichen Claims sollen angezeigt werden. Technische Metadaten aus `technicalClaimNames` und Bildfelder aus `imageClaimNames` werden davon bewusst ausgenommen beziehungsweise separat dargestellt. Fehlende Werte werden ausgeblendet; keine erfundenen Beispieldaten oder Anzeige-Fallbacks hinzufügen. Der Aussteller steuert zusätzlich den Bereich „verifiziert durch“. Optionale `displayInformation` aus dem Token kann Titel, Logo und Farben vorgeben.

Designquelle: [Frosch Wallet App in Figma](https://www.figma.com/design/D15DcZItr1P4lCa61vfOWN/Frosch-Wallet-App?node-id=73604-148644&m=dev). Für Designänderungen das Figma-Plugin verwenden und die Desktop-Screens direkt unterhalb des verlinkten Elements berücksichtigen; die ersten beiden dortigen Screens sind nur für die Mobile-App. Der blaue Außenrahmen in Figma stellt ein Smartphone dar und gehört nicht zum Web-UI.

## Einbettung und Build

- `vite.config.ts` erzeugt feste Namen unter `dist/openid4vp-verifier/assets/verifier.{js,css}` mit Basis-Pfad `/openid4vp-verifier/`.
- `../ConsumerApi.csproj` führt bei normalen Builds `npm ci` und `npm run build` aus und kopiert das Ergebnis nach `../wwwroot/openid4vp-verifier`.
- `../Dockerfile` baut das Bundle in einer eigenen Node-Stufe und kopiert es in das Consumer-API-Image.
- `../Views/AppOnboarding/AppOnboarding.cshtml` bindet CSS und JavaScript über `IFileVersionProvider` mit Cache-Busting ein.
- `../../../../.github/workflows/test.yml` installiert die Abhängigkeiten und führt `npm test` im Unit-Test-Job aus.

`node_modules/`, `dist/` und `../wwwroot/openid4vp-verifier/` sind generierte beziehungsweise kopierte Artefakte. Nicht direkt bearbeiten oder committen. Änderungen gehören in `src/`, das Razor-Markup oder die Build-Konfiguration. `package-lock.json` muss bei Abhängigkeitsänderungen zusammen mit `package.json` aktualisiert werden.

## Tests und lokale Prüfung

Führe im Verifier-Ordner mindestens aus:

```sh
npm ci
npm run typecheck
npm test
npm run build
```

Die Tests für `validatePresentedCredential` sollen reine Input-/Output-Tests ohne Mocks bleiben. Erzeuge signierte Test-Präsentationen mit echten Testschlüsseln und injiziere über `options.now` eine feste Zeit, damit Zeitprüfungen deterministisch sind. Decke bei Änderungen sowohl den Erfolgsfall als auch Manipulationen, falsche Bindungswerte, Zeitgrenzen, fehlende Pflichtdaten und Arrays mit teilweise ungültigen Präsentationen ab.

Wenn Markup oder Consumer-API-Einbettung geändert werden, führe zusätzlich die betroffenen .NET-Integrationstests beziehungsweise mindestens einen Build von `../ConsumerApi.csproj` aus. Bekannte Warnungen aus transitiven Kryptografie-Abhängigkeiten beim Vite-Build nicht mit Fehlern verwechseln, aber neue Warnungen prüfen und dokumentieren.
