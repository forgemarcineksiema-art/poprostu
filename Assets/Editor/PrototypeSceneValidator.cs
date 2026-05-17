using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using ValleDePlata.Prototype;

namespace ValleDePlata.Editor
{
    public static class PrototypeSceneValidator
    {
        private const string ScenePath = "Assets/Scenes/Phase1_FeelPrototype.unity";

        public static void ValidatePhase1Scene()
        {
            var scene = EditorSceneManager.OpenScene(ScenePath);
            if (!scene.IsValid() || !scene.isLoaded)
            {
                Fail($"Scene did not load: {ScenePath}");
            }

            RequireComponent<PrototypePlayerController>("Pablo Valera Prototype Controller");
            RequireComponent<PrototypeVehicleController>("Prototype Sedan");
            RequireComponent<PrototypeCameraRig>("Prototype Camera Rig");
            RequireComponent<PrototypeDebugHud>("Prototype Debug HUD");
            RequireComponent<PrototypePressureZone>("Pressure patrol marker");
            RequireComponent<PrototypeInteractable>("Workshop shutter interactable");

            RequireObject("Narrow asphalt route");
            RequireObject("Tight corner block");
            RequireObject("Static civilian car obstacle");
            RequireObject("Safe return marker");

            var camera = Camera.main;
            if (camera == null)
            {
                Fail("Scene has no MainCamera.");
            }

            Debug.Log("Phase 1 scene validation passed.");
        }

        private static void RequireObject(string objectName)
        {
            if (GameObject.Find(objectName) == null)
            {
                Fail($"Missing required object: {objectName}");
            }
        }

        private static void RequireComponent<T>(string objectName) where T : Component
        {
            var target = GameObject.Find(objectName);
            if (target == null)
            {
                Fail($"Missing required object: {objectName}");
            }

            if (target.GetComponent<T>() == null)
            {
                Fail($"{objectName} is missing component {typeof(T).Name}.");
            }
        }

        private static void Fail(string message)
        {
            Debug.LogError(message);
            throw new System.InvalidOperationException(message);
        }
    }
}
