# Reborn: pin the config opener's memory-reader conversion and count-return contract without launching or instrumenting the game.
[CmdletBinding()]
param([Parameter(Mandatory=$true)][string]$ImagePath,[switch]$SelfTest,[switch]$AsJson)
$ErrorActionPreference='Stop'
$routeSelfTest=$SelfTest;$routeAsJson=$AsJson
$null=. (Join-Path $PSScriptRoot 'Get-Ra3Ep11ConfigConsumer.ps1') -ImagePath $ImagePath
$SelfTest=$routeSelfTest;$AsJson=$routeAsJson
$routePins=@(
    [pscustomobject]@{Raw=0xd7e90;Length=486;Hash='661D1388C1B8AB8E19F22D3D589D06C713BAE324B8DE6B7FDB61738818A61A6B'},
    [pscustomobject]@{Raw=0xd7b90;Length=137;Hash='49022D44C02D22EF44A6CDD4042AAF7CC9B5F6D56E4EF31B5C592F63CEB09D10'},
    [pscustomobject]@{Raw=0xd7c40;Length=161;Hash='00305678003DF06AA004E8FC4802ECC08EA6CD5B00A7EF01C237166C2C1EFB35'},
    [pscustomobject]@{Raw=0xd7170;Length=137;Hash='6D710C8370A8E564BE18DCF7A7B0188E536D17CBBE1328B2D0BFDD035BBE663D'},
    [pscustomobject]@{Raw=0xd5920;Length=70;Hash='CEA9D69162F5DAA22B45B6ADA5A34EE593B781C648E90366702C7D3F8C09B58E'},
    [pscustomobject]@{Raw=0x7fa208;Length=96;Hash='A76D32CB1B0E75FB448B97A87F93E137D7B6C9BD66D842A591ECEAA25F1952E2'},
    [pscustomobject]@{Raw=0xd5bb0;Length=557;Hash='10464E8D0D0F1E03613690476FCA12B75D3E15F9B66842DCF945C14683123AEF'},
    [pscustomobject]@{Raw=0x569870;Length=47;Hash='97E92A48A7F78B20CE7116CC424EC99582F3DC3732FAC82E9B6EB0392FB60FED'},
    [pscustomobject]@{Raw=0x5698d0;Length=41;Hash='6BE970A69A236FF064C6236610218644B525F2882A815C7C90BD7AE515E77987'},
    [pscustomobject]@{Raw=0x5699e0;Length=118;Hash='F05A3D2930059FD30CABE26D1537BEC8229450697B4227340F93987671AC2F3F'},
    [pscustomobject]@{Raw=0x569cd0;Length=54;Hash='589E2A3F07B9189540477BA51F8D45FE9FDF91E32CEADD78A707D5BBA7FBCB1F'},
    [pscustomobject]@{Raw=0x569630;Length=282;Hash='B917EE749A90812CF345847CC1AE509469E8DF38593DCF69ED1E4F18B584C3A9'}
)
#-------------------------------------------------------------------------------------------------
<# Reborn: bind reviewed bodies and memory-reader vtable to the already SHA-admitted image; refuse altered private code/data slices. #>
#-------------------------------------------------------------------------------------------------
function Assert-Ep11ConfigReadRoute([byte[]]$Bytes) {
    foreach($pin in $routePins){
        if($pin.Raw+$pin.Length-gt $Bytes.Length){throw 'Read-route evidence truncated.'}
        if([Convert]::ToHexString([Security.Cryptography.SHA256]::HashData([byte[]]$Bytes[$pin.Raw..($pin.Raw+$pin.Length-1)]))-cne $pin.Hash){throw 'Read-route pin differs.'}
    }
    # Reborn: these raw mappings are specific to the admitted image, not a general RVA/raw identity assumption.
    if([BitConverter]::ToUInt32($Bytes,0x7fa208+0xc)-ne 0x4d5920 -or
       [BitConverter]::ToUInt32($Bytes,0x7fa208+0x54)-ne 0x4d7170 -or
       [BitConverter]::ToUInt32($Bytes,0x7fa270+0x38)-ne 0x4d7c40 -or
       [BitConverter]::ToUInt32($Bytes,0x7fa270+0xc)-ne 0x4d5bb0){throw 'Reviewed conversion/read vtable slot differs.'}
}
Assert-Ep11ConfigReadRoute $modBytes
if($SelfTest){
    # Reborn: mutate only private snapshots, including each instruction body and the concrete vtable; never invoke native virtual methods.
    foreach($pin in $routePins){$fault=[byte[]]$modBytes.Clone();$fault[$pin.Raw]=$fault[$pin.Raw]-bxor 1;$refused=$false;try{Assert-Ep11ConfigReadRoute $fault}catch{$refused=$true};if(-not $refused){throw 'Read-route mutation admitted.'}}
    foreach($raw in @((0x7fa208+0xc),(0x7fa208+0x54),(0x7fa270+0x38),(0x7fa270+0xc))){
        $fault=[byte[]]$modBytes.Clone();$fault[$raw]=$fault[$raw]-bxor 1;$refused=$false
        try{Assert-Ep11ConfigReadRoute $fault}catch{$refused=$true};if(-not $refused){throw 'Vtable slot mutation admitted.'}
    }
}
# Reborn: expose observation sites as a plan, never as executed game proof or a ready-to-run debugger recipe.
if([Convert]::ToHexString([Security.Cryptography.SHA256]::HashData((Read-PeSnapshot $ImagePath)))-cne $modHash){throw 'Read-route image changed during analysis.'}
$report=[pscustomobject]@{ImageSha256=$modHash;ReviewedPins=12;OpenerVa='0x004D7E90';RequestedFlags='0x401';ConversionFlag='0x400';SourceVtableVa='0x00BFA270';ConversionSlot='0x38';ConversionVa='0x004D7C40';MemoryVtableVa='0x00BFA208';PopulateSlot='0x54';PopulateVa='0x004D7170';MemoryReadSlot='0x0C';MemoryReadVa='0x004D5920'
    # Reborn: distinguish source engine byte counts and request fields from kernel NTSTATUS/IO_STATUS_BLOCK semantics.
    SourceReadVa='0x004D5BB0';SourceBufferedFlag='0x200';SourceRequestVa='0x009699E0';SourceWaitVa='0x00969870';SourceCountGetterVa='0x009698D0';SourceCountLowOffset='0x38';SourceCountHighOffset='0x3C';SourceRequestStateOffset='0x04';SourceReadEaxIsNtStatus=$false;SourceDirectReturnUsesLow32Count=$true;FullRequestCompletionSemanticsRecovered=$false
    # Reborn: worker completion and wait return are not interchangeable with full successful transfer or kernel completion.
    CompletionWaitVa='0x00969CD0';ReadWorkerVa='0x00969630';WaitCanReturnWithPendingState=$true;WorkerCanFinishAfterShortRead=$true;BackendReadSlot='0x14';BackendReadVaRecovered=$false
    ReaderOpenReturnVa='0x004D86F2';ReaderReadReturnVa='0x004D8725';LineSplitterEntryVa='0x004D9040'
    PopulationStoresActualReturnAsSize=$true;ZeroPopulationReturnCanSucceed=$true;ConversionFailureCanReturnOriginalReader=$true;CountReturnWithoutDestinationCopyPossible=$true
    KernelReadObserved=$false;MemoryReadObserved=$false;ExactProbeConsumedProven=$false;DebuggerRecipeReady=$false;TargetExecuted=$false;ProductionBuildReady=$false
    PrivateMutationRefusals=$(if($SelfTest){16}else{0})}
if($AsJson){$report|ConvertTo-Json -Depth 4}else{$report}
