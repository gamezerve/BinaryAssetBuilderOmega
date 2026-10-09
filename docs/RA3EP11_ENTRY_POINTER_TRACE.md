# EP1 1.1 descriptor-to-TypeHash pointer trace — October 9, 2026

## Result and scope

One reviewed path in loader `004CE8F0` carries descriptor+12 into the fourth
argument of `004AB880`. The gate compares entry+8 with registered metadata+8.
This independently rebases the descriptor-to-consumer segment to 1.1; it does
**not** prove the upstream stream entry read, descriptor construction, every
branch or the other two gate callers. No native game/compiler/codec executed.
The scripts only read the pinned images and mutate detached arrays for tests.

Target: `D:\Program Files (x86)\Steam\steamapps\common\Command and Conquer Red Alert 3 Uprising\Data\ra3ep1_1.1.game`.
SHA-256: `B1DA83EFD229570BAB9DE3A14E48B2D1E6128E302BD52384858FA8A413AE330B`.
Locations below are preferred image VAs (base `00400000`), not live ASLR addresses.

## Independently reviewed instructions

The existing gate scan supplied raw call candidate `000CEFA0`. Disassembly
back to the preceding function boundary identified loader `004CE8F0`, rather
than applying the 1.0 address delta. The version check is word `[ebx+4] == 7`.

| Location | Observed connection |
| --- | --- |
| `004CEB59–004CEB72` | pending-record index multiplied by 28; stack record pointer EBP |
| `004CEB80–004CEB8D` | context+8 descriptor array; index times 20; EBX selects descriptor |
| `004CEB8D`, `004CEBA1` | descriptor+12 loaded into EAX, stored at pending record+4 |
| `004CEB86`, `004CEB9B` | descriptor+16 stored at pending record+24, a distinct pointer |
| `004CEC8C–004CECA8` | separate allocation/copy branch requests 30h bytes and copies 0Ch DWORDs |
| `004CEDCF–004CEDDE` | pending record selected for deferred processing; ESI = record+4 |
| `004CEED1–004CEED4` | a different call uses a payload-derived pointer; do not confuse it with the gate |
| `004CEF93` | push ESI in the reviewed eight-argument gate call |
| `004CEFA0` | direct call `004AB880` |
| `004AB88D` | callee loads ESI from adjusted ESP+40h |
| `004AB8CA`, `004AB8D5` | entry+8 loaded and compared with metadata+8 |

The eight right-to-left argument pushes at `004CEF5C`, `004CEF73`,
`004CEF7E`, `004CEF8B`, `004CEF93`, `004CEF94`, `004CEF95`, `004CEF96`
put entry ESI at argument four. The callee allocates 24h local bytes and
saves three registers before its 40h load: `40h - 24h - 3*4 = 10h`,
the fourth stack argument relative to entry ESP. The earlier push preceding
`00416710` is cleaned before this sequence and is not a ninth gate argument.
Virtual calls and intervening helpers have not been exhaustively audited;
this is static pointer-flow evidence in the reviewed caller, not a live trace.

The copy branch shows a 48-byte representation, but it does not establish that
the copied allocation itself is the pointer passed at this particular gate call.
Entry+12 is separately compared with cached data at `004CEBE8`; it must not be
collapsed into entry+8 TypeHash or treated as recovered authoring ProcessingHash.
Entry+44 has distinct branch uses and remains conservatively unnamed.

## Exact evidence and tests

| Raw offset | Bytes | SHA-256 |
| --- | ---: | --- |
| CE8F0 | 40 | `4CDEECD2EF5F73CB4693AAA168F6E6FC452FBB4E492A0B14E599C2F17BF8BB5F` |
| CEB4F | 85 | `8B7A74058728BB3DB25D8BD8E54DA2EEDF29622016C2FB195F8939712B82BBE6` |
| CEC8C | 30 | `385D535BA5D597248992B4DE6025EDAA7B7033164F2740ADD1FCFF6581EB01E0` |
| CEDCF | 39 | `F600D298F6D614E32020BF62D173E66C402F3DFCAB1DD528F3BDBD5C83C41271` |
| CEF44 | 97 | `3729F410E166836A4EE99FE236B87E5A57650F9E54B17CA06D3B9F832CD0F354` |
| AB880 | 96 | `2421A8766269EDE315C48AF93EDD47AB5CAD03817E5CA119C69A9EA434835E2B` |

`scripts/Get-Ra3Ep11EntryPointerTrace.ps1` imports the separately pinned 1.1
consumer/runtime evidence, keeps all version/SHA guards intact, pins six slices
and rereads the target SHA at completion. The consumer's three gate candidates
remain `000CEFA0`, `000CF6E0`, `000CFE26`; only the first has this reviewed
argument trace. Two slice hashes happen to equal their 1.0 counterparts,
but the image guard, independently found addresses and remaining hashes differ.

`scripts/Test-Ra3Ep11EntryPointerTrace.ps1` checks every stable report field
against repeat JSON. Three valid stack checks, four malformed stack cases and
seven detached code faults pass. Faults target the version, descriptor stride,
descriptor pointer, copy count, deferred pointer, entry push and callee load.
No production guard or compiler behavior changed. The 165-group compiler suite
was not rerun for this standalone static audit.

```powershell
# Reborn: reproduce the bounded 1.1 pointer-flow evidence without executing the target image.
./scripts/Test-Ra3Ep11EntryPointerTrace.ps1 -ImagePath 'D:\Program Files (x86)\Steam\steamapps\common\Command and Conquer Red Alert 3 Uprising\Data\ra3ep1_1.1.game' -BaselineImagePath 'D:\Program Files (x86)\Steam\steamapps\common\Command and Conquer Red Alert 3 Uprising\Data\ra3ep1_1.0.game'
```

## Remaining migration gates

Next independently locate the concrete 1.1 reader vtable and inspect header read,
48-byte stream entry read and descriptor producer. Then rebase linked sidecar
loading, factories and ownership/lifetime evidence. Do not reuse 1.0 fixed
addresses or relax its guards. Existing 1.0 reports remain version-scoped.

This result reinforces that merely swapping XML/XSD cannot establish compiler
compatibility. Authentic authoring ProcessingHash, marshaller ABI, unresolved
AUDIO inputs and actual package loading remain separate blockers. The weighted
engineering estimate remains **52% complete / 48% remaining**, not a count of
commits or evidence files. No usable SDK or loaded mod is claimed.
