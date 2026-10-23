using System.Buffers.Binary;
using System.Xml;
using BinaryAssetBuilder.Core.Hashing;
using BinaryAssetBuilder.XmlCompiler;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: existing module identities are authored local names, not fixed names copied from the captured Conscript.
internal static class ConscriptModuleIdentityContract
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: validate the complete local scope before masking names; duplicate hashes and dangling supported local links remain errors. */
    //-------------------------------------------------------------------------------------------------
    internal static void Validate(XmlDocument source)
    {
        // Reborn: point duplicate/empty identity errors at the actual later declaration, not only the merged GameObject root.
        var seen=new HashSet<uint>();foreach(var owner in ConscriptModuleScalarContract.Owners(source))if(string.IsNullOrWhiteSpace(owner.Element.GetAttribute("id"))||!seen.Add(Hash(owner.Element)))throw SdkXmlDiagnostics.At(new InvalidDataException("Empty or hash-colliding local module ID: "+owner.Element.GetAttribute("id")),(XmlNode?)owner.Element.GetAttributeNode("id")??owner.Element);
        Ep1SelectedModuleIdentity.ValidateSource(source.DocumentElement!,true);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: rename every existing module together and update the explicitly supported local association when present. */
    //-------------------------------------------------------------------------------------------------
    internal static void AuthorProbe(XmlDocument source)
    {
        var owners=ConscriptModuleScalarContract.Owners(source);var names=new Dictionary<string,string>(StringComparer.OrdinalIgnoreCase);
        for(int i=0;i<owners.Length;i++){string name="RebornLocalModule_"+i;names.Add(owners[i].Element.GetAttribute("id"),name);owners[i].Element.SetAttribute("id",name);}
        foreach(var owner in owners)if(owner.Element.HasAttribute("AssociatedModule"))owner.Element.SetAttribute("AssociatedModule",names[owner.Element.GetAttribute("AssociatedModule")]);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: compare corresponding module kinds/positions while allowing only the local identity word to change. */
    //-------------------------------------------------------------------------------------------------
    internal static void Mask(XmlDocument source,XmlDocument baseline)
    {
        var owners=ConscriptModuleScalarContract.Owners(source);var models=ConscriptModuleScalarContract.Owners(baseline);
        // Reborn: identify the actual container/count difference instead of presenting an unexplained whole-GameObject topology failure.
        var differences=owners.Select(o=>o.Container).Concat(models.Select(o=>o.Container)).Distinct(StringComparer.Ordinal)
            .Select(name=>(Name:name,Source:owners.Count(o=>o.Container==name),Baseline:models.Count(o=>o.Container==name)))
            .Where(row=>row.Source!=row.Baseline).ToArray();
        if(differences.Length!=0){
            var node=source.DocumentElement!.ChildNodes.OfType<XmlElement>().FirstOrDefault(e=>e.LocalName==differences[0].Name)??source.DocumentElement;
            throw SdkXmlDiagnostics.At(new InvalidDataException("Module topology changes require separate native layout support. Current verification baseline differs: "+string.Join("; ",differences.Select(d=>d.Name+" source="+d.Source+", baseline="+d.Baseline))+". These are verification-baseline counts, not engine module limits."),node);
        }
        for(int i=0;i<owners.Length;i++)
        {
            if(owners[i].Container!=models[i].Container||owners[i].Index!=models[i].Index||owners[i].NativeType!=models[i].NativeType)throw SdkXmlDiagnostics.At(new InvalidDataException("Module kind/order changes require separate native layout support: "+owners[i].Container+"["+owners[i].Index+"] source="+owners[i].Element.LocalName+", current baseline="+models[i].Element.LocalName+"."),owners[i].Element);
            owners[i].Element.SetAttribute("id",models[i].Element.GetAttribute("id"));
        }
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: verify every module header hash at its dynamically resolved native address, including moved root-string allocations. */
    //-------------------------------------------------------------------------------------------------
    internal static void Verify(XmlDocument source,byte[] native,int prefix)
    {
        Validate(source);
        foreach(var owner in ConscriptModuleScalarContract.Owners(source))
        {
            int at=ConscriptModuleScalarContract.Address(native,prefix,owner)+4;
            if(at<0||at>native.Length-4||BinaryPrimitives.ReadUInt32LittleEndian(native.AsSpan(at,4))!=Hash(owner.Element))throw new InvalidDataException("Authored module identity differs: "+owner.Element.GetAttribute("id"));
        }
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: restore only verified local identity words before applying the whole original-stock ownership oracle. */
    //-------------------------------------------------------------------------------------------------
    internal static void VerifyAndRestore(XmlDocument source,byte[] actual,byte[] restored,byte[] baseline)
    {
        Verify(source,actual,0);
        foreach(var owner in ConscriptModuleScalarContract.Owners(source))baseline.AsSpan(ConscriptModuleScalarContract.Address(baseline,0,owner)+4,4).CopyTo(restored.AsSpan(ConscriptModuleScalarContract.Address(actual,0,owner)+4,4));
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: expected tokenized native tails carry authored module hashes while all metadata fields, pointers and selectors remain independently checked. */
    //-------------------------------------------------------------------------------------------------
    internal static void Patch(XmlDocument source,byte[] target,int prefix)
    {foreach(var owner in ConscriptModuleScalarContract.Owners(source))BinaryPrimitives.WriteUInt32LittleEndian(target.AsSpan(ConscriptModuleScalarContract.Address(target,prefix,owner)+4,4),Hash(owner.Element));}

    // Reborn: use the engine's case-insensitive local module identity hashing, not the distinct instance-ID hash.
    private static uint Hash(XmlElement element)=>FastHash.GetHashCode(element.GetAttribute("id").ToLowerInvariant());
}
