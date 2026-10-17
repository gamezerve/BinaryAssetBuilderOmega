using System.Buffers.Binary;
using System.Xml.Schema;
using BinaryAssetBuilder.Core;
using BinaryAssetBuilder.Core.Hashing;
using BinaryAssetBuilder.XmlCompiler;
using SageBinaryData;
using Chunk=Relo.Chunk;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: independently qualify every actual original root metadata owner; asset publication remains a separate gate.
internal static class GameObjectOriginalMetadataSmokeTest
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: stock tables supply names/tags/shape; independently walked typed native records alone translate their native targets. */
    //-------------------------------------------------------------------------------------------------
    internal static void Run(XmlSchemaSet schemas,Chunk native,InstanceDeclaration instance,string manifest)
    {
        var stock=GameObjectStockReview.Capture(manifest,"GameObject:SovietAntiInfantryInfantry");var topology=GameObjectModuleReview.Inspect(manifest,stock.Asset.Name);var oldRows=GameObjectStockReview.Tokens(stock.Bytes,0);var before=Clone(native);
        var output=Ep1GameObjectEmptyRootProjection.ComposeOriginal(native,schemas);int prefix=output.InstanceBuffer.Length-native.InstanceBuffer.Length;var rows=GameObjectStockReview.Tokens(output.InstanceBuffer,0);
        if(prefix!=stock.Projection.NativeRoot || rows.Count!=126 || !rows.Keys.Order().SequenceEqual(oldRows.Keys.Order()) || rows.Any(p=>p.Value.Type!=oldRows[p.Key].Type))throw new InvalidDataException("Original metadata root name/tag/extent differs: "+prefix);
        BehaviorContainerStockProof.VerifyTail(output,native,prefix);var expected=new byte[prefix];var owned=new bool[prefix];int rootLength=(rows.Count+1)*12;output.InstanceBuffer.AsSpan(0,rootLength).CopyTo(expected);Array.Fill(owned,true,0,rootLength);
        // Reborn: detach only separately proved modules/EP1 owners before the typed root-coordinate walk, retaining every ordinary root record and list.
        int[] detached={416,468,472,476,480,484,488,492,496,500,532,540,552,564};var cleared=native.InstanceBuffer.ToArray();var original=stock.Bytes.ToArray();foreach(int at in detached){Put(cleared,at,0);Put(original,stock.Projection.NativeRoot+at,0);}
        var selection=new Chunk{InstanceBuffer=cleared,RelocationBuffer=Pack(Slots(native.RelocationBuffer).Where(s=>!detached.Contains((int)s))),ImportsBuffer=native.ImportsBuffer.ToArray()};var rootComparer=new SelectedNativeGraphComparer(original,stock.Asset.References.ToArray(),selection,instance,requireAllReferences:false);rootComparer.CompareRootSelection(stock.Projection.NativeRoot);
        foreach(var field in Ep1GameObjectRootValueTokens.AllFields)if(rows[FastHash.GetHashCode(field.Name)].Offset!=prefix+field.Offset)throw new InvalidDataException("Original flat root binding differs: "+field.Name);
        int cut=(int)Word(native.InstanceBuffer,416);var baseBytes=native.InstanceBuffer.AsSpan(0,cut).ToArray();foreach(var field in Ep1GameObjectRootOwnersProjection.Fields)Put(baseBytes,field.Slot,0);var layout=Ep1GameObjectStringTokens.Inspect(baseBytes,Slots(native.RelocationBuffer).Where(s=>s<cut && !Ep1GameObjectRootOwnersProjection.Fields.Any(f=>f.Slot==s)).ToArray(),Slots(native.ImportsBuffer).Where(s=>s<cut).ToArray(),true);
        var maps=new Dictionary<string,Dictionary<int,int>>(StringComparer.Ordinal);
        foreach(string name in new[]{"Draws","AI","Body","ClientUpdates","ClientBehaviors"})
        {
            var container=topology.Containers.Single(c=>c.Name==name);int[] starts;int end,header;
            if(name=="Draws"){starts=layout.Draws.Modules.ToArray();end=layout.Draws.End;header=layout.Draws.Start;}else if(name=="AI"){starts=layout.Ai.Modules.Select(m=>m.Start).ToArray();end=layout.Ai.Modules[^1].End;header=480;}else if(name=="Body"){starts=new[]{layout.Body};end=layout.Geometry;header=-1;}else{var owner=layout.Clients.Single(c=>c.Slot==(name=="ClientUpdates"?492:496));starts=owner.Modules.ToArray();end=owner.End;header=owner.Start;}
            var map=new Dictionary<int,int>();if(header>=0){for(int j=0;j<8;j++)map.Add(container.CountAt+j,header+j);int items=(int)Word(native.InstanceBuffer,header+4);for(int j=0;j<starts.Length*4;j++)map.Add(container.Items+j,items+j);}
            for(int i=0;i<starts.Length;i++)
            {
                Type type=name switch{"Draws"=>i<2?typeof(W3DScriptedModelDrawModuleData):typeof(W3DTracerDrawModuleData),"AI"=>typeof(Ep1AIUpdateModuleData),"Body"=>typeof(Ep1ActiveBodyModuleData),"ClientUpdates"=>typeof(LargeGroupAudioClientUpdateModuleData),_=>container.Modules[i].Type switch{"ModelConditionSoundSelectorClientBehaviorModuleData"=>typeof(Ep1ModelConditionSoundSelectorClientBehaviorModuleData),"AnimationSoundClientBehaviorModuleData"=>typeof(AnimationSoundClientBehaviorModuleData),"ModelConditionAudioLoopClientBehaviorModuleData"=>typeof(ModelConditionAudioLoopClientBehaviorModuleData),_=>throw new InvalidDataException("Original metadata client family differs.")}};
                var leaf=Extract(native,starts[i],i+1<starts.Length?starts[i+1]:end);var comparer=new SelectedNativeGraphComparer(stock.Bytes,stock.Asset.References.ToArray(),leaf,instance,allowScriptCrLfEquivalent:name=="Draws",requireAllReferences:false);comparer.Compare(type,container.Modules[i].Native);foreach(var p in comparer.NativeOffsets)map.Add(p.Key,starts[i]+p.Value);
            }
            maps.Add(name,map);
        }
        // Reborn: root intervals must be disjoint and cover every metadata byte, with no inferred or ignored table gaps.
        var oldTables=oldRows.Values.Select(v=>v.Offset).Where(t=>t>=rootLength && t<stock.Projection.NativeRoot).Distinct().Order().ToArray();int tables=0;
        foreach(var pair in oldRows)
        {
            int old=pair.Value.Offset,at=rows[pair.Key].Offset;if(old==0){if(at!=0)throw new InvalidDataException("Original absent root target differs.");continue;}if(old>=stock.Projection.NativeRoot){if(!rootComparer.NativeOffsets.TryGetValue(old,out int coordinate) || at!=prefix+coordinate)throw new InvalidDataException("Original direct native root target differs.");continue;}
            if(old<rootLength)throw new InvalidDataException("Original root table aliases directory.");int index=Array.IndexOf(oldTables,old),end=index+1<oldTables.Length?oldTables[index+1]:stock.Projection.NativeRoot,length=end-old;
            if(length<=0 || length%12!=0 || at<rootLength || at>prefix-length || owned.AsSpan(at,length).Contains(true))throw new InvalidDataException("Original metadata owner overlap/bounds.");string name=topology.Containers.Select(c=>c.Name).Concat(Ep1GameObjectRootOwnersProjection.Fields.Select(f=>f.Name)).Append("Geometry").SingleOrDefault(n=>FastHash.GetHashCode(n)==pair.Key)??"Root";
            if(name=="Behaviors")
            {
                var leaf=Extract(native,layout.List,layout.Body);BehaviorContainerStockProof.Run(leaf,schemas,topology.Containers.Single(c=>c.Name==name).Modules,stock.Bytes,stock.Asset.References.ToArray(),instance,(id,_)=>Ep1BehaviorMetadataTokenizer.ReviewedNativeType(id),Detach(output,prefix,at,length,layout.List,layout.Body),controls:false);output.InstanceBuffer.AsSpan(at,length).CopyTo(expected.AsSpan(at));
            }
            else if(Ep1GameObjectRootOwnersProjection.Fields.Any(f=>f.Name==name) || name=="Geometry")
            {
                int start,limit,slot;Type type;if(name=="Geometry"){start=layout.Geometry;limit=cut;slot=500;type=typeof(Ep1Geometry);}else{var fields=Ep1GameObjectRootOwnersProjection.Fields;int i=Enumerable.Range(0,fields.Count).Single(i=>fields[i].Name==name);slot=fields[i].Slot;start=(int)Word(native.InstanceBuffer,slot);limit=i+1<fields.Count?(int)Word(native.InstanceBuffer,fields[i+1].Slot):native.InstanceBuffer.Length;type=fields[i].NativeType;}
                var leaf=Extract(native,start,limit);var comparer=new SelectedNativeGraphComparer(stock.Bytes,stock.Asset.References.ToArray(),leaf,instance,requireAllReferences:false,captureNativeCoordinates:false,captureNativePaths:true);comparer.Compare(type,(int)Word(stock.Bytes,stock.Projection.NativeRoot+slot));BehaviorMetadataStockProof.Compare(Detach(output,prefix,at,length,start,limit),leaf,stock.Bytes,old,end,type,comparer.NativeRecords);output.InstanceBuffer.AsSpan(at,length).CopyTo(expected.AsSpan(at));
            }
            else
            {
                var map=maps.TryGetValue(name,out var specific)?specific:rootComparer.NativeOffsets;var translated=stock.Bytes.AsSpan(old,length).ToArray();for(int offset=8;offset<length;offset+=12){uint target=Word(translated,offset);if(target!=0)Put(translated,offset,target>=old && target<end?target-(uint)old+(uint)at:map.TryGetValue((int)target,out int coordinate)?checked((uint)(prefix+coordinate)):throw new InvalidDataException("Original metadata target lacks native owner: "+name+"/"+target));}if(!output.InstanceBuffer.AsSpan(at,length).SequenceEqual(translated))throw new InvalidDataException("Original metadata stock table differs: "+name);translated.CopyTo(expected,at);
            }
            Array.Fill(owned,true,at,length);tables++;
        }
        if(owned.Contains(false))throw new InvalidDataException("Original metadata has unowned bytes.");Verify(output);if(!Equal(before,native) || !Equal(output,Ep1GameObjectEmptyRootProjection.ComposeOriginal(native,schemas)))throw new InvalidDataException("Original metadata repeat/input isolation differs.");int refusals=0;
        // Reborn: freeze only independently qualified expected bindings, then mutate every metadata word and reject non-null original-only pointer variants.
        for(int at=0;at<prefix;at+=4){var bad=Clone(output);bad.InstanceBuffer[at]^=1;Refuse(()=>Verify(bad));refusals++;}foreach(int slot in new[]{408,464,528}){var bad=Clone(native);Put(bad.InstanceBuffer,slot,1);Refuse(()=>Ep1GameObjectEmptyRootProjection.ComposeOriginal(bad,schemas));refusals++;}
        foreach(bool relocation in new[]{true,false}){var bad=Clone(output);if(relocation)bad.RelocationBuffer=Array.Empty<byte>();else bad.ImportsBuffer=Array.Empty<byte>();Refuse(()=>Verify(bad));refusals++;}var changed=Clone(output);changed.InstanceBuffer[prefix+140]^=1;Refuse(()=>Verify(changed));refusals++;
        Console.WriteLine($"Original Conscript metadata: PASS (126/126 original root fields, {tables} disjoint stock-bound metadata owners, full {prefix}-byte prefix; one Core/{instance.ReferencedInstances.Count} occurrences; BIN/RELO/IMP={output.InstanceBuffer.Length}/{output.RelocationBuffer.Length}/{output.ImportsBuffer.Length}; {refusals} output/native refusals; exact native tail inversion; no stream/package/gameplay or general inheritance admission).");
        //-------------------------------------------------------------------------------------------------
        /** Reborn: a frozen independently stock-qualified oracle supports exhaustive corruption controls without regenerating expected metadata through the producer. */
        //-------------------------------------------------------------------------------------------------
        void Verify(Chunk value){if(value.InstanceBuffer.Length<prefix || !value.InstanceBuffer.AsSpan(0,prefix).SequenceEqual(expected))throw new InvalidDataException("Original qualified metadata differs.");BehaviorContainerStockProof.VerifyTail(value,native,prefix);}
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: detach observed metadata, refusing every cross-owner target instead of regenerating observed bytes. */
    //-------------------------------------------------------------------------------------------------
    private static Chunk Detach(Chunk output,int prefix,int table,int length,int start,int end)
    {
        var leaf=Extract(output,prefix+start,prefix+end);var result=new Chunk{InstanceBuffer=new byte[length+leaf.InstanceBuffer.Length],RelocationBuffer=Pack(Slots(leaf.RelocationBuffer).Select(s=>s+(uint)length)),ImportsBuffer=Pack(Slots(leaf.ImportsBuffer).Select(s=>s+(uint)length))};output.InstanceBuffer.AsSpan(table,length).CopyTo(result.InstanceBuffer);leaf.InstanceBuffer.CopyTo(result.InstanceBuffer,length);foreach(uint slot in Slots(leaf.RelocationBuffer))Put(result.InstanceBuffer,length+(int)slot,Word(leaf.InstanceBuffer,(int)slot)+(uint)length);
        for(int at=8;at<length;at+=12){uint target=Word(result.InstanceBuffer,at);if(target!=0)Put(result.InstanceBuffer,at,target>=table && target<table+length?target-(uint)table:target>=prefix+start && target<prefix+end?target-(uint)(prefix+start)+(uint)length:throw new InvalidDataException("Original metadata detachment crosses owners."));}return result;
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: strict native extraction retains common Core indices and rebases only bounded owned pointers. */
    //-------------------------------------------------------------------------------------------------
    private static Chunk Extract(Chunk source,int start,int end){if(start<0 || end<=start || end>source.InstanceBuffer.Length)throw new InvalidDataException("Original metadata native bounds.");var result=new Chunk{InstanceBuffer=source.InstanceBuffer.AsSpan(start,end-start).ToArray(),RelocationBuffer=Pack(Slots(source.RelocationBuffer).Where(s=>s>=start && s<end).Select(s=>s-(uint)start)),ImportsBuffer=Pack(Slots(source.ImportsBuffer).Where(s=>s>=start && s<end).Select(s=>s-(uint)start))};foreach(uint slot in Slots(result.RelocationBuffer)){uint target=Word(result.InstanceBuffer,(int)slot);if(target<start || target>=end)throw new InvalidDataException("Original metadata native cross-owner pointer.");Put(result.InstanceBuffer,(int)slot,target-(uint)start);}return result;}
    //-------------------------------------------------------------------------------------------------
    /** Reborn: deliberate corruption must throw a bounded data refusal. */
    //-------------------------------------------------------------------------------------------------
    private static void Refuse(Action action){try{action();}catch(InvalidDataException){return;}throw new InvalidDataException("Original metadata corruption accepted.");}
    //-------------------------------------------------------------------------------------------------
    /** Reborn: determinism and input isolation include all buffers. */
    //-------------------------------------------------------------------------------------------------
    private static bool Equal(Chunk a,Chunk b)=>a.InstanceBuffer.SequenceEqual(b.InstanceBuffer) && a.RelocationBuffer.SequenceEqual(b.RelocationBuffer) && a.ImportsBuffer.SequenceEqual(b.ImportsBuffer);
    //-------------------------------------------------------------------------------------------------
    /** Reborn: controls mutate owned copies, not compiler input. */
    //-------------------------------------------------------------------------------------------------
    private static Chunk Clone(Chunk c)=>new(){InstanceBuffer=c.InstanceBuffer.ToArray(),RelocationBuffer=c.RelocationBuffer.ToArray(),ImportsBuffer=c.ImportsBuffer.ToArray()};
    //-------------------------------------------------------------------------------------------------
    /** Reborn: independent auxiliary framing preserves unique source order. */
    //-------------------------------------------------------------------------------------------------
    private static uint[] Slots(byte[] b){if(b.Length==0)return Array.Empty<uint>();if(b.Length<8 || b.Length%4!=0 || Word(b,b.Length-4)!=uint.MaxValue)throw new InvalidDataException("Original metadata auxiliary framing.");var values=Enumerable.Range(0,b.Length/4-1).Select(i=>Word(b,i*4)).ToArray();if(values.Distinct().Count()!=values.Length)throw new InvalidDataException("Original metadata duplicate auxiliary source.");return values;}
    //-------------------------------------------------------------------------------------------------
    /** Reborn: coordinate transformations retain sentinel framing. */
    //-------------------------------------------------------------------------------------------------
    private static byte[] Pack(IEnumerable<uint> values){var slots=values.ToArray();if(slots.Length==0)return Array.Empty<byte>();var b=new byte[(slots.Length+1)*4];for(int i=0;i<slots.Length;i++)Put(b,i*4,slots[i]);Put(b,slots.Length*4,uint.MaxValue);return b;}
    //-------------------------------------------------------------------------------------------------
    /** Reborn: bounded reads never escape the admitted arrays. */
    //-------------------------------------------------------------------------------------------------
    private static uint Word(byte[] b,int at){if(at<0 || at>b.Length-4)throw new InvalidDataException("Original metadata word bounds.");return BinaryPrimitives.ReadUInt32LittleEndian(b.AsSpan(at,4));}
    //-------------------------------------------------------------------------------------------------
    /** Reborn: coordinate writes affect only newly owned arrays. */
    //-------------------------------------------------------------------------------------------------
    private static void Put(byte[] b,int at,uint value)=>BinaryPrimitives.WriteUInt32LittleEndian(b.AsSpan(at,4),value);
}
