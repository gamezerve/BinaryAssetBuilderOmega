// Reborn: player-tech upgrade bindings use the ordinary source, identity and native compilation routes.
// Reborn: supported WeaponTemplate-only declarations select the normal backend without a placeholder GameObject.
// Reborn: authored AttributeModifier assets share ordinary includes, definitions, inheritance and source diagnostics.
// Reborn: authored special-power templates participate in shared source, inheritance and native stream publication.
// Reborn: experience templates share ordinary source preparation, inheritance and explicit asset emission.
// Reborn: particle templates participate in the shared source, inheritance and native asset pipeline.
// Reborn: authored FXList assets use the same include/inheritance/source-location pipeline as other supported assets.
using System.Security.Cryptography;
using System.Text;
using System.Xml;
using BinaryAssetBuilder.Core.SageXml;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: bridge the existing source-definition/inheritance engine into GameObject authoring, retaining actual all/instance publication roles.
// Reborn: UnitTypeIcon participates in normal source routing, inheritance, identity and diagnostic context.
internal static class ConscriptSourcePreparation
{
    private const string Ea="uri:ea.com:eala:asset";

    //-------------------------------------------------------------------------------------------------
    /** Reborn: capture a confined graph, prepare each owner in its defining context and publish all-imported owners only, never instance bases. */
    //-------------------------------------------------------------------------------------------------
    internal static ConscriptSourceInput.Input Load(string entry,SdkSourceSearch? search=null)
    {
        // Reborn: production source graphs scale with input size, not diagnostic fixture quotas.
        using var sourceScale=SdkSourceScale.Enter();
        // Reborn: nested global entry points share the mod's DATA alias root rather than inventing an AdditionalMaps/Data tree.
        entry=SdkEnvironmentPreflight.Absolute(entry);string root=search?.ModData??SdkEnvironmentPreflight.DirectoryPath(Path.GetDirectoryName(entry)!);
        // Reborn: real source XML uses process-representable reads rather than fixture file/graph byte quotas.
        byte[] initial=SdkEnvironmentPreflight.Read(entry,SdkSourceScale.XmlLimit(4*1048576));var first=Parse(initial,entry);
        // Reborn: preserve the previous standalone complete-object input without claiming it has an AssetDeclaration preprocessing context.
        if(first.DocumentElement?.LocalName is "ArmorTemplate" or "GameObject" or "PurchasePlayerTechButtonTemplate" or "UnitAbilityButtonTemplate" or "PlayerTemplate" or "PlayerTech" or "PlayerTechStoreTemplate" or "PlayerTechUpgradeBinding" or "ButtonSingleStateData" or "JoypadCommandBarButtonTemplate" or "PlayerPowerButtonTemplate" or "SpecialPowerTemplate" or "AttributeModifier" or "WeaponTemplate" or "CampaignEnumerator" or "CampaignTemplate" or "MissionTemplate" or "MpGameRules" or "Credits" or "TutorialMapData" or "MapMetaData" or "CameraSettings" or "Weather" or "Video" or "VideoEventList" or "PathMusicEvent" or "PathMusicGameDynamicState" or "GameMap" or "GameScriptList" or "RadarTerrainImage" or "TerrainTextureAtlas" or "UnitTypeIcon" or "ExperienceLevelTemplate" or "LocomotorTemplate" or "FXParticleSystemTemplate" or "FXList" or "LogicCommandSet" or "UpgradeTemplate" or "W3DContainer" or "W3DHierarchy" or "W3DCollisionBox" or "W3DMesh" or "W3DAnimation" or "OnDemandTexture" or "OnDemandTextureImage" or "HealthBarTemplate" or "HealthBarSettings") return new(first,null,new[]{new ConscriptSourceInput.SourceFile(entry,Hash(initial))}){Origins=new[]{new ConscriptSourceInput.AssetOrigin(first.DocumentElement!.LocalName,first.DocumentElement!.GetAttribute("id"),entry)},Locations=new(new[]{new SdkXmlDiagnostics.Snapshot(entry,initial)})};
        string fixture=ReferencePipelineSmokeTest.FindFixture();string output=Path.Combine(Path.GetTempPath(),"Reborn-SourcePlan-"+Guid.NewGuid().ToString("N"));
        var environment=new SdkEnvironmentPreflight.Report("ra3ep1",Path.GetDirectoryName(fixture)!,fixture,0,"",root,entry,Hash(initial),output,Array.Empty<SdkEnvironmentPreflight.Mapping>(),true,true,false,false,false,"0x5454A8E9",Array.Empty<string>());
        var paths=SdkSourcePathAudit.Inspect(environment,search:search,w3xSources:true,skipMissingArtIncludes:true);
        // Reborn: report the actual physical winner before resource/processor admission; finding a W3X never means it was compiled.
        foreach(var selection in paths.Searches.Where(s=>!s.ExpectedMissing&&s.LogicalPath.Contains(':')))Console.WriteLine("Reborn: source path ["+selection.Layer+"] "+selection.LogicalPath+" -> "+selection.PhysicalPath);
        if(!paths.ScopedPathAuditComplete || paths.StoppedAtLimit)
        {
            var failure=new InvalidDataException("Source graph: "+string.Join("; ",paths.Issues.Select(i=>i.Code+" "+i.LogicalPath+": "+i.Detail)));
            if(paths.Issues.FirstOrDefault(i=>i.Location!=null)?.Location is SdkXmlDiagnostics.Location location)SdkXmlDiagnostics.Attach(failure,location);throw failure;
        }
        var snapshots=paths.Sources.ToDictionary(s=>s.PhysicalPath,s=>SdkEnvironmentPreflight.Read(s.PhysicalPath,SdkSourceScale.XmlLimit(4*1048576)),StringComparer.OrdinalIgnoreCase);
        foreach(var source in paths.Sources)if(Hash(snapshots[source.PhysicalPath])!=source.Sha256)throw new InvalidDataException("Source changed during graph capture.");
        // Reborn: locating a real resource does not enable its processor; report the unique referring literal or an explicitly labelled document context.
        // Reborn: Texture/@File captures TGA raster inputs and precompiled DDS; other payload kinds still need their own processor.

        var textures = paths.Resources.Where(r =>
            (r.Field is "Texture/@File" or "OnDemandTexture/@File") &&
            (r.PhysicalPath.EndsWith(".tga", StringComparison.OrdinalIgnoreCase) ||
             r.PhysicalPath.EndsWith(".dds", StringComparison.OrdinalIgnoreCase))
        ).ToArray();

        // Reborn: WAV AudioFile resources are captured alongside raster inputs for their native audio processor.
        var audio=paths.Resources.Where(r=>r.Field=="AudioFile/@File"&&(r.PhysicalPath.EndsWith(".wav",StringComparison.OrdinalIgnoreCase)||r.PhysicalPath.EndsWith(".easnd",StringComparison.OrdinalIgnoreCase))).ToArray();
        // Reborn: terrain tile rasters are captured beside regular textures and later bound to the native atlas encoder.
        var terrain=paths.Resources.Where(r=>r.Field is "Tile/@BaseTexture" or "Tile/@NormalTexture").ToArray();
        // Reborn: capture the Pathfinder header for authored PathMusicEvent declarations and their instance bases.
        var musicHeaders=paths.Resources.Where(r=>r.Field=="PathMusicEvent/@PathfinderEventHeader"&&r.PhysicalPath.EndsWith(".h",StringComparison.OrdinalIgnoreCase));
        var accepted=textures.Concat(audio).Concat(terrain).Concat(musicHeaders).ToArray();
        var unsupported = paths.Resources.Except(accepted).ToArray();

        if (unsupported.Length!=0)
        {
            var failure=new InvalidDataException("Resource processor is not connected for this resource (Texture TGA/DDS, AudioFile WAV/EA SND, PathMusicEvent headers, and W3X declarations are supported); no resources are silently skipped.");
            var resource=unsupported.FirstOrDefault();string owner=resource?.Document??entry;
            var source=Parse(snapshots[owner],owner);
            if(resource!=null){var matches=source.SelectNodes("//@* | //*[not(*)]")!.Cast<XmlNode>().Where(n=>(n is XmlAttribute?n.Value:n.InnerText)==resource.LogicalPath).ToArray();if(matches.Length==1)SdkXmlDiagnostics.At(failure,matches[0]);}
            new SdkXmlDiagnostics.SourceMap(snapshots.Select(s=>new SdkXmlDiagnostics.Snapshot(s.Key,s.Value))).Enrich(failure,owner,source);throw failure;
        }
        var resources=accepted.Select(r=>{var bytes=SdkEnvironmentPreflight.Read(r.PhysicalPath,64*1048576);if(bytes.Length!=r.Bytes)throw new InvalidDataException("Resource changed during graph capture: "+r.PhysicalPath);return new ConscriptSourceInput.ResourceFile(r.Document,r.Field,r.LogicalPath,r.PhysicalPath,Hash(bytes),bytes);}).ToArray();
        // Reborn: path inventory is not language admission; reject ignored metadata throughout the graph before emitting anything.
        foreach(var snapshot in snapshots){var source=Parse(snapshot.Value,snapshot.Key);try{ValidateEnvelope(source.DocumentElement!);}catch(InvalidDataException error){new SdkXmlDiagnostics.SourceMap(new[]{new SdkXmlDiagnostics.Snapshot(snapshot.Key,snapshot.Value)}).Enrich(error,snapshot.Key,source);throw;}}
        // Reborn: merge schema-valid partial bases first; final source/native admission still applies to the emitted complete object only.
        var schemas=ConscriptSourceInput.Schemas();var fragments=new SdkGameObjectFragments(paths);var profile=new SdkInstanceInheritanceProfile(schemas,paths,expressions:true,conscriptCopy:true,gameObjectTrees:true,fragments:fragments);
        var assets=new List<XmlElement>();var origins=new Dictionary<XmlElement,string>();var emitted=new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        // Reborn: collect actual successful overlays, including source-only intermediate bases, independently of runtime import records.
        var inheritanceUses=new HashSet<SdkSelfAttributeInheritance.Overlay>();
        // Reborn: retain all-Include dependency order without a recursive emitter or repeated whole-graph edge scans.
        var allEdges=paths.Includes.Where(e=>e.Kind=="all"&&!e.SkippedMissingArt).ToLookup(e=>e.Document,StringComparer.OrdinalIgnoreCase);
        var work=new Stack<(string Path,bool Exit)>();var scheduled=new HashSet<string>(StringComparer.OrdinalIgnoreCase);work.Push((entry,false));
        while(work.Count!=0){var frame=work.Pop();if(frame.Exit){Emit(frame.Path);continue;}if(!scheduled.Add(frame.Path))continue;work.Push((frame.Path,true));foreach(var edge in allEdges[frame.Path].Reverse())work.Push((edge.PhysicalPath??throw new InvalidDataException("Unresolved all Include."),false));}
        var games=assets.Where(a=>a.LocalName=="GameObject").ToArray();var commands=assets.Where(a=>a.LocalName=="LogicCommand").ToArray();
        // Reborn: support assets are explicit emitted declarations, not unwanted instance-base leakage or hidden generated data.
        // Reborn: command-set-only mods use the same native graph without inventing a placeholder GameObject.
        // Reborn: every explicit GameObject is emitted; only instance imports stay source-only. The first object is a receipt anchor, not an asset-count restriction.
        // Reborn: an upgrade-only graph is a real standalone stream; never invent a GameObject just to select the backend.
        // Reborn: W3DContainer is also a real standalone asset stream, not an implicit GameObject.
        // Reborn: authored audio events may form a standalone stream or accompany actual GameObjects without dummy assets.
        if(games.Length==0&&!assets.Any(a=>a.LocalName is "AudioEvent" or "ArmorTemplate" or "PurchasePlayerTechButtonTemplate" or "UnitAbilityButtonTemplate" or "PlayerTemplate" or "PlayerTech" or "PlayerTechStoreTemplate" or "PlayerTechUpgradeBinding" or "ButtonSingleStateData" or "JoypadCommandBarButtonTemplate" or "PlayerPowerButtonTemplate" or "SpecialPowerTemplate" or "AttributeModifier" or "WeaponTemplate" or "CampaignEnumerator" or "CampaignTemplate" or "MissionTemplate" or "MpGameRules" or "Credits" or "TutorialMapData" or "MapMetaData" or "CameraSettings" or "Weather" or "Video" or "VideoEventList" or "PathMusicEvent" or "PathMusicGameDynamicState" or "GameMap" or "GameScriptList" or "RadarTerrainImage" or "TerrainTextureAtlas" or "UnitTypeIcon" or "ExperienceLevelTemplate" or "LocomotorTemplate" or "FXParticleSystemTemplate" or "FXList" or "LogicCommandSet" or "UpgradeTemplate" or "W3DContainer" or "W3DHierarchy" or "W3DCollisionBox" or "W3DMesh" or "W3DAnimation" or "Multisound" or "AudioFile" or "Texture" or "PackedTextureImage" or "OnDemandTexture" or "OnDemandTextureImage" or "HealthBarTemplate" or "HealthBarSettings"))throw new InvalidDataException("This route requires a supported emitted asset; imported bases must use type=instance.");
        var primary=games.FirstOrDefault()??assets.First(a=>a.LocalName is "AudioEvent" or "ArmorTemplate" or "PurchasePlayerTechButtonTemplate" or "UnitAbilityButtonTemplate" or "PlayerTemplate" or "PlayerTech" or "PlayerTechStoreTemplate" or "PlayerTechUpgradeBinding" or "ButtonSingleStateData" or "JoypadCommandBarButtonTemplate" or "PlayerPowerButtonTemplate" or "SpecialPowerTemplate" or "AttributeModifier" or "WeaponTemplate" or "CampaignEnumerator" or "CampaignTemplate" or "MissionTemplate" or "MpGameRules" or "Credits" or "TutorialMapData" or "MapMetaData" or "CameraSettings" or "Weather" or "Video" or "VideoEventList" or "PathMusicEvent" or "PathMusicGameDynamicState" or "GameMap" or "GameScriptList" or "RadarTerrainImage" or "TerrainTextureAtlas" or "UnitTypeIcon" or "ExperienceLevelTemplate" or "LocomotorTemplate" or "FXParticleSystemTemplate" or "FXList" or "LogicCommandSet" or "UpgradeTemplate" or "W3DContainer" or "W3DHierarchy" or "W3DCollisionBox" or "W3DMesh" or "W3DAnimation" or "Multisound" or "AudioFile" or "Texture" or "PackedTextureImage" or "OnDemandTexture" or "OnDemandTextureImage" or "HealthBarTemplate" or "HealthBarSettings");
        // Reborn: duplicates across included source documents use the same copy-friendly coded diagnostic as local declarations.
        var identities=assets.GroupBy(a=>a.LocalName+":"+a.GetAttribute("id"),StringComparer.OrdinalIgnoreCase);var duplicate=identities.FirstOrDefault(g=>g.Count()!=1);
        if(duplicate!=null){
            // Reborn: emission already knows each declaring file; resolve its raw declaration instead of guessing through an ambiguous cross-file identity.
            var declarations=duplicate.ToArray();var locations=declarations.Select(DeclarationLocation).ToArray();
            var error=SdkXmlDiagnostics.At(new InvalidDataException("DUPLICATEASSET ' "+duplicate.Key+" ' The asset is emitted more than once. Conflicting declarations: "+string.Join("; ",locations.Where((_,index)=>index!=1))+"."),declarations[1]);
            SdkXmlDiagnostics.Attach(error,locations[1]);throw error;
        }
        foreach(var source in paths.Sources)if(Hash(SdkEnvironmentPreflight.Read(source.PhysicalPath,SdkSourceScale.XmlLimit(4*1048576)))!=source.Sha256)throw new InvalidDataException("Source changed during inheritance/definition preparation.");
        // Reborn: XInclude fragments are real source dependencies, including hash and higher-priority mod-shadow checks before publication.
        fragments.VerifySources();
        var searches=paths.Searches.Concat(fragments.Searches).ToArray();SdkSourceSearch.Verify(searches);
        return new(Detach(primary),commands.Length==0?null:Detach(commands[0]),paths.Sources.Select(s=>new ConscriptSourceInput.SourceFile(s.PhysicalPath,s.Sha256)).Concat(fragments.Sources).DistinctBy(s=>s.Path,StringComparer.OrdinalIgnoreCase).ToArray()){AdditionalAssets=assets.Where(a=>a!=primary&&(commands.Length==0||a!=commands[0])).Select(Detach).ToArray(),Origins=assets.Select(a=>new ConscriptSourceInput.AssetOrigin(a.LocalName,a.GetAttribute("id"),origins[a])).ToArray(),InheritanceUses=inheritanceUses.ToArray(),Searches=searches,Warnings=paths.Issues.Where(i=>i.Code=="MISSINGART").ToArray(),Locations=new(snapshots.Select(s=>new SdkXmlDiagnostics.Snapshot(s.Key,s.Value)).Concat(fragments.Snapshots)),Resources=resources};

        //-------------------------------------------------------------------------------------------------
        /** Reborn: locate an emitted declaration only inside its captured physical source, preserving honest context fallback if no unique raw declaration survives. */
        //-------------------------------------------------------------------------------------------------
        SdkXmlDiagnostics.Location DeclarationLocation(XmlElement asset)
        {
            string file=origins[asset];var raw=SdkXmlDiagnostics.Lines(snapshots[file],file);var matches=raw.Root!.Elements().Where(e=>e.Name.NamespaceName==asset.NamespaceURI&&e.Name.LocalName==asset.LocalName&&(string?)e.Attribute("id")==asset.GetAttribute("id")).ToArray();
            return matches.Length==1?SdkXmlDiagnostics.Point(file,matches[0]):SdkXmlDiagnostics.Point(file,raw.Root,true);
        }

        //-------------------------------------------------------------------------------------------------
        /** Reborn: all Includes contribute emitted owner assets; instance Includes are read by the existing engine but never emitted by this traversal. */
        //-------------------------------------------------------------------------------------------------
        void Emit(string path)
        {
            if(!emitted.Add(path))return;XmlDocument? xml=null;
            try{
            xml=Parse(snapshots[path],path);var document=xml.DocumentElement!;
            // Reborn: child declarations have already been emitted by the iterative dependency walk.
            var owners=document.ChildNodes.OfType<XmlElement>().Where(e=>e.LocalName is not ("Tags" or "Includes" or "Defines")).ToArray();
            // Reborn: AudioEvent uses the same explicit source/include/schema path as other supported assets.
            if(owners.FirstOrDefault(a=>a.LocalName is not ("AudioEvent" or "GameObject" or "LogicCommand" or "LogicCommandSet" or "ArmorTemplate" or "WeaponTemplate" or "UpgradeTemplate" or "ObjectCreationList" or "PurchasePlayerTechButtonTemplate" or "UnitAbilityButtonTemplate" or "PlayerTemplate" or "PlayerTech" or "PlayerTechStoreTemplate" or "PlayerTechUpgradeBinding" or "ButtonSingleStateData" or "JoypadCommandBarButtonTemplate" or "PlayerPowerButtonTemplate" or "SpecialPowerTemplate" or "AttributeModifier" or "CampaignEnumerator" or "CampaignTemplate" or "MissionTemplate" or "MpGameRules" or "Credits" or "TutorialMapData" or "MapMetaData" or "CameraSettings" or "Weather" or "Video" or "VideoEventList" or "PathMusicEvent" or "PathMusicGameDynamicState" or "GameMap" or "GameScriptList" or "RadarTerrainImage" or "TerrainTextureAtlas" or "UnitTypeIcon" or "ExperienceLevelTemplate" or "LocomotorTemplate" or "FXParticleSystemTemplate" or "FXList" or "Texture" or "PackedTextureImage" or "W3DContainer" or "W3DHierarchy" or "W3DCollisionBox" or "W3DMesh" or "W3DAnimation" or "Multisound" or "AudioFile" or "OnDemandTexture" or "OnDemandTextureImage" or "HealthBarTemplate" or "HealthBarSettings")) is XmlElement unsupported)throw SdkXmlDiagnostics.At(new InvalidDataException("Unsupported emitted asset type '"+unsupported.LocalName+"'; no silent skipping."),unsupported);
            // Reborn: source-only definition leaves must still validate, even if no emitted owner happens to consume them.
            if(owners.Length==0){if(document.ChildNodes.OfType<XmlElement>().Any(e=>e.LocalName=="Defines"))SdkMetadataDefinitions.Prove(schemas,xml,path,snapshots[path]);return;}
            bool needsPreparation=document.GetElementsByTagName("Define",Ea).Count!=0 || document.GetElementsByTagName("include","http://www.w3.org/2001/XInclude").Count!=0 || owners.Any(a=>a.HasAttribute("inheritFrom") || a.SelectNodes(".//@* | .//text()")!.Cast<XmlNode>().Any(n=>(n.Value??"").StartsWith('=')));
            if(needsPreparation)
            {
                var prepared=profile.Apply(path,snapshots[path]);if(prepared.Bytes==null)throw SdkXmlDiagnostics.Carry(new InvalidDataException(string.Join("; ",prepared.Evidence.Diagnostics)),prepared);
                // Reborn: only a successful real merge contributes inheritance use; unused instance declarations alone are not usage roots.
                inheritanceUses.UnionWith(profile.InheritanceUses);
                xml=Parse(prepared.Bytes);owners=xml.DocumentElement!.ChildNodes.OfType<XmlElement>().Where(e=>e.LocalName is not ("Tags" or "Includes" or "Defines")).ToArray();
                // Reborn: only this successful Core overlay authorizes consuming the diagnostic marker; no raw inherited document is flattened by substitution.
                foreach(var owner in owners)owner.RemoveAttribute("inheritFrom");
            }
            foreach(var owner in owners){if(owner.HasAttribute("inheritFrom"))throw new InvalidDataException("Unprocessed inheritance marker.");assets.Add(owner);origins.Add(owner,path);}
            }catch(Exception error)when(error is InvalidDataException or XmlException){new SdkXmlDiagnostics.SourceMap(snapshots.Select(s=>new SdkXmlDiagnostics.Snapshot(s.Key,s.Value)).Concat(fragments.Snapshots)).Enrich(error,path,xml);throw;}
        }
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: preserve literal Lua/text while preventing XML entity/network processing in captured source preparation. */
    //-------------------------------------------------------------------------------------------------
    private static XmlDocument Parse(byte[] bytes,string? path=null)=>SdkXmlDiagnostics.Parse(bytes,path);

    //-------------------------------------------------------------------------------------------------
    /** Reborn: admit explicit EA envelopes, empty Tags and all/instance imports; reject foreign metadata and ignored reference directives. */
    //-------------------------------------------------------------------------------------------------
    private static void ValidateEnvelope(XmlElement root)
    {
        if(root.LocalName!="AssetDeclaration" || root.NamespaceURI!=Ea)throw SdkXmlDiagnostics.At(new InvalidDataException("EA AssetDeclaration required."),root);
        var nodes=ConscriptSourceInput.Envelope(root).ToArray();
        // Reborn: identify the actual foreign node or second metadata declaration instead of blaming the generated envelope.
        var invalid=nodes.FirstOrDefault(n=>n.NamespaceURI!=Ea)??nodes.GroupBy(n=>n.LocalName).Where(g=>(g.Key is "Tags" or "Includes" or "Defines")&&g.Count()>1).SelectMany(g=>g.Skip(1)).FirstOrDefault();
        if(invalid!=null)throw SdkXmlDiagnostics.At(new InvalidDataException("Foreign or duplicate source metadata."),invalid);
        foreach(var node in nodes)
        {
            if(node.LocalName is not ("Tags" or "Includes" or "Defines"))continue;
            if(node.Attributes.OfType<XmlAttribute>().FirstOrDefault(a=>a.NamespaceURI!="http://www.w3.org/2000/xmlns/") is XmlAttribute badAttribute)throw SdkXmlDiagnostics.At(new InvalidDataException("Unsupported source metadata attribute."),badAttribute);
            if(node.LocalName=="Tags" && node.ChildNodes.Cast<XmlNode>().Any(n=>n is XmlElement || n is not XmlComment && !string.IsNullOrWhiteSpace(n.InnerText)))throw SdkXmlDiagnostics.At(new InvalidDataException("Only empty Tags are enabled."),node);
            if(node.LocalName!="Includes")continue;
            foreach(XmlNode child in node.ChildNodes)
            {
                if(child is XmlComment || child is not XmlElement && string.IsNullOrWhiteSpace(child.InnerText))continue;
                if(child is not XmlElement include || include.LocalName!="Include" || include.NamespaceURI!=Ea || (include.GetAttribute("type") is not ("all" or "instance") && !(include.GetAttribute("type")=="reference"&&SdkStockStreamReference.Known(include.GetAttribute("source")))) || include.Attributes.OfType<XmlAttribute>().Any(a=>a.NamespaceURI!="http://www.w3.org/2000/xmlns/" && (a.NamespaceURI.Length!=0 || a.Name is not ("type" or "source"))) || include.ChildNodes.Cast<XmlNode>().Any(n=>n is XmlElement || n is not XmlComment && !string.IsNullOrWhiteSpace(n.InnerText)))throw SdkXmlDiagnostics.At(new InvalidDataException("Only explicit all/instance Includes or standard stock reference Includes are enabled."),child);
            }
        }
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: own the detached asset namespace without copying metadata or introducing new declarations. */
    //-------------------------------------------------------------------------------------------------
    private static XmlDocument Detach(XmlElement asset){var xml=new XmlDocument{PreserveWhitespace=true,XmlResolver=null};xml.AppendChild(xml.ImportNode(asset,true));if(xml.DocumentElement!.Prefix.Length==0 && !xml.DocumentElement.HasAttribute("xmlns"))xml.DocumentElement.SetAttribute("xmlns",Ea);return xml;}

    //-------------------------------------------------------------------------------------------------
    /** Reborn: raw source witnesses remain independent of processed owner hashes and native stream identity. */
    //-------------------------------------------------------------------------------------------------
    private static string Hash(byte[] bytes)=>Convert.ToHexString(SHA256.HashData(bytes));
}
