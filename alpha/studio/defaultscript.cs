// Reborn: prototype adapter for the existing EA Mod BuildStudio interface; preserve reference SDK files and never install or launch a game.
using System;
using System.Collections.Generic;
using System.IO;
using EALABuild;

namespace EALABuild
{
    class BuildScript : EALABuildScript
    {
        private string runtime;
        private string driver;
        // Reborn: require an explicit stock baseline location instead of inheriting the developer's default.
        private string staticManifest;
        // Reborn: optional legacy-config compatibility; weapon builds require an explicitly configured global manifest.
        private string globalManifest;
        // Reborn: standalone workspaces use checked prebuilt compiler and output directories, never hidden repository paths.
        private string compilerDirectory;
        private string outputRoot;
        // Reborn: the isolated CLR4 helper adds close protection without changing EA's executable or CodeDOM reference list.
        private Type lifecycle;
        private string buildHost;

        //-------------------------------------------------------------------------------------------------
        /** Reborn: load explicit local adapter configuration instead of RA3 registry paths or inherited destructive cleanup stages. */
        //-------------------------------------------------------------------------------------------------
        public void initialize()
        {
            BuildHelper.AutomaticallyResetBuildHelper = true;
            BuildHelper.EnableBenchmarking = true;
            // Reborn: EA's actual CodeDOM loader references only System.dll/EALABuild.dll, not System.Xml.dll.
            Dictionary<string, string> settings = ReadPaths(Path.Combine(StaticPaths.CurrentDirectory, "uprising-alpha.paths"));
            runtime = settings["PowerShell7"];
            driver = settings["Driver"];
            staticManifest = settings["StaticManifest"];
            globalManifest = settings.ContainsKey("GlobalManifest") ? settings["GlobalManifest"] : null;
            compilerDirectory = settings["CompilerDirectory"];
            outputRoot = Path.Combine(StaticPaths.CurrentDirectory, "builtmods");
            CheckPath(runtime); CheckPath(driver); CheckPath(staticManifest);
            if (globalManifest != null) CheckPath(globalManifest);
            CheckDirectory(compilerDirectory); CheckDirectory(outputRoot);
            buildHost = Path.Combine(StaticPaths.CurrentDirectory, "tools\\studio\\RebornStudioLifecycle.exe");
            CheckPath(buildHost);
            lifecycle = System.Reflection.Assembly.LoadFrom(buildHost).GetType("Reborn.StudioLifecycle", true);
            Lifecycle("Install");
        }

        //-------------------------------------------------------------------------------------------------
        /** Reborn: keep the familiar selected-mod build action but invoke only the reviewed alpha backend; unsupported input must return failure. */
        //-------------------------------------------------------------------------------------------------
        public void buildMod(string modName, Dictionary<string, Object> gui)
        {
            string name = BuildHelper.BuildTarget;
            // Reborn: the stable SDK uses a short fixed SKUDef basename, independent of the editable mod folder name.
            if (String.IsNullOrEmpty(name) || name.Length > 48 || name.IndexOfAny(new char[] { '\\', '/', '"', ':', ';', '\r', '\n' }) >= 0 || name == "." || name == "..")
                throw new InvalidDataException("Reborn: invalid alpha mod name.");
            string input = Path.Combine(Path.Combine(Path.Combine(StaticPaths.CurrentDirectory, "mods"), name), "data\\mod.xml");
            CheckPath(input);
            BuildHelper.CurrentBuildSteps = 1;
            // Reborn: separate the user's Reborn workspace from developer tests, including the strictly bounded complete Conscript build.
            BuildHelper.DisplayLine("Reborn: Uprising SDK Alpha; yard/AK47/armor-source profiles or original-ID Conscript with six editable scalars. Unsupported edits fail; new values need runtime testing.");
            // Reborn: session resources exist before EA starts the host; the suspended worker cannot escape its own job.
            string session = (string)Lifecycle("Begin");
            RunExecutableArguments request = new RunExecutableArguments(buildHost);
            // Reborn: bind every Studio build to the configured read-only stock baseline.
            request.Arguments = "--run " + session + " \"" + runtime + "\" -NoProfile -File \"" + driver + "\" -StudioSession " + session + " -ModXml \"" + input + "\" -StaticManifest \"" + staticManifest
                + "\" -CompilerDirectory \"" + compilerDirectory + "\" -OutputRoot \"" + outputRoot + "\"";
            // Reborn: preserve four-key armor workspaces without inferring a weapon dependency path.
            if (globalManifest != null) request.Arguments += " -GlobalManifest \"" + globalManifest + "\"";
            request.RedirectOutput = true; request.CreateWindow = false;
            // Reborn: retain the SDK's step-counter protocol before RunStep advances it for the callback.
            try
            {
                if (BuildHelper.GetNextExecutableStep() != 1)
                    throw new InvalidDataException("Reborn: unexpected alpha build step state.");
                BuildHelper.RunStep(StepType.RunExecutable, request);
            }
            catch { Lifecycle("Complete"); throw; }
        }

        //-------------------------------------------------------------------------------------------------
        /** Reborn: complete the single backend operation without advancing to unvalidated RA3 postprocessors. */
        //-------------------------------------------------------------------------------------------------
        public void onStepSuccess(int stepId) { Lifecycle("Complete"); BuildHelper.OnBuildComplete(); }

        //-------------------------------------------------------------------------------------------------
        /** Reborn: make an unsuccessful backend visible and retain diagnostic output without automatic retry or cleanup. */
        //-------------------------------------------------------------------------------------------------
        public void onStepFailure(int stepId) { Lifecycle("Complete"); BuildHelper.DisplayLine("Reborn: alpha build failed; no install or launch was attempted. Review the build output."); }

        //-------------------------------------------------------------------------------------------------
        /** Reborn: keep Windows Forms types in the separately compiled helper, preserving EA's exact two-reference CodeDOM contract. */
        //-------------------------------------------------------------------------------------------------
        private object Lifecycle(string method) { return lifecycle.GetMethod(method).Invoke(null, null); }

        //-------------------------------------------------------------------------------------------------
        /** Reborn: this alpha has exactly one indivisible validation/build/package operation. */
        //-------------------------------------------------------------------------------------------------
        public bool shouldExecuteStep(int stepId) { return stepId == 1; }

        //-------------------------------------------------------------------------------------------------
        /** Reborn: retain EA's options pane while explicitly describing this limited backend. */
        //-------------------------------------------------------------------------------------------------
        public List<GUIElementData> GetModBuildGUIData()
        {
            List<GUIElementData> options = new List<GUIElementData>();
            options.Add(new GUIElementData("alpha", "Uprising Alpha: Validate XML / Build Data / Make BIG", GUIElementData.LabelType, ScriptIndex.ModScript));
            options.Add(new GUIElementData("scope", "Fixed yard samples or 1..32 source armors with Includes/Defines/inheritFrom (experimental)", GUIElementData.LabelType, ScriptIndex.ModScript));
            options.Add(new GUIElementData("weapon", "Conscript: BuildTime / cost / health / vision / shroud / speed (bounded)", GUIElementData.LabelType, ScriptIndex.ModScript));
            return options;
        }

        //-------------------------------------------------------------------------------------------------
        /** Reborn: reject command-line quoting ambiguity and missing explicit local paths before requesting an external process. */
        //-------------------------------------------------------------------------------------------------
        private static void CheckPath(string path)
        {
            if (String.IsNullOrEmpty(path) || !Path.IsPathRooted(path) || path.IndexOf('"') >= 0 || path.IndexOf('\r') >= 0 || path.IndexOf('\n') >= 0 || !File.Exists(path))
                throw new InvalidDataException("Reborn: adapter requires existing absolute file paths.");
        }

        //-------------------------------------------------------------------------------------------------
        /** Reborn: require explicitly configured existing directories with unambiguous process argument quoting. */
        //-------------------------------------------------------------------------------------------------
        private static void CheckDirectory(string path)
        {
            if (String.IsNullOrEmpty(path) || !Path.IsPathRooted(path) || path.IndexOf('"') >= 0 || path.IndexOf('\r') >= 0 || path.IndexOf('\n') >= 0 || !Directory.Exists(path))
                throw new InvalidDataException("Reborn: adapter requires existing absolute directory paths.");
        }

        //-------------------------------------------------------------------------------------------------
        /** Reborn: require four legacy keys; optionally accept GlobalManifest and the official Conscript source root without implicit dependency defaults. */
        //-------------------------------------------------------------------------------------------------
        private static Dictionary<string, string> ReadPaths(string path)
        {
            if (new FileInfo(path).Length > 16384) throw new InvalidDataException("Reborn: alpha path configuration exceeds bound.");
            Dictionary<string, string> settings = new Dictionary<string, string>(StringComparer.Ordinal);
            foreach (string line in File.ReadAllLines(path))
            {
                if (line.Length == 0 || line.StartsWith("#", StringComparison.Ordinal)) continue;
                int separator = line.IndexOf('=');
                if (separator <= 0) throw new InvalidDataException("Reborn: invalid alpha path setting.");
                string key = line.Substring(0, separator);
                // Reborn: official source root is used only by the bounded Conscript build driver; legacy configurations remain valid.
                if ((key != "PowerShell7" && key != "Driver" && key != "StaticManifest" && key != "CompilerDirectory" && key != "GlobalManifest" && key != "UprisingXmlRoot") || settings.ContainsKey(key))
                    throw new InvalidDataException("Reborn: unknown or duplicate alpha path setting.");
                settings.Add(key, line.Substring(separator + 1));
            }
            if (!settings.ContainsKey("PowerShell7") || !settings.ContainsKey("Driver") || !settings.ContainsKey("StaticManifest") || !settings.ContainsKey("CompilerDirectory"))
                throw new InvalidDataException("Reborn: alpha requires all four base path settings; GlobalManifest enables weapon builds.");
            return settings;
        }
    }
}
