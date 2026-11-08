// Reborn: exercise the production Studio footer and lifecycle callbacks on the actual EA form without showing a window or starting a build.
using System;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;

class RebornStudioBuildStatusTest
{
    private static Type helper;
    private static Control indicator;
    private static int checks;
    //-------------------------------------------------------------------------------------------------
    /** Reborn: verify phase/map parsing, terminal states, UI-thread dispatch, resized footer geometry and actual rendered lamps. */
    //-------------------------------------------------------------------------------------------------
    [STAThread] static int Main(string[] args)
    {
        try
        {
            // Reborn: regression exceptions must reach the test log without a user-facing dialog.
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.ThrowException);
            Application.EnableVisualStyles(); Application.SetCompatibleTextRenderingDefault(false);
            Assembly.LoadFrom(Path.Combine(args[1],"EALABuild.dll"));
            var type=Assembly.LoadFrom(Path.Combine(args[1],"EALAModStudio.exe")).GetType("EALAModStudio.ModStudioUI");
            helper=Assembly.LoadFrom(args[0]).GetType("Reborn.StudioLifecycle");
            using(var form=(Form)Activator.CreateInstance(type,new object[]{new string[0]}))
            {
                form.ClientSize=new Size(1100,700); var handle=form.Handle;
                helper.GetMethod("Attach",BindingFlags.Static|BindingFlags.NonPublic).Invoke(null,new object[]{form});
                var footer=form.Controls["rebornBuildFooter"];indicator=footer.Controls["rebornBuildStatus"];
                var button=(Button)type.GetField("buttonBuildMod",BindingFlags.Instance|BindingFlags.NonPublic).GetValue(form);
                var split=(Control)type.GetField("splitContainer1",BindingFlags.Instance|BindingFlags.NonPublic).GetValue(form);
                var log=(RichTextBox)type.GetField("rtbGUIOutput",BindingFlags.Instance|BindingFlags.NonPublic).GetValue(form);
                Expect("Ready",Color.ForestGreen);
                foreach(var size in new[]{new Size(1100,700),new Size(960,560),new Size(1450,850)})
                {
                    form.ClientSize=size;form.PerformLayout();
                    Require(footer.Bottom==form.ClientSize.Height && split.Bottom<=footer.Top,"Footer overlaps the original content.");
                    Require(button.Parent==footer && indicator.Left>=button.Right && indicator.Right<=footer.ClientSize.Width,"Build button and status do not share a bounded bottom row.");
                }
                form.ClientSize=new Size(1100,700);form.PerformLayout();
                Invoke("BuildStarting");Expect("Starting build",Color.Gold);Require(!button.Enabled,"Build button remained enabled during a build.");
                log.AppendText("Step 4: Building AptUI\n");Expect("Compiling AptUI",Color.Gold);
                log.AppendText("Step 5: Building global data\n");Expect("Compiling global data",Color.Gold);
                log.AppendText("[00:01:02.234] Step 6: Building static data\n");Expect("Compiling static data",Color.Gold);
                log.AppendText("Warning: MISSINGART test\n");Expect("Compiling static data",Color.Gold);
                log.AppendText("Step 9: Building maps\nMap source: D:\\SDK\\mods\\Reborn\\Data\\Maps\\Official\\CAMP_S01_LeningradCanals_Rao\\map.xml\n");Expect("Compiling map: CAMP_S01_LeningradCanals_Rao",Color.Gold);
                log.AppendText("Map source: D:\\SDK with spaces\\Data\\Maps\\Official\\CAMP_A01_BrightonBeach_Smith\\map.");Expect("Compiling map: CAMP_S01_LeningradCanals_Rao",Color.Gold);
                log.AppendText("xml\n");Expect("Compiling map: CAMP_A01_BrightonBeach_Smith",Color.Gold);
                Save(form,Path.Combine(args[2],"running.png"));
                log.AppendText("Step 10: Creating big files\n");Expect("Creating BIG files",Color.Gold);
                log.AppendText("Step 11: Creating skudef file\n");Expect("Creating SKUDef",Color.Gold);
                Invoke("BuildSucceeded");Expect("Ready",Color.ForestGreen);Require(button.Enabled,"Build button was not restored after completion.");Save(form,Path.Combine(args[2],"ready.png"));
                Invoke("BuildStarting");log.AppendText("Error: ALPHA110 failed\nCritical: ALPHA111 Build Failed during Step 6\n");Expect("Stopped",Color.Firebrick);
                log.AppendText("Step time taken: 00:00:01\nStep 10: Creating big files\n");Expect("Stopped",Color.Firebrick);
                log.Clear();Expect("Stopped",Color.Firebrick);Invoke("BuildSucceeded");Expect("Stopped",Color.Firebrick);
                Save(form,Path.Combine(args[2],"stopped.png"));
                Invoke("BuildStarting");log.AppendText("Step 6: Building static data\nSkipped: [Optional] Static is unchecked.\n");Invoke("BuildSucceeded");Expect("Ready",Color.ForestGreen);
                Invoke("BuildFailed");Expect("Stopped",Color.Firebrick);
                Invoke("BuildStarting");Thread callback=new Thread(delegate(){Invoke("BuildFailed");});callback.Start();callback.Join();Application.DoEvents();Expect("Stopped",Color.Firebrick);
                Invoke("BuildStarting");Invoke("BuildSucceeded");Expect("Ready",Color.ForestGreen);
                // Reborn: terminal log lines must not enable the reparented button before EA releases its original disabled options owner.
                var originalOwner=(Control)type.GetField("flwModOptions",BindingFlags.Instance|BindingFlags.NonPublic).GetValue(form);
                originalOwner.Enabled=false;Invoke("BuildFailed");Require(!button.Enabled,"Native build disable was lost after reparenting.");
                originalOwner.Enabled=true;Require(button.Enabled,"Native build re-enable did not reach the footer button.");
                Require(indicator.AccessibleDescription==indicator.Text,"Status caption is unavailable to accessibility tools.");
            }
            Console.WriteLine("Reborn: Studio build status: PASS ("+checks+" checks; actual EA form; green idle/success, yellow stages/map names, red errors/preflight failure, warning tolerance, split lines, clear log, retry, callback threading, footer resizing; no visible window/build/game).");return 0;
        }
        catch(Exception error){Console.Error.WriteLine(error);return 1;}
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: invoke the public production completion callbacks without fabricating a worker result or starting a session. */
    //-------------------------------------------------------------------------------------------------
    static void Invoke(string name){helper.GetMethod(name).Invoke(null,null);}
    //-------------------------------------------------------------------------------------------------
    /** Reborn: inspect the same caption/color fields that the real painted footer consumes. */
    //-------------------------------------------------------------------------------------------------
    static void Expect(string text,Color color){Require(indicator.Text==text,"Status caption differs: "+indicator.Text+" expected "+text);var value=(Color)indicator.GetType().GetField("LampColor",BindingFlags.Instance|BindingFlags.NonPublic).GetValue(indicator);Require(value.ToArgb()==color.ToArgb(),"Status lamp differs for "+text);}
    //-------------------------------------------------------------------------------------------------
    /** Reborn: render the actual form offscreen so the footer can be inspected without opening the user's Studio. */
    //-------------------------------------------------------------------------------------------------
    static void Save(Form form,string path){var footer=form.Controls["rebornBuildFooter"];using(var image=new Bitmap(footer.Width,footer.Height))using(var graphics=Graphics.FromImage(image)){graphics.Clear(footer.BackColor);foreach(Control item in footer.Controls){using(var part=new Bitmap(item.Width,item.Height)){item.DrawToBitmap(part,new Rectangle(Point.Empty,part.Size));graphics.DrawImageUnscaled(part,item.Left,item.Top);}}image.Save(path);}}
    //-------------------------------------------------------------------------------------------------
    /** Reborn: fail the private fixture immediately when a live footer state or geometry differs. */
    //-------------------------------------------------------------------------------------------------
    static void Require(bool condition,string message){if(!condition)throw new Exception(message);checks++;}
}
