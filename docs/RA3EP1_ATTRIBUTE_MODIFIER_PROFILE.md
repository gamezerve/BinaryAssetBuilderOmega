# Experimental EP1 AttributeModifier profile

Date: October 1, 2026

## Result and policy

An explicit opt-in profile routes the no-dependency native modifier subset through
actual PluginDescriptor, PluginRegistry, ProcessInstance and document stages.
Eight outputs match the [native/golden proof](RA3EP1_ATTRIBUTE_MODIFIER_NATIVE.md).
There is no legacy KW registry fallback, static dispatch cache or implicit
production activation.

`Ra3Ep1AttributeModifierPlugin` exposes exactly one root:

- TypeId `0xC5E07887`, TypeHash `0x74425C11`, Tokenized=false, HasCustomData=false.
- Local ProcessingHash `0x74425C11 ^ 0x45503111`, VersionNumber=1.
- AllTypesHash `0x5454A8E9` identifies target, not registry completeness.
- CanWriteProductionOutput, CanUseBuildCache and CanReuseCompiledDocuments=false.

This profile supports Win32 only and returns detached type information. Failed
unsupported-platform initialization revokes readiness. It rejects unknown roots,
unannotated/unvalidated XML, inheritance, imports, file/weak dependencies, custom
data, unsupported attributes, non-leaf Modifier records and unresolved formulas.
StartFX, EndFX and Shader remain explicitly unsupported even if empty.

No settings flag can override the policy. Default and explicit type mappings are
tested, including a descriptor requesting UseBuildCache=true. OutputManager
construction and production document requests reject before output/cache access.
Precompiled reference lookup is disabled, and unchanged source replaces a poisoned
in-memory declaration rather than reusing experimental compiled documents.

## Schema and core TypeId integration

The core validates/defaults XML, then injects TypeId attributes while normalizing
references. This insertion changes XML validity status; requiring only the old
Validity=Valid flag would reject legitimate core documents. Trusting an old flag
or type annotation alone would also miss later invalid value changes.

The profile checks schema-bound type annotations and each injected TypeId against
the validated schema type. It copies the allowed XML into a detached document,
removes only those checked injected attributes and revalidates current values
using the owning document's schema set. Only that copy is marshalled. Normalized
source XML is not modified. An invalid StackingLimit introduced after validation
must fail schema revalidation; a formula introduced after validation must fail
the profile's explicit control guard.

## Implementation and verification

- `source/BinaryAssetBuilder.XmlCompiler/BinaryAssetBuilder/XmlCompiler/Ra3Ep1AttributeModifierPlugin.cs`:
  Initialize/ReInitialize, GetExtendedTypeInformation, ProcessInstance,
  CheckAttributes and CopyValidatedRoot.
- `source/BinaryAssetBuilder.ManifestInspector/Ep1AttributeModifierProfileSmokeTest.cs`:
  Run tests descriptor/registry, eight compiler entries, policy/platform/identity
  isolation, invalid post-validation mutations and explicit mapped-cache denial.
  TestDocument uses the actual DocumentProcessor/SessionCache/SchemaSet path,
  checks default StackingLimit, EP1 hash and nested core TypeId compatibility,
  poisons an old declaration and verifies fresh source replacement.
- `tests/fixtures/Ep1AttributeModifierProfile.xml` is an opt-in diagnostic settings
  fixture, not a production SDK build configuration.

```powershell
# Reborn: validate explicit modifier compiler/document integration without production stream emission.
& .\source\BinaryAssetBuilder.ManifestInspector\bin\x86\Release\net8.0\BinaryAssetBuilder.ManifestInspector.exe ep1-modifier-profile-self-test
```

Release/x86 build, all 60 registered compiler groups, layout tests and 33 enum
mappings pass. Native model/marshaller counters remain 784/760 out of 1,390.
The rounded overall estimate remains approximately 50%; this activates only a
diagnostic profile, not a working Uprising Mod SDK or in-game mod.

## Next gates

Prove StartFX/EndFX/Shader through real core normalization, dependency identities
and bounded game golden slices before allowing references in this profile.
Inheritance and arbitrary modifier combinations need separate proofs. Native
compiled-cache reuse, complete type registration, SDK/WorldBuilder packaging and
game loading remain unvalidated. Do not overwrite stock streams to test this.
