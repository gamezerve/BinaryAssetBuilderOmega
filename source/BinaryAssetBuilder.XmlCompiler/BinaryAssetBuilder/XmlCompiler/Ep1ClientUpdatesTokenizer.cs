using System.Buffers.Binary;
using System.IO;
using Chunk=Relo.Chunk;

namespace BinaryAssetBuilder.XmlCompiler;

// Reborn: reviewed ClientUpdates may mix group audio and EVA readiness leaves under one canonical source-sized list.
public static class Ep1ClientUpdatesTokenizer
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: reuse the validated list composer and select only explicitly reviewed concrete client-update writers. */
    //-------------------------------------------------------------------------------------------------
    public static Chunk Tokenize(Chunk native)=>Ep1DrawsTokenizer.TokenizeSelected(native,Leaf);

    //-------------------------------------------------------------------------------------------------
    /** Reborn: unknown client classes fail instead of inheriting the group-audio layout. */
    //-------------------------------------------------------------------------------------------------
    private static Chunk Leaf(Chunk native)
    {
        if(native?.InstanceBuffer==null||native.InstanceBuffer.Length<8)throw new InvalidDataException("Complete client update leaf required.");
        return BinaryPrimitives.ReadUInt32LittleEndian(native.InstanceBuffer) switch{0x9826A297u=>Ep1GroupAudioClientTokenizer.Tokenize(native),0xA220E7D6u=>Ep1EvaReadyClientTokenizer.Tokenize(native),_=>throw new InvalidDataException("Unreviewed client update native class.")};
    }
}
