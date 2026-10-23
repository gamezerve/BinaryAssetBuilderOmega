# AIUpdate optional chooser authoring

<!-- Reborn: document source-shaped AI leaf ownership without claiming every AI family or list topology is supported. -->
`AIUpdate` no longer has to retain Conscript's `UnitAITargetChooserData` child.
The complete GameObject path supports an absent chooser, an authored empty
chooser using schema defaults, or a chooser with edited implemented attributes.
Its optional allocation and strong `TargetingCompareList` occurrence follow the
authored source rather than a stock occurrence index.

The existing production AI writer is unchanged. A new independent contract uses
typed views of its 76-byte AI record, 36-byte chooser base and 40-byte concrete
chooser. Native values, flags, defaults, strings, padding, relocations and imports
are checked against effective source XML. Metadata grammar is qualified against
stock Conscript (chooser present) and SovietRefinery (chooser absent), and each
generated fragment is checked again inside the complete GameObject.

Supported chooser attributes include scan/reacquisition/startup timing, sympathy
range, dynamic-target flags, vision/rotation/chase booleans and the typed compare
list reference. AIUpdate's supported scalar fields, state machine, auto-acquire
mask and optional AttackPriority text compose with these changes.

This does **not** implement populated `ObjectVisionRange` records or
`SpecialContactPoints` item metadata. Those paths remain explicit unsupported
errors. AI module count changes and other AI families are separate work; the
current route retains its existing container topology.

## Evidence and remaining boundary

The original Conscript complete stream/package regression passed under
`alpha/.work/ai-chooser-original-20261023-a`, including 5 native and 10 metadata
corruption controls. The `chooser` regression group exercises original refinery
AI XML, an empty chooser and an edited chooser/text/mask/state graph. All three
complete stream/package cases passed, rejecting 92 native and 78 metadata
corruptions; the report is under `alpha/.work/ai-chooser-batch-20261023-b`.
The combined expanded SkirmishAI root regression also passed (40 native/21
metadata faults) under `alpha/.work/ai-chooser-skirmish-20261023-b`.

Preparation-only inspection of user Reborn passes the former chooser mismatch.
The next observed mismatch is
`Soviet/structures/SovietRefinery.xml(831,4)`,
`AudioArrayVoice/AudioEntry`: authored count 3 versus baseline 9. This historical
boundary has since been removed by the [root voice/EVA contract](RA3EP1_ROOT_VOICE_EVA.md).
The next observed mismatch is inherited CrusherInfo/CrushableLevel. Full refinery
compilation, publication and gameplay have not been verified. User mod XML,
published mod files and game installations were not changed by these diagnostics.

Implementation: `AIUpdateContract.cs`, `ConscriptVariableBehaviorSource.cs` and
`ConscriptVariableGraphVerifier.cs`. Use the same `alpha/EALAModStudio.exe`
workflow after the synchronized toolchain update.
