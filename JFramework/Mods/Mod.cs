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
        public string name;
        public bool shouldCompile = false;
        public string[] assemblies;
        public string[] bundles;
        public CustomSpawnable[] spawnables;
        public Prefab[] maps;

        [NonSerialized]
        public List<Assembly> loadedAssemblies = new List<Assembly>();
        [NonSerialized]
        public List<AssetBundle> loadedBundles = new List<AssetBundle>();
        [NonSerialized]
        public List<SpawnableAsset> loadedSpawnables = new List<SpawnableAsset>();
        [NonSerialized]
        public List<Map> loadedMaps = new List<Map>();
        [NonSerialized]
        public string path;

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
