using System.Buffers.Binary;
using System.Security.Cryptography;
using System.Text;
using System.Xml;
using BinaryAssetBuilder.Core;
using BinaryAssetBuilder.Core.Hashing;
using BinaryAssetBuilder.Core.SageXml;
using BinaryAssetBuilder.XmlCompiler;
using SageBinaryData;
using Chunk = Relo.Chunk;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: original complete client leaves join the selected root and complete Draws graph without claiming closure of the original Conscript behavior/root owners.
internal static class GameObjectClientsCompositionSmokeTest
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: test absent/empty/full client combinations, sibling ownership, shared strong occurrences and independently stock-bound client metadata in one root allocation. */
    //-------------------------------------------------------------------------------------------------
    internal static void Run(SchemaSet schemas,string sourceRoot,string manifest,ManifestAsset[] catalog,XmlElement[] draws,byte[] expectedDraws)
    {
        var stock=GameObjectStockReview.Capture(manifest,"GameObject:SovietAntiInfantryInfantry");var topology=GameObjectModuleReview.Inspect(manifest,stock.Asset.Name);
        var files=new[]{Path.Combine(sourceRoot,"BaseObjects","BaseObject.xml"),Path.Combine(sourceRoot,"BaseObjects","BaseInfantry.xml"),Path.Combine(sourceRoot,"Soviet","Units","SovietAntiInfantryInfantry.xml")};var captures=files.Select(f=>SdkEnvironmentPreflight.Read(f,1048576)).ToArray();var documents=captures.Select(b=>Xml(Encoding.UTF8.GetString(b).TrimStart('\uFEFF'))).ToArray();var objects=documents.Select(d=>d.GetElementsByTagName("GameObject",SchemaSet.XmlNamespace).OfType<XmlElement>().Single()).ToArray();
        if(objects[1].GetAttribute("inheritFrom")!="BaseObject" || objects[2].GetAttribute("inheritFrom")!="BaseInfantry" || objects[0].GetElementsByTagName("ClientBehaviors",SchemaSet.XmlNamespace).Count!=0)throw new InvalidDataException("Explicit selected client chain changed.");
        var clients=objects.Skip(1).SelectMany(o=>o.ChildNodes.OfType<XmlElement>().Where(e=>e.LocalName=="ClientBehaviors").SelectMany(c=>c.ChildNodes.OfType<XmlElement>())).ToArray();var updates=objects[2].GetElementsByTagName("ClientUpdates",SchemaSet.XmlNamespace).OfType<XmlElement>().Single().ChildNodes.OfType<XmlElement>().Single();if(clients.Length!=6 || updates.LocalName!="LargeGroupAudioClientUpdate")throw new InvalidDataException("Complete original client inventory required.");
        var targets=new[]{"UnitTypeIcon","UnitIntro","ArmorTemplate","DamageFX","FXList","AttributeModifier","WeaponTemplate","UpgradeTemplate","LocomotorTemplate"}.Select(t=>catalog.First(a=>a.TypeId==FastHash.GetHashCode(t))).ToArray();
        int cases=0,maxFields=0,maxImports=0;Chunk? full=null;InstanceDeclaration? fullInstance=null;string? fullSource=null;
        foreach(int updateCount in new[]{-1,0,1,2})foreach(int behaviorCount in new[]{-1,0,6})
        {
            bool withDraws=cases%3!=0;string source=Source(updateCount,behaviorCount,withDraws,false);var pair=Build(source);var output=Ep1GameObjectDestructionProjection.Compose(pair.Native);Verify(output,pair.Native,pair.Instance,updateCount,behaviorCount,withDraws,false);
            var repeat=Build(source);var repeated=Ep1GameObjectDestructionProjection.Compose(repeat.Native);if(!output.InstanceBuffer.SequenceEqual(repeated.InstanceBuffer) || !output.RelocationBuffer.SequenceEqual(repeated.RelocationBuffer) || !output.ImportsBuffer.SequenceEqual(repeated.ImportsBuffer))throw new InvalidDataException("Shared clients/root repeat differs.");
            maxFields=Math.Max(maxFields,GameObjectStockReview.Tokens(output.InstanceBuffer,0).Count);maxImports=Math.Max(maxImports,pair.Instance.ReferencedInstances.Count);if(updateCount==1 && behaviorCount==6){full=pair.Native;fullInstance=pair.Instance;fullSource=source;}cases++;
        }
        var authored=Build(Source(1,6,true,true));Verify(Ep1GameObjectDestructionProjection.Compose(authored.Native),authored.Native,authored.Instance,1,6,true,true);
        // Reborn: independently assert authored identities, optional scalar and forward behavior-to-client link before any comparison restores stock identities in a copied leaf.
        foreach(var selected in new[]{(492,1,"ClientUpdates"),(496,6,"ClientBehaviors")}){int header=(int)Word(authored.Native.InstanceBuffer,selected.Item1),items=(int)Word(authored.Native.InstanceBuffer,header+4);for(int i=0;i<selected.Item2;i++){int at=(int)Word(authored.Native.InstanceBuffer,items+i*4);if(Word(authored.Native.InstanceBuffer,at+4)!=FastHash.GetHashCode(("RebornEditedClient"+i+selected.Item3).ToLowerInvariant()))throw new InvalidDataException("Authored client root ID not applied.");if(selected.Item1==496 && i==4){int cap=(int)Word(authored.Native.InstanceBuffer,at+8);if(cap==0 || Word(authored.Native.InstanceBuffer,cap)!=0x418C0000)throw new InvalidDataException("Authored composed client range cap differs.");}}}
        int authoredList=(int)Word(authored.Native.InstanceBuffer,476),authoredDestroy=Enumerable.Range(0,3).Select(i=>(int)Word(authored.Native.InstanceBuffer,authoredList+8+i*4)).Single(at=>Word(authored.Native.InstanceBuffer,at)==0x0D96A2F4);if(Word(authored.Native.InstanceBuffer,authoredDestroy+52)!=FastHash.GetHashCode("reborneditedclient0clientbehaviors"))throw new InvalidDataException("Forward behavior-to-client identity differs.");cases++;
        int up=(int)Word(full!.InstanceBuffer,492),cb=(int)Word(full.InstanceBuffer,496),upFirst=(int)Word(full.InstanceBuffer,up+8),cbFirst=(int)Word(full.InstanceBuffer,cb+8),body=(int)Word(full.InstanceBuffer,488),draw=(int)Word(full.InstanceBuffer,472),ai=(int)Word(full.InstanceBuffer,484),behavior=(int)Word(full.InstanceBuffer,476);int nativeFaults=0,sourceFaults=0,detachedFaults=0;
        foreach(var fault in new (int At,uint Value)[]{(492,(uint)cb),(496,(uint)up),(496,(uint)body),(496,(uint)ai),(496,(uint)draw),(up,65),(cb,65),(up+4,(uint)cb),(cb+4,(uint)up),(cb+8,(uint)body),(cbFirst+4,Word(full.InstanceBuffer,body+4)),(cbFirst+4,Word(full.InstanceBuffer,upFirst+4)),(upFirst+4,Word(full.InstanceBuffer,body+4)),(upFirst+20,(uint)cb),(cbFirst+12,(uint)up),(528,(uint)cbFirst),(492,uint.MaxValue)}){var bad=Clone(full);Put(bad.InstanceBuffer,fault.At,fault.Value);Refuse(()=>Ep1GameObjectDestructionProjection.Compose(bad));nativeFaults++;}
        foreach(uint slot in new[]{492u,496u,(uint)(up+4),(uint)(cb+4),(uint)(cb+8)}){var bad=Clone(full);bad.RelocationBuffer=Pack(Slots(bad.RelocationBuffer).Where(s=>s!=slot));Refuse(()=>Ep1GameObjectDestructionProjection.Compose(bad));nativeFaults++;}
        var lostImport=Clone(full);uint clientImport=Slots(full.ImportsBuffer).First(s=>s>=cb && s<behavior);lostImport.ImportsBuffer=Pack(Slots(lostImport.ImportsBuffer).Where(s=>s!=clientImport));Refuse(()=>Ep1GameObjectDestructionProjection.Compose(lostImport));nativeFaults++;
        foreach(var edit in new[]{("ModuleTag_ShockDeathOverLand","Body"),("ModuleTag_ShockDeathOverLand","AI0"),("ModuleTag_ShockDeathOverLand","ModuleTag_Draw"),("ModuleTag_ShockDeathOverLand","LargeGroupKey"),("LargeGroupKey","Destroy"),("Frozen","moduletag_shockdeathoverland")}){Refuse(()=>Build(fullSource!.Replace("id=\""+edit.Item1+"\"","id=\""+edit.Item2+"\"",StringComparison.Ordinal)));sourceFaults++;}
        Refuse(()=>Build(fullSource!.Replace("LargeGroupAudioClientUpdate","RebornUnknownClient",StringComparison.Ordinal)));sourceFaults++;
        var duplicate=Xml(fullSource!);duplicate.DocumentElement!.AppendChild(duplicate.GetElementsByTagName("ClientUpdates",SchemaSet.XmlNamespace)[0]!.CloneNode(true));Refuse(()=>Build(duplicate.OuterXml));sourceFaults++;
        var outputFull=Ep1GameObjectDestructionProjection.Compose(full);var outputRoots=GameObjectStockReview.Tokens(outputFull.InstanceBuffer,0);int globalPrefix=outputRoots[FastHash.GetHashCode("BuildTime")].Offset-140;
        foreach(string name in new[]{"ClientUpdates","ClientBehaviors"})
        {
            int table=outputRoots[FastHash.GetHashCode(name)].Offset,length=name=="ClientUpdates"?216:7404;int rootRow=FindRow(outputFull.InstanceBuffer,FastHash.GetHashCode(name));
            foreach(int at in new[]{rootRow+4,rootRow+8}.Concat(Enumerable.Range(0,length/4).Select(i=>table+i*4))){var bad=Clone(outputFull);Put(bad.InstanceBuffer,at,Word(bad.InstanceBuffer,at)^1u);Refuse(()=>Verify(bad,full,fullInstance!,1,6,true,false));detachedFaults++;}
        }
        foreach(int at in new[]{492,496,upFirst+4,cbFirst+4,body+44}){var bad=Clone(outputFull);Put(bad.InstanceBuffer,globalPrefix+at,Word(bad.InstanceBuffer,globalPrefix+at)^1u);Refuse(()=>Verify(bad,full,fullInstance!,1,6,true,false));detachedFaults++;}
        foreach(bool relo in new[]{false,true}){var bad=Clone(outputFull);if(relo)bad.RelocationBuffer=Array.Empty<byte>();else bad.ImportsBuffer=Array.Empty<byte>();Refuse(()=>Verify(bad,full,fullInstance!,1,6,true,false));detachedFaults++;}
        for(int i=0;i<files.Length;i++)if(!SHA256.HashData(captures[i]).SequenceEqual(SHA256.HashData(SdkEnvironmentPreflight.Read(files[i],1048576))))throw new InvalidDataException("Official client source changed.");
        Console.WriteLine($"Shared root/Draws/clients: PASS ({cases} cases, up to {maxFields} partial root fields/{maxImports} shared Core occurrences, all six original ClientBehaviors and one original ClientUpdate, complete independent 7404/216-byte client and {expectedDraws.Length}-byte Draws prefixes; {nativeFaults} native/{sourceFaults} source/{detachedFaults} detached refusals; no full original behavior/root closure, GameObject stream, inheritance engine, package, playback or gameplay claim).");

        //-------------------------------------------------------------------------------------------------
        /** Reborn: original client members and full Draws join the selected source graph before one Core normalization; optional containers retain official root order. */
        //-------------------------------------------------------------------------------------------------
        string Source(int updateCount,int behaviorCount,bool withDraws,bool edit)
        {
            var document=GameObjectCombatCompositionSmokeTest.Adapt(Xml(GameObjectAiCompositionSmokeTest.Source(targets,15,2,2,false,0)).DocumentElement!,true);document.DocumentElement!.SetAttribute("id","RebornSharedClients");
            foreach(XmlElement chooser in document.GetElementsByTagName("UnitAITargetChooserData",SchemaSet.XmlNamespace))chooser.SetAttribute("TargetingCompareList","DefaultTargetingCompareList");
            if(withDraws){var container=document.CreateElement("Draws",SchemaSet.XmlNamespace);for(int i=0;i<draws.Length;i++){var leaf=document.CreateElement(i<2?"ScriptedModelDraw":"TracerModelDraw",SchemaSet.XmlNamespace);foreach(XmlAttribute a in draws[i].Attributes)if(a.Name!="xmlns" && a.Prefix!="xmlns")leaf.SetAttribute(a.LocalName,a.Value);foreach(XmlNode child in draws[i].ChildNodes)leaf.AppendChild(document.ImportNode(child,true));container.AppendChild(leaf);}document.DocumentElement.InsertBefore(container,document.GetElementsByTagName("Behaviors",SchemaSet.XmlNamespace)[0]);}
            var geometry=document.GetElementsByTagName("Geometry",SchemaSet.XmlNamespace)[0];foreach(var selected in new[]{("ClientUpdates",updateCount), ("ClientBehaviors",behaviorCount)})if(selected.Item2>=0){var container=document.CreateElement(selected.Item1,SchemaSet.XmlNamespace);foreach(var leaf in selected.Item1=="ClientUpdates"?Enumerable.Repeat(updates,selected.Item2):clients.Take(selected.Item2)){var copy=(XmlElement)document.ImportNode(leaf,true);if(selected.Item1=="ClientUpdates" && container.ChildNodes.Count!=0)copy.SetAttribute("id","RebornSecondClientUpdate");if(edit){copy.SetAttribute("id","RebornEditedClient"+container.ChildNodes.Count+selected.Item1);if(copy.LocalName=="AnimationSoundClientBehavior")copy.SetAttribute("MaxUpdateRangeCap","17.5");}container.AppendChild(copy);}document.DocumentElement.InsertBefore(container,geometry);}
            if(edit)((XmlElement)document.GetElementsByTagName("BuildingDestructionBehavior",SchemaSet.XmlNamespace)[0]!).SetAttribute("AssociatedModule","RebornEditedClient0ClientBehaviors");
            Ep1ClientBehaviorsNativeSmokeTest.ResolveSounds(document,schemas,catalog);return document.OuterXml;
        }
        //-------------------------------------------------------------------------------------------------
        /** Reborn: raw official schema and cross-container IDs are checked before one production reference-normalization pass and explicit selected root allocation. */
        //-------------------------------------------------------------------------------------------------
        (Chunk Native,InstanceDeclaration Instance) Build(string source)
        {var raw=Xml(source);GameObjectBodyProjectionSmokeTest.Validate(schemas.Schemas,source);Ep1SelectedModuleIdentity.ValidateSource(raw.DocumentElement!);var instance=ReferencePipelineSmokeTest.Normalize(schemas,source);foreach(var reference in instance.ReferencedInstances)if(catalog.Count(a=>a.TypeId==reference.TypeId && a.InstanceId==reference.InstanceId)!=1)throw new InvalidDataException("Shared client target missing/ambiguous.");return(GameObjectBodyProjectionSmokeTest.Compile(Xml(instance.XmlNode.OuterXml),selectedBehaviors:true),instance);}
        //-------------------------------------------------------------------------------------------------
        /** Reborn: independently compare complete stock native leaves and stock metadata; detach output without rerunning the composer and retain sibling root bindings. */
        //-------------------------------------------------------------------------------------------------
        void Verify(Chunk output,Chunk native,InstanceDeclaration instance,int updateCount,int behaviorCount,bool withDraws,bool edit)
        {
            var n=native.InstanceBuffer;var b=output.InstanceBuffer;var roots=GameObjectStockReview.Tokens(b,0);int prefix=roots[FastHash.GetHashCode("BuildTime")].Offset-140;uint[] relos=Slots(native.RelocationBuffer),imports=Slots(native.ImportsBuffer);
            if(!Slots(output.RelocationBuffer).SequenceEqual(relos.Select(s=>s+(uint)prefix)) || !Slots(output.ImportsBuffer).SequenceEqual(imports.Select(s=>s+(uint)prefix)) || !imports.Select(s=>Word(n,(int)s)).Order().SequenceEqual(Enumerable.Range(1,instance.ReferencedInstances.Count).Select(i=>(uint)i)))throw new InvalidDataException("Client root common auxiliary/occurrence table differs.");
            var recovered=b.AsSpan(prefix).ToArray();foreach(uint slot in relos)Put(recovered,(int)slot,Word(recovered,(int)slot)-(uint)prefix);if(!recovered.SequenceEqual(n))throw new InvalidDataException("Client root inverse native bytes differ.");
            foreach(var field in Ep1GameObjectRootValueTokens.AllFields)if(!roots.TryGetValue(FastHash.GetHashCode(field.Name),out var token) || token.Type!=field.Type || token.Offset!=prefix+field.Offset)throw new InvalidDataException("Client composition changed sibling root binding.");
            var layout=Ep1GameObjectStringTokens.Inspect(n,relos,imports);
            foreach(var sibling in new[]{("Behaviors",(int)Word(n,476)),("AI",480),("Draws",(int)Word(n,472))})if(sibling.Item2!=0){var token=roots[FastHash.GetHashCode(sibling.Item1)];if(Word(b,token.Offset+8)!=prefix+sibling.Item2)throw new InvalidDataException("Client composition changed sibling list binding.");}
            if(withDraws)CheckPrefix(roots[FastHash.GetHashCode("Draws")].Offset,layout.Draws.Start,layout.Draws.End,expectedDraws);
            foreach(var selection in new[]{("ClientUpdates",492,updateCount), ("ClientBehaviors",496,behaviorCount)})
            {
                bool present=roots.TryGetValue(FastHash.GetHashCode(selection.Item1),out var token);int start=(int)Word(n,selection.Item2);if(selection.Item3<0){if(present || start!=0)throw new InvalidDataException("Absent client retained storage/metadata.");continue;}
                var stockField=GameObjectStockReview.Tokens(stock.Bytes,0)[FastHash.GetHashCode(selection.Item1)];if(!present || token.Type!=stockField.Type || Word(b,token.Offset)!=0x6C827073 || Word(b,token.Offset+4)!=0x5D448FD8 || Word(b,token.Offset+8)!=prefix+start || Word(n,start)!=selection.Item3)throw new InvalidDataException("Client root wire/count binding differs.");
                // Reborn: every list marker and wrapper/base chain must bind the actual native module even for authored duplicate-family containers without a single stock-prefix extent.
                for(int i=0;i<selection.Item3;i++){int marker=token.Offset+12+i*12,table=(int)Word(b,marker+8),nativeStart=(int)Word(n,(int)Word(n,start+4)+i*4);if(Word(b,marker)!=0xB151A0EB || Word(b,marker+4)!=0 || Word(b,table)!=2 || Word(b,table+4)!=Word(n,nativeStart) || Word(b,table+8)!=table+12)throw new InvalidDataException("Client root concrete wrapper differs.");int depth=0;table+=12;while(Word(b,table)==1){if(++depth>8)throw new InvalidDataException("Client root base chain bound exceeded.");table=(int)Word(b,table+8);}var terminal=GameObjectStockReview.Tokens(b,table);if(!terminal.TryGetValue(FastHash.GetHashCode("TypeId"),out var typeBinding) || !terminal.TryGetValue(FastHash.GetHashCode("id"),out var idBinding) || typeBinding.Offset!=prefix+nativeStart || idBinding.Offset!=prefix+nativeStart+4)throw new InvalidDataException("Client root native identity binding differs.");}
                var owner=layout.Clients.Single(c=>c.Slot==selection.Item2);if(selection.Item3==0){if(owner.Prefix!=12)throw new InvalidDataException("Empty client metadata differs.");continue;}
                var original=topology.Containers.Single(c=>c.Name==selection.Item1);var map=new Dictionary<int,int>();for(int i=0;i<8;i++)map.Add(original.CountAt+i,i);for(int i=0;i<original.Modules.Length*4;i++)map.Add(original.Items+i,8+i);
                for(int i=0;i<selection.Item3;i++)
                {
                    int leafStart=owner.Modules[i],leafEnd=i+1<owner.Modules.Count?owner.Modules[i+1]:owner.End;var leaf=Extract(native,leafStart,leafEnd);if(edit)Put(leaf.InstanceBuffer,4,FastHash.GetHashCode(original.Modules[i].Type=="LargeGroupAudioClientUpdateModuleData"?updates.GetAttribute("id").ToLowerInvariant():clients[i].GetAttribute("id").ToLowerInvariant()));
                    if(edit && i==4 && selection.Item2==496)continue;
                    if(selection.Item3==2 && selection.Item2==492 && i==1)Put(leaf.InstanceBuffer,4,FastHash.GetHashCode(updates.GetAttribute("id").ToLowerInvariant()));
                    var comparer=new SelectedNativeGraphComparer(stock.Bytes,stock.Asset.References.ToArray(),leaf,instance,requireAllReferences:false);Type type=selection.Item2==492?typeof(LargeGroupAudioClientUpdateModuleData):i==4?typeof(AnimationSoundClientBehaviorModuleData):i is 1 or 5?typeof(Ep1ModelConditionSoundSelectorClientBehaviorModuleData):typeof(ModelConditionAudioLoopClientBehaviorModuleData);comparer.Compare(type,original.Modules[selection.Item2==492?0:i].Native);if(i<original.Modules.Length)foreach(var entry in comparer.NativeOffsets)map.Add(entry.Key,entry.Value+leafStart-start);
                }
                if(!edit && selection.Item3!=2){int length=selection.Item2==492?216:7404;var expected=stock.Bytes.AsSpan(stockField.Offset,length).ToArray();for(int at=8;at<length;at+=12){uint target=Word(expected,at);if(target!=0)Put(expected,at,target>=stockField.Offset && target<stockField.Offset+length?target-(uint)stockField.Offset:checked((uint)(length+map[(int)target])));}if(owner.Prefix!=length)throw new InvalidDataException("Client root metadata extent differs.");CheckPrefix(token.Offset,start,owner.End,expected);}
            }
            //-------------------------------------------------------------------------------------------------
            /** Reborn: translate the detached composed prefix to independent standalone oracle coordinates; native and sibling metadata escapes fail. */
            //-------------------------------------------------------------------------------------------------
            void CheckPrefix(int table,int start,int end,byte[] expected)
            {if(table<0 || table>prefix-expected.Length)throw new InvalidDataException("Client/Draw prefix exceeds metadata owner.");var actual=b.AsSpan(table,expected.Length).ToArray();for(int at=8;at<actual.Length;at+=12){uint target=Word(actual,at);if(target==0)continue;Put(actual,at,target>=table && target<table+actual.Length?target-(uint)table:target>=prefix+start && target<prefix+end?target-(uint)(prefix+start)+(uint)actual.Length:throw new InvalidDataException("Client/Draw metadata crosses owners."));}if(!actual.SequenceEqual(expected))throw new InvalidDataException("Independent composed client/Draw prefix differs.");}
        }
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: native fragments retain global occurrence indices while rebasing only owned relocation targets. */
    //-------------------------------------------------------------------------------------------------
    private static Chunk Extract(Chunk source,int start,int end){var chunk=new Chunk{InstanceBuffer=source.InstanceBuffer.AsSpan(start,end-start).ToArray(),RelocationBuffer=Pack(Slots(source.RelocationBuffer).Where(s=>s>=start && s<end).Select(s=>s-(uint)start)),ImportsBuffer=Pack(Slots(source.ImportsBuffer).Where(s=>s>=start && s<end).Select(s=>s-(uint)start))};foreach(uint slot in Slots(chunk.RelocationBuffer))Put(chunk.InstanceBuffer,(int)slot,Word(chunk.InstanceBuffer,(int)slot)-(uint)start);return chunk;}
    //-------------------------------------------------------------------------------------------------
    /** Reborn: metadata corruption controls find only complete bounded root rows. */
    //-------------------------------------------------------------------------------------------------
    private static int FindRow(byte[] b,uint name){for(int at=0;at<b.Length-12;at+=12){if(Word(b,at)==name)return at;if(Word(b,at)==0)break;}throw new InvalidDataException("Client root row missing.");}
    //-------------------------------------------------------------------------------------------------
    /** Reborn: source parsing forbids DTD/entity expansion and retains all original members and script whitespace. */
    //-------------------------------------------------------------------------------------------------
    private static XmlDocument Xml(string text){using var input=new StringReader(text);using var reader=XmlReader.Create(input,new XmlReaderSettings{DtdProcessing=DtdProcessing.Prohibit,XmlResolver=null});var document=new XmlDocument{XmlResolver=null,PreserveWhitespace=true};document.Load(reader);return document;}
    //-------------------------------------------------------------------------------------------------
    /** Reborn: malformed graphs must fail explicitly, not silently omit a client module. */
    //-------------------------------------------------------------------------------------------------
    private static void Refuse(Action action){try{action();}catch(Exception e)when(e is InvalidDataException or BinaryAssetBuilderException or FormatException or OverflowException){return;}throw new InvalidDataException("Malformed composed client graph accepted.");}
    //-------------------------------------------------------------------------------------------------
    /** Reborn: detached corruption controls own all three buffers. */
    //-------------------------------------------------------------------------------------------------
    private static Chunk Clone(Chunk c)=>new(){InstanceBuffer=c.InstanceBuffer.ToArray(),RelocationBuffer=c.RelocationBuffer.ToArray(),ImportsBuffer=c.ImportsBuffer.ToArray()};
    //-------------------------------------------------------------------------------------------------
    /** Reborn: auxiliary framing and duplicates are checked before any extraction. */
    //-------------------------------------------------------------------------------------------------
    private static uint[] Slots(byte[] b){if(b.Length==0)return Array.Empty<uint>();if(b.Length<8 || b.Length%4!=0 || Word(b,b.Length-4)!=uint.MaxValue)throw new InvalidDataException("Composed client auxiliary framing differs.");var result=Enumerable.Range(0,b.Length/4-1).Select(i=>Word(b,i*4)).ToArray();if(result.Distinct().Count()!=result.Length)throw new InvalidDataException("Composed client duplicate auxiliary.");return result;}
    //-------------------------------------------------------------------------------------------------
    /** Reborn: retain real Tracker slot order and an absent empty stream. */
    //-------------------------------------------------------------------------------------------------
    private static byte[] Pack(IEnumerable<uint> slots){var values=slots.ToArray();if(values.Length==0)return Array.Empty<byte>();var bytes=new byte[(values.Length+1)*4];for(int i=0;i<values.Length;i++)Put(bytes,i*4,values[i]);Put(bytes,values.Length*4,uint.MaxValue);return bytes;}
    //-------------------------------------------------------------------------------------------------
    /** Reborn: native and metadata reads never escape their owned buffer. */
    //-------------------------------------------------------------------------------------------------
    private static uint Word(byte[] b,int at){if(at<0 || at>b.Length-4)throw new InvalidDataException("Composed client word bounds differ.");return BinaryPrimitives.ReadUInt32LittleEndian(b.AsSpan(at,4));}
    //-------------------------------------------------------------------------------------------------
    /** Reborn: only owned test/output buffers receive pointer or corruption patches. */
    //-------------------------------------------------------------------------------------------------
    private static void Put(byte[] b,int at,uint value)=>BinaryPrimitives.WriteUInt32LittleEndian(b.AsSpan(at,4),value);
}
