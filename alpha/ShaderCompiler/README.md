# Reborn Shader Compiler

<!-- Reborn: describe the separate local authoring tool and its actual validation boundary. -->
Open `RebornShaderCompiler.exe`, select an FX source and output folder, and use
**Compile shader** or **Compile 4 KW samples**. Keep native loader validation on.
The default sources are experimental KW loader candidates. They pass Uprising
effect loading but have a confirmed rendering defect: they overwrite the shared
tint register and can turn other models red. They are not ready for gameplay.
Their evaluator/technique migration is also unfinished. The compiler does not
install outputs into mods automatically.

Outputs are under `Output`. Original reference sources stay unchanged.
See `../../docs/SHADER_COMPILER.md` for requirements, remaining work and evidence.
The reproduced failure and focused correction control are recorded in
`../../docs/SHADER_RENDER_STATE_INVESTIGATION.md`.
