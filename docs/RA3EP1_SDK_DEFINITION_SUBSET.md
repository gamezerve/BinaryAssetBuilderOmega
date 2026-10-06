# Bounded GlobalDefines expression subset — October 6, 2026

## Outcome

The explicit `diagnostic-definition-subset-v1` profile resolves the five expression
definitions observed in real Uprising GlobalData/GlobalDefines.xml: two aliases,
one single-quoted suffix concatenation and two integer multiply-add computations.
The computed repair-drone decal sizes are 440 and 640. Both PlayerTemplates source
documents now validate against the reviewed diagnostic schema, after nine base
and two EP1 asset substitutions. Reference source/schema files remain unchanged.

The real global.xml graph has 396 documents / 664 Include edges:

| Profile | Validated | RequiresPreprocessing | SchemaInvalid |
| --- | ---: | ---: | ---: |
| Raw default | 197 | 194 | 5 |
| Local literals | 198 | 198 | 0 |
| Include literals | 200 | 196 | 0 |
| Definition subset | 202 | 194 | 0 |

The new result resolves all five previously raw-schema-failing documents in this
reachable slice. It does **not** prove the rest of the corpus is preprocessed:
194 inheritFrom documents remain withheld. Six path issues and one typed AUDIO
header still prevent scoped completion. No limit was hit, exit is 2 and output
stays absent. There are 552 asset substitutions across five documents; the five
GlobalDefines computations are evidenced separately in each consuming closure,
not counted as extra asset substitutions or compiled assets.

Processed diagnostic XML SHA-256:

- System/PlayerTemplates.xml: `3656D37E87350325F8DB408BEEDDD9F959AB55F5098C56AFBF23F70EC5D6AF6A`
- EP1/System/PlayerTemplates_EP1.xml: `BDF94B81B033DE0B43C633C9F42054DB75D1B5393F99760F6E76E7B035F56FAA`

Original hashes, definition-source fingerprints, origin paths and each expression's
original/evaluated value remain separate in JSON. These serialized XML hashes are
not native type IDs, compiler cache keys or stream hashes.

## Command and admitted grammar

```text
BinaryAssetBuilder.ManifestInspector.exe sdk-typed-source-graph ra3ep1 <absolute-schema-root> <absolute-source-root> <absolute-entry.xml> <absolute-new-output-directory> --definition-expressions [--art-root absolute-directory] [--audio-root absolute-directory] [absolute.manifest=runtime.manifest ...]
BinaryAssetBuilder.ManifestInspector.exe sdk-definition-subset-self-test
```

The flag is mutually exclusive with local/Include literal flags and is rejected
on the path-only command. Raw and both earlier profiles keep their previous gates.
Definition expressions admit only these forms, with bounded optional whitespace:

- `=$NAME`: backward-visible alias, preserving literal text.
- `=$NAME+'suffix'`: a single unescaped, single-quoted suffix, at most 256 characters.
- `=($LEFT * integer) + $RIGHT`: nonnegative invariant integer operands, checked
  signed 64-bit multiplication/addition and canonical invariant integer output.

This is **not** a general parser: no other operator precedence, subtraction,
division, unary signs, floats, unit arithmetic, functions, escaped quotes,
forward/self references or expression execution is admitted. Asset expressions
still permit only exact direct `=$NAME` substitutions; the broader forms are
restricted to definition values. Unsupported definitions, even unused ones in
a needed closure, reject the complete transformation without partial evidence.

## Implementation and limits

`SdkDefinitionSubset.Evaluate` implements the three bounded forms without dynamic
code, reflection execution, scripting or the reference ExpressionEval DLL.
`SdkIncludeDefineProfile.Visit` derives local declaration order directly from XML,
not dictionary enumeration. Each local definition sees imported and earlier local
values. Child-first source visibility, same-origin diamond coalescing, cross-origin
duplicate rejection, source fingerprints and raw Include edge comparisons remain
mandatory. Overrides, reference/precompiled paths and inherited sources stay closed.

The profile is based on observed Uprising source syntax plus schema-valid results;
EA evaluator semantic equivalence is **not established**. Core EvaluateDefinitions
calls the dynamically loaded evaluator on local definitions in order; its internal
typing/coercion, recursion and arithmetic implementation are not available as
source in this checkout. The reference DLL was not executed or used as an oracle.
General evaluator/DLL comparison remains a reverse-engineering/validation gate.

Needed definition closures are evaluated atomically. No-expression source rows
retain their raw bytes and make no complete definition-evaluation claim.
Original Defines/value text is retained in serialized XML; resolved tables and
computed evidence are held separately and used for asset substitutions only.
This diagnostic representation is not asserted to be production compiler input.

The earlier path/source/Include/definition bounds remain: 512 sources, 4 MiB per
source, 32 MiB cached definition bytes, depth 32, 4,096 Include edges and 512 visible
definitions per owner. New limits are 512 expression/output characters, 10 factor
digits, 19 integer operand digits and 2,048 evaluated definitions per needed closure.
Integer overflow and expanded strings above 512 characters fail closed. Physical
resource resolution remains consumer-relative and root-confined after substitution.
Snapshot evidence is not an atomic filesystem transaction or full asset closure.

## Validation and next work

Owned fixtures cover the three positive forms, invariant locale, case-sensitive
lookup, overflow, units, unsupported functions/operators/quotes, forward/self/
missing names, duplicate definitions, unused unsupported arithmetic, atomic
rejection after successful earlier computations, stale imported sources,
definition-origin evidence, old literal-profile isolation and typed/resource graph
binding with unchanged sources and absent output.

All 120 default compiler groups and 33 enum checks pass. The test runner registers
120 groups. Model/marshaller inventory remains
785/1,390 and 762/1,390; all 48 EP1-only complex types retain model/marshaller coverage.
Weighted effort remains about 50.25%, rounded 50% complete / 50% remaining.
This source-preprocessing slice does not close native layout/type-table/processor,
missing AUDIO/ART, stream output, WorldBuilder packaging or actual game-load gates.
Next address inheritFrom with core-compatible instance visibility and override
order, without treating XSD inheritance as asset overlay semantics.
