# Local/cache asset copy recovery

Date: October 1, 2026

## Outcome

`BinaryAsset.CopyAsset()` no longer deletes existing asset/custom-data output
merely because a source candidate is invalid, missing or locked. Both candidates
are staged before publication. An ordinary failure publishing custom data rolls
the asset destination back; a blocked rollback retains its backup and logs the
recovery path. This is not a crash-atomic two-file transaction.

Release/x86 regression fixtures operate only in fresh GUID directories under
the local temporary directory. No real cache, game file or WorldBuilder binary
was modified or read. EP1 production output remains gated, and the overall
engineering-effort estimate remains approximately 50% complete / 50% remaining.

## Problems corrected

Previously, `CopyAsset()` cleared its header metadata, copied into a shared
`.tmp` name, deleted the existing asset and moved the new asset into place.
Custom data was copied afterward. Any exception caused the catch path to delete
both final destinations, including valid output belonging to an earlier build.

Additional defects discovered in the same path:

- Reads smaller than 16 bytes were rejected, including valid final chunks after
  the one-MiB transfer buffer was filled.
- Header loading depended on a generic wrapper that permits partial reads.
- Candidate chunk sizes were not validated before publication.
- `AssetHeader.IsValidFileLength()` summed signed sizes with 32-bit arithmetic,
  permitting overflow and not explicitly rejecting negative lengths.

## Current sequence and code locations

`source/BinaryAssetBuilder.Core/BinaryAssetBuilder/Core/BinaryAsset.cs`:

1. `CopyAsset()` holds the existing shared copy-buffer lock. It opens the source,
   requires all 32 header bytes, checks the four identity/hash fields and validates
   the complete candidate length before publication. Current output metadata is
   not replaced on a rejected candidate.
2. Copy the entire asset into a unique same-directory staging file, permitting
   short reads/final chunks while rejecting premature EOF. If required, stage
   the custom-data file too; missing/locked custom sources fail here.
3. `PublishCopiedFile()` uses `File.Replace()` with a unique backup for an existing
   destination, or `File.Move()` for a new destination. Asset is published first,
   then custom data. No pre-emptive deletion of existing destinations occurs.
4. On a later failure, `RestoreCopiedFile()` restores backups for files this attempt
   published, or removes only a newly published destination that had no predecessor.
   A blocked restore logs the destination/backup and preserves recovery data.
5. On success, install the candidate header and invalidate output availability.
   `RemoveCopyTemporary()` performs best-effort cleanup of this attempt's staging
   files and, after success, backups. Cleanup failure does not turn a completed
   publication into a failed copy or delete output.

`source/BinaryAssetBuilder.Utility/BinaryAssetBuilder/Utility/AssetHeader.cs`:
`IsValidFileLength()` rejects negative chunks and evaluates
`32L + instance + relocation + imports` with 64-bit arithmetic.

This changes failed-copy/validation behavior, not valid serialized bytes, hashes,
cache filename identities, checksum padding or target policy. No session-cache
version bump is required. `CommitFromLocal()` and `CommitFromCache()` continue to
use this same seam; failed calls still propagate through their existing failure/
retry handling. The experimental production guards remain unchanged.

## Verification

```powershell
# Reborn: create isolated synthetic copy/recovery fixtures, never real cache/game files.
& .\source\BinaryAssetBuilder.ManifestInspector\bin\x86\Release\net8.0\BinaryAssetBuilder.ManifestInspector.exe copy-recovery-self-test
```

`CopyRecoverySmokeTest.Run()` tests the real private copy method, using the
built-in null plugin as a diagnostic context rather than an approved EP1 compiler.
The compiler runner includes it as its 54th registered group.

22 scenarios:

- Each of the four wrong identity/hash fields rejects while preserving old output.
- Missing source, short header, truncated payload and extra payload reject.
- Negative instance, relocation and import lengths reject even when another
  chunk is increased so the signed total still matches file length.
- Missing/locked custom source fails before publication; locked asset destination
  and locked asset source preserve existing bytes.
- Locked custom destination causes pair publication failure: an existing asset
  is restored byte-for-byte; if no asset existed before, the newly published
  asset is removed. Old custom data remains unchanged in both cases.
- A valid asset/custom pair replaces both files exactly. Little-endian and
  big-endian valid asset copies retain exact bytes and correct in-memory metadata.
- Empty chunks and one-/fifteen-byte final transfer chunks are accepted.

For every completed scenario, final file contents and header metadata are checked;
no attempt-specific staging/backup files remain. Additional tests exercise a
blocked rollback directly: backup and current output remain intact while the
destination is locked, and retry after releasing the lock restores the backup.
A synthetic size calculation exceeding 32 bits verifies the widened length
validator without allocating a huge file.

## Remaining limits

- File replacement is atomic per file, not across the asset/custom-data pair.
  Process termination or power loss between replacements is not covered by
  exception rollback. There is no restart-time recovery journal yet.
- On filesystems where `File.Replace()` is unsupported, replacing existing output
  fails safely; there is no destructive delete-then-move fallback.
- File locks and rollback were tested on this Windows host, not network/OneDrive
  synchronization races or every filesystem. Cross-process publishers can still
  race; the shared lock only serializes this process's copies.
- A blocked rollback returns failure and retains recovery data; output may be
  partially updated until recovery is performed. It is not reported as success.
- Cleanup is best-effort. There is no automatic backup scavenger/recovery service.
- No payload digest is introduced; an identity/length-valid candidate is not
  cryptographically validated. Upstream dependency/InstanceHash invalidation and
  full EP1 compiler/type-table approval remain separate workstreams.

Next: inspect dependency/InstanceHash invalidation and patch stream checksums;
extend production build proof without weakening the experimental-profile gate.
