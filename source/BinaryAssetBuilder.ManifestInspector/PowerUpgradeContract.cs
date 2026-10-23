using System.Buffers.Binary;
using System.Runtime.InteropServices;
using BinaryAssetBuilder.Core.Hashing;
using SageBinaryData;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: legacy PowerUpgrade (the pre-RA3 upgrade that applies the owner's EnergyBonus) has no EP1 stock instance, but its class still
// exists in the game. Its record adds no field to UpgradeModuleData, so the full native/metadata proof borrows the stock-qualified
// UpgradeModuleData base of a real EP1 upgrade module (same ABI, same field/tag grammar) and replaces only the concrete class identity.
// A module id is mandatory (RA3 ignores an anonymous PowerUpgrade); the shared module-identity contract already rejects a blank id.
internal static class PowerUpgradeContract
{
    internal const uint TypeId=0xC3A62C68u;
    internal const int HeaderSize=36;
    private const int AbiEnd=33;
    internal sealed record Witness(byte[] Bytes,GameObjectModuleReview.Module Module,int End,string Basis);

    //-------------------------------------------------------------------------------------------------
    /** Reborn: build a comparison-only stock witness: a copy of the captured object whose basis upgrade module is relabeled as
        PowerUpgradeModuleData and whose own metadata table keeps only the inherited UpgradeModuleData row. Real output is never changed. */
    //-------------------------------------------------------------------------------------------------
    internal static Witness Create(byte[] stock,GameObjectModuleReview.Module[] behaviors,int behaviorsEnd)
    {
        if(FastHash.GetHashCode("PowerUpgradeModuleData")!=TypeId||Marshal.SizeOf<PowerUpgradeModuleData>()!=HeaderSize||Marshal.SizeOf<UpgradeModuleData>()!=HeaderSize||typeof(PowerUpgradeModuleData).GetFields().Length!=1)
            throw new InvalidDataException("PowerUpgrade/UpgradeModuleData ABI differs from the reviewed 36-byte base-only record.");
        var basis=behaviors.FirstOrDefault(m=>m.NativeModelPresent&&m.ClassChain.Length==4&&m.ClassChain[1]=="UpgradeModuleData"&&m.ClassChain[2]=="BehaviorModuleData"&&m.ClassChain[3]=="ModuleData")
            ??throw new InvalidDataException("No stock upgrade module with a direct UpgradeModuleData base is available to qualify PowerUpgrade.");
        int index=Array.IndexOf(behaviors,basis);int end=index+1<behaviors.Length?behaviors[index+1].Wrapper:behaviorsEnd;
        var bytes=stock.ToArray();
        // Reborn: the borrowed base must end at the reviewed ABI boundary; PowerUpgrade's trailing padding is zero.
        for(int i=AbiEnd;i<HeaderSize;i++)if(bytes[basis.Native+i]!=0)throw new InvalidDataException("Stock upgrade basis uses UpgradeModuleData padding; PowerUpgrade cannot borrow it.");
        Put(bytes,basis.Native,TypeId);
        if(Word(bytes,basis.Wrapper)!=2)throw new InvalidDataException("Stock upgrade basis wrapper differs.");
        Put(bytes,basis.Wrapper+4,TypeId);
        int table=(int)Word(bytes,basis.Wrapper+8),parent=-1;
        for(int at=table,row=0;;at+=12,row++){
            if(row>256||at<basis.Wrapper||at>end-12)throw new InvalidDataException("Stock upgrade basis own table bounds differ.");
            uint name=Word(bytes,at),tag=Word(bytes,at+4),target=Word(bytes,at+8);if(name==0&&tag==0&&target==0)break;
            if(name==1){if(tag!=FastHash.GetHashCode("UpgradeModuleData")||parent>=0)throw new InvalidDataException("Stock upgrade basis parent row differs.");parent=at;}
        }
        if(parent<0)throw new InvalidDataException("Stock upgrade basis has no UpgradeModuleData parent row.");
        uint inherited=Word(bytes,parent+8);
        if(inherited>=table&&inherited<table+24)throw new InvalidDataException("Stock upgrade basis inherited table overlaps its own table.");
        Put(bytes,table,1);Put(bytes,table+4,FastHash.GetHashCode("UpgradeModuleData"));Put(bytes,table+8,inherited);
        Put(bytes,table+12,0);Put(bytes,table+16,0);Put(bytes,table+20,0);
        var module=new GameObjectModuleReview.Module(basis.Wrapper,TypeId,"PowerUpgradeModuleData",true,basis.Native,new[]{"PowerUpgradeModuleData","UpgradeModuleData","BehaviorModuleData","ModuleData"});
        return new(bytes,module,end,basis.Type);
    }

    private static uint Word(byte[] b,int at)=>BinaryPrimitives.ReadUInt32LittleEndian(b.AsSpan(at,4));
    private static void Put(byte[] b,int at,uint value)=>BinaryPrimitives.WriteUInt32LittleEndian(b.AsSpan(at,4),value);
}
