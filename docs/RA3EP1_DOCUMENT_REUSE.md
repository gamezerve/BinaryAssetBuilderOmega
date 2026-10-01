# Retained and serialized document reuse proof

Date: October 1, 2026

## Outcome and scope

New tests carry actual cached documents across build cycles rather than comparing
only fresh sessions. They also save and load plain and compressed XML sessions
through the production APIs. Unchanged documents reuse metadata without loading
the XML again; reported source edits and changed file dependencies cause actual
reload, schema validation and fresh identities.

This exposed and fixed a real crash: cached documents with no stream hints store
their hint list as null. SessionCache.CheckFiles previously dereferenced that
list when such a document changed. It now treats null/empty hints as a full-stream
rebuild, just like a non-document file with no hints.

This proves the **document metadata lifecycle**, not native compiled-byte cache
reuse or EP1 production emission. DependencyProbe is a synthetic, unregistered
diagnostic schema root with TypeHash=0. No compiler/plugin is fabricated or
promoted; experimental EP1 document/output restrictions remain enabled.
Overall effort stays approximately 50% complete / 50% remaining.

## Code and real paths exercised

- `source/BinaryAssetBuilder.ManifestInspector/DocumentReuseSmokeTest.cs`:
  `TestCycles`, `Next`, `Process`, `TestMissingRetainedDependency`.
- `source/BinaryAssetBuilder.Core/BinaryAssetBuilder/Core/Session/SessionCache.cs`:
  `CheckFiles` now null-checks hints before selecting targeted versus full rebuild.
  Real `InitializeCache`, `TryGetFile`, `TryGetDocument`, `SaveCache`, `LoadCache`
  and `SaveDocumentToCache` participate in the tests. The resident checkpoint uses
  reflection only to call private MakeCacheable, without manufacturing documents.
- `Core/SageXml/DocumentProcessor.cs`: `OpenDocument`,
  `ProcessDocumentInternal`, included-document preparation and
  `ProcessDocumentContents` perform the actual reuse/load/validation lifecycle.
- `Core/SageXml/AssetDeclarationDocument.cs`: `Open`, `CurrentState.FromLast`,
  `ReInitialize`, `UpdateDocumentHashes`, `Load`, `Validate`, `MakeComplete` and
  `MakeCacheable` process real source XML and retained/serialized declarations.
- `tests/fixtures/DependencyHashPipeline.xsd`: existing diagnostic root uses
  official EP1 primitive, asset-base and FileReference definitions.

A fresh DocumentProcessor and empty PluginRegistry are created per cycle.
OutputManager is provided because ReInitialize consults output availability; all
paths are owned temporary paths, GenerateOutput is false, and unregistered roots
produce no BinaryAsset. Tests assert no `.asset` or `.manifest` is emitted.

No serialized field layout or content-hashing algorithm changed. Cache versions
remain 19/21 (`VERSION5`/other), as in the preceding atomic-handoff block. This fix
changes null handling, not cache identities or revisions, so no version bump is
needed. VERSION5 is not separately runtime-tested.

## Acceptance matrix

Each of resident objects, plain XML cache and compressed XML cache runs the same
sequence. Disk loads must retain a non-null LastState; unchanged-reuse assertions
also prevent silently treating a rejected cache as a successful fresh build.

| Mutation | Expected and observed result |
|---|---|
| None after first build | IsLoaded=false; instance/dependency identities unchanged |
| Reported XML Payload 1 to 2, same size/time | IsLoaded=true; reason `content changed`; instance identity changes |
| Dependent file AAAA to LONGER, preserved time, no notification | Reload; reason `dependent file changed`; both identities change |
| Reported dependent file LONGER to CHANGE, same size/time | Reload; reason `dependent file changed`; instance identity changes |
| None after dependency reload | Quiet metadata reuse resumes |
| Remove Data field from XML | Reload; dependency hash becomes zero |
| Delete former, now-unreferenced data file | Document remains reusable; identity unchanged |
| Reintroduce missing Data field | FileNotFound, not a stale-success return |

A separate saved-document fixture first demonstrates unchanged reuse, then deletes
its still-referenced dependent file with source XML unchanged. Processing rejects
with FileNotFound. Restoring data and loading the last good checkpoint recovers
the original instance/dependency identities. This is not a claim that partially
failed in-memory document state is transactional.

```powershell
# Reborn: test real retained and disk-loaded document metadata without enabling native compiler output.
& .\source\BinaryAssetBuilder.ManifestInspector\bin\x86\Release\net8.0\BinaryAssetBuilder.ManifestInspector.exe document-reuse-self-test
```

Release/x86 inspector full-reference rebuild and main application build pass.
The full compiler runner now has 58 registered groups and passes; layout tests
and 33 schema enum mappings also pass. Inventory remains 784 models and 760
typed marshallers of 1,390 complex types. No game assets, large WorldBuilder BIN,
SDK installations or user session caches were modified.

## Remaining boundaries and next work

- Native `.asset` payload reuse, missing-output recompilation and type/processor
  revision invalidation need registered, validated compiler fixtures. This test
  intentionally keeps its root unregistered; it cannot prove those branches.
- Include/reference/inheritance changes are not newly proven by this one-root
  retained fixture. Earlier fresh-session tests remain narrower evidence.
- Stream-hint/precompiled production early returns and actual resident process
  timing remain separate integration gates; this test uses GenerateOutput=false.
- Unreported same-size/same-time edits still evade metadata signatures.
- Save/load round trips do not prove crash durability, malicious cache rejection,
  or rollback of a failed document build. Failed-state recovery here starts from
  the last good checkpoint.
- No Uprising mod has been loaded by this block. Final root registrations,
  processor native layouts, SDK/WorldBuilder packaging and game loading remain.

Next: return to real EP1 processor/type proofs, selecting a small root asset with
recoverable native/tokenized output rather than bypassing cache/profile safety.
