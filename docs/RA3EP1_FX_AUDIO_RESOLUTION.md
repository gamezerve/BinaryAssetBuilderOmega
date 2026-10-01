# EP1 FX concrete external audio resolution

## Completed gate

Official `AudioEventInfoRef` targets `BaseAudioEventInfo` (`4053C714`),
whereas the selected stock FX assets depend on concrete `AudioEvent`
(`844D7B9F`) and `Multisound` (`A3A7AF37`) identities. Previously,
`AssetDeclarationDocument.AddOutputInstance` searched only the exact base
type in external manifests. Local declaration lookup already searched schema
descendants; external metadata lookup did not. Replacing source XML/XSD alone
could therefore still produce missing or incorrect strong dependency metadata.

`ResolveManifestReference` now performs the following under one external-cache
lock and one validated cache generation:

1. Preserve exact type/instance identity precedence.
2. Otherwise search transitive descendants from `SchemaSet.GetDerivedTypes`.
3. Deduplicate identical type/instance pairs across manifests.
4. Accept one concrete candidate; reject multiple identities with
   `ReferencingError`; retain the existing missing-reference policy for zero.

Local declaration lookup keeps its existing precedence. External matches do
not enqueue native compilation of stock audio. Only the ordered
`ValidatedReferencedInstances` table receives the concrete handle; original
reference handles and normalized `name\index` XML remain unchanged. Native
one-biased selectors still select the same dependency-table positions.

`DocumentProcessor.Version` advances to 21 (VERSION5: 20) so old
document/intermediate identities cannot bypass the changed dependency semantics.
Production output, experimental output policies and cache restrictions are
not relaxed.

## Evidence and regression tests

`FXListPipeline.xsd` includes the unchanged official `AssetTypeAudio.xsd` for
real inheritance, without adding an audio compiler or audio root declarations
to its entry grammar. `FXAudioResolutionSmokeTest.Run` processes the official
three-root FX fixture through the core and invokes actual dependency preparation.
Synthetic external manifests prove identity selection only; their placeholder
hashes are not native audio compatibility evidence.

Tests cover missing targets and repeated failure, unrelated same-name types,
AudioEvent/Multisound ambiguity and recovery, exact-base precedence, duplicate
identities across manifests, transitive `AudioEventOverridable` lookup,
explicitly typed sibling rejection, removed mappings, and same-path manifest
identity replacement with timestamp refresh. Failed attempts must clear partial
dependency/visited state. Successful attempts preserve names/order, retain no
local dependency closure and produce identical native BIN/RELO/IMP chunks before
and after resolution.

The optional stock-manifest pass was executed against local Uprising
`global.manifest`, `static.manifest` and English `audio.manifest`. Actual core
resolution selected:

| Stock FX root | Ordered concrete dependency types |
|---|---|
| FX_DebrisHitGround | AudioEvent |
| FX_ALL_AntiGroundAircraft_VoiceDie | AudioEvent, Multisound |

This pass reads manifest metadata, not audio payloads or WorldBuilder BIN.
The separate native FX golden pass checks only selected stock slices, as
documented in [RA3EP1_FX_NATIVE.md](RA3EP1_FX_NATIVE.md).

```text
fx-audio-resolution-self-test [ep1-manifest ...]
fx-native-self-test [ep1-static-manifest ...]
compiler-self-test
layout-self-test
```

## Remaining gates

Verification completed: both Release/x86 projects build; all 74 compiler groups,
layout tests and 33 enum mappings pass. Stock native FX comparisons remain exact
at 28/0/0, 80/12/8 and 252/24/12 BIN/RELO/IMP bytes. The stock concrete audio
metadata pass also succeeds after the final document-version bump. Inventory
remains 785/1,390 models and 762/1,390 typed marshallers.

FXList remains unregistered and excluded from `diagnostic-build`. This is a
dependency-resolution milestone, not a runnable FX/audio SDK build. Next add
a narrowly admitted isolated FX processor: revalidate current declarations,
concrete dependency identities, selector bounds/order and injected type IDs;
reject unproven nugget variants and options. Then prove mixed-family diagnostic
stream round trips before extending command admission.

Audio payload compilation/packaging, other FX nugget layouts, final EP1 type
table/aggregate identity, SDK/WorldBuilder integration and actual Uprising mod
loading remain separate gates. Overall estimated effort remains approximately
50% complete / 50% remaining; this focused gate does not justify increasing the
rounded project estimate.
