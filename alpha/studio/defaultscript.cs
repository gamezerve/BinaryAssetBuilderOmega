// Reborn: prototype adapter for the existing EA Mod BuildStudio interface; preserve reference SDK files and never install or launch a game.
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using EALABuild;

namespace EALABuild
{
    class BuildScript : EALABuildScript
    {
        private string runtime;
        private string driver;

        //-------------------------------------------------------------------------------------------------
        /** Reborn: load explicit local adapter configuration instead of RA3 registry paths or inherited destructive cleanup stages. */
        //-------------------------------------------------------------------------------------------------
        public void initialize()
        {
            BuildHelper.AutomaticallyResetBuildHelper = true;
            BuildHelper.EnableBenchmarking = true;
            XmlDocument settings = new XmlDocument();
            settings.XmlResolver = null;
            settings.Load(Path.Combine(StaticPaths.CurrentDirectory, "uprising-alpha.xml"));
            runtime = settings.DocumentElement.GetAttribute("PowerShell7");
            driver = settings.DocumentElement.GetAttribute("Driver");
            CheckPath(runtime); CheckPath(driver);
        }

        //-------------------------------------------------------------------------------------------------
        /** Reborn: keep the familiar selected-mod build action but invoke only the reviewed alpha backend; unsupported input must return failure. */
        //-------------------------------------------------------------------------------------------------
        public void buildMod(string modName, Dictionary<string, Object> gui)
        {
            string name = BuildHelper.BuildTarget;
            if (String.IsNullOrEmpty(name) || name.Length > 15 || name.IndexOfAny(new char[] { '\\', '/', '"', ':', ';' }) >= 0 || name == "." || name == "..")
                throw new InvalidDataException("Reborn: invalid alpha mod name.");
            string input = Path.Combine(Path.Combine(Path.Combine(StaticPaths.CurrentDirectory, "mods"), name), "data\\mod.xml");
            CheckPath(input);
            BuildHelper.CurrentBuildSteps = 1;
            BuildHelper.DisplayLine("Reborn: Uprising SDK Alpha; supported build: ConYard GUN 5000 only. Other capabilities are not silently compiled.");
            RunExecutableArguments request = new RunExecutableArguments(runtime);
            request.Arguments = "-NoProfile -File \"" + driver + "\" -ModXml \"" + input + "\"";
            request.RedirectOutput = true; request.CreateWindow = false;
            // Reborn: retain the SDK's step-counter protocol before RunStep advances it for the callback.
            if (BuildHelper.GetNextExecutableStep() != 1)
                throw new InvalidDataException("Reborn: unexpected alpha build step state.");
            BuildHelper.RunStep(StepType.RunExecutable, request);
        }

        //-------------------------------------------------------------------------------------------------
        /** Reborn: complete the single backend operation without advancing to unvalidated RA3 postprocessors. */
        //-------------------------------------------------------------------------------------------------
        public void onStepSuccess(int stepId) { BuildHelper.OnBuildComplete(); }

        //-------------------------------------------------------------------------------------------------
        /** Reborn: make an unsuccessful backend visible and retain diagnostic output without automatic retry or cleanup. */
        //-------------------------------------------------------------------------------------------------
        public void onStepFailure(int stepId) { BuildHelper.DisplayLine("Reborn: alpha build failed; no install or launch was attempted. Review the build output."); }

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
            options.Add(new GUIElementData("scope", "Only AlliedConYardArmor GUN 5000 is package-enabled", GUIElementData.LabelType, ScriptIndex.ModScript));
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
    }
}
