# RA3 EP1 ObjectCreationList

Authored `<ObjectCreationList>` declarations are emitted as their own untokenized EP1 assets (stock fingerprint: type `0xE86E4D61`, type hash `0x1477F2E1`, tokenized `0`).

## Native layout

`SageBinaryData/Ep1ObjectCreationList.cs` follows the official RA3 SDK marshaler (`BinaryAssetBuilder.XmlCompiler.dll`, `Marshal<...Nugget>` push sizes and field offsets). The RA3 and EP1 OCL schemas are identical except for one inserted `OCNuggetOption` value (`CREATE_AT_SOURCE_IF_TARGET_REFLECTS`).

| Record | Size |
| --- | --- |
| ObjectCreationList (Base, Flags, six nugget lists) | 56 |
| CreateObjectNugget | 248 |
| AttackNugget | 16 |
| FireWeaponNugget | 8 |
| FlingObjectNugget | 52 |
| StrafeAreaNugget (`StrafeAreaSquad`) | 32 |
| ParabolicCurveNugget | 48 |

The legacy `ObjectCreationList` model (three lists, old option and disposition order, `ParticleSystem`) is not the EP1 layout and is not used.

## Qualification

Every build that contains an OCL:

1. Checks the ABI sizes and offsets above.
2. Proves eight original EP1 static-stream OCLs (DATA:EP1 sources) byte-for-byte with `SchemaNativeContract.VerifyStock`.
3. For every nugget family or nugget field used by the authored lists that the static set does not exhibit, finds a literal witness in `GlobalData/ObjectCreationLists.xml` and proves it against the global stream.
4. Refuses any family or field that has no original witness (for example `Attack` and `FireWeapon`, which have no EP1 stock OCL).

The authored OCL is then compiled with the EP1 marshaler and proved with `SchemaNativeContract.Verify`: full byte, relocation and import ownership, with every authored attribute consumed.
