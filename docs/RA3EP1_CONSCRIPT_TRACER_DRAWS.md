# Complete selected Conscript tracer Draw graphs

## Target and boundaries

The acceptance target remains the **complete new-ID Conscript**, as requested by
the user. This implementation batch addresses both existing tracer modules,
`ModuleTag_TracerModelDraw` and `ModuleTag_TracerModelDrawVeterancy`; it does not
replace that target with a reduced unit. Both scripted draws, including the
parachute include, other containers, remaining roots/behaviors and complete Core
publication still need their own integration gates.

`W3DTracerDrawModuleData` already had a native model/marshaler. The new gate uses
the real Draw polymorphic dispatcher, actual Core schema/default/reference
normalization and named stock texture/FX targets, rather than captured input
import indices. The new metadata writer is `Ep1TracerDrawTokenizer`.

## Wire contract recovered from stock

The root is 112 bytes: module identity at 0/4, ten real words at 8..44, weapon
slot/id at 48/52, texture/hit-FX/emit-FX references at 56/60/64, texture index at
68, inline HeadColor/TailColor at 72/88, optional status pointer at 104 and
UseAdditiveBlending at 108. Bytes 109..111 are zero padding.

The optional status block is 64 bytes: two 230-bit ObjectStatus masks, each in
eight uint spans. It starts at 112 in the canonical selected allocation. The
unused top 26 bits of each last span must be zero. This is not a pointer to a
pair of separately allocated masks.

Metadata consists of the wrapper, own fields and Draw/Module base tables through
offset 336; two 60-byte component tables describe the inline colors; an optional
36-byte status table starts at 456. Prefix size is 456 without status and 492
with it. Both selected stock tracers have a 176-byte complete native graph and
492-byte complete metadata prefix. Color type is `16ADF350`; status type is
`3E8F938C`; its mask fields use `A7BD811E`. Texture uses `BDB63F16`; FX uses
`0D1C8E83`. Native pointers, metadata targets and import occurrences are distinct
coordinate/ownership domains.

Source `FrameLifeTime` and other tracer real fields retain their existing real
conversion. Do not reuse the separate GameObject millisecond timeout converter
here. The type remains `54018E60`, with Draw base `17F61728` and Module base
`D75D93A2`.

## Validation design

The focused schema imports the official tracer declaration and shared color,
status and weapon-slot declarations. The small Draw/Module/status base surfaces
are drift-checked against the reviewed official candidate. The Texture stub
provides only reference inheritance, not texture-file compilation.

The two official-shaped source leaves are independently compared with complete
stock native bytes and all metadata words. Only explicit status pointers and
named import occurrence indices are canonicalized. Each import must identify
the same stock type/instance pair. A separate literal-hash expected table checks
every generated metadata byte and the inverse native graph/auxiliary streams.

Author cases combine all six weapon-slot enums, optional status absence/presence,
both blend flags, changed real/signed fields and repeated named FX imports.
Malformed native identity/enum/flags/padding/finite values/status bits/auxiliary
ownership, every detached metadata word, detached payload/import changes,
missing/ambiguous texture dependencies and unknown Draw dispatch are refused.

The legacy Draw dispatcher no longer silently omits unknown TypeIds: it throws
an explicit diagnostic error. Existing supported dispatch cases are unchanged.

Next scripted-draw batch should reuse and verify the existing Core
`XIncludingReaderWrapper` / `FileNameXmlResolver` path for
`DATA:Includes/ParachuteDrawSoviet.xml` and its namespaced XPointer, rather than
silently dropping the include or assuming an instance-Include test covers it.
The wrapper depends on `Mvp.Xml` and reports XInclude disabled if it cannot load
the reader; that runtime/source-closure behavior still needs a dedicated check.

## Final execution evidence

The final incremental inspector build succeeded in **8.77 seconds**, with zero
warnings and zero errors. Four newly exposed nullable warnings were corrected
with explicit schema-type guards before this final build; the earlier rebuilding
solution also reported the existing 111 warnings. The final source additionally
asserts exact edited width/sweep/lifetime/head/tail native words, not merely
repeatability.

Focused regression passed **58/58** groups in **368.540 seconds**; the tracer
group passed in **14.725 seconds**. Both complete stock graphs, **24** author
cases, **30** native refusals, **128** detached-output refusals and **4**
dependency/schema/unknown-dispatch refusals pass. All eight frozen runtime file
hashes stayed unchanged. Local evidence:
`artifacts/ConscriptTracerDraws-a/batch-report.json` and `ep1-tracer-draw.log`.
No Full run, game launch, installation mutation, alpha expansion or complete
Conscript publication is claimed by this diagnostic gate.
