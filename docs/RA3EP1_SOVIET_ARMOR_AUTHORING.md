# Second armor identity — October 12, 2026

Capability update: [combined armor authoring](RA3EP1_ARMOR_PAIR_AUTHORING.md) now
admits one of each supported identity in a shared stream. The one-per-build
restriction below records the earlier stage; arbitrary multi-asset input is
still not enabled.

Outcome update: the user reported increased damage in the separately authorized
Soviet GUN=5000 test. The original preparation sections retain their offline-only
status; the subsequent gameplay observation is recorded at the end.

## Capability

The alpha now accepts exactly one standalone AlliedConYardArmor **or**
SovietConYardArmor per build. Both use the thirteen ordered damage entries and
five optional percentage scalars already characterized. Canonical integer
0..10000 bounds and all other shape/XML restrictions remain. Multiple assets,
arbitrary identities, inheritance and Include support are not enabled.

This expands the verified identity envelope, not the processor binary format.
The existing EP1 processor/Core/OutputManager and fifteen-entry default-reader
package are reused. Stock matching is mandatory for the selected identity;
relabeling an Allied proof is insufficient.

## Reference evidence and shared consumers

External Uprising Xml (Uprising)/GlobalData/Armor.xml:1568 defines
SovietConYardArmor inheriting BaseFactionStructureArmor, the same parent as the
Allied yard. The flattened baseline independently matches the selected stock
asset's full BIN/RELO/IMP data: 856/8/0 bytes. The actual identity is
ArmorTemplate:SovietConYardArmor, InstanceId 57959878, not Allied's 8B801765.
The eighteen allowed float offsets remain unchanged; every other byte must
match the selected identity's baseline.

Read-only XML references identify multiple consumers:

- Soviet/Structures/SovietConstructionYard.xml:67.
- Japan/Structures/JapanConstructionYard.xml:64.
- Civilian/Amsterdam_AM/Buildings/AM_Futuretech_01.xml:198.

Thus SovietArmor is a shared armor override, **not** a promise that only Soviet
Construction Yards change. This reference-source relationship has not yet been
independently gameplay-tested with the new package. Creating a distinct armor
and rebinding a GameObject is outside the current alpha.

## Implementation and verification

ArmorAuthoringAdmission preserves a two-name allowlist through canonicalization
and exposes ArmorId in admission receipts. Ep1ArmorDocumentSmokeTest compiles the
same untouched stock parent values under the chosen identity and compares against
that identity in static.manifest before committing the authored output. Existing
type hashes, tokenizer, dependency guards and checksum rules are unchanged.
ArmorTrialPreparation derives its expected manifest name/InstanceId from the
admitted XML, independently recompiles payloads and rejects a foreign snapshot.
The alpha backend checks ArmorId in both stage receipts against selected XML;
older compiler receipts fail closed.

`artifacts/RebornSovietArmor-20261012-a` passes actual Core, own-stock baseline,
package readback and corruption/source mismatch checks. Its GUN=5000 BIG digest:
`0414E554AE9049631B163C508806F2A5EC2AE277030CE1A5DEE9374150C483BD`.

`armor-authored-build-self-test` in
`artifacts/RebornSovietArmorRegression-20261012-a` passes:

- all previous Allied legacy and eighteen-field/combined regressions;
- eighteen separate Soviet field=73 builds, each with actual stock comparison;
- Soviet GUN=5000 repeated byte-identical packaging and distinct manifest identity;
- rejecting an Allied snapshot for an already compiled Soviet diagnostic package.

Legacy Allied GUN=5000 and GUN=250 archive digests remain unchanged. Soviet
gameplay behavior is not proven by these offline tests. Earlier Allied GUN and
CANNON gameplay results do not automatically prove another identity's override.

## Sample

Select SovietArmor in the refreshed Studio workspace, or use
samples/armor/SovietConYardArmor.xml with the existing alpha backend. It changes
only GUN from 100 to 5000. Packaging remains isolated; building neither installs
files nor starts a game. Do not reuse the historical fixed Allied launch selector
for this different package. A new pinned candidate/preflight and separate launch
authorization are required for any Soviet gameplay test.

## Refreshed local package evidence

`UprisingAlpha-20261012-a` passes all four prebuilt samples: Reborn, Armor250,
ArmorCannon and SovietArmor, with selected-identity receipt checks, preserved
input/inventory and existing malformed-input/critical-callback controls.
`UprisingOverlay-20261012-a-OVERLAY-CANDIDATE.zip` passes binary privacy gating
and fresh extraction/setup with all four backend builds: 28 included files,
eight separately supplied EA/schema dependencies. ZIP SHA256:
`14ECD917F77F83BFD2A32815077FF9419AF51BE97FBFFE06E64E419A6DF56D36`.
The current admission self-test passes two identities x eighteen fields x four
values plus five legacy values and seventeen refusal inputs. The focused
document self-test also passes stock/output readback and production/precompiled
guards (`artifacts/RebornSovietArmor-document.log`). The broader compiler suite
last passed during the previous eighteen-field milestone; it was not rerun in
this identity-only increment. No GUI or game was started in this increment;
fresh UI observation and public redistribution review remain outstanding.

## Controlled gameplay preparation

`Invoke-Ra3Ep1ArmorTrial.ps1 -Soviet` now selects the pinned four-sample workspace
output `RebornAlphaInput-4cdaaed8530a4d5fa735fab8da5e37d9/build/package`.
The config is exactly the default-reader add-big fragment and the 3552-byte
archive must match the Soviet digest above. Combining Soviet with Cannon or
DefaultReader is refused before preflight. Without `-Run`, no game is started.
The read-only stock/config/package/process preflight passes with no existing
Uprising process (`artifacts/RebornSovietArmor-preflight-20261012.log`).

The external Uprising source binds the Conscript's primary weapon to
SovietAntiInfantryInfantryAK47 in
Soviet/Units/SovietAntiInfantryInfantry.xml:1809. GlobalData/Weapon.xml:3733
defines it, with active DamageType=GUN at line 3761. Its Molotov toggle is a
different secondary weapon and must not be used for the GUN check.

After separate launch authorization: start a fresh Soviet skirmish, keep the
Construction Yard unpacked, produce an unpromoted Conscript and force-attack
the yard in normal AK-47 mode. Record whether damage markedly increases relative
to stock. Avoid buffs and old saves. Exact 50x health loss is not an acceptance
requirement. An Allied yard, if separately available under comparable conditions,
is a foreign-identity control, not a prerequisite to reporting the first result.
Japan/civilian consumers need their own checks before being called gameplay
validated. No new game launch or gameplay observation has occurred yet.

## User-observed Soviet result — October 12, 2026

The user authorized one launch and answered "arttı" (increased) to the fresh
Soviet skirmish / normal Conscript AK-47 / unpacked Construction Yard test prompt.
This is positive qualitative gameplay evidence for the pinned SovietConYardArmor
GUN=5000 package, not an exact 50x damage measurement or an automated capture.
The reply did not separately report Japan/civilian behavior, an Allied armor
control, exact damage, game closure or all requested unit-state conditions.
Those additional facts are not inferred.

`artifacts/ArmorLaunch-3494f1bab7ee491aa26835821b0a66e2/launch.json` records one
launcher start with no observer failure and 37 matching config-argument child
observations. It is command-line evidence, not independent config-read or
manifest-load proof. Its conservative runtime-proof flags remain unchanged;
the user's report is recorded here as a separate evidence layer. No extra launch,
process termination or installation modification was used to save this result.
