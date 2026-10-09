# EP1 1.1 reader suffix and probe behavior

Static, read-only audit for image SHA-256
`B1DA83EFD229570BAB9DE3A14E48B2D1E6128E302BD52384858FA8A413AE330B`.
Extends [manifest queue](RA3EP11_MANIFEST_QUEUE.md); no game/codec execution.

Complete setter `004AA300` takes a suffix index and suffix text. It updates
reader+58h[index*4], then uses three slots in order. A nonempty reader slot
overrides the corresponding global slot at `00CF15E8`; empty reader slots
fall back to their global values. When reader+68h is zero it inserts selected
suffixes before the last dot of original reader+10h, preserving the extension,
and stores the effective path at reader+0Ch. The nonzero +68h branch copies the
original path instead. Complete non-ASCII/reference-string semantics remain open.

It removes an old tag-two cache entry, publishes the new effective path through
`006A34B0` with cache `00CC4AF8`, and stores this reader through the returned
payload slot. It derives reader+14h by truncating a copy at its last dot and
appending a backslash. This derived field is not the original config directory.
No suffix-index bounds or missing-dot safety is proven in the reviewed body.

The queue driver's argument one=1 is therefore suffix slot one, not a generic
boolean flag. `_L` and `_M` are observed strings; their full mode/language
interpretation is not assumed. Detached examples with other slots empty:

| Original | Slot-one suffix | Result |
| --- | --- | --- |
| static.manifest | _L | static_L.manifest |
| data/mod.manifest | _M | data/mod_M.manifest |

Probe adapter `004D7840` is just a 15-byte call to `004D6F10(path,null)`, returning
its boolean result. The previously reviewed source probe `004AA5B0` first
checks the effective path, can ask a provider for a positive version number,
format `_v%d` and set suffix slot two, then try again. Another path probes a
copied path with extension `.version`. These paths do not establish provider
version meaning, successful loading, version-file format or all fallbacks.

`Get-Ra3Ep11ReaderVariants.ps1` pins the whole setter, adapter and two literal
regions. `Test-Ra3Ep11ReaderVariants.ps1` requires five detached short-ASCII
filename fixtures, five diagnostic-policy rejections, sixteen private byte
fault rejections and repeat JSON. PowerShell 7 parameters are explicit 1.1
`-ImagePath` and 1.0 `-BaselineImagePath`. Fixture rejection rules are conservative
initial-PoC policy, not engine rejection claims. No native allocator or filename
routine is invoked. Production readiness and mod-package loading remain false.

Engineering estimate remains **52% / 48%**; the 165 compiler groups are not
rerun here. Next: bind package preflight to stock SKU/config paths and suffix
variants, and inspect BIG mounting before a game run can be meaningful.
