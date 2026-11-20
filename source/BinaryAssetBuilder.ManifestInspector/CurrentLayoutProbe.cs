using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Runtime.InteropServices;

namespace BinaryAssetBuilder.ManifestInspector;

internal static class CurrentLayoutProbe
{
    public static void Print(string typeName)
    {
        Type[] matches = Find(typeName);
        foreach (Type type in matches)
        {
            Console.WriteLine($"TYPE {type.FullName}");
            Console.WriteLine($"  Size={Marshal.SizeOf(type)} Layout={type.StructLayoutAttribute?.Value}");
            foreach (FieldInfo field in type.GetFields(InstanceFields))
            {
                long offset = Marshal.OffsetOf(type, field.Name).ToInt64();
                Console.WriteLine($"  {offset,4} {field.FieldType.FullName} {field.Name}");
            }
        }
    }

    // Reborn: use the same native field inventory for human and machine-readable layouts.
    private const BindingFlags InstanceFields=BindingFlags.Public|BindingFlags.NonPublic|BindingFlags.Instance;
    private static readonly MethodInfo SizeMethod=typeof(CurrentLayoutProbe)
        .GetMethod(nameof(UnmanagedSize),BindingFlags.NonPublic|BindingFlags.Static)!;

    //-------------------------------------------------------------------------------------------------
    /** Reborn: export measured x86 field sizes, including relocatable strings, fixed arrays and inline records. */
    //-------------------------------------------------------------------------------------------------
    internal static string Index(IEnumerable<string> typeNames)
    {
        var names = typeNames.ToArray();
        if (!names.SequenceEqual(new[] { "--ep1-campaign" }))
            return IndexRecords(names.SelectMany(Find), null);

        // Reborn: the separate EP1 export preserves legacy CLR lookups and exposes the writer's XML aliases.
        var aliases = new[] { "CampaignEnumerator", "CampaignTemplate", "MissionTemplate" }
            .ToDictionary(CampaignManagerCompiler.LayoutFor, name => name);
        return IndexRecords(aliases.Keys, aliases);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: use the same x86 measurements for direct CLR lookups and XML campaign aliases. */
    //-------------------------------------------------------------------------------------------------
    private static string IndexRecords(IEnumerable<Type> requested, IReadOnlyDictionary<Type, string>? aliases)
    {
        if(IntPtr.Size!=4)
            throw new InvalidOperationException("The EP1 layout index requires an x86 inspector process.");
        var types=requested.Distinct().OrderBy(type=>type.FullName,StringComparer.Ordinal);
        var records=types.Select(type=>new
        {
            name=type.FullName,
            xmlName=aliases?.GetValueOrDefault(type),
            size=FieldSize(type),
            fields=type.GetFields(InstanceFields).Select(field=>new
            {
                name=field.Name,
                type=field.FieldType.ToString(),
                offset=Marshal.OffsetOf(type,field.Name).ToInt32(),
                size=DeclaredFieldSize(field)
            }).OrderBy(field=>field.offset).ThenBy(field=>field.name,StringComparer.Ordinal).ToArray()
        }).ToArray();
        return JsonSerializer.Serialize(new { architecture="x86",pointerSize=IntPtr.Size,records },
            new JsonSerializerOptions { WriteIndented=true, DefaultIgnoreCondition=System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull });
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: fixed buffers use their declared element count, and pointers occupy exactly one x86 word. */
    //-------------------------------------------------------------------------------------------------
    private static int DeclaredFieldSize(FieldInfo field)
    {
        var buffer=field.GetCustomAttribute<FixedBufferAttribute>();
        return buffer==null ? FieldSize(field.FieldType) : checked(FieldSize(buffer.ElementType)*buffer.Length);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: avoid native interop BOOL sizes and guessed generic layouts by measuring managed unmanaged storage. */
    //-------------------------------------------------------------------------------------------------
    private static int FieldSize(Type type) => type.IsPointer || type.IsFunctionPointer
        ? IntPtr.Size
        : (int)SizeMethod.MakeGenericMethod(type).Invoke(null,null)!;

    //-------------------------------------------------------------------------------------------------
    /** Reborn: measure the same generic unmanaged storage used by the compiler's native allocator. */
    //-------------------------------------------------------------------------------------------------
    private static int UnmanagedSize<T>() where T:unmanaged => Unsafe.SizeOf<T>();

    //-------------------------------------------------------------------------------------------------
    /** Reborn: resolve the same short and qualified names in both layout commands. */
    //-------------------------------------------------------------------------------------------------
    private static Type[] Find(string typeName)
    {
        Type[] matches = typeof(SageBinaryData.ArmorTemplate).Assembly.GetTypes()
            .Where(type => type.Name.Equals(typeName, StringComparison.OrdinalIgnoreCase)
                || type.FullName?.Equals(typeName, StringComparison.OrdinalIgnoreCase) == true)
            .ToArray();
        if (matches.Length == 0)
        {
            throw new ArgumentException($"Current SageBinaryData type '{typeName}' was not found.");
        }

        return matches;
    }
}
