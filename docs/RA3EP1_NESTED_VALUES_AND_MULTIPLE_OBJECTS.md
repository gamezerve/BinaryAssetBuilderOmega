# Nested GameObject values and multiple emitted objects

## Enabled authoring

<!-- Reborn: native implementation and official schema jointly define editable values, not stock example literals. -->
`GameObjectNestedValueContract` binds plain-record attributes to concrete EP1
native fields and the existing scalar codecs. The following owners no longer
require stock scalar values:

- `ObjectResourceInfo/BuildCost`: implemented money-transaction attributes.
- `VisionInfo`: implemented EP1 attributes, including ShroudClearingHeight,
  side/rear percentages, bonus radius/segments and bonus percentages.
- `LocomotorSet`: implemented scalar/enum attributes, alongside existing typed
  target-reference handling.
- `Geometry`: implemented fixed attributes such as IsSmall.
- `Geometry/Shape` and existing `PlacementShape`: implemented fixed attributes,
  including dimensions, angles and packed flags.

This is a schema/native descriptor intersection, **not** permission to serialize
every attribute or child accepted by the XSD. Strings, pointers and lists do not
become scalar capabilities. Missing scalar attributes use their official
defaults/native zero. Existing element allocation and unimplemented list cardinality
remain separately constrained; LocomotorSet now has a source-sized list contract.
EP1 VisionInfo inserts ShroudClearingHeight at +8;
the legacy KW VisionInfo layout must not be used for this object.

Supported fixed and variable sources now use the direct complete-graph build
route, also used by mixed-asset builds. Normal builds prove their actual source
and emitted graph rather than replaying the entire archived native research suite.
The legacy fixed snapshot route remains a compatibility fallback for an existing
fixed shape not admitted by the direct route. No extra user switch or alternative
UI is needed.
Every emitted value is independently encoded from source and checked at its
live native owner address. Unrelated bytes, padding, imports, relocations and
metadata retain their existing complete ownership checks.

## Multiple GameObjects

<!-- Reborn: the primary object anchors legacy receipts only; every explicit object receives its own admission and native proof. -->
An AssetDeclaration/all-include graph may now emit multiple GameObjects.
Each object is separately admitted and compiled with its own module-template
mapping. All emitted objects and explicitly declared ArmorTemplate,
WeaponTemplate, LogicCommand and LogicCommandSet assets share mod-first typed
identity lookup and dependency-sorted stream serialization. Duplicate identities,
missing strong targets and local strong dependency cycles still fail.

The first emitted object is only the primary receipt/package anchor. It does
not define other objects' values or create their commands. To recruit a new ID,
author its LogicCommand and add that command to the appropriate command set.

Multiple objects are still limited to **implemented GameObject processor
coverage**. This does not enable arbitrary new behavior families, new model or
texture compilation, or every possible EA GameObject topology.

## Offline verification

Developer-only focused native proof cases:

- `conscript-module-graph-self-test <static> <global> <fresh-output> nested-values`
- `conscript-module-graph-self-test <static> <global> <fresh-output> nested-defaults`

Both compile the complete object, generate/read back the stream and package,
check deterministic output and reject deliberate native/metadata corruption.
`Test-Ra3Ep1DevSdk.ps1 -MultipleObjectsOnly` exercises three explicit GameObjects,
three explicit recruitment commands, shared local armor/weapons and one command
set in one eleven-asset build. It restores the previous RebornTest source and
never builds the user's Reborn or publishes automated output into Documents.

<!-- Reborn: movement list coverage is independent of single-value receipts and the original 32-row diagnostic quota. -->
The same integration test now uses one, zero and forty LocomotorSet rows across
the three objects. The focused native `movement` case group separately checks
expanded, omitted and default-only movement lists, including late-row corruption.
See [movement list details](RA3EP1_LOCOMOTOR_LISTS.md).

## Remaining structural work

- Optional absence/addition of whole nonmodule records.
- Source-sized geometry and other not-yet-contracted lists (BuildCost now has
  its own [optional resource/transaction contract](RA3EP1_RESOURCE_INFO.md)).
- Arbitrary nested record/string/reference topology beyond implemented owners.
- General W3X, DDS/TGA and audio processors.

Existing source-sized ArmorSet, covered weapon hardpoint/Weapon lists, upgrade
lists and behavior composition remain available; they are not newly restricted.
