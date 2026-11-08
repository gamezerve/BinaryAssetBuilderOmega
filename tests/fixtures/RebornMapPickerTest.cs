// Reborn: exercise the real checklist and owned selection-file handoff without starting a mod build or touching user mods.
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using Reborn;

internal sealed class MapPickerForm : Form
{
    private readonly ComboBox cmbMods = new ComboBox();
    private readonly Dictionary<string,Control> generatedModGUIData = new Dictionary<string,Control>();
    internal readonly CheckBox Maps = new CheckBox { Text = "Build Maps", Checked = true };
    internal readonly TableLayoutPanel Options = new TableLayoutPanel { Width = 350, Height = 70, ColumnCount = 1 };

    //-------------------------------------------------------------------------------------------------
    /** Reborn: reproduce EA's named fields and reserved option row on an invisible private form. */
    //-------------------------------------------------------------------------------------------------
    internal MapPickerForm()
    {
        Width = 500; Height = 600;
        var parent = new Panel { Width = 400, Height = 400 };
        Controls.Add(parent); parent.Controls.Add(Options);
        Options.Controls.Add(Maps, 0, 0);
        Options.Controls.Add(new Label { Text = "Official maps to build", AutoSize = true }, 0, 1);
        Options.Controls.Add(new CheckBox { Text = "Create BIG Files" }, 0, 2);
        generatedModGUIData.Add("buildmaps", Maps);
        cmbMods.Items.AddRange(new object[] { "Alpha", "Beta" }); cmbMods.SelectedIndex = 0;
    }

    internal void ChooseMod(int index) { cmbMods.SelectedIndex = index; }
}

internal static class RebornMapPickerTest
{
    private static int checks;
    //-------------------------------------------------------------------------------------------------
    /** Reborn: test All, multiple maps, empty selections, refresh, mod switches and session-owned large selection files. */
    //-------------------------------------------------------------------------------------------------
    [STAThread]
    private static int Main(string[] args)
    {
        try
        {
            string root = Path.GetFullPath(args[0]);
            if (Directory.Exists(root)) throw new Exception("Fresh fixture SDK required.");
            string official = Path.GetFullPath(Path.Combine(root, "mods/Alpha/Data/Maps/Official"));
            string[] names = { "All", "CAMP_A01_BrightonBeach_Smith", "CAMP_S01_LeningradCanals_Rao", "TUTORIAL_AICommander_Smith", "Map with spaces;Unicode_\u00dc", new String('L', 80) };
            foreach (string name in names) { Directory.CreateDirectory(Path.Combine(official,name)); File.WriteAllText(Path.Combine(official,name,"map.xml"), "<!-- Reborn: map selector fixture only. --><AssetDeclaration/>"); }
            string beta = Path.Combine(root,"mods/Beta/Data/Maps/Official/BetaMap"); Directory.CreateDirectory(beta); File.WriteAllText(Path.Combine(beta,"map.xml"), "<!-- Reborn: map selector fixture only. --><AssetDeclaration/>");
            using (var form = new MapPickerForm())
            {
                var flags = BindingFlags.Static | BindingFlags.NonPublic;
                var attach = typeof(StudioLifecycle).GetMethod("AttachMapPicker",flags);
                Check((bool)attach.Invoke(null,new object[] { form,root }), "checklist attachment");
                var list = (CheckedListBox)typeof(StudioLifecycle).GetField("mapPicker",flags).GetValue(null);
                // Reborn: the checklist must grow with its pane and the window must keep the build controls visible.
                Check(form.MinimumSize.Width>=960 && form.MinimumSize.Height>=560,"minimum window fits map controls");
                int initialWidth=list.Width; form.Options.Parent.Width+=120;
                Check(list.Width>=initialWidth+120,"checklist follows options width");
                Check(list.Items.Count==7 && list.GetItemChecked(0) && StudioLifecycle.GetSelectedMapDirectories()==null,"All is first and default");
                Check(list.CheckOnClick && list.HorizontalScrollbar && list.HorizontalExtent>list.Width,"clickable checks and long-name scrolling");
                list.SetItemChecked(2,true); list.SetItemChecked(3,true);
                string[] selected=StudioLifecycle.GetSelectedMapDirectories();
                Check(!list.GetItemChecked(0) && selected.Length==2,"multiple maps without All");
                Check(Array.IndexOf(selected,Path.Combine(official,"CAMP_A01_BrightonBeach_Smith"))>=0 && Array.IndexOf(selected,Path.Combine(official,"CAMP_S01_LeningradCanals_Rao"))>=0,"exact checked paths");
                string token=StudioLifecycle.Begin(); string payload=StudioLifecycle.SaveMapSelection(root,selected);
                Check(File.ReadAllLines(payload).Length==2,"owned selection handoff");
                File.Copy(payload,Path.Combine(root,"selection-for-backend.txt"));
                StudioLifecycle.Complete(); Check(!File.Exists(payload),"completed selection cleanup");
                token=StudioLifecycle.Begin(); var many=new string[2000]; for(int i=0;i<many.Length;i++)many[i]=Path.Combine(official,"LargeSelection_"+i);
                payload=StudioLifecycle.SaveMapSelection(root,many); Check(new FileInfo(payload).Length>32767 && File.ReadAllLines(payload).Length==2000,"selection exceeds command-line length safely");
                StudioLifecycle.Complete(); Check(!File.Exists(payload),"large selection cleanup");
                token=StudioLifecycle.Begin(); string occupied=Path.Combine(root,".work/map-selection-"+token+".txt"); File.WriteAllText(occupied,"Reborn: unrelated sentinel");
                bool refused=false; try { StudioLifecycle.SaveMapSelection(root,selected); } catch(IOException) { refused=true; }
                StudioLifecycle.Complete(); Check(refused && File.ReadAllText(occupied)=="Reborn: unrelated sentinel","pre-existing selection file preserved"); File.Delete(occupied);
                list.SetItemChecked(0,true); Check(list.CheckedItems.Count==1 && StudioLifecycle.GetSelectedMapDirectories()==null,"All clears individual maps");
                list.SetItemChecked(1,true); Check(StudioLifecycle.GetSelectedMapDirectories()[0]==Path.Combine(official,"All"),"All-named folder is a real map");
                list.SetItemChecked(1,false); refused=false; try { StudioLifecycle.GetSelectedMapDirectories(); } catch(InvalidDataException) { refused=true; } Check(refused,"empty selection rejected");
                var refresh=typeof(StudioLifecycle).GetMethod("RefreshMapPicker",flags);
                refresh.Invoke(null,new object[] { root,"Alpha",new string[0] }); Check(list.CheckedItems.Count==0,"refresh preserves empty choice");
                string missing=Path.Combine(official,"CAMP_A01_BrightonBeach_Smith"); File.Delete(Path.Combine(missing,"map.xml"));
                refresh.Invoke(null,new object[] { root,"Alpha",new string[] { missing } });
                Check(StudioLifecycle.GetSelectedMapDirectories()[0]==missing && !list.GetItemChecked(0),"missing selected source never expands to All");
                File.WriteAllText(Path.Combine(missing,"map.xml"),"<!-- Reborn: restored selector fixture. --><AssetDeclaration/>");
                form.Maps.Checked=false; Check(!list.Enabled,"Build Maps disables checklist");
                form.Maps.Checked=true; Check(list.Enabled,"Build Maps enables checklist");
                form.ChooseMod(1); Check(list.Items.Count==2 && StudioLifecycle.GetSelectedMapDirectories()==null,"mod switch resets to All");
                form.ChooseMod(0); Check(list.Items.Count==7 && StudioLifecycle.GetSelectedMapDirectories()==null,"returning mod refreshes its own maps");
            }
            Console.WriteLine("Reborn: map checklist PASS ("+checks+" checks; no user mods or native map builds).");
            return 0;
        }
        catch(Exception error) { Console.WriteLine(error); return 1; }
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: every failure names the selection contract that regressed. */
    //-------------------------------------------------------------------------------------------------
    private static void Check(bool condition,string name) { if(!condition)throw new Exception(name);checks++; }
}
