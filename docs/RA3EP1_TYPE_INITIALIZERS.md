# EP1 runtime hash initializer characterization — October 9, 2026

## Result

Limited static x86 instruction decoding establishes actual hash-table reads and
destination writes in **150 aligned straight-line initializer candidates** in
the pinned EP1 engine: **43 short templates** and **107 templates with an extra
vtable-shaped write**. Each reads a known name-associated hash slot, writes its
value to the same pushed object's `+8` address, and calls the same two targets.
This goes beyond raw address matching; exact instruction boundaries and operand
relationships are validated. It is not proof that every function is executed,
that the value survives subsequent calls, or that all runtime types use this path.

Covered examples include Texture, ArmorTemplate, AttributeModifier, GameObject,
LocomotorTemplate and WeaponTemplate. **PathMusicEvent is not covered** by these
two templates. The remaining **1,192 table names** require other paths or shapes;
they are not missing registrations merely because this narrow decoder skips them.

No authentic authoring ProcessingHash, aggregate hash derivation, plugin/cache
admission or game loading was recovered. No target code ran; the production
guards, official files and reference DLLs remain unchanged.

## Method and limits

Image SHA-256 is pinned to
`ABA6A8825A744A78487837D78AD5992962257CE68404F639D600F11DEEA2580B`.
The existing runtime table's 1,342 associations and independently SHA-checked
stock manifests supply the source-slot lookup. No fresh synthetic identity domain
is created. `scripts/Get-Ra3Ep1TypeInitializers.ps1` imports that read-only tool
once and searches only the pinned `.text` file region, `00001000..007D2000`.

`Read-TypeInitializer` decodes **only two exact 34-/44-byte instruction shapes**:
`A1` absolute EAX load, `68` immediate push, `A3` absolute EAX store, `E8` signed
relative call, optional `C7 05` absolute immediate store, `83 C4 08` stack
adjustment and `C3` return. Instruction meanings use the primary
[Intel IA-32 instruction-set reference](https://www.intel.com/content/www/us/en/content-details/851063/intel-64-and-ia-32-architectures-software-developer-s-manual-combined-volumes-2a-2b-2c-and-2d-instruction-set-reference-a-z.html).
This is a small reviewed decoder, not a general disassembler; local capstone,
iced-x86 and objdump/dumpbin tools were unavailable and none were installed.

Every candidate must be 16-byte aligned, fit the reviewed code region, read an
exact table hash address, target mapped raw `.data` addresses, preserve the
destination `pushed-object+8` relationship, and resolve both signed call operands
to `00417400` and `004D9A85`. The extended shape must write to the same object's
base and use a `.rdata`-mapped immediate as its vtable candidate. Names are unique
and the pinned accepted template count is 150. Exact routine bytes/SHA and all
operands are retained in output; the image is SHA-rechecked after inspection.

Preferred addresses are static PE VAs, not addresses observed in a running game.
No instruction prefixes, alternative register forms, branching bodies, shifted
function entries, relocation execution, startup reachability or entire table
call graph is admitted. Called-function semantics remain unproved. A hash transfer
to `+8` is not recovery of the complete runtime metadata object's ABI; subsequent
calls may modify state. Bounded snapshots are not atomic filesystem transactions.

## Texture example

Raw code offset `007BDF00`, preferred VA `00BBDF00`, 44 bytes, SHA-256
`3F0537FE986CE34EDDCB75764C0E290368D341EC17CCB2CC853A3AFC9FBD028F`.

| Instruction VA | Decoded instruction | Observed relationship |
| --- | --- | --- |
| `00BBDF00` | `mov eax, [00BE00D4]` | Texture slot contains `9F5FF8DA` |
| `00BBDF05` | `push 00CBE848` | first pushed object address |
| `00BBDF0A` | `mov [00CBE850], eax` | hash destination is object `+8` |
| `00BBDF0F` | `call 00417400` | common target, semantics unproved |
| `00BBDF14` | `push 00BCA440` | pushed code address, role unproved |
| `00BBDF19` | `mov dword [00CBE848], 00BF4AA4` | vtable-shaped base write |
| `00BBDF23` | `call 004D9A85` | second common target, semantics unproved |
| `00BBDF28` | `add esp, 8` | releases two pushed words |
| `00BBDF2B` | `ret` | end of admitted template |

The adjacent Texture name-pointer exception from the table audit remains
separate; this initializer reads its hash slot directly, not the parallel name
array. That is why the earlier literal hash-block base use led to this routine.

| Type | Raw initializer offset | Bytes | Object VA | Hash store VA |
| --- | --- | ---: | --- | --- |
| Texture | `007BDF00` | 44 | `00CBE848` | `00CBE850` |
| AttributeModifier | `007BE620` | 44 | `00CC29F4` | `00CC29FC` |
| GameObject | `007C2370` | 44 | `00CC8D38` | `00CC8D40` |
| ArmorTemplate | `007C27A0` | 34 | `00CC9018` | `00CC9020` |
| LocomotorTemplate | `007C27D0` | 44 | `00CC9028` | `00CC9030` |
| WeaponTemplate | `007C28E0` | 44 | `00CC9064` | `00CC906C` |

## Reproduce and tests

```powershell
# Reborn: decode only reviewed small instruction templates from the exact pinned image; do not execute or patch it.
./scripts/Get-Ra3Ep1TypeInitializers.ps1 -ImagePath 'D:\SteamLibrary\steamapps\common\Command and Conquer Red Alert 3 Uprising\Data\ra3ep1_1.0.game' -AsJson
# Reborn: verify routine bytes, operands, coverage, repeat JSON and detached-memory rejection cases.
./scripts/Test-Ra3Ep1TypeInitializers.ps1 -ImagePath 'D:\SteamLibrary\steamapps\common\Command and Conquer Red Alert 3 Uprising\Data\ra3ep1_1.0.game'
```

Tests pass: exact 150/43/107 coverage and repeated JSON; Texture routine digest
and hash destination; shorter Armor shape; uncovered music kept separate; eight
memory faults (opcode, source, destination, call, vtable target/range, alignment,
short-template second call); and rejection of another native image. Faults mutate
private arrays only after authentic baseline validation, not game/evidence files.
Runtime table and schema-role regressions also pass. These are standalone script
tests; C# compiler code was unchanged and no full compiler-suite rerun is claimed.

## Next gate

Review the common `00417400` body and initialization reachability before calling
these objects a complete active runtime registry. Establish the `+8` field's
runtime consumers and lookup contract for a small independently proven root.
Investigate uncovered music separately; do not force it into the accepted generic
templates or infer its authoring ProcessingHash from runtime metadata. Continue
to require native processor/dependency proof and an actual isolated game load.

Overall estimate remains **52% complete / 48% remaining**, compiler groups 165.
This advances code-use provenance but does not make the port a usable SDK.
