using System.Buffers.Binary;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Xml;
using System.Xml.Schema;
using BinaryAssetBuilder.Core;
using BinaryAssetBuilder.Core.Hashing;
// Reborn: share the exact official Core schema helpers with the bounded recruitment command compiler.
using BinaryAssetBuilder.Core.SageXml;
using BinaryAssetBuilder.XmlCompiler;
using Chunk=Relo.Chunk;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: qualify schema/native root capabilities, existing nested values and a diagnostic new identity on the complete Conscript graph, then prepare unlaunched trial candidates.
internal static class GameObjectConscriptTrialSmokeTest
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: recompile admitted source edits through actual normalization/allocation and prove all remaining graph ownership against the stock oracle. */
    //-------------------------------------------------------------------------------------------------
    internal static void Run(XmlDocument baseline,Chunk native,InstanceDeclaration original,Chunk tokens,SchemaSet sourceSchemas,ManifestAsset[] catalog,string manifest,string globalManifest,Func<XmlDocument,(Chunk Native,InstanceDeclaration Instance)> build,Action<Chunk,InstanceDeclaration> verifyOriginal,string? alphaSource=null,string? alphaOutput=null)
    {
        // Reborn: qualify the stock-matching construction-command marshaler before exposing any new-ID recruitment package.
        var schemas=sourceSchemas.Schemas;ConscriptRecruitmentSmokeTest.Qualify(sourceSchemas,manifest);
        if(Word(native.InstanceBuffer,140)!=0x40800000u)throw new InvalidDataException("Conscript original BuildTime anchor is not 4.");
        uint newId=InstanceHandle.GetInstanceId("RebornConscriptTrial");if(catalog.Any(a=>a.TypeId==0x942FFF2Du && a.InstanceId==newId))throw new InvalidDataException("Trial identity collides with a real catalog asset.");
        // Reborn: selected XML is safely parsed and compared to the complete captured source; only the original identity and six scalars enter Studio.
        XmlDocument? selected=null;XmlDocument? authoredCommand=null;
        if(alphaSource!=null)
        {
            if(alphaOutput==null || Directory.Exists(alphaOutput) || File.Exists(alphaOutput))throw new InvalidDataException("Fresh Conscript alpha output required.");
            var settings=new XmlReaderSettings{DtdProcessing=DtdProcessing.Prohibit,XmlResolver=null,MaxCharactersInDocument=1048576};
            using var reader=XmlReader.Create(alphaSource,settings);var input=new XmlDocument{PreserveWhitespace=true,XmlResolver=null};input.Load(reader);
            selected=new XmlDocument{PreserveWhitespace=true,XmlResolver=null};selected.LoadXml(input.DocumentElement?.OuterXml??throw new InvalidDataException("Conscript root absent."));
            string id=selected.DocumentElement!.GetAttribute("id");if(!ConscriptSourceInput.Identity(id) || id!="SovietAntiInfantryInfantry" && catalog.Any(a=>a.TypeId==0x942FFF2D && a.InstanceId==InstanceHandle.GetInstanceId(id)))throw new InvalidDataException("Custom GameObject ID is invalid or collides with another stock GameObject.");
            // Reborn: only a command explicitly captured by source preparation is compiled; new IDs alone never synthesize one.
            string commandPath=Path.Combine(Path.GetDirectoryName(alphaSource)!,"command.xml");if(File.Exists(commandPath)){using var commandReader=XmlReader.Create(commandPath,settings);authoredCommand=new XmlDocument{PreserveWhitespace=true,XmlResolver=null};authoredCommand.Load(commandReader);}
        }
        // Reborn: all unedited bytes retain their complete stock oracle; only six independently located words may differ.
        int[] scalarOffsets=ConscriptEditableScalars.Offsets(native.InstanceBuffer);
        // Reborn: all nested primitive descriptors must independently bind to the already stock-qualified module token/native coordinates before edits enter publication.
        int moduleScalarCount=ConscriptModuleScalarContract.Qualify(baseline,native.InstanceBuffer,tokens.InstanceBuffer,88824);
        string sourceSnapshot=baseline.OuterXml;var originalDependencies=original.ReferencedInstances.Select(h=>new AssetId(h.TypeId,h.InstanceId)).ToArray();string root=alphaOutput??Path.Combine(Path.GetTempPath(),"Reborn-ConscriptTrials-"+Guid.NewGuid().ToString("N"));Directory.CreateDirectory(root);int refusals=0;
        // Reborn: corruption controls reuse only exact-source expected tables within this immutable schema/catalog build; no cross-build or filename-only cache is trusted.
        var referenceExpectations=new Dictionary<string,AssetId[]>(StringComparer.Ordinal);
        // Reborn: retain the exact schema/native module field inventory beside this build's qualification evidence.
        File.WriteAllText(Path.Combine(root,"module-scalars.json"),JsonSerializer.Serialize(ConscriptModuleScalarContract.Describe(baseline),new JsonSerializerOptions{WriteIndented=true}));
        foreach(string id in selected==null?new[]{"SovietAntiInfantryInfantry","RebornConscriptTrial"}:new[]{selected.DocumentElement!.GetAttribute("id")})
        {
            var source=selected??(XmlDocument)baseline.CloneNode(true);if(selected==null){source.DocumentElement!.SetAttribute("id",id);source.DocumentElement.SetAttribute("BuildTime","1");}Admit(source);var pair=build(source);Verify(pair.Native,pair.Instance,id,source);var repeated=build(source);Verify(repeated.Native,repeated.Instance,id,source);
            var projected=Ep1GameObjectEmptyRootProjection.ComposeOriginal(pair.Native,schemas);var expected=Expected(source);if(!Equal(projected,expected) || !Equal(projected,Ep1GameObjectEmptyRootProjection.ComposeOriginal(repeated.Native,schemas)))throw new InvalidDataException("Authored Conscript metadata/native edit differs.");
            var bad=Clone(pair.Native);bad.InstanceBuffer[192]^=1;Refuse(()=>Verify(bad,pair.Instance,id,source));refusals++;
            foreach(int offset in ConscriptEditableScalars.Offsets(pair.Native.InstanceBuffer)){bad=Clone(pair.Native);bad.InstanceBuffer[offset]^=1;Refuse(()=>Verify(bad,pair.Instance,id,source));refusals++;}
            var streams=GameObjectOriginalStreamSmokeTest.Run(projected,pair.Instance,manifest,globalManifest,true);
            if(authoredCommand!=null)streams=ConscriptRecruitmentSmokeTest.Compose(streams,pair.Instance,sourceSchemas,manifest,globalManifest,authoredCommand);
            Package(Path.Combine(root,id!="SovietAntiInfantryInfantry"?"new-id":"override"),source,streams,id);
        }
        foreach(var change in new[]{("id","Invalid Conscript"),("BuildTime","NaN"),("BuildTime","not-a-real"),("UnknownRootField","2")}){var bad=(XmlDocument)baseline.CloneNode(true);bad.DocumentElement!.SetAttribute("BuildTime","1");bad.DocumentElement.SetAttribute(change.Item1,change.Item2);Refuse(()=>Admit(bad));refusals++;}
        // Reborn: independently compile each authored word and the combined profile; compare the entire projected graph, not only six floats.
        foreach(int selectedField in Enumerable.Range(0,6).Concat(new[]{-1,-2,-3}))
        {
            var probe=(XmlDocument)baseline.CloneNode(true);float[] values={2,25,500,350,650,90};
            for(int i=0;i<6;i++)if(selectedField<0 || selectedField==i){var field=ConscriptEditableScalars.Fields[i];float value=selectedField==-2?field.Min:selectedField==-3?field.Max:values[i];ConscriptEditableScalars.Element(probe,field).SetAttribute(field.Attribute,value.ToString(System.Globalization.CultureInfo.InvariantCulture));}
            Admit(probe);var pair=build(probe);Verify(pair.Native,pair.Instance,"SovietAntiInfantryInfantry",probe);
            if(!Equal(Expected(probe),Ep1GameObjectEmptyRootProjection.ComposeOriginal(pair.Native,schemas)))throw new InvalidDataException("Conscript scalar matrix metadata differs.");
        }
        foreach(var field in ConscriptEditableScalars.Fields)foreach(string invalid in new[]{"NaN","Infinity","-Infinity","not-a-number"}){var bad=(XmlDocument)baseline.CloneNode(true);ConscriptEditableScalars.Element(bad,field).SetAttribute(field.Attribute,invalid);Refuse(()=>Admit(bad));refusals++;}
        // Reborn: all existing primitive processor descriptors enter one source/native/metadata test; schema rules replace the former six-field business whitelist.
        var rootProbe=(XmlDocument)baseline.CloneNode(true);GameObjectScalarContract.AuthorProbe(rootProbe);GameObjectRootAttributeContract.AuthorProbe(rootProbe);GameObjectStrongReferenceContract.AuthorProbe(rootProbe,baseline,native.InstanceBuffer,originalDependencies,catalog);GameObjectRootStringContract.AuthorProbe(rootProbe,baseline);ConscriptModuleScalarContract.AuthorProbe(rootProbe);ConscriptModuleIdentityContract.AuthorProbe(rootProbe);Admit(rootProbe);var scalarPair=build(rootProbe);Verify(scalarPair.Native,scalarPair.Instance,"SovietAntiInfantryInfantry",rootProbe);
        var rootExpected=Expected(rootProbe);var rootProjected=Ep1GameObjectEmptyRootProjection.ComposeOriginal(scalarPair.Native,schemas);if(!Equal(rootExpected,rootProjected))throw new InvalidDataException("Grouped root primitive metadata differs: "+FirstDifference(rootProjected,rootExpected));
        // Reborn: the grouped growth/presence probe must also survive complete manifest/BIN/RELO/IMP serialization, not only in-memory projection.
        if(selected==null)_=GameObjectOriginalStreamSmokeTest.Run(rootProjected,scalarPair.Instance,manifest,globalManifest,true);
        // Reborn: each module primitive independently rejects a native payload mutation while the grouped fixture also changes root allocation and dependencies.
        foreach(var owner in ConscriptModuleScalarContract.Owners(rootProbe))foreach(var binding in ConscriptModuleScalarContract.Bindings(owner.NativeType)){var corrupted=Clone(scalarPair.Native);int at=ConscriptModuleScalarContract.Address(corrupted.InstanceBuffer,0,owner)+binding.Offset;corrupted.InstanceBuffer[at]^=1;Refuse(()=>Verify(corrupted,scalarPair.Instance,"SovietAntiInfantryInfantry",rootProbe));refusals++;}
        // Reborn: renamed local identities reject each corrupted header and duplicate/empty source names rather than falling back to captured tags.
        foreach(var owner in ConscriptModuleScalarContract.Owners(rootProbe)){var corrupted=Clone(scalarPair.Native);corrupted.InstanceBuffer[ConscriptModuleScalarContract.Address(corrupted.InstanceBuffer,0,owner)+4]^=1;Refuse(()=>Verify(corrupted,scalarPair.Instance,"SovietAntiInfantryInfantry",rootProbe));refusals++;}
        foreach(bool empty in new[]{false,true}){var bad=(XmlDocument)rootProbe.CloneNode(true);var owners=ConscriptModuleScalarContract.Owners(bad);owners[1].Element.SetAttribute("id",empty?string.Empty:owners[0].Element.GetAttribute("id").ToUpperInvariant());Refuse(()=>Admit(bad));refusals++;}
        // Reborn: every newly described enum/flag field rejects a malformed authored name before native allocation.
        foreach(var owner in ConscriptModuleScalarContract.Owners(rootProbe))foreach(var binding in ConscriptModuleScalarContract.Bindings(owner.NativeType).Where(b=>b.Codec.StartsWith("Enum:",StringComparison.Ordinal)||b.Codec.StartsWith("Flags:",StringComparison.Ordinal))){var bad=(XmlDocument)rootProbe.CloneNode(true);ConscriptModuleScalarContract.Owners(bad).Single(o=>o.Container==owner.Container&&o.Index==owner.Index).Element.SetAttribute(binding.Name,"RebornInvalidEnumFlag");Refuse(()=>Admit(bad));refusals++;}
        Console.WriteLine($"Conscript module value capabilities: PASS ({moduleScalarCount} schema/native/token-bound primitive/enum/flag attributes across {ConscriptModuleScalarContract.Owners(baseline).Length} existing modules; all local IDs renamed; grouped authored/native/metadata/stream and per-field/header corruption controls; invalid enum/flag and duplicate-ID refusal).");
        // Reborn: optional module primitives exercise official default omission without inheriting the former explicit-attribute trial limitation.
        var omitted=(XmlDocument)baseline.CloneNode(true);var body=(XmlElement)omitted.GetElementsByTagName("ActiveBody","uri:ea.com:eala:asset")[0]!;body.RemoveAttribute("MaxHealth");var ai=(XmlElement)omitted.GetElementsByTagName("AIUpdate","uri:ea.com:eala:asset")[0]!;ai.RemoveAttribute("StandGround");Admit(omitted);var omittedPair=build(omitted);Verify(omittedPair.Native,omittedPair.Instance,"SovietAntiInfantryInfantry",omitted);if(!Equal(Expected(omitted),Ep1GameObjectEmptyRootProjection.ComposeOriginal(omittedPair.Native,schemas)))throw new InvalidDataException("Module scalar omitted/default metadata differs.");
        // Reborn: official unsigned hex and numeric boolean spellings must survive the same complete module/native/metadata route as decimal authored sentinels.
        ai.SetAttribute("MaxCombineChildren","0xFFFFFFFF");ai.SetAttribute("StandGround","1");body.SetAttribute("RemoveUpgradesOnDeath","0");Admit(omitted);var spellingPair=build(omitted);Verify(spellingPair.Native,spellingPair.Instance,"SovietAntiInfantryInfantry",omitted);if(!Equal(Expected(omitted),Ep1GameObjectEmptyRootProjection.ComposeOriginal(spellingPair.Native,schemas)))throw new InvalidDataException("Module scalar hexadecimal/boolean metadata differs.");
        // Reborn: removing all six allocations proves negative rebase and reinsertion into the baseline auxiliary order, including a zero-current-string normalization path.
        var noStrings=(XmlDocument)baseline.CloneNode(true);foreach(var field in GameObjectRootStringContract.Fields)noStrings.DocumentElement!.RemoveAttribute(field.Name);Admit(noStrings);var noStringPair=build(noStrings);Verify(noStringPair.Native,noStringPair.Instance,"SovietAntiInfantryInfantry",noStrings);var noStringExpected=Expected(noStrings);var noStringProjected=Ep1GameObjectEmptyRootProjection.ComposeOriginal(noStringPair.Native,schemas);if(!Equal(noStringExpected,noStringProjected))throw new InvalidDataException("Removed root string metadata differs: "+FirstDifference(noStringProjected,noStringExpected));
        foreach(var binding in GameObjectScalarContract.Bindings){var corrupted=Clone(scalarPair.Native);corrupted.InstanceBuffer[binding.Native.Offset]^=1;Refuse(()=>Verify(corrupted,scalarPair.Instance,"SovietAntiInfantryInfantry",rootProbe));refusals++;}
        foreach(var binding in GameObjectRootAttributeContract.Bindings){var corrupted=Clone(scalarPair.Native);corrupted.InstanceBuffer[binding.Native.Offset]^=1;Refuse(()=>Verify(corrupted,scalarPair.Instance,"SovietAntiInfantryInfantry",rootProbe));refusals++;}
        // Reborn: every populated dynamic string rejects payload corruption without pinning valid text to an arbitrary example length.
        foreach(var field in GameObjectRootStringContract.Fields){uint pointer=Word(scalarPair.Native.InstanceBuffer,field.Offset+4);if(pointer==0)continue;var corrupted=Clone(scalarPair.Native);corrupted.InstanceBuffer[pointer]^=1;Refuse(()=>Verify(corrupted,scalarPair.Instance,"SovietAntiInfantryInfantry",rootProbe));refusals++;}
        // Reborn: pass the former 4096-character quota through actual native/metadata rebasing, not merely source admission.
        var longString=(XmlDocument)baseline.CloneNode(true);longString.DocumentElement!.SetAttribute("Description",new string('X',4097));Admit(longString);var longPair=build(longString);Verify(longPair.Native,longPair.Instance,"SovietAntiInfantryInfantry",longString);if(!Equal(Expected(longString),Ep1GameObjectEmptyRootProjection.ComposeOriginal(longPair.Native,schemas)))throw new InvalidDataException("Long root text native/metadata differs.");
        // Reborn: all four real icon/intro presence combinations recompile the complete graph; missing CrowdResponse still fails actual catalog resolution.
        for(int mask=0;mask<4;mask++)
        {
            var presence=(XmlDocument)baseline.CloneNode(true);if((mask&1)==0)presence.DocumentElement!.RemoveAttribute("UnitTypeIcon");if((mask&2)==0)presence.DocumentElement!.SetAttribute("UnitIntro",string.Empty);GameObjectRootStringContract.AuthorProbe(presence,baseline);Admit(presence);var changed=build(presence);Verify(changed.Native,changed.Instance,"SovietAntiInfantryInfantry",presence);var projected=Ep1GameObjectEmptyRootProjection.ComposeOriginal(changed.Native,schemas);var expected=Expected(presence);if(!Equal(projected,expected))throw new InvalidDataException("Root reference presence metadata differs: "+FirstDifference(projected,expected));
            if(selected==null && mask==0)_=GameObjectOriginalStreamSmokeTest.Run(projected,changed.Instance,manifest,globalManifest,true);
            var corrupted=Clone(changed.Native);corrupted.ImportsBuffer=corrupted.ImportsBuffer[..^4];Refuse(()=>Verify(corrupted,changed.Instance,"SovietAntiInfantryInfantry",presence));refusals++;
            // Reborn: valid framing cannot hide a wrong downstream selector or a nonzero selector in an absent root import slot.
            corrupted=Clone(changed.Native);int lastSlot=(int)Word(corrupted.ImportsBuffer,corrupted.ImportsBuffer.Length-8);Put(corrupted.InstanceBuffer,lastSlot,Word(corrupted.InstanceBuffer,lastSlot)+1);Refuse(()=>Verify(corrupted,changed.Instance,"SovietAntiInfantryInfantry",presence));refusals++;
            corrupted=Clone(changed.Native);Put(corrupted.InstanceBuffer,368,Word(corrupted.InstanceBuffer,368)+1);Refuse(()=>Verify(corrupted,changed.Instance,"SovietAntiInfantryInfantry",presence));refusals++;
        }
        // Reborn: retarget both existing weapon slots together; prove changed/repeated EP1 target identities through native, metadata and stream emission.
        var weapons=(XmlDocument)baseline.CloneNode(true);var weaponNodes=weapons.GetElementsByTagName("Weapon",SchemaSet.XmlNamespace).OfType<XmlElement>().Where(e=>e.ParentNode?.LocalName=="WeaponSlotHardpoint").ToArray();
        if(weaponNodes.Length!=2)throw new InvalidDataException("Conscript weapon slot fixture differs.");
        weaponNodes[0].SetAttribute("Template","JapanArcherInfantryEnergyBow");weaponNodes[1].SetAttribute("Template","JapanArcherInfantryEnergyBow");Admit(weapons);var weaponPair=build(weapons);Verify(weaponPair.Native,weaponPair.Instance,"SovietAntiInfantryInfantry",weapons);
        var weaponProjected=Ep1GameObjectEmptyRootProjection.ComposeOriginal(weaponPair.Native,schemas);if(!Equal(weaponProjected,Expected(weapons)))throw new InvalidDataException("Retargeted weapon metadata differs.");
        GameObjectOriginalStreamSmokeTest.Run(weaponProjected,weaponPair.Instance,manifest,globalManifest,true);
        // Reborn: unchanged native selectors cannot hide a forged dependency table or a missing/wrong-type/authored-index target.
        Refuse(()=>Verify(weaponPair.Native,original,"SovietAntiInfantryInfantry",weapons));refusals++;
        foreach(string invalid in new[]{"RebornMissingWeapon","GameObject:SovietAntiInfantryInfantry","WeaponTemplate:RebornMissingWeapon","JapanArcherInfantryEnergyBow\\1"}){var bad=(XmlDocument)weapons.CloneNode(true);((XmlElement)bad.GetElementsByTagName("Weapon",SchemaSet.XmlNamespace)[0]!).SetAttribute("Template",invalid);Refuse(()=>{Admit(bad);var pair=build(bad);Verify(pair.Native,pair.Instance,"SovietAntiInfantryInfantry",bad);});refusals++;}
        Console.WriteLine("Conscript typed reference retargeting: PASS (both weapon slots, repeated real EP1 target, complete native/metadata/stream proof; missing/wrong-type/index/forged-table rejection; no game).");
        var addedStrong=(XmlDocument)baseline.CloneNode(true);addedStrong.DocumentElement!.SetAttribute("CrowdResponse","RebornCrowdResponseProbe");Admit(addedStrong);Refuse(()=>build(addedStrong));refusals++;
        // Reborn: zero/default BuildTime and values beyond historical alpha caps are schema-representable inputs, not automatic authoring errors.
        var uncapped=(XmlDocument)baseline.CloneNode(true);uncapped.DocumentElement!.SetAttribute("BuildTime","0");uncapped.DocumentElement.SetAttribute("Scale","2");ConscriptEditableScalars.Element(uncapped,ConscriptEditableScalars.Fields[3]).SetAttribute("VisionRange","6000");Admit(uncapped);var uncappedPair=build(uncapped);Verify(uncappedPair.Native,uncappedPair.Instance,"SovietAntiInfantryInfantry",uncapped);
        uncapped.DocumentElement.RemoveAttribute("BuildTime");uncapped.DocumentElement.SetAttribute("IsTrainable","1");Admit(uncapped);var defaultPair=build(uncapped);Verify(defaultPair.Native,defaultPair.Instance,"SovietAntiInfantryInfantry",uncapped);
        Console.WriteLine("GameObject root capability: PASS ("+GameObjectScalarContract.Bindings.Count+" primitives + "+GameObjectRootAttributeContract.Bindings.Count+" fixed enum/flag/weak descriptors + optional root strong references/four icon-intro presence combinations + six bounded dynamic root strings; grouped native/metadata/stream/dependency checks; module topology retained).");
        var added=(XmlDocument)baseline.CloneNode(true);added.DocumentElement!.SetAttribute("BuildTime","1");added.DocumentElement.AppendChild(added.CreateElement("UnreviewedChild",baseline.DocumentElement!.NamespaceURI));Refuse(()=>Admit(added));refusals++;
        if(baseline.OuterXml!=sourceSnapshot)throw new InvalidDataException("Authored trial mutated original source.");Console.WriteLine($"Conscript authored trials: PASS (source-compiled identities={(selected==null?2:1)}, schema/ABI root primitives and existing nested numeric owners; grouped source/native/metadata controls, 49 modules/six owners/geometry retained, 273 non-root references plus authored root references, {refusals} source/native refusals; candidates={root}; offline recruitment candidate only; no install, launch or gameplay proof).");
        //-------------------------------------------------------------------------------------------------
        /** Reborn: only explicitly contracted root/nested capabilities and two diagnostic identities may differ; unknown changes never compile silently. */
        //-------------------------------------------------------------------------------------------------
        void Admit(XmlDocument input)=>ConscriptSourceInput.Admit(input,(XmlDocument)baseline.CloneNode(true),schemas);
        //-------------------------------------------------------------------------------------------------
        /** Reborn: restore every independently anchored authored value before reusing original native ownership; every other byte and auxiliary order stays unchanged. */
        //-------------------------------------------------------------------------------------------------
        void Verify(Chunk value,InstanceDeclaration instance,string id,XmlDocument source){string sourceKey=source.OuterXml;if(!referenceExpectations.TryGetValue(sourceKey,out var expectedDependencies)){expectedDependencies=ConscriptReferenceTargets.Expected(source,baseline,sourceSchemas,catalog,originalDependencies);referenceExpectations.Add(sourceKey,expectedDependencies);}if(instance.Handle.Name!="GameObject:"+id || instance.Handle.InstanceId!=InstanceHandle.GetInstanceId(id) || !instance.ReferencedInstances.Select(h=>new AssetId(h.TypeId,h.InstanceId)).SequenceEqual(expectedDependencies))throw new InvalidDataException("Authored identity/reference differs.");var restored=Clone(value);GameObjectScalarContract.VerifyAndRestore(source,value.InstanceBuffer,restored.InstanceBuffer,native.InstanceBuffer);GameObjectRootAttributeContract.VerifyAndRestore(source,value.InstanceBuffer,restored.InstanceBuffer,native.InstanceBuffer);ConscriptModuleScalarContract.VerifyAndRestore(source,value.InstanceBuffer,restored.InstanceBuffer,native.InstanceBuffer);ConscriptModuleIdentityContract.VerifyAndRestore(source,value.InstanceBuffer,restored.InstanceBuffer,native.InstanceBuffer);int[] actualOffsets=ConscriptEditableScalars.Offsets(value.InstanceBuffer);for(int i=1;i<actualOffsets.Length;i++)if(Word(value.InstanceBuffer,actualOffsets[i])!=ConscriptEditableScalars.Bits(source,ConscriptEditableScalars.Fields[i]))throw new InvalidDataException("Conscript scalar native value differs: "+ConscriptEditableScalars.Fields[i].Attribute);restored=GameObjectRootStringContract.Normalize(source,baseline,restored,native);GameObjectStrongReferenceContract.Restore(source,baseline,restored,native);for(int i=1;i<scalarOffsets.Length;i++)Put(restored.InstanceBuffer,scalarOffsets[i],Word(native.InstanceBuffer,scalarOffsets[i]));if(!Equal(restored,native))throw new InvalidDataException("Unreviewed authored native delta: "+FirstDifference(restored,native));verifyOriginal(restored,original);}
        //-------------------------------------------------------------------------------------------------
        /** Reborn: expected metadata keeps the independently captured stock graph except exact contracted values and dynamically rebased root-string storage. */
        //-------------------------------------------------------------------------------------------------
        Chunk Expected(XmlDocument source){var expected=GameObjectRootStringContract.Expected(baseline,source,tokens,88824);GameObjectStrongReferenceContract.Patch(source,baseline,expected,88824);GameObjectScalarContract.Patch(source,expected.InstanceBuffer,88824);GameObjectRootAttributeContract.Patch(source,expected.InstanceBuffer,88824);ConscriptModuleScalarContract.Patch(source,expected.InstanceBuffer,88824);ConscriptModuleIdentityContract.Patch(source,expected.InstanceBuffer,88824);int[] expectedOffsets=ConscriptEditableScalars.Offsets(expected.InstanceBuffer.AsSpan(88824).ToArray(),88824);for(int i=1;i<expectedOffsets.Length;i++)Put(expected.InstanceBuffer,88824+expectedOffsets[i],ConscriptEditableScalars.Bits(source,ConscriptEditableScalars.Fields[i]));return expected;}
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: reuse only low-level BIG packing, not armor admission; independently recover every GameObject stream variant and CRLF marker before exposing a fresh candidate. */
    //-------------------------------------------------------------------------------------------------
    internal static void Package(string directory,XmlDocument source,Dictionary<string,byte[]> streams,string id,int metadataPrefix=88824)
    {
        // Reborn: the receipt must report the actual serialized root origin, not an unchecked caller-supplied constant.
        // Reborn: mixed assets are dependency-sorted, so the primary GameObject need not be the first native stream entry.
        var metadata=ManifestReader.Read(streams["diagnostic.manifest"]);var primary=metadata.Assets.Single(a=>a.Name=="GameObject:"+id);int primaryOffset=8+metadata.Assets.TakeWhile(a=>a!=primary).Sum(a=>a.InstanceDataSize);
        var rootRows=GameObjectStockReview.Tokens(streams["diagnostic.bin"].AsSpan(primaryOffset,primary.InstanceDataSize).ToArray(),0);
        int buildTimeSlot=Ep1GameObjectRootValueTokens.AllFields.Single(f=>f.Name=="BuildTime").Offset;
        if(rootRows[FastHash.GetHashCode("BuildTime")].Offset-buildTimeSlot!=metadataPrefix)throw new InvalidDataException("Package metadata-prefix receipt differs from serialized data.");
        string[] extensions={".manifest",".bin",".relo",".imp"};byte[][] payloads=extensions.Select(e=>streams["diagnostic"+e]).ToArray();byte[] archive=ArmorDiagnosticPackage.Serialize(payloads,true,true);const string config="add-big RebornConscriptProbe.big\n";var files=new Dictionary<string,byte[]>{["RebornConscriptProbe.big"]=archive,["reborn_1.0.cfg"]=Encoding.ASCII.GetBytes(config),["reborn_1.0.skudef"]=Encoding.ASCII.GetBytes(config),["trial-source.xml"]=Encoding.UTF8.GetBytes(source.OuterXml),["DO_NOT_LAUNCH_YET.txt"]=Encoding.UTF8.GetBytes("Reborn: unlaunched Conscript candidate only. Mod SKU fragment, not stock SKU replacement. New-ID trial redirects the existing Conscript UNIT_BUILD command; all other Barracks choices/CommandSet remain stock. Separate runtime preflight and game-test authorization required.\n")};
        ModifierFXStreamSmokeTest.WriteNew(directory,files);Verify();Refuse(()=>ModifierFXStreamSmokeTest.WriteNew(directory,files));int corruptions=0;
        foreach(string name in files.Keys){byte[] bad=files[name].ToArray();bad[0]^=1;string path=Path.Combine(directory,name);try{File.WriteAllBytes(path,bad);Refuse(Verify);corruptions++;}finally{File.WriteAllBytes(path,files[name]);}}Verify();
        // Reborn: receipts report the independently read emitted reference table, including authored root presence changes.
        // Reborn: variable-graph callers pass their independently qualified metadata extent; existing fixed trial callers retain their default.
        var receipt=new{Asset="GameObject:"+id,BuildTime=ConscriptEditableScalars.Read(source,ConscriptEditableScalars.Fields[0]),EditableValues=ConscriptEditableScalars.Fields.ToDictionary(f=>f.Attribute,f=>ConscriptEditableScalars.Read(source,f)),StreamProfile="conscript-explicit-source",AssetCount=metadata.Assets.Count,AutomaticCommandGenerated=false,CommandOverride=metadata.Assets.Skip(1).Select(a=>a.Name).FirstOrDefault(),MetadataPrefix=metadataPrefix,ReferenceOccurrences=metadata.Assets[0].References.Count,ArchiveEntryCount=15,ArchiveSha256=Convert.ToHexString(SHA256.HashData(archive)),SourceSha256=Convert.ToHexString(SHA256.HashData(files["trial-source.xml"])),OfflineValidated=true,PackageCorruptionsRejected=corruptions,NewIdentity=id!="SovietAntiInfantryInfantry",RecruitableNewIdentity=id!="SovietAntiInfantryInfantry" && metadata.Assets.Any(a=>a.Name=="LogicCommand:"+ConscriptRecruitmentSmokeTest.CommandId),GameExecuted=false,InstalledFilesModified=false,RuntimeReady=false,InstanceHashPolicy="experimental-normalized-source",ConfigPath=Path.Combine(directory,"reborn_1.0.cfg"),NextGate="Controlled runtime preflight and override gameplay test; new-ID trial reuses the existing Conscript production slot; verify actual newly created identity in game."};File.WriteAllText(Path.Combine(directory,"trial-plan.json"),JsonSerializer.Serialize(receipt,new JsonSerializerOptions{WriteIndented=true}));
        //-------------------------------------------------------------------------------------------------
        /** Reborn: verify exact prepared config/source/notice bytes, independent BIG header/index geometry and all fifteen recovered variant payloads. */
        //-------------------------------------------------------------------------------------------------
        void Verify(){foreach(var file in files)if(!File.ReadAllBytes(Path.Combine(directory,file.Key)).SequenceEqual(file.Value))throw new InvalidDataException("Conscript trial prepared file differs.");string path=Path.Combine(directory,"RebornConscriptProbe.big");byte[] stored=File.ReadAllBytes(path);if(Encoding.ASCII.GetString(stored,0,4)!="BIG4" || Word(stored,4)!=stored.Length || BinaryPrimitives.ReadUInt32BigEndian(stored.AsSpan(8,4))!=15)throw new InvalidDataException("Conscript trial BIG header differs.");using var big=BigArchive.Open(path,15,4096);long at=BinaryPrimitives.ReadUInt32BigEndian(stored.AsSpan(12,4));int index=0;foreach(string variant in new[]{"","_l","_m"})foreach(string extension in extensions.Append(".version")){var entry=big.Entries[index++];byte[] expected=extension==".version"?Encoding.ASCII.GetBytes("\r\n"):payloads[Array.IndexOf(extensions,extension)];if(entry.Name!="data\\mod"+variant+extension || entry.Offset!=at || entry.StoredSize!=expected.Length || !big.ReadEntry(entry,1048576).SequenceEqual(expected))throw new InvalidDataException("Conscript trial variant differs.");at+=entry.StoredSize;}if(at!=stored.Length)throw new InvalidDataException("Conscript trial unowned archive suffix.");}
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: explicit refusal controls never count a successful corrupt candidate as rejection. */
    //-------------------------------------------------------------------------------------------------
    private static void Refuse(Action action){try{action();}catch(InvalidDataException){return;}
        // Reborn: covered weapon references now reach Core's real type checker instead of being rejected by stock-shape admission first.
        catch(BinaryAssetBuilderException error)when(error.ErrorCode==ErrorCode.ReferencingError){return;}throw new InvalidDataException("Conscript trial corruption accepted.");}
    //-------------------------------------------------------------------------------------------------
    /** Reborn: complete comparisons include native and auxiliary bytes/order. */
    //-------------------------------------------------------------------------------------------------
    private static bool Equal(Chunk a,Chunk b)=>a.InstanceBuffer.SequenceEqual(b.InstanceBuffer) && a.RelocationBuffer.SequenceEqual(b.RelocationBuffer) && a.ImportsBuffer.SequenceEqual(b.ImportsBuffer);
    //-------------------------------------------------------------------------------------------------
    /** Reborn: report the first exact native/RELO/IMP mismatch so a failed ownership proof identifies its coordinate. */
    //-------------------------------------------------------------------------------------------------
    private static string FirstDifference(Chunk actual,Chunk expected)
    {
        foreach(var part in new[]{("BIN",actual.InstanceBuffer,expected.InstanceBuffer),("RELO",actual.RelocationBuffer,expected.RelocationBuffer),("IMP",actual.ImportsBuffer,expected.ImportsBuffer)})
        {
            int shared=Math.Min(part.Item2.Length,part.Item3.Length);for(int i=0;i<shared;i++)if(part.Item2[i]!=part.Item3[i]){int word=i&~3;string detail=word<=shared-4?$"; words={Word(part.Item2,word):X8}/{Word(part.Item3,word):X8}":string.Empty;return $"{part.Item1}[{i}]={part.Item2[i]:X2}, expected {part.Item3[i]:X2}{detail}; lengths={part.Item2.Length}/{part.Item3.Length}";}if(part.Item2.Length!=part.Item3.Length)return $"{part.Item1} lengths={part.Item2.Length}/{part.Item3.Length}";
        }
        return "comparison predicate differed without a byte mismatch";
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: restoration and corruption controls own their buffers. */
    //-------------------------------------------------------------------------------------------------
    private static Chunk Clone(Chunk c)=>new(){InstanceBuffer=c.InstanceBuffer.ToArray(),RelocationBuffer=c.RelocationBuffer.ToArray(),ImportsBuffer=c.ImportsBuffer.ToArray()};
    //-------------------------------------------------------------------------------------------------
    /** Reborn: bounded scalar inspection cannot escape the qualified graph. */
    //-------------------------------------------------------------------------------------------------
    private static uint Word(byte[] b,int at){if(at<0 || at>b.Length-4)throw new InvalidDataException("Conscript trial word bounds.");return BinaryPrimitives.ReadUInt32LittleEndian(b.AsSpan(at,4));}
    //-------------------------------------------------------------------------------------------------
    /** Reborn: restore or author only a specifically reviewed word in an owned copy. */
    //-------------------------------------------------------------------------------------------------
    private static void Put(byte[] b,int at,uint value)=>BinaryPrimitives.WriteUInt32LittleEndian(b.AsSpan(at,4),value);
}
