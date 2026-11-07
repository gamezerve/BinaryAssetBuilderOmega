# Portable Uprising SDK alpha

<!-- Reborn: record the user's agreed runtime bundling and Windows Documents output behavior. -->
The SDK's user entry point remains `alpha/EALAModStudio.exe`. Copy the alpha
directory to a writable folder and open that executable there. Configuration
paths are resolved relative to Studio, not the shell's current directory.
No repository checkout, .NET SDK, separate PowerShell installation, registry
game discovery or unpacked `D:/TEMP` stream directory is required for building.

## Included dependencies

| Component | SDK location | Purpose |
| --- | --- | --- |
| PowerShell 7.6.5 and its own runtime | `runtime/powershell` | Build orchestration |
| x86 .NET 8.0.31, x86 host and hostfxr | `runtime/dotnet-x86` | 32-bit compiler process |
| Visual C++ x86 native libraries | `runtime/vc-x86` | Native audio encoder dependencies |
| Build scripts | `scripts` | Static/global build, logging and final packaging |
| Compiler assemblies and native audio encoder | `tools/compiler` | Asset compilation |
| Compiler schema/fixture resources | `tools/resources` | Native schema/source processing and developer controls |
| Static/global/audio stream baselines | `catalogs/ra3ep1/streams` | Stock/native processor inputs |
| Lookup manifests, WorldBuilder sidecars and stock CSF | `catalogs/ra3ep1` | Missing-asset/string checks and selected dependency copying |
| Original source library and schemas | `SageXML`, `Schemas` | DATA fallback, source inheritance and validation |
| Optional legacy DirectX shader libraries | `ShaderCompiler/*.dll` | Local shader utility/FXC loading |

Art and Audio folders, editable mod sources, Studio components and documentation
stay in the same SDK. The bundled runtime and baseline copies do not modify
their original installations. Original game audio/media payloads are not
automatically manufactured by the SDK.

## Remaining host requirements

- 64-bit Windows with 32-bit application support; this bundle targets Windows
  10/11. Its bundled PowerShell is 64-bit and its compiler remains 32-bit.
- Windows .NET Framework 4.x/CLR4 for EA Studio and its helper. The tested host
  uses .NET Framework 4.8; unlike the bundled modern .NET runtime, this remains
  a Windows component.
- A writable SDK directory and writable Windows Documents folder.
- GPU/driver Direct3D9 support for the optional shader utility's native graphics
  validation. Normal XML compilation does not create a graphics device.
- A compatible installed Uprising game for playing the result, not for the
  SDK-local catalog/source lookups or asset build.

The bundle is assembled from the user's local EA SDK/game references. Local
portability does not by itself establish permission to redistribute those EA
components publicly. Existing licenses/notices remain with their components.

## Output and copy behavior

As explicitly selected by the user, final packages still go to the **current
Windows user's Documents** directory:

`Red Alert 3 Uprising/Mods/<mod>/<mod>_<version>_Streams.big`

The optional Misc BIG and SKUDef follow the same directory/version convention.
The destination is obtained from Windows' Documents known folder; it is not
copied from the developer's username or hardcoded drive.

Successful Static, Global and AptUI outputs are retained separately under
`builtmods/<mod>/<family>/<generation>/files/data`. The `MapCatalog` family
retains the optional single-player map catalog produced by Build Maps.
`current.json` selects one successful generation per family. SHA256 inventories,
source provenance, weak-use evidence and captured warning coordinates accompany
each generation. The combined Streams BIG is created only for final publication;
no redundant combined BIG is retained under `builtmods`.

In Studio, checked build stages replace their complete output family after a
successful run. Unchecked stages reuse their last successful generation. Enable
Create BIG Files to package the selected new and saved outputs, or enable it
alone to repackage without recompiling. With Create BIG Files disabled, successful
compilation still saves its selected outputs. Validate-only maintenance checks do
not save generations. Failed compilation, dependency checks or final packaging
retain the previous stage index and published package. Packing verifies saved
file hashes and cross-stream strong references before publication.

The first package needs a successful Static output. Rebuilding a selected empty
Global or AptUI family removes its previous contents; leaving it unchecked keeps
those contents. A verified legacy `builtmods/<mod>/<mod>.big` is split into these
families automatically on first reuse. After successful migration, its redundant
BIG and internal SKUDef are removed.

Scratch data remains in `.work`. Neither directory contains required compiler
inputs. Both can be omitted from a copy, but discarding `builtmods` also discards
the outputs needed for partial builds and packaging-only runs; compile the
required families again. Startup creates missing directories without clearing existing contents. Do not copy
while a build or tool synchronization is active. Moving the folder does not
update paths saved in separate launchers outside this SDK.

Standard DATA:, ART: and AUDIO: includes move with the SDK. Literal external
paths authored in a mod remain external; portability does not rewrite user XML.

`uprising-alpha.paths` is SDK-relative. `toolchain.json` can retain an original
compiler-source path as provenance; that informational field is not opened by
the build. Already generated receipts/logs can similarly retain historical paths.

## Developer maintenance

`Sync-Ra3Ep1DevSdk.ps1` still refreshes the single alpha toolchain after a
successful compiler build. It also refreshes the portable script/resource
copies. It preserves user mods, dependency baselines, bundled runtimes and
relative settings. Updating a separate copied SDK requires copying the updated
files to it; the portable copy does not reach back into the original repository.

`Initialize-Ra3Ep1PortableSdk.ps1` assembles runtime/baseline copies from explicit
local inputs; it is a maintenance tool, not another user entry point.
`Test-Ra3Ep1PortableSdk.ps1` uses a fresh copy with spaces in its path, an unrelated
working directory, bundled runtimes and private final publication. It checks both
fast and developer modes without launching the game or publishing to Documents.

## Verification on 2026-11-03

An independent SDK copy in a folder containing spaces completed both fast and
developer RebornTest builds, including final BIG/SKUDef publication to a private
output root, from an unrelated Windows working directory. EA's CLR4 CodeDOM
adapter/path checks passed against both the live SDK and the copy. Apphost
tracing selected the copied x86 .NET runtime. Removing a bundled compiler
resource in the private copy caused the expected missing-resource error instead
of falling back to the still-available original repository.

Evidence remains under `alpha/.work/portable-sdk-20261103-a`: build logs, final
test packages, apphost trace and JSON reports. The large temporary SDK copy was
removed after verification. No live mod sources, Documents outputs or game
installation files were changed; no game was launched. A clean second Windows
machine and interactive Studio launch on it have not been tested.
