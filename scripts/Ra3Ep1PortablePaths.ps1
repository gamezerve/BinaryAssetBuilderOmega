# Reborn: resolve bundled SDK inputs from the copied SDK root, independently of the current working directory or repository.
function Get-RebornSdkRoot([string]$ScriptDirectory) {
    $parent=Split-Path -Parent $ScriptDirectory
    if(Test-Path -LiteralPath (Join-Path $parent 'uprising-alpha.paths')){return [IO.Path]::GetFullPath($parent)}
    return [IO.Path]::GetFullPath((Join-Path $parent 'alpha'))
}

# Reborn: all configured locations are literal paths relative to Studio unless explicitly absolute for legacy developer configurations.
function Initialize-RebornSdkWorkDirectories([string]$Sdk) {
    # Reborn: fresh portable copies can omit disposable folders; create them after checking the existing SDK ancestry.
    for($item=Get-Item -LiteralPath $Sdk;$null-ne $item;$item=$item.Parent){
        if(($item.Attributes-band [IO.FileAttributes]::ReparsePoint)-ne 0){throw 'Redirected SDK working directory refused.'}
    }
    foreach($name in @('.work','builtmods')){
        $path=Join-Path $Sdk $name
        if(Test-Path -LiteralPath $path){
            $item=Get-Item -LiteralPath $path -Force
            if(-not $item.PSIsContainer -or ($item.Attributes-band [IO.FileAttributes]::ReparsePoint)-ne 0){throw ('Invalid SDK working directory: '+$path)}
        }else{$null=New-Item -ItemType Directory -Path $path}
    }
}

# Reborn: all configured locations are literal paths relative to Studio unless explicitly absolute for legacy developer configurations.
function Read-RebornSdkPaths([string]$Sdk) {
    $settings=@{}
    foreach($line in Get-Content -LiteralPath (Join-Path $Sdk 'uprising-alpha.paths')){
        if(-not $line -or $line.StartsWith('#')){continue}
        $pair=$line.Split('=',2);if($pair.Count-ne 2 -or $settings.ContainsKey($pair[0])){throw 'Invalid or duplicate SDK path setting.'}
        $value=$pair[1];if(-not [IO.Path]::IsPathRooted($value)){$value=Join-Path $Sdk $value}
        $settings[$pair[0]]=[IO.Path]::GetFullPath($value)
    }
    # Reborn: apphost resolution uses only the bundled x86 runtime; PowerShell itself carries its own separate runtime.
    if(Test-Path -LiteralPath (Join-Path $Sdk 'runtime/dotnet-x86/dotnet.exe')){
        $env:DOTNET_ROOT_X86=Join-Path $Sdk 'runtime/dotnet-x86';$env:DOTNET_ROOT=$env:DOTNET_ROOT_X86
        ${env:DOTNET_ROOT(x86)}=$env:DOTNET_ROOT_X86;$env:DOTNET_MULTILEVEL_LOOKUP='0'
        $env:REBORN_PORTABLE_SDK_ROOT=$Sdk
        # Reborn: native audio dependencies are SDK-local and take precedence only inside this owned build process.
        $vc=Join-Path $Sdk 'runtime/vc-x86';if(Test-Path -LiteralPath $vc){$env:PATH=$vc+';'+$env:PATH}
    }
    return $settings
}
