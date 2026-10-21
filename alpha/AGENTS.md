# Alpha SDK workspace ownership

- The user's only entry point is `EALAModStudio.exe`. Do not present maintenance scripts as alternative user workflows.
- Keep exactly two direct mod directories: `mods/Reborn` and `mods/RebornTest`.
- `mods/Reborn` belongs to the user. Do not edit, replace, reset or delete its files as part of developer tests. Do not rebuild its output during automated regression checks.
- `mods/RebornTest` is the reusable developer test slot, including Conscript experiments. Put current runnable trial XML here, not in additional mod folders.
- Keep old/regression sample sources under `../tests/fixtures/live-alpha`, outside the Studio mod list. Automated tests must restore RebornTest's pre-test data afterward.
- Publish versioned Streams/Misc BIGs and SKUDef under the Windows Documents known folder's `Red Alert 3 Uprising/Mods/<name>` as requested by the user. Keep private compiler receipts under `builtmods/<name>`. Automated fixtures must use an isolated final output root under `.work`, never the user's Documents mods. Do not modify game installations or launch games without separate authorization.
- Compiler synchronization must preserve editable mod sources. Adding a compiler processor does not silently enable unsupported source profiles.
