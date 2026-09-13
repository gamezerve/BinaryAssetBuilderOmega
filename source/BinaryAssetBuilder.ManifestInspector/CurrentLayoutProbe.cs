using System.Reflection;
using System.Runtime.InteropServices;

namespace BinaryAssetBuilder.ManifestInspector;

internal static class CurrentLayoutProbe
{
    public static void Print(string typeName)
    {
        Type[] matches = typeof(SageBinaryData.ArmorTemplate).Assembly.GetTypes()
            .Where(type => type.Name.Equals(typeName, StringComparison.OrdinalIgnoreCase)
                || type.FullName?.Equals(typeName, StringComparison.OrdinalIgnoreCase) == true)
            .ToArray();
        if (matches.Length == 0)
        {
            throw new ArgumentException($"Current SageBinaryData type '{typeName}' was not found.");
        }

        foreach (Type type in matches)
        {
            Console.WriteLine($"TYPE {type.FullName}");
            Console.WriteLine($"  Size={Marshal.SizeOf(type)} Layout={type.StructLayoutAttribute?.Value}");
            foreach (FieldInfo field in type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
            {
                long offset = Marshal.OffsetOf(type, field.Name).ToInt64();
                Console.WriteLine($"  {offset,4} {field.FieldType.FullName} {field.Name}");
            }
        }
    }
}
