# Schema-correct ObjectResourceInfo and BuildCost

<!-- Reborn: optional resource records and source-sized transactions implement the actual XSD instead of a one-row Conscript example. -->
The alpha implements the official GameObject resource grammar:

| Source construct | Rule |
|---|---|
| GameObject/ObjectResourceInfo | Optional, at most one |
| ObjectResourceInfo/BuildCost | Zero or more transactions |
| BuildCost/@Account | Required when the transaction exists; SageUnsignedInt |
| BuildCost/@Amount | Required when the transaction exists; SageUnsignedInt |

Definitions are in `schemas/ra3ep1/xsd/AssetTypeGameObject.xsd` and
`schemas/ra3ep1/xsd/Includes/Base.xsd`. Schema-approved source spellings and
prepared expressions still have to fit an actual native uint32. Negative,
malformed and overflowing values are rejected at their source fields.

## Preserved distinctions

- Absent ObjectResourceInfo: null native root pointer, no resource allocation
  and no relocation for that pointer; the root metadata declaration has a null target.
- Present empty ObjectResourceInfo: allocated native record with zero count
  and null items pointer, with its own resource metadata.
- Explicit zero transaction: a real list row with Account and Amount=0.
- Multiple transactions: source-sized array and metadata, in authored/effective order.

The compiler never replaces absence with a zero-cost row. Existing native
MoneyTransaction processing remains responsible for producing bytes; the new
GameObjectResourceContract independently checks presence, allocation, counts,
metadata grammar and required values. GameObjectNestedValueContract checks every
transaction's uint32 representation. Root-owner slicing uses the first actually
allocated tail owner rather than assuming ObjectResourceInfo always exists.

Comparison-only resource/armor/movement normalization locates the next schema
family, not a fixed child ordinal, so optional earlier records do not corrupt
comparison XML order. Authored XML, definitions and inheritance semantics are
not replaced with stock source.

## Receipts and scope

ResourceInfoPresent and BuildCosts distinguish absent, empty and populated
records. Account/Amount are reported as exact uint32 values, including
4294967295. Legacy EditableValues.Amount is null when there is no unique cost
row. Mixed receipts report each GameObject's resource presence and cost count.

This does not enable every other optional GameObject record or an arbitrary
BaseInfantry-only object topology. Genuine unimplemented behavior/root layouts
remain errors. Missing-owner diagnostics now attach to the actual object node
rather than falling back to the first emitted GameObject.

## Developer verification

The `resource` group of `conscript-module-graph-self-test` tests five complete
native/metadata/stream/BIG cases: omitted owner, present-empty owner, explicit
zero cost, forty rows, and hexadecimal uint32 maximum. It also checks required
Account/Amount, duplicate resource owners, invalid/negative/overflow values,
first/last row and metadata corruption, deterministic output, and receipt fidelity.

The SDK's MultipleObjectsOnly integration combines omitted and forty-row costs
with zero/forty movement lists across three objects and shared support assets.
A missing Amount must report original XML coordinates and preserve the last
successful package. Developer data is restored and automated final output is
kept under alpha/.work, never the user's Documents or Reborn mod.

These checks establish offline build correctness, not gameplay behavior for
arbitrary resource account IDs.

<!-- Reborn: preserve measured outcomes and explicitly distinguish individual compiler checks from the final workspace-ownership guard. -->
The five resource cases passed full stream/package readback and repeatability;
45 native and 74 metadata corruptions were rejected. The source controls rejected
missing Account/Amount, negative/invalid/overflow values and duplicate owners.
XML source-location regression passed 25 checks.

The SDK mixed package and restored developer package both published successfully.
The missing-Amount case reported MixedAssets.xml(1747,18), failed as expected,
and retained the previous successful package. The final SDK batch summary was
not certified: its user-source hash guard detected Reborn/mod.xml being edited
during the run (273 to 616 bytes). The test did not restore or overwrite that
user file. Treat the individual build/diagnostic results above as evidence, not
the older dev-sdk-test.json as a fresh batch PASS.
