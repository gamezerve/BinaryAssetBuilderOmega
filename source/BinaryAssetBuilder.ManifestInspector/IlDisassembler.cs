using System.Reflection;
using System.Reflection.Emit;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using System.Reflection.PortableExecutable;

namespace BinaryAssetBuilder.ManifestInspector;

internal static class IlDisassembler
{
    private static readonly IReadOnlyDictionary<ushort, OpCode> OpCodesByValue =
        typeof(OpCodes).GetFields(BindingFlags.Public | BindingFlags.Static)
            .Where(field => field.FieldType == typeof(OpCode))
            .Select(field => (OpCode)field.GetValue(null)!)
            .ToDictionary(opCode => unchecked((ushort)opCode.Value));

    public static void Print(string assemblyPath, string tokenText)
    {
        int token = tokenText.StartsWith("0x", StringComparison.OrdinalIgnoreCase)
            ? Convert.ToInt32(tokenText[2..], 16)
            : Convert.ToInt32(tokenText);
        using FileStream stream = File.OpenRead(Path.GetFullPath(assemblyPath));
        using PEReader peReader = new(stream);
        MetadataReader reader = peReader.GetMetadataReader();
        Handle handle = MetadataTokens.Handle(token);
        if (handle.Kind != HandleKind.MethodDefinition)
        {
            throw new ArgumentException($"Token 0x{token:X8} is not a method definition.");
        }

        MethodDefinition method = reader.GetMethodDefinition((MethodDefinitionHandle)handle);
        MethodBodyBlock body = peReader.GetMethodBody(method.RelativeVirtualAddress);
        byte[] il = body.GetILBytes()
            ?? throw new InvalidDataException($"Method 0x{token:X8} has no IL body.");
        Console.WriteLine(
            $"METHOD token=0x{token:X8} rva=0x{method.RelativeVirtualAddress:X8} " +
            $"code={il.Length} maxstack={body.MaxStack} locals=0x{MetadataTokens.GetToken(body.LocalSignature):X8}");

        int position = 0;
        while (position < il.Length)
        {
            int instructionOffset = position;
            ushort value = il[position++];
            if (value == 0xFE)
            {
                value = (ushort)(0xFE00 | il[position++]);
            }
            if (!OpCodesByValue.TryGetValue(value, out OpCode opCode))
            {
                throw new InvalidDataException($"Unknown IL opcode 0x{value:X4} at IL_{instructionOffset:X4}.");
            }

            string operand = ReadOperand(il, ref position, instructionOffset, opCode.OperandType, reader);
            Console.WriteLine($"  IL_{instructionOffset:X4}: {opCode.Name,-13} {operand}");
        }
    }

    private static string ReadOperand(
        byte[] il,
        ref int position,
        int instructionOffset,
        OperandType operandType,
        MetadataReader reader)
    {
        switch (operandType)
        {
            case OperandType.InlineNone:
                return string.Empty;
            case OperandType.ShortInlineI:
                return ((sbyte)il[position++]).ToString();
            case OperandType.InlineI:
                return ReadInt32(il, ref position).ToString();
            case OperandType.InlineI8:
                long value64 = BitConverter.ToInt64(il, position);
                position += 8;
                return value64.ToString();
            case OperandType.ShortInlineR:
                float value32 = BitConverter.ToSingle(il, position);
                position += 4;
                return value32.ToString("R");
            case OperandType.InlineR:
                double valueDouble = BitConverter.ToDouble(il, position);
                position += 8;
                return valueDouble.ToString("R");
            case OperandType.ShortInlineVar:
                return il[position++].ToString();
            case OperandType.InlineVar:
                ushort variable = BitConverter.ToUInt16(il, position);
                position += 2;
                return variable.ToString();
            case OperandType.ShortInlineBrTarget:
                sbyte shortDelta = (sbyte)il[position++];
                return $"IL_{position + shortDelta:X4}";
            case OperandType.InlineBrTarget:
                int delta = ReadInt32(il, ref position);
                return $"IL_{position + delta:X4}";
            case OperandType.InlineSwitch:
                int count = ReadInt32(il, ref position);
                int basePosition = position + (count * 4);
                string[] targets = new string[count];
                for (int index = 0; index < count; index++)
                {
                    targets[index] = $"IL_{basePosition + ReadInt32(il, ref position):X4}";
                }
                return string.Join(", ", targets);
            case OperandType.InlineString:
                int stringToken = ReadInt32(il, ref position);
                return $"0x{stringToken:X8} \"{reader.GetUserString(MetadataTokens.UserStringHandle(stringToken & 0x00FFFFFF))}\"";
            case OperandType.InlineField:
            case OperandType.InlineMethod:
            case OperandType.InlineSig:
            case OperandType.InlineTok:
            case OperandType.InlineType:
                int token = ReadInt32(il, ref position);
                return $"0x{token:X8} {ResolveToken(reader, token)}";
            default:
                throw new NotSupportedException($"Unsupported operand type {operandType} at IL_{instructionOffset:X4}.");
        }
    }

    private static int ReadInt32(byte[] il, ref int position)
    {
        int value = BitConverter.ToInt32(il, position);
        position += 4;
        return value;
    }

    private static string ResolveToken(MetadataReader reader, int token)
    {
        Handle handle = MetadataTokens.Handle(token);
        return handle.Kind switch
        {
            HandleKind.MethodDefinition => reader.GetString(reader.GetMethodDefinition((MethodDefinitionHandle)handle).Name),
            HandleKind.MemberReference => reader.GetString(reader.GetMemberReference((MemberReferenceHandle)handle).Name),
            HandleKind.FieldDefinition => reader.GetString(reader.GetFieldDefinition((FieldDefinitionHandle)handle).Name),
            HandleKind.TypeDefinition => GetTypeName(reader, reader.GetTypeDefinition((TypeDefinitionHandle)handle)),
            HandleKind.TypeReference => GetTypeName(reader, reader.GetTypeReference((TypeReferenceHandle)handle)),
            HandleKind.TypeSpecification => "TypeSpecification",
            HandleKind.StandaloneSignature => "StandaloneSignature",
            HandleKind.MethodSpecification => "MethodSpecification",
            _ => handle.Kind.ToString()
        };
    }

    private static string GetTypeName(MetadataReader reader, TypeDefinition definition) =>
        JoinTypeName(reader.GetString(definition.Namespace), reader.GetString(definition.Name));

    private static string GetTypeName(MetadataReader reader, TypeReference reference) =>
        JoinTypeName(reader.GetString(reference.Namespace), reader.GetString(reference.Name));

    private static string JoinTypeName(string typeNamespace, string name) =>
        string.IsNullOrEmpty(typeNamespace) ? name : $"{typeNamespace}.{name}";
}
