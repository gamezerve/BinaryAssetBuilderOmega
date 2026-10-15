# GameObject root/container/destruction composition projection

## What changed

`Ep1GameObjectDestructionProjection.Compose` composes the generated destruction
fragment into a **partial root/container token graph**. The generated graph joins
BuildTime, Behaviors, the single behavior marker, concrete destruction wrapper,
base/DieMux metadata and variable Item field tables in one asset-relative address
space. It preserves the complete supplied native dispatch bytes and reference
indices; all actual native pointers and RELO/IMP slot locations shift once.

This is not a complete GameObject token writer. Exactly **two root fields** are
represented, not all 126 observed root fields. Other native root bytes are
preserved but unrepresented/unvalidated by this projection. It cannot be declared
a playable or tokenized GameObject asset, and is not registered as an SDK asset
processor. No manifest, BIG, game install, launch or alpha capability is added.

## Ownership contract

Input is canonical native GameObject dispatch output with one behavior:

- Root: 600 bytes; root Behaviors pointer at +476 points to +600.
- Behavior list: count=1 at +600, item pointer at +604 points to +608.
- The item at +608 points to a destruction native module at +612.
- Parent relocation ownership is exactly +476/+604/+608. Other root pointer
  allocations or root imports are refused, not silently dropped.
- The module subgraph must satisfy `Ep1DestructionTokenizer`'s explicit contract.
  Only its own pointer/slot coordinates are converted before fragment generation.

Output token prefix: a 36-byte two-field root table including terminator, a
12-byte Behaviors count header, one 12-byte item marker, then the generated module
prefix. Total is `372 + 72 * ItemCount` bytes (444 for the selected singleton).
Root BuildTime uses observed type 645864F3; Behaviors uses observed type 24FCCAE2.

Fragment-internal token targets shift by 60. Fragment-native targets instead bind
the preserved module at `outputNativeRoot + 612`; treating these two target
classes identically would produce wrong pointers. Metadata is not added to RELO.
Native RELO and IMP coordinates retain original ordering and dependency indices.

## Tests and evidence boundary

`GameObjectDestructionProjectionSmokeTest` independently walks the generated root
fields, list header/marker, native root/list/item pointers and concrete module
identity. It compares root type tags and every nested module token row against
stock semantic coordinates. Native round-trip inversion must recover every
original input byte and auxiliary slot, including the unchanged import indices.
Deterministic repeats and an authored BuildTime=12.5 edit are checked.

The same composition path runs for existing authored 1/2/3/256-item cases,
nullable bone strings, absent imports and repeated named Core references.
Non-singleton/null-string shapes remain authored evidence, not independently
stock-corroborated or gameplay acceptance.

Nine refusal controls cover wrong parent/list/item pointers/count/class, bad
module list storage, extra root relocation ownership, a module pointer escaping
into root storage, and a corrupted generated token marker target. Existing module
writer and inherited-token refusal controls still run in the shared Focused batch.

Implementation:
`source/BinaryAssetBuilder.XmlCompiler/BinaryAssetBuilder/XmlCompiler/Ep1GameObjectDestructionProjection.cs`.
Independent checks:
`source/BinaryAssetBuilder.ManifestInspector/GameObjectDestructionProjectionSmokeTest.cs`.

Next gates: explicit supported root-field admission/token coverage, other required
module/child writers, shared source/dependency ownership and full GameObject
tokenized stream validation before exposing a production processor or alpha build.

## Final regression

`artifacts/GameObjectDestructionProjection-a/batch-report.json`: **30/30 Focused
groups pass** in **81.998 seconds**. Allied/native/authored checks take **6.459
seconds**, source/fragment/projection checks **3.403 seconds**. Selected composed
BIN/RELO/IMP is **1168/24/8**, including a 444-byte partial token prefix.
The batch incremental build takes 1.17 seconds with zero warnings/errors. Eight
runtime hashes stay unchanged during workers; no game runs or installation files
change. No new Full-suite, complete GameObject output or gameplay acceptance is
claimed. The alpha package is unchanged.
