# Complete selected Conscript scripted-draw metadata

## Outcome

`Ep1ScriptedDrawTokenizer.Tokenize` now generates recursive metadata for both
complete selected Conscript scripted-draw graphs from their compiled native
buffers. The producer receives no stock template, stock offsets or unit identity.
It is still a selected component writer, not general GameObject publication.

| Complete selected graph | Model / animation states | Independent stock metadata prefix | Native BIN / RELO / IMP |
| --- | ---: | ---: | --- |
| Main scripted draw | 5 / 80 | 53,052 bytes | 25,304 / 712 / 636 |
| Parachute scripted draw | 5 / 4 | 4,512 bytes | 2,200 / 72 / 56 |

Both entire prefixes match independently captured stock metadata, after mapping
stock native coordinates through the complete typed native comparison. Every
metadata word is compared, including field names, wire types, base tables,
list count/item markers, ordered child tables, optional null entries and table
terminators. Metadata addresses and native addresses are rebased separately.

The output retains every native payload byte, every import occurrence value and
the original auxiliary stream order. Inverse relocation must recover the input
native graph exactly. The earlier explicit parachute Script CRLF/LF comparison
boundary remains: source/producer text is not rewritten and LUA is not executed.

## Source edits and corruption controls

Two further full-source builds change the weak module ID, MaxRecoilDistance to
17.5, UseProducerTexture and NoRotate. Real Core normalization and Draw dispatch
are rerun; literal native values and the invariant-lowercase ID hash are checked.
The same independently stock-proven metadata topology must match. All 10 model
and 84 animation states remain; this does not create a new GameObject ID.

Each stock graph has 33 malformed-input controls: wrong type/identity, enums,
boolean/flag bits, nonfinite real, malformed list owners/counts, unsupported
nonempty children, broken script headers/text, missing/duplicate auxiliary slots
and orphan storage. Detached controls mutate **every metadata word** plus native
payload and auxiliary data: 13,270 controls for main, 1,135 for parachute.
All 66 input and 14,405 detached controls pass.

## Producer ownership and limits

- Explicit native descriptors cover the root, Draw/Module bases, model states,
  animation states, models, animations, bone attachments, replacement textures
  and particles; primitive ExtraPublicBone items target native string headers.
- Strings preserve null versus allocated-empty and require bounded ASCII/NUL
  storage. Only identical character spans can be shared; record owners cannot
  overlap. No unowned nonzero bytes or zero gaps longer than three bytes pass.
- Native data is limited to 2 MiB; metadata to 4 MiB, individual lists to 4,096
  entries, text to 65,536 bytes, recursive depth to 32 and visits to 100,000.
- Nonnull Material/ShadowInfo and nonempty FXEvent, LuaEvent, Turret, SubObject,
  AttachModel or EmbedPortal currently **fail explicitly**. Their empty/null
  metadata is stock-proven; their populated variants are not enabled.
- The two complete source leaves still use the separately documented reviewed
  schema/particle-order/render-name preparation. This is not a production
  schema factory, XInclude/XPointer execution or general source closure.

## Files and next gate

- `source/BinaryAssetBuilder.XmlCompiler/BinaryAssetBuilder/XmlCompiler/Ep1ScriptedDrawTokenizer.cs`:
  `Tokenize`, invocation-local `Builder`, recursive `Emit`, exact owner checks.
- `source/BinaryAssetBuilder.ManifestInspector/SelectedNativeGraphComparer.cs`:
  stock-to-compiled native record/header coordinates; character payloads are
  deliberately not metadata coordinate anchors.
- `source/BinaryAssetBuilder.ManifestInspector/Ep1ScriptedDrawTokenSmokeTest.cs`:
  independent full-prefix oracle, inverse payload/auxiliary and refusal controls.
- `source/BinaryAssetBuilder.ManifestInspector/Ep1ScriptedDrawNativeSmokeTest.cs`:
  complete official source graphs and authored full-graph edits.
- CLI: `ep1-scripted-draw-token-self-test <official-xml-root> <static.manifest> <global.manifest>`.
  The batch adds this group only with explicit `-UprisingXmlRoot`.

The subsequent [shared Draws gate](RA3EP1_CONSCRIPT_SHARED_DRAWS.md) now composes
both scripted and both tracer modules on one Core occurrence table. Next: join
this container with the remaining whole-unit owners and their shared references.
There is no complete GameObject stream/package, alpha admission expansion,
new-model compilation, game launch or install change in this milestone.

## Regression evidence

Final Focused regression with explicit official XML root: **60/60** groups in
**265.734 seconds**; token group **14.480 seconds**, native group **13.960 seconds**.
The batch's initial build passed with zero warnings/errors in 1.82 seconds;
the preceding changed XmlCompiler build passed with its two existing audio
nullable-context warnings and zero errors. All eight runtime hashes remained
unchanged throughout the batch. This is not a Full-profile run.

Local evidence: `artifacts/ConscriptScriptedDrawTokens-a/batch-report.json`,
`compiler-build.log` and `ep1-scripted-draw-tokens.log`. The report explicitly
records `GameExecuted=false`, `InstalledFilesModified=false` and
`AlphaCapabilitiesExpanded=false`.
