using System.Collections.Immutable;
using System.Reflection;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using System.Reflection.PortableExecutable;

namespace BinaryAssetBuilder.ManifestInspector;

internal static class AssemblyFieldProbe
{
    public static void Print(string assemblyPath, string typeName)
    {
        string fullPath = Path.GetFullPath(assemblyPath);
        using FileStream stream = File.OpenRead(fullPath);
        using PEReader peReader = new PEReader(stream);
        if (!peReader.HasMetadata)
        {
            throw new InvalidDataException($"'{assemblyPath}' has no CLI metadata.");
        }

        MetadataReader reader = peReader.GetMetadataReader();
        SignatureNameProvider provider = new SignatureNameProvider();
        var matches = reader.TypeDefinitions
            .Select(handle => (Handle: handle, Definition: reader.GetTypeDefinition(handle)))
            .Where(item => TypeMatches(reader, item.Definition, typeName))
            .ToArray();
        if (matches.Length == 0)
        {
            throw new ArgumentException($"Type '{typeName}' was not found in '{assemblyPath}'.");
        }

        foreach (var match in matches)
        {
            string fullName = GetFullName(reader, match.Definition);
            TypeLayout layout = match.Definition.GetLayout();
            Console.WriteLine($"TYPE {fullName}");
            Console.WriteLine(
                $"  Attributes={match.Definition.Attributes & TypeAttributes.LayoutMask} " +
                $"Pack={layout.PackingSize} Size={layout.Size}");
            foreach (FieldDefinitionHandle fieldHandle in match.Definition.GetFields())
            {
                FieldDefinition field = reader.GetFieldDefinition(fieldHandle);
                string fieldType;
                try
                {
                    fieldType = field.DecodeSignature(provider, genericContext: null);
                }
                catch (BadImageFormatException)
                {
                    fieldType = "(undecodable signature)";
                }

                int offset = field.GetOffset();
                string offsetText = offset < 0 ? "-" : offset.ToString();
                string staticText = (field.Attributes & FieldAttributes.Static) != 0 ? " static" : string.Empty;
                Console.WriteLine($"  {offsetText,4} {fieldType} {reader.GetString(field.Name)}{staticText}");
            }
        }
    }

    public static void PrintMethods(string assemblyPath, string typeName, string? methodFilter)
    {
        string fullPath = Path.GetFullPath(assemblyPath);
        using FileStream stream = File.OpenRead(fullPath);
        using PEReader peReader = new PEReader(stream);
        MetadataReader reader = peReader.GetMetadataReader();
        SignatureNameProvider provider = new SignatureNameProvider();
        var matches = reader.TypeDefinitions
            .Select(handle => reader.GetTypeDefinition(handle))
            .Where(definition => TypeMatches(reader, definition, typeName))
            .ToArray();
        if (matches.Length == 0)
        {
            throw new ArgumentException($"Type '{typeName}' was not found in '{assemblyPath}'.");
        }

        foreach (TypeDefinition type in matches)
        {
            List<string> rows = [];
            foreach (MethodDefinitionHandle methodHandle in type.GetMethods())
            {
                MethodDefinition method = reader.GetMethodDefinition(methodHandle);
                string name = reader.GetString(method.Name);
                MethodSignature<string> signature;
                try
                {
                    signature = method.DecodeSignature(provider, genericContext: null);
                }
                catch (BadImageFormatException)
                {
                    if (string.IsNullOrEmpty(methodFilter)
                        || name.Contains(methodFilter, StringComparison.OrdinalIgnoreCase))
                    {
                        rows.Add($"  token=0x{MetadataTokens.GetToken(methodHandle):X8} rva=0x{method.RelativeVirtualAddress:X8} {name} (undecodable)");
                    }
                    continue;
                }

                string renderedSignature = $"{signature.ReturnType} {name}(" +
                    $"{string.Join(", ", signature.ParameterTypes)})";
                if (!string.IsNullOrEmpty(methodFilter)
                    && !name.Contains(methodFilter, StringComparison.OrdinalIgnoreCase)
                    && !renderedSignature.Contains(methodFilter, StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                rows.Add(
                    $"  token=0x{MetadataTokens.GetToken(methodHandle):X8} rva=0x{method.RelativeVirtualAddress:X8} " +
                    $"impl={method.ImplAttributes} {renderedSignature}");
            }

            if (rows.Count != 0)
            {
                Console.WriteLine($"TYPE {GetFullName(reader, type)}");
                foreach (string row in rows)
                {
                    Console.WriteLine(row);
                }
            }
        }
    }

    private static bool TypeMatches(MetadataReader reader, TypeDefinition definition, string requested)
    {
        string name = reader.GetString(definition.Name);
        if (requested.StartsWith('*') || requested.EndsWith('*'))
        {
            string fragment = requested.Trim('*');
            return name.Contains(fragment, StringComparison.OrdinalIgnoreCase)
                || GetFullName(reader, definition).Contains(fragment, StringComparison.OrdinalIgnoreCase);
        }
        return name.Equals(requested, StringComparison.OrdinalIgnoreCase)
            || GetFullName(reader, definition).Equals(requested, StringComparison.OrdinalIgnoreCase);
    }

    private static string GetFullName(MetadataReader reader, TypeDefinition definition)
    {
        string name = reader.GetString(definition.Name);
        string typeNamespace = reader.GetString(definition.Namespace);
        return string.IsNullOrEmpty(typeNamespace) ? name : $"{typeNamespace}.{name}";
    }

    private sealed class SignatureNameProvider : ISignatureTypeProvider<string, object?>
    {
        public string GetArrayType(string elementType, ArrayShape shape) => $"{elementType}[{new string(',', shape.Rank - 1)}]";
        public string GetByReferenceType(string elementType) => elementType + "&";
        public string GetFunctionPointerType(MethodSignature<string> signature) => "methodptr";
        public string GetGenericInstantiation(string genericType, ImmutableArray<string> typeArguments) =>
            $"{genericType}<{string.Join(", ", typeArguments)}>";
        public string GetGenericMethodParameter(object? genericContext, int index) => $"!!{index}";
        public string GetGenericTypeParameter(object? genericContext, int index) => $"!{index}";
        public string GetModifiedType(string modifier, string unmodifiedType, bool isRequired) => unmodifiedType;
        public string GetPinnedType(string elementType) => elementType;
        public string GetPointerType(string elementType) => elementType + "*";
        public string GetPrimitiveType(PrimitiveTypeCode typeCode) => typeCode.ToString();
        public string GetSZArrayType(string elementType) => elementType + "[]";

        public string GetTypeFromDefinition(
            MetadataReader reader,
            TypeDefinitionHandle handle,
            byte rawTypeKind)
        {
            TypeDefinition definition = reader.GetTypeDefinition(handle);
            return GetQualifiedName(reader.GetString(definition.Namespace), reader.GetString(definition.Name));
        }

        public string GetTypeFromReference(
            MetadataReader reader,
            TypeReferenceHandle handle,
            byte rawTypeKind)
        {
            TypeReference reference = reader.GetTypeReference(handle);
            return GetQualifiedName(reader.GetString(reference.Namespace), reader.GetString(reference.Name));
        }

        public string GetTypeFromSpecification(
            MetadataReader reader,
            object? genericContext,
            TypeSpecificationHandle handle,
            byte rawTypeKind) => reader.GetTypeSpecification(handle).DecodeSignature(this, genericContext);

        private static string GetQualifiedName(string typeNamespace, string name) =>
            string.IsNullOrEmpty(typeNamespace) ? name : $"{typeNamespace}.{name}";
    }
}
