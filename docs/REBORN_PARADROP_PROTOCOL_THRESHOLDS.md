# Reborn Paradrop protocol thresholds

<!-- Reborn: preserve observed failures separately from isolated native-action mocks. -->
## Live diagnostic findings — October 30, 2026

The user confirmed the general Lua callback caption on Tower construction.
The row-3 model-condition callback also fires, but all selected protocol queries
returned false. Using the row event directly reaches the marker grant attempt;
the subsequent marker query still reports **NOT granted**. Cooldown transfer
remains unresolved. That query result alone cannot distinguish a failed grant
from an unavailable player-query context.

Bounded inspection of `ra3ep1_1.1.game` shows a native context prerequisite:
player resolution at `0x005B0ED0` checks ScriptEngine field `+0x18FB4` before
resolving even an explicit player name, and returns zero if it is absent.
The executable's own diagnostic calls this `Unexpected NULL player`.
`PLAYER_HAS_PLAYER_TECH` reaches this path through `0x005BC0A0` /
`0x005B1720`; `PLAYER_GRANT_SPECIAL_POWER` reaches it from `0x005B7240`.
An object Lua callback therefore does not establish that player-scoped script
actions are usable. The actual context value during the user's callback has
not been observed; a missing context is a supported hypothesis, not a confirmed
runtime finding. No game process was attached to or modified.

Read-only inspection of the published Streams BIG confirms the level-2 helper
exists and imports `PlayerTech_RebornParadropTransfer2` at the expected
SpecialPowerTemplate field offset `0x144`. This rules out a missing level-2
helper reference in that inspected package. Future work must establish a valid
player-context route or use native purchase-driven Leech behavior before more
Lua grant trials; mocked action tests cannot establish this engine prerequisite.

<!-- Reborn: protocol purchases select menus; they must not create separate power or cooldown identities. -->
Source files:

- `alpha/mods/Reborn/Data/GlobalData/Paradrop.xml`
- `alpha/mods/Reborn/Data/Allied/Structures_Campaign/AlliedAirportControlTower.xml`
- `alpha/mods/Reborn/Data/GlobalData/LogicCommandSet.xml`
- `alpha/mods/Reborn/Data/UI/Mouse/UnitAbilityButtonTemplates.xml`
- `alpha/mods/Reborn/Misc/Data/Scripts/Scripts.lua`
- `alpha/mods/Reborn/Misc/Data/Scripts/ScriptEvents.xml`

## Canonical powers

There are exactly three power templates: `SpecialPowerParadropLvl1`,
`SpecialPowerParadropLvl2` and `SpecialPowerParadropLvl3`. The `_Protocol2` and
`_Protocol3` copies, commands, handlers and UI templates were removed. Buying
a different protocol in the same row must not select a different power ID.

| Purchase any protocol in this Allied row | Canonical command |
| --- | --- |
| Row 1: Air Power, Satellite Sweep, High Technology | Command_ParadropLvl1 |
| Row 3: Time Bomb 1, Chrono Rift 2, Cryo Satellite 1 | Command_ParadropLvl2 |
| Row 5: Time Bomb 3, Production Bonus Allies, Cryo Satellite 3 | Command_ParadropLvl3 |

The original `PlayerTechUpgradeBinding_Allied` entries are retained, with the
nine additional bindings granting three shared row upgrades. The powers keep
those upgrades and the Control Tower as GameDependency prerequisites. Their
RequiredPlayerTech now checks Allied faction membership, not a particular
protocol column. XP-only Generic_Rank technologies are not unlocks.

## Menu selection

Both `PlayerSpellBookCommandSet` and `AlliedAirportControlTowerCommandSet`
start with no Paradrop command. Rank1/Rank2/Rank3 command-set variants each
contain exactly one canonical Paradrop command. All other spellbook commands
are preserved in every ranked variant.

Three native CommandSetUpgrade modules on each object select the appropriate
menu from the shared Row1/Row3/Row5 upgrades. Lower-row modules conflict with
higher-row upgrades. This replaces the earlier protocol-specific template
visibility predicates. Actual sidebar refresh, faction visibility and casting
must be confirmed in a new match: source/native proof cannot establish UI
behavior.

## Shared timers

<!-- Reborn: same-template sharing across objects is different from transferring recharge across level IDs. -->
All three canonical templates use the official `SHARED_SYNC` flag. This asks
the engine to share a player's timer for the same template across spellbook
and Control Tower handler instances. Both interfaces use the same canonical
command and template for their current level. The author's retained original
spellbook handlers are preserved; only obsolete protocol copies are removed.

`SHARED_SYNC` exists in the original Uprising schemas and retail powers such
as `SpecialPowerTelekineticProjectorDevice`. Its EP1 bit is `0x1000`.
It does **not** merge different template IDs. The three castable powers keep
their original **300 / 330 / 360 second** reload values.

## Cross-level percentage transfer

<!-- Reborn: an upgrade is not a PlayerTech trigger; bridge each shared row transition into one grant-only technology marker. -->
Both owners' level-2/3 OCL handlers now pair
`LeechRechargePercentFromSpecialPower` with
`LeechRechargePercentPlayerTechTrigger`. The triggers are the two dedicated
`PlayerTech_RebornParadropTransfer2/3` markers, not a protocol from one column.
Native Leech processing performs the actual percentage transfer.

On PlayerSpellBook **and the physical Control Tower**, the existing Row3/Row5
upgrades set reserved model conditions **USER_9 / USER_10**. The same Lua
event list refreshes at those
transitions and object creation; it does not poll each frame. The Lua bridge:

1. Resolves the real owner from full `Player/Team` paths or bare default-team
   names such as `teamPlayer_1`, `defaultSkirmishTeamPlayer_1` and `teamPlyr...`.
   The first bridge accepted only full paths and silently skipped bare teams.
   Bounded inspection of the local Uprising Lua getter confirms it returns
   the stored team name, without constructing an owner-prefixed path.
2. Checks the actual purchased Allied row-3/5 protocols across all columns.
3. Uses the existing `PLAYER_GRANT_SPECIAL_POWER` action to grant each marker
   at most once, guarded by persistent native PlayerTech ownership.
4. Grants level 2 before level 3 when both thresholds already exist.

Two hidden grant-only SpecialPower templates and inert spellbook registrations
allow that existing action to resolve the marker technologies. They have zero
reload time, no OCL payload, no UI command and no aircraft. They are not
protocol-specific copies of the three castable Paradrop powers.

The retail Uprising Scripts.lua and ScriptEvents.xml were read from the local
Misc archive. Their original Lua prefix and all original event definitions are
preserved; only the Reborn bridge definitions are appended. The two script
files are packaged automatically through the mod's normal Misc BIG.

At 50% remaining, a 300-second level-1 cooldown should become **165 seconds**
remaining on level 2, then **180 seconds** on level 3 at the same percentage.
Buying another protocol in an already-unlocked row must not repeat the grant
or reset the active timer. Already-unlocked rows on creation also use the
persistent marker guard.

## Verification

- Exactly three castable canonical powers (plus two grant-only helpers) and no `_Protocol2` / `_Protocol3` IDs in the
  four active source files.
- Both objects select the same canonical command for locked/row1/row3/row5
  states, including construction after row5 has already been purchased.
- All non-Paradrop spellbook commands remain in every ranked menu.
- The combined focused graph compiles: **40 assets**, **64 local / 670 stock**
  reference occurrences, verified BIN/RELO/IMP checksum `6640842d`.
- Bounded readback confirms the `SHARED_SYNC` bit in all three emitted powers
  and an original retail Uprising witness.

- The Lua bridge passes **224 isolated checks** covering all nine cross-column
  promotion pairs, full and bare team names, repeated purchases/events, numeric/boolean conditions,
  player isolation, custom player names and level-2-before-3 grant ordering.
  Native game actions and the proportional-timer response are **mocked** in
  this test; this does not prove the live callbacks/action dispatch.
- Private Misc package readback verifies packaging integrity only; it does
  not establish that the packaged scripts are safe or correctly loaded in game.

<!-- Reborn: record the rejected map-local experiment so it is not repeated as a supported test path. -->
`DEBUG_STRING` is a script-debug output and is not reliable normal HUD text.
The user reported no diagnostic text and a cooldown that still becomes ready.
The Hostile Hostel map-local script experiment was withdrawn after the user
reported a crash. The two map-local files are absent from the live Misc source.
Do not repeat map-specific overrides. Map compilation and packaging have not
been established for this experiment; discovering a native script loader and
passing archive readback were insufficient validation.

Visible diagnostics use `SHOW_MILITARY_CAPTION(localizedKey, 6)`, whose two
argument types were checked against the retail action registration. Six
labels in Mod.str distinguish Tower callbacks, unresolved
owners, and successful/failed level-2/3 marker grants. All 224 mocked Lua
checks pass with mocked native boundaries. Marker ownership alone does
not prove native Leech transfer. Global Lua loading and callback delivery
remain unverified; the cooldown reset is not fixed.

The live upgrade-to-model-condition callback, player-tech grant and resulting
native Leech transfer still require an in-game test. No published mod package
was rebuilt and no game was launched during withdrawal. A previously published
Misc BIG may still contain the removed map-local files until Reborn is rebuilt
through `alpha/EALAModStudio.exe`. Do not use the old map-local diagnostic package.
