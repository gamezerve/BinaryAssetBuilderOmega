# Mod-first SDK source search

The live Studio frontend now supplies an explicit ordered search profile to both
Conscript and experimental armor source preparation. Earlier strict diagnostic
callers retain their single-root behavior.

| Reference | Search order |
| --- | --- |
| `DATA:GlobalData/GlobalDefines.xml` | `mods/<name>/Data/GlobalData/GlobalDefines.xml`, then `SageXML/GlobalData/GlobalDefines.xml` |
| `ART:SUAntiInfantryInfantry_SKN.w3x` | `mods/<name>/Art/<file>`, then `mods/<name>/Art/SU/<file>`, then `Art/SU/<file>` |
| `ART:SU/<file>` | `mods/<name>/Art/SU/<file>`, then `Art/SU/<file>` |
| `AUDIO:<relative-file>` | `mods/<name>/Audio/<relative-file>`, then `Audio/<relative-file>` |
| Unaliased relative Include | Relative to its declaring document, confined to that document's selected source root |

All SDK paths above are relative to the directory containing EALAModStudio.exe,
not the process working directory. Names are Windows-case-insensitive. The
first existing file wins. Malformed mod XML, directory shadows and unsafe/reparse
paths fail; they do not authorize falling through to a stock file.

## Local development stock library

The portable default data library is `alpha/SageXML`. This checkout has not copied
the entire EA XML library there. Its local ignored `alpha/uprising-alpha.paths`
explicitly sets `SdkDataRoot` to the existing official Uprising XML directory.
That replaces only the stock DATA root; mod Data still has priority.
Removing that setting restores the adjacent SageXML layout. This setting is
independent of `UprisingXmlRoot`, which remains the native qualification fixture
input. Stock ART/AUDIO roots are `alpha/Art` and `alpha/Audio`; RA3 resources are
not silently substituted for Uprising resources.

The reference RA3 `defaultscript.cs` passes ordered `/data`, `/art`, `/audio`
lists with mod paths before SDK SageXml/Art/Audio. Its inherited
`Core/IO/FileNameResolver.cs` searches those lists and applies two-character ART
fanout. The alpha deliberately adds direct mod-Art lookup before mod fanout to
support the authoring layout requested here. Registry, arbitrary current-directory
search, ROOT aliases and postfix/LOD variant search are not enabled by this work.

## Evidence and limits

Build Log prints the logical reference, winning layer and physical file. Final
build receipts retain `Backend.PathSearches` and raw XML hashes. Source
preparation and publication recheck precedence: a newly appearing mod override
invalidates an earlier SDK choice even when the SDK file itself is unchanged.
These are bounded filesystem checks, not an atomic filesystem transaction.

Resolving a resource is **not** compiling it. ART W3X Includes are classified as
resource dependencies instead of being misread as AssetDeclaration documents.
The current frontend still rejects resource-bearing builds until the relevant
processor is connected; no stock graph or binary silently replaces the authored
model/audio. Untouched official Conscript XML is therefore not yet a supported
complete source build.

The grouped `sdk-source-search-self-test` covers real inherited BuildTime edits
through stock and mod definitions, direct/fanout ART, AUDIO fallback/override,
stock-relative context, missing files, malformed overrides, late shadows,
directory shadows, unknown aliases and traversal refusal. Tests create disposable
fixtures only under an owned work directory, never the user's Reborn mod.

## Verification — October 18, 2026

- Grouped source-search integration passed, including an actually located W3X
  refusing at the processor gate rather than being parsed as AssetDeclaration.
- Six existing strict path/typed-source/definition/inheritance suites passed.
- An independent armor integration compiled a stock-SDK base whose own DATA
  reference selected mod definitions: GUN=250 replaced the stock fixture's 5000,
  and actual native/package compilation passed.
- Alpha regression passed **21/21**. RebornTest's final receipt contains five
  raw sources, four search selections and two emitted assets. Its BIG SHA-256
  remained `0195B0EF453DCCF7B09E602B63E8DE07B30272FD304A859F4386E98ECB8AA1B3`.
- Studio's exact-reference CLR4 CodeDOM/interface/config/options loader passed
  with the optional SdkDataRoot setting. Restart an already-open Studio once.
- No game/installation mutation or user Reborn source/output build occurred.

Generated evidence remains local under `alpha/.work/search-*.log` and
`dev-sdk-test.json`; fixture resources are synthetic, not redistributed EA assets.
