# EP1 linked sidecar setup — October 9, 2026

## Result: correction to the previous open hypothesis

The complete reviewed helper at preferred VA `00449760` performs **linked
sidecar setup**, not an asset-entry transformation. The previous descriptor
report left that role unresolved and provisionally described it as possible
linked postprocessing. This audit resolves that specific question.

The helper receives an entry buffer/count at its call site but never reads
those supplied arguments in its reviewed 1,055-byte body. It does not directly
write entry TypeHash, word44 or chunk sizes, nor forward the entry buffer to
its reviewed file/formatting calls. Its direct writes target reader sidecar
state: imports/relocations buffers, their cursors, and the binary stream handle.
This does not prove absence of arbitrary side effects in diagnostic callbacks,
all possible aliases or other reader implementations. It is static instruction
evidence, not a live execution or full interprocedural verification.

All twelve `.bin/.relo/.imp` companions of the four pinned EP1 manifests have
an eight-byte header, matching checksum and exact file size equal to eight
plus the sum of corresponding manifest chunk sizes. **WorldBuilder.bin is
included** at 1,394,571,528 bytes, without reading its payload or taking a binary
dump. It is an asset stream, not a PE executable.

## Recovered setup flow

| Preferred VA / step | Reviewed behavior |
| --- | --- |
| `00449791`–`00449798` | Replaces filename extension with `.imp` |
| `004497A6` | Calls full-file buffer helper `0043D040` on linked state+0 |
| `004497C4`–`004497CD` | If buffer exists, reads its second DWORD and compares to reader+36 (retained manifest checksum); adds 8 to imports cursor at linked state+20 |
| `00449847`–`0044985F` | Replaces extension with `.relo`, loads into linked state+8 |
| `0044987C`–`00449886` | Checks second DWORD against manifest checksum; adds 8 to relocation cursor at linked state+24 |
| `00449900`–`00449936` | Replaces extension with `.bin`, opens and stores handle at linked state+28 |
| `004499D8`–`00449A77` | Reads two DWORDs sequentially from binary stream, checks returned byte counts |
| `00449AF3`–`00449AFA` | Compares second binary DWORD with reader+36 checksum |
| `00449B71` | Returns true after setup path; other failure branches return false at `004499CA` |

`0043D040` opens a file, obtains its size through a virtual method, grows a
buffer if necessary, requests all file bytes, compares returned count to size,
closes the handle and returns that equality. Its 112-byte body is separately
pinned. **The audit script does not execute this helper or mimic its full-file
read**; it opens actual sidecars only to read eight bytes.

Imports/relocations checksum mismatch and binary short-read/checksum mismatch
use the same diagnostic-control pattern as the previously reviewed TypeHash
gate. Diagnostic suppression can continue the path. Thus this audit proves
checksum comparison/diagnostic paths, **not unconditional engine rejection**.
The helper does not establish a magic-word validation rule. Our audit's magic
and exact-length checks are deliberately stronger diagnostic checks, not
inferred native acceptance policy. No compiler guard is loosened.

The observed cursor increments start after the eight-byte header. They are not
proof of every downstream absolute read offset, stream seek behavior or reused
state reset. In particular, descriptor sums still start at zero; header/cursor
handling is separate state rather than a change to the manifest entry layout.

## Actual sidecar evidence

Each header contains little-endian magic at +0 and manifest StreamChecksum at
+4. The magic words below are observed in all twelve selected files and match
existing repo stream readers/writer fixtures; the helper's first binary word
is read but not directly compared to a magic constant in this body.

| Family | Manifest checksum | `.bin` bytes | `.relo` bytes | `.imp` bytes |
| --- | --- | ---: | ---: | ---: |
| global | `85FE7DA0` | 2,768,548 | 239,408 | 221,636 |
| static | `EA62EA71` | 357,478,164 | 6,916,336 | 124,536 |
| worldbuilder | `F2AC0280` | 1,394,571,528 | 21,616,304 | 113,732 |
| audio | `395CE3A2` | 940,776 | 108,736 | 8 |

| Extension | Observed magic | Payload size basis |
| --- | --- | --- |
| `.bin` | `BABB0000` | Sum of entry InstanceDataSize |
| `.relo` | `BABE0000` | Sum of entry RelocationDataSize |
| `.imp` | `BAB10000` | Sum of entry ImportsDataSize |

The header-only `audio.imp` is valid under these metadata checks because its
raw import-size sum is zero. All sidecars are resolved with ChangeExtension
from the four pinned absolute manifest paths in
`docs/RA3EP1_TYPE_TABLE_EVIDENCE.json`, including the local unpacked WorldBuilder
directory. No archive unpacking, source/schema editing or game-file mutation
is performed.

## Exact code provenance

Pinned engine SHA-256:
`ABA6A8825A744A78487837D78AD5992962257CE68404F639D600F11DEEA2580B`.

| Body | Raw offset | Bytes | SHA-256 |
| --- | --- | ---: | --- |
| Linked sidecar setup | `00049760` | 1,055 | `68841952584F2F76BEE266E09C508E60EC82BA472BC5F3F0D5462A55D5720C1B` |
| Full-file buffer read | `0003D040` | 112 | `660D5346B8004FD9577CF1CA8049EB924CFD5730FC62B4DED3C99415131B4BF3` |

Microsoft dumpbin static disassembly was reviewed from complete instruction
boundaries through each return. Native formatter/file-manager callees and
diagnostic virtual methods are not fully reconstructed or executed.

## Reproduce and validation

`scripts/Get-Ra3Ep1LinkedSidecars.ps1` imports the previous descriptor evidence.
`Assert-LinkedSidecarCode` checks both complete body pins.
`Read-SidecarHeader` rejects relative paths, directories, short/oversized files
and reparse ancestors, then reads exactly eight bytes with read-only access.
`Assert-SidecarHeader` checks observed magic, checksum and exact declared size.
It repeats all twelve header/length/timestamp snapshots and rechecks the engine
SHA. Each invocation reads **192 sidecar bytes** over two passes; **zero payload
bytes**. Tests invoke the audit twice, so those two invocations total 384 header
bytes. Manifest metadata is read separately by the inherited bounded audits.

These checks are not full stream SHA/payload validation or an atomic filesystem
snapshot. A same-length payload edit leaving header/timestamp unchanged could
go undetected. Header validity alone does not prove asset layout, relocation
content, imports resolution, authoring identity or game acceptance.

```powershell
# Reborn: review linked setup and tiny stream headers, including the large WorldBuilder.bin, without payload dumps or native execution.
./scripts/Get-Ra3Ep1LinkedSidecars.ps1 -ImagePath 'D:\SteamLibrary\steamapps\common\Command and Conquer Red Alert 3 Uprising\Data\ra3ep1_1.0.game' -AsJson
# Reborn: test header/code pins, repeat snapshots and detached corruption rejection without changing compiler policy.
./scripts/Test-Ra3Ep1LinkedSidecars.ps1 -ImagePath 'D:\SteamLibrary\steamapps\common\Command and Conquer Red Alert 3 Uprising\Data\ra3ep1_1.0.game'
```

Validation covers twelve authentic headers/exact lengths, repeat JSON,
four detached header faults (magic/checksum/length/truncation), and eight
private code faults. The previous descriptor-producer regression also passes.
Only the already built repo-owned managed name-hash diagnostic is invoked;
no target game, compiler or native codec executes. No C# changes/full compiler
suite rerun are claimed; the prior executed count remains 165 groups.

## Next gate

The suspected entry-transform blocker is now resolved as sidecar setup. Next
trace downstream linked chunk reads to combine descriptor-relative positions
with the sidecar cursor/header offsets, and verify concrete source-object
bindings on the outer loader path. Native capacity reuse, diagnostic semantics,
ProcessingHash/header provenance and actual game loading remain independent
questions. Effort stays approximately **52% complete / 48% remaining**; this
adds stream-reader evidence, not a usable SDK release.
