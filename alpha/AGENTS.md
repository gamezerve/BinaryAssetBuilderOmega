# Alpha SDK workspace ownership

- The user's only entry point is `EALAModStudio.exe`. Do not present maintenance scripts as alternative user workflows.
- Keep exactly two direct mod directories: `mods/Reborn` and `mods/RebornTest`.
- `mods/Reborn` belongs to the user. Do not edit, replace, reset or delete its files as part of developer tests. Do not rebuild its output during automated regression checks.
- `mods/RebornTest` is the reusable developer test slot, including Conscript experiments. Put current runnable trial XML here, not in additional mod folders.
- Keep old/regression sample sources under `../tests/fixtures/live-alpha`, outside the Studio mod list. Automated tests must restore RebornTest's pre-test data afterward.
- Publish generated BIG/SKUDef and receipts only under `builtmods/<name>`. Do not install or launch games without separate authorization.
- Compiler synchronization must preserve editable mod sources. Adding a compiler processor does not silently enable unsupported source profiles.
