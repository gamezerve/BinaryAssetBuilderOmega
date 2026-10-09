# EP1 1.0 sentinel insertion and reader lifetime — October 9, 2026

This follow-up resolves the previous head+8 ambiguity **under healthy circular
sentinel invariants**. It remains scoped to the pinned 1.0 image, not the newly
located 1.1 image. See [1.1 baseline and modconfig](RA3EP11_MODCONFIG_BASELINE.md).

## Insertion alias and consumption

Let S = owner+0Ch (embedded sentinel), F = [S] (first node), N = newly allocated
node. Empty lists have F=S. A healthy list has [F+4]=S in both empty/nonempty
cases. The observed `004CE7B3`..`004CE7C7` sequence is:

1. [N]=F; [N+4]=[F+4].
2. P=[F+4]; [P]=N. Because P=S, this updates **[S]=N** through an alias.
3. [F+4]=N. For an empty list F=S, this also updates sentinel.prev.
4. Reload A=[S]; [A+8]=reader. Now A=N, so this stores the reader in **N+8**.

The helper need not write through owner ECX: the caller changes sentinel.next
indirectly through the previous link. Earlier reports deliberately withheld
fresh-node provenance because this alias had not yet been analyzed. The literal
head+8 store observation remains correct; the healthy-list consequence is now
resolved. It is not evidence that malformed or arbitrary live lists are safe.

The pinned iterator `004D01E0` follows [S], consumes node+8 via a source handle,
then follows node.next to S. Synthetic empty/one/two/three-reader scenarios
confirm newest-first traversal (33,22,11 after inserting 11,22,33). A specific
runtime execution trace or all-global-list initialization is not claimed.

## Cleanup versus release

Concrete slot+24h points to `004184D0`. It calls source+18h's virtual+8 method
if non-null and clears source+18h. For linked-state source+54h, it similarly
releases linked+1Ch, frees linked+8 and linked+0 buffers and the linked-state
allocation, then clears source+54h. It does **not** free the reader object in
its reviewed body. Virtual callbacks are not assumed harmless or fully modeled.

The source-list release slice `004ABB2B`..`004ABB50` walks owner+0Ch, calls
each node+8 reader's virtual+8, then calls `00496FD0` on the sentinel and
restores both sentinel links to S. The node cleanup helper walks next pointers,
cleans nested node resources and frees nodes. Its nested helper semantics and
all caller paths are not asserted here.

For the concrete reader table, slot+8 is `00417190`. It dispatches virtual+3Ch
with argument 1. Slot+3Ch is `0049F4A0`, which calls `0049B390` and then frees
this via `00416980` when argument bit 0 is set. This establishes the conditional
object-free path for the reviewed concrete table, separate from slot+24h.
The full `0049B390` destructor/cache lifetime is still unfinished. Do not
interpret resource cleanup as deletion or claim cached objects survive release.

## Evidence and tests

Image SHA: `ABA6A8825A744A78487837D78AD5992962257CE68404F639D600F11DEEA2580B`.
Seven exact slice pins are recorded in `Assert-ReaderListLifetimeCode`:
raw CE7A2/40, 184D0/89, ABB2B/38, 96FD0/95, 17190/14,
7F2E78/64 and 9F4A0/30. Earlier factory/ownership code pins remain inherited.

```powershell
# Reborn: reproduce scoped 1.0 insertion-alias and cleanup/release evidence without native execution.
./scripts/Get-Ra3Ep1ReaderListLifetime.ps1 -ImagePath 'D:\SteamLibrary\steamapps\common\Command and Conquer Red Alert 3 Uprising\Data\ra3ep1_1.0.game' -AsJson
# Reborn: check four insertion scenarios, two broken-list refusals, twelve private byte faults and repeat JSON.
./scripts/Test-Ra3Ep1ReaderListLifetime.ps1 -ImagePath 'D:\SteamLibrary\steamapps\common\Command and Conquer Red Alert 3 Uprising\Data\ra3ep1_1.0.game'
```

The synthetic model uses a detached dictionary, validates healthy linkage and
rejects malformed previous links/cycles under a 100-node diagnostic bound.
These refusals are tool policy, not native guards. No allocator, callback,
destructor or game code executes. Inherited metadata checks and the repo-owned
managed name-hash diagnostic still run; asset payloads are not read. Tests and
ownership regressions pass. No full compiler suite rerun: previous 165 groups.

## Remaining gates

Global initializers/all-call-site invariants, concrete root/cache provenance,
full destructor behavior and arbitrary callbacks remain unresolved. The user
located a distinct 1.1 binary during this work; rebase runtime/type/stream evidence
for that image before treating 1.0 findings as 1.1 compatibility. Overall weighted
effort remains **52% complete / 48% remaining**, with no production SDK/game proof.
