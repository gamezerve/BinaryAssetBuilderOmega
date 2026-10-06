# Source-backed Include literal definitions — October 6, 2026

## Real source result

The real global.xml graph still contains 396 documents / 664 Include edges.
The new profile validates 200 documents; 196 require preprocessing (194 inherited
documents plus the two PlayerTemplates documents whose GlobalDefines closure
contains nonliteral definitions). Raw/default behavior is unchanged:

| Status | Raw default | Local-only profile | Include literal profile |
| --- | ---: | ---: | ---: |
| Validated | 197 | 198 | 200 |
| RequiresPreprocessing | 194 | 198 | 196 |
| SchemaInvalid | 5 | 0 | 0 |

AudioSettings resolves six imported expressions; GameLOD resolves one. Eva still
resolves its 534 local references, for 541 substitutions across three documents.
The other two raw failures are explicitly blocked, **not fixed**. There were no
stale/read failures or limits, but the original six path issues and unresolved
typed AUDIO header still prevent completion. Exit is 2; output remains absent.

The PC `Includes/PlatformSpecificAudioDefines.xml` is explicitly reached through
the captured DATA tree, not selected by registry/platform search. Its seven
literal definitions originate from source SHA-256
`9EBE10D13B3C0A06C4DA5C2DCE31527A141BAD3A5C8A03C7FAD911E9A0DB4633`.
GameLOD additionally witnesses its GraphicsHardware Include, which contributes
no definitions. Processed AudioSettings SHA-256 is
`8F451B09073B8E64B5C34D2AE4837884ABB5284FDC607B40467F7288B69B0DBB`;
processed GameLOD SHA-256 is
`3F0FFCCD80950EADA95C8E0A1EA814DAD73A1195667299D9B57650B5BCC69B17`.
Raw hashes and individual definition origins remain available in JSON evidence.
These are serialized diagnostic XML identities, not native asset/stream hashes.

## Scope and commands

The explicit `diagnostic-include-literals-v1` profile admits child-first literal
definition visibility through captured source-backed `all` and `instance` Includes.
This is a diagnostic XML preprocessor, not the EA evaluator or a production SDK.
Original source and schema files are never modified.

```text
BinaryAssetBuilder.ManifestInspector.exe sdk-typed-source-graph ra3ep1 <absolute-schema-root> <absolute-source-root> <absolute-entry.xml> <absolute-new-output-directory> --include-defines [--art-root absolute-directory] [--audio-root absolute-directory] [absolute.manifest=runtime.manifest ...]
BinaryAssetBuilder.ManifestInspector.exe sdk-include-defines-self-test
```

The new flag is mutually exclusive with `--local-defines`; both flags are rejected
on the path-only command. The raw default and original local-only profile remain
separate. No registry, platform search path, manifest substitution or native
processor is invoked. Reference/precompiled Includes remain closed, even if the
named XML happens to be readable. External manifest mappings do not relax this.

## Implementation and evidence

`SdkIncludeDefineProfile` validates every inventory path's canonical confinement
before any imported source read. `Capture` admits only bounded bytes whose length
and SHA-256 match the existing path-audit source record. It never follows a newly
discovered target absent from that inventory. Captured XML is parsed with DTD and
external resolution disabled. Missing, changed or redirected sources reject the
whole expression transformation, without partial bytes or trusted origins.

`Apply/Visit` compares each raw direct Include, in document order, with its captured
edge's kind, logical path and physical target. The existing confined resolver must
derive that same target/root. Children are processed before local declarations.
A shared diamond leaf can contribute the same definition twice only when its
origin document and literal value match. Equal values from different documents
still collide. Local/imported name collisions are rejected; overrides are not
implemented. Core `DefinitionSet.AddDefintions` provides the source-origin rule;
`DocumentProcessor.ProcessIncludedDocuments` provides child-first visibility.

`SdkLocalDefineProfile.ReadLiteralDefinitions` shares the original literal rules.
Only exact case-sensitive ASCII `=$NAME` substitutions are supported. Any chained
or arithmetic definition blocks the entire needed closure, including expressions
that are not directly referenced by the owner. This conservative gate avoids
claiming equivalence to core's eager EvaluateDefinitions with the EA evaluator.
No arithmetic values are guessed from stock binary assets.

No-expression source documents retain their original bytes and make no definition
closure claim. They can still be raw-schema-valid while containing definitions
whose eventual evaluation is unsupported. Inheritance in the owner or a needed
definition-source document remains closed. Imported child assets are not compiled
or made visible as asset references merely by accepting their literal definitions.

Successful preprocessing evidence records separate raw/processed hashes,
substitution count, each definition-source path/hash, and each visible definition's
origin path. Rejection exposes no partial transformed bytes, processed hash,
substitution count, source witnesses or origins. Resource paths are resolved in
the consuming asset document's context, not the defining Include's directory;
the existing physical root/path guards remain mandatory after substitution.

Bounds: 512 captured sources, 4 MiB per source, 32 MiB cached definition snapshots,
32 Include depth, 4,096 Include edges and 512 visible definitions per owner.
Substitution retains the local profile's 2,048 slots, 128-node depth, bounded names/
values and 4 MiB processed XML. The definition cache is additional to the existing
32 MiB typed-source recheck bound. Cached rechecked bytes are snapshot evidence,
not an atomic filesystem transaction: later concurrent changes can invalidate
individual graph rows and never establish production readiness.

## Remaining gates

GlobalDefines contains arithmetic and string/alias expressions, so merely
supporting literal imported values cannot finish the PlayerTemplates files.
Next inspect the evaluator's supported grammar and definition typing/order,
then admit a separately bounded arithmetic/alias profile with stock-source and
negative fixtures. Asset inheritance must follow core override order and source/
type visibility rules; it is not equivalent to XSD type inheritance.

Missing AUDIO/ART inputs, final native type tables/processors and stream format
proof, WorldBuilder packaging and actual game loading remain release gates.
ProductionBuildReady and FullDependencyCoverage stay false. Weighted effort
remains approximately 50.25%, rounded to 50% complete / 50% remaining.

## Validation

Release/x86 build and owned fixtures validate origin-aware all/instance diamond
inclusion, separate defining/consuming directories, raw/processed hashes, stale/
missing sources, mismatched/missing edges, incomplete path graphs, cycles,
cross-document duplicate names, local collisions, overrides, chains, even unused
arithmetic, reference Includes and inherited definition sources. Forged duplicate/
escaped inventories and combined profile flags are rejected. Typed graph tests
prove source preservation, output absence and post-substitution resource confinement.
The earlier local-only regression still passes. No official source is modified.

All 119 registered compiler groups and 33 enum checks pass. The final Release/x86
build reports zero warnings/errors. Inventory counts remain unchanged:
785/1,390 models and 762/1,390 typed marshallers. More diagnostic
coverage is not proof of a working Uprising mod or reason to inflate the overall
effort estimate per commit.
