# Experimental EP1 armor processor profile — 2026-10-01

## Outcome and scope

The golden-validated ArmorTemplate tokenizer is now reachable through an
explicit `IAssetBuilderPlugin` implementation:
`BinaryAssetBuilder.XmlCompiler.Ra3Ep1ArmorPlugin`. The profile does not inherit
the KW plugin, share its static registry/dispatch caches, or fall back to it.
Existing default compiler settings remain unchanged.

`Ep1ArmorProfileSmokeTest` loads the opt-in settings through the actual Settings
reader and PluginDescriptor/assembly loader, obtains metadata via PluginRegistry,
and calls the production `ProcessInstance` interface on a schema-validated
declaration. Its BIN/RELO/IMP exactly match the previous independently
golden-tested armor helper. This is compiler-entry validation, not yet a full
SDK document build or a playable mod.

| Profile field | Value |
|---|---|
| Name | RA3EP1-Armor-Experimental-v1 |
| Platform | Win32 only |
| Supported root | ArmorTemplate only |
| TypeId / TypeHash | 0x3A6C5E8E / 0xA0E237D8 |
| Tokenized | true |
| AllTypesHash | 0x5454A8E9, observed game target signature |
| ProcessingHash | TypeHash XOR 0x45503101, a project-local cache domain/revision |
| Production output | explicitly disabled |
| Binary build-cache reuse | explicitly disabled |

ProcessingHash is not presented as EA's hash derivation. The aggregate hash is
the observed game target signature, **not** evidence that this one-entry table
is complete. The legacy Plugin.AllTypesHash and KW registration values have
not changed. The earlier type-audit snapshot still describes that legacy plugin.

## Opt-in diagnostic configuration

`tests/fixtures/Ep1ArmorProfile.xml` selects the new plugin as the default with
AssetTypes="#all". This means unsupported roots reach its explicit rejection,
not an unrelated fallback compiler; it does not mean all types are supported.
BuildCache and UseBuildCache are false. This file is a focused test settings
fragment, **not** a complete replacement SDK build configuration.

Run after a Release/x86 inspector build:

```powershell
# Reborn: exercise the real profile load/registry/compiler route without writing production streams.
& .\source\BinaryAssetBuilder.ManifestInspector\bin\x86\Release\net8.0\BinaryAssetBuilder.ManifestInspector.exe ep1-armor-profile-self-test
```

The compiler self-test now invokes 51 groups, including this profile proof and the document-stage follow-up.
The separate armor-token-self-test still compares bounded real-game samples
and writes clearly labeled diagnostic fixtures.

## Safety boundaries

`IAssetBuilderOutputPolicy` separates a correct target signature from readiness.
`PluginRegistry.ValidateProductionOutput` examines both the default and all
explicitly mapped plugins. An experimental plugin cannot evade this guard by
being mapped only to ArmorTemplate. `OutputManager` calls the guard before its
constructor can move/delete previous outputs, and again before CommitManifest
can reuse or write a manifest. Existing hash/version checks remain in place.

The profile is deliberately not approved for production output. A null
DocumentProcessor is still permitted for existing metadata-only diagnostic
uses of OutputManager; that is not a production build entry point.

PluginRegistry also honors `CanUseBuildCache=false`, even when a mapped
descriptor requests UseBuildCache=true. This does not establish every session,
precompiled or linked-stream reuse path as safe; those remain follow-up work.

The profile rejects uninitialized use, Xbox360/PS3, unsupported TypeIds, wrong
XML roots/namespaces, inheritance, custom data, strong/weak dependencies,
unknown attributes/elements, unevaluated formulas and Damage="ALL". Rejected
platform reinitialization clears initialization rather than retaining Win32
eligibility. Each metadata query returns a fresh object to keep callers from
mutating the profile registration. Production processing still expects the
normal schema validation/expression normalization stage before entry.

## XML identity and cache invalidation

`InstanceDeclaration.XmlNode` previously hashed `value.Name`, so a valid
namespace prefix could turn ArmorTemplate into the incorrect `p:ArmorTemplate`
TypeId. It now uses LocalName for the current handle and unqualified inheritFrom
type. The prefix regression runs through the actual profile entry point.

DocumentProcessor cache versions were bumped from 14/15 to 16/17 to reject
cached declarations predating identity normalization and the new profile
policy. Existing session-cache regression checks reject the previous revision.

## Remaining work

The [focused document pipeline follow-up](RA3EP1_ARMOR_DOCUMENT_PIPELINE.md)
now covers source loading/validation/hash stages and separately denies
experimental session/precompiled reuse. Current DocumentProcessor versions
are 18/19; the 16/17 bump described above belongs to the earlier profile block.
This does not complete production stream writing or general legacy reuse rules.

1. Run a complete AssetDeclaration/schema/document pipeline with the explicit
   target profile, including reference stream configuration and unsupported
   root diagnostics; retain the production guard until output policy is justified.
2. Audit precompiled/session reuse, tokenized IsEquivalent behavior and the
   production stream checksum/InstanceHash policy. Disabled network binary
   cache reuse alone is not enough.
3. Define and test the conditions for a narrow production armor-only profile,
   distinct from declaring the entire SDK/type table complete.
4. Establish reversible packaging and actual Uprising loading, without replacing
   stock game streams. Expand validated processors only after this small path
   has been demonstrated.

Overall effort estimate remains approximately 50% complete / 50% remaining.
This block connects an already validated processor to a real plugin path; it
does not close the much larger complete-registry or in-game loading workstreams.
