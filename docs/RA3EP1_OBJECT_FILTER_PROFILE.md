# Experimental EP1 ObjectFilter compiler profile

## Scope and outcome

`Ra3Ep1ObjectFilterPlugin` explicitly binds the [stock-proven native subset](RA3EP1_OBJECT_FILTER_NATIVE.md)
through PluginDescriptor, PluginRegistry, the real document loader/default/reference
stages and ProcessInstance. All eleven compiler entries match the golden-tested
native buffers. The legacy KW registry and aggregate hash are unchanged.

Observed native metadata: TypeId 44A5973D, TypeHash DF72B4BA, Tokenized=false.
Local ProcessingHash is `DF72B4BA ^ 45503121`, VersionNumber=1; this is an isolated
processing/cache domain, not a newly calculated EA runtime hash. AllTypesHash
5454A8E9 indicates target only. Type information is detached and has no fallback.

ProfileName is `RA3EP1-ObjectFilter-Experimental-v1`. CanWriteProductionOutput,
CanUseBuildCache and CanReuseCompiledDocuments are all false. Win32 is the only
supported ABI; failed platform initialization revokes prior readiness. No settings
flag or explicit type mapping requesting cache use can override these policies.

## Bounded eligibility

Only schema-bound ObjectFilterAsset roots with exactly one inline Filter are
accepted. Root id must match the current declaration's InstanceId. The filter
must use NONE rule and NONE alignment; relationship is empty or ENEMIES, and
Include is empty or one of INFANTRY, AIRCRAFT, SHIP, VEHICLE. At most fifteen
IncludeThing weak leaves are accepted. Each normalized name must match the same
position's GameObject TypeId/InstanceId in WeakReferencedInstances; every metadata
entry must be consumed. Weak leaves do not generate strong imports.

Inheritance, strong/file references, custom data, Exclude/ExcludeThing, filter-local
id references, status masks, other rule/alignment/relationship/kind combinations,
unresolved formulas and nested controls are rejected. The native proof's synthetic
optional-fields fixture does not silently expand this profile's eligibility.

Injected TypeId attributes are checked against each original schema-bound type,
including Filter and weak leaves. A detached copy removes only checked injected
attributes and revalidates current values using the caller's schema set. This
handles core TypeId insertion without trusting stale Validity flags or mutating
the source declaration. Any generated IMP buffer must remain empty.

## Verification

- `source/BinaryAssetBuilder.XmlCompiler/BinaryAssetBuilder/XmlCompiler/Ra3Ep1ObjectFilterPlugin.cs`:
  Initialize/ReInitialize, detached GetExtendedTypeInformation, bounded
  ProcessInstance, checked attributes and formatting/control rejection.
- `tests/fixtures/Ep1ObjectFilterProfile.xml`: explicit diagnostic settings,
  not a production SDK build profile.
- `source/BinaryAssetBuilder.ManifestInspector/Ep1ObjectFilterProfileSmokeTest.cs`:
  descriptor/document/eleven compiler entries, ordered weak table checks, expanded
  attribute rejection, invalid injected leaf TypeId, root identity tampering,
  unvalidated input, stale-source replacement and platform/production/cache gates.

The weak-table rejection cases include reordered, missing, extra, wrong-name and
wrong-type entries. Strong/file dependency injections reject. A poisoned current
declaration is replaced by unchanged source on repeat document processing; the
caller's UsePrecompiled option remains unmodified. A production request rejects
before source/cache access, and explicit mapped-cache configuration still fails
production validation. No production/native stream is written by the harness.

Run `ep1-object-filter-profile-self-test` with the Release/x86 inspector, or the
full `compiler-self-test`. Full inspector/builder builds, all **64** compiler groups
and layout tests pass. Models/marshallers remain 784/760 of 1,390. The overall
rounded effort estimate remains approximately 50%; this is a third narrow compiler
family, not an operational Uprising Mod SDK or an in-game loading result.

## Next gates

Broaden only with real source/stock native evidence and explicit schema/reference
guards. Filter inheritance, other mask/rule combinations, expressions, complete
EP1 registration, compiled reuse, packaging and runtime behavior remain open.
Keep experimental output/cache restrictions closed until a complete target-aware
build and loading workflow is independently validated.
