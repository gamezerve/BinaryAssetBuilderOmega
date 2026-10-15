# Shared geometry, movement and combat object projection

October 15, 2026. The selected partial GameObject now composes generated Geometry
metadata with root values/strings/imports, ArmorSet, optional LocomotorSet, three
behavior modules and ActiveBody/ranges. It can resolve a genuinely new Core-built
armor target. This is not complete object stream/publication or new-unit gameplay.

## One real native graph and reference table

The selected allocator detaches Body and optional Geometry before the root pass,
then allocates selected behaviors, EP1 ActiveBody and EP1 Geometry **on the same
Tracker**. Geometry follows Body in native order and official raw source order.
No native chunks or hand-written bytes are appended to the allocator output.
Body is found by its container name rather than assumed to be the last child.

The owner decoder admits the root +500 Geometry pointer only when it targets an
aligned, bounded final graph after Body/behaviors. The composer partitions the
native geometry tail before passing it to the strict standalone writer. Body is
no longer mistaken to own the entire tail. Nested relocations cannot cross owners.

The root Geometry token points at the generated own-field table, not a concrete
module wrapper or native pointer storage. Local geometry metadata/native targets
are joined into the common coordinate system; original native pointers and RELO
slots are rebased exactly once. Geometry contributes no strong asset imports.

Present Geometry adds one reviewed root field. With Body, ArmorSet and movement,
the partial inventory is **91 fields**; absent Geometry preserves the older output.
Other root/container fields remain unreviewed and prevent complete publication.

## Source and combination gates

The focused RootGeometry schema includes the **actual official Geometry XSD** and
extends the existing movement/combat closure. Raw adapted GameObject source must
also validate against the reviewed official candidate before actual Core named
reference normalization. Independent static/global catalogs require unique typed
targets; authored numeric dependency indices are refused.

`gameobject-geometry-composition-self-test <static> <global>` covers:

- Absent/simple/complex Geometry, with/without movement, all six behavior orders,
  absent/present long root strings: **72 actual-Core/native source combinations**.
- Simple Geometry has one BOX; complex Geometry has rotation, placement, three
  RING shapes with names/offsets/angle and two labeled contact points.
- Armor, two weapons with upgrade, destruction FX, Body/ranges, icon/intro and
  optional locomotor share one occurrence table. AssociatedModule resolves Body.
- One extra valid case reuses the destruction bone-prefix text as a geometry
  name, proving equal semantic text is not itself an invalid cross-owner alias.
- Every combination repeats and must preserve BIN/RELO/IMP bytes exactly.

The independent verifier checks all flat root targets, behavior wrappers/helper
base chains, selected destruction/hardpoint/weapon children, Body/ranges, armor
and movement members, all actual typed reference identities/one-biased indices,
and full native/RELO/IMP round trips. It separately extracts the **composed output**
geometry metadata/native portion into local coordinates and applies the standalone
independent geometry verifier, rather than just comparing a repeated write.

`gameobject-authored-geometry-composition-self-test <absolute-fresh-output> <static>
<global>` builds a genuine new RebornComposedArmor stream through Core and runs
the same 73 cases. Removing that compiled target must fail. Only the armor is
published by that existing bounded build: no consuming GameObject stream is built.

Fifteen/sixteen stock/authored input refusals include root/nested cross-owner
pointers, missing relocation ownership, invalid imports, geometry flags, orphan
tail, schema enum and missing named target; seventeen composed metadata controls
refuse. An explicit native string-pointer mutation into destruction storage is
refused, independently of the valid repeated-text source example.

An additional **696-byte native-only helper/geometry graph** exercises Geometry
without Body/armor/movement and the corresponding 88-field projection. It uses
actual native dispatch, not appended bytes. It is intentionally not described as
valid official source authoring: the official GameObject schema requires Body.

## Files and next work

- `Marshaler.Ep1Geometry.cs`: explicit shared-Tracker allocation entry point.
- `GameObjectBodyProjectionSmokeTest.cs`: named containers and allocation order.
- `Ep1GameObjectStringTokens.cs`: root Geometry/tail ownership.
- `Ep1GameObjectDestructionProjection.cs`: native splitting and geometry joining.
- `GameObjectGeometryCompositionSmokeTest.cs`, `RootGeometryPipeline.xsd`:
  actual-Core/stock/authored/corruption/native-boundary combination tests.
- Two additional Focused groups.

Draw/model/material closures, AI/other module containers, remaining root graphs,
complete source/local-ID/default semantics, general processor/registry/stream
publication and controlled consumer gameplay remain open. Alpha admission and
installed files are unchanged; no game is executed by these tests.

## Final regression

`artifacts/GameObjectGeometryComposition-a/batch-report.json`: **47/47 Focused
groups pass in 125.413 seconds**. Stock/authored geometry composition takes
4.158/4.487 seconds. Eight compiler/runtime hashes remain unchanged. The batch
build succeeds in 1.17 seconds with zero warnings/errors. GameExecuted and
InstalledFilesModified are false; no Full-suite, consumer stream or gameplay proof.
