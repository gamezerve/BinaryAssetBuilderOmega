# EP1 geometry native port

October 15, 2026. The GameObject root now points to a separate EP1 Geometry model.
The legacy KW Geometry/Shape/GeometryType remain unchanged for other consumers.
This is native compatibility evidence, not complete GameObject publication.

## Recovered contract and correction

| Native field | EP1 offset |
| --- | ---: |
| RotationAnchorOffset pointer | 0 |
| PlacementShape pointer | 4 |
| Shape count/items | 8 / 12 |
| ContactPoint count/items | 16 / 20 |
| IsSmall | 24 |

The header is **28 bytes**, not the old 24. PlacementShape was missing in the
legacy model, shifting subsequent fields. GameObject's own Geometry pointer stays
at +500 and its native root stays 600 bytes. PlacementShape is an optional full
48-byte shape, not a member of the collision Shape list.

Each EP1 Shape remains 48 bytes: Type +0, ContactPointGeneration +4, Name string
header +8, MajorRadius +16, MinorRadius +20, Other string header +24, Height +32,
FrontAngle +36, Offset pointer +40, Active/UsedForHealthBox +44/+45 and padding.
The scoped enum adds RING=3. A ContactPoint is twenty bytes: Label string header,
then an inline twelve-byte Pos vector. Rotation/Offset metadata points directly
to native coordinates; it is not an additional x/y/z token table.

## Independent stock and source evidence

`ep1-geometry-native-self-test <static>` reads only bounded selected asset slices:

- AlliedConstructionYard: four collision shapes and offset/name variations.
- SovietAntiInfantryInfantry: one shape and IsSmall=true.
- JapanFortressShip: placement shape plus eight collision shapes.
- AlliedPowerPlantTurbine: contact-point list and native inline positions.
- NanoSwarmHiveEffect: the EP1 RING literal.

Independent stock field/list metadata reconstructs a semantic Geometry source
projection. The reviewed official candidate validates that projection; actual
marshaller output is compared to **every native header/shape/contact byte** and
all referenced string/vector contents. Comparison accounts for different base
addresses and stock string interning. This is not original-source preprocessing,
whole-object equality or copied native payload publication. Stock FrontAngle is
zero in this selected set; authored tests separately cover nonzero angle units.

Twenty-four authored cases combine 1/3/32 shapes, 0/2 contacts, optional rotation
and placement, all four shape types and all five contact-generation types, strings,
offsets, booleans, explicit values and a 90-degree angle. A separate omitted-field
case checks the official scalar/boolean defaults. Six malformed schema examples
refuse. Enum inventories are checked against the reviewed official schema.

Each source also passes through actual GameObject root dispatch. After normalizing
the root/standalone origin, native bytes and all relocation slots must match the
standalone graph. An independent slot collector checks the exact expected geometry
relocations and proves there are no strong asset imports.

## Boundaries and files

The selected root composer still refuses the unreviewed Geometry owner rather than
silently emitting an incomplete object. Generated geometry metadata, joining that
fragment with Body/behaviors, broader graph limits/diagnostics, recursive object
publication and gameplay remain separate gates. The alpha is not expanded.

- `GameObject/Ep1Geometry.cs`: scoped 28/48-byte models and four-value enum.
- `Marshaler.Ep1Geometry.cs`: defaults, placement/list/vector/string allocation.
- `GameObject.cs`: EP1 geometry pointer at the unchanged root offset.
- `Ep1GeometryNativeSmokeTest.cs`: five stock graphs, source/default/refusal and
  exact native/root/relocation proof; one new Focused group.

No game is launched and no installed files are modified.

## Final regression

`artifacts/Ep1GeometryNative-a/batch-report.json`: **44/44 Focused groups pass in
117.804 seconds**, with eight unchanged runtime hashes. The geometry group takes
3.389 seconds. The batch build succeeds in 1.17 seconds with zero warnings/errors;
GameExecuted and InstalledFilesModified are false. No Full-suite/gameplay proof.
