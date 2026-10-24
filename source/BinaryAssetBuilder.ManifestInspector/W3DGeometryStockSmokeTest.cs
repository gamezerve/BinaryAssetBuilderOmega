using System.Buffers.Binary;
using System.Runtime.InteropServices;
using System.Text;
using SageBinaryData;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: compare emitted model semantics with original EP1 graphs; exporter-rounded float text is compared within its precision, not byte-for-byte.
internal static class W3DGeometryStockSmokeTest
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: read only selected model chunks, proving hierarchy hashes, collision fields, mesh vertices/indices/declarations/skin maps and shader imports. */
    //-------------------------------------------------------------------------------------------------
    internal static void Run(string stockPath,string builtPath)
    {
        var stock=ManifestReader.Read(SdkEnvironmentPreflight.Read(stockPath,64*1048576));var built=ManifestReader.Read(SdkEnvironmentPreflight.Read(builtPath,64*1048576));int models=0,vertices=0,triangles=0;
        foreach(var asset in built.Assets.Where(a=>a.TypeName is "W3DHierarchy" or "W3DCollisionBox" or "W3DMesh")){
            var original=stock.Assets.Single(a=>a.TypeId==asset.TypeId&&a.InstanceId==asset.InstanceId);if(asset.TypeHash!=original.TypeHash||asset.Tokenized!=original.Tokenized)throw new InvalidDataException("Model fingerprint differs: "+asset.Name);
            var a=W3DContainerCompiler.ReadStock(stockPath,stock,original);var b=W3DContainerCompiler.ReadStock(builtPath,built,asset);
            if(asset.TypeName=="W3DHierarchy"){
                int count=checked((int)W(a,4));if(W(b,4)!=count||a.Length!=12+count*100||b.Length!=12+count*100)throw new InvalidDataException("EP1 hierarchy pivot ABI differs.");int pa=(int)W(a,8),pb=(int)W(b,8);
                for(int i=0;i<count;i++){int x=pa+i*100,y=pb+i*100;Equal(W(a,x),W(b,y),"pivot Name hash");Equal(W(a,x+4),W(b,y+4),"pivot Parent");for(int j=8;j<100;j+=4)Float(a,x+j,b,y+j);}
            }else if(asset.TypeName=="W3DCollisionBox"){
                if(a.Length!=32||b.Length!=32||a[28]!=b[28])throw new InvalidDataException("Collision box ABI/flag differs.");for(int i=4;i<28;i+=4)Float(a,i,b,i);
            }else{
                for(int i=12;i<52;i+=4)Float(a,i,b,i);Equal(W(a,8),W(b,8),"mesh geometry type");
                foreach(string flag in new[]{nameof(W3DMesh.Hidden),nameof(W3DMesh.CastShadow),nameof(W3DMesh.SortLevel)}){int offset=Marshal.OffsetOf<W3DMesh>(flag).ToInt32();Equal(a[offset],b[offset],flag);}
                int va=(int)W(a,4),vb=(int)W(b,4),n=(int)W(a,va),stride=(int)W(a,va+4);Equal((uint)n,W(b,vb),"vertex count");Equal((uint)stride,W(b,vb+4),"vertex stride");int da=(int)W(a,va+16),db=(int)W(b,vb+16),decl=(int)W(a,va+12);Equal((uint)decl,W(b,vb+12),"declaration length");
                if(!a.AsSpan(da,decl).SequenceEqual(b.AsSpan(db,decl)))throw new InvalidDataException("Stock PC vertex declaration differs.");int pa=(int)W(a,va+8),pb=(int)W(b,vb+8);
                foreach(var channel in W3DGeometryCompiler.Channels(a,da,decl)){int offset=channel.Offset,floats=channel.Type<4?channel.Type+1:0;for(int i=0;i<n;i++){int x=pa+i*stride+offset,y=pb+i*stride+offset;if(floats==0)Equal(W(a,x),W(b,y),"packed vertex color/bones");else for(int j=0;j<floats;j++)Float(a,x+j*4,b,y+j*4);}}
                int bones=(int)W(a,va+20);Equal((uint)bones,W(b,vb+20),"bone map count");if(bones>0&&!a.AsSpan((int)W(a,va+24),bones*2).SequenceEqual(b.AsSpan((int)W(b,vb+24),bones*2)))throw new InvalidDataException("Stock mesh bone map differs.");
                int field=Marshal.OffsetOf<W3DMesh>(nameof(W3DMesh.Triangles)).ToInt32(),nt=(int)W(a,field),ta=(int)W(a,field+4),tb=(int)W(b,field+4);Equal((uint)nt,W(b,field),"triangle count");
                for(int i=0;i<nt;i++){int x=ta+i*24,y=tb+i*24;Equal(W(a,x),W(b,y),"triangle index count");int count=(int)W(a,x);if(!a.AsSpan((int)W(a,x+4),count*4).SequenceEqual(b.AsSpan((int)W(b,y+4),count*4)))throw new InvalidDataException("Stock triangle indices differ.");for(int j=8;j<24;j+=4)Float(a,x+j,b,y+j);}
                Shader(a,b,Marshal.OffsetOf<W3DMesh>(nameof(W3DMesh.FXShader)).ToInt32(),original.References,asset.References);vertices+=n;triangles+=nt;
            }
            models++;Console.WriteLine("Reborn: EP1 model stock comparison: "+asset.Name+" PASS.");
        }
        if(models==0)throw new InvalidDataException("No model assets selected for stock comparison.");Console.WriteLine("Reborn: EP1 model stock comparison: PASS ("+models+" models, "+vertices+" vertices, "+triangles+" triangles; rounded-source float tolerance only, typed imports exact).");
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: shader strings, class IDs and referenced texture identities must agree even when heap offsets/reference occurrence order differ. */
    //-------------------------------------------------------------------------------------------------
    private static void Shader(byte[] a,byte[] b,int field,IReadOnlyList<AssetId> ar,IReadOnlyList<AssetId> br)
    {
        if(Text(a,field)!=Text(b,field)||Text(a,field+8)!=Text(b,field+8)||a[field+24]!=b[field+24])throw new InvalidDataException("Stock mesh shader selection differs.");int count=(int)W(a,field+16);Equal((uint)count,W(b,field+16),"shader constant count");int pa=(int)W(a,field+20),pb=(int)W(b,field+20);
        for(int i=0;i<count;i++){int x=(int)W(a,pa+i*4),y=(int)W(b,pb+i*4);uint type=W(a,x);Equal(type,W(b,y),"shader constant class");if(Text(a,x+4)!=Text(b,y+4))throw new InvalidDataException("Stock shader constant Name differs.");
            if(type==0xA59096A6){var oldId=ar[(int)W(a,x+12)-1];var newId=br[(int)W(b,y+12)-1];if(oldId!=newId)throw new InvalidDataException("Stock shader texture identity differs.");}
            else if(type==0xDF08DF25){int n=(int)W(a,x+12);Equal((uint)n,W(b,y+12),"shader float vector count");int fa=(int)W(a,x+16),fb=(int)W(b,y+16);for(int j=0;j<n;j++)Float(a,fa+j*4,b,fb+j*4);}
            else if(type==0x89181982)Equal(W(a,x+12),W(b,y+12),"shader Int");else if(type==0xA3F84C3D)Equal(a[x+12],b[y+12],"shader Bool");else throw new InvalidDataException("Unknown stock shader constant class.");
        }
    }

    // Reborn: stock and W3X-exported float32 values can differ by the six-decimal text export precision.
    private static void Float(byte[] a,int x,byte[] b,int y){float oldValue=BitConverter.UInt32BitsToSingle(W(a,x)),newValue=BitConverter.UInt32BitsToSingle(W(b,y));if(!float.IsFinite(oldValue)||!float.IsFinite(newValue)||MathF.Abs(oldValue-newValue)>1e-5f+MathF.Abs(oldValue)*1e-6f)throw new InvalidDataException("Stock model float differs at "+x+"/"+y+": "+oldValue+" vs "+newValue);}
    // Reborn: native strings are compared by bytes, never by their allocation addresses.
    private static string Text(byte[] b,int at)=>Encoding.Latin1.GetString(b,(int)W(b,at+4),(int)W(b,at));
    // Reborn: identifiers, counts and packed data have no numeric tolerance.
    private static void Equal(uint a,uint b,string field){if(a!=b)throw new InvalidDataException("Stock model "+field+" differs: "+a+" vs "+b);}
    // Reborn: every native word read is bounded to a selected asset chunk.
    private static uint W(byte[] b,int at){if(at<0||at>b.Length-4)throw new InvalidDataException("Stock model word out of bounds.");return BinaryPrimitives.ReadUInt32LittleEndian(b.AsSpan(at,4));}
}
