# Uprising SDK capability inventory — October 15, 2026

This inventory describes **delivered capability**, not the presence of legacy C#
classes or XML schemas. No major family yet has unrestricted EA-SDK parity.
The limited local alpha is useful now; diagnostic success is not alpha admission.

Status terms:

- **Alpha / bounded:** available through the limited build workflow, within its
  documented restrictions.
- **Experimental:** selected native/Core/stream cases have offline evidence;
  not general Studio authoring or gameplay acceptance.
- **Reference / open:** legacy implementation, schemas or source investigation
  exists, but a general EP1 build path has not been established.
- **Gameplay:** means only the specifically observed recipes, not every possible
  input or freshly rebuilt package.

## Large asset families

| Area | Delivered | Main missing gate | Current availability |
| --- | --- | --- | --- |
| ArmorTemplate | Custom IDs, bounded multiple-armor compilation, selected scalar/default semantics, Includes/definitions/inheritance source bridge, Core streams and BIG/config packaging. Specific Allied/Soviet armor overrides have gameplay observations. | General EA source semantics, arbitrary graph sizes and a new GameObject consuming a newly authored armor. | **Alpha / bounded**, most mature family. |
| WeaponTemplate | Complete selected stock-shaped AK-47 slice, dependencies/native/token/stream checks; Damage=50 has gameplay evidence. | Arbitrary IDs, different weapons/nuggets, unrestricted fields and mixed armor/weapon/GameObject builds. | **Alpha / fixed AK-47 slice** only. |
| GameObject (units/buildings/props) | 600-byte root layout; selected stock inventories and module topology; reviewed scalar/enum/flag/weak/string/root-import token projection. Selected native module ports and generated destruction fragment. | Remaining root/child graphs, recursive token writers, shared named dependency closure, general processor, complete mixed stream and new-unit gameplay acceptance. | **Experimental**; no general GameObject in alpha. |
| Behavior / AIUpdate / ClientUpdate / other GameObject modules | Selected basic helper/sell/group-audio, tracker/freefall, toggle AI/weapon/state, destruction, TechUpgrade and LUAEventList native comparisons/dispatch. | EP1 review of remaining legacy modules, all required containers, generic source preparation and recursive token publication. Native LUAEventList storage is not execution proof. | **Experimental**, selected families only. |
| AttributeModifier | Selected native/stock comparison, isolated plugin/Core profile and checked import experiments. | General FX/shader dependencies, unrestricted source semantics, production integration and gameplay. | **Experimental**. |
| ObjectFilterAsset | Selected filter subset, native/Core profile, reviewed weak IDs and source-merge cases. | Full rules/include/exclude combinations, general source graph and production integration. | **Experimental**. |
| FXList | Empty/sound-nugget subset, checked Core profile, audio target resolution and bounded diagnostic streams. | Particle/light/decal/model and other nuggets; full dependency closure and gameplay. | **Experimental**, not general visual FX. |
| AudioEvent / Multisound / AudioFile | Selected native layouts, ordered references, profiles/mixed diagnostic streams; supervised PCM-to-XAS encoding and bounded AudioFile/event candidate packages. | General formats/options, longer/stereo/music pipelines, Studio integration and actual in-game playback acceptance. | **Experimental**, not an audio-enabled general alpha. |
| ShaderOverride / materials / shaders | Selected ShaderOverride rules have native/Core profile evidence. | Full material/shader compilation, arbitrary techniques and art dependency closure. | **Experimental** for ShaderOverride; wider materials **open**. |
| W3D / W3X models, skeletons, animation | Existing legacy compiler/model source and schema/format investigation. | EP1 processor/layout verification, custom data and complete new model/animation build + gameplay proof. | **Reference / open**. |
| Texture / PackedTexture / PackedTextureImage | Legacy structures/processor source; selected existing image references are understood. | General EP1 TGA/DDS conversion, packed image production, stream/custom-data integration and new texture gameplay proof. | **Reference / open**. |
| UpgradeTemplate / SpecialPowerTemplate / Locomotor | Existing source/schema models; selected Upgrade source inheritance/preprocessing and module-level TechUpgrade evidence. | General native/token/Core/package acceptance per family. Source-valid Upgrade XML does not mean Upgrade asset compilation. | **Source review / open** as general asset families. |
| LogicCommand / LogicCommandSet / PlayerTemplate / UI / EVA / gameplay globals | Legacy definitions and selected reference/source diagnostics. | Per-family EP1 ABI/processor review, complete compile/package path and runtime acceptance. | **Reference / open**; weak reference support is not payload support. |
| Maps / campaign overrides / WorldBuilder | Existing RA3 override examples and Uprising installation/format/toolchain investigation, including bounded WorldBuilder reference analysis. | Supported map production, Uprising editor integration and reproducible new-map acceptance. | **Reference / open** as an integrated SDK workflow. |

The module names above are selected verified cases, not an exhaustive native
module inventory. Not every EA schema type is listed individually; unsupported
types must still fail rather than silently compile via the legacy KW table.

## Smaller/shared subsystems

| Subsystem | Delivered | Still missing / important boundary |
| --- | --- | --- |
| XML / XSD validation | Real Core/schema validation and Studio errors for malformed/unsupported inputs; reviewed EP1 candidate and source audits. | Unmodified full schema admission is not established: diagnostic shield/hook accommodations remain explicit. Schema-valid does not mean build-enabled. |
| Includes / instance / inheritance | Confined armor source bridge; broader diagnostic source graph/preprocessing and selected merge/default tests. | Arbitrary EA graph semantics, unrestricted replacement/shadowing and integration across all asset families. |
| Definitions / `$` expressions | Exact `=$NAME`, reviewed aliases/suffixes and bounded nonnegative integer multiply-add in the armor source route. | General expression language and all field/type contexts. |
| Scalars / enums / bit flags / defaults | Selected EP1 numeric widths, units, enum sentinels, KindOf masks and default/native/token tests. | Review all other asset/module fields; no generic compatibility inference from schema ordinal order. |
| Strings / localization | Six GameObject ASCII length/pointer headers with variable allocations; missing/empty/escaped/long test cases. | Lossless arbitrary Unicode/ANSI handling, localization tables, complete STR workflow and production string assets. |
| Strong / weak references | Real Core normalization and scoped typed resolution tests; selected stock identities; root/module import slot preservation. A separate gate now normalizes icon/intro and repeated FX names in one Core table and checks real catalogs. | Generic closure across a whole GameObject build and compiling every target payload. CrowdResponse is absent from the supplied catalogs; no positive named stock target proof is claimed for it. |
| Type IDs / hashes / hash fix | Extensive independent runtime/schema identity and fingerprint audits; bounded profiles use checked EP1 contracts. | Complete production registry and general RA3-style post-build hash correction across all families. |
| BIN / RELO / IMP / manifest | Independent readers, bounded real Core builds, selected token/native emitters and byte/slot/identity validation. | Complete recursive GameObject tokens, arbitrary mixed-type streams and broader production/cache paths. |
| VERSION / suffix / BIG / config / launch | Scoped package naming/selection, independent package checks and controlled launcher/game observations. | General install/run UI and robust arbitrary mod closure. A filename suffix cannot fix an incomplete asset graph. |
| Model / W3X / animation files | Legacy compiler source exists. | General EP1 custom-data processor and missing-file diagnostics integrated into an actual model-producing build. |
| TGA / DDS / image resources | Existing image identity/reference and legacy-source groundwork. | General conversion/packing/build integration; referring to a stock portrait is not importing a new texture. |
| FX / particles / decals / lights | Sound-FX subset and target checks. | General visual FX nugget/particle production and runtime tests. |
| Audio / MP3 / music | Bounded native AudioEvent/Multisound and supervised XAS leaf/event encoding experiments. | General music/MP3 integration and in-game playback. RA3 SDK MP3 extensions are not evidence of EP1 support. |
| Missing definitions / references / wrong XML/schema diagnostics | Implemented for admitted routes, including fail-closed unsupported inputs; actual Studio failure callback observed. | Uniform diagnostics for every future processor, full missing W3X/TGA/animation dependency reporting. |
| Studio / build scripts | Existing EA-style interface, logs/warnings/errors/help and limited local alpha packaging. | General input/options/features; current diagnostic compiler progress does not automatically update the alpha. |
| Portability / release / caching | Clean local extraction/overlay checks, explicit dependency paths and frozen-runtime regression evidence. | Not universal one-click portability: five absolute path settings and external runtimes/stock data remain; clean-machine acceptance, redistribution review and broader safe cache reuse remain open. |

## What is actually complete?

Specific bounded milestones are complete: the tested armor override recipes,
fixed AK-47 damage recipe, existing Studio success/error paths, independent stream
and package readers, selected native model comparisons and their regression gates.
**No whole major asset family is declared complete for unrestricted authoring.**

The main unfinished integration is a new user-ID GameObject with a complete
token/native graph, referencing newly compiled armor/weapon/assets in one valid
stream, then exposed in Studio and accepted in gameplay. Texture/model/audio/map
work must subsequently open their own production and runtime gates.

The full-product planning estimate remains **about 45%, with a 40–50% range**.
It is subjective weighted engineering maturity, not a count of build-enabled
types. See [progress snapshot](RA3EP1_PROGRESS_SNAPSHOT.md) for the weighting.
Do not raise it merely because another selected field test passes.

## Evidence entry points

- [Alpha contract and limits](../alpha/README.md)
- [Weapon slice](RA3EP1_WEAPON_VERTICAL_SLICE.md)
- [GameObject root strings](RA3EP1_GAMEOBJECT_ROOT_STRINGS.md) and
  [root import-slot follow-up](RA3EP1_GAMEOBJECT_ROOT_STRONG_SLOTS.md), followed by
  [actual Core root/module names](RA3EP1_GAMEOBJECT_ROOT_NAMED_SOURCE.md)
- [Basic module ports](RA3EP1_BASIC_MODULE_PORTS.md)
- [AttributeModifier](RA3EP1_ATTRIBUTE_MODIFIER_PROFILE.md),
  [ObjectFilter](RA3EP1_OBJECT_FILTER_PROFILE.md),
  [FX](RA3EP1_FX_PROFILE.md), [ShaderOverride](RA3EP1_SHADER_OVERRIDE_PROFILE.md)
- [Multisound](RA3EP1_MULTISOUND_PROFILE.md) and
  [duration AudioEvent encoding](RA3EP1_AUDIO_DURATION_EVENT.md)
- [Source Upgrade admission](RA3EP1_SDK_INSTANCE_UPGRADES.md) and
  [source audio trees](RA3EP1_SDK_INSTANCE_AUDIO_TREES.md)

Older reports retain their historical scope and estimates. This inventory and
the current progress snapshot distinguish them from the current product contract.
