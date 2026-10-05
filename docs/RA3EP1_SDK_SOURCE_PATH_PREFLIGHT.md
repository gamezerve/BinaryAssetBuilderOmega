# Bounded SDK source-path preflight

## Contract

This adds a read-only, snapshot-only path graph to the existing explicit EP1
environment/schema/manifest gate. It does not compile sources, create output,
read codec/art payload bodies, use registry discovery or prove game loading.
The original `sdk-preflight` command and PowerShell environment wrapper are unchanged.

```text
BinaryAssetBuilder.ManifestInspector.exe sdk-source-preflight ra3ep1 <absolute-schema-root> <absolute-source-root> <absolute-entry.xml> <absolute-new-output-directory> [--art-root absolute-directory] [--audio-root absolute-directory] [absolute.manifest=runtime.manifest ...]
BinaryAssetBuilder.ManifestInspector.exe sdk-source-preflight-self-test
```

Exit 0 means the **scoped path graph** has no issues. Exit 2 emits a JSON report
with incomplete graph diagnostics; it is not a successful build. Invalid command or
root/environment inputs use the existing CLI exception/failure handling. Consumers
must check both exit status and report flags, never infer readiness from JSON presence.

The JSON contains the original `Environment` report and a separate `SourcePaths`
report. `ProductionBuildReady` and `FullDependencyCoverage` are always false.
`Environment.IncludedSourcesValidated` remains false: following paths is not XSD
validation. Physical paths remain distinct from game-visible external manifest names.

## Implementation and resolver evidence

- `source/BinaryAssetBuilder.ManifestInspector/SdkSourcePathAudit.cs`:
  `Inspect`, local `Visit`/`ResourcePath`, and `Resolve` perform confined reads,
  cycle detection, edge attribution and resource metadata checks.
- `SdkEnvironmentPreflight.cs`: shared internal path/read guards reject device/ADS
  aliases, reparse ancestry and bounded-read violations.
- `Program.cs`: explicit root options, duplicate/unknown option rejection,
  JSON evidence and separate incomplete exit status.
- `SdkSourcePathAuditSmokeTest.cs` and `CompilerSmokeTest.cs`: managed fixtures.
- Core reference: `source/BinaryAssetBuilder.Core/BinaryAssetBuilder/Core/IO/FileNameResolver.cs`,
  `ResolvePath`/`SearchPaths`; `AssetDeclarationDocument.GatherUnvalidatedIncludes`
  resolves Include source paths and retains inclusion roles.

Relative paths remain confined to their current document's explicit root. DATA
switches to the source root. ART basenames use the first two filename characters
as a directory, matching the literal branch of the core resolver; ART paths with
directories are relative to the selected ART root. AUDIO paths use the selected
AUDIO root. Internal `..` is allowed only when its canonical result stays inside
the relevant root. Roots must be existing, non-reparse, distinct and non-overlapping;
requested output must not be inside any source/art/audio root.

All/instance/reference edges retain source role/order, including repeated edges to
a shared document. Each document is read once, active cycles are reported, and
reference XML availability is inspected rather than substituting compiled manifests.
Only EA AssetDeclaration XML/W3X Include documents are admitted; DTD/entities reject.
Document fingerprints are exact SHA-256 over bounded bytes. An entry changed since
environment inspection reports `StaleEntry`.

Resource detection covers explicit ART/AUDIO attribute and leaf-text literals and
AudioFile `File` attributes. It is intentionally **not a schema-typed exhaustive
inventory**: other relative file fields, expressions and asset IDs are not inferred.
Resources record existence and length only, not payload content/hash/correctness.
ROOT, absolute dependency paths, macros, registry/search fallbacks, postfix/LOD
variants and trailing-dot/space path aliases are outside this profile.
This is not full behavioral equivalence to the legacy resolver.

Bounds: 512 XML documents, depth 32, 4 MiB per document, 32 MiB aggregate XML,
4,096 Include edges, 2,048 resource records and 128 diagnostics. Hitting a bound
sets `StoppedAtLimit` and cannot report the graph complete. No atomic filesystem
snapshot is claimed; later build planning must recheck source and dependency evidence.

## Validation and real inputs

Release/x86 build passes. All 112 managed compiler groups and 33 generated-enum
checks pass. New fixtures cover roles/shared-node reuse, internal parent paths,
ART fanout/subdirectories, AUDIO/plain AudioFile paths, missing roots/files,
cycles, device/ADS/macro/escape rejection, child DTDs, stale entry fingerprints
and depth/edge/resource/diagnostic limits. A resource held with an exclusive file
lock still passes metadata inspection, demonstrating that its body is not opened.
Default tests do not invoke native codecs. Requested output stays absent.

Real source root:

`D:\OneDrive\CNC Files\CnC_Modding_Support-main (Official XML, Schema, Script, Shader, Maps)\Uprising\Xml (Uprising)`

`global.xml` was audited with the staged EP1 schema catalog and **no invented
ART/AUDIO roots**. Result: exit 2; 396 XML documents, 664 Include edges, zero
successful resource records, six issues, no traversal limit, output absent.
The zero resource count does not mean the game has no art/audio dependencies.

| Document suffix | Logical path | Open evidence |
| --- | --- | --- |
| Sounds/MasterAudioAssets.xml | AUDIO:audioassets.xml | Explicit AUDIO root not supplied |
| Sounds/MasterAudioAssets.xml | AUDIO:audioassets_notfordemo.xml | Explicit AUDIO root not supplied |
| Sounds/MasterAudioAssets.xml | AUDIO:audioassets_retailonly.xml | Explicit AUDIO root not supplied |
| PathMusic/BasePathMusicEvent.xml | AUDIO:Pathfinder/RA3EPMus/PC/RA3EPMus.h | Explicit AUDIO root not supplied |
| SkirmishAI/Personalities/AIPersonalityLibrary.xml | DATA:maps/official/CAMP_S06_Iceland_Bass/AIP_S06_SovietKrukov.xml. | Trailing-dot alias outside safe profile |
| SkirmishAI/States/AIStateLibrary.xml | DATA:maps/official/CAMP_S06_Iceland_Bass/AIS_S06_SovietKrukov.xml. | Trailing-dot alias outside safe profile |

The AUDIO findings do not establish that the files are missing on disk; no fallback
root was searched. The two `.xml.` findings are not yet proven source errors:
Windows/legacy normalization must be examined before any explicit migration rewrite.
No official/reference source was edited, no production build ran, and no full
worldbuilder.bin dump was taken.

## Remaining work

Locate and verify the actual Uprising AUDIO/ART source roots; keep compiled
unpacked assets separate from original source files. Re-run this graph with explicit
roots, investigate the two trailing-dot Includes, then inventory schema-typed file
dependencies and bind their snapshots to admitted target-aware build plans.
Final EP1 type hashes/native layouts, unported processors, WorldBuilder packaging
and a real minimal mod load remain separate release gates. The conservative effort
estimate remains approximately 50% complete / 50% remaining; this bounded SDK
planning improvement is too small to justify another rounded percentage point.
