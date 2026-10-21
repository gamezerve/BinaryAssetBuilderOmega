# BinaryAssetBuilder
A RA3 Uprising (EP1) port of the Kane's Wrath-based .NET BinaryAssetBuilder.

Final target: RA3-SDK-style general Uprising authoring, including user-defined
asset IDs, instance includes, inheritance, `$` definitions/expressions,
TGA/DDS textures and W3X models. Current alpha capability restrictions are
temporary. See the [final product contract](docs/RA3EP1_SDK_RELEASE_PLAN.md).

## Current outcome — October 21, 2026

Active development branch: `main` in `gamezerve/BinaryAssetBuilderOmega`.
It contains the Uprising SDK work previously developed on
`feature/ra3ep1-manifest-inspector`; the original `master` history is retained.

<!-- Reborn: reuse working EP1 list processors rather than treating stock cardinality as an authoring capability limit. -->
[ArmorSet and hardpoint/Weapon lists](docs/RA3EP1_COMBAT_LIST_AUTHORING.md) now
follow source count and optional reference presence. The 32-armor/8-hardpoint/
16-weapon trial quotas are removed. Weapon Ordering is normalized using the
official sortBy semantics; omitted/default nested attributes remain supported.
This is implemented EP1 hardpoint support, not unimplemented turret/contained
layouts or a general multi-asset GameObject frontend.

<!-- Reborn: source text keys are editable values, not fixed stock example identities. -->
`DisplayName` and `DisplayNameTransformed` can now be changed, added, omitted or
explicitly emptied, together with root descriptions and variable Behaviors.
Both the source gate and final metadata producer use authored presence/content,
not equality to `Name:SovietAntiInfantryInfantry`. These are ASCII localization
keys, not a localization-table compiler; a key's translated text remains a
separate resource. See [root text authoring](docs/RA3EP1_ROOT_TEXT_AUTHORING.md).

<!-- Reborn: nested upgrade fields are source-driven ABI structures, not stock-sized examples. -->
[Nested upgrade authoring](docs/RA3EP1_UPGRADE_NESTED_FIELDS.md) now supports
adding/removing/omitting `TriggeredBy` and `ConflictsWith` entries, plus an optional
`CustomAnimAndDuration` record with editable/defaulted attributes. Native arrays,
optional pointer presence and metadata extents follow source. The old 4096-item
tokenizer quota is replaced with actual buffer bounds; a 5000-entry package passes.
Nine live-driver controls pass, including inheritance, real invalid input errors
and last-good output retention. This is scoped structure support, not all nested
fields or general GameObject authoring.

<!-- Reborn: existing authoring features must compose rather than becoming mutually exclusive trial profiles. -->
Variable `Behaviors` now combines with compatible existing strong-reference
retargeting and typed upgrade trigger edits. Reference identities come from the
authored source and stock catalog, not frozen module-template names. The former
64-behavior quota is removed across source admission, marshaling and metadata;
actual buffer/ownership checks remain. Eight complete graph/package cases pass,
including 65 behaviors, combined weapon/upgrade edits and wrong-target corruption
rejection. Upgrade weak references now report genuinely missing assets with
source coordinates. This does not enable unimplemented module layouts/resources.

<!-- Reborn: versioned final publication follows the user's requested RA3 SDK workflow. -->
Studio now has a **Mod Version** field (default `1.00`). Final builds publish
`<name>_<version>_Streams.big`, `<name>_<version>_Misc.big` and
`<name>_<version>.skudef` to the Windows Documents known folder under
`Red Alert 3 Uprising/Mods/<name>/`. The SKUDef includes Streams and optional nonempty Misc.
`alpha/mods/<name>/Misc/` is packaged root-relative, excluding `.asset`
intermediates like the official SDK. Missing/empty or excluded-only Misc produces
no Misc archive or SKUDef entry. Other versions and unrelated user files are preserved; modified or
unmanaged same-version collisions fail visibly. See [packaging details](docs/RA3EP1_FINAL_PACKAGING.md).

<!-- Reborn: source diagnostics must refer to authored files, never misleading generated-XML coordinates. -->
[XML diagnostics](docs/RA3EP1_XML_DIAGNOSTICS.md) now carry physical source
file/line/column information through schema validation, inheritance and fragment
preparation. Uncertain origins are explicitly labelled source context rather
than presented as exact field locations. This does not expand the supported
GameObject topology.

<!-- Reborn: variable behavior composition has independent full-object evidence before its separate Studio publication gate is widened. -->
[Variable module graph qualification](docs/RA3EP1_VARIABLE_MODULE_GRAPHS.md) now
passes complete Conscript cases with 18/30/36/48 behaviors, including removal,
reordering and new-ID clones. Native allocations, reference occurrence counts
and metadata extents are derived from each graph. Six cases now produce and
independently read back real manifest/BIN/RELO/IMP and BIG/SKUDef packages,
including a new-ID variable object plus an explicitly authored production
command. The graph-qualified serializer binds exact source, ordered targets and
all three buffers; 30 native, 60 metadata and 3480 stream/reference controls pass.
The live Studio build now admits reviewed variable `Behaviors`: add/remove,
reorder and clone known complete payload shapes with new local IDs, while editing
their fixed fields and reviewed fixed root values. Normal EA includes/definitions/
inheritance and explicit production commands work on this route. Unsupported
nested shapes and colliding IDs retain the last successful package. Existing
compatible strong targets and upgrade trigger names now compose with variable
topology; other containers/root dynamic strings and changed reference presence
still require additional implementation.
User Reborn sources/output remain untouched; this is compile/package support,
not new gameplay validation or general GameObject support.

<!-- Reborn: track the user's official RA3 Studio transcript as a required output contract, separate from implemented alpha capabilities. -->
The [build log compatibility contract](docs/RA3EP1_BUILD_LOG_CONTRACT.md) records
the official RA3 SDK's eleven phases, timestamped compiler details, diagnostics,
postprocessor counts and timing as the target Uprising build output. This is a
requirement, not a claim that the current alpha already implements those stages.
The live driver now shows numbered phases/timings and explicit skips, streams
diagnostics without JSON receipts, and saves readable success/failure logs.
Full backend diagnostics remain separately under `alpha/.work/last-detail.log`
or `last-failure-detail.log`; package/source support is unchanged.

<!-- Reborn: aggregate missing names against emitted mod assets plus configured original-game manifests before fail-fast compilation. -->
Step 6 now lists [missing assets and definitions](docs/RA3EP1_MISSING_DEPENDENCIES.md)
with source/owner/line details, including definition-to-definition dependencies.
Availability uses mod-produced identities plus stock static/global manifests;
instance-only witnesses do not count as produced assets. The latest list is
`alpha/.work/last-dependencies.json`. Weak/dynamic and arbitrary inherited/default
reference coverage is not claimed by this source preflight.

**Explicit EA XML frontend:** RebornTest now uses an AssetDeclaration `mod.xml`
including `Conscript.xml` and `CommandData.xml`. No LogicCommand is generated
implicitly. XML headers/namespaces/local all-XML includes and attribute order
are accepted. RebornTest now also uses actual instance inheritance from
`ConscriptBase.xml` and a `DATA:Defines.xml` BuildTime definition. The base is not
emitted. The GameObject source route now merges schema-valid partial bases and
nested derived bodies through Core, expands literal EA root/child XInclude
fragments, and compares bare/qualified reference spellings by compatible identity.
Final reviewed graph/native checks remain mandatory; arbitrary new module graphs
and ART/W3X resource compilation are still disabled. See
[precise source support and omission/order rules](docs/RA3EP1_EA_SOURCE_FRONTEND.md).

<!-- Reborn: replace captured-name restrictions with schema/catalog/native dependency validation on existing strong-reference slots. -->
Existing nonempty schema-typed strong references inside the Conscript graph can
now select other compatible stock assets, including `Weapon/@Template`.
Targets are no longer pinned to the original Conscript names. Missing targets,
wrong types, ambiguous identities and authored compiler indices still fail;
native import slots and emitted target occurrences remain verified. This does
not enable new module/list topology or arbitrary new asset compilation.

<!-- Reborn: derive editable module value categories from schema/native descriptors instead of captured Conscript literals. -->
Reviewed module values now include **412 fixed-width attribute slots** across
the existing 49-module Conscript graph: the previous 384 primitive slots plus
8 enum and 20 fixed-flag slots. Local module IDs may also be renamed, with
hash-collision checks and independently verified native headers. This is not
module addition/removal/reordering. See [module value capabilities and remaining
limits](docs/RA3EP1_MODULE_VALUE_CAPABILITIES.md).

**SDK source lookup:** DATA/ART/AUDIO now use explicit mod-first, stock-SDK
fallback search. This local SDK contains the original Uprising source copy under
`alpha/SageXML`, using adjacent stock DATA lookup without an external override. Resource lookup
does not enable resource compilation. See [exact search order](docs/RA3EP1_SOURCE_SEARCH.md).

**One live alpha SDK:** open `alpha/EALAModStudio.exe`, edit the selected mod's
`alpha/mods/<name>/data/mod.xml`, then use **Build Mod**. Successful builds reuse
the Documents mod folder for versioned final archives/SKUDef. Internal
`alpha/builtmods/<name>/` keeps compatibility copies, `build.json` and
`last-build.log`; no final CFG is published. Failed builds preserve the last good
output. Successful Release/x86 compiler builds refresh the tools in this same
SDK without replacing editable mod sources. No new dated SDK copy is needed.
This remains a repository-linked local alpha, not a portable public release.
See [supported alpha input](alpha/README.md) and
[maintenance / retained lessons](docs/RA3EP1_LIVE_ALPHA_AND_RETENTION.md).
Studio now lists only the user's **Reborn** and the developer's **RebornTest**.
RebornTest now uses schema/native capability contracts instead of the six-field
authoring whitelist: all 63 root primitive ABI descriptors, 15 fixed root
enum/KindOf/weak-ID descriptors, optional stock-target root strong references
with dependency reindexing, independent bounded allocation of all six root ASCII strings, plus
384 schema/native/token-qualified primitive slots across the 49 existing modules,
and the existing nested numeric owners. The module registry is derived from the
official schema and reviewed native models. It supports direct numeric/boolean,
Time/Angle/Duration/Percentage/Velocity fields while retaining each module's
identity and child/list/reference structure. See the
[complete per-family field inventory](docs/RA3EP1_MODULE_PRIMITIVE_CAPABILITIES.md).
The full module primitive graph passes native/metadata/stream verification and
384 per-slot corruption controls; Studio backend regression passes **25/25**,
including a combined draw/physics/death/AI/body package and last-good-output preservation.
Scale=1.2 and
ThreatLevel=15 demonstrate the primitive route. Former gameplay caps are removed;
module topology and reference changes outside the three reviewed root slots remain restricted.
UnitTypeIcon/UnitIntro may now be removed, restored or retargeted independently;
all four presence combinations pass with resized strings and 273..275 real dependencies.
CrowdResponse requires a real catalog target; none is available in the current stock catalogs.
See [current capability contract](docs/RA3EP1_GAMEOBJECT_SCALAR_CAPABILITIES.md).
The original-ID six-scalar trial now has qualitative user gameplay confirmation.
The bounded `RebornConscriptTrial` (cost=15, time=2) has user-reported gameplay
success: a new GameObject plus an overridden existing UNIT_BUILD command. It reuses the
Conscript Barracks slot, leaving CommandSet/other choices unchanged; recruitment
was gameplay-observed for that package. Its command is now explicitly authored in XML.
See [historical recruitment scope](docs/RA3EP1_CONSCRIPT_RECRUITMENT.md).
The bounded real source-build route still rejects other GameObject changes;
this is not general GameObject authoring. Automated regressions reuse RebornTest
and leave the user's Reborn source and outputs untouched. Old armor/weapon
samples are retained as fixtures outside the Studio mod list.

Closing Studio during its own active build now asks for cancellation. Yes stops
only that session's worker tree and releases the SDK lock; No keeps Studio/build
running. Final output publication is protected against mid-commit cancellation.
Restart an already-open Studio once after this update; see the
[lifecycle implementation and checks](docs/RA3EP1_STUDIO_LIFECYCLE.md).

Obsolete generated SDK/artifact snapshots have been consolidated: 311 directories
and 117,989 files were removed only after complete archive/hash verification.
The recoverable local backup and inventory reduce net storage by approximately
1.86 GB. Live mod sources, required test inputs and runtime evidence were retained;
see the maintenance report above for the exact archive location and recovery policy.

**User-confirmed gameplay:** the SKUDef-only trial worked and the Conscript
was reported produced in approximately **one second**, matching authored
BuildTime=1 rather than stock 4. This supplies the first gameplay evidence for
the complete source-compiled Conscript **original-identity GameObject override**,
not merely an armor/weapon edit. Timing is a human observation, not an
instrumented measurement or execution proof for every module/LUA branch.
`New-Ra3Ep1ConscriptTrial.ps1` and `Invoke-Ra3Ep1ConscriptTrial.ps1` now default
to **SKUDef-only**; `-LegacyCfg` explicitly preserves the historical dual-file
route. The tested folder contains no CFG counterpart. No universal CFG+SKUDef
requirement exists for this observed route. New-ID recruitment and general
GameObject authoring remain separate.

One authorized **SKUDef-only** Conscript trial has now been started using
`artifacts/Conscript-Sku-20261018-a/reborn_1.0.skudef` (163-character path).
This separate candidate contains **no `.cfg` counterpart** and retains the same
pinned BIG/source bytes. The 30-second observer recorded **39 matching child
argument snapshots**, no observer failure and no installation writes. Main-menu,
config consumption and BuildTime behavior were not inferred from those
snapshots. Subsequent user gameplay feedback is recorded separately above;
the raw launch report remains unchanged. The legacy CFG candidate is retained
as historical evidence, not selected by default.

The fixed BuildTime=1 override is now exported unchanged to the short local
folder **`artifacts/Conscript-20261018-a`**. Its config path is **156 characters**,
and pinned archive/source hashes, exact config/SKU bytes and readback pass.
`scripts/New-Ra3Ep1ConscriptTrial.ps1` provides export, `-CheckOnly` and
`-SelfTest`; five byte/receipt controls and three output-policy refusals pass.
`scripts/Invoke-Ra3Ep1ConscriptTrial.ps1` **without `-Run`** passes the reviewed
installation/SKU/campaign/loose-file/process checks. That preflight did not
launch a game; the subsequent authorized SKUDef experiment is recorded above.
`-Run` always requires a separately authorized single controlled trial. See the
[file map and runtime preparation](docs/RA3EP1_CONSCRIPT_AUTHORED_TRIALS.md#short-export-and-file-map).

Two [source-authored Conscript trial candidates](docs/RA3EP1_CONSCRIPT_AUTHORED_TRIALS.md)
now pass offline qualification: the original identity with **BuildTime 4 -> 1**,
and **RebornConscriptTrial** with the same edit. Both are genuinely recompiled
from the captured source, retaining all 49 modules, six populated owners,
geometry, 126 metadata fields and 275 reference occurrences. Each produces a
separately read-back **15-entry BIG / config / mod-SKU fragment / source snapshot /
receipt**. Source admission is explicitly limited to those two identities and
that one edit; arbitrary GameObjects are not enabled in the alpha.

The new identity has **no CommandButton/CommandSet/spawn integration**, so it is
not yet recruitable from the menu. Neither candidate has been installed or
launched. The original batch config paths remain **256 / 258 characters**.
The override now has the separately validated **156-character export** above;
the new-ID candidate still needs short export and recruitment integration.
Receipts conservatively retain `RuntimeReady=false`; no gameplay is inferred
from a successful preflight.

Authored-trial Focused regression: **74/74** in **225.049 seconds**, four
isolated workers. The actual-original/native/metadata/three-stream/two-package
group took **75.403 seconds**. Each stream profile passed **580** refusals;
the authored gate adds **8** source/native refusals and each package rejects
**5** file corruptions plus an occupied output. Eight runtime hashes stayed
stable; batch build had zero warnings/errors. No Full run, game/installation
change or alpha capability expansion.

The [actual original Conscript diagnostic stream](docs/RA3EP1_CONSCRIPT_ORIGINAL_STREAM.md)
now writes and independently reads back **one tokenized v7 GameObject**, its
complete qualified metadata/native graph and **275 ordered external reference
occurrences**. Real static/global metadata map to explicit runtime paths. Core
checksum packing, Manifest/Utility readers, deterministic fresh output and
**580** stream/dependency/profile refusals pass. Default tokenized GameObject
serialization remains blocked; this is a separate original-identity diagnostic.
The instance hash is explicitly an experimental normalized-source hash, not a
full EA processed-document identity. External native closure, arbitrary/new-ID
authoring, BIG/config packaging and gameplay are not proved by that stream-only
milestone. The separate authored profile and offline package gate now pass
above; shorter-path/runtime preparation and gameplay remain open.

Original-stream Focused regression: **74/74** in **409.014 seconds**, four
isolated workers. The actual-original native/metadata/stream group took
**77.205 seconds**; unchanged all-module and selected-root groups passed in
**39.023 / 33.842 seconds**. All eight frozen runtime hashes stayed stable;
batch build had zero warnings/errors. No Full run, game launch, installation
change or alpha capability expansion.

The [actual original Conscript metadata](docs/RA3EP1_CONSCRIPT_ORIGINAL_METADATA.md)
now also passes independent stock qualification: **126/126 root fields**, **21
disjoint metadata owners**, and the entire **88,824-byte prefix**, retaining the
already qualified complete native graph and one Core / **275 occurrences**.
BIN/RELO/IMP is **128,184 / 1,800 / 1,104 bytes**. **22,212** metadata/native
corruption controls pass, including every metadata word. The original-only
route retains null DisplayNameTransformed, Flammability and LiveCameraOffset;
the previous selected-root route is unchanged. This closes actual-root metadata
qualification, not stream publication, playable new-ID packaging, general
inheritance or alpha admission. Scoped diagnostic stream serialization now
passes the separate gate above; general production publication remains open.

Original-metadata Focused regression: **74/74** in **389.051 seconds**, four
isolated workers. The original-native group now also runs full metadata proof
and took **79.091 seconds**; the previous 126-field selected-root group passed
in **46.883 seconds**. All eight frozen runtime hashes stayed stable; the batch
build had zero warnings/errors. No Full run, game launch or installation change.

The [actual original Conscript native graph](docs/RA3EP1_CONSCRIPT_ORIGINAL_NATIVE.md)
now independently matches stock semantics: **all 600 root bytes / actual root
values / all 49 modules / six populated owners / geometry**, compiled on one
Core table / **275 strong occurrences** / one Tracker. A complete disjoint
ownership union checks **449 relocation sources and 275 import slots**, with no
unowned native payload. BIN/RELO/IMP is **39,360 / 1,800 / 1,104 bytes**.
**108** native/source refusals pass. The existing single explicitly bounded
Script CRLF/LF equivalence remains; this is not raw address/byte identity.
The source is the explicitly captured three-object chain, not the prior
synthetic root values or a general inheritance/include engine.

This exposed **14 incorrect legacy root defaults**, now corrected to official
RA3/EP1 schema values. An independent direct-marshaler gate checks omitted,
explicit-default and explicit-zero source vectors, keeping Time seconds distinct
from raw logic-frame integers. Explicit inherited values remain authoritative.
Actual-root metadata and scoped diagnostic streams now pass the separate gates
above; general production publication remains open. The earlier 126-name gate uses the selected
root scenario. Neither milestone makes a playable new-ID package or expands
alpha admission. No game or installation change.

Original-native Focused regression: **74/74** in **402.885 seconds**, four
isolated workers; original-native group **59.483 seconds**, default-vector
group **9.225 seconds**, prior 126-name group **48.228 seconds**. Eight frozen
runtime hashes stayed stable; batch build had zero warnings/errors. No Full
run. Timings include overlapping diagnostic scenarios, not a mod-build benchmark.

The [complete named Conscript root profile](docs/RA3EP1_CONSCRIPT_EMPTY_ROOTS.md)
now matches **126/126 stock field names and wire tags**, retaining all 49 original
modules and six populated owners on one Core / **271 occurrences** / one Tracker.
The remaining sixteen names have independently qualified **five empty list
tables and eleven null pointer targets**, including native offsets, schema
optionality/type/cardinality and original-source absence. **102** native/source/
output refusals pass. BIN/RELO/IMP is **129,848 / 1,860 / 1,088 bytes**, with an
**89,316-byte metadata prefix**. Populated variants of those sixteen fields are
explicitly refused. The prior 104-field and 110-field routes remain unchanged.
At this earlier named-field milestone, selected root values still required
original-source qualification. The actual native-value gate above now passes;
complete original metadata now passes too; stream publication and gameplay
remain open.
This is not 100% SDK completion, a general GameObject compiler or alpha expansion.

Complete-name Focused regression: **72/72** in **612.261 seconds**, two workers;
126-field group **39.977 seconds**, unchanged 110-field group **40.041 seconds**,
unchanged 49-module group **39.194 seconds**. Eight frozen runtime hashes stayed
stable; the batch build had zero warnings/errors. No Full run, game launch or
installation change. Overlapping regression-case timing is not a mod-build benchmark.

All [49 original Conscript modules and six populated root owners](docs/RA3EP1_CONSCRIPT_COMBINED_OWNERS.md)
now independently match stock on **one selected root / one Core table / 271
strong occurrences / one Tracker**. The generated partial projection has **110
named root fields**, an **89,064-byte metadata prefix**, a **40,532-byte native
graph**, and **BIN/RELO/IMP = 129,596 / 1,860 / 1,088 bytes**. The combined gate
passes 46 scope/ownership/output refusals and checks the real composed metadata,
not a regenerated expected fragment. Existing default and 49-module-only routes
remain unchanged. At that milestone, **16 stock-empty/null names** and full
original root-value/source qualification still blocked original-root publication.
This is not a playable new-ID package, general GameObject admission or an alpha
capability expansion. Its sixteen-name metadata gap is now closed by the
separate empty-profile gate above; the actual original-value and metadata gates
now pass as well. General source semantics and stream publication remain open.

Combined-owner Focused regression: **71/71** in **643.863 seconds** with two
workers; combined-owner group **44.474 seconds**, unchanged all-module group
**46.869 seconds**, standalone remaining-owner group **10.216 seconds**. All
eight runtime hashes stayed stable; the batch build had zero warnings/errors.
No Full run, game launch or installation change. Timing includes overlapping
regression cases and is not a promised build-time benchmark.

The [six remaining populated Conscript root owners](docs/RA3EP1_CONSCRIPT_REMAINING_ROOTS.md)
now independently match stock native graphs **and generated metadata**: resource
costs, skirmish AI, voice, EVA, shadow and crusher data. One Core normalization /
23 strong occurrences / one diagnostic Tracker feeds all six. Their generated
metadata totals **2,412 bytes**; **43 authored/refusal controls**, **18 native
corruption refusals** and **30 metadata corruption refusals** pass. EP1-specific
skirmish, crusher and EVA layouts leave the old KW layouts/dispatch unchanged.
At that standalone milestone these owners were not yet joined to the all-49-module selected root below; the combined gate above now joins them. The
sixteen empty/null metadata fields are now qualified by the complete-name gate;
original-source qualification of selected values still blocks complete publication.
Unreviewed populated child variants fail explicitly; no alpha or gameplay
capability is newly claimed.

Final Focused regression with the official XML root: **70/70** in **434.649
seconds**; remaining-root group **5.494 seconds**, existing all-module group
**27.627 seconds**. All eight frozen runtime files remained unchanged; the batch
build had zero warnings/errors. No Full run, game launch or installation change.

All [49 original Conscript module graphs](docs/RA3EP1_CONSCRIPT_ALL_MODULES.md)
now independently match stock on **one selected root / one Core table / 248
strong occurrences / one Tracker**: 36 Behaviors, four Draws, one original
AIUpdate, one original ActiveBody, one ClientUpdate and six ClientBehaviors.
All six container metadata graphs are checked against independent stock tables;
the complete native tail and RELO/IMP round-trip exactly. Generated BIN/RELO/IMP
is **126,612 / 1,788 / 996 bytes**, with an **86,580-byte metadata prefix**.
The root still has **104 selected partial-root fields**: this is not the entire
original root, a general GameObject stream, a playable new-ID package or an alpha
admission expansion. **43** ownership/source/detached/default-profile refusals
pass; the old public signatures and three-behavior default route remain intact.
The stock root has 126 named fields: 22 names remain outside this selected
profile. Its six populated pointer owners now have the separate qualification
above, but are not integrated into this projection. Existing selected root values still also need
original-source qualification. Next: full original root closure, then asset
stream publication and controlled gameplay. Final Focused regression:
**69/69** in **237.026 seconds**; all-module group **15.978 seconds**. Eight
runtime hashes stayed stable; the batch build had zero warnings/errors. No
Full run, game launch or installation change.

The [complete original Conscript Behaviors container](docs/RA3EP1_CONSCRIPT_BEHAVIORS_COMPOSITION.md)
now passes independent stock metadata/native comparison with **all 36 leaves /
24 families / one Core table / 21 strong occurrences / one Tracker**. Its
**17,052-byte metadata prefix** combines 444 bytes of count/item rows with the
16,608 already qualified leaf bytes. BIN/RELO/IMP is **22,672 / 388 / 88 bytes**.
Only generated metadata is joined; the shared native graph is copied once, and
every native pointer/auxiliary slot shifts once. **166 shared metadata/input
refusals** and three authored container scenarios (reverse order, renamed helper,
mixed eight-leaf subset) pass alongside the existing native/leaf gates. Reordered
and subset native graphs independently retain their stock semantic references.
At that milestone the selected root composer was not broadened. The subsequent
explicit opt-in all-module route above now integrates the original Body/AI and
all 49 modules; remaining original root owners, complete dependency publication
and new-ID gameplay remain open. No alpha capability expansion, game or installation change.
Final Focused regression with explicit official XML root: **68/68** in
**236.678 seconds**; complete-container group **7.283 seconds**. Eight runtime
hashes stayed stable; the batch build had zero warnings/errors. No Full run.

All [36 original Conscript behavior metadata graphs](docs/RA3EP1_CONSCRIPT_ALL_BEHAVIORS_TOKENS.md)
now independently match stock across **24 families / 16,608 metadata bytes**.
The schema/native-input generator receives no stock token tables or coordinates;
reviewed wire signatures distinguish field-level reference targets and repeated
types. Independent field-context comparison handles shared stock allocations
without ambiguous global address substitution. The gate checks deterministic
output, unchanged inputs, exact native-tail/RELO/IMP transformation and **250
metadata/input refusals**. Fourteen authored/refusal controls plus one real
Core-normalized, two-upgrade-list control pass. Metadata exposed two additional
KW/EP1 differences: CreateObjectDie has no KW debris header; FireWeaponWhenDead
adds two flags. Their isolated EP1 records leave legacy dispatch unchanged.
The corrected shared native graph is **5,620 / 388 / 88 bytes**, with 21 strong
occurrences. That milestone proved complete **individual leaf** metadata; the
subsequent shared-container gate above now passes too, not a complete GameObject
package. Full
root/Body/AI composition, all-49-module integration and new-ID gameplay remain
open; no alpha admission expansion or installation change. Final Focused
regression with explicit official XML root: **67/67** in **231.180 seconds**;
all-36 metadata group **6.959 seconds**. Eight runtime hashes remained stable;
the batch build had zero warnings/errors. No Full run or game launch.

The earlier [36 original Conscript behavior native gate](docs/RA3EP1_CONSCRIPT_ALL_BEHAVIORS_NATIVE.md)
passed complete stock comparison across **24 families**, both separately and
on **one Core table / 21 strong occurrences / one Tracker**. Its shared native
container was **5,624 / 388 / 88 bytes** (BIN/RELO/IMP). Four isolated EP1 records
fixed SquishCollide, Physics, HitReaction and StatusBitsUpgrade without changing
legacy KW dispatch. Seven complete SlowDeath sources retained every member after
stable family grouping; one actual external definition was explicitly captured.
142 native corruptions, ten authored/refusal controls and nine shared-container/
reference refusals passed. This proved the selected original native graphs, not
arbitrary variants of all 24 families. The subsequent metadata gate above now
passes and strengthens two native layouts that empty/default fields could not
distinguish. Full-root composition and complete GameObject publication remain
open. Final Focused
regression with explicit official XML root: **66/66** in **330.759 seconds**,
all-36 behavior group 7.889 seconds, eight runtime hashes stable. No Full run,
game launch, installation mutation or alpha admission expansion.

The [selected root + complete Conscript client containers](docs/RA3EP1_GAMEOBJECT_CLIENTS_COMPOSITION.md)
now reaches **104 partial root fields / 237 shared Core occurrences** on one
Tracker, retaining all four complete Draws, all six original ClientBehaviors
and the original LargeGroupAudioClientUpdate. Independent stock prefixes remain
**58,608 / 7,404 / 216 bytes**. Thirteen combined source scenarios and 23 native,
eight source and 1,916 detached-output refusals pass. Cross-container local IDs
and a forward behavior-to-client link are checked. This remains a selected
partial root/behavior/AI profile, not the complete original Conscript, an alpha
capability expansion or new gameplay evidence. Final Focused regression with
explicit official XML root: **65/65** in **230.342 seconds**, combined client
group 23.529 seconds, eight runtime hashes stable. No Full run, game launch or
installation mutation.

The [generated Conscript client metadata](docs/RA3EP1_CONSCRIPT_CLIENT_BEHAVIORS_TOKENS.md)
now matches the complete **7,404-byte independent stock prefix** for all six
original client leaves. The native-only-input writer checks allocation/auxiliary
ownership and preserves the one Core table's 48 sound occurrences. Nine authored
token variants cover IDs, range caps, frame lists, order and subsets; 20 malformed/
unreviewed token controls refuse. Populated EvaEvent is blocked in the EP1
marshaler rather than passed through the legacy string-pointer ABI. AudioArraySound
and object-specific/named voice children remain outside populated token admission.
Root/client composition was the next gate at this milestone and now passes above;
whole-object packaging and new-ID gameplay remain open. This is not an alpha
capability expansion. Final Focused regression with explicit
official XML root: **64/64** in **210.929 seconds**, client token group 7.016
seconds, eight runtime hashes stable. No Full run, game or installation change.

All [six complete Conscript client-behavior native graphs](docs/RA3EP1_CONSCRIPT_CLIENT_BEHAVIORS_NATIVE.md)
now match stock on **one Core table / 48 strong sound occurrences**. This exposed
KW/EP1 voice/sound enum drift; separate EP1 records preserve the old KW route.
All 67 voice/39 sound values, five authored variants and 43 refusal controls pass.
The two inherited source leaves are explicitly retained, not skipped. Generated
client metadata was unproven at this native-only milestone; the subsequent
metadata and root/client integration gates above now pass. Audio playback
remains unproven.
Final Focused regression with explicit official XML root: **63/63** in
**210.082 seconds**, client group 7.387 seconds, eight runtime hashes stable.
No Full run, game launch, install mutation or alpha admission expansion.

The [selected root + complete Conscript Draws graph](docs/RA3EP1_GAMEOBJECT_DRAWS_COMPOSITION.md)
now reaches **102 partial root fields / 189 shared Core imports** on one Tracker.
All four complete Draws retain their 58,608-byte independent stock metadata oracle
inside the combined armor/movement/Body/behavior/AI/geometry/vision graph.
Thirteen scenarios and cross-container identity/ownership/metadata refusals pass.
The original Conscript's remaining root/behavior/client owners and complete
object-stream/package validation remain open; no reduced unit is substituted.
Final Focused regression with explicit official XML root: **62/62** in
**238.945 seconds**, root/Draws group 19.762 seconds, eight runtime hashes stable.
No Full run, game launch, install mutation or alpha admission expansion.

The [complete four-module Conscript Draws container](docs/RA3EP1_CONSCRIPT_SHARED_DRAWS.md)
now passes **one Core occurrence table / 173 strong slots**, one Tracker and a
complete independent **58,608-byte** stock metadata prefix. Twelve source cases
cover order/subsets, full module-ID/scalar edits and optional tracer status;
14,682 refusals pass. The subsequent selected root integration is above; original
remaining root/client/behavior owners and publication remain open. The complete
Conscript is still the gameplay target.
Final Focused regression with explicit official XML root: **61/61** in
**225.340 seconds**, shared Draws group 15.610 seconds, eight runtime hashes
stable. No Full run, game launch, install mutation or alpha admission expansion.

Both [complete Conscript scripted-draw metadata graphs](docs/RA3EP1_CONSCRIPT_SCRIPTED_DRAW_TOKENS.md)
now match independent full stock prefixes: **53,052 and 4,512 bytes**. The writer
builds recursive tables from native data, without a stock template. Both full
source graphs also pass new weak module-ID/scalar edits; 66 malformed native and
14,405 detached-output controls pass. Unsupported populated child families fail
explicitly. The subsequent shared four-module Draws gate is above; whole
GameObject publication remains open. This is not an alpha admission expansion
or new gameplay evidence.
Final Focused regression with explicit official XML root: **60/60** in
**265.734 seconds**, token group 14.480 seconds, eight runtime hashes stable.
No Full run, game launch or installed-file change.

Both [complete Conscript scripted-draw native graphs](docs/RA3EP1_CONSCRIPT_SCRIPTED_DRAW_NATIVE.md)
now pass actual Core named-reference and Draw dispatch comparison: **10 model
states, 84 animation states and 171 strong import slots**. This exposed and fixes
the ParticleSysBone interior ABI/id-string error and missing AnimationState NONE
default. The main graph matches stock; the parachute graph has one explicitly
bounded Script CRLF/LF equivalence. No source members are dropped. The follow-up
token and shared-container gates above also verify generated scripted-draw
metadata and Draws composition. Whole GameObject publication remains open. The target
is still the complete Conscript, not a reduced replacement.
Final Focused regression with explicit official XML root: **59/59** in
**217.593 seconds**, scripted-draw group 8.906 seconds, eight runtime hashes
stable. No Full run, game/install changes or alpha admission expansion.

The [two complete Conscript tracer Draw graphs](docs/RA3EP1_CONSCRIPT_TRACER_DRAWS.md)
now match independent stock native/metadata output through real Draw dispatch
and Core texture names. Twenty-four authored combinations check edited tracer
real/frame/color values, all weapon slots, blend flags, optional status and
repeated FX imports. Unknown Draw classes now fail instead of silently vanishing.
The complete Conscript remains the target; scripted/parachute drawing and full
object publication are not claimed by this selected-module milestone.
Final Focused regression: **58/58** in **368.540 seconds**, tracer group
14.725 seconds, eight runtime hashes stable; no Full run or game/install changes.

The [group-audio client metadata fragment](docs/RA3EP1_GROUP_AUDIO_CLIENT_TOKENS.md)
matches a complete independent stock metadata/native graph. Twelve real-dispatch
cases cover ordered 1/2/32/64-key lists and raw frame/ushort values; 15 native,
53 detached-output and 8 source/boundary refusals pass. This does not enable
audio playback, shared ClientUpdates composition or whole GameObject publication.
Final Focused regression: **57/57** in **289.857 seconds**, client group
8.673 seconds, eight frozen runtime hashes stable; no Full run or game/install changes.

The first new-ID GameObject gameplay target and its three remaining delivery
gates are tracked in the [progress snapshot](docs/RA3EP1_PROGRESS_SNAPSHOT.md#first-new-id-gameobject-gameplay-gate).
The target remains the **complete Conscript graph**, not a reduced substitute,
and reuses stock art; completing the entire SDK is not a prerequisite.

The [timeout duration correction](docs/RA3EP1_GAMEOBJECT_DURATION.md) restores
the stock/schema default: 31000 source milliseconds become 155 native logic
frames, rather than the old missing-attribute fallback 0. Ten full shared AI/root
cases and eleven refusals pass. The new descriptor raises the selected root to
**101 partial fields**; generic Duration conversion and alpha admission are unchanged.
Final Focused regression: **56/56** in **368.721 seconds**, duration group
10.010 seconds, eight runtime hashes stable; no Full run or game/install changes.

The [shared inline AI projection](docs/RA3EP1_GAMEOBJECT_AI_COMPOSITION.md) now
joins common AI with armor, movement, Body, three behaviors, geometry and pointed
root children on one Tracker/Core occurrence table. Its 864-case stock-target
test reaches **100 partial root fields**, checks AI counts 0/1/2 and extends local
identity scope across AI/Body/Behaviors. A new-armor variant exercises the same
path. This is not complete GameObject stream publication or an alpha expansion.
Final Focused regression: **55/55** in **423.644 seconds**, stock/authored AI
groups 26.400/26.070 seconds, eight runtime hashes stable, no game/install changes.
Full-product planning estimate: **about 48% (40–50% range)**; see the transparent
[maturity breakdown](docs/RA3EP1_PROGRESS_SNAPSHOT.md), not an asset-coverage score.

The [generated AI fragment follow-up](docs/RA3EP1_AI_UPDATE_TOKENS.md) matches two
complete independent stock metadata prefixes, with 348/552-byte generated layouts
for absent/present chooser graphs. Empty-contact variants cover all 680 selected
state/locomotor/chooser/string combinations. Nonempty contact/vision list tokens
still refuse; shared AI/root composition and general publication remain open.
Final Focused regression: **53/53** in **390.651 seconds**, AI token group
28.998 seconds, eight runtime hashes stable and no game/installation changes.

The [common AIUpdate native port](docs/RA3EP1_AI_UPDATE_NATIVE.md) replaces the
wrong KW concrete layout with separate 76-byte EP1 AI and 40-byte chooser models.
Two complete selected stock native graphs and 680 state/locomotor/chooser/string
cases pass actual Core name/default normalization. Inline AI counts 0/1/2 use real
dispatch and shared reference occurrences. Generated AI metadata, nonempty vision
filters and integration with the selected combat projection remain open; the
partial token root stays at 99 fields and the alpha admission gate is unchanged.
Final Focused regression: **52/52** in **336.633 seconds**, AI group 22.782
seconds, eight runtime hashes stable and no game/installation changes. Final
ASCII/list/finite/boolean guards and their source hashes are verified; no Full run.

The [EP1 vision correction](docs/RA3EP1_VISION_INFO.md) restores the missing
ShroudClearingHeight word through a separate 40-byte model. Three complete stock
native/metadata graphs match; 576 actual-Core combinations join optional vision
with all prior pointed roots and selected movement/combat/geometry owners. The
largest partial root reaches 99 fields. General publication and alpha admission
are still unfinished; this does not claim new gameplay evidence.
Final Focused regression: **51/51** in **378 seconds**, vision group 18.408
seconds, eight runtime hashes stable and no game/installation changes. This also
rechecks the corrected unsigned Flammability metadata; no Full-suite run is claimed.

The [pointed root child follow-up](docs/RA3EP1_POINTED_ROOT_CHILDREN.md) adds
DisplayName/DisplayNameTransformed, Flammability and LiveCameraOffset to the shared
selected graph. Both stock-target and newly built armor-target modes exercise 288
combinations. The largest partial root reaches 98 fields; positive stock names are
verified, while nonnull flammability/camera still need independent stock evidence.
General GameObject publication and alpha admission remain separate gates.
Final Focused regression: **50/50** in **309.755 seconds**, eight runtime hashes
stable, no game/installation changes. Pointer stock/authored groups take
20.651/13.238 seconds; this is not a Full-suite or runtime acceptance result.

The [inline root child correction](docs/RA3EP1_INLINE_ROOT_CHILDREN.md) fixes
Buildable, ThingClass and DisplayColor: these are inline enum/packed-color words,
not allocated pointers. Five stock direct-target contracts and 257 authored/default
cases pass; the root remains 600 bytes with no extra allocations. Three new flat
descriptors raise the selected full combination to 94 partial root fields. Nonzero
stock/runtime behavior and general publication remain separate gates.
Final Focused regression: **48/48** in **127.251 seconds**, inline group 3.515
seconds, eight runtime hashes stable and no game/installation changes.

The [shared geometry/movement/combat projection](docs/RA3EP1_GAMEOBJECT_GEOMETRY_COMPOSITION.md)
now combines all selected owners on one real Tracker/Core occurrence table,
including a genuinely new compiled armor target. Stock/authored modes each cover
73 source combinations, optional geometry/movement, all six behavior orders,
strings, ranges, weapons and placement/contact graphs. The partial root reaches
91 reviewed fields; complete GameObject publication and alpha expansion remain open.
Final Focused regression: **47/47** in **125.413 seconds**, stock/authored geometry
groups 4.158/4.487 seconds, eight runtime hashes stable, no game/install changes.

The [generated geometry fragment](docs/RA3EP1_GEOMETRY_TOKENS.md) now matches every
metadata prefix byte in five independent stock examples. Native ownership,
placement/shapes/contacts, direct vector targets and pointer rebasing are explicit;
33 native and 40 detached metadata/payload corruptions refuse. Root composition
and complete GameObject publication remain separate unfinished gates.
Final Focused regression: **45/45** in **121.457 seconds**, eight runtime hashes
stable and no game/installation changes.

The [EP1 geometry native port](docs/RA3EP1_GEOMETRY_NATIVE.md) corrects the missing
PlacementShape pointer: a 28-byte header replaces the legacy root geometry layout,
with a separate RING-aware Shape model. Five selected stock graphs match complete
native records/payloads; 24 authored cases cover placement, rotation, strings,
contacts, enums and defaults through standalone and real root dispatch. Geometry
metadata/composition/publication remains a separate gate; the alpha is unchanged.
Final Focused regression: **44/44** in **117.804 seconds**; geometry group 3.389
seconds, eight runtime hashes stable and no game/installation changes.

The [selected module identity gate](docs/RA3EP1_SELECTED_MODULE_IDENTITY.md) now
checks Body/behavior hash uniqueness and destruction AssociatedModule closure
before allocation and independently in native projection. Its 25 valid cases
and source/native refusals cover all six module orders, forward/backward/case-folded
links and per-object scope. This remains a strict partial-object profile, not a
general local-ID/inheritObject resolver or alpha expansion.
Final Focused regression: **43/43** in **118.130 seconds**; identity group 3.528
seconds, eight runtime hashes stable and no game/installation changes.

The [movement/combat follow-up](docs/RA3EP1_MOVEMENT_COMPOSITION.md) adds scoped
root LocomotorSet native data and generated count/member metadata to the shared
armor/body/three-behavior graph. Four stock movement arrays match native output;
69 source/composition cases cover all twenty condition names, 0/1/3/32 records,
named Core references and dynamic allocations. Present movement raises the partial
root inventory to 90 fields; no complete object publication or alpha expansion.
Final Focused regression: **42/42** in **116.493 seconds**, eight runtime hashes
stable, no game or installed-file changes.

The [SMC helper writer and three-module composition](docs/RA3EP1_SMC_COMBAT_COMPOSITION.md)
now adds the required helper to destruction and EP1 weapon behaviors. Both selected
stock helpers match all 8 native/168 metadata bytes. All six behavior orders pass
48 actual-Core source/value/list cases, with a separately compiled new-armor mode.
The 620-byte minimal native helper graph also composes. This closes selected
helper tokenization/integration, not remaining movement/draw/geometry or full
GameObject publication. Root field count stays 89 with ArmorSet/Body present.
Final Focused regression: **41/41** in **115.529 seconds**; stock/authored three-module
groups 3.960/4.590 seconds, eight runtime hashes stable and no game/install changes.

The [selected combat GameObject composition](docs/RA3EP1_GAMEOBJECT_COMBAT_COMPOSITION.md)
now joins armor, Body and one/two destruction/EP1 weapon behaviors in either
order, using explicit native allocation and one actual Core reference table.
Sixteen source/value/list cases plus a single-weapon case pass; an authored-armor
variant uses a genuine new Core stream and refuses when that target is removed.
Root metadata remains 89 fields with Body/ArmorSet present. Legacy dispatch,
general source/asset publication and the alpha remain unchanged.
Final Focused regression: **38/38** in **104.923 seconds**; stock/authored combat
groups 3.610/3.960 seconds, eight runtime hashes stable, no game/install changes.

The [EP1 weapon-set hardpoint port and token writer](docs/RA3EP1_WEAPON_SET_FRAGMENT.md)
now reproduces the complete selected Peacekeeper/Conscript native graphs and
generated token prefixes (152/480 and 232/588 bytes). Separate 44/28/80-byte
models preserve forbidden-status, auto-choice and metrics fields missing in KW.
Actual Core weapon/upgrade names, five authored list shapes, sixteen input and
54 metadata refusal controls pass. Legacy dispatch, general turret support and
enclosing multi-behavior/GameObject publication remain unchanged.
Final Focused regression: **36/36** in **143.162 seconds**; weapon-set group 9.575
seconds, eight runtime hashes stable and no game/install changes.

The [combined ArmorSet writer and newly authored consumer](docs/RA3EP1_GAMEOBJECT_ARMOR_COMPOSITION.md)
now joins 0..32 ordered armors, optional DamageFX and literal condition masks to
the root/destruction/ActiveBody projection. Sixteen stock-target cases and the
same sixteen cases referencing a real Core-compiled RebornComposedArmor pass;
removing the new target catalog refuses rather than falling back to stock.
Both-present output has **89 root fields**, with earlier 88/87-field paths retained.
This does not publish a consuming GameObject or expand the alpha.
Final Focused regression: **35/35** in **136.888 seconds**; stock/authored consumer
groups 5.950/6.470 seconds, eight runtime hashes stable and no game/install changes.

The [root/destruction/EP1 body composition](docs/RA3EP1_GAMEOBJECT_BODY_COMPOSITION.md)
now joins all three native owners and their generated metadata in one output.
An explicit EP1 body allocator preserves legacy KW dispatch. Eight actual-Core
cases combine dynamic root strings and 0/1/3/64 body ranges with shared named
imports. Body-present output has **88 root fields**; absent-Body output retains
87 and its previous layout. Eleven owner faults and 188 metadata corruptions
are refused. This is partial GameObject composition, not a published asset,
general source compiler or expanded alpha.
Final Focused regression: **33/33** in **90.472 seconds**; combined body group
3.617 seconds, all eight runtime hashes stable and no game/install changes.

The earlier [EP1 ActiveBody native/token-fragment port](docs/RA3EP1_ACTIVE_BODY_FRAGMENT.md)
adds a separate 76-byte layout and actual metadata writer without overwriting KW
models/dispatch. Two selected stock bodies match completely; authored Core cases
cover four parent imports and 0/1/3/64 range modifiers, with percentage/time/flag
checks, eleven input and sixty output-metadata refusals. That milestone was a
standalone fragment; the follow-up above adds root composition, not a complete
GameObject or expanded alpha.
Final Focused regression: **32/32** in **87.900 seconds**; body group 9.319 seconds,
eight runtime hashes stable and no game/install changes.

The [named root/module source gate](docs/RA3EP1_GAMEOBJECT_ROOT_NAMED_SOURCE.md)
now normalizes UnitTypeIcon/UnitIntro and repeated destruction FX names in one
actual Core reference table. Eight plain/typed presence cases bind to independent
real stock identities; twelve source/catalog/semantic failure controls pass.
The supplied catalogs have no CrowdResponse targets, so that field has explicit
missing-target evidence rather than a fabricated positive. This remains a focused
diagnostic path: target payload compilation, complete GameObject and alpha
publication remain open. Root metadata stays at 87 fields.
Final Focused regression: **31/31** in **84.376 seconds**; new named-root group
3.417 seconds, all eight runtime hashes stable, no game/install changes.

See the [large-family and smaller-subsystem capability inventory](docs/RA3EP1_CAPABILITY_MATRIX.md)
for what is alpha-enabled, experimental or still open. No whole major family is
yet unrestricted EA-SDK parity; overall planning maturity is now about 47%
(40–50% range), not a percentage of build-enabled asset types.

The [root strong-import slot follow-up](docs/RA3EP1_GAMEOBJECT_ROOT_STRONG_SLOTS.md)
adds CrowdResponse/UnitTypeIcon/UnitIntro storage ownership and token descriptors,
bringing the partial projection to **87 root fields including Behaviors**.
Root imports are no longer incorrectly included in module-only IMP subtraction;
shared dependency indices survive root/module composition and moved strings.
Eight presence combinations and ten ownership refusals pass. These new tests use
diagnostic normalized indices, not actual Core root-name resolution or target
payload compilation. General GameObject and alpha publication remain closed.
Final Focused regression: **30/30** in **83.388 seconds**, including 172 metadata
corruption controls; eight runtime hashes remain stable.

The [root-string/dynamic-allocation batch](docs/RA3EP1_GAMEOBJECT_ROOT_STRINGS.md)
adds six string headers, bringing generated root metadata to **84 fields**.
Behavior/module addresses are decoded instead of fixed at 600/608/612. Omitted,
empty, ASCII/XML-escaped and 4096-character strings preserve pointer and auxiliary
ownership, including integration with actual Core named FX imports. General
encoding, strong root references and complete GameObject/alpha publication remain
outside this partial projection.
Final Focused regression: **30/30** in **83.366 seconds**; 37 malformed string
cases and 166 metadata corruptions are refused, with eight runtime hashes stable.

The earlier [enum/flag/weak-ID batch](docs/RA3EP1_GAMEOBJECT_ENUM_FLAG_WEAK.md) adds 15
root fields, bringing generated metadata to **78 fields including Behaviors**.
GameObject-specific EP1 radar/editor/production enum mismatches are corrected;
50 enum literals, 291 KindOf bit positions and seven actual-Core weak identities
are checked. Three stock roots pin the new field contracts and selected enum
values; four real stock weak IDs are corroborated. Weak-ID conversion is not
target existence or payload support. The root projection is still incomplete
and does not expand the alpha package.
Final Focused regression: **30/30** in **81.046 seconds**, including 154 root-value
metadata refusals; eight runtime hashes remain unchanged during workers.

The earlier [scalar-root batch](docs/RA3EP1_GAMEOBJECT_SCALAR_TOKENS.md) widens generated
root tokens to reviewed float/integer/time/angle and packed ushort/byte/bool
families plus Behaviors. Every scalar descriptor is checked against two stock
roots, and two all-field authored sets exercise schema/native/token conversion.
Root/container/module coordinates now scale with the emitted field count.
This is still a partial root projection, not general GameObject publication.
The batch emits **62 scalar fields + Behaviors = 63 root fields**, with 124
independent scalar type/target corruption controls. This is not SDK percentage.
Final Focused regression: **30/30** in **80.569 seconds**; all eight runtime hashes
remain unchanged during workers. The selected composed BIN is now 1900 bytes.

The earlier [GameObject destruction projection](docs/RA3EP1_GAMEOBJECT_DESTRUCTION_PROJECTION.md)
now joins BuildTime/Behaviors root tokens, the behavior container and generated
module/Item tokens in one address space. Native graph/import indices survive
global pointer and auxiliary-slot rebasing. **Only two root fields are emitted**;
this is not a complete or publishable GameObject asset and does not expand alpha.
Final Focused regression: **30/30** in **81.998 seconds**, including composed
stock/native joins, authored 1/2/3/256-item cases and nine projection refusals.

The [destruction token writer](docs/RA3EP1_DESTRUCTION_TOKEN_WRITER.md) now generates
the module fragment's inheritance/inline/list token graph from native compiler
data, without copying stock prefixes. Selected output is independently compared
with stock; native pointer/RELO/IMP round trips, deterministic repeats and authored
multi-item cases are checked. This advances writing, not only stock validation.
The writer is not yet composed into a general GameObject stream or exposed by
the alpha registry/Studio package.
Final Focused regression: **30/30** in **109.505 seconds**; stock-generated token
comparison, native round trips, 1/2/3/256-item cases and refusal controls pass.

The [destruction inheritance/DieMux checkpoint](docs/RA3EP1_DESTRUCTION_BASE_TOKENS.md)
adds explicit base-class marker/type/table checks, terminal TypeId/id anchors and
all eight selected inline DieMux field relationships. Empty veterancy and absent
status storage are independently checked. Detached corruptions exercise the same
comparison used by native and actual Core source tests. This is selected stock
token validation, not general token generation or expanded alpha support.
Final Focused regression: **30/30** in **96.211 seconds**, including **40**
inherited/inline corruption refusals; eight runtime hashes remained unchanged.

The selected stock destruction Item now has explicit token/native checks for BonePrefix, FX, CreationObject and FlingVelocity, including their schema type IDs and native offsets. A detached-copy corruption test confirms that a shifted BonePrefix token target is rejected. The manual Focused regression passed 30/30 groups. This strengthens selected nested-token validation; it does not enable general GameObject tokenized publication or expand alpha capabilities.

The [destruction source walkthrough](docs/RA3EP1_DESTRUCTION_SOURCE_WALKTHROUGH.md)
now demonstrates actual Core normalization of named FXList references and official
defaults, with a complete selected stock native graph/dispatch/import-identity
comparison. Three authored items check ordered occurrence imports and repeated
targets; missing/wrong/ambiguous dependencies and authored numeric indices fail.
This replaces captured FX input indices **for this selected source gate**, not
general GameObject source closure, FX payload compilation or tokenized publication.
Alpha h is unchanged. The walkthrough includes the exact files, commands and
comparison criteria needed to reproduce this engineering step.
Final Focused regression: **30/30** groups in **90.503 seconds**; the new source
gate took **3.967 seconds**, with eight runtime hashes unchanged during workers.

The [Allied module batch](docs/RA3EP1_ALLIED_MODULES_NATIVE.md) adds destruction,
tech-upgrade and LUA-event-list native models/marshalers and real behavior dispatch.
Four selected complete stock native graphs match. Ordered/default/edited source
controls and invalid-input checks are included. Destruction FX still uses an
explicit captured-index diagnostic; LUA callback execution is not proven.
There are no remaining missing native model names in the original two-object
inventory, but existing-model EP1 review and recursive tokenized GameObject
publication remain open. Alpha h is unchanged.
Final focused regression: **29/29** groups in **85.380 seconds**; the new family
gate took **7.141 seconds**. Eight runtime hashes remained unchanged during workers.

The [toggle dispatch milestone](docs/RA3EP1_TOGGLE_DISPATCH.md) now verifies
five complete selected stock/Core module graphs, including Dreadnought's Module
record and Gigafortress's geometry strings. Actual GameObject polymorphic native
dispatch/class ID and shifted relocation/import slots match. Canonical literal
fixture ordering and the official 4.0s value are explicit, not generic source
preprocessing. General GameObject tokenized publication and alpha h remain closed.
Final focused regression: **28/28** groups in **87.182 seconds**, with eight
runtime hashes unchanged; source/dispatch checks took **33.614 seconds**.

The earlier [toggle source/Core checkpoint](docs/RA3EP1_TOGGLE_SOURCE_NATIVE.md)
normalizes the three complete selected modules through actual Core, without
captured source indices. Independent static/global lookup resolves audio families
and all strong dependencies; complete selected native graphs/import identities
match. Authored Module/geometry lists, local-ID context and schema/reference
refusals pass. This is a focused source/reference seam, not full GameObject
document build or publication; polymorphic dispatch and alpha h remain unchanged.
Final focused regression: **28/28** groups in **113.314 seconds**; new source
checks took **17.441 seconds**, with eight runtime hashes unchanged during workers.

Full-target planning estimate: **about 47% (40–50% range)**, not a test-pass or
asset-coverage ratio. The limited local alpha already exists. See the
[remaining-work snapshot and weighting](docs/RA3EP1_PROGRESS_SNAPSHOT.md).

The [direct toggle module checkpoint](docs/RA3EP1_TOGGLE_MODULE_NATIVE.md) adds
native state/module models and marshalers. Three selected complete native graphs
match stock, including the special-ability base and empty-string relocation.
References currently use captured-index diagnostics; normal source normalization,
nonempty child-list stock comparison and polymorphic dispatch remain separate.
This is implementation progress, not general GameObject publication in alpha h.
Final focused regression: **27/27** groups in **66.833 seconds**, with eight
runtime hashes unchanged. No new Full-suite run is claimed for this checkpoint.

The [standalone toggle AI implementation](docs/RA3EP1_TOGGLE_AI_NATIVE.md)
adds native AI/weapon models and marshalers. Six selected stock AI graphs match,
including four present optional pointer/value joins, absent weapon children,
flags/defaults and actual Core-normalized weapon identities/imports. Authored
controls exercise all thirteen optional allocation slots and edited XML values.
Nine present optional layouts remain without stock corroboration. Enclosing
toggle dispatch and GameObject publication are still closed; alpha h is unchanged.
Final Full regression: **28/28** groups passed with all eight runtime hashes
unchanged during workers. New AI checks took **10.017 seconds**; the existing
broad compiler suite dominated the **1087.326-second** total.

The [toggle-status layout checkpoint](docs/RA3EP1_TOGGLE_STATUS_LAYOUT.md)
verifies the existing 252-byte special-ability base's field offsets and two
304-byte stock toggle-state records, including inline AI, separate weapon
pointers and empty child-list anchors. Nine detached corruption controls pass.
This establishes the next module's layout prerequisite, not a new compiler
capability: present optional AI values, references and marshaler payloads still
need validation. Alpha h and the four observed missing model families are unchanged.
Final focused regression: **25/25** groups in **55.667 seconds**, with eight
runtime hashes unchanged during workers. This is not a new Full-suite run.

The [damage/motion batch](docs/RA3EP1_DAMAGE_MOTION_MODULES.md) adds
SecondaryDamageTracker and FreefallUpdate with real polymorphic dispatch.
Two complete selected tracker graphs and one freefall payload match stock,
including color, ordered effects, packed options/bools and optional status masks.
Plain modifier names pass through actual Core normalization and resolve to the
same stock identities; native import/relocation slots are checked. The observed
missing model families in the two reviewed objects fall from six to four.
GameObject tokenization/publication and alpha h remain unchanged.
Final Full regression: **26/26** groups pass, with all eight runtime-file hashes
unchanged during workers; the detailed damage/motion report records exact scope.

The [first module-port batch](docs/RA3EP1_BASIC_MODULE_PORTS.md) implements
ObjectSMCHelper, StructureSellUpdate and LargeGroupAudioClientUpdate models,
marshalers and real polymorphic dispatch. Four stock comparisons verify complete
selected payloads plus audio key data; edited IDs/scalars/key order and a combined
three-family native graph pass. This reduces the missing native model families
observed in the two reviewed objects from nine to six, not the entire SDK backlog.
General GameObject tokenization/publication and alpha h remain unchanged.
Final Full regression passes **25/25** groups, including the broad compiler
suite; all eight runtime hashes remain unchanged during workers.

The [module/AI follow-up](docs/RA3EP1_GAMEOBJECT_MODULE_TOPOLOGY.md) resolves the
previous +484 mystery: EP1 AI is an inline count/items list at +480/+484, not a
single AI pointer followed by UnitSpecificFX. The native model and marshaler are
corrected without moving Body (+488) or changing the 600-byte root. Actual 0/1/2
AI builds and relocation/ID/scalar controls pass; excess/unknown AI is refused.
Six container topologies cover 93 stock module instances in two objects, with
nine missing native model families explicitly listed. Final focused regression
passes **22/22** groups; the broader compiler suite also passes against identical
shared assembly hashes. Full module payload/tokenization/mixed publication and
GameObject input in alpha h remain closed.

The new named-reference follow-up in the [GameObject armor-consumer slice](docs/RA3EP1_GAMEOBJECT_VERTICAL_SLICE.md)
normalizes plain XML armor/DamageFX names through the actual Core reference
handler, resolves stock and genuinely Core-built custom armor identities, and
checks native import slots without captured numeric indices. A focused schema
drift-checks the selected official declarations; this is not full official
GameObject schema validation. Fourteen refusal controls and repeat/edited-source
checks pass. The final batch passes **20/20** groups in **37.272 seconds**, plus
a **5.31-second**, zero-warning/error build. GameObject publication and the h
alpha remain unchanged. Next: complete module/native/token graph, then a mixed
GameObject/armor Core stream, before exposing new consumers in Studio.

The earlier [GameObject armor-consumer slice](docs/RA3EP1_GAMEOBJECT_VERTICAL_SLICE.md)
now proves the stock root/token/native/dependency chain for Conscript and Allied
Construction Yard. The existing marshaler reproduces their selected ArmorSet
records and BuildTime, with exact relocation/import positions. Fifteen synthetic
corruption controls accompany the two stock comparisons. This is a bounded native
projection using captured reference indices, not complete GameObject/XML/Core
publication. The h alpha is unchanged; GameObject input remains unsupported there.

Armor inheritance follow-up: **distinct-Damage list append already works** in
the h source route. Imported intermediate and owner lists preserve
base-then-derived order; a local chain also passes. Independent literal package
equality and changed-coefficient controls pass, with **18/18** focused groups in
39.504 seconds and unchanged runtime hashes. The exported h compiler builds the
three-file [append example](samples/armor/source-append/mod.xml) unchanged.
This corrects an overly narrow copy-only description; duplicate Damage and
Damage-key replacement remain refused, and no new gameplay proof is claimed.

The source-enabled local alpha `UprisingAlpha-20261013-h` now connects the general
armor source bridge to the existing Studio build driver. Its new `ArmorSource`
sample combines Includes, `$` definitions, inheritance and custom IDs in one
real Core/package build, with explicit ALPHA004 experimental warnings. Eight
samples pass in a source-tree-free workspace and fresh ZIP extraction; the seven
old golden package hashes remain unchanged. Exact-reference CLR4 script loading
passes without opening Studio. The new package has no gameplay proof or automatic
GameObject consumers. See the [source integration report](docs/RA3EP1_ALPHA_SOURCE_INTEGRATION.md)
and [alpha authoring guide](alpha/README.md) for limits and the next user GUI test.

The new [armor source bridge](docs/RA3EP1_ARMOR_SOURCE_BRIDGE.md) connects confined
instance Includes, definition-only all Includes, local/imported inheritance and
exact `=$NAME` field substitution to the real general armor Core/package path.
It preserves defining-source contexts and publishes only owner-declared armors.
The literal single/set commands and preserved g alpha remain unchanged; arbitrary
arithmetic and duplicate-damage inherited list replacement are not enabled.
Final source-bridge regression passes **18/18** focused groups in **43.618
seconds**, plus a 5.33-second incremental compiler build with zero warnings/errors.
The four-file source package equals the independent literal package byte-for-byte;
repeat/changed-input controls and nineteen pre-output refusals pass. All eight
compiler/runtime hashes stayed unchanged. Full was not rerun for this adapter;
Core/native/profile implementations were unchanged.

October 13 update: the [general armor backend](docs/RA3EP1_GENERAL_ARMOR_AUTHORING.md)
now accepts user-chosen IDs, variable 0..64-entry unique damage lists, decimal/%
literal percentages and official scalar defaults. Real Core/scoped streams,
two-reader readback and default-reader candidates are produced without another
hard-coded unit recipe. The explicit set command also accepts 1..32 user-defined
armors in one real Core stream, rejects duplicate names/lookup IDs and validates
every member's cumulative BIN/RELO/IMP slice through two readers.
The literal commands still refuse inheritance/includes/definitions/mixed types;
the source command above adds a separate bounded preparation path. The exported
g alpha is unchanged; the new h alpha adds the separate source adapter. New IDs
need consuming GameObjects before they can visibly affect gameplay.
Earlier single-armor focused regression: 16/16 groups pass in about 26 seconds, plus a roughly
5-second incremental compiler build; existing armor/weapon behavior stays covered.
Final multi-armor code passes **17/17** focused groups in **32.522 seconds**
(27.08-second compiler build, zero errors and 54 analyzer warnings).
The set group covers 1/2/3/13/32 members, custom/historical pairs, checksum
capacity boundaries and corruption controls in 14.956 seconds. The initial
expanded Full run passed 19/19 before the final routing/checksum refinements;
see the linked backend report for exact evidence and rerun boundaries.

The [batched offline test workflow](docs/RA3EP1_BATCH_TESTING.md) now builds once,
runs 24 focused groups with isolated concurrent workers and aggregates failures
without stopping the remaining groups. `-Full` adds broader compiler/armor
regressions. Source-semantic diagnostic passes do not expand playable alpha
capabilities or imply mixed-stream support.
The final two-worker focused run passes 15/15 groups in about 28 seconds
excluding the compiler build; all eight compiler/runtime file hashes remain
unchanged. Failure-continuation and occupied-output controls also pass.

The next type is a [bounded Conscript weapon vertical slice](docs/RA3EP1_WEAPON_VERTICAL_SLICE.md).
Read-only stock review now verifies AK47 root scalar token/native relationships,
selected DamageNugget class/list/GUN=5 relationships, direct reference names
through static/global metadata, repeat output and corruption refusals.
The real marshaler now reproduces all 528 normalized native AK47 BIN bytes
from reference XML, after correcting an EP1 default, DamageNugget field order
and DamageFX enum order. A fixed diagnostic token-layout reproduction also
matches all 2508 stock BIN bytes and 28 RELO bytes, verifies an isolated
Damage=5→50 edit and seven corruption refusals. This does not enable generic
tokenization or alpha weapon publication.
The focused weapon proof now validates official EP1 type definitions, inserts
schema defaults and obtains reference indices through Core normalization.
Both baseline and Damage=50 XML retain the exact stock-byte proof. Eight
schema/control/dependency refusals pass. A new private document diagnostic now
loads the authored weapon through DocumentProcessor, prepares actual external
dependencies and dispatches a checked weapon plugin. Its stock baseline,
repeat, isolated Damage=50 edit and restored source/hash pass, together with
four additional refusal controls. A separately opted-in scoped OutputManager
now writes the actual intermediate asset and linked v7 manifest/BIN/RELO/IMP;
two readers verify ordered dependencies and runtime external paths. Baseline
repeat, Damage=50 hash/checksum/payload invalidation and nine scoped refusal
controls pass. This is diagnostic stream output, not a playable package or
expanded alpha support. A separate `weapon-package-probe` now prepares baseline
Damage=5 and edited Damage=50 default-reader candidates: fifteen exact BIG
entries, three blank-CRLF version markers, matching config/mod-SKU fragments,
two-reader metadata and ten package refusal controls pass. Repeated baseline
packages are byte-identical. Stock archives are not bundled or replaced.
Independent runtime external-reference loading evidence remains unavailable;
the diagnostic command itself does not change the exported alpha.
The user has now authorized one Damage=50 candidate launch. Strict preflight
passes and the 30-second observer completes without error, with 37 matching
1.1 child config-argument snapshots. In response to the requested fresh-Soviet
skirmish/normal-AK47 comparison, the user reports increased damage. This is the
first positive behavioral observation for the exact Damage=50 weapon candidate,
not a measured multiplier, config-read trace or general WeaponTemplate proof.
Bounded editable weapon authoring and alpha integration now pass offline,
retaining strict unsupported-type/identity/shape errors and armor regressions.
The selected-input backend is now implemented: `weapon-authored-build` admits
the complete reviewed AK47 XML as standalone or a one-member declaration, with
only Damage editable (integer 0..10000, optionally `.0`). Actual Core stock-byte
comparison, isolated edit, scoped streams and verified default-reader packaging
pass for 0/5/50/73/10000, with byte-identical repeated 73 output and eighteen
input refusal controls. No automatic installation or launch occurs. The new
BuildStudio adapter/export dispatches ConscriptAK47 through this path with an
explicit GlobalManifest setting and ALPHA003 scope warning. Seven sample builds,
fresh ZIP extraction, external-dependency overlay setup and relocation pass;
the exact-reference CLR4 loader/options test passes. Actual updated GUI build
interaction remains unobserved. See [selected weapon authoring](docs/RA3EP1_WEAPON_VERTICAL_SLICE.md#selected-xml-weapon-authoring-backend).
Generic weapon tokenization, dependency closure and arbitrary IDs/shapes remain
unsupported; the alpha accepts only this complete AK47 shape with Damage edits.

The [diagnostic compatibility contract](docs/RA3EP1_SDK_RELEASE_PLAN.md)
separates current alpha errors from planned general XML/schema, definition,
W3X/reference and post-build integrity checks. The modified reference SDK's
hash-fix chain is not blindly executed against Uprising output.

Legacy fixed armor authoring remains bounded: at most one AlliedConYardArmor
and one SovietConYardArmor per build, thirteen ordered damage entries each plus
five optional root scalars, canonical integer percentages 0..10000. New IDs,
changed list shapes and `inheritFrom` are rejected. Existing generic inheritance
diagnostics alone do not enable inheritance in that legacy path. The new
experimental h source route supports the reviewed inheritance subset above.

The first narrow ArmorTemplate override now works in-game by user observation:
AlliedConYardArmor GUN 100 → 5000 caused markedly increased damage with the
default `mod.manifest` loading recipe. This is not yet a complete playable
Uprising SDK. A [one-command fixed armor reproducer](docs/RA3EP1_ARMOR_MOD_BUILD.md)
now combines compilation, stock comparisons and the validated package recipe.
Editable armor XML now reaches actual Core/OutputManager compilation and
verified default-reader packaging, rather than replaying a fixed fixture.
The [expanded armor authoring path](docs/RA3EP1_ARMOR_EXPANDED_AUTHORING.md) now
admits thirteen ordered coefficients and five optional scalars (integer 0..10000)
for the same AlliedConYardArmor identity. All eighteen fields pass offline
compile/package comparisons. The separately authorized ArmorCannon trial now
also has user-reported markedly increased damage with CANNON=5000 and GUN=100.
The user also confirmed normal Peacekeeper damage with GUN=100, providing the
unchanged damage-type control. Other coefficients and root scalars remain
offline-only evidence.
The [second armor identity](docs/RA3EP1_SOVIET_ARMOR_AUTHORING.md),
SovietConYardArmor, now also passes its own stock/Core/package comparisons,
eighteen field edits, repeat and cross-identity refusal. The
[combined armor path](docs/RA3EP1_ARMOR_PAIR_AUTHORING.md) now builds one of each
together into the same actual Core stream. Duplicate/third assets remain refused.
The reference XML also binds Japan's yard and one civilian
building to this shared armor. The separately authorized Soviet GUN=5000 trial
now also has user-reported increased damage with the requested Conscript test.
Japan/civilian consumers and foreign-identity gameplay controls remain untested.
Previous local candidate: `UprisingAlpha-20261012-g`, with seven offline-verified
samples including CombinedArmor, CombinedStrong and ConscriptAK47. Its fresh overlay extraction
also passes; the new workspace GUI has not yet been observed.
The combined package now has user-reported markedly increased Conscript damage
for Soviet GUN=5000. Allied Peacekeeper GUN=250 remains inconclusive: the user
was unsure of its stock damage and observed no exaggerated increase. Both
members are not yet independently confirmed in-game within this package.
The fixed `Invoke-Ra3Ep1ArmorTrial.ps1 -Combined` launch preparation now passes
its no-game preflight and rejects conflicting selectors. One authorized combined
launch completed without an observer error; the separate stronger comparison
below now supplies positive Allied evidence.
A separate repository sample, `CombinedArmorStrong`, now passes offline builds
with Allied/Soviet GUN=5000/5000. Only Allied GUN differs from the preserved pair.
Its pinned `-CombinedStrong` launch now has user-reported increased Peacekeeper
damage in the requested Allied test. Thus both identities have positive gameplay
observations in two-member streams, across two different package revisions.
Soviet was not re-tested in the 5000/5000 revision; Allied GUN=250 remains
inconclusive, and no exact damage multiplier is claimed.
The repository source sample is exported as `CombinedStrong` to satisfy Studio's
unchanged 15-character mod-name bound. Both pair samples are in the e workspace.
The e overlay has 30 payload files and eight external EA/schema dependencies;
private-path auditing, fresh setup, six builds and failure diagnostics pass.
It remains a local candidate with external prerequisites and outstanding public
redistribution review, not a complete standalone SDK.
The refreshed local workspace `UprisingAlpha-20261011-h` and clean overlay
extraction pass all three samples (Reborn, Armor250, ArmorCannon); the full
compiler regression suite also passes. Packaging now forces recompilation to
prevent stale managed debug paths. These are local candidates, not a public release.

Release plan: [Armor Preview versus general SDK gates](docs/RA3EP1_SDK_RELEASE_PLAN.md).
The first alpha targets the existing EA Mod BuildStudio interface with explicit
limited-capability warnings, not broad feature parity before release. An
[EA-interface adapter prototype](docs/RA3EP1_ALPHA_STUDIO_ADAPTER.md) compiles
against the actual loader's reference set. An isolated local workspace and
fresh ZIP extraction both pass offline builds without the repository source tree.
The user's real Studio screenshot and full log now confirm Reborn dropdown
discovery, output capture and a successful build callback (1.83 seconds).
Independent output readback matches the previously game-tested GUN-5000 BIG.
The subsequent Error Log screenshot confirms both ALPHA001 and ALPHA002 under
Warnings, with no error entries shown. The corrected InvalidXml run now visibly
reaches the failure callback without Build Completed. Fresh packaging of this
fix now passes with the corrected failure marker; public distribution review
remains. Local help renders correctly, and the
user's subsequent Reborn build succeeds after the failure (1.73 seconds), with
the same verified BIG digest. Earlier timing ranges
refer to broader scopes, not a prerequisite to releasing this limited alpha.
Editable [input admission and building](docs/RA3EP1_ARMOR_AUTHORING_ADMISSION.md)
now support canonical integer percentages 0..10000 for AlliedConYardArmor's
thirteen coefficients and five optional root scalars.
Five values and repeated output passed offline; 5000 retains the exact
user-tested package digest. Updated clean extraction passes; the next alpha gate
is distribution review, not completion of every processor.
See [local workspace and extraction evidence](docs/RA3EP1_ALPHA_WORKSPACE.md).

The manual InvalidXml GUI test correctly categorized ALPHA100 as an error, but
revealed that EA Studio still called the success callback: its process completion
does not consult ExitCode, and only `Critical:` sets `stepFailed`. The backend
now also emits `Critical: ALPHA101` on failure. Offline refusal passes; the user's
repeat screenshot confirms the failure callback and no Build Completed line.
EA binaries are unchanged; the earlier ZIP predates this fix.
The new LOCAL-USE-ONLY ZIP extracted to `UprisingAlpha-extracted-c` contains the
fix and updated help. Both sample builds and DTD refusal with ALPHA100/ALPHA101
pass; success logs contain neither failure marker. ZIP SHA256:
`D6E70F459B066A9301589A3CA758FF4A31ED182373BBB53050FE156770C2118A`.
This local artifact is not a public release or a new gameplay test.

A [user-supplied dependency overlay](docs/RA3EP1_ALPHA_OVERLAY.md) now separates
EA Studio and official schemas from the candidate ZIP. Clean setup verifies
eight pinned external files, refuses mismatched/occupied inputs and reproduces
both sample builds. Its relative manifest excludes absolute preparation paths.
Compiler/fixture provenance and embedded metadata review remain; no public
release artifact has been uploaded.
The [bounded binary audit](docs/RA3EP1_ALPHA_BINARY_AUDIT.md) found and removed
local managed PDB paths from candidate builds. The overlay exporter now refuses
the old path-bearing files; a fresh candidate passes setup and both sample
regressions with unchanged mod digests. Native apphost vendor metadata and
distribution notices remain explicitly documented, not silently cleared.
The newest overlay also includes a [relocation helper](docs/RA3EP1_ALPHA_RELOCATION.md):
it updates internal paths after copying the configured folder, validates external
paths and preserves an exact config backup. Both sample builds pass in the new
location. This is not a runtime-bundled, cross-machine-tested portable SDK.
The latest overlay [preserves supplied vendor notices](docs/RA3EP1_ALPHA_NOTICES.md)
and upstream attribution, refuses missing notice inputs and passes fresh setup
and relocation regressions. Exact host/fixture provenance review remains open;
these copies are not a public redistribution approval.

| User-visible milestone | Current evidence |
| --- | --- |
| Compile a focused ArmorTemplate XML path | Schema, plugin and document controls pass |
| Match actual Uprising armor payloads | Static 744/856-byte and WorldBuilder 128-byte BIN samples match, with their RELO data |
| Reproduce this path from current source | New one-command [armor PoC runner](docs/RA3EP1_ARMOR_POC_RUNNER.md): build + four checks passed |
| Write actual Core identity/checksum armor streams | [Four cases pass](docs/RA3EP1_ARMOR_CORE_STREAMS.md): empty/repeat/edit/populated, two-reader readback and corruption refusal |
| Exercise actual intermediate commit and linked output | [Scoped OutputManager path passes](docs/RA3EP1_ARMOR_OUTPUT_MANAGER.md): one populated armor, real `.asset` commit and manifest/link writers |
| Package the verified linked family | [Diagnostic BIG4 passes](docs/RA3EP1_ARMOR_DIAGNOSTIC_PACKAGE.md): four exact archive entries and corruption/no-overwrite controls |
| Prepare a behavior-changing asset | [AlliedConYardArmor GUN 100 → 5000](docs/RA3EP1_ALLIED_CONYARD_OVERRIDE.md) is packaged; stock baseline matches and all other bytes are preserved |
| Prepare its runtime loading route | Queue candidate tested without observed effect; [second default-reader candidate prepared](docs/RA3EP1_ARMOR_LOADING_COMPARISON.md): fifteen entries, unchanged stream bytes, detached controls and no-launch preflight pass |
| Start with the armor candidate | Second default-reader trial reached the menu (user confirmation); 33 direct 1.1 child snapshots matched its config argument |
| Load our authored change in Uprising | [First behavioral success](docs/RA3EP1_ARMOR_LOADING_COMPARISON.md): user reported markedly increased yard damage in the second trial; exact multiplier and individual file reads were not instrumented |
| Rebuild the working example in one command | `Build-Ra3Ep1ArmorMod.ps1` composes a fresh XML/Core build and default-route packaging, requiring the exact user-tested package digest; fixed example only |
| Build edited XML through the alpha backend | `Invoke-Ra3Ep1AlphaBuild.ps1 -ModXml samples/armor/AlliedConYardArmor.xml` builds the selected GUN coefficient through actual Core; five coefficients, repeat, corruption, mismatch and no-overwrite controls pass offline |

Run `scripts/Invoke-Ra3Ep1ArmorPoc.ps1` under PowerShell 7 to reproduce the
isolated diagnostic streams and `readiness.json`; it does not install files
or start a game. See the linked guide for the exact command and limitations.
Current `document/core-*` streams use real Core InstanceHash/checksum values;
the older `static`/`worldbuilder` golden fixtures keep their fixed diagnostic
values. Those older test streams have no in-game validation; the current
AlliedConYardArmor default-reader package has user-observed behavioral validation.
The runner also emits one `document/RebornArmorOutput-*` directory through the
real OutputManager and reports `OutputManagerCommitLinkValidated`. This is
explicitly diagnostic-only; `.version` publication and general production
output remain disabled.
The runner now writes a 1,222-byte diagnostic BIG4 and reports
`DiagnosticArmorPackageValidated` and `BehaviorOverridePrepared`. It targets
the existing AlliedConYardArmor identity, with only GUN changed from 100 to
5000 (damage multiplier 1.0 → 50.0). A separate preparation command now creates
a 3,650-byte three-variant BIG plus `reborn_1.0.cfg` and an identical mod SKU
fragment. It does not install or launch anything. No observed gameplay
override existed at preparation time. The subsequent default-reader trial now
has user-confirmed increased damage. Generated preparation/process reports keep
their original false runtime flags; later user evidence is recorded separately.
The `1.0` in the
mod config is a mod version label; the selected game remains Uprising 1.1.
ProcessingHash has no standalone manifest field and stock equality is not
a proven runtime requirement; the linked report separates this from the
characterized TypeHash and unresolved runtime acceptance.

Why progress previously felt slow: earlier work mainly hardened the debugger
path without closing package or loaded-mod gates. The narrow armor package
has now passed a user-observed in-game test. The historical 52%
figure is a coarse effort estimate, not measured readiness; it should not be
used as a countdown. Track the milestones above instead. Measured inventory
remains 785/1,390 models and 762/1,390 typed marshallers; those counts do not
prove runtime compatibility. Broad work can wait until this narrow path's
remaining production contract is resolved. Config consumption still needs
proof, but debugger subsystem expansion is not a prerequisite for compiler work.

<details>
<summary>Detailed engineering history and historical effort estimates</summary>

## Uprising progress — October 11, 2026

Latest output classification: [bounded module-load stop prefixes](docs/RA3EP11_MODULE_LOAD_STOP_PREFIX.md).
An explicit stop-window opt-in can classify up to 32 strictly shaped module
records before a fresh stop marker; default/context/clear framing remains
strict. Six detached positives / 21 refusals pass with inherited bridge
controls. The complete 2,234-character recorded helper window still refuses
its unknown exception rather than silently dropping it. Printed module paths
are not identity evidence. No debugger/game executed; native runtime remains
disabled. Startup exceptions/backend integration remain open. **52% / 48%**.

Latest safety correction: [admitted initial-stop recovery](docs/RA3EP11_INITIAL_STOP_RECOVERY.md).
The generic unarmed forced-debugger-exit fallback is removed. A verified
initial helper stop now requires an empty breakpoint table, an independent
six-byte entry reread, normal detach and exact natural-completion evidence.
One injected initial-stop control and the existing armed recovery/default
quiet-display regressions passed; eight pre-action refusal guards pass.
Unknown stop/transport remains unresolved, not guessed safe. No Uprising
execution or config consumption was proved. Estimate remains **52% / 48%**:
this closes a helper safety gap, not a new compiler/runtime milestone.

Latest actual calibration: [helper stop-display profile](docs/RA3EP11_DEBUGGER_DISPLAY_CALIBRATION.md).
Two fixed non-game controls verified all five automatic display categories off
and strict complete-prompt stop/context/clear framing, original-byte restoration
and natural exit. Default display regression also passed. A prior report-format
refusal is retained separately: its legacy unarmed debugger termination did
not prove helper completion. Display tests pass 3 positives / 13 refusals;
bridge regression remains 4/24. Startup/event classification remains open;
the legacy generic forced-exit fallback is removed in the follow-up above.
No Uprising execution/config-read proof. **52% / 48%**.

Latest transport integration: [bounded output-to-controller bridge](docs/RA3EP11_NATIVE_OUTPUT_BRIDGE.md).
Phase/nonce-scoped response windows now dispatch shared pump output only after
complete prompt framing; partial chunks cannot authorize captures. Four
detached positives / 24 refusal and retry checks pass, including a character-
fragmented five-stop chain, no-response timeout and recorded helper context/
clear format replay. Default CDB stop diagnostics are deliberately refused:
their bounded calibration is still required. No game/debugger was launched
in this follow-up. Native execution/config consumption remain open. **52% / 48%**.

Latest backend component: [shared bounded output pump](docs/RA3EP11_DEBUGGER_OUTPUT_PUMP.md).
The admission helper now uses a reusable completed-task reader with independent
stdout/stderr caps, pre-append overflow checks, explicit EOF and permanent read
faults; it never disposes caller-owned streams. Five detached positives / nine
refusal/retry checks pass. Two actual non-game helper regressions pass normal
clear/resume and injected-failure cooperative detach with the new pump. No
Uprising was executed; native output-to-state integration remains open. **52% / 48%**.

Latest lifecycle integration: [composed native supervisor decisions](docs/RA3EP11_NATIVE_SUPERVISOR_SESSION.md).
The final captured reader return now stays stopped for cleanup instead of
losing its pending-stop identity through continuation. Observation failures
survive successful supplied cleanup; no-response deadlines and unknown states
cannot choose a forced-exit fallback. Four detached positives / 28 refusal
and retry checks pass, together with inherited observation/cleanup suites.
This is policy integration, not an execution backend. No game/debugger was
started and runtime config consumption remains unproved. **52% / 48%**.

Latest cleanup integration: [five-site native evidence session](docs/RA3EP11_NATIVE_CLEANUP_SESSION.md).
Original site bytes now come from private admission-captured copies. A known
matching stop, an empty breakpoint list, five original bytes before detach,
normal debugger exit and five bytes after detach are separately required.
Two detached positives / 207 refusal and retry checks pass; observation failure
is preserved. No actual clear/detach or game operation was performed, and real
cleanup proof is still required. Native execution remains disabled. **52% / 48%**.

Latest integration: [native admission-to-observation session](docs/RA3EP11_NATIVE_OBSERVATION_SESSION.md).
The pinned native gate now feeds an exact-probe decoder through independently
matched stop/context PID, thread and site identities before memory captures.
Three detached positives / 39 refusal and retry checks pass, including replay,
partial frames, capture failures and admission-before-read ordering. Exported
chains retain their cleanup requirement. No debugger/game commands were sent;
native five-site cleanup and runtime consumption remain open. **52% / 48%**.

Latest recovery implementation: [cooperative helper exception cleanup](docs/RA3EP11_COOPERATIVE_HELPER_RECOVERY.md).
Errors at a verified owned breakpoint now clear/verify/detach normally;
unknown armed states no longer use the forced-kill fallback. Capture-refusal
and timeout injections both passed actual cleanup and natural helper exit,
without erasing the original observation failure. Normal clear/resume also
passed. This does not fix lost/broken debugger transport or enable native
game execution. Actual config consumption remains open; **52% / 48%**.

Latest safety result: [armed-breakpoint exit controls](docs/RA3EP11_ARMED_EXIT_HELPER_CONTROL.md).
A bounded helper timeout followed by normal detach passed: original byte
restored, helper survived and completed naturally. Forced debugger loss
failed despite byte restoration: no normal completion marker within the
deadline. That execution path is now blocked before any process action;
the later exact helper PID was absent, with no normal-exit proof. Native
game execution remains disabled. Armed-list tests pass 2 positives / 10
faults; this is a safety boundary, not mod readiness. Estimate **52% / 48%**.

Latest exact-probe preparation: [bounded config selection](docs/RA3EP11_CONFIG_PROBE_FILTER.md).
Optional filtered sessions stop unrelated path comparison at its first
mismatch and ignore known untracked-thread events without reader/buffer
captures. Nested same-thread ambiguity and read failures fault permanently;
ignored-only streams cannot prove a read. Seven detached positives and 20
fault/retry refusals pass; all stops, including ignored calls, are capped at
128. No game/debugger was executed in this follow-up. Live native integration
and armed-breakpoint failure cleanup remain open; estimate **52% / 48%**.

Latest actual breakpoint milestone: [admitted-helper event transport](docs/RA3EP11_BREAKPOINT_HELPER_CALIBRATION.md).
The shared gate now precedes a helper-only delay breakpoint; its owned hit
matches a separate register response. Explicit clearing, an empty breakpoint
listing and independent original-byte restoration pass before normal resume.
Three actual controls pass, including the final clear-list guard; a failed
managed PE-entry experiment is recorded separately. No game was launched.
Probe-only selection and native config consumption remain open; **52% / 48%**.

Latest actual helper milestone: [CDB context calibration](docs/RA3EP11_CONTEXT_HELPER_CALIBRATION.md).
Two admitted helper runs each returned two stable nonce-bound x86 contexts;
owned-handle stack reads matched and actual stale responses were refused.
Both helpers/debuggers exited normally. Completion framing also passes
10 detached faults and 5 partial-chunk refusals. No game was executed and
native config consumption remains unproved. Estimate **52% / 48%**; next
is breakpoint-event transport and probe-only selection, not mod readiness.

Latest debugger bridge: [nonce-bound context transport](docs/RA3EP11_CONFIG_DEBUGGER_TRANSPORT.md).
One requested register response now feeds the incremental decoder with
thread-scoped invocation IDs. Three detached controls and 40 refusal/retry
checks pass, including interleaved threads, stale responses and pending
completion. No commands were sent; actual CDB framing still needs helper
calibration. Native execution/config consumption remain unproved; **52% / 48%**.

Latest streaming preparation: the [snapshot adapter](docs/RA3EP11_CONFIG_SNAPSHOT_ADAPTER.md)
now accepts one stop at a time through a bounded session. Captures happen at
that stop, not by rereading old snapshots after the process resumes. Three
detached streaming/isolation positives and 18 refusal/retry checks pass;
the previous two adapter cases / 26 faults still pass. Faulted or closed
sessions cannot read again. No live collector, process query or debugger run
was added. Runtime config consumption remains unproved; **52% / 48%**.

Latest implementation: [x86 config snapshot adapter](docs/RA3EP11_CONFIG_SNAPSHOT_ADAPTER.md).
Reviewed stack/register locations now feed the engine correlation validator
through an exact-address bounded read callback. Two detached reader cases
and 26 faults pass; invalid initial identity/register inputs perform no reads.
This is not an owned-process collector: native debugger execution remains
disabled and no live game evidence was collected. Estimate **52% / 48%**.

Latest evidence preparation: [engine config-read correlation](docs/RA3EP11_CONFIG_READ_CORRELATION.md).
The detached validator links exact-probe entry, concrete reader object, actual
one-byte read buffer, splitter input and caller return within one same-thread
invocation. Four positive cases and 41 injected refusals pass, including
interleaved threads and sequential pointer reuse. The native admission plan
now covers the return site: 24 ranges / 4,216 bytes, with 26 plan refusals and
33 shared-gate refusals passing. These are supplied/disk-derived fixtures,
not live game observations. No game or debugger was executed; the native
collector remains disabled/unimplemented. Estimate remains **52% / 48%**:
runtime config consumption and a loaded mod are still unproved.

Previous integration: [shared supervisor admission gate](docs/RA3EP11_SUPERVISOR_ADMISSION_GATE.md).
The native factory plan and calibrated helper supervisor now use the same
identity/read decision. The 23-range native disk fixture passes; 18 identity
and 7 range faults refuse before any read callback, and 8 read faults stop at
the first failed range. Actual helper admission, shared-gate byte refusal and
withheld admission controls pass. Native preflight rejects `-Run` before any
action: no native game capture/launch path is enabled and no config consumption
is proved. English report updated; estimate **52% / 48%**.

Previous native preparation: [fixed-base admission plan](docs/RA3EP11_NATIVE_ADMISSION_PLAN.md).
The pinned 1.1 executable has stripped relocations and no relocation directory;
the native policy requires actual base `00400000`, rather than applying the
managed helper's byte policy. Twenty-three code/vtable ranges total 3,906 bytes;
IAT and writable sections are excluded from raw-byte comparison. Twenty-six
detached refusal cases pass. No process was queried or executed and no live
game bytes were validated. Native supervisor integration and actual config
consumption remain open; estimate **52% / 48%**.

Latest admission milestone: [initial-break helper gate](docs/RA3EP11_DEBUGGER_ADMISSION_CONTROL.md).
The new supervisor waits at CDB's initial break, validates the owned helper's
PID/parent/path and reads six stage-specific entry bytes at its actual live base
before sending observation `g`. Admission, injected-byte refusal and withheld
admission controls pass, as do four detached positives and sixteen refusals.
Rejected helpers receive cleanup detach and therefore can run afterward; this
is not execution containment. The managed helper's raw CLR thunk remains
unchanged at this stage despite a nonpreferred base, so its byte policy must
not be copied into Uprising. Native-game live pins/relocation, config consumption
and game-debug authorization remain open. No game was launched; estimate **52% / 48%**.

Latest helper-only safety milestone: [debugger ownership and breakpoint cleanup](docs/RA3EP11_DEBUGGER_LIFETIME_CONTROL.md).
Normal detach, forced debugger loss and active-breakpoint detach controls pass.
Initial debugger PID matches the owned helper and its direct-parent identity.
In the active-breakpoint case, one native code byte is independently read after
detach and matches its original value; the helper then exits naturally. Two
detached positives and twelve evidence refusals pass. The read-control runner
also requests `-pd`. This is not a game detachment guarantee: ownership is
validated after resume, not enforced at initial break, and forced-loss cleanup
with installed breakpoints is untested. Actual config read/parser consumption
and an early game admission gate remain open. No game was launched.

Latest static bridge: [config disk backend](docs/RA3EP11_CONFIG_DISK_BACKEND.md).
A concrete provider now connects the engine request to `CreateFileW`/`ReadFile`.
The wrapper ignores the Windows read boolean and returns its output count,
so both must be observed separately. Five new pins, three import bindings and
seven private mutation refusals pass. Live provider selection/actual config
read remain unproved; no new game or debugger execution in this follow-up.

Next read-observation preparation: [native config read route](docs/RA3EP11_CONFIG_READ_ROUTE.md).
The source-reader fallback is now independently pinned: direct-request and
buffered branches return engine byte counts, not NTSTATUS. The completion
wait can time out while pending; a worker can finish after a short read.
Twelve code/data pins and sixteen private mutation refusals pass; full internal completion-state
semantics remain open. No debugger or game was executed in this follow-up.
The `0x400` open flag attempts a memory-reader conversion before parsing;
zero-byte population can still count as conversion success. A positive read
return also needs a non-null destination/content check. Static pins
and vtable bindings define narrower observation sites, not game-read proof
or an authorized debugger recipe. No new runtime trial was performed.

Latest runtime milestone: [short-name config control](docs/RA3EP11_SHORT_NAME_CONFIG_TRIAL.md).
The user-authorized same-content `probe_1.0.cfg` trial reached the main menu
(user confirmed); 34/34 child snapshots match its `-modconfig` path. No
Application 1000/1001 events were found in the narrow trial window. This
strengthens the filename-limit explanation for the historical crash without
proving the exact caller or actual config read. No debugger or mod package.

Latest diagnosis: [post-config identity limit](docs/RA3EP11_CONFIG_IDENTITY_LIMIT.md).
Our old inert probe's 20-byte basename does not fit the reviewed 16-byte
identity copy after config processing. The recorded CRT fault maps to the
`strcpy_s` error path, consistent with this defect but not exact-caller proof.
The runner/planner now refuse that name; the same-content `probe_1.0.cfg`
follow-up reached the menu as described above. No game/CRT patch or guard relaxation.

Historical failed runtime result: [old-name inert trial](docs/RA3EP11_INERT_CONFIG_TRIAL.md).
The single-LF probe path reached the 1.1 child's visible command line, but the
game crashed without user-confirmed menu arrival. Windows Event 1000 matches
the PID and records `MSVCR80.dll`, exception `0xc000000d`, offset `0x00014584`.
Root cause and actual config consumption remain unproved. No debugger, mod
package, system-wide recording or automatic retry was used. The quoted-only
matcher was corrected offline (5/5 snapshots); raw evidence is preserved.
Overall estimate stays **52% / 48%**.

First real runtime milestone: [normal 1.1 baseline](docs/RA3EP11_NORMAL_BASELINE.md).
One user-authorized normal launch selected `ra3ep1_1.1.game`; 41 process
snapshots show its default `-config` SKU argument, and the user confirmed the
main menu opened. No debugger, mod package or `-modconfig` was used. Config
consumption and authored-mod loading remain unproved. Overall estimate stays
approximately **52% / 48%**; this closes launch-selection uncertainty only.

Historical development branch: `feature/ra3ep1-manifest-inspector` (now continued
on `main`). This earlier snapshot was not yet a usable
Uprising Mod SDK release. Replacing XML/XSD files alone is insufficient:
native layouts, type hashes, dependencies and in-game loading must also pass validation.

Approximately **52% complete / 48% remaining**. This is an engineering-effort
estimate, not a file-coverage metric or a measure of working game mods.
It is rounded from weighted workstreams and is not increased per commit.
The containment audit and attach-base recovery accumulated enough work to
reassess the native-layout workstream from 53% to 55%; the overall estimate
moves from about 49% to 50%. Major type-table and in-game gates remain open.
The subsequent environment/path/catalog/effective-schema/candidate diagnostics
raise the manually estimated SDK tools/dependencies workstream from 20% to 25%.
The accumulated recursive preparation/removal milestones now reassess that SDK
workstream from 25% to 30%. Its 15% weight adds 0.75 overall point: 50.25% becomes
51%. Other workstreams and zero proven game loading are unchanged. This is not
a measured percentage of usable mods or automatic credit per test/commit.
The accumulated audio-expression/breadth/full-owner/native-corroboration work
and complete measured XML preparation now reassess SDK tooling from 30% to 35%.
Its 15% weight adds another 0.75 point: 51.0% becomes 51.75%, rounded 52%.
Missing dependencies, native emission and game loading remain open.

Measured inventory: **785/1,390** EP1 complex types have models and
**762/1,390** have typed marshallers. The compiler test runner invokes
**165 test groups** (some contain several fixtures). These counters can grow
without making a usable SDK; they measure coverage, not game compatibility.
The coverage script also reports `CompilerTestGroupsDeclared`; it counts
registered groups but does not execute them. Native PE evidence and the managed
encoder WAV fixture, isolated AudioFile serializer, authored input profile and
fixed package/local event proofs add groups without changing model/marshaller
counts; hash boundary, core identity/preparation, publication, cleanup and managed
worker supervision/authored snapshot/pool/path/catalog/binding/candidate/review/graph/local/Include/definition-subset/self-inheritance/child-copy/complex-leaf/sequence-tree/direct-instance/root-file/empty-child-merge/instance-chain/removal/choice-characterization/choice-copy/consumed-marker/bitflag/filter/upgrade-characterization/upgrade-normalization/metadata-leaf/pre-inheritance-expression/sibling-identity-characterization/identical-state/ordered-state-readd/CC32-reference-review/cross-state-removal/music-volume-offset/audio-tree-budget/sound-offset/sound-singleton-characterization/full-sound-owner-review/sound-singleton-admission/dependency-classification/known-map-alias/stock-music-review/header-semantics/music-runtime/music-authored-snapshot/music-package/music-Core-identity/music-reference-identity/music-Core-preparation/music-Core-publication/music-Core-checksum/music-experimental-Core-v1 regressions add groups and all 162 groups
were executed.
Controlled music dispatch/selection/selected publication add three further groups; all 165 groups were executed.
Default tests do not invoke native codecs.

First runtime-test target: **2–4 focused work sessions** to prepare a controlled
config/unchanged-stock loading smoke test, not a promised date. An authored XML
asset changing the game has separate selected-type/hash/dependency/package
gates and no reliable date yet. See [first mod-test plan](docs/RA3EP11_FIRST_MOD_TEST_PLAN.md).
The complete SDK is not a prerequisite for a narrow test; synthetic diagnostic
music packages remain ineligible. The first normal baseline is now complete;
the earlier 2–4-session estimate concerned preparation, not a new countdown.

Current installation update: [campaign archive restoration](docs/RA3EP11_CAMPAIGN_RESTORE.md).
The user-identified original campaign backup was restored to MapsCampaign.big
with unchanged SHA-256 and no overwrite. The user then authorized reversible
isolation of four loose mapmetadata files and 118 files in Data/maps; all 122
backup SHA-256 hashes match. The originals are absent from the installation,
and backup contents remain local. The Phase A planner reports known overrides
absent without claiming complete stock authenticity. Earlier missing-campaign
observations below are historical. Restoration/isolation did not start a game;
the later separately authorized normal baseline is linked above.

Latest implementation: [stream marker/family audit](docs/RA3EP1_STREAM_VARIANT_AUDIT.md).
`stream-variant-audit` now checks a bounded selected BIG marker, exact candidate
stream siblings, linked manifest structure and target-specific checksum headers.
Three real KW/RA3 examples pass, plus 16 detached positives and 26 refusals.
It neither certifies EP1 compatibility nor resolves cdata/patch bases or proves
native suffix selection. No game or reference package is modified; overall
estimate remains **52% / 48%**. All 165 existing compiler groups and layout tests
were rerun successfully; the new variant CLI tests are counted separately.

The [first runtime-test plan](docs/RA3EP11_FIRST_MOD_TEST_PLAN.md) now explicitly
separates mod SKU selection, optional launcher game-version requests, SKU
executable selection and stream `.version` suffixes. RA3 loading conventions
do not independently establish EP1 forwarding or compatibility; the actual
1.1 child and config read remain required. No game/launcher settings changed.
Private reference-source audit details remain local, not published here.

Latest package evidence: [user-built Reborn comparison](docs/REBORN_MOD_PACKAGE_COMPARISON.md).
Actual KW/WrathEd and RA3/EA SDK examples confirm nonempty `_mod` markers as
well as whitespace-only markers. RA3 deneme packages `map.version` with `_mod`
and `map_mod.*`; its map references `worldbuilder.11.manifest` as a patch base.
Five selected manifests pass structural inspection; this is not payload/runtime
validation. KW v5 / RA3 v6 packages are not converted to EP1 v7 by renaming.
No sample files or installed games were changed. Effort remains **52% / 48%**.

[Owned-helper debugger calibration](docs/RA3EP11_DEBUGGER_READ_CONTROL.md).
The existing CDB debugger observed one exact probe open/read/return in a
newly owned non-game helper: native status 0, one actual byte, value 0A,
and matching native/managed PID. Separate x64 and x86 command recipes now pass
owned-helper calibration; x64 is default and x86 requires explicit selection.
No administrator elevation, WPR, installation,
existing-process attach or game launch was needed. Nine detached fault cases
are refused; the default runner is preflight only. This is not a game read/load
result: Uprising still needs a separate owned-launch/instrumentation plan.
Software breakpoints affect only the helper's temporary memory and timing.
Effort **52% / 48%**; compiler groups 165 (not rerun).

[Actual read-control ETL result](docs/RA3EP11_READ_CONTROL_RESULT.md).
The user's administrator capture succeeded, but read-only decoding found both
positive and negative probe reads and 24 other PIDs: the requested process-name
scope is REJECTED. -Record is disabled even with elevation; do not rerun the
previous command or use the equivalent game candidate. Exact ETL/count pins,
five detached correlation checks and repeat decoding pass. Adjacent completion
candidates are not a universal loss-free/byte-transfer proof. Raw ETL stays
ignored/local. Next: validate replacement capture-time isolation or obtain
explicit approval for disclosed broader local capture/post-filtering.
Effort **52% / 48%**; no game mod loaded; compiler groups 165 (not rerun).

[Non-game config-read control](docs/RA3EP11_CONFIG_READ_CONTROL.md).
Both distinct compiled helper processes successfully read the one-byte fixture
in helper-only validation. Reviewed source/scope and four detached refusals
pass. A real WPR start was denied (0x80070005): sandbox approval is not Windows
administrator elevation. The historical recording subsequently succeeded in
the user's admin session but failed its scope control, as documented above.
The runner now refuses -Record; the old administrator command is superseded.
Helper-only validation remains available. Effort **52% / 48%**; compiler groups
165 (not rerun).

[Narrow config-read trace candidate](docs/RA3EP11_CONFIG_TRACE_CANDIDATE.md).
WPR metadata accepts one Kernel-File provider, the ra3ep1_1.1.game process-name
filter, five event IDs and a 4 MiB configured buffer product; no stacks or broad
system profile is requested. Eight scope mutations/two XML-policy refusals and
repeat JSON pass. Local event templates distinguish filename/file-object/key,
read request and Irp completion. Live filtering/correlation are not proved;
no trace or game was started. Next: a non-game controlled read capture before
using this candidate for the prepared probe. Effort **52% / 48%**; compiler
groups 165 (not rerun).

[Phase A inert config-read plan](docs/RA3EP11_CONFIG_READ_SMOKE_PLAN.md).
A byte-pinned single-LF external config (zero directives/assets) and checked
baseline/probe launch-request arrays are prepared. Two whitespace fixtures,
nine refusal fixtures, exact config identity and repeat JSON pass. The current
profile explicitly discloses missing MapsCampaign.big without aliasing it.
No launch occurs: actual child/version/argument capture and a validated
attributable config-read signal remain. This is preparation, not mod loading.
Effort **52% / 48%**; 165 compiler groups (not rerun).

[EP1 launcher profile and scoped observer](docs/RA3EP11_LAUNCHER_PROFILE.md).
The selected launcher has a .bind entry point; five launch/config literal pins
and four API import bindings are verified, but their consumer flow, version
selection and argument forwarding remain unproved. Five literal faults, one
truncation refusal and repeat JSON pass. A separate read-only observer accepts
an explicit already-running launcher PID and records immediate same-installation
game child paths/raw command lines. Its two positive/three refusal detached tests
pass; no live process query or game launch was performed. Next: controlled
baseline/isolated-config experiment preparation and actual child/read evidence.
Effort **52% / 48%**; 165 compiler groups (not rerun).

[EP1 1.1 bounded installed-config inventory](docs/RA3EP11_CONFIG_INVENTORY.md).
Four small loose ASCII configs are content-pinned; both WorldBuilder configs
are byte-identical. No filesystem.cfg was found in the root/Data directories
or the 13 available configured archive directories (17,383 entries). This is
scoped absence, not a proved runtime fallback. Zero archive payload bytes read;
three text/two directory fixtures, one size refusal, six malformed-directory
refusals and repeat JSON pass. Effort **52% / 48%**; 165 compiler groups (not
rerun). Launcher static boundaries/observation preparation are documented above.

[EP1 1.1 scoped startup config order](docs/RA3EP11_STARTUP_CONFIG_ORDER.md).
Conditional mod config reading precedes reverse additional config candidates
and filesystem.cfg. Base read failure synthesizes set-search-path; success may
append a language path. Later startup updates can therefore matter to effective
mod paths. Three scoped slices/five strings, six ordering fixtures, three policy
rejections, nineteen faults and repeat JSON pass. No whole-startup or effective
override claim; no archive payload read. Effort **52% / 48%**; compiler groups
165 (not rerun). The bounded config inventory above follows up this milestone.

[EP1 1.1 scoped search-path pairs](docs/RA3EP11_SEARCH_PATH_PAIRS.md).
The native builder clears and rebuilds ordered semicolon root/registration
pairs, rather than appending old pairs. It removes only one trailing slash,
preserves duplicates/whitespace and stores provider resolution results without
a null check. Context +38h is the unchanged capacity/iteration bound, not an
active count written by this routine. The complete config writer now links
set-search-path to rebuilding. Two complete pins, six pair/one rebuild/one
open-composition fixtures, nine policy rejections, sixteen faults and repeat
JSON pass. No payload is read. Native capacity/empty-token safety, full startup
order, authentic compiler hash and game loading remain open. Effort **52% / 48%**;
compiler groups 165 (not rerun).

[EP1 1.1 scoped provider-open forwarding](docs/RA3EP11_PROVIDER_OPEN_FORWARDING.md).
Explicit providers receive the path after one leading dot-separator removal,
without interior slash conversion or prefix case changes. Default-provider
opens instead walk ordered search-root/registration pairs: relative names use
exact %s/%s joining, rooted names bypass joining, null roots stop and first
success wins. Empty search lists do not gain an invented raw fallback.
Two complete bodies/one literal, six forwarding/eight search-plan/four composed
prefix fixtures, three policy rejections, eighteen faults and repeat JSON pass.
No archive payload is read. Runtime search-pair contents, startup priority,
native safety/lifetime, compiler identity and game loading remain open.
Effort **52% / 48%**; compiler groups 165 (not rerun).

[EP1 1.1 scoped provider routing](docs/RA3EP11_PROVIDER_ROUTING.md).
Provider prefix routing is distinct from per-archive file selection: first
matching registered alias wins (ASCII case-insensitive), unknown prefixes
return null, and unprefixed queries use the context default. Registration and
alias nodes append to their lists. A pinned startup slice connects BIG interface
registration to virtual open slot +0C and the previously reviewed BIG open body.
Seven complete bodies, a scoped startup slice/two vtable slices/alias literal,
eight detached routing fixtures, three policy rejections, twenty byte faults
and repeat JSON pass. No archive payload is read. Complete startup order,
path qualification, global precedence, native safety and game loading remain
open. Effort **52% / 48%**; compiler groups 165 (not rerun).

[EP1 1.1 scoped BIG open selection](docs/RA3EP11_BIG_OPEN_SELECTION.md).
The manager searches its forward list and retains the first successful node.
An exact archive selector has a distinct ordinary-miss early failure; Viv4
misses continue. Open preparation removes exact big: and at most one leading
separator, without interior slash normalization. Slash conversion/trailing
slash removal belongs to a separate iterator initializer, not the copy helper.
Three complete pins/one literal, eight query/eight selection/four iterator
fixtures, three policy rejections, sixteen faults and repeat JSON pass.
No archive payload is read. Actual startup order, global provider precedence,
native stream lifetime, authentic compiler hash and game loading remain open.
Effort **52% / 48%**; compiler groups 165 (not rerun).

[EP1 1.1 scoped BIG name index](docs/RA3EP11_BIG_NAME_INDEX.md).
The native mount builds eight-byte hash/directory-pointer records and sorts
unsigned name hashes. ASCII case folds but separators remain distinct; this
is not compiler ProcessingHash. Thirteen stock directory-only models cover
17,383 entries, with six repeated-name hash groups and no distinct-name hash
collision within an available archive. Native adjacent collision comparisons
use the midpoint name, so arbitrary collision correctness is not claimed.
Seven complete pins, eight hash fixtures, three name-policy rejections, one
collision directory/six malformed directories, sixteen faults and repeat JSON
pass. No archive payload is read. Native array identity, outer lookup/precedence,
authentic compiler hash and game loading remain open. Effort **52% / 48%**;
compiler groups 165 (not rerun).

[EP1 1.1 scoped BIG mount metadata](docs/RA3EP11_BIG_MOUNT_METADATA.md).
The add-big mode reaches the pinned native mount/index route; node allocation,
header classification, directory-size arithmetic and list append are recovered
within their reviewed scope. Thirteen stock BIG4 directories agree. Four
whole-body pins, six detached header fixtures, one truncation rejection,
twelve byte faults and repeat JSON pass, with zero archive payload bytes.
Full lookup/precedence, native failure safety, authentic compiler hash and
game loading remain open. Effort **52% / 48%**; compiler groups 165 (not rerun).
This is a documented stopping checkpoint, not a usable SDK release.

[EP1 1.1 fresh archive sidecar headers](docs/RA3EP11_ARCHIVE_SIDECAR_HEADERS.md).
Twelve BIN/RELO/IMP logical headers and lengths match the freshly bridged
manifest checksums/totals: six raw and six compressed literal prefixes.
Only 384 sidecar prefix bytes are read per invocation, including eight-byte
WorldBuilder BIN headers; no whole instance stream is read/decompressed.
Three positive/three malformed detached fixtures and repeat JSON pass.
Stored BIG length is not logical length for compressed streams. Whole bodies,
authentic compiler hash and game loading remain open. Effort **52% / 48%**;
compiler groups 165 (not rerun). Next: native BIG mount/package profile.

[EP1 1.1 fresh core manifest archive bridge](docs/RA3EP11_CORE_MANIFEST_ARCHIVE_BRIDGE.md).
Four selected manifests in the configured 1.1 archives now match complete
SHA-256 of the captured unpacked evidence: three RefPack-expanded and one raw
(WorldBuilder), covering 55,519 entries. Three malformed detached decoder
fixtures and repeat JSON pass. No adjacent BIN/RELO/IMP payload is read; the
large worldbuilder instance stream is untouched. Stock campaign completeness,
authentic compiler ProcessingHash and game loading remain open. Effort **52% /
48%**; compiler groups 165 (not rerun). Next: fresh sidecar headers/lengths.

[EP1 1.1 stock package preflight](docs/RA3EP11_STOCK_PACKAGE_PREFLIGHT.md).
The stock 1.1 SKU matches the audited executable but MapsCampaign.big is missing
under its configured name (a Disabled-named file is present and not substituted).
Thirteen available archives contain 17,383 entries and 639 manifest directory
entries, including static/_l/_m/global/worldbuilder. Repeat preflight reads only
directory metadata, zero archive payload bytes. Stock completeness, payload
validation and game loading remain separate unresolved gates. Effort **52% /
48%**; compiler groups 165 (not rerun). No installed file was changed.

[EP1 1.1 reader suffix/probe behavior](docs/RA3EP11_READER_VARIANTS.md).
004AA300 takes a suffix index/text, selects three reader-over-global suffix
slots and inserts them before the last dot unless reader+68h bypasses it.
The queue driver passes slot one; source probing can set slot two to _v%d.
004D7840 is a file-probe adapter, not a manifest parser. Four pins, five detached
filename fixtures/five policy rejections, sixteen faults and repeat JSON pass.
Full fallback/native safety and game loading remain open. Effort **52% / 48%**;
compiler groups 165 (not rerun). Next: stock-bound package preflight/BIG mounting.

[EP1 1.1 config manifest queue to reader bridge](docs/RA3EP11_MANIFEST_QUEUE.md).
Driver 0064D440 sends queued add-manifest strings to ordinary reader factory
004AAC80 when its reader vector is empty, and conditionally calls 004D05E0
after source probing. A third wrapper caller forwards queue argument zero to
the ninth loader argument. Four independent pins, eighteen detached faults
and repeat JSON pass. Full variant/mount/startup semantics and game loading
remain open. Effort **52% / 48%**; compiler groups 165 (not rerun).

[EP1 1.1 config consumer](docs/RA3EP11_CONFIG_CONSUMER.md).
File probing (004D6F10) is separate from config reading (004D86B0), line
splitting (004D9040) and nine-prefix dispatch (004D8DE0). Native add-big,
recursive config and add-manifest routes are pinned; add-manifest queues a
string and is not proof of asset loading. Eight pins, eleven detached prefix
fixtures, three policy rejections, eighteen faults and repeat JSON pass.
Full mounting/queue consumption and game loading remain open. Effort **52% /
48%**; compiler groups 165 (not rerun). Next: mount and manifest queue consumers.

[EP1 1.1 scoped reader cache lifetime](docs/RA3EP11_READER_CACHE_LIFETIME.md).
The complete destructor calls lookup/removal for both ordinary/tag-two caches,
then resource and member cleanup. Ordinary publication returns node+20h on
both existing/insertion paths. Six whole-body pins, five detached deletion
flags, eighteen detached faults and repeat JSON pass. Full tree/key/member
safety and game loading remain open. Effort **52% / 48%**; compiler groups 165
(not rerun). Next: mod-config file consumer and package-loading prerequisites.

[EP1 1.1 reader list lifetime](docs/RA3EP11_READER_LIST_LIFETIME.md).
Seven independently pinned regions recover newest-first insertion under a
healthy circular sentinel, source-list virtual release/reset, resource-only
cleanup and conditional reader deletion. Four insertion scenarios, two broken
lists, twelve detached faults and repeat JSON pass. Complete cache lifetime,
all initializers and game loading remain open. Effort **52% / 48%**; compiler
groups 165 (not rerun). Next: destructor/cache helper internals.

[EP1 1.1 pointer membership/source forwarding](docs/RA3EP11_READER_OWNERSHIP.md).
Helper 0045F060 checks reader pointer equality across three global lists; it
does not validate asset contents. Iterator 004D04A0 forwards node+8 directly
or as a local handle copy to wrapper 004CFEB0. In both reviewed calls wrapper
argument six is zero, hence the added ninth loader argument is zero. Four raw
call candidates are pinned, only two caller paths reviewed. Three whole-body
pins, six membership fixtures/two invalid groups, twelve detached faults and
repeat JSON pass. Healthy-list insertion aliases, deletion/cache lifetime and
the ninth argument's consumer remain open. Effort **52% / 48%**; compiler groups
165 (not rerun). Next: 1.1 circular-list insertion and cleanup/delete paths.

[EP1 1.1 referenced-reader factories](docs/RA3EP11_READER_FACTORY.md).
The concrete vtable routes tag 2 through slot 2Ch/004AAE80, other signed tags
through slot 28h/004AAE60 and ordinary factory 004AAC80. Cache hits return
node+20h reader pointers without reviewed vtable revalidation; misses call
004AAC10 with distinct flags. Factory owner is loader argument three, list
owner argument four. Ordinary publication writes through a cache-helper slot;
helper internals and healthy-list insertion aliases remain unproved here.
Six code/vtable pins, six tag routes/two invalid tags, eleven detached faults
and repeat JSON pass. Effort **52% / 48%**; compiler groups 165 (not rerun).
Next: 1.1 membership helper, source-list iterator and reader cleanup/lifetime.

[EP1 1.1 loader argument provenance](docs/RA3EP11_LOADER_ARGUMENTS.md).
The added ninth loader argument comes from wrapper argument six. The eighth
flag already exists in 1.0: first branch passes 1, second passes 0. Reviewed
1.1 branches use argument eight for cache-related entry routing and forward
it as gate argument seven; C4h after one push is still argument eight, not nine.
Six current/baseline pins, six positive/four invalid stack checks, ten detached
faults and repeat JSON pass. Ninth-argument consumer/full semantics remain
unproved; no authoring or bypass permission follows. Effort **52% / 48%**;
compiler groups 165 (not rerun). Next: 1.1 factories/source ownership and
caller-side provenance of wrapper argument six.

[EP1 1.1 wrapper-local lifecycle](docs/RA3EP11_LOADER_LIFECYCLE.md).
Wrapper 004CFEB0 initializes fresh entry/descriptor pointers and capacities,
passes context as argument seven and frees both buffers on normal cleanup.
Its 1.1 loader calls have nine arguments/36-byte cleanup, versus eight/32 in
the independently checked 1.0 wrapper: do not assume identical call contracts.
Four exact code pins, four fresh-invocation fixtures, seven detached faults
and repeat JSON pass. Capacity-equals-count is scoped to successful fresh
preparation in this wrapper, not allocator failures or all reader callers.
Effort **52% / 48%**; compiler groups 165 (not rerun). Next: independently
rebase factories/source ownership and resolve the added 1.1 argument's uses.

[EP1 1.1 linked chunk addressing/ranges](docs/RA3EP11_LINKED_CHUNK_RANGES.md).
The concrete queue/dispatch path forwards descriptor pointers into 00418340;
bin/relo/imp offsets each receive the eight-byte header bias. The memcpy thunk
is independently bound to its import. All 166,557 ranges across 55,519 captured
stock entries fit their twelve sidecars and end exactly at file lengths,
including WorldBuilder. Four positive/four invalid range fixtures, nine detached
code/import faults and repeat JSON pass. Each audit reads 288 header bytes and
zero payload bytes. Native read-result/bounds safety, factories/lifetime and
game loading remain unproved. Effort **52% / 48%**; compiler groups 165 (not
rerun). Next: 1.1 context creation/cleanup, then factory/source ownership.

[EP1 1.1 linked-sidecar setup](docs/RA3EP11_LINKED_SIDECARS.md).
The complete helper 00449810 prepares sidecars, not entry hash transformations.
Two whole helper bodies and three suffix literals are pinned independently to
1.1. All twelve captured-stock companions pass header/checksum/exact-length
checks, including the 1,394,571,528-byte WorldBuilder.bin. Each audit reads only
192 sidecar header bytes over two passes, zero payload bytes. Four detached
header faults, eleven private code/literal faults and repeat JSON pass. Native
checksum diagnostics do not prove unconditional rejection or magic validation.
No native execution, production-policy change or fresh 1.1 archive extraction.
Effort **52% / 48%**; compiler groups 165 (not rerun). Next: 1.1 chunk read/range
dispatch, then factories/context lifetime and modconfig package handling.

[EP1 1.1 concrete reader/descriptor producer](docs/RA3EP11_DESCRIPTOR_PRODUCER.md).
The reviewed concrete reader reads a 52-byte header and requests 48 bytes per
entry. Producer 00449750 writes 20-byte descriptors with entry pointer+12,
source pointer+16 and cumulative bin/relo/imp offsets+0/+4/+8. Its loop uses
buffer capacity, not requested count; projections remain count-equals-capacity
only. Seven code/vtable pins, four captured-stock projections (55,519 entries),
five malformed raw fixtures, eight detached code faults and repeat JSON pass.
No adjacent payload or native code executed. Linked post-read helper, factory
selection/lifetime and full stream provenance remain open. Effort **52% / 48%**;
compiler groups 165 (not rerun). Next: rebase linked-sidecar setup/ranges to 1.1.

[EP1 1.1 descriptor-to-gate pointer trace](docs/RA3EP11_ENTRY_POINTER_TRACE.md).
One independently disassembled loader path carries descriptor+12 through a
28-byte pending record+4 into the TypeHash gate's fourth argument (ESI).
Descriptor stride is 20 bytes; a separate branch copies 12 DWORDs/48 bytes.
Six exact code pins, three stack fixtures, four malformed-stack refusals,
seven detached code faults and repeat JSON pass. Stream entry reading,
descriptor construction and the other two gate callers are not yet rebased.
No engine execution or production-policy change. Effort **52% / 48%**;
compiler groups 165 (not rerun). Next: independently recover the 1.1 reader
vtable, header/entry read and descriptor producer.

[EP1 1.1 initializer/metadata sample](docs/RA3EP11_TYPE_INITIALIZERS.md).
Two reviewed templates decode 150 initializers (43 short/107 vtable). Every
sample copies the runtime hash into object+8 and registers that same object;
all 150 raw TypeIds match the managed name provider, including 138 stock roots.
Ten detached faults, opaque-word preservation, repeat JSON and consumer
regressions pass. This closes the sampled table-to-metadata consumer link,
not full startup reachability or the remaining 1,192 runtime-name coverage.
PathMusicEvent is not in this template sample. No production/authoring hash
change or native execution. Effort **52% / 48%**, compiler groups 165 (not rerun).
Next: rebase stream entry/factory/header/descriptor flow to the 1.1 hash gate.

[EP1 1.1 registry/hash consumer](docs/RA3EP11_HASH_CONSUMER.md).
Independently reviewed TypeId lookup/registration and inline TypeHash comparison
share registry head 00CF1528. Entry+8 is compared with metadata+8; separate
lookup callers instead use opaque metadata+12. Seven branch fixtures, nine
private code faults, repeat JSON and original 1.0 consumer regressions pass.
The 1.1 initializer-to-metadata hash link and complete stream-pointer provenance
remain open; diagnostic entry is not proven unconditional rejection. No native
execution or production-policy change. Effort **52% / 48%**, compiler groups
165 (not rerun). Next initializer/object binding and stream-loader rebase.

[EP1 1.1 runtime type/hash table](docs/RA3EP11_RUNTIME_TYPE_TABLE.md).
Independently located 1.1 arrays yield 1,342 ordered name/hash pairs identical
to the pinned 1.0 table; all 254 roots from four previously captured stock
manifests match. Pointer slots and hash slots moved by different amounts, so
no global address shift is assumed. Ten detached faults, four public version/
path/evidence refusals, repeat JSON and original 1.0 regressions pass. This is
runtime identity evidence, not a fresh audit of the new installation's BIG
archives, authoring ProcessingHashes, layout compatibility or loaded mods.
Next: rebase registry/hash consumer and native stream pipeline to 1.1.
Effort **52% / 48%**, compiler groups 165 (not rerun).

[EP1 1.1 baseline and modconfig](docs/RA3EP11_MODCONFIG_BASELINE.md).
The user's second Steam installation contains a distinct 13,381,632-byte
ra3ep1_1.1.game; its 1.1 SkuDef selects that executable. Previous fixed-offset
runtime audits remain scoped to the identical 1.0 images and correctly refuse
1.1. A separate pinned 1.1 audit verifies -modconfig table/parser/handler,
path storage and downstream consumption. Seven code faults and repeat JSON
pass. The option also has a literal/handler in 1.0; it is not proven 1.1-only.
No launcher forwarding or successful mod load is claimed. Next priority:
independently rebase runtime type/hash/stream evidence to 1.1, then recover
config-file handling. Effort **52% / 48%**, compiler groups 165 (not rerun).

[EP1 1.0 sentinel insertion and reader lifetime](docs/RA3EP1_READER_LIST_LIFETIME.md).
Healthy circular-sentinel aliases explain how the indirect previous-link write
updates the first node, making the following head+8 store a fresh-node payload.
Source-list release calls reader slot+8, clears nodes and restores self-links;
concrete release delegates to deleting slot+3Ch. Normal slot+24h cleanup closes
resources/sidecar buffers without directly freeing the reader object. Four
insertion scenarios, two malformed lists, twelve byte faults, repeat JSON and
ownership regressions pass. All-list initializers/cache/callback guarantees
remain open; these 1.0 addresses must not be silently reused for 1.1.

[EP1 source forwarding and pointer membership](docs/RA3EP1_READER_OWNERSHIP.md).
The ordinary helper tests reader-pointer membership in three global lists; it
does not validate asset contents. Wrapper source argument 2 supplies header,
first-loader factory dispatch and normal cleanup, while argument 3 supplies
the list owner. Both reviewed iterator branches forward node+8 source data
into the wrapper. Six membership fixtures, two invalid group counts, eleven
private code faults, repeat JSON and factory regressions pass. Specific head
store traversal/container lifetime and concrete root-reader provenance remain
open. No production/game proof; effort **52% / 48%**, compiler groups 165
(not rerun). Next list head movement/lifetime and initial root-reader binding.

[EP1 referenced-reader factory/cache/storage](docs/RA3EP1_READER_FACTORY.md).
Raw tag 2 uses virtual slot 2Ch; other tags use 28h. Concrete table targets,
new-reader construction and ordinary cache publication are pinned. Stack tracking
distinguishes factory-owner argument 3 from list-owner argument 4. Conditional
reader storage is at the existing head+8, not a proven fresh-node payload.
Six route fixtures, two invalid tags, eleven detached code faults, repeat JSON
and lifecycle regressions pass. Live owner-vtable/cache provenance and admission
semantics remain open. No production changes; effort **52% / 48%**, compiler
groups 165 (not rerun). Next outer-owner binding and stored-reader consumers.

[EP1 wrapper-local loader lifecycle](docs/RA3EP1_LOADER_LIFECYCLE.md).
Both wrapper dispatch branches pass a fresh zero-capacity context and join
normal entry/descriptor cleanup. The capacity-based producer therefore does
not reuse an earlier wrapper invocation's capacity. Concrete reader constructor
is pinned, but complete factory/cache-to-loader source binding remains open.
Repeat JSON, four fresh/zero-count fixtures, seven code faults and descriptor
regressions pass. No target/production changes; effort **52% / 48%**, compiler
groups 165 (not rerun). Next trace concrete reader factory/cache source binding.

[EP1 linked chunk addressing](docs/RA3EP1_LINKED_CHUNK_RANGES.md).
Queue/dispatcher/reader evidence connects descriptor-relative positions to
physical offsets +8: binary seek/read and relocation/import memcpy. All 166,557
ranges from 55,519 stock entries fit twelve sidecar lengths, with exact final
endpoints and zero payload reads. Repeat digests, four positive/four rejection
range fixtures, nine code/import faults and sidecar regressions pass. Native
read results/bounds are not checked in the reviewed method; our stricter audit
does not become engine policy. No production/game proof; effort **52% / 48%**,
compiler groups 165 (not rerun). Next concrete source binding/capacity lifecycle.

[EP1 linked sidecar setup](docs/RA3EP1_LINKED_SIDECARS.md).
Full helper review resolves 00449760 as sidecar preparation, not entry-table
transformation: it opens .imp/.relo/.bin, checks manifest checksums and advances
past eight-byte headers. Twelve authentic headers/exact sizes match, including
1,394,571,528-byte WorldBuilder.bin with zero payload reads. Repeat JSON, four
header faults, eight code faults and descriptor regressions pass. Magic checks
are diagnostic policy, not proven native rejection; no full payload or game
proof. Effort **52% / 48%**, compiler groups 165 (not rerun). Next trace linked
chunk positions/cursors and concrete source-object binding.

[EP1 concrete reader/descriptor producer](docs/RA3EP1_DESCRIPTOR_PRODUCER.md).
The recovered producer at VA 004496A0 connects 48-byte entries to 20-byte
descriptors with cumulative instance/relocation/import positions. A constructed
concrete reader uses a 52-byte physical header and count*48 table; four pinned
EP1 manifests/55,519 entries corroborate that layout and repeated projections.
All four are linked: post-read helper 00449760 remains unresolved, as does
capacity-reuse behavior. Fresh projections do not claim native linked loading.
Seven code faults, five raw rejections and entry-pointer regressions pass;
no target execution/production changes. Effort **52% / 48%**, compiler groups
165 (not rerun). Next review linked-entry transformation before full provenance.

[EP1 bounded entry-pointer trace](docs/RA3EP1_ENTRY_POINTER_TRACE.md).
The first hash-gate caller carries an entry pointer from a 20-byte descriptor
row through a 28-byte pending record to argument four; a separate 48-byte copy
branch and entry hash reads are pinned. The diagnostic wrapper forwards to the
global object's virtual slot 64h; its implementation/policy remains unknown.
Repeat JSON, three positive/two rejection stack cases, seven code faults and
hash-consumer regressions pass. Descriptor construction/disk provenance and the
other two caller traces remain open. No target execution or production changes;
effort **52% / 48%**, compiler groups 165 (not rerun). Next resolve the descriptor
producer and actual virtual source-reader implementation.

[EP1 runtime TypeHash consumer](docs/RA3EP1_TYPEHASH_CONSUMER.md).
An inline ResourceManager lookup compares entry+8 with registered metadata+8
at VA 004AB615 and reaches the explicit TypeHash mismatch diagnostic. Missing
metadata, nonzero entry word44 and zero entry hash skip this comparison; these
observations do not authorize compiler bypasses. Three separate small-lookup
call contexts consume opaque word12, not this hash. Repeat JSON, seven branch
cases, five private code faults and registry regressions pass. Complete stream
pointer provenance and unconditional rejection are not proved; no target or
production changes. Effort **52% / 48%**, compiler groups 165 (not rerun here).
Next recover bounded entry-pointer provenance and diagnostic control semantics.

[EP1 registry/lookup characterization](docs/RA3EP1_REGISTRY_LOOKUP.md).
Pinned shared registration and 34-byte linked-list lookup separate the TypeId
key from the metadata object's initialized hash. All 150 raw object TypeIds match
the managed FastHash provider; 138 match independently observed stock roots.
The opaque object+12 word differs from Tokenized in 122 cases and is never
relabelled. This lookup returns a metadata pointer and does not consume TypeHash.
Repeat/JSON, body/object faults and opaque preservation tests pass; initializer
regressions pass. No target execution/production changes. Effort **52% / 48%**,
groups 165; next follow lookup callers to actual returned-object hash consumers.

[EP1 runtime hash initializers](docs/RA3EP1_TYPE_INITIALIZERS.md).
Limited x86 decoding verifies 150 named hash-table reads/object+8 writes in
43 short and 107 vtable-writing initializer templates. Exact opcodes, mapped
operands and signed call targets are checked, with routine SHA/bytes retained.
Texture and five independently proven roots are covered; PathMusicEvent and
1,192 other table rows are not admitted by these templates. Repeat/JSON, eight
memory faults and artifact rejection pass; table/schema regressions pass.
No startup reachability, full metadata ABI or authoring ProcessingHash claim;
production guards stay closed. Effort stays **52% / 48%**, compiler groups 165.

[EP1 runtime/schema roles](docs/RA3EP1_RUNTIME_SCHEMA_ROLES.md).
All 1,342 runtime names match declared schema complex types: 299 direct asset
choices and 1,043 non-direct complex names; all 254 observed roots are direct
choices. Forty-eight schema-only names and both duplicate declarations remain
visible. Repeat/JSON, authored/runtime separation and XML fault tests pass.
New RA3Music.h candidate matches only 70/197 stock events, misses 123 and conflicts
on four zero-valued stock events; it is not the missing EP1 header replacement.
One bounded hash-block address-use target is recorded, with callers unverified.
No production/schema/parser guards changed. Effort stays **52% / 48%**, groups 165.

[EP1 runtime name/hash table reconciliation](docs/RA3EP1_RUNTIME_TYPE_TABLE.md).
Pinned runtime decoding recovers 1,341 parallel name/hash rows plus a separate
Texture slot. All 254 independently observed stock root fingerprints match;
weather enum pointers are explicitly excluded. Ordered/raw table fingerprints,
repeat/JSON, twelve memory faults and three public pin rejections pass. The
remaining 1,088 runtime names are not automatically admitted compiler roots.
No ProcessingHash, aggregate hash derivation or game-loading proof; production
guards remain closed. Estimated effort stays **52% / 48%**, groups stay 165.
Next map runtime names to official schema roles and audit actual table callers.

[EP1 executable music/type provenance](docs/RA3EP1_EXECUTABLE_MUSIC_EVIDENCE.md).
Static engine/launcher PE review pins build SHA/CodeView provenance and records
five music type-name pointer candidates, six stock TypeId matches and one stock
TypeHash match. These are bounded reverse-engineering targets, not a recovered
compiler ProcessingHash/header or verified type registry. Owned static-PE and
archive regression scripts pass; no target code executed. Compiler groups stay
165 and estimated effort stays **52% / 48%**; next establish the actual runtime
name/hash table relationship before considering any production identity change.

[EP1 archive source provenance inventory](docs/RA3EP1_ARCHIVE_SOURCE_EVIDENCE.md).
Read-only inventory of 14 shipping BIG directories covers 17,497 records with
2,174,196 metadata bytes read and zero payload reads. No named music header,
AUDIO source XML or compiler artifacts were found; embedded content was not
searched. Six duplicate multiplayer map names remain visible. Bounded scripts
and owned-fixture regressions pass; compiler coverage stays 165 groups and
effort stays **52% / 48%**. Authentic EP1 metadata/header recovery remains open.

[Actual selected PathMusic package v2](docs/RA3EP1_PATHMUSIC_SELECTED_PACKAGE_V2.md).
Explicit v2 commands publish actual Core-selected plugin native buffers after
frozen native/identity checks. Local TypeHash `48E303B8`, checksum `117BE305` and
exact v2 marker isolate old profiles; v1 bytes remain unchanged. Two-reader/native,
repeat/cross-profile, detached/corrupt/stale/late/no-overwrite tests pass; all 165
groups pass. Two-event CLI manifest/streams are 240/44/16/8 bytes, marker 682 bytes.
Effort **52% / 48%**; next authentic EP1 metadata and unresolved AUDIO/header
provenance, not another version label or production/game admission claim.

[Local PathMusic Core selection profile v2](docs/RA3EP1_PATHMUSIC_SELECTED_COMPILER_V2.md).
Explicit local TypeHash `48E303B8` is deterministically derived from the reviewed
native contract, not EA metadata. Core receives it before owner creation; unchanged
AddOutputInstance now selects owners and creates real dependency tables. Weak
self/cycles, missing file/target, native/hash invariance and checksum separation
pass; all 164 groups pass. Two-event CLI checksum `117BE305` repeats exactly.
Old zero-hash profiles/packages stay unchanged. No production/cache/reuse or game
admission; authentic EP1 ProcessingHash remains unrecovered. Effort **52% / 48%**;
next a separately versioned selected-owner experimental package/readback.

[Controlled in-memory PathMusic compiler](docs/RA3EP1_PATHMUSIC_CONTROLLED_COMPILER.md).
Fresh Core XML and private registered plugin dispatch match frozen music native
chunks/checksum. Mutation/platform/stale/output/cache/reuse guards pass; all 163
groups pass. Two-event CLI reports repeat exactly, with two processor calls and
checksum `CF2CABDA`. Important integration gate: Core's AddOutputInstance skips
zero TypeHash, so normal dependency/output selection remains unprepared.
`CoreOutputSelectionSkipped=true`; this memory-only path does not bypass that guard
or complete standard output integration. Effort **52% / 48%**; next explicitly
justified nonzero metadata and authentic EP1 processing/type-table provenance.

[PathMusic experimental Core package profile v1](docs/RA3EP1_PATHMUSIC_EXPERIMENTAL_CORE_V1.md).
Explicit versioned commands publish actual synthetic-domain Core InstanceHash and
padded identity checksum with zero type/catalog hashes. Distinct exact profile
markers and raw source/header fingerprints prevent cross-profile and unchanged-hash
provenance confusion. Two-reader/native, repeat/legacy-byte, corruption/stale/late
edit/no-overwrite tests pass; all 162 groups pass. CLI manifest/streams are
240/44/16/8 bytes; matching verification passes, cross-profile verification fails.
Effort **52% / 48%**; next controlled music processor integration, not stock
EP1 processing metadata or game-load admission.

[Bounded PathMusic Core checksum contract](docs/RA3EP1_PATHMUSIC_CORE_CHECKSUM.md).
Fresh synthetic-domain music identities now reproduce the unchanged Core output
checksum through an independent 20-byte-row/256-byte-padded layout. Order,
identity/type hash and strong-count sensitivity, direct weak/file exclusions,
stale/refreshed/restored inputs and bounds pass. All 161 groups pass. Two-owner
CLI actual/expected checksum is `CF2CABDA`; missing arguments return 1.
Existing package policy is unchanged. Effort **52% / 48%**; next separately
versioned experimental Core manifest profile, not production output.

[Current-Core PathMusic diagnostic publication gate](docs/RA3EP1_PATHMUSIC_CORE_PACKAGE_GATE.md).
Explicit Core-bound package/verify commands require current actual Core/native
binding before staging and commit/readback. Stale/late input edits and competing
destinations refuse; failed staging is retained. All 160 groups pass. Serialized
bytes remain identical to the older diagnostic profile: no implicit Core hash
substitution or historical publication attestation. CLI package/verify pass with
240-byte manifest and 44/16/8 streams. Effort **52% / 48%**; next explicitly
versioned synthetic-Core manifest identity/checksum profile, not production output.

[Immutable Core/native PathMusic preparation](docs/RA3EP1_PATHMUSIC_CORE_PREPARATION.md).
Local preparation pairs independently verified actual Core identities with the
same frozen native closure. Current Core/raw checks surround serialization;
timestamp-preserving edits, native-equal comments/default provenance and missing
headers refuse until explicit refresh/restoration. Detached reports/chunks cannot
mutate captured evidence. All 159 groups pass; no package identity relabeling,
EP1 processing hash recovery or production admission. Effort **52% / 48%**;
next explicit Core-bound experimental publication gate and independent readback.

[Pinned reference PathMusic processing metadata](docs/RA3EP1_PATHMUSIC_REFERENCE_IDENTITY.md).
Static field/branch/dispatcher checks recover RA3 reference ProcessingHash
`76D0CEE6`, TypeHash `76D0CEEF`, catalog `54EEE764` and HasCustomData=false.
TypeHash differs from observed EP1 `599CDAF2`; this is not EP1 processing hash
recovery or permission to relabel diagnostic packages. No DLL execution.
All 158 groups pass. Effort **52% / 48%**; next immutable Core/native binding
under a declared domain, with EP1 metadata/authentic dependencies still open.

[PathMusic actual Core identity proof](docs/RA3EP1_PATHMUSIC_CORE_IDENTITY.md).
Actual Core InstanceHash matches independent authored XML/512-block and padded
weak-type/header folding for 1–8 owners under an explicitly synthetic processing
domain. Shared header edits invalidate all owners; omitted/default cache attributes
retain different XML hash provenance from explicit true despite equal runtime
values. Physical directory relocation does not alter identities. All 157 groups
pass; no recovered EA ProcessingHash or package relabeling. Effort **52% / 48%**;
next reference processing metadata and immutable Core/native preparation binding.

[Local PathMusic diagnostic package](docs/RA3EP1_PATHMUSIC_PACKAGE_PROBE.md).
Explicit package/verify commands frame 1–8 local music owners with two-reader
metadata/native readback, weak-ID closure and linked offsets. TypeHash and
AllTypesHash are zero; diagnostic SHA prefixes are not EA/Core processing hashes.
Corrupt/missing/orphan/stale inputs and existing destinations refuse. All 156
compiler groups pass; CLI two-event package has 44/16/8-byte linked streams.
No official AUDIO/production/game admission. Effort **52% / 48%**; next actual
Core/reference music processing identity and authentic dependency binding.

[Local authored PathMusic snapshot](docs/RA3EP1_PATHMUSIC_AUTHORED_SNAPSHOT.md).
The explicit local events.xml/events.h profile validates 1–8 owners, exact local
weak alternates and canonical nonzero header literals, then freezes raw inputs and
detached native hashes. Same-size/timestamp-preserving edits invalidate old
preparation. Diagnostic fingerprint is not a production processing hash; no AUDIO
dependency bypass or package output. Reproducible two-event synthetic fixture and
all 155 compiler groups pass. Effort **52% / 48%**; next isolated package readback,
with authentic dependencies, Core identity and game-loading gates still open.

[Isolated PathMusic runtime serialization](docs/RA3EP1_PATHMUSIC_RUNTIME_PROBE.md).
Actual Win32 tracker output matches all 197 stock records (19 alternates, four
zero event words), including full BIN/RELO and no imports. This replays observed
stock values and does not recover a header. Separate synthetic header fixtures
prove strict unique/nonzero literal preparation against independent native goldens.
Runtime schema type is not an authored AssetDeclaration root; production schema
and processor registration stay unchanged. All 154 groups pass. Expanded graph
still 398 valid / four AUDIO issues / 198 header occurrences, no output/game proof.
Effort **52% / 48%**; next scoped input/identity and isolated package readback.

[Reference PathMusic header semantics](docs/RA3EP1_PATHMUSIC_HEADER_SEMANTICS.md).
Pinned static IL identifies the header parser and event processor: the runtime
event word comes from a hexadecimal `PATH_EVENT_<id>` definition, not an asset
symbol hash. Legacy missing/zero results warn and continue with zero; that policy
is not enabled for EP1 production. Explicit reference/header review commands and
a conservative managed diagnostic model retain first-match/zero distinctions,
lexical hazards and unsupported CRT results. No reference DLL execution or
authentic header recovery. All 153 groups pass; graph/dependency coverage and
effort **52% / 48%** unchanged. Next: authentic header or separately proved stock
binding, then content invalidation/zero policy/native emission checks.

[PathMusic authored/stock reconciliation](docs/RA3EP1_PATHMUSIC_STOCK_REVIEW.md).
Read-only `pathmusic-stock-review` matches all 197 authored names/instance IDs
and observes 178 plain 16-byte records plus 19 relocated 20-byte alternate records.
Only 3,228 BIN / 152 RELO bytes per pass; repeated reports agree. Offset-4 event
words differ from every asset ID and are zero in four records: no guessed hash
algorithm or synthesized header. A 94-file RA3 audio collection was found, not
admitted as complete EP1 AUDIO input. All 152 compiler groups pass; graph stays
398 valid / four AUDIO path issues / 198 header occurrences. Effort **52% / 48%**,
no processor/native emission/game proof. Next: reference PathMusic header lookup
and zero-event semantics, or an independently proved stock-reference contract.

[Known map Include aliases](docs/RA3EP1_SDK_KNOWN_MAP_ALIASES.md).
Explicit `--known-map-aliases` with `--instance-sound-singletons` admits only
two exact library/all paths after native Windows identity and captured-source
proof. Both map closures were independently prepared first; no generic path
normalization or source rewrite. Real graph: **398 validated / 0 XML blockers**,
666 Includes, four AUDIO path issues, 198 occurrences of one required header.
Zero earlier-valid regressions, raw hash mismatches or repeated-run differences;
no output/native/game proof. All 151 compiler groups pass. Effort stays
**52% / 48%**. Next: authentic AUDIO inputs and PathMusic header/processor contract.

[Remaining dependency/path classification](docs/RA3EP1_SDK_DEPENDENCY_REVIEW.md).
Read-only `sdk-dependency-review` identifies 198 occurrences of one required
PathMusic header, not 198 proved missing files. Four path issues have no explicit
AUDIO root; two known trailing-dot map Includes have equal ordinary/dotted
Windows snapshots but remain outside resolver admission and the captured graph.
Stock manifests contain compiled music/audio assets, not replacement source
Includes or headers. All 150 compiler groups pass; no resolver/reference changes.
Graph remains **396/396 captured XML valid**, six path issues and 198 dependency
occurrences; effort **52% / 48%**, no production/game readiness. Next: prepare
both uncaptured map sources before a narrow alias opt-in, and recover AUDIO inputs.

[Known sound singleton admission](docs/RA3EP1_SDK_INSTANCE_SOUND_SINGLETONS.md).
Independent `--instance-sound-singletons` admits only the two proved complete
literal sound bodies and verifies every explicit final owner field/child after
the actual Core merge. Real graph: **396 validated / 0 XML blockers**, zero
earlier-valid regressions. SoundEffects has 1800 overlays, 353 substitutions,
276 calculations and two singleton witnesses; repeated hash/post-audit checks
agree. Earlier sound-offset scope stays 395 / 1. All 149 compiler groups pass.
Six path issues and 198 missing dependency occurrences still prevent complete
closure; no output/native emission/game proof. Effort is now **52% / 48%**.
Next: classify remaining dependencies and paths before changing resolver rules.

[Complete isolated sound owner review](docs/RA3EP1_SDK_SOUND_OWNER_REVIEW.md).
Read-only `sdk-sound-owner-review` independently predicts and verifies every
explicit root field, direct child/order and Sound reference of the two pinned
owners after existing guarded BaseSoundEffect preparation. Both complete XML
owners pass final schema checks; two real runs have identical reports. This is
not graph admission: the existing profile still refuses the whole source, and
the last measured graph remains **395 valid / 1 blocked**. All 148 compiler
groups pass. Reference XML/XSD/Core unchanged; native owner byte emission and
game loading unproved. Effort stays **51% / 49%**. Next: separately scoped
singleton admission with complete whole-source/graph atomic validation.

[Sound singleton Core/native characterization](docs/RA3EP1_SDK_SOUND_SINGLETON_SEMANTICS.md).
Owned fixtures against the reviewed EP1 schema prove ordered field-level overlay:
later explicit values win, but omitted fields retain earlier/inherited values.
Selected stock native slices independently corroborate PitchShift -1 / 1 and
NonInterruptibleTime 0 / 0.8 seconds for the two known conflicting owners.
This does not expand preprocessing admission. The last measured graph remains
**395 valid / 1 blocked**; full owner/base projection must be proved next.
All 147 compiler groups pass. Source/schema/Core remain unchanged; no game or
production emission proof. Effort stays **51% / 49%**. A full rebuild reports
three existing Core/XmlCompiler warnings, zero errors; no new test warning.

[Typed sound integer offsets and expression-stage review](docs/RA3EP1_SDK_INSTANCE_SOUND_OFFSETS.md).
Independent `--instance-sound-offsets` admits only schema-checked integer
arithmetic on selected AudioEvent/AudioEventOverridable and singleton PitchShift
fields, with per-field bounds and actual Core result checks. A separate read-only
review of SoundEffects records **353 substitutions / 276 calculations**, with
identical hashes on two runs. Whole graph remains **395 valid / 1 blocked**:
two conflicting singleton repeats prevent SoundEffects owner validation. Earlier
audio-tree scope remains 395 / 1; no earlier-valid regressions. All **146 compiler
groups** pass. Reference XML/XSD/Core remain unchanged; no native/game proof.
Effort stays **51% / 49%** because this expression-stage advance does not close
the owner, native-layout or game-loading gates. Next: independently characterize
the unchanged Core's handling of the two singleton repeats before any admission.

[Bounded shallow audio document breadth](docs/RA3EP1_SDK_INSTANCE_AUDIO_TREES.md).
Independent `--instance-audio-trees` permits up to 16,384 elements only for
proved shallow audio owners, with per-owner/node/depth/attribute/metadata and
aggregate merge-pair limits. Voice.xml validates with 1,698 overlays and 884
substitutions: **395 valid / 1 blocked**, zero earlier-valid regressions. Earlier
music scope stays 394 / 2 and retains its 8192 tree limit. All 145 compiler groups
pass; reference XML/XSD/Core remain unchanged. At that milestone, SoundEffects
arithmetic remained closed; no native/game proof. Effort stayed **51% / 49%**.

[Bounded MusicTrack Volume offsets](docs/RA3EP1_SDK_INSTANCE_MUSIC_OFFSETS.md).
Independent `--instance-music-offsets` allows only direct MusicTrack.Volume
integer offsets with exact Percentage schema type and operands/results in 0..100.
Music.xml now validates; its authored `70 + 5` resolves to `75` before inheritance
and is verified again against the actual Core output. Real graph: **394 valid /
2 blocked**, zero earlier-valid regressions. Earlier cross-removal scope stays
393 / 3. All 144 compiler groups pass. SoundEffects arithmetic and Voice's tree
bound remain closed; native emission and game loading are unproved.
Effort stays **51% / 49%**. Next: audio tree/merge-complexity review and separately
typed SoundEffects arithmetic; XML counts are not usable-mod readiness.

[Empty cross-QName state removal admission](docs/RA3EP1_SDK_INSTANCE_CROSS_STATE_REMOVALS.md).
Independent `--instance-cross-state-removals` admits one empty authored BuildState
Remove against a unique inherited StrategicState per AI owner, with exact schema
reference types and actual-core agreement on all state fields/order. Whole CC32
now validates: **393 valid / 3 blocked**, zero earlier-valid regressions. Earlier
state-readd scope stays 392 / 4. Replacement, reverse direction and mixed
cross-removal/re-add owners remain closed. All 143 compiler groups pass; no
reference XML/XSD/core edits, native emission or game proof. Effort stays
**51% / 49%**. Next: bounded audio-expression inventory and Voice tree-limit review.

[Pinned CC32 source/core/native-metadata review](docs/RA3EP1_SDK_CC32_REVIEW.md).
Read-only `sdk-cc32-review` proves that the unchanged core removes CC32's inherited
StrategicState via its authored BuildState Remove and yields a schema-valid
isolated owner. The stock native owner's 109 manifest references omit the exact
target identity. This corroborates removal but does not decode the native state
layout or authorize generic cross-QName operations. No admission expands:
**392 valid / 4 blocked**, effort **51% / 49%**. All 142 compiler groups pass.
Next: separately prove a tightly bounded removal-only scope with exact resolved
target, QName/reference-type evidence and predicted core result; no XML renaming.

[Ordered StrategicState Remove/re-add admission](docs/RA3EP1_SDK_INSTANCE_STATE_READDS.md).
Independent `--instance-state-readds` retains original commands, requires an
existing exact-QName resolved target and checks the unchanged core against a
predicted complete state-field/order projection. Gibraltar validates with eight
pairs and CC01 with one: **392 valid / 4 blocked**, zero earlier-valid regressions.
Earlier identical-state scope stays 390 / 6; cross-QName removal remains closed.
All 141 compiler groups, 33 enum checks, three classifier fixtures and three
CLI isolation probes pass. Final build has zero warnings/errors; reference
XML/XSD/core files are unchanged. No output/native/game proof; effort **51% / 49%**.
Remaining: Music/SoundEffects expressions, Voice inventory bound and CC32's
cross-QName command. Next: inspect CC32 reference/native evidence before admission.

[Identical StrategicState admission](docs/RA3EP1_SDK_INSTANCE_IDENTICAL_STATES.md).
Independent `--instance-identical-states` folds only bounded, literal, identical
empty StrategicState siblings after proving the unchanged core's actual result.
The complete AIP_S04_AlliedGroundBase owner now validates: **390 valid / 6 blocked**,
zero earlier-valid regressions. Conflicting repeats, ordered Remove/re-add and
cross-QName commands remain closed; earlier expression scope stays 389 / 7.
All 140 compiler groups, 33 enum checks, three classifier fixtures and three
CLI isolation probes pass; final build has zero warnings/errors. Reference
XML/XSD and core implementation are unchanged; no output/native/game proof.
Manual effort stays **51% / 49%**. Next: separately prove ordered state Remove/re-add.

[Sibling identity operation review](docs/RA3EP1_SDK_SIBLING_IDENTITY_REVIEW.md).
The remaining AI identity blockers split into identical duplicate states,
ordered Remove/re-add pairs and cross-QName removal. New core fixtures prove
that ordinary overlay is not equivalent to Remove/re-add: old-only fields and
entry order differ. Final schema validation can pass after destructive keyed
operations. No identity guard is relaxed; real counts stay **389 valid / 7 blocked**.
All 139 compiler groups pass. Manual effort stays **51% / 49%**; no native/game
proof. Next: independent narrowly bounded identical-state coalescing, followed
by separately proved ordered Remove/re-add support; cross-QName removal stays closed.

[Expressions before guarded inheritance](docs/RA3EP1_SDK_INSTANCE_EXPRESSIONS.md).
Independent `--instance-expressions` resolves captured definitions in each
source's own context before overlays. The complete ObjectCreationLists owner
now validates: 54 substitutions, one local overlay and one consumed undeclared
marker. Imported non-inheritable bases and identity/directive expressions remain
closed. Real counts reach **389 valid / 7 blocked**, with zero earlier-valid
regressions. No source/schema/core edits or output/native/game proof.
All 138 compiler groups, 33 enum checks, three classifier fixtures and three
expression CLI isolation probes pass. Final build has zero warnings/errors.
Effort stays **51% complete / 49% remaining**: native compatibility and game
loading remain open. Next: characterize the remaining sibling-ID collisions.

[Definition-only leaf Include admission](docs/RA3EP1_SDK_INSTANCE_METADATA.md).
Independent `--instance-metadata` proves bounded all-Include definition leaves
with zero asset exports. ObjectCreationLists now passes its Include-role gate,
then stops at unsupported expressions in inherited assets. Its 54 expression
attributes need a separately proved expressions-before-overrides step. Real
counts remain **388 valid / 8 blocked**, with zero earlier-valid regressions.
No partial witness/output/native/game proof; effort stays **51% / 49%**.
All 137 compiler groups, 33 enum checks, three classifier fixtures and three
metadata CLI isolation probes pass. Final build has zero warnings/errors.

[Complementary upgrade singleton admission](docs/RA3EP1_SDK_INSTANCE_UPGRADES.md).
Independent `--instance-upgrades` normalizes only bounded, disjoint complementary
GameDependency pairs and checks the unchanged core's actual attribute union and
ordered references. The entire prepared Upgrade library now validates:
**388 valid / 8 blocked**, zero earlier-valid regressions. Older profiles remain
unchanged. Source/schema files are untouched; no output/native/game proof.
Manual effort stays **51% / 49%**. Next: ObjectCreationLists Include visibility.
All 136 compiler groups, 33 enum checks, three classifier fixtures and three
upgrade CLI isolation probes pass. Final build has zero warnings/errors.

[Upgrade singleton normalization review](docs/RA3EP1_SDK_UPGRADE_REVIEW.md).
The unchanged core folds the two Allied upgrades' complementary GameDependency
pairs without dropping their reference or unpacking condition; both isolated
owners validate against the pinned EP1 schema. Conflict fixtures also prove
that final validation alone can hide last-write-wins data loss. This is a partial
read-only experiment, not a new admission profile: real graph counts stay
**387 valid / 9 blocked**, and manual effort stays **51% / 49%**.
Next: separately proved bounded complementary-singleton normalization.
All 135 compiler groups, 33 enum checks, three classifier fixtures and three
review CLI refusal probes pass. Final build has zero warnings/errors.

[One-sided matched ObjectFilter copying](docs/RA3EP1_SDK_INSTANCE_FILTERS.md).
The independent `--instance-filters` profile proves ordered weak-reference
payload copying for matched AIMicroManagerData/IgnoreTargets branches, then
checks the unchanged core's actual result. Two populated sides remain closed.
Real admission reaches **387 valid / 9 blocked**, with zero earlier-valid
regressions. AIMicroManagerLibrary validates without changing reference schemas
or the core joiner. Six path issues and 198 missing-header references remain;
no output/native/game proof. Manual effort stays **51% / 49%**.
All 134 compiler groups, 33 enum checks, three classifier fixtures and three
filter CLI isolation probes pass. Next: the Upgrade XML/schema occurrence conflict.

[Whole-token-proven bitflag modifiers](docs/RA3EP1_SDK_INSTANCE_BITFLAGS.md).
The independent `--instance-bitflags` option permits only bounded signed
VitalKindOf/ForbiddenKindOf operations on inherited AITargetingHeuristic assets,
after proving token semantics agree with the unchanged core's substring logic.
Real admission reaches **386 valid / 10 blocked**: AITargetHeuristicLibrary now
validates, with five operations and zero earlier-valid regressions. Missing
base fields and substring collisions remain closed. Six path issues and 198
missing-header references remain; no output/native/game proof. Manual effort
stays **51% complete / 49% remaining**. Next: bounded ObjectFilter branch matching.
All 133 compiler groups, 33 enum checks, three classifier fixtures and three
bitflag CLI isolation probes pass. Final build has zero warnings/errors.

[Consumed inheritance markers](docs/RA3EP1_SDK_INSTANCE_MARKERS.md).
The independent `--instance-markers` profile reproduces core declaration-time
inheritFrom consumption, with undeclared markers limited to two reviewed local
AI types. Imported non-inheritable bases remain closed. Relevant XSD files match
the Uprising references; no schema edit is needed for this pipeline step.
Real admission stays **385 valid / 11 blocked**, with zero regressions: the two
AI libraries now reach later bitflag-list and matched-branch guards, rather than
the undeclared marker guard. Counts do not rise until those separate semantics
are supported. Manual effort stays **51% complete / 49% remaining**; no game proof.
All 132 compiler groups, 33 enum checks, three classifier fixtures and three
marker CLI isolation probes pass. The full real audit was repeated for both
choice and marker profiles; neither creates an output directory.

[Bounded repeated-choice copying](docs/RA3EP1_SDK_INSTANCE_CHOICES.md).
The separate `--instance-choices` option admits only nested flat repeated choices
with unit alternatives, aggregate cardinality checks and existing strict tree/
identity/source guards. Real admission advances to **385 valid / 11 blocked**:
all eleven reviewed owners newly validate, with zero earlier-valid regressions.
Older removal-only admission stays 374 / 22. Singleton choice replacement and
populated-branch matching remain closed. Six path issues and 198 uses of one
missing AUDIO header remain; no output/native stream/game proof. Manual effort
stays **51% complete / 49% remaining**, not 385/396 working-mod readiness.
All 131 compiler groups, 33 enum checks, three classifier fixtures and three
choice CLI isolation probes pass. Final build has zero warnings/errors.
Next is reviewing XML/XSD inheritance mismatches in the two AI libraries;
the undeclared field is inheritFrom, not a newly discovered native payload field.

[Choice-particle behavior review](docs/RA3EP1_SDK_CHOICE_REVIEW.md).
The new regression group characterizes anonymous repeated rules and their order,
choice aggregate cardinality, cross-QName ID replacement, same-key text append,
and destructive singleton replacement. Existing profiles remain closed to choice
particles. Read-only inspection identifies 69 authored Heuristic blocks across
ten blocked owners; the eleventh inherits through three direct Includes. No
choice profile is enabled yet, so admission remains **374 valid / 22 blocked**.
All 130 compiler groups, 33 enum checks and three classifier fixtures pass;
the full real-source audit was repeated and created no output directory.
This review does not increase the manual **51% complete / 49% remaining** estimate.

[Bounded keyed child removal](docs/RA3EP1_SDK_INSTANCE_REMOVALS.md).
The independent `--instance-removals` flag admits exact literal Remove/id stubs
only for existing same-QName repeated empty-complex children. Real admission now
reaches **374 valid / 22 blocked**, with 120 newly valid documents and zero
earlier-valid regressions. 105 validated documents record 481 owner-authored
removals; consumed directives do not survive in processed XML. All 129 groups,
33 enum checks and three classifier fixtures pass; the full suite required
permission for an existing owned temporary audio-file move. Final build has zero
warnings/errors. Six path issues and 198 uses of one missing AUDIO header remain;
reference bytes and older profile admission are unchanged, no output/game proof.
The subsequent choice review above identifies the exact particle shapes affecting
11 remaining documents; bounded choice-copy admission is the next implementation.
Manual weighted effort is reassessed to about 51% complete / 49% remaining.

[Recursive direct-instance preparation](docs/RA3EP1_SDK_INSTANCE_CHAINS.md).
The separate `--instance-chains` profile prepares each child in its own scope and
exposes only its own declarations, not transitive handles. Full per-owner/per-base
raw/processed source closures, cache-aware depth, cycle/stale and prepared-byte
guards are tested. Real admission advances to **254 valid / 142 blocked**: 47 newly
valid documents (34 maps, 12 SkirmishAI, one Sounds), zero earlier-valid regressions.
MissionDialogue.xml now has 1,272 overlays. All 128 default groups, 33 enum checks
and three classifier fixtures pass. Six path issues and 198 uses of one missing
AUDIO header remain. Older profiles are unchanged; no output/game proof. The next
concrete scope is bounded child-removal directives. Effort remains about 50.25%,
rounded 50%; this measures XML preparation progress, not a usable SDK release.

[Local empty-child matching](docs/RA3EP1_SDK_SELF_CHILD_MERGE.md).
The independent `--self-child-merge` flag adds singleton-name/repeated-ID matching
only for empty complex children. Core matched text appends rather than replaces;
text/branch matching and cross-QName ID collisions remain closed. All 127 default
groups, 33 enum checks and three classifier fixtures pass. Real local admission
stays 205 valid / 191 blocked with zero earlier-tree regressions; the unchanged
root-file profile remains the best 207 / 189. Include chains remain closed. No
output/game proof; weighted effort stays about 50.25%, rounded 50%.

[Direct-instance Include-chain triage](docs/RA3EP1_SDK_INSTANCE_INCLUDE_BLOCKERS.md).
The 178 first Include-free blockers reduce to 23 shared imported sources; all have
instance-only child edges but all remain independently blocked. The largest three
account for 69 owners. A read-only classifier and three owned script fixtures
make this reproducible. Recursive preparation alone will not solve populated-child
merging in the inspected personality chain. Admission stays 207 valid / 189 blocked;
compiler groups remain 126 and the effort estimate remains about 50%.

Previous direct-only admission milestone: [Inherited explicit-root file fields](docs/RA3EP1_SDK_INSTANCE_ROOT_FILES.md).
The separate `--instance-root-files` profile admits imported DATA/ART/AUDIO fields
under explicit diagnostic roots, while keeping imported relative paths closed.
PathMusicEvents.xml now validates with 197 overlays: **207 valid / 189 blocked**.
The unresolved typed field count becomes 198 occurrences of the same AUDIO header,
not 198 distinct missing files. Root availability, confinement and file existence
remain separate gates; native wildcard/postfix search and file-hash rewriting are
not emulated. All 126 default groups and 33 enum checks pass. Reference bytes, six
path issues and earlier-profile admission remain unchanged; no output/game proof.
English reports are updated. Weighted effort stays about 50.25%, rounded 50%.

[Narrow direct-instance inheritance](docs/RA3EP1_SDK_INSTANCE_INHERITANCE.md).
The explicit `--instance-inheritance` profile admits rechecked direct-instance,
Include-free base documents with BaseInheritableAsset-derived types and no imported
file fields. Source-local chains are expanded before copy-only owner overlays;
temporary bases never appear in the returned owner XML. Voice_RetailOnly.xml now
validates after 102 overlays using BaseUnitResponse: **206 valid / 190 blocked**.
No earlier-valid document regresses. Transitive/all/reference visibility, inherited
file-field provenance, populated-child merging and general expressions remain
closed. All 125 default groups and 33 enum checks pass; English documentation is
updated, reference bytes and six path issues unchanged, no output/game build proof.
Weighted effort stays about 50.25%, rounded 50% complete / 50% remaining.

[Bounded one-sided sequence-tree inheritance](docs/RA3EP1_SDK_SELF_TREE_COPY.md).
The independent `--self-tree-copy` profile admits recursive direct-sequence children
and bounded IDs unique across all direct siblings, without populated-child merging.
Owned tests prove inherited/anonymous/recursive schema lookup, order, attributes,
32-level depth and 8,192-element bounds. The real graph **stays 205 valid / 191
blocked** with zero earlier-valid regressions: opening tree/ID gates exposes 65
documents first blocked on unavailable same-document bases. Include inheritance
remains closed. Earlier profiles, reference bytes, six path issues and the typed
AUDIO header are unchanged. All 124 default groups and 33 enum checks pass; no
output or game build is claimed. Weighted effort stays about 50.25%, rounded 50%.

[One-sided complex leaf inheritance](docs/RA3EP1_SDK_SELF_COMPLEX_CHILD_COPY.md).
The independent `--self-complex-child-copy` profile admits attributed simpleContent
and empty-content children on only one side of local overlays. BaseSoundEffect.xml
(four overlays) and Speech.xml (one) newly validate: **205 valid / 191 blocked**.
Nested/mixed children, child IDs, populated-child merging and imported bases remain
closed. Earlier profiles and reference bytes are unchanged. Six path issues and
the typed AUDIO header remain; no output or game build is claimed. All 123 default
groups and 33 enum checks pass. Weighted effort stays about 50.25%, rounded 50%.

[One-sided flat child inheritance](docs/RA3EP1_SDK_SELF_CHILD_COPY.md).
The explicit `--self-child-copy` profile admits simple sequence children from
only one side of a same-document overlay, using the existing core joiner. Real
AmbientStream.xml validates after ten overlays; formatting whitespace follows
core's default parsing behavior. The graph is now 203 valid / 193 blocked.
Complex/populated-child merges and imported bases stay closed. Earlier profiles,
reference bytes and six path issues remain unchanged; no output or game build
is claimed. All 122 default groups and 33 enum checks pass. Weighted effort stays
about 50.25%, rounded 50% complete / 50% remaining.

[Core-backed local attribute inheritance PoC](docs/RA3EP1_SDK_SELF_ATTRIBUTE_INHERITANCE.md).
The explicit `--self-attribute-inheritance` profile uses core NodeJoiner for
same-document/same-type expression-free leaf assets, with atomic rejection and
cycle/depth/amplification guards. Owned fixtures prove inherited required attributes
and typed file binding. The real graph stays 202 valid / 194 blocked: 191 documents
first encounter child content and three lack local bases. No real inherited document
is newly validated; broader child/Include semantics remain open. Earlier profiles,
reference files and missing path/resource gates are unchanged. All 121 default
groups and 33 enum checks pass; no output or game build is claimed. Effort stays
about 50.25%, rounded 50% complete / 50% remaining.

[Bounded GlobalDefines expression subset](docs/RA3EP1_SDK_DEFINITION_SUBSET.md).
The explicit `--definition-expressions` profile admits only observed backward
aliases, one suffix concatenation and checked integer multiply-add definitions.
GlobalDefines resolves five expressions (numeric results 440/640); base/EP1
PlayerTemplates now validate after 9/2 substitutions. The 396-document graph has
202 valid / 194 inheritFrom-blocked documents. Six path issues and the typed AUDIO
header remain; no output or production build is claimed. This is not general EA
evaluator equivalence. Raw/local/Include-literal profiles and reference bytes are
unchanged. All 120 default groups and 33 enum checks pass. Effort stays about 50.25%.

[Source-backed Include literal definitions](docs/RA3EP1_SDK_INCLUDE_DEFINES.md).
The explicit `--include-defines` profile imports rechecked literal definitions via
all/instance Includes, preserves origin-aware duplicate rules and keeps reference,
override, arithmetic and inheritance gates closed. Real AudioSettings (six
substitutions) and GameLOD (one) now validate; Eva retains its 534 substitutions.
The 396-document graph has 200 valid / 196 preprocessing-blocked documents.
PlayerTemplates still requires general definition evaluation; six source-path
issues and the typed AUDIO header remain. Raw/local defaults and reference files
are unchanged. All 119 default groups and 33 enum checks pass; no output or game
build is claimed. Weighted effort remains approximately 50.25%.

[Bounded local define diagnostics](docs/RA3EP1_SDK_LOCAL_DEFINES.md).
The explicit typed-graph `--local-defines` profile resolves 534 local expressions
in real `Sounds/Eva.xml`, without modifying sources or invoking the EA evaluator.
Of 396 reachable documents, 198 validate and 198 require preprocessing (194
inheritance blocks plus four imported-definition blocks). Only Eva's raw schema
failure is resolved; reclassification of the other four is not a fix. Six original
path issues and the typed AUDIO header remain. No output is created; this is not a
production build. Raw default behavior and reference files are unchanged.
All 118 default groups and 33 enum checks pass. Weighted effort stays about 50.25%.

[Rechecked typed source graph](docs/RA3EP1_SDK_TYPED_SOURCE_GRAPH.md).
`sdk-typed-source-graph` rechecks the reachable XML fingerprints and binds them to
one explicitly reviewed compiled schema. Real global.xml yields 396 documents:
197 validate as raw XML, 194 require inheritFrom preprocessing, and five fail raw
schema validation with unevaluated expressions. One typed AUDIO header path still
requires a physical root; six original path issues remain. No limit was hit or
output created. All 117 default groups and 33 enum checks pass. This is not complete
source/payload closure or a production build. Weighted effort remains about 50.25%.

[Reviewed dummy-hook schema admission](docs/RA3EP1_SDK_REVIEWED_HOOKS.md).
`sdk-reviewed-schema-candidate` admits diagnostic binding only for the exact pinned
Shield candidate and two exact VertexData/Handle warnings after effective-attribute
and positive/negative source probes. Warnings remain visible; clean-schema status
remains false. Real global.xml and BasePathMusicEvent.xml validate; the latter binds
its AUDIO PathfinderEventHeader. The 16 effective file attributes do not prove payload
availability, complete Include closure or processor readiness. All 116 default groups
and 33 enum checks pass. Default/unreviewed gates and reference inputs are unchanged.
Overall weighted effort remains approximately 50.25%, rounded to 50%.

[Pinned in-memory Shield schema candidate](docs/RA3EP1_SDK_SHIELD_CANDIDATE.md).
`sdk-shield-schema-candidate` removes only the fingerprint-reviewed second Shield
definition in captured memory and records separate original/candidate digests.
The 837-XSD candidate now structurally compiles with zero errors and two prohibition
warnings (VertexData/Handle); clean-schema admission and real source binding stay
closed. Default `sdk-effective-schema` still rejects the original duplicate.
Reference schemas are untouched. All 115 default groups and 33 enum checks pass,
including the existing Yuriko shield 460/12/0 graph regression.

[Effective schema and typed-source gate](docs/RA3EP1_SDK_EFFECTIVE_SCHEMA.md).
`sdk-effective-schema [absolute-source.xml]` compiles captured XSD Includes with
external resolution disabled. Valid synthetic schemas prove inherited/group/
prohibited attributes, xsi:type and DataBlob element binding. The real 837-XSD
Include closure fails: ShieldSphereUpdateModuleData is declared twice in one file.
The staged and external reference file hashes match; reference XSDs remain untouched.
Real global.xml binding is explicitly skipped, not passed. All 114 default groups
and 33 enum checks pass. This closes a diagnostic blind spot, not the schema/build
gate; effort remains approximately 50% complete / 50% remaining.

[Schema file-reference declaration inventory](docs/RA3EP1_SDK_FILE_REFERENCE_CATALOG.md).
`sdk-file-reference-catalog` inventories 17 declared file fields across 843 staged
schemas, including five DataBlob fields, namespace-aware restriction ancestry and
pipeline-only OnDemandTexture.File evidence. All 113 default groups and 33 enum
checks pass. The two trailing-dot map Includes have normal `.xml` files on disk;
searched Uprising reference/unpacked roots lack the three audioassets XMLs and
RA3EPMus.h. The 1,514 numbered WAV tracks and compiled EnglishAudio stream are not
an original AUDIO source root. Inherited/instance binding remains open; the effort
estimate is unchanged at approximately 50% complete / 50% remaining.

[Bounded SDK source-path preflight](docs/RA3EP1_SDK_SOURCE_PATH_PREFLIGHT.md).
`sdk-source-preflight` adds reachable Include attribution and optional explicit
ART/AUDIO roots without compiling or reading resource payloads. A real global.xml
audit visited 396 XML documents and retained 664 Include edges without hitting a cap.
It returned incomplete: three AUDIO Includes and one AUDIO header lack an explicit
root, and two map Includes end in `.xml.` outside the literal safe path profile.
Reference files remain unchanged. All 112 default groups and 33 enum checks pass.
This is not schema-complete dependency coverage or game compatibility; effort remains
about 50% complete / 50% remaining. The original environment-only wrapper is unchanged.

[Explicit target-aware SDK environment preflight](docs/RA3EP1_SDK_ENVIRONMENT_PREFLIGHT.md).
`sdk-preflight` and `scripts/Test-Ra3Ep1SdkEnvironment.ps1` validate explicit EP1
schema/source/output paths and external manifest mappings without registry fallback,
builder launch or output writes. The staged 843-XSD byte catalog must match; wrong
targets/custom schemas, overlap, DTDs, traversal, patch bases and oversized metadata
reject. Real external Uprising global.xml and global/static manifest preflight passed
with output still absent. All 111 managed groups and 33 enum checks pass. This is
snapshot-only metadata planning, not Include resolution, production build readiness
or a usable Uprising SDK. The overall effort estimate remains about 50%.

[Supervised duration AudioEvent mixed packages](docs/RA3EP1_AUDIO_DURATION_EVENT.md).
The new `supervised-audio-duration-event` command freezes version-2 duration leaves
plus event.xml. Singleton, late/reversed subset and all-eight maximum streamed
selections pass actual core/native XAS, independent parent reconstruction and both
package readers. Selected payload changes invalidate event fingerprints; unselected
payload changes do not. Matching-inventory selector/weight/import/custom/source
corruption rejects. All 110 managed groups pass. Canonical and leaf-only commands
keep their own admission contracts; production processing and game loading remain open.

[Supervised bounded-duration AudioFile pools](docs/RA3EP1_AUDIO_DURATION_POOL.md).
Separate version-2 duration commands now prepare actual core identities, encode exact
mono 48 kHz PCM16/XAS WAVs with 12,000..96,000 samples (250 ms..2 s), and publish
parent-verified leaf-only packages. RAM/streamed duration pairs, non-frame-aligned
sample tails and eight maximum distinct/shared/all-streamed leaves passed real XAS
proofs and matching-inventory forged-total rejection. The largest tested worker
inventory is 3,994,573 bytes within the unchanged 4 MiB cap. All 109 managed groups,
old fixed controls and 3/8-Sound mixed native vectors pass. Original version-1 and
canonical fixed/mixed commands retain 250 ms admission. General music graphs,
other rates/channels/codecs, playback and game loading remain unproven.

The [managed duration candidate](docs/RA3EP1_AUDIO_DURATION_CANDIDATE.md) is the prior
preparation milestone; its 108-group counters are historical.

[Immutable 1–8 Sound selection vectors](docs/RA3EP1_AUDIO_EVENT_VECTORS.md).
Variable pool events now select up to eight distinct local Sounds, with independent
source weights/defaults and selection order. Private cloned vectors retain value
equality and cannot be changed through caller arrays. Event wire sizes, reference
tables and imports derive from selection count. Three/eight-Sound mixed packages
passed actual core and native XAS integration, including last-selector/import
corruption rejection. All 107 managed groups and old fixed control/native proofs
passed. Fixed two-source commands retain their two-target bound and previous defaults.

[Supervised variable mixed AudioEvent/AudioFile packages](docs/RA3EP1_AUDIO_POOL_MIXED.md).
`supervised-audio-pool-event <absolute-audited-audio.dll> <source-directory>` freezes
explicit event.xml with the 1–8-leaf pool, encodes the audio and appends the selected
event to a checked linked package. The parent recompiles frozen event source and
reconstructs exact package bytes, references/imports and dependency hashes before
acceptance. Managed and actual 1/3/8-source mixed proofs passed, including targeted
selector/weight/control/import corruption with matching inventories and stale
original/input event rejection. Existing fixed control proofs still pass.
This is diagnostic-only; one event/up to eight unique Sounds and narrow canonical PCM/XAS
remain the admitted profile, not a general music compiler or playable SDK.

[Variable pool AudioEvent closure preflight](docs/RA3EP1_AUDIO_POOL_EVENT.md).
`audio-pool-event-preflight <encoded-worker-directory> <event-xml>` independently
verifies a leaf package and compiles one/two selected Sound references against its
1–8 actual identities. Pool slots are distinct from event-local one-biased import
selectors. Real core recompilation, selected dependency fingerprints and original/
owned-copy/raw-output staleness checks passed for singleton, late and reversed
targets; integration against actual eight-leaf XAS also passed. This preflight command
still emits raw event evidence only; mixed publication uses the separate mode above.
The existing fixed event controls/native proofs passed.

[Supervised variable AudioFile packages](docs/RA3EP1_AUDIO_POOL_PACKAGE.md).
`supervised-audio-pool-package <absolute-audited-audio.dll> <source-directory>`
encodes the explicit 1–8-source pool and stages linked manifest/bin/relo/imp plus
custom payloads. Actual source names and explicit RAM/streamed flags remain bound
to parent-reconstructed core preparations. Both readers check dynamic order/offsets;
exact package bytes and artifact membership must match before acceptance. Seven
native cases (37 leaves), including reversed source order and eight distinct
streamed dependencies, passed with matching-inventory corruption rejection.
The old fixed package path also passed. This is leaf-only diagnostic publication;
selected AudioEvent references now have the separate closure preflight above;
mixed publication and game loading remain open.

[Supervised variable AudioFile raw encoding](docs/RA3EP1_AUDIO_POOL_WORKER.md).
The explicit 1–8-source pool now has separate managed worker preflight and opt-in
native XAS encoding commands. The parent reconstructs each core binding and checks
copied sources, frozen PCM, exact runtime/relocations, custom framing, ordered core
metadata and the complete output file set. Managed failure tests and real native
singleton/mixed/eight-source shared/distinct proofs passed, including forged
matching inventories with invalid contents. This publishes raw leaf evidence only;
dynamic package tables are now implemented through the separate package command;
selected event references remain the next gate.
The existing fixed two-source package/event encoder remains unchanged and passed.

[Explicit variable AudioFile pool preflight](docs/RA3EP1_AUTHORED_AUDIO_POOL.md).
`authored-audio-pool-preflight <source-directory>` freezes an explicit inventory of
1–8 XML sources and their shared or distinct direct WAV dependencies. Each leaf
passes the real isolated core preparation path; source order, unique SAGE IDs and
exact original/owned-copy bytes are checked. Managed 1/3/8-source fixtures and
malformed/stale input rejection passed. This command emits metadata only; native
raw encoding is now available through the separate supervised pool command. The existing
supervised encoder still uses its fixed two-source contract.

[Bounded authored AudioEvent control flags](docs/RA3EP1_AUDIO_EVENT_CONTROLS.md).
The four-file path admits unique LOOP, INTERRUPT, FADE_ON_KILL and
IMMEDIATE_DECAY_ON_KILL tokens, their combinations, or explicit empty Control.
Source-derived EP1 bits must match offset 44 exactly and independent parent
recompilation. All sixteen subsets pass full native/package tests; real supervised
audio encoding passed zero/single/combined controls with selected Sound lists.
Unsupported/numeric/duplicate tokens reject. This proves flag serialization, not
in-game loop/fade behavior, and does not widen production plugin admission.

[Selected local AudioEvent Sound lists](docs/RA3EP1_AUDIO_EVENT_LISTS.md).
The four-file path selects one or two distinct local Sounds in XML order, including
RAM-only, streamed-only and reversed pairs. Event dimensions, manifest references,
imports and dependency fingerprints follow the selected list. Managed and actual
native proofs passed all three variants; empty/duplicate/unknown/oversized lists
reject. Both audio leaves are still encoded and packaged; this is not an arbitrary
audio pool or source graph.

[Authored local AudioEvent volume and weights](docs/RA3EP1_AUDIO_EVENT_SCALARS.md).
The four-file path now accepts Volume decimal literals in 0..100 and independent
Sound weights in 0..1,000,000 (omitted weights use the official 1000 default).
Both-zero mixtures and nonliteral/nonfinite/out-of-range values reject. Frozen
source settings must match exact native scalar words and independent parent
recompilation. Boundary/default/corruption regressions and actual Volume=37.5,
weights=125/875 native encoding passed. Selected-list/control support extends the
original fixed graph, not a general event/music compiler.

[Frozen caller AudioEvent source and supervised local closure](docs/RA3EP1_AUTHORED_AUDIO_EVENT.md).
`supervised-authored-audio-event-poc <absolute-audited-audio.dll> <source-directory>`
adds an explicit four-file path: caller event.xml is frozen alongside both audio
sources and PCM. Its literal ID and raw source bytes are preserved; exact local
references and independently recompiled native event bytes must agree before
acceptance. Real encoding, timestamp-preserving stale event rejection and input-copy
tamper rejection passed. The initial fixed Volume=60 and weights 1000/800 profile
has now been extended by the scalar milestone above, not a general event/music importer.
See the linked report for the exact contract and XML example.

[Caller-owned AudioFile identities and local package closure](docs/RA3EP1_AUTHORED_AUDIO_IDENTITIES.md).
`supervised-authored-audio-poc <absolute-audited-audio.dll> <source-directory>` now
reads RAM/streamed XML and PCM from a caller directory, validates/freeze-copies them,
and uses protocol v2 to bind input/output evidence. Caller PCM content and printable
subtitles may vary; originals are read-only and rechecked before acceptance. Real
silence/subtitle/custom-name encoding passed, while stale originals and tampered input copies
were rejected. Literal asset names now come from XML; case/hash aliases reject and
local event references follow those names. This still requires ram.xml/streamed.xml/input.wav,
exactly two source slots and canonical 250 ms mono 48 kHz PCM16/XAS settings; it is not
a general music importer or production AudioFile compiler. See the linked contract
and [XML example](docs/RA3EP1_AUTHORED_AUDIO_SNAPSHOT.md) before using real inputs.

[Supervised audio worker and bounded result acceptance](docs/RA3EP1_AUDIO_SUPERVISOR.md).
`supervised-core-audio-poc <absolute-audited-audio.dll>` runs encoding in a hidden
child process. The parent enforces timeout/exit/log/protocol limits, rehashes owned
artifacts and independently checks current core identities and both packages before
writing a diagnostic acceptance marker. Twelve managed transport failure tests and
two opt-in native tamper tests reject invalid results, including a corrupt manifest
with a matching hash inventory. Fixed fixture outputs remain unchanged. This is not
a sandbox, generic authored-input worker, production AudioFile plugin or playable SDK.

[Audio encoder failure boundaries and native crash prevention](docs/RA3EP1_AUDIO_ENCODER_FAILURES.md).
Eight safe opt-in worker scenarios verify completed cleanup calls and rejection
of changed XML/WAV after encoding or before publication. Independent cleanup
attempts all remaining resources even if a managed release callback throws.
An invalid output-parent experiment crashed the native worker with 0xC0000005;
this condition is now rejected before native create and is not retried as a native
error test. Other native failures remain unproven, so general/production AudioFile
compilation stays closed; bounded supervision is now implemented for the fixed PoC.

[Actual core preparation to native audio diagnostic packages](docs/RA3EP1_CORE_AUDIO_ENCODER.md).
The opt-in `core-audio-encoder-poc <absolute-audited-audio.dll>` now uses actual
core AudioFile identities and frozen PCM to encode RAM/streamed XAS, then rechecks
disk XML/WAV and runtime metadata before publishing two/three-record diagnostic
packages. Two native runs reproduced 28 identical files; the older authored-only
PoC still passes. Core identity and diagnostic content hashes remain separate.
General native failure recovery, a general AudioFile plugin and playable SDK output are
not yet proven or enabled.

[Current core AudioFile preparation bridge](docs/RA3EP1_CORE_AUDIOFILE_PREPARATION.md).
Real core instances now bridge to the narrow authored PCM profile with disk XML/WAV
rechecks, normalized-path and identity binding, immutable input and stale/recovery
checks. Same-size timestamp-preserving WAV edits revoke old preparation. This is
inspector-local and managed-only; native encoder integration and production audio
compilation remain closed.

[AudioFile XML and file-dependency identity proof](docs/RA3EP1_AUDIOFILE_IDENTITY.md).
Actual core parsing of official-schema AudioFile XML and an owned WAV now matches
an independently reconstructed InstanceHash, including 256-byte dependency-buffer
padding. XML/PCM/processor changes, path spelling, missing-file recovery and
production-output denial are checked. This is a managed hash-only gate, not audio
compiler admission or a stock Uprising identity match.

[Identity hash audit and exact text-block correction](docs/RA3EP1_IDENTITY_HASH_BOUNDARY.md).
The current XML/text writer dropped exact 512-character final blocks; a failing
regression and pinned EA IL confirm the defect. The inclusive boundary is fixed
and document versions are bumped to 22/23 to reject old identities/caches.
Reference RA3 Win32 AudioFile ProcessingHash 8FE79286 and seed version 11 are
observed, not asserted as Uprising production settings. Stock InstanceHash remains
unproven and audio-package content hashes remain explicitly diagnostic.

[Fixed local AudioEvent to AudioFile package](docs/RA3EP1_LOCAL_AUDIO_PACKAGE.md).
A core-normalized, isolated AudioEvent now references both encoded local AudioFiles
in one verified three-entry package: 352/36/20 linked BIN/RELO/IMP, two ordered
local reference tuples and two custom files. Changed leaf fingerprints revoke
parent preparation; refresh/recovery and corruption checks pass. Preparation is
explicit for this fixed graph, not general core dependency resolution. Production
hashes, general AudioFile admission and game loading remain unverified.

[Fixed two-AudioFile diagnostic package](docs/RA3EP1_AUDIOFILE_PACKAGE.md).
The actual opt-in encoder now stages and verifies a two-entry v7 manifest,
176/28/8 linked BIN/RELO/IMP and two identity-named custom files. Both readers,
runtime/custom framing, frozen bytes and corruption/preservation tests pass.
InstanceHash is explicitly diagnostic, not the EA production algorithm. General
AudioFile admission and game loading remain open; fixed local event closure now
passes separately, not arbitrary source graphs.

[Checked authored AudioFile input profile](docs/RA3EP1_AUDIOFILE_INPUT_PROFILE.md).
Official-schema XML, current EP1 identity and canonical mono 48 kHz PCM16 WAV
are checked before native work. Immutable snapshots reject changed XML/WAV/hash
and unsupported codec/rate/platform options. The opt-in native experiment now
uses prepared settings; RAM/streamed output hashes remain identical. This is
an authored input gate, not a registered AudioFile processor; the subsequent fixed
package proof remains separate from general admission.

[Isolated EP1 AudioFile runtime serialization](docs/RA3EP1_AUDIOFILE_SERIALIZATION.md).
A separate 32-byte serializer matches complete BIN/RELO/IMP slices of two real
RAM/streamed records (76/8/0 and 88/12/0 bytes). Alignment, ownership and invalid
inputs are tested. The opt-in WAV experiment now validates encoded custom data
against independently reread serialized runtime fields and saves raw native buffers.
No general AudioFile XML/compiler admission, linked manifest packaging or game loading is enabled.

[Native audio API audit and WAV encoder PoC](docs/RA3EP1_AUDIO_ENCODER_POC.md).
The current library exports all 20 bindings, but rejects the existing source's
LAYER3 output container 34. Reference SND container 39 succeeds in an opt-in
worker: mono 48 kHz PCM -> codec 29 RAM/streamed output, with identical hashes
across two runs and independently checked framing. Production audio code is
unchanged; the subsequent isolated serializer does not establish decoder or game-loading compatibility.

[Original audio archive comparison and reconciliation](docs/RA3EP1_AUDIO_ARCHIVE_COMPARISON.md):
All four rejected custom records are valid in the original Steam archive and
differ from the shorter unpacked copies. An explicit verified in-memory overlay
passes all 12,951 custom records / 281,614 blocks. No files are repaired or changed;
the default local-only audit still rejects four copies. This closes the framing
reference-data question, not encoder, production packaging or in-game validation.

[Custom audio framing and four rejected records](docs/RA3EP1_AUDIO_CUSTOM_FRAMING.md):
12,947 identity-mapped EnglishAudio custom files pass bounded RAM/streamed block
lengths, declared sample totals and EOF checks (268,674 blocks). Four files reject;
the local-only audit deliberately reports INCOMPLETE and exits 1. The subsequent
original comparison and explicit read-only reconciled audit pass separately. No
compressed payload decoding, encoder activation or production admission is enabled.

[AudioFile runtime envelope and streamed boundary](docs/RA3EP1_AUDIOFILE_RUNTIME.md):
The recovered 32-byte envelope differs from the legacy 28-byte ABI: subtitle
length/pointer are inline, shifting subsequent fields by four bytes. All 12,951
actual EnglishAudio envelopes/relocations pass; 1,280 embedded eight-byte headers
agree with native rate/channels/samples, and 11,671 records have no inline header.
This read-only audit does not decode/encode audio or admit AudioFile compilation.

[AudioEvent in bounded diagnostic builds](docs/RA3EP1_DIAGNOSTIC_AUDIOEVENT_BUILD.md):
`diagnostic-build` now admits checked AudioEvent roots as its sixth family.
Six-family Include graphs preserve source/metadata snapshots, dependency-first
ordering and exclusive verified publication. Authored AudioFile selector suffixes
and formulas reject before normalization; wrong/duplicate/missing external
AudioFile metadata reject. The committed four-root CLI example ran with actual
EnglishAudio metadata, producing 376/44/44 linked streams. AudioFile codecs,
production SDK output and in-game loading remain closed.

[Fixed AudioEvent / Multisound / FX stream](docs/RA3EP1_AUDIOEVENT_FX_STREAM.md):
A three-level Include graph now compiles local FX → Multisound → AudioEvent →
external AudioFile metadata. Native totals 312/36/28 become 320/44/36 linked
BIN/RELO/IMP bytes. Ordered concrete selectors, source attribution and runtime
mapping names round-trip through both readers. Selected AudioFile metadata must
be unique and match EP1 hash 53C81E47/tokenized false. Actual EnglishAudio mapping,
20 corruptions, missing/edit/recovery and existing-output preservation pass.
The separate general bounded admission now passes; this fixed proof does not enable codecs.

[Checked isolated AudioEvent profile](docs/RA3EP1_AUDIOEVENT_PROFILE.md):
An explicit Win32 ProcessInstance entry now reproduces all five stock native
records with prepared concrete AudioFile identities. Current XML scalar/range
edits are revalidated; stale/changed identities, reordered selectors, incomplete
tables, unsupported options, duplicate aliases and the 33rd reference reject.
Production/cache/reuse and AudioFile codecs stay closed. General bounded AudioEvent
admission now uses this narrow profile. Source copies and prepared tuples are checked separately
from the shared stream-admission fingerprint gate, now exercised by the fixed proof.

[Isolated AudioEvent native proof](docs/RA3EP1_AUDIOEVENT_NATIVE.md):
The recovered 128-byte base, 152-byte root and 12-byte weighted AudioFile references
match five real Uprising records' complete BIN/RELO/IMP bytes and ordered
AudioFile identities. Cryo's three-list loop and InitialDelay, IFV's nondefault
weight/shifted FADE_ON_KILL bit, and Yuriko's weighted footsteps/Delay now have
real stock comparisons, alongside the original impact. Independent defaults and
every implemented optional base pointer also pass. The legacy 96/120/8-byte
path remains unchanged. LimitGroup references reject; remaining stock evidence,
wider diagnostic options, AudioFile codecs and production/game loading remain open.

[Multisound in bounded diagnostic builds](docs/RA3EP1_DIAGNOSTIC_MULTISOUND_BUILD.md):
`diagnostic-build` now admits narrow authored/local Multisounds as its fifth
family. Nested local sounds use real dependency-first order; selected local cycles
reject. Five-family Include/snapshot/fingerprint/recovery/publication checks pass.
The committed DiagnosticMultisoundProbe.xml was executed through the actual CLI
with stock global metadata, producing three entries and 216/28/36 linked streams.
Narrow AudioEvent command admission now passes separately; AudioFile command
admission, production SDK output and game loading stay closed.

[Fixed local Multisound / FX mixed stream](docs/RA3EP1_MULTISOUND_FX_STREAM.md):
A three-level Include chain compiles modifier → local FX → local Multisound →
stock AudioEvent targets. Linked BIN/RELO/IMP sizes are 216/28/36 bytes; native
selectors, concrete reference tuples and sources round-trip through both readers.
Wrong/duplicate/missing external metadata, 20 stream corruptions and leaf edits /
loss / recovery are tested. The shared stream gate now explicitly requires every
selected root's prepared reference table. Narrow public Multisound diagnostic
admission now passes; production/game-loading gates remain closed.

[Checked Multisound compiler profile](docs/RA3EP1_MULTISOUND_PROFILE.md):
Explicit isolated ProcessInstance entries now match three complete stock native
buffers and prepared concrete dependency tuples. Missing/ambiguous/duplicate
targets, stale/tampered tables, current source edits and the 32/33-child boundary
are tested. Production/cache/reuse policies remain closed. Fixed mixed-stream
proof and narrow public Multisound admission now pass. Wider sound roots/options
remain unadmitted.

[Isolated Multisound native proof](docs/RA3EP1_MULTISOUND_NATIVE.md):
An explicit EP1 16/28-byte root/child path now matches three complete stock
Multisound BIN/RELO/IMP slices and ordered AudioEvent identities. Optional
pointers, explicit zero and percentage handling have independent synthetic goldens.
The old 16/8-byte legacy layout remains unchanged. Checked isolated ProcessInstance
and bounded Multisound roots are available; production audio registration stays closed.

[External audio fingerprints and native gaps](docs/RA3EP1_AUDIO_FINGERPRINTS.md):
Selected external AudioEvent/Multisound dependencies now require stock EP1 hashes
and non-tokenized metadata. Wrong fingerprints reject; restored inputs recover
identical output. Bounded native reads confirm old sound layouts cannot be reused:
AudioEvent root 120 vs 152 bytes, audio-file references 8 vs 12, and legacy
Multisound children 8 vs 28. Multisound and a narrow AudioEvent subset now have
isolated native proofs; public narrow AudioEvent admission now passes. AudioFile recovery and wider audio
profiles remain open.

[FX support in bounded diagnostic builds](docs/RA3EP1_DIAGNOSTIC_FX_BUILD.md):
`diagnostic-build` now accepts the isolated empty/two-Sound FX subset, alongside
shader/filter/modifier Include graphs. Real Uprising manifests resolve the concrete
audio targets; all three command-produced FX slices and reference tuples match stock.
Duplicate/ambiguous/wrong/missing targets, edited/frozen sources, unsupported options,
injected TypeIds/selectors and publication failures are tested. The checked-in
`DiagnosticFXProbe.xml` runs through the actual CLI and produces five entries with
572/72/44 linked BIN/RELO/IMP bytes. External audio is metadata-only, not rebuilt or
validated as a native payload. Production SDK output and in-game loading remain
unverified; this is not yet a playable mod. See the English usage/report for limits.

| Workstream | Status |
|---|---|
| Manifest v7, BIG/RefPack readers and stream headers | Implemented; checked with real game inputs and structural tests |
| EP1 schema inventory and enum migration | Inventoried; all 48 EP1-only complex types have models/marshallers |
| Shared RA3/KW native layouts and processors | In progress; model presence does not establish compatibility |
| Final EP1 type table / AllTypesHash | Incomplete; legacy hash and explicit experimental-profile policies block production output |
| SDK scripts and WorldBuilder packaging | Partial; not validated end to end |
| Loading a mod in Uprising | Not yet validated |

### Recently completed

- Admitted the narrow FX profile to `diagnostic-build`, preserving frozen inputs, explicit concrete audio mapping and exclusive verified publication. Four-family Include tests and a real CLI example pass; authored TypeIds and Sound selector suffixes now reject before silent core rewriting. See [FX command integration and usage](docs/RA3EP1_DIAGNOSTIC_FX_BUILD.md).

- Recovered FXList's 28-byte root and FXNugget's 44-byte base. Fixed four optional condition masks that were incorrectly inline (268-byte base); they now use pointers and allocate EP1 masks only when present. Empty, single-sound and two-mask source literals match real EP1 streams exactly (28/0/0, 80/12/8, 252/24/12). Added synthetic four-mask/empty-mask/default checks and advanced the legacy compiler revision to 4. Concrete AudioEvent/Multisound resolution, the isolated profile and bounded command now have separate follow-up proofs; full production FX registration remains closed. See [native FX evidence](docs/RA3EP1_FX_NATIVE.md).
- Added the stock-proven ObjectFilterAsset profile to `diagnostic-build` using a separate three-family declaration schema. Eleven emitted filter slices match the existing native goldens, which were also rechecked against the real EP1 static streams. Mixed included shader/filter/modifier entries preserve source identity, strong shader imports and inline weak GameObject IDs; unused tentative filters stay excluded. Expanded controls and official-invalid weak name syntax reject without publishing. The command's GameObject weak IDs do not establish target availability or in-game behavior. See [diagnostic filter integration](docs/RA3EP1_DIAGNOSTIC_FILTER_BUILD.md).
- Extended `diagnostic-build` to bounded nested `all`/`instance` Include graphs. Approved XML is frozen into deterministic flat snapshots; the real resolution closure excludes unused tentative roots and includes required local targets. Manifest entries retain their actual sanitized source identity. Native selection, frozen/edited sources, mixed external FX metadata, cycles/loss/recovery and path/graph limits are tested. A checked-in Include example builds through the actual command. Reference Includes and production SDK output remain disabled. See [bounded diagnostic command and usage](docs/RA3EP1_BOUNDED_DIAGNOSTIC_BUILD.md).
- Added `diagnostic-build` for bounded modifier/shader XML and explicit external manifest/runtime mappings. It freezes approved inputs, rejects unsupported controls/targets, compiles isolated profiles, verifies staged output through both readers and publishes only a new directory. Existing/raced destinations survive; late corruption rejects without publishing. See [bounded diagnostic command and usage](docs/RA3EP1_BOUNDED_DIAGNOSTIC_BUILD.md).
- Added a fixed two-family diagnostic stream round trip from actual resolved document/compiler entries. Manifest identities, ordered local/external references and all native slices agree through both readers; repeated output is deterministic and four corruptions reject. This exposed and fixed Utility.Manifest's four-byte linked offset for prefixed EP1 streams (now eight); prefixless v7 behavior and v6 rejection remain unchanged. No production commit/link, packaged FX stream or game loading is enabled. See [diagnostic stream proof](docs/RA3EP1_MODIFIER_SHADER_STREAM.md).
- Proved nested instance/all Includes with a local shader and external FX metadata in the same modifier: final native selectors match both resolved identities without compiling FX. Leaf edits, external mapping/manifest replacement and loss/recovery pass. Fixed two real core bugs: cached existence accepted a deleted Include source, and failed document calls left stale processing stacks that caused false circular-dependency errors on retry. True cycles and schema errors still reject repeatedly, including ordinary resident document reuse. Reference Include production builds remain blocked. See [Include/mixed-target proof](docs/RA3EP1_INCLUDED_MODIFIER_SHADER.md).
- Connected modifier and shader profiles in a real mixed document: forward references normalize and resolve to local shader identities, and final pointer/RELO/IMP selectors choose those exact targets. Removed targets fail repeatedly; restoration, source retargeting and poisoned-declaration reload recover correctly. This exposed and fixed stale XML id acceptance in both modifier modes. No production/linker output is enabled. See [modifier/shader graph proof](docs/RA3EP1_MODIFIER_SHADER_GRAPH.md).
- Added an isolated Win32 ShaderOverride compiler profile with fresh observed type metadata, bounded literal POIDs, checked injected TypeIds and detached current-value schema validation. Four actual descriptor/document compiler entries preserve source-native output, including PsychicCrush's documented difference. Default fields, unsigned priority boundaries, tampered controls/dependencies, fresh reload and production/cache/platform restrictions are tested. Full builder/inspector builds, 66 groups and layout checks pass. See [ShaderOverride profile](docs/RA3EP1_SHADER_OVERRIDE_PROFILE.md).
- Recovered ShaderOverride's 16-byte root and rule records, optional material-ID pointers and technique strings. Three supplied XML literals match stock EP1 exactly. PsychicCrush's supplied XML differs from stock by one replacement ID; an explicitly labeled detached stock variant matches all 460/132/0 bytes without changing the original XML or adding compiler exceptions. Full builder/inspector builds, 65 compiler groups, layout checks and 33 enum mappings pass. Production registration remains closed. See [ShaderOverride native proof and source variance](docs/RA3EP1_SHADER_OVERRIDE_NATIVE.md).
- Added a separate experimental ObjectFilter profile for the stock-proven NONE-rule infiltration subset. Eleven actual descriptor/document compiler entries match the native proof. It checks root identity, ordered GameObject weak metadata and nested injected TypeIds, rejects expanded controls, and forces fresh source reload. Production/cache/platform restrictions remain closed. See [filter profile](docs/RA3EP1_OBJECT_FILTER_PROFILE.md).
- Matched eleven ObjectFilterAsset roots byte-for-byte against stock EP1 BIN/RELO/IMP slices. Real document default/weak-reference normalization preserves the same output; separate synthetic checks cover optional status masks and include/exclude lists. This is the third focused asset-family proof, not production registration or proof of inherited filters. See [ObjectFilter native proof](docs/RA3EP1_OBJECT_FILTER_NATIVE.md).
- Fixed real output-dependency retry behavior: a failed reference pass no longer leaves a partial validated list that lets the next attempt succeed. Each preparation rechecks current external mappings and file dependencies; errors clear partial state and the visited marker. Ordered external resolution, local chains/cycles, recursive failure/recovery and weak self/tentative/external location tests pass without native output. See [dependency resolution retry proof](docs/RA3EP1_DEPENDENCY_RESOLUTION.md).
- Added direct diagnostic opt-in for normalized modifier imports, with a separate processing domain and strict index/name/type-to-table checks. The four stock goldens still match through the actual plugin entry; malformed/stale metadata is rejected. Real external lookup resolves all seven FX/Shader targets and refreshes correctly when mappings are removed/restored. Descriptor-created v1 and all production/cache gates remain unchanged. See [import profile and lookup proof](docs/RA3EP1_ATTRIBUTE_MODIFIER_IMPORTS.md#explicit-import-profile-follow-up).
- Corrected shared final import encoding: core XML indices remain zero-based, but BIN values are dependency-index + 1 (zero means null). Restored AttributeModifier's optional Shader pointer. Four import-bearing stock EP1 assets now match all native chunks and ordered dependency identities exactly; the no-dependency experimental profile remains restricted. Added boundary/null/repeated-serialization tests and invalidated old intermediate identities. See [import encoding proof](docs/RA3EP1_ATTRIBUTE_MODIFIER_IMPORTS.md).
- Added an isolated opt-in EP1 AttributeModifier profile for the validated no-dependency subset. Eight compiler-entry outputs match the native proof; real document stages preserve defaults and reject stale experimental reuse. Detached schema revalidation handles core TypeId insertion without trusting stale validity flags. Production, binary-cache and precompiled/session reuse remain denied. See [modifier profile](docs/RA3EP1_ATTRIBUTE_MODIFIER_PROFILE.md).
- Matched eight AttributeModifier assets byte-for-byte against real EP1 static/WorldBuilder BIN/RELO/IMP slices, including BLAT_TRIGGER and RADIATION_ARMOR. Fixed absent optional model masks incorrectly allocating 120 extra bytes; advanced the legacy processor cache revision without changing its KW type hashes. This establishes a second focused root-processor proof, not production registration. See [modifier native proof](docs/RA3EP1_ATTRIBUTE_MODIFIER_NATIVE.md).
- Proved document metadata reuse/reload across resident, plain-XML and compressed sessions. Reported same-signature XML/dependency edits and timestamp-preserving dependency size changes reload the source and change identity; quiet documents remain reusable. Fixed a real crash when cached documents have null stream hints. Deleted live dependencies reject rather than silently retaining stale metadata. See [document reuse proof](docs/RA3EP1_DOCUMENT_REUSE.md).
- Made monitor/cache handoff atomic: late events remain queued for the next build, failed initialization restores the consumed batch, and trust is captured with its paths. Incomplete reports still force hashes for known changes without falsely claiming completeness. Tested 500 concurrent events, overflow, replay/ownership guards and builder integration. See [atomic batch proof](docs/RA3EP1_MONITOR_BATCH_HANDOFF.md).
- Made watcher-reported edits force content hashing even with unchanged size/timestamp. Fixed resident metadata sampling and false monitor errors on unrelated unchanged files; added case-insensitive path matching, per-configuration invalidation and stream-hint tests. Watcher callbacks now retain both rename paths, include created/nested files and synchronize snapshots. See [watcher/cache proof](docs/RA3EP1_WATCHER_CACHE.md).
- Fixed timestamp-only file hash reuse: size changes now invalidate source/dependency hashes, deletion/restoration no longer leaves a zero/stale hash, and old sessions are invalidated. Tested full document strong/weak ID changes separately from file-content dependencies; missing file references now report FileNotFound. See [dependency hash proof](docs/RA3EP1_DEPENDENCY_HASHES.md).
- Made local/cache asset copies preserve existing output on rejection or failure: validate/stage before replacement, restore the previous asset if custom-data publication fails, and retain recovery backups if rollback is blocked. Fixed small final copy chunks and signed/overflowing chunk-length checks; 22 copy scenarios plus blocked-recovery tests pass. See [copy recovery proof](docs/RA3EP1_COPY_RECOVERY.md).
- Reconstructed the stored identity checksum exactly from global/static/WorldBuilder/EnglishAudio EP1 manifest metadata. All four match the official capacity-padded algorithm; all four reject the logical-length-only alternative. Preserved that compatibility contract and tested patch equivalence separately from strict intermediate/cache copies. See [checksum and cache audit](docs/RA3EP1_CHECKSUM_CACHE_AUDIT.md).
- Fixed coordinated BIN/RELO/IMP repair: a valid BIN can no longer cause a missing/broken auxiliary stream to be rebuilt with an empty payload. Reuse checks now include exact expected lengths and direct asset/link/version writes honor experimental profile restrictions. See [linker regression proof](docs/RA3EP1_LINKED_STREAM_REPAIR.md).
- Tested real intermediate asset commits, two-asset concatenation, patch-base payload exclusion and 15 independent linked-file repair cases using isolated synthetic fixtures; production EP1 compilation remains blocked.
- Exercised the real document loader/schema/default/hash stages with the EP1 armor profile; matched populated output to the golden-tested tokenizer.
- Closed production early-return paths and disabled experimental session/precompiled document reuse; verified stale cached declarations cannot replace source XML. See [document pipeline proof](docs/RA3EP1_ARMOR_DOCUMENT_PIPELINE.md).
- Added an opt-in Win32 EP1 armor plugin with exact tokenized compiler-entry output, isolated registrations and explicit production/cache restrictions. See [experimental profile](docs/RA3EP1_ARMOR_PROFILE.md).
- Recovered ArmorTemplate's missing tokenization step: exact `744/8/0` byte match with static EP1 armor and `128/0/0` with a WorldBuilder armor fixture.
- Added a schema-to-native-to-tokenized one-asset diagnostic writer; the production type registry/output gate remains unchanged. See [armor PoC](docs/RA3EP1_ARMOR_TOKEN_POC.md).
- Added a metadata-only EP1 type-registry audit with source SHA-256 fingerprints and conflict/target guards.
- Audited global/static/WorldBuilder/EnglishAudio: 254 observed root asset types, 249 unregistered and five with wrong KW type hashes. See [type-table audit](docs/RA3EP1_TYPE_TABLE_AUDIT.md).
- Corrected ScriptedModel, dependency/death masks, invisibility and tint layouts.
- Restored missing RA3 fields in OpenContain and PassengerData, removed
  KW-only fields and preserved EP1 mask expansions.
- Removed KW slot/grab/weapon-set fields from TransportContain and corrected
  GarrisonContain's InitialRoster from a pointer to an inline record.
- Added transport/garrison regression checks to the layout and compiler tests.
- Audited HordeGarrison's vector pointers and restored the missing
  ContestableGarrisonContain model, marshaller and behavior-module dispatch.
- Audited Heal/Tunnel containment tails; restored SlaughterHordeContain's
  missing FX reference and normalized percentage conversion for cash refunds.
- Corrected HordeContain's EVA asset reference, removed KW-only root fields,
  and restored the 16-byte RankInfo stride for nested position lists.
- Restored ProductionQueueHordeContain's model, marshallers and dispatch.
- Recovered AttachUpdate's masks, imports, optional pointers and EP1 flags/bone-name extension.
- Restored LeechTargetingAttachUpdate and MoneyGainAttachUpdate model/marshaller/dispatch support.
- Restored InfiltratorContain's missing masks, effects, weak ID, EVA/FX imports and optional filter-reference pointers.
- Recovered LaserState's string, particle lists and pointer fields; added EP1 RequiresWeapon and SweepingLaser angle/options.
- Restored the missing ConvergingLaserState model, marshaller and behavior dispatch.
- Verified schema-inserted infiltrator defaults through dependency-index normalization and native imports against real EP1 manifests.
- Fixed inherited refType lookup and typed weak-reference normalization; invalidated old session caches.
- Added explicit external lookup/runtime stream mapping and preserved separate normal/patch entries.
- Fixed patch-base retention and asset lookup; reject wrong-target bases and refresh changed external manifests.

Latest verification: Release/x86 build, `layout-self-test` and
`compiler-self-test` passed. The two-passenger OpenContain fixture produces
`428/8/0`, HordeTransport `364/8/0`, and Garrison `168/0/0` bytes of
`bin/relo/imp`. The new HordeGarrison fixture produces `220/16/0`; the
ContestableGarrison dispatch fixture produces `240/8/0` (including its
four-byte outer pointer slot). These are native-marshalling tests, not proof
that a mod loads in-game.

The latest containment leaf tests also pass: Slaughter `344/0/8`, Heal
`188/0/0`, and Tunnel `192/0/0`. Slaughter verifies that a refund value of
`25` is stored as `0.25` and that its FX reference creates an import entry.
HordeContain's two-rank fixture passes with `592/20/12`, checking nested
positions, weak unit IDs, leader defaults and EVA/modifier import slots.
ProductionQueueHordeContain's dispatch fixture passes with `156/16/0`,
checking two eight-byte template records and an optional filter pointer.
AttachUpdate checks cover an empty root and a populated EP1 bone-name/mask/import
fixture; its standalone EVA asset dependency remains unported.
The preceding Release/x86 rebuild and its 43 compiler test groups passed. AttachUpdate
fixtures produce `368/0/0` and `576/32/16`; Leech dispatch produces `372/8/0`
and MoneyGain dispatch `448/12/0`. The enum check now verifies 33 schema mappings.

The focused reference-pipeline test now validates infiltrator's four named
defaults: official schema default insertion, production normalization into
`name\dependency-index`, native imports and matching type/instance IDs in real
EP1 global/static manifests. The suffix is a dependency-table index, not an
asset hash or arbitrary numeric asset ID. Raw-marshalling fixtures alone do
not exercise this pipeline. The focused harness stubs unrelated asset layouts;
it does not claim a complete SDK build or game load.

Previous layout-block verification: the full Release/x86 rebuild, layout tests, all 45
compiler test groups and 33 enum mappings pass. Infiltrator emits `144/12/32`,
populated LaserState `136/24/12`, Sweeping dispatch `80/8/0` and Converging
dispatch `144/8/0` bytes of `bin/relo/imp`. The sweeping fixture also checks
EP1 angle conversion, option removal and RequiresWeapon=false. A Turkish-culture
regression caught and fixed culture-sensitive weak-ID hashing: uppercase-I
asset names now use invariant normalization. These tests still do not prove
in-game loading. Overall effort remains approximately 50% complete.

Previous reference-block verification: full Release/x86 dependency rebuild, final inspector
rebuild, layout tests, all 46 compiler groups and 33 enum mappings pass.
The schema-default infiltrator fixture produces `136/0/20`, including a real
import selecting dependency index zero (now correctly encoded as BIN word one). Repeated runs produce identical native chunks
and dependency identities; a previous-revision session cache is rejected.
Inherited reference types, attribute overrides, invalid explicit types and
typed/trimmed weak names are regression-tested. The four default targets exist
in the local Uprising global/static manifests, checked without opening BIN data.

Current verification: Release/x86 full dependency rebuild, final test-runner
rebuild, all 47 compiler groups, layout checks, writer round trip and 33 enum
mappings pass. External-link tests cover runtime path serialization, settings
round trip, normal/patch roles, retained base metadata, same-path manifest
refresh, wrong-target rejection and production lookup against real EP1 manifests.
No game BIN was opened or modified; generated fixtures contain only metadata.

External builds now require an explicit runtime-name mapping in addition to
local manifest files; otherwise production emission fails rather than silently
omitting the stream references. Physical paths are lookup inputs, not game paths.

```xml
<!-- Reborn: pair local lookup files with relative names visible to the game's stream loader. -->
<Settings ExternalManifests="Base/global.manifest;Base/static.manifest"
          ExternalManifestReferences="global.manifest;static.manifest" />
```

`/emr` supplies the same mapping on the command line. Names are paired in order,
must be relative `.manifest` paths, and cannot contain traversal or empty
segments. Reference-bearing manifests are regenerated even when asset checksums
match, preventing stale runtime names. The final EP1 type-table safety gate
remains enabled; this is not yet a validated SDK/game-load build.

### Next steps and acceptance gates

1. Audit remaining containment derivatives and other shared native layouts.
2. Complete the EP1 type table and verify `AllTypesHash=0x5454A8E9`.
3. Compare tokenized output with real Uprising asset fixtures.
4. Complete SDK source/schema/dependency paths and WorldBuilder packaging.
5. Load a minimal mod in Uprising and verify byte-identical repeat builds.

Detailed findings, file/class references and limitations:
[EP1 implementation status](docs/RA3EP1_IMPLEMENTATION_STATUS.md).
This summary and the detailed report are updated together as work progresses.
Changes are published only to the working branch in
`gamezerve/BinaryAssetBuilderOmega`; no pushes or PRs go to the Qibbi upstream.

</details>

## Legacy KW asset list (historical reference)

The following list and type IDs come from the earlier KW implementation.
They are not a Uprising compatibility checklist or a verified EP1 type table.

### Implemented Asset Types
* [ ] TestGameObject
* [ ] TestTexture
* [ ] TestTextureCollection
* [x] WeaponTemplate
* [x] LocomotorTemplate
* [ ] GameObject                                                0x132408DB
* [ ] FXParticleSystemTemplate                                  0xA148D511
* [ ] Weather                                                   0x368A8BA2
* [ ] ShadowMap                                                 0xC6389FA6
* [ ] WaterTransparency                                         0x331DA6CE
* [ ] Texture
* [ ] OnDemandTexture
* [ ] W3DMesh                                                   0xC9D7E778
* [ ] W3DContainer                                              0x909DD93F
* [ ] W3DHierarchy                                              0x3BC26A7A
* [ ] W3DAnimation                                              0xCC069193
* [ ] W3DCollisionBox                                           0xC917E725
* [ ] ArmyDefinition                                            0x57213EA5
* [ ] AIPersonalityDefinition                                   0x7DCE182F
* [ ] FXList                                                    0xEBE8A8A4
* [ ] ObjectCreationList                                        0x683D4DE5
* [ ] ObjectFilterAsset                                         0x25970AF7
* [ ] SpecialPowerTemplate                                      0x5EF0ACA9
* [ ] UpgradeTemplate                                           0x1E53F384
* [ ] SkirmishOpeningMove                                       0x21EE29FA
* [ ] AIStateDefinition                                         0x262BE85F
* [ ] AIStrategicStateDefinition                                0x1E27DA26
* [ ] AIBudgetStateDefinition                                   0xA10F9630
* [ ] AITargetingHeuristic                                      0xB7A2C222
* [ ] GameMap                                                   0x3EC9C79B
* [x] AttributeModifier
* [x] ArmorTemplate
* [ ] MissionTemplate                                           0x0D283295
* [ ] TheaterOfWarTemplate                                      0xE60C9724
* [ ] CampaignTemplate                                          0xAC60B530
* [ ] RadiusCursorLibrary                                       0xD62B490F
* [ ] AudioFile                                                 0x46410F77
* [ ] AudioEvent                                                0x1B886049
* [ ] MusicTrack                                                0x1469548A
* [ ] DialogEvent                                               0x8655CDB4
* [ ] AmbientStream                                             0xDABB1C4B
* [ ] Multisound                                                0x12B1C67C
* [ ] MusicPalette                                              0x6A7AF822
* [ ] MusicScriptConditionNugget_LocalPlayerIsObserver          0xAFB6AF3A
* [ ] MusicScriptConditionNugget_UnitsFarFromBase               0xD889BF98
* [ ] MusicScriptConditionNugget_TimeFromStartOfLevel           0xAA4A9E23
* [ ] MusicScriptConditionNugget_TrackPlayedCount               0x4FCFFAB1
* [ ] MusicScriptConditionNugget_SpecificTrackTypePlaying       0xBCAD9B77
* [ ] MusicScriptConditionNugget_AnyTrackPlaying                0x337BC326
* [ ] MusicScriptConditionNugget_ObjectsOfTypeExist             0x9586411C
* [ ] MusicScriptConditionNugget_EvaEventPlayedRecently         0x1F200F13
* [ ] MusicScriptConditionNugget_ObjectsNearEvaEvent            0x0EC4D160
* [ ] MusicScriptConditionNugget_ScoredKillCount                0x5C0F93DC
* [ ] MusicScriptConditionNugget_Not                            0xB886383B
* [ ] MusicScriptConditionNugget_Or                             0x81114695
* [ ] MusicScriptConditionNugget_And                            0x10173347
* [ ] MusicScriptTrack                                          0x702C8407
* [ ] LocalBuildListMonitor                                     0x99CC030A
* [ ] MpGameRules                                               0xEDDBB607
* [ ] ExperienceLevelTemplate                                   0xAE55047B
* [ ] MissionObjectiveList                                      0xC385A8C1
* [ ] StringHashTable                                           0x2C112832
* [ ] InGameUISettings                                          0x49FE3760
* [ ] DamageFX                                                  0x4DF81EBD
* [ ] MultiplayerSettings                                       0x1BAF4C42
* [ ] OnlineChatColors                                          0xF3645AA7
* [ ] MultiplayerColor                                          0x966F336A
* [ ] GameLODPreset                                             0x19DAC24D
* [ ] StaticGameLOD                                             0xBEAF1CC9
* [ ] DynamicGameLOD                                            0x71BAD792
* [ ] AudioLOD                                                  0x3ABBF00F
* [ ] VideoEventList                                            0x999FCBE3
* [ ] UIConfigList                                              0xB3B7607A
* [ ] PackedTextureImage                                        0x2FAEB748
* [ ] OnDemandTextureImage                                      0xF3F4AEEC
* [ ] TerrainTextureAtlas
* [ ] Mouse                                                     0x73FE99B0
* [ ] Achievement                                               0xC8D16E6D
* [ ] StanceTemplate                                            0x5C6E0E41
* [ ] TargetingCompareList                                      0x57CA5C81
* [ ] TargetingDistanceCompare                                  0xED45F096
* [ ] TargetingCombatChainCompare                               0x553808EF
* [ ] TargetingInTurretArcCompare                               0xCD24391A
* [ ] Road                                                      0xDCF3C28B
* [ ] Environment                                               0x878C42E0
* [ ] LogicCommand                                              0x97D0A46E
* [ ] LogicCommandSet                                           0x6D148BD7
* [ ] MiscAudio                                                 0xFA4817E2
* [ ] AudioSettings                                             0x89AA7DDE
* [ ] CrowdResponse                                             0x66FB33A0
* [ ] MapMetaData                                               0x59013A51
* [ ] LargeGroupAudioMap                                        0x9CBC0553
* [ ] AptAptData                                                0x36866072
* [ ] AptConstData                                              0x1CE8E595
* [ ] AptDatData                                                0x3BF7FEB9
* [ ] AptGeometryData                                           0x58F89E8B
* [ ] MappableKey                                               0xE005A668
* [ ] HotKeySlot                                                0x1AC54E60
* [ ] DefaultHotKeys                                            0x0E12479D
* [ ] InGameUIGroupSelectionCommandSlots                        0xF6CE1A68
* [ ] InGameUILookAtCommandSlots                                0x8F9F9918
* [ ] InGameUITacticalCommandSlots                              0xC24AEFF1
* [ ] InGameUIVoiceChatCommandSlots                             0x3592E352
* [ ] InGameUISideBarCommandSlots                               0xAF956455
* [ ] InGameUIPlayerPowerCommandSlots                           0x4AB425C6
* [ ] InGameUIUnitAbilityCommandSlots                           0x9DAA4182
* [ ] GameScriptList                                            0x5AC6FA18
* [ ] IntelDB                                                   0xFBB64F90
* [ ] BootupDisplaySequence                                     0x84C1C2F0
* [ ] UnitTypeIcon                                              0xF7AB74BE
* [ ] ImageSequence                                             0x217CF953
* [ ] UnitOverlayIconSettings                                   0xDFC78E66
* [ ] TheVersion                                                0xF659EF49
* [ ] DLContent                                                 0x4E1A5713
* [ ] PhaseEffect                                               0x4877D566
* [ ] ConnectionLineManager                                     0x7AEB73B2
* [ ] InGameUIFixedElementHotKeySlotMap                         0x475EA260

### Tiberium Wars Only Types
* [ ] AudioFileMP3Passthrough                                   0x610DB321
* [ ] MP3MusicTrack
* [ ] MP3DialogEvent
* [ ] MP3AmbientStream
* [ ] UnitAbilityButtonTemplateStore                            0x5A48D289
* [ ] PlayerPowerButtonTemplateStore                            0xDB57AB4F
* [ ] CommandSet                                                0x3CFF78A1

### Kane's Wrath Only Types
* [ ] UnitAbilityButtonTemplate
* [ ] PlayerPowerButtonTemplate
* [ ] StrikeForceBuildTemplate
* [ ] MetagameOperationsInfoType
* [ ] MetaGameUITacticalCommandSlots
* [ ] MetaGameUICommonOpCommandSlots
* [ ] MetaGameMapZoneData
* [ ] MetaGameStaticData
* [ ] ButtonSingleStateData
* [ ] JoypadCommandBarTemplate
* [ ] JoypadCommandBarButtonTemplate
* [ ] UIJoypadCommandBarButtonBuild
* [ ] UIJoypadCommandBarHomogenousGroup
* [ ] UIJoypadCommandBarMixedGroup
* [ ] UIJoypadCommandBarSingleUnit
* [ ] UIJoypadCommandBarStances
* [ ] UIJoypadCommandBarTopMenu
* [ ] UIJoypadCommandBarMgTopMenu
