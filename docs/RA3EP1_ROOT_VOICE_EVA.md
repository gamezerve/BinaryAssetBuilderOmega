# Source-sized GameObject voice and EVA arrays

<!-- Reborn: distinguish source-owned voice/event arrays from unrelated audio resource compilation and unqualified voice families. -->
`AudioArrayVoice/AudioEntry` and `EvaEventArray/EvaEntry` now follow source
cardinality on the complete GameObject route. Each whole owner may be omitted,
authored empty or populated. Presence remains distinct from an allocated-empty
header. The former nine-Conscript-voice equality requirement is removed.

Voice entries retain the twelve-byte EP1 layout: an optional Sound reference
pointee, an inline strong EvaEvent reference and the EP1 voice ordinal. A new
`Ep1SoundOrEvaEvent` model keeps the older KW `SoundOrEvaEvent` model unchanged.
Populated EvaEvent names are no longer written as legacy string pointers.
Both fields use schema/catalog resolution and actual Core reference occurrences.
The same inline EVA reference is also enabled in existing
`ModelConditionSoundSelectorClientBehavior` voice overrides, removing the old
empty-EVA-only tokenizer gate on that path.
The voice tokenizer's old 4096-item check is replaced by the real native item
extent; ordinary allocation and metadata bounds still apply.

The refinery's original selection sound and the two inherited BaseStructure
capture announcements qualify sound/EVA native storage against independently
read stock entries. Root EVA qualification uses stock Conscript. Generated
metadata must match stock grammar, source-bound coordinates and exact auxiliary
ownership before the complete stream/package is accepted.

## Boundaries

Populated `ObjectSpecificEntry` and `NamedEntry` remain unsupported by the root
tokenizer; their concrete EP1 nested families have not been qualified. This
change does not compile new audio files or EvaEvent assets. It references
already resolvable assets and does not enable every audio module/resource type.

Preparation-only inspection of Reborn passes the prior three-versus-nine voice
count failure. The next observed mismatch is inherited
`BaseObjects/BaseSovietStructure.xml(22,4)`,
`CrusherInfo/@CrushableLevel=40` versus the Conscript baseline value 10.
This historical crusher boundary has since been removed by the
[CrusherInfo contract](RA3EP1_CRUSHER_INFO.md), and the complete prepared refinery
now passes offline native/metadata/stream/BIG compilation. Final publication and
runtime behavior are not claimed.
User mod XML, published mods and game files are not modified by diagnostics.

## Developer evidence

The `audio` filter of `conscript-module-graph-self-test` passed four complete
stream/package cases: original refinery selection/capture voices, empty owners,
omitted owners and 18 voice plus 20 EVA entries. It rejected 46 native and 56
metadata corruptions. Evidence: `alpha/.work/root-audio-batch-20261023-d`.
Original Conscript passed (5 native/10 metadata faults) under
`alpha/.work/root-audio-original-20261023-d`. Expanded client/draw composition
passed (21 native/58 metadata faults) under
`alpha/.work/root-audio-clients-20261023-d`. These are offline graph/package
checks, not gameplay evidence or a fresh complete historical regression batch.
The expanded root arrays plus a newly authored client voice override with
`EvaEvent=BuildingStolen` passed together under
`alpha/.work/root-audio-client-eva-20261023-f` (13 native/14 metadata faults,
292 ordered external reference occurrences).

Implementation: `Ep1ObjectSound.cs`, `Marshaler.Ep1ClientBehaviors.cs`,
`Ep1BehaviorMetadataTokenizer.cs`, `Ep1ClientBehaviorsTokenizer.cs`, `Ep1GameObjectRootOwnersProjection.cs` and
`GameObjectExtendedRootContract.cs`.
