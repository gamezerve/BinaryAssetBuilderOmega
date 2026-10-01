# Atomic monitor/cache batch handoff

Date: October 1, 2026

## Outcome

The main builder no longer gets a notification snapshot and later clears the
entire live queue. Notifications received during cache initialization stay
pending for the next build, including repeated reports of the same path. Paths,
event count and trust are captured together. If cache initialization throws,
the consumed batch is restored without deleting newer events.

This closes an in-process handoff race, not OS notification completeness or a
filesystem snapshot guarantee. Production EP1 type/profile gates remain enabled.
Overall effort remains approximately 50% complete / 50% remaining; inventory
stays at 784 models / 760 typed marshallers out of 1,390 complex types.

## Implementation and compatibility

- `source/BinaryAssetBuilder.Core/BinaryAssetBuilder/Core/IO/PathMonitor.cs`:
  `DrainChanges()` takes an immutable detached `ChangeBatch` and clears only the
  pending generation under the callback lock. `CompleteBatch()` acknowledges the
  consumed batch without clearing live events. `RestoreBatch()` merges its paths
  and raw count back into the live queue and retains uncertainty. Ownership and
  single-use guards reject foreign, completed or already restored batches.
- `PathMonitor.InitializeCache()` performs drain, cache initialization and
  acknowledge; exceptions restore the batch and propagate. Nonresident mode
  retains the existing metadata-only policy. Event counters saturate rather than
  wrapping; trust is false at the existing 20-event limit, on watcher error, on
  failed flush, or when restoring an uncertain batch.
- `source/BinaryAssetBuilder/BinaryAssetBuilder/BinaryAssetBuilder.cs`:
  `InitializeSessionCache()` uses this handoff, replacing the separate trust
  precheck, `GetChangedFiles()` and later `Reset()`.
- `source/BinaryAssetBuilder.Core/BinaryAssetBuilder/Core/ISessionCache.cs` and
  `Core/Session/SessionCache.cs`: the new two-argument `InitializeCache()` separates
  known positive paths from notification completeness. Known paths always force
  rehashing. An incomplete list does not raise a completeness-based omission error
  for an unreported metadata-dirty file; metadata fallback still discovers it.
  The old overload preserves its existing direct-call behavior for nonempty lists.

The interface overload requires external custom ISessionCache implementations to
be rebuilt/updated; SessionCache is the repository's implementation. Legacy
GetChangedFiles/Reset remain for compatibility, not as the builder's handoff.
Session versions advance from 18/20 to 19/21 (`VERSION5`/other), rejecting caches
possibly stale from lost notifications. Release/x86 tests reject version 20;
VERSION5 is updated but not separately runtime-tested. Serialized field layout,
content hashing, processor revisions, native output and type hashes are unchanged.

## Reproduce and acceptance checks

```powershell
# Reborn: exercise atomic watcher batches and real retained cache records in isolated temporary fixtures.
& .\source\BinaryAssetBuilder.ManifestInspector\bin\x86\Release\net8.0\BinaryAssetBuilder.ManifestInspector.exe monitor-batch-self-test
```

`MonitorBatchSmokeTest` verifies:

- Late and repeated-path reports survive successful acknowledgement.
- Batch paths remain detached and immutable across later callbacks.
- Failed initialization restores paths alongside events received during failure.
- Replayed acknowledgements/restores and foreign-monitor batches are rejected.
- Crossing the event limit after an earlier trust read is captured as untrusted.
- Restored uncertainty survives the next drain; raw counters saturate safely.
- Real retained SessionCache records refresh a reported same-size/same-time edit
  from an incomplete batch and an omitted size-changing edit by metadata scan.
- The actual handoff helper, used by the builder, passes paths/completeness
  correctly on success, failure, retry and nonresident initialization.
- Across concurrent callbacks and drains, all 500 unique paths and raw events
  are accounted for exactly once in consumed batches.

Release/x86 inspector and main application builds pass. Full compiler regression
runner (57 registered groups), layout tests and 33 enum mappings pass. Model and
marshaller counts are unchanged. These tests use deterministic callback injection
and temporary synthetic files; no game BIN or WorldBuilder BIN is read/modified.

## Remaining risks and next gates

- Unreported same-size/same-time edits still evade metadata-only fallback.
- OS notification latency/completeness, native volume flushing, OneDrive/network
  delivery and actual resident-process integration timing are not demonstrated
  by deterministic callback tests. A flush failure is conservative, not repaired.
- Files may change during hashing/building; batch handoff is not a stable input
  snapshot. No mid-build reproducibility guarantee is added.
- Restoration is in-memory and covers cache-initialization exceptions, not
  process crashes or a durable notification journal. It does not roll back
  arbitrary cache mutations or work performed after successful initialization.
- Full compiled-document dependency invalidation, remaining processors/type
  registrations, SDK/WorldBuilder packaging and in-game mod loading remain open.

Next: test compiled-document reuse end to end with changed source and dependent
file signatures, without bypassing the experimental production/cache restrictions.
