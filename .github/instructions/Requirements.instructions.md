eLetter25 – Anforderungsliste (Final)\
Stand: 2026-02-08\
Scope: Single-User, lokal, robuste asynchrone Verarbeitung (Hangfire), Domain Events beibehalten

1. Zielbild\
   eLetter25 dient der Verwaltung privater Dokumente (PDF) inkl. Upload, Textgewinnung (PDF-Text-Extraktion + OCR fallback), Review/Metadatenpflege, Archivierung und leistungsfähiger Suche (Volltext, Stemming, Fuzzy) über OCR-Text, Tags, Kontakte, Belegarten und Custom Fields. Verarbeitung ist robust asynchron (wiederaufnehmbar, Retry/Failed).
2. Fachliche Anforderungen

2.1 Dokumenteingang (Upload)

- Das System muss PDF-Dateien über die UI hochladen können.
- Beim Upload muss optional eine Belegart auswählbar sein.
- Upload darf nicht blockieren; der Nutzer erhält sofort eine Bestätigung.
- Nach Upload wird ein Dokument im Status `Registered` im Posteingang angelegt.

2.2 Dateiablage (File Storage)

- Dateien müssen im Dateisystem abgelegt werden.
- Ablageformat: `{BasePath}\{DocumentId}.pdf` (nur DocumentId, kein Unterordner nach Datum).
- Original-Dateiname muss separat in der DB gespeichert werden.
- Storage ist über ein Interface abstrahiert, um später Docker/Volumes zu ermöglichen.

2.3 Dokument-Lifecycle und Status\
Das System muss den dokumentzentrierten Lifecycle verwalten:

- `Registered` (Upload abgeschlossen, Datei gespeichert, Datensatz vorhanden)
- `Processing` (Extraktion/OCR/Parsing läuft)
- `ValidationNeeded` (Verarbeitung fertig, Review erforderlich)
- `Archived` (final abgelegt)
- `Failed` (Verarbeitung fehlgeschlagen; Retry möglich)
- `Deleted` (Papierkorb)

Statusregeln:

- Verarbeitung setzt Status `Processing`.
- Erfolgreiche Verarbeitung setzt Status `ValidationNeeded`.
- Archivierung setzt Status `Archived`.
- Löschen verschiebt nach `Deleted` (kein Hard Delete).
- Nur manuelles Leeren des Papierkorbs löscht endgültig (DB + Dateisystem).

2.4 Asynchroner Workflow (robust mit Hangfire)

- Nach Upload muss automatisch ein Hangfire-Job gestartet werden, der das Dokument verarbeitet.
- Der Job muss robust sein:
    - Überlebt Neustarts (persistente Hangfire Storage in PostgreSQL).
    - Hat definierte Retry-Policy.
    - Markiert nach Max-Retries Status `Failed`.
- Es muss eine manuelle Aktion „Erneut verarbeiten“ geben (mindestens für `Failed`, optional auch für `ValidationNeeded`).
- Parallelität muss begrenzt sein (Default: 1–2 gleichzeitige OCR-Jobs, konfigurierbar).

2.5 Textgewinnung (Extract-then-OCR)

- Das System muss zuerst versuchen, Text direkt aus dem PDF zu extrahieren (ohne OCR).
- Wenn extrahierter Text leer/ungenügend ist, muss OCR ausgeführt werden.
- OCR muss Deutsch und Englisch unterstützen.
- Der finale Suchtext muss persistent gespeichert werden (für Volltext/Fuzzy/Stemming).

2.6 Datumserkennung (Belegdatum)

- Nach Textgewinnung muss ein Belegdatum automatisch vorgeschlagen werden.
- Heuristik/Regex-basiert, nicht „KI-magisch“ erforderlich.
- Nutzer muss den Vorschlag im Review bestätigen/ändern können.
- Upload-Datum und Belegdatum sind getrennte Felder.

2.7 Absender & Kontakte (Snapshot + Link)

- Jedes Dokument muss einen unveränderbaren „Original-Absender“ (Snapshot-Text) speichern.
- Zusätzlich kann ein Dokument optional mit einem Contact verknüpft sein.
- Contact ist „lebend“ und verwaltbar (Name, Adresse, Telefon, E-Mail).
- Änderungen am Contact dürfen niemals den Dokument-Snapshot verändern.

2.8 Belegarten (Document Types) und Felddefinitionen

- Belegarten müssen in der UI verwaltbar sein (CRUD).
- Jede Belegart definiert dynamische Felder (FieldDefinitions) mit:
    - Key (intern, stabil)
    - Label (UI)
    - DataType: Text, Number, Date, Currency, Boolean
    - IsRequired (Pflichtfeld)
    - Optional: Validierungsregeln (Regex, Min/Max, Länge, Decimal Places)
    - SearchMode (siehe 2.9)

Änderungsverhalten:

- Änderungen an Belegarten/Feldern gelten:
    - für neue Dokumente sofort,
    - für bestehende Dokumente erst „ab dem nächsten Speichern/Aktualisieren der Metadaten“.
- Archivierte Dokumente werden nicht automatisch als „ungültig“ markiert. Validierung greift nur bei aktiver Änderung/Speichern.

2.9 Custom Fields (Werte) pro Dokument

- Pro Dokument müssen dynamische Feldwerte gespeichert werden.
- Beim Speichern/Archivieren muss serverseitig validiert werden:
    - Pflichtfelder vorhanden
    - Typ korrekt
    - Zusatzregeln erfüllt
- Custom Fields müssen in Suche/Filterung nutzbar sein gemäß SearchMode:

SearchMode (pro FieldDefinition):

- `Exact`: exakte Suche (z.B. Rechnungsnummer, Versicherungsnummer)
- `FullText`: Stemming/Volltext (keine Fuzzy-Ähnlichkeit nötig)
- `FullTextAndFuzzy`: Stemming + Tippfehler-Toleranz

2.10 Tags

- Tags müssen frei vergeben werden können.
- Autocomplete muss vorhandene Tags vorschlagen.
- Tags müssen filterbar sein.

2.11 Review & Archivierung

- Es muss eine Review-Oberfläche geben:
    - PDF Viewer
    - Formular für Metadaten (Belegart, Custom Fields, Tags, Contact-Link, Absender-Snapshot, Belegdatum)
- Archivierung setzt Status `Archived`.
- Snapshot-Felder sind nach Archivierung unveränderbar.
- Metadaten sind nach Archivierung weiterhin editierbar (Tags/Contact/Custom Fields), sofern du nicht später eine strengere Policy einführst.

2.12 Suche (Volltext, Stemming, Fuzzy)

- Suche muss Volltext über extrahierten/OCR-Text bieten.
- Suche muss Stemming unterstützen (Deutsch und Englisch).
- Suche muss Fuzzy unterstützen (Tippfehler tolerant).
- Suche muss kombinierbar sein mit Filtern:
    - Tags
    - Belegart
    - Datum (Range)
    - Contact/Absender
    - Custom Fields (gemäß SearchMode, „Exact“ strikt)
- Ranking:
    - Treffer in Absender/Contact höher gewichten als nur Fließtext.
    - Exact-Matches (Exact-Felder) höher gewichten als fuzzy Treffer.

2.13 Papierkorb (Soft Delete)

- Löschen verschiebt Dokumente in Status `Deleted`.
- Dokumente im Papierkorb bleiben such-/sichtbar (separater View), aber standardmäßig nicht in normalen Suchergebnissen.
- Papierkorb darf nur manuell geleert werden.
- Beim Leeren werden:
    - DB-Datensatz und alle abhängigen Daten entfernt
    - Datei `{DocumentId}.pdf` im Dateisystem gelöscht

3. Nicht-funktionale Anforderungen

3.1 Robustheit & Idempotenz

- OCR/Processing-Jobs müssen idempotent sein:
    - Wiederholte Ausführung darf keine doppelten Dokumente/Artefakte erzeugen.
    - Statusübergänge müssen guard-basiert erfolgen.
- Fehler müssen nachvollziehbar sein (UI: verständliche Meldung; Logs: technische Details).

3.2 Performance & UX

- Upload darf nie an OCR-Zeit gekoppelt sein.
- Verarbeitung läuft im Hintergrund; UI zeigt Status und Fortschritt (mindestens: Status, optional: Seitenzahl/Step).
- Suche soll schnell sein; Indexe sind Pflicht.

3.3 Wartbarkeit / Clean Architecture

- Clean Architecture bleibt: Domain/Application/Infrastructure/API.
- Domain Events bleiben im Domain Layer als fachliche Signale.
- Technische Ausführung (OCR, Storage, Jobs) bleibt in Infrastructure/Worker.

4. Technische Anforderungen (konkret)

4.1 Datenbank

- PostgreSQL ist Single Source of Truth für Dokumentdaten, Metadaten, Belegarten, Tags, Kontakte sowie Hangfire Storage.
- Custom Fields werden als jsonb gespeichert (oder äquivalent strukturiert, aber jsonb ist Default).
- Suchmechanik basiert auf PostgreSQL Full Text Search + Erweiterungen/Indexierung für Fuzzy/Stemming.

4.2 Hangfire

- Hangfire läuft im gleichen Host (Monolith), aber mit persistentem Storage in PostgreSQL.
- Mindest-Jobs:
    - `ProcessDocumentJob(documentId)`
    - optional `ReprocessDocumentJob(documentId)` (kann alias sein)
- Dashboard ist lokal verfügbar und geschützt (Windows Auth ok).
- Retry Policy + Failed Handling sind konfiguriert.

4.3 OCR / Text Extraction

- Es gibt Ports:
    - `ITextExtractor` (PDF Text)
    - `IOcrService` (OCR für de/en)
- Implementierungen sind austauschbar (später Docker).

4.4 Storage

- Port `IDocumentStorage`.
- Default: Windows-Dateisystem, Pfadkonfiguration über Settings.
- Späterer Wechsel zu Docker Volume darf ohne Domain/Application-Änderungen möglich sein.

4.5 Domain Events (fachlich)

- Domain Events werden mindestens für Statuswechsel genutzt, z.B.:
    - `DocumentRegistered`
    - `DocumentProcessingStarted`
    - `DocumentProcessingCompleted` / `OcrCompleted`
    - `DocumentProcessingFailed`
    - `DocumentArchived`
    - `DocumentDeleted` / `TrashEmptied`
- Domain Events dürfen nicht direkt OCR ausführen; sie sind Trigger/Signal.
