# Frozen live-alpha regression sources

These are input fixtures, not additional Studio mods. The former armor/AK47
sample directories were moved here unchanged. Reborn is an independent frozen
baseline, not a link to the user's editable mod. ConscriptBuildTime1 contains
the reviewed complete original-ID source used by the bounded Conscript route.
The Conscript XML intentionally retains captured whitespace, including Script
text; two file-specific Git attributes preserve it rather than trimming it.

The regression driver stages each input only in alpha/mods/RebornTest/data,
restores that directory afterward, and never compiles the user's Reborn mod.
Keep unrelated new tests outside alpha/mods rather than growing its UI list.

Official-source provenance and public redistribution review remain incomplete;
these local development inputs do not establish a cleared SDK release package.
