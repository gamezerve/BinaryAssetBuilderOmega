# UnpauseSpecialPowerUpgrade

<!-- Reborn: distinguish schema/compiler support for a legacy family from a populated retail witness or a gameplay guarantee. -->
The alpha compiler accepts `UnpauseSpecialPowerUpgrade` using the original
Uprising schema. It extends `UpgradeModuleData` with:

- `SpecialPowerTemplate`: a strong reference to a mod-produced or stock power.
- `ObeyRechageOnTrigger`: boolean, default `false`. The spelling **Rechage**
  is EA's original XSD spelling and must be preserved.

All inherited Upgrade fields are available, including `TriggeredBy`,
`ConflictsWith`, `Permanent`, trigger requirements, waypoint flags and optional
`CustomAnimAndDuration`. SampleMod's Titan declaration uses valid syntax; no
user XML was modified to enable this support.

The native layout is the stock-qualified 36-byte Upgrade base followed by the
power reference at offset 36 and boolean at 40 (44 bytes including padding).
Metadata uses the original schema extension and reviewed power-reference and
boolean wire signatures. The existing Upgrade and animation metadata grammar
is learned from original Uprising AttributeModifier/SubObjectsUpgrade records.

No original Uprising XML usage was found. A targeted class-wrapper search also
found no `UnpauseSpecialPowerUpgradeModuleData` wrapper in the local retail
static, global or WorldBuilder BINs. Both installed Uprising 1.0 and 1.1 game
executables contain the family name; that alone does not establish its runtime
behavior. Compilation therefore emits one `LEGACYMODULE` warning when the
family is used. There is no fabricated retail Unpause witness.

The regression checks current-source native bytes, imports, metadata, inherited
lists/animation, defaults, `ObeyRechageOnTrigger=true`, full GameObject
composition and rejection of a corrupted power index. Fast and developer
builds retain current-source validation. Triggering an upgrade on a paused
power still needs an in-game check.

Main implementation: `UnpauseSpecialPowerUpgradeContract.cs`,
`StockMetadataGrammar.QualifyUnpauseSpecialPowerUpgrade`,
`ConscriptVariableBehaviorSource`, `ConscriptVariableGraphVerifier` and
`Ep1BehaviorMetadataTokenizer`.
