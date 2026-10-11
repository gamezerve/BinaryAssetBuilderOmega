# Uprising Mod SDK Alpha — local workspace candidate

This is a limited, experimental **local-use candidate**, not a public release
or a complete Uprising SDK. It retains the existing EA Mod BuildStudio interface.
User-provided screenshot/log evidence now confirms actual Studio loading,
Reborn selection, output capture and successful completion, with independently
verified package bytes. A subsequent Error Log screenshot confirms ALPHA001 and
ALPHA002 under Warnings. The corrected InvalidXml screenshot also confirms the
failure callback without Build Completed. Updated local packaging passes clean
extraction with the fix. These checks do not establish all
original Studio features.

Supported: exactly one flattened `AlliedConYardArmor` with thirteen ordered armor
entries. Only GUN Percent is editable, as an integer 0..10000. Unsupported XML
fails rather than being silently omitted. The bundled sample uses 5000.

Prerequisites: PowerShell 7, an x86 .NET 8 runtime, your existing RA3 Mod SDK
Studio components and compatible .NET Framework 4/CodeDOM environment, plus an
unpacked Uprising `static.manifest` and its corresponding BIN/RELO/IMP files.
Game binaries, stock streams, codecs and WorldBuilder are not provided.

`uprising-alpha.paths` configures four literal `key=absolute-path` lines:
PowerShell7, StaticManifest, Driver and
CompilerDirectory. Update Driver and CompilerDirectory after relocating this
workspace. Launching the UI is a separate test; no preparation/build command
starts a game or modifies a game installation.
The local `EALAModStudio.exe.config` selects CLR4 rather than repairing the
machine's old CLR2 compiler. The user-run Reborn GUI success path passed with it.

For an offline backend build, run from this workspace in PowerShell 7:

```powershell
# Reborn: compile only the supported sample into a fresh builtmods child; do not install or launch.
& .\scripts\Invoke-Ra3Ep1AlphaBuild.ps1 -ModXml .\mods\Reborn\data\mod.xml `
  -StaticManifest 'C:\YOUR\UNPACKED\data\static.manifest' `
  -CompilerDirectory (Join-Path $PWD 'tools/compiler') `
  -OutputRoot (Join-Path $PWD 'builtmods')
```

Output is a fresh `builtmods/RebornAlphaInput-<guid>` directory. The final
`alpha-build.json` points to a verified BIG/config/SKU package and its authored
receipt. Intermediate/admission reports alone do not mean the build succeeded.
The SKU is a mod fragment, not a replacement stock SKU. No automatic install
or launch is provided. Only the earlier GUN-5000 package has user-observed
in-game behavioral evidence; changed values are initially offline validated.

The preparation inventory records source/copy hashes and origins. Existing EA
SDK files are copied locally from your supplied installation, not newly
published by this project. The project LICENSE does not establish rights for
all EA components or official schema material. Public redistribution review
is still open; do not upload this workspace as a reviewed release ZIP.

This candidate contains a focused compiler executable with additional developer
commands; only the documented armor backend is part of the alpha scope.

For the separate dependency overlay candidate, see OVERLAY.md when supplied.
It requires user-supplied matching EA components/schema files rather than
bundling them. Compiler/fixture provenance review still precedes public release.
