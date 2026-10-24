using Relo;
using SageBinaryData;
using System;
using System.Runtime.InteropServices;
using ShortDict = System.Collections.Generic.Dictionary<ushort, ushort>;
using ShortList = System.Collections.Generic.List<ushort>;

public enum UsageType
{
    Vertex,
    Normal,
    Tangent,
    Binormal,
    TexCoord,
    VertexColor,
    BlendIndex,
    BlendWeight
}

public class ConverterData
{
    public ShortDict InverseBoneMappingTable;
    public Tracker Tracker;
}

public class DataMapping
{
    public UsageType UsageType;
    public int UsageIndex;
    public unsafe void* SrcData;
    public int SrcDeltaToNextElement;
    public int Offset;
}

[StructLayout(LayoutKind.Sequential)]
public struct XMHALF2
{
    public ushort X;
    public ushort Y;
}

[StructLayout(LayoutKind.Sequential)]
public struct XMHALF4
{
    public ushort X;
    public ushort Y;
    public ushort Z;
    public ushort W;
}

public enum VertexElementType : byte
{
    FLOAT1,
    FLOAT2,
    FLOAT3,
    FLOAT4,
    D3DCOLOR,
    UBYTE4,
    SHORT2,
    SHORT4,
    UBYTE4N,
    SHORT2N,
    SHORT4N,
    USHORT2N,
    USHORT4N,
    UDEC3,
    DEC3N,
    FLOAT16_2,
    FLOAT16_4,
    UNUSED
}

public enum VertexElementMethod : byte
{
    DEFAULT,
    PARTIALU,
    PARTIALV,
    CROSSUV,
    UV,
    LOOKUP,
    LOOKUPPRESAMPLED
}

public enum VertexElementUsage : byte
{
    POSITION,
    BLENDWEIGHT,
    BLENDINDICES,
    NORMAL,
    PSIZE,
    TEXCOORD,
    TANGENT,
    BINORMAL,
    TESSFACTOR,
    POSITIONT,
    COLOR,
    FOG,
    DEPTH,
    SAMPLE
}

[StructLayout(LayoutKind.Sequential)]
public struct D3DVertexElement9
{
    public short Stream;
    public short Offset;
    public VertexElementType Type;
    public VertexElementMethod Method;
    public VertexElementUsage Usage;
    public byte UsageIndex;
}

internal class W3DMeshProcessor : IDisposable
{
    // Reborn: EP1 stores ASCII vertex declarations; old callers retain the binary D3D9 declaration by default.
    internal bool UseEp1VertexDeclaration { get; set; }
    private unsafe RGBAColor* _defaultColor;

    public unsafe W3DMeshProcessor()
    {
        _defaultColor = (RGBAColor*)Marshal.AllocHGlobal(sizeof(RGBAColor));
        _defaultColor->Base.R = 1.0f;
        _defaultColor->Base.G = 1.0f;
        _defaultColor->Base.B = 1.0f;
        _defaultColor->A = 1.0f;
    }

    private static ushort CompressFloatToHalf(uint f)
    {
        uint mask1 = f & int.MaxValue;
        uint mask2 = (uint)(f & int.MinValue) >> 16;
        if (mask1 > 0x47FFEFFFu)
        {
            return (ushort)((int)mask2 | short.MaxValue);
        }
        uint val;
        if (mask1 < 0x38800000u)
        {
            val = (uint)(((int)mask1 & 0x7FFFFF) | 0x800000) >> (int)(113 - (mask1 >> 23));
        }
        else
        {
            val = mask1 + 0xC8000000;
        }
        return (ushort)((int)mask2 | (int)((uint)((int)val + 0xFFF + ((int)(val >> 13) & 1)) >> 13));
    }

    private static unsafe void CopyDirect<T>(int numElements, sbyte* srcData, int srcPitch, sbyte* destData, int destPitch, ConverterData converterData) where T : unmanaged
    {
        for (; numElements != 0; --numElements)
        {
            Native.MsVcRt.MemCpy((IntPtr)destData, (IntPtr)srcData, new Native.SizeT(sizeof(T)));
            srcData += srcPitch;
            destData += destPitch;
        }
    }

    private static unsafe void CopyTexCoord(int numElements, sbyte* srcData, int srcPitch, sbyte* destData, int destPitch, ConverterData converterData)
    {
        for (; numElements != 0; --numElements)
        {
            Vector2* destVec = (Vector2*)destData;
            Vector2* srcVec = (Vector2*)srcData;
            float num = srcVec->X;
            if (converterData.Tracker.IsBigEndian)
            {
                converterData.Tracker.InplaceEndianToPlatform((uint*)&num);
            }
            destVec->X = num;
            num = 1.0f - srcVec->Y;
            if (converterData.Tracker.IsBigEndian)
            {
                converterData.Tracker.InplaceEndianToPlatform((uint*)&num);
            }
            destVec->Y = num;
            srcData += srcPitch;
            destData += destPitch;
        }
    }

    private static unsafe void CompressTexCoord(int numElements, sbyte* srcData, int srcPitch, sbyte* destData, int destPitch, ConverterData converterData)
    {
        for (; numElements != 0; --numElements)
        {
            XMHALF2* destVec = (XMHALF2*)destData;
            Vector2* srcVec = (Vector2*)srcData;
            float x = srcVec->X;
            float y = 1.0f - srcVec->Y;
            ushort iX = CompressFloatToHalf(*(uint*)&x);
            ushort iY = CompressFloatToHalf(*(uint*)&y);
            if (converterData.Tracker.IsBigEndian)
            {
                converterData.Tracker.InplaceEndianToPlatform(&iX);
                converterData.Tracker.InplaceEndianToPlatform(&iY);
            }
            destVec->X = iX;
            destVec->Y = iY;
            srcData += srcPitch;
            destData += destPitch;
        }
    }

    private static unsafe void Vector4ToD3DCOLOR(int numElements, sbyte* srcData, int srcPitch, sbyte* destData, int destPitch, ConverterData converterData)
    {
        for (; numElements != 0; --numElements)
        {
            uint* color = (uint*)destData;
            Vector4* srcVec = (Vector4*)srcData;
            uint result = ((uint)((double)srcVec->W * byte.MaxValue) << 24)
                        | ((uint)((double)srcVec->X * byte.MaxValue) << 16)
                        | ((uint)((double)srcVec->Y * byte.MaxValue) << 8)
                        | (uint)((double)srcVec->Z * byte.MaxValue);
            if (converterData.Tracker.IsBigEndian)
            {
                converterData.Tracker.InplaceEndianToPlatform(&result);
            }
            *color = result;
            srcData += srcPitch;
            destData += destPitch;
        }
    }

    private static unsafe void Vector3ToHalf4(int numElements, sbyte* srcData, int srcPitch, sbyte* destData, int destPitch, ConverterData converterData)
    {
        for (; numElements != 0; --numElements)
        {
            XMHALF4* destVec = (XMHALF4*)destData;
            Vector3* srcVec = (Vector3*)srcData;
            float x = srcVec->X;
            float y = srcVec->Y;
            float z = srcVec->Z;
            float w = 0.0f;
            ushort iX = CompressFloatToHalf(*(uint*)&x);
            ushort iY = CompressFloatToHalf(*(uint*)&y);
            ushort iZ = CompressFloatToHalf(*(uint*)&z);
            ushort iW = CompressFloatToHalf(*(uint*)&w);
            if (converterData.Tracker.IsBigEndian)
            {
                converterData.Tracker.InplaceEndianToPlatform(&iX);
                converterData.Tracker.InplaceEndianToPlatform(&iY);
                converterData.Tracker.InplaceEndianToPlatform(&iZ);
                converterData.Tracker.InplaceEndianToPlatform(&iW);
            }
            destVec->X = iX;
            destVec->Y = iY;
            destVec->Z = iZ;
            destVec->W = iW;
            srcData += srcPitch;
            destData += destPitch;
        }
    }

    private static unsafe void FloatToHalf(int numElements, sbyte* srcData, int srcPitch, sbyte* destData, int destPitch, ConverterData converterData)
    {
        for (; numElements != 0; --numElements)
        {
            ushort* dest = (ushort*)destData;
            float src = *(float*)srcData;
            ushort iSrc = CompressFloatToHalf(*(uint*)&src);
            if (converterData.Tracker.IsBigEndian)
            {
                converterData.Tracker.InplaceEndianToPlatform(&iSrc);
            }
            *dest = iSrc;
            srcData += srcPitch;
            destData += destPitch;
        }
    }

    private static unsafe void ConvertBlendIndex(int numElements, sbyte* srcData, int srcPitch, sbyte* destData, int destPitch, ConverterData converterData)
    {
        for (; numElements != 0; --numElements)
        {
            ushort src = *(ushort*)srcData;
            *destData = (sbyte)converterData.InverseBoneMappingTable[src];
            srcData += srcPitch;
            destData += destPitch;
        }
    }

    private static unsafe void CompressNormalizedDirection(int numElements, sbyte* srcData, int srcPitch, sbyte* destData, int destPitch, ConverterData converterData)
    {
        for (; numElements != 0; --numElements)
        {
            // TODO:
            srcData += srcPitch;
            destData += destPitch;
        }
    }

    private unsafe void DetermineVertexDataContents(W3DMeshPipelineVertexData* pipelineData, System.Collections.Generic.List<DataMapping> mappings)
    {
        mappings.Clear();
        for (int idx = 0; idx < pipelineData->Vertices.Count; ++idx)
        {
            mappings.Add(new DataMapping
            {
                UsageType = UsageType.Vertex,
                UsageIndex = idx,
                SrcData = pipelineData->Vertices.Items[idx].V.Items,
                SrcDeltaToNextElement = sizeof(Vector3),
                Offset = 0
            });
        }
        for (int idx = 0; idx < pipelineData->Normals.Count; ++idx)
        {
            mappings.Add(new DataMapping
            {
                UsageType = UsageType.Normal,
                UsageIndex = idx,
                SrcData = pipelineData->Normals.Items[idx].N.Items,
                SrcDeltaToNextElement = sizeof(Vector3),
                Offset = 0
            });
        }
        if ((IntPtr)pipelineData->VertexColors != IntPtr.Zero)
        {
            mappings.Add(new DataMapping
            {
                UsageType = UsageType.VertexColor,
                UsageIndex = 0,
                SrcData = pipelineData->VertexColors->C.Items,
                SrcDeltaToNextElement = sizeof(Vector4),
                Offset = 0
            });
        }
        else
        {
            mappings.Add(new DataMapping
            {
                UsageType = UsageType.VertexColor,
                UsageIndex = 0,
                SrcData = _defaultColor,
                SrcDeltaToNextElement = 0,
                Offset = 0
            });
        }
        if ((IntPtr)pipelineData->Tangents != IntPtr.Zero && (IntPtr)pipelineData->Binormals != IntPtr.Zero)
        {
            mappings.Add(new DataMapping
            {
                UsageType = UsageType.Tangent,
                UsageIndex = 0,
                SrcData = pipelineData->Tangents->T.Items,
                SrcDeltaToNextElement = sizeof(Vector3),
                Offset = 0
            });
            mappings.Add(new DataMapping
            {
                UsageType = UsageType.Binormal,
                UsageIndex = 0,
                SrcData = pipelineData->Binormals->B.Items,
                SrcDeltaToNextElement = sizeof(Vector3),
                Offset = 0
            });
        }
        for (int idx = 0; idx < pipelineData->TexCoords.Count; ++idx)
        {
            mappings.Add(new DataMapping
            {
                UsageType = UsageType.TexCoord,
                UsageIndex = idx,
                SrcData = pipelineData->TexCoords.Items[idx].T.Items,
                SrcDeltaToNextElement = sizeof(Vector2),
                Offset = 0
            });
        }
        // Reborn: skin influence streams belong to each vertex, independently of the number of position/morph streams.
        int num = (int)pipelineData->BoneInfluences.Count;
        for (int idx = 0; idx < num; ++idx)
        {
            mappings.Add(new DataMapping
            {
                UsageType = UsageType.BlendIndex,
                UsageIndex = idx,
                SrcData = &pipelineData->BoneInfluences.Items[idx].I.Items->Bone,
                SrcDeltaToNextElement = sizeof(BoneInfluence),
                Offset = 0
            });
        }
        if (num <= 1)
        {
            return;
        }
        for (int idx = 0; idx < num; ++idx)
        {
            mappings.Add(new DataMapping
            {
                UsageType = UsageType.BlendWeight,
                UsageIndex = idx,
                SrcData = &pipelineData->BoneInfluences.Items[idx].I.Items->Weight,
                SrcDeltaToNextElement = sizeof(BoneInfluence),
                Offset = 0
            });
        }
    }

    private static unsafe int DetermineVertexDataLayout(System.Collections.Generic.List<DataMapping> mappings, Tracker state, bool ep1)
    {
        int result = 0;
        foreach (DataMapping mapping in mappings)
        {
            mapping.Offset = result;
            switch (mapping.UsageType)
            {
                case UsageType.Vertex:
                case UsageType.Normal:
                case UsageType.Tangent:
                case UsageType.Binormal:
                    result += sizeof(Vector3);
                    break;
                case UsageType.TexCoord:
                    result += sizeof(Vector2);
                    break;
                case UsageType.VertexColor:
                    result += sizeof(uint);
                    break;
                case UsageType.BlendIndex:
                    if (mapping.UsageIndex == 0)
                    {
                        result += sizeof(uint);
                    }
                    else
                    {
                        mapping.Offset -= 4 - mapping.UsageIndex;
                    }
                    break;
                case UsageType.BlendWeight:
                    // Reborn: EP1 skin weights share one normalized four-byte channel, unlike the legacy float-per-influence layout.
                    if(!ep1)result += sizeof(float);
                    else if(mapping.UsageIndex==0)result+=4;
                    else mapping.Offset-=4-mapping.UsageIndex;
                    break;
            }
        }
        result = (result + 3) & -4;
        return result;
    }

    private static unsafe void ComputeBoneMappingTable(W3DMeshPipelineVertexData* pipelineData, ShortList boneMappingTable, ShortDict inverseBoneMappingTable)
    {
        boneMappingTable.Clear();
        inverseBoneMappingTable.Clear();
        System.Collections.Generic.SortedSet<ushort> set = new System.Collections.Generic.SortedSet<ushort>();
        for (int idx = 0; idx < pipelineData->BoneInfluences.Count; ++idx)
        {

            List<BoneInfluence>* bones = &pipelineData->BoneInfluences.Items[idx].I;
            for (int idy = 0; idy < bones->Count; ++idy)
            {
                ushort bone = bones->Items[idy].Bone;
                set.Add(bone);
            }
        }
        boneMappingTable.AddRange(set);
        for (int idx = 0; idx < boneMappingTable.Count; ++idx)
        {
            inverseBoneMappingTable[boneMappingTable[idx]] = (ushort)idx;
        }
    }

    private static unsafe void BuildVertexDeclaration_Windows(System.Collections.Generic.List<DataMapping> mappings, List<byte>* declaration, Tracker state)
    {
        System.Collections.Generic.List<D3DVertexElement9> elements = new System.Collections.Generic.List<D3DVertexElement9>();
        foreach (DataMapping mapping in mappings)
        {
            if (mapping.UsageType == UsageType.BlendIndex && mapping.UsageIndex >= 1)
            {
                continue;
            }
            if (mapping.UsageType == UsageType.BlendWeight && mapping.UsageIndex >= 1)
            {
                D3DVertexElement9 last = elements[^1];
                // Reborn: a multi-component weight declaration grows FLOAT1..FLOAT4; its BLENDWEIGHT semantic stays unchanged.
                last.Type = (VertexElementType)((byte)last.Type + 1);
                elements[^1] = last;
                continue;
            }
            D3DVertexElement9 vertexElement = new D3DVertexElement9();
            vertexElement.Usage = mapping.UsageType switch
            {
                UsageType.Vertex => VertexElementUsage.POSITION,
                UsageType.Normal => VertexElementUsage.NORMAL,
                UsageType.Tangent => VertexElementUsage.TANGENT,
                UsageType.Binormal => VertexElementUsage.BINORMAL,
                UsageType.TexCoord => VertexElementUsage.TEXCOORD,
                UsageType.VertexColor => VertexElementUsage.COLOR,
                UsageType.BlendIndex => VertexElementUsage.BLENDINDICES,
                UsageType.BlendWeight => VertexElementUsage.BLENDWEIGHT,
                _ => throw new ArgumentOutOfRangeException(nameof(vertexElement.Usage))
            };
            vertexElement.UsageIndex = (byte)mapping.UsageIndex;
            vertexElement.Type = mapping.UsageType switch
            {
                UsageType.Vertex => VertexElementType.FLOAT3,
                UsageType.Normal => VertexElementType.FLOAT3,
                UsageType.Tangent => VertexElementType.FLOAT3,
                UsageType.Binormal => VertexElementType.FLOAT3,
                UsageType.TexCoord => VertexElementType.FLOAT2,
                UsageType.VertexColor => VertexElementType.D3DCOLOR,
                UsageType.BlendIndex => VertexElementType.D3DCOLOR,
                UsageType.BlendWeight => VertexElementType.FLOAT1,
                _ => throw new ArgumentOutOfRangeException(nameof(vertexElement.Usage))
            };
            vertexElement.Offset = (short)mapping.Offset;
            elements.Add(vertexElement);
        }
        elements.Add(new D3DVertexElement9
        {
            Stream = byte.MaxValue,
            Type = VertexElementType.UNUSED
        });
        declaration->Count = (uint)(elements.Count * sizeof(D3DVertexElement9));
        using Tracker.Context context = state.Push((void**)&declaration->Items, 1u, declaration->Count);
        byte* dest = declaration->Items;
        foreach (D3DVertexElement9 vertexElement in elements)
        {
            D3DVertexElement9* ptr = &vertexElement;
            Native.MsVcRt.MemCpy((IntPtr)dest, (IntPtr)ptr, new Native.SizeT(sizeof(D3DVertexElement9)));
            dest += sizeof(D3DVertexElement9);
        }
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: EP1 PC declarations match stock ASCII descriptors such as p0:00:3f32 t0:34:2f32 i0:44:4u8. */
    //-------------------------------------------------------------------------------------------------
    private static unsafe void BuildVertexDeclaration_Ep1(System.Collections.Generic.List<DataMapping> mappings, List<byte>* declaration, Tracker state)
    {
        var fields=new System.Collections.Generic.List<string>();
        foreach(var mapping in mappings){
            if((mapping.UsageType==UsageType.BlendIndex||mapping.UsageType==UsageType.BlendWeight)&&mapping.UsageIndex>0)continue;
            string semantic=mapping.UsageType switch{UsageType.Vertex=>"p",UsageType.Normal=>"n",UsageType.Tangent=>"g",UsageType.Binormal=>"b",UsageType.TexCoord=>"t",UsageType.VertexColor=>"c",UsageType.BlendIndex=>"i",UsageType.BlendWeight=>"w",_=>throw new InvalidOperationException("Unknown EP1 vertex channel.")};
            string format=mapping.UsageType switch{UsageType.Vertex or UsageType.Normal or UsageType.Tangent or UsageType.Binormal=>"3f32",UsageType.TexCoord=>"2f32",UsageType.VertexColor=>"4u8n",UsageType.BlendIndex=>"4u8",UsageType.BlendWeight=>"4u8n",_=>throw new InvalidOperationException("Unknown EP1 vertex format.")};
            fields.Add(semantic+mapping.UsageIndex.ToString(System.Globalization.CultureInfo.InvariantCulture)+":"+mapping.Offset.ToString("X2",System.Globalization.CultureInfo.InvariantCulture)+":"+format);
        }
        byte[] bytes=System.Text.Encoding.ASCII.GetBytes(string.Join(" ",fields));declaration->Count=(uint)bytes.Length;
        using Tracker.Context context=state.Push((void**)&declaration->Items,1u,(uint)bytes.Length);for(int i=0;i<bytes.Length;i++)declaration->Items[i]=bytes[i];
    }

    private static unsafe void BuildVertexDeclaration_Xenon(System.Collections.Generic.List<DataMapping> mappings, List<byte>* declaration, Tracker state)
    {
        throw new NotImplementedException();
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: quantize up to four normalized skin influences to EP1's 4u8n channel, distributing rounding remainder deterministically. */
    //-------------------------------------------------------------------------------------------------
    private static unsafe void WriteEp1Weights(W3DMeshPipelineVertexData* input,int vertices,byte* target,int stride)
    {
        int count=(int)input->BoneInfluences.Count;var fractions=new double[4];var values=new int[4];
        for(int vertex=0;vertex<vertices;vertex++){
            double sum=0;for(int j=0;j<count;j++){float value=input->BoneInfluences.Items[j].I.Items[vertex].Weight;if(!float.IsFinite(value)||value<0)throw new InvalidOperationException("Invalid skin weight.");sum+=value;}if(sum<=0)throw new InvalidOperationException("Skin weights have zero total.");
            int total=0;for(int j=0;j<count;j++){double exact=input->BoneInfluences.Items[j].I.Items[vertex].Weight/sum*255;values[j]=(int)Math.Floor(exact);fractions[j]=exact-values[j];total+=values[j];}
            while(total<255){int selected=0;for(int j=1;j<count;j++)if(fractions[j]>fractions[selected])selected=j;values[selected]++;fractions[selected]=-1;total++;}
            for(int j=0;j<4;j++)target[vertex*stride+j]=j<count?(byte)values[j]:(byte)0;
        }
    }

    public unsafe void BuildVertexData(W3DMeshPipelineVertexData* pipelineData, W3DMeshVertexData* outputData, Tracker state)
    {
        System.Collections.Generic.List<DataMapping> mappings = new System.Collections.Generic.List<DataMapping>();
        DetermineVertexDataContents(pipelineData, mappings);
        ConverterData converterData = new ConverterData
        {
            InverseBoneMappingTable = new ShortDict(),
            Tracker = state
        };
        int vertexDataSize = DetermineVertexDataLayout(mappings, state, UseEp1VertexDeclaration);
        ShortList boneMap = new ShortList();
        ComputeBoneMappingTable(pipelineData, boneMap, converterData.InverseBoneMappingTable);
        int numVertices = (int)pipelineData->Vertices.Items->V.Count;
        int numVerticesOut = numVertices;
        uint binaryVertexDataSize = (uint)(numVertices * vertexDataSize);
        if (state.IsBigEndian)
        {
            state.InplaceEndianToPlatform((uint*)&numVerticesOut);
            state.InplaceEndianToPlatform((uint*)&vertexDataSize);
        }
        outputData->NumVertices = numVerticesOut;
        outputData->VertexElementData.Count = (uint)vertexDataSize;
        using (Tracker.Context context = state.Push((void**)&outputData->VertexElementData.Items, 1u, binaryVertexDataSize))
        {
            foreach (DataMapping mapping in mappings)
            {
                switch (mapping.UsageType)
                {
                    case UsageType.Vertex:
                    case UsageType.Normal:
                    case UsageType.Tangent:
                    case UsageType.Binormal:
                        CopyDirect<Vector3>(numVertices,
                                            (sbyte*)mapping.SrcData,
                                            mapping.SrcDeltaToNextElement,
                                            (sbyte*)outputData->VertexElementData.Items + mapping.Offset,
                                            vertexDataSize,
                                            converterData);
                        break;
                    case UsageType.TexCoord:
                        CopyTexCoord(numVertices,
                                     (sbyte*)mapping.SrcData,
                                     mapping.SrcDeltaToNextElement,
                                     (sbyte*)outputData->VertexElementData.Items + mapping.Offset,
                                     vertexDataSize,
                                     converterData);
                        break;
                    case UsageType.VertexColor:
                        Vector4ToD3DCOLOR(numVertices,
                                          (sbyte*)mapping.SrcData,
                                          mapping.SrcDeltaToNextElement,
                                          (sbyte*)outputData->VertexElementData.Items + mapping.Offset,
                                          vertexDataSize,
                                          converterData);
                        break;
                    case UsageType.BlendIndex:
                        ConvertBlendIndex(numVertices,
                                          (sbyte*)mapping.SrcData,
                                          mapping.SrcDeltaToNextElement,
                                          (sbyte*)outputData->VertexElementData.Items + mapping.Offset,
                                          vertexDataSize,
                                          converterData);
                        break;
                    case UsageType.BlendWeight:
                        // Reborn: the first EP1 mapping writes all normalized influences together, preserving a byte sum of 255.
                        if(UseEp1VertexDeclaration){if(mapping.UsageIndex==0)WriteEp1Weights(pipelineData,numVertices,outputData->VertexElementData.Items+mapping.Offset,vertexDataSize);break;}
                        CopyDirect<float>(numVertices,
                                          (sbyte*)mapping.SrcData,
                                          mapping.SrcDeltaToNextElement,
                                          (sbyte*)outputData->VertexElementData.Items + mapping.Offset,
                                          vertexDataSize,
                                          converterData);
                        break;
                }
            }
        }
        if (state.IsBigEndian)
        {
            BuildVertexDeclaration_Xenon(mappings, &outputData->VertexDeclaration, state);
        }
        else
        {
            if(UseEp1VertexDeclaration)BuildVertexDeclaration_Ep1(mappings, &outputData->VertexDeclaration, state);
            else BuildVertexDeclaration_Windows(mappings, &outputData->VertexDeclaration, state);
        }
        uint boneMapCount = (uint)boneMap.Count;
        if (state.IsBigEndian)
        {
            state.InplaceEndianToPlatform(&boneMapCount);
        }
        outputData->Bones.Count = boneMapCount;
        using (Tracker.Context context = state.Push((void**)&outputData->Bones.Items, 2u, (uint)boneMap.Count))
        {
            ushort* boneData = outputData->Bones.Items;
            foreach (ushort bone in boneMap)
            {
                if (state.IsBigEndian)
                {
                    state.InplaceEndianToPlatform(&bone);
                }
                *boneData++ = bone;
            }
        }
    }

    public unsafe void Dispose()
    {
        Marshal.FreeHGlobal((IntPtr)_defaultColor);
    }
}
