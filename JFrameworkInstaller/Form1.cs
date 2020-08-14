using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using dnlib.DotNet;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Rocks;

namespace JFrameworkInstaller
{
    public partial class MainForm : Form
    {
        string assemblyPath;

        public MainForm()
        {
            InitializeComponent();
        }

        private void Info_Click(object sender, EventArgs e)
        {

        }

        private void Install_Click(object sender, EventArgs e)
        {
            Process[] peoplePlayground = Process.GetProcessesByName("People Playground");
            if(peoplePlayground.Length==0)
            {
                Info.Text = "People Playground not running, make sure it is running to install.";
            }
            else
            {
                assemblyPath = peoplePlayground[0].MainModule.FileName.Replace("People Playground.exe", "");
                var resolver = new DefaultAssemblyResolver();
                resolver.AddSearchDirectory($@"{assemblyPath}People Playground_Data\Managed\");
                if (MessageBox.Show($"Confirm you would like to install JFramework in {assemblyPath}?", "Confirm", MessageBoxButtons.YesNo)==DialogResult.Yes)
                {
                    Info.Text = "Locating Assembly-CSharp.dll...";
                    if(File.Exists(assemblyPath+ @"People Playground_Data\Managed\Assembly-CSharp.dll"))
                    {
                        Info.Text = "Assembly-CSharp.dll located, patching... (Game will close)";
                        assemblyPath += @"People Playground_Data\Managed\Assembly-CSharp.dll";
                        peoplePlayground[0].CloseMainWindow();
                        peoplePlayground[0].WaitForExit();

                        AssemblyDefinition ass = AssemblyDefinition.ReadAssembly(assemblyPath, new ReaderParameters() { AssemblyResolver = resolver });
                        ModuleDefinition mod = ass.MainModule;
                        if (mod.GetType("ModInitialisationBehaviour")!=null && mod.GetType("JFrameworkIdentifier") ==null)
                        {
                            TypeDefinition type = mod.GetType("ModInitialisationBehaviour");
                            Info.Text = $"{type.FullName} is ModInitialisationBehaviour";

                            MethodDefinition toEdit = type.Methods.Single(m => m.Name=="Start");

                            TypeDefinition identifier = new TypeDefinition("", "JFrameworkIdentifier", Mono.Cecil.TypeAttributes.Public);
                            mod.Types.Add(identifier);
                            TypeReference t = mod.ImportReference(typeof(JFramework.Main));
                            MethodReference JFrameworkEntry = mod.ImportReference(typeof(JFramework.Main).GetMethod("FrameworkEntry"));

                            var processor = toEdit.Body.GetILProcessor();
                            var newInstruction = processor.Create(OpCodes.Call, JFrameworkEntry);
                            var firstInstruction = toEdit.Body.Instructions[0];
                            processor.InsertBefore(firstInstruction, newInstruction);

                            ass.Write(assemblyPath.Replace("CSharp", "CSharpModified"));
                            mod.Dispose();
                            ass.Dispose();
                            File.Delete(assemblyPath);
                            File.Move(assemblyPath.Replace("CSharp", "CSharpModified"), assemblyPath);

                            Info.Text = "Game has been patched successfully;";

                        }
                        else if(mod.GetType("JFrameworkIdentifier") !=null)
                        {
                            Info.Text = "JFramework is already installed, if you believe it not to be then validate your game integrity";
                        }
                        else
                        {
                            Info.Text = "Failed, wrong Assembly-CSharp.dll found.";
                        }

                        if (mod != null)
                        {
                            mod.Dispose();
                            ass.Dispose();
                        }
                    }
                    else
                    {
                        Info.Text = "Failed to locate Assembly-CSharp.dll, please verify the integrity of your game.";
                    }
                }
            }
        }

        private void Check_Click(object sender, EventArgs e)
        {
            Process[] peoplePlayground = Process.GetProcessesByName("People Playground");
            if (peoplePlayground.Length == 0)
            {
                Info.Text = "People Playground not running, make sure it is running to check install.";
            }
            else
            {
                assemblyPath = peoplePlayground[0].MainModule.FileName.Replace("People Playground.exe", "");
                if (MessageBox.Show($"Confirm you would like to install JFramework in {assemblyPath}?", "Confirm", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    Info.Text = "Locating Assembly-CSharp.dll...";
                    if (File.Exists(assemblyPath + @"People Playground_Data\Managed\Assembly-CSharp.dll"))
                    {
                        Info.Text = "Assembly-CSharp.dll located, checking";
                        assemblyPath += @"People Playground_Data\Managed\Assembly-CSharp.dll";

                        AssemblyDefinition ass = AssemblyDefinition.ReadAssembly(assemblyPath);
                        ModuleDefinition mod = ass.MainModule;
                        if (mod.GetType("JFrameworkIdentifier") == null)
                        {

                            Info.Text = "JFramework is not installed";
                        }
                        else 
                        {
                            Info.Text = "JFramework is installed";
                        }
                        mod.Dispose();
                        ass.Dispose();
                    }
                    else
                    {
                        Info.Text = "Failed to locate Assembly-CSharp.dll, please verify the integrity of your game.";
                    }
                }
            }
        }
    }
}
