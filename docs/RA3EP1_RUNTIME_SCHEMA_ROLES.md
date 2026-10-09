# EP1 runtime/schema role reconciliation — October 9, 2026

## Result

Every one of the **1,342 pinned runtime table names** has a global complex-type
declaration in the staged official EP1 XSD catalog. **299** are direct typed
elements of `AssetDeclaration`'s top-level choice; **1,043** are complex types
not directly listed there. All **254 independently observed stock roots** fall
inside those 299 direct choices, leaving **45 direct choices** not observed as
roots in the four-manifest evidence set.

The schema contains **1,390 distinct complex names**, leaving **48 schema-only
names** absent from this decoded runtime table. This is not the same set as the
48 EP1-only schema additions in the RA3/EP1 coverage comparison: the latter all
appear in the runtime table. A matching count must not imply matching membership.

This is structural declaration-role evidence, not schema compilation, an
inheritance-flattened native graph, processor registration or game compatibility.
The 1,043 non-direct names must not all be called nested-only or runtime-only:
those stronger roles require usage and processor evidence. A `Runtime` name
suffix is reported separately and is never used to grant or reject asset admission.

## Catalog provenance

Root: `schemas/ra3ep1/xsd`, **843 XSD files**, read-only snapshots.
Catalog fingerprint: `8EC054C2C8D9C785D928736863534957B94D6C446D4F0B4A7BB70C9E850FEECC`.
The fingerprint hashes UTF-8 concatenated `relative/path<TAB>rawSHA256<LF>` rows
in the observed file order. It is a script/catalog fingerprint, not EA AllTypesHash
and not the effective-schema tool's separately defined digest domain.

Image/runtime table provenance remains the exact pins in
[runtime table reconciliation](RA3EP1_RUNTIME_TYPE_TABLE.md): image SHA
`ABA6A8825A744A78487837D78AD5992962257CE68404F639D600F11DEEA2580B`, ordered
runtime name/hash SHA `35AFC7028B8BA559C2AA654EEAD6C7C3E85872DF58C8F792152C11E7FB9E6AF1`.
Stock role flags reuse the independently pinned evidence artifact and its
SHA-checked actual manifests, not caller-provided name lists.

The catalog retains **both declarations** of `ShieldSphereUpdateModuleData`
and `UpdateModuleData`. It does not silently pick a declaration or apply the
separate reviewed effective-schema normalization. The existing shield-schema
issue remains visible; this inventory does not imply that unmodified XSD compiles.
`GameData.xsd` uses the separate `uri:ea.com:eala:asset:gamedata` namespace;
Parameter/Function provenance retains it. Runtime name matches must resolve in
the asset namespace, not merely share a local name with another namespace.

## Music roles and schema-only examples

| Type | Direct AssetDeclaration choice | Observed stock root |
| --- | --- | --- |
| PathMusicMap | yes | yes |
| PathMusicTrack | yes | yes |
| PathMusicEvent | yes | yes |
| PathMusicEventSet | yes | yes |
| PathMusicGameDynamicState / StateSet / Transition | yes | yes |
| PathMusicMapRuntime | no | no |
| PathMusicEventRuntime | no | no |

The runtime records' presence and different hashes do not authorize writing
them as authored source roots. The schema comment explicitly distinguishes the
event reference's authored and processed runtime forms; the header/processor
contract remains a separate gate.

Schema-only examples include BaseAssetType/BaseInheritableAsset, Include/Tag/Define,
choice wrappers such as BehaviorModules/FXNuggetTypes, RGBColor/Vector3/W3DVector3,
and CaveContainModuleData/HorseHordeContainModuleData. Their absence from this
table does not prove absence of engine support or that each needs a new compiler
registration. Full schema-only names and all declarations are in script output.

## Tool and tests

`scripts/Get-Ra3Ep1RuntimeSchemaRoles.ps1` uses `Read-RoleCatalog` and
`Get-RoleDeclarations`. It calls the pinned runtime tool once and reuses its
bounded input helper in script scope. XSD file count is capped at 1,024, each file
at 2 MiB, total at 64 MiB; XML resolution is disabled and DTDs prohibited.
The direct choice XPath intentionally excludes nested elements and does not
follow includes, derive inherited roots or guess root QName prefixes. File/hash
catalog observations are repeated afterward, but are not atomic filesystem
transactions. No output/game files or official schemas are changed.

```powershell
# Reborn: inventory literal schema roles; no schema compilation, native execution or plugin registration.
./scripts/Get-Ra3Ep1RuntimeSchemaRoles.ps1 -ImagePath 'D:\SteamLibrary\steamapps\common\Command and Conquer Red Alert 3 Uprising\Data\ra3ep1_1.0.game' -AsJson
# Reborn: check real counts, provenance, repeat JSON and in-memory declaration faults.
./scripts/Test-Ra3Ep1RuntimeSchemaRoles.ps1 -ImagePath 'D:\SteamLibrary\steamapps\common\Command and Conquer Red Alert 3 Uprising\Data\ra3ep1_1.0.game'
```

Tests pass for all measured counts, exact repeat catalog/rows/schema-only JSON,
authored/runtime music separation, duplicate preservation and a positive fixture
that distinguishes direct choice from nested elements. Four malformed in-memory
catalog cases reject DTD, unreviewed namespace, unresolved direct root and
unreviewed prefixed root policy. Runtime table regression tests also pass.
No C# compiler change or full compiler-suite rerun; groups remain 165.

## Limited table-use target

A separate bounded literal search found the preferred raw hash-block base
`00BE00D4` once in `.text`, at raw offset `007BDF01`. Its 28-byte context from
`007BDEF5` is `E88BBB91FF59C3CCCCCCCCA1D400BE006848E8CB00A350E8CB00E8EC`.
The base `00BE00D8` and parallel name-array base `00CBC0A8` had zero literal
matches. This supplies a narrow initialization/caller investigation target;
no disassembly/control-flow validation was performed and table callers remain
unverified. Other code can use shifted bases or indirect references.

## Newly found header candidate — not an EP1 replacement

Support-tree inventory found
`D:\OneDrive\CNC Files\CnC_Modding_Support-main (Official XML, Schema, Script, Shader, Maps)\Uprising\RA3Music.h`.
It is 63,212 bytes, SHA-256
`2051F33A86299B579F46508CB2A7AFF7067DB3CB3794EB9FE987FC140EFE37DD`.
The content identifies project RA3Music, not the required RA3EPMus path.
Its location under Uprising is not proof of EP1 authoring provenance.

Bounded Latin-1 lexical inventory of standalone `#define PATH_EVENT_name +0xhex`
lines found **248 unique event names**. Against the exact 197 stock event names:
**70 constants match**, **123 names are absent**, and **four values conflict**.
This is a candidate comparison, not the reference parser or accepted header
snapshot. It does not generate a header, rewrite values or close the AUDIO path.

| Event | Stock word4 | RA3Music candidate |
| --- | --- | --- |
| S_MPThreat1_1BrightonAction | `00000000` | `01E22B3F` |
| S_MPThreat1_1NYCExplore | `00000000` | `015FF951` |
| S_MPThreat1BrightonAction | `00000000` | `018BCC22` |
| S_MPThreat1NYCExplore | `00000000` | `013D27E4` |

Missing examples include Initialise, ChallengeMapTrack, all six MenuTrackEP1
names, and S_A01IgaIntro. This partial overlap corroborates some shared music
constants while directly rejecting wholesale RA3-to-EP1 header substitution.
The file also has one non-ASCII byte. The unchanged strict `pathmusic-header-review`
command rejects it with `Bounded ASCII header bytes required`, exit 1. That refusal
was verified; no parser policy was relaxed and the original file remains untouched.

Next: bounded disassembly/control-flow evidence at the hash-block-use target,
then processor metadata recovery for a small, independently proven root profile.
The missing EP1 header and production gates remain open. Estimate remains
**52% complete / 48% remaining**; there is still no proven game load.
