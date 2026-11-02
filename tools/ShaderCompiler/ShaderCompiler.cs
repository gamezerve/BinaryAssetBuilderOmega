using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

// Reborn: a dedicated local FX compiler keeps experimental shader authoring separate from normal Mod Studio compilation.
internal static class ShaderBuild
{
    internal static readonly string[] Samples={"ObjectsGDI.fx","ObjectsNOD.fx","ObjectsAlienIonHull.fx","ObjectsGenericEnvMap.fx"};
    internal static Process Active;
    internal static volatile bool Cancelled;

    //-------------------------------------------------------------------------------------------------
    /** Reborn: quote literal Windows paths for FXC, which is invoked directly with no command shell. */
    //-------------------------------------------------------------------------------------------------
    internal static string Quote(string value){if(value.IndexOf('"')>=0)throw new ArgumentException("Paths cannot contain quotation marks.");return "\""+value+(value.EndsWith("\\",StringComparison.Ordinal)?"\\":"")+"\"";}

    //-------------------------------------------------------------------------------------------------
    /** Reborn: cancel only this tool's active compiler child; no SDK or game process is enumerated or killed. */
    //-------------------------------------------------------------------------------------------------
    internal static void Cancel(){Cancelled=true;var child=Active;if(child!=null)try{if(!child.HasExited)child.Kill();}catch(InvalidOperationException){} }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: compile to a private temporary file and publish only after all requested validation passes. */
    //-------------------------------------------------------------------------------------------------
    internal static void Compile(string source,string output,string compiler,bool validate,Action<string> log)
    {
        // Reborn: stdout/stderr callbacks may arrive concurrently; keep file/UI log sinks serialized.
        var emit=log;var logGate=new object();log=delegate(string message){lock(logGate)emit(message);};
        source=Path.GetFullPath(source);output=Path.GetFullPath(output);compiler=Path.GetFullPath(compiler);
        if(!File.Exists(source)||!source.EndsWith(".fx",StringComparison.OrdinalIgnoreCase))throw new FileNotFoundException("Select an existing .fx source.",source);
        if(!File.Exists(compiler))throw new FileNotFoundException("Select the legacy FXC from the RA3 Mod SDK.",compiler);
        Directory.CreateDirectory(output);string final=Path.Combine(output,Path.GetFileNameWithoutExtension(source).ToLowerInvariant()+".fxo");
        string temporary=Path.Combine(output,".reborn-"+Guid.NewGuid().ToString("N")+".fxo");
        using(var gate=new FileStream(Path.Combine(output,".shadercompiler.lock"),FileMode.OpenOrCreate,FileAccess.ReadWrite,FileShare.None))try {
            if(Cancelled)throw new OperationCanceledException();
            log("Compiling "+source);log("Profile: fx_2_0; EA_PLATFORM_WINDOWS; _WW3D_; _WW3D_VERSION_=1; legacy /LD.");
            var info=new ProcessStartInfo(compiler,"/nologo /LD /T fx_2_0 /D EA_PLATFORM_WINDOWS=1 /D _WW3D_=1 /D _WW3D_VERSION_=1 /I "+Quote(Path.GetDirectoryName(source))+" /Fo "+Quote(temporary)+" "+Quote(source)) { UseShellExecute=false,CreateNoWindow=true,RedirectStandardOutput=true,RedirectStandardError=true,WorkingDirectory=Path.GetDirectoryName(source) };
            using(var process=new Process { StartInfo=info }) {
                process.OutputDataReceived+=delegate(object sender,DataReceivedEventArgs e){if(e.Data!=null)log(e.Data);};
                process.ErrorDataReceived+=delegate(object sender,DataReceivedEventArgs e){if(e.Data!=null)log(e.Data);};
                process.Start();Active=process;process.BeginOutputReadLine();process.BeginErrorReadLine();if(Cancelled)Cancel();process.WaitForExit();Active=null;
                if(Cancelled)throw new OperationCanceledException();if(process.ExitCode!=0||!File.Exists(temporary))throw new InvalidDataException("FXC failed; previous output was retained.");
            }
            if(validate)EffectValidator.Validate(temporary,log);else log("Warning: native effect-loader validation was skipped.");
            // Reborn: expose the known remaining gameplay gap rather than claiming that a successful effect load proves a complete port.
            string objects=Path.Combine(Path.GetDirectoryName(source),"Objects.fxh");if(File.Exists(objects)){
                var names=Regex.Matches(File.ReadAllText(objects),"USE_EXPRESSION_EVALUATOR\\(\"(Objects_[^\"]+)\"\\)").Cast<Match>().Select(m=>m.Groups[1].Value).Distinct().ToArray();
                if(names.Length>0)log("Warning: legacy evaluator names remain: "+string.Join(", ",names)+". These names are absent from Uprising; evaluator/technique migration and in-game visual validation are still required.");
            }
            if(Cancelled)throw new OperationCanceledException();
            if(File.Exists(final))File.Replace(temporary,final,null);else File.Move(temporary,final);
            string sha;using(var hash=SHA256.Create())using(var input=File.OpenRead(final))sha=BitConverter.ToString(hash.ComputeHash(input)).Replace("-","");
            File.WriteAllText(final+".receipt.txt","Reborn: experimental shader build\r\nSource="+source+"\r\nCompiler="+compiler+"\r\nEffectLoaderValidated="+validate+"\r\nRenderingValidated=False\r\nSHA256="+sha+"\r\n",Encoding.UTF8);
            log("Output: "+final);log("SHA256: "+sha);log("Rendering/gameplay: NOT VERIFIED. Original source trees and live mod packages were not changed.");
        }finally { Active=null;if(File.Exists(temporary))File.Delete(temporary); }
    }
}

// Reborn: keep the reusable compiler UI small, dark and explicit about the difference between loading and full game compatibility.
internal sealed class ShaderWindow : Form
{
    private readonly TextBox source=new TextBox(),output=new TextBox(),fxc=new TextBox(),log=new TextBox();
    private readonly CheckBox validate=new CheckBox { Text="Validate with Uprising DirectX loader",Checked=true,AutoSize=true };
    private readonly Button build=new Button { Text="Compile shader" },batch=new Button { Text="Compile 4 KW samples" },cancel=new Button { Text="Cancel",Enabled=false };
    private bool busy;

    //-------------------------------------------------------------------------------------------------
    /** Reborn: use SDK-relative local defaults; never hardcode the developer's reference source paths into the UI. */
    //-------------------------------------------------------------------------------------------------
    internal ShaderWindow()
    {
        Text="Reborn Shader Compiler — Uprising (experimental)";ClientSize=new Size(940,660);MinimumSize=new Size(760,520);BackColor=Color.FromArgb(30,30,30);ForeColor=Color.Gainsboro;
        var layout=new TableLayoutPanel { Dock=DockStyle.Fill,Padding=new Padding(12),ColumnCount=3,RowCount=7 };
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute,100));layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent,100));layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute,90));
        for(int i=0;i<5;i++)layout.RowStyles.Add(new RowStyle(SizeType.Absolute,i==4?50:34));layout.RowStyles.Add(new RowStyle(SizeType.Absolute,42));layout.RowStyles.Add(new RowStyle(SizeType.Percent,100));Controls.Add(layout);
        string root=AppDomain.CurrentDomain.BaseDirectory;source.Text=Path.Combine(root,"Sources","KW-EP1-LoaderCandidate","ObjectsGDI.fx");output.Text=Path.Combine(root,"Output");fxc.Text=Path.Combine(root,"fxc.exe");
        AddRow(layout,0,"FX source",source,false);AddRow(layout,1,"Output folder",output,true);AddRow(layout,2,"FXC",fxc,false);
        layout.Controls.Add(validate,1,3);layout.SetColumnSpan(validate,2);
        var note=new Label { Text="Experimental KW samples pass effect loading. Legacy evaluator/technique migration and in-game visual checks remain. Successful compilation alone does not prove full Uprising compatibility.",Dock=DockStyle.Fill,ForeColor=Color.FromArgb(220,170,70) };layout.Controls.Add(note,0,4);layout.SetColumnSpan(note,3);
        var buttons=new FlowLayoutPanel { Dock=DockStyle.Fill };foreach(var button in new[]{build,batch,cancel}){button.AutoSize=true;button.Height=30;buttons.Controls.Add(button);}layout.Controls.Add(buttons,0,5);layout.SetColumnSpan(buttons,3);
        log.Multiline=true;log.ReadOnly=true;log.ScrollBars=ScrollBars.Both;log.WordWrap=false;log.Dock=DockStyle.Fill;log.Font=new Font(FontFamily.GenericMonospace,9);layout.Controls.Add(log,0,6);layout.SetColumnSpan(log,3);
        foreach(var box in new[]{source,output,fxc,log}){box.BackColor=Color.FromArgb(43,43,43);box.ForeColor=Color.Gainsboro;box.BorderStyle=BorderStyle.FixedSingle;box.Dock=DockStyle.Fill;}
        build.Click+=delegate { Start(false); };batch.Click+=delegate { Start(true); };cancel.Click+=delegate { ShaderBuild.Cancel(); };FormClosing+=CloseRequested;
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: source/compiler file selection and output-folder selection use ordinary Windows pickers. */
    //-------------------------------------------------------------------------------------------------
    private void AddRow(TableLayoutPanel layout,int row,string title,TextBox field,bool folder)
    {
        layout.Controls.Add(new Label { Text=title,AutoSize=true,Padding=new Padding(0,5,0,0) },0,row);layout.Controls.Add(field,1,row);var browse=new Button { Text="Browse...",Dock=DockStyle.Fill };layout.Controls.Add(browse,2,row);
        browse.Click+=delegate { if(folder){using(var picker=new FolderBrowserDialog()){if(picker.ShowDialog(this)==DialogResult.OK)field.Text=picker.SelectedPath;}}else using(var picker=new OpenFileDialog { Filter=title=="FXC"?"FX compiler|fxc.exe":"FX source|*.fx|All files|*.*" }){if(picker.ShowDialog(this)==DialogResult.OK)field.Text=picker.FileName;} };
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: compile in a worker and keep log callbacks asynchronous so a close/cancel cannot deadlock with FXC output. */
    //-------------------------------------------------------------------------------------------------
    private void Start(bool all)
    {
        if(busy)return;string file=source.Text,folder=output.Text,compiler=fxc.Text;bool check=validate.Checked;busy=true;ShaderBuild.Cancelled=false;build.Enabled=batch.Enabled=false;cancel.Enabled=true;log.Clear();
        Task.Factory.StartNew(delegate {
            try { foreach(string path in all?ShaderBuild.Samples.Select(n=>Path.Combine(Path.GetDirectoryName(file),n)):new[]{file})ShaderBuild.Compile(path,folder,compiler,check,Write);Write("Completed."); }
            catch(Exception error){Write("Error: "+error.Message);}
            finally { if(!IsDisposed)try{BeginInvoke((Action)delegate { busy=false;build.Enabled=batch.Enabled=true;cancel.Enabled=false; });}catch(InvalidOperationException){} }
        });
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: ignore late worker messages after the form has closed. */
    //-------------------------------------------------------------------------------------------------
    private void Write(string message){if(IsDisposed||!IsHandleCreated)return;try{BeginInvoke((Action)delegate { log.AppendText(message+Environment.NewLine); });}catch(InvalidOperationException){} }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: prompt only while this program owns active work, and terminate only its compiler child on confirmed cancellation. */
    //-------------------------------------------------------------------------------------------------
    private void CloseRequested(object sender,FormClosingEventArgs e){if(!busy)return;if(MessageBox.Show(this,"Shader compilation is still running. Cancel it and close?","Reborn Shader Compiler",MessageBoxButtons.YesNo,MessageBoxIcon.Question)!=DialogResult.Yes)e.Cancel=true;else ShaderBuild.Cancel();}

    //-------------------------------------------------------------------------------------------------
    /** Reborn: provide private command-line verification while ordinary users launch the graphical program. */
    //-------------------------------------------------------------------------------------------------
    [STAThread] private static int Main(string[] args)
    {
        if(args.Length==3&&args[0]=="--validate") { try{EffectValidator.Validate(args[1],s=>File.AppendAllText(args[2],s+Environment.NewLine));return 0;}catch(Exception error){File.AppendAllText(args[2],"Error: "+error.Message+Environment.NewLine);return 1;} }
        if(args.Length==5&&args[0]=="--compile") { try{ShaderBuild.Compile(args[1],args[2],args[3],true,s=>File.AppendAllText(args[4],s+Environment.NewLine));return 0;}catch(Exception error){File.AppendAllText(args[4],"Error: "+error.Message+Environment.NewLine);return 1;} }
        Application.EnableVisualStyles();Application.SetCompatibleTextRenderingDefault(false);
        // Reborn: capture the form offscreen for developer layout verification without displaying a user window.
        if(args.Length==2&&args[0]=="--capture-ui"){using(var form=new ShaderWindow()){form.ShowInTaskbar=false;form.Opacity=0;form.Show();Application.DoEvents();using(var bitmap=new Bitmap(form.Width,form.Height)){form.DrawToBitmap(bitmap,new Rectangle(0,0,form.Width,form.Height));bitmap.Save(args[1],System.Drawing.Imaging.ImageFormat.Png);}form.Close();}return 0;}
        Application.Run(new ShaderWindow());return 0;
    }
}
