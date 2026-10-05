# Reviewed dummy-hook schema admission

## Outcome

Real Uprising global.xml and PathMusic/BasePathMusicEvent.xml now pass one-source
typed XML validation against the explicitly reviewed diagnostic Shield candidate.
Sixteen effective file attributes are available; BasePathMusicEvent binds one
PathfinderEventHeader FileReference to `AUDIO:Pathfinder\RA3EPMus\PC\RA3EPMus.h`.
No header/payload body is opened, and its existence remains unproved by this command.
Global.xml binds zero file fields because this command does not follow its Includes.

```text
BinaryAssetBuilder.ManifestInspector.exe sdk-reviewed-schema-candidate [absolute-source.xml]
BinaryAssetBuilder.ManifestInspector.exe sdk-reviewed-schema-candidate-self-test
```

Exit 0 means diagnostic schema admission and, if requested, one-source validation.
It is not a build or complete dependency result. Exit 2 denotes source/admission
incompleteness; invalid review authority fails rather than broadening the policy.

SchemaEngineCompiled is true. SchemaCompiled remains false because both warnings
are retained. The new SchemaAdmitted field is true only after explicit review;
WarningPolicy is `diagnostic-dummy-hooks-v1` and WarningReviewChecks records probes.
ReadOnly/SnapshotOnly remain true; ProductionBuildReady/FullDependencyCoverage false.
Default sdk-effective-schema and unreviewed sdk-shield-schema-candidate gates remain
unchanged, and reference XSD/XML bytes are never edited.

## Narrow policy and proofs

`SdkSchemaHookReview.Review` requires the exact candidate catalog digest
`3A54DC3DDC54C1AACF15C03AF6AD67ED007AD99608CEDF78CC43EAD45AA969D3`, structural
engine success, zero schema errors and exactly the two prior warning messages with
in-memory file URI/line provenance (AssetTypeW3D:337 and AssetTypePathMusic:43).
This digest covers the full staged candidate catalog, not only the two warning files.
Changed, missing, extra, differently located or differently worded warnings reject.
Runtime/localization/schema updates therefore require a fresh review, not auto-waiver.

The compiled W3DMesh.VertexData and PathMusicTrack.Handle attributes must be absent,
and neither owner may have an attribute wildcard. In-memory positive mesh/track
sources must validate; track File/PathfinderTrackHeader values must bind exactly.
Adding authored VertexData or Handle must produce an attribute-specific XML
validation failure with no trusted partial file fields. These probes rerun on each
reviewed admission. They prove schema input behavior, not valid geometry/music data.

No second schema transformation was introduced. The Shield-only source/candidate
digests and transformation record are retained; neither hook declaration is deleted.
Clean-schema status is not relabeled as successful and warnings are not suppressed.
One-source binding continues to use captured bounded bytes with DTD/resolvers disabled.

## Processor evidence and remaining risk

`source/BinaryAssetBuilder.W3XCompiler/Marshaler.W3D.cs` marshals vertex/normal/
tangent/bone element data into W3DMeshPipelineVertexData, pushes a generated VertexData
context and calls `W3DMeshProcessor.BuildVertexData`; it does not source that pointer
from an authored VertexData attribute. `W3DMeshProcessor.cs` computes mappings,
layout and output vertex data. This code reading supports the dummy-hook input
review; no W3D processor or native mesh build was run in this milestone.

AssetTypePathMusic.xsd states that BAB fills Handle, but exact-name source searches
found no PathMusicTrack model/processor/header/Handle implementation in the current
C# source tree apart from diagnostic tests. Thus automatic Handle generation is
not implemented/proven by this review. Do not substitute numbered extracted WAVs
or compiled audio cdata for the original Pathfinder source/header requirements.
PathMusic production compilation remains a separate port/reverse-engineering task.

Effective attributes are compiled/inherited schema evidence, not an exhaustive
instance dependency list. DataBlob element binding remains supported by Bind, but
its particles are not counted in the 16-attribute inventory. Asset inheritFrom
preprocessing, source Include traversal, physical resource resolution/fingerprints,
final type hashes, WorldBuilder packaging and in-game loading remain open.

## Validation and next step

Release/x86 builds; all 116 default compiler groups and 33 enum checks pass.
New tests check exact warning/digest review, repeatable source probes, policy/Shield
authority, missing/extra/changed-provenance/error rejection, and unreviewed gate isolation.
The real one-source CLI checks both returned exit 0 with two visible warnings:

| Source | Validated | Bound file fields | Meaning |
| --- | --- | ---: | --- |
| global.xml | Yes | 0 | Root XML only; Includes were not validated here |
| PathMusic/BasePathMusicEvent.xml | Yes | 1 | AUDIO header logical value identified; payload not read/resolved |

Next connect the reviewed compiled schema to the existing bounded reachable-source
graph, preserving each source fingerprint and reporting inheritance/preprocessing
validation failures instead of trusted partial dependencies. Physical AUDIO/ART
root attribution and missing-file findings must stay separate from type binding.
Do not claim a production SDK from diagnostic schema admission.

Model/marshaller inventory remains 785/1,390 and 762/1,390; the engineering-effort
estimate remains approximately 50.25%, rounded to 50% complete / 50% remaining.
