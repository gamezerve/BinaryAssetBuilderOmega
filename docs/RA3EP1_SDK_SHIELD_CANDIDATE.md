# Pinned in-memory Shield schema candidate

## Outcome and commands

The diagnostic `diagnostic-shield-first-v1` candidate structurally compiles the
837-XSD CnC3Types closure after removing the reviewed second Shield definition in
memory. There are zero schema errors and two warnings. Reference files are unchanged;
the default strict gate still rejects their duplicate. Candidate source binding is
not admitted while warnings remain.

```text
BinaryAssetBuilder.ManifestInspector.exe sdk-shield-schema-candidate [absolute-source.xml]
BinaryAssetBuilder.ManifestInspector.exe sdk-shield-schema-candidate-self-test
```

This is an explicitly separate diagnostic command, not production SDK activation.
Exit 2 currently denotes the incomplete clean-schema gate. The report distinguishes:

- SchemaEngineCompiled: structural .NET engine success (true for this candidate).
- SchemaCompiled: zero-error/zero-warning admission (false).
- Errors and Warnings: separate severity/provenance inventories, bounded to 64
  messages per severity and 1,024 characters per message.
- CandidateProfile, CandidateCatalogSha256 and Changes: explicit transformation
  identity, distinct from the untouched SchemaCatalogSha256.

EffectiveFileAttributes remains empty and SourceBinding remains null under this
closed admission gate; that is not complete source/dependency coverage. ReadOnly
and SnapshotOnly remain true; FullDependencyCoverage and ProductionBuildReady false.
No resolver/network/registry fallback or payload reads are added.

## Fingerprinted transformation

`SdkShieldSchemaCandidate.Apply` accepts only
`modules/shieldsphereupdate.xsd` with exact SHA-256
`DA4B7000A8B24F201ADDF5F1C597FA4036206E089372B21F1A2AD6C211442A80`.
It requires exactly two named declarations and the reviewed Sphere/Update bases,
keeps the first Sphere-based declaration and removes only the second duplicate.
XML serialization is deterministic; only the owned snapshot dictionary entry is
replaced. The input array, other snapshots and on-disk XSD are unchanged.
Changed input bytes and repeated application reject, rather than generic deletion.

Resulting file SHA-256:
`7F40355A0D3FFFC68FC24A00B8413AAA37EF8AA85808347473EE45397C22CDCD`.

Untouched 843-file catalog digest:
`63EE69B6B00BF60A2F09A28509BE7CBFCB2F935627782DA7B46DE53308CE6F5A`.

Candidate catalog digest:
`3A54DC3DDC54C1AACF15C03AF6AD67ED007AD99608CEDF78CC43EAD45AA969D3`.

These are metadata/snapshot hashes, not official-release signatures or EA
AllTypesHash/cache identities. Catalog membership/hash rechecks remain in place.

## Native/model evidence

The retained base matches
`source/SageBinaryData/SageBinaryData/Modules/SphereModuleUpdate.cs` and
`source/BinaryAssetBuilder.XmlCompiler/Marshaler.Modules.SphereModuleUpdate.cs`.
Tests verify ShieldSphereUpdateModuleData has the Sphere base and x86 size 312;
YurikoShieldSphereUpdateModuleData is 328 bytes. Existing native reference evidence
for GameObject:YurikoShieldProp/type 0x95D855B6 is documented in implementation
status. It supports this candidate but is not an exhaustive module/SDK proof.

The full suite reruns `CompilerSmokeTest.TestYurikoShieldSphereUpdate`, checking
the existing 460-byte instance / 12-byte relocation / zero-import graph with the
expected radii, timing, sphere bone, damage/status fields and minor-damage mask.
This managed regression is not a new read or game-load test of stock binary data.

## Remaining warnings

| Schema | Location | Warning/semantic question |
| --- | --- | --- |
| AssetTypeW3D.xsd | VertexData, line 337 | Prohibited declaration is ignored in this extension; comment calls it a custom-data dummy hook |
| AssetTypePathMusic.xsd | PathMusicTrack.Handle, line 43 | Prohibited declaration is ignored in this extension; comment says BAB populates Handle automatically |

.NET reports that `use="prohibited"` only prevents inheritance of an identically
named attribute from a base type. Both warnings carry in-memory URI/line provenance.
They are not fatal engine errors, but they are not silently waived. Whether the
ignored declarations preserve the required source/processor semantics must be
tested before a reviewed warning policy or further normalization is introduced.
Neither declaration was deleted or changed in this milestone.

## Validation and next step

Release/x86 builds with zero warnings/errors; all 115 managed compiler groups and
33 enum checks pass. The new group tests exact source pinning, deterministic output,
unchanged arrays/other files/reference bytes, replay/changed-source rejection,
312/328-byte model inheritance, digest separation and closed warning admission.
Model/marshaller inventory stays 785/1,390 and 762/1,390.

Next: compare compiled AttributeUses and positive/negative source validation for
VertexData/Handle, including their processor-only handling. Introduce only an
explicit fingerprinted reviewed policy or a separately attributed normalization,
then bind real sources with complete dependency/path snapshots. Do not weaken
default schema/path gates merely to get a success exit status.
Final type hashes/processors, original AUDIO/ART inputs, WorldBuilder packaging and
an actual Uprising mod load remain required release gates.

SDK tools/dependency effort is now estimated at 25%, up from 20% after the combined
environment/path/catalog/effective-schema/candidate work. Other workstreams retain
their previous estimates: weighted total 12 + 9.75 + 24.75 + 3.75 + 0 = 50.25%.
The general estimate therefore remains approximately 50% complete / 50% remaining.
This is manual engineering-effort assessment, not a percentage of working mods.
