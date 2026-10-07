# Choice-particle admission review

Date: 2026-10-07. This milestone characterizes the unchanged core joiner;
it does **not** enable a new preprocessing profile or claim game compatibility.

## Real source and schema evidence

The previous `--instance-removals` audit found 11 first blockers requiring
non-flat particle support, among 396 source documents (374 valid / 22 blocked).
Read-only inspection now identifies the concrete AI opening-move choice path:

`schemas/ra3ep1/xsd/SkirmishAI/AssetTypeAIPersonalityDefinition.xsd`
defines `AIWeightedOpeningMove` with an optional singleton `Heuristic` element
of type `HeuristicChoice`. `OpeningMove` itself is a repeated element with a
`Name` reference attribute, **not an id key**.

`schemas/ra3ep1/xsd/SkirmishAI/AssetTypeAIStateDefinition.xsd`
defines `HeuristicChoice` as a direct `xs:choice minOccurs="1"
maxOccurs="unbounded"` with 29 alternatives. Alternatives include `MapNameHeuristic`,
`PathToTargetHeuristic`, `ObjectOfTypeExistsHeuristic`, and `ConstantHeuristic`.
Individual alternatives have their default `maxOccurs="1"`: this does not
limit how many times the alternative can occur across repeated choice slots.
`AIStateLinearCombinationHeuristic` additionally contains a `WeightedHeuristic`
sequence whose anonymous Heuristic type has a singleton choice. Supporting the
outer repeated choice is not blanket admission of that inner singleton shape.

Source root used:
`D:\OneDrive\CNC Files\CnC_Modding_Support-main (Official XML, Schema, Script, Shader, Maps)\Uprising\Xml (Uprising)`.

| Owner relative to source root | Authored Heuristic blocks |
| --- | ---: |
| SkirmishAI/Personalities/AlliedBalanced.xml | 8 |
| SkirmishAI/Personalities/AlliedSquadronLeader.xml | 7 |
| SkirmishAI/Personalities/AlliedSpecialForces.xml | 7 |
| SkirmishAI/Personalities/SovietBalanced.xml | 8 |
| SkirmishAI/Personalities/SovietShockSpecialist.xml | 8 |
| SkirmishAI/Personalities/SovietAirMarshall.xml | 3 |
| SkirmishAI/Personalities/JapanBalanced.xml | 9 |
| SkirmishAI/Personalities/JapanMechaWarfare.xml | 9 |
| SkirmishAI/Personalities/JapanFleetCommand.xml | 9 |
| SkirmishAI/Personalities/TestPersonalities.xml | 0 |
| EP1/SkirmishAI/Personalities/CommandersChallenge/AIP_CC_29.xml | 1 |

Total: 69 authored blocks in ten owners. `TestPersonalities.xml` directly
includes AlliedBalanced, SovietBalanced, and JapanBalanced and inherits their
assets, explaining why a document with no authored Heuristic block is blocked.
These counts identify review targets, not proof that a future choice profile
will validate all eleven documents; later blockers may emerge.

## Executable core characterization

`source/BinaryAssetBuilder.ManifestInspector/SdkChoiceSemanticsSmokeTest.cs`
is registered in `CompilerSmokeTest.Run`. It uses owned in-memory schemas/XML
and the existing `BinaryAssetBuilder.Core.SageXml.NodeJoiner.Override`.
No official reference file or core joiner is modified.

Verification: all 130 registered compiler groups executed and passed, including
this new characterization group; 33 enum checks and three Include-classifier
fixtures passed. The full real-source graph audit was repeated with
`--instance-removals`: 374 Validated / 22 RequiresPreprocessing, expected exit 2,
and no output directory created. Model/marshaller counts stay 785/1390 and
762/1390. The full suite was run with permission for its existing owned temporary
audio-file move, not permission to mutate external reference assets.

The fixtures pin these distinctions:

- Anonymous repeated choice alternatives preserve repetition and authored order
  when copied to an empty destination. Anonymous repeated OpeningMove branches
  remain separate: Name is not silently promoted to an inheritance key.
- A repeated choice permits multiple occurrences of the same alternative.
  Choice aggregate cardinality is different from alternative cardinality.
  The joiner itself does not enforce the aggregate maximum; final schema
  validation must reject an overflow.
- Repeated-choice siblings sharing an id can match across different QNames.
  The later alternative replaces the earlier one, even while copying just one
  populated side. Final output validation cannot detect the lost source rule.
- Same-QName/id matching appends text (`left` + `right`) while overwriting
  attributes; this is not literal payload replacement.
- A singleton choice can replace an earlier different alternative without any
  id. Invalid source cardinality can thereby become valid-looking merged XML.
- Existing tree-copy, empty-child-merge, and keyed-removal profiles continue to
  refuse choice particles atomically, with no processed hash/overlay/removal
  evidence published on refusal.

Core functions requiring review are `GetObjectType`, `SelectSame`,
`FindInBase`, `AppendCorrespondedXmlNode`, and `ReplaceXmlNode` in
`source/BinaryAssetBuilder.Core/BinaryAssetBuilder/Core/SageXml/NodeJoiner.cs`.
`FindInBase` handles sequences, whereas `GetObjectType` also handles direct
choices. Choice matching uses the **parent particle's** `MaxOccurs`, not each
alternative's `MaxOccurs`.

## Next implementation boundary

A separate explicit profile is needed; existing flags must retain their scope.
The first candidate should admit direct flat repeated choices for one-sided
copy only. Each particle's direct items must be elements, not structural nested
sequence/choice/group/wildcard particles. Existing schema-selected sequence
descendant types can retain their current tree guards; descendant singleton
choices need independent admission and must not ride along implicitly.
It must distinguish choice aggregate occurrence limits from selected-alternative
limits, retain unique literal sibling IDs across QNames, reject malformed
singleton cardinality before core allocation, and preserve current depth/node/
byte/chain/source-closure limits and final schema binding.

Top-level two-sided sequence matching must still reject matched populated
branches. Anonymous repeated OpeningMove additions may be copied, but no new
Name-based identity, nested choice merging, expression evaluation, or directive
scope should be inferred. Unknown alternatives, duplicate IDs, groups,
wildcards, namespace prefixes and expression/directive forms need negative
fixtures. Scope failures must remain atomic.

After that profile is tested, compare it to `--instance-removals` on the full
396-document graph: report newly valid owners, zero prior-valid regressions,
remaining first blockers, source witnesses and unresolved dependencies. Do not
claim an 11-document gain before running this audit. This review alone changes
neither the 374/22 admission counts nor the manual 51% complete / 49% remaining
engineering-effort estimate. Native stream/type-table and in-game validation
remain separate open gates.
