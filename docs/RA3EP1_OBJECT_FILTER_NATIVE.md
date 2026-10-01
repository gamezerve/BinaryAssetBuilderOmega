# EP1 ObjectFilterAsset native proof

## Outcome

Eleven source-derived infiltration filters match stock EP1 BIN/RELO/IMP exactly.
No native model change was necessary in this block: the existing inline layout,
previously migrated masks and invariant weak-ID marshalling already match this
subset. Actual document schema/default/weak-reference normalization produces the
same native buffers. This establishes a third focused asset-family proof after
ArmorTemplate and AttributeModifier, not a complete third production processor.

Observed EP1 identity: TypeId `44A5973D`, TypeHash `DF72B4BA`, Tokenized=0.
The commented legacy KW table entry contains a different hash (`25970AF7`) and
must not be uncommented as an EP1 port. ObjectFilterAsset remains unregistered;
the legacy aggregate/type registry and all experimental production gates are unchanged.

| Infiltration filter name suffix | BIN/RELO/IMP |
|---|---|
| AircraftObjectFilter | 124/0/0 |
| AircraftStructuresFilter | 132/8/0 |
| CanEnterObjectFilter | 132/8/0 |
| EnergyStructuresFilter | 140/8/0 |
| InfantryObjectFilter | 124/0/0 |
| InfantryStructuresFilter | 136/8/0 |
| ParalyzeObjectFilter | 184/8/0 |
| ShipObjectFilter | 124/0/0 |
| ShipStructuresFilter | 136/8/0 |
| VehicleObjectFilter | 124/0/0 |
| VehicleStructuresFilter | 136/8/0 |

The table names follow the prefix `ObjectFilterAsset:Infiltration`.

## Layout and semantics

`ObjectFilterAsset` has a four-byte BaseAssetType slot followed by an inline
120-byte ObjectFilter. Thus the root is 124 bytes, not a pointer to a filter.
The filter retains a four-byte typed ID, rule/relationship/alignment, two 40-byte
EP1 KindOf masks, optional status-mask pointers, and two count/pointer weak lists.
IncludeThing list count is at root offset 108 and its pointer at 112; the first
list payload begins at 124. Each weak name contributes four bytes, not an import.

CanEnter has ENEMIES bit 2 and two weak IDs in source order. Energy has four weak
names; Paralyze has fifteen, producing 184 bytes. The list pointer generates one
RELO entry plus its FFFFFFFF sentinel. All eleven stock roots have no strong manifest
dependencies or IMP entries. A null/absent optional field remains zero.

An explicitly synthetic optional-fields fixture produces 200/20/0: two 32-byte
status masks, a two-name include list and a separate one-name exclude list. It checks
ANY, SAME_PLAYER, GOOD, NO_BRIBE and UNDER_IRON_CURTAIN bits and exact weak hashes.
That fixture is structural coverage only; it is not a twelfth real-game golden.

## Source and schema evidence

Supplied XML:
`D:\OneDrive\CNC Files\CnC_Modding_Support-main (Official XML, Schema, Script, Shader, Maps)\Uprising\Xml (Uprising)\Neutral\InfiltratorContainFilters.xml`
SHA-256 `D2DC3EA59BF3687C0F9CB337B0E38124BFD1029F178D505DB680AFE3AC49A99C`.
The fixture preserves the eleven literal filter definitions without unrelated
Tags/GlobalDefines inclusion; none of these definitions needs expressions/inheritance.

Repository official EP1 schemas:

- `schemas/ra3ep1/xsd/AssetTypeObjectFilter.xsd`: SHA-256
  `DC065C093B7AEB5EDBC5E7812A057A8685AD693AAA429773C7804221A66D0F8C`.
- `schemas/ra3ep1/xsd/Includes/ObjectFilter.xsd`: SHA-256
  `4EFB551492B9DD7BEDD42FC66D81C18362D27982AE6DD428E5FF3525E960ED24`.

The focused XSD also includes the supplied base, asset base, references, KindOf and
ObjectStatus definitions. Invalid rule values are rejected through schema validation.

Stock manifest:
`D:\TEMP\Red Alert 3 Uprising Source Data\Static Data\data\static.manifest`
SHA-256 `74FACD9056EEE20942BB274A924BEFFB03D7ADBB71B9609E804A39619B2C6081`.
The harness verifies EP1 version/aggregate, native type fingerprint, chunk lengths,
absence of strong references, and BIN/RELO/IMP magic/checksums before comparing
eleven bounded slices. It never dumps or rewrites the full game binary.

## Implementation and reproduction

- `source/SageBinaryData/SageBinaryData/ObjectFilter.cs` and `Includes/ObjectFilter.cs`:
  existing root/inline model, unchanged by this block.
- `source/BinaryAssetBuilder.XmlCompiler/Marshaler.ObjectFilter.cs` and
  `Marshaler.Includes.ObjectFilter.cs`: existing native marshallers, unchanged.
- `source/BinaryAssetBuilder.ManifestInspector/ObjectFilterNativeSmokeTest.cs`:
  default/native/weak-list tests, actual unregistered document normalization,
  separate synthetic optional coverage and bounded stock comparisons.
- `tests/fixtures/ObjectFilterPipeline.xsd` and `ObjectFilterProbe.xml`:
  focused official schema graph and source-derived fixtures.

Run `object-filter-self-test` with the Release/x86 inspector, optionally passing
the stock static manifest above. All eleven supported goldens must be present in
any supplied manifest; a partial manifest cannot silently count as the full proof.
The full `compiler-self-test` includes these tests without requiring local game data.
Inspector build, all **63** compiler groups and layout tests pass. Native inventory
remains 784 models and 760 typed marshallers of 1,390; overall effort stays about 50%.

## Remaining gates

Add only a bounded, explicitly experimental compiler profile after checking
normalized weak metadata consistency and schema/type identity. Inheritance,
arbitrary rule/mask combinations, ID-based filter references, source expressions,
complete target registration, cached native reuse and runtime loading remain open.
The managed TestObject/TestTemplate runtime-method placeholders are not invoked by
asset compilation; these native slice tests do not validate the engine's filter behavior.
