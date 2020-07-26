using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using System.IO;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;
using Mono.Cecil;

namespace JFramework
{
    public class Main
    {
        public static string gamePath = Application.dataPath + "/../";
        public static string modPath = "Unknown";
        static bool hasStarted;

        public static void Entry() 
        {
            if (hasStarted) return;
            hasStarted = true;

            Process[] peoplePlayground = Process.GetProcessesByName("People Playground");
            string assemblyPath = peoplePlayground[0].MainModule.FileName.Replace("People Playground.exe", "");

            if (!File.Exists(assemblyPath + @"People Playground_Data\Managed\JFramework.dll") && File.Exists(Path.Combine(modPath,"JFramework.dll")))
            {
                File.Copy(Path.Combine(modPath, "JFramework.dll"), assemblyPath + @"People Playground_Data\Managed\JFramework.dll");
            }
            if (!File.Exists(assemblyPath + @"People Playground_Data\Managed\Mono.Cecil.dll") && File.Exists(Path.Combine(modPath, "Mono.Cecil.dll")))
            {
                File.Copy(Path.Combine(modPath, "Mono.Cecil.dll"), assemblyPath + @"People Playground_Data\Managed\Mono.Cecil.dll");
            }
            if (!File.Exists(assemblyPath + @"People Playground_Data\Managed\Mono.Cecil.Mdb.dll") && File.Exists(Path.Combine(modPath, "Mono.Cecil.Mdb.dll")))
            {
                File.Copy(Path.Combine(modPath, "Mono.Cecil.Mdb.dll"), assemblyPath + @"People Playground_Data\Managed\Mono.Cecil.Mdb.dll");
            }
            if (!File.Exists(assemblyPath + @"People Playground_Data\Managed\0Harmony.dll") && File.Exists(Path.Combine(modPath, "0Harmony.dll")))
            {
                File.Copy(Path.Combine(modPath, "0Harmony.dll"), assemblyPath + @"People Playground_Data\Managed\0Harmony.dll");
            }


            assemblyPath += @"People Playground_Data\Managed\Assembly-CSharp.dll";

            AssemblyDefinition ass = AssemblyDefinition.ReadAssembly(assemblyPath);
            ModuleDefinition mod = ass.MainModule;
            if (mod.GetType("JFrameworkIdentifier") == null)
            {
                UnityEngine.Debug.Log("Path is " + modPath);

                if (File.Exists(Path.Combine(modPath, "JFrameworkInstaller.exe")))
                {
                    string path = Path.Combine(modPath, "JFrameworkInstaller.exe");
                    UnityEngine.Debug.Log("Installer found at " + path);
                    ModAPI.Notify("Installer found at " + path);

                    Process installer = new Process();
                    installer.StartInfo.UseShellExecute = false;
                    installer.StartInfo.FileName = path;
                    installer.Start();
                }
                else
                {
                    UnityEngine.Debug.LogError("Installer could not be found in mod folder, please try to reinstall the mod");
                    ModAPI.Notify("Installer could not be found in mod folder, please try to reinstall the mod");
                }
            }
            else
            {
                ModAPI.Notify("JFramework already installed");
            }
            mod.Dispose();
            ass.Dispose();
        }
    }
}
