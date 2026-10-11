# Expanded ConYard armor authoring — October 11, 2026

## Scope

The alpha authoring gate now accepts all thirteen ordered Armor Percent values
and five optional root scalars of AlliedConYardArmor. Each value must be a
canonical integer 0..10000; omitted root scalars default to 100. The percentage
is converted to a multiplier using the existing marshaler's single-precision
`value * 0.01f`, not a newly invented representation. This matters for values
such as 73, whose multiplication/division rounding can differ.

Optional fields: Default, DamageScalar, SideDamageScalar, RearDamageScalar,
FlankedPenalty. The thirteen original damage names/order remain mandatory.
No new armor identity, inheritance, Include, list insertion/removal/reordering,
formula, decimal, percent suffix or arbitrary root attribute is admitted.
Other asset types and general production/cache reuse remain closed.

This reuses the existing EP1 processor, marshaler and tokenizer. It expands the
authoring/verification envelope rather than rewriting the compiler or assuming
KW fallback is valid. Only the earlier GUN=5000 package has user-observed gameplay
evidence; expanded coefficients/scalars are experimental until controlled tests.

## Implementation

- ArmorAuthoringAdmission.Admit validates the fixed shape and canonical values,
  canonicalizes through the fixture and independently verifies each value.
- Values maps five root slots at offsets 724..740 (stride 4) and thirteen list
  floats at 756..852 (stride 8); GUN remains offset 788. VerifyPayload checks all
  eighteen float bits then requires every remaining byte to match stock.
- Ep1ArmorDocumentSmokeTest.WriteConYardOverride always constructs stock baseline
  from the untouched fixture with GUN=100, not from authored non-GUN/scalar edits.
  Actual Core/OutputManager output is compared to this baseline and XML values.
- ArmorTrialPreparation independently recompiles the admitted XML when verifying
  archive slices. Identity/token data, RELO, IMP and linked-stream contracts remain
  unchanged. Default-reader packaging still has fifteen entries.
- Admission/authored/alpha receipts carry all eighteen Percentages and
  EditableFloatCount=18. The backend compares the complete field set, not just GUN.
  OnlyGunFloatEditable is now false; admission receipts still prove no package.
- Studio labels, ALPHA002 warning and local help describe the expanded scope.
  Existing ALPHA100/ALPHA101 failure behavior is retained.

## Evidence

`armor-authoring-input-self-test` passes the five legacy GUN values, eighteen
fields x four values (0/73/250/10000), and seventeen malformed/expanded-input
refusals. `armor-authored-build-self-test` in
`artifacts/RebornArmorExpanded-20261011-a` passes the original five packages,
fresh legacy Core identities, repeat GUN-250 and exact game-tested GUN-5000
digest. It then builds eighteen separate field-73 packages plus an all-fields-250
package and its repeat. Actual stock comparison, independent payload readback,
single-field isolation, full-byte repeat and existing corruption/source-mismatch
controls pass. No game is launched.

`UprisingAlpha-20261011-g` passes prebuilt backend tests for Reborn, Armor250 and
ArmorCannon, input/inventory preservation, DTD refusal and critical callback
marker checks. Exact EA CodeDOM reference/interface/config/options harness also
passes, without launching Studio. It is local evidence, not a newly observed
GUI run. The first packaging attempt caught stale debug-symbol outputs after
switching build modes; workspace preparation now forces non-incremental builds.
The refused overlay was not created; its audit is retained for diagnosis.

After the non-incremental packaging fix, `UprisingAlpha-20261011-h` passes all
three prebuilt sample builds and the existing negative/callback-marker controls.
`UprisingOverlay-20261011-f-OVERLAY-CANDIDATE.zip` passes the binary privacy gate
and fresh setup/extraction test: 27 payload files, eight separately supplied
EA/schema dependencies, three successful offline builds. No private path hints
or managed absolute CodeView paths remain. The native .NET apphost retains its
documented vendor build metadata; this is not a claim that all binary paths are
absent. ZIP SHA256:
`1B4792D1684CEC2CC6E13A14DE5594D64DF64545ED951A95F1C9C6B490CE8957`.
Public redistribution review and new GUI/game observations remain outstanding.

The complete `compiler-self-test` also passes. Its initial sandboxed run could
not rename an owned temporary audio-test file; the approved rerun completed
successfully (`artifacts/RebornArmorExpanded-compiler-self-test-elevated.log`).
This exercised the broader existing regression suite without enabling those
other asset capabilities in the alpha UI. A separate backend scalar build with
Default=73 also passes full eighteen-field receipt comparison.

ArmorCannon sets CANNON=5000, GUN=100 and preserves every other stock coefficient.
Its independently verified BIG SHA256 is
`40C09781BAB82643FBBC6DF5A8DFA674B74BF16B8B93F4A5B605FF5778F0BE7F`.
This is an offline sample, not a gameplay result.

## Usage

Edit mods/Reborn/data/mod.xml in a current alpha workspace, or choose ArmorCannon
for a non-GUN example. Root attributes use the same percent units:

```xml
<!-- Reborn: example scalar attributes only; keep all thirteen ordered Armor children from the supported sample. -->
<ArmorTemplate xmlns="uri:ea.com:eala:asset" id="AlliedConYardArmor"
               DamageScalar="250" SideDamageScalar="100" RearDamageScalar="100">
  <!-- Reborn: insert the complete existing thirteen-entry list here; this abbreviated excerpt is not buildable alone. -->
</ArmorTemplate>
```

No mod is installed and no game is started by a build. Next controlled gameplay
check should distinguish CANNON editing from unchanged GUN, using a fresh match
and weapons whose actual damage types have been verified. Scalar combinations
can interact; do not assume a damage multiplier from a single coefficient without
considering those fields. New runtime tests require separate launch authorization.
