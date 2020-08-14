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
using UnityEngine.Rendering;

namespace JFramework
{
    [HarmonyPatch(typeof(MapSelectionMenuBehaviour), "Start")]
    class MapPatch
    {
        [HarmonyPostfix]
        static void Postfix(MapSelectionMenuBehaviour __instance)
        {
            Debug.Log("Adding maps");
            foreach (Mod mod in Main.mods)
            {
                foreach (Map a in mod.loadedMaps)
                {
                    UnityEngine.Object.Instantiate<GameObject>(__instance.MapViewPrefab, Vector3.zero, Quaternion.identity, __instance.transform).GetComponent<MapViewBehaviour>().Map = a;
                }
            }
        }
    }

    [HarmonyPatch(typeof(PhysicalBehaviour), "Awake")]
    class PhysicalAwake
    {
        [HarmonyPostfix]
        static bool Prefix(PhysicalBehaviour __instance)
        {
            __instance.spriteRenderer = __instance.GetComponent<SpriteRenderer>();
            if(__instance.spriteRenderer==null) 
                __instance.spriteRenderer = __instance.GetComponentInChildren<SpriteRenderer>();
            __instance.InitialBounds = __instance.spriteRenderer.bounds;
            __instance.rigidbody = __instance.GetComponent<Rigidbody2D>();
            __instance.TrueInitialMass = __instance.rigidbody.mass;
            __instance.rigidbody.useAutoMass = false;
            __instance.ContextMenuOptions = __instance.gameObject.AddComponent<ContextMenuOptionComponent>();
            __instance.colliders = (from c in __instance.GetComponents<Collider2D>()
                              where !c.isTrigger
                              select c).ToArray<Collider2D>();
            return false;
        }
    }

    [HarmonyPatch(typeof(ToolControllerBehaviour), "DetermineHovering")]
    class HoveringPatch
    {
        [HarmonyPostfix]
        static bool Prefix(ToolControllerBehaviour __instance, ref PhysicalBehaviour ___currentlyHovering, SelectionController ___selectionController)
        {
            ___currentlyHovering = null;
            Collider2D collider2D = Physics2D.OverlapPoint(Global.main.MousePosition, __instance.SelectionLayer);
            if (!collider2D)
            {
                ___selectionController.SetHovering(null);
                return false;
            }
            PhysicalBehaviour component = collider2D.GetComponent<PhysicalBehaviour>();
            if(component==null)
            {
                component = collider2D.GetComponentInParent<PhysicalBehaviour>();
            }
            if (!component)
            {
                ___selectionController.SetHovering(null);
                return false;
            }
            if (___currentlyHovering != component)
            {
                ___selectionController.SetHovering(component);
            }
            ___currentlyHovering = component;
            return false;
        }
    }

    [HarmonyPatch(typeof(DecalControllerBehaviour), "CreateContainer")]
    class DecalPatch
    {
        [HarmonyPostfix]
        static bool Prefix(DecalControllerBehaviour __instance, ref bool ___dirty, ref int ___originalSortingLayer, ref SpriteMask ___spriteMask)
        {
            ___dirty = true;
            string text = __instance.DecalDescriptor.name + " container";
            Transform transform = __instance.transform.Find(text);
            if (transform)
            {
                __instance.decalHolder = transform.gameObject;
            }
            else
            {
                __instance.decalHolder = new GameObject(text);
            }
            __instance.decalHolder.transform.SetParent(__instance.transform);
            __instance.decalHolder.transform.localPosition = Vector3.zero;
            __instance.decalHolder.transform.localScale = Vector3.one;
            __instance.decalHolder.AddComponent<Optout>();
            SpriteRenderer component = __instance.GetComponent<SpriteRenderer>();
            if(component==null)
            {
                component = __instance.GetComponentInChildren<SpriteRenderer>();
            }
            __instance.decalHolder.transform.rotation = component.transform.rotation;
            ___originalSortingLayer = component.sortingLayerID;
            SortingGroup sortingGroup = __instance.decalHolder.AddComponent<SortingGroup>();
            sortingGroup.sortingLayerID = ___originalSortingLayer;
            sortingGroup.sortingOrder = 1;
            ___spriteMask = __instance.decalHolder.AddComponent<SpriteMask>();
            ___spriteMask.sprite = component.sprite;
            ___spriteMask.sortingOrder = 1;
            return false;
        }
    }

    [HarmonyPatch(typeof(PhysicalBehaviour), "CreateOutlineObject")]
    class OutlinePatch
    {
        [HarmonyPostfix]
        static bool Prefix(PhysicalBehaviour __instance, ref MaterialPropertyBlock ___propertyBlock)
        {
            __instance.selectionOutlineObject = new GameObject("Outline");
            __instance.selectionOutlineObject.AddComponent<Optout>();
            SpriteRenderer spriteRenderer = __instance.selectionOutlineObject.AddComponent<SpriteRenderer>();
            spriteRenderer.sortingLayerName = "Top";
            spriteRenderer.sortingOrder = int.MaxValue;
            spriteRenderer.transform.SetParent(__instance.transform, false);
            spriteRenderer.sprite = __instance.spriteRenderer.sprite;
            spriteRenderer.transform.rotation = __instance.spriteRenderer.transform.rotation;
            spriteRenderer.sharedMaterial = Global.main.SelectionOutlineMaterial;
            ___propertyBlock = new MaterialPropertyBlock();
            spriteRenderer.GetPropertyBlock(___propertyBlock);
            Vector2 vector = new Vector2((float)spriteRenderer.sprite.texture.width, (float)spriteRenderer.sprite.texture.height);
            Vector2 min = Utils.GetMin<Vector2>(spriteRenderer.sprite.uv, (Vector2 v) => v.sqrMagnitude);
            Vector2 vector2 = new Vector2(spriteRenderer.sprite.rect.width, spriteRenderer.sprite.rect.height);
            Vector4 value = new Vector4(min.x, min.y, vector2.x / vector.x, vector2.y / vector.y);
            ___propertyBlock.SetVector("_AtlasTransform", value);
            spriteRenderer.SetPropertyBlock(___propertyBlock);
            return false;
        }
    }

    [HarmonyPatch(typeof(CatalogBehaviour), "Populate")]
    class PopulatePatch
    {
        [HarmonyPrefix]
        static bool Prefix(CatalogBehaviour __instance, ref List<SpawnableAsset> ___spawnables)
        {
            Console.WriteLine("Populate patch triggered");
            List<SpawnableAsset> spawnables = Resources.LoadAll<SpawnableAsset>("SpawnableAssets").ToList();
            foreach (Mod mod in Main.mods)
            {
                foreach (SpawnableAsset s in mod.loadedSpawnables)
                {
                    bool hasChanged = false;
                    foreach (Category c in __instance.Catalog.Categories)
                    {
                        if (c.name == s.Category.name)
                        {
                            s.Category = c;
                            hasChanged = true;
                        }
                    }

                    if (!hasChanged)
                    {
                        List<Category> newL = __instance.Catalog.Categories.ToList();
                        newL.Add(s.Category);
                        __instance.Catalog.Categories = newL.ToArray();
                    }

                    Debug.Log($"Adding {s.name} under {s.Category.name}");
                }
                Debug.Log(1);
                spawnables.AddRange(mod.loadedSpawnables);
            }
            Debug.Log(2);
            __instance.Catalog.Items = spawnables.ToArray();
            Debug.Log(3);
            ___spawnables = new List<SpawnableAsset>(__instance.Catalog.Items);
            Debug.Log(4);
            (__instance.GetType()?.GetField("modActionByAsset", BindingFlags.Instance | BindingFlags.NonPublic)?.GetValue(__instance) as Dictionary<SpawnableAsset, Action<GameObject>>)?.Clear();
            Debug.Log(5);
            MethodInfo ApplyModifications = __instance.GetType().GetMethod("ApplyModifications", BindingFlags.NonPublic | BindingFlags.Instance);
            MethodInfo InstantiateBackgroundScripts = __instance.GetType().GetMethod("InstantiateBackgroundScripts", BindingFlags.NonPublic | BindingFlags.Instance);
            __instance.GetType().Assembly.GetType("ModificationManager").GetMethod("InvokeMain", BindingFlags.NonPublic | BindingFlags.Static).Invoke(null, new object[] { });
            ApplyModifications.Invoke(__instance, new object[] { });
            InstantiateBackgroundScripts.Invoke(__instance, new object[] { });
            return false;
        }
    }
}
