# EP1 runtime registry/lookup characterization — October 9, 2026

## Result

The shared initializer call at preferred VA `00417400` is a linked-list
registration body. A neighboring 34-byte lookup at `00417340` traverses the
same list, compares **TypeId**, and returns the registered metadata object
pointer, or zero when not found. **This lookup does not read TypeHash**.
It is therefore not the missing stream/type-hash validation stage.

All **150 initializer-associated static objects** have a raw `+4` TypeId
matching the current production FastHash provider for their exact names.
**138** are roots in the independently pinned stock manifest evidence, and
their TypeIds also match that evidence. Their raw file `+8` words are zero
before the initializer copies the independently observed runtime table hashes.
This corroborates the distinct TypeId and initialized TypeHash domains; it
does not recover authoring ProcessingHash or the entire metadata ABI.

The raw object `+12` word must remain **opaque**: it differs from stock
Tokenized in **122/138** observed root cases. Only 16 agree (13 zero/zero and
three one/one). Guessing that word is a Tokenized flag would be incorrect.
We have not identified its actual semantics.

## Exact code provenance

Pinned engine SHA:
`ABA6A8825A744A78487837D78AD5992962257CE68404F639D600F11DEEA2580B`.

| Body | Raw offset | Preferred VA | Bytes | SHA-256 |
| --- | --- | --- | ---: | --- |
| Registration | `00017400` | `00417400` | 211 | `F6347C4BE68B782CF6889175AAD7EFA3FD06E918A2431BDB6746C2699A77E9F1` |
| Lookup | `00017340` | `00417340` | 34 | `DBEE0BF378AECD49BCF8D0DFE9418BF39C6E61760B37C1F22A71D20547B32A3E` |

These are reviewed static body boundaries/pins, not runtime reachability or
execution traces. The image was not loaded as a native library or executed.
Instruction interpretation follows the same narrow IA-32 approach and primary
Intel reference used in [initializer evidence](RA3EP1_TYPE_INITIALIZERS.md).

## Registry structures and control flow

Both bodies access list head VA `00CEA428`, which lies in the virtual `.data`
range beyond its raw file data. No fabricated disk bytes were read at that VA.
The registration body reads TypeId from the pushed object's `+4` field. Its
search loop compares node `+4` against that TypeId, advances through node `+0`,
and checks node `+12` when encountering the same key. Duplicate/error handling
includes indirect diagnostics; its complete policy/called semantics are not
recovered and must not be transplanted into the compiler.

The insertion path pushes size 16 and calls `004168F0`, then writes the new
record and publishes the new list head. The allocation-size argument and
following writes are observed; the called allocator's implementation and
failure behavior have not been analyzed.

| Offset | Inserted list-node content | Evidence |
| --- | --- | --- |
| `+0` | previous list head | links the new head |
| `+4` | object TypeId | search/lookup key |
| `+8` | repeated object TypeId | same ESI value stored twice; **not TypeHash** |
| `+12` | metadata object pointer | lookup return value |

The node and object layouts are separate: the initializer's hash write targets
**object `+8`**, not **list-node `+8`**. Confusing them would turn a TypeId copy
into a false hash-validation proof.

The lookup body reads its argument, scans `node+4`, follows `node+0`, returns
`node+12` on match and zero after an empty/exhausted list. Its exact bytes are:

```text
A128A4CE0085C074128B4C24048D490039480474098B0085C075F533C0C38B400CC3
```

It contains no `+8` metadata/hash read and no calls. Whether its callers later
compare a loaded stream TypeHash against the returned object's `+8` field remains
the next investigation target; this report does not claim that consumer path.

## Static object examples

| Object | Raw TypeId at `+4` | Raw word at `+8` | Unknown word at `+12` |
| --- | --- | --- | --- |
| Texture | `21E727DA` | zero | zero |
| ArmorTemplate | `3A6C5E8E` | zero | one |
| AttributeModifier | `C5E07887` | zero | one |
| GameObject | `942FFF2D` | zero | one |
| LocomotorTemplate | `ECC2A1D3` | zero | one |
| WeaponTemplate | `94D4D96E` | zero | one |

The first word is a raw vtable-shaped pointer, not a fully recovered class
definition. Some initializer shapes replace it after their shared registration
call. Subsequent constructor calls could modify other fields; this is a static
pre-initialization snapshot plus instruction evidence, not final runtime state.

## Reproduce and validation

`scripts/Get-Ra3Ep1RegistryEvidence.ps1` uses `Assert-RegistryBodies` and
`Read-RegistryObjects`, importing the existing pinned initializer/table tools.
It retains their bounded input policies, checks exact body SHA pins, verifies
object reads against raw `.data` boundaries, and preserves unknown words.
The only executable it runs is the **already built repo-owned managed inspector's
read-only `hash` command**, for 150 exact names. It never executes the game,
reference compiler or codec DLL. The independently pinned stock evidence bytes
are rechecked, and the engine is SHA-rechecked after review. These are bounded
snapshots, not atomic concurrency or complete-stream integrity guarantees.

```powershell
# Reborn: statically review registry bodies/objects and invoke only the managed name-hash diagnostic, not the game.
./scripts/Get-Ra3Ep1RegistryEvidence.ps1 -ImagePath 'D:\SteamLibrary\steamapps\common\Command and Conquer Red Alert 3 Uprising\Data\ra3ep1_1.0.game' -AsJson
# Reborn: test exact body pins, object identities, opaque flags and repeat JSON using private memory faults.
./scripts/Test-Ra3Ep1RegistryEvidence.ps1 -ImagePath 'D:\SteamLibrary\steamapps\common\Command and Conquer Red Alert 3 Uprising\Data\ra3ep1_1.0.game'
```

Tests passed: 150 name/object TypeIds, 138 stock-root identities, exact repeat
JSON, 122 opaque-word/tokenized mismatches, reviewed Texture/Armor examples,
four private faults (lookup, registration, TypeId, raw hash), and preservation
of an arbitrary opaque `+12` word without Boolean/tokenized coercion. The existing
initializer regressions also pass. Fault arrays are detached; no game or schema
files are modified. C# compiler code was unchanged and no full compiler-suite
rerun is claimed; the existing count stays 165 groups.

## Next gate

Follow callers of the small lookup and inspect returned-object hash consumers,
with bounded call-site/control-flow and stream metadata evidence. Keep startup
reachability, full registry coverage, unknown flag semantics and authoring
ProcessingHash as separate questions. Do not change compiler registration,
cache keys or production AllTypesHash merely because runtime lookup is understood.

Overall estimate remains **52% complete / 48% remaining**. Runtime key/object
relationships are clearer, but no working production SDK or game load is proved.
