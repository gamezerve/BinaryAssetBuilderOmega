using System.Text.Json;
using BinaryAssetBuilder.Core;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: the corpus runner amortizes process startup only; each request still executes the complete production command and its proofs.
internal static class SdkStockCompileWorker
{
    internal sealed record Request(string[] Arguments,string Log,string Work);

    //-------------------------------------------------------------------------------------------------
    /** Reborn: execute a bounded synchronous sequence, restoring console, settings and temporary paths after every command. */
    //-------------------------------------------------------------------------------------------------
    internal static int Run(string sdk)
    {
        sdk=SdkEnvironmentPreflight.DirectoryPath(sdk);
        string workRoot=Path.Combine(sdk,".work")+Path.DirectorySeparatorChar;
        using var schemaCatalog=SdkEffectiveSchema.ReuseAdmittedCatalog();
        var output=Console.Out;
        for(int count=0;count<32;count++)
        {
            string? line=Console.ReadLine();if(line==null)return 0;
            var request=JsonSerializer.Deserialize<Request>(line)??throw new InvalidDataException("Stock worker request required.");
            if(request.Arguments.Length==0||request.Arguments[0] is not ("conscript-source-prepare" or "conscript-alpha-build"))throw new InvalidDataException("Only stock qualification commands are admitted.");
            string log=Confined(request.Log),work=Confined(request.Work);
            var savedOut=Console.Out;var savedError=Console.Error;var savedSettings=Settings.Current;
            string? savedTemp=Environment.GetEnvironmentVariable("TEMP"),savedTmp=Environment.GetEnvironmentVariable("TMP");
            int code;
            using(var writer=new StreamWriter(log,false))
            {
                try
                {
                    Environment.SetEnvironmentVariable("TEMP",work);Environment.SetEnvironmentVariable("TMP",work);
                    Console.SetOut(writer);Console.SetError(writer);
                    code=Program.Main(request.Arguments);
                    SdkEffectiveSchema.VerifyReusedCatalog();
                }
                finally
                {
                    Console.SetOut(savedOut);Console.SetError(savedError);Settings.Current=savedSettings;
                    Environment.SetEnvironmentVariable("TEMP",savedTemp);Environment.SetEnvironmentVariable("TMP",savedTmp);
                }
            }
            output.WriteLine(code);output.Flush();
            // Reborn: discard per-command graphs before another file; no generated payload or proof result is reused.
            GC.Collect();GC.WaitForPendingFinalizers();GC.Collect();
        }
        return 0;

        //-------------------------------------------------------------------------------------------------
        /** Reborn: diagnostics and temporary files remain below the selected SDK's private work directory. */
        //-------------------------------------------------------------------------------------------------
        string Confined(string path)
        {
            string full=SdkEnvironmentPreflight.Absolute(path);
            if(!full.StartsWith(workRoot,StringComparison.OrdinalIgnoreCase))throw new InvalidDataException("Stock worker output escapes SDK work.");
            SdkEnvironmentPreflight.CheckPath(full);return full;
        }
    }
}
