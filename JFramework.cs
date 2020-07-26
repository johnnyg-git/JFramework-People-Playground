using System.Reflection;
using System.IO;
using UnityEngine;

namespace JFramework
{
    public class EntryPoint
    {
        public static void Main()
        {
            ModAPI.Register<FrameworkInit>();
        }
    }

    public class FrameworkInit : MonoBehaviour
    {
        void Start()
        {
            string modPath = Application.dataPath + "/../" + ModAPI.Metadata.MetaLocation;
            Debug.Log("Path is " + modPath);

            if (File.Exists(Path.Combine(modPath, "JFramework.dll")))
            {
                string path = Path.Combine(modPath, "JFramework.dll");
                Debug.Log("Main DLL found at " + path);
                ModAPI.Notify("Main DLL found at " + path);
                Assembly mainAssembly = Assembly.LoadFile(path);
                mainAssembly.GetType("JFramework.Main").GetField("modPath", BindingFlags.Static | BindingFlags.Public).SetValue(null, modPath);
                mainAssembly.GetType("JFramework.Main").GetMethod("Entry").Invoke(null, new object[0]);

            }
            else
            {
                Debug.LogError("Main DLL could not be found in mod folder, please try to reinstall the mod");
                ModAPI.Notify("Main DLL could not be found in mod folder, please try to reinstall the mod");
            }
        }
    }
}