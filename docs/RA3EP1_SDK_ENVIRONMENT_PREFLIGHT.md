# Explicit target-aware SDK environment preflight

October 5, 2026. This is a **read-only planning gate**, not an SDK build launcher.
It validates explicit target/schema/source/output paths and optional external
manifest metadata without executing reference RA3 batch files, mutating registry,
clearing caches, loading codecs or creating outputs. Weighted overall effort stays
approximately 50% complete / 50% remaining. Production SDK/game loading remains open.

## Why this boundary is needed

The reference `Working RA3 Compiler for Reference/tools/BinaryAssetBuilder.exe.config`
uses relative `..\Schemas\xsd\CnC3Types.xsd`, `dataRoot=..`, Art/Audio and
SAGEXML/Mods lookup paths. Its legacy default `#all` XML plugin and audio/texture/W3X/
tokenizer processors are not an audited EP1 production configuration. Its audio list
also contains SDK-specific MP3/music additions; they are not automatically EP1 types.
`BuildMapV2.bat` reads `UserDataLeafName` from two RA3 registry locations at lines
136–137. None of these files are modified or executed by this gate. Merely replacing
schemas or copying that registry lookup would not establish Uprising compatibility.

## Commands and contract

The inspector command is:

`sdk-preflight ra3ep1 <schema-root> <source-root> <source-entry.xml> <new-output-directory> [absolute.manifest=relative-runtime.manifest ...]`

`scripts/Test-Ra3Ep1SdkEnvironment.ps1` wraps that command with mandatory Target,
SchemaRoot, SourceRoot, SourceEntry and OutputDirectory parameters. ExternalMappings
is optional; `-AsJson` returns JSON rather than a PowerShell object. InspectorPath
defaults to the existing checkout's Release/x86 apphost. The wrapper does not restore
or build it. No shell expression evaluation or environment/registry substitute is used.

All selected physical paths must be fully qualified literals. Drive-relative
`D:Temp`, current-directory-relative and wildcard paths reject. Existing reparse
ancestors/children reject.
Windows device namespaces/reserved device components and alternate data streams
reject by syntax before an OS read, even if a path appears to end in .xml.
SourceEntry must be a segment-bounded child of SourceRoot, an XML file, and a
DTD-free EA AssetDeclaration. This is **entry syntax only**, not
schema validation or include graph resolution. Output must be a new child beneath
an existing parent, outside both source/schema trees; it remains absent on success
and failure. Existing outputs are never touched.

The schema catalog must exactly match this checkout's staged `schemas/ra3ep1/xsd`
catalog by relative XSD filename and SHA-256. Even annotation/format edits reject;
this is a deliberate byte-level planning fingerprint, not semantic schema equality.
It is not an immutable release signature or proof of an official/final type table.
An external schema clone with identical bytes has the same catalog fingerprint.
RA3/custom schema trees are not silently substituted.

Up to eight physical/runtime manifest pairs are allowed. Physical manifests and
game-visible runtime names must be independently unique; runtime names normalize
slashes/case and reject traversal, absolute paths, macros, selectors and wrong
extensions. Linked, internally valid v7 / `AllTypesHash=0x5454A8E9` metadata without
patch bases is required. Referenced runtime availability and adjacent BIN/RELO/IMP/
custom data are not validated. That constant is an expected game fingerprint, not
a computed compiler readiness result.

JSON carries `ReadOnly=true`, `SnapshotOnly=true`, `SchemaCatalogMatches=true`,
`IncludedSourcesValidated=false`, and `ProductionBuildReady=false`. The wrapper
checks the read-only/nonproduction flags. No generated configuration is applied.
Repeat preflight before a later build; evidence is not a transactional lock.

## Implementation paths/functions

- `source/BinaryAssetBuilder.ManifestInspector/SdkEnvironmentPreflight.cs`:
  `Inspect` validates paths, entry/mappings and schema parity; `Catalog` visits checked
  directories before descent; `Digest` hashes ordered length-delimited names/hashes;
  `Read`, `CheckPath`, `RuntimeName` implement bounded reads and attribution.
- `SdkEnvironmentPreflightSmokeTest.Run`: one default managed compiler group plus
  the explicit `sdk-preflight-self-test` command.
- `Program.Main` and `scripts/Test-Ra3Ep1SdkEnvironment.ps1`: JSON and PowerShell
  interfaces, distinct from `diagnostic-build` or a production builder.

Bounds: 1,024 XSDs, 128 directories, depth 8, 512-character relative schema names,
2 MiB per XSD and 64 MiB total schema bytes. Source entry is at most 4 MiB, and
external manifest stored/decompressed metadata is at most 16 MiB each. RefPack
decompression is capped before passing bytes to the shared reader. No full binary
stream or recursive 528 MB source-tree dump is performed. Path checks are not a
security sandbox or atomic adversarial-filesystem guarantee.

## Executed evidence

Release/x86 build, all **111 default compiler test groups** and all 33 enum checks
passed. Structural inventory stays 785/1,390 models and 762/1,390 typed marshallers.
The standalone preflight regression and both PowerShell object/JSON paths passed.
One measured wrapper call completed in about 608 ms; this is a local observation,
not a performance guarantee. Catalog reads avoid rescanning all ancestors twice
per XSD and malformed path/source/mapping inputs reject before catalog hashing.

Tests cover exact schema clones/stable hashes, modified/missing/extra XSDs, wrong
target/case/schema, relative and sibling-prefix paths, source/output overlap,
missing parents, DTD/entity attempts and oversized source entries. Mapping tests
cover duplicates, wrong targets, patch roles, path/macros/traversal and a tiny
RefPack header declaring an oversized output. Source bytes, Settings.Current and
requested output absence remain unchanged through accepted/rejected inspection.
Tests write only fresh owned fixtures, not the reference schema/source tree.

A real preflight passed using the user's external Uprising source root:

`D:\OneDrive\CNC Files\CnC_Modding_Support-main (Official XML, Schema, Script, Shader, Maps)\Uprising\Xml (Uprising)`

Entry `global.xml` (5,147 bytes) SHA-256:
`DEB5B6595A52C5261C42279D92A9510831A74FF1C0000C0A2B5A00C7CB39E3FE`.
The staged 843-XSD / 1,542,159-byte catalog fingerprint is:
`63EE69B6B00BF60A2F09A28509BE7CBFCB2F935627782DA7B46DE53308CE6F5A`.

Real external inputs from `D:\TEMP\Red Alert 3 Uprising Source Data`:

| Physical metadata suffix | Runtime name | Assets | Stored SHA-256 |
| --- | --- | ---: | --- |
| Global Data/data/global.manifest | global.manifest | 11,357 | 08A415789062B1707EDBA3C456884B94791097505431E17A03D10EE34DF050FD |
| Static Data/data/static.manifest | static.manifest | 13,872 | 74FACD9056EEE20942BB274A924BEFFB03D7ADBB71B9609E804A39619B2C6081 |

No output directory was created. The full source tree and worldbuilder.bin were
not read; prior targeted WorldBuilder binary evidence remains a separate milestone.

## Next steps

Add bounded source Include/art/audio path attribution under explicit roots, keeping
physical dependency paths separate from game-visible manifest names. Then connect
only admitted diagnostic families to a target-aware build plan with fresh source/
schema/dependency snapshots and no registry fallback. Full official-source builds
remain blocked by unported processors/type hashes; do not launch legacy `#all`
production compilation merely because this path/metadata preflight passes.
WorldBuilder data packaging and an actual minimal Uprising mod load remain required.
