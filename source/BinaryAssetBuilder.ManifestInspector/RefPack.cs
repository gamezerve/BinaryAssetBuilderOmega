using System.Buffers.Binary;

namespace BinaryAssetBuilder.ManifestInspector;

internal static class RefPack
{
    private const int DefaultMaximumOutputSize = 256 * 1024 * 1024;

    public static bool IsCompressed(ReadOnlySpan<byte> input) =>
        input.Length >= 2 && input[1] == 0xFB && (input[0] & 0x1F) == 0x10;

    public static byte[] Decompress(ReadOnlySpan<byte> input, int maximumOutputSize = DefaultMaximumOutputSize)
    {
        if (!IsCompressed(input))
        {
            throw new InvalidDataException("Input does not have a RefPack header.");
        }

        var largeFile = (input[0] & 0x80) != 0;
        var hasCompressedSize = (input[0] & 0x01) != 0;
        var sizeWidth = largeFile ? 4 : 3;
        var position = 2;

        if (hasCompressedSize)
        {
            EnsureAvailable(input, position, sizeWidth);
            position += sizeWidth;
        }

        EnsureAvailable(input, position, sizeWidth);
        var outputSize = ReadBigEndianSize(input.Slice(position, sizeWidth));
        position += sizeWidth;
        if (outputSize > maximumOutputSize)
        {
            throw new InvalidDataException(
                $"RefPack output is {outputSize:N0} bytes; limit is {maximumOutputSize:N0} bytes.");
        }

        var output = new byte[outputSize];
        var outputPosition = 0;
        while (position < input.Length)
        {
            var control = input[position++];
            int literalCount;
            int copyCount;
            int copyOffset;

            if (control >= 0xFC)
            {
                literalCount = control & 0x03;
                CopyLiterals(input, ref position, output, ref outputPosition, literalCount);
                break;
            }

            if (control >= 0xE0)
            {
                literalCount = ((control & 0x1F) << 2) + 4;
                CopyLiterals(input, ref position, output, ref outputPosition, literalCount);
                continue;
            }

            if (control >= 0xC0)
            {
                EnsureAvailable(input, position, 3);
                var byte1 = input[position++];
                var byte2 = input[position++];
                var byte3 = input[position++];
                literalCount = control & 0x03;
                copyCount = ((control & 0x0C) << 6) + byte3 + 5;
                copyOffset = ((control & 0x10) << 12) + (byte1 << 8) + byte2 + 1;
            }
            else if (control >= 0x80)
            {
                EnsureAvailable(input, position, 2);
                var byte1 = input[position++];
                var byte2 = input[position++];
                literalCount = byte1 >> 6;
                copyCount = (control & 0x3F) + 4;
                copyOffset = ((byte1 & 0x3F) << 8) + byte2 + 1;
            }
            else
            {
                EnsureAvailable(input, position, 1);
                var byte1 = input[position++];
                literalCount = control & 0x03;
                copyCount = ((control & 0x1C) >> 2) + 3;
                copyOffset = ((control & 0x60) << 3) + byte1 + 1;
            }

            CopyLiterals(input, ref position, output, ref outputPosition, literalCount);
            CopyBackReference(output, ref outputPosition, copyOffset, copyCount);
        }

        if (outputPosition != output.Length)
        {
            throw new InvalidDataException(
                $"RefPack produced {outputPosition:N0} bytes, expected {output.Length:N0}.");
        }

        return output;
    }

    private static int ReadBigEndianSize(ReadOnlySpan<byte> bytes) => bytes.Length switch
    {
        3 => (bytes[0] << 16) | (bytes[1] << 8) | bytes[2],
        4 => checked((int)BinaryPrimitives.ReadUInt32BigEndian(bytes)),
        _ => throw new ArgumentOutOfRangeException(nameof(bytes))
    };

    private static void CopyLiterals(
        ReadOnlySpan<byte> input,
        ref int inputPosition,
        Span<byte> output,
        ref int outputPosition,
        int count)
    {
        EnsureAvailable(input, inputPosition, count);
        EnsureOutputAvailable(output, outputPosition, count);
        input.Slice(inputPosition, count).CopyTo(output.Slice(outputPosition, count));
        inputPosition += count;
        outputPosition += count;
    }

    private static void CopyBackReference(
        Span<byte> output,
        ref int outputPosition,
        int offset,
        int count)
    {
        if (offset <= 0 || offset > outputPosition)
        {
            throw new InvalidDataException($"Invalid RefPack back-reference offset {offset}.");
        }

        EnsureOutputAvailable(output, outputPosition, count);
        for (var index = 0; index < count; index++)
        {
            output[outputPosition] = output[outputPosition - offset];
            outputPosition++;
        }
    }

    private static void EnsureAvailable(ReadOnlySpan<byte> input, int position, int count)
    {
        if (position < 0 || count < 0 || position > input.Length - count)
        {
            throw new InvalidDataException("Unexpected end of RefPack input.");
        }
    }

    private static void EnsureOutputAvailable(Span<byte> output, int position, int count)
    {
        if (position < 0 || count < 0 || position > output.Length - count)
        {
            throw new InvalidDataException("RefPack command exceeds declared output size.");
        }
    }
}
