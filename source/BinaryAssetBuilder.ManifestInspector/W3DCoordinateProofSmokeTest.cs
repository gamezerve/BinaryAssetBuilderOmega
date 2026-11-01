using System.Xml;
using Relo;
using SageBinaryData;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: retain nonzero W3X axes while independently rejecting corrupt coordinate payloads.
internal static class W3DCoordinateProofSmokeTest
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: exercise uppercase model axes, lowercase gameplay axes and each scalar corruption without stock files or publication. */
    //-------------------------------------------------------------------------------------------------
    internal static void Run()
    {
        int checks = 0;
        foreach (bool upper in new[] { true, false })
        foreach (bool quaternion in new[] { false, true })
        {
            string[] names = quaternion ? new[] { "X", "Y", "Z", "W" } : new[] { "X", "Y", "Z" };
            float[] values = quaternion ? new[] { 1.25f, -2.5f, 10.610683f, 0.75f } : new[] { 1.25f, -2.5f, 10.610683f };
            var document = new XmlDocument();
            var root = document.CreateElement("Coordinates");
            document.AppendChild(root);
            var bytes = new byte[values.Length * 4];
            for (int i = 0; i < values.Length; i++)
            {
                root.SetAttribute(upper ? names[i] : names[i].ToLowerInvariant(), XmlConvert.ToString(values[i]));
                BitConverter.GetBytes(values[i]).CopyTo(bytes, i * 4);
            }
            Type type = quaternion ? typeof(Quaternion) : typeof(Vector3);
            SchemaNativeContract.Verify(root, type, 0, new Chunk { InstanceBuffer = bytes }, Array.Empty<AssetId>());
            checks++;
            for (int i = 0; i < values.Length; i++)
            {
                var corrupt = bytes.ToArray();
                corrupt[i * 4] ^= 1;
                try
                {
                    SchemaNativeContract.Verify(root, type, 0, new Chunk { InstanceBuffer = corrupt }, Array.Empty<AssetId>());
                }
                catch (InvalidDataException error) when (error.Message.Contains("Schema-proved native value differs:", StringComparison.Ordinal))
                {
                    checks++;
                    continue;
                }
                throw new InvalidOperationException("Corrupt coordinate accepted: " + type.Name + "." + names[i]);
            }
        }
        Console.WriteLine("Reborn: coordinate source/native proof: PASS (" + checks + " uppercase/lowercase vector/quaternion and corruption checks; no game or publication).");
    }
}
