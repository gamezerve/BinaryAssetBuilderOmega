# Schema file-reference declaration inventory

## Outcome and command

`sdk-file-reference-catalog` emits JSON for 843 staged EP1 XSD files and 17
lexically declared file fields, including five DataBlob fields. It takes no
arguments, reports the absolute staged schema root/catalog SHA-256, and never
compiles source assets, reads payload bodies or writes output. Its exit 0 means
inventory success, not build readiness. ReadOnly/SnapshotOnly are true;
FullDependencyCoverage/ProductionBuildReady remain false.

`sdk-file-reference-catalog-self-test` runs the separate managed regression group.
Release/x86 builds and all 113 default compiler groups / 33 enum checks pass.
Model/marshaller coverage remains 785/1,390 and 762/1,390 respectively. The weighted
effort estimate remains approximately 50% complete / 50% remaining.

## Why FileReference ancestry matters

Core `source/BinaryAssetBuilder.Core/BinaryAssetBuilder/Core/SageXml/AssetDeclarationDocument.cs`
uses `HandleReferenceType` and `XmlSchemaType.IsDerivedFrom` to dispatch types
derived from `XmlFileReferenceType`. `HandleFileReferenceType` then trims/lowercases
the logical path, resolves/hash-checks it, records document/instance file dependencies
and replaces the node value with its physical reference path. This behavior is
distinct from resolving asset IDs and from Include path traversal.

`schemas/ra3ep1/xsd/Base/AssetBase.xsd` restricts FileReference from xs:anyURI
and DataBlob from FileReference. Testing only literal ART/AUDIO strings or only
the type's local name misses relative DataBlob files and risks namespace collisions.

| Owner type | Declared fields | Type |
| --- | --- | --- |
| AptAptData, AptConstData, AptDatData, AptGeometryData | File on each | DataBlob |
| AudioFile | File | FileReference |
| PathMusicMap | File | FileReference |
| PathMusicTrack | File, PathfinderTrackHeader | FileReference |
| PathMusicEvent | PathfinderEventHeader | FileReference |
| TestTexture | File element; CustomData attribute | DataBlob; FileReference |
| TerrainTextureTile | BaseTexture, NormalTexture | FileReference |
| TerrainTextureAtlasRuntime | BaseTextureAtlas, NormalTextureAtlas | FileReference |
| Texture, OnDemandTexture | File on each | FileReference |

OnDemandTexture.File retains its xas:pipelineOnly flag. These are declarations,
not counted source-file references or proof that every owner processor is ported.

## Implementation and bounds

`SdkFileReferenceCatalog.Inspect` reuses the existing bounded schema catalog,
captures each schema's exact bytes and checks its SHA-256 against that catalog,
then rechecks catalog membership/fingerprints after collecting declarations.
The reported catalog digest is the existing length-delimited metadata digest,
not EA AllTypesHash or a compiler cache identity. This consecutive-read evidence
is snapshot-only, not an atomic filesystem transaction or official-release signature.

`Collect` parses bounded in-memory XSD with DTDs/resolvers disabled, follows named
simple-type restriction ancestry and direct anonymous inline restrictions, and
selects attribute/element declarations reaching the EA FileReference QName.
`QName` preserves namespace identity and supports xs:QName surrounding whitespace.
For InlineRestriction fields, DeclaredType is the named restriction base rather
than an asserted anonymous type name. Conflicting ancestry, selected-field cycles,
unbound QName prefixes and over-limit evidence reject.

Bounds: 1,024 documents, 2 MiB each, 64 MiB aggregate, 64 ancestry steps and
4,096 selected declarations. Shared directory/reparse/device/ADS guards still apply.
Tests independently check the 17 staged declarations, five DataBlob fields,
pipeline-only metadata, namespace collisions, inline restrictions, valid QName
padding, conflicting/cyclic ancestry, DTDs and oversized inputs.

This is deliberately not a compiled effective schema. It does not expand complex
type inheritance or attribute groups, model list/union semantics, bind xsi:type to
instances, or validate source XML. Those are required before declaring exhaustive
schema-typed dependency coverage. Existing source-path audit flags are unchanged.

## Real source-path follow-up

Under the external Uprising Xml (Uprising) root, these normal files exist:

- Maps/official/CAMP_S06_Iceland_Bass/AIP_S06_SovietKrukov.xml: 3,657 bytes.
- Maps/official/CAMP_S06_Iceland_Bass/AIS_S06_SovietKrukov.xml: 2,477 bytes.

The two Include literals still end in `.xml.` and still reject in the safe literal
path profile. File existence establishes possible normalization counterparts,
not equivalence of reads, content or build semantics. No reference XML was edited.
An explicit, fingerprinted migration mapping or separately tested compatibility
policy is preferable to silently weakening all path guards.

Filename inventories under the external Uprising support root and these D:\TEMP
directories found no audioassets*.xml or RA3EPMus.h:

- RA3 Uprising Sources
- Red Alert 3 Uprising Source Data
- Audio Manifest

This is a scoped search result, not a claim about every disk directory.
RA3 Uprising Sources/Tracks contains 1,514 WAV files with numbered names.
EnglishAudio/data contains compiled audio.manifest/bin/relo/imp and cdata; it is
not the missing original AUDIO XML/header tree. No waveform/cdata payloads or
large worldbuilder.bin bodies were read for this milestone. No ART root was inferred.

## Next work

Build a bounded effective-schema/instance binding layer before connecting these
declarations to reachable XML. Preserve inherited fields, source roles and exact
snapshots; keep FileReference payload dependencies separate from asset/import
dependencies. Resolve the reviewed trailing-dot paths explicitly, and establish
the original AUDIO/ART provenance or document unavailable source inputs.
Final processor/type hash, WorldBuilder packaging and actual Uprising mod-load
validation remain separate release gates.
