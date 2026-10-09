# Reborn: recover scoped provider-open path forwarding and default search-path traversal without running native providers or asserting runtime package success.
[CmdletBinding()]
param([Parameter(Mandatory=$true)] [string] $ImagePath,[Parameter(Mandatory=$true)] [string] $BaselineImagePath,[Parameter(Mandatory=$true)] [string] $SkuDefinitionPath,[switch] $SelfTest,[switch] $AsJson)
$ErrorActionPreference='Stop'
$forwardSelfTest=$SelfTest;$forwardAsJson=$AsJson
$routing=. (Join-Path $PSScriptRoot 'Get-Ra3Ep11ProviderRouting.ps1') -ImagePath $ImagePath -BaselineImagePath $BaselineImagePath -SkuDefinitionPath $SkuDefinitionPath
$SelfTest=$forwardSelfTest;$AsJson=$forwardAsJson

#-------------------------------------------------------------------------------------------------
<# Reborn: pin the complete wrapper-open and existence-probe bodies plus the exact search-root join literal; no native code is executed. #>
#-------------------------------------------------------------------------------------------------
function Assert-Ep11ProviderOpenForwardingCode([byte[]] $Bytes) {
    foreach($slice in @(
        [pscustomobject]@{Offset=0x56ba00;Length=412;Hash='894A9C4145BA11C8C7799071E54FBADFC533405A9499551C77A8DD3C251E0EC6'},
        [pscustomobject]@{Offset=0x56abc0;Length=58;Hash='8A42716A8A44DAF5D9CCEA1A8AC3BF3CB8940646E9B89D93912A9B7B0C83717E'}
    )){
        if($slice.Offset+$slice.Length-gt $Bytes.Length){throw 'Provider-open slice outside image.'}
        if([Convert]::ToHexString([Security.Cryptography.SHA256]::HashData([byte[]]$Bytes[$slice.Offset..($slice.Offset+$slice.Length-1)]))-cne $slice.Hash){throw 'Reviewed provider-open forwarding changed.'}
    }
    if([Text.Encoding]::ASCII.GetString($Bytes,0x87f1e0,6)-cne ('%s/%s'+[char]0)){throw 'Provider search-root join literal differs.'}
}

#-------------------------------------------------------------------------------------------------
<# Reborn: model one leading dot-separator removal on bounded printable ASCII, leaving prefix case, interior separators and dot segments unchanged. #>
#-------------------------------------------------------------------------------------------------
function Get-Ep11DetachedForwardPath([string] $Path) {
    if($Path.Length-gt 255){throw 'Detached forwarding path exceeds policy bound.'}
    foreach($character in $Path.ToCharArray()){if([int]$character-lt 32 -or [int]$character-gt 126){throw 'Detached forwarding path requires printable ASCII.'}}
    if($Path.StartsWith('./',[StringComparison]::Ordinal) -or $Path.StartsWith('.\',[StringComparison]::Ordinal)){return $Path.Substring(2)}
    return $Path
}

#-------------------------------------------------------------------------------------------------
<# Reborn: model resolved-provider attempts with supplied hits only; default-root null terminates iteration, root-leading queries bypass joining and no raw default fallback is invented. #>
#-------------------------------------------------------------------------------------------------
function Get-Ep11DetachedOpenPlan([string] $Path,[bool] $DefaultSearch,[object[]] $Targets) {
    $forward=Get-Ep11DetachedForwardPath $Path
    if(-not $DefaultSearch -and $Targets.Count-ne 1){throw 'Explicit detached plan needs exactly one supplied provider.'}
    $attempts=[Collections.Generic.List[object]]::new();$selected=$null;$nullRootStopped=$false
    foreach($target in $Targets){
        if($DefaultSearch -and $null-eq $target.Root){$nullRootStopped=$true;break}
        $name=$forward
        if($DefaultSearch -and -not ($forward.StartsWith('/',[StringComparison]::Ordinal) -or $forward.StartsWith('\',[StringComparison]::Ordinal))){
            $null=Get-Ep11DetachedForwardPath $target.Root
            $name=$target.Root+'/'+$forward
            if($name.Length-gt 255){throw 'Detached joined path exceeds policy bound.'}
        }
        $attempts.Add([pscustomobject]@{Provider=$target.Id;Path=$name;SuppliedHit=[bool]$target.Hit})
        if($target.Hit){$selected=$target.Id;break}
    }
    return [pscustomobject]@{Selected=$selected;Attempts=$attempts.ToArray();NullRootStopped=$nullRootStopped}
}
Assert-Ep11ProviderOpenForwardingCode $ep11Bytes
if($SelfTest){
    # Reborn: forward-path fixtures cover single dot-prefix removal while preserving interior characters and repeated dot prefixes.
    foreach($fixture in @(
        [pscustomobject]@{Input='./big:/data/file';Expected='big:/data/file'},[pscustomobject]@{Input='.\big:\data\file';Expected='big:\data\file'},
        [pscustomobject]@{Input='BIG:/data/file';Expected='BIG:/data/file'},[pscustomobject]@{Input='././data';Expected='./data'},
        [pscustomobject]@{Input='data/../file';Expected='data/../file'},[pscustomobject]@{Input='';Expected=''}
    )){if((Get-Ep11DetachedForwardPath $fixture.Input)-cne $fixture.Expected){throw 'Detached forwarded path differs.'}}
    $a=[pscustomobject]@{Id='a';Root='big:/root';Hit=$false};$b=[pscustomobject]@{Id='b';Root='second';Hit=$true}
    foreach($case in @('explicit','later-hit','first-hit','rooted','empty-list','null-root','all-miss','double-slash')){
        $x=$a.PSObject.Copy();$y=$b.PSObject.Copy();$targets=@($x,$y);$default=$true;$path='data/file';$selected='b';$count=2;$stop=$false;$firstPath='big:/root/data/file'
        switch($case){
            'explicit'{$default=$false;$targets=@($y);$path='BIG:\data\file';$count=1;$firstPath=$path}
            'first-hit'{$x.Hit=$true;$selected='a';$count=1}
            'rooted'{$path='/data/file';$firstPath=$path}
            'empty-list'{$targets=@();$selected=$null;$count=0}
            'null-root'{$x.Root=$null;$selected=$null;$count=0;$stop=$true}
            'all-miss'{$y.Hit=$false;$selected=$null}
            'double-slash'{$x.Root='big:/root/';$firstPath='big:/root//data/file'}
        }
        $plan=Get-Ep11DetachedOpenPlan $path $default $targets
        if($plan.Selected-cne $selected -or $plan.Attempts.Count-ne $count -or $plan.NullRootStopped-ne $stop -or ($count-gt 0 -and $plan.Attempts[0].Path-cne $firstPath)){throw "Detached provider-open plan differs: $case"}
    }
    # Reborn: compose previously reviewed routing and BIG query preparation without treating provider selection as a successful asset open.
    $providers=@([pscustomobject]@{Id='big';HasInterface=$true;Aliases=@('big:')})
    foreach($fixture in @(
        [pscustomobject]@{Input='big:/data/file';BigName='data/file';Prefix=$true},
        [pscustomobject]@{Input='BIG:/data/file';BigName='BIG:/data/file';Prefix=$false},
        [pscustomobject]@{Input='big:\data\file';BigName='data\file';Prefix=$true},
        [pscustomobject]@{Input='big://data/file';BigName='/data/file';Prefix=$true}
    )){
        $provider=Select-Ep11DetachedProvider $fixture.Input $providers $null
        $name=Get-Ep11DetachedForwardPath $fixture.Input;$query=Get-Ep11DetachedBigOpenQuery $name
        if($provider.Selected-cne 'big' -or $query.Name-cne $fixture.BigName -or $query.PrefixRemoved-ne $fixture.Prefix){throw 'Detached routing/forwarding/BIG preparation composition differs.'}
    }
    # Reborn: unsupported input and detached code/literal faults remain diagnostic rejections, never installed-file edits.
    foreach($path in @(([string][char]0),([string][char]233),('x'*256))){$rejected=$false;try{$null=Get-Ep11DetachedForwardPath $path}catch{$rejected=$true};if(-not $rejected){throw 'Unsupported forwarding input admitted.'}}
    foreach($offset in @(0x56ba27,0x56ba4c,0x56ba61,0x56ba7b,0x56ba87,0x56ba92,0x56bab8,0x56bae4,0x56baf9,0x56bb14,0x56bb35,0x56bb6c,0x56bb7c,0x56bb7d,0x56abcc,0x56abdc,0x56abea,0x87f1e0)){
        $fault=[byte[]]$ep11Bytes.Clone();$fault[$offset]=$fault[$offset]-bxor 1;$rejected=$false
        try{Assert-Ep11ProviderOpenForwardingCode $fault}catch{$rejected=$true};if(-not $rejected){throw 'Provider-open forwarding fault admitted.'}
    }
}
$forwardAfter=Read-RuntimeInput $ImagePath 16777216
if([Convert]::ToHexString([Security.Cryptography.SHA256]::HashData($forwardAfter))-cne $routing.ImageSha256){throw 'Provider-open image changed during review.'}
$report=[pscustomobject]@{
    ImageSha256=$routing.ImageSha256;WrapperOpenVa='0x0096BA00';ExistsProbeVa='0x0096ABC0';ReviewedWholeBodyPins=2;ReviewedLiteralPins=1
    WrapperHandleOffset=8;WrapperRegistrationOffset=12;WrapperInterfaceOffset=16;ProviderOpenVirtualSlotOffset=12;ProviderSizeVirtualSlotOffset=32
    RemovesSingleLeadingDotSeparator=$true;ExplicitProviderPreservesPrefixAndInteriorSeparators=$true;DefaultSearchCountContextOffset=56;DefaultSearchPairsContextOffset=60;DefaultSearchPairStride=8;DefaultSearchStopsAtNullRoot=$true
    DefaultRelativeJoinFormat='%s/%s';DefaultRootedQueryBypassesJoin=$true;DefaultFirstSuccessfulAttemptWins=$true;DefaultNoSearchEntriesMeansNoOpen=$true;DefaultRegistrationPairUsesNextNode=$true
    ExistsProbePassesNullQualificationOutput=$true;ExistsProbeOpenOption=0;ExistsProbeTestsWrapperHandle=$true;AsciiInsensitiveProviderRoutingDoesNotGuaranteeExactBigPrefixStripping=$true
    ScopedProviderOpenForwardingRecovered=$true;NativeWrapperNullRegistrationSafetyProven=$false;NativeJoinedPathBufferSafetyProven=$false;ActualSearchPathContentsRecovered=$false;ActualStartupMountOrderRecovered=$false;GlobalFileProviderPrecedenceRecovered=$false;NativeStreamLifecycleRecovered=$false
    StockArchiveCount=$routing.StockArchiveCount;StockEntryCount=$routing.StockEntryCount;ArchiveDirectoryMetadataBytesRead=$routing.ArchiveDirectoryMetadataBytesRead;ArchivePayloadBytesRead=0
    ConfiguredStockArchiveSetComplete=$routing.ConfiguredStockArchiveSetComplete;MissingConfiguredArchives=$routing.MissingConfiguredArchives
    ReadOnly=$true;TargetExecuted=$false;ModPackageLoaded=$false;AuthenticEp1ProcessingHashRecovered=$false;ProductionBuildReady=$false
    FaultTestsPassed=[bool]$SelfTest;ForwardPathFixturesExecuted=$(if($SelfTest){6}else{0});OpenPlanFixturesExecuted=$(if($SelfTest){8}else{0});ComposedPrefixFixturesExecuted=$(if($SelfTest){4}else{0});PolicyRejectionsExecuted=$(if($SelfTest){3}else{0});MemoryFaultsExecuted=$(if($SelfTest){18}else{0})
}
if($AsJson){ConvertTo-Json -InputObject $report -Depth 8}else{$report}
