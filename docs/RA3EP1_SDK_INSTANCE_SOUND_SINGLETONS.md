# Separately scoped known sound singleton admission

## Outcome (2026-10-08)

Independent `--instance-sound-singletons` closes the last measured XML
preprocessing blocker: **396 Validated / 0 RequiresPreprocessing**, across
396 reachable documents and 664 Include edges. Earlier `--instance-sound-offsets`
still reports **395 / 1**. There are zero earlier-valid regressions.

This is not a usable SDK or complete dependency closure. Six path issues and
198 missing typed dependency occurrences remain; ScopedGraphComplete and
ProductionBuildReady remain false, and the diagnostic command returns **2**.
All raw observed source hashes match captured identities; post-audit finds
zero changed sources. The requested output directory is absent. No reference
XML/XSD/Core implementation changes or native emission/game execution.

## Narrow admitted contract

Only AudioEvent:BuildingInfiltrated1/PitchShift and
AudioEvent:StreetLampCrush/NonInterruptibleTime are eligible. The independently
proved [complete owner review](RA3EP1_SDK_SOUND_OWNER_REVIEW.md) and
[selected native range corroboration](RA3EP1_SDK_SOUND_SINGLETON_SEMANTICS.md)
precede this opt-in. This is not generic last-write-wins or duplicate folding.

After bounded source-local expression preparation, admission requires the exact
reviewed complete literal child body, including Sound references/order,
exactly two occurrences of the known child,
and an empty direct AudioEvent:BaseSoundEffect base without an inherited marker.
Root attribute presence/values may differ only within existing literal schema
and preparation guards; every resulting explicit owner field is predicted and
verified. There are at most two normalization plans per document, 64 root
attributes per reviewed owner/base, 16 direct leaves and 4096 characters per
field/text. The pre-normalization document has at most 16384 elements; earlier
resource/byte/depth/chain/Include/amplification limits still apply.

The owner schema must be an exact named non-wildcard AudioEvent sequence; the
child must be optional maxOccurs=1, named empty non-wildcard RealRange or
TimeRange. Changed cardinality, owner/body/handle, unknown/directive root
attributes, partial pairs, third occurrences, populated bases, nested payloads
and unreviewed references remain closed. Unknown owners and other repeated
children retain the existing cardinality refusal.

`SdkSoundSingletons.Normalize` first predicts the full final owner, probes the
unchanged Core on the original repeated body and compares every explicit field
and child against that independent prediction. Only then are the owner's
direct element children replaced in memory with the proved projection. Original
root attributes, inheritFrom and unrelated document metadata remain intact.
The raw source file is never rewritten. This is preprocessing, not reference
XML cleanup or schema relaxation.

Normal inheritance still runs through the existing guarded Core path. After
temporary imported bases are removed, `SdkSoundSingletons.Verify` checks the
complete actual final owner again. Singleton witnesses publish only with the
whole source result; later source/schema/preparation failure erases processed
hashes, singleton/overlay/import/arithmetic/source witnesses. The profile
composes earlier typed sound arithmetic but does not change older flag defaults.

## Real SoundEffects result

- Raw source SHA-256:
  `F054D586CAAB71C22AEEA42AA2A71CF91568913DA7FC50D5CE844AD191A2073C`
- Final whole-owner-document SHA-256:
  `0C534F9247B64AFDE5283F5A7BC20D328C65C56AFCFE8C71D27E310A1684D18E`
- 1800 overlays; 353 expression substitutions including 276 sound calculations.
- Two source-local normalization witnesses: BuildingInfiltrated1 PitchShift
  2->1 and StreetLampCrush NonInterruptibleTime 2->1.
- Two captured/prepared sources: SoundEffects.xml and BaseSoundEffect.xml.
  Base raw hash `69AF7CD3D9C7DDACA8B394AC7976CF4690BD2501E73716110E650950247D86EC`,
  processed hash `6E7D1FC145729543D9EFEB7B3A6C066A8C4FE656890FD5AC84EAD9490B52D1CD`.
- Repeated graph run preserves the final SoundEffects hash; independent raw
  source post-audit shows no changed captured source and no output creation.

The full final schema/typed binding is performed after preparation. Individual
owner/native range evidence does not establish complete native bytes, stream
layout, all AudioFile identities or physical payload availability.

## Implementation and validation

Paths are relative to `source/BinaryAssetBuilder.ManifestInspector/`:

- `SdkSoundSingletons.cs`: bounded `Normalize` plans and final `Verify`.
- `SdkSoundOwnerReview.cs`: reused independent `Predict`/`Verify` whole projection;
  root-attribute bound applies to review and admission alike.
- `SdkInstanceInheritanceProfile.cs`: separately named sound singleton option,
  expression/base preparation, normalization before delegation and final proof.
- `SdkSelfAttributeInheritance.cs`: atomic singleton witness field only; no
  change to its copy/cardinality semantics or Core implementation.
- `SdkTypedSourceGraph.cs` / `Program.cs`: exclusive option wiring and scope guards.
- `SdkInstanceSoundSingletonsSmokeTest.cs`: compiler group 149 and focused
  `sdk-instance-sound-singletons-self-test`.

Owned fixtures exercise imported/local bases, full fixture graph, final inherited
field tamper detection, old-profile refusal, changed/unknown/extra/partial bodies,
populated base, unknown/directive root attributes, widened singleton schema,
attribute resource bounds, late/stale atomic failure and API exclusivity.
CLI conflict, duplicate option and path-only misuse probes return exit 1.
Coverage stays 785/1390 complex models, 762/1390 typed marshallers and 48/48
EP1-only types. All 149 compiler groups pass; no native codecs in default tests.
Final incremental build has zero warnings and errors; the previously documented
three full-rebuild Core/XmlCompiler warnings are not changed by this milestone.

## Remaining work and estimate

The XML preparation milestone is complete for this captured graph, not the SDK.
Next, classify the six path issues and 198 missing dependency occurrences;
distinguish missing authored headers/resources from stock/precompiled assets
before changing resolver rules or claiming full closure. Type-table/hash coverage,
native layouts/emission, target-aware build scripts, packaging/WorldBuilder and
actual Uprising game loading remain independent gates.

Manual effort reassessment: SDK tooling/dependency workstream rises from 30% to
35% based on accumulated audio expressions, breadth/complexity proof, native-
corroborated owner review and complete measured XML preparation. Its 15% weight
adds 0.75 overall point: 51.0% -> 51.75%, rounded **52% complete / 48% remaining**.
Other workstreams and zero proven game loading are unchanged. This is not
automatic credit per XML/test/commit or a claim of 52% working game mods.
