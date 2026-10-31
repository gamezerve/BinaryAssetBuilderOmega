
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;
using System.Xml;
using BinaryAssetBuilder.Core;
using BinaryAssetBuilder.XmlCompiler;

namespace BinaryAssetBuilder.ManifestInspector;

internal static class OnDemandTextureCDataPackage
{
    internal static void Publish(
        string directory,
        string entry,
        IEnumerable<(XmlDocument Source, InstanceDeclaration Instance)> assets)
    {
        var textures = assets
            .Where(a => a.Source.DocumentElement!.LocalName == "OnDemandTexture")
            .ToArray();

        if (textures.Length == 0)
            return;

        string path = Path.Combine(directory, "RebornConscriptProbe.big");
        string temporary = path + ".cdata.tmp";

        var files = new Dictionary<string, byte[]>(StringComparer.OrdinalIgnoreCase);

        using (var original = BigArchive.Open(path, 15, 4096))
        {
            foreach (var item in original.Entries)
                files.Add(item.Name, original.ReadEntry(item, checked((int)item.StoredSize)));
        }

        foreach (var texture in textures)
        {
            var element = texture.Source.DocumentElement!;
            string relative = element.GetAttribute("File");

            var match = Regex.Match(
                relative,
                @"^resources/([0-9A-Fa-f]{64})\.dds$",
                RegexOptions.CultureInvariant);

            if (!match.Success)
                throw new InvalidDataException("OnDemandTexture requires a captured DDS: " + relative);

            string resource = Path.Combine(
                Path.GetDirectoryName(Path.GetFullPath(entry))!,
                relative.Replace('/', Path.DirectorySeparatorChar));

            byte[] dds = File.ReadAllBytes(resource);

            string hash = Convert.ToHexString(SHA256.HashData(dds));

            if (!hash.Equals(match.Groups[1].Value, StringComparison.OrdinalIgnoreCase))
                throw new InvalidDataException("OnDemandTexture DDS hash differs: " + resource);

            var handle = texture.Instance.Handle;

            string name = OnDemandTextureCData.FileName(
                handle.TypeId,
                handle.InstanceId,
                handle.TypeHash,
                handle.InstanceHash);

            byte[] cdata = OnDemandTextureCData.Build(
                handle.TypeId,
                handle.InstanceId,
                handle.TypeHash,
                handle.InstanceHash,
                dds);

            OnDemandTextureCData.Verify(
                cdata,
                dds,
                handle.TypeId,
                handle.InstanceId,
                handle.TypeHash,
                handle.InstanceHash);

            foreach (string suffix in new[] { "", "_l", "_m" })
            {
                string archiveName = "data\\mod" + suffix + "\\cdata\\" + name;

                if (!files.TryAdd(archiveName, cdata))
                    throw new InvalidDataException("Duplicate CDATA archive entry: " + archiveName);
            }
        }

        byte[] archive = WorldBuilderAssetResolver.Pack(files);

        try
        {
            File.WriteAllBytes(temporary, archive);

            using (var verified = BigArchive.Open(temporary, files.Count, 4096))
            {
                if (verified.Entries.Count != files.Count)
                    throw new InvalidDataException("CDATA archive entry count differs.");

                foreach (var item in files)
                {
                    var found = verified.Entries.Single(e =>
                        e.Name.Equals(item.Key, StringComparison.OrdinalIgnoreCase));

                    if (!verified.ReadEntry(found, item.Value.Length).SequenceEqual(item.Value))
                        throw new InvalidDataException("CDATA archive readback differs: " + item.Key);
                }
            }

            File.Move(temporary, path, true);
        }
        finally
        {
            if (File.Exists(temporary))
                File.Delete(temporary);
        }

        string receiptPath = Path.Combine(directory, "trial-plan.json");
        var receipt = JsonNode.Parse(File.ReadAllText(receiptPath))!;

        receipt["ArchiveEntryCount"] = files.Count;
        receipt["ArchiveSha256"] = Convert.ToHexString(SHA256.HashData(archive));
        receipt["OnDemandTextureCDataCount"] = textures.Length;

        File.WriteAllText(
            receiptPath,
            receipt.ToJsonString(new JsonSerializerOptions { WriteIndented = true }));
    }
}
