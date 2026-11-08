using System.Runtime.InteropServices;
using BinaryAssetBuilder.Core.Hashing;
using SageBinaryData;
namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: these explicit layouts were independently read from the original PC engine factory/tagged-field loaders; they are not stock asset-byte witnesses.
internal static class StockRuntimeAbiContract
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: reject any adapter size, source class hash or field offset which differs from the recorded original engine ABI. */
    //-------------------------------------------------------------------------------------------------
    internal static void Qualify(string name,Type type,uint wire,uint fingerprint)
    {
        if(FastHash.GetHashCode(name)!=wire)throw new InvalidDataException("Original runtime ABI class differs: "+name);
        // Reborn: all Conquer derivatives share the original twelve-byte base and its two integer destinations.
        if(name.StartsWith("ConquerCardDefinition",StringComparison.Ordinal))Check(typeof(ConquerCardDefinitionBase),12,("Base",0),("Cost",4),("Maximum",8));
        switch(name)
        {
            case "ConquerCardDefinitionUnit":
                Check(type,32,("Base",0),("Unit",12),("Upgrade",16),("Veterancy",24));
                Check(typeof(ConquerCardUpgrade),8,("Upgrade",0),("Cost",4));Check(typeof(ConquerCardVeterancy),8,("Veterancy",0),("Cost",4));
                if(fingerprint!=0xDA316C85u)throw new InvalidDataException("Original Conquer unit fingerprint differs.");
                break;
            case "ConquerCardDefinitionStructure":
                Check(type,16,("Base",0),("Structure",12));
                if(fingerprint!=0xCA2996F8u)throw new InvalidDataException("Original Conquer structure fingerprint differs.");
                break;
            case "ConquerCardDefinitionPower":
                Check(type,24,("Base",0),("Power",12),("Side",16));
                if(fingerprint!=0x22FF8476u)throw new InvalidDataException("Original Conquer power fingerprint differs.");
                break;
            case "SiegeEvaEvent":
                Check(type,16,("Base",0),("Seconds",4),("EvaEvent",8),("EventType",12));
                if(fingerprint!=0x1082F8ACu||(int)SiegeEvaEventType.ALL!=2)throw new InvalidDataException("Original Siege event ABI differs.");
                break;
            case "DLContent":
                Check(type,24,("Base",0),("License",4),("Patch",12),("PackageNumber",20));
                Check(typeof(DLLicenseGroup),12,("LicenseMask",0),("Asset",4));Check(typeof(DLPatch),12,("Name",0),("PatchNumber",8));
                if(fingerprint!=0x629BD41Cu)throw new InvalidDataException("Original DLC schema fingerprint differs.");
                break;
            case "BootupDisplaySequence":
                Check(type,16,("Base",0),("LoadingTextTexture",4),("DisplayItem",8));
                Check(typeof(BootupDisplayItem),28,("Type",0),("Duration",4),("Priority",8),("RandomTexture",12),("Movie",20),("ShowOnReboot",24));
                if(fingerprint!=0xC10DBCE4u)throw new InvalidDataException("Original bootup schema fingerprint differs.");
                break;
            case "DistributeEnergyToAlliesBehaviorModuleData":
                Check(type,24,("Base",0),("ComradePercentage",20));
                if(Marshal.SizeOf<CaptureBehaviorModuleData>()!=20||fingerprint!=0x436C1AE7u)throw new InvalidDataException("Original energy-sharing ABI differs.");
                break;
            case "TheVersion":
                Check(type,12,("Base",0),("Version",4));Check(typeof(VersionType),8,("Major",0),("Minor",4));
                if(fingerprint!=0xA5DF846Fu)throw new InvalidDataException("Original version schema fingerprint differs.");
                break;
            case "AIStateStructureOrHarvesterUnderAttackHeuristic":
                Check(type,12,("Base",0),("TimeSinceAttacked",4),("UnderAttack",8));
                if(fingerprint!=0x283098D9u)throw new InvalidDataException("Original under-attack schema fingerprint differs.");
                break;
            default:throw new NotSupportedException("Original engine ABI has no reviewed witness: "+name);
        }
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: exercise nonzero fields, sequence records, strong import slots and string relocation at original runtime ABI destinations; these are writer probes, not retail asset witnesses. */
    //-------------------------------------------------------------------------------------------------
    internal static void WriterSelfTest()
    {
        var schemas=ConscriptSourceInput.Schemas();
        Relo.Chunk Build(string source)
        {
            var xml=SdkXmlDiagnostics.Parse(System.Text.Encoding.UTF8.GetBytes(source));
            return StockAssetRootCompiler.Compile(xml.DocumentElement!,schemas);
        }
        uint Word(Relo.Chunk chunk,int offset)=>System.Buffers.Binary.BinaryPrimitives.ReadUInt32LittleEndian(chunk.InstanceBuffer.AsSpan(offset,4));
        void Require(bool condition){if(!condition)throw new InvalidDataException("Original runtime ABI writer probe differs.");}
        const string ns=" xmlns='uri:ea.com:eala:asset' id='RebornRuntimeAbiProbe'";
        var unit=Build("<ConquerCardDefinitionUnit"+ns+" Cost='13' Maximum='7' Unit='GameObject\\0'><Upgrade Upgrade='UpgradeTemplate\\1' Cost='5'/><Veterancy Veterancy='ELITE' Cost='9'/></ConquerCardDefinitionUnit>");
        int upgrade=checked((int)Word(unit,20)),veterancy=checked((int)Word(unit,28));
        Require(Word(unit,4)==13&&Word(unit,8)==7&&Word(unit,12)==1&&Word(unit,16)==1&&Word(unit,24)==1&&Word(unit,upgrade)==2&&Word(unit,upgrade+4)==5&&Word(unit,veterancy)==2&&Word(unit,veterancy+4)==9&&unit.ImportsBuffer.Length==12);
        var structure=Build("<ConquerCardDefinitionStructure"+ns+" Cost='21' Structure='GameObject\\0'/>");
        Require(structure.InstanceBuffer.Length==16&&Word(structure,4)==21&&Word(structure,8)==0&&structure.ImportsBuffer.Length==8);
        var power=Build("<ConquerCardDefinitionPower"+ns+" Cost='31' Power='SpecialPowerTemplate\\0' Side='Soviet'/>");
        int side=checked((int)Word(power,20));
        Require(Word(power,4)==31&&Word(power,16)==6&&System.Text.Encoding.ASCII.GetString(power.InstanceBuffer,side,6)=="Soviet"&&power.ImportsBuffer.Length==8);
        var siege=Build("<SiegeEvaEvent"+ns+" Seconds='2500ms' EvaEvent='EvaEvent\\0' EventType='GENERIC'/>");
        Require(siege.InstanceBuffer.Length==16&&Word(siege,4)==0x40200000&&Word(siege,12)==1&&siege.ImportsBuffer.Length==8);
        var defaultSiege=Build("<SiegeEvaEvent"+ns+" Seconds='1s' EvaEvent='EvaEvent\\0'/>");Require(Word(defaultSiege,12)==2);
        var dlc=Build("<DLContent"+ns+" PackageNumber='7'><Patch Name='RebornPatch' PatchNumber='3'/></DLContent>");
        int patch=checked((int)Word(dlc,16));
        Require(Word(dlc,12)==1&&dlc.InstanceBuffer[20]==7&&Word(dlc,patch)==11&&dlc.InstanceBuffer[patch+8]==3);
        Console.WriteLine("Reborn: original runtime ABI writer probes passed: unit choices, structure, power string, Siege time/default and DLC patch.");
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: compare every reviewed field and total extent against independent allocation/loader destination observations. */
    //-------------------------------------------------------------------------------------------------
    private static void Check(Type type,int size,params (string Name,int Offset)[] fields)
    {
        if(Marshal.SizeOf(type)!=size||type.GetFields().Count(f=>!f.IsStatic)!=fields.Length)throw new InvalidDataException("Original runtime ABI extent differs: "+type.Name);
        foreach(var field in fields)if(Marshal.OffsetOf(type,field.Name).ToInt32()!=field.Offset)throw new InvalidDataException("Original runtime ABI field differs: "+type.Name+"."+field.Name);
    }
}
