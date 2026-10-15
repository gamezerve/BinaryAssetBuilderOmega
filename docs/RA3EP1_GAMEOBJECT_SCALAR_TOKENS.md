# Batched GameObject scalar-root token expansion

Follow-up: the [enum/flag/weak-ID batch](RA3EP1_GAMEOBJECT_ENUM_FLAG_WEAK.md)
extends root metadata to 78 fields and adds scoped native enum corrections.
The 63-field/124-corruption numbers below describe this earlier scalar checkpoint.

## Scope

`Ep1GameObjectScalarTokens` supplies 62 explicit read-only scalar descriptors for
root floats, signed/unsigned integers, Time, Angle, ushort, byte and SageBool.
`Ep1GameObjectDestructionProjection` now emits these fields plus Behaviors instead
of only BuildTime/Behaviors: 63 represented root fields including Behaviors.
This is a root metadata count, not asset coverage or SDK completion percentage.
The root table, container and module fragment offsets
are derived from the descriptor count; the former fixed 60-byte envelope is gone.
No stock prefix bytes or reflection-generated offsets enter the production writer.

This is still a **partial root projection**, not a supported GameObject asset
processor. String/list/pointer/reference/enum/bitflag and other omitted root fields
remain unrepresented. Native bytes for those fields are preserved, not validated
or secretly declared supported. The existing single-destruction envelope and
module optional/list limits remain in force. Registry, alpha and packaging gates
are unchanged; no manifest/BIG/gameplay acceptance is claimed.

## Independent checks

`GameObjectScalarTokenSmokeTest` compares every descriptor's field-name hash,
schema type tag and native offset against **both** AlliedConstructionYard and
SovietAntiInfantryInfantry stock roots. Emission does not merely reflect the
legacy native model and assume its fields are correct.

Two all-field authored source sets are validated against the explicit reviewed
`diagnostic-shield-first-v1` / `diagnostic-dummy-hooks-v1` schema candidate. This
is not unmodified official schema compilation. A required Body is included only
in validation context; it is not compiled into this deliberately partial native
projection. The real GameObject marshaler compiles the explicit root scalar
attributes and real polymorphic destruction behavior. Internal TypeId selection
is added only after raw XML validation, never accepted as authored schema syntax.

Independent expected literals check all generated native values, including:

- Float and Angle numeric storage.
- Time seconds and 1000ms → 1.0 second conversion.
- Int/uint values, packed ushort/byte fields and both bool states.
- Per-field generated token type/native binding and whole native/RELO/IMP
  round-trip integrity through the composed writer.

Unknown root attributes, invalid numeric/bool values and byte overflow fail
schema validation. Official SageUnsignedShort is a string regex that accepts
65536 lexically; native ushort conversion must reject that overflow instead.
The tests require failure across validation plus native compilation, without
weakening or modifying the official schema. Each emitted scalar's type and
target words also have independent detached output corruption controls.
There are 124 such type/target corruption refusals, in addition to the earlier
nine projection-envelope refusals and module/input/token controls.
These are focused source/native/token checks, not full
Core document preprocessing, Include/inherit/definition evaluation or runtime
proof for every field. Explicit value tests also do not establish parity of all
legacy marshaler defaults with schema-inserted defaults.

## Files and next gates

- `source/BinaryAssetBuilder.XmlCompiler/BinaryAssetBuilder/XmlCompiler/Ep1GameObjectScalarTokens.cs`
- `source/BinaryAssetBuilder.XmlCompiler/BinaryAssetBuilder/XmlCompiler/Ep1GameObjectDestructionProjection.cs`
- `source/BinaryAssetBuilder.ManifestInspector/GameObjectScalarTokenSmokeTest.cs`
- `source/BinaryAssetBuilder.ManifestInspector/GameObjectDestructionProjectionSmokeTest.cs`

Checks run inside the existing Allied/module and Core/destruction Focused groups,
not as dozens of inflated new test-group counters. Next gates are explicit
source/root admission and remaining enum/flag/reference/string/indirect families,
other module/container writers and a complete verified mixed GameObject stream.

## Final regression

`artifacts/GameObjectScalarRoot-a/batch-report.json`: **30/30 Focused groups pass**
in **80.569 seconds**. The batched scalar/native checks take **6.388 seconds**;
actual Core/destruction/projection checks **3.455 seconds**. The selected composed
BIN/RELO/IMP is **1900/24/8**, including the 1176-byte partial token prefix.
124 scalar type/target corruptions, the earlier nine envelope refusals and the
module/base/token controls all pass. The batch incremental build takes 1.16
seconds with zero warnings/errors. Eight runtime hashes remain unchanged during
workers; no game runs or installation files change. No new Full-suite, complete
GameObject stream, alpha capability expansion or gameplay proof is claimed.
