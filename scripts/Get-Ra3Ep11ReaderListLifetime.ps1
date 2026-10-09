# Reborn: recover circular-sentinel insertion aliases and separate resource cleanup from conditional reader deletion using static evidence and synthetic memory only.
[CmdletBinding()]
param([Parameter(Mandatory=$true)] [string] $ImagePath,[Parameter(Mandatory=$true)] [string] $BaselineImagePath,[switch] $SelfTest,[switch] $AsJson)
$ErrorActionPreference='Stop'
$listSelfTest=$SelfTest;$listAsJson=$AsJson
$ownership=. (Join-Path $PSScriptRoot 'Get-Ra3Ep11ReaderOwnership.ps1') -ImagePath $ImagePath -BaselineImagePath $BaselineImagePath
$SelfTest=$listSelfTest;$AsJson=$listAsJson

#-------------------------------------------------------------------------------------------------
<# Reborn: pin insertion, resource cleanup, source-list release/reset, node cleanup, release thunk and concrete vtable without executing native instructions. #>
#-------------------------------------------------------------------------------------------------
function Assert-Ep11ReaderListLifetimeCode([byte[]] $Bytes) {
    foreach($slice in @(
        [pscustomobject]@{Offset=0xcea62;Length=40;Hash='AD7113EED780DC5A95D6F758A8CD5231CA5953753D7CCFFB2D23645BE35857FF'},
        [pscustomobject]@{Offset=0x184b0;Length=89;Hash='FDC12F1FB1632FB8CE082E92F1CF02D0A48BE32C402C6E194C5C91061D9B2D19'},
        [pscustomobject]@{Offset=0xabdeb;Length=38;Hash='4B6C07E94F191E3508EA7E1AF51719C2FCE0C5B04FBD62A969B8C59B1E809634'},
        [pscustomobject]@{Offset=0x97090;Length=95;Hash='FF4FC42B24166BBEA4E9FC11BE147B529F7E7A8597D7E9E26E0845EC9BC197CE'},
        [pscustomobject]@{Offset=0x171c0;Length=14;Hash='2A807D2070C8724D202C529406CFB1919B5F31A8B22075A126CDF19C30436235'},
        [pscustomobject]@{Offset=0x7f8ea0;Length=64;Hash='A244CEABBEC7086DEA1BB11E7E916D50995BA1BDFF0B85C2F48C0A6C414CB747'},
        [pscustomobject]@{Offset=0x9f800;Length=30;Hash='128C2C83DC5D2127A5169183F8350DC2D9D7A52F33B6CEC5F96320052C95788C'}
    )){
        if($slice.Offset+$slice.Length-gt $Bytes.Length){throw 'Reader lifetime slice outside image.'}
        if([Convert]::ToHexString([Security.Cryptography.SHA256]::HashData([byte[]]$Bytes[$slice.Offset..($slice.Offset+$slice.Length-1)]))-cne $slice.Hash){throw 'Reviewed reader lifetime changed.'}
    }
}

#-------------------------------------------------------------------------------------------------
<# Reborn: validate synthetic circular-list linkage under an explicit diagnostic bound; this is not native corruption handling or memory dereferencing. #>
#-------------------------------------------------------------------------------------------------
function Get-SyntheticReaderNodes([hashtable] $Memory,[long] $Sentinel) {
    if(-not $Memory.ContainsKey($Sentinel) -or -not $Memory.ContainsKey($Sentinel+4)){throw 'Synthetic sentinel missing.'}
    $seen=@{};$previous=$Sentinel;$current=[long]$Memory[$Sentinel];$nodes=[Collections.Generic.List[long]]::new()
    while($current-ne $Sentinel){
        if($nodes.Count-ge 100 -or $seen.ContainsKey($current) -or -not $Memory.ContainsKey($current) -or
            -not $Memory.ContainsKey($current+4) -or -not $Memory.ContainsKey($current+8) -or [long]$Memory[$current+4]-ne $previous){throw 'Synthetic circular-list invariant differs.'}
        $seen[$current]=$true;$nodes.Add($current);$previous=$current;$current=[long]$Memory[$current]
    }
    if([long]$Memory[$Sentinel+4]-ne $previous){throw 'Synthetic sentinel tail differs.'}
    return ,($nodes.ToArray())
}

#-------------------------------------------------------------------------------------------------
<# Reborn: model the exact observed link-write order so the first-node previous alias updates sentinel.next before reloading and storing the reader. #>
#-------------------------------------------------------------------------------------------------
function Invoke-SyntheticReaderInsertion([hashtable] $Memory,[long] $Sentinel,[long] $NewNode,[uint32] $Reader) {
    $null=Get-SyntheticReaderNodes $Memory $Sentinel
    if($Memory.ContainsKey($NewNode) -or $Memory.ContainsKey($NewNode+4) -or $Memory.ContainsKey($NewNode+8)){throw 'Synthetic new node overlaps existing words.'}
    $first=[long]$Memory[$Sentinel]
    $Memory[$NewNode]=$first
    $Memory[$NewNode+4]=[long]$Memory[$first+4]
    $previous=[long]$Memory[$first+4]
    $Memory[$previous]=$NewNode
    $Memory[$first+4]=$NewNode
    $reloaded=[long]$Memory[$Sentinel]
    $Memory[$reloaded+8]=$Reader
    if($reloaded-ne $NewNode){throw 'Synthetic insertion did not reload the new first node.'}
    $null=Get-SyntheticReaderNodes $Memory $Sentinel
    return $reloaded
}

#-------------------------------------------------------------------------------------------------
<# Reborn: build bounded detached empty/successive insertion scenarios and expose traversal order; no native allocator or destructor is emulated. #>
#-------------------------------------------------------------------------------------------------
function Get-SyntheticReaderScenario([uint32[]] $Readers) {
    if($Readers.Count-gt 100){throw 'Synthetic reader scenario exceeds diagnostic bound.'}
    $sentinel=[long]0x1000;$memory=@{};$memory[$sentinel]=$sentinel;$memory[$sentinel+4]=$sentinel
    for($index=0;$index-lt $Readers.Count;$index++){
        $null=Invoke-SyntheticReaderInsertion $memory $sentinel ([long](0x2000+64*$index)) $Readers[$index]
    }
    $nodes=Get-SyntheticReaderNodes $memory $sentinel
    [pscustomobject]@{InsertedCount=$Readers.Count;FirstNode=[long]$memory[$sentinel];TailNode=[long]$memory[$sentinel+4];TraversalReaders=@(foreach($node in $nodes){[uint32]$memory[$node+8]});FreshNodePayloadAliasRecovered=$true}
}
Assert-Ep11ReaderListLifetimeCode $ep11Bytes
$scenarios=@(Get-SyntheticReaderScenario ([uint32[]]@());Get-SyntheticReaderScenario ([uint32[]]@(11));Get-SyntheticReaderScenario ([uint32[]]@(11,22));Get-SyntheticReaderScenario ([uint32[]]@(11,22,33)))
if($SelfTest){
    # Reborn: require prepend/LIFO traversal for empty and one/two/three-reader fixtures under healthy sentinel linkage.
    $expected=@('','11','22,11','33,22,11')
    foreach($index in 0..3){if(($scenarios[$index].TraversalReaders -join ',')-cne $expected[$index]){throw 'Synthetic reader traversal differs.'}}
    # Reborn: reject detached broken previous links and a non-sentinel cycle as diagnostic policy, not an engine safety claim.
    foreach($faultKind in @('previous','cycle')){
        $sentinel=[long]0x1000;$node=[long]0x2000;$memory=@{}
        $memory[$sentinel]=$node;$memory[$sentinel+4]=$node;$memory[$node]=$sentinel;$memory[$node+4]=$sentinel;$memory[$node+8]=[uint32]11
        if($faultKind-eq 'previous'){$memory[$node+4]=$node}else{$memory[$node]=$node}
        $rejected=$false;try{$null=Invoke-SyntheticReaderInsertion $memory $sentinel ([long]0x3000) 22}catch{$rejected=$true}
        if(-not $rejected){throw 'Broken synthetic list admitted.'}
    }
    # Reborn: detached byte faults cover link alias, payload reload, cleanup fields, release slot, sentinel reset, node free and concrete deleting slot.
    foreach($offset in @(0xcea80,0xcea85,0xcea87,0x184c0,0x184f7,0x184ff,0xabdfa,0xabe0c,0x970db,0x171c6,0x7f8edc,0x9f810)){
        $fault=[byte[]]$ep11Bytes.Clone();$fault[$offset]=$fault[$offset]-bxor 1;$rejected=$false
        try{Assert-Ep11ReaderListLifetimeCode $fault}catch{$rejected=$true}
        if(-not $rejected){throw 'Reader lifetime code fault admitted.'}
    }
}
$listAfter=Read-RuntimeInput $ImagePath 16777216
if([Convert]::ToHexString([Security.Cryptography.SHA256]::HashData($listAfter))-cne $ownership.ImageSha256){throw 'Lifetime image changed during review.'}
$report=[pscustomobject]@{
    ImageSha256=$ownership.ImageSha256;InsertionVa='0x004CEA62';CleanupVa='0x004184B0';SourceListReleaseVa='0x004ABDEB';NodeCleanupVa='0x00497090';ReleaseThunkVa='0x004171C0'
    OwnerSentinelOffset=12;NodeReaderOffset=8;FreshNodePayloadRecoveredUnderHealthySentinelInvariant=$true;TraversalAfterPrepend='newest first'
    SourceListResetRestoresSelfLinks=$true;ConcreteReleaseVirtualSlot=8;ConcreteReleaseDelegatedSlot=60;ConcreteDeletingTargetVa='0x0049F800'
    ResourceCleanupFreesReaderObjectInReviewedBody=$false;ResourceCleanupClearsSourceField=24;ResourceCleanupClearsLinkedField=84
    ScopedResourceCleanupRecovered=$true;ScopedSourceListReleaseRecovered=$true;ConcreteReleaseConditionalObjectFreeRecovered=$true;DeletingTargetFullCacheLifetimeRecovered=$false
    AllListInitializersAndInvariantsProven=$false;ArbitraryCallbackSafetyProven=$false;AllSourceObjectBindingsRecovered=$false;CompleteStreamPointerProvenanceRecovered=$false
    ProductionBuildReady=$false;ModPackageLoaded=$false;ReadOnly=$true;TargetExecuted=$false;ManagedInspectorHashCommandExecuted=$ownership.ManagedInspectorHashCommandExecuted
    Scenarios=$scenarios;FaultTestsPassed=[bool]$SelfTest;InsertionScenarioFixturesExecuted=$(if($SelfTest){4}else{0});BrokenListFixturesExecuted=$(if($SelfTest){2}else{0});MemoryFaultsExecuted=$(if($SelfTest){12}else{0})
}
if($AsJson){ConvertTo-Json -InputObject $report -Depth 7}else{$report}
