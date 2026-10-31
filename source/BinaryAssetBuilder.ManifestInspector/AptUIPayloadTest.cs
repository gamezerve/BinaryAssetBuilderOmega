
using BinaryAssetBuilder.Core;
using BinaryAssetBuilder.Core.SageXml;
using BinaryAssetBuilder.XmlCompiler;
using System.Security.Cryptography;
using System.Xml;
using System.Xml.Linq;

namespace BinaryAssetBuilder.ManifestInspector;

internal static class AptUIPayloadTest
{
    private static readonly HashSet<string> AssetTypes = new(StringComparer.Ordinal)
    {
        "AptAptData",
        "AptConstData",
        "AptDatData",
        "AptGeometryData"
    };

    public static void Run(string xmlPath)
    {
        string root = Path.GetFullPath(Path.GetDirectoryName(xmlPath)!);
        var visited = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        CompilerSmokeTest.InitializeHashProvider();

        var plugin = new Plugin();
        plugin.Initialize(TargetPlatform.Win32);

        int assets = 0;
        long totalBytes = 0;

        void Process(string path)
        {
            path = Path.GetFullPath(path);

            if (!File.Exists(path))
                throw new FileNotFoundException("AptUI XML not found", path);

            if (!visited.Add(path))
                return;

            var settings = new XmlReaderSettings
            {
                DtdProcessing = DtdProcessing.Prohibit,
                XmlResolver = null,
                MaxCharactersInDocument = 4 * 1024 * 1024
            };

            using var reader = XmlReader.Create(path, settings);
            var document = XDocument.Load(reader);

            foreach (var element in document.Descendants())
            {
                if (element.Name.LocalName == "Include")
                {
                    string? include = element.Attribute("source")?.Value;

                    if (!string.IsNullOrWhiteSpace(include))
                        Process(Resolve(path, include, root));

                    continue;
                }

                if (!AssetTypes.Contains(element.Name.LocalName))
                    continue;

                string type = element.Name.LocalName;
                string file = element.Attribute("File")?.Value
                    ?? throw new InvalidDataException($"{type} has no File attribute.");

                string resolved = Resolve(path, file, root);
                byte[] bytes = File.ReadAllBytes(resolved);

                if (bytes.Length == 0)
                    throw new InvalidDataException($"Empty AptUI payload: {resolved}");

                string hash = Convert.ToHexString(SHA256.HashData(bytes));

                var xmlDocument = new XmlDocument { XmlResolver = null };
                xmlDocument.LoadXml(element.ToString(SaveOptions.DisableFormatting));

                Console.WriteLine($"{type}: {file}");
                Console.WriteLine($"  Size: {bytes.Length}");
                Console.WriteLine($"  SHA256: {hash}");

                assets++;
                totalBytes += bytes.Length;
            }
        }

        Process(xmlPath);

        if (assets == 0)
            throw new InvalidDataException("No AptUI asset declarations found.");

        Console.WriteLine($"AptUI XML files: {visited.Count}");
        Console.WriteLine($"AptUI assets: {assets}");
        Console.WriteLine($"AptUI payload bytes: {totalBytes}");
        Console.WriteLine("AptUI payload test passed.");
    }

    private static string Resolve(string xml, string relative, string root)
    {
        string path = Path.GetFullPath(
            Path.Combine(Path.GetDirectoryName(xml)!, relative));

        string prefix = Path.TrimEndingDirectorySeparator(root)
            + Path.DirectorySeparatorChar;

        if (!path.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
            throw new InvalidDataException($"AptUI path escapes source root: {relative}");

        if (!File.Exists(path))
            throw new FileNotFoundException("AptUI source not found", path);

        return path;
    }
}
