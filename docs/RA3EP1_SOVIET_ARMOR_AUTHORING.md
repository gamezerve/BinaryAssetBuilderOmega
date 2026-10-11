# Second armor identity — October 12, 2026

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
