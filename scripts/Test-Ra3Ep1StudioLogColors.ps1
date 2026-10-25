# Reborn: exercise actual RichTextBox recoloring against EA's configured warning color without opening Studio or starting a build.
[CmdletBinding()]
param([Parameter(Mandatory)][string]$OutputDirectory,[string]$HelperPath)
$ErrorActionPreference='Stop';$repo=Split-Path -Parent $PSScriptRoot;$sdk=Join-Path $repo 'alpha';$output=[IO.Path]::GetFullPath($OutputDirectory)
if([IO.Path]::GetDirectoryName($output)-ine (Join-Path $sdk '.work') -or (Test-Path -LiteralPath $output)){throw 'Use a fresh direct child of alpha/.work.'};$null=New-Item -ItemType Directory -Path $output
$code=@'
// Reborn: a private hidden form supplies EA's output field name to the production lifecycle helper.
using System;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;
class ColorForm:Form { public RichTextBox rtbGUIOutput=new RichTextBox(); public ColorForm(){Controls.Add(rtbGUIOutput);} }
class RebornColorCheck {
    //-------------------------------------------------------------------------------------------------
    /** Reborn: optional skips follow the configured warning palette; unsupported skips stay red and timings stay green. */
    //-------------------------------------------------------------------------------------------------
    [STAThread] static int Main(string[] args){try{
        var ea=Assembly.LoadFrom(args[1]).GetType("EALABuild.BuildHelper");var field=ea.GetField("warningColor",BindingFlags.Static|BindingFlags.Public|BindingFlags.NonPublic);Color original=(Color)field.GetValue(null);var helper=Assembly.LoadFrom(args[0]).GetType("Reborn.StudioLifecycle");
        using(var form=new ColorForm()){helper.GetMethod("Attach",BindingFlags.Static|BindingFlags.NonPublic).Invoke(null,new object[]{form});var box=form.rtbGUIOutput;
            box.AppendText("Skipped: [Optional] Build Global Data is unchecked.\nSkipped: [No input] Empty source.\nSkipped: Unsupported stage.\nStep time taken: 00:00:01 [12:00:00]\n");
            Check(box,"Skipped: [Optional]",original);Check(box,"Skipped: [No input]",original);Check(box,"Skipped: Unsupported",Color.Red);Check(box,"Step time taken:",Color.Green);
            try{Color custom=Color.FromArgb(150,105,35);field.SetValue(null,custom);box.Clear();box.AppendText("Skipped: [Optional] Custom palette.\n");Check(box,"Skipped: [Optional]",custom);}finally{field.SetValue(null,original);}
        }Console.WriteLine("Reborn: Studio colors: PASS (optional/no-input skips match EA warning color and palette changes; unsupported red, timings green; no visible window).");return 0;
        }catch(Exception error){Console.Error.WriteLine(error);return 1;}
    }
    // Reborn: inspect stored formatting rather than a screenshot or a duplicate color-selection implementation.
    static void Check(RichTextBox box,string text,Color expected){int at=box.Text.IndexOf(text,StringComparison.Ordinal);if(at<0)throw new Exception("Missing fixture line");box.Select(at,text.Length);if(box.SelectionColor.ToArgb()!=expected.ToArgb())throw new Exception("Color mismatch: "+text+"; actual="+box.SelectionColor+", expected="+expected);}
}
'@
$source=Join-Path $output 'ColorCheck.cs';$exe=Join-Path $output 'ColorCheck.exe';[IO.File]::WriteAllText($source,$code)
& (Join-Path $env:WINDIR 'Microsoft.NET/Framework/v4.0.30319/csc.exe') /nologo /target:exe /platform:anycpu /reference:System.Windows.Forms.dll /reference:System.Drawing.dll ('/out:'+$exe) $source
if($LASTEXITCODE-ne 0){throw 'Color harness failed to compile.'};if(-not $HelperPath){$HelperPath=Join-Path $sdk 'tools/studio/RebornStudioLifecycle.exe'};& $exe $HelperPath (Join-Path $sdk 'EALABuild.dll');if($LASTEXITCODE-ne 0){throw 'Studio color regression failed.'}
