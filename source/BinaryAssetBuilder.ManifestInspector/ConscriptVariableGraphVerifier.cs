using System.Buffers.Binary;
using System.Xml;
using System.Xml.Schema;
using BinaryAssetBuilder.Core;
using BinaryAssetBuilder.Core.Hashing;
using BinaryAssetBuilder.XmlCompiler;
using SageBinaryData;
using Chunk=Relo.Chunk;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: reusable full native/metadata proof; authored identities select reviewed owners instead of fixed sizes or new list ordinals.
internal sealed class ConscriptVariableGraphVerifier
{
    private static readonly int[] Detached={416,468,472,476,480,484,488,492,496,500,532,540,552,564};
    private readonly XmlSchemaSet official;
    private readonly (byte[] Metadata,ManifestDocument Document,ManifestAsset Asset,long Offset,byte[] Bytes,GameObjectStockReview.Projection Projection) stock;
    private readonly GameObjectModuleReview.Proof topology;
    private readonly Dictionary<string,GameObjectModuleReview.Module> templates;

    //-------------------------------------------------------------------------------------------------
    /** Reborn: bind source templates to independently captured stock identities once per build. */
    //-------------------------------------------------------------------------------------------------
    internal ConscriptVariableGraphVerifier(XmlSchemaSet schemas,string manifest,XmlDocument baseline)
    {
        official=schemas;stock=GameObjectStockReview.Capture(manifest,"GameObject:SovietAntiInfantryInfantry");topology=GameObjectModuleReview.Inspect(manifest,stock.Asset.Name);
        templates=ConscriptModuleScalarContract.Owners(baseline).ToDictionary(o=>o.Element.GetAttribute("id"),o=>topology.Containers.Single(c=>c.Name==o.Container).Modules[o.Index],StringComparer.Ordinal);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: source identities/order select independently captured templates; changed counts never select stock offsets by the new ordinal. */
    //-------------------------------------------------------------------------------------------------
    internal Chunk Verify(XmlDocument source,Chunk native,InstanceDeclaration instance,Dictionary<string,string> origins,Chunk? supplied=null)
    {
        byte[] b=native.InstanceBuffer;var relos=Slots(native.RelocationBuffer);var imports=Slots(native.ImportsBuffer);int cut=(int)Word(b,416);
        // Reborn: verify authored fixed fields and restore only comparison bytes, never the actual output.
        var cleared=b.ToArray();var stockNative=stock.Bytes.AsSpan(stock.Projection.NativeRoot).ToArray();
        GameObjectScalarContract.VerifyAndRestore(source,b,cleared,stockNative);
        GameObjectRootAttributeContract.VerifyAndRestore(source,b,cleared,stockNative);
        ConscriptModuleScalarContract.Verify(source,b,0);
        var originalStock=stock.Bytes.ToArray();foreach(int at in Detached){Put(cleared,at,0);Put(originalStock,stock.Projection.NativeRoot+at,0);}
        var root=new Chunk{InstanceBuffer=cleared,RelocationBuffer=Pack(relos.Where(s=>!Detached.Contains((int)s))),ImportsBuffer=native.ImportsBuffer.ToArray()};
        var rootComparer=new SelectedNativeGraphComparer(originalStock,stock.Asset.References.ToArray(),root,instance,requireAllReferences:false);
        var selection=rootComparer.CompareRootSelection(stock.Projection.NativeRoot);
        var covered=selection.Covered;var ownedRelos=new HashSet<uint>(selection.Relocations);var ownedImports=new HashSet<uint>(selection.Imports);
        var baseBytes=b.AsSpan(0,cut).ToArray();foreach(var field in Ep1GameObjectRootOwnersProjection.Fields)Put(baseBytes,field.Slot,0);
        var layout=Ep1GameObjectStringTokens.Inspect(baseBytes,relos.Where(s=>s<cut&&!Ep1GameObjectRootOwnersProjection.Fields.Any(f=>f.Slot==s)).ToArray(),imports.Where(s=>s<cut).ToArray(),true);
        var tokens=supplied??Ep1GameObjectEmptyRootProjection.ComposeOriginal(native,official);int prefix=tokens.InstanceBuffer.Length-b.Length;BehaviorContainerStockProof.VerifyTail(tokens,native,prefix);
        var rows=GameObjectStockReview.Tokens(tokens.InstanceBuffer,0);var stockRows=GameObjectStockReview.Tokens(stock.Bytes,0);
        if(!rows.Keys.Order().SequenceEqual(stockRows.Keys.Order())||rows.Any(p=>p.Value.Type!=stockRows[p.Key].Type))throw new InvalidDataException("Variable graph root metadata fields differ.");
        var maps=new Dictionary<string,Dictionary<int,int>>(StringComparer.Ordinal);
        var fixedGraphs=new Dictionary<int,(int Start,int End,Type Type,SelectedNativeGraphComparer Comparer)>();int behaviorLength=0;
        foreach(var group in ConscriptModuleScalarContract.Owners(source).GroupBy(o=>o.Container))
        {
            var owners=group.ToArray();int header=group.Key=="Body"?-1:group.Key=="AI"?480:(int)Word(b,group.Key switch{"Draws"=>472,"Behaviors"=>476,"ClientUpdates"=>492,"ClientBehaviors"=>496,_=>throw new InvalidDataException("Unknown module container.")});
            int end=group.Key switch{"Draws"=>layout.Draws.End,"Behaviors"=>layout.Body,"AI"=>layout.Ai.Modules[^1].End,"Body"=>layout.Geometry,_=>layout.Clients.Single(c=>c.Slot==(group.Key=="ClientUpdates"?492:496)).End};
            if(header>=0){if(Word(b,header)!=owners.Length)throw new InvalidDataException("Source/native module count differs.");Cover(header,8,group.Key=="AI");int items=(int)Word(b,header+4);Cover(items,owners.Length*4);Add(ownedRelos,(uint)(header+4));for(int i=0;i<owners.Length;i++)Add(ownedRelos,(uint)(items+i*4));}
            if(group.Key!="AI")Add(ownedRelos,(uint)(group.Key switch{"Body"=>488,"Draws"=>472,"Behaviors"=>476,"ClientUpdates"=>492,_=>496}));
            var map=new Dictionary<int,int>();var stockContainer=topology.Containers.Single(c=>c.Name==group.Key);
            if(group.Key!="Behaviors"&&header>=0){for(int i=0;i<8;i++)map.Add(stockContainer.CountAt+i,header+i);int items=(int)Word(b,header+4);for(int i=0;i<owners.Length*4;i++)map.Add(stockContainer.Items+i,items+i);}
            int behaviorAt=group.Key=="Behaviors"?rows[FastHash.GetHashCode("Behaviors")].Offset:0;
            int behaviorNext=behaviorAt+12+owners.Length*12;
            foreach(var owner in owners)
            {
                int start=ConscriptModuleScalarContract.Address(b,0,owner),stop=owner.Index+1<owners.Length?ConscriptModuleScalarContract.Address(b,0,owners[owner.Index+1]):end;var model=templates[origins[owner.Element.GetAttribute("id")]];
                if(Word(b,start)!=model.TypeId||Word(b,start+4)!=FastHash.GetHashCode(owner.Element.GetAttribute("id").ToLowerInvariant()))throw new InvalidDataException("Source module type/local identity differs.");
                var leaf=Extract(native,start,stop);var compared=Clone(leaf);Put(compared.InstanceBuffer,4,Word(stock.Bytes,model.Native+4));
                // Reborn: fixed source/native values were independently checked; nested graphs and padding remain compared in full.
                foreach(var binding in ConscriptModuleScalarContract.Bindings(owner.NativeType))stock.Bytes.AsSpan(model.Native+binding.Offset,binding.Width).CopyTo(compared.InstanceBuffer.AsSpan(binding.Offset,binding.Width));
                var comparer=new SelectedNativeGraphComparer(stock.Bytes,stock.Asset.References.ToArray(),compared,instance,allowScriptCrLfEquivalent:group.Key=="Draws",requireAllReferences:false,captureNativeCoordinates:group.Key!="Behaviors",captureNativePaths:group.Key=="Behaviors");comparer.Compare(owner.NativeType,model.Native);Own(leaf,start,stop);
                foreach(var coordinate in comparer.NativeOffsets)map.Add(coordinate.Key,start+coordinate.Value);
                if(group.Key=="Behaviors")
                {
                    var standalone=Ep1BehaviorMetadataTokenizer.Tokenize(leaf,official,model.Type,owner.NativeType);var stockModules=topology.Containers.Single(c=>c.Name=="Behaviors").Modules;int index=Array.IndexOf(stockModules,model);int stockEnd=index+1<stockModules.Length?stockModules[index+1].Wrapper:stockRows[FastHash.GetHashCode("AI")].Offset;
                    int localPrefix=BehaviorMetadataStockProof.Compare(standalone,leaf,stock.Bytes,model.Wrapper,stockEnd,owner.NativeType,comparer.NativeRecords);
                    int list=rows[FastHash.GetHashCode("Behaviors")].Offset;if(Word(tokens.InstanceBuffer,list)!=0x6C827073||Word(tokens.InstanceBuffer,list+4)!=0x5D448FD8||Word(tokens.InstanceBuffer,list+8)!=prefix+layout.List)throw new InvalidDataException("Variable behavior count metadata differs.");
                    int row=list+12+owner.Index*12,at=(int)Word(tokens.InstanceBuffer,row+8);if(Word(tokens.InstanceBuffer,row)!=0xB151A0EB||Word(tokens.InstanceBuffer,row+4)!=0||at!=behaviorNext)throw new InvalidDataException("Variable behavior wrapper/order differs.");
                    for(int offset=0;offset<localPrefix;offset+=12){uint target=Word(standalone.InstanceBuffer,offset+8);uint expected=target==0?0:target<localPrefix?target+(uint)at:target-(uint)localPrefix+(uint)(prefix+start);if(Word(tokens.InstanceBuffer,at+offset)!=Word(standalone.InstanceBuffer,offset)||Word(tokens.InstanceBuffer,at+offset+4)!=Word(standalone.InstanceBuffer,offset+4)||Word(tokens.InstanceBuffer,at+offset+8)!=expected)throw new InvalidDataException("Variable behavior metadata splice differs.");}
                    behaviorNext+=localPrefix;
                }
            }
            if(group.Key=="Behaviors")behaviorLength=behaviorNext-behaviorAt;else maps.Add(group.Key,map);
        }
        Add(ownedRelos,500);Fixed(typeof(Ep1Geometry),(int)Word(stock.Bytes,stock.Projection.NativeRoot+500),layout.Geometry,cut);
        for(int i=0;i<Ep1GameObjectRootOwnersProjection.Fields.Count;i++){var field=Ep1GameObjectRootOwnersProjection.Fields[i];int start=(int)Word(b,field.Slot),end=i+1<Ep1GameObjectRootOwnersProjection.Fields.Count?(int)Word(b,Ep1GameObjectRootOwnersProjection.Fields[i+1].Slot):b.Length;Add(ownedRelos,(uint)field.Slot);Fixed(field.NativeType,(int)Word(stock.Bytes,stock.Projection.NativeRoot+field.Slot),start,end);}
        if(!ownedRelos.Order().SequenceEqual(relos.Order())||!ownedImports.Order().SequenceEqual(imports.Order())||!imports.Select(s=>Word(b,(int)s)).Order().SequenceEqual(Enumerable.Range(1,instance.ReferencedInstances.Count).Select(i=>(uint)i)))throw new InvalidDataException("Variable graph auxiliary/reference ownership differs.");
        int gap=0;for(int i=0;i<b.Length;i++){if(covered[i])gap=0;else if(b[i]!=0||++gap>3)throw new InvalidDataException("Unowned variable graph native allocation.");}
        // Reborn: every non-behavior metadata owner retains its independent stock field map; changed behavior extents are source-composed, not fixed at 88824.
        int rootLength=(stockRows.Count+1)*12;var metadataOwned=new bool[prefix];Array.Fill(metadataOwned,true,0,rootLength);
        var oldTables=stockRows.Values.Select(v=>v.Offset).Where(at=>at>=rootLength&&at<stock.Projection.NativeRoot).Distinct().Order().ToArray();
        foreach(var pair in stockRows)
        {
            int old=pair.Value.Offset,at=rows[pair.Key].Offset;
            if(old==0){if(at!=0)throw new InvalidDataException("Absent root metadata target differs.");continue;}
            if(old>=stock.Projection.NativeRoot){if(!rootComparer.NativeOffsets.TryGetValue(old,out int coordinate)||at!=prefix+coordinate)throw new InvalidDataException("Direct root metadata coordinate differs.");continue;}
            int index=Array.IndexOf(oldTables,old),end=index+1<oldTables.Length?oldTables[index+1]:stock.Projection.NativeRoot;
            string name=topology.Containers.Select(c=>c.Name).Concat(Ep1GameObjectRootOwnersProjection.Fields.Select(f=>f.Name)).Append("Geometry").SingleOrDefault(n=>FastHash.GetHashCode(n)==pair.Key)??"Root";
            int length=name=="Behaviors"?behaviorLength:end-old;
            if(length<12||length%12!=0||at<rootLength||at>prefix-length||metadataOwned.AsSpan(at,length).Contains(true))throw new InvalidDataException("Variable metadata owner overlap/bounds.");
            if(name!="Behaviors")
            {
                int slot=name=="Geometry"?500:Ep1GameObjectRootOwnersProjection.Fields.FirstOrDefault(f=>f.Name==name).Slot;
                if(fixedGraphs.TryGetValue(slot,out var graph)){var leaf=Extract(native,graph.Start,graph.End);BehaviorMetadataStockProof.Compare(Detach(tokens,prefix,at,length,graph.Start,graph.End),leaf,stock.Bytes,old,end,graph.Type,graph.Comparer.NativeRecords);}
                else{var coordinates=maps.GetValueOrDefault(name)??rootComparer.NativeOffsets;var expected=stock.Bytes.AsSpan(old,end-old).ToArray();for(int offset=8;offset<length;offset+=12){uint target=Word(expected,offset);if(target!=0)Put(expected,offset,target>=old&&target<end?target-(uint)old+(uint)at:coordinates.TryGetValue((int)target,out int coordinate)?(uint)(prefix+coordinate):throw new InvalidDataException("Unmapped stock metadata target: "+name));}if(!tokens.InstanceBuffer.AsSpan(at,length).SequenceEqual(expected))throw new InvalidDataException("Variable metadata stock owner differs: "+name);}
            }
            Array.Fill(metadataOwned,true,at,length);
        }
        if(metadataOwned.Contains(false))throw new InvalidDataException("Unowned variable graph metadata.");return tokens;

        // Reborn: unchanged nonmodule owners retain independent complete stock payload comparison despite shifted addresses/reference indices.
        void Fixed(Type type,int old,int start,int end){var leaf=Extract(native,start,end);var comparer=new SelectedNativeGraphComparer(stock.Bytes,stock.Asset.References.ToArray(),leaf,instance,requireAllReferences:false,captureNativeCoordinates:false,captureNativePaths:true);comparer.Compare(type,old);fixedGraphs.Add(type==typeof(Ep1Geometry)?500:Ep1GameObjectRootOwnersProjection.Fields.Single(f=>f.NativeType==type).Slot,(start,end,type,comparer));Own(leaf,start,end);}
        // Reborn: whole-graph ownership is the disjoint union of exact leaf allocations and their auxiliary sources.
        void Own(Chunk leaf,int start,int end){Cover(start,end-start);foreach(uint s in Slots(leaf.RelocationBuffer))Add(ownedRelos,s+(uint)start);foreach(uint s in Slots(leaf.ImportsBuffer))Add(ownedImports,s+(uint)start);}
        // Reborn: only the inline AI header may overlap the typed root's already verified bytes.
        void Cover(int start,int size,bool inline=false){if(start<0||size<0||start>b.Length-size||!inline&&covered.AsSpan(start,size).Contains(true))throw new InvalidDataException("Variable graph owner overlap/bounds.");Array.Fill(covered,true,start,size);}
    }

    // Reborn: rebase a combined metadata owner and its native tail into a standalone stock-proof input without changing any occurrence index.
    private static Chunk Detach(Chunk output,int prefix,int table,int length,int start,int end){var leaf=Extract(output,prefix+start,prefix+end);var result=new Chunk{InstanceBuffer=new byte[length+leaf.InstanceBuffer.Length],RelocationBuffer=Pack(Slots(leaf.RelocationBuffer).Select(s=>s+(uint)length)),ImportsBuffer=Pack(Slots(leaf.ImportsBuffer).Select(s=>s+(uint)length))};output.InstanceBuffer.AsSpan(table,length).CopyTo(result.InstanceBuffer);leaf.InstanceBuffer.CopyTo(result.InstanceBuffer,length);foreach(uint slot in Slots(leaf.RelocationBuffer))Put(result.InstanceBuffer,length+(int)slot,Word(leaf.InstanceBuffer,(int)slot)+(uint)length);for(int at=8;at<length;at+=12){uint target=Word(result.InstanceBuffer,at);if(target!=0)Put(result.InstanceBuffer,at,target>=table&&target<table+length?target-(uint)table:target>=prefix+start&&target<prefix+end?target-(uint)(prefix+start)+(uint)length:throw new InvalidDataException("Variable metadata detachment crosses owners."));}return result;}

    // Reborn: extraction retains common Core indices but rejects pointers escaping their complete owner allocation.
    private static Chunk Extract(Chunk source,int start,int end){if(start<0||end<start||end>source.InstanceBuffer.Length)throw new InvalidDataException("Module extraction bounds.");var leaf=new Chunk{InstanceBuffer=source.InstanceBuffer.AsSpan(start,end-start).ToArray(),RelocationBuffer=Pack(Slots(source.RelocationBuffer).Where(s=>s>=start&&s<end).Select(s=>s-(uint)start)),ImportsBuffer=Pack(Slots(source.ImportsBuffer).Where(s=>s>=start&&s<end).Select(s=>s-(uint)start))};foreach(uint s in Slots(leaf.RelocationBuffer)){uint target=Word(leaf.InstanceBuffer,(int)s);if(target<start||target>=end)throw new InvalidDataException("Cross-module native pointer.");Put(leaf.InstanceBuffer,(int)s,target-(uint)start);}return leaf;}
    // Reborn: duplicate auxiliary ownership must fail rather than silently collapse in a set.
    private static void Add(HashSet<uint> slots,uint value){if(!slots.Add(value))throw new InvalidDataException("Duplicate variable graph auxiliary owner.");}
    // Reborn: diagnostics retain caller-owned snapshots and independent copies for mutation controls.
    private static Chunk Clone(Chunk c)=>new(){InstanceBuffer=c.InstanceBuffer.ToArray(),RelocationBuffer=c.RelocationBuffer.ToArray(),ImportsBuffer=c.ImportsBuffer.ToArray()};
    // Reborn: repeated output equality includes all three native streams.
    private static bool Equal(Chunk a,Chunk b)=>a.InstanceBuffer.SequenceEqual(b.InstanceBuffer)&&a.RelocationBuffer.SequenceEqual(b.RelocationBuffer)&&a.ImportsBuffer.SequenceEqual(b.ImportsBuffer);
    // Reborn: explicit framing and unique aligned slots precede native partitioning.
    private static uint[] Slots(byte[] bytes){if(bytes.Length==0)return Array.Empty<uint>();if(bytes.Length<8||bytes.Length%4!=0||Word(bytes,bytes.Length-4)!=uint.MaxValue)throw new InvalidDataException("Variable graph auxiliary framing.");var slots=Enumerable.Range(0,bytes.Length/4-1).Select(i=>Word(bytes,i*4)).ToArray();if(slots.Any(s=>s%4!=0)||slots.Distinct().Count()!=slots.Length)throw new InvalidDataException("Duplicate/unaligned graph auxiliary slot.");return slots;}
    // Reborn: ordered nonempty auxiliary streams have one terminating sentinel.
    private static byte[] Pack(IEnumerable<uint> source){var slots=source.ToArray();if(slots.Length==0)return Array.Empty<byte>();var bytes=new byte[(slots.Length+1)*4];for(int i=0;i<slots.Length;i++)Put(bytes,i*4,slots[i]);Put(bytes,slots.Length*4,uint.MaxValue);return bytes;}
    // Reborn: all diagnostic word reads stay within their supplied graph.
    private static uint Word(byte[] bytes,int at){if(at<0||at>bytes.Length-4)throw new InvalidDataException("Variable graph word bounds.");return BinaryPrimitives.ReadUInt32LittleEndian(bytes.AsSpan(at,4));}
    // Reborn: only newly owned comparison arrays receive identity/coordinate patches.
    private static void Put(byte[] bytes,int at,uint value)=>BinaryPrimitives.WriteUInt32LittleEndian(bytes.AsSpan(at,4),value);
    // Reborn: each deliberately corrupt graph must be refused, not merely reported as incomplete.
    private static void Refuse(Action action){try{action();}catch(InvalidDataException){return;}throw new InvalidDataException("Variable module corruption accepted.");}
}
