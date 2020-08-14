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
using HarmonyLib;
using Newtonsoft.Json;
using Debug = UnityEngine.Debug;

namespace JFramework
{
    public class Main
    {
        public static string gamePath = Application.dataPath + "/../";
        public static string modPath = "Unknown";
        public static Harmony harmony;
        static bool hasStarted;

        public static List<Mod> mods = new List<Mod>();

        public static void FrameworkEntry()
        {
            if (hasStarted) return;
            hasStarted = true;
            ConsoleManager.CreateConsole();

            for (int i = 0; i <= 31; i++) //user defined layers start with layer 8 and unity supports 31 layers
            {
                if (!SortingLayer.IsValid(i)) continue;
                var layerN = SortingLayer.IDToName(i); //get the name of the layer
                Debug.Log($"{i} - {layerN}");
            }

            var assembly = Assembly.GetCallingAssembly();
            Console.WriteLine("Running on " + assembly.GetName());
            harmony = new Harmony("johnnyjohnny.modloader");
            harmony.PatchAll();

            Console.WriteLine("Harmony patched");
        }

        internal static void LoadSpawnables()
        {
            foreach (Mod mod in mods)
            {
                foreach (CustomSpawnable spawnable in mod.spawnables)
                {
                    SpawnableAsset a = mod.Deserialize<SpawnableAsset>(spawnable.spawnableAsset);
                    if(a!=null)
                    {
                        mod.loadedSpawnables.Add(a);
                        Debug.Log($"Spawnable {a.name} was loaded for {mod.name}");
                    }
                    else
                    {
                        Debug.Log($"Failed to load {spawnable.spawnableAsset.asset} in {spawnable.spawnableAsset.bundle} for {mod.name}");
                    }
                }
            }
        }

        internal static void LoadMaps()
        {
            Debug.Log("h");
            MapSelectionMenuBehaviour menu = GameObject.FindObjectOfType<MapSelectionMenuBehaviour>();
            foreach (Mod mod in mods)
            {
                foreach (Prefab prefab in mod.maps)
                {
                    Debug.Log(prefab.asset);
                    Map a = mod.Deserialize<Map>(prefab);
                    if (a != null)
                    {
                        mod.loadedMaps.Add(a);
                        UnityEngine.Object.Instantiate<GameObject>(menu.MapViewPrefab, Vector3.zero, Quaternion.identity, menu.transform).GetComponent<MapViewBehaviour>().Map = a;
                        Debug.Log($"Map {a.name} was loaded for {mod.name}");
                    }
                    else
                    {
                        Debug.Log($"Failed to load {prefab.asset} in {prefab.bundle} for {mod.name}");
                    }
                }
            }
        }

        internal static void LoadBundles()
        {
            foreach (Mod mod in mods)
            {
                foreach (string bundle in mod.bundles)
                {
                    string path = $@"{mod.path}\{bundle}";
                    if (File.Exists(path))
                    {
                        mod.loadedBundles.Add(AssetBundle.LoadFromFile(path));
                        Debug.Log($"Bundle {bundle} was loaded for {mod.name}");
                    }
                    else Debug.Log($"Could not find bundle {bundle} for {mod.name}");
                }
            }
        }

        internal static void LoadAssemblies()
        {
            foreach (Mod mod in mods)
            {
                foreach (string assembly in mod.assemblies)
                {
                    string path = $@"{mod.path}\{assembly}";
                    if (File.Exists(path))
                    {
                        mod.loadedAssemblies.Add(Assembly.LoadFile(path));
                    }
                    else Debug.Log($"Could not find assembly {assembly} for {mod.name}");
                }
            }
        }

        internal static void LoadJsons(string fromFolder)
        {
            foreach (string file in Directory.GetFiles(fromFolder, "*.json", SearchOption.AllDirectories))
            {
                if (file.Contains("jmod"))
                {
                    string lines = File.ReadAllText(file);
                    Mod info = JsonConvert.DeserializeObject<Mod>(lines);
                    info.path = file.Replace("jmod.json", "");
                    mods.Add(info);
                    Debug.Log($"Loaded JFramework Mod:\nName: {info.name}\nPath: {info.path}");
                }
            }
        }
    }
}
