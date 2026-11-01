# Reborn Paradrop: real spellbook UI and event-only synchronization

<!-- Reborn: record the installed chain and distinguish offline evidence from game behavior. -->
Updated November 1, 2026. The user reports that the previous revision still has
no sidebar shortcut and promotion makes the power ready. Creation captions for
the spellbook/clock were probably visible. Previous offline checks therefore did
not establish correct runtime UI discovery or lifecycle ordering.

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
