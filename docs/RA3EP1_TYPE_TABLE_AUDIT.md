# EP1 root asset type-table audit — 2026-10-01

## Result

Replacing the source XML/XSD files is insufficient. The current XmlCompiler
project defines both `KANESWRATH` and `VERSION7`: the latter selects stream
format, not the correct game type registry. `Plugin.AllTypesHash` still returns
KW's `0x12B3E763`, while all four inspected EP1 streams require `0x5454A8E9`.
The production gate in `OutputManager.CommitManifest` remains closed.

The active `Plugin.Initialize` tuple list registers only WeaponTemplate,
LocomotorTemplate, GameObject, AttributeModifier and ArmorTemplate. The large
commented type switch below `GetExtendedTypeInformation` does not register
anything. StringHashTable has a separate fallback, giving six current registry
entries; it was not present in these four evidence streams.

| Measurement | Result |
|---|---:|
| Distinct observed EP1 root asset types | 254 |
| Unregistered observed types | 249 |
| Registered observed types with wrong type hash | 5 |
| Matching observed registrations | 0 |
| Observed root types with current models | 90 |
| Observed root types with direct typed marshallers | 83 |
| Conflicting fingerprints / name-to-ID mismatches | 0 / 0 |

These root asset counts differ intentionally from the 1,390 complex schema
types: most complex types are nested records/modules, not plugin entry points.
Model presence is not native ABI validation. Direct marshaller presence does
not prove that a specialized processor, custom data or tokenization is correct.

## Verified mismatches

| Asset type | TypeId | Current KW TypeHash | Observed EP1 TypeHash | Tokenized |
|---|---|---|---|---:|
| ArmorTemplate | 0x3A6C5E8E | 0x6D59C409 | 0xA0E237D8 | 1 |
| AttributeModifier | 0xC5E07887 | 0x8C925761 | 0x74425C11 | 0 |
| GameObject | 0x942FFF2D | 0x50612DE9 | 0xCEB9FE36 | 1 |
| LocomotorTemplate | 0xECC2A1D3 | 0xDC4EEAC2 | 0x2A95936A | 0 |
| WeaponTemplate | 0x94D4D96E | 0x90AEAB74 | 0x27E8D984 | 1 |

Each mismatch was observed in both static and WorldBuilder streams. These
hashes are evidence, not an activated table. The aggregate hash must not be
computed by naively combining this observed subset, and must not be changed
alone to bypass the production gate.

## Reproduce

After a Release/x86 inspector build, run:

```powershell
# Reborn: read only small manifest metadata, not WorldBuilder's large BIN payload.
$ep1Root = 'D:\TEMP\Red Alert 3 Uprising Source Data'
& .\source\BinaryAssetBuilder.ManifestInspector\bin\x86\Release\net8.0\BinaryAssetBuilder.ManifestInspector.exe type-audit `
  "$ep1Root\Global Data\data\global.manifest" `
  "$ep1Root\Static Data\data\static.manifest" `
  "$ep1Root\WorldBuilder\data\worldbuilder.manifest" `
  "$ep1Root\EnglishAudio\data\audio.manifest" --json
```

The command reports every observed type/hash/tokenization fingerprint,
registration mismatch, model/marshaller presence, asset count and source path.
SHA-256 covers exactly the manifest bytes parsed. No adjacent `.bin`, `.relo`
or `.imp` payload is opened. The checked-in evidence snapshot is
`RA3EP1_TYPE_TABLE_EVIDENCE.json`; local paths identify provenance and are not
build requirements. Other installations can supply their own paths.

Implementation: `TypeRegistryAudit.Print`, `ValidateTarget` and `Classify` in
`source/BinaryAssetBuilder.ManifestInspector/TypeRegistryAudit.cs`.
The audit rejects other stream targets and malformed manifest metadata.
Conflicting fingerprints take precedence over a matching registration;
matching names are checked against the production FastHash TypeId algorithm.
The CLI snapshots/restores the plugin's private static registration dictionary
while inspecting the real Win32 initialization and fallback. It is intended
as a standalone diagnostic, not a concurrent production compiler service.
Regression checks in `CompilerSmokeTest.TestTypeRegistryAudit` cover all five
classification outcomes and wrong-version/wrong-game rejection. They do not
claim to validate the entire table.

## Next implementation gates

The first focused ArmorTemplate processor proof is now complete; see
[armor native/tokenized PoC](RA3EP1_ARMOR_TOKEN_POC.md) for exact static and
WorldBuilder byte matches. Production profile activation remains outstanding.

1. Start a minimal PoC with ArmorTemplate or AttributeModifier. Compare the
   official RA3 native layout and production EP1 schema against the current
   marshaller, including mask lengths, default values, reference imports and
   tokenization/processor behavior. Build a controlled fixture with known
   expected bytes; compare small real asset slices where semantics are known.
2. Add an explicit EP1 target profile and a provenance-backed registration
   subset only after those layouts/processors pass. Keep KW and EP1 type hashes
   distinct; do not silently reinterpret `KANESWRATH` as EP1. Review
   `CreateTypeInfo`'s ProcessingHash policy and platform adjustment separately.
3. Validate specialized Texture/AudioFile/W3D processors. For example, models
   exist for AudioFile/W3DAnimation/W3DMesh but the audit finds no direct typed
   marshaller. Those need processor analysis, not automatic registration.
4. Validate one-asset manifest/bin/relo/imp output with exact EP1 headers,
   dependency identities, external runtime references and deterministic cache
   behavior. A fixture writer passing structural checks is not a working SDK.
5. Package a reversible isolated mod and establish the actual Uprising loading
   path before claiming runtime support. Do not overwrite stock game streams.
   WorldBuilder packaging and complete SDK scripts remain separate gates.

Missing UI/settings roots need not all be implemented to demonstrate a tiny
mod that references stock streams, but stock references must resolve correctly.
The 254-type observed table is not asserted to be the complete game universe;
additional locale, LOD, map and campaign streams can extend the evidence set.

The overall engineering estimate remains approximately 50% complete / 50%
remaining. This block makes the remaining registry work measurable without
closing native processor or in-game loading gates.
