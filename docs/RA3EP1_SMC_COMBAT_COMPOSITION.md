# Required SMC helper tokenization and three-module composition

October 15, 2026. The selected object graph now adds the required
ObjectSMCHelperModuleData beside destruction and EP1 WeaponSetUpdate. Its native
model already had selected stock proof; this milestone supplies the **generated
inheritance metadata** and joins the helper into the common coordinate system.

## Native and token contract

`Ep1SmcHelperTokenizer` accepts exactly eight native bytes (class ID 7F72F242 and
weak local ID), with no relocations or strong imports. It emits a 168-byte prefix:
concrete wrapper, five marker/terminator tables for ObjectHelperModuleData,
UpdateModuleData, ContainModuleData, BehaviorModuleData and ModuleData, then
terminal TypeId/id field tokens. No stock prefix is copied into the writer.

The stock gate uses actual native GameObject dispatch and independently captured
Allied Construction Yard and Conscript bytes. Both complete native payloads and
all generated prefix bytes match after canonical coordinate comparison. Six
input refusals and 25 detached output corruption controls pass.

The root owner decoder admits one to three ordered canonical behavior pointers.
Eight-byte minimum owners are needed for SMC; each class-specific tokenizer still
enforces its own full size/shape. This does not allow an eight-byte destruction or
weapon module: those writers refuse truncation. A separate minimal 620-byte
no-Body/no-Armor native helper graph exercises the new lower boundary.

The explicit native selector adds SMC through its existing compatible dispatch;
unknown classes and duplicate/empty local behavior IDs still refuse. The shared
composer dispatches SMC to its dedicated writer, not a generic empty-class fallback.

## Combined evidence

`gameobject-smc-combat-self-test <static> <global>` covers all six permutations
of SMC/destruction/weapon modules, one/two weapons, zero/three body range modifiers
and absent/present root string storage: **48 actual-Core/native/output cases**.
The original two-module commands remain separate regression gates.

The independent verifier checks every helper inherited table and native identity
target in the composed address space, plus the existing root/armor/body/weapon/
destruction ownership and reference checks. SMC adds no strong references and
must not change the identities/order of the other source occurrences. Root field
count remains 89 when ArmorSet and Body are present.

`gameobject-authored-smc-combat-self-test <absolute-fresh-output> <static> <global>`
builds a real new armor stream and runs the same 48 combined cases. Target removal
must fail; this is not a synthetic metadata target. Stock/authored variants have
eleven/twelve input refusals and thirteen composed metadata controls each.

These are authored combined graph tests, not whole-stock-object equality or a
new playable unit. The original two stock helper prefixes and other selected
module native/token comparisons retain their separate proof boundaries.

## Files and remaining work

- `Ep1SmcHelperTokenizer.cs`, `Ep1SmcHelperTokenSmokeTest.cs`
- `Ep1GameObjectStringTokens.cs`: one-to-three native module owners.
- `Ep1GameObjectDestructionProjection.cs`: explicit SMC fragment selection.
- `Marshaler.Ep1SelectedBehaviors.cs`: explicit helper allocation/identity rules.
- `GameObjectCombatCompositionSmokeTest.cs`, `RootCombatPipeline.xsd`:
  optional helper position and official helper schema includes.
- Three additional Focused groups, bringing the batch to 41.

Remaining general object work includes movement/AI, draw/geometry and other root
graphs, more module writers, complete source/default/local-ID closure and checked
registry/stream/package integration. The alpha, installation and gameplay status
are unchanged. Distinct behavior IDs in this selector are not yet a complete
cross-container local-ID collector for arbitrary GameObject source graphs.

## Final regression

`artifacts/GameObjectSmcCombat-a/batch-report.json`: **41/41 Focused groups pass in
115.529 seconds**. SMC stock/token group takes 10.170 seconds; stock/authored
three-module groups take 3.960/4.590 seconds. The batch build succeeds in 1.16
seconds with zero warnings/errors. All eight runtime hashes remain unchanged;
GameExecuted and InstalledFilesModified are false. No Full-suite, complete consumer
stream/package or gameplay result is claimed.
