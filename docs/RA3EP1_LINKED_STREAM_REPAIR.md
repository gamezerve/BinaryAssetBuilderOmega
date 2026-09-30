# Intermediate asset commits and coordinated linked-stream repair

Date: October 1, 2026

## Outcome and scope

The real `BinaryAsset.Commit()` and `OutputManager.LinkStream()` paths now have
a small reproducible regression test. A partial linked generation no longer
loses relocation/import payloads during repair. Direct asset, linker and version
writers also enforce the experimental processor output policy before any write.

This is an infrastructure proof using synthetic payloads. It does **not** approve
the EP1 type table, enable the armor profile for production, demonstrate valid
native references, or establish that Uprising loads a mod. No game binaries were
read or modified during this test. Overall effort remains approximately 50%
complete / 50% remaining; model/marshaller coverage is unchanged.

## Defect found in the existing linker

`source/BinaryAssetBuilder.Core/BinaryAssetBuilder/Core/OutputManager.cs`,
`LinkStream()` previously considered BIN, RELO and IMP independently:

1. BIN's marker/checksum matched, so the asset concatenation loop was skipped.
2. That same loop was the only producer of the in-memory RELO and IMP payloads.
3. If an auxiliary file was absent or had the wrong marker/checksum, its rebuild
   wrote a new header followed by an empty buffer.

Thus a valid BIN plus an invalid auxiliary file could produce a header-only
auxiliary stream rather than restoring the committed asset bytes. A truncated
file with its header intact was also accepted as current. Read probes used
`OpenOrCreate`, unnecessarily creating missing output files while checking them.

## Implementation

`LinkStream()` first checks `PluginRegistry.ValidateProductionOutput()`, then
sums the local asset chunk sizes in output order, using checked 64-bit totals.
Entries inherited from `BasePatchStream` contribute no local payload.

`LinkedStreamFileMatches()` requires an existing file, the expected exact length,
the target-specific marker and the document checksum. Its read-only probe does
not create missing outputs. If **any** of the three files fails, all three are
regenerated from committed intermediate assets; if all pass, none is rewritten.

EP1 stream headers retain the previously validated eight-byte encoding:

| Stream | Marker | Payload source |
|---|---|---|
| BIN | `BABB0000` | Asset instance chunks in output order |
| RELO | `BABE0000` | Corresponding relocation chunks |
| IMP | `BAB10000` | Corresponding import chunks |

The legacy non-`VERSION7` branch retains four-byte checksum-only headers.
The active Release/x86 build tests the EP1 branch; the legacy branch has not
received a separate build/runtime validation in this block.

Additional guards in `BinaryAsset.Commit()` and `OutputManager.CreateVersionFile()`
prevent direct calls from bypassing experimental output restrictions. The new
test switches an already-created fixture manager's registry to the experimental
armor plugin and requires the exact policy exception from all three direct
write routes. There is no fabricated production-ready EP1 registry.

## Reproducible test

From the repository root after a Release/x86 build:

```powershell
# Reborn: create isolated synthetic commit/link fixtures; no game assets or production profile are used.
& .\source\BinaryAssetBuilder.ManifestInspector\bin\x86\Release\net8.0\BinaryAssetBuilder.ManifestInspector.exe linked-stream-self-test artifacts/link-regression
```

`LinkedStreamSmokeTest.Run()` uses the existing built-in null plugin as a
diagnostic context, arbitrary nonzero asset hashes and explicitly synthetic
payloads. It initializes focused document/patch metadata with reflection; it
does not run XML compilation or pretend the fixture is a game-compatible mod.
Each run owns a fresh GUID subdirectory and restores global settings afterward.

Checks:

- Two real intermediate `.asset` commits: header identity, `4/4/8` chunk lengths,
  exact concatenated payloads and release of the consumed memory buffers.
- Real linker concatenation into `16/16/24` total-byte BIN/RELO/IMP files,
  including their eight-byte headers and a fixed diagnostic checksum.
- An inherited base entry with `1000/100/80` metadata sizes has no local asset
  file and contributes zero bytes; this tests the linker's base-entry branch,
  not full patch-base manifest discovery (covered by the external-link test).
- A second intact link preserves deliberately fixed file timestamps.
- For **each** of BIN/RELO/IMP: missing file, header-only truncation, wrong marker,
  wrong checksum and one appended byte. All 15 cases restore every complete file
  to its exact expected bytes while the other two initially remain valid.
- After switching to the experimental armor plugin: direct link, version and
  intermediate asset commit calls fail specifically on policy; existing linked
  bytes remain unchanged and no version/temporary asset file appears.

The compiler regression runner includes this as its 52nd registered test group.
Release/x86 rebuild, the compiler/layout runners, the armor profile/document
tests and the manifest writer round trip pass.

## Remaining limits and next work

- Reuse validates header and length, **not a cryptographic payload digest**.
  Same-length payload corruption with an intact marker/checksum can still pass.
  The document checksum is an upstream identity checksum, not a checksum of the
  linked bytes. Its derivation and cache equivalence need separate investigation.
- The three destination replacements remain sequential, not a transactional
  all-or-nothing publication. Interrupted/failed writes remain a separate risk.
- Auxiliary concatenation still buffers entire generated RELO/IMP streams in
  memory; large-build streaming/performance has not been improved here.
- Golden-tested tokenized armor bytes are not yet passed through an approved
  production document-to-manifest build. The explicit profile gate remains shut.
- Next: investigate checksum input length versus backing-buffer capacity and
  tokenized asset cache equivalence, then expand production pipeline proof once
  type-table/profile eligibility is established.
