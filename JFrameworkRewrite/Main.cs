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
using HarmonyLib;
using Newtonsoft.Json;
using Debug = UnityEngine.Debug;
using Steamworks;
using Object = UnityEngine.Object;

namespace JFramework
{
    public class Main
    {
        /// <summary>
        /// Path to the game directory
        /// </summary>
        public static string gamePath { get; internal set; } = Application.dataPath + "/../";
        private static Harmony harmony;
        private static bool hasStarted;

        /// <summary>
        /// All mods loaded
        /// </summary>
        public static List<Mod> mods { get; internal set; } = new List<Mod>();
        /// <summary>
        /// Will be ran at the start of the game, will not be able to be ran more than once
        /// </summary>
        public static void FrameworkEntry()
        {
            if (hasStarted) return;
            hasStarted = true;
            ConsoleManager.CreateConsole();
            var assembly = Assembly.GetCallingAssembly();
            Console.WriteLine("Running on " + assembly.GetName());
            harmony = new Harmony("johnnyjohnny.modloader");
            harmony.PatchAll();

            Console.WriteLine("Harmony patched");
            LoadMods();
        }

        internal static void LoadMods()
        {
            Debug.Log("Loading mods");
            mods.Clear();
            Debug.Log("Loading workshop mods");

            var numSubscribedItems = SteamUGC.GetNumSubscribedItems();
            var array = new PublishedFileId_t[numSubscribedItems];
			SteamUGC.GetSubscribedItems(array, numSubscribedItems);
			foreach (var publishedFileIdT in array)
			{
				ulong num;
				string text;
				uint num2;
                if (!SteamUGC.GetItemInstallInfo(publishedFileIdT, out num, out text, 1024U, out num2) ||
                    !Directory.Exists(text)) continue;
                foreach(var jmod in Directory.GetFiles(text, "jmod.json", SearchOption.AllDirectories))
                {
                    var info = JsonConvert.DeserializeObject<Mod>(File.ReadAllText(jmod));
                    mods.Add(info);
                    Debug.Log($"Loaded JFramework Mod from Workshop:\nName: {info.name}\nPath: {info.path}");
                }
            }

            Debug.Log("Workshop mods loaded, loading mods folder");

            foreach (var file in Directory.GetFiles(gamePath+"Mods", "jmod.json", SearchOption.AllDirectories))
            {
                string lines = File.ReadAllText(file);
                var info = JsonConvert.DeserializeObject<Mod>(lines);
                info.path = file.Replace("jmod.json", "");
                mods.Add(info);
                Debug.Log($"Loaded JFramework Mod from Mods folder:\nName: {info.name}\nPath: {info.path}");
            }

            Debug.Log("Mods folder loaded");

            LoadAssemblies();
            LoadBundles();
            LoadSpawnables();
            LoadMaps();
            InstantiateMonos();
        }

        internal static void LoadAssemblies()
        {
            Debug.Log("Loading assemblies");
            foreach (var mod in mods)
            {
                foreach (var assembly in mod.assemblies)
                {
                    var path = $@"{mod.path}\{assembly}";
                    if (File.Exists(path))
                    {
                        var loadedAssembly = Assembly.LoadFile(path);
                        mod.loadedAssemblies.Add(loadedAssembly);
                        Debug.Log($"{loadedAssembly.FullName} has been loaded for {mod.name}");
                    }
                    else Debug.Log($"Could not find assembly {assembly} for {mod.name}");
                }
            }
        }

        internal static void LoadBundles()
        {
            foreach (var mod in mods)
            {
                foreach (var bundle in mod.bundles)
                {
                    var path = $@"{mod.path}\{bundle}";
                    if (File.Exists(path))
                    {
                        mod.loadedBundles.Add(AssetBundle.LoadFromFile(path));
                        Debug.Log($"Bundle {bundle} was loaded for {mod.name}");
                    }
                    else Debug.Log($"Could not find bundle {bundle} for {mod.name}");
                }
            }
        }

        internal static void LoadSpawnables()
        {
            foreach (var mod in mods)
            {
                foreach (var spawnable in mod.spawnables)
                {
                    SpawnableAsset a = mod.Deserialize<SpawnableAsset>(spawnable.spawnableAsset);
                    if (a != null)
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
            Debug.Log("Loading maps");
            var menu = Object.FindObjectOfType<MapSelectionMenuBehaviour>();
            foreach (var mod in mods)
            {
                foreach (var prefab in mod.maps)
                {
                    Debug.Log(prefab.map.asset);
                    var a = mod.Deserialize<Map>(prefab.map);
                    if (a != null)
                    {
                        mod.loadedMaps.Add(a);
                        Object.Instantiate(menu.MapViewPrefab, Vector3.zero, Quaternion.identity, menu.transform).GetComponent<MapViewBehaviour>().Map = a;
                        Debug.Log($"Map {a.name} was loaded for {mod.name}");
                    }
                    else
                    {
                        Debug.Log($"Failed to load {prefab.map.asset} in {prefab.map.bundle} for {mod.name}");
                    }
                }
            }
        }

        private static void InstantiateMonos()
        {
            foreach (var mod in mods)
            {
                foreach (Assembly ass in mod.loadedAssemblies)
                {
                    foreach (Type type in ass.GetTypes())
                    {
                        if (!type.IsClass) continue;
                        if (type.IsSubclassOf(typeof(ModModule)))
                        {
                            Object.DontDestroyOnLoad(new GameObject().AddComponent(type).gameObject);
                        }
                    }
                }
            }
        }
    }
}
