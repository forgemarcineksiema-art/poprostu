using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using ValleDePlata.Prototype;

namespace ValleDePlata.Editor
{
    public static class PrototypeEnvironmentKitCurator
    {
        private const string StreetKitFolder = "Assets/Models/Environment/ValleDePlataStreetKit";

        private static readonly string[] StructuralPrefabs =
        {
            "VDP_Corner_Alley_01",
            "VDP_Facade_Plaster_01",
            "VDP_Facade_Shop_01",
            "VDP_Road_Narrow_01",
            "VDP_Rooftop_Parapet_01",
            "VDP_Shutter_Workshop_01",
            "VDP_Sidewalk_Curb_01",
            "VDP_Stairs_01",
            "VDP_Wall_Concrete_01"
        };

        private static readonly string[] DressingPrefabs =
        {
            "VDP_Awning_Market_01",
            "VDP_Balcony_01",
            "VDP_Lamp_Street_01",
            "VDP_Planter_Veg_01",
            "VDP_Pole_Cable_01",
            "VDP_Prop_Street_01",
            "VDP_Sign_Faded_01"
        };

        [MenuItem("Valle de Plata/Curate Valle de Plata Street Kit")]
        public static void CurateValleDePlataStreetKit()
        {
            if (!PrototypeLayers.AreConfigured(out var missingLayers))
            {
                throw new System.InvalidOperationException($"Missing prototype layers: {missingLayers}");
            }

            foreach (var prefabName in StructuralPrefabs)
            {
                CuratePrefab(prefabName, PrototypeLayers.WorldStatic, enableBlockingColliders: true);
            }

            foreach (var prefabName in DressingPrefabs)
            {
                CuratePrefab(prefabName, PrototypeLayers.CameraIgnore, enableBlockingColliders: false);
            }

            CurateSampleBlock();
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("Valle de Plata street kit curation complete.");
        }

        private static void CuratePrefab(string prefabName, int layer, bool enableBlockingColliders)
        {
            var prefabPath = $"{StreetKitFolder}/Prefabs/{prefabName}.prefab";
            var root = PrefabUtility.LoadPrefabContents(prefabPath);
            try
            {
                PrototypeLayers.SetLayerRecursively(root, layer);
                foreach (var collider in root.GetComponentsInChildren<Collider>(true))
                {
                    collider.enabled = enableBlockingColliders;
                    collider.isTrigger = false;
                }

                PrefabUtility.SaveAsPrefabAsset(root, prefabPath);
            }
            finally
            {
                PrefabUtility.UnloadPrefabContents(root);
            }
        }

        private static void CurateSampleBlock()
        {
            var samplePath = $"{StreetKitFolder}/VDP_StreetKit_SampleBlock.prefab";
            var root = PrefabUtility.LoadPrefabContents(samplePath);
            try
            {
                var structuralNames = new HashSet<string>(StructuralPrefabs);
                var dressingNames = new HashSet<string>(DressingPrefabs);

                PrototypeLayers.SetLayerRecursively(root, PrototypeLayers.WorldStatic);
                foreach (var child in root.GetComponentsInChildren<Transform>(true).Where(transform => transform != root.transform))
                {
                    var prefabRoot = PrefabUtility.GetOutermostPrefabInstanceRoot(child.gameObject);
                    if (prefabRoot != null && prefabRoot != child.gameObject)
                    {
                        continue;
                    }

                    if (dressingNames.Contains(child.name))
                    {
                        PrototypeLayers.SetLayerRecursively(child.gameObject, PrototypeLayers.CameraIgnore);
                        SetBlockingColliders(child.gameObject, enabled: false);
                    }
                    else if (structuralNames.Contains(child.name))
                    {
                        PrototypeLayers.SetLayerRecursively(child.gameObject, PrototypeLayers.WorldStatic);
                        SetBlockingColliders(child.gameObject, enabled: true);
                    }
                }

                PrefabUtility.SaveAsPrefabAsset(root, samplePath);
            }
            finally
            {
                PrefabUtility.UnloadPrefabContents(root);
            }
        }

        private static void SetBlockingColliders(GameObject root, bool enabled)
        {
            foreach (var collider in root.GetComponentsInChildren<Collider>(true))
            {
                collider.enabled = enabled;
                collider.isTrigger = false;
            }
        }
    }
}
