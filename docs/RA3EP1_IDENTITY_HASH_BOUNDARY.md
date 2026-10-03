# Identity hash audit and exact text-block correction

Date: October 4, 2026. A concrete current-source hash defect was reproduced and
corrected against pinned EA reference IL. Stock Uprising AudioFile InstanceHash
equivalence is still not proven; diagnostic audio-package hashes remain labeled
diagnostic and are not replaced with a guessed production algorithm.

## Reproduced defect and correction

`source/BinaryAssetBuilder.Core/BinaryAssetBuilder/Core/Hashing/HashingWriter.cs`,
`Write(string)`, previously used `idx + 512 < current.Length`. For exact multiples
of 512 characters it skipped the final full block, then cleared the leftover
string. This could omit data entirely, and writer call partitioning could change
which content was lost. The pinned EA writer uses the inclusive boundary.

The new regression was executed before the fix and failed at length=512,
writeChunk=1, seed 12345678: expected E9A66643, actual 12345678. The loop now uses
`<=`, including each complete block once. Partial-tail behavior remains intact.
This is a character-block boundary, not a 512-byte file-read boundary.

DocumentProcessor versions are bumped from 20/21 to 22/23 (VERSION5/other builds)
to invalidate previous document/intermediate identities. The active Release/x86
build uses version 23. Existing old-document/session rejection tests pass after
the bump. SessionCache.CacheVersion is unchanged; document-version invalidation
is a separate mechanism. The VERSION5 definition is updated but not built in this
turn. Fresh core instance hashes can change even for short XML because the seed
contains this version. Old cache reuse must not be used to preserve old hashes.

## Reference evidence, metadata only

`HashingWriterBoundarySmokeTest.ReferenceEvidence` reads PE/CLI metadata and IL,
not executable code. It pins the reference DLL SHA256 and bounded method shapes
before interpreting known offsets. It never loads the mixed-mode/reference DLLs.

Reference files under `Working RA3 Compiler for Reference/tools`:

- Core SHA256 B36D5015F97532457D64072034F9F5C629A4C060A2D34C49C4F89FD6E6F79E47.
- AudioCompiler SHA256 A14D15ADDEF502C2DF6F6BA272D6098C4C131FBEA9C3ED0149C11C4556347139.

Relevant reference methods/offsets:

| Reference method | Token / evidence |
| --- | --- |
| Core.HashingWriter.Write(string) | 06000188; IL_006A `ble.s` includes exact block |
| Core.HashingWriter..cctor | 0600018B; IL_0000 initializes block size 512 |
| Core.AssetDeclarationDocument.ValidateInstances | 0600005D; IL_0061 uses document seed version 11 |
| Core.HashProvider.GetXmlHash(uint, ref XmlNode) | 0600017D; XmlWriter/WriteTo/Flush then final hash |
| AudioCompiler.Plugin.GetAudioFileExtendedTypeInformation | 06000120; Win32=0 branch IL_0040 ProcessingHash 8FE79286; IL_0065 TypeHash 53C81E47 |

The platform value is read from the reference TargetPlatform enum's constant
metadata. The platform=1 branch has ProcessingHash 8FE78B86 and must not be
mistaken for Win32. The reference type hash agrees with observed EP1 AudioFiles,
but that does not establish the EP1 producer's processor/version seed.

## What InstanceHash actually consumes

Current `AssetDeclarationDocument.ValidateInstances` follows the same inspected
construction sequence as the reference:

1. Seed text hashing with ProcessingHash and DocumentProcessor.Version text.
2. Hash the schema-defaulted/merged XML before reference normalization and TypeId
   injection, using HashProvider.GetXmlHash and its serialized text blocks.
3. Normalize strong/weak references and record declared type IDs in a stream.
4. Append content hashes of referenced files.
5. XOR the hash of the stream's full MemoryStream backing buffer, including
   capacity padding when nonempty, into the XML-derived InstanceHash.

Referenced-file hashing starts with file length and folds actual bytes in reader
chunks (current reader buffers are 1 MiB). Document DependentFileHash additionally
tracks paths/content separately. Strong runtime asset references do not by
themselves incorporate the target's content hash into the parent's core identity.
This differs from our explicitly diagnostic local-event dependency hash recipe.

The current experimental audio profiles use their own processing policies and
the current document version. Reference version 11 and ProcessingHash 8FE79286
must not be silently substituted into them. XML order/defaults/whitespace and
serialization encoding must also be measured: Encoding.Default differs across
runtime/platform environments, and matching stock IDs/type hashes alone cannot
recover these original inputs. No stock InstanceHash comparison is claimed here.

## Validation and next gate

`hashing-writer-boundary-self-test` is the 95th compiler test group. It tests ten
text lengths (0/1/511/512/513/1023/1024/1025/1536/2048) across six write partitions,
repeated finalization and eight independently captured XML serializations.
All 95 compiler groups pass after the correction; both Release/x86 projects
build, with existing layout/diagnostic/enum checks retained. Models/marshallers
remain 785/1,390 and 762/1,390. Overall effort is still roughly 50% complete /
50% remaining; correcting the primitive is not completing production audio.

Next: a hash-only official-schema AudioFile/FileReference harness using owned WAV
bytes, with independent reconstruction of XML seed, referenced-file content hash
and padded dependency stream. Then compare reference/current contexts explicitly
before integrating a checked isolated audio processor. Original stock source WAV
and exact producer settings would be required for a genuine stock InstanceHash
match. No game files, reference SDK DLLs or production audio registry were changed.
