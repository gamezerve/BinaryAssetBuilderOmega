# Reproducing the destruction FX source/Core checkpoint

This is an engineering walkthrough, **not a new supported alpha mod.xml recipe**.
It explains what to open, run, compare and change for this specific port. Do not
modify your installed game or official XML/XSD references. No game launch is needed.

## 1. Open the source and its contract

Open the local repository in your code editor:
`C:\Users\drknt\.codex\.chatgpt-projects\g-p-6aa6a9d9e7f48191931d3f26b6511794\BinaryAssetBuilderOmega2-uprising`.

Then open these files side by side:

- Official authored example:
  `D:\OneDrive\CNC Files\CnC_Modding_Support-main (Official XML, Schema, Script, Shader, Maps)\Uprising\Xml (Uprising)\Allied\Structures\AlliedConstructionYard.xml`.
  Find `ModuleTag_BuildingDestruction`. Its Item uses `FX="SovietRefineryHit"`,
  `CreationObject="AlliedBarracks_Debris"`, a bone prefix and velocity 100.
- `schemas/ra3ep1/xsd/Modules/BuildingDestructionBehavior.xsd`.
  It declares FX as **FXListRef** (strong asset reference), CreationObject as
  **GameObjectWeakRef**, and FillerMesh as **Poid/SubObject**. It also supplies
  MaxDebrisCloudsPerSecond=2.0, FillerMesh=NEWSKIN_FILL and item velocity=1.0.
- Follow the base to `Modules/Die.xsd` and `Includes/DieMux.xsd`.
  DieMuxData is required. It has inherited defaults of its own.
- Follow reference types through `Includes/Ref.xsd` and `Base/AssetBase.xsd`.

The XML supplies names/values; the XSD supplies their meaning, cardinality and
defaults. Neither alone proves the native byte layout.

## 2. Inspect the stock binary evidence, without dumping the game

Open **PowerShell 7**. The following commands use this checkout and existing
unpacked data; they do not edit those data. Build first and stop on a failed build
so you cannot accidentally test an older executable.

```powershell
# Reborn: select the working checkout, not the original read-only references or installed game.
Set-Location -LiteralPath 'C:\Users\drknt\.codex\.chatgpt-projects\g-p-6aa6a9d9e7f48191931d3f26b6511794\BinaryAssetBuilderOmega2-uprising'
# Reborn: build the x86 inspector and its compiler dependencies; do not test a stale binary after failure.
& 'C:\Program Files\dotnet\dotnet.exe' build '.\source\BinaryAssetBuilder.ManifestInspector\BinaryAssetBuilder.ManifestInspector.csproj' --no-restore -c Release -p:Platform=x86 -m:1 -nr:false -v:q
if ($LASTEXITCODE -ne 0) { throw 'Build failed; inspect the compiler errors first.' }
# Reborn: use explicit existing stock metadata and the freshly built inspector.
$probeExe = Join-Path (Get-Location).Path 'source\BinaryAssetBuilder.ManifestInspector\bin\x86\Release\net8.0\BinaryAssetBuilder.ManifestInspector.exe'
$probeStatic = 'D:\TEMP\Red Alert 3 Uprising Source Data\Static Data\data\static.manifest'
# Reborn: inspect only the selected stock object/module graph, not a full binary dump.
& $probeExe gameobject-module-review $probeStatic 'GameObject:AlliedConstructionYard'
```

Find BuildingDestructionModuleData/class ID **0D96A2F4**. The reader follows this
asset's bounded slice and token/native pointers; there is no need to open the
entire large BIN in a hex editor. For this selected module the native root is 72
bytes: Die base 52, AssociatedModule at +52, MaxDebris at +56, FillerMesh at +60,
Item count at +64 and item pointer at +68. One item is 20 bytes: bone string 8,
FX import 4, weak object ID 4, velocity 4. These offsets came from stock
token/native joins and complete selected native comparisons, not XSD attribute
order. Do not generalize them to unrelated module types.

## 3. Open the implementation and understand what was added

- `source/SageBinaryData/SageBinaryData/Modules/Ep1AlliedModules.cs`:
  native structs mirror the verified layout. FillerMesh uses TypedAssetId rather
  than StringHash because Poid identities are invariant-lowercased. The previous
  comparison actually detected the wrong case-sensitive hash choice.
- `source/BinaryAssetBuilder.XmlCompiler/Marshaler.Ep1AlliedModules.cs`:
  `Marshal(BuildingDestructionModuleData*)` reads attributes/items and invokes
  the Die base marshaler; the item marshaler emits bone/FX/weak ID/velocity.
- `source/BinaryAssetBuilder.XmlCompiler/Marshaler.GameObject.cs`:
  concrete class ID dispatch selects this marshaler for a behavior node.

These were already added in the prior checkpoint. **This checkpoint does not
invent another native layout or copy a stock dependency index.** Its missing step
was converting authored `SovietRefineryHit` into the compiler's own reference table.

## 4. Connect source names to Core references

Open `tests/fixtures/DestructionPipeline.xsd`. It directly includes the relevant
official primitive/reference/die/destruction declarations. Two copied module base
declarations are checked against official BaseModules; inheritance-only FXList,
GameObject and SubObject stubs do not grant full asset compiler support.

Open `source/BinaryAssetBuilder.ManifestInspector/Ep1DestructionSourceSmokeTest.cs`:

1. `CheckSchema` refuses drift in copied base declarations.
2. `Normalize` validates XML, inserts schema defaults and calls
   `ReferencePipelineSmokeTest.Normalize`, which invokes the actual Core
   `AssetDeclarationDocument.HandleAssetReferenceType` seam. It does not implement
   a replacement reference algorithm.
3. The resulting strong target must exist **exactly once** in the independently
   loaded static manifest, with the expected FXList type and instance identity.
4. Native compilation uses the normal behavior dispatch and marshaler.
5. `Ep1AlliedModuleSmokeTest.Compare` maps both the stock import index and the new
   output index through their respective reference tables. It compares their
   actual type/instance pairs before rebasing that word. Equal raw index numbers
   are not required and would not prove equal targets.

For the single named FX, Core emits `SovietRefineryHit\0`: the suffix is its own
zero-based dependency-table slot. The native marshaler writes slot+1, so 1 is a
real import and 0 means no reference. Neither number should be hardcoded into
authored XML. Repeated references are appended separately at this seam; a later
whole-stream linker may have a different table policy, which is not tested here.

The original captured-index diagnostic remains a separate regression, not hidden
as the new source test. Weak-reference existence, inherited/nested token tables,
complete source-graph preprocessing and FX payload compilation are still outside
this checkpoint.

## 5. Run the targeted checks

Continue in the same PowerShell session after the successful build:

```powershell
# Reborn: retain the old native/captured-index gate independently.
& $probeExe ep1-allied-modules-self-test $probeStatic
if ($LASTEXITCODE -ne 0) { throw 'Native module regression failed.' }
# Reborn: run actual named-FX/Core/default/import checks without launching or installing a mod.
& $probeExe ep1-destruction-source-self-test $probeStatic
if ($LASTEXITCODE -ne 0) { throw 'Destruction source regression failed.' }
```

Success requires more than a build message. The new gate must show PASS after:

- full selected stock native graph, real dispatch and exact auxiliary slots;
- repeated builds with identical BIN/RELO/IMP output;
- a distinct real FX target being refused as a match for the original stock FX;
- three authored items generating occurrence import indices **1/2/3**, with the
  first and third resolving to the same target (Core appends each occurrence at
  this seam; it does not deduplicate these entries);
- unknown FX, wrong typed target, ambiguous catalog entry, authored numeric index,
  missing DieMuxData and unknown XML attribute all being refused.

If one fails, read its error/stack and fix the relevant source/schema/reference/
layout stage. Do not silence it, change official schemas, or clear unmatched
bytes merely to force equality.

## 6. Run the batch before committing

`scripts/Test-Ra3Ep1Batch.ps1` adds the case `ep1-destruction-source`. Choose a
**new, nonexistent** output directory every run; existing evidence is not overwritten.

```powershell
# Reborn: batch independently checks source/native/armor/weapon regressions and freezes the compiler bytes during workers.
& '.\scripts\Test-Ra3Ep1Batch.ps1' -OutputDirectory '.\artifacts\DestructionSource-manual-01' -StaticManifest $probeStatic -GlobalManifest 'D:\TEMP\Red Alert 3 Uprising Source Data\Global Data\data\global.manifest' -Workers 2
if ($LASTEXITCODE -ne 0) { throw 'Batch failed; inspect batch-report.json and the failing case log.' }
```

Read `batch-report.json`, `ep1-destruction-source.log` and `compiler-build.log` in
that output directory. Check all groups passed, CompilerUnchanged=true,
GameExecuted=false and InstalledFilesModified=false. This Focused gate is not a
new Full-suite or gameplay result. Only then update the English README/evidence
notes, inspect the diff and commit the specific files. Publish only to
`gamezerve/BinaryAssetBuilderOmega`, never the Qibbi upstream.

## Recorded result

`artifacts/DestructionSource-a/batch-report.json`: **30/30 Focused groups pass**,
**90.503 seconds** total; the new source gate takes **3.967 seconds**. The batch's
incremental build has zero warnings/errors in 1.23 seconds. Eight runtime hashes
remain unchanged during workers; no game ran or installation file changed.
No new Full-suite or gameplay acceptance is claimed, and alpha h is unchanged.

The initial targeted attempt failed because the new test expected deduplication
at this seam. Reading `HandleAssetReferenceType` showed that it appends every
occurrence. The test and explanation were corrected to that actual contract;
production Core behavior was not changed merely to satisfy the new expectation.
