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
            // Reborn: worker transport is handled before compiler initialization; synthetic worker failure tests never load native codecs.
            if (args.FirstOrDefault() == "audio-encoder-worker" && args.Length == 3) { AudioEncoderSupervisor.Worker(args[1],args[2]); return 0; }
            // Reborn: the supervising parent validates results independently and never loads the native audio DLL itself.
            if (args.FirstOrDefault() == "supervised-core-audio-poc" && args.Length == 2) { CompilerSmokeTest.InitializeHashProvider(); AudioEncoderSupervisor.Run(args[1]); return 0; }
            // Reborn: copy and validate the narrow caller-source pair before launching native work; originals are read-only.
            if (args.FirstOrDefault() == "supervised-authored-audio-poc" && args.Length == 3) { CompilerSmokeTest.InitializeHashProvider(); AudioEncoderSupervisor.Run(args[1],"encode-authored",authored:AuthoredAudioSnapshot.Read(args[2])); return 0; }
            // Reborn: caller event admission explicitly freezes four files, retaining the legacy three-file command unchanged.
            if (args.FirstOrDefault() == "supervised-authored-audio-event-poc" && args.Length == 3) { CompilerSmokeTest.InitializeHashProvider(); AudioEncoderSupervisor.Run(args[1],"encode-authored-event",authored:AuthoredAudioSnapshot.Read(args[2],true)); return 0; }
            if (args.FirstOrDefault() == "authored-audio-snapshot-self-test") { CompilerSmokeTest.InitializeHashProvider(); AuthoredAudioSnapshotSmokeTest.Run(); return 0; }
            // Reborn: separate opt-in pool worker modes retain the fixed encoder path and do not emit package/event graphs.
            if (args.FirstOrDefault() == "supervised-audio-pool-preflight" && args.Length == 2) { CompilerSmokeTest.InitializeHashProvider(); AudioEncoderSupervisor.Run(Path.Combine(Path.GetTempPath(),"unused-native-library.dll"),"preflight-pool",pool:AuthoredAudioPool.Read(args[1])); return 0; }
            if (args.FirstOrDefault() == "supervised-audio-pool-encode" && args.Length == 3) { CompilerSmokeTest.InitializeHashProvider(); AudioEncoderSupervisor.Run(args[1],"encode-pool",pool:AuthoredAudioPool.Read(args[2])); return 0; }
            // Reborn: variable package publication is explicit and leaf-only, never an implicit extension of raw/event commands.
            if (args.FirstOrDefault() == "supervised-audio-pool-package" && args.Length == 3) { CompilerSmokeTest.InitializeHashProvider(); AudioEncoderSupervisor.Run(args[1],"encode-pool-package",pool:AuthoredAudioPool.Read(args[2])); return 0; }
            // Reborn: mixed publication requires an explicitly frozen event.xml and never widens leaf-only worker modes.
            if (args.FirstOrDefault() == "supervised-audio-pool-event" && args.Length == 3) { CompilerSmokeTest.InitializeHashProvider(); AudioEncoderSupervisor.Run(args[1],"encode-pool-event",pool:AuthoredAudioPool.Read(args[2],true)); return 0; }
            // Reborn: version-2 duration commands are explicit and leaf-only; canonical/mixed commands retain their old bounds.
            if (args.FirstOrDefault() == "supervised-audio-duration-preflight" && args.Length == 2) { CompilerSmokeTest.InitializeHashProvider(); AudioEncoderSupervisor.Run(Path.Combine(Path.GetTempPath(),"unused-native-library.dll"),"preflight-pool-duration",pool:AuthoredAudioPool.Read(args[1],durationCandidate:true)); return 0; }
            if (args.FirstOrDefault() == "supervised-audio-duration-encode" && args.Length == 3) { CompilerSmokeTest.InitializeHashProvider(); AudioEncoderSupervisor.Run(args[1],"encode-pool-duration",pool:AuthoredAudioPool.Read(args[2],durationCandidate:true)); return 0; }
            if (args.FirstOrDefault() == "supervised-audio-duration-package" && args.Length == 3) { CompilerSmokeTest.InitializeHashProvider(); AudioEncoderSupervisor.Run(args[1],"encode-pool-duration-package",pool:AuthoredAudioPool.Read(args[2],durationCandidate:true)); return 0; }
            if (args.FirstOrDefault() == "audio-duration-pool-self-test") { CompilerSmokeTest.InitializeHashProvider(); AudioDurationPoolSmokeTest.Run(); return 0; }
            if (args.FirstOrDefault() == "audio-duration-native-proof" && args.Length == 2) { CompilerSmokeTest.InitializeHashProvider(); AudioDurationPoolSmokeTest.NativeProof(args[1]); return 0; }
            // Reborn: duration mixed publication requires version-2 PCM admission plus explicitly frozen event.xml.
            if (args.FirstOrDefault() == "supervised-audio-duration-event" && args.Length == 3) { CompilerSmokeTest.InitializeHashProvider(); AudioEncoderSupervisor.Run(args[1],"encode-pool-duration-event",pool:AuthoredAudioPool.Read(args[2],true,true)); return 0; }
            if (args.FirstOrDefault() == "audio-duration-event-native-proof" && args.Length == 2) { CompilerSmokeTest.InitializeHashProvider(); AudioDurationEventSmokeTest.Run(args[1]); return 0; }
            if (args.FirstOrDefault() == "audio-pool-mixed-self-test") { CompilerSmokeTest.InitializeHashProvider(); AudioPoolEventSmokeTest.MixedRun(); return 0; }
            // Reborn: vector regression and native integration are explicit and preserve all fixed-pair command defaults.
            if (args.FirstOrDefault() == "audio-event-vector-self-test") { CompilerSmokeTest.InitializeHashProvider(); AudioEventVectorSmokeTest.Run(); return 0; }
            if (args.FirstOrDefault() == "audio-event-vector-native-proof" && args.Length == 2) { CompilerSmokeTest.InitializeHashProvider(); AudioEventVectorSmokeTest.NativeProof(args[1]); return 0; }
            if (args.FirstOrDefault() == "audio-pool-mixed-native-proof" && args.Length == 2) { CompilerSmokeTest.InitializeHashProvider(); AudioPoolEventSmokeTest.MixedNativeProof(args[1]); return 0; }
            if (args.FirstOrDefault() == "audio-pool-package-native-proof" && args.Length == 2) { CompilerSmokeTest.InitializeHashProvider(); AudioPoolWorkerSmokeTest.NativeProof(args[1],true); return 0; }
            if (args.FirstOrDefault() == "audio-pool-package-self-test") { CompilerSmokeTest.InitializeHashProvider(); AudioPoolPackageSmokeTest.Run(); return 0; }
            // Reborn: variable event closure is explicit, independently verifies a leaf package and never emits a mixed manifest.
            if (args.FirstOrDefault() == "audio-pool-event-preflight" && args.Length == 3) { CompilerSmokeTest.InitializeHashProvider(); AudioPoolEventPreflight.Run(args[1],args[2]); return 0; }
            if (args.FirstOrDefault() == "audio-pool-event-self-test") { CompilerSmokeTest.InitializeHashProvider(); AudioPoolEventSmokeTest.Run(); return 0; }
            if (args.FirstOrDefault() == "audio-pool-event-native-proof" && args.Length == 2) { CompilerSmokeTest.InitializeHashProvider(); AudioPoolEventSmokeTest.NativeProof(args[1]); return 0; }
            // Reborn: pool worker/native regression entry points are explicit and mutate only freshly owned fixtures.
            if (args.FirstOrDefault() == "audio-pool-worker-self-test") { CompilerSmokeTest.InitializeHashProvider(); AudioPoolWorkerSmokeTest.Run(); return 0; }
            if (args.FirstOrDefault() == "audio-pool-native-proof" && args.Length == 2) { CompilerSmokeTest.InitializeHashProvider(); AudioPoolWorkerSmokeTest.NativeProof(args[1]); return 0; }
            // Reborn: the original unsupervised preflight remains metadata-only and never launches a codec.
            if (args.FirstOrDefault() == "authored-audio-pool-preflight" && args.Length == 2)
            {
                CompilerSmokeTest.InitializeHashProvider(); var result = AuthoredAudioPool.Read(args[1]).Preflight();
                Console.WriteLine("Owned pool preflight copies: "+result.Directory);
                foreach (var row in result.Rows) Console.WriteLine($"{row.Source}: AudioFile:{row.Name}, id={row.Id:X8}, core={row.CoreHash:X8}, streamed={row.Streamed}");
                Console.WriteLine("Audio pool preflight: OK (frozen source/current core metadata only; no native/event/stream/production admission)."); return 0;
            }
            // Reborn: the standalone pool regression command performs only owned managed fixture work.
            if (args.FirstOrDefault() == "authored-audio-pool-self-test") { CompilerSmokeTest.InitializeHashProvider(); AuthoredAudioPoolSmokeTest.Run(); return 0; }
            // Reborn: caller PCM/subtitle and stale-original integration proof uses only owned sources and opt-in supervised native children.
            if (args.FirstOrDefault() == "authored-audio-native-proof" && args.Length == 2) { CompilerSmokeTest.InitializeHashProvider(); AuthoredAudioNativeProbe.Run(args[1]); return 0; }
            // Reborn: opt-in native evidence exercises event provenance/staleness separately from the three-file baseline.
            if (args.FirstOrDefault() == "authored-audio-event-native-proof" && args.Length == 2) { CompilerSmokeTest.InitializeHashProvider(); AuthoredAudioNativeProbe.Run(args[1],true); return 0; }
            // Reborn: opt-in singleton/reversed list evidence remains isolated from all default managed tests.
            if (args.FirstOrDefault() == "authored-audio-list-native-proof" && args.Length == 2)
            { CompilerSmokeTest.InitializeHashProvider(); foreach (string selection in new[] { "ram","streamed","reversed" }) AuthoredAudioNativeProbe.Run(args[1],true,selection); return 0; }
            // Reborn: opt-in control evidence covers zero/single/combined bits across singleton and reversed list shapes.
            if (args.FirstOrDefault() == "authored-audio-control-native-proof" && args.Length == 2)
            {
                CompilerSmokeTest.InitializeHashProvider(); AuthoredAudioNativeProbe.Run(args[1],true,"streamed","",0);
                AuthoredAudioNativeProbe.Run(args[1],true,"ram","LOOP",1);
                AuthoredAudioNativeProbe.Run(args[1],true,"reversed","LOOP INTERRUPT FADE_ON_KILL IMMEDIATE_DECAY_ON_KILL",0x129); return 0;
            }
            // Reborn: real encoded-result tamper tests execute native work only in supervised opt-in children.
            if (args.FirstOrDefault() == "supervised-audio-tamper-test" && args.Length == 2) { CompilerSmokeTest.InitializeHashProvider(); AudioEncoderSupervisor.NativeTamperTests(args[1]); return 0; }
            if (args.FirstOrDefault() == "audio-supervisor-self-test") { AudioEncoderSupervisorSmokeTest.Run(); return 0; }
            // Reborn: pin reference identity evidence and exercise managed hash boundaries without loading old compilers or codecs.
            if (args.FirstOrDefault() == "hashing-writer-boundary-self-test") { HashingWriterBoundarySmokeTest.Run(); return 0; }
            // Reborn: exercise real AudioFile file-reference identity with owned WAV fixtures and no native codec/output.
            if (args.FirstOrDefault() == "audiofile-identity-self-test") { CompilerSmokeTest.InitializeHashProvider(); AudioFileIdentitySmokeTest.Run(); return 0; }
            // Reborn: verify disk/core preparation invalidation without admitting native audio or production streams.
            if (args.FirstOrDefault() == "core-audiofile-preparation-self-test") { CompilerSmokeTest.InitializeHashProvider(); AudioFileCorePreparationSmokeTest.Run(); return 0; }
            // Reborn: current-source package publication regression never executes the optional native encoder.
            if (args.FirstOrDefault() == "core-audio-package-gate-self-test") { CompilerSmokeTest.InitializeHashProvider(); CoreAudioPackageGateSmokeTest.Run(); return 0; }
            // Reborn: managed callbacks test the real cleanup control flow without native DLL execution.
            if (args.FirstOrDefault() == "audio-encoder-cleanup-self-test") { AudioEncoderCleanupSmokeTest.Run(); return 0; }
            // Reborn: local event/audio packaging proof uses synthetic framing unless the native encoder command is explicitly invoked.
            if (args.FirstOrDefault() == "local-audio-package-self-test") { CompilerSmokeTest.InitializeHashProvider(); AudioFileLocalEventSmokeTest.Run(); return 0; }
            // Reborn: managed fixed-package fixtures never execute native codecs or activate production AudioFile compilation.
            if (args.FirstOrDefault() == "audiofile-package-self-test") { CompilerSmokeTest.InitializeHashProvider(); AudioFilePackageSmokeTest.Run(); return 0; }
            // Reborn: authored input preparation is managed-only and remains separate from codec/plugin/stream admission.
            if (args.FirstOrDefault() == "ep1-audiofile-input-self-test") { CompilerSmokeTest.InitializeHashProvider(); Ep1AudioFileInputSmokeTest.Run(); return 0; }
            // Reborn: native AudioFile serialization proof optionally compares actual selected stock slices without registering an audio processor.
            if (args.FirstOrDefault() == "audiofile-serializer-self-test") { AudioFileSerializationSmokeTest.Run(args.Skip(1).ToArray()); return 0; }
            // Reborn: inspect audio library API evidence without invoking DLL entry points or codecs.
            if (args.FirstOrDefault() == "native-audio-api-audit" && args.Length == 2) { NativeAudioApiProbe.Run(args[1]); return 0; }
            // Reborn: default-compatible audio regressions inspect PE data and managed WAV bytes but never initialize native codecs.
            if (args.FirstOrDefault() == "native-audio-api-self-test") { NativeAudioApiProbe.SelfTest(); return 0; }
            if (args.FirstOrDefault() == "audio-encoder-wav-self-test") { AudioEncoderPoc.SelfTest(); return 0; }
            // Reborn: native encoding is opt-in and runs only in this standalone inspector process, never in default compiler tests.
            if (args.FirstOrDefault() == "audio-encoder-poc" && args.Length == 2) { CompilerSmokeTest.InitializeHashProvider(); AudioEncoderPoc.Run(args[1]); return 0; }
            // Reborn: opt-in native encoding now accepts actual core preparation and rechecks current sources before diagnostic publication.
            if (args.FirstOrDefault() == "core-audio-encoder-poc" && args.Length == 2) { CompilerSmokeTest.InitializeHashProvider(); AudioEncoderPoc.Run(args[1],true); return 0; }
            // Reborn: native fault scenarios are opt-in standalone workers, never part of default compiler tests.
            if (args.FirstOrDefault() == "core-audio-encoder-fault" && args.Length == 3) { CompilerSmokeTest.InitializeHashProvider(); AudioEncoderFaultAudit.Run(args[1],args[2]); return 0; }
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
            // Reborn: expose staged file-field declaration evidence separately from source-path completeness.
            // Reborn: strict effective schema evidence stays separate from production compilation and source readiness.
            if (args.FirstOrDefault() is "sdk-effective-schema" or "sdk-shield-schema-candidate" or "sdk-reviewed-schema-candidate")
            {
                if (args.Length is < 1 or > 2) throw new ArgumentException("sdk-effective-schema / sdk-shield-schema-candidate / sdk-reviewed-schema-candidate [absolute-source.xml]");
                // Reborn: normalization is opt-in through a separate named diagnostic command, never a silent resolver fallback.
                var effective = SdkEffectiveSchema.Inspect(args.Length == 2 ? args[1] : null,args[0] != "sdk-effective-schema",args[0] == "sdk-reviewed-schema-candidate"); Console.WriteLine(JsonSerializer.Serialize(effective,JsonOptions)); return effective.SchemaAdmitted && effective.SourceBinding?.XmlValidated != false ? 0 : 2;
            }
            // Reborn: keep effective-schema positive fixtures and real duplicate rejection available without source compilation.
            if (args.FirstOrDefault() == "sdk-effective-schema-self-test") { SdkEffectiveSchemaSmokeTest.Run(); return 0; }
            // Reborn: prove pinned normalization independently of default strict schema compilation.
            if (args.FirstOrDefault() == "sdk-shield-schema-candidate-self-test") { SdkShieldSchemaCandidateSmokeTest.Run(); return 0; }
            // Reborn: test exact warning admission with actual compiled candidate source probes, never codecs/processors.
            if (args.FirstOrDefault() == "sdk-reviewed-schema-candidate-self-test") { SdkSchemaHookReviewSmokeTest.Run(); return 0; }
            if (args.FirstOrDefault() is "sdk-file-reference-catalog" or "sdk-file-reference-catalog-self-test")
            {
                if (args.Length != 1) throw new ArgumentException("File-reference catalog commands take no arguments; the staged schema root is explicit in the report.");
                if (args[0] == "sdk-file-reference-catalog-self-test") SdkFileReferenceCatalogSmokeTest.Run();
                else Console.WriteLine(JsonSerializer.Serialize(SdkFileReferenceCatalog.Inspect(),JsonOptions));
                return 0;
            }
            // Reborn: add explicit bounded source-path planning without changing the original environment-only command.
            // Reborn: expose successful sound expression preparation separately from repeated-singleton owner admission.
            if (args.FirstOrDefault() == "sdk-sound-expression-review")
            {
                if (args.Length != 6 || args[1] != "ra3ep1") throw new ArgumentException("sdk-sound-expression-review ra3ep1 <schema-root> <source-root> <source-entry.xml> <new-output-directory>");
                var environment = SdkEnvironmentPreflight.Inspect(args[1],args[2],args[3],args[4],args[5],Array.Empty<string>());
                var paths = SdkSourcePathAudit.Inspect(environment);
                System.Xml.Schema.XmlSchemaSet? schemas = null;
                var schema = SdkEffectiveSchema.Inspect(shieldCandidate:true,reviewHooks:true,onAdmitted:set => schemas = set);
                if (!schema.SchemaAdmitted || schemas == null) throw new InvalidDataException("Reviewed diagnostic schema required.");
                Console.WriteLine(JsonSerializer.Serialize(SdkSoundExpressionReview.Inspect(paths,schemas,environment.SourceEntry),JsonOptions));
                return 2;
            }
            if (args.FirstOrDefault() is "sdk-source-preflight" or "sdk-typed-source-graph")
            {
                if (args.Length < 6) throw new ArgumentException("sdk-source-preflight ra3ep1 <schema-root> <source-root> <source-entry.xml> <new-output-directory> [--art-root absolute-directory] [--audio-root absolute-directory] [absolute.manifest=runtime.manifest ...]");
                string? art = null,audio = null; List<string> mappings = new();
                // Reborn: diagnostic local literal expressions are never implicitly enabled for the path-only/default graph commands.
                // Reborn: complex leaf copying is a separate diagnostic profile, not an implicit widening of older flags.
                // Reborn: matching empty children remains a distinct opt-in, separate from tree copying and imported base visibility.
                // Reborn: recursive preparation remains separately explicit from direct-only visibility and local child matching.
                bool localDefines = false,includeDefines = false,definitionExpressions = false,selfAttributeInheritance = false,selfChildCopy = false,selfComplexChildCopy = false,selfTreeCopy = false,instanceInheritance = false,instanceRootFiles = false,selfChildMerge = false,instanceChains = false,instanceRemovals = false,instanceChoices = false,instanceMarkers = false,instanceBitflags = false,instanceFilters = false,instanceUpgrades = false,instanceMetadata = false,instanceExpressions = false,instanceIdenticalStates = false,instanceStateReadds = false,instanceCrossStateRemovals = false,instanceMusicOffsets = false,instanceAudioTrees = false,instanceSoundOffsets = false,instanceSoundSingletons = false;
                for (int index = 6; index < args.Length; index++)
                {
                    string option = args[index];
                    if (option is "--local-defines" or "--include-defines" or "--definition-expressions" or "--self-attribute-inheritance" or "--self-child-copy" or "--self-complex-child-copy" or "--self-tree-copy" or "--instance-inheritance" or "--instance-root-files" or "--self-child-merge" or "--instance-chains" or "--instance-removals" or "--instance-choices" or "--instance-markers" or "--instance-bitflags" or "--instance-filters" or "--instance-upgrades" or "--instance-metadata" or "--instance-expressions" or "--instance-identical-states" or "--instance-state-readds" or "--instance-cross-state-removals" or "--instance-music-offsets" or "--instance-audio-trees" or "--instance-sound-offsets" or "--instance-sound-singletons")
                    {
                        if (args[0] != "sdk-typed-source-graph" || localDefines || includeDefines || definitionExpressions || selfAttributeInheritance || selfChildCopy || selfComplexChildCopy || selfTreeCopy || instanceInheritance || instanceRootFiles || selfChildMerge || instanceChains || instanceRemovals || instanceChoices || instanceMarkers || instanceBitflags || instanceFilters || instanceUpgrades || instanceMetadata || instanceExpressions || instanceIdenticalStates || instanceStateReadds || instanceCrossStateRemovals || instanceMusicOffsets || instanceAudioTrees || instanceSoundOffsets || instanceSoundSingletons) throw new ArgumentException("Choose one preprocessing profile on the typed graph command only.");
                        localDefines = option == "--local-defines"; includeDefines = option == "--include-defines";
                        // Reborn: the three-form definition subset must be requested separately from either literal profile.
                        definitionExpressions = option == "--definition-expressions";
                        // Reborn: local leaf overlays are separately explicit and preserve the subset on non-inherited documents only.
                        selfAttributeInheritance = option == "--self-attribute-inheritance";
                        // Reborn: flat one-sided child copying is a distinct admission rule, never enabled by the original leaf flag.
                        selfChildCopy = option == "--self-child-copy";
                        // Reborn: select complex leaf copying only from its own explicit command-line option.
                        selfComplexChildCopy = option == "--self-complex-child-copy";
                        // Reborn: recursive sequence-tree copying is selected only by its distinct opt-in flag.
                        selfTreeCopy = option == "--self-tree-copy";
                        // Reborn: source-backed direct-instance visibility is independently explicit and remains copy-only.
                        instanceInheritance = option == "--instance-inheritance";
                        // Reborn: admit inherited alias-root file fields only with their own explicit profile.
                        instanceRootFiles = option == "--instance-root-files";
                        // Reborn: match only schema-empty complex children under the new independent local profile.
                        selfChildMerge = option == "--self-child-merge";
                        // Reborn: recursively prepare child documents without exporting transitive handles as direct bases.
                        instanceChains = option == "--instance-chains";
                        // Reborn: admit keyed empty-child Remove commands only under their independent profile.
                        instanceRemovals = option == "--instance-removals";
                        // Reborn: enable repeated-choice copying only through its independently requested profile.
                        instanceChoices = option == "--instance-choices";
                        // Reborn: consume pipeline inheritance markers only under the independently requested diagnostic profile.
                        instanceMarkers = option == "--instance-markers";
                        // Reborn: enable whole-token-proven enum-list modifications only through their independent profile.
                        instanceBitflags = option == "--instance-bitflags";
                        // Reborn: explicitly select one-sided matched ObjectFilter payload copying, never general branch merging.
                        instanceFilters = option == "--instance-filters";
                        // Reborn: select separately proved complementary UpgradeTemplate singleton normalization.
                        instanceUpgrades = option == "--instance-upgrades";
                        // Reborn: enable only separately proved definition-only leaf all Includes, without expression-before-inheritance admission.
                        instanceMetadata = option == "--instance-metadata";
                        // Reborn: explicitly select expression substitution before guarded owner inheritance.
                        instanceExpressions = option == "--instance-expressions";
                        // Reborn: identical-state folding is separately requested and cannot silently widen expression-only preparation.
                        instanceIdenticalStates = option == "--instance-identical-states";
                        // Reborn: explicitly select ordered literal state Remove/re-add rather than changing duplicate coalescing.
                        instanceStateReadds = option == "--instance-state-readds";
                        // Reborn: explicitly select removal-only cross-QName state commands, never generic replacement.
                        instanceCrossStateRemovals = option == "--instance-cross-state-removals";
                        // Reborn: request direct MusicTrack.Volume integer offsets without opening other audio arithmetic.
                        instanceMusicOffsets = option == "--instance-music-offsets";
                        // Reborn: admit only separately proved shallow broad-audio budgets, retaining old tree limits elsewhere.
                        instanceAudioTrees = option == "--instance-audio-trees";
                        // Reborn: select schema-typed sound integer offsets, never a generic evaluator.
                        instanceSoundOffsets = option == "--instance-sound-offsets";
                        // Reborn: choose only the known full-owner-proved sound singleton scope.
                        instanceSoundSingletons = option == "--instance-sound-singletons";
                    }
                    else if (option is "--art-root" or "--audio-root")
                    {
                        if (++index >= args.Length || args[index].StartsWith("--",StringComparison.Ordinal)) throw new ArgumentException("Explicit root option requires a value.");
                        if (option == "--art-root") { if (art != null) throw new ArgumentException("Duplicate ART root."); art = args[index]; }
                        else { if (audio != null) throw new ArgumentException("Duplicate AUDIO root."); audio = args[index]; }
                    }
                    else if (option.StartsWith("--",StringComparison.Ordinal)) throw new ArgumentException("Unknown source preflight option.");
                    else mappings.Add(option);
                }
                var environment = SdkEnvironmentPreflight.Inspect(args[1],args[2],args[3],args[4],args[5],mappings.ToArray());
                var paths = SdkSourcePathAudit.Inspect(environment,art,audio);
                // Reborn: typed graph admission is explicit and preserves the original path-only command/report contract.
                if (args[0] == "sdk-typed-source-graph")
                {
                    var typed = SdkTypedSourceGraph.Inspect(paths,localDefines,includeDefines,definitionExpressions,selfAttributeInheritance,selfChildCopy,selfComplexChildCopy,selfTreeCopy,instanceInheritance,instanceRootFiles,selfChildMerge,instanceChains,instanceRemovals,instanceChoices,instanceMarkers,instanceBitflags,instanceFilters,instanceUpgrades,instanceMetadata,instanceExpressions,instanceIdenticalStates,instanceStateReadds,instanceCrossStateRemovals,instanceMusicOffsets,instanceAudioTrees,instanceSoundOffsets,instanceSoundSingletons);
                    Console.WriteLine(JsonSerializer.Serialize(new { environment.Target,ReadOnly = true,SnapshotOnly = true,ProductionBuildReady = false,Environment = environment,SourcePaths = paths,TypedSources = typed },JsonOptions));
                    return typed.Graph.ScopedGraphComplete ? 0 : 2;
                }
                Console.WriteLine(JsonSerializer.Serialize(new { environment.Target,ReadOnly = true,SnapshotOnly = true,ProductionBuildReady = false,Environment = environment,SourcePaths = paths },JsonOptions));
                return paths.ScopedPathAuditComplete ? 0 : 2;
            }
            // Reborn: run path-only fixtures without invoking codecs, builders or registry discovery.
            if (args.FirstOrDefault() == "sdk-source-preflight-self-test") { SdkSourcePathAuditSmokeTest.Run(); return 0; }
            // Reborn: expose managed graph snapshot/type/resource regressions separately from production SDK builds.
            if (args.FirstOrDefault() == "sdk-typed-source-graph-self-test") { SdkTypedSourceGraphSmokeTest.Run(); return 0; }
            // Reborn: independently exercise bounded literal expression diagnostics without native asset compilation.
            if (args.FirstOrDefault() == "sdk-local-defines-self-test") { SdkLocalDefineProfileSmokeTest.Run(); return 0; }
            // Reborn: test imported literal origin/snapshot rules separately from EA expression evaluation and production builds.
            if (args.FirstOrDefault() == "sdk-include-defines-self-test") { SdkIncludeDefineProfileSmokeTest.Run(); return 0; }
            // Reborn: independently test bounded definition syntax without loading/executing the reference evaluator DLL.
            if (args.FirstOrDefault() == "sdk-definition-subset-self-test") { SdkDefinitionSubsetSmokeTest.Run(); return 0; }
            // Reborn: test local leaf overlays through the existing core joiner without admitting imported or complex inheritance.
            if (args.FirstOrDefault() == "sdk-self-attribute-inheritance-self-test") { SdkSelfAttributeInheritanceSmokeTest.Run(); return 0; }
            // Reborn: exercise flat child copying independently of complex merge/instance-source semantics.
            if (args.FirstOrDefault() == "sdk-self-child-copy-self-test") { SdkSelfChildCopySmokeTest.Run(); return 0; }
            // Reborn: expose complex leaf fixtures independently from the default compiler suite.
            if (args.FirstOrDefault() == "sdk-self-complex-child-copy-self-test") { SdkSelfComplexChildCopySmokeTest.Run(); return 0; }
            // Reborn: expose recursive sequence-tree copying fixtures separately from the default compiler suite.
            if (args.FirstOrDefault() == "sdk-self-tree-copy-self-test") { SdkSelfTreeCopySmokeTest.Run(); return 0; }
            // Reborn: independently exercise two-sided empty-child matching without admitting text/branch/imported merging.
            if (args.FirstOrDefault() == "sdk-self-child-merge-self-test") { SdkSelfChildMergeSmokeTest.Run(); return 0; }
            // Reborn: expose direct-instance eligibility/source/provenance regression fixtures independently.
            if (args.FirstOrDefault() == "sdk-instance-inheritance-self-test") { SdkInstanceInheritanceSmokeTest.Run(); return 0; }
            // Reborn: expose alias-root versus consumer-relative field handling tests independently.
            if (args.FirstOrDefault() == "sdk-instance-root-files-self-test") { SdkInstanceRootFilesSmokeTest.Run(); return 0; }
            // Reborn: test recursive preparation, closure identity and direct-definition eligibility independently.
            if (args.FirstOrDefault() == "sdk-instance-chains-self-test") { SdkInstanceChainsSmokeTest.Run(); return 0; }
            // Reborn: expose managed removal identity/cardinality/source-closure tests independently.
            if (args.FirstOrDefault() == "sdk-instance-removals-self-test") { SdkInstanceRemovalsSmokeTest.Run(); return 0; }
            // Reborn: run bounded repeated-choice admission fixtures without production compiler or output activation.
            if (args.FirstOrDefault() == "sdk-instance-choices-self-test") { SdkInstanceChoicesSmokeTest.Run(); return 0; }
            // Reborn: test declaration-time marker consumption without widening production compiler admission.
            if (args.FirstOrDefault() == "sdk-instance-markers-self-test") { SdkInstanceMarkersSmokeTest.Run(); return 0; }
            // Reborn: independently prove bounded KindOf modifiers without invoking production compilation.
            if (args.FirstOrDefault() == "sdk-instance-bitflags-self-test") { SdkInstanceBitflagsSmokeTest.Run(); return 0; }
            // Reborn: expose the independently scoped matched ObjectFilter regression without running native compilers.
            if (args.FirstOrDefault() == "sdk-instance-filters-self-test") { SdkInstanceFiltersSmokeTest.Run(); return 0; }
            // Reborn: characterize duplicate upgrade singleton semantics without admitting production normalization.
            if (args.FirstOrDefault() == "sdk-upgrade-semantics-self-test") { SdkUpgradeSemanticsSmokeTest.Run(); return 0; }
            // Reborn: independently test complementary upgrade singleton admission without native compilers.
            if (args.FirstOrDefault() == "sdk-instance-upgrades-self-test") { SdkInstanceUpgradesSmokeTest.Run(); return 0; }
            // Reborn: independently test definition-only leaf Include authority without evaluating inherited expressions.
            if (args.FirstOrDefault() == "sdk-instance-metadata-self-test") { SdkInstanceMetadataSmokeTest.Run(); return 0; }
            // Reborn: test separately admitted expression-before-overlay ordering without native compiler execution.
            if (args.FirstOrDefault() == "sdk-instance-expressions-self-test") { SdkInstanceExpressionsSmokeTest.Run(); return 0; }
            // Reborn: expose owned sibling-key characterization without admitting new source normalization.
            if (args.FirstOrDefault() == "sdk-sibling-identity-semantics-self-test") { SdkSiblingIdentitySemanticsSmokeTest.Run(); return 0; }
            // Reborn: test narrow identical-state admission independently of sibling operation characterization.
            if (args.FirstOrDefault() == "sdk-instance-identical-states-self-test") { SdkInstanceIdenticalStatesSmokeTest.Run(); return 0; }
            // Reborn: independently test original ordered state commands without native compiler execution.
            if (args.FirstOrDefault() == "sdk-instance-state-readds-self-test") { SdkInstanceStateReaddsSmokeTest.Run(); return 0; }
            // Reborn: expose read-only pinned CC32 source/core/native-metadata review without changing graph admission.
            if (args.FirstOrDefault() == "sdk-cc32-review")
            {
                if (args.Length != 3) throw new ArgumentException("sdk-cc32-review <absolute-Uprising-source-root> <absolute-EP1-global.manifest>");
                Console.WriteLine(JsonSerializer.Serialize(SdkCc32Review.Review(args[1],args[2]),JsonOptions)); return 0;
            }
            // Reborn: independently test metadata identity boundaries without external native payloads.
            if (args.FirstOrDefault() == "sdk-cc32-review-self-test") { SdkCc32ReviewSmokeTest.Run(); return 0; }
            // Reborn: test independent removal-only cross-QName admission without native emission.
            // Reborn: exercise bounded music arithmetic, definition closure and earlier-profile isolation without native codecs.
            // Reborn: test independent audio breadth/work guards without native codecs or game output.
            // Reborn: test typed sound arithmetic and actual Core field retention independently of earlier scopes.
            if (args.FirstOrDefault() == "sdk-instance-sound-offsets-self-test") { SdkInstanceSoundOffsetsSmokeTest.Run(); return 0; }
            // Reborn: run independently scoped known sound singleton admission fixtures without native codecs.
            if (args.FirstOrDefault() == "sdk-instance-sound-singletons-self-test") { SdkInstanceSoundSingletonsSmokeTest.Run(); return 0; }
            // Reborn: characterize reviewed-schema sound singleton conflicts without native execution or graph admission.
            if (args.FirstOrDefault() == "sdk-sound-singleton-semantics-self-test") { SdkSoundSingletonSemanticsSmokeTest.Run(); return 0; }
            // Reborn: review complete pinned isolated sound owners without admitting the whole source or writing outputs.
            if (args.FirstOrDefault() == "sdk-sound-owner-review")
            {
                if (args.Length != 2) throw new ArgumentException("sdk-sound-owner-review <absolute-Uprising-source-root>");
                Console.WriteLine(JsonSerializer.Serialize(SdkSoundOwnerReview.Review(args[1]),JsonOptions)); return 2;
            }
            // Reborn: run owned projection/tamper fixtures without external source/game dependencies.
            if (args.FirstOrDefault() == "sdk-sound-owner-review-self-test") { SdkSoundOwnerReviewSmokeTest.Run(); return 0; }
            if (args.FirstOrDefault() == "sdk-instance-audio-trees-self-test") { SdkInstanceAudioTreesSmokeTest.Run(); return 0; }
            if (args.FirstOrDefault() == "sdk-instance-music-offsets-self-test") { SdkInstanceMusicOffsetsSmokeTest.Run(); return 0; }
            if (args.FirstOrDefault() == "sdk-instance-cross-state-removals-self-test") { SdkInstanceCrossStateRemovalsSmokeTest.Run(); return 0; }
            // Reborn: review explicit source bytes only for the two known upgrade owners, without graph admission or emitted output.
            if (args.FirstOrDefault() == "sdk-upgrade-semantics-review")
            {
                if (args.Length != 2) throw new ArgumentException("sdk-upgrade-semantics-review <absolute-upgrade.xml>");
                Console.WriteLine(JsonSerializer.Serialize(SdkUpgradeSemanticsSmokeTest.Review(args[1]),JsonOptions)); return 0;
            }
            if (args.FirstOrDefault() == "sdk-preflight")
            {
                // Reborn: inspect explicit roots and target metadata only; never execute the reference SDK batch files or production compiler.
                if (args.Length < 6) throw new ArgumentException("sdk-preflight ra3ep1 <schema-root> <source-root> <source-entry.xml> <new-output-directory> [absolute.manifest=runtime.manifest ...]");
                Console.WriteLine(JsonSerializer.Serialize(SdkEnvironmentPreflight.Inspect(args[1],args[2],args[3],args[4],args[5],args.Skip(6).ToArray()),JsonOptions)); return 0;
            }
            // Reborn: standalone environment tests use owned fixtures and never launch native codecs or the legacy SDK.
            if (args.FirstOrDefault() == "sdk-preflight-self-test") { CompilerSmokeTest.InitializeHashProvider(); SdkEnvironmentPreflightSmokeTest.Run(); return 0; }
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
        // Reborn: SDK planning is explicitly target-aware/read-only and separate from diagnostic or production compilation.
        Console.WriteLine("  sdk-preflight ra3ep1 <schema-root> <source-root> <source-entry.xml> <new-output-directory> [absolute.manifest=runtime.manifest ...]");
        Console.WriteLine("  sdk-preflight-self-test");
        // Reborn: expose optional explicit ART/AUDIO roots and incomplete graph exit status separately from environment readiness.
        Console.WriteLine("  sdk-source-preflight ra3ep1 <schema-root> <source-root> <source-entry.xml> <new-output-directory> [--art-root absolute-directory] [--audio-root absolute-directory] [absolute.manifest=runtime.manifest ...]");
        Console.WriteLine("  sdk-source-preflight-self-test");
        // Reborn: report declaration inventory without promising source binding or payload availability.
        Console.WriteLine("  sdk-file-reference-catalog");
        Console.WriteLine("  sdk-file-reference-catalog-self-test");
        // Reborn: report strict compiled XSD evidence without opening source compilation.
        Console.WriteLine("  sdk-effective-schema [absolute-source.xml]");
        Console.WriteLine("  sdk-effective-schema-self-test");
        // Reborn: fingerprint-pinned in-memory candidate keeps the default strict schema gate intact.
        Console.WriteLine("  sdk-shield-schema-candidate [absolute-source.xml]");
        Console.WriteLine("  sdk-shield-schema-candidate-self-test");
        // Reborn: reviewed warning admission remains explicit, diagnostic and separate from clean/default schema status.
        Console.WriteLine("  sdk-reviewed-schema-candidate [absolute-source.xml]");
        Console.WriteLine("  sdk-reviewed-schema-candidate-self-test");
        // Reborn: combine existing explicit path/root planning with rechecked reviewed-schema source bindings.
        Console.WriteLine("  sdk-typed-source-graph ra3ep1 <schema-root> <source-root> <source-entry.xml> <new-output-directory> [--local-defines | --include-defines | --definition-expressions | --self-attribute-inheritance | --self-child-copy | --self-complex-child-copy | --self-tree-copy | --instance-inheritance | --instance-root-files | --self-child-merge | --instance-chains | --instance-removals | --instance-choices | --instance-markers | --instance-bitflags | --instance-filters | --instance-upgrades | --instance-metadata | --instance-expressions | --instance-identical-states | --instance-state-readds | --instance-cross-state-removals | --instance-music-offsets | --instance-audio-trees | --instance-sound-offsets | --instance-sound-singletons] [--art-root absolute-directory] [--audio-root absolute-directory] [absolute.manifest=runtime.manifest ...]");
        Console.WriteLine("  sdk-typed-source-graph-self-test");
        Console.WriteLine("  sdk-local-defines-self-test");
        Console.WriteLine("  sdk-include-defines-self-test");
        Console.WriteLine("  sdk-definition-subset-self-test");
        Console.WriteLine("  sdk-self-attribute-inheritance-self-test");
        Console.WriteLine("  sdk-self-child-copy-self-test");
        // Reborn: advertise the separately scoped complex leaf admission test.
        Console.WriteLine("  sdk-self-complex-child-copy-self-test");
        // Reborn: advertise the bounded recursive sequence-tree admission test.
        Console.WriteLine("  sdk-self-tree-copy-self-test");
        // Reborn: expose the separately named empty-child merge fixture runner.
        Console.WriteLine("  sdk-self-child-merge-self-test");
        // Reborn: advertise imported-base eligibility and source-identity tests.
        Console.WriteLine("  sdk-instance-inheritance-self-test");
        // Reborn: advertise inherited alias-root field regression checks.
        Console.WriteLine("  sdk-instance-root-files-self-test");
        // Reborn: list the independently bounded recursive instance preparation runner.
        Console.WriteLine("  sdk-instance-chains-self-test");
        // Reborn: list the independent bounded removal fixture runner.
        Console.WriteLine("  sdk-instance-removals-self-test");
        // Reborn: expose the independent repeated-choice diagnostic test entry point.
        Console.WriteLine("  sdk-instance-choices-self-test");
        // Reborn: expose consumed-marker fixtures as an independently runnable diagnostic test.
        Console.WriteLine("  sdk-instance-markers-self-test");
        // Reborn: expose the separate enum-list modifier regression command.
        Console.WriteLine("  sdk-instance-bitflags-self-test");
        Console.WriteLine("  sdk-instance-filters-self-test");
        // Reborn: expose the independent upgrade singleton characterization group.
        Console.WriteLine("  sdk-upgrade-semantics-self-test");
        // Reborn: expose the separate upgrade normalization admission regression.
        Console.WriteLine("  sdk-instance-upgrades-self-test");
        // Reborn: expose metadata-only Include regressions separately from upgrade normalization.
        Console.WriteLine("  sdk-instance-metadata-self-test");
        // Reborn: expose expression stage regressions independently of metadata-only Includes.
        Console.WriteLine("  sdk-instance-expressions-self-test");
        // Reborn: expose destructive keyed-operation regression checks without authorizing normalization.
        Console.WriteLine("  sdk-sibling-identity-semantics-self-test");
        // Reborn: expose separately admitted identical-state regression coverage.
        Console.WriteLine("  sdk-instance-identical-states-self-test");
        // Reborn: expose resolved-base ordered state command regression coverage.
        Console.WriteLine("  sdk-instance-state-readds-self-test");
        // Reborn: separate pinned read-only review from owned metadata regression tests and production preprocessing.
        Console.WriteLine("  sdk-cc32-review <absolute-Uprising-source-root> <absolute-EP1-global.manifest>");
        Console.WriteLine("  sdk-cc32-review-self-test");
        // Reborn: expose exact-target removal-only regressions separately from CC32 review.
        Console.WriteLine("  sdk-instance-cross-state-removals-self-test");
        Console.WriteLine("  sdk-instance-music-offsets-self-test");
        Console.WriteLine("  sdk-instance-audio-trees-self-test");
        Console.WriteLine("  sdk-instance-sound-offsets-self-test");
        // Reborn: expose separate singleton admission tests, not an implicit extension of the arithmetic profile.
        Console.WriteLine("  sdk-instance-sound-singletons-self-test");
        // Reborn: expose independent sound singleton characterization, not a preprocessing flag.
        Console.WriteLine("  sdk-sound-singleton-semantics-self-test");
        // Reborn: isolated complete-owner review remains explicitly separate from graph admission.
        Console.WriteLine("  sdk-sound-owner-review <absolute-Uprising-source-root>");
        Console.WriteLine("  sdk-sound-owner-review-self-test");
        Console.WriteLine("  sdk-sound-expression-review ra3ep1 <schema-root> <source-root> <source-entry.xml> <new-output-directory>");
        // Reborn: the explicit source review remains partial and read-only even if both isolated owners validate.
        Console.WriteLine("  sdk-upgrade-semantics-review <absolute-upgrade.xml>");
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
        // Reborn: managed authored input checks do not invoke native codecs or write build output.
        Console.WriteLine("  ep1-audiofile-input-self-test");
        // Reborn: packaging proof is synthetic by default; actual encoded packaging remains an explicit encoder PoC run.
        Console.WriteLine("  audiofile-package-self-test");
        // Reborn: fixed local selectors and custom-data closure are separate from general SDK graph admission.
        Console.WriteLine("  local-audio-package-self-test");
        // Reborn: text identity compatibility must precede any claim of stock audio InstanceHash equivalence.
        Console.WriteLine("  hashing-writer-boundary-self-test");
        // Reborn: expose managed core/file identity verification separately from optional native encoding.
        Console.WriteLine("  audiofile-identity-self-test");
        // Reborn: keep real core preparation testing separate from explicit native codec execution.
        Console.WriteLine("  core-audiofile-preparation-self-test");
        // Reborn: test current source-to-package binding without native DLL execution.
        Console.WriteLine("  core-audio-package-gate-self-test");
        // Reborn: cleanup callback regression remains managed-only.
        Console.WriteLine("  audio-encoder-cleanup-self-test");
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
        // Reborn: distinguish actual core/native integration from the older authored-only experiment.
        Console.WriteLine("  core-audio-encoder-poc <absolute-audited-audio.dll>");
        // Reborn: one explicitly selected fault runs per worker process.
        Console.WriteLine("  core-audio-encoder-fault <absolute-audited-audio.dll> <scenario>");
        // Reborn: supervised native work and managed transport regressions are distinct entry points.
        Console.WriteLine("  supervised-core-audio-poc <absolute-audited-audio.dll>");
        Console.WriteLine("  audio-supervisor-self-test");
        // Reborn: native evidence corruption tests never run in the default compiler suite.
        Console.WriteLine("  supervised-audio-tamper-test <absolute-audited-audio.dll>");
        // Reborn: external authored input admission remains an explicitly opt-in narrow diagnostic command.
        Console.WriteLine("  supervised-authored-audio-poc <absolute-audited-audio.dll> <source-directory>");
        // Reborn: include caller event source only through the explicit four-file diagnostic command.
        Console.WriteLine("  supervised-authored-audio-event-poc <absolute-audited-audio.dll> <source-directory>");
        Console.WriteLine("  authored-audio-snapshot-self-test");
        // Reborn: native authored-content evidence is separate from managed snapshot admission tests.
        Console.WriteLine("  authored-audio-native-proof <absolute-audited-audio.dll>");
        // Reborn: keep caller event native execution out of all default managed regressions.
        Console.WriteLine("  authored-audio-event-native-proof <absolute-audited-audio.dll>");
        // Reborn: exercise three native list variants and their stale/tampered input rejection in separate child jobs.
        Console.WriteLine("  authored-audio-list-native-proof <absolute-audited-audio.dll>");
        // Reborn: native control-bit integration is explicit, never part of the default managed test runner.
        Console.WriteLine("  authored-audio-control-native-proof <absolute-audited-audio.dll>");
        // Reborn: variable audio pools remain a separate managed preflight boundary.
        Console.WriteLine("  authored-audio-pool-preflight <source-directory>");
        Console.WriteLine("  authored-audio-pool-self-test");
        // Reborn: variable raw encoding is supervised/opt-in, separate from managed preflight and fixed package proofs.
        Console.WriteLine("  supervised-audio-pool-preflight <source-directory>");
        Console.WriteLine("  supervised-audio-pool-encode <absolute-audited-audio.dll> <source-directory>");
        // Reborn: leaf-only variable packages are opt-in and tested separately from raw encoding.
        Console.WriteLine("  supervised-audio-pool-package <absolute-audited-audio.dll> <source-directory>");
        // Reborn: selected event closure and mixed package acceptance use a dedicated opt-in worker mode.
        Console.WriteLine("  supervised-audio-pool-event <absolute-audited-audio.dll> <source-directory>");
        // Reborn: expose only explicitly versioned leaf-only duration commands and opt-in native evidence.
        Console.WriteLine("  supervised-audio-duration-preflight <version-2-source-directory>");
        Console.WriteLine("  supervised-audio-duration-encode <absolute-audited-audio.dll> <version-2-source-directory>");
        Console.WriteLine("  supervised-audio-duration-package <absolute-audited-audio.dll> <version-2-source-directory>");
        Console.WriteLine("  audio-duration-pool-self-test");
        Console.WriteLine("  audio-duration-native-proof <absolute-audited-audio.dll>");
        // Reborn: keep mixed duration integration opt-in and distinct from leaf-only duration commands.
        Console.WriteLine("  supervised-audio-duration-event <absolute-audited-audio.dll> <version-2-source-directory>");
        Console.WriteLine("  audio-duration-event-native-proof <absolute-audited-audio.dll>");
        Console.WriteLine("  audio-pool-mixed-self-test");
        // Reborn: default vector tests never invoke the codec; native proof is opt-in.
        Console.WriteLine("  audio-event-vector-self-test");
        Console.WriteLine("  audio-event-vector-native-proof <absolute-audited-audio.dll>");
        Console.WriteLine("  audio-pool-mixed-native-proof <absolute-audited-audio.dll>");
        Console.WriteLine("  audio-pool-package-native-proof <absolute-audited-audio.dll>");
        Console.WriteLine("  audio-pool-package-self-test");
        // Reborn: event preflight consumes validated encoded worker evidence, not an unchecked arbitrary source graph.
        Console.WriteLine("  audio-pool-event-preflight <encoded-worker-directory> <event-xml>");
        Console.WriteLine("  audio-pool-event-self-test");
        Console.WriteLine("  audio-pool-event-native-proof <absolute-audited-audio.dll>");
        Console.WriteLine("  audio-pool-worker-self-test");
        Console.WriteLine("  audio-pool-native-proof <absolute-audited-audio.dll>");
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
