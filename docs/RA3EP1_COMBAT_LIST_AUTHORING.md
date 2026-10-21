# Source-sized ArmorSet and hardpoint Weapon lists

<!-- Reborn: existing EP1 native processors are reused; authoring no longer has to reproduce a captured Conscript list shape. -->
Use the existing `alpha/EALAModStudio.exe` workflow. In currently supported
GameObject graphs, ArmorSet entries and WeaponSetUpdate hardpoint/Weapon rows
are now authored lists, not copies of the stock unit's three armor sets and
two weapons. They compose with variable Behaviors, edited display names,
compatible named references, includes, definitions and instance inheritance.

## What is editable

- Add, remove or omit all `ArmorSet` entries. `Armor` is required by the official
  XSD; `DamageFX` is optional. Conditions may use any official ArmorSetType list.
- Add, remove or omit all `WeaponSlotHardpoint` entries inside WeaponSetUpdate.
- Add, remove or omit all `Weapon` rows inside each hardpoint. Template and
  Upgrade imports follow their optional source presence and real named targets.
- Edit hardpoint ID, interleaving, choice criteria and boolean fields; edit each
  weapon's Ordering, status/forbidden-status masks and both EP1 boolean fields.
- Omitted optional attributes use processor/schema defaults. Boolean `1`/`0`
  lexicals are canonicalized. Weapons are stably sorted by native Ordering before
  Core occurrence assignment, implementing the official `xas:sortBy` declaration.

```xml
<!-- Reborn: ordinary EA XML, placed in the official GameObject sequence. -->
<ArmorSet Armor="AlliedConYardArmor" Conditions="VETERAN SHRINK_EFFECT" />
<!-- Reborn: this module belongs inside the GameObject Behaviors element. -->
<WeaponSetUpdate id="ModuleTag_CustomWeapons">
  <WeaponSlotHardpoint ID="1" AutoChooseWeapon="true">
    <Weapon Ordering="PRIMARY_WEAPON" Template="JapanArcherInfantryEnergyBow"
            Upgrade="Upgrade_Veterancy_HEROIC" />
  </WeaponSlotHardpoint>
</WeaponSetUpdate>
```

The former 32-ArmorSet, 8-hardpoint and 16-weapon trial quotas are removed.
The independent literal weapon metadata writer also no longer has its old
16-KiB fragment quota. Actual checked source/graph allocation bounds still apply;
this is not a claim that the game supports an unlimited useful number of slots.

## Implementation and remaining scope

The existing 12-byte ArmorTemplateSet and separate EP1 44/28/80-byte weapon ABI
already implement the loops. The source gate now admits their variable shape.
ConscriptArmorSetContract binds each row/condition/import to authored values and
checks its captured metadata grammar. ConscriptWeaponSetContract does the same
for hardpoint/weapon scalar words, masks, defaults and actual Core target IDs.
The generic schema metadata producer is cross-checked against the existing
independent literal stock-qualified weapon writer and native-tail round trip.
Other siblings retain their stock-qualified layout checks; actual output is not
rewritten into a stock shape.

Zero ArmorSet entries retain a real zero-count metadata declaration rather than
silently restoring old rows. Optional reference slots really disappear from IMP;
common reference occurrences are reindexed consistently across the entire object.

The current EP1 WeaponSet processor has no implemented turret/hierarchical-turret/
contained layout; those still fail explicitly. Newly defining ArmorTemplate or
WeaponTemplate assets alongside a custom GameObject is now supported by the
[mixed asset route](RA3EP1_MIXED_ASSET_AUTHORING.md), within its implemented weapon
topology. The examples above use existing catalog assets. This does
not unlock every other GameObject owner, arbitrary nested module structure,
models, textures or audio.

## Validation

The grouped complete-object stream/BIG test covers 40 ArmorSet entries, 10
hardpoints and 170 weapons together, including optional DamageFX/Template/Upgrade
presence and initially unsorted weapons. It also covers one-entry and completely
omitted lists. The developer-slot driver suite combines these capabilities with
a new object ID, explicit recruitment command, variable Behaviors and real
includes/definitions/inheritance. It keeps user Reborn sources/output untouched,
publishes only into isolated alpha/.work output, and restores developer data.
No game is launched by these checks; new list behavior has no independent
in-game observation yet.

| Complete stream/BIG case | Metadata bytes | Native bytes | Reference occurrences |
| --- | ---: | ---: | ---: |
| 40 armor sets, 10 hardpoints, 170 weapons | 110268 | 53488 | 517 |
| Armor/hardpoint lists omitted | 88308 | 39128 | 267 |
| One armor set, one hardpoint/weapon | 88596 | 39248 | 271 |

All three cases pass with 29 native and 39 metadata corruption refusals. The
22-check XML diagnostics suite and original two-stock-graph weapon ABI/metadata
suite (16 invalid input and 54 metadata refusals) also pass. Real missing armor
and invalid Ordering controls report their actual source coordinates rather
than being rejected as differences from a stock example.
