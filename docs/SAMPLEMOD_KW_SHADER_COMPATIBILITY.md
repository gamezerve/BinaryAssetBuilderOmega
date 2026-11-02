# SampleMod KW shader startup failure

<!-- Reborn: retain confirmed runtime-loader evidence so future cleanup does not lose the reason copying KW FXO files failed. -->
The missing `ObjectsGenericEnvMap.fxo` was a genuine packaging omission, but
adding it did not resolve the startup crash. The final Misc BIG now contains all
four custom shader files, byte-identical to the user's sources:

- `objectsalienionhull.fxo`
- `objectsgdi.fxo`
- `objectsgenericenvmap.fxo`
- `objectsnod.fxo`

The generated mesh records contain nonempty, correctly serialized shader names.
The corresponding native string layout also matches selected retail mesh
headers. This does not establish every mesh feature's gameplay correctness,
but the empty `File` text in the crash is not an empty authored shader name.

## Confirmed effect-loader incompatibility

Bounded static inspection of the installed `ra3ep1_1.1.game` identifies the
failure in the ShaderFactory path. The executable imports
`D3DXCreateEffectEx` from `d3dx9_35.dll` and supplies this application-managed
constant list:

`AmbientLightColor;DirectionalLight;PointLight;Cloud;NoCloudMultiplier;RecolorColor;ShadowMapWorldToShadow;OpacityOverride;TintColor;EyePosition;ViewProjection;WorldBones;`

An independent x86 probe created a hidden 1x1 Direct3D device and used the same
DirectX runtime. It did not launch, attach to, patch or change the game.

| Input | Ordinary D3DXCreateEffect | Uprising-style D3DXCreateEffectEx with its skip constants |
| --- | --- | --- |
| All four user KW shaders | `0x00000000` | `0x8876086C`, error buffer null |
| Original Uprising DefaultW3D shader | `0x00000000` | `0x00000000` |

Single-constant controls also reject several matching KW parameters, including
AmbientLightColor, DirectionalLight, Cloud, NoCloudMultiplier, OpacityOverride,
ViewProjection and WorldBones. The exact failures vary with the shader.

This reproduces the DirectX rejection and null error-text condition behind the
startup dialog. These FXOs are not simply missing or generically unreadable;
they fail the Uprising effect-loader contract. D3DXCreateEffectEx requires
skipped constants to be register-bound and used only in HLSL shader code.
See [Microsoft's API contract](https://learn.microsoft.com/en-us/windows/win32/direct3d9/d3dxcreateeffectex).

## Next correction

Port the model materials to suitable stock Uprising shaders (for example,
ObjectsAllied or ObjectsGeneric), matching the texture/parameter and technique
contracts as well as the shader name. Alternatively, adapt and recompile the
original shader sources for Uprising's application-managed constants. Copying
the original KW FXO files alone is insufficient. Do not change the game binary
or disable its constant-management path as a workaround.

No user mod source, final package or compiler behavior was changed during this
investigation. The in-game result of a material/shader port remains untested.

Private evidence, under `alpha/.work`:

- `ShaderEffectProbe-20261102.ps1`
- `ShaderEffectProbe-20261102-native.log`
- `ShaderEffectProbe-20261102-skipconstants.log`
- `ShaderEffectProbe-20261102-retail-control.log`
- `ShaderFailure-20261102-disasm.txt`
- `ShaderSkipConstants-20261102.txt`

The initial restricted-context Direct3D device creation failed; the successful
results above use normal Windows DirectX access. This was a probe-environment
limitation, not a shader failure.
