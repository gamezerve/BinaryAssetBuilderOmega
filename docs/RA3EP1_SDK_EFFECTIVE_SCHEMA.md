# Effective schema and typed-source gate

## Outcome

The real staged CnC3Types Include closure reaches 837 XSD documents, but strict
.NET schema compilation fails because ShieldSphereUpdateModuleData is declared
twice. Thus no real effective fields or trusted source bindings are published.
This is a new executable diagnostic gate, not a usable SDK build or a resolved
schema normalization decision.

```text
BinaryAssetBuilder.ManifestInspector.exe sdk-effective-schema [absolute-source.xml]
BinaryAssetBuilder.ManifestInspector.exe sdk-effective-schema-self-test
```

The first command reports JSON; exit 0 requires schema compilation and, if requested,
one-source validation. Exit 2 denotes incomplete schema/source evidence. When schema
compilation fails, RequestedSource is retained, SourceBinding is null and
EffectiveFileAttributes is empty. Empty is not complete. Unsupported command/path
syntax still rejects. ReadOnly and SnapshotOnly are true; FullDependencyCoverage
and ProductionBuildReady are always false.

Real external Uprising global.xml was requested through this command. It returned
exit 2 and skipped source binding at the duplicate-schema gate. No output was created,
no registry/settings/native processors were used and no reference XSD/XML was edited.

## Implementation

`source/BinaryAssetBuilder.ManifestInspector/SdkEffectiveSchema.cs`:

- `Inspect` captures/fingerprints the staged catalog and rechecks membership and
  hashes after compilation. It optionally reads one explicit absolute source only
  after schema compilation passes.
- `Compile` follows only captured relative XSD Includes, flattens them once as core
  `SchemaSet.ReadSchema` does, and compiles with XmlResolver=null. Imports/redefines,
  uncaptured paths, root escapes, URI/macros and DTDs reject. No schemaLocation can
  authorize network/filesystem reads.
- `Attributes` uses compiled complex-type AttributeUses and IsDerivedFrom to expand
  inherited/attribute-group fields and exclude prohibited attributes.
- `Bind` validates a captured EA AssetDeclaration against only the supplied compiled
  set, then selects FileReference-derived attributes/elements from SchemaInfo.
  Validation handles xsi:type; DataBlob is admitted through its ancestry. XML schema
  hints are not loaded. Invalid XML returns no trusted partial binding inventory.

Diagnostic declaration URIs under `file:///C:/Reborn-InMemorySchemas/` identify
snapshot provenance only: no file at that URI is opened or required. Logical field
values are retained; they are not payload paths, content hashes or compiler identities.
Schema default attributes may affect the in-memory validated DOM, never source files.

Bounds: 1,024 schema snapshots, 2 MiB each / 64 MiB total, Include depth 32,
4,096 effective file attributes, source 4 MiB, 2,048 selected source fields,
512-character field values and 64 diagnostics of at most 1,024 characters each.
Any compiler diagnostic closes compilation readiness. Consecutive hashes are not
an atomic filesystem snapshot or an EA AllTypesHash proof.

## Real duplicate evidence

`schemas/ra3ep1/xsd/Modules/ShieldSphereUpdate.xsd` contains both declarations:

| Declaration | Base | Distinct structure |
| --- | --- | --- |
| First, line 14 | SphereModuleUpdateModuleData | Adds shield damage/status/model/AttributeModifierName/ShieldedObjectStatus/options |
| Second, line 40 | UpdateModuleData | Repeats sphere geometry/filter/timing and adds ShieldBoneName/ShieldSizeMultiplier alongside shield fields |

The file and the external reference at
`D:\OneDrive\CNC Files\CnC_Modding_Support-main (Official XML, Schema, Script, Shader, Maps)\Uprising\Schemas (Uprising)\Modules\ShieldSphereUpdate.xsd`
have identical SHA-256:
`DA4B7000A8B24F201ADDF5F1C597FA4036206E089372B21F1A2AD6C211442A80`.
This establishes that the staged duplicate matches the supplied reference; it does
not authenticate an EA distribution or establish why both versions were retained.

`source/SageBinaryData/SageBinaryData/Modules/SphereModuleUpdate.cs` defines
ShieldSphereUpdateModuleData with a SphereModuleUpdateModuleData base and fields
matching the first declaration. The matching marshaller is
`source/BinaryAssetBuilder.XmlCompiler/Marshaler.Modules.SphereModuleUpdate.cs`.
YurikoShieldSphereUpdateModuleData derives from that shield model. This makes the
first declaration a concrete normalization candidate, not proof that choosing it
alone fixes schema compatibility, final type hashes or game loading.

## Tests and limits

Release/x86 builds with zero warnings/errors. All 114 default compiler groups and
33 enum checks pass. The new group deliberately expects the real staged duplicate
to remain a closed gate; passing that regression is not passing real schema compilation.

Valid entirely in-memory fixtures separately prove two-file Include closure,
FileReference/DataBlob ancestry, group/inherited fields, prohibited restrictions,
xsi:type selection and DataBlob element values. Unknown source types return no
trusted fields. URI/escape/import/DTD/oversize inputs reject. A forbidden remote
schema hint in the positive source does not authorize resolution.

This is not full source graph validation or asset inheritance preprocessing.
Effective global named-type attributes are enumerated; one admitted source can
bind element/attribute values. Include traversal, inherited asset-instance overlays,
payload existence/content snapshots, processor layout and final type tables remain
separate gates. Model/marshaller counts remain 785/1,390 and 762/1,390.
Weighted effort remains approximately 50% complete / 50% remaining.

## Next step

Create a fingerprinted diagnostic normalization layer on owned in-memory/working
copies, preserving the untouched reference catalog. Test the first Shield declaration
candidate against existing native shield evidence, then rerun strict compilation to
expose further schema errors. Record every transformation and its source hash;
never hide it behind resolver fallback or report normalized hashes as official hashes.
Only a compiled reviewed schema candidate can proceed to real reachable-source
binding and file/payload dependency closure. WorldBuilder packaging and an actual
minimal Uprising mod load remain required release gates.
