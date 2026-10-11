# Uprising SDK release gates and effort estimate

## Current position — October 12, 2026

The six-sample e local alpha and fresh overlay setup/build now pass. Two yard
armor identities have positive combined-stream gameplay observations across
separate revisions; Allied GUN=250 remains inconclusive. Public redistribution
review and new-workspace GUI observation remain separate. The next type is the
[Conscript AK47 weapon slice](RA3EP1_WEAPON_VERTICAL_SLICE.md): root token/native
scalar relationships pass read-only stock review, but full tokenization,
dependency closure and an authored weapon build are still required. The older
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
