# Uprising SDK release gates and effort estimate

## Current position — October 11, 2026

One existing armor identity has a user-observed in-game override. The same
working package can be reproduced from current source in one command. That
is a useful vertical slice, not evidence that all asset processors or the
complete EA SDK workflow are compatible.

The next release should be called **Uprising Armor Preview**, not a complete
RA3 Uprising Mod SDK. A zipped working sample could be assembled now, but
its fixed example, local paths and repository/runtime dependencies would not
make it a portable editable SDK. Do not label it as one.

| Deliverable | Remaining gates | Working effort estimate |
| --- | --- | --- |
| Armor Preview | Editable GUN input through actual document/Core/package path; 100/250/5000 cases; portable paths/runtime instructions; clean-directory package test and restore guidance | Approximately 2–5 focused working days |
| Basic gameplay SDK alpha | GameObject/Weapon and needed modules, dependency/import closure, reference/override precedence, repeatable build and controlled gameplay checks | Several weeks; reassess after the first unit/weapon vertical slice |
| RA3-SDK-like broad release | Wider typed processor coverage, inheritance/includes/cache contracts, textures/models/audio/shaders, map workflows and WorldBuilder integration, distribution/dependency review | Several months or longer; no reliable fixed release date yet |

These are provisional engineering estimates, not measured remaining hours,
calendar deadlines or a promise of unattended background work. Unknown runtime
contracts can expand them. Frequent conversation continuations are not work-day
units. No credible overall percentage follows from the single armor success;
the old 52% figure is historical and not a release countdown.

## Preview acceptance checklist

1. Accept a small standalone XML with fixed `AlliedConYardArmor` identity;
   initially permit only canonical integer GUN percentages 0..10000. Reject
   formulas, includes, inheritance, unknown attributes and other asset types.
2. Freeze and compile that input through the actual document and scoped
   OutputManager path. Independently compare all other BIN bytes and RELO/IMP
   against the stock-baseline contract. Fresh input must get fresh Core hashes;
   never reuse the fixed-5000 manifest hash/checksum for changed input.
3. Validate every default `mod{,_l,_m}` family and its version marker against
   the current admitted input, rather than the old fixture or fixed package
   digest. Preserve the fixed recipe regression. Do not enable general
   production, cache or precompiled reuse merely to make this build succeed.
4. Provide a normal user-facing build command, example XML, output-directory
   layout and meaningful logs. Eliminate developer-specific absolute paths
   from the distributed entry point; explicitly configure stock references.
5. Assemble only an explicit reviewed file allowlist. Include project license
   and notices; review external components independently. Do not bundle game
   executables, installed archives, large stock BINs, personal local artifacts
   or assume the repository MIT license covers every EA SDK dependency.
6. Test extraction/build from a clean directory with documented prerequisites.
   Confirm no dependency on hidden source folders or the developer's cache.
   A local ZIP can follow these checks; a public release is a separate action.
7. Obtain separately authorized gameplay checks for baseline and another value,
   using a fresh skirmish and confirmed GUN weapon. Document normal closure and
   restoration. Never replace stock archives as an automatic install step.

## Current next implementation

`ArmorAuthoringAdmission` adds a bounded XML admission stage and canonical
snapshot. It proves an accepted GUN coefficient changes only the known float
slot in tokenized compiler output. It is **not** yet connected to editable
linked-stream/package publication. CLI preflight reports that distinction
explicitly. The fixed `Build-Ra3Ep1ArmorMod.ps1` remains a reproducer, not an
arbitrary-input build command.

Once this snapshot is bound to the real document/Core pipeline, changed XML
can produce a parameterized package. That closes the principal functional
gap before packaging the Armor Preview. The successful default-reader trial
does not by itself resolve queue-route failure, exact runtime file reads,
generic duplicate asset ordering or full WorldBuilder support.
