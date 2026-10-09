# EP1 1.1 loader argument provenance — October 9, 2026

## Result: keep the eighth and ninth arguments distinct

The added **ninth** argument in the reviewed 1.1 wrapper comes from that
wrapper's **sixth** argument. Its ultimate caller-side source and downstream
consumer remain unresolved. The cache-related **eighth** flag is not new:
the corresponding 1.0 wrapper already passes 1 in its first branch and 0 in
its second. The 1.1 wrapper preserves those eighth-argument values.

This refines the [nine-argument lifecycle finding](RA3EP11_LOADER_LIFECYCLE.md),
without assigning a feature name or authoring meaning to the added argument.
The ninth argument must not be called the cache flag, a TypeHash bypass,
modconfig option or patch mode from this evidence. No game/compiler/codec ran.

## Wrapper and consumer stack provenance

Both image identities remain pinned independently:

| Image | SHA-256 |
| --- | --- |
| 1.1 | `B1DA83EFD229570BAB9DE3A14E48B2D1E6128E302BD52384858FA8A413AE330B` |
| 1.0 baseline | `ABA6A8825A744A78487837D78AD5992962257CE68404F639D600F11DEEA2580B` |

Files are in `D:\Program Files (x86)\Steam\steamapps\common\Command and Conquer Red Alert 3 Uprising\Data`.
Addresses below are preferred image VAs, not live ASLR addresses.

Wrapper `004CFEB0` has 60h local bytes and one saved register. At
`004CFFA7`, before temporary argument pushes, `[esp+7Ch]` is entry ESP+18h,
so wrapper argument six. `004CFFAB` pushes this as the ninth loader argument.
The next push at `004CFFB0` is literal 1, the eighth argument. The alternate
branch loads the same wrapper argument six at `004CFFE3`, pushes it at
`004CFFE7`, then pushes zero EAX at `004CFFEC` as argument eight. The zero
comes from earlier initialization and is preserved along that branch.

In the separately disassembled 1.0 wrapper, `004CFCD5` pushes literal 1 as
argument eight, and `004CFD13` pushes zero as argument eight. Its cleanup is
20h/32 bytes at `004CFD4F`; 1.1 cleanup is 24h/36 at `004D000F`. The lower
seven positions, including context argument seven, are preserved in the
reviewed wrapper calls.

For the first and second 1.1 loader branches, 90h locals plus four saved
registers mean `[esp+C0h]` with no temporary pushes is **argument eight**:
`C0h - 90h - 10h = 20h`. Checks at `004CEC20` and `004CF360` branch around
cache-related entry handling when the low byte is zero. Nonzero proceeds to
an entry TypeId-based helper `004173A0` and additional cache logic. The full
helper/cache semantics and every branch are not reconstructed here. The same
first-branch low-byte check occurs in baseline 1.0 at `004CE960`.

At `004CEF6C`, after one temporary push, `[esp+C4h]` is **still argument
eight**, not nine: `C4h - 90h - 10h - 4 = 20h`. Its value is pushed at
`004CEF73` as argument seven of the eight-argument gate `004AB880`. This is
forwarding evidence, not recovered full gate behavior for that parameter.
The TypeHash comparison uses entry+8 versus metadata+8 independently of this
argument in the already reviewed comparison prefix. No hash guard is loosened.

An unadjusted C4h load would instead be argument nine. Accounting for pushes
is essential: merely finding C4h in disassembly is not evidence that the new
ninth argument is consumed. **Absence of a proven consumer here does not mean
the ninth argument is globally unused.**

## Evidence pins and tests

| Version / raw offset | Bytes | SHA-256 |
| --- | ---: | --- |
| 1.1 CFF5C wrapper | 205 | `2AA453BC67F93DD4A28BAD0FE41542AE1DF3DE16687F954A037EE2AF852592E0` |
| 1.1 CEC20 first flag branch | 43 | `CEEB4844F7BAD0A2EBFA4C0146CA28D64B1C7CC1F3C5E5BA57F6841B93752F8A` |
| 1.1 CF360 second flag branch | 43 | `33A7A323D23641703A70463300CC547B06DDAE2BD2283786C603D62B0EFB2367` |
| 1.1 CEF5C gate pushes | 59 | `BADF2F8B1D1C28C861AEAC09B6B29BB6A4FF8C8BA04783FC767322A1CA3C9435` |
| 1.0 CFC9C wrapper | 205 | `9403EC39651F48BC1A77C7E0C1265024E6095DD2D1651381AEFF4DB972007556` |
| 1.0 CE960 preserved flag check | 14 | `9B1C3D6EA52B79BA29B36C92036E2CC6240D25A126203BE6F4ADDFA8200922F9` |

`Get-Ra3Ep11LoaderArguments.ps1` imports the strict 1.1 lifecycle evidence,
pins these six slices across both images and rechecks both complete image
hashes at completion. The stack model explicitly accounts for locals, saved
registers and temporary pushes. Self-test performs six positive stack checks,
four invalid-stack refusals and ten detached byte faults; a plain invocation
performs the four contract checks without self-test fixtures. Repeat JSON
compares every stable report field while excluding fixture counters.
Shipping images, synced sources and reference schemas are never modified.

```powershell
# Reborn: distinguish versioned loader argument provenance without native execution or invented ninth-argument semantics.
./scripts/Test-Ra3Ep11LoaderArguments.ps1 -ImagePath 'D:\Program Files (x86)\Steam\steamapps\common\Command and Conquer Red Alert 3 Uprising\Data\ra3ep1_1.1.game' -BaselineImagePath 'D:\Program Files (x86)\Steam\steamapps\common\Command and Conquer Red Alert 3 Uprising\Data\ra3ep1_1.0.game'
```

Next rebase concrete factories/source ownership and trace caller-side wrapper
argument six. Full ninth-argument consumer semantics, all source bindings,
authentic authoring ProcessingHash, native emission ABI and game loading remain
open. The 165-group compiler suite was not rerun. Weighted engineering effort
remains **52% complete / 48% remaining**; this is not a usable SDK release.
