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
            RequireComponent<PrototypeRunMetrics>("Phase 1 Run Metrics");
            RequireComponent<PrototypeWorldState>("Prototype World State");
            RequireComponent<PrototypeMissionSpine>("Pierwszy Front Mission Spine");
            RequireComponent<PrototypePressureZone>("Pressure patrol marker");
            RequireComponent<PrototypeInteractable>("Workshop shutter interactable");
            RequireComponent<PrototypeInteractable>("Public violence test target");
            RequireComponent<PrototypeWorldReactionMarker>("Civilian panic marker");
            RequireComponent<PrototypeWorldReactionMarker>("Shop shutter closes marker");
            RequireComponent<PrototypeWorldReactionMarker>("Police pressure moves closer marker");
            RequireComponent<PrototypeInteractable>("Rios bribe test officer");
            RequireComponent<PrototypeWorldReactionMarker>("Bribe roadblock opens marker");
            RequireComponent<PrototypeWorldReactionMarker>("Rios leverage marker");
            RequireComponent<PrototypeWorldReactionMarker>("Risk cargo hidden marker");
            RequireComponent<PrototypeInteractable>("Mateo protected test contact");
            RequireComponent<PrototypeInteractable>("Mateo humiliated test contact");
            RequireComponent<PrototypeWorldReactionMarker>("Mateo early warning marker");
            RequireComponent<PrototypeWorldReactionMarker>("Mateo late warning marker");
            RequireComponent<PrototypeInteractable>("El Respiro dirty cash pickup");
            RequireComponent<PrototypeInteractable>("El Respiro front takeover");
            RequireComponent<PrototypeWorldReactionMarker>("Dirty cash carried marker");
            RequireComponent<PrototypeWorldReactionMarker>("El Respiro Pablo watched marker");
            RequireComponent<PrototypeWorldReactionMarker>("Barrio reaction to front marker");
            RequireComponent<PrototypeInteractable>("Dirty cash seizure failstate");
            RequireComponent<PrototypeWorldReactionMarker>("Seized cash partial failure marker");
            RequireComponent<PrototypeRouteProgress>("Phase 1 Route Progress");

            RequireObject("Narrow asphalt route");
            RequireObject("Tight corner block");
            RequireObject("Static civilian car obstacle");
            RequireObject("Safe return marker");
            RequireRouteCheckpoint(0, "Start on foot");
            RequireRouteCheckpoint(1, "Enter vehicle lane");
            RequireRouteCheckpoint(2, "Patrol pressure turn");
            RequireRouteCheckpoint(3, "Workshop interaction stop");
            RequireRouteCheckpoint(4, "Safe return");

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

        private static T RequireComponent<T>(string objectName) where T : Component
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

            return target.GetComponent<T>();
        }

        private static void RequireRouteCheckpoint(int index, string label)
        {
            var checkpoint = RequireComponent<PrototypeRouteCheckpoint>($"Route checkpoint {index}: {label}");
            if (checkpoint.CheckpointIndex != index)
            {
                Fail($"Route checkpoint {label} has index {checkpoint.CheckpointIndex}, expected {index}.");
            }
        }

        private static void Fail(string message)
        {
            Debug.LogError(message);
            throw new System.InvalidOperationException(message);
        }
    }
}
