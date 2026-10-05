# Supervised variable AudioFile packages

October 5, 2026. The explicit 1–8-source pool now has a leaf-only linked package
publication path. It remains diagnostic, not a production AudioFile plugin or
playable Uprising SDK. Weighted project effort remains approximately 50% complete
/ 50% remaining; this is not a claim that half of game assets load successfully.

## Contract and implementation

`supervised-audio-pool-package <absolute-audited-audio.dll> <source-directory>`
requires the existing bounded `audio-pool.json` inventory and narrow direct XML/WAV
profile from [pool preflight](RA3EP1_AUTHORED_AUDIO_POOL.md). It uses explicit protocol
v2 mode `encode-pool-package`. The raw/preflight modes do not silently gain output.
The fixed two-source AudioFile/AudioEvent path remains separately admitted.

Repository-relative implementation paths:

- `source/BinaryAssetBuilder.ManifestInspector/AudioFilePackageProbe.cs`:
  Entry accepts an explicit optional streamed flag for variable records, validates
  the shared bounded source-leaf contract, and freezes runtime/custom buffers.
  Fixed records retain source-slot inference only when the flag is omitted.
  Serialize/Verify/Publish require explicit `variable:true` to admit 1–8 ordered
  unique variable leaves. Case-insensitive source aliases and SAGE ID collisions
  reject. Variable/fixed records cannot be mixed; a variable package cannot contain
  the fixed AudioEvent parent. Fixed defaults preserve their previous output bytes.
- `AuthoredAudioPool.ValidateSource` is shared with Entry so package source names
  retain exactly the same leaf/length/device restrictions as inventory admission.
- `AudioPoolResultGate.Verify` reconstructs each current core preparation, compares
  exact runtime/relocations/frozen PCM, and reconstructs variable immutable records
  using actual source names and source-derived play location. Package mode regenerates
  every expected byte from these records, checks both readers, and verifies exact
  output membership. Caller originals and owned source copies are rechecked.
- `AudioPoolResultGate.Publish` revalidates raw/core evidence before staging. It uses
  the no-overwrite publisher, verifies staged bytes and renames the owned directory;
  it then checks the complete raw/package evidence. Failed staging is retained.
- `AudioEncoderPoc.RunPool` dispatches publication only after native shutdown and
  core metadata completion. `AudioEncoderSupervisor.Run/Worker/ValidateResult` bind
  explicit mode, frozen input/hash inventory, exit/nonce and parent reconstruction
  before writing ACCEPTED.json. Source changes can revoke acceptance even after
  owned diagnostic staging; caller sources and game files are never written.
- `AudioPoolPackageSmokeTest.Run` registers one new default managed group.
  `AudioPoolWorkerSmokeTest.NativeProof(library, packaged:true)` supplies the explicit
  native integration and fresh forged-result rejection jobs.

Output under the owned worker directory consists of the existing frozen/raw/core
evidence and `package/diagnostic.manifest`, `.bin`, `.relo`, `.imp`, the diagnostic
notice and `package/diagnostic/cdata/<type.typehash.id.contenthash>.cdata`.
Package source rows use actual inventory filenames, not ram.xml/streamed.xml aliases.
Play location is independent of the filename. Manifest row order follows inventory
order, including reversed/nonlexical order.

## Stream and identity checks

The diagnostic v7 linked writer uses the existing EP1 header/prefix conventions:
BIN/BABB0000, RELO/BABE0000 and IMP/BAB10000 with eight-byte checksum-bearing stream
prefixes. AudioFile type ID/type hash remain 166B084D/53C81E47; AllTypesHash 5454A8E9
belongs to the diagnostic schema domain, not a recovered production type catalog.
Payload-derived InstanceHash remains distinct from core InstanceHash and EA cache
parity. Custom tuple paths must match the exact captured payload-derived identity.

Both manifest readers check row names/IDs/hashes/source names, flags, empty references,
sizes and linked offsets. BIN/RELO lengths are 8 plus the sum of approved chunks;
IMP is exactly 8 because these direct AudioFile leaves have no imports. No external
manifest or event reference is introduced. Expected package bytes are regenerated
independently by the parent, and extra/missing raw/package artifacts reject.

Only this new package mode permits at most 80 owned output files: the maximum
eight-distinct/all-streamed fixture produces 71 files (17 frozen inputs, 40 raw
artifacts, one metadata file and 13 package artifacts). All older modes keep 64.
Directory/depth, per-file/aggregate size, JSON/log and timeout limits are unchanged.
Inventory rejects arbitrary file-limit expansion beyond the two supported limits.

## Executed evidence

Final Release/x86 build passed with zero warnings/errors. All 104 default compiler
groups passed, with no native codec execution in the default suite. All 33 enum
checks passed. Structural counts remain 785/1,390 models and 762/1,390 marshallers.

Managed synthetic tag-04 fixtures pass 1/3/8 reordered variable leaves with both
readers, independently expected stream lengths and immutable buffer ownership.
Zero/nine leaves, repeated/case-alias source names, colliding case-alias SAGE names,
mixed fixed admission, reserved devices and contradictory play location reject.
Changed manifest/streams/custom bytes, missing payloads and wrong expected order
reject; exact restoration passes. Existing output directories are not overwritten.
Synthetic compressed bodies establish framing/packaging, not codec correctness.

Seven final native cases passed, encoding 37 leaves. Job suffixes identify owned
directories under `C:\Users\drknt\AppData\Local\Temp\Reborn-SupervisedAudio-<suffix>`.

| Sources | Shape/order | Accepted job suffix |
| --- | --- | --- |
| 1 | distinct RAM | fd9fd4369adb4dc0a5ef15317a8bbe2e |
| 1 | distinct streamed | b7273daee20a4b35aeec8976e0fed5c0 |
| 3 | distinct mixed, tone2/tone1/tone0 | 232673266efa4528a22dd54d8feef65c |
| 8 | shared mixed | 824587e62a4447e0a700cc16ca645fc7 |
| 8 | distinct mixed | 01b2296461d743ceaa7ad03075a554a5 |
| 8 | shared all streamed | dad0b37d00ee485998cea98de0000fbc |
| 8 | distinct all streamed | 5420f284bc7249268443dadced8ea5e9 |

The last job has 71 inventoried files. Its package manifest/BIN/RELO/IMP lengths are
692/712/104/8 bytes. Parent reconstruction checked eight variable rows and actual
custom payloads through both readers before acceptance.

Each native case rejects altered raw runtime/PCM/SNR/core metadata after encoding.
Fresh forged jobs copy bounded owned evidence, modify raw runtime or each linked
package file/custom payload, and recompute the complete reported hash inventory.
Full parent ValidateResult still rejects semantic/package mismatches; forged jobs
receive no acceptance marker. These are post-encoding evidence checks, not unsafe
native API fault injection or mutation of caller/game inputs.

The fixed supervised core encoder also passed in job suffix
ca5d5f935f174ae0ace12abbc89acdf8. Its native artifact tamper and matching-inventory
corrupt-manifest tests rejected both jobs as expected. Default worker tests still
cover transport/timeout/log/JSON/nonce/path failures and stale originals/copies.

## Remaining work

Next resolve selected AudioEvent Sound references against the immutable variable
pool, then derive dynamic reference/import tables and selectors from selected
targets rather than fixed source slots. Verify graph closure, dependency fingerprints,
order, collision/missing-target rejection and parent event recompilation before
mixed publication. This leaf-only package path deliberately rejects an event today.

General durations/rates/channels/codecs, music graphs, production processor/hash/cache
parity, remaining native layouts, WorldBuilder integration and Uprising game loading
remain open. Framing checks do not decode compressed samples back to PCM. Native
process isolation and no-overwrite staging are not a security sandbox or atomic
concurrent-filesystem transaction. XML/XSD substitution alone remains insufficient.
