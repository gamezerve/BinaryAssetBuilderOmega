# UpgradeTemplate authoring

<!-- Reborn: explicit upgrade assets use the official source language and real native stream layout, not generated placeholder upgrades. -->
The alpha frontend and mixed/support stream backend now admit explicit
`UpgradeTemplate` declarations. A new ID creates a new upgrade asset; an existing
stock ID overrides that upgrade. Referencing an ID in `TriggeredBy` does not
declare it. Ordinary EA `AssetDeclaration`, all/instance XML includes,
same-type `inheritFrom`, definitions and expressions use the shared frontend.
Instance-only bases are not emitted.

The user entry point remains `alpha/EALAModStudio.exe`. Upgrade-only mods do not
need a dummy GameObject, LogicCommand or command set. Upgrades may also accompany
the already supported GameObject/Armor/Weapon/command assets in one package.

## Native format

Original EP1 global upgrades are **not tokenized** (`Tokenized=0`), with type hash
`0x5D55CE2C`. Their native header is 140 bytes. The old 168-byte KW model remains
unchanged; the isolated `Ep1UpgradeTemplate` model/writer corrects its EVA string
fields and optional string pointers to inline asset-reference words. Audio, EVA
and FX references use real Core occurrence indices and imports.

All official existing fields participate in the source/native proof: the four
label strings, Type, BuildTime, BuildCost, sound/EVA/FX references, discount object
weak ID, GroupName/GroupOrder, AI heuristic, Options, IconImage,
WaypointQueueable and optional GameDependency. EP1's
`OBJECT_UPGRADE_PROJECTED` option is included. AIUpgradeHeuristicType keeps INVALID
at zero, with IGNORE=5, as proven against stock. No XML field is silently ignored.
An implementation error or genuinely unsupported nested layout still fails.

Each build containing upgrades qualifies the model against original stock/source
witnesses: Upgrade_AlliesFaction, InfantryCaptureBuilding, Upgrade_Pack and
Upgrade_JapanWarFactoryTech3. These cover plain records, labels, prerequisites,
inherited EVA references and the projected-upgrade option. Source/native checking
then independently checks authored values, strings, references, lists, optional
storage, padding and relocation/import ownership. Stock bytes are never emitted
as a template.

## Example

```xml
<?xml version="1.0" encoding="utf-8"?>
<AssetDeclaration xmlns="uri:ea.com:eala:asset">
  <!-- Reborn: explicitly declare this new upgrade; a weak trigger alone would not create it. -->
  <UpgradeTemplate id="Upgrade_PowerPlantTurbine"
                   Type="OBJECT"
                   BuildTime="5s"
                   BuildCost="250" />
</AssetDeclaration>
```

This declares data only; granting/researching it still requires the appropriate
authored command/special-power/gameplay relationships. It does not by itself add
a UI button or establish PowerUpgrade runtime behavior. Missing weak targets
remain located warnings; missing strong targets remain errors. Labels are checked
against the configured stock localization table and the mod's STR table.

## Implementation and verification

Implementation: `UpgradeTemplateCompiler.cs`, `UpgradeTemplateSmokeTest.cs`,
`Ep1UpgradeTemplate.cs`, `Marshaler.Ep1UpgradeTemplate.cs`, the shared source
preparation/inheritance/routing classes, and `ConscriptMultiAssetBuild.cs`.

Developer regression inputs and packages stay in private `alpha/.work` folders;
the tests do not alter Reborn/RebornTest sources, publish to Documents, change
game files or launch the game. Offline package evidence is not gameplay evidence.

<!-- Reborn: record actual measured regressions separately from supported source-language claims. -->
Release/x86 builds pass with zero warnings/errors on the final incremental build.
`upgrade-template-self-test` passes 5 complete source/stream/BIG cases and rejects
31 native/auxiliary corruption controls. The rich case fills every existing
field, including all EVA references, audio, FX, uint-max GroupOrder and
GameDependency; the default/new-ID case omits them. The inherited case uses an
instance-only base, a definition-expanded BuildCost=350, inherited Type/Options
and inherited GameDependency. The mixed case emits the authored upgrade plus a
Conscript that references it. Evidence:
`alpha/.work/upgrade-authoring-20261023-c/upgrade-report.json`.

The actual Studio backend also validates an upgrade-only inherited source graph
and its package receipt without a placeholder object. Evidence:
`alpha/.work/upgrade-studio-route-20261023-a`.
Missing-dependency and missing-string regression suites pass in
`alpha/.work/upgrade-dependency-regression-20261023-a` and
`alpha/.work/upgrade-string-regression-20261023-a`.
The final frontend additionally passes paired empty-tag inheritance and rejects
an invalid boolean at the original source file/line, under
`alpha/.work/upgrade-source-controls-20261023-a`.

The complete original `SageXML/GlobalData/upgrade.xml` also passes through an
ordinary `Include type="all" source="DATA:GlobalData/upgrade.xml"` wrapper:
**97 explicit upgrades** prepare, compile and pass stream/BIG/receipt readback
in one actual Studio-backend validation. This includes original same-file
inheritance, projected options, EVA references and the complementary split
GameDependency declarations. Normalization updates the owner references after
Core replaces an XML node; it does not emit the old detached owner or lose its
fields. Evidence: `alpha/.work/upgrade-original-file-20261023-d`.
The 104 stock global upgrade identities are not the count of declarations in
this particular original XML file. No original source file was modified.
