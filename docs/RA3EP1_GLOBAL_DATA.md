# AdditionalMaps global data

<!-- Reborn: describe the same source-to-package workflow offered by the Studio checkbox. -->
Use `alpha/EALAModStudio.exe`, select your mod, check **Build Global Data**,
enter the mod version and build normally. The checkbox is EA's native persisted
build option. The existing static entry remains `Data/Static_Mod.xml`.

Global entry points live in your mod's `Data/AdditionalMaps` directory and must
be named `MapMetaData_<name>.xml`, for example `MapMetaData_Global.xml` or
`MapMetaData_Music.xml`. This follows the reference RA3 SDK/Bibber build script:
it compiles additional early-load streams, rather than replacing the game's
`data/global.manifest`. The entry point is an ordinary EA `AssetDeclaration`;
it does not need to contain any `MapMetaData` asset.

Include your global definitions from that entry. Do not also include them into
`Static_Mod.xml`. Compiled global identities are available to the subsequent
static dependency audit and native linker. Retail catalogs remain lookup inputs;
their payloads are not copied into the mod.

`DATA:` uses the enclosing mod `Data` root, even though the entry point is inside
`AdditionalMaps`. Mod files take precedence over `SageXML`. A relative sibling
include from copied mod XML first searches beside that XML, then the corresponding
`SageXML` folder. Thus `Sounds/SoundEffects.xml` can retain its original
`BaseSoundEffect.xml` instance include. Source-only overridable audio bases in that
original file are accepted without claiming native AudioEventOverridable support.
The same mod `Misc/Data/mod.str` is used for global and static string checking.

Each global input is independently audited, prepared, schema checked, compiled,
dependency linked and read back. Its native stream is included once as:

- `data/additionalmaps/MapMetaData_<name>.manifest`
- `data/additionalmaps/MapMetaData_<name>.bin`
- `data/additionalmaps/MapMetaData_<name>.relo`
- `data/additionalmaps/MapMetaData_<name>.imp`
- `data/additionalmaps/MapMetaData_<name>.version` (blank suffix, CRLF)

These families accompany the static stream in
`<Mod>_<Version>_Streams.big`. Existing optional Misc packaging and the final
Documents output directory are unchanged. The SKUDef still lists the Streams
BIG and, when nonempty, the Misc BIG; it needs no per-manifest commands.

<!-- Reborn: archive entry paths and the runtime reference table have different roots; EP1 prepends Data\\ when opening references. -->
Archive entries retain `data/`, but manifest reference strings omit it:
`audio.manifest`, `static.manifest`, `global.manifest`, or
`additionalmaps/mapmetadata_global.manifest`. Both EP1 1.0 and 1.1 reader
factories prepend `Data\` to relative reference strings. A reference written
as `data/audio.manifest` therefore targets the nonexistent
`Data/data/audio.manifest`. This was corrected after the first in-game audio
trial; see [runtime audio diagnosis](RA3EP1_REACTOR_AUDIO_RUNTIME.md).

The checkbox controls fresh compilation **and inclusion**. If unchecked, the
new package contains no additional global streams. Previous globals are not
implicitly reused. A missing/empty AdditionalMaps directory compiles no globals.
Validation-only builds qualify both stages without publishing either.
Global failure aborts the build before publication, preserving the previous
successful package. Receipts retain global entries, source hashes and alias
selections separately from the static receipt.

## Current scope

The stage uses existing alpha processors. AudioEvent and UpgradeTemplate are
working global examples; other original global types still require their actual
processors. Music, actual MapMetaData authoring and arbitrary global settings
are not enabled merely by adding this checkbox.

Files are compiled in filename order. A global stream can reference an earlier
compiled global stream; unsupported forward references fail explicitly rather
than emitting unresolved imports. Duplicate identities across global streams
are rejected. Put mutually dependent definitions in one entry/include closure.

Early globals carry only runtime manifests actually needed by their imports.
They must not pull `data/static.manifest` into startup; static-only imports report
an error identifying the target. Static output records real references to the
compiled AdditionalMaps manifests instead of duplicating their assets.

The `AdditionalMaps\mapmetadata_*.manifest` discovery string was found in both
installed EP1 1.0 and 1.1 executables. The reference SDK documents this early-load
mechanism. Offline compilation/archive validation is separate from gameplay:
the newly packaged global audio still needs an in-game listening test.

## Developer verification

`scripts/Test-Ra3Ep1GlobalData.ps1` exercises the actual Studio backend in the
developer mod slot, restoring its original source afterward. Final publication
goes to a fresh private `.work` directory. It checks separate audio and upgrade
streams, static imports of a new global upgrade, exact archive families and blank
version markers, absence of an automatic static dependency from early audio,
the checkbox, missing global sources, validation-only behavior and preservation
of the last good package on failure. It never builds or modifies Reborn output.
`Test-Ra3Ep1StudioPaths.ps1` also checks the native checkbox through EA's CLR4
CodeDOM contract.

<!-- Reborn: retain actual source qualification evidence without publishing or changing the user's mod. -->
The user's `MapMetaData_Global.xml` closure was also compiled from a private
read-only source mirror on October 24, 2026. Its two Reactor AudioEvents resolved
17 retail AudioFile references and passed native/BIN/RELO/IMP/manifest readback
(output checksum `74e3c72a`). The original `BaseSoundEffect.xml` instance source
was found through the SageXML sibling fallback. No Reborn output or game files
were changed, and no game was launched.
