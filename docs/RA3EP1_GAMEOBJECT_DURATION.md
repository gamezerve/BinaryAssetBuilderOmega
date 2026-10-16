# GameObject timeout duration default and metadata

October 15, 2026. The remaining flat root scalar,
EvaEventSecondDamageFarFromFirstTimeoutMS at **+396**, uses token type
**6F6D1AE7** (DurationUnsignedInt), not float Time or raw unsigned milliseconds.
The official source default is **31000** milliseconds. Both independently read
AlliedConstructionYard and Conscript native words are **155 logic frames**.

The existing generic Duration conversion already performs the matching
ceiling-to-frames conversion at five logic frames per second; it is **not
changed**. The GameObject-specific fallback was incorrectly 0 and is corrected
to 31000. A new four-byte scalar descriptor emits the actual duration type/native
target. The flat set becomes **90 descriptors** (63 scalar, 18 extra, six inline
ASCII headers, three strong slots); the largest selected shared root reaches
**101 fields**. The native root remains 600 bytes with no additional allocations,
relocation slots or imports for this field.

## Source and native evidence

`gameobject-duration-self-test <static> <global>` independently checks the two
stock primitive tags/native addresses and values. A focused fixture's default,
type and use constraints must match the reviewed official GameObject declaration.
Its source attributes are unqualified even though named schema types belong to
the asset namespace; this distinction is checked at the schema lookup seam.

Ten full selected AI/root source cases cover omitted/default, explicit 0, 1,
199, 200, 201, 1000, 31000, 1000000 and uint.MaxValue milliseconds. Expected
native frames are 155, 0, 1, 1, 1, 2, 5, 155, 5000 and 21474836 respectively.
Core default insertion is checked explicitly. Bypassing it at this attribute
also produces the independently stock-matching 155-frame native fallback.

The maximum-input case pins the **current float32 conversion policy**, not
independent EA extreme-value parity. Float32 multiplication gives 21474836 there,
where ideal real-number ceiling would differ by one. The selected stock evidence
establishes the normal default, not every possible duration rounding edge.

All prior shared AI, local module-id scope, armor/movement/body/behavior/geometry/
vision/pointed-root metadata and reference checks stay active. Repeated native,
relocation and import buffers match. Eleven refusals cover negative/overflowing,
fractional/suffixed/non-numeric/unresolved literals, attempted duration relocation
or import slots, detached metadata type/address corruption and an incorrect
raw-millisecond native output word.

Source values in this profile are whole-number milliseconds without a unit
suffix, matching the existing uint-based native duration converter. This is not
a general expression/default-preparation or arbitrary lexical DurationTag port.
The six malformed literals are rejected rather than silently using the default.

## Files and boundaries

- `Marshaler.GameObject.cs`: only this field's fallback changes.
- `Ep1GameObjectScalarTokens.cs`: stock-pinned descriptor.
- `GameObjectScalarTokenSmokeTest.cs`: the existing all-scalar value sets now
  independently distinguish 201ms/31000ms as 2/155 frames.
- `GameObjectDurationSmokeTest.cs`, `RootDurationPipeline.xsd`: dedicated actual
  Core/default/full selected-graph and refusal tests.
- Focused adapter and count messages retain the attribute/default and derive
  current root counts from descriptors rather than stale constants.

This is not complete GameObject stream publication, alpha admission or gameplay
proof of the EVA timeout behavior. No game or installation files are changed.
The other 25 root fields are structured/list/container work, not additional
unreviewed flat primitive fields in the selected 126-field stock inventory.

## Final regression

Focused regression passed **56/56** groups in **368.721 seconds**;
`gameobject-duration` passed in **10.010 seconds**. All eight frozen runtime
file hashes remained unchanged. Evidence: local
`artifacts/GameObjectDuration-a/batch-report.json`. No Full run, game launch,
installation mutation or alpha admission change is claimed.
