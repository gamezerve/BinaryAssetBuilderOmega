# EP1 1.1 scoped BIG mount metadata

## Scope and identity

This read-only audit connects the previously recovered `add-big` dispatch
mode (2) to native mounting and validates stock directory headers. It does not
execute the game, extract archive payloads or implement a replacement mount.
The 1.1 image SHA-256 is
`B1DA83EFD229570BAB9DE3A14E48B2D1E6128E302BD52384858FA8A413AE330B`.
The baseline and configured stock SKU retain the prerequisite identity guards.

Implementation: `scripts/Get-Ra3Ep11BigMountMetadata.ps1`.
Repeat/refusal regression: `scripts/Test-Ra3Ep11BigMountMetadata.ps1`.

## Independently pinned native bodies

| VA | Raw offset | Bytes | Role |
| --- | --- | ---: | --- |
| 00995160 | 595160 | 704 | Mount/open, directory read, node creation and list append |
| 009950F0 | 5950F0 | 101 | Archive-node constructor |
| 00996060 | 596060 | 119 | Header classifier |
| 009961A0 | 5961A0 | 79 | Declared directory-size helper |

All four complete reviewed bodies have independent SHA-256 pins in the audit.
Changed code is rejected, not interpreted as another compatible game version.

## Recovered mount sequence

`00995160` prepares the input path and opens an IO wrapper. Mode bit value 1
controls an alternate-name attempt with fallback to the original path; the
alternate-name helper's complete semantics remain unreviewed. A missing handle
returns failure. It requests 16 header bytes, computes the declared directory
size, allocates directory storage and reads the remaining directory bytes.
It then allocates a 104-byte archive node and invokes `009950F0`.

The constructor stores the directory pointer at node+08h, initializes the
index member at +0Ch and records the header class at +30h. Mode bit value 2
calls index helper `00996880` with that member and the manager allocator.
The configured `add-big` mode therefore selects this helper, not the
alternate-name bit. Full helper internals have not been recovered.

The node is appended to the manager's linked list: first at +10h, tail at
+14h, count at +18h; node+00h is the next link. This establishes scoped append
behavior, not the archive winner rule when multiple archives contain a path.
Unmount, full index lookup and search precedence remain open.

## Header arithmetic, not format admission

| Native class | Signature | Declared directory bytes |
| ---: | --- | --- |
| 0 | First three bytes `BIG`; fourth byte ignored | Big-endian uint32 at +12 |
| 1 | Bytes `C0 FB` | Big-endian uint16 at +2, plus 4 |
| 2 | Exact `Viv4` | Big-endian uint32 at +12 |
| 3 | Otherwise | 0 |

The diagnostic's existing archive reader still accepts only `BIG4` or `BIGF`.
Native acceptance of `BIGX` in the detached classifier fixture is not grounds
to broaden that reader. `C0 FB` here is an archive-header classification, not
the `10 FB`/`90 FB` RefPack payload marker. Alternate formats are arithmetic
fixtures only, not validated stock-format implementations.

## Stock evidence and tests

Thirteen available configured archives are `BIG4`, class 0; each declared
directory size matches the separately validated directory snapshot and hash.
One invocation reads 4,312,132 archive-directory metadata bytes including
prerequisites and **zero archive payload bytes**. In particular, the large
WorldBuilder BIN is not dumped or decompressed by this audit.

The test runs six detached header fixtures, rejects a 15-byte truncated
header, rejects twelve private image-byte faults and compares every stable
nested field against a second JSON run. The image is rehashed after review.
The missing configured `Data\MapsCampaign.big` remains a separate stock-profile
failure; `MapsCampaign (Disabled).big` is not substituted or renamed.

## Safety boundaries and migration implications

The reviewed mount body does not check the return values of its initial and
remaining-directory reads. Allocation failure safety is not proven; the node
allocation's null path reaches later node-dependent operations. These are
observations about this pinned body, not claims about every engine allocator
or IO implementation. The detached diagnostic requires bounded input and
does not reproduce unsafe native allocation/read behavior.

XML/XSD replacement alone still cannot establish a working Uprising SDK.
The stock manifest bridge and sidecar-header audit prove selected existing
package identities, while this audit identifies their mount route. Authentic
EP1 ProcessingHash generation, compiler dependencies/layouts, complete path
lookup/precedence and an authored package accepted by the game remain open.
No zero-hash workaround or production guard relaxation was introduced.

Effort remains approximately **52% complete / 48% remaining**. This milestone
adds scoped evidence, not a working-mod percentage or a game-load result.
The 165 compiler test groups were not rerun for this PowerShell-only milestone.
