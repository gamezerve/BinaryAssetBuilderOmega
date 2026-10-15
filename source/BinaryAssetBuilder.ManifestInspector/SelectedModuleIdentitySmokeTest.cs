using System.Buffers.Binary;
using System.Xml;
using System.Xml.Schema;
using BinaryAssetBuilder.Core.Hashing;
using BinaryAssetBuilder.Core.SageXml;
using BinaryAssetBuilder.XmlCompiler;
using Chunk=Relo.Chunk;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: local module scope is tested independently of asset-reference tables and pipeline-only IDs on nonmodule list records.
internal static class SelectedModuleIdentitySmokeTest
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: exercise forward/backward/case-folded local links, object-local scope lifetime and source/native bypass controls in one offline batch. */
    //-------------------------------------------------------------------------------------------------
    internal static void Run()
    {
        XmlSchemaSet? schemas=null;SdkEffectiveSchema.Inspect(shieldCandidate:true,reviewHooks:true,onAdmitted:s=>schemas=s);if(schemas==null)throw new InvalidDataException("Reviewed identity schema required.");
        int cases=0,sourceFaults=0,nativeFaults=0;Chunk? populated=null;
        string[] names={"Smc","Destroy","Shooter","Body"};
        foreach(bool reverse in new[]{false,true})foreach(int position in new[]{0,1,2})foreach(string target in names)
        {
            var source=Source(reverse,position,target.ToUpperInvariant());GameObjectBodyProjectionSmokeTest.Validate(schemas,source.OuterXml);
            var native=GameObjectBodyProjectionSmokeTest.Compile(source,selectedBehaviors:true);var output=Ep1GameObjectDestructionProjection.Compose(native);var repeat=GameObjectBodyProjectionSmokeTest.Compile(source,selectedBehaviors:true);
            if(!native.InstanceBuffer.SequenceEqual(repeat.InstanceBuffer) || native.ImportsBuffer.Length!=0 || output.ImportsBuffer.Length!=0)throw new InvalidDataException("Local scope mutated repeat output or became an asset import.");
            int list=(int)Word(native.InstanceBuffer,476),body=(int)Word(native.InstanceBuffer,488);var modules=Enumerable.Range(0,3).Select(i=>(int)Word(native.InstanceBuffer,list+8+i*4)).ToArray();
            uint[] expected=reverse?new[]{Hash("Shooter"),Hash("Destroy")}:new[]{Hash("Destroy"),Hash("Shooter")};var ids=expected.ToList();ids.Insert(position,Hash("Smc"));
            for(int i=0;i<3;i++)if(Word(native.InstanceBuffer,modules[i]+4)!=ids[i])throw new InvalidDataException("Local behavior ID hash differs.");
            int destruction=modules.Single(m=>Word(native.InstanceBuffer,m)==0x0D96A2F4);
            if(Word(native.InstanceBuffer,body+4)!=Hash("Body") || Word(native.InstanceBuffer,destruction+52)!=Hash(target))throw new InvalidDataException("Cross-container local target hash differs.");
            if(!reverse && position==0 && target=="Body")populated=native;cases++;
        }
        var valid=Source(false,0,"Body");
        foreach(var fault in new[]{("Body","id","Smc"),("Body","id","sMC"),("Body","id",""),("Body","id"," "),("Destroy","id","Smc"),("Shooter","id","dEsTrOy"),("Smc","id",""),("Destroy","AssociatedModule","MissingLocalModule"),("Destroy","AssociatedModule","")})
        {
            var bad=(XmlDocument)valid.CloneNode(true);var module=bad.SelectNodes("//*[@id]")!.Cast<XmlElement>().Where(e=>e.LocalName is "ActiveBody" or "ObjectSMCHelper" or "BuildingDestructionBehavior" or "WeaponSetUpdate").Single(e=>e.GetAttribute("id")==fault.Item1);module.SetAttribute(fault.Item2,fault.Item3);Refuse(()=>GameObjectBodyProjectionSmokeTest.Compile(bad,selectedBehaviors:true));sourceFaults++;
        }
        var missing=(XmlDocument)valid.CloneNode(true);missing.GetElementsByTagName("ActiveBody",SchemaSet.XmlNamespace)[0]!.Attributes!.RemoveNamedItem("id");Refuse(()=>GameObjectBodyProjectionSmokeTest.Compile(missing,selectedBehaviors:true));sourceFaults++;
        foreach(string mode in new[]{"body-alias","body-case-alias","behavior-alias","dangling-associated"})
        {
            var bad=Clone(populated!);int list=(int)Word(bad.InstanceBuffer,476),body=(int)Word(bad.InstanceBuffer,488),first=(int)Word(bad.InstanceBuffer,list+8),second=(int)Word(bad.InstanceBuffer,list+12);
            if(mode.StartsWith("body",StringComparison.Ordinal))Put(bad.InstanceBuffer,body+4,mode=="body-alias"?Word(bad.InstanceBuffer,first+4):Hash("sMc"));
            else if(mode=="behavior-alias")Put(bad.InstanceBuffer,second+4,Word(bad.InstanceBuffer,first+4));
            else Put(bad.InstanceBuffer,second+52,Hash("MissingLocalModule"));
            Refuse(()=>Ep1GameObjectDestructionProjection.Compose(bad));nativeFaults++;
        }
        // Reborn: an omitted optional local link is zero, whereas pipeline-only list IDs are not ModuleData declarations in this selected resolver.
        var omitted=Source(false,0,"Body");((XmlElement)omitted.GetElementsByTagName("BuildingDestructionBehavior",SchemaSet.XmlNamespace)[0]!).RemoveAttribute("AssociatedModule");
        var plain=GameObjectBodyProjectionSmokeTest.Compile(omitted,selectedBehaviors:true);Ep1GameObjectDestructionProjection.Compose(plain);cases++;
        Console.WriteLine($"Selected module identity: PASS ({cases} native/token cases, all six behavior orders, four local targets and case folding; {sourceFaults} source and {nativeFaults} raw native refusals; no asset imports/general local-ID or full object publication claim).");
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: root and nonmodule pipeline IDs intentionally reuse a module name to prove the resolver does not collect every XML id indiscriminately. */
    //-------------------------------------------------------------------------------------------------
    private static XmlDocument Source(bool reverse,int position,string target)
    {
        string destruction=$"<BuildingDestructionBehavior id='Destroy' AssociatedModule='{target}'><DieMuxData/><Item/></BuildingDestructionBehavior>";
        string weapon="<WeaponSetUpdate id='Shooter'/>";var modules=reverse?new List<string>{weapon,destruction}:new List<string>{destruction,weapon};modules.Insert(position,"<ObjectSMCHelper id='Smc'/>");
        var d=new XmlDocument{XmlResolver=null};d.LoadXml("<GameObject xmlns='uri:ea.com:eala:asset' id='Smc'><LocomotorSet id='Body'/><Behaviors>"+string.Concat(modules)+"</Behaviors><Body><ActiveBody id='Body' MaxHealth='2000'/></Body></GameObject>");return d;
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: engine module names use lowercase hashes rather than case-sensitive XML spelling. */
    //-------------------------------------------------------------------------------------------------
    private static uint Hash(string name)=>FastHash.GetHashCode(name.ToLowerInvariant());
    //-------------------------------------------------------------------------------------------------
    /** Reborn: expected refusal must be the selected scope gate, not an unrelated access violation or parser failure. */
    //-------------------------------------------------------------------------------------------------
    private static void Refuse(Action a){try{a();}catch(InvalidDataException){return;}throw new InvalidDataException("Invalid selected module scope admitted.");}
    //-------------------------------------------------------------------------------------------------
    /** Reborn: corruption controls own each mutable buffer. */
    //-------------------------------------------------------------------------------------------------
    private static Chunk Clone(Chunk c)=>new(){InstanceBuffer=c.InstanceBuffer.ToArray(),RelocationBuffer=c.RelocationBuffer.ToArray(),ImportsBuffer=c.ImportsBuffer.ToArray()};
    //-------------------------------------------------------------------------------------------------
    /** Reborn: identity/pointer words remain little-endian bounded data. */
    //-------------------------------------------------------------------------------------------------
    private static uint Word(byte[] b,int at)=>BinaryPrimitives.ReadUInt32LittleEndian(b.AsSpan(at,4));
    //-------------------------------------------------------------------------------------------------
    /** Reborn: mutations change owned diagnostic words only. */
    //-------------------------------------------------------------------------------------------------
    private static void Put(byte[] b,int at,uint value)=>BinaryPrimitives.WriteUInt32LittleEndian(b.AsSpan(at,4),value);
}
