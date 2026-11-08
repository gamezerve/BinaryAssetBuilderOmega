using System.Xml;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: original configuration witnesses use the production capture/fragment/expression path without any user-mod overrides.
internal static class StockOriginalUiConfig
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: prepare one original configuration in a fresh private wrapper, preserving all captured source dependencies. */
    //-------------------------------------------------------------------------------------------------
    internal static XmlElement[] Load(string relative,string type)
    {
        string fixtures=Path.GetDirectoryName(ReferencePipelineSmokeTest.FindFixture())!;
        for(var folder=new DirectoryInfo(fixtures);folder!=null;folder=folder.Parent)
        foreach(string prefix in new[]{"SageXML","alpha/SageXML"})
        {
            string root=Path.Combine(folder.FullName,prefix),original=Path.Combine(root,relative);
            if(!File.Exists(original))continue;
            string sdk=Path.GetDirectoryName(root)!,workRoot=Path.Combine(sdk,".work");
            string work=Path.GetFullPath(Path.Combine(workRoot,"stock-native-original-"+Guid.NewGuid().ToString("N")));
            if(!work.StartsWith(Path.GetFullPath(workRoot)+Path.DirectorySeparatorChar,StringComparison.OrdinalIgnoreCase))throw new InvalidDataException("Original witness workspace escapes SDK work root.");
            SdkEnvironmentPreflight.CheckPath(work);
            string data=Path.Combine(work,"Data"),entry=Path.Combine(data,"Static_Mod.xml");Directory.CreateDirectory(data);
            try
            {
                File.WriteAllText(entry,"<AssetDeclaration xmlns='uri:ea.com:eala:asset'><!-- Reborn: independent original configuration witness. --><Includes><Include type='all' source='DATA:"+System.Security.SecurityElement.Escape(relative)+"'/></Includes></AssetDeclaration>");
                var input=ConscriptSourcePreparation.Load(entry,SdkSourceSearch.ForMod(entry,sdk,root));
                return new[]{input.Game}.Concat(input.AdditionalAssets).Where(d=>d.DocumentElement!.LocalName==type).Select(d=>d.DocumentElement!).ToArray();
            }
            finally
            {
                // Reborn: remove only our exact wrapper and empty directories; unexpected additional files are never recursively deleted.
                File.Delete(entry);Directory.Delete(data);Directory.Delete(work);
            }
        }
        throw new InvalidDataException("Original configuration source not found: "+relative);
    }
}