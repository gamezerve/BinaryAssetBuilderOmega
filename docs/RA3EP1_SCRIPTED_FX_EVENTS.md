# Scripted draw FX events

`ModelConditionState/FXEvent` and `AnimationState/FXEvent` use the existing
28-byte EP1 native writer and a populated metadata list. Authored event count
and order are preserved. Each event retains `Frame`, `FrameStep`, `FrameStop`,
`FireWhenSkipped`, the strong `Effect` FXList reference, and the optional `Bone`
string. Empty event lists keep the existing zero-count representation.

The original `SovietPowerPlantAdvanced` spark events independently qualify the
native fields, semantic imports and six-row event metadata grammar. The generic
source proof owns all authored bytes, allocations, relocations and imports;
the metadata writer cannot silently drop a requested event. No retail event
payload is copied into the mod.

User Reborn sources and published packages remain untouched by private reactor
validation. Gameplay timing and visual effects still require an in-game test.

Validation: six original reactor spark records passed stock native/import and
metadata checks. The complete captured `SovietPowerPlantAdvanced` then compiled
privately with 112 stock reference occurrences and linked BIN/RELO/IMP readback
(stream checksum `763dd00a`). This is one complete reactor asset, not a claim
that the entire 38-asset Reborn mod or its gameplay has been retested.
