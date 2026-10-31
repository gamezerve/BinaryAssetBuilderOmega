
using System.Xml;
using Relo;

namespace BinaryAssetBuilder.ManifestInspector;

internal static class OnDemandTextureCompiler
{
    internal static Chunk Compile(XmlElement source, byte[] payload, string path)
    {
        if (source.LocalName != "OnDemandTexture")
            throw new InvalidDataException("Expected OnDemandTexture.");

        if (!path.EndsWith(".dds", StringComparison.OrdinalIgnoreCase))
            throw new InvalidDataException("OnDemandTexture currently requires a precompiled DDS.");

        var info = Ep1DdsTexture.Inspect(payload, path);

        if (source.GetAttribute("OutputFormat") != info.Format)
            throw new InvalidDataException("OnDemandTexture DDS format mismatch: " + path);

        return new Chunk
        {
            InstanceBuffer = new byte[8],
            RelocationBuffer = Array.Empty<byte>(),
            ImportsBuffer = Array.Empty<byte>()
        };
    }
}
