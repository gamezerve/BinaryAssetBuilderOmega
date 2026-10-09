# EP1 1.1 fresh archive sidecar headers

Read-only follow-up to the [four-manifest archive bridge](RA3EP11_CORE_MANIFEST_ARCHIVE_BRIDGE.md).
Twelve uniquely named BIN/RELO/IMP archive entries match their fresh manifest's
checksum and projected chunk total plus the physical eight-byte sidecar header.
No whole instance stream is read, extracted, hashed or decompressed.

| Stream | BIN logical bytes | RELO logical bytes | IMP logical bytes | Checksum |
| --- | ---: | ---: | ---: | --- |
| global | 2,768,548 | 239,408 | 221,636 | 85FE7DA0 |
| static | 357,478,164 | 6,916,336 | 124,536 | EA62EA71 |
| worldbuilder | 1,394,571,528 | 21,616,304 | 113,732 | F2AC0280 |
| audio | 940,776 | 108,736 | 8 | 395CE3A2 |

Global/static/audio BIN and RELO entries are RefPack-compressed in the archive;
all four IMP files and WorldBuilder BIN/RELO are raw. A stored BIG entry length
must therefore not be compared directly with a manifest logical chunk total.
For example static.bin is stored as 163,571,168 bytes but declares 357,478,164
logical bytes in its RefPack header. Full compressed-body integrity is not
established by this prefix-only check.

## Narrow prefix policy

`Get-Ra3Ep11ArchiveSidecarHeaders.ps1` opens each of four explicit archives
read-only, holds its shared-read handle across two directory snapshots and
two passes over three selected sidecars, and retains exact index SHA identity.
Duplicate/missing selected names are rejected; unrelated map duplicates are
not selected. Raw entries read only eight bytes per pass.

Compressed entries read eight initial bytes, then reread sixteen bytes to
observe the complete RefPack size header and first eight literal output bytes.
The observed subset has no compressed-size field and begins with a literal
command supplying at least eight logical bytes. Other forms fail closed rather
than guessing or decoding a whole 357 MB stream. No large output buffer is
allocated. The declared logical size is cross-checked against manifest totals,
but remains a header claim rather than complete decompression validation.

One invocation reads **384 sidecar prefix bytes**, including rereads, and
observes 96 distinct logical header bytes. Of worldbuilder.bin's 1,394,571,528
bytes, only the first eight are read twice. Its contents are not PE-disassembled,
dumped or treated as an executable. The core manifest bridge separately reads
6,919,826 selected manifest bytes; total archive-directory reads are 6,738,399
metadata bytes per invocation. These counts deliberately separate manifests,
sidecar prefixes and whole instance-stream work.

`Test-Ra3Ep11ArchiveSidecarHeaders.ps1`, with explicit 1.1 image, 1.0 baseline
and 1.1 SKU under PowerShell 7, requires twelve matches, six raw/six compressed
entries, three positive detached prefix fixtures, three malformed rejections
and complete nested repeat JSON. Two invocations read 768 sidecar prefix bytes,
not a full binary dump. The bridge now exposes fresh manifest checksum words.

Complete sidecar contents, authentic EP1 ProcessingHash, full stock archive
completeness (campaign filename is still missing) and actual game loading
remain open. Engineering estimate remains **52% / 48%**; the 165 compiler
groups are not rerun. Next: native BIG header/mount metadata behavior and an
explicitly isolated package validation profile; no production guard relaxation.
