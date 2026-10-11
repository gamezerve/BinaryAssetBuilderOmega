# Combined armor authoring — October 12, 2026

## Supported input

An AssetDeclaration may now contain exactly one AlliedConYardArmor and one
SovietConYardArmor together. Each retains thirteen ordered Armor entries and
five optional percentage scalars, canonical integer 0..10000. Standalone and
one-member declarations retain the previous path. A duplicate, third asset,
unknown identity/type, Include, inheritance, DTD or invalid value fails the
whole build. The limit is two named armors, not arbitrary multi-asset support.

CombinedArmor sets Allied GUN=250 and Soviet GUN=5000. Both declarations enter
one real Core document, one OutputManager and one linked stream family. This
does not concatenate two single-asset BINs or mount two competing mod readers.
The BIG still contains fifteen entries: the same empty/_l/_m families with
manifest/bin/relo/imp/version. The manifest now has two asset records, BIN has
two 856-byte members, RELO two 8-byte slices, and IMP remains empty apart from
its header. The sample BIG is 6348 bytes, SHA256:
`826859F2CD92D840B37D0AABB239DF1C799F433A3A8014912F34757AEF5554E3`.

## Implementation

- ArmorPairAuthoredBuild.Admit bounds the declaration to sixteen KiB, delegates
  each member to the existing validator and sorts canonical members by identity.
  Stage freezes that result before output; receipts retain two separate complete
  eighteen-value maps, not an ambiguous single GUN value.
- ArmorPairDocumentSmokeTest compiles each untouched baseline under its own name
  and independently matches stock before the shared document is written. Each
  current authored payload is compared to its own baseline and tokenizer output.
- OutputManager.CreateExperimentalArmorPairDiagnostic is a separate opt-in;
  the original single-asset factory still refuses a two-instance document.
  ExperimentalArmorOutputScope freezes two separate object identities/XMLs/
  hashes/expected buffers. Membership checks use reference identity, not the
  InstanceDeclaration.Equals implementation (which compares handles).
- The real writer creates two intermediate .asset files, commits a shared manifest
  and links cumulative per-stream slices. Full membership is mandatory before
  publication. The two-row checksum uses the existing padded 256-byte Core hash
  domain; it is independently reconstructed from written manifest rows.
- ArmorTrialPreparation.ReadPairSource independently recompiles both admitted
  XMLs, verifies distinct manifest identities and exact slice boundaries, and
  verifies common headers/checksum before the existing default package recipe.
- The alpha backend compares each member's eighteen values between admission
  and build receipts; old one-member receipt shape is not accepted for pairs.
  Error/critical callback behavior remains unchanged.

Production output eligibility, build cache, precompiled reuse, external manifests,
dependencies, custom data and generic GameObject/Weapon compilation remain closed.
No game files are replaced and no game is launched by these operations.

## Offline evidence

`armor-pair-document-self-test` passes in
`artifacts/RebornArmorPairDocument-20261012-b`: two own-stock comparisons, actual
Core/committed/linked output, two-reader identity checks, independent checksum,
complete byte-slice readback, incomplete output/single-factory refusals and
corruption of either in-memory or committed member. Source changes are refused.
The first diagnostic attempt found dependency preparation was not yet complete;
the harness now prepares both members before freezing output authority. It did
not relax the dependency guard or publish a successful artifact on that attempt.

`armor-pair-authored-self-test` passes in
`artifacts/RebornArmorPairRegression-20261012-a`: normal versus reversed XML
ordering produces byte-identical packages, all thirty-six fields edited to 73
produce a newly verified package, eight input-shape refusals create no admitted
output, occupied output is preserved, and source mismatch of either member and
the existing package corruption controls fail closed.

The existing single-armor authored suite passes again in
`artifacts/RebornArmorPairLegacyRegression-20261012-a`, retaining both identities'
eighteen-field coverage and the original Allied golden digests. Combined backend
build and complete two-member receipt checks pass. This is not a new full general
compiler-suite run. Individual Allied/Soviet gameplay successes do not prove the
combined package's runtime loading; that remains a separate authorized test.

## Usage

Select CombinedArmor in a current workspace and edit its mods/CombinedArmor/data/
mod.xml. Keep one of each allowed identity and each full ordered thirteen-entry
list. The chosen XML is frozen, compiled and packaged into a fresh builtmods
directory. Read alpha-build.json and build/pair-build.json for both member maps.
SovietConYardArmor is also referenced by Japan's yard and a civilian building;
the shared-armor warning still applies. Public redistribution review remains
outstanding; no generated local ZIP has been published to GitHub.

## Current local workspace

`UprisingAlpha-20261012-c` passes all five prebuilt sample builds, including the
two-member receipt and combined golden digest, without a repository source tree.
The overlay candidate passes binary privacy gating and fresh extraction/setup
with the same five builds: 29 included files, eight external dependencies.
`artifacts/RebornArmorPairDocument-20261012-c` additionally passes equal-handle
foreign-object substitution refusal for both selected members and build-cache
mutation refusal. The current workspace includes these controls and updated
single-armor gameplay warning text; the earlier b candidate also passed its
five-sample and clean extraction checks. The focused single-document test passes
again with current reference-membership guards (`artifacts/RebornArmorPairSingleGuard.log`).
Current overlay SHA256:
`2E3788F081678FF377129B3CEE71E816A2AE288743CC8F4532C6D7CACE287C00`.
This new sample has not yet been observed in the GUI or in a game. Public release
review remains separate. The full generic compiler suite was not rerun here.

## Controlled runtime test preparation

`scripts/Invoke-Ra3Ep1ArmorTrial.ps1 -Combined` now pins the exact c workspace
package at `builtmods/RebornAlphaInput-c9d132f96e1049e689f1574c741ee126/build/package`,
with the 6348-byte size and SHA256 above. It refuses combination with any other
trial selector. The no-Run preflight passed on October 12: stock identities,
campaign archive, exact config/archive bytes and absence of existing Uprising
processes. All three conflicting-selector checks failed before preflight as
intended. No game was started or installation file changed.

After separate approval, adding `-Run` performs one launch with the pinned config.
In that session, use two fresh skirmishes: Allied Peacekeeper against an unpacked
Allied Construction Yard (GUN=250), then Soviet Conscript in normal AK-47 mode
against an unpacked Soviet Construction Yard (GUN=5000). Expect a smaller increase
for Allied and a large increase for Soviet relative to stock, not a claimed exact
health-loss ratio. Avoid saved matches, alternate weapons and temporary buffs.
Record the two observations separately; a menu opening or one successful armor
does not prove the other member loaded. Japan/civilian shared consumers remain
untested. Automated launch receipts do not claim gameplay confirmation.
