# Explicit mixed GameObject asset builds

## What changed

The normal alpha Studio build accepts one complete supported GameObject and
multiple explicitly declared ArmorTemplate, WeaponTemplate, LogicCommand and
LogicCommandSet assets in its source/include graph. These are freshly compiled
assets, not copies of stock binary payloads or automatically invented commands.
The existing single-object and object-plus-command routes remain available.

October 22: a LogicCommandSet may also be the primary asset with no GameObject.
Standalone command-set graphs can emit several sets and explicit support assets;
they reuse the same EP1 native serializer and stock/runtime stream references.
No dummy unit is generated. For example, this is a complete minimal mod.xml:

```xml
<!-- Reborn: standalone command-set override; no recompiled unit or hidden command declaration is required. -->
<AssetDeclaration xmlns="uri:ea.com:eala:asset">
  <LogicCommandSet id="SovietBarracksCommandSet">
    <Cmd>Command_ConstructSovietAntiInfantryInfantry</Cmd>
  </LogicCommandSet>
  <LogicCommandSet id="MyBarracksCommands" inheritFrom="SovietBarracksCommandSet"/>
</AssetDeclaration>
```

Cmd list order follows the source. An override replaces the stock asset's complete
list; retain whichever stock commands you still want. A new set ID only becomes
visible when a consuming GameObject points to it. Missing strong Cmd targets fail.
For source inheritance, use the AssetDeclaration/include context containing the
visible base rather than a bare inherited LogicCommandSet root.

GameObject SelectPortrait/ButtonImage and their transformed counterparts use
ImageWeakRef (PackedTextureImage), and CommandSet uses LogicCommandSetWeakRef.
Their nonempty IDs now check emitted mod plus stock identities and report missing
targets as source-positioned warnings. This does not create native imports for
weak references or enable an image resource compiler. Empty optional names remain
valid. Other unimplemented dynamic-name checks and inherited fields absent from
raw emitted source are not claimed as fully audited.

Local typed IDs override stock lookup. New armor/weapon IDs can therefore be used
directly by the object's ArmorSet and WeaponSetUpdate. Local strong dependencies
are topologically sorted before Core stream serialization. Stock dependencies
remain external references to static/global manifests. Duplicate local typed/hash
identities, missing targets and local strong cycles fail instead of guessing.

Includes, definitions/expressions and actual Core same-type `inheritFrom` overlays
compose with these declarations. `type="instance"` bases remain source-only;
`type="all"` includes emit their declarations. Inherited defaults are not injected
into a child before the overlay. Official schema constraints still apply.

## Authoring

Open `alpha/EALAModStudio.exe` and build your mod as usual. Put explicit declarations
in its `data/mod.xml` or files included with `type="all"`. For example, alongside
the complete GameObject and WeaponTemplate source:

```xml
<!-- Reborn: this fragment declares support assets; the complete supported object and weapon source must also be included. -->
<AssetDeclaration xmlns="uri:ea.com:eala:asset"
                  xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance"
                  xmlns:xi="http://www.w3.org/2001/XInclude">
  <ArmorTemplate id="MyArmor" Default="100">
    <Armor Damage="GUN" Percent="50"/>
    <Armor Damage="CANNON" Percent="25"/>
  </ArmorTemplate>
  <ArmorTemplate id="MyVeteranArmor" inheritFrom="MyArmor" Default="50"/>
  <LogicCommand id="Command_ConstructMyUnit" Type="UNIT_BUILD">
    <Object>MyUnit</Object>
  </LogicCommand>
</AssetDeclaration>
```

Set the complete object's `id="MyUnit"`, point its armor rows to the new armor IDs,
and its hardpoint weapon rows to your new weapon IDs. To expose a new production
button, explicitly override `SovietBarracksCommandSet`: retain its wanted stock
`Cmd` entries and append `<Cmd>Command_ConstructMyUnit</Cmd>`. Merely declaring the
new object does not add it to a production list. A new localization key also needs
the corresponding resource; no translation text is invented.

Final versioned Streams BIG/SKUDef publication and optional Misc packaging are
unchanged. Private `build.json` reports every emitted asset, local dependency
edges and whether the declared command/set chain reaches the new unit.
`DeclaredBarracksRecruitmentChain` is offline linkage evidence, not gameplay proof.

## Current real processor scope

- GameObject, when present: one emitted complete object using the implemented Conscript-derived
  EP1 owners/layouts, including the existing editable fields and variable lists.
  This change does not implement arbitrary new module families or multiple emitted
  GameObjects in this route.
- ArmorTemplate: existing EP1 armor processor, including sparse rows and defaults;
  prepared same-type inheritance is supported. Its unsupported dependency/custom
  payload forms are not silently accepted.
- WeaponTemplate: the recovered fixed single-DamageNugget topology, with new IDs
  and metadata-bound numeric/enum/boolean fields, timing records and existing
  populated sound/FX slots. Range, clip size and damage are not pinned to the
  stock AK47 values. Other nugget families, extra nuggets, additional pointer/list
  allocations or changing the reference-slot topology still need implementation.
  Nonfinite values and authored compiler discriminator `TypeId` are rejected.
- LogicCommand: implemented UNIT_BUILD/Object form, not every command payload.
- LogicCommandSet: actual native Cmd array with source-sized count; its ABI is
  independently matched against the stock Soviet Barracks set.

No W3X/TGA/DDS/audio/localization processor is added by this change. XML schema and
source-coordinate checks remain; schema-valid but unimplemented layouts fail.
Instance hashes still use the existing experimental normalized-source policy.

## Verification

The grouped developer regression declares seven assets in consumer-first order:
new unit, Barracks command set, new UNIT_BUILD command, two local armors and two
local weapons. The second armor and weapon inherit from their local bases; weapon
damage uses an imported definition. A second successful build reverses declaration
order. Negative cases cover missing weapon/command targets, duplicate identities,
extra DamageNugget topology, nonfinite weapon data and authored TypeId.

Tests check actual manifest/BIN/RELO/IMP readback, per-type fingerprints, local
edges, deterministic stream serialization and final BIG/SKUDef publication.
Failed input must preserve the last successful output. Tests use RebornTest only,
restore its original source and never build/modify the user's Reborn or launch
the game. The combined new-ID graph still requires a controlled gameplay test.

October 21 verification: all 10 grouped live-SDK controls passed, including the
restored developer build and its existing recruitment receipt. XML diagnostic
regression passed 25 checks, source-inheritance regression passed, and the legacy
authored-weapon package regression passed. The compiler built with zero warnings
and zero errors; the stable alpha compiler was synchronized to the new binaries.

October 22 verification was intentionally short: standalone stock override plus
inherited new-ID set published successfully, a missing Cmd failed without replacing
the package, and the restored developer source/recruitment receipt passed. The
dependency audit regression covers the three weak use sites, empty optional IDs,
and local/stock resolution. A read-only audit of the user's intentional portrait,
button and command-set typos reported all three with their original coordinates.
The user's source was not changed or rebuilt, and no game was launched.
