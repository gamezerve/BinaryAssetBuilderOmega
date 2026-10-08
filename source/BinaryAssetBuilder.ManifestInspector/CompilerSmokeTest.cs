using System.Buffers.Binary;
using System.Xml;
using BinaryAssetBuilder.Core.Hashing;
using Relo;
using SageBinaryData;

namespace BinaryAssetBuilder.ManifestInspector;

internal static class CompilerSmokeTest
{
    public static unsafe void Run()
    {
        InitializeHashProvider();
        // Reborn: current XML identity hashing must include exact reference-proven text block boundaries.
        TestHashingWriterBoundary();
        // Reborn: reconstruct actual AudioFile XML/file identity before permitting any codec processor integration.
        TestAudioFileIdentity();
        // Reborn: connect normalized core XML/file identity to immutable authored PCM preparation without codecs.
        TestCoreAudioFilePreparation();
        // Reborn: publication must recheck actual core audio bindings even when compressed framing is synthetic in default tests.
        TestCoreAudioPackageGate();
        // Reborn: native cleanup control flow is tested with managed callbacks so default tests never load codecs.
        TestAudioEncoderCleanup();
        // Reborn: supervise real managed transport failures without loading native codecs in default tests.
        TestAudioEncoderSupervisor();
        // Reborn: caller-source XML/PCM must pass bounded snapshot admission without executing native codecs.
        TestAuthoredAudioSnapshot();
        // Reborn: variable inventory/core preparation is managed-only, separate from the fixed native audio worker path.
        TestAuthoredAudioPool();
        // Reborn: variable pool transport and acceptance run real managed workers, never native codecs in default tests.
        TestAudioPoolWorker();
        // Reborn: bounded variable package tables are tested with synthetic compressed bodies, never codecs by default.
        TestAudioPoolPackage();
        // Reborn: variable pool event closure remains a managed-only synthetic-framing regression by default.
        TestAudioPoolEvent();
        // Reborn: mixed variable closure uses synthetic codec framing in the default suite.
        TestAudioPoolMixed();
        // Reborn: larger event Sound vectors remain synthetic-framing/managed-only in default tests.
        TestAudioEventVectors();
        TestAttributeModifier();
        // Reborn: exercise isolated EP1 sound records while retaining the legacy child ABI.
        TestMultisoundNative();
        // Reborn: recover native AudioEvent ABI without changing legacy audio or enabling codecs.
        TestAudioEventNative();
        // Reborn: recover AudioFile envelopes without changing the legacy runtime ABI or opening codec generation.
        TestAudioFileRuntime();
        // Reborn: independent runtime bytes must match stock while the legacy AudioFile ABI remains unchanged.
        TestAudioFileSerialization();
        // Reborn: authored AudioFile inputs must pass official schema, PCM and immutable current-source gates before optional native work.
        TestAudioFileInput();
        // Reborn: experimental duration preparation is managed-only and cannot widen existing audio worker/package admission.
        TestAudioDurationCandidate();
        // Reborn: versioned duration core/worker/package regressions remain managed-only in the default suite.
        TestAudioDurationPool();
        // Reborn: mixed duration event closure uses synthetic compressed bodies in default tests, never native codecs.
        TestAudioDurationEvent();
        // Reborn: SDK planning must validate explicit EP1 metadata without writing outputs or changing legacy discovery/settings.
        TestSdkEnvironmentPreflight();
        // Reborn: source-path planning remains a separate bounded fixture from production dependency resolution.
        TestSdkSourcePathAudit();
        // Reborn: typed declaration inventory is distinct from literal path graph and production dependency binding.
        TestSdkFileReferenceCatalog();
        // Reborn: effective source binding must not hide the staged duplicate-schema blocker.
        TestSdkEffectiveSchema();
        // Reborn: diagnostic schema normalization must remain fingerprint-pinned and distinct from reference evidence.
        TestSdkShieldSchemaCandidate();
        // Reborn: exact reviewed warning admission must not become a production or generic ignore-warnings gate.
        TestSdkSchemaHookReview();
        // Reborn: graph source snapshots and XML/resource/preprocessing states need independent bounded regressions.
        TestSdkTypedSourceGraph();
        TestSdkLocalDefineProfile();
        TestSdkIncludeDefineProfile();
        TestSdkDefinitionSubset();
        TestSdkSelfAttributeInheritance();
        TestSdkSelfChildCopy();
        // Reborn: register independently scoped complex leaf copy fixtures in the default runner.
        TestSdkSelfComplexChildCopy();
        // Reborn: execute recursive sequence-tree admission independently from all earlier copy-only profiles.
        TestSdkSelfTreeCopy();
        // Reborn: run the independent two-sided empty-child matching group in the default managed suite.
        TestSdkSelfChildMerge();
        // Reborn: register imported-instance eligibility and snapshot/provenance guards independently.
        TestSdkInstanceInheritance();
        // Reborn: register root-qualified imported files without enabling defining-document relative fallback.
        TestSdkInstanceRootFiles();
        // Reborn: register recursive preparation separately from older direct-only instance profiles.
        TestSdkInstanceChains();
        // Reborn: register literal keyed removal behavior separately from chain-only preparation.
        TestSdkInstanceRemovals();
        // Reborn: characterize choice copying and destructive matching without expanding any existing admission profile.
        TestSdkChoiceSemantics();
        // Reborn: register separately admitted repeated-choice copying after the unchanged core characterization.
        TestSdkInstanceChoices();
        // Reborn: register consumed pipeline-marker semantics independently of retained-marker choice preparation.
        TestSdkInstanceMarkers();
        // Reborn: register whole-token-proven modifiers separately from marker-only preparation.
        TestSdkInstanceBitflags();
        // Reborn: register one-sided matched micromanager filter payload copying independently of enum modifiers.
        TestSdkInstanceFilters();
        // Reborn: characterize upgrade singleton folding separately without widening any diagnostic profile.
        TestSdkUpgradeSemantics();
        // Reborn: register independent complementary-singleton admission after core characterization.
        TestSdkInstanceUpgrades();
        // Reborn: independently register metadata-only leaf Includes without instance exports.
        TestSdkInstanceMetadata();
        // Reborn: register expression-before-inheritance stage admission independently of metadata-only Includes.
        TestSdkInstanceExpressions();
        // Reborn: characterize sibling identity operations independently without widening preprocessing profiles.
        TestSdkSiblingIdentitySemantics();
        // Reborn: independently register identical-state admission without relaxing command-pair or cross-QName guards.
        TestSdkInstanceIdenticalStates();
        // Reborn: register ordered state command admission independently of literal duplicate coalescing.
        TestSdkInstanceStateReadds();
        // Reborn: register CC32 metadata identity review without opening cross-QName preprocessing.
        TestSdkCc32Review();
        // Reborn: register separately proved empty cross-QName state removal without changing earlier defaults.
        TestSdkInstanceCrossStateRemovals();
        // Reborn: pin bounded MusicTrack.Volume arithmetic and definition-context isolation independently of earlier profiles.
        TestSdkInstanceMusicOffsets();
        // Reborn: pin independent shallow broad-audio resource admission and unchanged generic tree bounds.
        TestSdkInstanceAudioTrees();
        // Reborn: pin separately typed sound arithmetic without widening earlier Music/tree profiles.
        TestSdkInstanceSoundOffsets();
        // Reborn: characterize conflicting sound singletons without widening source admission.
        TestSdkSoundSingletonSemantics();
        // Reborn: prove complete isolated sound owner projection independently of preprocessing admission.
        TestSdkSoundOwnerReview();
        // Reborn: register independent full-owner-proved singleton admission and atomic refusal fixtures.
        TestSdkInstanceSoundSingletons();
        // Reborn: classify dependency occurrences without changing resolver admission.
        TestSdkDependencyReview();
        // Reborn: register exact map alias capture/replay without widening strict path defaults.
        TestSdkKnownMapAliases();
        // Reborn: reconcile exact authored/stock music identities and bounded raw words without admitting a Pathfinder processor.
        TestPathMusicStockReview();
        // Reborn: characterize reference header lexical/zero semantics independently of music processor admission.
        TestPathMusicHeaderSemantics();
        // Reborn: independent music runtime tracker goldens and strict literal preparation remain isolated from production processors.
        TestPathMusicRuntimeProbe();
        // Reborn: pin local authored music content snapshots and stale-input refusal independently of official AUDIO closure.
        TestPathMusicAuthoredSnapshot();
        // Reborn: local music package framing stays isolated from official type hashes and production publication.
        TestPathMusicPackage();
        // Reborn: actual Core music hashing remains synthetic-domain evidence independent of package identity fields.
        TestPathMusicCoreIdentity();
        // Reborn: pinned reference processing metadata cannot silently become EP1 production identity.
        TestPathMusicReferenceIdentity();
        // Reborn: bind fresh actual Core music identity to frozen native preparation without production package admission.
        TestPathMusicCorePreparation();
        // Reborn: publication must prove fresh Core music binding without silently changing diagnostic hash policy.
        TestPathMusicCorePackage();
        // Reborn: pin actual padded output checksum before experimental music manifest identity admission.
        TestPathMusicCoreChecksum();
        // Reborn: require versioned synthetic music identities and cross-profile refusal before experimental publication.
        TestPathMusicExperimentalCore();
        // Reborn: controlled registered music dispatch must retain every production and immutable-closure guard.
        TestPathMusicControlledCompiler();
        // Reborn: explicit local v2 metadata must enter real Core selection while old zero-hash profiles remain excluded.
        TestPathMusicSelectedCompiler();
        // Reborn: selected actual native payloads require a distinct v2 package policy and unchanged publication safeguards.
        TestPathMusicSelectedPackage();
        // Reborn: fixed custom-data packaging uses synthetic framing and must not initialize native codecs in default tests.
        TestAudioFilePackage();
        // Reborn: fixed local event/audio closure must prove selectors and dependency fingerprint invalidation without native codecs.
        TestLocalAudioPackage();
        // Reborn: custom sound frame boundaries must agree with native totals without decoding compressed payloads.
        TestAudioCustomData();
        // Reborn: pin original/unpacked comparison boundaries without modifying reference corpus files.
        TestAudioArchiveComparison();
        // Reborn: metadata and WAV fixture regressions must not execute native codecs during the ordinary compiler suite.
        TestNativeAudioApi();
        TestAudioEncoderWave();
        // Reborn: prove checked sound entry policies and prepared dependency identities separately from native-only evidence.
        TestMultisoundProfile();
        // Reborn: checked AudioEvent admission stays separate from native evidence and public streams.
        TestAudioEventProfile();
        // Reborn: local AudioEvent closure requires external AudioFile fingerprint and independent mixed-stream proofs.
        TestAudioEventFXStream();
        // Reborn: general command event admission must preserve snapshot, ordering and publication contracts.
        TestDiagnosticAudioEventBuild();
        // Reborn: exercise local Multisound closure and independent mixed stream identity/native readback.
        TestMultisoundFXStream();
        // Reborn: public bounded sound admission must preserve all prior snapshot/stream/publication gates.
        TestDiagnosticMultisoundBuild();
        TestDieMuxData();
        TestGameDependency();
        TestSlowDeath();
        TestInvisibilityUpdate();
        TestInvisibilitySpecialPower();
        TestLocomotorTemplate();
        TestWeaponTemplate();
        TestTintObjectsNugget();
        // Reborn: exercise the shared containment header and relocated passenger stride.
        TestOpenContain();
        // Reborn: guard transport inheritance and the by-value garrison roster.
        TestContainDerivatives();
        // Reborn: check optional vector relocation and the newly restored contestable dispatch.
        TestGarrisonDerivatives();
        // Reborn: cover normalized slaughter refunds, FX imports and the Heal/Tunnel tails.
        TestContainLeafModules();
        // Reborn: verify the corrected horde header, consecutive ranks and EVA/modifier imports.
        TestHordeContain();
        // Reborn: cover production queue dispatch, weak IDs and optional filters across consecutive records.
        TestProductionQueueHordeContain();
        // Reborn: check restored attach defaults, EP1 flags and optional mask pointers.
        TestAttachUpdate();
        // Reborn: guard infiltrator imports and the restored laser-family records.
        TestInfiltratorContain();
        TestLaserStateFamily();
        // Reborn: validate schema-inserted defaults through the production reference normalizer.
        TestReferencePipeline();
        // Reborn: guard external runtime paths, patch roles and stale manifest identity caches.
        TestExternalManifestLinks();
        // Reborn: keep conflicting or wrong-game metadata from masquerading as compiler readiness.
        TestTypeRegistryAudit();
        // Reborn: verify native-to-tokenized armor conversion and isolated EP1 stream serialization.
        TestArmorTokenPipeline();
        // Reborn: validate the isolated EP1 processor profile, its metadata and fail-closed registry policies.
        TestEp1ArmorProfile();
        // Reborn: validate real document stages and disabled session/precompiled reuse for experimental profiles.
        TestEp1ArmorDocument();
        // Reborn: prevent auxiliary stream loss when repairing partially valid linked output generations.
        TestLinkedStreamRepair();
        // Reborn: distinguish identity checksum compatibility and tokenized patch matching from byte-integrity validation.
        TestChecksumAudit();
        // Reborn: failed local/cache candidates must not delete previously valid intermediate output.
        TestCopyRecovery();
        // Reborn: file-reference changes must invalidate source identities while runtime ID dependencies remain distinct.
        TestDependencyHashes();
        // Reborn: reported content changes cannot hide behind unchanged timestamps or prior resident metadata snapshots.
        TestWatcherCache();
        // Reborn: cache initialization cannot erase new callbacks or discard a failed notification handoff.
        TestMonitorBatch();
        // Reborn: exercise retained and serialized document reload decisions after actual source/dependency mutations.
        TestDocumentReuse();
        // Reborn: establish the second EP1 root processor proof without changing the production type registry.
        TestAttributeModifierNative();
        // Reborn: bind the validated native subset to explicit experimental compiler/document policy.
        TestEp1ModifierProfile();
        // Reborn: compare real core-normalized imports and shader pointer records with the final runtime encoding contract.
        TestModifierImports();
        // Reborn: output dependency retries must never trust partially validated or unmapped external targets.
        TestDependencyResolution();
        // Reborn: validate the inline ObjectFilter root and ordered weak lists before any EP1 registration.
        TestObjectFilterNative();
        // Reborn: verify isolated ObjectFilter compiler entry and checked weak metadata without relaxing production policy.
        TestObjectFilterProfile();
        // Reborn: recover shader root/rule layout and verify material POIDs do not become imports.
        TestShaderOverrideNative();
        // Reborn: exercise isolated shader descriptor/document entry and fail closed on stale controls or identity.
        TestShaderOverrideProfile();
        // Reborn: prove modifier imports select registered local shader targets through the real document dependency stage.
        TestModifierShaderGraph();
        // Reborn: test nested Include locations and mixed native selector identities without external target compilation.
        TestIncludedModifierShader();
        // Reborn: write and read only a bounded diagnostic stream from resolved native compiler entries.
        TestModifierShaderStream();
        // Reborn: verify bounded command admission/publication independently of the fixed stream fixture.
        TestBoundedDiagnosticBuild();
        TestDiagnosticIncludeBuild();
        // Reborn: weak filter IDs must survive three-family command serialization without invented strong imports.
        TestDiagnosticFilterBuild();
        // Reborn: recover native FX root/polymorphic base before opening a runnable processor profile.
        TestFXListNative();
        // Reborn: prove schema-derived concrete audio metadata separately from native FX layout.
        TestFXAudioResolution();
        // Reborn: prove checked FX compiler entries separately from diagnostic stream command admission.
        TestFXProfile();
        // Reborn: prove mixed local FX/external audio stream readback before broadening diagnostic command admission.
        TestModifierFXStream();
        // Reborn: verify narrow FX support through the public bounded build service with all publication guards intact.
        TestDiagnosticFXBuild();
        TestSpawnedSlaveUpdate();
        TestUnitUnpackUpdate();
        TestAddObjectsToLiftUpdate();
        TestLureObjectsUpdate();
        TestFlingStoredObjectsSpecialPower();
        TestLiftObjectUpdate();
        TestProjectileReplaceSelfSpecialAbility();
        TestProjectilePath();
        TestYurikoHotKeys();
        TestDynamicsSettings();
        TestMainMenuPersonality();
        TestOverridableAudio();
        TestMovieArchive();
        TestScenarioUi();
        TestScenarioManager();
        TestRedAlertButton();
        TestUprisingMusicConditions();
        TestMapNameHeuristic();
        TestDynamicsJointSet();
        TestDynamicsDraw();
        TestScriptedModelRecords();
        TestAudioDynamicsCollide();
        TestDamageDynamicsCollide();
        TestReactionFXOnDamage();
        TestDamageSphereUpdate();
        TestYurikoShieldSphereUpdate();
        TestGameObject();
        Console.WriteLine("Uprising compiler self-test: OK");
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: seed weak-ID and voice StringHash bins used by standalone compiler smoke tests. */
    //-------------------------------------------------------------------------------------------------
    internal static void InitializeHashProvider()
    {
        Settings.Current = new Settings
        {
            StringHashBinDescriptors =
            [
                new StringHashBinDescriptor
                {
                    SchemaTypeName = "ObjectPersistenceID",
                    BinName = "POID",
                    IsCaseSensitive = false
                },
                // Reborn: voice-event StringHash marshalling records case-sensitive hashes in this bin.
                new StringHashBinDescriptor
                {
                    SchemaTypeName = "StringHash",
                    BinName = "STRINGHASH",
                    IsCaseSensitive = true
                }
            ]
        };
        HashProvider.InitializeStringHashes(Path.GetTempPath());
    }

    private static unsafe void TestAttributeModifier()
    {
        const string xml = """
            <AttributeModifier xmlns="uri:ea.com:eala:asset"
                Category="SHRINK" Duration="1s"
                ReplaceInCategoryIfLongest="true" IgnoreIfAnticategoryActive="true"
                StartFX="FXList\123" EndFX="FXList\456"
                ModelConditionsSet="SPECIAL_POWER_SELECTED_PENDING"
                ModelConditionsClear="TOPPLED"
                ObjectStatusToSet="BRIDGE_DEAD"
                StackingLimit="2" ArmorSetType="SHRINK_EFFECT"
                Shader="ShaderOverride\789">
              <Modifier Type="RADIATION_ARMOR" Value="50%" />
            </AttributeModifier>
            """;

        XmlDocument document = new();
        document.LoadXml(xml);
        XmlNamespaceManager namespaces = new(document.NameTable);
        namespaces.AddNamespace("ea", "uri:ea.com:eala:asset");
        Node node = new(document.CreateNavigator()!.SelectSingleNode("/ea:AttributeModifier", namespaces)!, namespaces);

        AttributeModifier* root;
        using Tracker tracker = new((void**)&root, (uint)sizeof(AttributeModifier), false);
        Marshaler.Marshal(node, root, tracker);
        Chunk chunk = new();
        if (!tracker.MakeRelocatable(chunk))
        {
            throw new InvalidDataException("Tracker failed to produce a relocatable chunk.");
        }

        ReadOnlySpan<byte> instance = chunk.InstanceBuffer;
        // Reborn: the optional shader adds its own four-byte record after the three mask payloads.
        Expect(instance.Length == 220, "instance bytes", 220, instance.Length);
        Expect(ReadUInt32(instance, 4) == 15, "Category=SHRINK", 15, ReadUInt32(instance, 4));
        Expect(ReadUInt32(instance, 12) == 124, "StartFX import", 124, ReadUInt32(instance, 12)); // Reborn: final BIN imports are one-biased.
        Expect(ReadUInt32(instance, 16) == 457, "EndFX import", 457, ReadUInt32(instance, 16)); // Reborn: final BIN imports are one-biased.
        Expect(ReadUInt32(instance, 20) == 56, "ModelConditionsSet relocation", 56, ReadUInt32(instance, 20));
        Expect(ReadUInt32(instance, 24) == 116, "ModelConditionsClear relocation", 116, ReadUInt32(instance, 24));
        Expect(ReadUInt32(instance, 28) == 176, "ObjectStatusToSet relocation", 176, ReadUInt32(instance, 28));
        Expect(ReadUInt32(instance, 32) == 2, "StackingLimit", 2, ReadUInt32(instance, 32));
        Expect(ReadUInt32(instance, 36) == 14, "ArmorSetType=SHRINK_EFFECT", 14, ReadUInt32(instance, 36));
        Expect(ReadUInt32(instance, 40) == 208, "Shader record relocation", 208, ReadUInt32(instance, 40));
        Expect(ReadUInt32(instance, 208) == 790, "Shader record import", 790, ReadUInt32(instance, 208));
        Expect(ReadUInt32(instance, 44) == 1, "Modifier count", 1, ReadUInt32(instance, 44));
        Expect(ReadUInt32(instance, 48) == 212, "Modifier relocation", 212, ReadUInt32(instance, 48));
        Expect(instance[52] == 1, "ReplaceInCategoryIfLongest", 1, instance[52]);
        Expect(instance[53] == 1, "IgnoreIfAnticategoryActive", 1, instance[53]);
        Expect(ReadUInt32(instance, 212) == 36, "Modifier.Type=RADIATION_ARMOR", 36, ReadUInt32(instance, 212));
        Expect(chunk.RelocationBuffer.Length == 24, "relocation bytes", 24, chunk.RelocationBuffer.Length);
        Expect(chunk.ImportsBuffer.Length == 16, "imports bytes", 16, chunk.ImportsBuffer.Length);

        Console.WriteLine(
            $"  AttributeModifier bin={chunk.InstanceBuffer.Length}, " +
            $"relo={chunk.RelocationBuffer.Length}, imp={chunk.ImportsBuffer.Length}");
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: Verify DieMux emits relocated Uprising object-status masks behind the RA3 pointer ABI. */
    //-------------------------------------------------------------------------------------------------
    private static unsafe void TestDieMuxData()
    {
        const string xml = """
            <DieMuxData xmlns="uri:ea.com:eala:asset"
                ExemptStatus="DESTROYED" RequiredStatus="CAN_ATTACK"
                DamageAmountRequired="25" MinKillerAngle="10d" MaxKillerAngle="20d"
                DeathTypes="NORMAL" DeathTypesForbidden="CRUSHED" />
            """;

        XmlDocument document = new();
        document.LoadXml(xml);
        XmlNamespaceManager namespaces = new(document.NameTable);
        namespaces.AddNamespace("ea", "uri:ea.com:eala:asset");
        Node node = new(document.CreateNavigator()!.SelectSingleNode("/ea:DieMuxData", namespaces)!, namespaces);

        DieMuxDataType* root;
        using Tracker tracker = new((void**)&root, (uint)sizeof(DieMuxDataType), false);
        Marshaler.Marshal(node, root, tracker);
        Chunk chunk = new();
        tracker.MakeRelocatable(chunk);
        ReadOnlySpan<byte> instance = chunk.InstanceBuffer;
        Expect(instance.Length == 108, "DieMuxData instance bytes", 108, instance.Length);
        Expect(ReadUInt32(instance, 8) == 44, "DieMuxData ExemptStatus relocation", 44, ReadUInt32(instance, 8));
        Expect(ReadUInt32(instance, 12) == 76, "DieMuxData RequiredStatus relocation", 76, ReadUInt32(instance, 12));
        Expect(ReadUInt32(instance, 16) == 0x41C80000, "DieMuxData DamageAmountRequired", 0x41C80000, ReadUInt32(instance, 16));
        Expect(ReadUInt32(instance, 44) == 1, "DieMuxData ExemptStatus=DESTROYED", 1, ReadUInt32(instance, 44));
        Expect(ReadUInt32(instance, 76) == 2, "DieMuxData RequiredStatus=CAN_ATTACK", 2, ReadUInt32(instance, 76));
        Expect(chunk.RelocationBuffer.Length == 12, "DieMuxData relocation bytes", 12, chunk.RelocationBuffer.Length);
        Expect(chunk.ImportsBuffer.Length == 0, "DieMuxData imports bytes", 0, chunk.ImportsBuffer.Length);
        Console.WriteLine($"  DieMuxData bin={chunk.InstanceBuffer.Length}, relo={chunk.RelocationBuffer.Length}, imp={chunk.ImportsBuffer.Length}");
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: Verify GameDependency emits its three variable-width masks through RA3 relocation slots. */
    //-------------------------------------------------------------------------------------------------
    private static unsafe void TestGameDependency()
    {
        const string xml = """
            <GameDependency xmlns="uri:ea.com:eala:asset"
                RequiredModelConditionsAny="USER_1"
                ForbiddenModelConditions="USER_2"
                RequiredObjectStatusAny="CAN_ATTACK" />
            """;

        XmlDocument document = new();
        document.LoadXml(xml);
        XmlNamespaceManager namespaces = new(document.NameTable);
        namespaces.AddNamespace("ea", "uri:ea.com:eala:asset");
        Node node = new(document.CreateNavigator()!.SelectSingleNode("/ea:GameDependency", namespaces)!, namespaces);

        GameDependencyType* root;
        using Tracker tracker = new((void**)&root, (uint)sizeof(GameDependencyType), false);
        Marshaler.Marshal(node, root, tracker);
        Chunk chunk = new();
        tracker.MakeRelocatable(chunk);
        ReadOnlySpan<byte> instance = chunk.InstanceBuffer;
        Expect(instance.Length == 192, "GameDependency instance bytes", 192, instance.Length);
        Expect(ReadUInt32(instance, 0) == 40, "GameDependency required-model relocation", 40, ReadUInt32(instance, 0));
        Expect(ReadUInt32(instance, 4) == 100, "GameDependency forbidden-model relocation", 100, ReadUInt32(instance, 4));
        Expect(ReadUInt32(instance, 8) == 160, "GameDependency object-status relocation", 160, ReadUInt32(instance, 8));
        Expect(ReadUInt32(instance, 160) == 2, "GameDependency RequiredObjectStatusAny=CAN_ATTACK", 2, ReadUInt32(instance, 160));
        Expect(chunk.RelocationBuffer.Length == 16, "GameDependency relocation bytes", 16, chunk.RelocationBuffer.Length);
        Expect(chunk.ImportsBuffer.Length == 0, "GameDependency imports bytes", 0, chunk.ImportsBuffer.Length);
        Console.WriteLine($"  GameDependency bin={chunk.InstanceBuffer.Length}, relo={chunk.RelocationBuffer.Length}, imp={chunk.ImportsBuffer.Length}");
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: Verify SlowDeath writes all optional condition masks through their native RA3 pointers. */
    //-------------------------------------------------------------------------------------------------
    private static unsafe void TestSlowDeath()
    {
        const string xml = """
            <SlowDeath xmlns="uri:ea.com:eala:asset"
                SinkRate="1" DeathFlags="USER_1" DeathTypes="USER_2"
                DeathObjectStatusBits="CAN_ATTACK" Fade="true" FadeTime="2s" />
            """;

        XmlDocument document = new();
        document.LoadXml(xml);
        XmlNamespaceManager namespaces = new(document.NameTable);
        namespaces.AddNamespace("ea", "uri:ea.com:eala:asset");
        Node node = new(document.CreateNavigator()!.SelectSingleNode("/ea:SlowDeath", namespaces)!, namespaces);

        SlowDeathBehaviorModuleData* root;
        using Tracker tracker = new((void**)&root, (uint)sizeof(SlowDeathBehaviorModuleData), false);
        Marshaler.Marshal(node, root, tracker);
        Chunk chunk = new();
        tracker.MakeRelocatable(chunk);
        ReadOnlySpan<byte> instance = chunk.InstanceBuffer;
        Expect(instance.Length == 268, "SlowDeath instance bytes", 268, instance.Length);
        Expect(ReadUInt32(instance, 56) == 116, "SlowDeath DeathFlags relocation", 116, ReadUInt32(instance, 56));
        Expect(ReadUInt32(instance, 68) == 176, "SlowDeath DeathTypes relocation", 176, ReadUInt32(instance, 68));
        Expect(ReadUInt32(instance, 72) == 236, "SlowDeath DeathObjectStatusBits relocation", 236, ReadUInt32(instance, 72));
        Expect(ReadUInt32(instance, 60) == 0x40000000, "SlowDeath FadeTime", 0x40000000, ReadUInt32(instance, 60));
        Expect(instance[113] == 1, "SlowDeath Fade", 1, instance[113]);
        Expect(ReadUInt32(instance, 236) == 2, "SlowDeath status=CAN_ATTACK", 2, ReadUInt32(instance, 236));
        Console.WriteLine($"  SlowDeath bin={chunk.InstanceBuffer.Length}, relo={chunk.RelocationBuffer.Length}, imp={chunk.ImportsBuffer.Length}");
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: Verify template-based Uprising invisibility update data and its inline EP1 object filter. */
    //-------------------------------------------------------------------------------------------------
    private static unsafe void TestInvisibilityUpdate()
    {
        const string xml = """
            <InvisibilityUpdate xmlns="uri:ea.com:eala:asset"
                InvisibilityTemplate="InvisibilityTemplate\321"
                UpdatePeriod="2s" RequiredNearbyObjectRange="50">
              <RequiresNearbyObjectFilter Rule="ALL" Include="INFANTRY" />
            </InvisibilityUpdate>
            """;

        XmlDocument document = new();
        document.LoadXml(xml);
        XmlNamespaceManager namespaces = new(document.NameTable);
        namespaces.AddNamespace("ea", "uri:ea.com:eala:asset");
        Node node = new(document.CreateNavigator()!.SelectSingleNode("/ea:InvisibilityUpdate", namespaces)!, namespaces);

        InvisibilityUpdateModuleData* root;
        using Tracker tracker = new((void**)&root, (uint)sizeof(InvisibilityUpdateModuleData), false);
        Marshaler.Marshal(node, root, tracker);
        Chunk chunk = new();
        tracker.MakeRelocatable(chunk);
        ReadOnlySpan<byte> instance = chunk.InstanceBuffer;
        Expect(instance.Length == 148, "InvisibilityUpdate instance bytes", 148, instance.Length);
        Expect(ReadUInt32(instance, 8) == 322, "InvisibilityUpdate template import", 322, ReadUInt32(instance, 8)); // Reborn: final BIN imports are one-biased.
        Expect(ReadUInt32(instance, 12) == 0x40000000, "InvisibilityUpdate period", 0x40000000, ReadUInt32(instance, 12));
        Expect(ReadUInt32(instance, 16) == 0x42480000, "InvisibilityUpdate nearby range", 0x42480000, ReadUInt32(instance, 16));
        Expect(ReadUInt32(instance, 32) == 1, "InvisibilityUpdate filter rule", 1, ReadUInt32(instance, 32));
        Expect(chunk.ImportsBuffer.Length == 8, "InvisibilityUpdate imports bytes", 8, chunk.ImportsBuffer.Length);
        Console.WriteLine($"  InvisibilityUpdate bin={chunk.InstanceBuffer.Length}, relo={chunk.RelocationBuffer.Length}, imp={chunk.ImportsBuffer.Length}");
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: Verify Uprising invisibility special power field order and optional filter relocation. */
    //-------------------------------------------------------------------------------------------------
    private static unsafe void TestInvisibilitySpecialPower()
    {
        const string xml = """
            <InvisibilitySpecialPower xmlns="uri:ea.com:eala:asset"
                InvisibilityTemplate="InvisibilityTemplate\654"
                BroadcastRadius="75" Duration="3s" Permanent="true">
              <ObjectFilter Rule="ANY" Include="VEHICLE" />
            </InvisibilitySpecialPower>
            """;

        XmlDocument document = new();
        document.LoadXml(xml);
        XmlNamespaceManager namespaces = new(document.NameTable);
        namespaces.AddNamespace("ea", "uri:ea.com:eala:asset");
        Node node = new(document.CreateNavigator()!.SelectSingleNode("/ea:InvisibilitySpecialPower", namespaces)!, namespaces);

        InvisibilitySpecialPowerModuleData* root;
        using Tracker tracker = new((void**)&root, (uint)sizeof(InvisibilitySpecialPowerModuleData), false);
        Marshaler.Marshal(node, root, tracker);
        Chunk chunk = new();
        tracker.MakeRelocatable(chunk);
        ReadOnlySpan<byte> instance = chunk.InstanceBuffer;
        Expect(instance.Length == 616, "InvisibilitySpecialPower instance bytes", 616, instance.Length);
        Expect(ReadUInt32(instance, 476) == 655, "InvisibilitySpecialPower template import", 655, ReadUInt32(instance, 476)); // Reborn: final BIN imports are one-biased.
        Expect(ReadUInt32(instance, 480) == 0x42960000, "InvisibilitySpecialPower radius", 0x42960000, ReadUInt32(instance, 480));
        Expect(ReadUInt32(instance, 484) == 0x40400000, "InvisibilitySpecialPower duration", 0x40400000, ReadUInt32(instance, 484));
        Expect(ReadUInt32(instance, 488) == 496, "InvisibilitySpecialPower filter relocation", 496, ReadUInt32(instance, 488));
        Expect(instance[492] == 1, "InvisibilitySpecialPower permanent", 1, instance[492]);
        Expect(chunk.ImportsBuffer.Length == 8, "InvisibilitySpecialPower imports bytes", 8, chunk.ImportsBuffer.Length);
        Console.WriteLine($"  InvisibilitySpecialPower bin={chunk.InstanceBuffer.Length}, relo={chunk.RelocationBuffer.Length}, imp={chunk.ImportsBuffer.Length}");
    }

    private static unsafe void TestLocomotorTemplate()
    {
        const string xml = """
            <LocomotorTemplate xmlns="uri:ea.com:eala:asset"
                Surfaces="CRUSHABLE_WALL" BehaviorZ="DO_NOT_MODIFY_HEIGHT"
                IgnoreLowSpeedAngleMultiplier="true">
              <JetLocomotorData Options="NO_CIRCLE_WHILE_USING_SPECIALPOWER"
                  AttackPathStartRunDistance="10" AttackPathClimbDistance="20"
                  AttackPathDiveDistanceStart="30" AttackPathDiveDistanceEnd="40" />
            </LocomotorTemplate>
            """;

        XmlDocument document = new();
        document.LoadXml(xml);
        XmlNamespaceManager namespaces = new(document.NameTable);
        namespaces.AddNamespace("ea", "uri:ea.com:eala:asset");
        Node node = new(document.CreateNavigator()!.SelectSingleNode("/ea:LocomotorTemplate", namespaces)!, namespaces);

        LocomotorTemplate* root;
        using Tracker tracker = new((void**)&root, (uint)sizeof(LocomotorTemplate), false);
        Marshaler.Marshal(node, root, tracker);
        Chunk chunk = new();
        tracker.MakeRelocatable(chunk);
        ReadOnlySpan<byte> instance = chunk.InstanceBuffer;
        Expect(instance.Length == 416, "Locomotor instance bytes", 416, instance.Length);
        Expect(ReadUInt32(instance, 4) == 0x400, "Surfaces=CRUSHABLE_WALL", 0x400, ReadUInt32(instance, 4));
        Expect(ReadUInt32(instance, 96) == 11, "BehaviorZ=DO_NOT_MODIFY_HEIGHT", 11, ReadUInt32(instance, 96));
        Expect(ReadUInt32(instance, 364) == 396, "JetLocomotorData relocation", 396, ReadUInt32(instance, 364));
        Expect(instance[393] == 1, "IgnoreLowSpeedAngleMultiplier", 1, instance[393]);
        Expect(ReadUInt32(instance, 396) == 1, "JetLocomotorData.Options", 1, ReadUInt32(instance, 396));
        Expect(chunk.RelocationBuffer.Length == 8, "Locomotor relocation bytes", 8, chunk.RelocationBuffer.Length);
        Expect(chunk.ImportsBuffer.Length == 0, "Locomotor imports bytes", 0, chunk.ImportsBuffer.Length);
        Console.WriteLine(
            $"  LocomotorTemplate bin={chunk.InstanceBuffer.Length}, " +
            $"relo={chunk.RelocationBuffer.Length}, imp={chunk.ImportsBuffer.Length}");
    }

    private static unsafe void TestWeaponTemplate()
    {
        const string xml = """
            <WeaponTemplate xmlns="uri:ea.com:eala:asset"
                AttackRange="250" ScatterAlways="true"
                ProjectileSelf="true" ProjectileSelfUsesPathfinder="false"
                Flags="FORCE_KILL_GARRISONED_UNITS"
                RequiredAntiMask="ANTI_WATER ANTI_LIFTED_GROUND_UNIT"
                VirtualDamage="SHARE" UpdateBarrelModelConditions="true" />
            """;

        XmlDocument document = new();
        document.LoadXml(xml);
        XmlNamespaceManager namespaces = new(document.NameTable);
        namespaces.AddNamespace("ea", "uri:ea.com:eala:asset");
        Node node = new(document.CreateNavigator()!.SelectSingleNode("/ea:WeaponTemplate", namespaces)!, namespaces);

        WeaponTemplate* root;
        using Tracker tracker = new((void**)&root, (uint)sizeof(WeaponTemplate), false);
        Marshaler.Marshal(node, root, tracker);
        Chunk chunk = new();
        tracker.MakeRelocatable(chunk);
        ReadOnlySpan<byte> instance = chunk.InstanceBuffer;
        Expect(instance.Length == 340, "WeaponTemplate instance bytes", 340, instance.Length);
        Expect(ReadUInt32(instance, 4) == 0x437A0000, "AttackRange=250", 0x437A0000, ReadUInt32(instance, 4));
        Expect(ReadUInt32(instance, 180) == 0x1000, "Flags=FORCE_KILL_GARRISONED_UNITS", 0x1000, ReadUInt32(instance, 180));
        Expect(ReadUInt32(instance, 216) == 0x802, "RequiredAntiMask", 0x802, ReadUInt32(instance, 216));
        Expect(ReadUInt32(instance, 228) == 2, "VirtualDamage=SHARE", 2, ReadUInt32(instance, 228));
        Expect(instance[301] == 1, "ScatterAlways", 1, instance[301]);
        Expect(instance[311] == 1, "ProjectileSelf", 1, instance[311]);
        Expect(instance[312] == 0, "ProjectileSelfUsesPathfinder", 0, instance[312]);
        Expect(instance[338] == 1, "UpdateBarrelModelConditions", 1, instance[338]);
        Expect(chunk.RelocationBuffer.Length == 0, "WeaponTemplate relocation bytes", 0, chunk.RelocationBuffer.Length);
        Expect(chunk.ImportsBuffer.Length == 0, "WeaponTemplate imports bytes", 0, chunk.ImportsBuffer.Length);
        Console.WriteLine(
            $"  WeaponTemplate bin={chunk.InstanceBuffer.Length}, " +
            $"relo={chunk.RelocationBuffer.Length}, imp={chunk.ImportsBuffer.Length}");
    }

    private static unsafe void TestGameObject()
    {
        const string xml = """
            <GameObject xmlns="uri:ea.com:eala:asset"
                CamouflageDetectorLevel="3" PathPriority="7"
                InvisibilityOpacityMin="0.25" InvisibilityOpacityMax="0.75"
                BuildInProximityToSamePlayerStucture="false">
              <CrusherInfo CrushAircraftWhileStationary="true"
                  DefaultCrushKillDelay="0.75s" CannotCrushTarget="true" />
              <ProjectedBuildabilityInfo Radius="12"
                  AllowedBuildabilityHeightVariation="75" PrimaryBuidability="false" />
            </GameObject>
            """;

        XmlDocument document = new();
        document.LoadXml(xml);
        XmlNamespaceManager namespaces = new(document.NameTable);
        namespaces.AddNamespace("ea", "uri:ea.com:eala:asset");
        Node node = new(document.CreateNavigator()!.SelectSingleNode("/ea:GameObject", namespaces)!, namespaces);

        GameObject* root;
        using Tracker tracker = new((void**)&root, (uint)sizeof(GameObject), false);
        Marshaler.Marshal(node, root, tracker);
        Chunk chunk = new();
        tracker.MakeRelocatable(chunk);
        ReadOnlySpan<byte> instance = chunk.InstanceBuffer;
        Expect(instance.Length == 768, "GameObject instance bytes", 768, instance.Length);
        Expect(ReadUInt32(instance, 296) == 3, "CamouflageDetectorLevel", 3, ReadUInt32(instance, 296));
        Expect(ReadUInt32(instance, 304) == 7, "PathPriority", 7, ReadUInt32(instance, 304));
        Expect(ReadUInt32(instance, 376) == 0x3E800000, "InvisibilityOpacityMin", 0x3E800000, ReadUInt32(instance, 376));
        Expect(ReadUInt32(instance, 380) == 0x3F400000, "InvisibilityOpacityMax", 0x3F400000, ReadUInt32(instance, 380));
        Expect(instance[589] == 1, "IsTrainable default", 1, instance[589]);
        Expect(instance[597] == 1, "CanPathThroughGates default", 1, instance[597]);
        Expect(instance[598] == 0, "BuildInProximityToSamePlayerStucture", 0, instance[598]);
        Expect(ReadUInt32(instance, 564) == 600, "CrusherInfo relocation", 600, ReadUInt32(instance, 564));
        Expect(ReadUInt32(instance, 568) == 1, "ProjectedBuildabilityInfo count", 1, ReadUInt32(instance, 568));
        Expect(ReadUInt32(instance, 572) == 652, "ProjectedBuildabilityInfo relocation", 652, ReadUInt32(instance, 572));
        Expect(instance[646] == 1, "CrushAircraftWhileStationary", 1, instance[646]);
        Expect(instance[650] == 1, "CannotCrushTarget", 1, instance[650]);
        Expect(ReadUInt32(instance, 652) == 0x41400000, "ProjectedBuildabilityInfo.Radius", 0x41400000, ReadUInt32(instance, 652));
        Expect(ReadUInt32(instance, 756) == 0x42960000, "AllowedBuildabilityHeightVariation", 0x42960000, ReadUInt32(instance, 756));
        Expect(instance[764] == 0, "PrimaryBuidability", 0, instance[764]);
        Expect(chunk.RelocationBuffer.Length == 12, "GameObject relocation bytes", 12, chunk.RelocationBuffer.Length);
        Expect(chunk.ImportsBuffer.Length == 0, "GameObject imports bytes", 0, chunk.ImportsBuffer.Length);
        Console.WriteLine(
            $"  GameObject bin={chunk.InstanceBuffer.Length}, " +
            $"relo={chunk.RelocationBuffer.Length}, imp={chunk.ImportsBuffer.Length}");
    }

    private static unsafe void TestSpawnedSlaveUpdate()
    {
        const string xml = """
            <SpawnedSlaveUpdate xmlns="uri:ea.com:eala:asset"
                LeashRange="300" AttackRange="250"
                DieOnMastersDeath="true"
                UseSlaverAsControlForEvaObjectSightedEvents="false" />
            """;

        XmlDocument document = new();
        document.LoadXml(xml);
        XmlNamespaceManager namespaces = new(document.NameTable);
        namespaces.AddNamespace("ea", "uri:ea.com:eala:asset");
        Node node = new(document.CreateNavigator()!.SelectSingleNode("/ea:SpawnedSlaveUpdate", namespaces)!, namespaces);

        SpawnedSlaveUpdateModuleData* root;
        using Tracker tracker = new((void**)&root, (uint)sizeof(SpawnedSlaveUpdateModuleData), false);
        Marshaler.Marshal(node, root, tracker);
        Chunk chunk = new();
        tracker.MakeRelocatable(chunk);
        ReadOnlySpan<byte> instance = chunk.InstanceBuffer;
        Expect(instance.Length == 112, "SpawnedSlaveUpdate instance bytes", 112, instance.Length);
        Expect(ReadUInt32(instance, 8) == 300, "SpawnedSlaveUpdate.LeashRange", 300, ReadUInt32(instance, 8));
        Expect(ReadUInt32(instance, 20) == 250, "SpawnedSlaveUpdate.AttackRange", 250, ReadUInt32(instance, 20));
        Expect(instance[109] == 1, "SpawnedSlaveUpdate.DieOnMastersDeath", 1, instance[109]);
        Expect(chunk.RelocationBuffer.Length == 0, "SpawnedSlaveUpdate relocation bytes", 0, chunk.RelocationBuffer.Length);
        Expect(chunk.ImportsBuffer.Length == 0, "SpawnedSlaveUpdate imports bytes", 0, chunk.ImportsBuffer.Length);
        Console.WriteLine($"  SpawnedSlaveUpdate bin={chunk.InstanceBuffer.Length}, relo=0, imp=0");
    }

    private static unsafe void TestUnitUnpackUpdate()
    {
        const string xml = """
            <UnitUnpackUpdate xmlns="uri:ea.com:eala:asset"
                UnpackTime="20s" OffsetHeightAboveWater="5.0" />
            """;

        XmlDocument document = new();
        document.LoadXml(xml);
        XmlNamespaceManager namespaces = new(document.NameTable);
        namespaces.AddNamespace("ea", "uri:ea.com:eala:asset");
        Node node = new(document.CreateNavigator()!.SelectSingleNode("/ea:UnitUnpackUpdate", namespaces)!, namespaces);

        UnitUnpackUpdateModuleData* root;
        using Tracker tracker = new((void**)&root, (uint)sizeof(UnitUnpackUpdateModuleData), false);
        Marshaler.Marshal(node, root, tracker);
        Chunk chunk = new();
        tracker.MakeRelocatable(chunk);
        ReadOnlySpan<byte> instance = chunk.InstanceBuffer;
        Expect(instance.Length == 20, "UnitUnpackUpdate instance bytes", 20, instance.Length);
        Expect(ReadUInt32(instance, 8) == 0x41A00000, "UnitUnpackUpdate.UnpackTime", 0x41A00000, ReadUInt32(instance, 8));
        Expect(ReadUInt32(instance, 16) == 0x40A00000, "UnitUnpackUpdate.OffsetHeightAboveWater", 0x40A00000, ReadUInt32(instance, 16));
        Expect(chunk.RelocationBuffer.Length == 0, "UnitUnpackUpdate relocation bytes", 0, chunk.RelocationBuffer.Length);
        Expect(chunk.ImportsBuffer.Length == 0, "UnitUnpackUpdate imports bytes", 0, chunk.ImportsBuffer.Length);
        Console.WriteLine($"  UnitUnpackUpdate bin={chunk.InstanceBuffer.Length}, relo=0, imp=0");
    }

    private static unsafe void TestAddObjectsToLiftUpdate()
    {
        const string xml = """
            <AddObjectsToLiftUpdateSpecialPower xmlns="uri:ea.com:eala:asset"
                Radius="15" LiftObjectLinkID="101" />
            """;

        XmlDocument document = new();
        document.LoadXml(xml);
        XmlNamespaceManager namespaces = new(document.NameTable);
        namespaces.AddNamespace("ea", "uri:ea.com:eala:asset");
        Node node = new(document.CreateNavigator()!.SelectSingleNode("/ea:AddObjectsToLiftUpdateSpecialPower", namespaces)!, namespaces);

        AddObjectsToLiftUpdateSpecialPowerModuleData* root;
        using Tracker tracker = new((void**)&root, (uint)sizeof(AddObjectsToLiftUpdateSpecialPowerModuleData), false);
        Marshaler.Marshal(node, root, tracker);
        Chunk chunk = new();
        tracker.MakeRelocatable(chunk);
        ReadOnlySpan<byte> instance = chunk.InstanceBuffer;
        Expect(instance.Length == 496, "AddObjectsToLiftUpdate instance bytes", 496, instance.Length);
        Expect(ReadUInt32(instance, 476) == 0x41700000, "AddObjectsToLiftUpdate.Radius", 0x41700000, ReadUInt32(instance, 476));
        Expect(ReadUInt32(instance, 492) == 101, "AddObjectsToLiftUpdate.LiftObjectLinkID", 101, ReadUInt32(instance, 492));
        Console.WriteLine($"  AddObjectsToLiftUpdate bin={chunk.InstanceBuffer.Length}, relo={chunk.RelocationBuffer.Length}, imp={chunk.ImportsBuffer.Length}");
    }

    private static unsafe void TestLureObjectsUpdate()
    {
        const string xml = """
            <LureObjectsUpdate xmlns="uri:ea.com:eala:asset"
                LureObjectLinkID="101" GuardAttackRange="175"
                GuardStatus="UNSELECTABLE"
                DisabledTypesToProcess="FROZEN">
              <GuardOffset x="60" y="0" z="0" />
              <GuardOffset x="45" y="-45" z="0" />
            </LureObjectsUpdate>
            """;

        XmlDocument document = new();
        document.LoadXml(xml);
        XmlNamespaceManager namespaces = new(document.NameTable);
        namespaces.AddNamespace("ea", "uri:ea.com:eala:asset");
        Node node = new(document.CreateNavigator()!.SelectSingleNode("/ea:LureObjectsUpdate", namespaces)!, namespaces);

        LureObjectsUpdateModuleData* root;
        using Tracker tracker = new((void**)&root, (uint)sizeof(LureObjectsUpdateModuleData), false);
        Marshaler.Marshal(node, root, tracker);
        Chunk chunk = new();
        tracker.MakeRelocatable(chunk);
        ReadOnlySpan<byte> instance = chunk.InstanceBuffer;
        Expect(instance.Length == 96, "LureObjectsUpdate instance bytes", 96, instance.Length);
        Expect(ReadUInt32(instance, 8) == 101, "LureObjectsUpdate.LureObjectLinkID", 101, ReadUInt32(instance, 8));
        Expect(ReadUInt32(instance, 12) == 0x49742400, "LureObjectsUpdate.GuardRadiusMaxSqr", 0x49742400, ReadUInt32(instance, 12));
        Expect(ReadUInt32(instance, 16) == 0x432F0000, "LureObjectsUpdate.GuardAttackRange", 0x432F0000, ReadUInt32(instance, 16));
        Expect(ReadUInt32(instance, 52) == 10, "LureObjectsUpdate.UpdateRate", 10, ReadUInt32(instance, 52));
        Expect(ReadUInt32(instance, 60) == 0x1000, "LureObjectsUpdate.DisabledTypesToProcess", 0x1000, ReadUInt32(instance, 60));
        Expect(ReadUInt32(instance, 64) == 2, "LureObjectsUpdate.GuardOffset count", 2, ReadUInt32(instance, 64));
        Expect(ReadUInt32(instance, 68) == 72, "LureObjectsUpdate.GuardOffset relocation", 72, ReadUInt32(instance, 68));
        Expect(ReadUInt32(instance, 72) == 0x42700000, "LureObjectsUpdate.GuardOffset[0].X", 0x42700000, ReadUInt32(instance, 72));
        Expect(ReadUInt32(instance, 88) == 0xC2340000, "LureObjectsUpdate.GuardOffset[1].Y", unchecked((int)0xC2340000), ReadUInt32(instance, 88));
        Expect(chunk.RelocationBuffer.Length == 8, "LureObjectsUpdate relocation bytes", 8, chunk.RelocationBuffer.Length);
        Console.WriteLine($"  LureObjectsUpdate bin={chunk.InstanceBuffer.Length}, relo={chunk.RelocationBuffer.Length}, imp={chunk.ImportsBuffer.Length}");
    }

    private static unsafe void TestFlingStoredObjectsSpecialPower()
    {
        const string xml = """
            <FlingStoredObjectsSpecialPower xmlns="uri:ea.com:eala:asset"
                SpecialPowerTemplate="SpecialPowerTemplate\123"
                CanAffectObjectFilter="ObjectFilterAsset\456"
                DisabledTypesToIgnore="FROZEN" ObjectFilterDistType="CIRCLE"
                AvailableAtStart="true" StoreObjectsLinkID="101" LiftObjectLinkID="102"
                MaximumVelocity="800" MinimumVelocity="700">
              <ObjectMap />
              <ObjectMap />
            </FlingStoredObjectsSpecialPower>
            """;

        XmlDocument document = new();
        document.LoadXml(xml);
        XmlNamespaceManager namespaces = new(document.NameTable);
        namespaces.AddNamespace("ea", "uri:ea.com:eala:asset");
        Node node = new(document.CreateNavigator()!.SelectSingleNode("/ea:FlingStoredObjectsSpecialPower", namespaces)!, namespaces);

        FlingStoredObjectsSpecialPowerModuleData* root;
        using Tracker tracker = new((void**)&root, (uint)sizeof(FlingStoredObjectsSpecialPowerModuleData), false);
        Marshaler.Marshal(node, root, tracker);
        Chunk chunk = new();
        tracker.MakeRelocatable(chunk);
        ReadOnlySpan<byte> instance = chunk.InstanceBuffer;
        Expect(instance.Length == 516, "FlingStoredObjectsSpecialPower instance bytes", 516, instance.Length);
        Expect(ReadUInt32(instance, 8) == 124, "Fling SpecialPowerTemplate import", 124, ReadUInt32(instance, 8)); // Reborn: final BIN imports are one-biased.
        Expect(ReadUInt32(instance, 84) == 0x1000, "Fling DisabledTypesToIgnore", 0x1000, ReadUInt32(instance, 84));
        Expect(ReadUInt32(instance, 88) == 457, "Fling CanAffectObjectFilter import", 457, ReadUInt32(instance, 88)); // Reborn: final BIN imports are one-biased.
        Expect(ReadUInt32(instance, 92) == 1, "Fling ObjectFilterDistType=CIRCLE", 1, ReadUInt32(instance, 92));
        Expect(instance[469] == 1, "Fling AvailableAtStart", 1, instance[469]);
        Expect(ReadUInt32(instance, 476) == 101, "Fling StoreObjectsLinkID", 101, ReadUInt32(instance, 476));
        Expect(ReadUInt32(instance, 480) == 102, "Fling LiftObjectLinkID", 102, ReadUInt32(instance, 480));
        Expect(ReadUInt32(instance, 484) == 0x44480000, "Fling MaximumVelocity", 0x44480000, ReadUInt32(instance, 484));
        Expect(ReadUInt32(instance, 488) == 0x442F0000, "Fling MinimumVelocity", 0x442F0000, ReadUInt32(instance, 488));
        Expect(ReadUInt32(instance, 492) == 2, "Fling ObjectMap count", 2, ReadUInt32(instance, 492));
        Expect(ReadUInt32(instance, 496) == 500, "Fling ObjectMap relocation", 500, ReadUInt32(instance, 496));
        Expect(chunk.RelocationBuffer.Length == 8, "Fling relocation bytes", 8, chunk.RelocationBuffer.Length);
        Expect(chunk.ImportsBuffer.Length == 12, "Fling imports bytes", 12, chunk.ImportsBuffer.Length);
        Console.WriteLine($"  FlingStoredObjectsSpecialPower bin={chunk.InstanceBuffer.Length}, relo={chunk.RelocationBuffer.Length}, imp={chunk.ImportsBuffer.Length}");
    }

    private static unsafe void TestLiftObjectUpdate()
    {
        const string xml = """
            <LiftObjectUpdate xmlns="uri:ea.com:eala:asset"
                LiftObjectLinkID="101" CrusherModifiesVelocity="true"
                LiftVelocity="4" MaxElevationFromGround="90"
                TimeIncrement="20s" MaxTimeLifted="20s"
                RotationSpeed="0.1" Shader="ShaderOverride\123"
                ShakeIntensity="0.002" ShakeRadius="-1" ShakeFade="1"
                DisabledTypesToProcess="FROZEN">
              <ModelStateObjectFilters>
                <LiftedUnitModelState ModelState="REACT_5">
                  <ObjectFilter Rule="ANY" Include="INFANTRY" />
                </LiftedUnitModelState>
              </ModelStateObjectFilters>
            </LiftObjectUpdate>
            """;

        XmlDocument document = new();
        document.LoadXml(xml);
        XmlNamespaceManager namespaces = new(document.NameTable);
        namespaces.AddNamespace("ea", "uri:ea.com:eala:asset");
        Node node = new(document.CreateNavigator()!.SelectSingleNode("/ea:LiftObjectUpdate", namespaces)!, namespaces);

        LiftObjectUpdateModuleData* root;
        using Tracker tracker = new((void**)&root, (uint)sizeof(LiftObjectUpdateModuleData), false);
        Marshaler.Marshal(node, root, tracker);
        Chunk chunk = new();
        tracker.MakeRelocatable(chunk);
        ReadOnlySpan<byte> instance = chunk.InstanceBuffer;
        Expect(instance.Length == 260, "LiftObjectUpdate instance bytes", 260, instance.Length);
        Expect(ReadUInt32(instance, 8) == 101, "LiftObjectUpdate.LiftObjectLinkID", 101, ReadUInt32(instance, 8));
        Expect(ReadUInt32(instance, 12) == 0x40800000, "LiftObjectUpdate.LiftVelocity", 0x40800000, ReadUInt32(instance, 12));
        Expect(ReadUInt32(instance, 16) == 0x42B40000, "LiftObjectUpdate.MaxElevationFromGround", 0x42B40000, ReadUInt32(instance, 16));
        Expect(ReadUInt32(instance, 40) == 0x3DCCCCCD, "LiftObjectUpdate.RotationSpeed", 0x3DCCCCCD, ReadUInt32(instance, 40));
        Expect(ReadUInt32(instance, 44) == 124, "LiftObjectUpdate.Shader import", 124, ReadUInt32(instance, 44)); // Reborn: final BIN imports are one-biased.
        Expect(ReadUInt32(instance, 48) == 0x3B03126F, "LiftObjectUpdate.ShakeIntensity", 0x3B03126F, ReadUInt32(instance, 48));
        Expect(ReadUInt32(instance, 52) == 0xBF800000, "LiftObjectUpdate.ShakeRadius", unchecked((int)0xBF800000), ReadUInt32(instance, 52));
        Expect(ReadUInt32(instance, 60) == 0x1000, "LiftObjectUpdate.DisabledTypesToProcess", 0x1000, ReadUInt32(instance, 60));
        Expect(ReadUInt32(instance, 64) == 1, "LiftObjectUpdate model-state count", 1, ReadUInt32(instance, 64));
        Expect(ReadUInt32(instance, 68) == 76, "LiftObjectUpdate model-state relocation", 76, ReadUInt32(instance, 68));
        Expect(instance[72] == 1, "LiftObjectUpdate.CrusherModifiesVelocity", 1, instance[72]);
        Expect(ReadUInt32(instance, 108) == 0x10000, "LiftObjectUpdate ModelState=REACT_5", 0x10000, ReadUInt32(instance, 108));
        Expect(ReadUInt32(instance, 136) == 140, "LiftObjectUpdate ObjectFilter relocation", 140, ReadUInt32(instance, 136));
        Expect(ReadUInt32(instance, 144) == 2, "LiftObjectUpdate ObjectFilter.Rule=ANY", 2, ReadUInt32(instance, 144));
        Expect(chunk.RelocationBuffer.Length == 12, "LiftObjectUpdate relocation bytes", 12, chunk.RelocationBuffer.Length);
        Expect(chunk.ImportsBuffer.Length == 8, "LiftObjectUpdate imports bytes", 8, chunk.ImportsBuffer.Length);
        Console.WriteLine($"  LiftObjectUpdate bin={chunk.InstanceBuffer.Length}, relo={chunk.RelocationBuffer.Length}, imp={chunk.ImportsBuffer.Length}");
    }

    private static unsafe void TestProjectileReplaceSelfSpecialAbility()
    {
        const string xml = """
            <ProjectileReplaceSelfSpecialAbility xmlns="uri:ea.com:eala:asset"
                SpecialPowerTemplate="SpecialPowerTemplate\68"
                StartAbilityRange="200" PackTime="3s"
                Options="RECONSTITUTE_STORED_COMMAND IGNORE_FACING_CHECK USE_OBJECT_GEOMETRY_FOR_WITHIN_RANGE_CHECK FAIL_WITH_INVALID_APPROACH"
                SetObjectStatusOnTrigger="IGNORE_AI_COMMAND"
                ClearObjectStatusOnExit="IGNORE_AI_COMMAND"
                MinimumUnpackTimeAfterSpecialPowerInitiation="1.25s"
                ClearTriggerDistance="225"
                ReplaceOptions="CHECK_BUILD_ASSISTANT DISABLE_DURING_REPLACE CLEAR_LOCATION REPLACE_OVER_ENEMIES TRANSFER_EXPERIENCE"
                LaunchingWeapon="WeaponTemplate\69"
                OtherObjectCreationList="ObjectCreationList\71" />
            """;

        XmlDocument document = new();
        document.LoadXml(xml);
        XmlNamespaceManager namespaces = new(document.NameTable);
        namespaces.AddNamespace("ea", "uri:ea.com:eala:asset");
        Node node = new(document.CreateNavigator()!.SelectSingleNode("/ea:ProjectileReplaceSelfSpecialAbility", namespaces)!, namespaces);

        ProjectileReplaceSelfSpecialAbilityModuleData* root;
        using Tracker tracker = new((void**)&root, (uint)sizeof(ProjectileReplaceSelfSpecialAbilityModuleData), false);
        Marshaler.Marshal(node, root, tracker);
        Chunk chunk = new();
        tracker.MakeRelocatable(chunk);
        ReadOnlySpan<byte> instance = chunk.InstanceBuffer;
        Expect(instance.Length == 284, "ProjectileReplaceSelf instance bytes", 284, instance.Length);
        Expect(ReadUInt32(instance, 8) == 69, "ProjectileReplaceSelf SpecialPowerTemplate import", 69, ReadUInt32(instance, 8)); // Reborn: final BIN imports are one-biased.
        Expect(ReadUInt32(instance, 12) == 0x43480000, "ProjectileReplaceSelf StartAbilityRange", 0x43480000, ReadUInt32(instance, 12));
        Expect(ReadUInt32(instance, 32) == 0x40400000, "ProjectileReplaceSelf PackTime", 0x40400000, ReadUInt32(instance, 32));
        Expect(ReadUInt32(instance, 44) == 0x44180000, "ProjectileReplaceSelf Options", 0x44180000, ReadUInt32(instance, 44));
        Expect(ReadUInt32(instance, 148) == 0x2000, "ProjectileReplaceSelf set status", 0x2000, ReadUInt32(instance, 148));
        Expect(ReadUInt32(instance, 180) == 0x2000, "ProjectileReplaceSelf clear status", 0x2000, ReadUInt32(instance, 180));
        Expect(ReadUInt32(instance, 228) == 0x8, "ProjectileReplaceSelf DisabledTypesToProcess", 0x8, ReadUInt32(instance, 228));
        Expect(ReadUInt32(instance, 240) == 0x3FA00000, "ProjectileReplaceSelf minimum unpack time", 0x3FA00000, ReadUInt32(instance, 240));
        Expect(instance[249] == 1, "ProjectileReplaceSelf GoIdleInStartPreparation", 1, instance[249]);
        Expect(instance[250] == 1, "ProjectileReplaceSelf FaceTarget", 1, instance[250]);
        Expect(ReadUInt32(instance, 256) == 0x1D8, "ProjectileReplaceSelf ReplaceOptions", 0x1D8, ReadUInt32(instance, 256));
        Expect(ReadUInt32(instance, 260) == 0x43610000, "ProjectileReplaceSelf ClearTriggerDistance", 0x43610000, ReadUInt32(instance, 260));
        Expect(ReadUInt32(instance, 264) == 0, "ProjectileReplaceSelf replacement count", 0, ReadUInt32(instance, 264));
        Expect(ReadUInt32(instance, 268) == 0, "ProjectileReplaceSelf omitted replacement pointer", 0, ReadUInt32(instance, 268));
        Expect(ReadUInt32(instance, 272) == 70, "ProjectileReplaceSelf LaunchingWeapon import", 70, ReadUInt32(instance, 272)); // Reborn: final BIN imports are one-biased.
        Expect(ReadUInt32(instance, 276) == 280, "ProjectileReplaceSelf OCL relocation", 280, ReadUInt32(instance, 276));
        Expect(ReadUInt32(instance, 280) == 72, "ProjectileReplaceSelf OCL import", 72, ReadUInt32(instance, 280)); // Reborn: final BIN imports are one-biased.
        Expect(chunk.RelocationBuffer.Length == 8, "ProjectileReplaceSelf relocation bytes", 8, chunk.RelocationBuffer.Length);
        Expect(chunk.ImportsBuffer.Length == 16, "ProjectileReplaceSelf imports bytes", 16, chunk.ImportsBuffer.Length);
        Console.WriteLine($"  ProjectileReplaceSelf bin={chunk.InstanceBuffer.Length}, relo={chunk.RelocationBuffer.Length}, imp={chunk.ImportsBuffer.Length}");
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: Validate the EP1 ProjectilePath node ABI against EA's five-node ProjectilePath_Foo fixture. */
    //-------------------------------------------------------------------------------------------------
    private static unsafe void TestProjectilePath()
    {
        const string xml = """
            <ProjectilePath xmlns="uri:ea.com:eala:asset">
              <Node>
                <InVec x="26.0649" y="-2.84037" z="6.802" />
                <Point x="29.0575" y="-4.72524" z="15.1092" />
                <OutVec x="32.0502" y="-6.61011" z="23.4164" />
              </Node>
              <Node>
                <InVec x="28.7771" y="-11.9631" z="40.4948" />
                <Point x="18.1657" y="-11.3092" z="49.0047" />
                <OutVec x="7.55436" y="-10.6554" z="57.5145" />
              </Node>
              <Node>
                <InVec x="-13.9382" y="-4.19995" z="65.2511" />
                <Point x="-34.6108" y="-0.802208" z="66.1682" />
                <OutVec x="-55.2834" y="2.59553" z="67.0853" />
              </Node>
              <Node>
                <InVec x="-79.2485" y="8.94352" z="62.1112" />
                <Point x="-105.87" y="9.07722" z="54.5073" />
                <OutVec x="-132.491" y="9.21093" z="46.9035" />
              </Node>
              <Node>
                <InVec x="-161.981" y="6.18518" z="35.5827" />
                <Point x="-194.34" y="0.0" z="20.5451" />
                <OutVec x="-227.34" y="-6.18518" z="5.5451" />
              </Node>
            </ProjectilePath>
            """;

        XmlDocument document = new();
        document.LoadXml(xml);
        XmlNamespaceManager namespaces = new(document.NameTable);
        namespaces.AddNamespace("ea", "uri:ea.com:eala:asset");
        Node node = new(document.CreateNavigator()!.SelectSingleNode("/ea:ProjectilePath", namespaces)!, namespaces);

        ProjectilePath* root;
        using Tracker tracker = new((void**)&root, (uint)sizeof(ProjectilePath), false);
        Marshaler.Marshal(node, root, tracker);
        Chunk chunk = new();
        if (!tracker.MakeRelocatable(chunk))
        {
            throw new InvalidDataException("Tracker failed to produce a relocatable ProjectilePath chunk.");
        }

        ReadOnlySpan<byte> instance = chunk.InstanceBuffer;
        Expect(instance.Length == 256, "ProjectilePath instance bytes", 256, instance.Length);
        Expect(ReadUInt32(instance, 4) == 0, "ProjectilePath omitted ComponentScale", 0, ReadUInt32(instance, 4));
        Expect(ReadUInt32(instance, 8) == 5, "ProjectilePath node count", 5, ReadUInt32(instance, 8));
        Expect(ReadUInt32(instance, 12) == 16, "ProjectilePath node relocation", 16, ReadUInt32(instance, 12));
        Expect(ReadUInt32(instance, 16) == 0x41D084EA, "ProjectilePath first InVec.x", 0x41D084EA, ReadUInt32(instance, 16));
        Expect(ReadUInt32(instance, 28) == 0, "ProjectilePath default InVec.w", 0, ReadUInt32(instance, 28));
        // Reborn: The final node's OutVec occupies the last 16 bytes of the 256-byte EA fixture.
        Expect(ReadUInt32(instance, 240) == 0xC363570A, "ProjectilePath final OutVec.x", unchecked((int)0xC363570A), ReadUInt32(instance, 240));
        Expect(ReadUInt32(instance, 244) == 0xC0C5ECFF, "ProjectilePath final OutVec.y", unchecked((int)0xC0C5ECFF), ReadUInt32(instance, 244));
        Expect(ReadUInt32(instance, 248) == 0x40B17176, "ProjectilePath final OutVec.z", 0x40B17176, ReadUInt32(instance, 248));
        Expect(chunk.RelocationBuffer.Length == 8, "ProjectilePath relocation bytes", 8, chunk.RelocationBuffer.Length);
        Expect(chunk.ImportsBuffer.Length == 0, "ProjectilePath imports bytes", 0, chunk.ImportsBuffer.Length);
        Console.WriteLine($"  ProjectilePath bin={chunk.InstanceBuffer.Length}, relo={chunk.RelocationBuffer.Length}, imp={chunk.ImportsBuffer.Length}");
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: Validate the EP1 YurikoHotKeys root, list stride, references, and modifier flags. */
    //-------------------------------------------------------------------------------------------------
    private static unsafe void TestYurikoHotKeys()
    {
        const string xml = """
            <YurikoHotKeys xmlns="uri:ea.com:eala:asset">
              <Map>
                <HotKey Slot="HotKeySlot\1" Key="MappableKey\2" Modifiers="CTRL" />
                <HotKey Slot="HotKeySlot\3" Key="MappableKey\4" Modifiers="SHIFT" />
              </Map>
            </YurikoHotKeys>
            """;

        XmlDocument document = new();
        document.LoadXml(xml);
        XmlNamespaceManager namespaces = new(document.NameTable);
        namespaces.AddNamespace("ea", "uri:ea.com:eala:asset");
        Node node = new(document.CreateNavigator()!.SelectSingleNode("/ea:YurikoHotKeys", namespaces)!, namespaces);

        YurikoHotKeys* root;
        using Tracker tracker = new((void**)&root, (uint)sizeof(YurikoHotKeys), false);
        Marshaler.Marshal(node, root, tracker);
        Chunk chunk = new();
        if (!tracker.MakeRelocatable(chunk))
        {
            throw new InvalidDataException("Tracker failed to produce a relocatable YurikoHotKeys chunk.");
        }

        ReadOnlySpan<byte> instance = chunk.InstanceBuffer;
        Expect(instance.Length == 36, "YurikoHotKeys instance bytes", 36, instance.Length);
        Expect(ReadUInt32(instance, 4) == 2, "YurikoHotKeys entry count", 2, ReadUInt32(instance, 4));
        Expect(ReadUInt32(instance, 8) == 12, "YurikoHotKeys entry relocation", 12, ReadUInt32(instance, 8));
        Expect(ReadUInt32(instance, 12) == 2, "YurikoHotKeys first slot import", 2, ReadUInt32(instance, 12)); // Reborn: final BIN imports are one-biased.
        Expect(ReadUInt32(instance, 16) == 3, "YurikoHotKeys first key import", 3, ReadUInt32(instance, 16)); // Reborn: final BIN imports are one-biased.
        Expect(ReadUInt32(instance, 20) == 1, "YurikoHotKeys CTRL modifier", 1, ReadUInt32(instance, 20));
        Expect(ReadUInt32(instance, 32) == 4, "YurikoHotKeys SHIFT modifier", 4, ReadUInt32(instance, 32));
        Expect(chunk.RelocationBuffer.Length == 8, "YurikoHotKeys relocation bytes", 8, chunk.RelocationBuffer.Length);
        // Reborn: Four import-source offsets plus the stream terminator occupy 20 bytes.
        Expect(chunk.ImportsBuffer.Length == 20, "YurikoHotKeys imports bytes", 20, chunk.ImportsBuffer.Length);
        Console.WriteLine($"  YurikoHotKeys bin={chunk.InstanceBuffer.Length}, relo={chunk.RelocationBuffer.Length}, imp={chunk.ImportsBuffer.Length}");
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: Reproduce the EP1 Settings_Dynamics payload and all schema defaults. */
    //-------------------------------------------------------------------------------------------------
    private static unsafe void TestDynamicsSettings()
    {
        const string xml = """
            <DynamicsSettings xmlns="uri:ea.com:eala:asset" MaximumContacts="2048" />
            """;

        XmlDocument document = new();
        document.LoadXml(xml);
        XmlNamespaceManager namespaces = new(document.NameTable);
        namespaces.AddNamespace("ea", "uri:ea.com:eala:asset");
        Node node = new(document.CreateNavigator()!.SelectSingleNode("/ea:DynamicsSettings", namespaces)!, namespaces);

        DynamicsSettings* root;
        using Tracker tracker = new((void**)&root, (uint)sizeof(DynamicsSettings), false);
        Marshaler.Marshal(node, root, tracker);
        Chunk chunk = new();
        tracker.MakeRelocatable(chunk);
        ReadOnlySpan<byte> instance = chunk.InstanceBuffer;
        Expect(instance.Length == 24, "DynamicsSettings instance bytes", 24, instance.Length);
        Expect(ReadUInt32(instance, 4) == 1024, "DynamicsSettings.MaximumObjects", 1024, ReadUInt32(instance, 4));
        Expect(ReadUInt32(instance, 8) == 2048, "DynamicsSettings.MaximumContacts", 2048, ReadUInt32(instance, 8));
        Expect(ReadUInt32(instance, 12) == 1024, "DynamicsSettings.MaximumContactPairs", 1024, ReadUInt32(instance, 12));
        Expect(ReadUInt32(instance, 16) == 256, "DynamicsSettings.MaximumJoints", 256, ReadUInt32(instance, 16));
        Expect(instance[20] == 0, "DynamicsSettings.CreateGlobalIsland", 0, instance[20]);
        Expect(chunk.RelocationBuffer.Length == 0, "DynamicsSettings relocation bytes", 0, chunk.RelocationBuffer.Length);
        Expect(chunk.ImportsBuffer.Length == 0, "DynamicsSettings imports bytes", 0, chunk.ImportsBuffer.Length);
        Console.WriteLine($"  DynamicsSettings bin={chunk.InstanceBuffer.Length}, relo={chunk.RelocationBuffer.Length}, imp={chunk.ImportsBuffer.Length}");
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: Validate both EP1 main-menu personality roots against their real static-stream layouts. */
    //-------------------------------------------------------------------------------------------------
    private static unsafe void TestMainMenuPersonality()
    {
        const string templateXml = """
            <MainMenuPersonalityTemplate xmlns="uri:ea.com:eala:asset">
              <MainMenuPersonalityImage>mainMenuPersonality_CryoTrooper</MainMenuPersonalityImage>
              <MainMenuPersonalityMusic>MenuTrackEP1_Cryo</MainMenuPersonalityMusic>
            </MainMenuPersonalityTemplate>
            """;
        XmlDocument templateDocument = new();
        templateDocument.LoadXml(templateXml);
        XmlNamespaceManager templateNamespaces = new(templateDocument.NameTable);
        templateNamespaces.AddNamespace("ea", "uri:ea.com:eala:asset");
        Node templateNode = new(templateDocument.CreateNavigator()!.SelectSingleNode("/ea:MainMenuPersonalityTemplate", templateNamespaces)!, templateNamespaces);

        MainMenuPersonalityTemplate* templateRoot;
        using Tracker templateTracker = new((void**)&templateRoot, (uint)sizeof(MainMenuPersonalityTemplate), false);
        Marshaler.Marshal(templateNode, templateRoot, templateTracker);
        Chunk templateChunk = new();
        templateTracker.MakeRelocatable(templateChunk);
        ReadOnlySpan<byte> templateInstance = templateChunk.InstanceBuffer;
        Expect(templateInstance.Length == 36, "MainMenuPersonalityTemplate instance bytes", 36, templateInstance.Length);
        Expect(ReadUInt32(templateInstance, 4) == 0xBDF87A1B, "MainMenuPersonalityTemplate image hash", unchecked((int)0xBDF87A1B), ReadUInt32(templateInstance, 4));
        Expect(ReadUInt32(templateInstance, 8) == 17, "MainMenuPersonalityTemplate music length", 17, ReadUInt32(templateInstance, 8));
        Expect(ReadUInt32(templateInstance, 12) == 16, "MainMenuPersonalityTemplate music relocation", 16, ReadUInt32(templateInstance, 12));
        Expect(ReadUInt32(templateInstance, 16) == 0x756E654D, "MainMenuPersonalityTemplate music bytes", 0x756E654D, ReadUInt32(templateInstance, 16));
        Expect(templateChunk.RelocationBuffer.Length == 8, "MainMenuPersonalityTemplate relocation bytes", 8, templateChunk.RelocationBuffer.Length);
        Expect(templateChunk.ImportsBuffer.Length == 0, "MainMenuPersonalityTemplate imports bytes", 0, templateChunk.ImportsBuffer.Length);

        const string groupXml = """
            <MainMenuPersonalityGroup xmlns="uri:ea.com:eala:asset" DefaultPersonality="MainMenuPersonalityTemplate\11">
              <MainMenuPersonality>MainMenuPersonalityTemplate\12</MainMenuPersonality>
              <MainMenuPersonality>MainMenuPersonalityTemplate\13</MainMenuPersonality>
            </MainMenuPersonalityGroup>
            """;
        XmlDocument groupDocument = new();
        groupDocument.LoadXml(groupXml);
        XmlNamespaceManager groupNamespaces = new(groupDocument.NameTable);
        groupNamespaces.AddNamespace("ea", "uri:ea.com:eala:asset");
        Node groupNode = new(groupDocument.CreateNavigator()!.SelectSingleNode("/ea:MainMenuPersonalityGroup", groupNamespaces)!, groupNamespaces);

        MainMenuPersonalityGroup* groupRoot;
        using Tracker groupTracker = new((void**)&groupRoot, (uint)sizeof(MainMenuPersonalityGroup), false);
        Marshaler.Marshal(groupNode, groupRoot, groupTracker);
        Chunk groupChunk = new();
        groupTracker.MakeRelocatable(groupChunk);
        ReadOnlySpan<byte> groupInstance = groupChunk.InstanceBuffer;
        Expect(groupInstance.Length == 24, "MainMenuPersonalityGroup instance bytes", 24, groupInstance.Length);
        Expect(ReadUInt32(groupInstance, 4) == 12, "MainMenuPersonalityGroup default import", 12, ReadUInt32(groupInstance, 4)); // Reborn: final BIN imports are one-biased.
        Expect(ReadUInt32(groupInstance, 8) == 2, "MainMenuPersonalityGroup entry count", 2, ReadUInt32(groupInstance, 8));
        Expect(ReadUInt32(groupInstance, 12) == 16, "MainMenuPersonalityGroup entry relocation", 16, ReadUInt32(groupInstance, 12));
        Expect(ReadUInt32(groupInstance, 16) == 13, "MainMenuPersonalityGroup first import", 13, ReadUInt32(groupInstance, 16)); // Reborn: final BIN imports are one-biased.
        Expect(ReadUInt32(groupInstance, 20) == 14, "MainMenuPersonalityGroup second import", 14, ReadUInt32(groupInstance, 20)); // Reborn: final BIN imports are one-biased.
        Expect(groupChunk.RelocationBuffer.Length == 8, "MainMenuPersonalityGroup relocation bytes", 8, groupChunk.RelocationBuffer.Length);
        Expect(groupChunk.ImportsBuffer.Length == 16, "MainMenuPersonalityGroup imports bytes", 16, groupChunk.ImportsBuffer.Length);
        Console.WriteLine($"  MainMenuPersonality template={templateChunk.InstanceBuffer.Length}/{templateChunk.RelocationBuffer.Length}/{templateChunk.ImportsBuffer.Length}, group={groupChunk.InstanceBuffer.Length}/{groupChunk.RelocationBuffer.Length}/{groupChunk.ImportsBuffer.Length}");
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: Verify EP1's overridable audio roots retain their base layouts and list/import streams. */
    //-------------------------------------------------------------------------------------------------
    private static unsafe void TestOverridableAudio()
    {
        const string audioXml = """
            <AudioEventOverridable xmlns="uri:ea.com:eala:asset">
              <Sound Weight="750">AudioFile\23</Sound>
            </AudioEventOverridable>
            """;
        XmlDocument audioDocument = new();
        audioDocument.LoadXml(audioXml);
        XmlNamespaceManager audioNamespaces = new(audioDocument.NameTable);
        audioNamespaces.AddNamespace("ea", "uri:ea.com:eala:asset");
        Node audioNode = new(audioDocument.CreateNavigator()!.SelectSingleNode("/ea:AudioEventOverridable", audioNamespaces)!, audioNamespaces);

        AudioEventOverridable* audioRoot;
        using Tracker audioTracker = new((void**)&audioRoot, (uint)sizeof(AudioEventOverridable), false);
        Marshaler.Marshal(audioNode, audioRoot, audioTracker);
        Chunk audioChunk = new();
        audioTracker.MakeRelocatable(audioChunk);
        ReadOnlySpan<byte> audioInstance = audioChunk.InstanceBuffer;
        Expect(audioInstance.Length == 128, "AudioEventOverridable instance bytes", 128, audioInstance.Length);
        Expect(ReadUInt32(audioInstance, 104) == 1, "AudioEventOverridable sound count", 1, ReadUInt32(audioInstance, 104));
        Expect(ReadUInt32(audioInstance, 108) == 120, "AudioEventOverridable sound relocation", 120, ReadUInt32(audioInstance, 108));
        Expect(ReadUInt32(audioInstance, 120) == 24, "AudioEventOverridable sound import", 24, ReadUInt32(audioInstance, 120)); // Reborn: final BIN imports are one-biased.
        Expect(ReadUInt32(audioInstance, 124) == 750, "AudioEventOverridable sound weight", 750, ReadUInt32(audioInstance, 124));
        Expect(audioChunk.RelocationBuffer.Length == 8, "AudioEventOverridable relocation bytes", 8, audioChunk.RelocationBuffer.Length);
        Expect(audioChunk.ImportsBuffer.Length == 8, "AudioEventOverridable imports bytes", 8, audioChunk.ImportsBuffer.Length);

        const string multisoundXml = """
            <MultisoundOverridable xmlns="uri:ea.com:eala:asset" Control="PLAY_ONE">
              <Subsound Weight="500">AudioEvent\31</Subsound>
            </MultisoundOverridable>
            """;
        XmlDocument multisoundDocument = new();
        multisoundDocument.LoadXml(multisoundXml);
        XmlNamespaceManager multisoundNamespaces = new(multisoundDocument.NameTable);
        multisoundNamespaces.AddNamespace("ea", "uri:ea.com:eala:asset");
        Node multisoundNode = new(multisoundDocument.CreateNavigator()!.SelectSingleNode("/ea:MultisoundOverridable", multisoundNamespaces)!, multisoundNamespaces);

        MultisoundOverridable* multisoundRoot;
        using Tracker multisoundTracker = new((void**)&multisoundRoot, (uint)sizeof(MultisoundOverridable), false);
        Marshaler.Marshal(multisoundNode, multisoundRoot, multisoundTracker);
        Chunk multisoundChunk = new();
        multisoundTracker.MakeRelocatable(multisoundChunk);
        ReadOnlySpan<byte> multisoundInstance = multisoundChunk.InstanceBuffer;
        Expect(multisoundInstance.Length == 24, "MultisoundOverridable instance bytes", 24, multisoundInstance.Length);
        Expect(ReadUInt32(multisoundInstance, 4) == 2, "MultisoundOverridable control", 2, ReadUInt32(multisoundInstance, 4));
        Expect(ReadUInt32(multisoundInstance, 8) == 1, "MultisoundOverridable subsound count", 1, ReadUInt32(multisoundInstance, 8));
        Expect(ReadUInt32(multisoundInstance, 12) == 16, "MultisoundOverridable subsound relocation", 16, ReadUInt32(multisoundInstance, 12));
        Expect(ReadUInt32(multisoundInstance, 16) == 32, "MultisoundOverridable subsound import", 32, ReadUInt32(multisoundInstance, 16)); // Reborn: final BIN imports are one-biased.
        Expect(ReadUInt32(multisoundInstance, 20) == 500, "MultisoundOverridable subsound weight", 500, ReadUInt32(multisoundInstance, 20));
        Expect(multisoundChunk.RelocationBuffer.Length == 8, "MultisoundOverridable relocation bytes", 8, multisoundChunk.RelocationBuffer.Length);
        Expect(multisoundChunk.ImportsBuffer.Length == 8, "MultisoundOverridable imports bytes", 8, multisoundChunk.ImportsBuffer.Length);
        Console.WriteLine($"  OverridableAudio event={audioChunk.InstanceBuffer.Length}/{audioChunk.RelocationBuffer.Length}/{audioChunk.ImportsBuffer.Length}, multisound={multisoundChunk.InstanceBuffer.Length}/{multisoundChunk.RelocationBuffer.Length}/{multisoundChunk.ImportsBuffer.Length}");
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: Exercise every EP1 movie-archive subtype through the corrected three-list root. */
    //-------------------------------------------------------------------------------------------------
    private static unsafe void TestMovieArchive()
    {
        const string xml = """
            <UIComponentMovieArchive xmlns="uri:ea.com:eala:asset" Priority="500">
              <GeneralMovie Movie="GEN" PreviewImage="OnDemandTextureImage\11" DisplayName="NAME" Description="DESC" Icon="ICON" />
              <ScenarioMovie Movie="SCN" PreviewImage="OnDemandTextureImage\12" DisplayName="NAME" Description="DESC" Icon="ICON" UnlockRequirement="CriticalPath" />
              <CampaignMovie Movie="CMP" PreviewImage="OnDemandTextureImage\13" DisplayName="NAME" Description="DESC" Icon="ICON" Faction="Yuriko" ProgressLock="3" />
            </UIComponentMovieArchive>
            """;
        XmlDocument document = new();
        document.LoadXml(xml);
        XmlNamespaceManager namespaces = new(document.NameTable);
        namespaces.AddNamespace("ea", "uri:ea.com:eala:asset");
        Node node = new(document.CreateNavigator()!.SelectSingleNode("/ea:UIComponentMovieArchive", namespaces)!, namespaces);

        UIComponentMovieArchive* root;
        using Tracker tracker = new((void**)&root, (uint)sizeof(UIComponentMovieArchive), false);
        Marshaler.Marshal(node, root, tracker);
        Chunk chunk = new();
        tracker.MakeRelocatable(chunk);
        ReadOnlySpan<byte> instance = chunk.InstanceBuffer;
        Expect(ReadUInt32(instance, 4) == 500, "UIComponentMovieArchive priority", 500, ReadUInt32(instance, 4));
        Expect(ReadUInt32(instance, 8) == 1, "UIComponentMovieArchive general count", 1, ReadUInt32(instance, 8));
        Expect(ReadUInt32(instance, 16) == 1, "UIComponentMovieArchive scenario count", 1, ReadUInt32(instance, 16));
        Expect(ReadUInt32(instance, 24) == 1, "UIComponentMovieArchive campaign count", 1, ReadUInt32(instance, 24));
        uint generalOffset = ReadUInt32(instance, 12);
        uint scenarioOffset = ReadUInt32(instance, 20);
        uint campaignOffset = ReadUInt32(instance, 28);
        Expect(ReadUInt32(instance, checked((int)generalOffset + 8)) == 12, "GeneralArchiveMovie preview import", 12, ReadUInt32(instance, checked((int)generalOffset + 8))); // Reborn: final BIN imports are one-biased.
        Expect(ReadUInt32(instance, checked((int)scenarioOffset + 8)) == 13, "ScenarioArchiveMovie preview import", 13, ReadUInt32(instance, checked((int)scenarioOffset + 8))); // Reborn: final BIN imports are one-biased.
        Expect(ReadUInt32(instance, checked((int)scenarioOffset + 36)) == 1, "ScenarioArchiveMovie unlock", 1, ReadUInt32(instance, checked((int)scenarioOffset + 36)));
        Expect(ReadUInt32(instance, checked((int)campaignOffset + 8)) == 14, "CampaignArchiveMovie preview import", 14, ReadUInt32(instance, checked((int)campaignOffset + 8))); // Reborn: final BIN imports are one-biased.
        Expect(ReadUInt32(instance, checked((int)campaignOffset + 36)) == 3, "CampaignArchiveMovie faction", 3, ReadUInt32(instance, checked((int)campaignOffset + 36)));
        Expect(ReadUInt32(instance, checked((int)campaignOffset + 40)) == 3, "CampaignArchiveMovie progress lock", 3, ReadUInt32(instance, checked((int)campaignOffset + 40)));
        Expect(chunk.RelocationBuffer.Length == 64, "UIComponentMovieArchive relocation bytes", 64, chunk.RelocationBuffer.Length);
        Expect(chunk.ImportsBuffer.Length == 16, "UIComponentMovieArchive imports bytes", 16, chunk.ImportsBuffer.Length);
        Console.WriteLine($"  MovieArchive bin={chunk.InstanceBuffer.Length}, relo={chunk.RelocationBuffer.Length}, imp={chunk.ImportsBuffer.Length}");
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: Reproduce the official EP1 scenario preview and fieldless scenario component XML. */
    //-------------------------------------------------------------------------------------------------
    private static unsafe void TestScenarioUi()
    {
        const string previewXml = """
            <UIScenarioMapPreview xmlns="uri:ea.com:eala:asset">
              <FactionSettings Faction="Allies" PlayerImage="PackedTextureImage\21" />
              <FactionSettings Faction="Soviet" PlayerImage="PackedTextureImage\22" />
              <FactionSettings Faction="Japan" PlayerImage="PackedTextureImage\23" />
            </UIScenarioMapPreview>
            """;
        XmlDocument previewDocument = new();
        previewDocument.LoadXml(previewXml);
        XmlNamespaceManager previewNamespaces = new(previewDocument.NameTable);
        previewNamespaces.AddNamespace("ea", "uri:ea.com:eala:asset");
        Node previewNode = new(previewDocument.CreateNavigator()!.SelectSingleNode("/ea:UIScenarioMapPreview", previewNamespaces)!, previewNamespaces);

        UIScenarioMapPreview* previewRoot;
        using Tracker previewTracker = new((void**)&previewRoot, (uint)sizeof(UIScenarioMapPreview), false);
        Marshaler.Marshal(previewNode, previewRoot, previewTracker);
        Chunk previewChunk = new();
        previewTracker.MakeRelocatable(previewChunk);
        ReadOnlySpan<byte> previewInstance = previewChunk.InstanceBuffer;
        Expect(previewInstance.Length == 36, "UIScenarioMapPreview instance bytes", 36, previewInstance.Length);
        Expect(ReadUInt32(previewInstance, 4) == 3, "UIScenarioMapPreview faction count", 3, ReadUInt32(previewInstance, 4));
        Expect(ReadUInt32(previewInstance, 8) == 12, "UIScenarioMapPreview faction relocation", 12, ReadUInt32(previewInstance, 8));
        Expect(ReadUInt32(previewInstance, 12) == 0, "UIScenarioMapPreview Allies value", 0, ReadUInt32(previewInstance, 12));
        Expect(ReadUInt32(previewInstance, 16) == 22, "UIScenarioMapPreview Allies image", 22, ReadUInt32(previewInstance, 16)); // Reborn: image references use one-biased final imports.
        Expect(ReadUInt32(previewInstance, 20) == 1, "UIScenarioMapPreview Soviet value", 1, ReadUInt32(previewInstance, 20));
        Expect(ReadUInt32(previewInstance, 24) == 23, "UIScenarioMapPreview Soviet image", 23, ReadUInt32(previewInstance, 24)); // Reborn: image references use one-biased final imports.
        Expect(ReadUInt32(previewInstance, 28) == 2, "UIScenarioMapPreview Japan value", 2, ReadUInt32(previewInstance, 28));
        Expect(ReadUInt32(previewInstance, 32) == 24, "UIScenarioMapPreview Japan image", 24, ReadUInt32(previewInstance, 32)); // Reborn: image references use one-biased final imports.
        Expect(previewChunk.RelocationBuffer.Length == 8, "UIScenarioMapPreview relocation bytes", 8, previewChunk.RelocationBuffer.Length);
        Expect(previewChunk.ImportsBuffer.Length == 16, "UIScenarioMapPreview imports bytes", 16, previewChunk.ImportsBuffer.Length);

        const string componentXml = """
            <UIComponentScenario xmlns="uri:ea.com:eala:asset" Priority="598" />
            """;
        XmlDocument componentDocument = new();
        componentDocument.LoadXml(componentXml);
        XmlNamespaceManager componentNamespaces = new(componentDocument.NameTable);
        componentNamespaces.AddNamespace("ea", "uri:ea.com:eala:asset");
        Node componentNode = new(componentDocument.CreateNavigator()!.SelectSingleNode("/ea:UIComponentScenario", componentNamespaces)!, componentNamespaces);

        UIComponentScenario* componentRoot;
        using Tracker componentTracker = new((void**)&componentRoot, (uint)sizeof(UIComponentScenario), false);
        Marshaler.Marshal(componentNode, componentRoot, componentTracker);
        Chunk componentChunk = new();
        componentTracker.MakeRelocatable(componentChunk);
        ReadOnlySpan<byte> componentInstance = componentChunk.InstanceBuffer;
        Expect(componentInstance.Length == 8, "UIComponentScenario instance bytes", 8, componentInstance.Length);
        Expect(ReadUInt32(componentInstance, 4) == 598, "UIComponentScenario priority", 598, ReadUInt32(componentInstance, 4));
        Expect(componentChunk.RelocationBuffer.Length == 0, "UIComponentScenario relocation bytes", 0, componentChunk.RelocationBuffer.Length);
        Expect(componentChunk.ImportsBuffer.Length == 0, "UIComponentScenario imports bytes", 0, componentChunk.ImportsBuffer.Length);
        Console.WriteLine($"  ScenarioUI preview={previewChunk.InstanceBuffer.Length}/{previewChunk.RelocationBuffer.Length}/{previewChunk.ImportsBuffer.Length}, component={componentChunk.InstanceBuffer.Length}/{componentChunk.RelocationBuffer.Length}/{componentChunk.ImportsBuffer.Length}");
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: Validate the EP1 scenario-manager hierarchy against the recovered native root sizes. */
    //-------------------------------------------------------------------------------------------------
    private static unsafe void TestScenarioManager()
    {
        const string xml = """
            <ScenarioManagerData xmlns="uri:ea.com:eala:asset" IntroMovie="I" CriticalPathCompletionMovie="C" FullCompletionMovie="F">
              <ScenarioTemplate id="Scenario_Test" UiName="U" Title="T" ShortTitle="S" Description="D" MapName="M"
                  IsStartingScenario="true" IsCriticalPath="true" UseRandomCrates="true"
                  PlayerStartPosition="1" PlayerTeam="2" PlayerColor="ColorBlue" ParTime="5s"
                  ParTimeScoreBonus="100" DifficultyScoreBonus="200" MaxEfficiencyScoreMultiplier="4"
                  UnitUnlock="TestUnit">
                <Enemy Faction="PlayerTemplate\11" Personality="AIPersonalityDefinition\12"
                    Portrait="PackedTextureImage\13" Difficulty="HARD" Team="3" StartPosition="4" Color="ColorRed" />
                <ScenarioUnlock>Scenario_Next</ScenarioUnlock>
              </ScenarioTemplate>
              <UnlockableUnit Faction="PlayerTemplate\14" Name="UnlockedUnit" />
            </ScenarioManagerData>
            """;
        XmlDocument document = new();
        document.LoadXml(xml);
        XmlNamespaceManager namespaces = new(document.NameTable);
        namespaces.AddNamespace("ea", "uri:ea.com:eala:asset");
        Node node = new(document.CreateNavigator()!.SelectSingleNode("/ea:ScenarioManagerData", namespaces)!, namespaces);

        ScenarioManagerData* root;
        using Tracker tracker = new((void**)&root, (uint)sizeof(ScenarioManagerData), false);
        Marshaler.Marshal(node, root, tracker);
        Chunk chunk = new();
        tracker.MakeRelocatable(chunk);
        ReadOnlySpan<byte> instance = chunk.InstanceBuffer;
        Expect(instance.Length == 216, "ScenarioManagerData instance bytes", 216, instance.Length);
        Expect(ReadUInt32(instance, 4) == 1, "ScenarioManagerData intro length", 1, ReadUInt32(instance, 4));
        Expect(ReadUInt32(instance, 28) == 50000, "ScenarioManagerData max deposit", 50000, ReadUInt32(instance, 28));
        Expect(ReadUInt32(instance, 32) == 1, "ScenarioManagerData scenario count", 1, ReadUInt32(instance, 32));
        Expect(ReadUInt32(instance, 40) == 1, "ScenarioManagerData unlockable count", 1, ReadUInt32(instance, 40));
        uint scenarioOffset = ReadUInt32(instance, 36);
        uint unlockableOffset = ReadUInt32(instance, 44);
        Expect(ReadUInt32(instance, checked((int)scenarioOffset + 44)) == 1, "ScenarioTemplate player start", 1, ReadUInt32(instance, checked((int)scenarioOffset + 44)));
        Expect(ReadUInt32(instance, checked((int)scenarioOffset + 48)) == 2, "ScenarioTemplate player team", 2, ReadUInt32(instance, checked((int)scenarioOffset + 48)));
        Expect(ReadUInt32(instance, checked((int)scenarioOffset + 56)) == 0x40A00000, "ScenarioTemplate par time", 0x40A00000, ReadUInt32(instance, checked((int)scenarioOffset + 56)));
        Expect(ReadUInt32(instance, checked((int)scenarioOffset + 60)) == 100, "ScenarioTemplate par bonus", 100, ReadUInt32(instance, checked((int)scenarioOffset + 60)));
        Expect(ReadUInt32(instance, checked((int)scenarioOffset + 64)) == 200, "ScenarioTemplate difficulty bonus", 200, ReadUInt32(instance, checked((int)scenarioOffset + 64)));
        Expect(ReadUInt32(instance, checked((int)scenarioOffset + 68)) == 0x40800000, "ScenarioTemplate efficiency multiplier", 0x40800000, ReadUInt32(instance, checked((int)scenarioOffset + 68)));
        Expect(instance[checked((int)scenarioOffset + 92)] == 1, "ScenarioTemplate starting flag", 1, instance[checked((int)scenarioOffset + 92)]);
        Expect(instance[checked((int)scenarioOffset + 93)] == 1, "ScenarioTemplate critical flag", 1, instance[checked((int)scenarioOffset + 93)]);
        Expect(instance[checked((int)scenarioOffset + 94)] == 1, "ScenarioTemplate crates flag", 1, instance[checked((int)scenarioOffset + 94)]);
        uint enemyOffset = ReadUInt32(instance, checked((int)scenarioOffset + 80));
        Expect(ReadUInt32(instance, checked((int)enemyOffset)) == 12, "ScenarioEnemy faction import", 12, ReadUInt32(instance, checked((int)enemyOffset))); // Reborn: final BIN imports are one-biased.
        Expect(ReadUInt32(instance, checked((int)enemyOffset + 4)) == 13, "ScenarioEnemy personality import", 13, ReadUInt32(instance, checked((int)enemyOffset + 4))); // Reborn: final BIN imports are one-biased.
        Expect(ReadUInt32(instance, checked((int)enemyOffset + 8)) == 14, "ScenarioEnemy portrait import", 14, ReadUInt32(instance, checked((int)enemyOffset + 8))); // Reborn: final BIN imports are one-biased.
        Expect(ReadUInt32(instance, checked((int)enemyOffset + 12)) == 2, "ScenarioEnemy difficulty", 2, ReadUInt32(instance, checked((int)enemyOffset + 12)));
        Expect(ReadUInt32(instance, checked((int)unlockableOffset)) == 15, "UnlockableUnit faction import", 15, ReadUInt32(instance, checked((int)unlockableOffset))); // Reborn: final imports reserve zero for null.
        Expect(chunk.RelocationBuffer.Length == 52, "ScenarioManagerData relocation bytes", 52, chunk.RelocationBuffer.Length);
        Expect(chunk.ImportsBuffer.Length == 20, "ScenarioManagerData imports bytes", 20, chunk.ImportsBuffer.Length);
        Console.WriteLine($"  ScenarioManager bin={chunk.InstanceBuffer.Length}, relo={chunk.RelocationBuffer.Length}, imp={chunk.ImportsBuffer.Length}");
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: Reproduce the real EP1 Red Alert button's 36-byte root and two localized strings. */
    //-------------------------------------------------------------------------------------------------
    private static unsafe void TestRedAlertButton()
    {
        const string xml = """
            <UIMouseTacticalRedAlertButton xmlns="uri:ea.com:eala:asset" id="UIMouseTacticalRedAlertButton">
              <MouseOverHelp Title="NAME:RedAlertButton" Description="DESC:RedAlertButton" />
            </UIMouseTacticalRedAlertButton>
            """;
        XmlDocument document = new();
        document.LoadXml(xml);
        XmlNamespaceManager namespaces = new(document.NameTable);
        namespaces.AddNamespace("ea", "uri:ea.com:eala:asset");
        Node node = new(document.CreateNavigator()!.SelectSingleNode("/ea:UIMouseTacticalRedAlertButton", namespaces)!, namespaces);

        UIMouseTacticalRedAlertButton* root;
        using Tracker tracker = new((void**)&root, (uint)sizeof(UIMouseTacticalRedAlertButton), false);
        Marshaler.Marshal(node, root, tracker);
        Chunk chunk = new();
        tracker.MakeRelocatable(chunk);
        ReadOnlySpan<byte> instance = chunk.InstanceBuffer;
        Expect(instance.Length == 76, "RedAlertButton instance bytes", 76, instance.Length);
        Expect(ReadUInt32(instance, 4) == 19, "RedAlertButton title length", 19, ReadUInt32(instance, 4));
        Expect(ReadUInt32(instance, 12) == 19, "RedAlertButton description length", 19, ReadUInt32(instance, 12));
        Expect(chunk.RelocationBuffer.Length == 12, "RedAlertButton relocation bytes", 12, chunk.RelocationBuffer.Length);
        Expect(chunk.ImportsBuffer.Length == 0, "RedAlertButton imports bytes", 0, chunk.ImportsBuffer.Length);
        Console.WriteLine($"  RedAlertButton bin={chunk.InstanceBuffer.Length}, relo={chunk.RelocationBuffer.Length}, imp={chunk.ImportsBuffer.Length}");
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: Reproduce both EP1-only music conditions from their real global-stream values. */
    //-------------------------------------------------------------------------------------------------
    private static unsafe void TestUprisingMusicConditions()
    {
        const string buttonXml = """
            <MusicScriptConditionNugget_LocalPlayerHitRedAlertButton xmlns="uri:ea.com:eala:asset"
                id="PlayerActiavtedRedAlert" DurationToReturnTrueAfterButtonHit="5.0s" />
            """;
        XmlDocument buttonDocument = new();
        buttonDocument.LoadXml(buttonXml);
        XmlNamespaceManager buttonNamespaces = new(buttonDocument.NameTable);
        buttonNamespaces.AddNamespace("ea", "uri:ea.com:eala:asset");
        Node buttonNode = new(buttonDocument.CreateNavigator()!.SelectSingleNode("/ea:MusicScriptConditionNugget_LocalPlayerHitRedAlertButton", buttonNamespaces)!, buttonNamespaces);
        MusicScriptConditionNugget_LocalPlayerHitRedAlertButton* buttonRoot;
        using Tracker buttonTracker = new((void**)&buttonRoot, (uint)sizeof(MusicScriptConditionNugget_LocalPlayerHitRedAlertButton), false);
        Marshaler.Marshal(buttonNode, buttonRoot, buttonTracker);
        Chunk buttonChunk = new();
        buttonTracker.MakeRelocatable(buttonChunk);
        Expect(buttonChunk.InstanceBuffer.Length == 8, "Red Alert music condition instance bytes", 8, buttonChunk.InstanceBuffer.Length);
        Expect(ReadUInt32(buttonChunk.InstanceBuffer, 4) == 0x40A00000, "Red Alert music condition duration", 0x40A00000, ReadUInt32(buttonChunk.InstanceBuffer, 4));
        Expect(buttonChunk.RelocationBuffer.Length == 0, "Red Alert music condition relocation bytes", 0, buttonChunk.RelocationBuffer.Length);
        Expect(buttonChunk.ImportsBuffer.Length == 0, "Red Alert music condition imports bytes", 0, buttonChunk.ImportsBuffer.Length);

        const string proximityXml = """
            <MusicScriptConditionNugget_ObjectTypesInProximity xmlns="uri:ea.com:eala:asset"
                id="SomeEnemyUnits_NearExpedition1" TypeAFilter="ObjectFilterAsset\1" TypeACount="1"
                TypeBFilter="ObjectFilterAsset\2" TypeBCount="3" Distance="300.0"
                TimeBetweenConditionChecks="0.6s" />
            """;
        XmlDocument proximityDocument = new();
        proximityDocument.LoadXml(proximityXml);
        XmlNamespaceManager proximityNamespaces = new(proximityDocument.NameTable);
        proximityNamespaces.AddNamespace("ea", "uri:ea.com:eala:asset");
        Node proximityNode = new(proximityDocument.CreateNavigator()!.SelectSingleNode("/ea:MusicScriptConditionNugget_ObjectTypesInProximity", proximityNamespaces)!, proximityNamespaces);
        MusicScriptConditionNugget_ObjectTypesInProximity* proximityRoot;
        using Tracker proximityTracker = new((void**)&proximityRoot, (uint)sizeof(MusicScriptConditionNugget_ObjectTypesInProximity), false);
        Marshaler.Marshal(proximityNode, proximityRoot, proximityTracker);
        Chunk proximityChunk = new();
        proximityTracker.MakeRelocatable(proximityChunk);
        ReadOnlySpan<byte> instance = proximityChunk.InstanceBuffer;
        Expect(instance.Length == 28, "proximity music condition instance bytes", 28, instance.Length);
        Expect(ReadUInt32(instance, 4) == 0x3F19999A, "proximity check interval", 0x3F19999A, ReadUInt32(instance, 4));
        Expect(ReadUInt32(instance, 8) == 2, "proximity type A import", 2, ReadUInt32(instance, 8)); // Reborn: final BIN imports are one-biased.
        Expect(ReadUInt32(instance, 12) == 1, "proximity type A count", 1, ReadUInt32(instance, 12));
        Expect(ReadUInt32(instance, 16) == 3, "proximity type B import", 3, ReadUInt32(instance, 16)); // Reborn: final BIN imports are one-biased.
        Expect(ReadUInt32(instance, 20) == 3, "proximity type B count", 3, ReadUInt32(instance, 20));
        Expect(ReadUInt32(instance, 24) == 0x43960000, "proximity distance", 0x43960000, ReadUInt32(instance, 24));
        Expect(proximityChunk.RelocationBuffer.Length == 0, "proximity music condition relocation bytes", 0, proximityChunk.RelocationBuffer.Length);
        Expect(proximityChunk.ImportsBuffer.Length == 12, "proximity music condition imports bytes", 12, proximityChunk.ImportsBuffer.Length);
        Console.WriteLine($"  MusicConditions redAlert={buttonChunk.InstanceBuffer.Length}/{buttonChunk.RelocationBuffer.Length}/{buttonChunk.ImportsBuffer.Length}, proximity={proximityChunk.InstanceBuffer.Length}/{proximityChunk.RelocationBuffer.Length}/{proximityChunk.ImportsBuffer.Length}");
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: Reproduce the first EP1 map-name heuristic embedded in 2SovietShockSpecialist. */
    //-------------------------------------------------------------------------------------------------
    private static unsafe void TestMapNameHeuristic()
    {
        const string mapName = @"data\maps\official\MAP_MP_2_Feasel5\MAP_MP_2_Feasel5.map";
        const string xml = """
            <MapNameHeuristic xmlns="uri:ea.com:eala:asset"
                Name="data\maps\official\MAP_MP_2_Feasel5\MAP_MP_2_Feasel5.map" />
            """;
        XmlDocument document = new();
        document.LoadXml(xml);
        XmlNamespaceManager namespaces = new(document.NameTable);
        namespaces.AddNamespace("ea", "uri:ea.com:eala:asset");
        Node node = new(document.CreateNavigator()!.SelectSingleNode("/ea:MapNameHeuristic", namespaces)!, namespaces);

        AIStateMapNameHeuristic* root;
        using Tracker tracker = new((void**)&root, (uint)sizeof(AIStateMapNameHeuristic), false);
        root->Base.TypeId = 0xDAFA6EB8u;
        Marshaler.Marshal(node, root, tracker);
        Chunk chunk = new();
        tracker.MakeRelocatable(chunk);
        ReadOnlySpan<byte> instance = chunk.InstanceBuffer;
        Expect(instance.Length == 76, "map-name heuristic instance bytes", 76, instance.Length);
        Expect(ReadUInt32(instance, 0) == 0xDAFA6EB8, "map-name heuristic type ID", unchecked((int)0xDAFA6EB8u), ReadUInt32(instance, 0));
        Expect(ReadUInt32(instance, 4) == mapName.Length, "map-name heuristic string length", mapName.Length, checked((int)ReadUInt32(instance, 4)));
        Expect(instance[12] == 1, "map-name heuristic default pass flag", 1, instance[12]);
        Expect(chunk.RelocationBuffer.Length == 8, "map-name heuristic relocation bytes", 8, chunk.RelocationBuffer.Length);
        Expect(chunk.ImportsBuffer.Length == 0, "map-name heuristic imports bytes", 0, chunk.ImportsBuffer.Length);
        Console.WriteLine($"  MapNameHeuristic bin={chunk.InstanceBuffer.Length}, relo={chunk.RelocationBuffer.Length}, imp={chunk.ImportsBuffer.Length}");
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: Exercise every EP1 joint subtype and its optional Vector3 allocations. */
    //-------------------------------------------------------------------------------------------------
    private static unsafe void TestDynamicsJointSet()
    {
        const string xml = """
            <Joints xmlns="uri:ea.com:eala:asset">
              <Joint>
                <Frame>
                  <Child BoneName="B_Spine"><Position x="1" y="2" z="3" /></Child>
                  <Parent BoneName="B_Hips" />
                </Frame>
                <Limits SwingType="SWING_CONE" SwingDisplacementLimit="0.1" SwingAngleLimit="0.5"
                    TwistType="TWIST_ARC" TwistDisplacementLimit="0.2" TwistAngleLimit="0.3" InertiaOverride="0.4">
                  <Position x="4" y="5" z="6" />
                </Limits>
              </Joint>
            </Joints>
            """;
        XmlDocument document = new();
        document.LoadXml(xml);
        XmlNamespaceManager namespaces = new(document.NameTable);
        namespaces.AddNamespace("ea", "uri:ea.com:eala:asset");
        Node node = new(document.CreateNavigator()!.SelectSingleNode("/ea:Joints", namespaces)!, namespaces);

        DynamicsJointSetType* root;
        using Tracker tracker = new((void**)&root, (uint)sizeof(DynamicsJointSetType), false);
        Marshaler.Marshal(node, root, tracker);
        Chunk chunk = new();
        tracker.MakeRelocatable(chunk);
        ReadOnlySpan<byte> instance = chunk.InstanceBuffer;
        Expect(instance.Length == 104, "dynamics joint-set instance bytes", 104, instance.Length);
        Expect(ReadUInt32(instance, 0) == 1, "dynamics joint count", 1, ReadUInt32(instance, 0));
        uint jointOffset = ReadUInt32(instance, 4);
        Expect(ReadUInt32(instance, checked((int)jointOffset)) == 7, "dynamics child bone length", 7, ReadUInt32(instance, checked((int)jointOffset)));
        Expect(ReadUInt32(instance, checked((int)jointOffset + 12)) == 6, "dynamics parent bone length", 6, ReadUInt32(instance, checked((int)jointOffset + 12)));
        Expect(ReadUInt32(instance, checked((int)jointOffset + 24)) == 1, "dynamics swing type", 1, ReadUInt32(instance, checked((int)jointOffset + 24)));
        Expect(ReadUInt32(instance, checked((int)jointOffset + 36)) == 1, "dynamics twist type", 1, ReadUInt32(instance, checked((int)jointOffset + 36)));
        Expect(ReadUInt32(instance, checked((int)jointOffset + 48)) == 0x3ECCCCCD, "dynamics inertia override", 0x3ECCCCCD, ReadUInt32(instance, checked((int)jointOffset + 48)));
        Expect(chunk.RelocationBuffer.Length == 24, "dynamics joint-set relocation bytes", 24, chunk.RelocationBuffer.Length);
        Expect(chunk.ImportsBuffer.Length == 0, "dynamics joint-set imports bytes", 0, chunk.ImportsBuffer.Length);
        Console.WriteLine($"  DynamicsJointSet bin={chunk.InstanceBuffer.Length}, relo={chunk.RelocationBuffer.Length}, imp={chunk.ImportsBuffer.Length}");
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: Exercise the corrected RA3 base and complete 256-byte EP1 dynamics draw pipeline. */
    //-------------------------------------------------------------------------------------------------
    private static unsafe void TestDynamicsDraw()
    {
        const string xml = """
            <DynamicsDraw xmlns="uri:ea.com:eala:asset" id="ModuleTag_Draw"
                Collision="NONINTERCOLLIDING" Explodiness="8" FlingPerturbation="15">
              <BoneVolumes>
                <BoneVolume BoneName="B" Mass="10" ContactTag="DEBRIS">
                  <Sphere Radius="1">
                    <Translation x="1" y="2" z="3" />
                    <Rotation x="0" y="0" z="0" w="1" />
                  </Sphere>
                </BoneVolume>
              </BoneVolumes>
              <Joints>
                <Joint>
                  <Frame><Child BoneName="B_Child" /><Parent BoneName="B_Parent" /></Frame>
                  <Limits SwingType="SWING_CONE" />
                </Joint>
              </Joints>
              <Lifetime Delay="8s" FadeTime="8s" />
            </DynamicsDraw>
            """;
        XmlDocument document = new();
        document.LoadXml(xml);
        XmlNamespaceManager namespaces = new(document.NameTable);
        namespaces.AddNamespace("ea", "uri:ea.com:eala:asset");
        Node node = new(document.CreateNavigator()!.SelectSingleNode("/ea:DynamicsDraw", namespaces)!, namespaces);

        W3DDynamicsDrawModuleData* root;
        using Tracker tracker = new((void**)&root, (uint)sizeof(W3DDynamicsDrawModuleData), false);
        Marshaler.Marshal(node, root, tracker);
        Chunk chunk = new();
        tracker.MakeRelocatable(chunk);
        ReadOnlySpan<byte> instance = chunk.InstanceBuffer;
        Expect(instance.Length == 484, "dynamics draw instance bytes", 484, instance.Length);
        Expect(ReadUInt32(instance, 216) == 0, "dynamics draw collision", 0, ReadUInt32(instance, 216));
        Expect(ReadUInt32(instance, 224) == 2, "dynamics draw priority", 2, ReadUInt32(instance, 224));
        Expect(ReadUInt32(instance, 232) == 0x41000000, "dynamics draw explodiness", 0x41000000, ReadUInt32(instance, 232));
        Expect(ReadUInt32(instance, 236) == 0x41700000, "dynamics draw fling perturbation", 0x41700000, ReadUInt32(instance, 236));
        Expect(ReadUInt32(instance, 240) != 0, "dynamics draw bone volumes pointer", 1, ReadUInt32(instance, 240) == 0 ? 0 : 1);
        Expect(ReadUInt32(instance, 244) != 0, "dynamics draw lifetime pointer", 1, ReadUInt32(instance, 244) == 0 ? 0 : 1);
        Expect(ReadUInt32(instance, 248) != 0, "dynamics draw joints pointer", 1, ReadUInt32(instance, 248) == 0 ? 0 : 1);
        Expect(instance[252] == 1, "dynamics draw initially-active default", 1, instance[252]);
        Expect(chunk.RelocationBuffer.Length == 48, "dynamics draw relocation bytes", 48, chunk.RelocationBuffer.Length);
        Expect(chunk.ImportsBuffer.Length == 0, "dynamics draw imports bytes", 0, chunk.ImportsBuffer.Length);
        Console.WriteLine($"  DynamicsDraw bin={chunk.InstanceBuffer.Length}, relo={chunk.RelocationBuffer.Length}, imp={chunk.ImportsBuffer.Length}");
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: exercise the corrected RA3 animation bit sets, enums and particle record in one graph. */
    //-------------------------------------------------------------------------------------------------
    private static unsafe void TestScriptedModelRecords()
    {
        const string xml = """
            <AnimationState xmlns="uri:ea.com:eala:asset" ParseCondStateType="PARSE_NORMAL"
                ConditionsYes="SPECIAL_POWER_SELECTED_PENDING"
                Flags="RANDOMSTART IGNORE_MOVEMENT_SPEED">
              <Animation AnimationMode="LOOP" AnimationAbsoluteTime="2s" />
              <ParticleSysBone FXTrigger="NONE" FXAction="SPAWN" />
            </AnimationState>
            """;
        XmlDocument document = new();
        document.LoadXml(xml);
        XmlNamespaceManager namespaces = new(document.NameTable);
        namespaces.AddNamespace("ea", "uri:ea.com:eala:asset");
        Node node = new(document.CreateNavigator()!.SelectSingleNode("/ea:AnimationState", namespaces)!, namespaces);

        AnimationState* root;
        using Tracker tracker = new((void**)&root, (uint)sizeof(AnimationState), false);
        Marshaler.Marshal(node, root, tracker);
        Chunk chunk = new();
        tracker.MakeRelocatable(chunk);
        ReadOnlySpan<byte> instance = chunk.InstanceBuffer;
        Expect(instance.Length == 236, "scripted-model records instance bytes", 236, instance.Length);
        Expect(ReadUInt32(instance, 88) == 0x802, "animation-state flags", 0x802, ReadUInt32(instance, 88));
        Expect(ReadUInt32(instance, 100) == 1, "animation-state animation count", 1, ReadUInt32(instance, 100));
        Expect(ReadUInt32(instance, 104) == 140, "animation-state animation relocation", 140, ReadUInt32(instance, 104));
        Expect(ReadUInt32(instance, 128) == 1, "animation-state particle count", 1, ReadUInt32(instance, 128));
        Expect(ReadUInt32(instance, 132) == 200, "animation-state particle relocation", 200, ReadUInt32(instance, 132));
        Expect(ReadUInt32(instance, 152) == 1, "animation mode LOOP", 1, ReadUInt32(instance, 152));
        Expect(ReadUInt32(instance, 220) == 3, "particle action SPAWN", 3, ReadUInt32(instance, 220));
        Expect(chunk.RelocationBuffer.Length == 12, "scripted-model records relocation bytes", 12, chunk.RelocationBuffer.Length);
        Expect(chunk.ImportsBuffer.Length == 0, "scripted-model records imports bytes", 0, chunk.ImportsBuffer.Length);
        Console.WriteLine($"  ScriptedModelRecords bin={chunk.InstanceBuffer.Length}, relo={chunk.RelocationBuffer.Length}, imp={chunk.ImportsBuffer.Length}");
    }

    private static unsafe void TestAudioDynamicsCollide()
    {
        const string xml = """
            <AudioDynamicsCollide xmlns="uri:ea.com:eala:asset"
                MinimumImpactVelocity="5.0">
              <MagnitudeSoundSelector>
                <Entry MinimumMagnitude="1.0" Sound="" />
                <Entry MinimumMagnitude="4.0" Sound="" />
              </MagnitudeSoundSelector>
            </AudioDynamicsCollide>
            """;

        XmlDocument document = new();
        document.LoadXml(xml);
        XmlNamespaceManager namespaces = new(document.NameTable);
        namespaces.AddNamespace("ea", "uri:ea.com:eala:asset");
        Node node = new(document.CreateNavigator()!.SelectSingleNode("/ea:AudioDynamicsCollide", namespaces)!, namespaces);

        AudioDynamicsCollideModuleData* root;
        using Tracker tracker = new((void**)&root, (uint)sizeof(AudioDynamicsCollideModuleData), false);
        Marshaler.Marshal(node, root, tracker);
        Chunk chunk = new();
        tracker.MakeRelocatable(chunk);
        ReadOnlySpan<byte> instance = chunk.InstanceBuffer;
        Expect(instance.Length == 36, "AudioDynamicsCollide instance bytes", 36, instance.Length);
        Expect(ReadUInt32(instance, 8) == 0x40A00000, "AudioDynamicsCollide.MinimumImpactVelocity", 0x40A00000, ReadUInt32(instance, 8));
        Expect(ReadUInt32(instance, 12) == 2, "AudioDynamicsCollide.Entry count", 2, ReadUInt32(instance, 12));
        Expect(ReadUInt32(instance, 16) == 20, "AudioDynamicsCollide.Entry relocation", 20, ReadUInt32(instance, 16));
        Expect(ReadUInt32(instance, 20) == 0x3F800000, "AudioDynamicsCollide.Entry[0].MinimumMagnitude", 0x3F800000, ReadUInt32(instance, 20));
        Expect(ReadUInt32(instance, 28) == 0x40800000, "AudioDynamicsCollide.Entry[1].MinimumMagnitude", 0x40800000, ReadUInt32(instance, 28));
        Expect(chunk.RelocationBuffer.Length == 8, "AudioDynamicsCollide relocation bytes", 8, chunk.RelocationBuffer.Length);
        Expect(chunk.ImportsBuffer.Length == 0, "AudioDynamicsCollide imports bytes", 0, chunk.ImportsBuffer.Length);
        Console.WriteLine($"  AudioDynamicsCollide bin={chunk.InstanceBuffer.Length}, relo={chunk.RelocationBuffer.Length}, imp={chunk.ImportsBuffer.Length}");
    }

    private static unsafe void TestDamageDynamicsCollide()
    {
        const string xml = """
            <DamageDynamicsCollide xmlns="uri:ea.com:eala:asset">
              <DamageNugget Radius="0" OnlyKillOwnerWhenTriggered="true"
                  DelayTimeSeconds="0s" DamageType="UNRESISTABLE"
                  DamageFXType="JAPAN_CANNON" DeathType="SUICIDED" />
            </DamageDynamicsCollide>
            """;

        XmlDocument document = new();
        document.LoadXml(xml);
        XmlNamespaceManager namespaces = new(document.NameTable);
        namespaces.AddNamespace("ea", "uri:ea.com:eala:asset");
        Node node = new(document.CreateNavigator()!.SelectSingleNode("/ea:DamageDynamicsCollide", namespaces)!, namespaces);

        DamageDynamicsCollideModuleData* root;
        using Tracker tracker = new((void**)&root, (uint)sizeof(DamageDynamicsCollideModuleData), false);
        Marshaler.Marshal(node, root, tracker);
        Chunk chunk = new();
        tracker.MakeRelocatable(chunk);
        ReadOnlySpan<byte> instance = chunk.InstanceBuffer;
        Expect(instance.Length == 176, "DamageDynamicsCollide instance bytes", 176, instance.Length);
        Expect(ReadUInt32(instance, 8) == 0x41200000, "DamageDynamicsCollide.MaxMagnitude", 0x41200000, ReadUInt32(instance, 8));
        Expect(ReadUInt32(instance, 12) == 1, "DamageDynamicsCollide.DamageNugget count", 1, ReadUInt32(instance, 12));
        Expect(ReadUInt32(instance, 16) == 20, "DamageDynamicsCollide.DamageNugget relocation", 20, ReadUInt32(instance, 16));
        Expect(ReadUInt32(instance, 36) == 0, "DamageNugget.Radius", 0, ReadUInt32(instance, 36));
        Expect(instance[170] == 1, "DamageNugget.OnlyKillOwnerWhenTriggered", 1, instance[170]);
        Expect(chunk.ImportsBuffer.Length == 0, "DamageDynamicsCollide imports bytes", 0, chunk.ImportsBuffer.Length);
        Console.WriteLine($"  DamageDynamicsCollide bin={chunk.InstanceBuffer.Length}, relo={chunk.RelocationBuffer.Length}, imp={chunk.ImportsBuffer.Length}");
    }

    private static unsafe void TestReactionFXOnDamage()
    {
        const string xml = """
            <ReactionFXOnDamage xmlns="uri:ea.com:eala:asset">
              <DamageReactionFXTrigger PercentDamagedThreshold="99.99"
                  TimeBetweenTriggers="5.0s" />
              <HealingReactionFXTrigger TimeBetweenTriggers="2.0s"
                  ResetTimerWhenTimerBlocks="true"
                  SourceFilter="AOF_YurikoHealStations"
                  SoundToPlay="NEU_HealthStation_Heal" />
            </ReactionFXOnDamage>
            """;

        XmlDocument document = new();
        document.LoadXml(xml);
        XmlNamespaceManager namespaces = new(document.NameTable);
        namespaces.AddNamespace("ea", "uri:ea.com:eala:asset");
        Node node = new(document.CreateNavigator()!.SelectSingleNode("/ea:ReactionFXOnDamage", namespaces)!, namespaces);

        ReactionFXOnDamageModuleData* root;
        using Tracker tracker = new((void**)&root, (uint)sizeof(ReactionFXOnDamageModuleData), false);
        Marshaler.Marshal(node, root, tracker);
        Chunk chunk = new();
        tracker.MakeRelocatable(chunk);
        ReadOnlySpan<byte> instance = chunk.InstanceBuffer;
        Expect(instance.Length == 92, "ReactionFXOnDamage instance bytes", 92, instance.Length);
        Expect(ReadUInt32(instance, 8) == 1, "ReactionFXOnDamage damage count", 1, ReadUInt32(instance, 8));
        Expect(ReadUInt32(instance, 12) == 24, "ReactionFXOnDamage damage list relocation", 24, ReadUInt32(instance, 12));
        Expect(ReadUInt32(instance, 16) == 1, "ReactionFXOnDamage healing count", 1, ReadUInt32(instance, 16));
        Expect(ReadUInt32(instance, 20) == 56, "ReactionFXOnDamage healing list relocation", 56, ReadUInt32(instance, 20));
        Expect(ReadUInt32(instance, 24) == 48, "ReactionFXOnDamage damage threshold relocation", 48, ReadUInt32(instance, 24));
        Expect(ReadUInt32(instance, 28) == 52, "ReactionFXOnDamage damage timer relocation", 52, ReadUInt32(instance, 28));
        Expect(ReadUInt32(instance, 36) == 0, "ReactionFXOnDamage omitted damage voice", 0, ReadUInt32(instance, 36));
        Expect(ReadUInt32(instance, 48) == 0x42C7FAE1, "ReactionFXOnDamage damage threshold", 0x42C7FAE1, ReadUInt32(instance, 48));
        Expect(ReadUInt32(instance, 52) == 0x40A00000, "ReactionFXOnDamage damage timer", 0x40A00000, ReadUInt32(instance, 52));
        Expect(ReadUInt32(instance, 60) == 80, "ReactionFXOnDamage healing timer relocation", 80, ReadUInt32(instance, 60));
        Expect(ReadUInt32(instance, 64) == 84, "ReactionFXOnDamage healing filter relocation", 84, ReadUInt32(instance, 64));
        Expect(ReadUInt32(instance, 72) == 88, "ReactionFXOnDamage healing sound relocation", 88, ReadUInt32(instance, 72));
        Expect(instance[76] == 1, "ReactionFXOnDamage reset timer", 1, instance[76]);
        Expect(chunk.RelocationBuffer.Length == 32, "ReactionFXOnDamage relocation bytes", 32, chunk.RelocationBuffer.Length);
        Expect(chunk.ImportsBuffer.Length == 0, "ReactionFXOnDamage standalone imports bytes", 0, chunk.ImportsBuffer.Length);
        Console.WriteLine($"  ReactionFXOnDamage bin={chunk.InstanceBuffer.Length}, relo={chunk.RelocationBuffer.Length}, imp={chunk.ImportsBuffer.Length}");
    }

    private static unsafe void TestDamageSphereUpdate()
    {
        const string xml = """
            <DamageSphereUpdate xmlns="uri:ea.com:eala:asset"
                UnpackTime="1.35s" Weapon="AlliedFutureTankNeutronWeapon_IncrementalWeapon"
                RadiusMin="25" RadiusMax="180" ExpansionPerSecond="125"
                SphereBoneName="SHIELDLARGE" SphereSizeMultiplier="16.0"
                UnpackModelConditions="USER_4" ModelConditions="USER_5"
                UnpackObjectStatus="WEAPON_UPGRADED_01" ObjectStatus="WEAPON_UPGRADED_02">
              <ObjectFilter Rule="ALL"
                  Exclude="BRIDGE BRIDGE_SEGMENT BRIDGE_ENDCAP BRIDGE_GATEHOUSE" />
            </DamageSphereUpdate>
            """;

        XmlDocument document = new();
        document.LoadXml(xml);
        XmlNamespaceManager namespaces = new(document.NameTable);
        namespaces.AddNamespace("ea", "uri:ea.com:eala:asset");
        Node node = new(document.CreateNavigator()!.SelectSingleNode("/ea:DamageSphereUpdate", namespaces)!, namespaces);

        DamageSphereUpdateModuleData* root;
        using Tracker tracker = new((void**)&root, (uint)sizeof(DamageSphereUpdateModuleData), false);
        Marshaler.Marshal(node, root, tracker);
        Chunk chunk = new();
        tracker.MakeRelocatable(chunk);
        ReadOnlySpan<byte> instance = chunk.InstanceBuffer;
        Expect(instance.Length == 384, "DamageSphereUpdate instance bytes", 384, instance.Length);
        Expect(ReadUInt32(instance, 8) == 0x43340000, "DamageSphereUpdate RadiusMax", 0x43340000, ReadUInt32(instance, 8));
        Expect(ReadUInt32(instance, 12) == 0x41C80000, "DamageSphereUpdate RadiusMin", 0x41C80000, ReadUInt32(instance, 12));
        Expect(ReadUInt32(instance, 24) == 0x3F800000, "DamageSphereUpdate ScanFrequency", 0x3F800000, ReadUInt32(instance, 24));
        Expect(ReadUInt32(instance, 28) == 0x41200000, "DamageSphereUpdate Duration", 0x41200000, ReadUInt32(instance, 28));
        Expect(ReadUInt32(instance, 32) == 11, "DamageSphereUpdate sphere bone length", 11, ReadUInt32(instance, 32));
        Expect(ReadUInt32(instance, 36) == 372, "DamageSphereUpdate sphere bone relocation", 372, ReadUInt32(instance, 36));
        Expect(ReadUInt32(instance, 40) == 0x41800000, "DamageSphereUpdate SphereSizeMultiplier", 0x41800000, ReadUInt32(instance, 40));
        Expect(ReadUInt32(instance, 52) == 1, "DamageSphereUpdate ObjectFilter.Rule", 1, ReadUInt32(instance, 52));
        Expect(instance[172] == 0, "DamageSphereUpdate InitiallyActive", 0, instance[172]);
        Expect(instance[173] == 1, "DamageSphereUpdate DrawDebugCircle", 1, instance[173]);
        Expect(ReadUInt32(instance, 176) == 0x3FACCCCD, "DamageSphereUpdate UnpackTime", 0x3FACCCCD, ReadUInt32(instance, 176));
        Expect(ReadUInt32(instance, 184) == 0x42FA0000, "DamageSphereUpdate ExpansionPerSecond", 0x42FA0000, ReadUInt32(instance, 184));
        Expect(chunk.ImportsBuffer.Length == 0, "DamageSphereUpdate standalone imports bytes", 0, chunk.ImportsBuffer.Length);
        Console.WriteLine($"  DamageSphereUpdate bin={chunk.InstanceBuffer.Length}, relo={chunk.RelocationBuffer.Length}, imp={chunk.ImportsBuffer.Length}");
    }

    private static unsafe void TestYurikoShieldSphereUpdate()
    {
        const string xml = """
            <YurikoShieldSphereUpdate xmlns="uri:ea.com:eala:asset"
                InitiallyActive="true" RadiusMin="24" RadiusMax="24"
                ScanFrequency="0.25s" Duration="10s" MaxDamage="9999999999"
                DamageTypesNotToAbsorb="HEALING RADIATION"
                ObjectStatus="GENERIC_TOGGLE_STATE IGNORING_STEALTH"
                ModelCondition="USER_4" SphereBoneName="SHIELDSMALL"
                MajorShieldHitFX="FX_YurikoShieldHitSmallMajor"
                MinorShieldHitFX="FX_YurikoShieldHitSmallMinor"
                MinorShieldDamageTypes="GUN CANNON LASER UNDEFINED MAGIC PIERCE">
              <ObjectFilter Rule="ANY" Relationship="ALLIES"
                  Include="INFANTRY VEHICLE STRUCTURE" />
              <IgnoreInsideToInsideCheck Rule="ALL" />
            </YurikoShieldSphereUpdate>
            """;

        XmlDocument document = new();
        document.LoadXml(xml);
        XmlNamespaceManager namespaces = new(document.NameTable);
        namespaces.AddNamespace("ea", "uri:ea.com:eala:asset");
        Node node = new(document.CreateNavigator()!.SelectSingleNode("/ea:YurikoShieldSphereUpdate", namespaces)!, namespaces);

        YurikoShieldSphereUpdateModuleData* root;
        using Tracker tracker = new((void**)&root, (uint)sizeof(YurikoShieldSphereUpdateModuleData), false);
        Marshaler.Marshal(node, root, tracker);
        Chunk chunk = new();
        tracker.MakeRelocatable(chunk);
        ReadOnlySpan<byte> instance = chunk.InstanceBuffer;
        Expect(instance.Length == 460, "YurikoShieldSphereUpdate instance bytes", 460, instance.Length);
        Expect(ReadUInt32(instance, 8) == 0x41C00000, "YurikoShieldSphereUpdate RadiusMax", 0x41C00000, ReadUInt32(instance, 8));
        Expect(ReadUInt32(instance, 16) == 0x20, "YurikoShieldSphereUpdate DamageTypesNotToAbsorb[0]", 0x20, ReadUInt32(instance, 16));
        Expect(ReadUInt32(instance, 20) == 0x20, "YurikoShieldSphereUpdate DamageTypesNotToAbsorb[1]", 0x20, ReadUInt32(instance, 20));
        Expect(ReadUInt32(instance, 24) == 0x3E800000, "YurikoShieldSphereUpdate ScanFrequency", 0x3E800000, ReadUInt32(instance, 24));
        Expect(ReadUInt32(instance, 32) == 11, "YurikoShieldSphereUpdate sphere bone length", 11, ReadUInt32(instance, 32));
        Expect(ReadUInt32(instance, 36) == 328, "YurikoShieldSphereUpdate sphere bone relocation", 328, ReadUInt32(instance, 36));
        Expect(ReadUInt32(instance, 168) == 340, "YurikoShieldSphereUpdate ignore-filter relocation", 340, ReadUInt32(instance, 168));
        Expect(instance[172] == 1, "YurikoShieldSphereUpdate InitiallyActive", 1, instance[172]);
        Expect(ReadUInt32(instance, 176) == 0x501502F9, "YurikoShieldSphereUpdate MaxDamage", 0x501502F9, ReadUInt32(instance, 176));
        Expect(ReadUInt32(instance, 320) == 0x00626008, "YurikoShieldSphereUpdate minor damage flags", 0x00626008, ReadUInt32(instance, 320));
        Expect(chunk.ImportsBuffer.Length == 0, "YurikoShieldSphereUpdate standalone imports bytes", 0, chunk.ImportsBuffer.Length);
        Console.WriteLine($"  YurikoShieldSphereUpdate bin={chunk.InstanceBuffer.Length}, relo={chunk.RelocationBuffer.Length}, imp={chunk.ImportsBuffer.Length}");
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: Verify tint defaults and by-value color follow the native effect header without damage fields. */
    //-------------------------------------------------------------------------------------------------
    private static unsafe void TestTintObjectsNugget()
    {
        const string xml = """
            <TintObjectsNugget xmlns="uri:ea.com:eala:asset" Frequency="4" Amplitude="0.5">
              <Color R="1" G="0.5" B="0.25" />
            </TintObjectsNugget>
            """;
        XmlDocument document = new();
        document.LoadXml(xml);
        XmlNamespaceManager namespaces = new(document.NameTable);
        namespaces.AddNamespace("ea", "uri:ea.com:eala:asset");
        Node node = new(document.CreateNavigator()!.SelectSingleNode("/ea:TintObjectsNugget", namespaces)!, namespaces);
        TintObjectsNuggetType* root;
        using Tracker tracker = new((void**)&root, (uint)sizeof(TintObjectsNuggetType), false);
        Marshaler.Marshal(node, root, tracker);
        Chunk chunk = new();
        tracker.MakeRelocatable(chunk);
        ReadOnlySpan<byte> instance = chunk.InstanceBuffer;
        Expect(instance.Length == 72, "TintObjectsNugget instance bytes", 72, instance.Length);
        Expect(ReadUInt32(instance, 40) == 0x40000000, "TintObjectsNugget default pre-time", 0x40000000, ReadUInt32(instance, 40));
        Expect(ReadUInt32(instance, 52) == 0x40800000, "TintObjectsNugget frequency", 0x40800000, ReadUInt32(instance, 52));
        Expect(ReadUInt32(instance, 60) == 0x3F800000, "TintObjectsNugget red", 0x3F800000, ReadUInt32(instance, 60));
        Expect(ReadUInt32(instance, 68) == 0x3E800000, "TintObjectsNugget blue", 0x3E800000, ReadUInt32(instance, 68));
        Expect(chunk.RelocationBuffer.Length == 0, "TintObjectsNugget relocation bytes", 0, chunk.RelocationBuffer.Length);
        Expect(chunk.ImportsBuffer.Length == 0, "TintObjectsNugget imports bytes", 0, chunk.ImportsBuffer.Length);
        Console.WriteLine($"  TintObjectsNugget bin={chunk.InstanceBuffer.Length}, relo={chunk.RelocationBuffer.Length}, imp={chunk.ImportsBuffer.Length}");
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: Verify containment defaults, inline masks and consecutive EP1 passenger records. */
    //-------------------------------------------------------------------------------------------------
    private static unsafe void TestOpenContain()
    {
        const string xml = """
            <OpenContain xmlns="uri:ea.com:eala:asset" ContainMax="6" PassDisabilityToRiders="true" ObjectStatusOfContained="CAN_ATTACK">
              <PassengerData MaxPassengers="2" SlingUnderBone="true"><Filter Rule="ALL" /></PassengerData>
              <PassengerData><Filter Rule="ALL" /></PassengerData>
            </OpenContain>
            """;
        XmlDocument document = new();
        document.LoadXml(xml);
        XmlNamespaceManager namespaces = new(document.NameTable);
        namespaces.AddNamespace("ea", "uri:ea.com:eala:asset");
        Node node = new(document.CreateNavigator()!.SelectSingleNode("/ea:OpenContain", namespaces)!, namespaces);
        OpenContainModuleData* root;
        using Tracker tracker = new((void**)&root, (uint)sizeof(OpenContainModuleData), false);
        Marshaler.Marshal(node, root, tracker);
        Chunk chunk = new();
        tracker.MakeRelocatable(chunk);
        ReadOnlySpan<byte> instance = chunk.InstanceBuffer;
        Expect(instance.Length == 428, "OpenContain instance bytes", 428, instance.Length);
        Expect(ReadUInt32(instance, 8) == 6, "OpenContain capacity", 6, ReadUInt32(instance, 8));
        uint disabled = (1u << (int)DisabledType.UNDERPOWERED) | (1u << (int)DisabledType.EMP);
        Expect(ReadUInt32(instance, 36) == disabled, "OpenContain disabled defaults", (int)disabled, ReadUInt32(instance, 36));
        Expect(ReadUInt32(instance, 40) == 2, "OpenContain passenger status", 2, ReadUInt32(instance, 40));
        int occupied = (int)ObjectStatusType.CONTAINER_OCCUPIED;
        uint occupiedBit = 1u << (occupied % 32);
        Expect(ReadUInt32(instance, 72 + 4 * (occupied / 32)) == occupiedBit, "OpenContain occupancy default", (int)occupiedBit, ReadUInt32(instance, 72 + 4 * (occupied / 32)));
        Expect(ReadUInt32(instance, 104) == 100, "OpenContain modifier time", 100, ReadUInt32(instance, 104));
        Expect(ReadUInt32(instance, 120) == 2, "OpenContain passenger count", 2, ReadUInt32(instance, 120));
        Expect(ReadUInt32(instance, 124) == 156, "OpenContain passenger relocation", 156, ReadUInt32(instance, 124));
        Expect(instance[149] == 1 && instance[155] == 1, "OpenContain rider and enabled flags", 1, instance[149]);
        Expect(ReadUInt32(instance, 164) == 2, "Passenger capacity", 2, ReadUInt32(instance, 164));
        Expect(instance[288] == 1 && instance[424] == 0, "Passenger sling flags and stride", 1, instance[288]);
        Expect(ReadUInt32(instance, 300) == 0, "Passenger default capacity", 0, ReadUInt32(instance, 300));
        // Reborn: the one list pointer is followed by the relocation table's terminating sentinel.
        Expect(chunk.RelocationBuffer.Length == 8, "OpenContain relocation bytes", 8, chunk.RelocationBuffer.Length);
        Expect(ReadUInt32(chunk.RelocationBuffer, 0) == 124, "OpenContain relocation slot", 124, ReadUInt32(chunk.RelocationBuffer, 0));
        Expect(ReadUInt32(chunk.RelocationBuffer, 4) == uint.MaxValue, "OpenContain relocation sentinel", -1, ReadUInt32(chunk.RelocationBuffer, 4));
        Expect(chunk.ImportsBuffer.Length == 0, "OpenContain imports bytes", 0, chunk.ImportsBuffer.Length);
        Console.WriteLine($"  OpenContain bin={chunk.InstanceBuffer.Length}, relo={chunk.RelocationBuffer.Length}, imp={chunk.ImportsBuffer.Length}");
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: Verify inherited transport defaults, payload relocation and garrison's inline roster. */
    //-------------------------------------------------------------------------------------------------
    private static unsafe void TestContainDerivatives()
    {
        XmlDocument document = new();
        document.LoadXml("""
            <HordeTransportContain xmlns="uri:ea.com:eala:asset" FlyOffMapOnEmpty="true" EnterFadeTime="2" ExtendedExitContainerChecks="true">
              <InitialPayload Name="TestPassenger" Count="3" />
            </HordeTransportContain>
            """);
        XmlNamespaceManager namespaces = new(document.NameTable);
        namespaces.AddNamespace("ea", "uri:ea.com:eala:asset");
        Node node = new(document.CreateNavigator()!.SelectSingleNode("/ea:HordeTransportContain", namespaces)!, namespaces);
        HordeTransportContainModuleData* transport;
        using Tracker transportTracker = new((void**)&transport, (uint)sizeof(HordeTransportContainModuleData), false);
        Marshaler.Marshal(node, transport, transportTracker);
        Chunk transportChunk = new();
        transportTracker.MakeRelocatable(transportChunk);
        ReadOnlySpan<byte> bytes = transportChunk.InstanceBuffer;
        Expect(bytes.Length == 364, "HordeTransport instance bytes", 364, bytes.Length);
        Expect(ReadUInt32(bytes, 304) == 0x40000000, "Transport enter fade", 0x40000000, ReadUInt32(bytes, 304));
        Expect(ReadUInt32(bytes, 312) == 0x3F333333, "Transport snappyness default", 0x3F333333, ReadUInt32(bytes, 312));
        Expect(ReadUInt32(bytes, 316) == 1 && ReadUInt32(bytes, 320) == 356, "Transport payload list", 356, ReadUInt32(bytes, 320));
        // Reborn: weak GameObject IDs hash the case-insensitive name; they are not strong reference tokens.
        uint passengerId = FastHash.GetHashCode("testpassenger");
        Expect(ReadUInt32(bytes, 356) == passengerId && ReadUInt32(bytes, 360) == 3, "Transport payload record", unchecked((int)passengerId), ReadUInt32(bytes, 356));
        Expect(bytes[340] == 1 && bytes[343] == 1 && bytes[348] == 1 && bytes[352] == 1, "Transport inherited defaults and horde flag", 1, bytes[352]);
        Expect(transportChunk.RelocationBuffer.Length == 8 && ReadUInt32(transportChunk.RelocationBuffer, 0) == 320, "Transport payload relocation", 320, ReadUInt32(transportChunk.RelocationBuffer, 0));
        Expect(transportChunk.ImportsBuffer.Length == 0, "Transport weak payload imports", 0, transportChunk.ImportsBuffer.Length);

        document.LoadXml("""
            <GarrisonContain xmlns="uri:ea.com:eala:asset" MobileGarrison="true">
              <InitialRoster TemplateId="TestGarrisonUnit" Count="4" />
            </GarrisonContain>
            """);
        node = new Node(document.CreateNavigator()!.SelectSingleNode("/ea:GarrisonContain", namespaces)!, namespaces);
        GarrisonContainModuleData* garrison;
        using Tracker garrisonTracker = new((void**)&garrison, (uint)sizeof(GarrisonContainModuleData), false);
        Marshaler.Marshal(node, garrison, garrisonTracker);
        Chunk garrisonChunk = new();
        garrisonTracker.MakeRelocatable(garrisonChunk);
        bytes = garrisonChunk.InstanceBuffer;
        Expect(bytes.Length == 168, "Garrison instance bytes", 168, bytes.Length);
        // Reborn: the inline roster also stores a weak hashed ID without an import entry.
        uint rosterId = FastHash.GetHashCode("testgarrisonunit");
        Expect(ReadUInt32(bytes, 156) == rosterId && ReadUInt32(bytes, 160) == 4, "Garrison inline roster", unchecked((int)rosterId), ReadUInt32(bytes, 156));
        Expect(bytes[164] == 1 && bytes[166] == 0, "Garrison flags and capture default", 1, bytes[164]);
        Expect(garrisonChunk.RelocationBuffer.Length == 0 && garrisonChunk.ImportsBuffer.Length == 0, "Garrison no roster relocation or imports", 0, garrisonChunk.RelocationBuffer.Length);
        Console.WriteLine($"  Contain derivatives hordeTransport={transportChunk.InstanceBuffer.Length}/8/0, garrison={garrisonChunk.InstanceBuffer.Length}/0/0");
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: Verify garrison derivative vectors, optional defaults and polymorphic dispatch. */
    //-------------------------------------------------------------------------------------------------
    private static unsafe void TestGarrisonDerivatives()
    {
        XmlDocument document = new();
        document.LoadXml("""
            <HordeGarrisonContain xmlns="uri:ea.com:eala:asset" ExitDelay="7">
              <EntryOffset x="1" y="2" z="3" />
              <EntryPosition x="4" y="5" z="6" />
              <ExitOffset x="7" y="8" z="9" />
            </HordeGarrisonContain>
            """);
        XmlNamespaceManager namespaces = new(document.NameTable);
        namespaces.AddNamespace("ea", "uri:ea.com:eala:asset");
        Node node = new(document.CreateNavigator()!.SelectSingleNode("/ea:HordeGarrisonContain", namespaces)!, namespaces);
        HordeGarrisonContainModuleData* horde;
        using Tracker hordeTracker = new((void**)&horde, (uint)sizeof(HordeGarrisonContainModuleData), false);
        Marshaler.Marshal(node, horde, hordeTracker);
        Chunk hordeChunk = new();
        hordeTracker.MakeRelocatable(hordeChunk);
        ReadOnlySpan<byte> bytes = hordeChunk.InstanceBuffer;
        Expect(bytes.Length == 220, "HordeGarrison instance bytes", 220, bytes.Length);
        Expect(ReadUInt32(bytes, 168) == 7, "HordeGarrison raw exit delay", 7, ReadUInt32(bytes, 168));
        Expect(ReadUInt32(bytes, 172) == 184 && ReadUInt32(bytes, 176) == 196 && ReadUInt32(bytes, 180) == 208, "HordeGarrison vector pointers", 208, ReadUInt32(bytes, 180));
        Expect(ReadUInt32(bytes, 184) == 0x3F800000 && ReadUInt32(bytes, 216) == 0x41100000, "HordeGarrison vector values", 0x41100000, ReadUInt32(bytes, 216));
        Expect(hordeChunk.RelocationBuffer.Length == 16 && hordeChunk.ImportsBuffer.Length == 0, "HordeGarrison relocation bytes", 16, hordeChunk.RelocationBuffer.Length);

        document.LoadXml("""
            <ContestableGarrisonContain xmlns="uri:ea.com:eala:asset" TypeId="0x8C50F0D7" RequiredClearingObjectStatus="CAN_ATTACK" />
            """);
        node = new Node(document.CreateNavigator()!.SelectSingleNode("/ea:ContestableGarrisonContain", namespaces)!, namespaces);
        BehaviorModuleData** slot;
        using Tracker contestTracker = new((void**)&slot, (uint)sizeof(BehaviorModuleData*), false);
        Marshaler.Marshal(node, slot, contestTracker);
        Chunk contestChunk = new();
        contestTracker.MakeRelocatable(contestChunk);
        bytes = contestChunk.InstanceBuffer;
        Expect(bytes.Length == 240, "ContestableGarrison dispatch instance", 240, bytes.Length);
        Expect(ReadUInt32(bytes, 0) == 4, "ContestableGarrison root pointer", 4, ReadUInt32(bytes, 0));
        Expect(ReadUInt32(bytes, 4) == 0x8C50F0D7, "ContestableGarrison type hash", unchecked((int)0x8C50F0D7), ReadUInt32(bytes, 4));
        Expect(ReadUInt32(bytes, 172) == 2, "ContestableGarrison required status", 2, ReadUInt32(bytes, 172));
        int iron = (int)ObjectStatusType.UNDER_IRON_CURTAIN;
        uint ironBit = 1u << (iron % 32);
        Expect(ReadUInt32(bytes, 204 + 4 * (iron / 32)) == ironBit, "ContestableGarrison forbidden default", unchecked((int)ironBit), ReadUInt32(bytes, 204 + 4 * (iron / 32)));
        Expect(ReadUInt32(bytes, 236) == 0x3F800000, "ContestableGarrison eject default", 0x3F800000, ReadUInt32(bytes, 236));
        Expect(contestChunk.RelocationBuffer.Length == 8 && contestChunk.ImportsBuffer.Length == 0, "ContestableGarrison dispatch relocation", 8, contestChunk.RelocationBuffer.Length);
        Console.WriteLine($"  Garrison derivatives horde={hordeChunk.InstanceBuffer.Length}/16/0, contestableDispatch={contestChunk.InstanceBuffer.Length}/8/0");
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: Verify audited containment leaf fields without mistaking weak IDs for imports. */
    //-------------------------------------------------------------------------------------------------
    private static unsafe void TestContainLeafModules()
    {
        XmlDocument document = new();
        document.LoadXml("""
            <SlaughterHordeContain xmlns="uri:ea.com:eala:asset" CashBackPercent="25" CanAlwaysEnterStatus="CAN_ATTACK" SlaughterFX="FXList\123">
              <CanAlwaysEnterObjectFilter Rule="ALL" />
            </SlaughterHordeContain>
            """);
        XmlNamespaceManager namespaces = new(document.NameTable);
        namespaces.AddNamespace("ea", "uri:ea.com:eala:asset");
        Node node = new(document.CreateNavigator()!.SelectSingleNode("/ea:SlaughterHordeContain", namespaces)!, namespaces);
        SlaughterHordeContainModuleData* slaughter;
        using Tracker slaughterTracker = new((void**)&slaughter, (uint)sizeof(SlaughterHordeContainModuleData), false);
        Marshaler.Marshal(node, slaughter, slaughterTracker);
        Chunk slaughterChunk = new();
        slaughterTracker.MakeRelocatable(slaughterChunk);
        ReadOnlySpan<byte> bytes = slaughterChunk.InstanceBuffer;
        Expect(bytes.Length == 344, "Slaughter instance bytes", 344, bytes.Length);
        Expect(ReadUInt32(bytes, 184) == 0x3E800000, "Slaughter refund 25 percent", 0x3E800000, ReadUInt32(bytes, 184));
        Expect(ReadUInt32(bytes, 188) == 2, "Slaughter allowed status", 2, ReadUInt32(bytes, 188));
        Expect(ReadUInt32(bytes, 220) == 124, "Slaughter FX token", 124, ReadUInt32(bytes, 220)); // Reborn: final import indices reserve zero for null.
        Expect(ReadUInt32(bytes, 228) == 1, "Slaughter filter rule", 1, ReadUInt32(bytes, 228));
        Expect(slaughterChunk.RelocationBuffer.Length == 0 && slaughterChunk.ImportsBuffer.Length == 8, "Slaughter FX import bytes", 8, slaughterChunk.ImportsBuffer.Length);
        Expect(ReadUInt32(slaughterChunk.ImportsBuffer, 0) == 220, "Slaughter FX import slot", 220, ReadUInt32(slaughterChunk.ImportsBuffer, 0));

        document.LoadXml("""
            <HealContain xmlns="uri:ea.com:eala:asset" TimeForFullHeal="2s" />
            """);
        node = new Node(document.CreateNavigator()!.SelectSingleNode("/ea:HealContain", namespaces)!, namespaces);
        HealContainModuleData* heal;
        using Tracker healTracker = new((void**)&heal, (uint)sizeof(HealContainModuleData), false);
        Marshaler.Marshal(node, heal, healTracker);
        Chunk healChunk = new();
        healTracker.MakeRelocatable(healChunk);
        Expect(healChunk.InstanceBuffer.Length == 188 && ReadUInt32(healChunk.InstanceBuffer, 184) == 0x40000000, "Heal duration", 0x40000000, ReadUInt32(healChunk.InstanceBuffer, 184));
        Expect(healChunk.RelocationBuffer.Length == 0 && healChunk.ImportsBuffer.Length == 0, "Heal no external records", 0, healChunk.RelocationBuffer.Length);

        document.LoadXml("""
            <TunnelContain xmlns="uri:ea.com:eala:asset" TunnelMasterObject="TestTunnelMaster" />
            """);
        node = new Node(document.CreateNavigator()!.SelectSingleNode("/ea:TunnelContain", namespaces)!, namespaces);
        TunnelContainModuleData* tunnel;
        using Tracker tunnelTracker = new((void**)&tunnel, (uint)sizeof(TunnelContainModuleData), false);
        Marshaler.Marshal(node, tunnel, tunnelTracker);
        Chunk tunnelChunk = new();
        tunnelTracker.MakeRelocatable(tunnelChunk);
        uint masterId = FastHash.GetHashCode("testtunnelmaster");
        Expect(tunnelChunk.InstanceBuffer.Length == 192 && ReadUInt32(tunnelChunk.InstanceBuffer, 184) == masterId, "Tunnel weak master ID", unchecked((int)masterId), ReadUInt32(tunnelChunk.InstanceBuffer, 184));
        Expect(tunnelChunk.InstanceBuffer[188] == 0, "Tunnel delete default", 0, tunnelChunk.InstanceBuffer[188]);
        Expect(tunnelChunk.RelocationBuffer.Length == 0 && tunnelChunk.ImportsBuffer.Length == 0, "Tunnel weak reference has no import", 0, tunnelChunk.ImportsBuffer.Length);
        Console.WriteLine($"  Contain leaf modules slaughter={slaughterChunk.InstanceBuffer.Length}/0/8, heal={healChunk.InstanceBuffer.Length}/0/0, tunnel={tunnelChunk.InstanceBuffer.Length}/0/0");
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: Verify horde defaults, sixteen-byte rank stride and nested position relocations. */
    //-------------------------------------------------------------------------------------------------
    private static unsafe void TestHordeContain()
    {
        XmlDocument document = new();
        document.LoadXml("""
            <HordeContain xmlns="uri:ea.com:eala:asset" EvaEventLastMemberDeath="EvaEvent\123" ForbiddenCoverStatus="CAN_ATTACK">
              <RankInfo RankID="1" UnitType="TestRankOne"><Position X="2" /></RankInfo>
              <RankInfo RankID="2" UnitType="TestRankTwo"><Position Y="3" /></RankInfo>
              <AttributeModifier>AttributeModifier\456</AttributeModifier>
            </HordeContain>
            """);
        XmlNamespaceManager namespaces = new(document.NameTable);
        namespaces.AddNamespace("ea", "uri:ea.com:eala:asset");
        Node node = new(document.CreateNavigator()!.SelectSingleNode("/ea:HordeContain", namespaces)!, namespaces);
        HordeContainModuleData* root;
        using Tracker tracker = new((void**)&root, (uint)sizeof(HordeContainModuleData), false);
        Marshaler.Marshal(node, root, tracker);
        Chunk chunk = new();
        tracker.MakeRelocatable(chunk);
        ReadOnlySpan<byte> bytes = chunk.InstanceBuffer;
        Expect(bytes.Length == 592, "Horde instance bytes", 592, bytes.Length);
        Expect(ReadUInt32(bytes, 364) == 0x42700000, "Horde default leash", 0x42700000, ReadUInt32(bytes, 364));
        Expect(ReadUInt32(bytes, 372) == 124, "Horde EVA token", 124, ReadUInt32(bytes, 372)); // Reborn: final import indices reserve zero for null.
        Expect(ReadUInt32(bytes, 424) == 2 && bytes[520] == 1 && bytes[521] == 0, "Horde status and defaults", 2, ReadUInt32(bytes, 424));
        Expect(ReadUInt32(bytes, 460) == 2 && ReadUInt32(bytes, 464) == 524, "Horde rank list", 524, ReadUInt32(bytes, 464));
        Expect(ReadUInt32(bytes, 524) == 1 && ReadUInt32(bytes, 540) == 2, "Horde rank stride", 2, ReadUInt32(bytes, 540));
        uint unitId = FastHash.GetHashCode("testranktwo");
        Expect(ReadUInt32(bytes, 544) == unitId, "Horde rank weak ID", unchecked((int)unitId), ReadUInt32(bytes, 544));
        Expect(ReadUInt32(bytes, 536) == 556 && ReadUInt32(bytes, 552) == 572, "Horde nested positions", 572, ReadUInt32(bytes, 552));
        Expect(ReadUInt32(bytes, 556) == 0x40000000 && ReadUInt32(bytes, 576) == 0x40400000, "Horde position values", 0x40400000, ReadUInt32(bytes, 576));
        Expect(ReadUInt32(bytes, 564) == uint.MaxValue && ReadUInt32(bytes, 580) == uint.MaxValue, "Horde default leader rank", -1, ReadUInt32(bytes, 580));
        Expect(ReadUInt32(bytes, 516) == 588 && ReadUInt32(bytes, 588) == 457, "Horde modifier reference", 457, ReadUInt32(bytes, 588)); // Reborn: bias the import value, not its pointer.
        Expect(chunk.RelocationBuffer.Length == 20 && chunk.ImportsBuffer.Length == 12, "Horde relocation and imports", 20, chunk.RelocationBuffer.Length);
        Expect(ReadUInt32(chunk.ImportsBuffer, 0) == 372 && ReadUInt32(chunk.ImportsBuffer, 4) == 588, "Horde import slots", 588, ReadUInt32(chunk.ImportsBuffer, 4));
        Console.WriteLine($"  HordeContain bin={chunk.InstanceBuffer.Length}, relo={chunk.RelocationBuffer.Length}, imp={chunk.ImportsBuffer.Length}");
    }


    //-------------------------------------------------------------------------------------------------
    /** Reborn: verify actual schema defaults and dependency indices before native marshalling. */
    //-------------------------------------------------------------------------------------------------
    private static void TestReferencePipeline()
    {
        ReferencePipelineSmokeTest.Run(ReferencePipelineSmokeTest.FindFixture());
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: regression-check evidence precedence and the EP1-only audit target boundary. */
    //-------------------------------------------------------------------------------------------------
    private static void TestTypeRegistryAudit()
    {
        if (TypeRegistryAudit.Classify(2, true, true, true) != "conflicting-evidence"
            || TypeRegistryAudit.Classify(1, false, true, true) != "identity-mismatch"
            || TypeRegistryAudit.Classify(1, true, false, true) != "unregistered"
            || TypeRegistryAudit.Classify(1, true, true, false) != "type-hash-mismatch"
            || TypeRegistryAudit.Classify(1, true, true, true) != "hash-match-only")
            throw new InvalidOperationException("Type registry audit classification regression.");
        TypeRegistryAudit.ValidateTarget(7, 0x5454A8E9u);
        foreach (var target in new[] { (Version: (ushort)6, Hash: 0x5454A8E9u), (Version: (ushort)7, Hash: 0x12B3E763u) })
        {
            bool rejected = false;
            try { TypeRegistryAudit.ValidateTarget(target.Version, target.Hash); }
            catch (InvalidDataException) { rejected = true; }
            if (!rejected) throw new InvalidOperationException("Wrong-game type audit input was accepted.");
        }
        Console.WriteLine("PASS EP1 type registry audit evidence and target guards");
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: compiler regression tests include armor tokens without enabling the production EP1 registry. */
    //-------------------------------------------------------------------------------------------------
    private static void TestArmorTokenPipeline()
    {
        ArmorTokenSmokeTest.Run(Path.Combine(Path.GetTempPath(), "Reborn-Ep1ArmorToken-" + Guid.NewGuid().ToString("N")));
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: exercise the real plugin path without allowing experimental profiles to commit production streams. */
    //-------------------------------------------------------------------------------------------------
    private static void TestEp1ArmorProfile()
    {
        Ep1ArmorProfileSmokeTest.Run();
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: document regression fixtures stay isolated from game files and production SDK output. */
    //-------------------------------------------------------------------------------------------------
    private static void TestEp1ArmorDocument()
    {
        Ep1ArmorDocumentSmokeTest.Run(Path.Combine(Path.GetTempPath(), "Reborn-Ep1ArmorDocument-" + Guid.NewGuid().ToString("N")));
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: verify intermediate commits, coordinated linking and policy guards without enabling an EP1 compiler profile. */
    //-------------------------------------------------------------------------------------------------
    private static void TestLinkedStreamRepair()
    {
        LinkedStreamSmokeTest.Run(Path.Combine(Path.GetTempPath(), "Reborn-Ep1LinkedStreams-" + Guid.NewGuid().ToString("N")));
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: retain official checksum/candidate behavior while preventing overclaims about cache or payload integrity. */
    //-------------------------------------------------------------------------------------------------
    private static void TestChecksumAudit()
    {
        ChecksumAudit.Run();
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: retain existing asset/custom-data bytes when validation, staging or replacement fails. */
    //-------------------------------------------------------------------------------------------------
    private static void TestCopyRecovery()
    {
        CopyRecoverySmokeTest.Run();
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: prove file-signature invalidation and focused full-document dependency identity handling. */
    //-------------------------------------------------------------------------------------------------
    private static void TestDependencyHashes()
    {
        DependencyHashSmokeTest.Run();
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: exercise resident cache snapshots, forced content refresh and synchronized watcher callbacks. */
    //-------------------------------------------------------------------------------------------------
    private static void TestWatcherCache()
    {
        WatcherCacheSmokeTest.Run();
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: verify immutable notification generations and the production builder handoff helper. */
    //-------------------------------------------------------------------------------------------------
    private static void TestMonitorBatch()
    {
        MonitorBatchSmokeTest.Run();
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: guard cached document reuse and source/dependency reload through retained and serialized sessions. */
    //-------------------------------------------------------------------------------------------------
    private static void TestDocumentReuse()
    {
        DocumentReuseSmokeTest.Run();
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: validate official schema defaults, mask/list ABI and deterministic native modifier output. */
    //-------------------------------------------------------------------------------------------------
    private static void TestAttributeModifierNative()
    {
        AttributeModifierNativeSmokeTest.Run(Array.Empty<string>());
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: exercise the explicit modifier profile and full document path without promoting production eligibility. */
    //-------------------------------------------------------------------------------------------------
    private static void TestEp1ModifierProfile()
    {
        Ep1AttributeModifierProfileSmokeTest.Run();
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: guard the shared one-biased import serializer using full normalized modifier documents. */
    //-------------------------------------------------------------------------------------------------
    private static void TestModifierImports()
    {
        AttributeModifierImportSmokeTest.Run(Array.Empty<string>());
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: exercise the production dependency preparation seam independently of experimental output authorization. */
    //-------------------------------------------------------------------------------------------------
    private static void TestDependencyResolution() => DependencyResolutionSmokeTest.Run();

    //-------------------------------------------------------------------------------------------------
    /** Reborn: add source/default/native filter coverage without requiring local game files in the regression suite. */
    //-------------------------------------------------------------------------------------------------
    private static void TestObjectFilterNative() => ObjectFilterNativeSmokeTest.Run(Array.Empty<string>());

    //-------------------------------------------------------------------------------------------------
    /** Reborn: exercise the bounded ObjectFilter descriptor/document profile and its fail-closed eligibility checks. */
    //-------------------------------------------------------------------------------------------------
    private static void TestObjectFilterProfile() => Ep1ObjectFilterProfileSmokeTest.Run();

    //-------------------------------------------------------------------------------------------------
    /** Reborn: validate shader ABI/default/document proof without requiring stock game data in the full regression suite. */
    //-------------------------------------------------------------------------------------------------
    private static void TestShaderOverrideNative() => ShaderOverrideNativeSmokeTest.Run(Array.Empty<string>());

    //-------------------------------------------------------------------------------------------------
    /** Reborn: verify the bounded shader profile independently of production registration or stock game-data availability. */
    //-------------------------------------------------------------------------------------------------
    private static void TestShaderOverrideProfile() => Ep1ShaderOverrideProfileSmokeTest.Run();

    //-------------------------------------------------------------------------------------------------
    /** Reborn: connect bounded EP1 compiler families without enabling output/cache reuse or invoking the production linker. */
    //-------------------------------------------------------------------------------------------------
    private static void TestModifierShaderGraph() => ModifierShaderGraphSmokeTest.Run();

    //-------------------------------------------------------------------------------------------------
    /** Reborn: exercise real include parsing, nested reload and external metadata refresh under closed production policies. */
    //-------------------------------------------------------------------------------------------------
    private static void TestIncludedModifierShader() => IncludedModifierShaderSmokeTest.Run();

    //-------------------------------------------------------------------------------------------------
    /** Reborn: verify mixed-family stream serialization separately from the blocked production linker and game runtime. */
    //-------------------------------------------------------------------------------------------------
    private static void TestModifierShaderStream() => ModifierShaderStreamSmokeTest.Run();

    //-------------------------------------------------------------------------------------------------
    /** Reborn: prove approved snapshots, output preservation and fail-closed publication for the limited diagnostic build entry. */
    //-------------------------------------------------------------------------------------------------
    private static void TestBoundedDiagnosticBuild() => BoundedDiagnosticBuildSmokeTest.Run();

    //-------------------------------------------------------------------------------------------------
    /** Reborn: prove bounded Include admission and native source attribution through the diagnostic command's real service. */
    //-------------------------------------------------------------------------------------------------
    private static void TestDiagnosticIncludeBuild() => DiagnosticIncludeBuildSmokeTest.Run();

    //-------------------------------------------------------------------------------------------------
    /** Reborn: prove native filter output and weak reference semantics through the public diagnostic build service. */
    //-------------------------------------------------------------------------------------------------
    private static void TestDiagnosticFilterBuild() => DiagnosticFilterBuildSmokeTest.Run();

    //-------------------------------------------------------------------------------------------------
    /** Reborn: verify optional FX base masks and source-normalized native chunks with production registration disabled. */
    //-------------------------------------------------------------------------------------------------
    private static void TestFXListNative() => FXListNativeSmokeTest.Run(Array.Empty<string>());

    //-------------------------------------------------------------------------------------------------
    /** Reborn: register native sound evidence independently from production audio processors. */
    //-------------------------------------------------------------------------------------------------
    private static void TestMultisoundNative() => MultisoundNativeSmokeTest.Run();

    //-------------------------------------------------------------------------------------------------
    /** Reborn: exercise isolated AudioEvent native evidence without registering legacy audio or codecs. */
    //-------------------------------------------------------------------------------------------------
    private static void TestAudioEventNative() => AudioEventNativeSmokeTest.Run();

    //-------------------------------------------------------------------------------------------------
    /** Reborn: register isolated checked sound profile evidence without activating production audio. */
    //-------------------------------------------------------------------------------------------------
    private static void TestMultisoundProfile() => Ep1MultisoundProfileSmokeTest.Run();

    //-------------------------------------------------------------------------------------------------
    /** Reborn: exercise isolated checked AudioEvent entries and prepared AudioFile identities. */
    //-------------------------------------------------------------------------------------------------
    private static void TestAudioEventProfile() => Ep1AudioEventProfileSmokeTest.Run();

    //-------------------------------------------------------------------------------------------------
    /** Reborn: exercise fixed local event/sound/FX streams without production or public admission. */
    //-------------------------------------------------------------------------------------------------
    private static void TestAudioEventFXStream() => AudioEventFXStreamSmokeTest.Run();

    //-------------------------------------------------------------------------------------------------
    /** Reborn: exercise the bounded AudioEvent command through the same service used by the CLI. */
    //-------------------------------------------------------------------------------------------------
    private static void TestDiagnosticAudioEventBuild() => DiagnosticAudioEventBuildSmokeTest.Run();

    //-------------------------------------------------------------------------------------------------
    /** Reborn: validate the recovered fixed AudioFile prefix and bounded optional native ranges. */
    //-------------------------------------------------------------------------------------------------
    private static void TestAudioFileRuntime() => AudioFileRuntimeProbe.SelfTest();

    //-------------------------------------------------------------------------------------------------
    /** Reborn: prove isolated EP1 AudioFile runtime serialization without enabling codecs or production asset entries. */
    //-------------------------------------------------------------------------------------------------
    private static void TestAudioFileSerialization() => AudioFileSerializationSmokeTest.Run();

    //-------------------------------------------------------------------------------------------------
    /** Reborn: test narrow authored audio input preparation without executing native codecs or enabling production output. */
    //-------------------------------------------------------------------------------------------------
    private static void TestAudioFileInput() => Ep1AudioFileInputSmokeTest.Run();

    //-------------------------------------------------------------------------------------------------
    /** Reborn: test bounded candidate PCM duration separately from the canonical production/core admission profile. */
    //-------------------------------------------------------------------------------------------------
    private static void TestAudioDurationCandidate() => AudioDurationCandidateSmokeTest.Run();

    //-------------------------------------------------------------------------------------------------
    /** Reborn: exercise explicit duration pool admission and independent managed worker result reconstruction. */
    //-------------------------------------------------------------------------------------------------
    private static void TestAudioDurationPool() => AudioDurationPoolSmokeTest.Run();

    //-------------------------------------------------------------------------------------------------
    /** Reborn: verify selected duration fingerprints and mixed package tables with real core processing and synthetic audio. */
    //-------------------------------------------------------------------------------------------------
    private static void TestAudioDurationEvent() => AudioDurationEventSmokeTest.Run();

    //-------------------------------------------------------------------------------------------------
    /** Reborn: prove target-aware read-only SDK path/schema/manifest environment admission independently of production build readiness. */
    //-------------------------------------------------------------------------------------------------
    private static void TestSdkEnvironmentPreflight() => SdkEnvironmentPreflightSmokeTest.Run();

    //-------------------------------------------------------------------------------------------------
    /** Reborn: register the separate bounded source path audit in the measured compiler regression inventory. */
    //-------------------------------------------------------------------------------------------------
    private static void TestSdkSourcePathAudit() => SdkSourcePathAuditSmokeTest.Run();

    //-------------------------------------------------------------------------------------------------
    /** Reborn: count and execute staged file-dependency declaration regressions without validating or compiling source instances. */
    //-------------------------------------------------------------------------------------------------
    private static void TestSdkFileReferenceCatalog() => SdkFileReferenceCatalogSmokeTest.Run();

    //-------------------------------------------------------------------------------------------------
    /** Reborn: execute effective schema/inherited source regressions independently of production processors and game loading. */
    //-------------------------------------------------------------------------------------------------
    private static void TestSdkEffectiveSchema() => SdkEffectiveSchemaSmokeTest.Run();

    //-------------------------------------------------------------------------------------------------
    /** Reborn: test the explicit Shield schema candidate without altering the default strict/reference gate. */
    //-------------------------------------------------------------------------------------------------
    private static void TestSdkShieldSchemaCandidate() => SdkShieldSchemaCandidateSmokeTest.Run();

    //-------------------------------------------------------------------------------------------------
    /** Reborn: test explicit fingerprint/probe-based schema warning admission separately from clean/default schema compilation. */
    //-------------------------------------------------------------------------------------------------
    private static void TestSdkSchemaHookReview() => SdkSchemaHookReviewSmokeTest.Run();

    //-------------------------------------------------------------------------------------------------
    /** Reborn: execute typed graph snapshot/resource/preprocessing regressions without production compilation. */
    //-------------------------------------------------------------------------------------------------
    private static void TestSdkTypedSourceGraph() => SdkTypedSourceGraphSmokeTest.Run();

    //-------------------------------------------------------------------------------------------------
    /** Reborn: exercise explicit bounded local literal preprocessing separately from core/EA expression evaluation. */
    //-------------------------------------------------------------------------------------------------
    private static void TestSdkLocalDefineProfile() => SdkLocalDefineProfileSmokeTest.Run();

    //-------------------------------------------------------------------------------------------------
    /** Reborn: test source-backed literal Include visibility and origin-aware duplicates without production evaluator/native execution. */
    //-------------------------------------------------------------------------------------------------
    private static void TestSdkIncludeDefineProfile() => SdkIncludeDefineProfileSmokeTest.Run();

    //-------------------------------------------------------------------------------------------------
    /** Reborn: test the separately admitted definition subset without broadening literal profiles or executing the reference evaluator DLL. */
    //-------------------------------------------------------------------------------------------------
    private static void TestSdkDefinitionSubset() => SdkDefinitionSubsetSmokeTest.Run();

    //-------------------------------------------------------------------------------------------------
    /** Reborn: execute the core-backed local leaf overlay subset tests without enabling production or imported inheritance. */
    //-------------------------------------------------------------------------------------------------
    private static void TestSdkSelfAttributeInheritance() => SdkSelfAttributeInheritanceSmokeTest.Run();

    //-------------------------------------------------------------------------------------------------
    /** Reborn: verify one-sided flat child copying separately from the original local leaf profile and broader core child merges. */
    //-------------------------------------------------------------------------------------------------
    private static void TestSdkSelfChildCopy() => SdkSelfChildCopySmokeTest.Run();

    //-------------------------------------------------------------------------------------------------
    /** Reborn: execute complex leaf copy admission without expanding flat/attribute-only profiles. */
    //-------------------------------------------------------------------------------------------------
    private static void TestSdkSelfComplexChildCopy() => SdkSelfComplexChildCopySmokeTest.Run();

    //-------------------------------------------------------------------------------------------------
    /** Reborn: verify bounded recursive copying and original-profile isolation without native processor execution. */
    //-------------------------------------------------------------------------------------------------
    private static void TestSdkSelfTreeCopy() => SdkSelfTreeCopySmokeTest.Run();

    //-------------------------------------------------------------------------------------------------
    /** Reborn: verify empty-child matching, anonymous append and two-sided scope guards without native compilation. */
    //-------------------------------------------------------------------------------------------------
    private static void TestSdkSelfChildMerge() => SdkSelfChildMergeSmokeTest.Run();

    //-------------------------------------------------------------------------------------------------
    /** Reborn: exercise narrow direct-instance XML inheritance without loading native processors or publishing streams. */
    //-------------------------------------------------------------------------------------------------
    private static void TestSdkInstanceInheritance() => SdkInstanceInheritanceSmokeTest.Run();

    //-------------------------------------------------------------------------------------------------
    /** Reborn: prove inherited alias roots and owned-field overrides remain confined and distinct from native/cache preparation. */
    //-------------------------------------------------------------------------------------------------
    private static void TestSdkInstanceRootFiles() => SdkInstanceRootFilesSmokeTest.Run();

    //-------------------------------------------------------------------------------------------------
    /** Reborn: verify direct-definition eligibility, source closures, cache/depth limits and stale/cyclic source rejection without native compilation. */
    //-------------------------------------------------------------------------------------------------
    private static void TestSdkInstanceChains() => SdkInstanceChainsSmokeTest.Run();

    //-------------------------------------------------------------------------------------------------
    /** Reborn: verify strict removal identity, source closures and post-removal validation without native processors. */
    //-------------------------------------------------------------------------------------------------
    private static void TestSdkInstanceRemovals() => SdkInstanceRemovalsSmokeTest.Run();

    //-------------------------------------------------------------------------------------------------
    /** Reborn: pin core choice cardinality/identity behavior before admitting bounded choice trees. */
    //-------------------------------------------------------------------------------------------------
    private static void TestSdkChoiceSemantics() => SdkChoiceSemanticsSmokeTest.Run();

    //-------------------------------------------------------------------------------------------------
    /** Reborn: prove nested repeated-choice copying and strict preservation of all earlier profile boundaries. */
    //-------------------------------------------------------------------------------------------------
    private static void TestSdkInstanceChoices() => SdkInstanceChoicesSmokeTest.Run();

    //-------------------------------------------------------------------------------------------------
    /** Reborn: prove declaration-time marker consumption and retain the imported non-inheritable gate. */
    //-------------------------------------------------------------------------------------------------
    private static void TestSdkInstanceMarkers() => SdkInstanceMarkersSmokeTest.Run();

    //-------------------------------------------------------------------------------------------------
    /** Reborn: pin enum-list token identity, ordered modifications and unchanged core substring hazards. */
    //-------------------------------------------------------------------------------------------------
    private static void TestSdkInstanceBitflags() => SdkInstanceBitflagsSmokeTest.Run();

    //-------------------------------------------------------------------------------------------------
    /** Reborn: pin ordered weak-reference copying, bounded refusal and final schema binding for matched micromanager filters. */
    //-------------------------------------------------------------------------------------------------
    private static void TestSdkInstanceFilters() => SdkInstanceFiltersSmokeTest.Run();

    //-------------------------------------------------------------------------------------------------
    /** Reborn: pin duplicate singleton upgrade requirements and validation-hidden conflicts before normalization admission. */
    //-------------------------------------------------------------------------------------------------
    private static void TestSdkUpgradeSemantics() => SdkUpgradeSemanticsSmokeTest.Run();

    //-------------------------------------------------------------------------------------------------
    /** Reborn: prove bounded upgrade normalization and unchanged earlier profile refusals. */
    //-------------------------------------------------------------------------------------------------
    private static void TestSdkInstanceUpgrades() => SdkInstanceUpgradesSmokeTest.Run();

    //-------------------------------------------------------------------------------------------------
    /** Reborn: test bounded metadata Include authority and unchanged expression/inheritance boundaries. */
    //-------------------------------------------------------------------------------------------------
    private static void TestSdkInstanceMetadata() => SdkInstanceMetadataSmokeTest.Run();

    //-------------------------------------------------------------------------------------------------
    /** Reborn: prove defining-context expression substitution precedes guarded overlays without widening older profiles. */
    //-------------------------------------------------------------------------------------------------
    private static void TestSdkInstanceExpressions() => SdkInstanceExpressionsSmokeTest.Run();

    //-------------------------------------------------------------------------------------------------
    /** Reborn: pin repeated-key loss and ordered command semantics before a separate source-bound normalization scope. */
    //-------------------------------------------------------------------------------------------------
    private static void TestSdkSiblingIdentitySemantics() => SdkSiblingIdentitySemanticsSmokeTest.Run();

    //-------------------------------------------------------------------------------------------------
    /** Reborn: prove bounded identical-state folding with source-local evidence and unchanged earlier refusals. */
    //-------------------------------------------------------------------------------------------------
    private static void TestSdkInstanceIdenticalStates() => SdkInstanceIdenticalStatesSmokeTest.Run();

    //-------------------------------------------------------------------------------------------------
    /** Reborn: prove ordered literal state commands against resolved base fields and actual core positions. */
    //-------------------------------------------------------------------------------------------------
    private static void TestSdkInstanceStateReadds() => SdkInstanceStateReaddsSmokeTest.Run();

    //-------------------------------------------------------------------------------------------------
    /** Reborn: prove native metadata identity boundaries while retaining existing destructive-operation refusal tests. */
    //-------------------------------------------------------------------------------------------------
    private static void TestSdkCc32Review() => SdkCc32ReviewSmokeTest.Run();

    //-------------------------------------------------------------------------------------------------
    /** Reborn: prove exact reference-bearing state removal and atomic refusal of every broader operation. */
    //-------------------------------------------------------------------------------------------------
    private static void TestSdkInstanceCrossStateRemovals() => SdkInstanceCrossStateRemovalsSmokeTest.Run();

    //-------------------------------------------------------------------------------------------------
    /** Reborn: exercise music-only integer offsets, source closure, atomic refusals and final schema binding. */
    //-------------------------------------------------------------------------------------------------
    private static void TestSdkInstanceMusicOffsets() => SdkInstanceMusicOffsetsSmokeTest.Run();

    //-------------------------------------------------------------------------------------------------
    /** Reborn: exercise bounded broad audio owners, actual Core merges and atomic resource failures. */
    //-------------------------------------------------------------------------------------------------
    private static void TestSdkInstanceAudioTrees() => SdkInstanceAudioTreesSmokeTest.Run();

    //-------------------------------------------------------------------------------------------------
    /** Reborn: exercise typed sound arithmetic, actual Core results and atomic source failures. */
    //-------------------------------------------------------------------------------------------------
    private static void TestSdkInstanceSoundOffsets() => SdkInstanceSoundOffsetsSmokeTest.Run();

    //-------------------------------------------------------------------------------------------------
    /** Reborn: execute sound singleton order/retention evidence separately from normalization or native proof. */
    //-------------------------------------------------------------------------------------------------
    private static void TestSdkSoundSingletonSemantics() => SdkSoundSingletonSemanticsSmokeTest.Run();

    //-------------------------------------------------------------------------------------------------
    /** Reborn: register full isolated owner field/child projection and tamper refusals without native execution. */
    //-------------------------------------------------------------------------------------------------
    private static void TestSdkSoundOwnerReview() => SdkSoundOwnerReviewSmokeTest.Run();

    //-------------------------------------------------------------------------------------------------
    /** Reborn: verify separately admitted known sound bodies while preserving all earlier profile refusals. */
    //-------------------------------------------------------------------------------------------------
    private static void TestSdkInstanceSoundSingletons() => SdkInstanceSoundSingletonsSmokeTest.Run();

    //-------------------------------------------------------------------------------------------------
    /** Reborn: register distinct dependency/root/alias evidence tests without external source/game requirements. */
    //-------------------------------------------------------------------------------------------------
    private static void TestSdkDependencyReview() => SdkDependencyReviewSmokeTest.Run();

    //-------------------------------------------------------------------------------------------------
    /** Reborn: execute independently opted-in native alias identity and expanded source graph fixtures. */
    //-------------------------------------------------------------------------------------------------
    private static void TestSdkKnownMapAliases() => SdkKnownMapAliasesSmokeTest.Run();

    //-------------------------------------------------------------------------------------------------
    /** Reborn: register bounded stock music observations independently of source/header admission. */
    //-------------------------------------------------------------------------------------------------
    private static void TestPathMusicStockReview() => PathMusicStockReviewSmokeTest.Run();

    //-------------------------------------------------------------------------------------------------
    /** Reborn: register bounded literal and legacy-quirk diagnostic tests without external DLL execution. */
    //-------------------------------------------------------------------------------------------------
    private static void TestPathMusicHeaderSemantics() => PathMusicHeaderSemanticsSmokeTest.Run();

    //-------------------------------------------------------------------------------------------------
    /** Reborn: execute isolated header-to-native fixture proof without admitting general music compilation. */
    //-------------------------------------------------------------------------------------------------
    private static void TestPathMusicRuntimeProbe() => PathMusicRuntimeProbeSmokeTest.Run();

    //-------------------------------------------------------------------------------------------------
    /** Reborn: register bounded local music source/header identities and isolated preparation regression fixtures. */
    //-------------------------------------------------------------------------------------------------
    private static void TestPathMusicAuthoredSnapshot() => PathMusicAuthoredSnapshotSmokeTest.Run();

    //-------------------------------------------------------------------------------------------------
    /** Reborn: register diagnostic music package readback and no-overwrite/stale-input regressions. */
    //-------------------------------------------------------------------------------------------------
    private static void TestPathMusicPackage() => PathMusicPackageSmokeTest.Run();

    //-------------------------------------------------------------------------------------------------
    /** Reborn: register independent Core music XML/weak/file identity reconstruction and change-isolation tests. */
    //-------------------------------------------------------------------------------------------------
    private static void TestPathMusicCoreIdentity() => PathMusicCoreIdentitySmokeTest.Run();

    //-------------------------------------------------------------------------------------------------
    /** Reborn: register pinned static music processing metadata and RA3/EP1 identity mismatch regressions. */
    //-------------------------------------------------------------------------------------------------
    private static void TestPathMusicReferenceIdentity() => PathMusicReferenceIdentitySmokeTest.Run();

    //-------------------------------------------------------------------------------------------------
    /** Reborn: register immutable Core/native music binding and stale source/header provenance regressions. */
    //-------------------------------------------------------------------------------------------------
    private static void TestPathMusicCorePreparation() => PathMusicCorePreparationSmokeTest.Run();

    //-------------------------------------------------------------------------------------------------
    /** Reborn: register current-Core music publication, late failure and competing-output regressions. */
    //-------------------------------------------------------------------------------------------------
    private static void TestPathMusicCorePackage() => PathMusicCorePackageSmokeTest.Run();

    //-------------------------------------------------------------------------------------------------
    /** Reborn: register fresh music identity checksum padding, field selection and stale-input regressions. */
    //-------------------------------------------------------------------------------------------------
    private static void TestPathMusicCoreChecksum() => PathMusicCoreChecksumSmokeTest.Run();

    //-------------------------------------------------------------------------------------------------
    /** Reborn: register independent experimental music manifest identity/native readback and safe publication regressions. */
    //-------------------------------------------------------------------------------------------------
    private static void TestPathMusicExperimentalCore() => PathMusicExperimentalCoreSmokeTest.Run();

    //-------------------------------------------------------------------------------------------------
    /** Reborn: register real Core music preparation/dependency/native dispatch containment and mutation regressions. */
    //-------------------------------------------------------------------------------------------------
    private static void TestPathMusicControlledCompiler() => PathMusicControlledCompilerSmokeTest.Run();

    //-------------------------------------------------------------------------------------------------
    /** Reborn: register local nonzero metadata, actual selection/dependency preparation and failure containment proofs. */
    //-------------------------------------------------------------------------------------------------
    private static void TestPathMusicSelectedCompiler() => PathMusicSelectedCompilerSmokeTest.Run();

    //-------------------------------------------------------------------------------------------------
    /** Reborn: register selected v2 native package framing/readback, profile isolation and safe publication regressions. */
    //-------------------------------------------------------------------------------------------------
    private static void TestPathMusicSelectedPackage() => PathMusicSelectedPackageSmokeTest.Run();

    //-------------------------------------------------------------------------------------------------
    /** Reborn: prove fixed two-entry AudioFile packaging separately from production compiler admission. */
    //-------------------------------------------------------------------------------------------------
    private static void TestAudioFilePackage() => AudioFilePackageSmokeTest.Run();

    //-------------------------------------------------------------------------------------------------
    /** Reborn: test mixed local event/audio packaging while keeping general AudioFile compilation closed. */
    //-------------------------------------------------------------------------------------------------
    private static void TestLocalAudioPackage() => AudioFileLocalEventSmokeTest.Run();

    //-------------------------------------------------------------------------------------------------
    /** Reborn: verify XML/text identity boundary behavior independently of audio payload fingerprints. */
    //-------------------------------------------------------------------------------------------------
    private static void TestHashingWriterBoundary() => HashingWriterBoundarySmokeTest.Run();

    //-------------------------------------------------------------------------------------------------
    /** Reborn: verify actual core AudioFile identity against independent XML and file-hash reconstruction. */
    //-------------------------------------------------------------------------------------------------
    private static void TestAudioFileIdentity() => AudioFileIdentitySmokeTest.Run();

    //-------------------------------------------------------------------------------------------------
    /** Reborn: reject stale current disk/core data before isolated AudioFile encoding can be integrated. */
    //-------------------------------------------------------------------------------------------------
    private static void TestCoreAudioFilePreparation() => AudioFileCorePreparationSmokeTest.Run();

    //-------------------------------------------------------------------------------------------------
    /** Reborn: prove stale current-source/runtime rejection at the same gate used by optional native core encoding. */
    //-------------------------------------------------------------------------------------------------
    private static void TestCoreAudioPackageGate() => CoreAudioPackageGateSmokeTest.Run();

    //-------------------------------------------------------------------------------------------------
    /** Reborn: one failed close callback must not suppress other cleanup or leave handles eligible for repeated release. */
    //-------------------------------------------------------------------------------------------------
    private static void TestAudioEncoderCleanup() => AudioEncoderCleanupSmokeTest.Run();

    //-------------------------------------------------------------------------------------------------
    /** Reborn: zero exit is insufficient; worker protocol/timeout/path failures must never receive acceptance. */
    //-------------------------------------------------------------------------------------------------
    private static void TestAudioEncoderSupervisor() => AudioEncoderSupervisorSmokeTest.Run();

    //-------------------------------------------------------------------------------------------------
    /** Reborn: freeze bounded caller audio while preserving originals and rejecting unsupported settings/source changes. */
    //-------------------------------------------------------------------------------------------------
    private static void TestAuthoredAudioSnapshot() => AuthoredAudioSnapshotSmokeTest.Run();

    //-------------------------------------------------------------------------------------------------
    /** Reborn: validate explicitly listed variable audio sources and dependencies without codecs or streams. */
    //-------------------------------------------------------------------------------------------------
    private static void TestAuthoredAudioPool() => AuthoredAudioPoolSmokeTest.Run();

    //-------------------------------------------------------------------------------------------------
    /** Reborn: independently verify variable pool worker evidence and negative acceptance boundaries. */
    //-------------------------------------------------------------------------------------------------
    private static void TestAudioPoolWorker() => AudioPoolWorkerSmokeTest.Run();

    //-------------------------------------------------------------------------------------------------
    /** Reborn: validate variable leaf-only package cardinality/order and both linked stream readers. */
    //-------------------------------------------------------------------------------------------------
    private static void TestAudioPoolPackage() => AudioPoolPackageSmokeTest.Run();

    //-------------------------------------------------------------------------------------------------
    /** Reborn: prove actual core event selection against a bounded variable pool without mixed publication. */
    //-------------------------------------------------------------------------------------------------
    private static void TestAudioPoolEvent() => AudioPoolEventSmokeTest.Run();

    //-------------------------------------------------------------------------------------------------
    /** Reborn: test mixed linked tables and independent parent acceptance without invoking native codecs. */
    //-------------------------------------------------------------------------------------------------
    private static void TestAudioPoolMixed() => AudioPoolEventSmokeTest.MixedRun();

    //-------------------------------------------------------------------------------------------------
    /** Reborn: prove immutable 1..8 selection vectors and dynamic event wire/import tables. */
    //-------------------------------------------------------------------------------------------------
    private static void TestAudioEventVectors() => AudioEventVectorSmokeTest.Run();

    //-------------------------------------------------------------------------------------------------
    /** Reborn: guard custom block envelopes separately from native ABI and codec processing. */
    //-------------------------------------------------------------------------------------------------
    private static void TestAudioCustomData() => AudioCustomDataProbe.SelfTest();

    //-------------------------------------------------------------------------------------------------
    /** Reborn: validate chunked original-entry comparisons independently of game archive availability. */
    //-------------------------------------------------------------------------------------------------
    private static void TestAudioArchiveComparison() => AudioArchiveComparisonProbe.SelfTest();

    //-------------------------------------------------------------------------------------------------
    /** Reborn: pin native/reference PE API evidence without DLL initialization. */
    //-------------------------------------------------------------------------------------------------
    private static void TestNativeAudioApi() => NativeAudioApiProbe.SelfTest();

    //-------------------------------------------------------------------------------------------------
    /** Reborn: validate the owned encoder input golden without launching codec experiments. */
    //-------------------------------------------------------------------------------------------------
    private static void TestAudioEncoderWave() => AudioEncoderPoc.SelfTest();

    //-------------------------------------------------------------------------------------------------
    /** Reborn: register fixed local-sound stream evidence without widening production or diagnostic command admission. */
    //-------------------------------------------------------------------------------------------------
    private static void TestMultisoundFXStream() => MultisoundFXStreamSmokeTest.Run();

    //-------------------------------------------------------------------------------------------------
    /** Reborn: register public-service sound integration proof independently from native/profile/fixed-stream groups. */
    //-------------------------------------------------------------------------------------------------
    private static void TestDiagnosticMultisoundBuild() => DiagnosticMultisoundBuildSmokeTest.Run();

    //-------------------------------------------------------------------------------------------------
    /** Reborn: verify actual external derived resolution without registering a runnable FX/audio processor. */
    //-------------------------------------------------------------------------------------------------
    private static void TestFXAudioResolution() => FXAudioResolutionSmokeTest.Run(Array.Empty<string>());

    //-------------------------------------------------------------------------------------------------
    /** Reborn: verify isolated FX descriptor/document/compiler policy and tamper rejection. */
    //-------------------------------------------------------------------------------------------------
    private static void TestFXProfile() => Ep1FXListProfileSmokeTest.Run();

    //-------------------------------------------------------------------------------------------------
    /** Reborn: exercise actual modifier/FX compiler graph serialization and independent native identity readback. */
    //-------------------------------------------------------------------------------------------------
    private static void TestModifierFXStream() => ModifierFXStreamSmokeTest.Run();

    //-------------------------------------------------------------------------------------------------
    /** Reborn: prove four-family Include/native/dependency/publication behavior through the actual diagnostic command service. */
    //-------------------------------------------------------------------------------------------------
    private static void TestDiagnosticFXBuild() => DiagnosticFXBuildSmokeTest.Run();

    //-------------------------------------------------------------------------------------------------
    /** Reborn: validate external linking with isolated tiny metadata fixtures, never game BIN streams. */
    //-------------------------------------------------------------------------------------------------
    private static void TestExternalManifestLinks()
    {
        ExternalLinkSmokeTest.Run(Path.Combine(Path.GetTempPath(), "Reborn-Ep1ExternalLink-" + Guid.NewGuid().ToString("N")));
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: check recovered infiltrator offsets using already-normalized strong references. */
    //-------------------------------------------------------------------------------------------------
    private static unsafe void TestInfiltratorContain()
    {
        XmlDocument document = new();
        document.LoadXml("""
            <InfiltratorContain xmlns="uri:ea.com:eala:asset"
              CanEnterFilter="ObjectFilterAsset\123" ReplaceWith="ObjectCreationList\234"
              EvaEventForInfiltratingPlayer="EvaEvent\345" EvaEventForInfiltratedPlayer="EvaEvent\456"
              FXForInfiltrate="FXList\567" UnitFilter="ObjectFilterAsset\678"
              StructureFilter="ObjectFilterAsset\789" Effect="KILL"
              ObjectRef="TestInfiltrator" ImmediatelyEnabled="true" />
            """);
        XmlNamespaceManager namespaces = new(document.NameTable);
        namespaces.AddNamespace("ea", "uri:ea.com:eala:asset");
        Node node = new(document.CreateNavigator()!.SelectSingleNode("/ea:InfiltratorContain", namespaces)!, namespaces);
        InfiltratorContainModuleData* root;
        using Tracker tracker = new((void**)&root, (uint)sizeof(InfiltratorContainModuleData), false);
        // Reborn: reproduce the uppercase-I weak-ID regression regardless of the host's current language.
        System.Globalization.CultureInfo previousCulture = System.Globalization.CultureInfo.CurrentCulture;
        try
        {
            System.Globalization.CultureInfo.CurrentCulture = System.Globalization.CultureInfo.GetCultureInfo("tr-TR");
            Marshaler.Marshal(node, root, tracker);
        }
        finally
        {
            System.Globalization.CultureInfo.CurrentCulture = previousCulture;
        }
        Chunk chunk = new();
        tracker.MakeRelocatable(chunk);
        ReadOnlySpan<byte> bytes = chunk.InstanceBuffer;
        Expect(bytes.Length == 144, "Infiltrator bytes", 144, bytes.Length);
        Expect(ReadUInt32(bytes, 8) == 0x40000000, "Infiltrator blocked duration", 0x40000000, ReadUInt32(bytes, 8));
        Expect(ReadUInt32(bytes, 44) == 124 && ReadUInt32(bytes, 48) == 235, "Infiltrator inline references", 235, ReadUInt32(bytes, 48)); // Reborn: final imports are one-biased.
        Expect(ReadUInt32(bytes, 52) == FastHash.GetHashCode("VoiceInfiltrate"), "Infiltrator voice hash", unchecked((int)FastHash.GetHashCode("VoiceInfiltrate")), ReadUInt32(bytes, 52));
        Expect(ReadUInt32(bytes, 60) == 346 && ReadUInt32(bytes, 64) == 457 && ReadUInt32(bytes, 68) == 568, "Infiltrator EVA and FX references", 568, ReadUInt32(bytes, 68)); // Reborn: final imports are one-biased.
        Expect(ReadUInt32(bytes, 84) == 8, "Infiltrator effect ordering", 8, ReadUInt32(bytes, 84));
        Expect(ReadUInt32(bytes, 88) == FastHash.GetHashCode("testinfiltrator"), "Infiltrator weak ID", unchecked((int)FastHash.GetHashCode("testinfiltrator")), ReadUInt32(bytes, 88));
        Expect(ReadUInt32(bytes, 92) == 136 && ReadUInt32(bytes, 96) == 140, "Infiltrator optional reference pointers", 140, ReadUInt32(bytes, 96));
        Expect(ReadUInt32(bytes, 136) == 679 && ReadUInt32(bytes, 140) == 790 && bytes[132] == 1, "Infiltrator pointed references and enable", 790, ReadUInt32(bytes, 140)); // Reborn: bias pointed import values only.
        int noRefund = (int)ObjectStatusType.NO_REFUND;
        Expect((ReadUInt32(bytes, 100 + noRefund / 32 * 4) & (1u << (noRefund % 32))) != 0, "Infiltrator default NO_REFUND", 1, 1);
        Expect(chunk.RelocationBuffer.Length == 12 && chunk.ImportsBuffer.Length == 32, "Infiltrator tables", 32, chunk.ImportsBuffer.Length);
        uint[] imports = [44, 48, 60, 64, 68, 136, 140];
        for (int index = 0; index < imports.Length; index++)
        {
            Expect(ReadUInt32(chunk.ImportsBuffer, index * 4) == imports[index], "Infiltrator import slot", (int)imports[index], ReadUInt32(chunk.ImportsBuffer, index * 4));
        }
        Console.WriteLine($"  InfiltratorContain={chunk.InstanceBuffer.Length}/{chunk.RelocationBuffer.Length}/{chunk.ImportsBuffer.Length}");
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: validate particle-list imports, optional laser allocations and derived-module dispatch. */
    //-------------------------------------------------------------------------------------------------
    private static unsafe void TestLaserStateFamily()
    {
        XmlDocument document = new();
        document.LoadXml("""
            <LaserState xmlns="uri:ea.com:eala:asset" LaserId="7" OriginBoneName="A">
              <LaserEndParticleSystem>FXParticleSystemTemplate\123</LaserEndParticleSystem>
              <LaserStartParticleSystem>FXParticleSystemTemplate\456</LaserStartParticleSystem>
              <EndOffset x="1" y="2" z="3" />
              <ObjectStatusValidation />
            </LaserState>
            """);
        XmlNamespaceManager namespaces = new(document.NameTable);
        namespaces.AddNamespace("ea", "uri:ea.com:eala:asset");
        Node node = new(document.CreateNavigator()!.SelectSingleNode("/ea:LaserState", namespaces)!, namespaces);
        LaserStateModuleData* root;
        using Tracker tracker = new((void**)&root, (uint)sizeof(LaserStateModuleData), false);
        Marshaler.Marshal(node, root, tracker);
        Chunk chunk = new();
        tracker.MakeRelocatable(chunk);
        ReadOnlySpan<byte> bytes = chunk.InstanceBuffer;
        Expect(bytes.Length == 136, "Laser populated bytes", 136, bytes.Length);
        Expect(ReadUInt32(bytes, 8) == 7 && ReadUInt32(bytes, 16) == 48 && bytes[48] == (byte)'A', "Laser ID and bone", 48, ReadUInt32(bytes, 16));
        Expect(ReadUInt32(bytes, 20) == 1 && ReadUInt32(bytes, 24) == 52 && ReadUInt32(bytes, 52) == 124, "Laser end list", 124, ReadUInt32(bytes, 52)); // Reborn: preserve list offsets while biasing imports.
        Expect(ReadUInt32(bytes, 28) == 1 && ReadUInt32(bytes, 32) == 56 && ReadUInt32(bytes, 56) == 457, "Laser start list", 457, ReadUInt32(bytes, 56)); // Reborn: preserve list offsets while biasing imports.
        Expect(ReadUInt32(bytes, 36) == 60 && ReadUInt32(bytes, 40) == 72 && bytes[44] == 1, "Laser optional pointers and weapon default", 72, ReadUInt32(bytes, 40));
        Expect(ReadUInt32(bytes, 60) == 0x3F800000 && ReadUInt32(bytes, 68) == 0x40400000, "Laser vector", 0x40400000, ReadUInt32(bytes, 68));
        Expect(chunk.RelocationBuffer.Length == 24 && chunk.ImportsBuffer.Length == 12, "Laser populated tables", 24, chunk.RelocationBuffer.Length);
        Console.WriteLine($"  LaserState={chunk.InstanceBuffer.Length}/{chunk.RelocationBuffer.Length}/{chunk.ImportsBuffer.Length}");

        string[] fixtures =
        [
            """<SweepingLaserState xmlns="uri:ea.com:eala:asset" TypeId="0x3C934753" />""",
            """<ConvergingLaserState xmlns="uri:ea.com:eala:asset" TypeId="0x5F7498F9" />""",
            // Reborn: cover EP1 option removal, angle conversion and an explicit false weapon requirement.
            """<SweepingLaserState xmlns="uri:ea.com:eala:asset" TypeId="0x3C934753" RequiresWeapon="false" Angle="90d" SweepingLaserOptions="ALL -SWEEP_HORIZONTAL -SWEEP_VERTICAL -MOVE_BACK_AND_FORTH" />"""
        ];
        for (int index = 0; index < fixtures.Length; index++)
        {
            XmlDocument derivedDocument = new();
            derivedDocument.LoadXml(fixtures[index]);
            XmlNamespaceManager derivedNamespaces = new(derivedDocument.NameTable);
            derivedNamespaces.AddNamespace("ea", "uri:ea.com:eala:asset");
            Node derivedNode = new(derivedDocument.CreateNavigator()!.SelectSingleNode("/*", derivedNamespaces)!, derivedNamespaces);
            BehaviorModuleData** slot;
            using Tracker derivedTracker = new((void**)&slot, (uint)sizeof(BehaviorModuleData*), false);
            Marshaler.Marshal(derivedNode, slot, derivedTracker);
            Chunk derivedChunk = new();
            derivedTracker.MakeRelocatable(derivedChunk);
            int expected = index == 1 ? 144 : 80;
            Expect(derivedChunk.InstanceBuffer.Length == expected, "Laser derived bytes", expected, derivedChunk.InstanceBuffer.Length);
            int expectedWeapon = index == 2 ? 0 : 1;
            Expect(derivedChunk.InstanceBuffer[48] == expectedWeapon, "Laser derived weapon requirement", expectedWeapon, derivedChunk.InstanceBuffer[48]);
            int radiusOffset = index == 1 ? 60 : 52;
            Expect(ReadUInt32(derivedChunk.InstanceBuffer, radiusOffset) == 0x41200000, "Laser radius default", 0x41200000, ReadUInt32(derivedChunk.InstanceBuffer, radiusOffset));
            if (index != 1)
            {
                uint expectedOptions = index == 2 ? 8u : 5u;
                Expect(ReadUInt32(derivedChunk.InstanceBuffer, 76) == expectedOptions, "Sweeping options", (int)expectedOptions, ReadUInt32(derivedChunk.InstanceBuffer, 76));
                if (index == 2)
                {
                    float angle = BitConverter.UInt32BitsToSingle(ReadUInt32(derivedChunk.InstanceBuffer, 56));
                    Expect(Math.Abs(angle - MathF.PI / 2f) < 0.000001f, "Sweeping angle radians", 1, 1);
                }
            }
            else
            {
                Expect(ReadUInt32(derivedChunk.InstanceBuffer, 140) == 0x3F800000, "Converging lifetime default", 0x3F800000, ReadUInt32(derivedChunk.InstanceBuffer, 140));
            }
            Expect(derivedChunk.RelocationBuffer.Length == 8 && derivedChunk.ImportsBuffer.Length == 0, "Laser derived tables", 8, derivedChunk.RelocationBuffer.Length);
            Console.WriteLine($"  Laser derived{index}={derivedChunk.InstanceBuffer.Length}/{derivedChunk.RelocationBuffer.Length}/{derivedChunk.ImportsBuffer.Length}");
        }
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: Verify production queue native dispatch and eight-byte template records with optional filters. */
    //-------------------------------------------------------------------------------------------------
    private static unsafe void TestProductionQueueHordeContain()
    {
        XmlDocument document = new();
        document.LoadXml("""
            <ProductionQueueHordeContain xmlns="uri:ea.com:eala:asset" TypeId="0xFE135CA0">
              <TemplateContainer Template="TestQueueOne"><ObjectFilter Rule="ALL" /></TemplateContainer>
              <TemplateContainer Template="TestQueueTwo" />
            </ProductionQueueHordeContain>
            """);
        XmlNamespaceManager namespaces = new(document.NameTable);
        namespaces.AddNamespace("ea", "uri:ea.com:eala:asset");
        Node node = new(document.CreateNavigator()!.SelectSingleNode("/ea:ProductionQueueHordeContain", namespaces)!, namespaces);
        BehaviorModuleData** slot;
        using Tracker tracker = new((void**)&slot, (uint)sizeof(BehaviorModuleData*), false);
        Marshaler.Marshal(node, slot, tracker);
        Chunk chunk = new();
        tracker.MakeRelocatable(chunk);
        ReadOnlySpan<byte> bytes = chunk.InstanceBuffer;
        Expect(bytes.Length == 156, "ProductionQueue dispatch bytes", 156, bytes.Length);
        Expect(ReadUInt32(bytes, 0) == 4 && ReadUInt32(bytes, 4) == 0xFE135CA0, "ProductionQueue dispatch type", unchecked((int)0xFE135CA0), ReadUInt32(bytes, 4));
        Expect(ReadUInt32(bytes, 12) == 2 && ReadUInt32(bytes, 16) == 20, "ProductionQueue template list", 20, ReadUInt32(bytes, 16));
        uint firstId = FastHash.GetHashCode("testqueueone");
        uint secondId = FastHash.GetHashCode("testqueuetwo");
        Expect(ReadUInt32(bytes, 20) == firstId && ReadUInt32(bytes, 28) == secondId, "ProductionQueue weak IDs and stride", unchecked((int)secondId), ReadUInt32(bytes, 28));
        Expect(ReadUInt32(bytes, 24) == 36 && ReadUInt32(bytes, 32) == 0, "ProductionQueue optional filter pointers", 36, ReadUInt32(bytes, 24));
        Expect(ReadUInt32(bytes, 40) == 1, "ProductionQueue filter rule", 1, ReadUInt32(bytes, 40));
        Expect(chunk.RelocationBuffer.Length == 16 && chunk.ImportsBuffer.Length == 0, "ProductionQueue relocation and weak imports", 16, chunk.RelocationBuffer.Length);
        Expect(ReadUInt32(chunk.RelocationBuffer, 0) == 0 && ReadUInt32(chunk.RelocationBuffer, 4) == 16 && ReadUInt32(chunk.RelocationBuffer, 8) == 24, "ProductionQueue relocation slots", 24, ReadUInt32(chunk.RelocationBuffer, 8));
        Console.WriteLine($"  ProductionQueueHordeContain dispatch={chunk.InstanceBuffer.Length}/{chunk.RelocationBuffer.Length}/{chunk.ImportsBuffer.Length}");
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: Verify attach defaults and populated pointer/import records recovered from official RA3 IL. */
    //-------------------------------------------------------------------------------------------------
    private static unsafe void TestAttachUpdate()
    {
        string[] fixtures =
        [
            """<AttachUpdate xmlns="uri:ea.com:eala:asset" />""",
            """
            <AttachUpdate xmlns="uri:ea.com:eala:asset" ParentStatusToCopy="CAN_ATTACK" ParentStatusToPrefer="CAN_ATTACK"
                ParentOwnerAttachmentEvaEvent="EvaEvent\123" DetachFXList="FXList\456" Flags="USE_BONE_POSITION" AttachBoneName="A"
                DamageTypesToNotLeech="EXPLOSION" DeathTypesToNotLeech="NORMAL">
              <ObjectFilter Rule="ALL" />
              <ModifierToLeechFromParent>AttributeModifier\789</ModifierToLeechFromParent>
            </AttachUpdate>
            """
        ];
        for (int index = 0; index < fixtures.Length; index++)
        {
            XmlDocument document = new();
            document.LoadXml(fixtures[index]);
            XmlNamespaceManager namespaces = new(document.NameTable);
            namespaces.AddNamespace("ea", "uri:ea.com:eala:asset");
            Node node = new(document.CreateNavigator()!.SelectSingleNode("/ea:AttachUpdate", namespaces)!, namespaces);
            AttachUpdateModuleData* root;
            using Tracker tracker = new((void**)&root, (uint)sizeof(AttachUpdateModuleData), false);
            Marshaler.Marshal(node, root, tracker);
            Chunk chunk = new();
            tracker.MakeRelocatable(chunk);
            ReadOnlySpan<byte> bytes = chunk.InstanceBuffer;
            int expectedSize = index == 0 ? 368 : 576;
            Expect(bytes.Length == expectedSize, "Attach instance bytes", expectedSize, bytes.Length);
            uint expectedFlags = index == 0 ? 0x70u : 0x08000000u;
            Expect(ReadUInt32(bytes, 284) == expectedFlags, "Attach schema flag ordering", (int)expectedFlags, ReadUInt32(bytes, 284));
            Expect(ReadUInt32(bytes, 240) == 0x447A0000, "Attach default range", 0x447A0000, ReadUInt32(bytes, 240));
            Expect(ReadUInt32(bytes, 288) == (uint)DeathType.ALL && ReadUInt32(bytes, 344) == (uint)DeathType.NORMAL, "Attach death defaults", (int)DeathType.NORMAL, ReadUInt32(bytes, 344));
            if (index == 0)
            {
                Expect(chunk.RelocationBuffer.Length == 0 && chunk.ImportsBuffer.Length == 0, "Attach absent optional records", 0, chunk.RelocationBuffer.Length);
            }
            else
            {
                Expect(ReadUInt32(bytes, 228) == 368 && ReadUInt32(bytes, 232) == 400, "Attach status pointers", 400, ReadUInt32(bytes, 232));
                Expect(ReadUInt32(bytes, 368) == 2 && ReadUInt32(bytes, 400) == 2, "Attach pointed statuses", 2, ReadUInt32(bytes, 400));
                Expect(ReadUInt32(bytes, 336) == 432 && ReadUInt32(bytes, 340) == 440, "Attach leech mask pointers", 440, ReadUInt32(bytes, 340));
                Expect(ReadUInt32(bytes, 348) == 448 && ReadUInt32(bytes, 356) == 568, "Attach filter and modifier pointers", 568, ReadUInt32(bytes, 356));
                Expect(ReadUInt32(bytes, 244) == 124 && ReadUInt32(bytes, 260) == 457 && ReadUInt32(bytes, 568) == 790, "Attach imported tokens", 790, ReadUInt32(bytes, 568)); // Reborn: final BIN imports are one-biased.
                // Reborn: exercise EP1's added bone string as well as its high-order USE_BONE_POSITION flag.
                Expect(ReadUInt32(bytes, 364) == 572 && bytes[572] == (byte)'A', "Attach EP1 bone string", 572, ReadUInt32(bytes, 364));
                Expect(chunk.RelocationBuffer.Length == 32 && chunk.ImportsBuffer.Length == 16, "Attach pointer and import tables", 32, chunk.RelocationBuffer.Length);
            }
            Console.WriteLine($"  AttachUpdate fixture{index}={chunk.InstanceBuffer.Length}/{chunk.RelocationBuffer.Length}/{chunk.ImportsBuffer.Length}");
        }
        // Reborn: exercise the distinct leech dispatch hash with inherited defaults and no optional payloads.
        XmlDocument leechDocument = new();
        leechDocument.LoadXml("""<LeechTargetingAttachUpdate xmlns="uri:ea.com:eala:asset" TypeId="0xCA6038A6" />""");
        XmlNamespaceManager leechNamespaces = new(leechDocument.NameTable);
        leechNamespaces.AddNamespace("ea", "uri:ea.com:eala:asset");
        Node leechNode = new(leechDocument.CreateNavigator()!.SelectSingleNode("/ea:LeechTargetingAttachUpdate", leechNamespaces)!, leechNamespaces);
        BehaviorModuleData** slot;
        using Tracker leechTracker = new((void**)&slot, (uint)sizeof(BehaviorModuleData*), false);
        Marshaler.Marshal(leechNode, slot, leechTracker);
        Chunk leechChunk = new();
        leechTracker.MakeRelocatable(leechChunk);
        Expect(leechChunk.InstanceBuffer.Length == 372, "Leech dispatch instance bytes", 372, leechChunk.InstanceBuffer.Length);
        Expect(ReadUInt32(leechChunk.InstanceBuffer, 4) == 0xCA6038A6, "Leech dispatch type hash", unchecked((int)0xCA6038A6), ReadUInt32(leechChunk.InstanceBuffer, 4));
        Expect(ReadUInt32(leechChunk.InstanceBuffer, 288) == 0x70, "Leech inherited flags", 0x70, ReadUInt32(leechChunk.InstanceBuffer, 288));
        Expect(leechChunk.RelocationBuffer.Length == 8 && leechChunk.ImportsBuffer.Length == 0, "Leech dispatch pointer only", 8, leechChunk.RelocationBuffer.Length);
        Console.WriteLine($"  LeechTargetingAttachUpdate dispatch={leechChunk.InstanceBuffer.Length}/8/0");
        // Reborn: verify MoneyGain's separate validation block and default 100% purchase-price fraction.
        leechDocument.LoadXml("""
            <MoneyGainAttachUpdate xmlns="uri:ea.com:eala:asset" TypeId="0xB1A54585" ActionType="ON_DETACH_PARENT_DEAD">
              <MoneyGainObjectStatusValidation RequiredStatus="CAN_ATTACK" />
            </MoneyGainAttachUpdate>
            """);
        Node moneyNode = new(leechDocument.CreateNavigator()!.SelectSingleNode("/ea:MoneyGainAttachUpdate", leechNamespaces)!, leechNamespaces);
        BehaviorModuleData** moneySlot;
        using Tracker moneyTracker = new((void**)&moneySlot, (uint)sizeof(BehaviorModuleData*), false);
        Marshaler.Marshal(moneyNode, moneySlot, moneyTracker);
        Chunk moneyChunk = new();
        moneyTracker.MakeRelocatable(moneyChunk);
        Expect(moneyChunk.InstanceBuffer.Length == 448, "MoneyGain dispatch bytes", 448, moneyChunk.InstanceBuffer.Length);
        Expect(ReadUInt32(moneyChunk.InstanceBuffer, 4) == 0xB1A54585, "MoneyGain type hash", unchecked((int)0xB1A54585), ReadUInt32(moneyChunk.InstanceBuffer, 4));
        Expect(ReadUInt32(moneyChunk.InstanceBuffer, 376) == 0x3F800000, "MoneyGain default price fraction", 0x3F800000, ReadUInt32(moneyChunk.InstanceBuffer, 376));
        Expect(ReadUInt32(moneyChunk.InstanceBuffer, 380) == 384 && ReadUInt32(moneyChunk.InstanceBuffer, 416) == 2, "MoneyGain validation pointer and required status", 384, ReadUInt32(moneyChunk.InstanceBuffer, 380));
        Expect(moneyChunk.RelocationBuffer.Length == 12 && moneyChunk.ImportsBuffer.Length == 0, "MoneyGain relocation bytes", 12, moneyChunk.RelocationBuffer.Length);
        Console.WriteLine($"  MoneyGainAttachUpdate dispatch={moneyChunk.InstanceBuffer.Length}/12/0");
    }

    private static uint ReadUInt32(ReadOnlySpan<byte> bytes, int offset) =>
        BinaryPrimitives.ReadUInt32LittleEndian(bytes.Slice(offset, sizeof(uint)));

    private static void Expect(bool condition, string name, int expected, long actual)
    {
        if (!condition)
        {
            throw new InvalidDataException($"{name}: expected {expected}, got {actual}.");
        }
    }
}
