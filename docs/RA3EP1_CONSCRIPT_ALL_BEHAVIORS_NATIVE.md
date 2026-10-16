# All 36 original Conscript Behaviors: native gate

## Outcome

All **36 original behavior leaves / 24 concrete families** now match their
complete stock native graphs, first individually and then together through
**one actual Core occurrence table / 21 strong references / one Tracker**.
The shared container is **BIN/RELO/IMP = 5,624 / 388 / 88 bytes**. Both standalone
and combined compilation repeat deterministically. No separately compiled native
leaf bytes are appended to produce the combined graph.

The first unmodified compiler survey verified 23/36 leaves. The remaining work
clustered into four actual EP1/KW model differences, seven source-order cases,
one external definition, and native-only comparison of shared stock allocations.
The intermediate survey reached 35/36 before fixing the KW-only StatusBitsUpgrade
tail. A strict command now requires **all 36 rows**, not a passing survey with
known blocked rows.

This is **not generated behavior metadata**, a full original GameObject root,
an SDK alpha expansion or a new playable package. The existing selected root
composer still admits only its earlier three behavior classes. It is not silently
expanded to these 24 families by this diagnostic gate. Existing complete Draws
and client/root gates remain separate and active.

## Verified original inventory

| Concrete family | Complete original leaves |
| --- | ---: |
| ObjectSMCHelper | 1 |
| ObjectDefectionHelper | 1 |
| AttributeModifierPoolUpdate | 1 |
| SquishCollide | 1 |
| CrushDie | 1 |
| StancesBehavior | 1 |
| DestroyDie | 2 |
| CreateObjectDie | 1 |
| FXListBehavior | 3 |
| FireWeaponWhenDeadBehavior | 1 |
| AutoHealBehavior | 1 |
| SecondaryDamageTracker | 1 |
| RunOffMapBehavior | 1 |
| LevelUpUpgrade | 3 |
| SuppressionUpdate | 1 |
| FreefallUpdate | 1 |
| WeaponSetUpdate | 1 |
| PhysicsBehavior | 1 |
| SlowDeathBehavior | 7 |
| SpecialPower | 1 |
| ToggleStatusSpecialAbilityUpdate | 1 |
| StatusBitsUpgrade | 1 |
| HitReactionBehavior | 1 |
| ModelConditionUpgrade | 2 |

This qualifies these exact full original graphs. It does not claim every optional
child or authored variant of every family is implemented. The existing explicit
EP1 weapon-set route is reused, not sent through its incompatible legacy layout.

## Four isolated EP1 corrections

| Family | EP1 correction | Legacy KW route |
| --- | --- | --- |
| SquishCollide | 16-byte record: Time at 8; UseVehicleStyleCrush / AllowAllyCrush at 12/13; no CrushKillDelay | Original 20-byte record unchanged |
| Physics | Three shock timers at 24/28/32 are float Time values; defaults 1s / 1s / 1.5s | Original 104-byte uint-timer record unchanged |
| HitReaction | 48 bytes: Time timers, two-word death mask at 32, status-validation pointer at 40, three booleans at 44..46 | Original 36-byte record unchanged |
| StatusBitsUpgrade | 100-byte root without the KW-only ApplyToContained byte; weak upgrade list follows the root | Original conditional 104-byte record unchanged |

`Marshaler.MarshalEp1InfantryBehavior` explicitly allocates the isolated CLR
records and writes the original wire IDs: `B5EB41ED`, `79BFEDCB`, `59D12990`,
`786C9458`. It never hashes the new CLR names into runtime class IDs. Only this
explicit route and its diagnostic caller gain the corrections; legacy behavior
dispatch remains unchanged.

HitReaction's field offsets/types are independently anchored to its bounded stock
own-field metadata table, including the observed null ObjectStatusValidation
target. Populated validation is an authored/schema/literal control, not new
positive stock or gameplay evidence. Physics FX fields are retained by the new
marshaler, but this milestone does not newly qualify all optional populated FX
variants absent from the selected original graph.

## Source preparation and references

Read-only captured inputs, each at most 1 MiB:

- `BaseObjects/BaseObject.xml`
- `BaseObjects/BaseInfantry.xml`
- `Soviet/Units/SovietAntiInfantryInfantry.xml`
- `GlobalData/GlobalDefines.xml`

The explicit BaseObject -> BaseInfantry -> Conscript chain, 36 distinct source IDs
and every source-to-stock concrete schema type are checked. Captured source
hashes are checked again after both standalone and shared/authored controls.
This is not general inheritance/include execution.
The full inventory contains **two BaseObject, 19 BaseInfantry and 15 direct
Conscript leaves**; none of the 21 explicitly inherited leaves is skipped.

Seven original SlowDeath leaves interleave independent child families outside
the official XSD sequence. Only `FX`, `OCL`, `Weapon`, `Sound`, `DieMuxData` are
stably grouped into that reviewed order on owned copies. Unknown families fail;
every original element and within-family order is checked unchanged. Sound list
strings remain the official schema's string-list representation, not invented
strong imports or compiled audio assets.

One literal source expression, `=$MAX_SECONDARY_DAMAGE_SCALER`, is replaced by
the single captured `MAX_SECONDARY_DAMAGE_SCALER` definition from the actual
GlobalDefines file, currently `200%`. Drift or duplicate/missing definition fails.
This explicit capture is not a replacement general `$` expression engine.

The diagnostic resolver uses validated strong-reference schema annotations and
concrete derived target types to resolve plain names uniquely in the real linked
static/global catalogs. One production Core normalization then assigns actual
occurrence indices. No captured compiler indices are authored. Missing and
duplicate concrete targets are tested as explicit refusals. Weak references keep
their actual schema normalization/hash behavior; this does not establish general
weak dependency closure for publication.

## Shared stock allocations and metadata boundary

Some original stock records share bitmask or weak-reference list allocations
that the compiler independently allocates per field. Their complete native data
can match while a single stock-address-to-compiled-address dictionary is ambiguous.
That dictionary is needed for metadata oracle translation, not for native values.

`SelectedNativeGraphComparer` therefore adds an explicit native-only
`captureNativeCoordinates:false` mode for this survey. It still compares every
typed native field, pointed/list/string payload, padding, semantic strong target,
exact RELO/IMP inventory and full compiled byte ownership. It exports **no**
coordinate map. The original strict default remains unchanged for every existing
metadata gate. No ambiguous mapping is silently used to claim generated metadata;
future full behavior tokenization needs field-context-aware mapping/ownership.

## Verification controls

- 36 full standalone stock comparisons and repeats.
- **142 native corruptions**: class, local ID, final payload byte and missing
  nonempty RELO/IMP streams.
- **Ten authored/refusal controls**: five positive shapes (Squish flags/time,
  Physics edited/default Time values, HitReaction timers/mask/flags/present
  validation, StatusBitsUpgrade masks/list boundary), three forbidden KW-only
  source attributes, malformed Time and unknown dispatch. Legacy and EP1 size
  anchors are checked independently.
- One complete shared 36-leaf graph and repeat, with all 21 Core occurrence words
  consumed once. Every ordered leaf is independently re-walked against stock;
  pointers cannot cross leaf owners. The enclosing native count/items/array
  relocation set is checked independently.
- **Nine shared/reference refusals**: wrong count/header/first or aliased item/
  identity, missing output RELO/IMP, missing creation-list name and duplicate
  creation-list catalog identity.

The survey emits JSON rows with ID, class, source, status/stage, detail, three
buffer sizes and reference count. `Blocked` rows never count as native-qualified.
The strict command returns failure if any row is blocked or shared proof fails.

## Files and next gate

- `source/SageBinaryData/SageBinaryData/Modules/Ep1InfantryBehaviors.cs`
- `source/BinaryAssetBuilder.XmlCompiler/Marshaler.Ep1InfantryBehaviors.cs`
- `source/BinaryAssetBuilder.ManifestInspector/ConscriptBehaviorSurvey.cs`
- `source/BinaryAssetBuilder.ManifestInspector/SelectedNativeGraphComparer.cs`
- `source/BinaryAssetBuilder.ManifestInspector/Program.cs`
- `scripts/Test-Ra3Ep1Batch.ps1`

CLI: `conscript-behavior-native-self-test <official-xml-root> <static.manifest> <global.manifest>`.
`conscript-behavior-survey` exposes blocked-stage discovery separately. The
Focused batch adds only the strict all-36 native command with explicit
`-UprisingXmlRoot`.

Next: generate and independently verify metadata for these complete behavior
graphs, then join the actual remaining original root/Body/AI owners with all 36
behaviors, four Draws and seven client modules. Full object metadata/dependency
streams, a genuine new-ID package and controlled gameplay still follow. No
reduced unit is substituted.

## Regression evidence

Final Focused batch with explicit official XML root: **66/66 groups passed** in
**330.759 seconds**; all-36 native behavior group **7.889 seconds**. All eight
runtime hashes remain unchanged through the batch. The initial build passes in
1.36 seconds with zero warnings/errors; the final changed Inspector build also
has zero warnings/errors. Earlier dependency builds reported their existing
warning totals; no unresolved new warning is retained in the final build.

Evidence: `artifacts/ConscriptAll36Behaviors-a/batch-report.json`,
`compiler-build.log` and the complete JSON `conscript-behavior-native.log`.
The report records `GameExecuted=false`, `InstalledFilesModified=false` and
`AlphaCapabilitiesExpanded=false`. No Full-profile run is claimed. Elapsed batch
time is evidence for this run, not a controlled performance comparison with
earlier runs.
