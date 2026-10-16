# EP1 group-audio client metadata fragment

## Scope

The selected `LargeGroupAudioClientUpdateModuleData` client record uses the
existing 28-byte EP1 native model and actual GameObject client dispatch. This
follow-up adds generated inheritance/field/list metadata, not audio asset
compilation, playback, complete ClientUpdates root composition or GameObject
stream publication. Stock proof is the Conscript
`GameObject:SovietAntiInfantryInfantry` module `LargeGroupKey`.

## Reviewed wire contract

| Native offset | Field | Metadata type / meaning |
| ---: | --- | --- |
| 0 | TypeId | `9826A297`, terminal field type `23E650B8` |
| 4 | id | local weak module ID, `BDF07965` |
| 8 | FramesBetweenUpdatesMin | uint `763177A4`; literal frames, stock 3 |
| 12 | FramesBetweenUpdatesVariation | uint `763177A4`; literal frames, stock 2 |
| 16 / 20 | Key count / items | list `58BEC727`; ordered contiguous uint hashes |
| 24 | UnitWeight | ushort `43501092`, stock 1; two zero padding bytes |

Native allocation is `28 + 4*N` bytes, with the only local relocation at 20 and
no imports. The selected writer accepts 1..64 keys, preserving order and repeated
hashes. The official schema allows an unbounded list; 64 is a diagnostic writer
limit, not an EA format or eventual SDK limit.

Generated metadata is `180 + 12*N` bytes. The inheritance chain is
`LargeGroupAudioClientUpdateModuleData -> ClientUpdateModuleData ->
BehaviorModuleData -> ModuleData`. The count marker at 168 uses
`6C827073 / 5D448FD8`, targeting native count. Each element uses
`B151A0EB / 00000000`, targeting its native hash word directly, **not another
metadata table**. Metadata targets and native relocation slots have different
origins and are rebased separately. Zero terminators remain explicit.

The frame attributes deliberately retain their existing literal-frame conversion
and do not use the GameObject timeout's millisecond Duration conversion. No legacy
general uint/duration or audio processor is changed.

## Validation gate

`Ep1GroupAudioClientTokenSmokeTest` checks the complete independent 192-byte stock
metadata prefix and complete 32-byte native graph for one key, canonicalizing only
the reviewed native Key pointer and metadata/native origins. An independently
specified literal-tag verifier also checks all generated words, inverse native
payload and exact auxiliary streams, without invoking the producer to repair a
detached fragment.

Author controls exercise 1/2/32/64 ordered keys, repeated hashes, default/zero/max
raw frame values and ushort weights through raw reviewed schema validation and
actual client polymorphic dispatch. Repeated generation must be identical.
Native identity/count/pointer/padding/extent and auxiliary corruptions, every
detached metadata word, detached payload/pointer/auxiliary changes, malformed
source literals and the explicit 65-key writer boundary must refuse.

Files: `Ep1GroupAudioClientTokenizer.cs`, `Ep1GroupAudioClientTokenSmokeTest.cs`,
the inspector command `ep1-group-audio-client-token-self-test`, and its dedicated
Focused batch group.

## Final execution evidence

Build succeeded with zero errors and 111 existing warnings. The standalone
client test passed: **12 author cases, 15 native refusals, 53 detached-output
refusals and 8 source/boundary refusals**. Focused regression then passed
**57/57** groups in **289.857 seconds**; the client group took **8.673 seconds**.
All eight frozen runtime file hashes remained unchanged. Local evidence:
`artifacts/Ep1GroupAudioClientTokens-a/batch-report.json` and the dedicated client
log. No Full run, game launch, installation mutation or alpha expansion.
