using System.Runtime.InteropServices;
using System.Text;
using System.Xml;
using BinaryAssetBuilder.Core.SageXml;
using Relo;
using SageBinaryData;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: explicitly reviewed asset roots use original EP1 native witnesses; this is not a blanket reflection admission path.
internal static class StockAssetRootCompiler
{
    internal sealed record Family(string Name,Type Type,int Size,string Source,bool Global,uint TypeId,uint TypeHash,bool WorldBuilder=false);
    internal static readonly Family[] Families={
        // Reborn: original global micromanagers retain their complete 176-byte header and actual EP1 fingerprint.
        new("AIMicroManagerData",typeof(Ep1AIMicroManagerData),176,"SkirmishAI/AIMicroManagerLibrary.xml",true,0x9077F377u,0x5960CDD4u),
        // Reborn: personality records use the complete original 520-byte global header and real EP1 fingerprint.
        new("AIPersonalityDefinition",typeof(Ep1AIPersonalityDefinition),520,"SkirmishAI/Personalities/BasePersonality.xml",true,0xD6D4F18Eu,0xC6B1A12Fu),
        // Reborn: strategic states require the complete original state/tactic/team ABI and global native fingerprint.
        new("AIStrategicStateDefinition",typeof(Ep1AIStrategicStateDefinition),64,"SkirmishAI/States/JapanBaseStates.xml",true,0x242FF6D4u,0xC04C76A1u),
        // Reborn: build-state entries retain their complete original global EP1 ABI and native fingerprint.
        new("AIBuildStateDefinition",typeof(Ep1AIBuildStateDefinition),20,"SkirmishAI/States/JapanBaseStates.xml",true,0x620C480Bu,0x835BC3A7u),
        // Reborn: budgets require the complete original EP1 heuristic graph and bank-account shares.
        new("AIBudgetStateDefinition",typeof(Ep1AIBudgetStateDefinition),20,"SkirmishAI/States/AIBudgetStateLibrary.xml",true,0xAC78CE63u,0xC4384827u),
        // Reborn: radius cursors require the complete original static decal-component ABI.
        new("RadiusCursor",typeof(Ep1RadiusCursor),12,"UI/RadiusCursors.xml",false,0x2078A161u,0xA0AF5530u),
        // Reborn: colors, UI fonts and shader override rules retain their independently observed EP1 fingerprints.
        new("MultiplayerColor",typeof(Ep1MultiplayerColor),28,"GlobalData/MultiplayerColors.xml",true,0x8E28081Du,0x131C20A0u),
        new("HeaderTemplate",typeof(Ep1HeaderTemplate),20,"System/HeaderTemplates.xml",true,0x48A3EEC6u,0x401FF46Bu),
        new("ShaderOverride",typeof(ShaderOverride),16,"GlobalData/ShaderOverrides.xml",false,0xBCC23F6Cu,0x3D5B1D16u),

        // Reborn: registries and rank grants have distinct original static/global native contracts.
        new("RegisteredObjectSet",typeof(Ep1RegisteredObjectSet),132,"GlobalData/RegisteredObjectSets.xml",false,0x3090CD40u,0x16B5ECC2u),
        new("PlayerTechRank",typeof(Ep1PlayerTechRank),20,"System/PlayerTechRanks.xml",true,0x6B7227C2u,0x12271BCEu),

        // Reborn: original emotion templates use their complete static 324-byte native header.
        new("EmotionTemplate",typeof(Ep1EmotionTemplate),324,"GlobalData/EmotionTemplates.xml",false,0xA9152D94u,0x3869C1A5u),
        // Reborn: all formation roots retain their original global fingerprints and variable nested lists.
        new("FormationAssistantUnitDefinition",typeof(Ep1FormationAssistantUnitDefinition),24,"GlobalData/FormationAssistant.xml",true,0x31F4DCE2u,0x5CD9BEA2u),
        new("FormationAssistantTemplate",typeof(Ep1FormationAssistantTemplate),12,"GlobalData/FormationAssistant.xml",true,0xAD9779B6u,0x09539ECBu),
        new("FormationSelectionTemplate",typeof(Ep1FormationSelectionTemplate),16,"GlobalData/FormationAssistant.xml",true,0xC92313E0u,0x05318E41u),
        new("FormationAssistant",typeof(Ep1FormationAssistant),40,"GlobalData/FormationAssistant.xml",true,0x7504005Cu,0x162572CCu),

        // Reborn: input singletons retain their actual static/global placement and native fingerprints.
        new("GeneralHotKeySlots",typeof(Ep1GeneralHotKeySlots),12,"GlobalData/GeneralHotKeySlots.xml",false,0x18DD9C43u,0x2710C7A0u),
        new("BasicMouseCursors",typeof(Ep1BasicMouseCursors),20,"GlobalData/BasicMouseCursors.xml",true,0x246F4433u,0x5A796288u),

        // Reborn: the EP1 singleton keeps all one hundred original audio slots and its real global fingerprint.
        new("MiscAudio",typeof(Ep1MiscAudio),404,"Sounds/MiscAudio.xml",true,0x7B6AE7D5u,0x28FADC88u),
        // Reborn: loading-screen strings, pools and per-map sequences retain distinct original global layouts.
        new("EngagingContent",typeof(Ep1EngagingContent),28,"GlobalData/EngagingContentCommon.xml",true,0xB6472112u,0x2B3B15E0u),
        new("EngagingContentGroup",typeof(Ep1EngagingContentGroup),12,"GlobalData/EngagingContentCommon.xml",true,0x00AC224Eu,0x59E0AC87u),
        new("EngagingContentSequence",typeof(Ep1EngagingContentSequence),20,"GlobalData/EngagingContentCommon.xml",true,0xE279D385u,0x0D81F33Du),
        // Reborn: cursor, font, vibration, mouse and sound settings retain distinct native fingerprints.
        new("MouseCursor",typeof(Ep1MouseCursor),16,"GlobalData/MouseCursors.xml",true,0xE1920287u,0x13ED4B89u),
        new("FontTranslation",typeof(Ep1FontTranslation),20,"GlobalData/FontTranslation.xml",true,0x4E9436E2u,0x6C315185u),
        new("FontDirectory",typeof(Ep1FontDirectory),12,"GlobalData/FontDirectory.xml",true,0xD4BABAC7u,0xD7993143u),
        new("Mouse",typeof(Ep1Mouse),20,"GlobalData/Mouse.xml",false,0x9687F57Bu,0x4A535D4Bu),
        new("ForceFeedbackEvent",typeof(Ep1ForceFeedbackEvent),20,"GlobalData/ForceFeedbackEvents.xml",false,0x1C48076Fu,0xE1FE71E8u),
        new("AudioEventLimitGroup",typeof(Ep1AudioEventLimitGroup),8,"Sounds/AudioEventLimitGroup.xml",true,0xE8D44EF5u,0x95314DEAu),
        new("AnimationSoundSettings",typeof(Ep1AnimationSoundSettings),8,"Sounds/AnimationSoundSettings.xml",true,0x4D9B7C79u,0xBCF803D4u),
        // Reborn: complete global AIData uses its original 220-byte native header and three tracked tree names.
        new("AIData",typeof(Ep1AIData),220,"SkirmishAI/AIData.xml",true,0xE4266B40u,0x4123A1AAu),
        // Reborn: Red Alert effects remain linked strong imports rather than copied modifier payloads.
        new("RedAlertTemplate",typeof(Ep1RedAlertTemplate),96,"GlobalData/RedAlerts.xml",false,0xF6D753D1u,0x49F4D026u),
        // Reborn: the original global targeting heuristics use the complete 212-byte EP1 record.
        new("AITargetingHeuristic",typeof(Ep1AITargetingHeuristic),212,"SkirmishAI/AITargetHeuristicLibrary.xml",true,0x373E10FAu,0x0EEA25F7u),
        // Reborn: retail static carries unit-intro voices and invisibility templates.
        new("UnitIntro",typeof(Ep1UnitIntro),12,"GlobalData/UnitIntros.xml",false,0x6F78B1A4u,0x570806D2u),
        new("InvisibilityTemplate",typeof(Ep1InvisibilityTemplate),252,"GlobalData/InvisibilityTemplates.xml",false,0x554FC288u,0xCCD315FFu),
        // Reborn: EVA roots are global; original terrain road witnesses are retained in the editor catalog.
        new("EvaSettings",typeof(Ep1EvaSettings),20,"Sounds/Eva.xml",true,0x8CC42BF2u,0xAFAD556Cu),
        new("EvaEvent",typeof(Ep1EvaEvent),44,"Sounds/Eva.xml",true,0x1553B19Eu,0x70A64BEAu),
        new("Road",typeof(Road),24,"GlobalData/Roads.xml",false,0xD19D90C6u,0x2BFD2964u,WorldBuilder:true),
        // Reborn: strategic-name imports and opening orders are distinct witnessed EP1 global roots.
        new("AIStrategicStateName",typeof(Ep1StrategicStateName),16,"SkirmishAI/States/AIStrategicStateNames.xml",true,0x77D6BE90u,0x048CE7A4u),
        new("SkirmishOpeningMove",typeof(SkirmishOpeningMove),20,"SkirmishAI/OpeningMoves/SovietOpeningMoves.xml",true,0xECBE73E8u,0x6FF5D680u),
        new("HotKeySlot",typeof(Ep1HotKeySlot),24,"GlobalData/HotKeySlots.xml",true,0xA6E6BBA7u,0x93E23672u),
        new("MappableKey",typeof(MappableKey),12,"GlobalData/MappableKeys.xml",true,0x5080A5D8u,0x84B6AEC1u),
        new("UIQuickChatMessage",typeof(Ep1UIQuickChatMessage),16,"UI/QuickChatMessages.xml",false,0xF4491BA9u,0x2C498C12u)};
    internal static bool Supports(string name)=>Families.Any(f=>f.Name==name);
    internal static Family For(string name)=>Families.Single(f=>f.Name==name);

    //-------------------------------------------------------------------------------------------------
    /** Reborn: prove every distinct authored group/optional-audio shape against its actual original native record before admission. */
    //-------------------------------------------------------------------------------------------------
    internal static void Qualify(Family family,string manifest,string global,SchemaSet schemas,ManifestAsset[] available)
    {
        if(Marshal.SizeOf(family.Type)!=family.Size)throw new InvalidDataException("EP1 asset root layout differs: "+family.Name);
        string path=family.WorldBuilder?WorldBuilderAssetResolver.CatalogPath():family.Global?global:manifest;
        var catalog=ManifestReader.Read(SdkEnvironmentPreflight.Read(path,64*1048576));
        // Reborn: the independent witness must be a valid linked EP1 catalog, including editor-only witnesses.
        TypeRegistryAudit.ValidateTarget(catalog.Header.Version,catalog.Header.AllTypesHash);
        if(!catalog.Header.IsLinked||catalog.Validate().Count!=0)throw new InvalidDataException("Invalid original asset-root witness catalog.");
        var examples=catalog.Assets.Where(a=>a.TypeName==family.Name).ToArray();
        if(examples.Length==0||examples.Any(a=>a.Tokenized!=0||a.TypeId!=family.TypeId||a.TypeHash!=family.TypeHash))throw new InvalidDataException("EP1 asset root fingerprint differs: "+family.Name);
        var originals=GameObjectExtendedRootContract.Original(family.Source,family.Name)
            .Where(e=>examples.Any(a=>a.Name==family.Name+":"+e.GetAttribute("id")))
            .Where(e=>!e.OuterXml.Contains("=$",StringComparison.Ordinal)&&!e.HasAttribute("inheritFrom")).ToArray();
        if(originals.Length==0)throw new InvalidDataException("No original source-backed root witness: "+family.Name);
        // Reborn: editor-only witness textures are allowed only in this original proof, never injected into runtime resolution.
        if(family.WorldBuilder){var known=available.Select(a=>new AssetId(a.TypeId,a.InstanceId)).ToHashSet();available=available.Concat(catalog.Assets.Where(a=>!known.Contains(new(a.TypeId,a.InstanceId)))).ToArray();}
        // Reborn: the base witness has no delay entries; prove populated inherited list records independently as well.
        if(family.Name=="AIMicroManagerData")foreach(string id in new[]{"StandardMicroManager_MEDIUM","StandardMicroManager_EASY","NoRiotShieldTest"})
        {
            var original=GameObjectExtendedRootContract.Original(family.Source,family.Name).Single(e=>e.GetAttribute("id")==id);
            var entries=original.ChildNodes.OfType<XmlElement>().Where(e=>e.LocalName=="SpecificSpecialPowerDelay"&&e.NamespaceURI==SchemaSet.XmlNamespace).ToArray();
            var owner=examples.Single(a=>a.Name==family.Name+":"+id);var bytes=W3DContainerCompiler.ReadStock(path,catalog,owner);
            uint count=System.Buffers.Binary.BinaryPrimitives.ReadUInt32LittleEndian(bytes.AsSpan(144,4)),pointer=System.Buffers.Binary.BinaryPrimitives.ReadUInt32LittleEndian(bytes.AsSpan(148,4));
            if(count!=entries.Length||count!=1||pointer>bytes.Length-16)throw new InvalidDataException("Original micro-manager delay list extent differs.");
            var probe=SdkXmlDiagnostics.Parse(Encoding.UTF8.GetBytes("<AIMicroManagerData xmlns=\""+SchemaSet.XmlNamespace+"\" id=\"RebornMicroDelayWitness\"/>"));
            probe.DocumentElement!.AppendChild(probe.ImportNode(entries[0],true));var proof=SchemaNativeContract.Resolve(probe,schemas,available).GetElementsByTagName("SpecificSpecialPowerDelay",SchemaSet.XmlNamespace)[0] as XmlElement;
            SchemaNativeContract.VerifyStock(proof!,typeof(Ep1AISpecificSpecialPowerDelayInfo),0,bytes,checked((int)pointer),owner.References.ToArray(),required:true);
        }
        foreach(var original in family.Name is "AIMicroManagerData" or "AIPersonalityDefinition" or "AIStrategicStateDefinition" or "AIBuildStateDefinition" or "AIBudgetStateDefinition" or "RadiusCursor" or "MultiplayerColor" or "HeaderTemplate" or "ShaderOverride" or "RegisteredObjectSet" or "PlayerTechRank" or "EmotionTemplate" or "FormationAssistantUnitDefinition" or "FormationAssistantTemplate" or "FormationSelectionTemplate" or "FormationAssistant" or "EvaEvent" or "InvisibilityTemplate" or "AITargetingHeuristic" or "MouseCursor" or "FontTranslation" or "ForceFeedbackEvent" or "EngagingContent" or "EngagingContentGroup" or "EngagingContentSequence"?originals:originals.GroupBy(e=>family.Name=="HotKeySlot"?e.GetAttribute("Group"):e.HasAttribute("Sound").ToString()).Select(g=>g.First()))
        {
            string name=family.Name+":"+original.GetAttribute("id");
            var asset=examples.Single(a=>a.Name==name);
            var source=SdkXmlDiagnostics.Parse(Encoding.UTF8.GetBytes(original.OuterXml));
            Normalize(source);var proof=SchemaNativeContract.Resolve(source,schemas,available);proof.DocumentElement!.RemoveAttribute("id");
            SchemaNativeContract.VerifyStock(proof.DocumentElement,family.Type,0,W3DContainerCompiler.ReadStock(path,catalog,asset),0,asset.References.ToArray(),required:true);
        }
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: remove indentation-only character nodes from reviewed complex asset roots with no element children. */
    //-------------------------------------------------------------------------------------------------
    internal static void NormalizeFormatting(XmlElement root)
    {
        if(!Supports(root.LocalName)||root.NamespaceURI!=SchemaSet.XmlNamespace||root.ChildNodes.OfType<XmlElement>().Any())return;
        foreach(var node in root.ChildNodes.Cast<XmlNode>().Where(n=>n.NodeType is XmlNodeType.Whitespace or XmlNodeType.SignificantWhitespace||n.NodeType==XmlNodeType.Text&&string.IsNullOrWhiteSpace(n.Value)).ToArray())root.RemoveChild(node);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: EA's BuildingPlacementDistance list is explicitly sorted by Distance in the original XSD. */
    //-------------------------------------------------------------------------------------------------
    internal static void Normalize(XmlDocument source)
    {
        var root=source.DocumentElement!;NormalizeFormatting(root);if(root.LocalName!="AITargetingHeuristic")return;
        var distances=root.ChildNodes.OfType<XmlElement>().Where(e=>e.LocalName=="BuildingPlacementDistance"&&e.NamespaceURI==SchemaSet.XmlNamespace).ToArray();
        var sorted=distances.OrderBy(e=>XmlConvert.ToSingle(e.GetAttribute("Distance"))).ToArray();
        if(sorted.SequenceEqual(distances))return;
        foreach(var item in distances)root.RemoveChild(item);foreach(var item in sorted)root.AppendChild(item);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: allocate a source-sized native root through its explicitly selected typed writer and track all auxiliary storage. */
    //-------------------------------------------------------------------------------------------------
    internal static unsafe Chunk Compile(XmlNode source)
    {
        var family=For(source.LocalName);
        var xml=SdkXmlDiagnostics.Parse(Encoding.UTF8.GetBytes(source.OuterXml));
        Normalize(xml);
        // Reborn: heuristic discriminators are private writer metadata; the admitted source and proof XML remain unchanged.
        if(family.Name is "AIBudgetStateDefinition" or "AIBuildStateDefinition" or "AIStrategicStateDefinition" or "AIPersonalityDefinition")BinaryAssetBuilder.XmlCompiler.Ep1AIHeuristicLayout.PrepareWriter(xml.DocumentElement!);
        var namespaces=new XmlNamespaceManager(xml.NameTable);namespaces.AddNamespace("ea",SchemaSet.XmlNamespace);
        var node=new Node(xml.DocumentElement!.CreateNavigator()!,namespaces);
        void* root;using var tracker=new Tracker(&root,(uint)family.Size,false);
        switch(family.Name)
        {
            case "AIStrategicStateName":Marshaler.Marshal(node,(Ep1StrategicStateName*)root,tracker);break;
            case "SkirmishOpeningMove":Marshaler.Marshal(node,(SkirmishOpeningMove*)root,tracker);break;
            case "EvaSettings":Marshaler.Marshal(node,(Ep1EvaSettings*)root,tracker);break;
            case "EvaEvent":Marshaler.Marshal(node,(Ep1EvaEvent*)root,tracker);break;
            case "Road":Marshaler.Marshal(node,(Road*)root,tracker);break;
            case "UnitIntro":Marshaler.Marshal(node,(Ep1UnitIntro*)root,tracker);break;
            case "InvisibilityTemplate":Marshaler.Marshal(node,(Ep1InvisibilityTemplate*)root,tracker);break;
            case "AITargetingHeuristic":Marshaler.Marshal(node,(Ep1AITargetingHeuristic*)root,tracker);break;
            case "RedAlertTemplate":Marshaler.Marshal(node,(Ep1RedAlertTemplate*)root,tracker);break;
            case "AIData":Marshaler.Marshal(node,(Ep1AIData*)root,tracker);break;
            case "MouseCursor":Marshaler.Marshal(node,(Ep1MouseCursor*)root,tracker);break;
            case "FontTranslation":Marshaler.Marshal(node,(Ep1FontTranslation*)root,tracker);break;
            case "FontDirectory":Marshaler.Marshal(node,(Ep1FontDirectory*)root,tracker);break;
            case "Mouse":Marshaler.Marshal(node,(Ep1Mouse*)root,tracker);break;
            case "ForceFeedbackEvent":Marshaler.Marshal(node,(Ep1ForceFeedbackEvent*)root,tracker);break;
            case "AudioEventLimitGroup":Marshaler.Marshal(node,(Ep1AudioEventLimitGroup*)root,tracker);break;
            case "AnimationSoundSettings":Marshaler.Marshal(node,(Ep1AnimationSoundSettings*)root,tracker);break;
            case "EngagingContent":Marshaler.Marshal(node,(Ep1EngagingContent*)root,tracker);break;
            case "EngagingContentGroup":Marshaler.Marshal(node,(Ep1EngagingContentGroup*)root,tracker);break;
            case "EngagingContentSequence":Marshaler.Marshal(node,(Ep1EngagingContentSequence*)root,tracker);break;
            case "GeneralHotKeySlots":Marshaler.Marshal(node,(Ep1GeneralHotKeySlots*)root,tracker);break;
            case "BasicMouseCursors":Marshaler.Marshal(node,(Ep1BasicMouseCursors*)root,tracker);break;
            case "FormationAssistantUnitDefinition":Marshaler.Marshal(node,(Ep1FormationAssistantUnitDefinition*)root,tracker);break;
            case "FormationAssistantTemplate":Marshaler.Marshal(node,(Ep1FormationAssistantTemplate*)root,tracker);break;
            case "FormationSelectionTemplate":Marshaler.Marshal(node,(Ep1FormationSelectionTemplate*)root,tracker);break;
            case "FormationAssistant":Marshaler.Marshal(node,(Ep1FormationAssistant*)root,tracker);break;
            case "EmotionTemplate":Marshaler.Marshal(node,(Ep1EmotionTemplate*)root,tracker);break;
            case "RegisteredObjectSet":Marshaler.Marshal(node,(Ep1RegisteredObjectSet*)root,tracker);break;
            case "PlayerTechRank":Marshaler.Marshal(node,(Ep1PlayerTechRank*)root,tracker);break;
            case "MultiplayerColor":Marshaler.Marshal(node,(Ep1MultiplayerColor*)root,tracker);break;
            case "HeaderTemplate":Marshaler.Marshal(node,(Ep1HeaderTemplate*)root,tracker);break;
            case "ShaderOverride":Marshaler.Marshal(node,(ShaderOverride*)root,tracker);break;
            case "RadiusCursor":Marshaler.Marshal(node,(Ep1RadiusCursor*)root,tracker);break;
            case "AIBudgetStateDefinition":Marshaler.Marshal(node,(Ep1AIBudgetStateDefinition*)root,tracker);break;
            case "AIBuildStateDefinition":Marshaler.Marshal(node,(Ep1AIBuildStateDefinition*)root,tracker);break;
            case "AIStrategicStateDefinition":Marshaler.Marshal(node,(Ep1AIStrategicStateDefinition*)root,tracker);break;
            case "AIPersonalityDefinition":Marshaler.Marshal(node,(Ep1AIPersonalityDefinition*)root,tracker);break;
            case "AIMicroManagerData":Marshaler.Marshal(node,(Ep1AIMicroManagerData*)root,tracker);break;
            case "MiscAudio":Marshaler.Marshal(node,(Ep1MiscAudio*)root,tracker);break;
            case "HotKeySlot":Marshaler.Marshal(node,(Ep1HotKeySlot*)root,tracker);break;
            case "MappableKey":Marshaler.Marshal(node,(MappableKey*)root,tracker);break;
            case "UIQuickChatMessage":Marshaler.Marshal(node,(Ep1UIQuickChatMessage*)root,tracker);break;
            default:throw new InvalidDataException("Unreviewed asset root: "+family.Name);
        }
        var chunk=new Chunk();if(!tracker.MakeRelocatable(chunk))throw new InvalidDataException("asset root allocation failed: "+family.Name);return chunk;
    }
}
