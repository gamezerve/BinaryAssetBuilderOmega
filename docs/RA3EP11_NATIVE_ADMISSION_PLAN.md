# Reborn: pinned native 1.1 admission ranges

Date: October 10, 2026 (Europe/Istanbul). Static/detached checks only.

## Native load policy

The reviewed `Data\ra3ep1_1.1.game` is an unmanaged I386 PE32 executable:

- SHA-256 `B1DA83EFD229570BAB9DE3A14E48B2D1E6128E302BD52384858FA8A413AE330B`.
- Preferred image base `00400000`; SizeOfImage 13,754,368 bytes.
- COFF `RelocsStripped` is set; relocation directory RVA and size are both zero.
- DLL characteristics are zero, including absence of `DYNAMIC_BASE`.
- IAT occupies RVA `007D8000` through exclusive end `007D8828` (2,088 bytes).

The PE specification describes the base relocation directory and address-delta
fixups in [Microsoft's PE format reference](https://learn.microsoft.com/en-us/windows/win32/debug/pe-format#the-reloc-section-image-only).
For this exact image the implementation **requires live base `00400000`**.
It refuses other bases rather than synthesizing relocation records or applying
the managed helper's stage-specific raw CLR thunk policy. This does not prove
where a running copy actually loads or whether a runtime patch/protection layer
changes its contents. A future supervisor must inspect the actual module.

## Implemented interfaces

`Get-Ra3Ep11NativeAdmissionPlan.ps1` accepts an explicit image path and optional
`-SelfTest` / `-AsJson`. It reuses the complete SHA admission, config/read-route
pins and disk-backend import audit, then rechecks the source SHA after analysis.
There is no Run, debugger, process query, attach or memory-writing route.

`Ra3Ep11NativeAdmissionPolicy.ps1` provides:

- `Assert-Ep11NativeLoadPolicy`: exact native profile and preferred-base policy.
- `Get-Ep11NativeRawRange`: bounded unique section mapping; excludes writable
  sections, raw padding/unbacked ranges and the loader-mutated IAT.
- `Assert-Ep11NativeAdmissionObservations`: every trusted in-process plan range
  must have one same-address successful read, full actual count, exact byte-array
  length and matching SHA. Missing, duplicate or unknown observations refuse.

The plan has **23 ranges / 3,906 bytes**. Each range is at most 557 bytes;
the mapper's hard maximum is 4,096. It compares code and immutable table bytes,
not the whole loaded image, mutable config buffers or resolved IAT pointers.
Ranges contain addresses, lengths and expected hashes, not a binary dump.

Selected bindings (the plan includes the complete reviewed bodies):

| Preferred VA | Purpose | Bytes |
| --- | --- | --- |
| `004D86B0` | Config reader | 169 |
| `004D9040` | Line splitter | 287 |
| `004D985E` | Top-level config gate | 77 |
| `004D7170` | Memory-reader population | 137 |
| `004D5BB0` | Source reader | 557 |
| `0096C1D0` | Windows disk opener | 338 |
| `0096BBF0` | Windows disk reader / ReadFile return | 46 |
| `00BFA208` | Memory-reader vtable | 96 |
| `00C7F200` | Concrete disk-provider vtable | 64 |
| `00BFA27C`, `00BFA2A8` | Source read/conversion slots | 4 each |

RVA/raw mapping is derived through section headers even where values happen
to be equal in this particular image. Read-only section flags are admission
constraints, not proof that a runtime actor cannot change page protection or
patch code. Any observed byte mismatch must refuse admission, not be masked.
Verify these bytes **before installing software breakpoints**; trap bytes must
not be mistaken for original game code in subsequent comparisons.

## Detached validation

The installed pinned image passes 23 disk-derived observation matches and
26 refusal cases:

- Ten observation faults: altered bytes, count, status, address, duplicate,
  missing, unknown name, nonpreferred base, buffer length and nonboolean status.
- Five range faults: IAT, writable `.data`, zero RVA, oversized and zero length.
- Three live-base faults: zero, shifted and unaligned base.
- Eight independent metadata faults: relocation RVA/size, DLL flags, managed
  image, missing stripped flag, PE32+, other machine and image size.

These fixtures never read or alter a process. They must not set
`OwnershipValidated`, `LiveBytesValidated`, `TargetExecuted` or
`GameRecipeReady` true. The observation comparison consumes a trusted plan
produced in the same admitted code path; it does not authenticate a supplied
JSON plan or independently bind supplied bytes to an OS process.

Two repeated `-AsJson` runs produced identical plans. The installed native
1.0 image was rejected by the inherited exact 1.1 SHA admission, with no target
execution. No comparison bound or version guard was relaxed.

## Remaining runtime integration

Combine the helper-calibrated initial-break supervisor with this native plan:
verify exact process ownership and actual module base, inspect every range
through the retained owned process handle, then install narrow config-reader
observations. Do not compare IAT slots against disk thunk RVAs: verify their
runtime provider addresses separately if needed. Current slot/table pinning
does not prove which provider the actual config read selects.

This plan does not launch Uprising or authorize a trial. A direct-native
debugger route still needs explicit authorization, disclosure of how it differs
from the verified launcher baseline and bounded cleanup that may resume the
game. Actual config read, parser consumption and authored mod loading remain
unproved. Overall effort remains approximately **52% complete / 48% remaining**;
the 165 compiler groups were not rerun for this standalone evidence change.
