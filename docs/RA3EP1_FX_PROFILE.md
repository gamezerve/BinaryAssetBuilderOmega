# Isolated EP1 FXList compiler profile

## Scope

`Ra3Ep1FXListPlugin` now exposes a checked `ProcessInstance` entry for the
stock-proven empty/sound subset. It is activated only by an explicit experimental
descriptor/registry; no legacy KW or production EP1 type table is changed.
`diagnostic-build` still admits only its previous three families and excludes FX.
This profile is not a usable SDK release or a playable mod build.

| Metadata | Value |
|---|---|
| Profile | RA3EP1-FXList-Experimental-v1 |
| Root TypeId / TypeHash | 86682E78 / 17B3B82D |
| ProcessingHash | 17B3B82D XOR 45503141 |
| Observed EP1 aggregate | 5454A8E9 (not a complete registered type table) |
| Tokenized / custom data | false / false |
| Platform | Win32 only |
| Production output / build cache / compiled document reuse | all disabled |

An explicit descriptor asking for cache use cannot bypass the plugin policy.
Unsupported platform reinitialization first revokes readiness. Metadata returned
to callers is fresh, not shared mutable registration state. Production requests
fail before accessing missing source/cache or native output state.

## Admitted XML and dependency rules

- One current schema-bound FXList root with matching declaration id and type hash.
- Exactly one NuggetList, containing zero to two Sound nuggets.
- Optional source-required or source-excluded `FLYING` mask; at most one per Sound.
- INVALID weather and false/default booleans only; no cull tracking controls.
- No secondary masks, object filters, disabled lists, other nugget families,
  inheritance, custom data, formulas, nested controls or weak/file dependencies.
- Strong dependency preparation must have completed, including the empty case.
- Every normalized Sound selector must equal its ordered zero-based slot,
  with matching original reference identity and concrete resolved identity.
- Concrete audio targets are limited to AudioEvent or Multisound. Untyped/base
  references may select either; explicitly typed references may not widen to a
  sibling. Other schema descendants remain supported by the core resolver but
  are not admitted by this narrower FX profile.
- No unused, missing, duplicate-selector, null or extra dependency slots.

Injected TypeId attributes must match their schema types. The plugin validates
the current XML values in a detached copy after stripping injected attributes,
then restores schema-derived polymorphic IDs before native dispatch. Caller XML
and reference handles are not modified. This avoids trusting stale schema-validity
flags while retaining the recovered Sound native layout and default insertion.

## Evidence

`Ep1FXListProfileSmokeTest.Run` activates the descriptor, processes actual official
FX fixtures through the document pipeline, prepares dependencies and calls the
new compiler. It checks deterministic native equality, fresh reload despite
caller precompiled options, and platform/output/cache isolation. Negative cases
cover changed ids/type hashes/injected IDs, attributes/options/masks, malformed
or reordered selectors, changed concrete identities, null/extra slots, unsupported
schema-valid EvaEvent nuggets, nested filters, foreign namespaces and processing
instructions. Missing external targets revoke validated metadata; restoring them
recovers identical compiler output.

Additional literal AudioEvent names compile successfully. Rewriting the same
source path selects the new audio identity rather than resident stale state.
The tiny synthetic manifests prove identity selection only, not audio payload
or native audio type-hash compatibility.

With local Uprising global/static/English audio manifests supplied, the test
uses real external resolution and compares **actual plugin output** with selected
stock FX slices. Both complete ordered dependency `(TypeId, InstanceId)` tuples
and all BIN/RELO/IMP bytes match:

| Root | BIN / RELO / IMP | Concrete dependencies |
|---|---|---|
| FX_NONE | 28 / 0 / 0 | none |
| FX_DebrisHitGround | 80 / 12 / 8 | AudioEvent |
| FX_ALL_AntiGroundAircraft_VoiceDie | 252 / 24 / 12 | AudioEvent, Multisound |

Only manifest metadata and selected native slices are read; no full binary dump,
audio payload rebuild or WorldBuilder binary rewrite is performed.

```text
ep1-fx-profile-self-test [ep1-global-manifest ep1-static-manifest ep1-audio-manifest]
compiler-self-test
layout-self-test
```

The optional comparison requires a manifest containing all three selected FX
roots (normally static). Release/x86 builds, all 75 compiler groups, layout tests
and 33 enum mappings pass. Inventory remains 785/1,390 models and 762/1,390 typed
marshallers; adding an isolated processor does not increase those model counters.

## Next gate

The subsequent [fixed modifier/FX/audio stream proof](RA3EP1_MODIFIER_FX_STREAM.md)
now verifies concrete dependency tables, final one-biased selectors, runtime
mapping, native offsets, deterministic bytes and failure preservation. Next extend
bounded diagnostic command admission and its full input/publication tests. Full audio/particle processing, final EP1
registration, SDK/WorldBuilder packaging and actual Uprising loading remain open.
Overall engineering estimate remains approximately 50% complete / 50% remaining.

Related evidence: [native FX layout](RA3EP1_FX_NATIVE.md),
[core derived audio resolution](RA3EP1_FX_AUDIO_RESOLUTION.md).
