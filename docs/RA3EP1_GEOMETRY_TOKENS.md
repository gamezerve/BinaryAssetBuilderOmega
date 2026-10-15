# Generated EP1 geometry metadata

October 15, 2026. `Ep1GeometryTokenizer` now generates a complete bounded standalone
Geometry token/native fragment after the [native port](RA3EP1_GEOMETRY_NATIVE.md).
This does not yet join Geometry into the partial GameObject composer or publish
a complete object. The local alpha is unchanged.

## Metadata contract

The five Geometry fields are IsSmall, RotationAnchorOffset, PlacementShape, Shape
and ContactPoint, followed by a zero terminator. Rotation/shape Offset and contact
Pos point directly at fixed-size native vectors, not additional field tables.
PlacementShape points at an optional eleven-field/terminator shape table. Shape
and ContactPoint use count headers and inline-element markers. A contact table
contains Label/Pos and its terminator; a shape table contains all eleven official
fields and its terminator. String tokens point at native length/pointer headers.

For P=0/1 placement, S collision shapes and C contacts, the generated prefix is
`96 + 144*P + 156*S + 48*C` bytes. The writer emits no concrete class wrapper for
these non-polymorphic geometry records. Native bytes follow the prefix; native
pointer values and relocation slots are rebased once. Geometry imports no assets.
No captured metadata prefix is copied into the writer.

## Admission and negative controls

The bounded profile admits 1..32 collision shapes, 0..128 contacts and optional
placement/rotation. It checks complete owner bounds, exact relocation membership,
finite scalar/vector values, enum/flag widths and zero native padding. Structs and
vectors cannot alias. Exactly identical string spans may be interned; other
overlaps fail. Strings currently require non-NUL ASCII up to 4096 bytes plus a
terminator, with absent and allocated-empty forms handled separately. Only up to
three zero alignment bytes may remain between recognized allocations: hidden
tails, orphan data and unreviewed imports fail. These limits are diagnostic profile
bounds, not claims about engine maximums or general Unicode support.

Thirty-three native refusal controls cover bad counts, aliases, pointers, enum/
flag/scalar payloads, length/terminator/encoding errors, missing/extra/duplicate/
malformed relocation entries, imports, truncation and orphan tails. Forty detached
metadata/payload corruptions fail an independent verifier without re-running the
producer. Explicit empty strings and the 128-contact boundary are positive cases.

## Independent stock proof

`ep1-geometry-token-self-test <static>` retains all five selected native stock
comparisons, 24 authored shape/contact/rotation/placement combinations and omitted
defaults from the native gate. A field/vector owner map is collected by that
independent native comparison. Every captured metadata target is then normalized
through its proven owner, or through a bounded metadata-relative address. All
generated prefix bytes must equal the resulting stock prefix, including zero
terminators, empty lists and nullable targets.

Thus AlliedConstructionYard, Conscript, JapanFortressShip, AlliedPowerPlantTurbine
and NanoSwarmHiveEffect establish full selected metadata equality as well as
complete native-record/string/vector equality. This is not whole-object equality
or proof of arbitrary original-source inheritance/default preprocessing.

Files: `Ep1GeometryTokenizer.cs`, `Ep1GeometryTokenSmokeTest.cs`, the optional token
mode in `Ep1GeometryNativeSmokeTest.cs`, inspector command and Focused batch recipe.
Next gate: root/body/behavior/geometry native ownership and metadata composition,
then the remaining object containers and complete stream/publication validation.

## Final regression

`artifacts/Ep1GeometryTokens-a/batch-report.json`: **45/45 Focused groups pass in
121.457 seconds**; geometry token group 3.743 seconds. Eight runtime hashes remain
unchanged; GameExecuted and InstalledFilesModified are false. The batch build
succeeds in 1.18 seconds with zero warnings/errors. No Full-suite or gameplay run.
