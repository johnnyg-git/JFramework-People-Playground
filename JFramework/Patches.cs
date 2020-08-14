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
using System.Security.Policy;

namespace JFramework
{
    [HarmonyPatch(typeof(ModLoader), "LoadMods")]
    public class LoadModsPatch
    {
        public static List<string> pathChecked = new List<string>();
        [HarmonyPostfix]
        static void Postfix()
        {
            Debug.Log("LoadMods patch triggered");
            Debug.Log("Loading mods");
            foreach (ModMetaData modMetaData in ModLoader.LoadedMods)
            {
                if (!modMetaData.Active || modMetaData.MetaLocation.Contains("Mods")) continue;
                Main.LoadJsons(modMetaData.MetaLocation+"/../");
                break;
            }
            Main.LoadJsons(Main.gamePath + "Mods");
            Debug.Log("Loading assemblies");
            Main.LoadAssemblies();
            Debug.Log("Loading bundles");
            Main.LoadBundles();
            Debug.Log("Loading spawnables");
            Main.LoadSpawnables();
            Debug.Log("Loading maps");
            Main.LoadMaps();
        }
    }

    [HarmonyPatch(typeof(MapSelectionMenuBehaviour), "Awake")]
    public class MapPatch
    {
        static bool first = true;

        [HarmonyPostfix]
        static void Postfix(MapSelectionMenuBehaviour __instance)
        {
            if(first)
            {
                Debug.Log("First");
                first = false;
                return;
            }
            Debug.Log("Adding maps");
            foreach (Mod mod in Main.mods)
            {
                foreach (Map a in mod.loadedMaps) {
                    UnityEngine.Object.Instantiate<GameObject>(__instance.MapViewPrefab, Vector3.zero, Quaternion.identity, __instance.transform).GetComponent<MapViewBehaviour>().Map = a;
                }
            }
        }
    }

    [HarmonyPatch(typeof(CatalogBehaviour), "Awake")]
    public class PopulatePatch
    {
        [HarmonyPrefix]
        static bool Prefix(CatalogBehaviour __instance, ref Dictionary<Modification, ModMetaData> ___modifications, ref Dictionary<SpawnableAsset, Action<GameObject>> ___modActionByAsset, ref List<SpawnableAsset> ___spawnables)
        {
            Console.WriteLine("Populate patch triggered");
            List<SpawnableAsset> spawnables = Resources.LoadAll<SpawnableAsset>("SpawnableAssets").ToList();
            foreach (Mod mod in Main.mods)
            {
                foreach(SpawnableAsset s in mod.loadedSpawnables)
                {
                    foreach(Category c in __instance.Catalog.Categories)
                    {
                        if(c.name==s.Category.name)
                            s.Category = c;
                    }
                    Debug.Log($"Adding {s.name} under {s.Category.name}");
                }
                spawnables.AddRange(mod.loadedSpawnables);
            }
            __instance.Catalog.Items = spawnables.ToArray();
            ___spawnables = new List<SpawnableAsset>(__instance.Catalog.Items);
            ___modActionByAsset.Clear();
            ___modifications.Clear();
            __instance.BackgroundScripts.Clear();
            MethodInfo RegisterModifications = __instance.GetType().GetMethod("RegisterModifications", BindingFlags.NonPublic | BindingFlags.Instance);
            MethodInfo ApplyModifications = __instance.GetType().GetMethod("ApplyModifications", BindingFlags.NonPublic | BindingFlags.Instance);
            MethodInfo InstantiateBackgroundScripts = __instance.GetType().GetMethod("InstantiateBackgroundScripts", BindingFlags.NonPublic | BindingFlags.Instance);
            RegisterModifications.Invoke(__instance, new object[] { });
            ApplyModifications.Invoke(__instance, new object[] { });
            InstantiateBackgroundScripts.Invoke(__instance, new object[] { });
            return false;
        }
    }
}
