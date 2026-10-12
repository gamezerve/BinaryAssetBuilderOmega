using System.Xml;
using BinaryAssetBuilder.Core;
using BinaryAssetBuilder.Core.Hashing;
using BinaryAssetBuilder.Core.SageXml;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: focused weapon schema/default/reference preparation is diagnostic; no processor registration or production output.
internal static class WeaponSchemaPreparation
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: validate actual EP1 types and invoke Core reference normalization before identifying concrete metadata targets. */
    //-------------------------------------------------------------------------------------------------
    internal static XmlDocument Prepare(XmlDocument source, params string[] manifests)
    {
        // Reborn: a schema-valid control is not implemented inheritance/expression support; refuse it rather than ignoring its semantics.
        var input = source.DocumentElement ?? throw new InvalidDataException("Weapon root absent.");
        if (input.GetAttribute("id") != "SovietAntiInfantryInfantryAK47" || input.HasAttribute("inheritFrom")
            || input.SelectNodes(".//@*")!.Cast<XmlAttribute>().Any(attribute => attribute.Value.TrimStart().StartsWith("=", StringComparison.Ordinal)))
            throw new InvalidDataException("Focused weapon requires its fixed ID without inheritance or unresolved expressions.");
        Settings saved = Settings.Current;
        try
        {
            string fixtures = Path.GetDirectoryName(ReferencePipelineSmokeTest.FindFixture())!;
            FXListNativeSmokeTest.CheckExtractedEnums(fixtures, "WeaponDocumentPipeline.xsd");
            Settings.Current = new Settings { SchemaPath = Path.Combine(fixtures, "WeaponDocumentPipeline.xsd"), BuildCache = false,
                StringHashBinDescriptors = Array.Empty<StringHashBinDescriptor>() };
            var schemas = new SchemaSet(false);
            var normalized = ReferencePipelineSmokeTest.Normalize(schemas, source.DocumentElement!.OuterXml);
            // Reborn: deriving polymorphic identity from current schema prevents hand-injected nugget type tags.
            var nugget = ((XmlElement)normalized.XmlNode).GetElementsByTagName("DamageNugget", SchemaSet.XmlNamespace).Cast<XmlElement>().Single();
            string type = nugget.SchemaInfo.SchemaType?.Name ?? throw new InvalidDataException("Damage nugget lacks schema type.");
            if (type != "DamageNuggetType") throw new InvalidDataException("Unexpected weapon nugget schema type.");
            nugget.SetAttribute("TypeId", FastHash.GetHashCode(type).ToString(System.Globalization.CultureInfo.InvariantCulture));
            if (normalized.ReferencedInstances.Count != 3 || normalized.WeakReferencedInstances.Count != 0)
                throw new InvalidDataException("Focused weapon has an unproven dependency shape.");
            var names = new[] { "SOV_Conscript_GunFireMS", "FX_MuzzleSoviet", "FX_MuzzleSoviet" };
            if (!normalized.ReferencedInstances.Select(reference => reference.InstanceName).SequenceEqual(names))
                throw new InvalidDataException("Focused weapon dependency names/order differ.");
            if (manifests.Length != 0)
            {
                var targets = manifests.SelectMany(path => ManifestReader.Read(File.ReadAllBytes(path)).Assets).ToArray();
                var expected = new[] { "Multisound:SOV_Conscript_GunFireMS", "FXList:FX_MuzzleSoviet", "FXList:FX_MuzzleSoviet" };
                for (int index = 0; index < normalized.ReferencedInstances.Count; index++)
                {
                    var reference = normalized.ReferencedInstances[index];
                    var types = schemas.GetDerivedTypes(reference.TypeName)?.Cast<string>().Append(reference.TypeName).ToArray()
                        ?? new[] { reference.TypeName };
                    var candidates = targets.Where(asset => asset.InstanceId == reference.InstanceId
                        && types.Any(name => FastHash.GetHashCode(name) == asset.TypeId)).Select(asset => asset.Name).Distinct().ToArray();
                    if (candidates.Length != 1 || candidates[0] != expected[index])
                        throw new InvalidDataException("Weapon schema dependency missing, ambiguous or wrong: " + reference.Name);
                }
            }
            return normalized.XmlNode.OwnerDocument!;
        }
        finally { Settings.Current = saved; }
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: reject detached XML/schema/unsupported-control/dependency faults and preserve caller settings and source bytes. */
    //-------------------------------------------------------------------------------------------------
    internal static void CheckControls(XmlDocument source, params string[] manifests)
    {
        string original = source.OuterXml; Settings saved = Settings.Current;
        foreach (int fault in Enumerable.Range(0, 8))
        {
            var changed = new XmlDocument { XmlResolver = null }; changed.LoadXml(original);
            var root = changed.DocumentElement!;
            switch (fault)
            {
                case 0: root.SetAttribute("UnknownWeaponAttribute", "1"); break;
                case 1: ((XmlElement)root.GetElementsByTagName("DamageNugget", SchemaSet.XmlNamespace)[0]!).SetAttribute("DamageType", "INVALID_GUN"); break;
                case 2: root.AppendChild(changed.CreateElement("UnknownWeaponChild", SchemaSet.XmlNamespace)); break;
                case 3: root.RemoveAttribute("FireSoundPerClip"); break;
                case 4: root.SetAttribute("FireFX", "MissingWeaponFX"); break;
                case 5: root.SetAttribute("id", "NewWeaponNotAdmitted"); break;
                case 6: root.SetAttribute("inheritFrom", "BaseWeapon"); break;
                case 7: root.SetAttribute("AttackRange", "=$MISSING"); break;
            }
            bool refused = false;
            try { Prepare(changed, manifests); }
            catch (InvalidDataException) { refused = true; }
            if (!refused || !ReferenceEquals(saved, Settings.Current) || source.OuterXml != original)
                throw new InvalidDataException("Weapon preparation control failed: " + fault);
        }
    }
}
