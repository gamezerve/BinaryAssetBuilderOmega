# GameObject schema/native primitive capability

The Conscript frontend no longer uses the old six-field authoring whitelist or
its invented gameplay ranges. This is the first capability-based expansion,
**not** a claim that arbitrary GameObjects or module graphs compile now.

## Admission contract

`GameObjectScalarContract` intersects official effective GameObject schema
attributes with the already stock-reviewed `Ep1GameObjectScalarTokens.Fields`
processor ABI descriptors and concrete `SageBinaryData.GameObject` field types.
The registry currently supplies **63 root attributes**. That count describes
implemented processor coverage, not a new configurable maximum or trial-specific
authoring list. New processor descriptors acquire the same generic checks.

`GameObjectRootAttributeContract` adds the official-schema intersection for
**15 fixed-size root attributes**: both 291-bit KindOf masks, six EP1 enum words
and seven weak asset IDs. Weak IDs use the actual invariant engine name hash;
they do not create strong imports or prove that a target payload exists.
`GameObjectStrongReferenceContract` additionally permits one-for-one retargeting
of root strong-reference slots already populated by the admitted Conscript graph.
The target must resolve in the current stock catalogs and its captured target
type, occurrence position, import count and order must remain unchanged.
`GameObjectRootStringContract` enables content changes in existing root ASCII
string allocations. The authored value must have the same ASCII byte length as
the admitted Conscript value; this keeps every downstream native pointer and
relocation source stable while checking the exact payload and terminator.

Supported codecs are finite single-precision real, signed/unsigned 32-bit integer,
unsigned 16-bit and 8-bit integer, SageBool, Angle, Time and Duration. Official
schema lexical/range/default rules apply. Time/Angle unit conversion and Duration
logic-frame conversion are independently checked against actual compiled bytes.
XML boolean `0`/`1` forms are canonicalized for the legacy marshaler. Missing
optional root scalars use schema defaults without materializing every optional
field into the submitted source. NaN/INF/overflow/unknown attributes fail.

Examples of newly editable root attributes include Scale, ThreatLevel,
BountyValue, EnergyProduction, VoicePriority, IsTrainable, IsForbidden,
TransportSlotCount, PlacementViewAngle and JustBuiltTime. Definitions and
attribute-only instance inheritance can supply them. The existing nested
Amount/MaxHealth/VisionRange/ShroudClearingRange/Speed owners remain supported;
their former practical alpha caps are also removed. The schema/native type still
limits representability. Schema-valid does not mean good gameplay tuning.

## Native and publication proof

The compiler compares each root value at its exact ABI offset **and width**,
including adjacent packed booleans/byte/ushort fields. It then restores all
supported scalar locations in an owned copy and reuses the complete stock graph
oracle: the remainder, allocation graph, relocations, imports and dependency order
must remain exact. Expected metadata patches the same typed value locations;
the independent metadata producer and serializer must agree. Serialization
checks every root primitive again. Repeat determinism alone is not the proof.

The grouped test authors every primitive and fixed-root codec in one complete
source, retargets the existing root imports to different real stock assets,
changes every populated root string without changing its allocation extent,
compiles it, checks native/metadata/dependency results, and corrupts each fixed
storage descriptor to prove refusal. It also checks boolean spelling, omitted
default BuildTime, BuildTime=0, Scale=2 and VisionRange=6000, without turning
those into new gameplay claims.
Legacy golden fixtures still must reproduce their exact earlier archive.

## Remaining capability boundaries

The complete Conscript shape, selected module graph and 275 strong-reference
occurrences remain the supported topology. Root enums/KindOf masks and weak IDs
are now editable. Already-present root strong references may retarget stock
assets one-for-one. Existing root ASCII strings may change at fixed byte length.
Resizing/adding/removing string allocations, adding/removing imports, module/list
composition, resource bodies and arbitrary nested fields still need their own
typed processor/ownership/dependency contracts, not removal of safety checks.
The six-field **business whitelist** is gone; full schema-driven GameObject
coverage is still in progress.

RebornTest now demonstrates Scale=1.2 and ThreatLevel=15 in Conscript.xml while
retaining the explicit construction command and instance/definition layout.
Use only EALAModStudio.exe. No game is automatically launched by this work.

## Verification — October 18, 2026

- Compiler: zero warnings/errors on the final incremental build.
- Grouped native/metadata probe passed all 63 descriptors using distinct same-type
  numeric sentinels and inversion of every original/default boolean. All 63
  individual native-slot corruption controls were rejected.
- A second grouped probe passed 15 fixed enum/KindOf/weak-ID descriptors and
  one-for-one stock retargeting of the two populated Conscript root strong slots.
  The same probe changed all three populated root string allocations at fixed
  byte length and verified native plus rebased metadata payloads.
  The complete authored-trial suite rejected 127 source/native corruption cases,
  including string payload, string resize and strong-import presence controls.
- Boolean `1`, omitted default BuildTime, BuildTime=0, Scale=2 and VisionRange=6000
  passed source/native verification; invalid real, boolean and overflow controls
  refused. These are offline tests, not new game-load observations.
- Alpha regression: **21/21**, including unchanged original BuildTime=1 golden
  BIG, failure preservation and user Reborn isolation.
- Source-search/inheritance-expression and exact-reference Studio loader tests
  passed. Final Scale=1.2/ThreatLevel=15 RebornTest native/package build passed.
- Generated evidence remains in local `alpha/.work/scalar-*.log` and
  `dev-sdk-test.json`; original EA source and user Reborn content were not edited.
