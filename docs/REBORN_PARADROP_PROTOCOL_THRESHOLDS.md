# Reborn Paradrop protocol thresholds

<!-- Reborn: protocol purchases select menus; they must not create separate power or cooldown identities. -->
Source files:

- `alpha/mods/Reborn/Data/GlobalData/Paradrop.xml`
- `alpha/mods/Reborn/Data/Allied/Structures_Campaign/AlliedAirportControlTower.xml`
- `alpha/mods/Reborn/Data/GlobalData/LogicCommandSet.xml`
- `alpha/mods/Reborn/Data/UI/Mouse/UnitAbilityButtonTemplates.xml`

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
It does **not** merge different template IDs: level 1, 2 and 3 remain distinct
powers with the original 300/330/360-second reload values. Cross-level recharge
transfer is not implemented by this flag. Runtime testing must distinguish
same-level building/sidebar synchronization from a level-up transition.

## Verification

- Exactly three canonical powers and no `_Protocol2` / `_Protocol3` IDs in the
  four active source files.
- Both objects select the same canonical command for locked/row1/row3/row5
  states, including construction after row5 has already been purchased.
- All non-Paradrop spellbook commands remain in every ranked menu.
- The combined focused graph compiles: **36 assets**, **52 local / 666 stock**
  reference occurrences, verified BIN/RELO/IMP checksum `3c67baf9`.
- Bounded readback confirms the `SHARED_SYNC` bit in all three emitted powers
  and an original retail Uprising witness.

No published mod package was rebuilt and no game was launched. Rebuild Reborn
using `alpha/EALAModStudio.exe`; check visibility and purchases in multiple
columns, then cast from the building and verify the sidebar shares its timer.
