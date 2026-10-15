using System;
using System.Collections.Generic;
using System.Linq;

namespace BinaryAssetBuilder.XmlCompiler;

// Reborn: explicit scalar/enum/flag/weak/string/strong-slot descriptors drive a partial projection, not implicit indirect-graph admission.
public static class Ep1GameObjectRootValueTokens
{
    // Reborn: independent stock contracts include two 40-byte inline KindOf masks, not native allocation pointers.
    public static IReadOnlyList<Ep1GameObjectScalarTokens.Field> ExtraFields { get; } = Array.AsReadOnly(new Ep1GameObjectScalarTokens.Field[] {
        new("KindOf",4,0x9AFC9193u,40), new("RadarPriority",84,0x9D109CB8u,4), new("Side",120,0x6FAC143Bu,4),
        new("EditorSorting",132,0xD996CC2Du,4), new("CommandSet",152,0x85D44B5Du,4),
        new("SelectPortrait",156,0x3CBE9E76u,4), new("SelectPortraitTransformed",160,0x3CBE9E76u,4),
        new("ButtonImage",164,0x3CBE9E76u,4), new("ButtonImageTransformed",168,0x3CBE9E76u,4),
        new("ProductionQueueType",308,0xBC88486Au,4), new("BuildPlacementTypeFlag",312,0x20B9244Bu,4),
        new("BuildOnRequiredObjectKindOf",316,0x9AFC9193u,40), new("UnitCategory",356,0x93FB5F40u,4),
        new("WeaponCategory",360,0x27D46083u,4), new("HealthBar",384,0xE0A943E1u,4),
        // Reborn: child-element syntax does not imply pointer storage; these EP1 enum/color records are inline root words.
        new("Buildable",436,0xF57E48C8u,4),new("ThingClass",440,0x66BA691Bu,4),new("DisplayColor",460,0x9E3E975Bu,4)
    });
    // Reborn: stable combined descriptors drive root table extent and metadata corruption coverage without changing the scalar test's literal policy.
    public static IReadOnlyList<Ep1GameObjectScalarTokens.Field> AllFields { get; } = Array.AsReadOnly(Ep1GameObjectScalarTokens.Fields.Concat(ExtraFields).Concat(Ep1GameObjectStringTokens.Fields).Concat(Ep1GameObjectStrongTokens.Fields).ToArray());
}
