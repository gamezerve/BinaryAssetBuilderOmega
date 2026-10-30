# Reborn Paradrop protocol thresholds

<!-- Reborn: record the authored purchase-unlock contract and distinguish compile proof from gameplay evidence. -->
Source: `alpha/mods/Reborn/Data/GlobalData/Paradrop.xml`.

| Paradrop level | Purchase any one protocol in this Allied row |
| --- | --- |
| 1 | Row 1: Air Power, Satellite Sweep, High Technology |
| 2 | Row 3: Time Bomb 1, Chrono Rift 2, Cryo Satellite 1 |
| 3 | Row 5: Time Bomb 3, Production Bonus Allies, Cryo Satellite 3 |

Each row's three purchases grant the same `PLAYER` UpgradeTemplate through
`PlayerTechUpgradeBinding_Allied`. The two original High Technology/Air Power
bindings are retained. The existing Allied PlayerTemplate already references
this binding asset, so no new PlayerTemplate or purchase button is needed.

Each power's GameDependency requires its threshold upgrade and the existing
`AlliedAirportControlTower`. Lower levels forbid higher threshold upgrades.
XP-only `PlayerTech_Generic_Rank_*` values are not used. The three power IDs,
commands, OCL troop payloads and 300/330/360-second reload values are unchanged.

The old level-2/3 leech-recharge attributes were tied to two specific protocols.
They were removed because a shared upgrade is not a PlayerTech trigger. Each
level now has its own timer; this change does **not** implement shared cooldown
transfer across levels. Availability, UI hiding versus disabling, recharge on
unlock and the lifetime of the overridden faction binding require a fresh-match
runtime check. XML/schema and native proof alone cannot establish these behaviors.

Compiler coverage: official PlayerTechUpgradeBinding schema, 12-byte root and
8-byte binding pairs; ordinary source includes/inheritance, typed import
resolution and mandatory source/native proof. Three original faction records
qualify against retail. The private test links a shared upgrade and power
dependency into a stream and verifies BIN/RELO/IMP readback.

The actual Paradrop source plus its three existing mod button-state definitions
also compiles privately: **23 assets**, **33 local / 394 stock** reference
occurrences, verified stream checksum `f2666a04`. Paradrop by itself does not
include those UI definitions; the normal Reborn source graph supplies them.

The authored unlock predicates were checked over all 512 combinations of the
nine threshold protocols: zero active levels before purchase, otherwise only
the highest purchased threshold passes the upgrade predicates. This is a logic
check, not an engine gameplay test.

Build Reborn using `alpha/EALAModStudio.exe`. In a new Allied match, build the
Control Tower and test a branch other than Air Power first. Buying two row-1
protocols must not grant level 2; reaching XP rank 3 without a row-3 purchase
must not grant it either. Then purchase row 3 and row 5, checking which buttons
appear and the recharge state at each transition. Test without the tower as
well. No game was launched and no published Reborn package was rebuilt during
the automated checks.
