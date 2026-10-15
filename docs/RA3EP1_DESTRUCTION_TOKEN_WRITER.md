# Destruction module-fragment token writer

## Implementation advance

`Ep1DestructionTokenizer.Tokenize` now **generates** a token prefix from reviewed
descriptors and a canonical native destruction module. It does not copy stock
prefix bytes or inspect a stock file. The output contains concrete wrapper,
Die/Behavior/Module inheritance, TypeId/id, inline DieMux fields, the empty
veterancy count header, Item list markers and a field table for each item.

This is a reusable compiler-side fragment writer, not just another stock reader.
It remains explicitly outside the asset registry and general GameObject/plugin
publication path. A fragment is not a loadable GameObject asset by itself.
No manifest, BIG, alpha package or runtime acceptance is claimed here.

## Contract

- Win32 little-endian native module with concrete ID 0D96A2F4 and 72-byte root.
- 1–256 ordered contiguous 20-byte items starting at native +72.
- Empty VeterancyLevels, absent ExemptStatus and RequiredStatus only; other
  optional layouts fail rather than receiving incomplete token metadata.
- Nullable or bounded null-terminated bone strings, weak object/module/hash
  identities and FX import words remain native data; the writer does not invent
  dependency identities or evaluate source semantics.
- Exact native RELO/IMP slot membership, sentinel and string pointer checks
  precede rebasing. Native pointer values and auxiliary slot locations shift by
  the prefix size; token metadata is never inserted into RELO.
- Token prefix size is `312 + 72 * itemCount`; the selected singleton is 384 bytes.
  Empty table terminators are generated as zero words, not copied placeholders.

The native graph and import words are preserved, including Core's separate
reference occurrences. Production dependency closure/linking remains separate.

## Independent validation

`DestructionTokenWriterSmokeTest.Stock` extracts the module's own native graph
from real GameObject polymorphic dispatch after actual Core FX/default
normalization. It removes only the checked parent envelope, not arbitrary stock
data. The new writer receives this standalone chunk.

The generated singleton prefix is compared row-for-row with the independently
captured stock module after token/native address rebasing. The stock prefix is
used only as an expected result in the inspector, never an input to the writer.
Native round-trip checks undo only validated pointer shifts and require exact
original bytes and corresponding auxiliary slots. Repeated emission must be
deterministic. Authored multi-item paths check count-dependent markers/field
targets, strings, ordered repeated named FX targets and absent imports. Additional
authored controls exercise one null-string/no-import item, the 256-item limit and
explicit rejection at 257. Non-singleton/null-string token shapes are authored
evidence, not independently stock-corroborated layouts or gameplay results.

Nine malformed-input controls cover class identity, unsupported empty/absent
storage, noncanonical item pointers, bad string pointers and invalid RELO/IMP
streams. Existing stock token corruption controls remain in the same regression.

## Files and next gate

- Writer: `source/BinaryAssetBuilder.XmlCompiler/BinaryAssetBuilder/XmlCompiler/Ep1DestructionTokenizer.cs`
- Independent evidence: `source/BinaryAssetBuilder.ManifestInspector/DestructionTokenWriterSmokeTest.cs`
- Actual Core source invocation: `Ep1DestructionSourceSmokeTest.Run`
- Authored no-import invocation: `Ep1AlliedModuleSmokeTest.Run`

Run the existing Focused batch. The next integration gate is composing fragments
inside a generated GameObject root/container token graph with shared reference
and relocation ownership. Present optional/list cases and other module families
still require their own writers/contracts; alpha capabilities are unchanged.

## Final regression

`artifacts/DestructionWriter-a/batch-report.json`: **30/30 Focused groups pass** in
**109.505 seconds**. Allied/native/authoring cases take **8.046 seconds**, actual
Core/source/writer cases **4.606 seconds**. The selected generated fragment is
BIN/RELO/IMP **496/12/8**, including its 384-byte prefix. Nine malformed native
inputs and the 257-item boundary are refused; prior 40 inherited/inline corruption
controls remain passing. The batch incremental build takes 8.98 seconds with
zero warnings/errors. Eight runtime hashes remain unchanged during workers;
no game runs or installation changes occur. No new Full-suite result is claimed.
