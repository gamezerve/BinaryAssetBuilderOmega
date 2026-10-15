# EP1 VisionInfo native record and root composition

October 15, 2026. Uprising's VisionInfo has **ten fields / 40 bytes**. The legacy
36-byte model omitted ShroudClearingHeight at +8, shifting all subsequent fields.
A separate Ep1VisionInfo now serves the GameObject root pointer at +560; legacy
VisionInfo and its marshaller remain unchanged for unrelated consumers. The root
still occupies 600 bytes.

| Field | Native offset | Token type | Default native value |
| --- | ---: | --- | --- |
| VisionRange | 0 | 645864F3 | 0 |
| ShroudClearingRange | 4 | 645864F3 | 0 |
| ShroudClearingHeight | 8 | 645864F3 | 50 |
| VisionSide | 12 | 645864F3 | 1 |
| VisionRear | 16 | 645864F3 | 1 |
| VisionBonusTestRadius | 20 | 645864F3 | 100 |
| VisionBonusTestSegments | 24 | 763177A4 | 8 unsigned |
| VisionBonusPercentPerFoot | 28 | 645864F3 | 0 |
| MaxVisionBonusPercent | 32 | 645864F3 | 0.5 |
| MinVisionBonusPercent | 36 | 645864F3 | -0.5 |

The root token type is 649F97D1 and targets an own-field metadata table, not the
native pointer slot/pointee directly. The ten rows plus zero terminator occupy
132 bytes. No auxiliary relocation/import slots exist inside the 40-byte record;
only the enclosing root pointer is relocated.

## Independent stock and source evidence

`ep1-vision-info-self-test <static> <global>` recovers the complete metadata/native
graphs from AlliedConstructionYard, SovietAntiInfantryInfantry and JapanFortressShip.
Every field type/target is checked independently. Reconstructed semantic XML is
validated against the reviewed official schema candidate, marshalled through the
new overload, then compared against all 40 native bytes and all 132 normalized
metadata bytes. Native vision ranges differ (150/200/425), shroud ranges differ
(1000/500/500); all three selected stock heights are 50. Nondefault height edits
have authored evidence, not independent nondefault stock/gameplay proof.

The focused Core fixture's ten attribute types/defaults/use constraints must match
the reviewed official schema. Three authored/default profiles include the restored
height, fractional/signed reals, negative/over-100 percentages and uint.MaxValue
segments. Percentage parsing retains the current float32 multiplication by .01:
80 produces bits 3F4CCCCC rather than float literal .8's 3F4CCCCD. The test pins
this current conversion explicitly; broader EA decimal-rounding parity is not
inferred from the selected stock percentages.

An attempted extra stock example, ObservationPostTechStructure, was absent from
the supplied static catalog and is not counted as evidence. No asset was substituted
or silently skipped to manufacture another positive comparison.

## Shared graph and negative controls

**576 actual-Core combinations** cover present/absent vision, three source profiles,
all 16 presence masks of the other four pointed roots and all six orders of the
three selected behaviors. Armor/movement/body/geometry, occurrence identities,
pointer rebasing and every previous fragment check remain active. Repeated builds
must match all three buffers. The maximum selected root reaches **99 fields**,
not the full 126-field stock inventory.

Optional root own-field tables now receive disjoint variable-sized metadata extents,
so Flammability's 60 bytes and VisionInfo's 132 bytes coexist without aliases.
Vision owns a bounded, aligned 40-byte pre-behavior allocation; all nine real words
must be finite. The unsigned segment word is not interpreted as a float.

Controls reject 15 root-native faults, 22 detached metadata faults and four bad
source cases. Another 22 standalone controls cover NaN/infinity at every real
position, legacy/trailing record widths and forbidden auxiliary slots. The wrong
signed primitive tag is explicitly refused for the segment word.

## Files and boundaries

- `source/SageBinaryData/SageBinaryData/GameObject/Ep1VisionInfo.cs`
- `source/BinaryAssetBuilder.XmlCompiler/Marshaler.Ep1VisionInfo.cs`
- `source/BinaryAssetBuilder.XmlCompiler/BinaryAssetBuilder/XmlCompiler/Ep1VisionInfoTokenizer.cs`
- `source/BinaryAssetBuilder.ManifestInspector/Ep1VisionInfoSmokeTest.cs`
- `tests/fixtures/RootVisionPipeline.xsd`

Root model, ownership, metadata composition and existing combined verification are
updated at their existing seams. This is not a complete GameObject stream,
unrestricted source processor, new mod package, alpha expansion or gameplay proof.
No game or installed files are touched. Remaining root containers, Draw/AI/client
graphs and complete publication still block general GameObject authoring.

## Final regression

`artifacts/Ep1VisionInfo-a/batch-report.json`: **51/51 Focused groups pass in
378 seconds**, with the vision group taking 18.408 seconds. Eight runtime hashes
remain stable; GameExecuted, InstalledFilesModified and AlphaCapabilitiesExpanded
are false. The batch incremental build succeeds in 2.42 seconds with zero warnings
and errors. This also rechecks the corrected unsigned Flammability primitive tag
in both stock-target and new-armor-target pointer groups. No Full-suite or gameplay
acceptance run is claimed.
