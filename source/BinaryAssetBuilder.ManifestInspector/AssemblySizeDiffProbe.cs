using System.Reflection.Metadata;
using System.Reflection.PortableExecutable;
using System.Runtime.InteropServices;

namespace BinaryAssetBuilder.ManifestInspector;

internal static class AssemblySizeDiffProbe
{
    private sealed record LayoutDifference(string TypeName, int ReferenceSize, int? CurrentSize)
    {
        public int? Delta => CurrentSize - ReferenceSize;
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: compare official tokenizer layout declarations with the current SageBinaryData ABI. */
    //-------------------------------------------------------------------------------------------------
    public static void Print(string assemblyPath, string? topValue)
    {
        int top = ParseTop(topValue);
        Dictionary<string, int> referenceSizes = ReadReferenceSizes(Path.GetFullPath(assemblyPath));
        Dictionary<string, int> currentSizes = ReadCurrentSizes();
        LayoutDifference[] rows = referenceSizes
            .Select(pair => new LayoutDifference(
                pair.Key,
                pair.Value,
                currentSizes.TryGetValue(pair.Key, out int currentSize) ? currentSize : null))
            .ToArray();
        LayoutDifference[] mismatches = rows
            .Where(row => row.CurrentSize.HasValue && row.CurrentSize != row.ReferenceSize)
            .OrderByDescending(row => Math.Abs(row.Delta!.Value))
            .ThenBy(row => row.TypeName, StringComparer.Ordinal)
            .ToArray();

        Console.WriteLine($"REFERENCE {Path.GetFullPath(assemblyPath)}");
        Console.WriteLine(
            $"ReferenceLayouts={referenceSizes.Count} Matched={rows.Count(row => row.CurrentSize.HasValue)} " +
            $"Same={rows.Count(row => row.CurrentSize == row.ReferenceSize)} " +
            $"Mismatched={mismatches.Length} MissingCurrent={rows.Count(row => !row.CurrentSize.HasValue)}");
        Console.WriteLine(" Delta Current Reference Type");
        foreach (LayoutDifference row in mismatches.Take(top))
        {
            Console.WriteLine($"{row.Delta,6} {row.CurrentSize,7} {row.ReferenceSize,9} {row.TypeName}");
        }

        if (mismatches.Length > top)
        {
            Console.WriteLine($"... {mismatches.Length - top} additional mismatch(es); use --top to show more.");
        }
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: read explicit native sizes without loading the reference compiler or its dependencies. */
    //-------------------------------------------------------------------------------------------------
    private static Dictionary<string, int> ReadReferenceSizes(string assemblyPath)
    {
        using FileStream stream = File.OpenRead(assemblyPath);
        using var peReader = new PEReader(stream);
        MetadataReader metadata = peReader.GetMetadataReader();
        var result = new Dictionary<string, int>(StringComparer.Ordinal);
        foreach (TypeDefinitionHandle handle in metadata.TypeDefinitions)
        {
            TypeDefinition definition = metadata.GetTypeDefinition(handle);
            string typeNamespace = metadata.GetString(definition.Namespace);
            TypeLayout layout = definition.GetLayout();
            if (!typeNamespace.Equals("SageBinaryData", StringComparison.Ordinal) || layout.Size <= 0)
            {
                continue;
            }

            result[$"{typeNamespace}.{metadata.GetString(definition.Name)}"] = layout.Size;
        }

        return result;
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: measure every marshalable type emitted by the in-tree SageBinaryData assembly. */
    //-------------------------------------------------------------------------------------------------
    private static Dictionary<string, int> ReadCurrentSizes()
    {
        var result = new Dictionary<string, int>(StringComparer.Ordinal);
        foreach (Type type in typeof(SageBinaryData.ArmorTemplate).Assembly.GetTypes())
        {
            if (type.FullName is null || type.Namespace?.Equals("SageBinaryData", StringComparison.Ordinal) != true)
            {
                continue;
            }

            try
            {
                result[type.FullName] = Marshal.SizeOf(type);
            }
            catch (ArgumentException)
            {
                // Reborn: generic/helper types without a native ABI are irrelevant to stream layouts.
            }
        }

        return result;
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: validate the optional output limit while keeping full comparison counts. */
    //-------------------------------------------------------------------------------------------------
    private static int ParseTop(string? value)
    {
        if (value is null)
        {
            return 50;
        }

        if (!int.TryParse(value, out int top) || top <= 0)
        {
            throw new ArgumentException($"Invalid positive integer '{value}' for --top.");
        }

        return top;
    }
}
