using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace JFramework
{
    [Serializable]
    public class Mod
    {
        /// <summary>
        /// Name of the mod
        /// </summary>
        public string name;
        /// <summary>
        /// Assemblies to try find and load
        /// </summary>
        public string[] assemblies;
        /// <summary>
        /// AssetBundles to try find and load
        /// </summary>
        public string[] bundles;
        /// <summary>
        /// Spawnable items to try find inside of the AssetBundles and load
        /// </summary>
        public CustomSpawnable[] spawnables;
        /// <summary>
        /// Maps to try find inside of the AssetBundles and load
        /// </summary>
        public CustomMap[] maps;

        /// <summary>
        /// All assemblies that could be found and successfully loaded
        /// </summary>
        public List<Assembly> loadedAssemblies { get; internal set; } = new List<Assembly>();
        /// <summary>
        /// All AssetBundles that could be found and successfully loaded
        /// </summary>
        public List<AssetBundle> loadedBundles { get; internal set; } = new List<AssetBundle>();
        /// <summary>
        /// All spawnable items that could be found and successfully loaded
        /// </summary>
        public List<SpawnableAsset> loadedSpawnables = new List<SpawnableAsset>();
        /// <summary>
        /// All maps that could be found and successfully loaded
        /// </summary>
        public List<Map> loadedMaps { get; internal set; } = new List<Map>();
        /// <summary>
        /// Path to the mods directory
        /// </summary>
        public string path { get; internal set; }

        /// <summary>
        /// Used to load assets from the mods AssetBundles
        /// </summary>
        /// <typeparam name="T">Type to try and load, UnityEngine.Object</typeparam>
        /// <param name="prefab">Asset to attempt to find</param>
        /// <returns>The asset as T if found or null if not</returns>
        public T Deserialize<T>(Prefab prefab) where T : UnityEngine.Object
        {
            foreach (AssetBundle bundle in loadedBundles)
            {
                if (bundle.name == prefab.bundle && bundle.Contains(prefab.asset))
                {
                    T returner = bundle.LoadAsset(prefab.asset) as T;
                    return returner;
                }
            }
            Debug.Log($"Failed to deserialize {prefab.asset} in {prefab.bundle}");
            return null;
        }
    }
}
