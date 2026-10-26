# Reborn: batch independent offline compiler/source/authoring checks; failures are collected, never converted to playable feature claims.
[CmdletBinding()]
param(
    [Parameter(Mandatory)][string]$OutputDirectory,
    [string]$StaticManifest,
    [string]$GlobalManifest,
    # Reborn: complete official scripted leaves are optional external read-only inputs, not embedded machine-specific source paths.
    [string]$UprisingXmlRoot,
    [ValidateRange(1,4)][int]$Workers=2,
    [switch]$Full,
    [switch]$SchedulerControl
)
$ErrorActionPreference='Stop'
if($PSVersionTable.PSVersion.Major-lt 7){throw 'PowerShell 7 is required.'}
$repo=Split-Path -Parent $PSScriptRoot
$OutputDirectory=[IO.Path]::GetFullPath($OutputDirectory)
if(Test-Path -LiteralPath $OutputDirectory){throw 'Batch output must be fresh; no existing artifacts are overwritten.'}
$parent=Split-Path -Parent $OutputDirectory
if(-not (Test-Path -LiteralPath $parent -PathType Container)){throw 'Batch output requires an existing parent.'}
for($item=Get-Item -LiteralPath $parent;$null-ne $item;$item=$item.Parent){
    if(($item.Attributes-band [IO.FileAttributes]::ReparsePoint)-ne 0){throw 'Redirected batch output refused.'}
}
if(-not $SchedulerControl){
    foreach($path in @($StaticManifest,$GlobalManifest)){
        if(-not $path -or -not [IO.Path]::IsPathFullyQualified($path) -or -not (Test-Path -LiteralPath $path -PathType Leaf)){throw 'Explicit existing static/global manifests are required.'}
    }
}
$null=New-Item -ItemType Directory -Path $OutputDirectory
$cases=[Collections.Generic.List[object]]::new()
#-------------------------------------------------------------------------------------------------
# Reborn: fixed command recipes are controlled by this script, never loaded from untrusted config or interpreted as shell expressions.
#-------------------------------------------------------------------------------------------------
function Add-BatchCase([string]$Name,[string]$Evidence,[string[]]$Arguments){
    $cases.Add([pscustomobject]@{Name=$Name;Evidence=$Evidence;Arguments=$Arguments})
}
$exe=Join-Path $repo 'source/BinaryAssetBuilder.ManifestInspector/bin/x86/Release/net8.0/BinaryAssetBuilder.ManifestInspector.exe'
$compilerHash=$null
$compilerFiles=@()
if($SchedulerControl){
    # Reborn: disposable process controls prove nonzero-exit continuation; no compiler, manifests, game or install files are involved.
    $exe=(Get-Process -Id $PID).Path
    foreach($case in @(@('control-pass',0),@('control-fail',7),@('control-after-failure',0))){
        Add-BatchCase $case[0] 'SchedulerControlOnly' @('-NoProfile','-Command',('# Reborn: owned scheduler fixture.'+[Environment]::NewLine+'Write-Output "Reborn: scheduler fixture"; exit '+$case[1]))
    }
}else{
    # Reborn: build exactly once, then bind every worker/report to the same compiler bytes.
    & dotnet build (Join-Path $repo 'source/BinaryAssetBuilder.ManifestInspector/BinaryAssetBuilder.ManifestInspector.csproj') --no-restore -c Release -p:Platform=x86 -m:1 -nr:false -v:q *> (Join-Path $OutputDirectory 'compiler-build.log')
    if($LASTEXITCODE-ne 0){throw 'Batch compiler build failed; no worker started. Inspect compiler-build.log.'}
    $compilerHash=(Get-FileHash -LiteralPath $exe -Algorithm SHA256).Hash
    # Reborn: the apphost alone does not identify managed compiler code; freeze the complete runtime/assembly dependency set.
    $compilerFiles=@(foreach($name in @('BinaryAssetBuilder.ManifestInspector.exe','BinaryAssetBuilder.ManifestInspector.dll',
        'BinaryAssetBuilder.ManifestInspector.deps.json','BinaryAssetBuilder.ManifestInspector.runtimeconfig.json',
        'BinaryAssetBuilder.Core.dll','BinaryAssetBuilder.Utility.dll','BinaryAssetBuilder.XmlCompiler.dll','BinaryAssetBuilder.W3XCompiler.dll','SageBinaryData.dll')){
        $path=Join-Path (Split-Path -Parent $exe) $name
        [pscustomobject]@{Name=$name;Path=$path;Sha256=(Get-FileHash -LiteralPath $path -Algorithm SHA256).Hash}
    })
    foreach($case in @(
        @('layout','layout-self-test'),
        @('armor-input','armor-authoring-input-self-test'),
        @('weapon-input','weapon-authoring-input-self-test'),
        @('includes-defines','sdk-include-defines-self-test'),
        @('definition-subset','sdk-definition-subset-self-test'),
        @('inheritance','sdk-instance-inheritance-self-test'),
        @('expressions','sdk-instance-expressions-self-test'),
        @('file-reference-catalog','sdk-file-reference-catalog-self-test'),
        @('document-reuse','document-reuse-self-test'),
        @('dependency-resolution','dependency-resolution-self-test'),
        @('dependency-hash','dependency-hash-self-test'),
        @('checksum','checksum-self-test'))){Add-BatchCase $case[0] 'DiagnosticOnly' @($case[1])}
    # Reborn: original-file diagnostics are a repeatable offline batch gate, not a reason to launch another one-field gameplay trial.
    Add-BatchCase 'xml-source-diagnostics' 'OriginalXmlSourceCoordinates' @('sdk-xml-diagnostics-self-test',(Join-Path $OutputDirectory 'xml-source-diagnostics'))
    # Reborn: original/edit/default airfield modules retain their real EP1 native layouts and stock metadata grammar.
    Add-BatchCase 'airfield-behaviors' 'StockQualifiedAirfieldModules' @('airfield-behaviors-self-test',$StaticManifest,$GlobalManifest)
    # Reborn: reactor explosion conditions, retargeted weapons and probability rows retain independent native/import/metadata checks.
    Add-BatchCase 'fire-weapon-death' 'StockQualifiedDeathWeapons' @('fire-weapon-death-self-test',$StaticManifest,$GlobalManifest)
    # Reborn: automatic income exercises the EP1 transaction lists and full GameObject composition.
    Add-BatchCase 'auto-deposit' 'StockQualifiedAutoDeposit' @('auto-deposit-self-test',$StaticManifest,$GlobalManifest)
    # Reborn: slaughter refunds and inherited container data are checked against the real Crane/carrier records.
    Add-BatchCase 'slaughter-contain' 'StockQualifiedSlaughterContain' @('slaughter-contain-self-test',$StaticManifest,$GlobalManifest)
    # Reborn: identity-only production markers retain their inherited wire metadata in complete GameObjects.
    Add-BatchCase 'production-update-info' 'StockQualifiedProductionInfo' @('production-update-info-self-test',$StaticManifest,$GlobalManifest)
    # Reborn: replacement targets, options and inherited ability fields retain retail-qualified metadata.
    Add-BatchCase 'replace-self' 'StockQualifiedReplaceSelf' @('replace-self-special-ability-self-test',$StaticManifest,$GlobalManifest)
    # Reborn: wall length, filter lists and both secondary masks use the new retail-qualified wall-builder writer.
    # Reborn: include the source-driven timer-monitor regression without touching user mod outputs.
    # Reborn: include the identity-only radar module in the offline regression batch.
    # Reborn: cover power-manager required power and optional EVA imports.
    # Reborn: verify original and authored production bonuses through full object composition.
    # Reborn: exercise schema-driven StoreObjectsSpecialPower fields and original chronosphere records.
    # Reborn: qualify teleport stored objects and source-driven GameObject composition.
    # Reborn: cover full EP1 teleport-update source/native and nested filter composition.
    # Reborn: cover OCL power EP1 fields, all location ordinals, filter and upgrade lists.
    # Reborn: cover player upgrade power stock records, retargeting and empty/multiple strong upgrade lists.
    # Reborn: cover the identity-only template-ID store through the normal EP1 behavior pipeline.
    # Reborn: cover restored EP1 weapon-fire fields, retargeting, defaults and whole-object composition.
    Add-BatchCase 'weapon-fire-ability' 'StockQualifiedWeaponFireAbility' @('weapon-fire-ability-self-test',$StaticManifest,$GlobalManifest)
    # Reborn: cover PropBody allocation, identity and complete root metadata.
    # Reborn: qualify EVA-ready announcements and mixed source-sized client lists.
    # Reborn: cover omitted and present VisionInfo without Conscript-only scalar assumptions.
    Add-BatchCase 'optional-vision-info' 'OptionalVisionInfo' @('optional-vision-info-self-test',$StaticManifest,$GlobalManifest)
    Add-BatchCase 'eva-ready-client' 'StockQualifiedEvaReadyClient' @('eva-ready-client-self-test',$StaticManifest,$GlobalManifest)
    Add-BatchCase 'prop-body' 'StockQualifiedPropBody' @('prop-body-self-test',$StaticManifest,$GlobalManifest)
    Add-BatchCase 'store-template-id' 'StockQualifiedStoreTemplateID' @('store-template-id-self-test',$StaticManifest,$GlobalManifest)
    Add-BatchCase 'player-upgrade-power' 'StockQualifiedPlayerUpgradePower' @('player-upgrade-power-self-test',$StaticManifest,$GlobalManifest)
    Add-BatchCase 'ocl-special-power' 'StockQualifiedOclSpecialPower' @('ocl-special-power-self-test',$StaticManifest,$GlobalManifest)
    Add-BatchCase 'teleport-stored-update' 'StockQualifiedTeleportStoredUpdate' @('teleport-stored-update-self-test',$StaticManifest,$GlobalManifest)
    Add-BatchCase 'teleport-stored-objects' 'StockQualifiedTeleportStoredObjects' @('teleport-stored-objects-self-test',$StaticManifest,$GlobalManifest)
    Add-BatchCase 'store-objects' 'StockQualifiedStoreObjects' @('store-objects-self-test',$StaticManifest,$GlobalManifest)
    Add-BatchCase 'production-bonus' 'StockQualifiedProductionBonus' @('production-bonus-self-test',$StaticManifest,$GlobalManifest)
    Add-BatchCase 'player-power-manager' 'StockQualifiedPlayerPowerManager' @('player-power-manager-self-test',$StaticManifest,$GlobalManifest)
    Add-BatchCase 'radar-behavior' 'StockQualifiedRadarBehavior' @('radar-behavior-self-test',$StaticManifest,$GlobalManifest)
    Add-BatchCase 'monitor-special-power-timer' 'StockQualifiedMonitorSpecialPowerTimer' @('monitor-special-power-timer-self-test',$StaticManifest,$GlobalManifest)
    Add-BatchCase 'wall-builder' 'StockQualifiedWallBuilder' @('wall-builder-self-test',$StaticManifest,$GlobalManifest)
    # Reborn: all FXList nugget kinds, inheritance and real local/stock dependencies use an isolated output root.
    Add-BatchCase 'fxlist-general' 'GeneralFXListNativeAndStream' @('fxlist-general-self-test',$StaticManifest,$GlobalManifest,(Join-Path $OutputDirectory 'fxlist-general'))
    # Reborn: particle templates compile privately with a retail GPU graph and final FX-to-particle stream readback.
    Add-BatchCase 'particle-template' 'ParticleNativeAndStream' @('particle-template-self-test',$StaticManifest,$GlobalManifest,(Join-Path $OutputDirectory 'particle-template'))
    # Reborn: qualify variable complete-object modules independently before widening Studio's fixed publication profile.
    Add-BatchCase 'conscript-variable-modules' 'VariableConscriptQualifiedStreamsAndPackages' @('conscript-module-graph-self-test',$StaticManifest,$GlobalManifest,(Join-Path $OutputDirectory 'variable-module-graphs'))
    Add-BatchCase 'weapon-authored-values' 'BoundedScopedPackage' @('weapon-authored-build-self-test',(Join-Path $OutputDirectory 'weapon-builds'),$StaticManifest,$GlobalManifest)
    # Reborn: independently pin stock GameObject armor-consumer links before permitting a source/compiler/publication path.
    Add-BatchCase 'gameobject-stock' 'BoundedStockNativeProjection' @('gameobject-stock-self-test',$StaticManifest)
    # Reborn: map every native root field against stock tokens without claiming indirect module semantics.
    Add-BatchCase 'gameobject-root-layout' 'BoundedRootLayoutEvidence' @('gameobject-root-layout-self-test',$StaticManifest)
    # Reborn: six module-container graphs and actual inline AI marshaler controls are one batched offline gate.
    Add-BatchCase 'gameobject-modules' 'BoundedModuleTopology' @('gameobject-module-self-test',$StaticManifest)
    # Reborn: three real module dispatch/payload implementations share one stock/edited-source regression gate.
    Add-BatchCase 'ep1-basic-modules' 'BoundedModuleNativePayload' @('ep1-basic-module-self-test',$StaticManifest)
    # Reborn: primitive nonempty Key list tokens need independent stock/client and detached corruption coverage.
    Add-BatchCase 'ep1-group-audio-client-tokens' 'BoundedStockClientPrimitiveListTokens' @('ep1-group-audio-client-token-self-test',$StaticManifest)
    # Reborn: retain both complete Conscript tracers with real texture/FX names and exact metadata/native ownership.
    Add-BatchCase 'ep1-tracer-draw' 'BoundedConscriptDrawNativeTokenGraphs' @('ep1-tracer-draw-self-test',$StaticManifest,$GlobalManifest)
    # Reborn: add this complete native/script/reference gate only when its official source root is explicitly supplied; no reduced unit is substituted.
    if($UprisingXmlRoot){
        Add-BatchCase 'ep1-scripted-draw-native' 'CompleteSelectedConscriptScriptedNativeGraphs' @('ep1-scripted-draw-native-self-test',$UprisingXmlRoot,$StaticManifest,$GlobalManifest)
        # Reborn: independently compare recursive metadata for the same complete source graphs and authored edits.
        Add-BatchCase 'ep1-scripted-draw-tokens' 'CompleteSelectedConscriptScriptedTokenGraphs' @('ep1-scripted-draw-token-self-test',$UprisingXmlRoot,$StaticManifest,$GlobalManifest)
        # Reborn: both complete scripted and both tracer modules share one Core/Tracker container before whole object integration.
        Add-BatchCase 'ep1-draws-composition' 'CompleteSelectedConscriptSharedDraws' @('ep1-draws-composition-self-test',$UprisingXmlRoot,$StaticManifest,$GlobalManifest)
        # Reborn: selected root/Body/behavior/AI and all four complete Draws share one Core occurrence table and Tracker.
        Add-BatchCase 'gameobject-draws-composition' 'SelectedRootCompleteDrawsSharedGraph' @('gameobject-draws-composition-self-test',$UprisingXmlRoot,$StaticManifest,$GlobalManifest)
        # Reborn: six original client behavior graphs include explicitly captured inherited leaves and real Core sound names.
        Add-BatchCase 'ep1-client-behaviors-native' 'CompleteSelectedConscriptClientNativeGraphs' @('ep1-client-behaviors-native-self-test',$UprisingXmlRoot,$StaticManifest,$GlobalManifest)
        # Reborn: generated recursive client metadata is verified independently against all six original stock leaves.
        Add-BatchCase 'ep1-client-behaviors-tokens' 'GeneratedSelectedConscriptClientMetadata' @('ep1-client-behaviors-token-self-test',$UprisingXmlRoot,$StaticManifest,$GlobalManifest)
        # Reborn: shared client/root/Draws composition does not replace standalone complete stock-native/token gates.
        Add-BatchCase 'gameobject-clients-composition' 'SharedRootDrawsClientContainers' @('gameobject-clients-composition-self-test',$UprisingXmlRoot,$StaticManifest,$GlobalManifest)
        # Reborn: all original 36 Conscript behavior native graphs must pass; blocked survey rows do not count as success.
        Add-BatchCase 'conscript-behavior-native' 'All36OriginalConscriptBehaviorNativeGraphs' @('conscript-behavior-native-self-test',$UprisingXmlRoot,$StaticManifest,$GlobalManifest)
        # Reborn: all original behavior metadata must independently match stock; a survey with explicit blocked rows is not a passing regression.
        Add-BatchCase 'conscript-behavior-tokens' 'All36OriginalConscriptBehaviorMetadataGraphs' @('conscript-behavior-token-self-test',$UprisingXmlRoot,$StaticManifest,$GlobalManifest)
        # Reborn: independently verify one shared metadata/native container with all original behavior leaves and common Core indices.
        Add-BatchCase 'conscript-behavior-composition' 'Shared36OriginalConscriptBehaviorMetadataContainer' @('conscript-behavior-composition-self-test',$UprisingXmlRoot,$StaticManifest,$GlobalManifest)
        # Reborn: all original module owners must independently match stock on one selected root/Core/Tracker graph; full-root publication remains separate.
        Add-BatchCase 'gameobject-all-modules' 'All49OriginalConscriptModulesOnSelectedRoot' @('gameobject-all-modules-self-test',$UprisingXmlRoot,$StaticManifest,$GlobalManifest)
        # Reborn: all six populated remaining original root owners require native and independent metadata proof; no blocked discovery rows count as success.
        Add-BatchCase 'conscript-remaining-roots' 'SixOriginalPopulatedRootNativeMetadataOwners' @('conscript-remaining-root-tokens-self-test',$UprisingXmlRoot,$StaticManifest,$GlobalManifest)
        # Reborn: all 49 modules and six original populated root owners share one Core/Tracker before independent stock proof.
        Add-BatchCase 'gameobject-root-owners' 'All49ModulesAndSixPopulatedOriginalRootOwners' @('gameobject-root-owners-self-test',$UprisingXmlRoot,$StaticManifest,$GlobalManifest)
        # Reborn: complete stock name/type coverage includes five empty list tables and eleven null targets, not complete original values or populated child admission.
        Add-BatchCase 'gameobject-root-fields' 'All126StockRootNamesAndEmptyNullContracts' @('gameobject-root-fields-self-test',$UprisingXmlRoot,$StaticManifest,$GlobalManifest)
        # Reborn: actual root values and every native owner require a complete disjoint coverage/auxiliary union, not only field names.
        Add-BatchCase 'gameobject-original-native' 'CompleteOriginalConscriptNativeOwnershipAndValues' @('gameobject-original-native-self-test',$UprisingXmlRoot,$StaticManifest,$GlobalManifest)
    }
    # Reborn: destruction/tech/LUA native dispatch and complete selected graphs share one gate; captured FX and callback limits remain explicit.
    Add-BatchCase 'ep1-allied-modules' 'BoundedCapturedIndexModuleNativePayload' @('ep1-allied-modules-self-test',$StaticManifest)
    # Reborn: one real Core occurrence table binds named root and module imports to independently read stock catalogs.
    Add-BatchCase 'gameobject-root-strong-source' 'BoundedCoreNormalizedRootModuleImports' @('gameobject-root-strong-source-self-test',$StaticManifest,$GlobalManifest)
    # Reborn: separate EP1 body ABI/token fragment gate preserves legacy dispatch and explicit publication boundaries.
    Add-BatchCase 'ep1-active-body' 'BoundedCoreBodyNativeTokenFragment' @('ep1-active-body-self-test',$StaticManifest)
    # Reborn: combined root/body/destruction native owners and shared Core indices are verified before any publication gate.
    Add-BatchCase 'gameobject-body-projection' 'BoundedRootBodyBehaviorProjection' @('gameobject-body-projection-self-test',$StaticManifest,$GlobalManifest)
    # Reborn: ordered armor owners share root/body/behavior references; this is not full GameObject asset publication.
    Add-BatchCase 'gameobject-armor-composition' 'BoundedArmorRootBodyBehaviorProjection' @('gameobject-armor-composition-self-test',$StaticManifest,$GlobalManifest)
    # Reborn: a newly compiled custom armor manifest must resolve in the combined consumer and fail when removed.
    Add-BatchCase 'gameobject-authored-armor-composition' 'BoundedAuthoredArmorConsumerProjection' @('gameobject-authored-armor-composition-self-test',(Join-Path $OutputDirectory 'armor-consumer'),$StaticManifest,$GlobalManifest)
    # Reborn: separate EP1 hardpoint ABI/token writer must match real stock graphs before any enclosing object integration.
    Add-BatchCase 'ep1-weapon-set' 'BoundedCoreWeaponSetNativeTokenFragment' @('ep1-weapon-set-self-test',$StaticManifest,$GlobalManifest)
    # Reborn: original Sentry Gun, edited turret arrays and complete object composition use isolated native checks.
    Add-BatchCase 'weapon-turret' 'WeaponTurretNativeAndComposition' @('weapon-turret-self-test',$StaticManifest,$GlobalManifest)
    # Reborn: source-driven tracers cover absent/present optional status, multiple draws and real imports.
    Add-BatchCase 'tracer-source' 'SourceDrivenTracerComposition' @('tracer-source-self-test',$StaticManifest,$GlobalManifest)
    # Reborn: scripted turret names, angles, IDs and list counts retain independent retail-qualified metadata.
    Add-BatchCase 'scripted-turret' 'ScriptedTurretComposition' @('scripted-turret-self-test',$StaticManifest,$GlobalManifest)
    # Reborn: original Heavy Walker XInclude expressions resolve per declaring document with physical diagnostics.
    Add-BatchCase 'fragment-definitions' 'FragmentDefinitionContexts' @('fragment-definitions-self-test',(Join-Path $OutputDirectory 'fragment-definitions'))
    # Reborn: experience levels retain stock-qualified EP1 pointer/list layout and ordinary authored stream publication.
    Add-BatchCase 'experience-level' 'ExperienceNativeAndStream' @('experience-level-self-test',$StaticManifest,$GlobalManifest,(Join-Path $OutputDirectory 'experience-level'))
    # Reborn: special-power flags, tech/EVA selectors and local authored commands use stock-qualified EP1 data.
    # Reborn: standalone player power buttons exercise all four data alternatives privately.
    # Reborn: exercise every joypad Data alternative using private package output.
    # Reborn: standalone button-state data is validated with both button consumer families.
    # Reborn: same-ID imported bases retain the authored identity through the complete package path.
    Add-BatchCase 'same-id-instance' 'SameIdInstance' @('same-id-instance-self-test',$StaticManifest,$GlobalManifest,(Join-Path $OutputDirectory 'same-id-instance-output'))
    # Reborn: verify PlayerTech nodes in global/static private streams and their power dependencies.
    # Reborn: unit-ability button tests cover all data alternatives and real local state/command bindings.
    # Reborn: purchase buttons bind local state and technology ranks without touching user mod outputs.
    Add-BatchCase 'purchase-tech-button' 'PurchaseTechButton' @('purchase-tech-button-self-test',$StaticManifest,$GlobalManifest,(Join-Path $OutputDirectory 'purchase-tech-button-output'))
    Add-BatchCase 'unit-ability-button' 'UnitAbilityButton' @('unit-ability-button-self-test',$StaticManifest,$GlobalManifest,(Join-Path $OutputDirectory 'unit-ability-button-output'))
    Add-BatchCase 'player-tech' 'PlayerTech' @('player-tech-self-test',$StaticManifest,$GlobalManifest,(Join-Path $OutputDirectory 'player-tech-output'))
    Add-BatchCase 'button-state' 'ButtonState' @('button-state-self-test',$StaticManifest,$GlobalManifest,(Join-Path $OutputDirectory 'button-state-output'))
    Add-BatchCase 'joypad-button' 'JoypadButton' @('joypad-button-self-test',$StaticManifest,$GlobalManifest,(Join-Path $OutputDirectory 'joypad-button-output'))
    Add-BatchCase 'player-power-button' 'PlayerPowerButton' @('player-power-button-self-test',$StaticManifest,$GlobalManifest,(Join-Path $OutputDirectory 'player-power-button-output'))
    Add-BatchCase 'special-power-template' 'SpecialPowerNativeAndStream' @('special-power-template-self-test',$StaticManifest,$GlobalManifest,(Join-Path $OutputDirectory 'special-power-template'))
    # Reborn: selected destruction/weapon modules share armor/body/root references, with optional newly built armor proof.
    Add-BatchCase 'gameobject-combat-composition' 'BoundedCombatNativeTokenProjection' @('gameobject-combat-composition-self-test',$StaticManifest,$GlobalManifest)
    Add-BatchCase 'gameobject-authored-combat-composition' 'BoundedAuthoredArmorCombatConsumer' @('gameobject-authored-combat-composition-self-test',(Join-Path $OutputDirectory 'combat-consumer'),$StaticManifest,$GlobalManifest)
    # Reborn: stock-corroborated required helper and all three-module orders keep native/token/reference ownership explicit.
    Add-BatchCase 'ep1-smc-helper-tokens' 'BoundedStockSmcTokenFragment' @('ep1-smc-helper-token-self-test',$StaticManifest)
    Add-BatchCase 'gameobject-smc-combat' 'BoundedThreeModuleCombatProjection' @('gameobject-smc-combat-self-test',$StaticManifest,$GlobalManifest)
    Add-BatchCase 'gameobject-authored-smc-combat' 'BoundedAuthoredArmorThreeModuleConsumer' @('gameobject-authored-smc-combat-self-test',(Join-Path $OutputDirectory 'smc-combat-consumer'),$StaticManifest,$GlobalManifest)
    # Reborn: independent stock locomotor records and scoped enum values precede combined movement/combat ownership admission.
    Add-BatchCase 'gameobject-movement-composition' 'BoundedCoreMovementCombatProjection' @('gameobject-movement-composition-self-test',$StaticManifest,$GlobalManifest)
    # Reborn: selected module-local hashes span Body/Behaviors but are not global asset imports or arbitrary XML id declarations.
    Add-BatchCase 'selected-module-identity' 'BoundedSelectedModuleIdentityScope' @('selected-module-identity-self-test')
    # Reborn: independently stock-corroborated placement/contact/RING geometry uses a separate EP1 native model.
    Add-BatchCase 'ep1-geometry-native' 'BoundedStockGeometryNativeGraph' @('ep1-geometry-native-self-test',$StaticManifest)
    # Reborn: generated geometry tokens require full independent stock-prefix and adversarial ownership checks.
    Add-BatchCase 'ep1-geometry-tokens' 'BoundedStockGeometryTokenFragment' @('ep1-geometry-token-self-test',$StaticManifest)
    # Reborn: optional geometry joins movement/armor/body/three-behavior graphs with stock or genuinely new compiled armor targets.
    Add-BatchCase 'gameobject-geometry-composition' 'BoundedCoreGeometryCombatProjection' @('gameobject-geometry-composition-self-test',$StaticManifest,$GlobalManifest)
    Add-BatchCase 'gameobject-authored-geometry-composition' 'BoundedAuthoredArmorGeometryConsumer' @('gameobject-authored-geometry-composition-self-test',(Join-Path $OutputDirectory 'geometry-consumer'),$StaticManifest,$GlobalManifest)
    # Reborn: by-value enum/color child syntax must never introduce native enum/color allocation pointers.
    Add-BatchCase 'gameobject-inline-children' 'BoundedInlineRootChildSemantics' @('gameobject-inline-child-self-test',$StaticManifest)
    # Reborn: pointed name headers/flammability/camera join the full selected graph with stock or genuine new compiled armor targets.
    Add-BatchCase 'gameobject-pointer-roots' 'BoundedCorePointedRootComposition' @('gameobject-pointer-root-self-test',$StaticManifest,$GlobalManifest)
    Add-BatchCase 'gameobject-authored-pointer-roots' 'BoundedAuthoredArmorPointedRootConsumer' @('gameobject-authored-pointer-root-self-test',(Join-Path $OutputDirectory 'pointer-root-consumer'),$StaticManifest,$GlobalManifest)
    # Reborn: complete independent stock vision tables and optional shared root owners use the separate forty-byte EP1 record.
    Add-BatchCase 'ep1-vision-info' 'BoundedStockVisionAndCoreComposition' @('ep1-vision-info-self-test',$StaticManifest,$GlobalManifest)
    # Reborn: the concrete EP1 AI/chooser native port has independent stock/default/reference/inline-dispatch gates before token publication.
    Add-BatchCase 'ep1-ai-update' 'BoundedStockAiNativeAndCoreReferences' @('ep1-ai-update-self-test',$StaticManifest,$GlobalManifest)
    # Reborn: bounded generated AI inheritance/chooser metadata must match complete independent stock prefixes before root composition.
    Add-BatchCase 'ep1-ai-update-tokens' 'BoundedStockAiTokenFragments' @('ep1-ai-update-token-self-test',$StaticManifest,$GlobalManifest)
    # Reborn: inline AI/count/identity joins the complete selected graph with stock or genuine newly compiled armor targets.
    Add-BatchCase 'gameobject-ai-composition' 'BoundedCoreAiCombatGeometryComposition' @('gameobject-ai-composition-self-test',$StaticManifest,$GlobalManifest)
    Add-BatchCase 'gameobject-authored-ai-composition' 'BoundedAuthoredArmorAiConsumer' @('gameobject-authored-ai-composition-self-test',(Join-Path $OutputDirectory 'ai-consumer'),$StaticManifest,$GlobalManifest)
    # Reborn: the timeout scalar's default and frame conversion remain independently stock/schema/Core checked inside the shared graph.
    Add-BatchCase 'gameobject-duration' 'BoundedStockDurationDefaultAndCoreGraph' @('gameobject-duration-self-test',$StaticManifest,$GlobalManifest)
    # Reborn: direct omitted/explicit/zero defaults cannot hide behind schema default materialization or synthetic stock metadata.
    Add-BatchCase 'gameobject-root-defaults' 'FourteenOfficialRootDefaultsAndOverrides' @('gameobject-root-defaults-self-test')
    # Reborn: actual Core destruction FX names/defaults and semantic imports must pass without captured source dependency indices.
    Add-BatchCase 'ep1-destruction-source' 'BoundedCoreNormalizedModuleNativePayload' @('ep1-destruction-source-self-test',$StaticManifest)
    # Reborn: nested effect/status graphs and Core-normalized strong imports share one new update-family gate.
    Add-BatchCase 'ep1-damage-motion' 'BoundedNestedModuleNativePayload' @('ep1-damage-motion-self-test',$StaticManifest)
    # Reborn: stock toggle/base/nested anchors and detached corruptions are a layout gate, not a new compiler capability.
    Add-BatchCase 'toggle-status-layout' 'BoundedNestedLayoutReview' @('toggle-status-layout-self-test',$StaticManifest)
    # Reborn: selected complete standalone AI graphs include present pointer values and real Core-normalized weapon imports, not enclosing module dispatch.
    Add-BatchCase 'ep1-toggle-ai' 'BoundedStandaloneAiNativePayload' @('ep1-toggle-ai-self-test',$StaticManifest)
    # Reborn: concrete module graphs currently use captured-index diagnostics; normal source/dispatch/publication is deliberately separate.
    Add-BatchCase 'ep1-toggle-module' 'BoundedCapturedIndexModuleNativePayload' @('ep1-toggle-module-self-test',$StaticManifest)
    # Reborn: independently catalog-normalized full module reference/default graphs must not substitute captured stock dependency indices.
    Add-BatchCase 'ep1-toggle-source' 'BoundedCoreNormalizedModuleNativePayload' @('ep1-toggle-source-self-test',$StaticManifest,$GlobalManifest)
    # Reborn: actual named source references must bind stock and newly compiled armor without captured numeric dependency indices.
    Add-BatchCase 'gameobject-armor-source' 'BoundedNamedNativeProjection' @('gameobject-armor-source-self-test',(Join-Path $OutputDirectory 'go'),$StaticManifest)
    Add-BatchCase 'armor-core' 'BoundedScopedStream' @('ep1-armor-document-self-test',(Join-Path $OutputDirectory 'armor-core'),$StaticManifest)
    Add-BatchCase 'armor-pair-core' 'BoundedScopedStream' @('armor-pair-document-self-test',(Join-Path $OutputDirectory 'armor-pair-core'),$StaticManifest)
    # Reborn: general single-armor identities/list/literals are tested jointly without widening the exported alpha gate.
    Add-BatchCase 'armor-general-authoring' 'ExperimentalGeneralArmorPackage' @('armor-general-authored-self-test',(Join-Path $OutputDirectory 'general-armors'))
    # Reborn: independently exercise multiple user-defined identities in one real stream, including the bounded maximum.
    Add-BatchCase 'armor-general-sets' 'ExperimentalGeneralArmorSetPackage' @('armor-general-set-self-test',(Join-Path $OutputDirectory 'armor-sets'))
    # Reborn: connect captured source semantics to native set/package verification, without expanding the exported alpha.
    Add-BatchCase 'armor-source-bridge' 'ExperimentalArmorSourcePackage' @('armor-source-self-test',(Join-Path $OutputDirectory 'as'))
    if($Full){
        Add-BatchCase 'armor-authored-fields' 'BoundedScopedPackage' @('armor-authored-build-self-test',(Join-Path $OutputDirectory 'armor-builds'),$StaticManifest)
        Add-BatchCase 'compiler-broad' 'DiagnosticOnly' @('compiler-self-test')
    }
}
$rows=[Collections.Generic.List[object]]::new()
$active=[Collections.Generic.List[object]]::new()
$next=0;$started=[DateTime]::UtcNow
while($next-lt $cases.Count -or $active.Count-gt 0){
    while($next-lt $cases.Count -and $active.Count-lt $Workers){
        $case=$cases[$next];$next++
        $temporary=Join-Path $OutputDirectory ($case.Name+'-temp');$null=New-Item -ItemType Directory -Path $temporary
        $info=[Diagnostics.ProcessStartInfo]::new($exe)
        $info.WorkingDirectory=$repo;$info.UseShellExecute=$false;$info.CreateNoWindow=$true
        $info.RedirectStandardOutput=$true;$info.RedirectStandardError=$true
        # Reborn: separate processes isolate Core static settings/caches; each owns its TEMP/TMP, files and logs.
        $info.Environment['TEMP']=$temporary;$info.Environment['TMP']=$temporary
        foreach($argument in $case.Arguments){$info.ArgumentList.Add($argument)}
        try{
            $process=[Diagnostics.Process]::Start($info)
            $active.Add([pscustomobject]@{Case=$case;Process=$process;Started=[DateTime]::UtcNow;
                Output=$process.StandardOutput.ReadToEndAsync();Error=$process.StandardError.ReadToEndAsync()})
            Write-Output ('Reborn: batch started '+$case.Name)
        }catch{
            $rows.Add([pscustomobject]@{Name=$case.Name;Evidence=$case.Evidence;Status='StartFailed';ExitCode=$null;Error=$_.Exception.Message;ElapsedSeconds=0;Log=$null})
        }
    }
    foreach($job in @($active.ToArray())){
        if(-not $job.Process.HasExited){continue}
        $job.Process.WaitForExit()
        $log=Join-Path $OutputDirectory ($job.Case.Name+'.log')
        [IO.File]::WriteAllText($log,$job.Output.GetAwaiter().GetResult()+[Environment]::NewLine+$job.Error.GetAwaiter().GetResult())
        $code=$job.Process.ExitCode;$status=if($code-eq 0){'Passed'}else{'Failed'}
        $rows.Add([pscustomobject]@{Name=$job.Case.Name;Evidence=$job.Case.Evidence;Status=$status;ExitCode=$code;
            ElapsedSeconds=[Math]::Round(([DateTime]::UtcNow-$job.Started).TotalSeconds,3);Log=$log})
        Write-Output ('Reborn: batch '+$status+' '+$job.Case.Name+' (exit '+$code+')')
        $job.Process.Dispose();$null=$active.Remove($job)
    }
    # Reborn: short scheduler polling does not serialize child tests or pause user-visible progress for long intervals.
    if($active.Count-gt 0){Start-Sleep -Milliseconds 100}
}
$failed=@($rows|Where-Object Status -NE 'Passed')
$unchanged=$true
foreach($entry in $compilerFiles){if((Get-FileHash -LiteralPath $entry.Path -Algorithm SHA256).Hash-cne $entry.Sha256){$unchanged=$false}}
$report=[pscustomobject]@{Mode=$(if($SchedulerControl){'SchedulerControl'}elseif($Full){'Full'}else{'Focused'});
    CompilerPath=$exe;CompilerSha256=$compilerHash;CompilerFiles=$compilerFiles;CompilerUnchanged=$unchanged;Workers=$Workers;
    PlannedGroups=$cases.Count;CompletedGroups=$rows.Count;PassedGroups=$rows.Count-$failed.Count;FailedGroups=$failed.Count;
    ElapsedSeconds=[Math]::Round(([DateTime]::UtcNow-$started).TotalSeconds,3);Cases=@($rows|Sort-Object Name);
    GameExecuted=$false;InstalledFilesModified=$false;AlphaCapabilitiesExpanded=$false;
    Limitation='Diagnostic source semantics are not general playable publication; a batch is separate packages, not a mixed-asset stream.'}
[IO.File]::WriteAllText((Join-Path $OutputDirectory 'batch-report.json'),($report|ConvertTo-Json -Depth 7))
Write-Output ('Reborn: batch complete: '+$report.PassedGroups+'/'+$report.PlannedGroups+' groups passed; report '+(Join-Path $OutputDirectory 'batch-report.json'))
if($SchedulerControl){
    if($rows.Count-ne 3 -or $failed.Count-ne 1 -or @($rows|Where-Object {$_.Name-ceq 'control-fail' -and $_.ExitCode-eq 7}).Count-ne 1 -or @($rows|Where-Object {$_.Name-ceq 'control-after-failure' -and $_.Status-ceq 'Passed'}).Count-ne 1){throw 'Scheduler failure/continuation controls failed.'}
    Write-Output 'Reborn: scheduler controls PASS; expected failure is retained in the report.'
}elseif($failed.Count-gt 0 -or -not $unchanged){exit 1}
