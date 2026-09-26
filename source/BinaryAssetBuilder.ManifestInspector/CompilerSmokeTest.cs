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
        TestSpawnedSlaveUpdate();
        TestUnitUnpackUpdate();
        TestAddObjectsToLiftUpdate();
        TestLureObjectsUpdate();
        TestFlingStoredObjectsSpecialPower();
        TestLiftObjectUpdate();
        TestProjectileReplaceSelfSpecialAbility();
        TestProjectilePath();
        TestYurikoHotKeys();
        TestAudioDynamicsCollide();
        TestDamageDynamicsCollide();
        TestReactionFXOnDamage();
        TestDamageSphereUpdate();
        TestYurikoShieldSphereUpdate();
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

    private static unsafe void TestSpawnedSlaveUpdate()
    {
        const string xml = """
            <SpawnedSlaveUpdate xmlns="uri:ea.com:eala:asset"
                LeashRange="300" AttackRange="250"
                DieOnMastersDeath="true"
                UseSlaverAsControlForEvaObjectSightedEvents="false" />
            """;

        XmlDocument document = new();
        document.LoadXml(xml);
        XmlNamespaceManager namespaces = new(document.NameTable);
        namespaces.AddNamespace("ea", "uri:ea.com:eala:asset");
        Node node = new(document.CreateNavigator()!.SelectSingleNode("/ea:SpawnedSlaveUpdate", namespaces)!, namespaces);

        SpawnedSlaveUpdateModuleData* root;
        using Tracker tracker = new((void**)&root, (uint)sizeof(SpawnedSlaveUpdateModuleData), false);
        Marshaler.Marshal(node, root, tracker);
        Chunk chunk = new();
        tracker.MakeRelocatable(chunk);
        ReadOnlySpan<byte> instance = chunk.InstanceBuffer;
        Expect(instance.Length == 112, "SpawnedSlaveUpdate instance bytes", 112, instance.Length);
        Expect(ReadUInt32(instance, 8) == 300, "SpawnedSlaveUpdate.LeashRange", 300, ReadUInt32(instance, 8));
        Expect(ReadUInt32(instance, 20) == 250, "SpawnedSlaveUpdate.AttackRange", 250, ReadUInt32(instance, 20));
        Expect(instance[109] == 1, "SpawnedSlaveUpdate.DieOnMastersDeath", 1, instance[109]);
        Expect(chunk.RelocationBuffer.Length == 0, "SpawnedSlaveUpdate relocation bytes", 0, chunk.RelocationBuffer.Length);
        Expect(chunk.ImportsBuffer.Length == 0, "SpawnedSlaveUpdate imports bytes", 0, chunk.ImportsBuffer.Length);
        Console.WriteLine($"  SpawnedSlaveUpdate bin={chunk.InstanceBuffer.Length}, relo=0, imp=0");
    }

    private static unsafe void TestUnitUnpackUpdate()
    {
        const string xml = """
            <UnitUnpackUpdate xmlns="uri:ea.com:eala:asset"
                UnpackTime="20s" OffsetHeightAboveWater="5.0" />
            """;

        XmlDocument document = new();
        document.LoadXml(xml);
        XmlNamespaceManager namespaces = new(document.NameTable);
        namespaces.AddNamespace("ea", "uri:ea.com:eala:asset");
        Node node = new(document.CreateNavigator()!.SelectSingleNode("/ea:UnitUnpackUpdate", namespaces)!, namespaces);

        UnitUnpackUpdateModuleData* root;
        using Tracker tracker = new((void**)&root, (uint)sizeof(UnitUnpackUpdateModuleData), false);
        Marshaler.Marshal(node, root, tracker);
        Chunk chunk = new();
        tracker.MakeRelocatable(chunk);
        ReadOnlySpan<byte> instance = chunk.InstanceBuffer;
        Expect(instance.Length == 20, "UnitUnpackUpdate instance bytes", 20, instance.Length);
        Expect(ReadUInt32(instance, 8) == 0x41A00000, "UnitUnpackUpdate.UnpackTime", 0x41A00000, ReadUInt32(instance, 8));
        Expect(ReadUInt32(instance, 16) == 0x40A00000, "UnitUnpackUpdate.OffsetHeightAboveWater", 0x40A00000, ReadUInt32(instance, 16));
        Expect(chunk.RelocationBuffer.Length == 0, "UnitUnpackUpdate relocation bytes", 0, chunk.RelocationBuffer.Length);
        Expect(chunk.ImportsBuffer.Length == 0, "UnitUnpackUpdate imports bytes", 0, chunk.ImportsBuffer.Length);
        Console.WriteLine($"  UnitUnpackUpdate bin={chunk.InstanceBuffer.Length}, relo=0, imp=0");
    }

    private static unsafe void TestAddObjectsToLiftUpdate()
    {
        const string xml = """
            <AddObjectsToLiftUpdateSpecialPower xmlns="uri:ea.com:eala:asset"
                Radius="15" LiftObjectLinkID="101" />
            """;

        XmlDocument document = new();
        document.LoadXml(xml);
        XmlNamespaceManager namespaces = new(document.NameTable);
        namespaces.AddNamespace("ea", "uri:ea.com:eala:asset");
        Node node = new(document.CreateNavigator()!.SelectSingleNode("/ea:AddObjectsToLiftUpdateSpecialPower", namespaces)!, namespaces);

        AddObjectsToLiftUpdateSpecialPowerModuleData* root;
        using Tracker tracker = new((void**)&root, (uint)sizeof(AddObjectsToLiftUpdateSpecialPowerModuleData), false);
        Marshaler.Marshal(node, root, tracker);
        Chunk chunk = new();
        tracker.MakeRelocatable(chunk);
        ReadOnlySpan<byte> instance = chunk.InstanceBuffer;
        Expect(instance.Length == 496, "AddObjectsToLiftUpdate instance bytes", 496, instance.Length);
        Expect(ReadUInt32(instance, 476) == 0x41700000, "AddObjectsToLiftUpdate.Radius", 0x41700000, ReadUInt32(instance, 476));
        Expect(ReadUInt32(instance, 492) == 101, "AddObjectsToLiftUpdate.LiftObjectLinkID", 101, ReadUInt32(instance, 492));
        Console.WriteLine($"  AddObjectsToLiftUpdate bin={chunk.InstanceBuffer.Length}, relo={chunk.RelocationBuffer.Length}, imp={chunk.ImportsBuffer.Length}");
    }

    private static unsafe void TestLureObjectsUpdate()
    {
        const string xml = """
            <LureObjectsUpdate xmlns="uri:ea.com:eala:asset"
                LureObjectLinkID="101" GuardAttackRange="175"
                GuardStatus="UNSELECTABLE"
                DisabledTypesToProcess="FROZEN">
              <GuardOffset x="60" y="0" z="0" />
              <GuardOffset x="45" y="-45" z="0" />
            </LureObjectsUpdate>
            """;

        XmlDocument document = new();
        document.LoadXml(xml);
        XmlNamespaceManager namespaces = new(document.NameTable);
        namespaces.AddNamespace("ea", "uri:ea.com:eala:asset");
        Node node = new(document.CreateNavigator()!.SelectSingleNode("/ea:LureObjectsUpdate", namespaces)!, namespaces);

        LureObjectsUpdateModuleData* root;
        using Tracker tracker = new((void**)&root, (uint)sizeof(LureObjectsUpdateModuleData), false);
        Marshaler.Marshal(node, root, tracker);
        Chunk chunk = new();
        tracker.MakeRelocatable(chunk);
        ReadOnlySpan<byte> instance = chunk.InstanceBuffer;
        Expect(instance.Length == 96, "LureObjectsUpdate instance bytes", 96, instance.Length);
        Expect(ReadUInt32(instance, 8) == 101, "LureObjectsUpdate.LureObjectLinkID", 101, ReadUInt32(instance, 8));
        Expect(ReadUInt32(instance, 12) == 0x49742400, "LureObjectsUpdate.GuardRadiusMaxSqr", 0x49742400, ReadUInt32(instance, 12));
        Expect(ReadUInt32(instance, 16) == 0x432F0000, "LureObjectsUpdate.GuardAttackRange", 0x432F0000, ReadUInt32(instance, 16));
        Expect(ReadUInt32(instance, 52) == 10, "LureObjectsUpdate.UpdateRate", 10, ReadUInt32(instance, 52));
        Expect(ReadUInt32(instance, 60) == 0x1000, "LureObjectsUpdate.DisabledTypesToProcess", 0x1000, ReadUInt32(instance, 60));
        Expect(ReadUInt32(instance, 64) == 2, "LureObjectsUpdate.GuardOffset count", 2, ReadUInt32(instance, 64));
        Expect(ReadUInt32(instance, 68) == 72, "LureObjectsUpdate.GuardOffset relocation", 72, ReadUInt32(instance, 68));
        Expect(ReadUInt32(instance, 72) == 0x42700000, "LureObjectsUpdate.GuardOffset[0].X", 0x42700000, ReadUInt32(instance, 72));
        Expect(ReadUInt32(instance, 88) == 0xC2340000, "LureObjectsUpdate.GuardOffset[1].Y", unchecked((int)0xC2340000), ReadUInt32(instance, 88));
        Expect(chunk.RelocationBuffer.Length == 8, "LureObjectsUpdate relocation bytes", 8, chunk.RelocationBuffer.Length);
        Console.WriteLine($"  LureObjectsUpdate bin={chunk.InstanceBuffer.Length}, relo={chunk.RelocationBuffer.Length}, imp={chunk.ImportsBuffer.Length}");
    }

    private static unsafe void TestFlingStoredObjectsSpecialPower()
    {
        const string xml = """
            <FlingStoredObjectsSpecialPower xmlns="uri:ea.com:eala:asset"
                SpecialPowerTemplate="SpecialPowerTemplate\123"
                CanAffectObjectFilter="ObjectFilterAsset\456"
                DisabledTypesToIgnore="FROZEN" ObjectFilterDistType="CIRCLE"
                AvailableAtStart="true" StoreObjectsLinkID="101" LiftObjectLinkID="102"
                MaximumVelocity="800" MinimumVelocity="700">
              <ObjectMap />
              <ObjectMap />
            </FlingStoredObjectsSpecialPower>
            """;

        XmlDocument document = new();
        document.LoadXml(xml);
        XmlNamespaceManager namespaces = new(document.NameTable);
        namespaces.AddNamespace("ea", "uri:ea.com:eala:asset");
        Node node = new(document.CreateNavigator()!.SelectSingleNode("/ea:FlingStoredObjectsSpecialPower", namespaces)!, namespaces);

        FlingStoredObjectsSpecialPowerModuleData* root;
        using Tracker tracker = new((void**)&root, (uint)sizeof(FlingStoredObjectsSpecialPowerModuleData), false);
        Marshaler.Marshal(node, root, tracker);
        Chunk chunk = new();
        tracker.MakeRelocatable(chunk);
        ReadOnlySpan<byte> instance = chunk.InstanceBuffer;
        Expect(instance.Length == 516, "FlingStoredObjectsSpecialPower instance bytes", 516, instance.Length);
        Expect(ReadUInt32(instance, 8) == 123, "Fling SpecialPowerTemplate import", 123, ReadUInt32(instance, 8));
        Expect(ReadUInt32(instance, 84) == 0x1000, "Fling DisabledTypesToIgnore", 0x1000, ReadUInt32(instance, 84));
        Expect(ReadUInt32(instance, 88) == 456, "Fling CanAffectObjectFilter import", 456, ReadUInt32(instance, 88));
        Expect(ReadUInt32(instance, 92) == 1, "Fling ObjectFilterDistType=CIRCLE", 1, ReadUInt32(instance, 92));
        Expect(instance[469] == 1, "Fling AvailableAtStart", 1, instance[469]);
        Expect(ReadUInt32(instance, 476) == 101, "Fling StoreObjectsLinkID", 101, ReadUInt32(instance, 476));
        Expect(ReadUInt32(instance, 480) == 102, "Fling LiftObjectLinkID", 102, ReadUInt32(instance, 480));
        Expect(ReadUInt32(instance, 484) == 0x44480000, "Fling MaximumVelocity", 0x44480000, ReadUInt32(instance, 484));
        Expect(ReadUInt32(instance, 488) == 0x442F0000, "Fling MinimumVelocity", 0x442F0000, ReadUInt32(instance, 488));
        Expect(ReadUInt32(instance, 492) == 2, "Fling ObjectMap count", 2, ReadUInt32(instance, 492));
        Expect(ReadUInt32(instance, 496) == 500, "Fling ObjectMap relocation", 500, ReadUInt32(instance, 496));
        Expect(chunk.RelocationBuffer.Length == 8, "Fling relocation bytes", 8, chunk.RelocationBuffer.Length);
        Expect(chunk.ImportsBuffer.Length == 12, "Fling imports bytes", 12, chunk.ImportsBuffer.Length);
        Console.WriteLine($"  FlingStoredObjectsSpecialPower bin={chunk.InstanceBuffer.Length}, relo={chunk.RelocationBuffer.Length}, imp={chunk.ImportsBuffer.Length}");
    }

    private static unsafe void TestLiftObjectUpdate()
    {
        const string xml = """
            <LiftObjectUpdate xmlns="uri:ea.com:eala:asset"
                LiftObjectLinkID="101" CrusherModifiesVelocity="true"
                LiftVelocity="4" MaxElevationFromGround="90"
                TimeIncrement="20s" MaxTimeLifted="20s"
                RotationSpeed="0.1" Shader="ShaderOverride\123"
                ShakeIntensity="0.002" ShakeRadius="-1" ShakeFade="1"
                DisabledTypesToProcess="FROZEN">
              <ModelStateObjectFilters>
                <LiftedUnitModelState ModelState="REACT_5">
                  <ObjectFilter Rule="ANY" Include="INFANTRY" />
                </LiftedUnitModelState>
              </ModelStateObjectFilters>
            </LiftObjectUpdate>
            """;

        XmlDocument document = new();
        document.LoadXml(xml);
        XmlNamespaceManager namespaces = new(document.NameTable);
        namespaces.AddNamespace("ea", "uri:ea.com:eala:asset");
        Node node = new(document.CreateNavigator()!.SelectSingleNode("/ea:LiftObjectUpdate", namespaces)!, namespaces);

        LiftObjectUpdateModuleData* root;
        using Tracker tracker = new((void**)&root, (uint)sizeof(LiftObjectUpdateModuleData), false);
        Marshaler.Marshal(node, root, tracker);
        Chunk chunk = new();
        tracker.MakeRelocatable(chunk);
        ReadOnlySpan<byte> instance = chunk.InstanceBuffer;
        Expect(instance.Length == 260, "LiftObjectUpdate instance bytes", 260, instance.Length);
        Expect(ReadUInt32(instance, 8) == 101, "LiftObjectUpdate.LiftObjectLinkID", 101, ReadUInt32(instance, 8));
        Expect(ReadUInt32(instance, 12) == 0x40800000, "LiftObjectUpdate.LiftVelocity", 0x40800000, ReadUInt32(instance, 12));
        Expect(ReadUInt32(instance, 16) == 0x42B40000, "LiftObjectUpdate.MaxElevationFromGround", 0x42B40000, ReadUInt32(instance, 16));
        Expect(ReadUInt32(instance, 40) == 0x3DCCCCCD, "LiftObjectUpdate.RotationSpeed", 0x3DCCCCCD, ReadUInt32(instance, 40));
        Expect(ReadUInt32(instance, 44) == 123, "LiftObjectUpdate.Shader import", 123, ReadUInt32(instance, 44));
        Expect(ReadUInt32(instance, 48) == 0x3B03126F, "LiftObjectUpdate.ShakeIntensity", 0x3B03126F, ReadUInt32(instance, 48));
        Expect(ReadUInt32(instance, 52) == 0xBF800000, "LiftObjectUpdate.ShakeRadius", unchecked((int)0xBF800000), ReadUInt32(instance, 52));
        Expect(ReadUInt32(instance, 60) == 0x1000, "LiftObjectUpdate.DisabledTypesToProcess", 0x1000, ReadUInt32(instance, 60));
        Expect(ReadUInt32(instance, 64) == 1, "LiftObjectUpdate model-state count", 1, ReadUInt32(instance, 64));
        Expect(ReadUInt32(instance, 68) == 76, "LiftObjectUpdate model-state relocation", 76, ReadUInt32(instance, 68));
        Expect(instance[72] == 1, "LiftObjectUpdate.CrusherModifiesVelocity", 1, instance[72]);
        Expect(ReadUInt32(instance, 108) == 0x10000, "LiftObjectUpdate ModelState=REACT_5", 0x10000, ReadUInt32(instance, 108));
        Expect(ReadUInt32(instance, 136) == 140, "LiftObjectUpdate ObjectFilter relocation", 140, ReadUInt32(instance, 136));
        Expect(ReadUInt32(instance, 144) == 2, "LiftObjectUpdate ObjectFilter.Rule=ANY", 2, ReadUInt32(instance, 144));
        Expect(chunk.RelocationBuffer.Length == 12, "LiftObjectUpdate relocation bytes", 12, chunk.RelocationBuffer.Length);
        Expect(chunk.ImportsBuffer.Length == 8, "LiftObjectUpdate imports bytes", 8, chunk.ImportsBuffer.Length);
        Console.WriteLine($"  LiftObjectUpdate bin={chunk.InstanceBuffer.Length}, relo={chunk.RelocationBuffer.Length}, imp={chunk.ImportsBuffer.Length}");
    }

    private static unsafe void TestProjectileReplaceSelfSpecialAbility()
    {
        const string xml = """
            <ProjectileReplaceSelfSpecialAbility xmlns="uri:ea.com:eala:asset"
                SpecialPowerTemplate="SpecialPowerTemplate\68"
                StartAbilityRange="200" PackTime="3s"
                Options="RECONSTITUTE_STORED_COMMAND IGNORE_FACING_CHECK USE_OBJECT_GEOMETRY_FOR_WITHIN_RANGE_CHECK FAIL_WITH_INVALID_APPROACH"
                SetObjectStatusOnTrigger="IGNORE_AI_COMMAND"
                ClearObjectStatusOnExit="IGNORE_AI_COMMAND"
                MinimumUnpackTimeAfterSpecialPowerInitiation="1.25s"
                ClearTriggerDistance="225"
                ReplaceOptions="CHECK_BUILD_ASSISTANT DISABLE_DURING_REPLACE CLEAR_LOCATION REPLACE_OVER_ENEMIES TRANSFER_EXPERIENCE"
                LaunchingWeapon="WeaponTemplate\69"
                OtherObjectCreationList="ObjectCreationList\71" />
            """;

        XmlDocument document = new();
        document.LoadXml(xml);
        XmlNamespaceManager namespaces = new(document.NameTable);
        namespaces.AddNamespace("ea", "uri:ea.com:eala:asset");
        Node node = new(document.CreateNavigator()!.SelectSingleNode("/ea:ProjectileReplaceSelfSpecialAbility", namespaces)!, namespaces);

        ProjectileReplaceSelfSpecialAbilityModuleData* root;
        using Tracker tracker = new((void**)&root, (uint)sizeof(ProjectileReplaceSelfSpecialAbilityModuleData), false);
        Marshaler.Marshal(node, root, tracker);
        Chunk chunk = new();
        tracker.MakeRelocatable(chunk);
        ReadOnlySpan<byte> instance = chunk.InstanceBuffer;
        Expect(instance.Length == 284, "ProjectileReplaceSelf instance bytes", 284, instance.Length);
        Expect(ReadUInt32(instance, 8) == 68, "ProjectileReplaceSelf SpecialPowerTemplate import", 68, ReadUInt32(instance, 8));
        Expect(ReadUInt32(instance, 12) == 0x43480000, "ProjectileReplaceSelf StartAbilityRange", 0x43480000, ReadUInt32(instance, 12));
        Expect(ReadUInt32(instance, 32) == 0x40400000, "ProjectileReplaceSelf PackTime", 0x40400000, ReadUInt32(instance, 32));
        Expect(ReadUInt32(instance, 44) == 0x44180000, "ProjectileReplaceSelf Options", 0x44180000, ReadUInt32(instance, 44));
        Expect(ReadUInt32(instance, 148) == 0x2000, "ProjectileReplaceSelf set status", 0x2000, ReadUInt32(instance, 148));
        Expect(ReadUInt32(instance, 180) == 0x2000, "ProjectileReplaceSelf clear status", 0x2000, ReadUInt32(instance, 180));
        Expect(ReadUInt32(instance, 228) == 0x8, "ProjectileReplaceSelf DisabledTypesToProcess", 0x8, ReadUInt32(instance, 228));
        Expect(ReadUInt32(instance, 240) == 0x3FA00000, "ProjectileReplaceSelf minimum unpack time", 0x3FA00000, ReadUInt32(instance, 240));
        Expect(instance[249] == 1, "ProjectileReplaceSelf GoIdleInStartPreparation", 1, instance[249]);
        Expect(instance[250] == 1, "ProjectileReplaceSelf FaceTarget", 1, instance[250]);
        Expect(ReadUInt32(instance, 256) == 0x1D8, "ProjectileReplaceSelf ReplaceOptions", 0x1D8, ReadUInt32(instance, 256));
        Expect(ReadUInt32(instance, 260) == 0x43610000, "ProjectileReplaceSelf ClearTriggerDistance", 0x43610000, ReadUInt32(instance, 260));
        Expect(ReadUInt32(instance, 264) == 0, "ProjectileReplaceSelf replacement count", 0, ReadUInt32(instance, 264));
        Expect(ReadUInt32(instance, 268) == 0, "ProjectileReplaceSelf omitted replacement pointer", 0, ReadUInt32(instance, 268));
        Expect(ReadUInt32(instance, 272) == 69, "ProjectileReplaceSelf LaunchingWeapon import", 69, ReadUInt32(instance, 272));
        Expect(ReadUInt32(instance, 276) == 280, "ProjectileReplaceSelf OCL relocation", 280, ReadUInt32(instance, 276));
        Expect(ReadUInt32(instance, 280) == 71, "ProjectileReplaceSelf OCL import", 71, ReadUInt32(instance, 280));
        Expect(chunk.RelocationBuffer.Length == 8, "ProjectileReplaceSelf relocation bytes", 8, chunk.RelocationBuffer.Length);
        Expect(chunk.ImportsBuffer.Length == 16, "ProjectileReplaceSelf imports bytes", 16, chunk.ImportsBuffer.Length);
        Console.WriteLine($"  ProjectileReplaceSelf bin={chunk.InstanceBuffer.Length}, relo={chunk.RelocationBuffer.Length}, imp={chunk.ImportsBuffer.Length}");
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: Validate the EP1 ProjectilePath node ABI against EA's five-node ProjectilePath_Foo fixture. */
    //-------------------------------------------------------------------------------------------------
    private static unsafe void TestProjectilePath()
    {
        const string xml = """
            <ProjectilePath xmlns="uri:ea.com:eala:asset">
              <Node>
                <InVec x="26.0649" y="-2.84037" z="6.802" />
                <Point x="29.0575" y="-4.72524" z="15.1092" />
                <OutVec x="32.0502" y="-6.61011" z="23.4164" />
              </Node>
              <Node>
                <InVec x="28.7771" y="-11.9631" z="40.4948" />
                <Point x="18.1657" y="-11.3092" z="49.0047" />
                <OutVec x="7.55436" y="-10.6554" z="57.5145" />
              </Node>
              <Node>
                <InVec x="-13.9382" y="-4.19995" z="65.2511" />
                <Point x="-34.6108" y="-0.802208" z="66.1682" />
                <OutVec x="-55.2834" y="2.59553" z="67.0853" />
              </Node>
              <Node>
                <InVec x="-79.2485" y="8.94352" z="62.1112" />
                <Point x="-105.87" y="9.07722" z="54.5073" />
                <OutVec x="-132.491" y="9.21093" z="46.9035" />
              </Node>
              <Node>
                <InVec x="-161.981" y="6.18518" z="35.5827" />
                <Point x="-194.34" y="0.0" z="20.5451" />
                <OutVec x="-227.34" y="-6.18518" z="5.5451" />
              </Node>
            </ProjectilePath>
            """;

        XmlDocument document = new();
        document.LoadXml(xml);
        XmlNamespaceManager namespaces = new(document.NameTable);
        namespaces.AddNamespace("ea", "uri:ea.com:eala:asset");
        Node node = new(document.CreateNavigator()!.SelectSingleNode("/ea:ProjectilePath", namespaces)!, namespaces);

        ProjectilePath* root;
        using Tracker tracker = new((void**)&root, (uint)sizeof(ProjectilePath), false);
        Marshaler.Marshal(node, root, tracker);
        Chunk chunk = new();
        if (!tracker.MakeRelocatable(chunk))
        {
            throw new InvalidDataException("Tracker failed to produce a relocatable ProjectilePath chunk.");
        }

        ReadOnlySpan<byte> instance = chunk.InstanceBuffer;
        Expect(instance.Length == 256, "ProjectilePath instance bytes", 256, instance.Length);
        Expect(ReadUInt32(instance, 4) == 0, "ProjectilePath omitted ComponentScale", 0, ReadUInt32(instance, 4));
        Expect(ReadUInt32(instance, 8) == 5, "ProjectilePath node count", 5, ReadUInt32(instance, 8));
        Expect(ReadUInt32(instance, 12) == 16, "ProjectilePath node relocation", 16, ReadUInt32(instance, 12));
        Expect(ReadUInt32(instance, 16) == 0x41D084EA, "ProjectilePath first InVec.x", 0x41D084EA, ReadUInt32(instance, 16));
        Expect(ReadUInt32(instance, 28) == 0, "ProjectilePath default InVec.w", 0, ReadUInt32(instance, 28));
        // Reborn: The final node's OutVec occupies the last 16 bytes of the 256-byte EA fixture.
        Expect(ReadUInt32(instance, 240) == 0xC363570A, "ProjectilePath final OutVec.x", unchecked((int)0xC363570A), ReadUInt32(instance, 240));
        Expect(ReadUInt32(instance, 244) == 0xC0C5ECFF, "ProjectilePath final OutVec.y", unchecked((int)0xC0C5ECFF), ReadUInt32(instance, 244));
        Expect(ReadUInt32(instance, 248) == 0x40B17176, "ProjectilePath final OutVec.z", 0x40B17176, ReadUInt32(instance, 248));
        Expect(chunk.RelocationBuffer.Length == 8, "ProjectilePath relocation bytes", 8, chunk.RelocationBuffer.Length);
        Expect(chunk.ImportsBuffer.Length == 0, "ProjectilePath imports bytes", 0, chunk.ImportsBuffer.Length);
        Console.WriteLine($"  ProjectilePath bin={chunk.InstanceBuffer.Length}, relo={chunk.RelocationBuffer.Length}, imp={chunk.ImportsBuffer.Length}");
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: Validate the EP1 YurikoHotKeys root, list stride, references, and modifier flags. */
    //-------------------------------------------------------------------------------------------------
    private static unsafe void TestYurikoHotKeys()
    {
        const string xml = """
            <YurikoHotKeys xmlns="uri:ea.com:eala:asset">
              <Map>
                <HotKey Slot="HotKeySlot\1" Key="MappableKey\2" Modifiers="CTRL" />
                <HotKey Slot="HotKeySlot\3" Key="MappableKey\4" Modifiers="SHIFT" />
              </Map>
            </YurikoHotKeys>
            """;

        XmlDocument document = new();
        document.LoadXml(xml);
        XmlNamespaceManager namespaces = new(document.NameTable);
        namespaces.AddNamespace("ea", "uri:ea.com:eala:asset");
        Node node = new(document.CreateNavigator()!.SelectSingleNode("/ea:YurikoHotKeys", namespaces)!, namespaces);

        YurikoHotKeys* root;
        using Tracker tracker = new((void**)&root, (uint)sizeof(YurikoHotKeys), false);
        Marshaler.Marshal(node, root, tracker);
        Chunk chunk = new();
        if (!tracker.MakeRelocatable(chunk))
        {
            throw new InvalidDataException("Tracker failed to produce a relocatable YurikoHotKeys chunk.");
        }

        ReadOnlySpan<byte> instance = chunk.InstanceBuffer;
        Expect(instance.Length == 36, "YurikoHotKeys instance bytes", 36, instance.Length);
        Expect(ReadUInt32(instance, 4) == 2, "YurikoHotKeys entry count", 2, ReadUInt32(instance, 4));
        Expect(ReadUInt32(instance, 8) == 12, "YurikoHotKeys entry relocation", 12, ReadUInt32(instance, 8));
        Expect(ReadUInt32(instance, 12) == 1, "YurikoHotKeys first slot import", 1, ReadUInt32(instance, 12));
        Expect(ReadUInt32(instance, 16) == 2, "YurikoHotKeys first key import", 2, ReadUInt32(instance, 16));
        Expect(ReadUInt32(instance, 20) == 1, "YurikoHotKeys CTRL modifier", 1, ReadUInt32(instance, 20));
        Expect(ReadUInt32(instance, 32) == 4, "YurikoHotKeys SHIFT modifier", 4, ReadUInt32(instance, 32));
        Expect(chunk.RelocationBuffer.Length == 8, "YurikoHotKeys relocation bytes", 8, chunk.RelocationBuffer.Length);
        // Reborn: Four import-source offsets plus the stream terminator occupy 20 bytes.
        Expect(chunk.ImportsBuffer.Length == 20, "YurikoHotKeys imports bytes", 20, chunk.ImportsBuffer.Length);
        Console.WriteLine($"  YurikoHotKeys bin={chunk.InstanceBuffer.Length}, relo={chunk.RelocationBuffer.Length}, imp={chunk.ImportsBuffer.Length}");
    }

    private static unsafe void TestAudioDynamicsCollide()
    {
        const string xml = """
            <AudioDynamicsCollide xmlns="uri:ea.com:eala:asset"
                MinimumImpactVelocity="5.0">
              <MagnitudeSoundSelector>
                <Entry MinimumMagnitude="1.0" Sound="" />
                <Entry MinimumMagnitude="4.0" Sound="" />
              </MagnitudeSoundSelector>
            </AudioDynamicsCollide>
            """;

        XmlDocument document = new();
        document.LoadXml(xml);
        XmlNamespaceManager namespaces = new(document.NameTable);
        namespaces.AddNamespace("ea", "uri:ea.com:eala:asset");
        Node node = new(document.CreateNavigator()!.SelectSingleNode("/ea:AudioDynamicsCollide", namespaces)!, namespaces);

        AudioDynamicsCollideModuleData* root;
        using Tracker tracker = new((void**)&root, (uint)sizeof(AudioDynamicsCollideModuleData), false);
        Marshaler.Marshal(node, root, tracker);
        Chunk chunk = new();
        tracker.MakeRelocatable(chunk);
        ReadOnlySpan<byte> instance = chunk.InstanceBuffer;
        Expect(instance.Length == 36, "AudioDynamicsCollide instance bytes", 36, instance.Length);
        Expect(ReadUInt32(instance, 8) == 0x40A00000, "AudioDynamicsCollide.MinimumImpactVelocity", 0x40A00000, ReadUInt32(instance, 8));
        Expect(ReadUInt32(instance, 12) == 2, "AudioDynamicsCollide.Entry count", 2, ReadUInt32(instance, 12));
        Expect(ReadUInt32(instance, 16) == 20, "AudioDynamicsCollide.Entry relocation", 20, ReadUInt32(instance, 16));
        Expect(ReadUInt32(instance, 20) == 0x3F800000, "AudioDynamicsCollide.Entry[0].MinimumMagnitude", 0x3F800000, ReadUInt32(instance, 20));
        Expect(ReadUInt32(instance, 28) == 0x40800000, "AudioDynamicsCollide.Entry[1].MinimumMagnitude", 0x40800000, ReadUInt32(instance, 28));
        Expect(chunk.RelocationBuffer.Length == 8, "AudioDynamicsCollide relocation bytes", 8, chunk.RelocationBuffer.Length);
        Expect(chunk.ImportsBuffer.Length == 0, "AudioDynamicsCollide imports bytes", 0, chunk.ImportsBuffer.Length);
        Console.WriteLine($"  AudioDynamicsCollide bin={chunk.InstanceBuffer.Length}, relo={chunk.RelocationBuffer.Length}, imp={chunk.ImportsBuffer.Length}");
    }

    private static unsafe void TestDamageDynamicsCollide()
    {
        const string xml = """
            <DamageDynamicsCollide xmlns="uri:ea.com:eala:asset">
              <DamageNugget Radius="0" OnlyKillOwnerWhenTriggered="true"
                  DelayTimeSeconds="0s" DamageType="UNRESISTABLE"
                  DamageFXType="JAPAN_CANNON" DeathType="SUICIDED" />
            </DamageDynamicsCollide>
            """;

        XmlDocument document = new();
        document.LoadXml(xml);
        XmlNamespaceManager namespaces = new(document.NameTable);
        namespaces.AddNamespace("ea", "uri:ea.com:eala:asset");
        Node node = new(document.CreateNavigator()!.SelectSingleNode("/ea:DamageDynamicsCollide", namespaces)!, namespaces);

        DamageDynamicsCollideModuleData* root;
        using Tracker tracker = new((void**)&root, (uint)sizeof(DamageDynamicsCollideModuleData), false);
        Marshaler.Marshal(node, root, tracker);
        Chunk chunk = new();
        tracker.MakeRelocatable(chunk);
        ReadOnlySpan<byte> instance = chunk.InstanceBuffer;
        Expect(instance.Length == 176, "DamageDynamicsCollide instance bytes", 176, instance.Length);
        Expect(ReadUInt32(instance, 8) == 0x41200000, "DamageDynamicsCollide.MaxMagnitude", 0x41200000, ReadUInt32(instance, 8));
        Expect(ReadUInt32(instance, 12) == 1, "DamageDynamicsCollide.DamageNugget count", 1, ReadUInt32(instance, 12));
        Expect(ReadUInt32(instance, 16) == 20, "DamageDynamicsCollide.DamageNugget relocation", 20, ReadUInt32(instance, 16));
        Expect(ReadUInt32(instance, 36) == 0, "DamageNugget.Radius", 0, ReadUInt32(instance, 36));
        Expect(instance[170] == 1, "DamageNugget.OnlyKillOwnerWhenTriggered", 1, instance[170]);
        Expect(chunk.ImportsBuffer.Length == 0, "DamageDynamicsCollide imports bytes", 0, chunk.ImportsBuffer.Length);
        Console.WriteLine($"  DamageDynamicsCollide bin={chunk.InstanceBuffer.Length}, relo={chunk.RelocationBuffer.Length}, imp={chunk.ImportsBuffer.Length}");
    }

    private static unsafe void TestReactionFXOnDamage()
    {
        const string xml = """
            <ReactionFXOnDamage xmlns="uri:ea.com:eala:asset">
              <DamageReactionFXTrigger PercentDamagedThreshold="99.99"
                  TimeBetweenTriggers="5.0s" />
              <HealingReactionFXTrigger TimeBetweenTriggers="2.0s"
                  ResetTimerWhenTimerBlocks="true"
                  SourceFilter="AOF_YurikoHealStations"
                  SoundToPlay="NEU_HealthStation_Heal" />
            </ReactionFXOnDamage>
            """;

        XmlDocument document = new();
        document.LoadXml(xml);
        XmlNamespaceManager namespaces = new(document.NameTable);
        namespaces.AddNamespace("ea", "uri:ea.com:eala:asset");
        Node node = new(document.CreateNavigator()!.SelectSingleNode("/ea:ReactionFXOnDamage", namespaces)!, namespaces);

        ReactionFXOnDamageModuleData* root;
        using Tracker tracker = new((void**)&root, (uint)sizeof(ReactionFXOnDamageModuleData), false);
        Marshaler.Marshal(node, root, tracker);
        Chunk chunk = new();
        tracker.MakeRelocatable(chunk);
        ReadOnlySpan<byte> instance = chunk.InstanceBuffer;
        Expect(instance.Length == 92, "ReactionFXOnDamage instance bytes", 92, instance.Length);
        Expect(ReadUInt32(instance, 8) == 1, "ReactionFXOnDamage damage count", 1, ReadUInt32(instance, 8));
        Expect(ReadUInt32(instance, 12) == 24, "ReactionFXOnDamage damage list relocation", 24, ReadUInt32(instance, 12));
        Expect(ReadUInt32(instance, 16) == 1, "ReactionFXOnDamage healing count", 1, ReadUInt32(instance, 16));
        Expect(ReadUInt32(instance, 20) == 56, "ReactionFXOnDamage healing list relocation", 56, ReadUInt32(instance, 20));
        Expect(ReadUInt32(instance, 24) == 48, "ReactionFXOnDamage damage threshold relocation", 48, ReadUInt32(instance, 24));
        Expect(ReadUInt32(instance, 28) == 52, "ReactionFXOnDamage damage timer relocation", 52, ReadUInt32(instance, 28));
        Expect(ReadUInt32(instance, 36) == 0, "ReactionFXOnDamage omitted damage voice", 0, ReadUInt32(instance, 36));
        Expect(ReadUInt32(instance, 48) == 0x42C7FAE1, "ReactionFXOnDamage damage threshold", 0x42C7FAE1, ReadUInt32(instance, 48));
        Expect(ReadUInt32(instance, 52) == 0x40A00000, "ReactionFXOnDamage damage timer", 0x40A00000, ReadUInt32(instance, 52));
        Expect(ReadUInt32(instance, 60) == 80, "ReactionFXOnDamage healing timer relocation", 80, ReadUInt32(instance, 60));
        Expect(ReadUInt32(instance, 64) == 84, "ReactionFXOnDamage healing filter relocation", 84, ReadUInt32(instance, 64));
        Expect(ReadUInt32(instance, 72) == 88, "ReactionFXOnDamage healing sound relocation", 88, ReadUInt32(instance, 72));
        Expect(instance[76] == 1, "ReactionFXOnDamage reset timer", 1, instance[76]);
        Expect(chunk.RelocationBuffer.Length == 32, "ReactionFXOnDamage relocation bytes", 32, chunk.RelocationBuffer.Length);
        Expect(chunk.ImportsBuffer.Length == 0, "ReactionFXOnDamage standalone imports bytes", 0, chunk.ImportsBuffer.Length);
        Console.WriteLine($"  ReactionFXOnDamage bin={chunk.InstanceBuffer.Length}, relo={chunk.RelocationBuffer.Length}, imp={chunk.ImportsBuffer.Length}");
    }

    private static unsafe void TestDamageSphereUpdate()
    {
        const string xml = """
            <DamageSphereUpdate xmlns="uri:ea.com:eala:asset"
                UnpackTime="1.35s" Weapon="AlliedFutureTankNeutronWeapon_IncrementalWeapon"
                RadiusMin="25" RadiusMax="180" ExpansionPerSecond="125"
                SphereBoneName="SHIELDLARGE" SphereSizeMultiplier="16.0"
                UnpackModelConditions="USER_4" ModelConditions="USER_5"
                UnpackObjectStatus="WEAPON_UPGRADED_01" ObjectStatus="WEAPON_UPGRADED_02">
              <ObjectFilter Rule="ALL"
                  Exclude="BRIDGE BRIDGE_SEGMENT BRIDGE_ENDCAP BRIDGE_GATEHOUSE" />
            </DamageSphereUpdate>
            """;

        XmlDocument document = new();
        document.LoadXml(xml);
        XmlNamespaceManager namespaces = new(document.NameTable);
        namespaces.AddNamespace("ea", "uri:ea.com:eala:asset");
        Node node = new(document.CreateNavigator()!.SelectSingleNode("/ea:DamageSphereUpdate", namespaces)!, namespaces);

        DamageSphereUpdateModuleData* root;
        using Tracker tracker = new((void**)&root, (uint)sizeof(DamageSphereUpdateModuleData), false);
        Marshaler.Marshal(node, root, tracker);
        Chunk chunk = new();
        tracker.MakeRelocatable(chunk);
        ReadOnlySpan<byte> instance = chunk.InstanceBuffer;
        Expect(instance.Length == 384, "DamageSphereUpdate instance bytes", 384, instance.Length);
        Expect(ReadUInt32(instance, 8) == 0x43340000, "DamageSphereUpdate RadiusMax", 0x43340000, ReadUInt32(instance, 8));
        Expect(ReadUInt32(instance, 12) == 0x41C80000, "DamageSphereUpdate RadiusMin", 0x41C80000, ReadUInt32(instance, 12));
        Expect(ReadUInt32(instance, 24) == 0x3F800000, "DamageSphereUpdate ScanFrequency", 0x3F800000, ReadUInt32(instance, 24));
        Expect(ReadUInt32(instance, 28) == 0x41200000, "DamageSphereUpdate Duration", 0x41200000, ReadUInt32(instance, 28));
        Expect(ReadUInt32(instance, 32) == 11, "DamageSphereUpdate sphere bone length", 11, ReadUInt32(instance, 32));
        Expect(ReadUInt32(instance, 36) == 372, "DamageSphereUpdate sphere bone relocation", 372, ReadUInt32(instance, 36));
        Expect(ReadUInt32(instance, 40) == 0x41800000, "DamageSphereUpdate SphereSizeMultiplier", 0x41800000, ReadUInt32(instance, 40));
        Expect(ReadUInt32(instance, 52) == 1, "DamageSphereUpdate ObjectFilter.Rule", 1, ReadUInt32(instance, 52));
        Expect(instance[172] == 0, "DamageSphereUpdate InitiallyActive", 0, instance[172]);
        Expect(instance[173] == 1, "DamageSphereUpdate DrawDebugCircle", 1, instance[173]);
        Expect(ReadUInt32(instance, 176) == 0x3FACCCCD, "DamageSphereUpdate UnpackTime", 0x3FACCCCD, ReadUInt32(instance, 176));
        Expect(ReadUInt32(instance, 184) == 0x42FA0000, "DamageSphereUpdate ExpansionPerSecond", 0x42FA0000, ReadUInt32(instance, 184));
        Expect(chunk.ImportsBuffer.Length == 0, "DamageSphereUpdate standalone imports bytes", 0, chunk.ImportsBuffer.Length);
        Console.WriteLine($"  DamageSphereUpdate bin={chunk.InstanceBuffer.Length}, relo={chunk.RelocationBuffer.Length}, imp={chunk.ImportsBuffer.Length}");
    }

    private static unsafe void TestYurikoShieldSphereUpdate()
    {
        const string xml = """
            <YurikoShieldSphereUpdate xmlns="uri:ea.com:eala:asset"
                InitiallyActive="true" RadiusMin="24" RadiusMax="24"
                ScanFrequency="0.25s" Duration="10s" MaxDamage="9999999999"
                DamageTypesNotToAbsorb="HEALING RADIATION"
                ObjectStatus="GENERIC_TOGGLE_STATE IGNORING_STEALTH"
                ModelCondition="USER_4" SphereBoneName="SHIELDSMALL"
                MajorShieldHitFX="FX_YurikoShieldHitSmallMajor"
                MinorShieldHitFX="FX_YurikoShieldHitSmallMinor"
                MinorShieldDamageTypes="GUN CANNON LASER UNDEFINED MAGIC PIERCE">
              <ObjectFilter Rule="ANY" Relationship="ALLIES"
                  Include="INFANTRY VEHICLE STRUCTURE" />
              <IgnoreInsideToInsideCheck Rule="ALL" />
            </YurikoShieldSphereUpdate>
            """;

        XmlDocument document = new();
        document.LoadXml(xml);
        XmlNamespaceManager namespaces = new(document.NameTable);
        namespaces.AddNamespace("ea", "uri:ea.com:eala:asset");
        Node node = new(document.CreateNavigator()!.SelectSingleNode("/ea:YurikoShieldSphereUpdate", namespaces)!, namespaces);

        YurikoShieldSphereUpdateModuleData* root;
        using Tracker tracker = new((void**)&root, (uint)sizeof(YurikoShieldSphereUpdateModuleData), false);
        Marshaler.Marshal(node, root, tracker);
        Chunk chunk = new();
        tracker.MakeRelocatable(chunk);
        ReadOnlySpan<byte> instance = chunk.InstanceBuffer;
        Expect(instance.Length == 460, "YurikoShieldSphereUpdate instance bytes", 460, instance.Length);
        Expect(ReadUInt32(instance, 8) == 0x41C00000, "YurikoShieldSphereUpdate RadiusMax", 0x41C00000, ReadUInt32(instance, 8));
        Expect(ReadUInt32(instance, 16) == 0x20, "YurikoShieldSphereUpdate DamageTypesNotToAbsorb[0]", 0x20, ReadUInt32(instance, 16));
        Expect(ReadUInt32(instance, 20) == 0x20, "YurikoShieldSphereUpdate DamageTypesNotToAbsorb[1]", 0x20, ReadUInt32(instance, 20));
        Expect(ReadUInt32(instance, 24) == 0x3E800000, "YurikoShieldSphereUpdate ScanFrequency", 0x3E800000, ReadUInt32(instance, 24));
        Expect(ReadUInt32(instance, 32) == 11, "YurikoShieldSphereUpdate sphere bone length", 11, ReadUInt32(instance, 32));
        Expect(ReadUInt32(instance, 36) == 328, "YurikoShieldSphereUpdate sphere bone relocation", 328, ReadUInt32(instance, 36));
        Expect(ReadUInt32(instance, 168) == 340, "YurikoShieldSphereUpdate ignore-filter relocation", 340, ReadUInt32(instance, 168));
        Expect(instance[172] == 1, "YurikoShieldSphereUpdate InitiallyActive", 1, instance[172]);
        Expect(ReadUInt32(instance, 176) == 0x501502F9, "YurikoShieldSphereUpdate MaxDamage", 0x501502F9, ReadUInt32(instance, 176));
        Expect(ReadUInt32(instance, 320) == 0x00626008, "YurikoShieldSphereUpdate minor damage flags", 0x00626008, ReadUInt32(instance, 320));
        Expect(chunk.ImportsBuffer.Length == 0, "YurikoShieldSphereUpdate standalone imports bytes", 0, chunk.ImportsBuffer.Length);
        Console.WriteLine($"  YurikoShieldSphereUpdate bin={chunk.InstanceBuffer.Length}, relo={chunk.RelocationBuffer.Length}, imp={chunk.ImportsBuffer.Length}");
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
