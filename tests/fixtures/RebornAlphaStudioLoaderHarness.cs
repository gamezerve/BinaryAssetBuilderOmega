// Reborn: emulate only the inspected Studio CodeDOM/interface/config seam under CLR4; no GUI, build callback, registry or game execution.
using System;
using System.CodeDom.Compiler;
using System.IO;
using Microsoft.CSharp;
using EALABuild;
using System.Reflection;
using System.Collections.Generic;

class RebornAlphaStudioLoaderHarness
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: compile with the exact two declared EA loader references, instantiate the adapter and validate its configuration/options without running a build. */
    //-------------------------------------------------------------------------------------------------
    static int Main(string[] args)
    {
        try
        {
            if (args.Length != 1) throw new InvalidDataException("Expected one local alpha workspace.");
            Directory.SetCurrentDirectory(Path.GetFullPath(args[0]));
            using (CSharpCodeProvider provider = new CSharpCodeProvider())
            {
                CompilerParameters parameters = new CompilerParameters(new string[] { "System.dll", "EALABuild.dll" });
                parameters.GenerateExecutable = false; parameters.GenerateInMemory = true;
                CompilerResults result = provider.CompileAssemblyFromFile(parameters, new string[] { "defaultscript.cs" });
                foreach (CompilerError error in result.Errors) Console.WriteLine(error.ToString());
                if (result.Errors.Count != 0) throw new InvalidDataException("Exact Studio loader compilation failed.");
                EALABuildScript adapter = null;
                foreach (Type type in result.CompiledAssembly.GetTypes())
                    if (typeof(EALABuildScript).IsAssignableFrom(type)) adapter = (EALABuildScript)type.GetConstructor(Type.EmptyTypes).Invoke(new object[0]);
                if (adapter == null) throw new InvalidDataException("EA loader interface discovery failed.");
                adapter.initialize();
                if (adapter.GetModBuildGUIData().Count != 5 || !adapter.shouldExecuteStep(1) || adapter.shouldExecuteStep(2))
                    throw new InvalidDataException("Adapter options/step scope differs.");
                // Reborn: test the exact GUI parser with optional localization config, not only the newer backend parser.
                MethodInfo readPaths = adapter.GetType().GetMethod("ReadPaths", BindingFlags.NonPublic | BindingFlags.Static);
                string actual = File.ReadAllText("uprising-alpha.paths");
                string probe = Path.Combine(".work", "studio-config-check-" + Guid.NewGuid().ToString("N"));
                Directory.CreateDirectory(probe);
                string optional = actual.Contains("StockStrings=") ? actual : actual + Environment.NewLine + "StockStrings=";
                string accepted = Path.Combine(probe, "accepted.paths"); File.WriteAllText(accepted, optional);
                Dictionary<string, string> values = (Dictionary<string, string>)readPaths.Invoke(null, new object[] { accepted });
                if (!values.ContainsKey("StockStrings")) throw new InvalidDataException("GUI lost optional localization configuration.");
                foreach (string extra in new string[] { "StockStrings=duplicate", "RebornUnknownKey=invalid" })
                {
                    string rejected = Path.Combine(probe, "rejected.paths"); File.WriteAllText(rejected, optional + Environment.NewLine + extra);
                    bool refused = false;
                    try { readPaths.Invoke(null, new object[] { rejected }); }
                    catch (TargetInvocationException error) { refused = error.InnerException is InvalidDataException; }
                    if (!refused) throw new InvalidDataException("GUI configuration duplicate/unknown key was not rejected.");
                }
                Console.WriteLine("Reborn: exact-reference CLR4 CodeDOM/interface/config/options PASS; no Studio window or build/game execution.");
            }
            return 0;
        }
        catch (Exception error) { Console.WriteLine("Reborn: loader harness failed: " + error); return 1; }
    }
}
