# EP1 executable music/type provenance — October 9, 2026

## Outcome

Bounded static inspection of the actual installed engine image found useful
reverse-engineering targets: music authored/runtime type names, one literal
stock PathMusicEvent TypeHash, six literal stock event TypeId matches, and
preferred-address pointer candidates for five named music types. An actual
CodeView record identifies the EP1 runtime build. No missing authoring header,
authentic compiler ProcessingHash or production build compatibility was recovered.
The target was not executed or loaded as a library; no registry/game assets changed.

`scripts/Get-Ra3Ep1PeEvidence.ps1` now reports SHA-256, PE architecture/sections,
DLL imports, bounded named-export evidence, CodeView GUID/age/path, ASCII/UTF16LE
printable-window offsets, fixed stock-word matches and derived string-address
matches. Explicit absolute images are capped at 16 MiB with reparse ancestors
rejected. This intentionally refuses a multi-gigabyte `worldbuilder.bin`; that
raw asset stream still needs manifest-directed bounded slices, not PE analysis.

## Exact images

Engine:
`D:\SteamLibrary\steamapps\common\Command and Conquer Red Alert 3 Uprising\Data\ra3ep1_1.0.game`

- Length: 9,484,336 bytes.
- SHA-256: `ABA6A8825A744A78487837D78AD5992962257CE68404F639D600F11DEEA2580B`.
- Native I386 / PE32; preferred image base `00400000`, entry RVA `000D9DA3`.
- Sections: `.text`, `.rdata`, `.data`, `.tls`, `rts.ver`, `.rsrc`.
- 498 named exports; zero matches for the targeted music/asset/schema/type-hash
  export-name filter. The first sixteen sampled names are Apt/EAString C++
  runtime symbols, not evidence of exported compiler entry points.
- Eighteen imported DLL names: `MSVCR80.dll`, `KERNEL32.dll`, `SHELL32.dll`,
  `SHLWAPI.dll`, `GDI32.dll`, `USER32.dll`, `ADVAPI32.dll`, `d3d9.dll`,
  `d3dx9_35.dll`, `DSOUND.dll`, `XINPUT1_3.dll`, `MSVCP80.dll`, `IMM32.dll`,
  `WS2_32.dll`, `NETAPI32.dll`, `USP10.dll`, `WINMM.dll`, `ole32.dll`.
- CodeView: `E:\Projects\Ra3_ep1\Production\Run\RTS-final.pdb`,
  GUID `c1324c28-e57a-434d-9edc-b20386836efe`, age 1. This is embedded debug
  provenance, **not an available PDB or an external path to execute**.

Launcher:
`D:\SteamLibrary\steamapps\common\Command and Conquer Red Alert 3 Uprising\RA3EP1.exe`

- Length: 1,565,448 bytes; native I386 / PE32; one named export.
- SHA-256: `05947CF2B1BBA51CEE55A6FF746D15B3898BF86611607648B5510E2240CDA887`.
- CodeView: `C:\dev\depot\cnc\RA3_EP1\Production\Code\SageLauncher\0.0-dev\source\release\CNC3.pdb`,
  GUID `63bfde31-3536-4317-8b77-de4608310443`, age 1.
- Its only targeted printable-window match is that PDB path. The engine image,
  not this launcher, is the useful current music/runtime investigation target.

## Bounded reverse-engineering targets

All offsets below are **raw file offsets**, not process addresses. Preferred
string VAs are calculated from PE section mappings and the image base; runtime
relocation and actual instruction/table use were not established.

| Type name | ASCII offset | Preferred VA | Literal VA match offset |
| --- | --- | --- | --- |
| PathMusicEvent | `007DE4D0` | `00BDE4D0` | `008BC4D4` |
| PathMusicEventRuntime | `007DE4B8` | `00BDE4B8` | `008BC4D8` |
| PathMusicTrack | `007DE4E0` | `00BDE4E0` | `008BC4D0` |
| PathMusicMap | `007DE4F0` | `00BDE4F0` | `008BC4CC` |
| PathMusicMapRuntime | `007DFE60` | `00BDFE60` | `008BC0AC` |

The four nearby matches in `.data` are promising type-name-array candidates.
This is an inference from adjacency and preferred VA matches, **not a proved
registry layout, index relationship or mapping to the hash block**.

Stock event TypeId `9A651D89` appears at six byte-match offsets in `.text`:
`000A617D`, `000C4891`, `0014C1BD`, `0014C24C`, `0014C267`, `00567F80`.
The script preserves only 40-byte contexts around each match; it does not
disassemble, identify functions, or call these matches verified instructions.

Stock event TypeHash `599CDAF2` appears once at `007E0504` in `.rdata`.
The 40-byte context from `007E04F4` is:

```text
642FDEBB60C16FD9CB4C1BBA33CF129FF2DA9C593271AC3A24440962A74DE2E2379D1D518E72F082
```

This supplies a narrow hash-block investigation target, not a recovered
ProcessingHash. No literal little-endian `5454A8E9` AllTypesHash bytes were
found in this engine image. That does not rule out a calculated, transformed
or externally supplied catalog identity.

Of 31 targeted printable windows in the engine, relevant examples are
`SagePathMusicManager` at `007FEA04`, `PathMusicDynamicStateSystem` at `0087C6A0`,
its relative C++ source path at `0087C708`, and `%.*s%d.mus` at `008AA714`.
The `.mus` format string is runtime naming evidence, not discovery of the
missing header or an authored track compiler. `PathfinderFootprintAngle`,
`PathfinderPath`, and the navigation pathfinder C++ filename are not by themselves
music evidence; the filter intentionally records potentially unrelated names.

At `007F205E`, an error message associates XSD drift with BinaryAssetBuilder
and GameEngine and advises aligning the working build's Perforce changelist.
This supports treating schema/compiler/engine provenance as a compatibility
concern, but does not tell us the relevant EP1 compiler version or hash algorithm.

## Reproduction and checks

```powershell
# Reborn: inspect only explicitly selected small PE images, never execute either one.
./scripts/Get-Ra3Ep1PeEvidence.ps1 -ImagePaths @('D:\SteamLibrary\steamapps\common\Command and Conquer Red Alert 3 Uprising\Data\ra3ep1_1.0.game', 'D:\SteamLibrary\steamapps\common\Command and Conquer Red Alert 3 Uprising\RA3EP1.exe') -AsJson
# Reborn: validate bounded static metadata, encoding offsets and rejection using owned fixtures.
./scripts/Test-Ra3Ep1PeEvidence.ps1
```

PE inspection uses managed `PEReader` over immutable snapshots, never
`Assembly.Load`, `LoadLibrary` or target execution. Every image is read twice
and SHA-256 compared. Import descriptors are capped at 128, named exports at
4,096, function slots at 16,384, names at 1,024 bytes, targeted strings at 512,
and literal matches at 128 per needle. ASCII and UTF16LE searches use printable
windows up to 1,024 characters; they are not complete arbitrary-encoding or
arbitrarily long string recovery. Imports report DLL names, not imported
function signatures, delay imports or ABI compatibility. Pointer matches are
literal preferred-address candidates only. File snapshots are not atomic
adversarial filesystem transactions.

Owned-copy tests pass: existing x86 fixture import/export metadata, appended
ASCII and UTF16 exact offsets, repeat/JSON hashes, one extra exact literal match
without semantic admission, malformed MZ/PE/truncated/e_lfanew rejection, and
absolute-path/oversize guards. The prior archive inventory script regressions
also pass. Tiny copies and one sparse over-limit fixture remain in the unique
temporary directory printed by the tests. No C# compiler implementation changed;
the previously executed 165-group result is retained, not claimed as rerun.

## Next gate and limits

Follow the `.data` name-pointer candidates and `.rdata` hash block with bounded
cross-reference/disassembly evidence to establish an actual table relationship.
Audit callers of the six event-TypeId byte matches before attributing them to
runtime asset loading. Any recovered runtime registry must be matched against
multiple independently observed manifest types, not only one music constant.
Runtime type identity still cannot supply a missing authoring ProcessingHash or
the authentic `RA3EPMus.h`; preserve the production/cache/AllTypesHash guards.

Overall estimate remains **52% complete / 48% remaining**. We now have concrete
runtime investigation addresses, but no new production/game-loading gate passed.
