# Reborn Shader Compiler

<!-- Reborn: document the usable compiler separately from the remaining experimental KW shader port. -->
Launch `alpha/ShaderCompiler/RebornShaderCompiler.exe`. This separate dark UI
compiles `.fx` sources using the bundled local copy of the RA3 SDK's legacy
FXC. It does not replace Mod Studio or change normal mod compilation.

1. Choose an FX source. The default is the prepared experimental ObjectsGDI.
2. Choose an output folder, normally the tool's `Output` directory.
3. Keep **Validate with Uprising DirectX loader** selected.
4. Select **Compile shader**, or **Compile 4 KW samples** to compile the four
   supplied candidates together.

The tool uses Windows / `fx_2_0`, `EA_PLATFORM_WINDOWS=1`, `_WW3D_=1`,
`_WW3D_VERSION_=1` and FXC's `/LD` legacy backend. Modern FXC releases are not
drop-in replacements: the original effects contain legacy shader profiles and
HLSL constructs. The included FXC is the local SDK's 9.19.949.1075 compiler.

Compilation runs in a worker with an owned cancellable child process. Closing
while busy asks whether to cancel. A failed compile or required validation
does not replace the previous successful FXO. Output-folder locking serializes
publication; a leftover lock filename is not a stale process lock.

## What validation proves

The x86 tool creates a private hidden Direct3D9 device and loads the result with
`d3dx9_35!D3DXCreateEffectEx`, passing Uprising's recovered application-managed
constant list. It never starts, attaches to or patches the game. Normal Windows
desktop graphics access and the DirectX9 runtime are required. If a device
cannot be created, validation is **unavailable**, not a shader rejection.

The four supplied KW loader candidates compile and pass this check. Their
unmodified retail KW FXOs fail the same check; the original Uprising DefaultW3D
passes. Each published FXO gets a receipt with compiler/source paths, its SHA256,
effect-loading result and `RenderingValidated=False`.

## Experimental source copies and remaining work

Original sources provided by the user remain read-only references. Prepared
copies are under `alpha/ShaderCompiler/Sources/KW-EP1-LoaderCandidate`. They keep
the original KW material functions, textures, techniques and IonHull/scrolling
code. Changes are isolated to the following loader adaptation:

- Engine-managed globals use the corresponding Uprising fixed register slots,
  derived from the supplied `RegisterMap.fxh`.
- Skinning uses the c128..c255 window (64 quaternion/translation bones); the low
  quality alias reuses that window, and the unskinned world matrix starts at c124.
- CPU effect-state expressions no longer read engine-managed OpacityOverride
  while expression evaluators are enabled.
- Old implicit vector-to-scalar `lit` inputs are made explicit.

**The current loader candidates have a confirmed rendering defect:** their
material passes overwrite Uprising's shared tint register and can turn other
models red. They are not ready for gameplay use. See
[the native render-state investigation](SHADER_RENDER_STATE_INVESTIGATION.md)
for the reproduced failure, original-shader control and focused correction test.

**These are loader candidates, not a completed visual port.** The remaining KW
evaluator names (`Objects_P0`, `Objects_P1`, `Objects_U`, `Objects_M`,
`Objects_L`, `Objects_CreateShadowMap`, `Objects_WorldNormShader`,
`Objects_DrawFlatColor`) are absent from the inspected Uprising 1.1 executable.
The tool warns about them rather than silently renaming them to `Objects`:
that would not prove shader-array indices or render-state behavior compatible.
The Uprising register reservation for unused globals, evaluator/technique
contracts, recoloring, shadow transforms, skinning and final visual behavior
still require migration and in-game validation.

The live SampleMod FXOs and Documents packages have not been overwritten.
Do not treat a successful loader check as proof that the original startup issue
and all subsequent rendering behavior are resolved in the game.

## Developer files

Implementation: `tools/ShaderCompiler/ShaderCompiler.cs` and `EffectValidator.cs`.
Build: `tools/ShaderCompiler/Build.ps1`. It builds only this utility using the
installed .NET Framework C# compiler; it does not invoke SDK synchronization.
Its optional `FxcPath` parameter copies an existing legacy SDK compiler.
`Prepare-KwLoaderCandidate.ps1` regenerates the experimental source copy from
the user's KW and Uprising shader directories into a **new** output directory,
checks Uprising's register map and records its provenance. No original source
directory is modified. Build outputs and generated EA source copies remain
local; the utility source and preparation recipe are kept in the repository.

Current private native/compile evidence is under
`alpha/.work/KwShaderPort-20261102` and
`alpha/.work/ShaderCompilerProgram-20261102`. Earlier failure/control evidence
is described in `SAMPLEMOD_KW_SHADER_COMPATIBILITY.md`.
