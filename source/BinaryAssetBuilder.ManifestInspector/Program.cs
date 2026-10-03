using System.Text.Json;

namespace BinaryAssetBuilder.ManifestInspector;

internal static class Program
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true
    };

    public static int Main(string[] args)
    {
        try
        {
            // Reborn: native AudioFile serialization proof optionally compares actual selected stock slices without registering an audio processor.
            if (args.FirstOrDefault() == "audiofile-serializer-self-test") { AudioFileSerializationSmokeTest.Run(args.Skip(1).ToArray()); return 0; }
            // Reborn: inspect audio library API evidence without invoking DLL entry points or codecs.
            if (args.FirstOrDefault() == "native-audio-api-audit" && args.Length == 2) { NativeAudioApiProbe.Run(args[1]); return 0; }
            // Reborn: default-compatible audio regressions inspect PE data and managed WAV bytes but never initialize native codecs.
            if (args.FirstOrDefault() == "native-audio-api-self-test") { NativeAudioApiProbe.SelfTest(); return 0; }
            if (args.FirstOrDefault() == "audio-encoder-wav-self-test") { AudioEncoderPoc.SelfTest(); return 0; }
            // Reborn: native encoding is opt-in and runs only in this standalone inspector process, never in default compiler tests.
            if (args.FirstOrDefault() == "audio-encoder-poc" && args.Length == 2) { AudioEncoderPoc.Run(args[1]); return 0; }
            // Reborn: expose read-only native audio envelope evidence separately from compilation and codec activation.
            // Reborn: compare the four rejected records directly with bounded original BIG entries, without extraction.
            if (args.FirstOrDefault() == "audio-archive-self-test") { AudioArchiveComparisonProbe.SelfTest(); return 0; }
            if (args.FirstOrDefault() == "audio-archive-compare" && args.Length == 3) { AudioArchiveComparisonProbe.Run(args[1],args[2]); return 0; }
            // Reborn: opt into a verified four-record in-memory overlay; strict local-only custom audits are unchanged.
            if (args.FirstOrDefault() == "audio-custom-reconciled-audit" && args.Length == 3)
            { var corrections = AudioArchiveComparisonProbe.ReadCorrections(args[1],args[2]); AudioFileRuntimeProbe.Run(args[1],true,corrections); return 0; }
            if (args.FirstOrDefault() == "audiofile-runtime-self-test") { AudioFileRuntimeProbe.SelfTest(); return 0; }
            // Reborn: validate mapped custom block envelopes without enabling codecs or copying custom payloads.
            if (args.FirstOrDefault() == "audio-custom-self-test") { AudioCustomDataProbe.SelfTest(); return 0; }
            if (args.FirstOrDefault() == "audio-custom-audit" && args.Length == 2) { AudioFileRuntimeProbe.Run(args[1],true); return 0; }
            if (args.FirstOrDefault() == "audiofile-runtime-audit" && args.Length == 2) { AudioFileRuntimeProbe.Run(args[1]); return 0; }
            // Reborn: general checked AudioEvent admission optionally exercises actual external AudioFile metadata.
            if (args.FirstOrDefault() == "diagnostic-audioevent-build-self-test")
            {
                CompilerSmokeTest.InitializeHashProvider(); DiagnosticAudioEventBuildSmokeTest.Run(args.Skip(1).ToArray()); return 0;
            }
            // Reborn: fixed local event/sound/FX proof optionally resolves real AudioFile metadata without public root admission.
            if (args.FirstOrDefault() == "audioevent-fx-stream-self-test")
            {
                CompilerSmokeTest.InitializeHashProvider(); AudioEventFXStreamSmokeTest.Run(args.Skip(1).ToArray()); return 0;
            }
            // Reborn: checked isolated AudioEvent entries optionally resolve real AudioFile metadata and compare stock slices.
            if (args.FirstOrDefault() == "ep1-audioevent-profile-self-test")
            {
                CompilerSmokeTest.InitializeHashProvider(); Ep1AudioEventProfileSmokeTest.Run(args.Skip(1).ToArray()); return 0;
            }
            // Reborn: isolated native AudioEvent evidence optionally compares bounded stock slices.
            if (args.FirstOrDefault() == "audioevent-native-self-test")
            {
                CompilerSmokeTest.InitializeHashProvider(); AudioEventNativeSmokeTest.Run(args.Skip(1).ToArray()); return 0;
            }
            // Reborn: exercise public sound admission with optional real stock mappings.
            if (args.FirstOrDefault() == "diagnostic-multisound-build-self-test")
            {
                CompilerSmokeTest.InitializeHashProvider(); DiagnosticMultisoundBuildSmokeTest.Run(args.Skip(1).ToArray()); return 0;
            }
            // Reborn: fixed local sound chain proof stays separate from public command admission and game packaging.
            if (args.FirstOrDefault() == "multisound-fx-stream-self-test")
            {
                CompilerSmokeTest.InitializeHashProvider();
                MultisoundFXStreamSmokeTest.Run(args.Skip(1).ToArray());
                return 0;
            }
            // Reborn: exercise checked sound profile entries without admitting production audio output.
            if (args.FirstOrDefault() == "ep1-multisound-profile-self-test")
            {
                CompilerSmokeTest.InitializeHashProvider();
                Ep1MultisoundProfileSmokeTest.Run(args.Skip(1).ToArray());
                return 0;
            }
            // Reborn: isolated EP1 native sound evidence never enables audio production or diagnostic root admission.
            if (args.FirstOrDefault() == "multisound-native-self-test")
            {
                CompilerSmokeTest.InitializeHashProvider();
                MultisoundNativeSmokeTest.Run(args.Skip(1).ToArray());
                return 0;
            }
            // Reborn: exercise narrow FX command admission and optional real-stock comparisons without production output.
            if (args.FirstOrDefault() == "diagnostic-fx-build-self-test")
            {
                CompilerSmokeTest.InitializeHashProvider();
                DiagnosticFXBuildSmokeTest.Run(args.Skip(1).ToArray());
                return 0;
            }
            // Reborn: fixed owned modifier/FX proof remains separate from general command admission tests.
            if (args.Length == 1 && args[0] == "modifier-fx-stream-self-test")
            {
                CompilerSmokeTest.InitializeHashProvider();
                ModifierFXStreamSmokeTest.Run();
                return 0;
            }
            // Reborn: exercise the isolated FX compiler without production stream publication.
            if (args.FirstOrDefault() == "ep1-fx-profile-self-test")
            {
                CompilerSmokeTest.InitializeHashProvider();
                Ep1FXListProfileSmokeTest.Run(args.Skip(1).ToArray());
                return 0;
            }
            // Reborn: concrete external audio proof never enables FX/audio processors or production output.
            if (args.FirstOrDefault() == "fx-audio-resolution-self-test")
            {
                CompilerSmokeTest.InitializeHashProvider();
                FXAudioResolutionSmokeTest.Run(args.Skip(1).ToArray());
                return 0;
            }
            // Reborn: native FX proof reads selected stock slices and never enables production processors.
            if (args.FirstOrDefault() == "fx-native-self-test")
            {
                CompilerSmokeTest.InitializeHashProvider();
                FXListNativeSmokeTest.Run(args.Skip(1).ToArray());
                return 0;
            }
            // Reborn: exercise bounded input/publication and Include guards without accepting production SDK builds.
            if (args.Length == 1 && args[0] == "diagnostic-build-self-test")
            {
                CompilerSmokeTest.InitializeHashProvider();
                BoundedDiagnosticBuildSmokeTest.Run();
                DiagnosticIncludeBuildSmokeTest.Run();
                DiagnosticFilterBuildSmokeTest.Run();
                DiagnosticFXBuildSmokeTest.Run();
                // Reborn: aggregate command proof now includes narrow authored/local Multisound admission.
                DiagnosticMultisoundBuildSmokeTest.Run();
                // Reborn: all generic command families must include checked AudioEvent snapshot/publication tests.
                DiagnosticAudioEventBuildSmokeTest.Run();
                return 0;
            }
            // Reborn: bounded diagnostic Include build publishes only a new verified directory and never enables production/cache policies.
            if (args.FirstOrDefault() == "diagnostic-build")
            {
                if (args.Length < 3) throw new ArgumentException("diagnostic-build <source.xml> <new-output-directory> [physical.manifest=runtime.manifest ...]");
                CompilerSmokeTest.InitializeHashProvider();
                Console.WriteLine("Diagnostic build verified (NOT a playable mod): " + BoundedDiagnosticBuild.Build(args[1], args[2], args.Skip(3).ToArray()));
                return 0;
            }
            // Reborn: fixed diagnostic stream proof writes only its own temporary fixtures, never production SDK output.
            if (args.Length == 1 && args[0] == "modifier-shader-stream-self-test")
            {
                CompilerSmokeTest.InitializeHashProvider();
                ModifierShaderStreamSmokeTest.Run();
                return 0;
            }
            // Reborn: run actual include integration with mixed local/external dependency identities.
            if (args.Length == 1 && args[0] == "included-modifier-shader-self-test")
            {
                CompilerSmokeTest.InitializeHashProvider();
                IncludedModifierShaderSmokeTest.Run();
                return 0;
            }
            // Reborn: expose real modifier/shader graph proof without a production output manager.
            if (args.Length == 1 && args[0] == "modifier-shader-graph-self-test")
            {
                CompilerSmokeTest.InitializeHashProvider();
                ModifierShaderGraphSmokeTest.Run();
                return 0;
            }
            // Reborn: validate isolated shader compiler eligibility without activating production output.
            if (args.Length == 1 && args[0] == "ep1-shader-profile-self-test")
            {
                CompilerSmokeTest.InitializeHashProvider();
                Ep1ShaderOverrideProfileSmokeTest.Run();
                return 0;
            }
            // Reborn: expose recovered shader native/document proof and optional bounded stock comparisons.
            if (args.FirstOrDefault() == "shader-override-self-test")
            {
                CompilerSmokeTest.InitializeHashProvider();
                ShaderOverrideNativeSmokeTest.Run(args.Skip(1).ToArray());
                return 0;
            }
            // Reborn: exercise only the isolated filter compiler/document profile, never production output.
            if (args.Length == 1 && args[0] == "ep1-object-filter-profile-self-test")
            {
                CompilerSmokeTest.InitializeHashProvider();
                Ep1ObjectFilterProfileSmokeTest.Run();
                return 0;
            }
            // Reborn: prove native ObjectFilter root output and optional stock slices without enabling production processors.
            if (args.FirstOrDefault() == "object-filter-self-test")
            {
                CompilerSmokeTest.InitializeHashProvider();
                ObjectFilterNativeSmokeTest.Run(args.Skip(1).ToArray());
                return 0;
            }
            // Reborn: run actual dependency preparation retry/mapping checks without authorizing output emission.
            if (args.Length == 1 && args[0] == "dependency-resolution-self-test")
            {
                CompilerSmokeTest.InitializeHashProvider();
                DependencyResolutionSmokeTest.Run();
                return 0;
            }
            // Reborn: verify normalized modifier references against one-biased native imports and optional real EP1 goldens.
            if (args.FirstOrDefault() == "modifier-import-self-test")
            {
                CompilerSmokeTest.InitializeHashProvider();
                AttributeModifierImportSmokeTest.Run(args.Skip(1).ToArray());
                return 0;
            }
            // Reborn: route native EP1 modifiers through the isolated descriptor/registry/document profile.
            if (args.Length == 1 && args[0] == "ep1-modifier-profile-self-test")
            {
                CompilerSmokeTest.InitializeHashProvider();
                Ep1AttributeModifierProfileSmokeTest.Run();
                return 0;
            }
            // Reborn: validate official EP1 modifier ABI and optionally compare named bounded real-game slices.
            if (args.FirstOrDefault() == "attribute-modifier-self-test")
            {
                CompilerSmokeTest.InitializeHashProvider();
                AttributeModifierNativeSmokeTest.Run(args.Skip(1).ToArray());
                return 0;
            }
            // Reborn: test real retained and serialized document reuse without authorizing diagnostic compiler output.
            if (args.Length == 1 && args[0] == "document-reuse-self-test")
            {
                CompilerSmokeTest.InitializeHashProvider();
                DocumentReuseSmokeTest.Run();
                return 0;
            }
            // Reborn: exercise the atomic monitor/cache handoff used by the builder, including initialization failure recovery.
            if (args.Length == 1 && args[0] == "monitor-batch-self-test")
            {
                CompilerSmokeTest.InitializeHashProvider();
                MonitorBatchSmokeTest.Run();
                return 0;
            }
            // Reborn: verify watcher-driven hash invalidation and resident stream hints without native compiler output.
            if (args.Length == 1 && args[0] == "watcher-cache-self-test")
            {
                CompilerSmokeTest.InitializeHashProvider();
                WatcherCacheSmokeTest.Run();
                return 0;
            }
            // Reborn: validate file-signature invalidation and document dependency hashes without production output.
            if (args.Length == 1 && args[0] == "dependency-hash-self-test")
            {
                CompilerSmokeTest.InitializeHashProvider();
                DependencyHashSmokeTest.Run();
                return 0;
            }
            // Reborn: exercise failed-copy preservation and asset/custom-data rollback in isolated synthetic directories.
            if (args.Length == 1 && args[0] == "copy-recovery-self-test")
            {
                CompilerSmokeTest.InitializeHashProvider();
                CopyRecoverySmokeTest.Run();
                return 0;
            }
            // Reborn: audit identity checksum candidates using only valid EP1 manifest metadata.
            if (args.Length >= 2 && args[0] == "checksum-audit")
            {
                ChecksumAudit.Print(args.Skip(1).ToArray());
                return 0;
            }
            // Reborn: lock official checksum padding and patch equivalence without changing compatibility behavior.
            if (args.Length == 1 && args[0] == "checksum-self-test")
            {
                CompilerSmokeTest.InitializeHashProvider();
                ChecksumAudit.Run();
                return 0;
            }
            // Reborn: validate real intermediate commits and coordinated linking with tiny synthetic diagnostic assets.
            if (args.Length == 2 && args[0] == "linked-stream-self-test")
            {
                CompilerSmokeTest.InitializeHashProvider();
                LinkedStreamSmokeTest.Run(args[1]);
                return 0;
            }
            // Reborn: exercise focused full-document stages and experimental cache guards without production output.
            if (args.Length == 2 && args[0] == "ep1-armor-document-self-test")
            {
                CompilerSmokeTest.InitializeHashProvider();
                Ep1ArmorDocumentSmokeTest.Run(args[1]);
                return 0;
            }
            // Reborn: exercise the opt-in EP1 profile through the actual descriptor and compiler entry point.
            if (args.Length == 1 && args[0] == "ep1-armor-profile-self-test")
            {
                CompilerSmokeTest.InitializeHashProvider();
                Ep1ArmorProfileSmokeTest.Run();
                return 0;
            }
            // Reborn: isolate experimental armor token/writer validation from production registry activation.
            if (args.FirstOrDefault() == "armor-token-self-test" && args.Length is 2 or 3)
            {
                ArmorTokenSmokeTest.Run(args[1], args.Length == 3 ? args[2] : null);
                return 0;
            }
            // Reborn: handle the metadata-only type audit independently of the older command whitelist.
            if (args.FirstOrDefault() == "type-audit" && args.Length >= 2)
            {
                TypeRegistryAudit.Print(args.Skip(1).Where(value => value != "--json").ToArray(), args.Contains("--json"));
                return 0;
            }
            // Reborn: expose focused schema-to-import validation with optional real EP1 manifests.
            if ((args.Length < 2 && args.FirstOrDefault() is not ("layout-self-test" or "compiler-self-test")) || args.FirstOrDefault() is not ("inspect" or "verify" or "compare" or "schema-diff" or "writer-self-test" or "utility-verify" or "assembly-fields" or "assembly-methods" or "assembly-il" or "assembly-size-diff" or "current-layout" or "layout-self-test" or "compiler-self-test" or "reference-self-test" or "external-link-self-test" or "asset-bytes" or "hash"))
            {
                PrintUsage();
                return 2;
            }

            var command = args[0];
            // Reborn: generate small link metadata fixtures and optionally validate the actual external EP1 lookup.
            if (command == "external-link-self-test")
            {
                CompilerSmokeTest.InitializeHashProvider();
                ExternalLinkSmokeTest.Run(args[1], args.Skip(2).ToArray());
                return 0;
            }
            // Reborn: reuse the production schema reference stage and optionally check its targets in game manifests.
            if (command == "reference-self-test")
            {
                CompilerSmokeTest.InitializeHashProvider();
                ReferencePipelineSmokeTest.Run(args[1], args.Skip(2).ToArray());
                return 0;
            }
            if (command == "hash")
            {
                foreach (string value in args.Skip(1))
                {
                    Console.WriteLine($"0x{BinaryAssetBuilder.Core.Hashing.FastHash.GetHashCode(value):X8} {value}");
                }
                return 0;
            }
            if (command == "compiler-self-test")
            {
                CompilerSmokeTest.Run();
                return 0;
            }

            if (command == "asset-bytes")
            {
                if (args.Length < 4)
                {
                    PrintUsage();
                    return 2;
                }
                AssetStreamProbe.Print(
                    args[1], args[2], args[3], GetOption(args, "--entry"),
                    GetOption(args, "--bin-entry"), GetOption(args, "--asset"),
                    ParseUInt32Option(args, "--find-u32"),
                    ParseInt32Option(args, "--offset"), ParseInt32Option(args, "--count"),
                    GetOption(args, "--relo"), GetOption(args, "--relo-entry"),
                    GetOption(args, "--imp"), GetOption(args, "--imp-entry"));
                return 0;
            }

            if (command == "layout-self-test")
            {
                UprisingLayoutSmokeTest.Run();
                return 0;
            }

            if (command == "assembly-il")
            {
                if (args.Length < 3)
                {
                    PrintUsage();
                    return 2;
                }
                IlDisassembler.Print(args[1], args[2]);
                return 0;
            }

            if (command == "current-layout")
            {
                CurrentLayoutProbe.Print(args[1]);
                return 0;
            }

            // Reborn: expose official-to-current ABI drift as a repeatable migration audit.
            if (command == "assembly-size-diff")
            {
                AssemblySizeDiffProbe.Print(args[1], GetOption(args, "--top"));
                return 0;
            }

            if (command == "assembly-fields")
            {
                if (args.Length < 3)
                {
                    PrintUsage();
                    return 2;
                }
                AssemblyFieldProbe.Print(args[1], args[2]);
                return 0;
            }

            if (command == "assembly-methods")
            {
                if (args.Length < 3)
                {
                    PrintUsage();
                    return 2;
                }
                AssemblyFieldProbe.PrintMethods(args[1], args[2], args.Length > 3 ? args[3] : null);
                return 0;
            }

            if (command == "writer-self-test")
            {
                WriterSmokeTest.Run(args[1]);
                return 0;
            }

            if (command == "utility-verify")
            {
                UtilityManifestVerifier.Verify(args[1], GetOption(args, "--entry"));
                return 0;
            }

            if (command == "schema-diff")
            {
                return CompareSchemas(args);
            }

            if (command == "compare")
            {
                return Compare(args);
            }

            var path = Path.GetFullPath(args[1]);
            var entryName = GetOption(args, "--entry");
            var json = args.Contains("--json", StringComparer.OrdinalIgnoreCase);
            var documents = LoadDocuments(path, entryName);

            if (json)
            {
                Console.WriteLine(JsonSerializer.Serialize(documents.Select(ToReport), JsonOptions));
            }
            else
            {
                foreach (var (source, document) in documents)
                {
                    PrintDocument(source, document);
                }
            }

            if (command == "verify")
            {
                var errors = documents
                    .SelectMany(item => item.Document.Validate().Select(error => $"{item.Source}: {error}"))
                    .ToArray();
                foreach (var error in errors)
                {
                    Console.Error.WriteLine($"ERROR: {error}");
                }

                return errors.Length == 0 ? 0 : 1;
            }

            return 0;
        }
        catch (Exception exception)
        {
            Console.Error.WriteLine($"ERROR: {exception}");
            return 1;
        }
    }

    private static int CompareSchemas(string[] args)
    {
        if (args.Length < 3)
        {
            PrintUsage();
            return 2;
        }

        var differences = SchemaComparer.Compare(args[1], args[2]);
        if (args.Contains("--json", StringComparer.OrdinalIgnoreCase))
        {
            Console.WriteLine(JsonSerializer.Serialize(differences, JsonOptions));
        }
        else
        {
            foreach (var difference in differences)
            {
                Console.WriteLine($"{difference.Status,-7} {difference.Path}");
            }

            Console.WriteLine(
                $"Summary: Added={differences.Count(item => item.Status == "Added")}, " +
                $"Removed={differences.Count(item => item.Status == "Removed")}, " +
                $"Changed={differences.Count(item => item.Status == "Changed")}");
        }

        return 0;
    }

    private static int Compare(string[] args)
    {
        if (args.Length < 3)
        {
            PrintUsage();
            return 2;
        }

        var left = LoadDocuments(Path.GetFullPath(args[1]), GetOption(args, "--left-entry"));
        var right = LoadDocuments(Path.GetFullPath(args[2]), GetOption(args, "--right-entry"));
        if (left.Count != 1 || right.Count != 1)
        {
            throw new ArgumentException("Compare requires exactly one manifest on each side; select BIG entries explicitly.");
        }

        var leftTypes = BuildTypeMap(left[0].Document);
        var rightTypes = BuildTypeMap(right[0].Document);
        var names = leftTypes.Keys.Union(rightTypes.Keys).OrderBy(name => name).ToArray();
        var rows = names.Select(name => new
        {
            TypeName = name,
            Left = leftTypes.GetValueOrDefault(name),
            Right = rightTypes.GetValueOrDefault(name),
            Status = !leftTypes.ContainsKey(name) ? "Added" :
                !rightTypes.ContainsKey(name) ? "Removed" :
                leftTypes[name] != rightTypes[name] ? "Changed" : "Same"
        }).ToArray();

        if (args.Contains("--json", StringComparer.OrdinalIgnoreCase))
        {
            Console.WriteLine(JsonSerializer.Serialize(rows, JsonOptions));
        }
        else
        {
            Console.WriteLine($"LEFT  {left[0].Source}");
            Console.WriteLine($"RIGHT {right[0].Source}");
            foreach (var row in rows.Where(row => row.Status != "Same"))
            {
                Console.WriteLine(
                    $"{row.Status,-7} {row.TypeName,-36} " +
                    $"{FormatFingerprint(row.Left),24} -> {FormatFingerprint(row.Right),24}");
            }

            Console.WriteLine(
                $"Summary: Added={rows.Count(row => row.Status == "Added")}, " +
                $"Removed={rows.Count(row => row.Status == "Removed")}, " +
                $"Changed={rows.Count(row => row.Status == "Changed")}, " +
                $"Same={rows.Count(row => row.Status == "Same")}");
        }

        return 0;
    }

    private static Dictionary<string, TypeFingerprint> BuildTypeMap(ManifestDocument document) =>
        document.Assets
            .GroupBy(asset => asset.TypeName)
            .ToDictionary(
                group => group.Key,
                group =>
                {
                    var sample = group.First();
                    return new TypeFingerprint(sample.TypeId, sample.TypeHash, sample.Tokenized);
                });

    private static string FormatFingerprint(TypeFingerprint? value) => value is null
        ? "-"
        : $"0x{value.TypeId:X8}/0x{value.TypeHash:X8}/T{value.Tokenized?.ToString() ?? "-"}";

    private static IReadOnlyList<(string Source, ManifestDocument Document)> LoadDocuments(
        string path,
        string? entryName)
    {
        if (!path.EndsWith(".big", StringComparison.OrdinalIgnoreCase))
        {
            return [(path, ManifestReader.Read(File.ReadAllBytes(path)))];
        }

        using var archive = BigArchive.Open(path);
        var entries = archive.Entries.Where(entry =>
            entry.Name.EndsWith(".manifest", StringComparison.OrdinalIgnoreCase) &&
            (entryName is null || entry.Name.Equals(entryName, StringComparison.OrdinalIgnoreCase)))
            .ToArray();
        if (entries.Length == 0)
        {
            throw new InvalidDataException(
                entryName is null
                    ? "BIG archive contains no .manifest entries."
                    : $"BIG archive does not contain manifest entry '{entryName}'.");
        }

        return entries
            .Select(entry => ($"{path}::{entry.Name}", ManifestReader.Read(archive.ReadEntry(entry))))
            .ToArray();
    }

    private static string? GetOption(IReadOnlyList<string> args, string name)
    {
        for (var index = 0; index < args.Count; index++)
        {
            if (args[index].Equals(name, StringComparison.OrdinalIgnoreCase))
            {
                if (index + 1 >= args.Count)
                {
                    throw new ArgumentException($"Missing value for {name}.");
                }

                return args[index + 1];
            }
        }

        return null;
    }

    private static uint? ParseUInt32Option(IReadOnlyList<string> args, string name)
    {
        string? value = GetOption(args, name);
        if (value is null)
        {
            return null;
        }

        string digits = value.StartsWith("0x", StringComparison.OrdinalIgnoreCase) ? value[2..] : value;
        if (!uint.TryParse(digits, System.Globalization.NumberStyles.HexNumber,
                System.Globalization.CultureInfo.InvariantCulture, out uint result))
        {
            throw new ArgumentException($"Invalid hexadecimal 32-bit value '{value}' for {name}.");
        }

        return result;
    }

    private static int? ParseInt32Option(IReadOnlyList<string> args, string name)
    {
        string? value = GetOption(args, name);
        if (value is null)
        {
            return null;
        }

        bool hexadecimal = value.StartsWith("0x", StringComparison.OrdinalIgnoreCase);
        string digits = hexadecimal ? value[2..] : value;
        System.Globalization.NumberStyles style = hexadecimal
            ? System.Globalization.NumberStyles.HexNumber
            : System.Globalization.NumberStyles.Integer;
        if (!int.TryParse(digits, style, System.Globalization.CultureInfo.InvariantCulture, out int result))
        {
            throw new ArgumentException($"Invalid non-negative 32-bit value '{value}' for {name}.");
        }

        return result;
    }

    private static void PrintDocument(string source, ManifestDocument document)
    {
        var header = document.Header;
        Console.WriteLine(source);
        Console.WriteLine(
            $"  Version={header.Version} Linked={header.IsLinked} Checksum=0x{header.StreamChecksum:X8} " +
            $"AllTypesHash=0x{header.AllTypesHash:X8}");
        Console.WriteLine(
            $"  Profile={document.Profile?.Name ?? "Unknown"} Assets={header.AssetCount:N0} " +
            $"InstanceBytes={header.TotalInstanceDataSize:N0} RefPack={document.WasRefPackCompressed}");
        Console.WriteLine(
            $"  References={string.Join(", ", document.ReferencedManifests.Select(reference =>
                reference.IsPatch ? $"{reference.Path} (patch)" : reference.Path).DefaultIfEmpty("(none)"))}");
        foreach (var group in document.Assets
                     .GroupBy(asset => asset.TypeName)
                     .OrderByDescending(group => group.Count())
                     .ThenBy(group => group.Key)
                     .Take(20))
        {
            var sample = group.First();
            Console.WriteLine(
                $"  {group.Count(),6:N0} {group.Key,-32} TypeId=0x{sample.TypeId:X8} TypeHash=0x{sample.TypeHash:X8}");
        }

        var errors = document.Validate();
        Console.WriteLine(errors.Count == 0 ? "  Validation=OK" : $"  Validation={errors.Count} error(s)");
    }

    private static object ToReport((string Source, ManifestDocument Document) item) => new
    {
        item.Source,
        item.Document.Profile?.Name,
        item.Document.Header,
        item.Document.ReferencedManifests,
        item.Document.WasRefPackCompressed,
        ValidationErrors = item.Document.Validate(),
        Types = item.Document.Assets
            .GroupBy(asset => new { asset.TypeName, asset.TypeId, asset.TypeHash, asset.Tokenized })
            .Select(group => new
            {
                group.Key.TypeName,
                TypeId = $"0x{group.Key.TypeId:X8}",
                TypeHash = $"0x{group.Key.TypeHash:X8}",
                group.Key.Tokenized,
                Count = group.Count()
            })
            .OrderByDescending(group => group.Count)
            .ThenBy(group => group.TypeName)
    };

    private static void PrintUsage()
    {
        Console.WriteLine("BinaryAssetBuilder.ManifestInspector");
        Console.WriteLine("  inspect <manifest-or-big> [--entry <BIG entry>] [--json]");
        Console.WriteLine("  verify  <manifest-or-big> [--entry <BIG entry>] [--json]");
        Console.WriteLine("  compare <left> <right> [--left-entry <entry>] [--right-entry <entry>] [--json]");
        Console.WriteLine("  schema-diff <left-xsd-directory> <right-xsd-directory> [--json]");
        Console.WriteLine("  writer-self-test <output-manifest>");
        // Reborn: this command writes only isolated synthetic asset/link regression fixtures.
        Console.WriteLine("  linked-stream-self-test <output-directory>");
        // Reborn: checksum auditing reads manifest metadata only and does not certify payload integrity.
        Console.WriteLine("  checksum-audit <ep1-manifest ...>");
        Console.WriteLine("  checksum-self-test");
        // Reborn: file-lock and rollback fixtures never operate on real build caches or game files.
        Console.WriteLine("  copy-recovery-self-test");
        // Reborn: dependency fixtures compare source identities, not approved native compiler output.
        Console.WriteLine("  dependency-hash-self-test");
        // Reborn: this checks strict output dependency metadata, not production compiler readiness.
        Console.WriteLine("  dependency-resolution-self-test");
        // Reborn: optional real manifests require all eleven source-derived filter goldens.
        Console.WriteLine("  object-filter-self-test [ep1-static-manifest ...]");
        // Reborn: this profile command keeps production/cache and unproven filter combinations disabled.
        Console.WriteLine("  ep1-object-filter-profile-self-test");
        // Reborn: shader proof requires selected stock roots, never full BIN dumps or production activation.
        Console.WriteLine("  shader-override-self-test [ep1-static-manifest ...]");
        // Reborn: shader profile proof retains all output/cache restrictions.
        Console.WriteLine("  ep1-shader-profile-self-test");
        // Reborn: graph proof keeps dependency and native compilation stages separate from production/linker output.
        Console.WriteLine("  modifier-shader-graph-self-test");
        // Reborn: include proof does not activate the production linker or external FX processors.
        Console.WriteLine("  included-modifier-shader-self-test");
        // Reborn: the multi-family serializer command is fixture-only and retains production restrictions.
        Console.WriteLine("  modifier-shader-stream-self-test");
        // Reborn: admitted Include graphs and explicit runtime mappings are bounded diagnostic inputs, not a full SDK build.
        Console.WriteLine("  diagnostic-build <source.xml> <new-output-directory> [physical.manifest=runtime.manifest ...]");
        // Reborn: command self-tests own only fresh temporary inputs and outputs.
        Console.WriteLine("  diagnostic-build-self-test");
        Console.WriteLine("  diagnostic-audioevent-build-self-test [ep1-audio-manifest ...]");
        // Reborn: optional stock mappings prove actual command FX slices and concrete audio identities without rebuilding audio payloads.
        Console.WriteLine("  diagnostic-fx-build-self-test [ep1-global-manifest ep1-static-manifest ep1-audio-manifest]");
        // Reborn: watcher fixtures own only temporary files and use deterministic callback injection.
        Console.WriteLine("  watcher-cache-self-test");
        // Reborn: atomic batch fixtures test event conservation without writing game/compiler output.
        Console.WriteLine("  monitor-batch-self-test");
        // Reborn: expose retained and disk-loaded document lifecycle proof separately from fresh-session hashing.
        Console.WriteLine("  document-reuse-self-test");
        // Reborn: optional manifests add bounded golden comparisons without production registration.
        Console.WriteLine("  attribute-modifier-self-test [ep1-manifest ...]");
        // Reborn: keep experimental processor/document proof separate from production output.
        Console.WriteLine("  ep1-modifier-profile-self-test");
        // Reborn: keep imported native golden proof distinct from the no-dependency experimental profile.
        Console.WriteLine("  modifier-import-self-test [ep1-static-manifest ...]");
        Console.WriteLine("  utility-verify <manifest-or-big> [--entry <BIG entry>]");
        Console.WriteLine("  assembly-fields <managed-assembly> <type-name>");
        // Reborn: document bounded AudioFile evidence commands without implying an encoder or public asset profile.
        Console.WriteLine("  audiofile-runtime-self-test");
        // Reborn: serialization evidence is separate from production compilation and custom-data packaging.
        Console.WriteLine("  audiofile-serializer-self-test [unpacked-ep1-audio-manifest ...]");
        Console.WriteLine("  audiofile-runtime-audit <unpacked-ep1-manifest>");
        // Reborn: custom framing evidence commands neither decode audio nor admit an AudioFile processor.
        Console.WriteLine("  audio-custom-self-test");
        Console.WriteLine("  audio-custom-audit <unpacked-ep1-manifest>");
        // Reborn: original-entry comparison is read-only and does not constitute codec or game-loading validation.
        Console.WriteLine("  audio-archive-self-test");
        // Reborn: a PE API inventory is read-only evidence, not permission to trust native signatures.
        Console.WriteLine("  native-audio-api-audit <native-or-reference-audio-dll>");
        Console.WriteLine("  native-audio-api-self-test");
        Console.WriteLine("  audio-encoder-wav-self-test");
        // Reborn: codec experimentation remains separate from public diagnostic-build and production output.
        Console.WriteLine("  audio-encoder-poc <absolute-audited-audio.dll>");
        Console.WriteLine("  audio-archive-compare <unpacked-ep1-manifest> <original-EnglishAudio.big>");
        // Reborn: reconciliation is a separate explicit audit command, not an automatic error fallback or corpus repair.
        Console.WriteLine("  audio-custom-reconciled-audit <unpacked-ep1-manifest> <original-EnglishAudio.big>");
        Console.WriteLine("  assembly-methods <managed-assembly> <type-name> [method-filter]");
        Console.WriteLine("  assembly-il <managed-assembly> <method-token>");
        Console.WriteLine("  assembly-size-diff <reference-tokenizer-assembly> [--top <count>]");
        Console.WriteLine("  current-layout <SageBinaryData-type-name>");
        Console.WriteLine("  layout-self-test");
        Console.WriteLine("  compiler-self-test");
        // Reborn: optional manifests add bounded sound goldens without registering audio processors.
        Console.WriteLine("  multisound-native-self-test [ep1-global-manifest ...]");
        Console.WriteLine("  audioevent-native-self-test [ep1-global-manifest ...]");
        // Reborn: optional manifests compare checked profile output and prepared sound dependencies against stock.
        Console.WriteLine("  ep1-multisound-profile-self-test [ep1-global-manifest ...]");
        Console.WriteLine("  ep1-audioevent-profile-self-test [ep1-global-manifest ep1-audio-manifest ...]");
        Console.WriteLine("  audioevent-fx-stream-self-test [ep1-audio-manifest ...]");
        // Reborn: optional mappings prove concrete leaf audio identities in a fixed mixed local stream.
        Console.WriteLine("  multisound-fx-stream-self-test [ep1-global-manifest ...]");
        // Reborn: general service sound snapshots, ordering, cycle and publication proof.
        Console.WriteLine("  diagnostic-multisound-build-self-test [ep1-global-manifest ...]");
        // Reborn: compare only selected native FX chunks while keeping FX processor registration closed.
        Console.WriteLine("  fx-native-self-test [ep1-static-manifest ...]");
        // Reborn: manifest arguments validate dependency identities without reading their BIN payloads.
        Console.WriteLine("  reference-self-test <schema-fixture> [ep1-manifest ...]");
        // Reborn: external-link checks create only tiny test manifests in the specified artifact directory.
        Console.WriteLine("  external-link-self-test <output-directory> [ep1-manifest ...]");
        Console.WriteLine("  asset-bytes <manifest-or-big> <bin-or-big> <type-name> [--entry <manifest-entry>] [--bin-entry <bin-entry>] [--asset <full-name>] [--find-u32 <hex>] [--offset <decimal-or-hex>] [--count <decimal-or-hex>] [--relo <relo-or-big>] [--relo-entry <BIG entry>] [--imp <imp-or-big>] [--imp-entry <BIG entry>]");
        Console.WriteLine("  hash <text> [additional-text ...]");
        // Reborn: inspect only manifest metadata; BIN payloads and compiler output remain untouched.
        Console.WriteLine("  type-audit <ep1-manifest ...> [--json]");
        // Reborn: optional game metadata triggers a single bounded golden-asset comparison.
        Console.WriteLine("  armor-token-self-test <output-directory> [ep1-static-or-worldbuilder-manifest]");
        // Reborn: profile verification performs no production manifest emission.
        Console.WriteLine("  ep1-armor-profile-self-test");
        // Reborn: only isolated XML fixtures are written by the document regression harness.
        Console.WriteLine("  ep1-armor-document-self-test <output-directory>");
    }

    private sealed record TypeFingerprint(uint TypeId, uint TypeHash, uint? Tokenized);
}
