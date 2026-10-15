# GameObject root, destruction and EP1 ActiveBody composition

October 15, 2026. The previously standalone EP1 body fragment now joins the
root/destruction projection in **one token/native/RELO/IMP coordinate system**.
This closes the selected body ownership/composition gate, not the full GameObject
processor or SDK publication gate.

## Native selection and ownership

`Marshaler.MarshalEp1ActiveBody` explicitly allocates the separate 76-byte EP1
model into a BodyModuleData pointer slot. It requires the reviewed concrete ID
1510882715 (5A0E3D9B). The native wire ID is explicit: hashing the C# type name
Ep1ActiveBodyModuleData would produce the wrong ID. Legacy polymorphic body
dispatch, legacy KW models and derived bodies are unchanged.

The diagnostic source adapter runs existing native GameObject/root/behavior
dispatch without its legacy Body element, then calls this explicit allocator on
the same Tracker and actual `root->Body` slot. It does not append hand-made native
bytes or generate a second reference table. This is an explicit diagnostic path,
not a newly enabled general public XML/document build processor.

`Ep1GameObjectStringTokens.Inspect` now admits an optional Body pointer at root
offset 488. A present Body must own an aligned tail after the single destruction
module; the root relocation list must include 488 exactly once. Existing string
allocation and root strong-import checks remain in force.

`Ep1GameObjectDestructionProjection.Compose` partitions native owners:

- Root/string/behavior envelope, followed by the destruction native graph.
- Optional final EP1 body graph, including its canonical range children.

The destruction and body tails are independently extracted/rebased and passed to
their existing checked token writers. Relocation pointers cannot cross owners;
strong references retain the original shared occurrence indices. Auxiliary slots
outside the entire native buffer fail before range partitioning, so filtering
cannot silently hide an out-of-owner import.

## Metadata layout

Without Body, the old **87-field** projection remains unchanged. With Body, one
indirect root token is added, giving **88 fields including Behaviors and Body**.
The stock-audited Body root token type is AFAD56F4, and its target is a concrete
wrapper table, not the native root's pointer word.

The token prefix contains the root table, behavior envelope, destruction fragment
and body fragment. Each fragment's internal table targets and native field targets
have different origins. Native pointer values and RELO/IMP slot locations shift
once by the total prefix; reference indices never shift. The complete original
native graph is retained after the token prefix.

## Batched evidence

`gameobject-body-projection-self-test <static.manifest> <global.manifest>` adds
an independent Focused group (now 33):

- Eight cases: 0/1/3/64 body range children, each with absent or 300-character
  EditorName root string allocations.
- Raw complete-container XML validates against the reviewed full schema candidate
  before the focused actual-Core reference stage. Shield/hook qualifications
  remain unchanged; this is not unmodified full-schema admission.
- One actual Core occurrence table covers two root names (icon/intro), destruction
  FX, four body parent references and repeated modifier children. All concrete
  type/instance identities must uniquely exist in validated static/global metadata.
- Native root/behavior dispatch and the explicit EP1 body allocator share one
  Tracker. The original indices survive composition and full native/RELO/IMP
  inverse shifting. Repeated builds compare all three composed buffers.
- Independent root/body/behavior/container/base/child token target checks, plus
  the Body root type/native-pointer contract from stock Allied Construction Yard.
- Eleven native owner faults, including aliases, missing RELO/import owners,
  cross-fragment pointers, invalid counts and out-of-buffer imports.
- 188 detached composed metadata corruptions, including all flat root fields,
  Body wrapper/base/list links and child field types/targets.

The earlier standalone body gate retains its independent complete selected stock
native/metadata comparisons. This milestone does not claim full combined source
equality with a stock object: the composed source is intentionally a partial root,
one destruction module and one ActiveBody.

## Files and remaining gates

- `Ep1GameObjectDestructionProjection.Compose`: disjoint native owners and joined metadata.
- `Ep1GameObjectStringTokens.Inspect`: optional Body pointer/RELO ownership.
- `Marshaler.Ep1ActiveBody.cs`: explicit ABI selector/allocator.
- `GameObjectBodyProjectionSmokeTest.cs`: independent combined source/native/output checks.
- `tests/fixtures/RootBodyPipeline.xsd`: official field/reference types and inheritance-only targets.

Other body classes, nonempty GrabObject storage, additional root/child graphs,
multiple general behavior/container graphs and generic source/default/target
closure remain open. ArmorSet/WeaponSet and other consumer graphs still need
integration before a complete new user-ID GameObject stream/package can be tested.
No alpha registry expansion, manifest/BIG publication, gameplay or installation
change is claimed by this composed projection.

## Final regression

`artifacts/GameObjectBodyComposition-b/batch-report.json`: **33/33 Focused groups
pass in 90.472 seconds**, including the final out-of-buffer auxiliary guard and
eleven owner refusal cases. The combined group takes 3.617 seconds. All eight
runtime hashes remain unchanged during workers; GameExecuted and
InstalledFilesModified are false.

The final batch build succeeds in 40.20 seconds with two pre-existing nullable
annotation warnings in audio files and zero errors. The prior initial build had
the broader pre-existing warning set. No new Full-suite run or gameplay is claimed.
The earlier `-a` report predates the final outer-bound guard and is not the final
evidence reference for this commit.
