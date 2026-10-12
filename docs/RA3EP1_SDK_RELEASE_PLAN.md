# Uprising SDK release gates and effort estimate

## Final product contract

The user's target is an RA3-Mod-SDK-style Uprising SDK, not a collection of
fixed override recipes. The intended authoring surface includes arbitrary
schema-valid new ArmorTemplate, GameObject and other asset identities; multiple
assets and files; official or authored XML through instance includes; inherited
objects/assets using the source syntax accepted by the SDK; definitions and `$`
references/expressions; TGA/DDS textures and W3X model declarations; and a familiar
Studio build/log/error workflow. EA's Uprising XML/schema semantics are the
compatibility target. Platform/file-format/runtime constraints still apply;
"arbitrary" does not mean malformed assets or physically unbounded resources.

The current one/two-yard identity restrictions are temporary experimental
publication boundaries, not a planned permanent user-facing limitation.
Generic inheritance/definitions have diagnostic implementations in the repo,
but they are not yet connected to general playable asset publication. Do not
rewrite the final goal as "armor SDK complete" merely because a preview ships.

Implementation must converge on shared layers instead of adding one hard-coded
compiler per unit: (1) source graph/includes/definitions/inheritance, (2) typed
schema/default/marshal/tokenization processors, (3) identity/reference/import
resolution, (4) scoped then general Core stream publication, (5) art/custom-data
processors and (6) Studio/packaging/environment integration. The weapon slice
is a test of those reusable layers, not the final limit on weapon identity.

Acceptance requires a mod creating multiple new armors and game objects under
user-chosen IDs, importing an official instance and a separate authored file,
inheriting and overriding selected fields, using shared `$` definitions, and
building referenced texture/model assets into a playable package. Each of
these capabilities needs explicit build and appropriate gameplay evidence.
Public distribution review is separate from achieving functional parity.

## Current position — October 12, 2026

October 13 consumer follow-up: the [GameObject native slice](RA3EP1_GAMEOBJECT_VERTICAL_SLICE.md)
verifies Conscript/Allied yard ArmorSet token/native/dependency relationships and
reproduces their selected armor-list bytes through the existing marshaler. The
follow-up proves actual Core name normalization and stock/new armor lookup for
the bounded BuildTime/ArmorSet projection. Full official GameObject schema
validation, complete GameObject/module tokenization and mixed
GameObject/armor Core publication remain gates; this diagnostic does not enable
GameObject input in the exported alpha.

October 13 source follow-up: the [armor source bridge](RA3EP1_ARMOR_SOURCE_BRIDGE.md)
connects captured instance/definition Includes, imported/local inheritance and
exact field substitutions to real scoped armor set publication. Defining source
contexts and raw closure hashes remain explicit. This closes a source-to-output
integration gap for the bounded armor subset. The source-enabled alpha now wires
this route into the existing Studio driver with ALPHA004, leaving old golden
samples unchanged. Full list-merge
semantics and consuming GameObject/runtime publication remain open.

October 13: [general armor authoring](RA3EP1_GENERAL_ARMOR_AUTHORING.md)
connects user-selected IDs, variable list lengths, decimal/% literals and schema
defaults to actual Core/scoped streams and offline default-reader packaging.
The separate set command now connects 1..32 distinct user-defined armors to one
Core manifest/linked stream, with duplicate lookup-ID rejection and per-member
scope freezing. This is a shared type path, not a new fixed-unit recipe. Actual updated GUI observation,
mixed types, broader source/list-merge semantics and consuming GameObject
publication remain the next integration gates; no new gameplay proof is claimed.

Development now uses a [batched validation matrix](RA3EP1_BATCH_TESTING.md):
one build, isolated workers, independent feature/negative-control groups and a
single report. Future milestones should connect reusable source/type/output
layers in larger batches rather than add one fixed unit field per turn. This
workflow change is not a capability-percentage increase or runtime proof.

The seven-sample g local alpha, fresh extraction, overlay setup/build and
relocation now pass. ConscriptAK47 adds one complete fixed weapon identity with
only Damage editable (integer 0..10000, optional .0); mixed armor/weapon builds
remain refused. Exact-reference CLR4 adapter loading/options pass, but actual
updated GUI interaction is still pending. Two yard
armor identities have positive combined-stream gameplay observations across
separate revisions; Allied GUN=250 remains inconclusive. Public redistribution
review and new-workspace GUI observation remain separate. The next type is the
[Conscript AK47 weapon slice](RA3EP1_WEAPON_VERTICAL_SLICE.md) now has full stock-byte
equality for its reviewed shape, positive user-reported diagnostic Damage=50
behavior and selected XML/Core/package builds. General weapon tokenization and
dependency closure remain required for broader support. The older
armor-only milestones below are historical scope, not the current type count.

The local alpha now exposes [all eighteen fixed ConYard armor percentage
fields](RA3EP1_ARMOR_EXPANDED_AUTHORING.md), not only GUN. Individual-field and
combined Core/package regressions pass offline; only the earlier GUN=5000
recipe has user-observed gameplay evidence. This is a capability expansion,
not completion of the general gameplay or broad SDK gates below.

One existing armor identity has a user-observed in-game override. The same
working package can be reproduced from current source in one command. That
is a useful vertical slice, not evidence that all asset processors or the
complete EA SDK workflow are compatible.

The next release now targets **Uprising Mod SDK Alpha**, retaining EA's existing
Mod BuildStudio interface while exposing a limited capability matrix. The
initial package can be armor-only; supported types grow after release.
It is not a complete RA3-SDK-equivalent release. See the
[Studio adapter prototype](RA3EP1_ALPHA_STUDIO_ADAPTER.md).
A local inventory-only ZIP has passed fresh extraction and editable armor builds
without a repository source tree. It is still a LOCAL-USE-ONLY candidate:
the real GUI success path now passes by user screenshot/log and independent
output readback. Both warnings are now visibly categorized in the user's Error
Log screenshot; the corrected InvalidXml failure callback also passes visually.
Local help and the subsequent Reborn success also pass. The corrected driver
and help now pass fresh extraction and both sample builds in extracted-c.
Public distribution/rights review remains. Do not label it
as a validated public SDK release. See [workspace evidence](RA3EP1_ALPHA_WORKSPACE.md).

A separate [overlay candidate](RA3EP1_ALPHA_OVERLAY.md) now passes clean setup
and both offline sample builds without bundling EA Studio or official schemas.
Eight external dependency hashes are verified before copying; the manifest has
no absolute preparation paths. Compiler/fixture provenance and embedded metadata
review still prevent treating this packaging candidate as a public-ready release.

| Deliverable | Remaining gates | Working effort estimate |
| --- | --- | --- |
| Limited EA-UI SDK alpha | Editable GUN/Core/package path, corrected clean extraction, Studio success, warning/error classification, corrected failure callback, subsequent success and local help pass; public distribution review remains | Local-use candidate is available now; public-ready packaging is a separate dependency/setup/metadata review, not an all-processors prerequisite |
| Basic gameplay SDK alpha | GameObject/Weapon and needed modules, dependency/import closure, reference/override precedence, repeatable build and controlled gameplay checks | Several weeks; reassess after the first unit/weapon vertical slice |
| RA3-SDK-like broad release | Wider typed processor coverage, inheritance/includes/cache contracts, textures/models/audio/shaders, map workflows and WorldBuilder integration, distribution/dependency review | Several months or longer; no reliable fixed release date yet |

The table's broader gameplay/full-SDK scopes are not prerequisites for the
first limited EA-UI-compatible alpha. Its remaining immediate gates are
actual Studio integration and distribution review. Diagnostic prefixes follow
inspected Studio dispatch; offline prebuilt/extracted package checks pass.
The user has explicitly selected this early-alpha
approach; do not defer it until GameObject/Weapon or every processor is complete.

These are provisional engineering estimates, not measured remaining hours,
calendar deadlines or a promise of unattended background work. Unknown runtime
contracts can expand them. Frequent conversation continuations are not work-day
units. No credible overall percentage follows from the single armor success;
the old 52% figure is historical and not a release countdown.

## Preview acceptance checklist

1. Accept a small standalone XML with fixed `AlliedConYardArmor` identity;
   initially permit only canonical integer GUN percentages 0..10000. Reject
   formulas, includes, inheritance, unknown attributes and other asset types.
2. Freeze and compile that input through the actual document and scoped
   OutputManager path. Independently compare all other BIN bytes and RELO/IMP
   against the stock-baseline contract. Fresh input must get fresh Core hashes;
   never reuse the fixed-5000 manifest hash/checksum for changed input.
3. Validate every default `mod{,_l,_m}` family and its version marker against
   the current admitted input, rather than the old fixture or fixed package
   digest. Preserve the fixed recipe regression. Do not enable general
   production, cache or precompiled reuse merely to make this build succeed.
4. Provide a normal user-facing build command, example XML, output-directory
   layout and meaningful logs. Eliminate developer-specific absolute paths
   from the distributed entry point; explicitly configure stock references.
5. Assemble only an explicit reviewed file allowlist. Include project license
   and notices; review external components independently. Do not bundle game
   executables, installed archives, large stock BINs, personal local artifacts
   or assume the repository MIT license covers every EA SDK dependency.
6. Test extraction/build from a clean directory with documented prerequisites.
   Confirm no dependency on hidden source folders or the developer's cache.
   A local ZIP can follow these checks; a public release is a separate action.
7. Obtain separately authorized gameplay checks for baseline and another value,
   using a fresh skirmish and confirmed GUN weapon. Document normal closure and
   restoration. Never replace stock archives as an automatic install step.

## Current next implementation

### Diagnostic and post-build compatibility contract

The final SDK must surface malformed XML, official EP1 XSD violations,
missing definitions, includes, art/W3X files and unresolved asset references
in the familiar Build Log/Error Log. Required input failures must stop
publication; unsupported processors must not silently skip assets. These
checks belong before publication, not merely after a BIG has been created.

Current alpha evidence covers malformed XML, bounded armor authoring rejection
and failed-build reporting. It does not establish general W3X, expression or
schema diagnostics for arbitrary assets. Core already has schema validation
(`AssetDeclarationDocument.HandleValidationEvents`), referenced-file checks
(`HandleFileReferenceType`) and unknown-asset checks. Expression evaluation
also depends on a separately loaded evaluator; the wrapper's existence is not
proof that missing-definition handling works in the released alpha.

The local, modified RA3 reference SDK invokes `hashfix.exe` in
`defaultscript.cs`, case 8, with mod.manifest and worldbuilder.manifest,
then assetresolver, streamsorter and LoDStreamBuilder. `BuildMap.bat` has a
related map-specific sequence. This is observed reference behavior, not proof
that every tool is an unmodified official EA component. No such executable
has been run against EP1 output here. A hash-fix invocation is a corrective
postprocessor, not itself XML validation.

For EP1, verify identities/type hashes, ordered dependencies, relocation and
import ranges, LOD/version routing and default-reader package consistency.
Use EP1 reference streams, never RA3 worldbuilder hashes as an automatic
substitute. If correct compiler output makes a legacy correction unnecessary,
retain the verified invariant rather than a cosmetic "hash fix" step.
Acceptance requires controlled invalid XML, wrong schema/enum, undefined `$`
name, missing include/W3X, unknown asset and corrupted-output tests, with clear
file/asset context and no successful publication. General diagnostics remain
an implementation gate, not an already shipped capability.

`ArmorAuthoringAdmission` now feeds `ArmorAuthoredBuild` and the actual
document/Core/scoped OutputManager path. Five coefficients, including
100/250/5000, have verified default-reader packages; repeated GUN-250 bytes
are deterministic and GUN-5000 retains the exact user-tested digest.
`Invoke-Ra3Ep1AlphaBuild.ps1` builds selected supported XML, rather than replaying
the fixed recipe. CLI preflight alone remains input-only; the final authored
receipt distinguishes successful package publication. The fixed
`Build-Ra3Ep1ArmorMod.ps1` stays an exact-recipe regression tool.

The principal editable-armor functional gap is closed offline. An isolated EA
Studio candidate includes explicit configuration, CLR4 selection, local help
and prebuilt tools; exact loader-reference/config/options checks and fresh ZIP
extraction pass. The user-run Reborn GUI build now also succeeds with the exact
game-tested digest, and its warning categorization is visible. The corrected
InvalidXml failure callback now passes too. Next repeat extraction with the
corrected driver and review public distribution; local help and subsequent
Reborn success have now been observed.
The successful default-reader trial
does not by itself resolve queue-route failure, exact runtime file reads,
generic duplicate asset ordering or full WorldBuilder support.
