# Missing asset and definition reporting

<!-- Reborn: document the actual source preflight scope, including the distinction between mod-produced identities and stock dependencies. -->

The live Studio build now runs a dependency preflight in step 6 before the
normal fail-fast compiler. It collects independent failures rather than showing
only the first missing name. An error stops publication and preserves the last
successful BIG/SKUDef.

## Asset availability

The lookup union consists of:

- Asset identities declared by the entry XML and its `type="all"` emission
  closure, using actual Core type/instance hashing.
- Asset identities in the explicitly configured stock static/global manifests.

Mod overrides and repeated stock identities are deduplicated by type/instance
ID. Schema-declared strong references accept compatible concrete/derived types;
a matching name on an incompatible asset type is not enough. Merely importing
an asset through `type="instance"` does not make it a mod-produced runtime
asset. Unrelated XML files elsewhere under SageXML are not assumed compiled.
Source targets take precedence over stock manifest fallback; within the selected
scope an exact type wins before derived-type fallback, following Core resolution.

`MISSINGASSET` identifies the absent target, referring asset, XML field,
source path and line/column. `AMBIGUOUSASSET` identifies multiple compatible
concrete targets rather than choosing one silently.

## Definition visibility and chains

Definitions follow captured all/instance Include visibility and the existing
earlier-local declaration rule, in each document's own context. A child document
does not inherit definitions from its consumer. Shared diamond imports from the
same origin coalesce; conflicting origins are reported.

- `MISSINGDEF`: a `$NAME` is undefined in that use site's visible scope.
- `FORWARDDEF`: the name is declared later locally, rather than absent entirely.
- `BLOCKEDDEF`: a used definition itself depends on a missing/forward name.
- `AMBIGUOUSDEF`: duplicate or conflicting definition origins.

Every definition is inspected, even if no asset currently consumes it. A
definition referencing another absent definition reports its own `Define:NAME`
owner, not just the eventual asset that fails to evaluate. Quoted dollar text
is not mistaken for a reference. Multiple independent uses retain separate
source locations.

## Output and limitations

Studio displays severity-prefixed use-site diagnostics plus totals. The latest
machine-readable list is `alpha/.work/last-dependencies.json`; readable failure
and full compiler logs remain separate. Reports are generated before temporary
work cleanup and never written into editable mod sources.

This is not an unused-asset sweep or proof of complete runtime closure.
The preflight checks explicit expressions and schema-typed strong references
in captured emitted source. It does not flatten arbitrary inheritance, invent
compiler defaults, prove weak/hash-only or dynamic string lookups, evaluate all
EA expression forms, tokenize custom/list-valued references, or recursively
discover all stock streams. Fields present
only in instance bases/defaults remain subject to the subsequent actual compiler
checks; they are not claimed as fully aggregated by this preflight. Current
Conscript final native/import validation still runs independently.

The stock catalog is the configured manifests, not every file installed with
the game. Additional supported stream types must explicitly extend that catalog.
If source/path inspection is incomplete, the report marks that fact and emits
errors rather than claiming a complete dependency pass. Schema/processor support
and the existing general GameObject inheritance blocker are unchanged.

## Verification

The isolated diagnostic test covers multiple missing assets/definitions,
definition chains and unused broken definitions, local+stock availability,
wrong asset types, instance-only exclusion, forward/private scopes, diamonds,
quoted literals, XML coordinates and stale-source rejection. The live SDK
regression also includes a grouped two-asset/two-definition failure and checks
that the previous package remains unchanged.
