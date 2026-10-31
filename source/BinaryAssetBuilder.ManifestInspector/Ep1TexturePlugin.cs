
using System.Xml;
using BinaryAssetBuilder.Core;
using BinaryAssetBuilder.Core.Hashing;
using Relo;
using SageBinaryData;

namespace BinaryAssetBuilder.ManifestInspector;

internal sealed class Ep1TexturePlugin : IAssetBuilderPlugin, IAssetBuilderOutputPolicy
{
    private bool initialized;

    public string ProfileName => "RA3EP1-Texture-v1";
    public bool CanWriteProductionOutput => false;
    public bool CanUseBuildCache => false;
    public bool CanReuseCompiledDocuments => false;
    public uint AllTypesHash => 0x5454A8E9u;
    public uint VersionNumber => 1;

    public void Initialize(TargetPlatform platform)
    {
        if (platform != TargetPlatform.Win32)
            throw new NotSupportedException("EP1 Texture supports only Win32.");

        initialized = true;
    }

    public void ReInitialize(TargetPlatform platform) => Initialize(platform);

    public ExtendedTypeInformation GetExtendedTypeInformation(uint typeId)
    {
        if (!initialized || typeId != Ep1TextureCompiler.TypeId)
            throw new NotSupportedException($"Unsupported Texture TypeId: 0x{typeId:X8}");

        return new ExtendedTypeInformation
        {
            TypeId = typeId,
            TypeName = "Texture",
            Type = typeof(Texture),
            TypeHash = Ep1TextureCompiler.TypeHash,
            ProcessingHash = Ep1TextureCompiler.TypeHash,
            Tokenized = false,
            HasCustomData = false,
            UseBuildCache = false
        };
    }

    public AssetBuffer ProcessInstance(InstanceDeclaration instance)
    {
        if (instance.Node is not XmlElement element ||
            element.LocalName != "Texture" ||
            instance.Handle.TypeId != Ep1TextureCompiler.TypeId)
            throw new InvalidDataException("Invalid AptUI Texture instance.");

        if (instance.ReferencedInstances.Count != 0 ||
            instance.InheritFromHandle != null)
            throw new NotSupportedException("Texture dependencies or inheritance are not supported.");

        string file = element.GetAttribute("File");

        if (string.IsNullOrWhiteSpace(file))
            throw new InvalidDataException("Texture File attribute is missing.");

        string directory = Path.GetDirectoryName(instance.Document.SourcePath)!;
        string path = Path.GetFullPath(Path.Combine(directory, file));

        if (!File.Exists(path))
            throw new FileNotFoundException("Texture source not found.", path);

        byte[] source = File.ReadAllBytes(path);
        var chunk = Ep1TextureCompiler.Compile(element, source, path);

        return new AssetBuffer
        {
            InstanceData = chunk.InstanceBuffer,
            RelocationData = chunk.RelocationBuffer,
            ImportsData = chunk.ImportsBuffer
        };
    }
}
