# Developer test mod

This is the reusable developer slot. The user's separate Reborn mod is not a test fixture.

Current trial: the complete original-ID `SovietAntiInfantryInfantry` Conscript,
with `BuildTime="1"`. Select **RebornTest → Build Mod** in the alpha's
EALAModStudio.exe. Outputs are under `alpha/builtmods/RebornTest/`.

The current bounded route recompiles this exact XML through Core. It refuses
other changes; arbitrary GameObjects, BuildTime values and new-ID recruitment
are not enabled. The output is expected to match the earlier one-second game
test's BIG. Fresh generation does not itself constitute another gameplay test.

Automated regressions temporarily use other fixtures here, then restore this
data directory and rebuild its current trial. No extra mod folders are created.
