# Original XML diagnostic locations

<!-- Reborn: describe source provenance honestly without implying new XML/compiler capability. -->
The alpha frontend reports source syntax, schema and selected unsupported-value/
structure errors as `Error: C:\path\source.xml(line,column): message` in Studio's
build/error logs. Detailed exception stacks remain in the separate raw log.
Columns are one-based character positions, not byte offsets.

Locations come from bounded, captured original XML snapshots. XML declarations,
comments, blank lines and multiline attributes therefore count normally.
Schema errors on a compact prepared document are rebound to the authored field;
the generated document's line number is not substituted for its source location.
Derived attributes, instance bases and confined EA XInclude fragments participate
in this lookup. Comparison-only ID normalization does not change the source ID.

When a field cannot be uniquely attributed, the log uses the declaring asset or
entry context and explicitly appends
`[source context; exact field origin unavailable]`. A parser/security error that
does not expose a line says `line unavailable`; no coordinate is fabricated.
This is not a complete expression/macro expansion source map. Ambiguous duplicate
origins, missing/default-generated fields and some legacy compiler exceptions
can still require the labelled context and raw diagnostic log.

Missing include/resource paths retain their referring source location when the
literal is unique. Dependency JSON now includes the path-audit location witness;
missing asset/definition findings retain their existing source/line/owner fields.
DTD processing and external entity/schema/network resolution remain disabled.

## Verification

<!-- Reborn: developer-only regression commands are not alternative user entry points. -->
`sdk-xml-diagnostics-self-test <fresh-output>` covers syntax/security refusals,
schema-node provenance, inherited fields, fragments, comparison masks, ambiguous
origins, immutable snapshots and actual Conscript frontend refusals.
`Test-Ra3Ep1BuildLog.ps1` checks GUI-facing coordinate preservation.
`Test-Ra3Ep1DevSdk.ps1 -DiagnosticsOnly` checks failed builds and last-good package
retention in the developer slot, restores its original source and rebuilds it.
It never edits/builds the user's `Reborn` mod or launches the game.

Verified October 21, 2026: 22 offline coordinate checks and 7 live driver checks
passed, together with the inheritance/fragment, missing-dependency and build-log
regressions. The restored developer package retained SHA-256
`87241958D0EC4CDB3A133FFC837E023A01B51716924F6B532A30BCBDE7EDF918`.
The user's Reborn source/output were unchanged; no game was executed.

This change adds diagnostics, not arbitrary GameObject/module topology support.
The reviewed source/native capability gates remain in force.
