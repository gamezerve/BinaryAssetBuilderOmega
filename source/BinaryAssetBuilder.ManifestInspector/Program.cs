using System.Text.Json;

namespace BinaryAssetBuilder.ManifestInspector;

internal static class Program
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true
    };

    public static int Main(string[] args)
    {
        try
        {
            if ((args.Length < 2 && args.FirstOrDefault() is not ("layout-self-test" or "compiler-self-test")) || args.FirstOrDefault() is not ("inspect" or "verify" or "compare" or "schema-diff" or "writer-self-test" or "utility-verify" or "assembly-fields" or "assembly-methods" or "assembly-il" or "current-layout" or "layout-self-test" or "compiler-self-test" or "asset-bytes" or "hash"))
            {
                PrintUsage();
                return 2;
            }

            var command = args[0];
            if (command == "hash")
            {
                foreach (string value in args.Skip(1))
                {
                    Console.WriteLine($"0x{BinaryAssetBuilder.Core.Hashing.FastHash.GetHashCode(value):X8} {value}");
                }
                return 0;
            }
            if (command == "compiler-self-test")
            {
                CompilerSmokeTest.Run();
                return 0;
            }

            if (command == "asset-bytes")
            {
                if (args.Length < 4)
                {
                    PrintUsage();
                    return 2;
                }
                AssetStreamProbe.Print(
                    args[1], args[2], args[3], GetOption(args, "--entry"),
                    GetOption(args, "--bin-entry"), GetOption(args, "--asset"),
                    ParseUInt32Option(args, "--find-u32"),
                    ParseInt32Option(args, "--offset"), ParseInt32Option(args, "--count"));
                return 0;
            }

            if (command == "layout-self-test")
            {
                UprisingLayoutSmokeTest.Run();
                return 0;
            }

            if (command == "assembly-il")
            {
                if (args.Length < 3)
                {
                    PrintUsage();
                    return 2;
                }
                IlDisassembler.Print(args[1], args[2]);
                return 0;
            }

            if (command == "current-layout")
            {
                CurrentLayoutProbe.Print(args[1]);
                return 0;
            }

            if (command == "assembly-fields")
            {
                if (args.Length < 3)
                {
                    PrintUsage();
                    return 2;
                }
                AssemblyFieldProbe.Print(args[1], args[2]);
                return 0;
            }

            if (command == "assembly-methods")
            {
                if (args.Length < 3)
                {
                    PrintUsage();
                    return 2;
                }
                AssemblyFieldProbe.PrintMethods(args[1], args[2], args.Length > 3 ? args[3] : null);
                return 0;
            }

            if (command == "writer-self-test")
            {
                WriterSmokeTest.Run(args[1]);
                return 0;
            }

            if (command == "utility-verify")
            {
                UtilityManifestVerifier.Verify(args[1], GetOption(args, "--entry"));
                return 0;
            }

            if (command == "schema-diff")
            {
                return CompareSchemas(args);
            }

            if (command == "compare")
            {
                return Compare(args);
            }

            var path = Path.GetFullPath(args[1]);
            var entryName = GetOption(args, "--entry");
            var json = args.Contains("--json", StringComparer.OrdinalIgnoreCase);
            var documents = LoadDocuments(path, entryName);

            if (json)
            {
                Console.WriteLine(JsonSerializer.Serialize(documents.Select(ToReport), JsonOptions));
            }
            else
            {
                foreach (var (source, document) in documents)
                {
                    PrintDocument(source, document);
                }
            }

            if (command == "verify")
            {
                var errors = documents
                    .SelectMany(item => item.Document.Validate().Select(error => $"{item.Source}: {error}"))
                    .ToArray();
                foreach (var error in errors)
                {
                    Console.Error.WriteLine($"ERROR: {error}");
                }

                return errors.Length == 0 ? 0 : 1;
            }

            return 0;
        }
        catch (Exception exception)
        {
            Console.Error.WriteLine($"ERROR: {exception}");
            return 1;
        }
    }

    private static int CompareSchemas(string[] args)
    {
        if (args.Length < 3)
        {
            PrintUsage();
            return 2;
        }

        var differences = SchemaComparer.Compare(args[1], args[2]);
        if (args.Contains("--json", StringComparer.OrdinalIgnoreCase))
        {
            Console.WriteLine(JsonSerializer.Serialize(differences, JsonOptions));
        }
        else
        {
            foreach (var difference in differences)
            {
                Console.WriteLine($"{difference.Status,-7} {difference.Path}");
            }

            Console.WriteLine(
                $"Summary: Added={differences.Count(item => item.Status == "Added")}, " +
                $"Removed={differences.Count(item => item.Status == "Removed")}, " +
                $"Changed={differences.Count(item => item.Status == "Changed")}");
        }

        return 0;
    }

    private static int Compare(string[] args)
    {
        if (args.Length < 3)
        {
            PrintUsage();
            return 2;
        }

        var left = LoadDocuments(Path.GetFullPath(args[1]), GetOption(args, "--left-entry"));
        var right = LoadDocuments(Path.GetFullPath(args[2]), GetOption(args, "--right-entry"));
        if (left.Count != 1 || right.Count != 1)
        {
            throw new ArgumentException("Compare requires exactly one manifest on each side; select BIG entries explicitly.");
        }

        var leftTypes = BuildTypeMap(left[0].Document);
        var rightTypes = BuildTypeMap(right[0].Document);
        var names = leftTypes.Keys.Union(rightTypes.Keys).OrderBy(name => name).ToArray();
        var rows = names.Select(name => new
        {
            TypeName = name,
            Left = leftTypes.GetValueOrDefault(name),
            Right = rightTypes.GetValueOrDefault(name),
            Status = !leftTypes.ContainsKey(name) ? "Added" :
                !rightTypes.ContainsKey(name) ? "Removed" :
                leftTypes[name] != rightTypes[name] ? "Changed" : "Same"
        }).ToArray();

        if (args.Contains("--json", StringComparer.OrdinalIgnoreCase))
        {
            Console.WriteLine(JsonSerializer.Serialize(rows, JsonOptions));
        }
        else
        {
            Console.WriteLine($"LEFT  {left[0].Source}");
            Console.WriteLine($"RIGHT {right[0].Source}");
            foreach (var row in rows.Where(row => row.Status != "Same"))
            {
                Console.WriteLine(
                    $"{row.Status,-7} {row.TypeName,-36} " +
                    $"{FormatFingerprint(row.Left),24} -> {FormatFingerprint(row.Right),24}");
            }

            Console.WriteLine(
                $"Summary: Added={rows.Count(row => row.Status == "Added")}, " +
                $"Removed={rows.Count(row => row.Status == "Removed")}, " +
                $"Changed={rows.Count(row => row.Status == "Changed")}, " +
                $"Same={rows.Count(row => row.Status == "Same")}");
        }

        return 0;
    }

    private static Dictionary<string, TypeFingerprint> BuildTypeMap(ManifestDocument document) =>
        document.Assets
            .GroupBy(asset => asset.TypeName)
            .ToDictionary(
                group => group.Key,
                group =>
                {
                    var sample = group.First();
                    return new TypeFingerprint(sample.TypeId, sample.TypeHash, sample.Tokenized);
                });

    private static string FormatFingerprint(TypeFingerprint? value) => value is null
        ? "-"
        : $"0x{value.TypeId:X8}/0x{value.TypeHash:X8}/T{value.Tokenized?.ToString() ?? "-"}";

    private static IReadOnlyList<(string Source, ManifestDocument Document)> LoadDocuments(
        string path,
        string? entryName)
    {
        if (!path.EndsWith(".big", StringComparison.OrdinalIgnoreCase))
        {
            return [(path, ManifestReader.Read(File.ReadAllBytes(path)))];
        }

        using var archive = BigArchive.Open(path);
        var entries = archive.Entries.Where(entry =>
            entry.Name.EndsWith(".manifest", StringComparison.OrdinalIgnoreCase) &&
            (entryName is null || entry.Name.Equals(entryName, StringComparison.OrdinalIgnoreCase)))
            .ToArray();
        if (entries.Length == 0)
        {
            throw new InvalidDataException(
                entryName is null
                    ? "BIG archive contains no .manifest entries."
                    : $"BIG archive does not contain manifest entry '{entryName}'.");
        }

        return entries
            .Select(entry => ($"{path}::{entry.Name}", ManifestReader.Read(archive.ReadEntry(entry))))
            .ToArray();
    }

    private static string? GetOption(IReadOnlyList<string> args, string name)
    {
        for (var index = 0; index < args.Count; index++)
        {
            if (args[index].Equals(name, StringComparison.OrdinalIgnoreCase))
            {
                if (index + 1 >= args.Count)
                {
                    throw new ArgumentException($"Missing value for {name}.");
                }

                return args[index + 1];
            }
        }

        return null;
    }

    private static uint? ParseUInt32Option(IReadOnlyList<string> args, string name)
    {
        string? value = GetOption(args, name);
        if (value is null)
        {
            return null;
        }

        string digits = value.StartsWith("0x", StringComparison.OrdinalIgnoreCase) ? value[2..] : value;
        if (!uint.TryParse(digits, System.Globalization.NumberStyles.HexNumber,
                System.Globalization.CultureInfo.InvariantCulture, out uint result))
        {
            throw new ArgumentException($"Invalid hexadecimal 32-bit value '{value}' for {name}.");
        }

        return result;
    }

    private static int? ParseInt32Option(IReadOnlyList<string> args, string name)
    {
        string? value = GetOption(args, name);
        if (value is null)
        {
            return null;
        }

        bool hexadecimal = value.StartsWith("0x", StringComparison.OrdinalIgnoreCase);
        string digits = hexadecimal ? value[2..] : value;
        System.Globalization.NumberStyles style = hexadecimal
            ? System.Globalization.NumberStyles.HexNumber
            : System.Globalization.NumberStyles.Integer;
        if (!int.TryParse(digits, style, System.Globalization.CultureInfo.InvariantCulture, out int result))
        {
            throw new ArgumentException($"Invalid non-negative 32-bit value '{value}' for {name}.");
        }

        return result;
    }

    private static void PrintDocument(string source, ManifestDocument document)
    {
        var header = document.Header;
        Console.WriteLine(source);
        Console.WriteLine(
            $"  Version={header.Version} Linked={header.IsLinked} Checksum=0x{header.StreamChecksum:X8} " +
            $"AllTypesHash=0x{header.AllTypesHash:X8}");
        Console.WriteLine(
            $"  Profile={document.Profile?.Name ?? "Unknown"} Assets={header.AssetCount:N0} " +
            $"InstanceBytes={header.TotalInstanceDataSize:N0} RefPack={document.WasRefPackCompressed}");
        Console.WriteLine(
            $"  References={string.Join(", ", document.ReferencedManifests.Select(reference =>
                reference.IsPatch ? $"{reference.Path} (patch)" : reference.Path).DefaultIfEmpty("(none)"))}");
        foreach (var group in document.Assets
                     .GroupBy(asset => asset.TypeName)
                     .OrderByDescending(group => group.Count())
                     .ThenBy(group => group.Key)
                     .Take(20))
        {
            var sample = group.First();
            Console.WriteLine(
                $"  {group.Count(),6:N0} {group.Key,-32} TypeId=0x{sample.TypeId:X8} TypeHash=0x{sample.TypeHash:X8}");
        }

        var errors = document.Validate();
        Console.WriteLine(errors.Count == 0 ? "  Validation=OK" : $"  Validation={errors.Count} error(s)");
    }

    private static object ToReport((string Source, ManifestDocument Document) item) => new
    {
        item.Source,
        item.Document.Profile?.Name,
        item.Document.Header,
        item.Document.ReferencedManifests,
        item.Document.WasRefPackCompressed,
        ValidationErrors = item.Document.Validate(),
        Types = item.Document.Assets
            .GroupBy(asset => new { asset.TypeName, asset.TypeId, asset.TypeHash, asset.Tokenized })
            .Select(group => new
            {
                group.Key.TypeName,
                TypeId = $"0x{group.Key.TypeId:X8}",
                TypeHash = $"0x{group.Key.TypeHash:X8}",
                group.Key.Tokenized,
                Count = group.Count()
            })
            .OrderByDescending(group => group.Count)
            .ThenBy(group => group.TypeName)
    };

    private static void PrintUsage()
    {
        Console.WriteLine("BinaryAssetBuilder.ManifestInspector");
        Console.WriteLine("  inspect <manifest-or-big> [--entry <BIG entry>] [--json]");
        Console.WriteLine("  verify  <manifest-or-big> [--entry <BIG entry>] [--json]");
        Console.WriteLine("  compare <left> <right> [--left-entry <entry>] [--right-entry <entry>] [--json]");
        Console.WriteLine("  schema-diff <left-xsd-directory> <right-xsd-directory> [--json]");
        Console.WriteLine("  writer-self-test <output-manifest>");
        Console.WriteLine("  utility-verify <manifest-or-big> [--entry <BIG entry>]");
        Console.WriteLine("  assembly-fields <managed-assembly> <type-name>");
        Console.WriteLine("  assembly-methods <managed-assembly> <type-name> [method-filter]");
        Console.WriteLine("  assembly-il <managed-assembly> <method-token>");
        Console.WriteLine("  current-layout <SageBinaryData-type-name>");
        Console.WriteLine("  layout-self-test");
        Console.WriteLine("  compiler-self-test");
        Console.WriteLine("  asset-bytes <manifest-or-big> <bin-or-big> <type-name> [--entry <manifest-entry>] [--bin-entry <bin-entry>] [--asset <full-name>] [--find-u32 <hex>] [--offset <decimal-or-hex>] [--count <decimal-or-hex>]");
        Console.WriteLine("  hash <text> [additional-text ...]");
    }

    private sealed record TypeFingerprint(uint TypeId, uint TypeHash, uint? Tokenized);
}
