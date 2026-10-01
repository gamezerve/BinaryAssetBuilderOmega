# Watcher-driven hashes and resident session refresh

Date: October 1, 2026

## Outcome

Reported file changes now force a content hash even when both timestamp and size
are unchanged. Resident session initialization samples fresh metadata rather than
the prior build's memoized values. Unrelated unchanged files no longer trigger
the path-monitor consistency error merely because they were absent from the list.

This is source/cache infrastructure proof, not a usable EP1 SDK or a game-load
test. No type registry or production-profile restriction was bypassed. Overall
effort remains approximately 50% complete / 50% remaining; model/marshaller counts
are unchanged. The compiler runner now includes 56 registered regression groups.

## Defects and implementation

`source/BinaryAssetBuilder.Core/BinaryAssetBuilder/Core/Session/SessionCache.cs`,
`CheckFiles()` had three related problems:

- Its first `IsDirty` call used memoized metadata from a previous resident build.
  The record was reset only after that check, too late to mark stream hints dirty.
- Watcher membership was checked case-sensitively against every cached file,
  including unchanged files. A nonempty notification list could therefore cause
  a false `ErrorCode.PathMonitor` exception for an unrelated file.
- Membership did not force content hashing; same-time/same-size notified changes
  could retain the previous content hash.

The method now normalizes full paths, uses an ordinal case-insensitive set,
resets each file before inspecting it and requests forced refresh for reported
paths. A notification applies to every cached configuration of the same physical
file. Known stream hints are dirtied; changed non-document files conservatively
dirty all streams. A nonempty report that omits an **actually metadata-dirty**
cached file still raises the intended consistency error. An empty list retains
the metadata-scan fallback; it does not force all files to be content-hashed.

`source/BinaryAssetBuilder.Core/BinaryAssetBuilder/Core/FileHashItem.cs`,
`Reset(bool forceRehash)` invalidates the previous persisted timestamp/length
signature when requested. It does not change content-hash calculation. The
existing parameterless reset remains metadata-only. Invalidating persisted
signature fields also preserves an unconsumed refresh across XML serialization.

Session versions advance from 17/19 to **18/20** (`VERSION5`/other). The active
EP1 Release/x86 test initially used 20 and rejected a retained version-19 session.
The later [atomic batch handoff](RA3EP1_MONITOR_BATCH_HANDOFF.md) advances the
active version to 21 and tests rejection of version 20.
The VERSION5 branch is updated but not separately runtime-tested. Serialized
field layout is unchanged from the preceding length-signature block; the version
bump invalidates sessions produced under the previous resident-refresh policy.

## Watcher event collection

`source/BinaryAssetBuilder.Core/BinaryAssetBuilder/Core/IO/PathMonitor.cs`:

- Listen for Created as well as Changed/Deleted events.
- Enable IncludeSubdirectories for nested SDK source/asset trees.
- Record both OldFullPath and FullPath for a rename.
- Synchronize callbacks, reset, trust checks and snapshots with one event lock.
- Deduplicate paths case-insensitively, while still counting raw events against
  the existing EventLimit of 20.
- Return a detached snapshot. A failed volume flush no longer discards already
  collected positive notifications.
- Preserve an unrecoverable watcher-error flag across reset; reset does not claim
  to repair the watcher.

The event set does not provide an ordering contract. The cache consumes it by
membership rather than event order. Existing event-limit/flush timing and the
builder's trust/fallback policy are otherwise retained.

## Reproducible validation

```powershell
# Reborn: test retained-session records and deterministic watcher callbacks using only temporary fixtures.
& .\source\BinaryAssetBuilder.ManifestInspector\bin\x86\Release\net8.0\BinaryAssetBuilder.ManifestInspector.exe watcher-cache-self-test
```

`WatcherCacheSmokeTest` exercises actual retained SessionCache records through
`InitializeCache`, `TryGetFile` and an in-memory checkpoint of the production
private MakeCacheable method. It verifies:

- Reported same-time/same-size edit changes the hash in two configurations.
- Mixed casing/slash styles and duplicate reports match the same physical file.
- An unrelated unchanged file remains clean and causes no false exception.
- Non-document changes dirty all streams.
- With an empty event list, timestamp-preserving size changes are found despite
  prior hash/metadata memoization.
- A genuinely dirty omitted file raises PathMonitor; metadata-scan fallback can
  then recover initialization.
- Pending forced hashes survive real XML signature serialization/deserialization.
- Focused cached document stream-hint metadata marks exactly its two known
  streams, and an unchanged subsequent resident initialization stays clean.
- The previous session version is actually rejected before reuse.

Watcher tests invoke the real callback handlers deterministically, then inspect
real FileSystemWatcher configuration and immediately dispose that test watcher.
They cover created callback payloads, both rename paths, case-insensitive
deduplication, detached snapshots, reset, 100 parallel callbacks, the trust limit,
sticky watcher errors and nested-directory configuration. They **do not** prove
OS notification delivery timing, native volume-flush behavior or notification
completeness on OneDrive/network filesystems.

Release/x86 build, all 56 compiler groups, layout tests and 33 enum mappings pass.
All file-writing fixtures own fresh temporary directories; no game/cache files
or the large WorldBuilder BIN were read or modified.

## Remaining boundaries

- Unreported changes preserving both size and timestamp still evade metadata-only
  fallback. There is no unconditional content-scan or secure payload digest.
- A watcher overflow/error or unavailable/untrusted watcher still routes through
  the builder's existing metadata fallback; absence of events is not proof that
  content was unchanged.
- The subsequent [atomic batch handoff](RA3EP1_MONITOR_BATCH_HANDOFF.md) resolves
  the builder's separate snapshot/reset and precheck-trust windows. Legacy
  GetChangedFiles/Reset remain available, but the builder no longer combines them.
  OS notification completeness and native volume-flush behavior remain unproven.
- Stream-hint tests use focused reflected cached metadata; they do not compile
  or reload a real EP1 mod. Native plugin behavior and dependency declarations
  remain unproven by these source hash tests.
- No mid-build mutation reproducibility guarantee, transactional session recovery,
  full compiled-document invalidation proof or game-load validation is added.

Next: audit compiled document reuse against these
fresh signatures, keeping experimental production and cache gates in place.
