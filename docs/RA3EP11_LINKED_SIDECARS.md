# EP1 1.1 linked-sidecar setup — October 9, 2026

## Result and limits

The previously unresolved post-read helper `00449810` performs linked-sidecar
setup. Its complete 1,055-byte body never reads the supplied entry-buffer/count
arguments; direct writes prepare reader sidecar buffers, cursors and binary
handle, not entry hash/size fields. This resolves the specific helper-role
question in the [1.1 descriptor producer](RA3EP11_DESCRIPTOR_PRODUCER.md).
It does not establish absence of all indirect callback effects, arbitrary
aliasing, other reader implementations or a complete stream-pointer trace.

All twelve `.bin/.relo/.imp` companions of the four already captured stock
manifests have matching eight-byte headers, manifest checksums and exact lengths
equal to eight plus projected entry chunk totals. **WorldBuilder.bin is included
at 1,394,571,528 bytes**, but only its eight-byte header is read per pass. It is
a linked asset stream, not a PE executable; no full binary dump is needed.

Native game/compiler/codecs were not executed. No game installation, reference
file, compiler policy or old 1.0 offset guard changed. Stock evidence is not a
new 1.1 BIG archive extraction or proof of full payload integrity.

## Independently reviewed 1.1 flow

Following the concrete entry method's direct call at `00449C73` identifies
helper `00449810`. Full disassembly through both returns establishes the flow
below. Direct calls from that helper identify buffer reader `0043D050`; it was
also disassembled through its return. Three referenced filename literals were
read separately and pinned. No assumed global address delta is evidence.

| Preferred VA | Reviewed connection |
| --- | --- |
| `00449842` | suffix pointer `00BF878C`, `.imp` |
| `00449856` | whole-file buffer helper `0043D050`, linked state+0 |
| `00449876–0044987D` | second imports DWORD versus reader+24h; state+14h cursor +=8 |
| `004498F8`, `0044990F` | suffix `00BF86EC`, `.relo`; buffer helper on state+8 |
| `0044992F–00449936` | second relo DWORD versus reader+24h; state+18h cursor +=8 |
| `004499B1–004499E6` | suffix `00BF864C`, `.bin`; open through `004D84F0`, store state+1Ch |
| `00449A88–00449B29` | read two consecutive DWORDs; check each read returns four bytes |
| `00449BA7` | second binary DWORD compared with reader+24h checksum |
| `00449A7A`, `00449C21` | false and true returns respectively |

The binary open flags depend on reader byte+69h, selecting 940h or B40h.
Their complete external file-manager semantics are not recovered here.
Reader+24h was previously traced to manifest header+8 StreamChecksum.
The first binary DWORD is read but not directly compared with a magic constant
in this helper. Imports/relo similarly compare only the second DWORD.

Imports/relo checksum mismatch, binary short reads and binary checksum mismatch
enter diagnostic paths. Calls to `0040EDE0` and logger global `00CF069C`
virtual+68h can bypass those diagnostics. Therefore **unconditional checksum
rejection is not proven**. Skipping a diagnostic is neither permission for the
compiler to emit invalid headers nor proof that a mod will load successfully.
The audit's magic and exact-length checks are stronger diagnostic checks, not
inferred native engine acceptance policy.

`0043D050` opens a file, asks its size through virtual+2Ch, grows the supplied
buffer when required, reads the requested file size through virtual+0Ch,
compares the returned byte count, closes through virtual+8 and returns that
comparison. Allocation failure, reused-state resets and external callbacks are
not promoted to safety guarantees. **The audit never runs this native helper
and never reproduces its full-file payload read.**

Descriptor cumulative positions still start at zero. Sidecar cursor increments
and binary header consumption are separate state. Their relationship to every
downstream absolute read needs the next chunk-dispatch/range audit.

## Header-only stock validation

| Family | Manifest checksum | `.bin` bytes | `.relo` bytes | `.imp` bytes |
| --- | --- | ---: | ---: | ---: |
| global | `85FE7DA0` | 2,768,548 | 239,408 | 221,636 |
| static | `EA62EA71` | 357,478,164 | 6,916,336 | 124,536 |
| worldbuilder | `F2AC0280` | 1,394,571,528 | 21,616,304 | 113,732 |
| audio | `395CE3A2` | 940,776 | 108,736 | 8 |

Observed little-endian magic words are `BABB0000` for bin, `BABE0000` for relo
and `BAB10000` for imp, followed by the manifest checksum at +4. Header-only
audio.imp agrees with zero imports payload. Files are resolved from the four
pinned absolute manifest paths in `docs/RA3EP1_TYPE_TABLE_EVIDENCE.json`, under
`D:\TEMP\Red Alert 3 Uprising Source Data`. No new archive extraction occurs.

Each audit invocation reads twelve headers twice: 96 bytes per pass, **192
sidecar bytes total**, zero sidecar payload. The test invokes the audit twice,
so its aggregate header read is 384 bytes. Images and bounded manifests are
also read separately and are not included in that sidecar-only byte counter.
The header helper requires absolute regular files, rejects reparse ancestors,
uses a read-only handle and checks length stability. The second pass compares
headers, lengths and modification timestamps; this is not an atomic snapshot
or cryptographic validation of the unexamined payload.

## Exact evidence pins

Image SHA-256:
`B1DA83EFD229570BAB9DE3A14E48B2D1E6128E302BD52384858FA8A413AE330B`.
Target is `D:\Program Files (x86)\Steam\steamapps\common\Command and Conquer Red Alert 3 Uprising\Data\ra3ep1_1.1.game`.
Addresses above are preferred VAs; offsets below are raw image offsets.

| Raw offset | Bytes | SHA-256 |
| --- | ---: | --- |
| 49810 complete setup | 1,055 | `C86914D089518C61A04CA633DACEC6FC939C5D1A305B64D5972334B003800D7A` |
| 3D050 complete buffer reader | 112 | `7A8F3BB19F59DC4BBF51B31767F4D9AA124A2169146E096192B6E90B0F4F2442` |
| 7F878C `.imp` including NUL | 5 | `04077A9D880567DF6980E81A38FB572AE05EF0BA8EE8C4BB63BD33E5439589FD` |
| 7F86EC `.relo` including NUL | 6 | `04C0BF178A538B9D127431C1B097C03A3D3B86FF076F6507B9417F8BD1801433` |
| 7F864C `.bin` including NUL | 5 | `812F74EB5FB37FE97ABC1890F84760B460B026DEA3A8D2B9495D449C335F7C01` |

`Get-Ra3Ep11LinkedSidecars.ps1` reuses the bounded header/projection validation
approach from 1.0, with independently reviewed 1.1 bodies/literals and strict
1.1 image guard. `Test-Ra3Ep11LinkedSidecars.ps1` validates all stable nested
report fields against repeat JSON. Four detached bad-header cases (magic,
checksum, length and truncation) and eleven detached instruction/literal faults
must fail. The large source streams are never modified.

```powershell
# Reborn: reproduce 1.1 linked setup evidence and header-only WorldBuilder-inclusive validation without native execution.
./scripts/Test-Ra3Ep11LinkedSidecars.ps1 -ImagePath 'D:\Program Files (x86)\Steam\steamapps\common\Command and Conquer Red Alert 3 Uprising\Data\ra3ep1_1.1.game' -BaselineImagePath 'D:\Program Files (x86)\Steam\steamapps\common\Command and Conquer Red Alert 3 Uprising\Data\ra3ep1_1.0.game'
```

## Next gate

Rebase concrete queue/dispatch/chunk reads and descriptor-range consumption,
then factories, source ownership and context lifetime. Authentic authoring
ProcessingHash, unresolved AUDIO inputs, native emission ABI and actual
modconfig package loading remain separate blockers. The 165-group compiler
suite was not rerun for this static audit. Weighted effort remains **52%
complete / 48% remaining**; no usable SDK or loaded mod is claimed.
