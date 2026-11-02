# Reborn Paradrop: real spellbook UI and event-only synchronization

<!-- Reborn: user-confirmed sidebar behavior is retained while replacement Towers rejoin the existing clock after construction. -->
## Latest: rebuilt Tower cooldown attachment (2026-11-02)

The user confirmed that sidebar visibility and cooldown synchronization now work.
A remaining runtime case affected a Tower sold/destroyed during cooldown and then
rebuilt: the sidebar kept the clock, but the new building's unit ability became
ready until a rank change synchronized it again.

The former lifecycle synchronization ran at `OnCreated`, with no
`OnBuildingComplete` binding. Native completion can initialize receiver powers
after that early write. `RebornParadropTowerBuilt` now attaches the completed
Tower to the existing owner clock and schedules the established one-shot
post-initialization synchronization. It does not restart/promote the clock,
spawn another provider, change sidebar gates, or add periodic UI writes.

Only `Scripts.lua` and `ScriptEvents.xml` are changed for this fix. Lua 5.1 tests
now include a simulated native countdown reset after OnCreated, construction
delay, complete loss of Towers, callback wrappers and rank 1/2/3 rebuilding:
2,276 mock checks passed. The event file passes EA's ScriptEvents XSD. The former
mock tested immediate replacement creation but did not simulate later native
power initialization; that coverage gap is now explicit. Engine delivery of the
new completion callback and the final visual result still need a gameplay test.

Rebuild Reborn through EALAModStudio, start a fresh match, cast Paradrop, sell or
destroy the Tower, and rebuild it while recharge is active. The completed
building and sidebar should display the same active-level remaining cooldown.
No map override, game launch, automatic Reborn package build, commit or push was
performed for this change. SampleMod investigation was stopped at the user's request.


<!-- Reborn: record the installed chain and distinguish offline evidence from game behavior. -->
Updated November 1, 2026. The user reports that the previous revision still has
no sidebar shortcut and promotion makes the power ready. Creation captions for
the spellbook/clock were probably visible. Previous offline checks therefore did
not establish correct runtime UI discovery or lifecycle ordering.

## November 2: real-spellbook native UI entries replace the failed provider route

<!-- Reborn: keep failed runtime experiments separate from the currently installed sidebar candidate. -->
Adding SPELL_BOOK to the provider did not produce a shortcut. Read-only capture
of process 31552 confirms the provider's role-6 callback, a native power manager
with Enabled=1 and one bound power instance. The real spellbook has no Paradrop
manager after the provider refactor. An enabled manager on an auxiliary object
therefore did not establish sidebar discovery. No new inference from the provider
creation caption is used as runtime success.

The new candidate removes all three providers, spawn requests and event lists.
The actual PlayerSpellBook now owns nine protocol-gated UI entries: three ranks
by three Allied branches. Each uses the real purchased protocol as RequiredPlayerTech;
higher-row techs forbid lower ranks. Within one row, earlier-priority purchased
branches forbid later entries, so multiple purchases never duplicate the button.
No private PlayerTech grant or world-script player context is required.

These are nine additional native SpecialPowerTemplate UI-view identities, not a
claim that the engine can express an OR over three RequiredPlayerTech references.
Their OCL effects/targeting match the existing canonical level, and all cast
conditions enter the existing real-book level callbacks. The three canonical
Tower power IDs and all gameplay payloads remain unchanged. At each existing
cast/lifecycle/promotion event, the same authoritative clock writes all UI-view
countdowns alongside canonical countdowns. Native timers advance themselves;
there are still no periodic UI writes. Switching protocol branches within a rank
therefore selects an already synchronized timer.

Each view includes its manager, OCL handler, mouse/joypad/unit-ability definition
and command on the real spellbook. The ordinary static command sets retain all
retail entries. No Tower source, map or game installation was changed.
Private compilation and stream readback pass for 103 assets. 1,801 Lua mock
checks cover casts, promotions, stable wrappers, player isolation and cross-view
countdowns. All 512 combinations of the nine relevant protocol purchases select
zero entries before purchase or exactly one entry at the highest purchased tier.
Independent native comparison confirms byte-identical canonical power records
and a byte-identical complete Tower payload. These checks do not execute native
UI rendering; a fresh-match sidebar test remains required.

Installed source files: Paradrop.xml, LogicCommandSet.xml, Scripts.lua and
ScriptEvents.xml. No automatic user package publication, process modification,
game launch, commit or push occurred.

## November 2: provider creation confirmed; missing spellbook initialization flag

<!-- Reborn: record a concrete native initialization prerequisite instead of equating the Lua creation caption with sidebar registration. -->
The user sees the sidebar-provider creation caption but no shortcut. That confirms
the provider's Lua creation callback; it does not confirm its OCL power's native
initialization. Retail SpecialPowerModule/OCLSpecialPower creation virtual slot
at 0x00774550 reads the owner's SPELL_BOOK bit and calls 0x0075ACB0 only when set.
The old providers omitted that bit. The called path initializes the power and
registers IS_PLAYER_POWER instances with the native power notification route.

All three providers now include SPELL_BOOK while retaining non-INERT AI/Lua
registration. No manager, command, canonical power, clock or timer logic changed.
The private 67-asset build and stream readback pass. An independent before/after
comparison of every native payload confirms exactly one changed byte per provider
(the SPELL_BOOK bit) and byte-identical payloads for all other 64 assets. INERT
remains clear in all three emitted provider masks. The new sidebar result still
requires runtime validation; it is not inferred from offline checks.

Only Paradrop.xml was installed. The user package was not rebuilt automatically,
and no game was launched, modified or stopped. No commit or push was made.

## November 2: failed player-context gate replaced by native UI owners

<!-- Reborn: supersede the failed private-tech visibility candidate with observed runtime evidence. -->
The visibility-tech candidate failed in the user's match: the initial sidebar
was empty, but after protocol purchase and Tower construction the grant-failure
caption appeared and the Tower power remained unavailable. That regression was
caused by requiring a private tech that the object-Lua grant path did not supply.

Read-only capture of process 4364 verified Player_2 as the real script name,
correct Tower/clock/spellbook ownership, all three Lua roles, an empty player-alias
cache and the failed-visibility marker. ScriptEngine+0x18FB4 was null. Native
player-mask resolution at 0x005B1720/0x005B0ED0 requires that world-script context
before resolving explicit names. Expanding alias spellings is insufficient;
the staged alias-only candidate was not installed.

The current candidate removes the three private PlayerTech assets and every
player-context action/condition from this feature. Canonical powers return to
the Allied faction requirement; existing protocol GameDependency rules remain.
Their IDs, payloads and 300/330/360-second reloads are unchanged. The persistent
clock and event-only timer arithmetic remain in place.

Only the current tier's invisible provider owns its PlayerPowerManager. The
real spellbook and Tower retain their canonical OCL handlers but no longer
advertise all three sidebar managers. At an unlocked row the clock grants an
OBJECT request upgrade, using the already proven strong-OCL creation route.
The new provider has non-INERT AI/Lua registration, one canonical power and its
matching command. A higher row retires the old provider through an object event;
late lower-tier spawns reject themselves. Provider callbacks use canonical
native-ID handles and role 6, so casts and new-provider creation receive the
existing shared timer. No additional power/cooldown identity is introduced.

Native PlayerPowerManager code at 0x00757040 evaluates required/forbidden techs,
and its lifecycle owns availability notifications. This supports using native
manager lifetime for advertisement, but does not establish runtime UI success.
Private compilation and stream readback pass for 67 explicit assets. 1,794 Lua
mock checks cover provider requests, retirement, stale queued spawns, canonical
sidebar casts, ownership isolation, failed requests and retained reload fractions.
A new-match UI test is still required. The running match and published package
were not changed; no commit or push was made.

## November 2: cooldown confirmed; sidebar visibility gates

<!-- Reborn: retain user-observed timing evidence separately from the next UI candidate. -->
The user confirms that clock creation now reports its active caption, a Tower
cast also starts the sidebar cooldown, and promotion from rank 1 to rank 2
preserves the remaining reload fraction. All three sidebar buttons remain
visible, with unavailable ranks disabled. The lifecycle correction is therefore
runtime-supported for this tested path; sidebar hiding still needs validation.

GameDependency controls cast availability. The normal retail rank pattern uses
RequiredPlayerTech and ForbiddenPlayerTech to exclude sidebar ranks. The three
powers now require separate zero-cost private visibility techs; higher visibility
techs forbid the lower ranks. Shared protocol row upgrades remain the source of
rank progression, regardless of the chosen Allied protocol branch.

At existing unlock/promotion/lifecycle refresh events, Lua grants the required
visibility tech through PLAYER_GRANT_SPECIAL_POWER using the existing canonical
power. Retail action 528 (0x005E1C98 -> 0x005B7240) grants the template's required
tech; it does not cast that power. No helper power or additional cooldown was
introduced. NAMED_OWNED_BY_PLAYER(player, namedClock) validates every player alias
before a grant; PLAYER_HAS_PLAYER_TECH verifies grant success. Failed resolution
or readback produces a specific caption without invalidating the saved clock.
The existing immediate/deferred timer publication follows visibility updates.

Private compilation/readback passes for 61 assets. 1,793 Lua mock checks cover
no visible tier before unlock, one tier at rows 1/3/5, nonmatching player slots,
player isolation, failed grant/recovery, and preserved cooldown fractions.
The UI result remains a new-match runtime check. Modified sources: Paradrop.xml,
Scripts.lua and Mod.str. No automated Reborn package publication, game launch,
commit or push occurred.

## November 2: live registration evidence and lifecycle correction

<!-- Reborn: distinguish observed native objects from unexecuted callback assumptions. -->
Read-only inspection of the running match confirmed that the current spellbook,
Tower and clock templates were loaded. Objects 1, 726 and 727 belonged to the
same player/team. Both before the first protocol and after a Tower cast, Lua's
role roster contained only Tower wrappers; clock/spellbook roles were absent.
The deployed Scripts.lua and ScriptEvents.xml exactly matched current sources,
and all three runtime LUAEventList modules pointed to their correct event lists.

Native registration at 0x0059F150 requires an AI interface (object+0x384) and
rejects INERT before considering ForceLuaRegistration. Live reads showed no AI
interface on the clock and the INERT exclusion set on the spellbook. The clock
now adds AIUpdate with enemy auto-acquisition disabled. The mod's spellbook
retains SPELL_BOOK/IMMOBILE/IGNORES_SELECT_ALL and removes INERT; stock SageXML
is unchanged. ForceLuaRegistration cannot bypass either of these early gates.

Two separate Tower callbacks also supplied different Lua wrapper tables for the
same native object ID 726. Callback entry points now canonicalize handles using
the native ID at the start of ObjectDescription. The retail formatter at
0x00761ED0 starts both named/unnamed descriptions with `Object %d `. Destruction
removes the canonical cache entry. Native saved upgrade bits, player isolation,
300/330/360-second reloads and event-only UI synchronization are retained.

Private source/native compilation and stream readback pass for 58 assets.
1,600 Lua mock checks include fresh callback wrappers, promotion, destruction,
player isolation and failed storage. New runtime event delivery remains untested.
The user currently sees all three sidebar ranks (two disabled after unlock);
correct hide/show behavior is still unresolved and is not claimed fixed here.
Only Paradrop.xml and Scripts.lua were installed. No package publication, game
launch/process write, map override, commit or push occurred.

## November 2: explicit clock-creation route

The positive-health revision still produced only the Tower creation caption;
casts/promotions still reported no registered clock. Health alone was not the
runtime fix. The new revision targets the missing child/event path directly:

- The known-working Tower OnCreated callback grants a one-shot **OBJECT** request
  upgrade, the same upgrade category as the original Hammer creation witness.
  A named-unit/object upgrade query immediately verifies the grant. Failure has
  its own `Paradrop clock creation request failed` caption.
- Tower ObjectCreationUpgrade now uses **UpgradeObject=OCL_RebornParadropClock**
  with a one-frame Delay and that OBJECT trigger, rather than the protocol PLAYER
  upgrade and weak ThingToSpawn name lookup.
- The OCL holds a strong reference to the clock. The clock retains health 1,
  ObjectSMCHelper and native saved bits; it adds the retail spellbook's empty
  ScriptedModelDraw pattern and a small noncolliding geometry for event delivery.
- Tower creation no longer reports a missing child during the expected deferred
  creation interval. Cast/rank failures still report honestly.

Targeted static inspection of retail ObjectCreationUpgrade update code at
0x007C66C0 identifies the OCL path (data+0x24, call 0x007366C0) separately from the
ThingToSpawn lookup (data+0x34, call 0x006FFFE0). This proves the two code paths,
not successful execution of our new source. The drawable/lifecycle explanation
remains a runtime hypothesis; no further sidebar behavior is claimed fixed.

Private source/native build: **58 assets** with stream readback. **1,594 Lua mock
assertions** and topology checks verify explicit request/readback, OBJECT trigger,
strong OCL target, prepared drawable/body/helper, player isolation and unchanged
zero periodic UI writes. Engine spawning and sidebar visibility remain unverified.
The next useful game observation is `Paradrop logic clock active` shortly after
building the Tower, independently of protocol purchase order. No game launch,
user package publication, commit or push was performed.

## Latest failed runtime test and package inspection

The user still sees `Paradrop clock is not registered; native cooldown preserved`.
The player-tag revision did not establish a registered persistent clock in game.
The deployed Misc BIG contains byte-identical current Scripts.lua, including the
new binding function. The published stream also contains the real book, clock,
custom hierarchy/mesh/container and the current matrix bytes. This rules out a
stale *published file* but does not prove which package the running game opened.

BaseObject supplies `ActiveBody` without health values. Official ActiveBody XSD
sets MaxHealth=0 and InitialHealth=-1. The clock now overrides that inherited body
with MaxHealth=1 and InitialHealth=1, retaining its id and unattackable flags.
This corrects a concrete lifetime configuration defect; immediate destruction as
the runtime cause is still an inference until a fresh match is observed. Source
preparation confirms one merged positive-health body; the 56-asset private native
build and stream readback pass. No user package was rebuilt automatically.

The previous radar model matrix claim was wrong: original EP1 JURADARSHIP_SKL
matrices have M33=0 too. Only the previous four-attribute matrix additions/comment
have been rolled back. The original radar mesh rebuild passes comparison against
WorldBuilder retail data (1,764 vertices and 1,484 triangles, declaration, packed
channels, bone data and material import identities). The custom mesh is not hidden
in the published BIN, its three textures exist in static, and its container/import
chain is present. Its invisibility and the sidebar failure are still unresolved;
no additional model or UI edits were made without a demonstrated cause.

## Player identity revision after the latest game test

The user observed `Paradrop clock is not registered; native cooldown preserved`
on building creation, casts and promotions. The team-name matching route therefore
failed in the real game; the earlier name-based mock success did not prove runtime
ownership. It has been removed, rather than relaxed to mix players of the same side.

The first logic clock grants its owner one of eight PLAYER upgrade identity tags.
Tower, book and replacement clocks inspect these through the PLAYER-capable
ObjectHasUpgrade binding. Identity is native player state, while remaining time
stays in the clock's OBJECT upgrades. Duplicate clocks reuse the same identity and
retire. No ordinary tick grants upgrades to visible objects. The supported slot
count is for the game's skirmish players; save/load roster reconstruction remains
unverified. Failed identity grants preserve native power timers.

Three matching PlayerPowerManager modules are also placed directly on the Tower.
The real book, its initial/ranked commands and mouse templates remain connected.
This is a sidebar candidate, not a claim that its visibility policy works in game.

Validation: 56 private native assets, 1,592 Lua mock assertions with ObjectTeamName
made an error on every call, isolated-player/duplicate-clock/timer controls, and a
private Misc readback. No game or user mod publication was performed. A fresh-match
runtime test is required for clock registration, native identity grants, sidebar
appearance and rank cooldown transfer.

## Previous candidate correction (failed runtime ownership)


- Keep all three canonical commands in the initial PlayerSpellBookCommandSet,
  so native discovery does not depend solely on a later CommandSetUpgrade.
  Rank-selected command sets still contain one Paradrop command each. Runtime
  filtering of the initial entries remains to be verified.
- After a creation/rank event, apply the saved countdown immediately and schedule
  one additional application on the logic clock's USER_11 expiry. A pending flag
  ignores initial negative-condition events and prevents repeated publication.
  This targets a possible native upgrade-order overwrite, not a proven engine
  trace. The native action truncates duration to integer seconds in the active
  world branch, so the delay is one second, not 0.1 seconds.
- Normalize qualified `teamPlayer_1/...`, `defaultSkirmishTeamPlayer_1`, and
  unqualified `Player_1` consistently before linking actors. Unknown empty owner
  values still cannot match. Missing clocks are reported during refresh too.
- Correct the row-one callback to pass no event payload as a numeric rank.

Private native build: 48 explicit assets. Lua 5.1 checks: 1,592 assertions,
including a simulated engine reset after the early write and one-shot recovery,
owner aliases, player isolation and zero ordinary-tick UI writes. A three-file
private Misc BIG was independently read back. These are offline checks; neither
runtime root cause is claimed confirmed or resolved. Rebuild in EALAModStudio
and test a fresh match; after promotion the new caption is
`Reborn: Paradrop post-upgrade timers synchronized.`

## Actual sidebar chain

The Allied PlayerTemplate uses `PlayerSpellBook` for both SpellBookSP and
SpellBookMP (`alpha/SageXML/System/PlayerTemplates.xml`). The installed override
now inherits that real object and adds:

- Three canonical OCL handlers and their PlayerPowerManager modules.
- Upgrade-selected PlayerSpellBookCommandSetRank1/2/3. The initial set now also
  lists all three commands for discovery; each ranked set contains its own rank.
- A Lua event list with explicit spellbook role bindings, including a row-1
  registration event and ObjectSMCHelper for timed cast model conditions.

The corresponding LogicCommand points to SpecialPowerParadropLvl1/2/3, and each
has an emitted mouse PlayerPowerButtonTemplate. Tower unit abilities use those
same power IDs. Invisible sidebar providers and their command sets were removed.
The other inherited spellbook powers and user command-set definitions are retained.

| Purchased row in any Allied protocol branch | Full cooldown | Aircraft |
| --- | --- | --- |
| 1 | 300 seconds | 1 |
| 3 | 330 seconds | 2 |
| 5 | 360 seconds | 3 |

## Avoiding periodic UI rebuilds

Previously the Tower received clock-bit OBJECT upgrades and three countdown
writes every second; it also executed a hidden pulse power. These operations
could refresh its visible command set or ability state.

A logic-only `RebornParadropClock` now owns the nine remaining-second upgrade bits
and two rank bits. A Tower's row-1 ObjectCreationUpgrade creates it. It has no
command set, player-power manager or special power. Duplicate clocks of the same
owner retire before publishing any ready state, and the surviving clock remains
after all Towers are destroyed.

The one-second pulse uses the existing native action
`UNIT_SET_MODELCONDITION_FOR_DURATION(self, "USER_8", 1, 100)`: duration in seconds,
then horde-member percentage. Retail dispatch at `0x005DD53D` calls `0x005C2DB0`,
which uses the direct object resolver and native timed-condition helper. No
periodic special-power activation is used.

A tick changes only the logic object's saved bits and rearms that condition.
It performs **zero countdown writes** on the Tower or spellbook. Native power
cooldowns advance themselves. Synchronization happens on casts, protocol events
and object creation. The explicit role roster gives direct references to the
real spellbook, avoiding reliance on spatial broadcasts reaching a SPELL_BOOK.
OnDestroyed callbacks remove dead references.

Promotion preserves `ceil(remaining * newReload / oldReload)`: 260 seconds of
level 1 becomes 286 at level 2. All interfaces use one authoritative saved clock.

## Confirmed earlier causes

Retail `ObjectHasUpgrade` (`0x005AA9C0`) reads PLAYER upgrades only. OBJECT clock
bits now use `SET_UNIT_REFERENCE` followed by `UNIT_HAS_UPGRADE` (`0x005C6EE0`).
The named reference is necessary because EvaluateCondition rejects userdata,
whereas ExecuteAction accepts direct object handles. Failed readback cannot
publish zero and erase a real native cooldown.

The mouse button definitions had been commented out and absent from the static
catalog; they are now emitted. The previous workaround also left the real
spellbook disconnected: its rank command sets appeared as unused in the full
build log. Explicit XML role bindings replaced failed template-name detection.

## Earlier revision evidence

The following records describe the preceding revision, which failed the user's
runtime test. See the latest candidate above for current changes and evidence.

## Files and evidence

- `alpha/mods/Reborn/Data/GlobalData/Paradrop.xml`: real book override, clock,
  canonical powers, mouse buttons and protocol bindings.
- `alpha/mods/Reborn/Data/Allied/Structures_Campaign/AlliedAirportControlTower.xml`:
  clock spawn, receiver events and canonical unit-ability handlers.
- `alpha/mods/Reborn/Misc/Data/Scripts/Scripts.lua`: named OBJECT queries,
  saved-state clock, direct event synchronization and role dispatch.
- `alpha/mods/Reborn/Misc/Data/Scripts/ScriptEvents.xml`: Clock/Tower/Spellbook
  creation, destruction, rank, cast and expiry bindings.
- `alpha/mods/Reborn/Misc/Data/Mod.str`: temporary diagnostic captions.

Private build: **48 explicit assets**, including the full inherited spellbook
and all three mouse power templates, with stream readback. **1,588 Lua mock
checks** cover zero periodic UI writes, zero visible clock-upgrade writes, no
pulse special-power calls, fraction transfer, casts from both interfaces,
new/replacement Towers, loss of all Towers, duplicate clock retirement, player
isolation and failed-storage preservation. XML checks verify the complete
spellbook-command-power-mouse-button chain; a private three-file Misc BIG is
read back independently. These checks do not execute engine UI or event delivery.

Rebuild through EALAModStudio and use a fresh Allied match. Check the real
spellbook/logic-clock captions, exactly one current-rank sidebar command, natural
Tower countdown without periodic disappearance, and rank promotion during reload.
Pause/save-load behavior and engine lifecycle restoration remain runtime checks.
No map override, game installation edit, automated Reborn publication, game launch,
commit or push was performed.
