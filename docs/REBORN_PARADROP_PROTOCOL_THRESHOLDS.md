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
XP-only `PlayerTech_Generic_Rank_*` values are not used. The original three
power IDs and their commands, OCL troop payloads and 300/330/360-second reload
values are retained.

## Sidebar visibility

<!-- Reborn: native tech visibility predicates are separate from GameDependency usability checks. -->
The initial shared-upgrade implementation restricted casting but left all three
buttons visible, including on Soviet/Japan. `GameDependency` does not replace
the native `RequiredPlayerTech` / `ForbiddenPlayerTech` visibility conditions.

Each rank now has three power variants: the original ID and `_Protocol2` /
`_Protocol3`. Each requires one actual Allied protocol purchase. Variants forbid
all higher-row threshold protocols, and the second/third variants also forbid
earlier alternatives in their own row. Consequently the authored tech
predicates select exactly one variant of the highest purchased rank. No
protocol purchases means none qualify; Soviet/Japan purchases grant none of
the required Allied techs. The Control Tower remains a usability dependency,
so its absence may disable an unlocked button rather than hide it.

Each variant has its own PlayerPowerManager, OCL handler, command and mouse /
joypad template, reusing the existing rank's button state and troop OCL.
When multiple same-row protocols are purchased, the first listed protocol
wins, irrespective of purchase order. These are engine-native tech gates;
there is no new schema attribute or game executable patch.

The variants must also be listed in both `PlayerSpellBookCommandSet` and
`AlliedAirportControlTowerCommandSet`, with a `UnitAbilityButtonTemplate` for
each variant's LogicCommand. Initially these consumer lists and mouse templates
contained only the three original commands, so only the first protocol column
was reachable despite the alternate powers passing compilation. All nine
variants are now linked through both command consumers and their mouse
command templates. Predicate checks alone do not prove this UI reachability.
The complete focused graph now passes native stream readback: **58 assets**,
**117 local / 558 stock** reference occurrences, checksum `99143e70`.
Actual second/third-column gameplay still requires an in-game check.

The old level-2/3 leech-recharge attributes were tied to two specific protocols.
They were removed because a shared upgrade is not a PlayerTech trigger. Each
power variant has its own timer; this change does **not** implement shared
cooldown transfer across levels or same-level protocol variants. Buying a
higher-priority alternative can switch the active variant and its timer. Availability, UI hiding versus disabling, recharge on
unlock and the lifetime of the overridden faction binding require a fresh-match
runtime check. XML/schema and native proof alone cannot establish these behaviors.

Compiler coverage: official PlayerTechUpgradeBinding schema, 12-byte root and
8-byte binding pairs; ordinary source includes/inheritance, typed import
resolution and mandatory source/native proof. Three original faction records
qualify against retail. The private test links a shared upgrade and power
dependency into a stream and verifies BIN/RELO/IMP readback.

The actual Paradrop source plus its three existing mod button-state definitions
compiles privately after the visibility fix: **47 assets**, **81 local / 481
stock** reference occurrences, verified stream checksum `bb929510`. Paradrop
by itself does not include those UI definitions; the normal Reborn source
graph supplies them. This supersedes the initial 23-asset compile check.

The authored unlock predicates were checked over all 512 combinations of the
nine threshold protocols: zero active levels before purchase, otherwise only
the highest purchased threshold passes the upgrade predicates. The follow-up
visibility check also evaluates all nine variants over those 512 combinations
and verifies no eligible variant for Soviet/Japan protocol sets. This is a logic
check, not an engine gameplay test.

Build Reborn using `alpha/EALAModStudio.exe`. In a new Allied match, build the
Control Tower and test a branch other than Air Power first. Buying two row-1
protocols must not grant level 2; reaching XP rank 3 without a row-3 purchase
must not grant it either. Then purchase row 3 and row 5, checking which buttons
appear and the recharge state at each transition. Test without the tower as
well. No game was launched and no published Reborn package was rebuilt during
the automated checks.
