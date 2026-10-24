namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: ordered mod-over-SDK alias roots are explicit input, never process-current-directory or registry discovery.
internal sealed record SdkSourceSearch(string ModData,string ModArt,string ModAudio,string SdkData,string SdkArt,string SdkAudio)
{
    // Reborn: retain every earlier candidate so publication can reject newly appearing overrides as well as changed selected files.
    internal sealed record Selection(string Document,string LogicalPath,string Layer,string PhysicalPath,string Root,string[] Candidates);
    internal string[] Roots => new[]{ModData,ModArt,ModAudio,SdkData,SdkArt,SdkAudio};

    //-------------------------------------------------------------------------------------------------
    /** Reborn: recheck path precedence independently of content hashes; late mod shadows invalidate a captured SDK winner. */
    //-------------------------------------------------------------------------------------------------
    internal static void Verify(IEnumerable<Selection> selections)
    {
        foreach(var selection in selections)
        {
            string? winner=null;
            foreach(string path in selection.Candidates){SdkEnvironmentPreflight.CheckPath(path);if(Directory.Exists(path))throw new InvalidDataException("Search candidate became a directory.");if(File.Exists(path)){winner=path;break;}}
            if(!string.Equals(winner,selection.PhysicalPath,StringComparison.OrdinalIgnoreCase))throw new InvalidDataException("Alias search precedence changed: "+selection.LogicalPath);
        }
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: standard SDK layout is portable; only an explicitly supplied stock XML root may replace SageXML during local development. */
    //-------------------------------------------------------------------------------------------------
    internal static SdkSourceSearch ForMod(string entry,string sdk,string? stockData=null)
    {
        string data=SdkEnvironmentPreflight.DirectoryPath(Path.GetDirectoryName(SdkEnvironmentPreflight.Absolute(entry))!);
        // Reborn: AdditionalMaps entries retain the enclosing mod Data authority; relative includes still start at their declaring file.
        for(var parent=new DirectoryInfo(data);parent!=null;parent=parent.Parent)
            if(parent.Name.Equals("Data",StringComparison.OrdinalIgnoreCase)){data=parent.FullName;break;}
        string mod=Path.GetDirectoryName(data)!;sdk=SdkEnvironmentPreflight.DirectoryPath(sdk);
        var search=new SdkSourceSearch(data,Path.Combine(mod,"Art"),Path.Combine(mod,"Audio"),stockData==null?Path.Combine(sdk,"SageXML"):SdkEnvironmentPreflight.DirectoryPath(stockData),Path.Combine(sdk,"Art"),Path.Combine(sdk,"Audio"));
        foreach(string root in search.Roots)SdkEnvironmentPreflight.CheckPath(root);
        for(int i=0;i<search.Roots.Length;i++)for(int j=i+1;j<search.Roots.Length;j++)
            if(search.Roots[i].Equals(search.Roots[j],StringComparison.OrdinalIgnoreCase) || SdkEnvironmentPreflight.Inside(search.Roots[i],search.Roots[j]) || SdkEnvironmentPreflight.Inside(search.Roots[j],search.Roots[i]))throw new InvalidDataException("Alias roots must be distinct and non-overlapping.");
        return search;
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: choose the first existing confined file; unsafe or directory overrides fail rather than masking a broken mod with stock data. */
    //-------------------------------------------------------------------------------------------------
    internal Selection Select(string document,string logical,string confinement)
    {
        int colon=logical.IndexOf(':');
        if(colon<0){
            var relative=SdkSourcePathAudit.Resolve(logical,Path.GetDirectoryName(document)!,confinement,ModData,ModArt,ModAudio);
            // Reborn: copied EA XML may keep sibling base includes; search the corresponding SageXML folder after the mod-local candidate.
            if(confinement.Equals(ModData,StringComparison.OrdinalIgnoreCase)){
                string fallback=Path.Combine(SdkData,Path.GetRelativePath(ModData,relative.Path));
                return Choose(new[]{(relative.Path,relative.Root,"relative"),(fallback,SdkData,"sdk-relative")});
            }
            return Choose(new[]{(relative.Path,relative.Root,"relative")});
        }
        string alias=logical[..colon].ToLowerInvariant(),tail=logical[(colon+1)..];
        string mod=alias switch{"data"=>ModData,"art"=>ModArt,"audio"=>ModAudio,_=>throw new InvalidDataException("Unsupported SDK alias: "+alias)};
        string stock=alias switch{"data"=>SdkData,"art"=>SdkArt,_=>SdkAudio};
        var candidates=new List<(string Path,string Root,string Layer)>();
        Add(mod,"mod",false);
        if(alias=="art" && !tail.Contains('/') && !tail.Contains('\\'))Add(mod,"mod-fanout",true);
        Add(stock,"sdk",alias=="art");
        return Choose(candidates.ToArray());

        //-------------------------------------------------------------------------------------------------
        /** Reborn: reuse the strict path parser for every candidate, including direct mod art and stock two-character fanout. */
        //-------------------------------------------------------------------------------------------------
        void Add(string root,string layer,bool fanout)
        {
            var resolved=SdkSourcePathAudit.Resolve((fanout?"ART:":"DATA:")+tail,root,root,root,root,root);
            candidates.Add((resolved.Path,root,layer));
        }

        //-------------------------------------------------------------------------------------------------
        /** Reborn: capture deterministic selection evidence and actionable searched paths without reading arbitrary resource bodies. */
        //-------------------------------------------------------------------------------------------------
        Selection Choose((string Path,string Root,string Layer)[] paths)
        {
            foreach(var candidate in paths)
            {
                SdkEnvironmentPreflight.CheckPath(candidate.Path);
                if(Directory.Exists(candidate.Path))throw new InvalidDataException("Alias candidate is a directory: "+candidate.Path);
                if(File.Exists(candidate.Path))return new(document,logical,candidate.Layer,candidate.Path,candidate.Root,paths.Select(p=>p.Path).Distinct(StringComparer.OrdinalIgnoreCase).ToArray());
            }
            throw new FileNotFoundException("Alias file missing; searched: "+string.Join("; ",paths.Select(p=>p.Path)));
        }
    }
}
