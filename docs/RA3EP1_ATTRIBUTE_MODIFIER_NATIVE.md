# AttributeModifier: exact EP1 native proof

Date: October 1, 2026

## Result

Eight source-derived modifiers match real Uprising BIN, RELO and IMP chunks
byte-for-byte: seven in static.manifest and one in worldbuilder.manifest. The
bounded proof covers default fields, list stride, percentages, model-condition
and object-status masks, duration, BLAT_TRIGGER and RADIATION_ARMOR.

Observed identity: TypeId `0xC5E07887`, TypeHash `0x74425C11`, Tokenized **0**.
This is a native processor path, unlike tokenized ArmorTemplate. It does not
activate production EP1 registration or prove arbitrary modifiers/game loading.
The rounded effort estimate remains 50%; model/marshaller counts are unchanged.

## Defect fixed

`source/BinaryAssetBuilder.XmlCompiler/Marshaler.AttributeModifier.cs` supplied
an empty-string default for ModelConditionsSet and ModelConditionsClear. The
pointer marshaller allocates a 60-byte mask for any non-null Value, even empty.
Absent optional attributes therefore generated two non-null pointers and 120
extra bytes, inconsistent with the real EP1 assets and official optional schema.
Both absent defaults are now null. Explicit mask values still allocate their
records; the existing fully populated 216/20/16 regression still passes.

The root is 56 bytes (RA3's official compiler metadata describes 60; the prior
EP1 port removed Anticategory). List count/pointer slots are at 44/48, with an
eight-byte Type/Percentage record. Optional masks are 60-byte ModelCondition and
32-byte ObjectStatus records. Default StackingLimit is 1; tail flags occupy 52/53.

`BinaryAssetBuilder/XmlCompiler/Plugin.cs` advances `_xmlCompilerVersion` from
1 to 2 and makes VersionNumber use that revision. Win32 legacy AttributeModifier
ProcessingHash is now `0x8C925761 ^ 2`; TypeHash and AllTypesHash remain the KW
values `0x8C925761` / `0x12B3E763`. This prevents old intermediate identities
surviving the native change without pretending the legacy registry is EP1.
The regression snapshots/restores the shared registry around this check.

## Exact fixtures

| Asset ID | Stream | BIN/RELO/IMP |
|---|---|---|
| AttributeModifier_RedAlert_Orange | static | 64/8/0 |
| Modifier_Test_Suppression_ForceMove | static | 64/8/0 |
| Unit_Veteran | static | 64/8/0 |
| Unit_Heroic | static | 128/12/0 |
| Modifier_Cover | static | 132/12/0 |
| AttributeModifier_TriggerBlatExplosion | static | 88/8/0 |
| AttributeModifier_SovietDesolatorInfantryBlatSlowdown | static | 72/8/0 |
| AttributeModifier_MechaKingSquishKillDelay | WorldBuilder | 88/8/0 |

`AttributeModifierNativeSmokeTest.Run/Compile/Compare` validates official schema
defaults, enums and invalid inputs, then calls the production native marshaller.
`tests/fixtures/AttributeModifierPipeline.xsd` includes actual EP1 primitive,
asset-base, references, masks, armor, shader-override and modifier declarations;
it does not stub those types. `AttributeModifierProbe.xml` contains focused
source-derived records, not the whole stock source package.

For supplied linked manifests, Compare checks metadata target/validity, all three
stream magic/checksum headers, each matched asset's fingerprints and sizes, then
seeks only its tiny chunks. The WorldBuilder sample starts at BIN offset
1,354,991,900 and reads 88 bytes. No full BIN dump or stock file writes occur.
The command prints SHA-256 for each matched BIN and metadata source. It rejects
a supplied manifest with no supported golden asset rather than claiming success.

## Provenance and reproduction

Local source root:
`D:\OneDrive\CNC Files\CnC_Modding_Support-main (Official XML, Schema, Script, Shader, Maps)\Uprising\Xml (Uprising)`.

- GlobalData/AttributeModifier.xml SHA-256:
  `EF50D9D05CA59651301169C1585C5CA2C9F38D52410461184EBE2B33C9DDF6C2`.
- EP1/GlobalData/AttributeModifiers_EP1.xml:
  `3B62C462047ED51B4EC0FAB72DD70717DE17E342DF390C539B6999BE46B2C9E5`.
- Japan/Units_SinglePlayerCampaign/JapanMechaKing.xml:
  `D751944879F6EC4A363E1AE95B05DBF89C8F0CF0CEDDAC130286535389A039AB`.
- EP1 AssetTypeAttributeModifier.xsd SHA-256, matching the local supplied schema:
  `E20DC82C50631871C7A26F66E6E3C283B0DC49071523811E4B47972E7CC0922C`.
- Clean official RA3 schema SHA-256:
  `8D4CEEC277557ED1B1D36145884761C39DDDA184ACE5CD53C80F7FFB41D16A52`.

Manifest hashes are in the preceding type-table evidence. The local source paths
document provenance; portable synthetic tests do not require those directories.

```powershell
# Reborn: compare only named small native chunks in local EP1 linked streams.
$ep1Data = 'D:\TEMP\Red Alert 3 Uprising Source Data'
& .\source\BinaryAssetBuilder.ManifestInspector\bin\x86\Release\net8.0\BinaryAssetBuilder.ManifestInspector.exe attribute-modifier-self-test `
  "$ep1Data\Static Data\data\static.manifest" `
  "$ep1Data\WorldBuilder\data\worldbuilder.manifest"
```

With no arguments, the command runs portable fixture/default/enum/revision tests
without reading game files. Release/x86 build, all 59 registered compiler groups,
layout checks and 33 enum mappings pass.

## Remaining gates

- StartFX, EndFX and Shader imports require a full core dependency-normalization
  plus real golden fixture; no-import exact matches do not prove that path.
- Inheritance, overrides and all combinations of optional masks/tail flags are
  not exhaustively covered by these eight assets.
- No experimental modifier plugin is activated by this block. Next: expose only
  the validated no-dependency subset in an isolated, fail-closed EP1 profile and
  compare its compiler entry against these native fixtures.
- Complete root registration/aggregate type compatibility, specialized processors,
  SDK/WorldBuilder packaging and in-game load remain required for a usable SDK.
