using System.Text;
using System.Xml;
using System.Xml.Schema;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: schema-backed source matching admits variable reviewed Behaviors without guessing native layouts from new module positions.
internal static class ConscriptVariableBehaviorSource
{
    internal sealed record Plan(Dictionary<string,string> Origins,int BehaviorCount);

    //-------------------------------------------------------------------------------------------------
    /** Reborn: match complete nested payload shapes to reviewed templates while allowing their fixed-width values and local IDs to vary. */
    //-------------------------------------------------------------------------------------------------
    internal static Plan Admit(XmlDocument source,XmlDocument baseline,XmlSchemaSet schemas)
    {
        ConscriptSharedSourceContract.Validate(source,baseline,schemas);
        if(!ConscriptSourceInput.Identity(source.DocumentElement!.GetAttribute("id")))throw SdkXmlDiagnostics.At(new InvalidDataException("Invalid GameObject identity."),source.DocumentElement);
        // Reborn: honor official weapon sortBy/boolean semantics before assigning common reference occurrences.
        ConscriptWeaponSetContract.Normalize(source);
        var actual=Qualified(source);var model=Qualified(baseline);
        // Reborn: nested numeric overflow must identify its authored attribute instead of failing later inside the native marshaler.
        foreach(var owner in ConscriptModuleScalarContract.Owners(source))ConscriptUpgradeStructure.ValidateSource(owner);
        var owners=ConscriptModuleScalarContract.Owners(actual);var originals=ConscriptModuleScalarContract.Owners(model);
        var behaviors=owners.Where(o=>o.Container=="Behaviors").ToArray();var templates=originals.Where(o=>o.Container=="Behaviors").ToArray();
        // Reborn: module count is source-driven, not capped at the old trial budget; existing checked buffer/depth bounds protect compilation.
        if(behaviors.Length<1)throw SdkXmlDiagnostics.At(new InvalidDataException("Empty Behaviors container serialization is not implemented on this variable graph path."),source.GetElementsByTagName("Behaviors", "uri:ea.com:eala:asset")[0]);
        var origins=new Dictionary<string,string>(StringComparer.Ordinal);var selected=new List<ConscriptModuleScalarContract.Owner>();
        foreach(var owner in behaviors)
        {
            // Reborn: exact existing identity breaks equivalent-template ties; new IDs may use any otherwise identical complete reviewed shape.
            var candidates=templates.Where(t=>t.Element.LocalName==owner.Element.LocalName).OrderByDescending(t=>t.Element.GetAttribute("id")==owner.Element.GetAttribute("id"));
            var matched=candidates.FirstOrDefault(t=>Matches(owner,t));
            if(matched==null)throw SdkXmlDiagnostics.At(new InvalidDataException("Behavior '"+owner.Element.LocalName+"' contains an unimplemented native layout or nested field. Covered hardpoint/Weapon and upgrade lists, compatible targets and fixed attributes are editable; other families/structures still require their EP1 processor support."),SdkXmlDiagnostics.Unmasked(owner.Element,source));
            origins.Add(owner.Element.GetAttribute("id"),matched.Element.GetAttribute("id"));selected.Add(matched);
        }
        // Reborn: every other root/container retains the existing shape, but reviewed root and direct module fixed fields remain authorable.
        var copy=Clone(actual);var comparison=Clone(model);var oldBehaviors=Container(copy);
        oldBehaviors.ParentNode!.ReplaceChild(copy.ImportNode(Container(comparison),true),oldBehaviors);
        copy.DocumentElement!.SetAttribute("id",comparison.DocumentElement!.GetAttribute("id"));ConscriptSharedSourceContract.Mask(copy,comparison);
        // Reborn: the native comparer checks text bytes and allocations independently after structural admission.
        GameObjectRootTextContract.Mask(copy,comparison);
        // Reborn: schema-valid ArmorSet lists are independently compiled, not pinned to the stock row count/presence.
        ConscriptArmorSetContract.Mask(copy,comparison);
        // Reborn: existing strong slots retain schema types/presence, but their target names are authored values on both compilation routes.
        ConscriptReferenceSpellings.Compare(copy,comparison,schemas,allowRetarget:true,allowUpgradeWeak:true);
        if(ConscriptSourceInput.Signature(copy.DocumentElement)!=ConscriptSourceInput.Signature(comparison.DocumentElement!))
        {
            string difference=ConscriptSourceInput.Difference(copy.DocumentElement,comparison.DocumentElement!,"GameObject",out var offending);
            throw SdkXmlDiagnostics.At(new InvalidDataException("GameObject contains an unsupported owner/layout: "+difference+". Root text, fixed values, source-sized ArmorSet and covered module lists are enabled; other structures still require EP1 processor support."),SdkXmlDiagnostics.Unmasked(offending,source));
        }
        foreach(var owner in owners.Where(o=>o.Container!="Behaviors"))
        {
            var original=originals.Single(o=>o.Container==owner.Container&&o.Index==owner.Index);origins.Add(owner.Element.GetAttribute("id"),original.Element.GetAttribute("id"));
        }
        // Reborn: source-order reference occurrences must be stable across attribute formatting and added/cloned behavior identities.
        var aligned=Clone(model);var list=Container(aligned);list.RemoveAll();
        foreach(var template in selected)list.AppendChild(aligned.ImportNode(template.Element,true));
        // Reborn: flexible upgrade children do not have to match the template's occurrence count during source-order alignment.
        var upgradeIds=owners.Where(o=>ConscriptUpgradeStructure.Supports(o.NativeType)).Select(o=>o.Element.GetAttribute("id")).ToHashSet(StringComparer.Ordinal);
        Align(source.DocumentElement!,aligned.DocumentElement!,upgradeIds);
        return new(origins,behaviors.Length);

        // Reborn: comparison copies materialize official defaults without introducing allocations into the user's authored source.
        XmlDocument Qualified(XmlDocument input){var document=Clone(input);document.Schemas.Add(schemas);document.Validate((_,e)=>throw SdkXmlDiagnostics.SchemaError(e));foreach(XmlAttribute a in document.SelectNodes("//@*")!)a.Value=a.Value;return Clone(document);}
        // Reborn: only known ABI fixed fields and the own local ID are masked; all nested fields/text/reference identities stay visible.
        bool Matches(ConscriptModuleScalarContract.Owner owner,ConscriptModuleScalarContract.Owner template)
        {
            var a=Clone(model);var b=Clone(model);var left=Container(a);var right=Container(b);left.RemoveAll();right.RemoveAll();
            var node=(XmlElement)a.ImportNode(owner.Element,true);var basis=(XmlElement)b.ImportNode(template.Element,true);left.AppendChild(node);right.AppendChild(basis);node.SetAttribute("id",basis.GetAttribute("id"));
            foreach(var field in ConscriptModuleScalarContract.Bindings(template.NativeType)){node.RemoveAttribute(field.Name);if(basis.HasAttribute(field.Name))node.SetAttribute(field.Name,basis.GetAttribute(field.Name));}
            // Reborn: list counts and optional animation presence are validated from source/native contracts, not frozen to this stock example.
            if(ConscriptUpgradeStructure.Supports(template.NativeType))ConscriptUpgradeStructure.Mask(node,basis);
            // Reborn: the implemented EP1 hardpoint processor accepts source-sized weapon records, not only the captured two-weapon example.
            if(ConscriptWeaponSetContract.Supports(template.NativeType))ConscriptWeaponSetContract.Mask(node,basis);
            ConscriptReferenceSpellings.Compare(a,b,schemas,allowRetarget:true,allowUpgradeWeak:true);
            return ConscriptSourceInput.Signature(node)==ConscriptSourceInput.Signature(basis);
        }
    }

    // Reborn: clone serialized values so XmlDocument does not lose default attribute value children during comparison.
    private static XmlDocument Clone(XmlDocument source)=>SdkXmlDiagnostics.Parse(Encoding.UTF8.GetBytes(source.DocumentElement!.OuterXml));
    // Reborn: schema admission requires exactly one direct Behaviors container; missing/duplicate topology is never inferred.
    private static XmlElement Container(XmlDocument source)=>source.DocumentElement!.ChildNodes.OfType<XmlElement>().Single(e=>e.LocalName=="Behaviors");
    // Reborn: keep authored child order and values while using reviewed template attribute order for Core occurrence assignment.
    private static void Align(XmlElement actual,XmlElement model,HashSet<string> upgradeIds)
    {
        var order=model.Attributes.OfType<XmlAttribute>().Select((a,i)=>(Key:(a.NamespaceURI,a.LocalName),Index:i)).ToDictionary(p=>p.Key,p=>p.Index);
        var attributes=actual.Attributes.OfType<XmlAttribute>().OrderBy(a=>order.GetValueOrDefault((a.NamespaceURI,a.LocalName),int.MaxValue)).ToArray();foreach(var a in attributes)actual.RemoveAttributeNode(a);foreach(var a in attributes)actual.Attributes.Append(a);
        bool flexible=upgradeIds.Contains(actual.GetAttribute("id"));
        // Reborn: optional display-name children contain text only and cannot alter Core asset-reference occurrence ordering.
        bool Skip(XmlElement e)=>(flexible&&ConscriptUpgradeStructure.Fields.Contains(e.LocalName))||(actual.LocalName=="GameObject"&&(GameObjectRootTextContract.Pointed.Contains(e.LocalName)||e.LocalName=="ArmorSet"))||(actual.LocalName=="WeaponSetUpdate"&&e.LocalName=="WeaponSlotHardpoint");
        var children=actual.ChildNodes.OfType<XmlElement>().Where(e=>!Skip(e)).ToArray();var models=model.ChildNodes.OfType<XmlElement>().Where(e=>!Skip(e)).ToArray();if(children.Length!=models.Length)throw SdkXmlDiagnostics.At(new InvalidDataException("Unsupported variable source child structure."),actual);
        for(int i=0;i<children.Length;i++){if(children[i].LocalName!=models[i].LocalName)throw SdkXmlDiagnostics.At(new InvalidDataException("Unsupported variable source child order."),children[i]);Align(children[i],models[i],upgradeIds);}
    }
}
