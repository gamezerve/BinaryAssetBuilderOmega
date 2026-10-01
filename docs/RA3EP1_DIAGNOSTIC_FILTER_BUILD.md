# ObjectFilter integration into diagnostic builds

Follow-up: [narrow FX integration](RA3EP1_DIAGNOSTIC_FX_BUILD.md) adds a fourth
experimental family to the command. The original three-family filter proof
below retains its scope and native bytes.

## Outcome

`diagnostic-build` now admits the stock-proven ObjectFilterAsset subset alongside
ShaderOverride and AttributeModifier, including bounded all/instance graphs.
It explicitly maps Ra3Ep1ObjectFilterPlugin with observed EP1 TypeId `44A5973D`,
TypeHash `DF72B4BA`, Tokenized=0. Production output, cache and document reuse
policies remain closed. This is not a playable Uprising mod or complete SDK.

The command uses a new DiagnosticAssetPipeline.xsd combining official schema
components for the three families. Earlier two-family proof schemas are unchanged;
no official XML/XSD reference files or game binaries were modified.

## Usage and native evidence

Run from the repository root after a Release/x86 inspector build. The parent
Release directory must exist; choose a new output child that does not exist.

```powershell
# Reborn: emit only a verified three-family diagnostic directory, never a playable game mod.
.\source\BinaryAssetBuilder.ManifestInspector\bin\x86\Release\net8.0\BinaryAssetBuilder.ManifestInspector.exe diagnostic-build tests/fixtures/DiagnosticFilterProbe.xml Release/MyFilterPoC
```

The parent includes DiagnosticFilterChildren.xml with `all` semantics. Its native
entries are ordered shader (40/12/0), weak filter (132/8/0), modifier (60/8/8).
Totals are 232/28/8 bytes; linked BIN/RELO/IMP files include eight-byte headers
and therefore occupy 240/36/16 bytes. The manifest contains exactly one strong
shader reference (eight bytes). The shader/filter source is `input-0001.xml`,
the parent modifier source is `source.xml`.

The actual command built that example locally to Git-ignored
`Release/DiagnosticFilterPoC-20261001/diagnostic.manifest`.

DiagnosticFilterBuildSmokeTest also builds all eleven source-derived infiltration
filters from ObjectFilterProbe.xml and compares every emitted BIN/RELO slice to
ObjectFilterNativeSmokeTest.Compile using the older standalone official schema.
All eleven have zero strong reference entries and zero IMP data. Their observed
hashes, tokenization and source identities agree through the command's two-reader
verification. The existing stock comparison was rerun against
`D:\TEMP\Red Alert 3 Uprising Source Data\Static Data\data\static.manifest`:
all eleven match real game slices exactly. Only selected native ranges were read;
no full BIN dump was taken. See [native layout/source evidence](RA3EP1_OBJECT_FILTER_NATIVE.md).

## Weak references are not strong imports

IncludeThing values become ordered inline four-byte GameObject instance hashes.
The filter root's count/pointer at offsets 108/112 identifies payload starting
at 124. The example carries AlliedInfiltrationInfantry and JapanInfiltrationInfantry
in source order; only the list pointer needs a RELO entry. There is no import
selector, strong manifest reference or forced GameObject compilation for these names.

The modifier still has its independent strong ShaderOverride reference. Its
one-biased selector at modifier-relative offset 56 remains one, selecting its
single validated dependency. Adding a filter does not alter that table.

A weak name need not resolve to an admitted local declaration or mapped manifest.
An explicitly synthetic unknown name compiles to its current hash, and changing
that name refreshes native data. This tests serialization only: it does not prove
the named object exists, that any filter matches objects in Uprising, or that
weak metadata can replace strong dependency validation. Adding a GameObject
metadata mapping does not promote these names into strong references.

The official Base/AssetBase.xsd WeakReference pattern admits unqualified ASCII
names, not `GameObject:name` syntax. The new integration explicitly tests that
prefixed names reject; the shared core's ability to normalize typed weak names
under other schemas does not override this official source constraint.

## Eligibility and rejection

The existing profile, not a broad legacy fallback, controls selected filters:

- Exactly one inline Filter, NONE rule/alignment.
- Empty or ENEMIES relationship; empty/INFANTRY/AIRCRAFT/SHIP/VEHICLE kind mask.
- At most fifteen IncludeThing leaves, checked against ordered normalized weak metadata.
- No ExcludeThing, excluded/status masks, broader rule/relationship/alignment,
  inheritance, expressions, strong/file dependencies or custom processing.
- Current schema values and injected TypeIds must validate; poisoned metadata rejects.

The graph preflight admits only matching Filter/IncludeThing nesting. The profile
validates native eligibility for selected roots. An unused instance-Include filter
remains tentative and is not compiled merely because its XML is present. Official
schema validation still applies to included XML; this is not a promise to validate
native profile eligibility for every unselected tentative root.

Tests cover deterministic repeat output, mixed included source attribution, fresh
weak edits, absent weak targets, tentative exclusion, official-invalid prefixed
names, ANY/ALLIES/status/exclude controls, excess leaves, nested controls and
poisoned TypeId rejection. Rejected builds do not publish the requested output or
retain known staging files, and caller settings are restored.

## Files, validation and remaining gates

- DiagnosticSourceGraph.Visit: bounded matching filter/leaf shape admission.
- BoundedDiagnosticBuild.Build: explicit profile mapping, real local resolution
  closure and shader/filter/modifier ordering; Serialize/Verify: native readback.
- Ra3Ep1ObjectFilterPlugin.ProcessInstance: existing native eligibility and weak
  identity/order validation; unchanged by this integration.
- DiagnosticFilterBuildSmokeTest.Run: eleven golden and three-family command proofs.
- tests/fixtures/DiagnosticAssetPipeline.xsd and DiagnosticFilterProbe.xml /
  DiagnosticFilterChildren.xml: restricted declaration universe and example graph.

All 72 compiler groups, layout tests, 33 enum mappings and both Release/x86 builds
pass. Inventory remains 785/1,390 models and 762/1,390 typed marshallers; overall
effort is still approximately 50% complete / 50% remaining.

The next native dependency gate is FXList: existing external FX evidence is only
manifest identity metadata, not a compiled/packaged local FX processor. Full EP1
type table/aggregate derivation, wider filters, cache/inheritance, production
stream lifecycle, SDK/WorldBuilder packaging and actual Uprising loading remain
unproven. See [command admission and publication guards](RA3EP1_BOUNDED_DIAGNOSTIC_BUILD.md).
