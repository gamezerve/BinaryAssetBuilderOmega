using System;
using System.Collections.Generic;

namespace BinaryAssetBuilder.XmlCompiler;

// Reborn: reviewed root import slots describe storage ownership, not target resolution or complete target-type compilation.
public static class Ep1GameObjectStrongTokens
{
    // Reborn: these are import indices, never relocation pointers; UnitIntro's native target model remains unported.
    public static IReadOnlyList<Ep1GameObjectScalarTokens.Field> Fields { get; } = Array.AsReadOnly(new Ep1GameObjectScalarTokens.Field[] {
        new("CrowdResponse",180,0x9AD999F3u,4), new("UnitTypeIcon",368,0xA1F1A41Fu,4), new("UnitIntro",400,0x6A76B67Au,4)
    });
}
