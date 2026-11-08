namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: locale is a distinct original runtime stream; its original catalog also participates in stock dependency lookup.
internal static class StockLocaleCatalog
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: locate only the SDK-local captured locale catalog, with no game discovery or runtime extraction. */
    //-------------------------------------------------------------------------------------------------
    internal static string? Find()
    {
        for(var folder=new DirectoryInfo(Path.GetDirectoryName(ReferencePipelineSmokeTest.FindFixture())!);folder!=null;folder=folder.Parent)
        foreach(string relative in new[]{"catalogs/ra3ep1/streams/locale/locale.manifest","alpha/catalogs/ra3ep1/streams/locale/locale.manifest"})
        {
            string path=Path.Combine(folder.FullName,relative);if(File.Exists(path))return path;
        }
        return null;
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: require a real linked original EP1 catalog before using any locale stock identities. */
    //-------------------------------------------------------------------------------------------------
    internal static ManifestAsset[] Read()
    {
        string? path=Find();if(path==null)return Array.Empty<ManifestAsset>();
        var catalog=ManifestReader.Read(SdkEnvironmentPreflight.Read(path,64*1048576));
        TypeRegistryAudit.ValidateTarget(catalog.Header.Version,catalog.Header.AllTypesHash);
        if(!catalog.Header.IsLinked||catalog.Validate().Count!=0)throw new InvalidDataException("Invalid original locale catalog.");
        return catalog.Assets.ToArray();
    }
}
