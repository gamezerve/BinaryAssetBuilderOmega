// Reborn: emulate only the inspected Studio CodeDOM/interface/config seam under CLR4; no GUI, build callback, registry or game execution.
using System;
using System.CodeDom.Compiler;
using System.IO;
using Microsoft.CSharp;
using EALABuild;

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
                if (adapter.GetModBuildGUIData().Count != 3 || !adapter.shouldExecuteStep(1) || adapter.shouldExecuteStep(2))
                    throw new InvalidDataException("Adapter options/step scope differs.");
                Console.WriteLine("Reborn: exact-reference CLR4 CodeDOM/interface/config/options PASS; no Studio window or build/game execution.");
            }
            return 0;
        }
        catch (Exception error) { Console.WriteLine("Reborn: loader harness failed: " + error); return 1; }
    }
}
