using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Xml;
using System.Xml.Schema;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: bridge captured source semantics into the existing multi-armor compiler without widening alpha or production policies.
internal static class GeneralArmorSourceBuild
{
    private const string Ea = "uri:ea.com:eala:asset";

    //-------------------------------------------------------------------------------------------------
    /** Reborn: capture/recheck confined armor sources, hold read-only Windows sharing locks, prepare inheritance/defines then compile one owner stream. */
    //-------------------------------------------------------------------------------------------------
    internal static void Run(string sourceRoot, string entry, string output, bool validateOnly = false)
    {
        sourceRoot = SdkEnvironmentPreflight.DirectoryPath(sourceRoot);
        entry = SdkEnvironmentPreflight.Absolute(entry); output = SdkEnvironmentPreflight.Absolute(output);
        // Reborn: focused armor deployments carry only their reviewed schema graph, not the full SDK catalog or a developer checkout.
        string schemaPath = Path.Combine(Path.GetDirectoryName(ReferencePipelineSmokeTest.FindFixture())!, "ArmorSourcePipeline.xsd");
        var environment = SourceEnvironment(sourceRoot, entry, output, schemaPath);
        var paths = SdkSourcePathAudit.Inspect(environment);
        if (!paths.ScopedPathAuditComplete || paths.StoppedAtLimit || paths.Sources.Length > 32 || paths.Includes.Length > 64 || paths.Resources.Length != 0)
            throw new InvalidDataException("Armor source build requires complete 1..32-source/64-edge file-free path inventory: "
                + string.Join("; ", paths.Issues.Select(issue => issue.Code + ": " + issue.Detail)));
        var locks = new List<FileStream>(); var snapshots = new Dictionary<string, byte[]>(StringComparer.OrdinalIgnoreCase);
        try
        {
            long total = 0;
            // Reborn: this bridge refuses duplicate engine identities anywhere in its captured armor closure, not only published owner declarations.
            var sourceIds = new HashSet<uint>();
            foreach (var source in paths.Sources)
            {
                // Reborn: no write/delete sharing permits original XML to remain stable throughout preparation and candidate publication on Windows.
                SdkEnvironmentPreflight.CheckPath(source.PhysicalPath);
                var stream = File.Open(source.PhysicalPath, FileMode.Open, FileAccess.Read, FileShare.Read); locks.Add(stream);
                if (stream.Length < 1 || stream.Length > 65536 || (total += stream.Length) > 1048576)
                    throw new InvalidDataException("Armor source graph exceeds 64 KiB per file/one MiB total.");
                byte[] bytes = new byte[(int)stream.Length]; stream.ReadExactly(bytes);
                if (Convert.ToHexString(SHA256.HashData(bytes)) != source.Sha256) throw new InvalidDataException("Captured armor source changed.");
                CheckSourceShape(bytes);
                foreach (var armor in Parse(bytes).DocumentElement!.ChildNodes.OfType<XmlElement>().Where(node => node.LocalName == "ArmorTemplate"))
                    if (!sourceIds.Add(BinaryAssetBuilder.Core.InstanceHandle.GetInstanceId(armor.GetAttribute("id"))))
                        throw new InvalidDataException("Duplicate hashed armor identity in the captured source closure.");
                snapshots.Add(source.PhysicalPath, bytes);
            }
            var schemas = new XmlSchemaSet { XmlResolver = new XmlUrlResolver() };
            schemas.Add(null, schemaPath);
            schemas.Compile();
            var result = new SdkInstanceInheritanceProfile(schemas, paths, expressions: true).Apply(entry, snapshots[entry]);
            if (result.Bytes == null) throw new InvalidDataException("Armor source preparation: " + string.Join("; ", result.Evidence.Diagnostics));
            var binding = SdkEffectiveSchema.Bind(schemas, entry, result.Bytes);
            if (!binding.XmlValidated || binding.Fields.Length != 0)
                throw new InvalidDataException("Prepared armor source schema: " + string.Join("; ", binding.Errors));
            var prepared = Parse(result.Bytes);
            // Reborn: metadata is removed only after the selected source profile proves/consumes every supported directive; imports are bases, not output assets.
            foreach (var metadata in prepared.DocumentElement!.ChildNodes.OfType<XmlElement>().Where(node => node.LocalName is "Tags" or "Includes" or "Defines").ToArray())
                prepared.DocumentElement.RemoveChild(metadata);
            byte[] normalized = Encoding.UTF8.GetBytes(prepared.OuterXml);
            var admitted = GeneralArmorAuthoredBuild.AdmitSet(normalized);
            VerifySnapshots();
            // Reborn: Studio validation-only requests must prepare/schema-check the whole graph without native compilation, directory or package publication.
            if (validateOnly)
            {
                Console.WriteLine(JsonSerializer.Serialize(new { ArmorCount = admitted.Length, SourceCount = paths.Sources.Length,
                    Sources = paths.Sources,
                    PreparedSha256 = Convert.ToHexString(SHA256.HashData(normalized)), ValidationOnly = true,
                    GameExecuted = false, OutputCreated = false }));
                return;
            }
            // Reborn: preparation cannot authorize overwriting a directory that appeared after the initial preflight.
            SdkEnvironmentPreflight.CheckPath(Path.GetDirectoryName(output)!);
            if (Directory.Exists(output) || File.Exists(output)) throw new InvalidDataException("Armor source output already exists.");
            Directory.CreateDirectory(output);
            string preparedPath = Path.Combine(output, "prepared-armors.xml"); File.WriteAllBytes(preparedPath, normalized);
            string compiled = Path.Combine(output, "compiled"); GeneralArmorAuthoredBuild.Run(preparedPath, compiled, true);
            VerifySnapshots();
            using var build = JsonDocument.Parse(File.ReadAllText(Path.Combine(compiled, "general-armor-build.json")));
            // Reborn: preserve raw closure and processed-owner identities separately from the native build receipt and runtime evidence.
            var receipt = new { SourceRoot = sourceRoot, SourceEntry = entry, Sources = paths.Sources,
                Preparation = result.Evidence, PreparedSha256 = Convert.ToHexString(SHA256.HashData(normalized)),
                OwnerOnlyOutput = true, SourceSharingLocksHeldThroughBuild = true, SourceSnapshotsVerified = true,
                ArmorCount = build.RootElement.GetProperty("ArmorCount").GetInt32(),
                BuildReceipt = Path.Combine(compiled, "general-armor-build.json"),
                DefaultReaderPackagePath = build.RootElement.GetProperty("DefaultReaderPackagePath").GetString(),
                AlphaCapabilitiesExpanded = false, GameExecuted = false, RuntimeOverrideProven = false };
            string json = JsonSerializer.Serialize(receipt, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(Path.Combine(output, "armor-source-build.json"), json); Console.WriteLine(json);

            //-------------------------------------------------------------------------------------------------
            /** Reborn: recheck held file handles and canonical paths against the complete captured closure before success publication. */
            //-------------------------------------------------------------------------------------------------
            void VerifySnapshots()
            {
                foreach (var source in paths.Sources)
                {
                    SdkEnvironmentPreflight.CheckPath(source.PhysicalPath);
                    if (!File.ReadAllBytes(source.PhysicalPath).SequenceEqual(snapshots[source.PhysicalPath]))
                        throw new InvalidDataException("Armor source snapshot changed during build.");
                }
            }
        }
        finally { foreach (var stream in locks) stream.Dispose(); }
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: validate explicit confined source/output geometry for the focused schema harness without falsely claiming full catalog parity. */
    //-------------------------------------------------------------------------------------------------
    private static SdkEnvironmentPreflight.Report SourceEnvironment(string root, string entry, string output, string schemaPath)
    {
        string schemaRoot = SdkEnvironmentPreflight.DirectoryPath(Path.GetDirectoryName(schemaPath)!);
        SdkEnvironmentPreflight.CheckPath(schemaPath);
        SdkEnvironmentPreflight.DirectoryPath(Path.GetDirectoryName(output)!);
        if (!entry.EndsWith(".xml", StringComparison.OrdinalIgnoreCase) || !SdkEnvironmentPreflight.Inside(root, entry)
            || SdkEnvironmentPreflight.Inside(root, output) || SdkEnvironmentPreflight.Inside(schemaRoot, output)
            || Directory.Exists(output) || File.Exists(output)) throw new InvalidDataException("Focused armor entry/output geometry is unsupported or occupied.");
        byte[] bytes = SdkEnvironmentPreflight.Read(entry, 65536); Parse(bytes);
        return new("ra3ep1", schemaRoot, schemaPath, 0, "", root, entry, Convert.ToHexString(SHA256.HashData(bytes)), output,
            Array.Empty<SdkEnvironmentPreflight.Mapping>(), true, true, false, false, false, "0x5454A8E9",
            new[] { "Focused armor path geometry only; full SDK schema catalog parity is not asserted." });
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: parse bounded XML with no entity resolution or hidden PI/text nodes; retain source values for the existing profile. */
    //-------------------------------------------------------------------------------------------------
    private static XmlDocument Parse(byte[] bytes)
    {
        using var input = new MemoryStream(bytes, false);
        using var reader = XmlReader.Create(input, new XmlReaderSettings { DtdProcessing = DtdProcessing.Prohibit, XmlResolver = null,
            IgnoreWhitespace = true, IgnoreComments = true, MaxCharactersInDocument = 4 * 1048576 });
        var xml = new XmlDocument { XmlResolver = null }; xml.Load(reader);
        if (xml.ChildNodes.Cast<XmlNode>().Any(node => node is not XmlElement && node is not XmlDeclaration)
            || xml.DocumentElement?.Name != "AssetDeclaration" || xml.DocumentElement.NamespaceURI != Ea)
            throw new InvalidDataException("Armor sources require plain EA AssetDeclaration XML.");
        return xml;
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: refuse mixed types and meaningful Tags before generic profiles can ignore them; only reviewed armor/source directives enter preparation. */
    //-------------------------------------------------------------------------------------------------
    private static void CheckSourceShape(byte[] bytes)
    {
        var xml = Parse(bytes); var root = xml.DocumentElement!;
        if (root.Attributes.OfType<XmlAttribute>().Any(attribute => attribute.Name != "xmlns" || attribute.Value != Ea))
            throw new InvalidDataException("Unsupported armor source root attribute.");
        foreach (XmlNode node in root.ChildNodes)
        {
            if (node is not XmlElement element || element.NamespaceURI != Ea || element.Prefix.Length != 0
                || element.LocalName is not ("ArmorTemplate" or "Tags" or "Includes" or "Defines"))
                throw new InvalidDataException("Only armor assets and reviewed metadata may enter the source bridge.");
            if (element.LocalName is "Tags" or "Includes" or "Defines")
            {
                if (element.HasAttributes || element.LocalName == "Tags" && element.HasChildNodes)
                    throw new InvalidDataException("Nonempty Tags or metadata attributes remain unsupported.");
            }
        }
        if (root.SelectNodes(".//processing-instruction()")!.Count != 0) throw new InvalidDataException("Armor source processing instructions are unsupported.");
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: jointly prove definitions, local/imported chains, defining contexts and identical native packages while rejecting unsupported graphs before output. */
    //-------------------------------------------------------------------------------------------------
    internal static void SelfTest(string directory)
    {
        directory = Path.GetFullPath(directory);
        if (Directory.Exists(directory) || File.Exists(directory)) throw new InvalidDataException("Armor source test output exists.");
        Directory.CreateDirectory(directory);
        string data = Path.Combine(directory, "src"); Directory.CreateDirectory(data);
        string entry = Path.Combine(data, "Entry.xml"), middle = Path.Combine(data, "Middle.xml"), basis = Path.Combine(data, "Base.xml"), definitions = Path.Combine(data, "Defs.xml");
        const string defs = "<Defines><Define name='GUN' value='250.5'/><Define name='ROOT' value='75.5%'/><Define name='ALIAS' value='=$GUN'/><Define name='LEFT' value='50'/><Define name='RIGHT' value='25'/><Define name='SIDE' value='=($LEFT*2)+$RIGHT'/></Defines>";
        const string baseBody = "<Includes><Include type='all' source='DATA:Defs.xml'/></Includes><ArmorTemplate id='RemoteBase' Default='=$ROOT'><Armor Damage='GUN' Percent='=$ALIAS'/></ArmorTemplate>";
        const string middleBody = "<Includes><Include type='instance' source='Base.xml'/></Includes><ArmorTemplate id='MiddleBase' inheritFrom='RemoteBase' DamageScalar='50'/>";
        const string ownerBody = "<Includes><Include type='instance' source='Middle.xml'/><Include type='all' source='Defs.xml'/></Includes><Defines><Define name='OWN' value='40'/></Defines><ArmorTemplate id='Owner' inheritFrom='ArmorTemplate:MiddleBase' SideDamageScalar='=$SIDE'/><ArmorTemplate id='LocalBase' Default='=$OWN'/><ArmorTemplate id='LocalDerived' inheritFrom='LocalBase' RearDamageScalar='25%'><Armor Damage='CANNON' Percent='10'/></ArmorTemplate>";
        Fixture(ownerBody);
        var before = Directory.GetFiles(data).ToDictionary(path => path, File.ReadAllBytes);
        // Reborn: preparation-only validation must not create output or depend on a full CnC3Types catalog in deployment.
        string validation = Path.Combine(directory, "v"); Run(data, entry, validation, true);
        if (Directory.Exists(validation)) throw new InvalidDataException("Validation-only armor source created output.");
        string output = Path.Combine(directory, "b"); Run(data, entry, output);
        string repeat = Path.Combine(directory, "r"); Run(data, entry, repeat);
        using var receipt = JsonDocument.Parse(File.ReadAllText(Path.Combine(output, "armor-source-build.json")));
        if (receipt.RootElement.GetProperty("ArmorCount").GetInt32() != 3 || receipt.RootElement.GetProperty("Sources").GetArrayLength() != 4
            || receipt.RootElement.GetProperty("Preparation").GetProperty("ImportedBases").GetArrayLength() != 1)
            throw new InvalidDataException("Armor source closure/owner selection differs.");
        var prepared = Parse(File.ReadAllBytes(Path.Combine(output, "prepared-armors.xml")));
        var owner = prepared.DocumentElement!.ChildNodes.OfType<XmlElement>().Single(node => node.GetAttribute("id") == "Owner");
        if (owner.GetAttribute("Default") != "75.5%" || owner.GetAttribute("DamageScalar") != "50" || owner.GetAttribute("SideDamageScalar") != "125"
            || owner.ChildNodes.OfType<XmlElement>().Single().GetAttribute("Percent") != "250.5" || owner.HasAttribute("inheritFrom"))
            throw new InvalidDataException("Armor source definition/inheritance values differ.");
        // Reborn: compare with an independently authored literal declaration, not only the prepared document replay.
        string literal = Path.Combine(directory, "literal.xml");
        File.WriteAllBytes(literal, Source("<ArmorTemplate id='Owner' Default='75.5%' DamageScalar='50' SideDamageScalar='125'><Armor Damage='GUN' Percent='250.5'/></ArmorTemplate><ArmorTemplate id='LocalBase' Default='40'/><ArmorTemplate id='LocalDerived' Default='40' RearDamageScalar='25%'><Armor Damage='CANNON' Percent='10'/></ArmorTemplate>"));
        string literalOutput = Path.Combine(directory, "l"); GeneralArmorAuthoredBuild.Run(literal, literalOutput, true);
        byte[] expected = Package(literalOutput, "general-armor-build.json");
        if (!Package(output, "armor-source-build.json").SequenceEqual(expected) || !Package(repeat, "armor-source-build.json").SequenceEqual(expected)
            || before.Any(row => !File.ReadAllBytes(row.Key).SequenceEqual(row.Value)))
            throw new InvalidDataException("Armor source literal/repeat/source preservation differs.");
        Refuse(() => Run(data, entry, output));
        // Reborn: exercise changed source values, not merely constant recipes or cached preparation evidence.
        File.WriteAllBytes(definitions, Source(defs.Replace("250.5", "5000", StringComparison.Ordinal)));
        string changed = Path.Combine(directory, "c"); Run(data, entry, changed);
        if (Package(changed, "armor-source-build.json").SequenceEqual(expected)) throw new InvalidDataException("Changed definition did not alter the native package.");
        // Reborn: prove two-sided unkeyed armor append across imported/local chains, not guessed Damage-key replacement or one-sided copying.
        string appendedOwner = ownerBody.Replace("SideDamageScalar='=$SIDE'/>", "SideDamageScalar='=$SIDE'><Armor Damage='CANNON' Percent='=$OWN'/></ArmorTemplate>", StringComparison.Ordinal)
            .Replace("<ArmorTemplate id='LocalBase' Default='=$OWN'/>", "<ArmorTemplate id='LocalBase' Default='=$OWN'><Armor Damage='GUN' Percent='=$ALIAS'/></ArmorTemplate>", StringComparison.Ordinal);
        Fixture(appendedOwner);
        File.WriteAllBytes(middle, Source(middleBody.Replace("DamageScalar='50'/>", "DamageScalar='50'><Armor Damage='SNIPER' Percent='25%'/></ArmorTemplate>", StringComparison.Ordinal)));
        string appended = Path.Combine(directory, "append"); Run(data, entry, appended);
        var appendedXml = Parse(File.ReadAllBytes(Path.Combine(appended, "prepared-armors.xml")));
        var appendedAssets = appendedXml.DocumentElement!.ChildNodes.OfType<XmlElement>().ToArray();
        string[] expectedDamage = { "GUN", "SNIPER", "CANNON" };
        if (!appendedAssets.Single(node => node.GetAttribute("id") == "Owner").ChildNodes.OfType<XmlElement>().Select(node => node.GetAttribute("Damage")).SequenceEqual(expectedDamage)
            || !appendedAssets.Single(node => node.GetAttribute("id") == "LocalDerived").ChildNodes.OfType<XmlElement>().Select(node => node.GetAttribute("Damage")).SequenceEqual(new[] { "GUN", "CANNON" }))
            throw new InvalidDataException("Imported/local armor append order differs from Core inheritance.");
        string appendLiteral = Path.Combine(directory, "append-literal.xml");
        File.WriteAllBytes(appendLiteral, Source("<ArmorTemplate id='Owner' Default='75.5%' DamageScalar='50' SideDamageScalar='125'><Armor Damage='GUN' Percent='250.5'/><Armor Damage='SNIPER' Percent='25%'/><Armor Damage='CANNON' Percent='40'/></ArmorTemplate><ArmorTemplate id='LocalBase' Default='40'><Armor Damage='GUN' Percent='250.5'/></ArmorTemplate><ArmorTemplate id='LocalDerived' Default='40' RearDamageScalar='25%'><Armor Damage='GUN' Percent='250.5'/><Armor Damage='CANNON' Percent='10'/></ArmorTemplate>"));
        string appendExpected = Path.Combine(directory, "append-expected"); GeneralArmorAuthoredBuild.Run(appendLiteral, appendExpected, true);
        byte[] appendPackage = Package(appended, "armor-source-build.json");
        if (!appendPackage.SequenceEqual(Package(appendExpected, "general-armor-build.json")))
            throw new InvalidDataException("Two-sided armor append does not reproduce independent literal package.");
        // Reborn: changed owner append coefficients must affect output; overlapping Damage remains a pre-output refusal, not last-wins normalization.
        Fixture(appendedOwner.Replace("Percent='=$OWN'", "Percent='80'", StringComparison.Ordinal));
        File.WriteAllBytes(middle, Source(middleBody.Replace("DamageScalar='50'/>", "DamageScalar='50'><Armor Damage='SNIPER' Percent='25%'/></ArmorTemplate>", StringComparison.Ordinal)));
        string appendChanged = Path.Combine(directory, "append-changed"); Run(data, entry, appendChanged);
        if (Package(appendChanged, "armor-source-build.json").SequenceEqual(appendPackage))
            throw new InvalidDataException("Changed appended coefficient did not alter package.");
        string[] faults = {
            ownerBody.Replace("=$SIDE", "=$MISSING", StringComparison.Ordinal),
            ownerBody.Replace("=$SIDE", "=1+2", StringComparison.Ordinal),
            ownerBody.Replace("ArmorTemplate:MiddleBase", "MissingBase", StringComparison.Ordinal),
            ownerBody.Replace("ArmorTemplate:MiddleBase", "WeaponTemplate:MiddleBase", StringComparison.Ordinal),
            ownerBody.Replace("type='instance'", "type='reference'", StringComparison.Ordinal),
            ownerBody.Replace("Middle.xml", "../Escape.xml", StringComparison.Ordinal),
            ownerBody.Replace("<ArmorTemplate id='LocalBase' Default='=$OWN'/>", "<GameObject id='Other'/>", StringComparison.Ordinal),
            ownerBody.Replace("id='Owner'", "id='=$OWN'", StringComparison.Ordinal),
            ownerBody.Replace("SideDamageScalar='=$SIDE'", "SideDamageScalar='10001'", StringComparison.Ordinal),
            ownerBody.Replace("<Defines>", "<Tags><Tag name='x' tag='y'/></Tags><Defines>", StringComparison.Ordinal),
            ownerBody.Replace("id='LocalDerived'", "id='localbase'", StringComparison.Ordinal),
            ownerBody.Replace("inheritFrom='LocalBase'", "inheritFrom='LocalDerived'", StringComparison.Ordinal),
            ownerBody.Replace("<ArmorTemplate id='Owner'", "<?hidden x?><ArmorTemplate id='Owner'", StringComparison.Ordinal),
            ownerBody.Replace("SideDamageScalar='=$SIDE'/>", "SideDamageScalar='=$SIDE'><Armor Damage='GUN' Percent='10'/></ArmorTemplate>", StringComparison.Ordinal),
            ownerBody.Replace("name='OWN'", "name='GUN'", StringComparison.Ordinal),
            ownerBody.Replace("id='Owner'", "id='remotebase'", StringComparison.Ordinal),
            ownerBody.Replace("value='40'", "value='40' override='true'", StringComparison.Ordinal)
        };
        for (int index = 0; index < faults.Length; index++)
        {
            Fixture(faults[index]); string refused = Path.Combine(directory, "n" + index);
            byte[] raw = File.ReadAllBytes(entry); Refuse(() => Run(data, entry, refused));
            if (Directory.Exists(refused) || !File.ReadAllBytes(entry).SequenceEqual(raw)) throw new InvalidDataException("Invalid armor graph published output or changed input.");
        }
        Fixture(ownerBody); File.WriteAllBytes(basis, Source("<Includes><Include type='instance' source='Entry.xml'/></Includes><ArmorTemplate id='RemoteBase'/>"));
        Refuse(() => Run(data, entry, Path.Combine(directory, "cycle")));
        Fixture(ownerBody); File.Delete(definitions); Refuse(() => Run(data, entry, Path.Combine(directory, "missing")));
        Console.WriteLine("Armor source bridge: PASS (4-file instance/metadata closure, imported/local chains, two-sided distinct-Damage append, definition alias/arithmetic contexts, independent literal package equality/changed-input controls and nineteen pre-output refusals; no game).");

        //-------------------------------------------------------------------------------------------------
        /** Reborn: create only owned XML fixtures; reference XSD and external game data remain unchanged. */
        //-------------------------------------------------------------------------------------------------
        void Fixture(string body)
        {
            File.WriteAllBytes(entry, Source(body)); File.WriteAllBytes(middle, Source(middleBody));
            File.WriteAllBytes(basis, Source(baseBody)); File.WriteAllBytes(definitions, Source(defs));
        }

        //-------------------------------------------------------------------------------------------------
        /** Reborn: require expected admission failures rather than accepting successful partial preparation or unrelated exceptions. */
        //-------------------------------------------------------------------------------------------------
        static void Refuse(Action action)
        {
            try { action(); } catch (Exception error) when (error is InvalidDataException or XmlException) { return; }
            throw new InvalidDataException("Unsupported armor source graph was accepted.");
        }

        //-------------------------------------------------------------------------------------------------
        /** Reborn: compare the actual default-reader archives named by successful receipts. */
        //-------------------------------------------------------------------------------------------------
        static byte[] Package(string root, string receiptName)
        {
            using var json = JsonDocument.Parse(File.ReadAllText(Path.Combine(root, receiptName)));
            return File.ReadAllBytes(json.RootElement.GetProperty("DefaultReaderPackagePath").GetString()!);
        }
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: encode owned source fixtures using the normal EA namespace without altering reference XML. */
    //-------------------------------------------------------------------------------------------------
    private static byte[] Source(string body) => Encoding.UTF8.GetBytes("<AssetDeclaration xmlns='" + Ea + "'>" + body + "</AssetDeclaration>");
}
