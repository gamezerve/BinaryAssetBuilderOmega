using System.Buffers.Binary;
using System.Xml;
using Relo;
using SageBinaryData;

namespace BinaryAssetBuilder.ManifestInspector;

internal static class CompilerSmokeTest
{
    public static unsafe void Run()
    {
        TestAttributeModifier();
        TestLocomotorTemplate();
        TestWeaponTemplate();
        TestGameObject();
        Console.WriteLine("Uprising compiler self-test: OK");
    }

    private static unsafe void TestAttributeModifier()
    {
        const string xml = """
            <AttributeModifier xmlns="uri:ea.com:eala:asset"
                Category="SHRINK" Duration="1s"
                ReplaceInCategoryIfLongest="true" IgnoreIfAnticategoryActive="true"
                StartFX="FXList\123" EndFX="FXList\456"
                ModelConditionsSet="SPECIAL_POWER_SELECTED_PENDING"
                ModelConditionsClear="TOPPLED"
                ObjectStatusToSet="BRIDGE_DEAD"
                StackingLimit="2" ArmorSetType="SHRINK_EFFECT"
                Shader="ShaderOverride\789">
              <Modifier Type="RADIATION_ARMOR" Value="50%" />
            </AttributeModifier>
            """;

        XmlDocument document = new();
        document.LoadXml(xml);
        XmlNamespaceManager namespaces = new(document.NameTable);
        namespaces.AddNamespace("ea", "uri:ea.com:eala:asset");
        Node node = new(document.CreateNavigator()!.SelectSingleNode("/ea:AttributeModifier", namespaces)!, namespaces);

        AttributeModifier* root;
        using Tracker tracker = new((void**)&root, (uint)sizeof(AttributeModifier), false);
        Marshaler.Marshal(node, root, tracker);
        Chunk chunk = new();
        if (!tracker.MakeRelocatable(chunk))
        {
            throw new InvalidDataException("Tracker failed to produce a relocatable chunk.");
        }

        ReadOnlySpan<byte> instance = chunk.InstanceBuffer;
        Expect(instance.Length == 216, "instance bytes", 216, instance.Length);
        Expect(ReadUInt32(instance, 4) == 15, "Category=SHRINK", 15, ReadUInt32(instance, 4));
        Expect(ReadUInt32(instance, 12) == 123, "StartFX import", 123, ReadUInt32(instance, 12));
        Expect(ReadUInt32(instance, 16) == 456, "EndFX import", 456, ReadUInt32(instance, 16));
        Expect(ReadUInt32(instance, 20) == 56, "ModelConditionsSet relocation", 56, ReadUInt32(instance, 20));
        Expect(ReadUInt32(instance, 24) == 116, "ModelConditionsClear relocation", 116, ReadUInt32(instance, 24));
        Expect(ReadUInt32(instance, 28) == 176, "ObjectStatusToSet relocation", 176, ReadUInt32(instance, 28));
        Expect(ReadUInt32(instance, 32) == 2, "StackingLimit", 2, ReadUInt32(instance, 32));
        Expect(ReadUInt32(instance, 36) == 14, "ArmorSetType=SHRINK_EFFECT", 14, ReadUInt32(instance, 36));
        Expect(ReadUInt32(instance, 40) == 789, "Shader import", 789, ReadUInt32(instance, 40));
        Expect(ReadUInt32(instance, 44) == 1, "Modifier count", 1, ReadUInt32(instance, 44));
        Expect(ReadUInt32(instance, 48) == 208, "Modifier relocation", 208, ReadUInt32(instance, 48));
        Expect(instance[52] == 1, "ReplaceInCategoryIfLongest", 1, instance[52]);
        Expect(instance[53] == 1, "IgnoreIfAnticategoryActive", 1, instance[53]);
        Expect(ReadUInt32(instance, 208) == 36, "Modifier.Type=RADIATION_ARMOR", 36, ReadUInt32(instance, 208));
        Expect(chunk.RelocationBuffer.Length == 20, "relocation bytes", 20, chunk.RelocationBuffer.Length);
        Expect(chunk.ImportsBuffer.Length == 16, "imports bytes", 16, chunk.ImportsBuffer.Length);

        Console.WriteLine(
            $"  AttributeModifier bin={chunk.InstanceBuffer.Length}, " +
            $"relo={chunk.RelocationBuffer.Length}, imp={chunk.ImportsBuffer.Length}");
    }

    private static unsafe void TestLocomotorTemplate()
    {
        const string xml = """
            <LocomotorTemplate xmlns="uri:ea.com:eala:asset"
                Surfaces="CRUSHABLE_WALL" BehaviorZ="DO_NOT_MODIFY_HEIGHT"
                IgnoreLowSpeedAngleMultiplier="true">
              <JetLocomotorData Options="NO_CIRCLE_WHILE_USING_SPECIALPOWER"
                  AttackPathStartRunDistance="10" AttackPathClimbDistance="20"
                  AttackPathDiveDistanceStart="30" AttackPathDiveDistanceEnd="40" />
            </LocomotorTemplate>
            """;

        XmlDocument document = new();
        document.LoadXml(xml);
        XmlNamespaceManager namespaces = new(document.NameTable);
        namespaces.AddNamespace("ea", "uri:ea.com:eala:asset");
        Node node = new(document.CreateNavigator()!.SelectSingleNode("/ea:LocomotorTemplate", namespaces)!, namespaces);

        LocomotorTemplate* root;
        using Tracker tracker = new((void**)&root, (uint)sizeof(LocomotorTemplate), false);
        Marshaler.Marshal(node, root, tracker);
        Chunk chunk = new();
        tracker.MakeRelocatable(chunk);
        ReadOnlySpan<byte> instance = chunk.InstanceBuffer;
        Expect(instance.Length == 416, "Locomotor instance bytes", 416, instance.Length);
        Expect(ReadUInt32(instance, 4) == 0x400, "Surfaces=CRUSHABLE_WALL", 0x400, ReadUInt32(instance, 4));
        Expect(ReadUInt32(instance, 96) == 11, "BehaviorZ=DO_NOT_MODIFY_HEIGHT", 11, ReadUInt32(instance, 96));
        Expect(ReadUInt32(instance, 364) == 396, "JetLocomotorData relocation", 396, ReadUInt32(instance, 364));
        Expect(instance[393] == 1, "IgnoreLowSpeedAngleMultiplier", 1, instance[393]);
        Expect(ReadUInt32(instance, 396) == 1, "JetLocomotorData.Options", 1, ReadUInt32(instance, 396));
        Expect(chunk.RelocationBuffer.Length == 8, "Locomotor relocation bytes", 8, chunk.RelocationBuffer.Length);
        Expect(chunk.ImportsBuffer.Length == 0, "Locomotor imports bytes", 0, chunk.ImportsBuffer.Length);
        Console.WriteLine(
            $"  LocomotorTemplate bin={chunk.InstanceBuffer.Length}, " +
            $"relo={chunk.RelocationBuffer.Length}, imp={chunk.ImportsBuffer.Length}");
    }

    private static unsafe void TestWeaponTemplate()
    {
        const string xml = """
            <WeaponTemplate xmlns="uri:ea.com:eala:asset"
                AttackRange="250" ScatterAlways="true"
                ProjectileSelf="true" ProjectileSelfUsesPathfinder="false"
                Flags="FORCE_KILL_GARRISONED_UNITS"
                RequiredAntiMask="ANTI_WATER ANTI_LIFTED_GROUND_UNIT"
                VirtualDamage="SHARE" UpdateBarrelModelConditions="true" />
            """;

        XmlDocument document = new();
        document.LoadXml(xml);
        XmlNamespaceManager namespaces = new(document.NameTable);
        namespaces.AddNamespace("ea", "uri:ea.com:eala:asset");
        Node node = new(document.CreateNavigator()!.SelectSingleNode("/ea:WeaponTemplate", namespaces)!, namespaces);

        WeaponTemplate* root;
        using Tracker tracker = new((void**)&root, (uint)sizeof(WeaponTemplate), false);
        Marshaler.Marshal(node, root, tracker);
        Chunk chunk = new();
        tracker.MakeRelocatable(chunk);
        ReadOnlySpan<byte> instance = chunk.InstanceBuffer;
        Expect(instance.Length == 340, "WeaponTemplate instance bytes", 340, instance.Length);
        Expect(ReadUInt32(instance, 4) == 0x437A0000, "AttackRange=250", 0x437A0000, ReadUInt32(instance, 4));
        Expect(ReadUInt32(instance, 180) == 0x1000, "Flags=FORCE_KILL_GARRISONED_UNITS", 0x1000, ReadUInt32(instance, 180));
        Expect(ReadUInt32(instance, 216) == 0x802, "RequiredAntiMask", 0x802, ReadUInt32(instance, 216));
        Expect(ReadUInt32(instance, 228) == 2, "VirtualDamage=SHARE", 2, ReadUInt32(instance, 228));
        Expect(instance[301] == 1, "ScatterAlways", 1, instance[301]);
        Expect(instance[311] == 1, "ProjectileSelf", 1, instance[311]);
        Expect(instance[312] == 0, "ProjectileSelfUsesPathfinder", 0, instance[312]);
        Expect(instance[338] == 1, "UpdateBarrelModelConditions", 1, instance[338]);
        Expect(chunk.RelocationBuffer.Length == 0, "WeaponTemplate relocation bytes", 0, chunk.RelocationBuffer.Length);
        Expect(chunk.ImportsBuffer.Length == 0, "WeaponTemplate imports bytes", 0, chunk.ImportsBuffer.Length);
        Console.WriteLine(
            $"  WeaponTemplate bin={chunk.InstanceBuffer.Length}, " +
            $"relo={chunk.RelocationBuffer.Length}, imp={chunk.ImportsBuffer.Length}");
    }

    private static unsafe void TestGameObject()
    {
        const string xml = """
            <GameObject xmlns="uri:ea.com:eala:asset"
                CamouflageDetectorLevel="3" PathPriority="7"
                InvisibilityOpacityMin="0.25" InvisibilityOpacityMax="0.75"
                BuildInProximityToSamePlayerStucture="false">
              <CrusherInfo CrushAircraftWhileStationary="true"
                  DefaultCrushKillDelay="0.75s" CannotCrushTarget="true" />
              <ProjectedBuildabilityInfo Radius="12"
                  AllowedBuildabilityHeightVariation="75" PrimaryBuidability="false" />
            </GameObject>
            """;

        XmlDocument document = new();
        document.LoadXml(xml);
        XmlNamespaceManager namespaces = new(document.NameTable);
        namespaces.AddNamespace("ea", "uri:ea.com:eala:asset");
        Node node = new(document.CreateNavigator()!.SelectSingleNode("/ea:GameObject", namespaces)!, namespaces);

        GameObject* root;
        using Tracker tracker = new((void**)&root, (uint)sizeof(GameObject), false);
        Marshaler.Marshal(node, root, tracker);
        Chunk chunk = new();
        tracker.MakeRelocatable(chunk);
        ReadOnlySpan<byte> instance = chunk.InstanceBuffer;
        Expect(instance.Length == 768, "GameObject instance bytes", 768, instance.Length);
        Expect(ReadUInt32(instance, 296) == 3, "CamouflageDetectorLevel", 3, ReadUInt32(instance, 296));
        Expect(ReadUInt32(instance, 304) == 7, "PathPriority", 7, ReadUInt32(instance, 304));
        Expect(ReadUInt32(instance, 376) == 0x3E800000, "InvisibilityOpacityMin", 0x3E800000, ReadUInt32(instance, 376));
        Expect(ReadUInt32(instance, 380) == 0x3F400000, "InvisibilityOpacityMax", 0x3F400000, ReadUInt32(instance, 380));
        Expect(instance[589] == 1, "IsTrainable default", 1, instance[589]);
        Expect(instance[597] == 1, "CanPathThroughGates default", 1, instance[597]);
        Expect(instance[598] == 0, "BuildInProximityToSamePlayerStucture", 0, instance[598]);
        Expect(ReadUInt32(instance, 564) == 600, "CrusherInfo relocation", 600, ReadUInt32(instance, 564));
        Expect(ReadUInt32(instance, 568) == 1, "ProjectedBuildabilityInfo count", 1, ReadUInt32(instance, 568));
        Expect(ReadUInt32(instance, 572) == 652, "ProjectedBuildabilityInfo relocation", 652, ReadUInt32(instance, 572));
        Expect(instance[646] == 1, "CrushAircraftWhileStationary", 1, instance[646]);
        Expect(instance[650] == 1, "CannotCrushTarget", 1, instance[650]);
        Expect(ReadUInt32(instance, 652) == 0x41400000, "ProjectedBuildabilityInfo.Radius", 0x41400000, ReadUInt32(instance, 652));
        Expect(ReadUInt32(instance, 756) == 0x42960000, "AllowedBuildabilityHeightVariation", 0x42960000, ReadUInt32(instance, 756));
        Expect(instance[764] == 0, "PrimaryBuidability", 0, instance[764]);
        Expect(chunk.RelocationBuffer.Length == 12, "GameObject relocation bytes", 12, chunk.RelocationBuffer.Length);
        Expect(chunk.ImportsBuffer.Length == 0, "GameObject imports bytes", 0, chunk.ImportsBuffer.Length);
        Console.WriteLine(
            $"  GameObject bin={chunk.InstanceBuffer.Length}, " +
            $"relo={chunk.RelocationBuffer.Length}, imp={chunk.ImportsBuffer.Length}");
    }

    private static uint ReadUInt32(ReadOnlySpan<byte> bytes, int offset) =>
        BinaryPrimitives.ReadUInt32LittleEndian(bytes.Slice(offset, sizeof(uint)));

    private static void Expect(bool condition, string name, int expected, long actual)
    {
        if (!condition)
        {
            throw new InvalidDataException($"{name}: expected {expected}, got {actual}.");
        }
    }
}
