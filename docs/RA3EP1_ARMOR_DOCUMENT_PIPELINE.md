# Focused EP1 armor document pipeline proof — 2026-10-01

## Result

The experimental armor profile is now tested beyond a hand-created declaration.
The test uses the real SessionCache, SchemaSet and DocumentProcessor document
entry point to load AssetDeclaration XML, collect instances, validate schemas,
insert defaults, normalize type identities and compute processor/document
hashes. The resulting declarations are then passed to the real plugin entry
point. Empty armor produces 128/0/0; the populated eleven-entry declaration
matches the previously golden-tested 744/8/0 output exactly.

This is **not** a production stream build. GenerateOutput=false is deliberate;
the test invokes the plugin after normal document validation. It does not
enable OutputManager emission, asset-commit/linking, SDK packaging or game loading.

## Findings and fixes

1. ProcessDocumentInternal could return through StreamHints/precompiled or
   Complete-document fast paths before reaching OutputManager's policy check.
   It now validates production eligibility at entry, before cache/file access,
   whenever GenerateOutput=true. Tests with a missing source and null Cache
   prove that denial comes from profile policy, not a later file/cache failure.
2. Network binary cache eligibility did not govern session/precompiled document
   reuse. IAssetBuilderOutputPolicy now has a separate
   CanReuseCompiledDocuments decision. PluginRegistry checks default and mapped
   processors. The experimental armor profile denies it.
3. Experimental OpenDocument always constructs a fresh source-backed document,
   rather than carrying old instances, includes or defines through a shallow/
   in-place reload. Stack-based circular-inclusion checking remains active.
   Precompiled inclusion/reference fast paths are disabled without mutating
   caller-owned ProcessOptions. Session state can still be stored for bookkeeping;
   it is not trusted as a compiled declaration on the next experimental open.
4. Core ValidateInstances injects a decimal TypeId attribute after schema
   validation on both root and nested records. The earlier direct plugin test
   did not expose this, and the profile rejected valid full-pipeline XML.
   CheckAttributes now accepts this internal field only when SchemaInfo has a
   named validated type and the numeric value equals its production FastHash.
   Tampered TypeId values are rejected. Unknown user attributes remain rejected.
5. DocumentProcessor versions are bumped to 18 (v5) / 19 (other targets) to
   invalidate cached declarations predating this reuse policy. The existing
   session-cache regression rejects the previous revision.

## Regression harness

`tests/fixtures/ArmorDocumentPipeline.xsd` includes the official EP1
Includes/Base.xsd, Base/AssetBase.xsd, Includes/Ref.xsd, Includes/DamageDef.xsd
and AssetTypeArmorTemplate.xsd. Unlike the earlier tiny native harness, it does
not stub BaseInheritableAsset or Percentage. Its top-level AssetDeclaration
container is deliberately focused on ArmorTemplate, not the complete SDK root.

`Ep1ArmorDocumentSmokeTest.Run` creates only isolated XML fixtures. It processes
the source, poisons the in-memory cached Default value, and processes the
unchanged file again. The fresh declaration restores the schema default and
produces identical bytes, proving the experimental open did not trust stale
completed session state. The populated test additionally checks core-generated
TypeIds on nested Armor entries and rejects a tampered root TypeId.

The harness supplies DataPaths explicitly, matching the processed-settings
contract expected by FileNameResolver. It uses an empty string-hash-bin list:
this armor test has no string-hash fields. It does not validate string-hash
stream emission or expression evaluator deployment. Settings.Current is restored
in finally; game XML and game binaries are untouched.

```powershell
# Reborn: test focused document stages and reuse guards using isolated artifact XML files.
& .\source\BinaryAssetBuilder.ManifestInspector\bin\x86\Release\net8.0\BinaryAssetBuilder.ManifestInspector.exe ep1-armor-document-self-test `
  artifacts/ep1-armor-document
```

The compiler self-test now invokes 51 groups. Model/marshaller inventory remains
784/760. The previous independent static/WorldBuilder golden comparisons remain
separate tests; this block does not read an entire game BIN.

## Still required

- Justify and implement a narrow production output policy, then run actual
  asset commit, dependency ordering, manifest writing and stream linking.
- Establish production checksum/InstanceHash semantics and review tokenized
  Utility.Manifest.IsEquivalent/cache reuse. This block avoids experimental
  reuse; it does not claim a general fix for all legacy cache rules.
- Test include/override/inheritance and multi-stream source graphs. The focused
  schema harness does not cover these or the complete 843-file EP1 schema graph.
- Prove reversible packaging and actual Uprising loading before calling the SDK
  usable. Keep stock game files untouched.

Overall estimate remains approximately 50% complete / 50% remaining. This is a
focused document-stage and safety proof, not completion of the runtime gates.
