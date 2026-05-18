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
            if (!PrototypeLayers.AreConfigured(out var missingLayers))
            {
                Fail($"Missing prototype layers: {missingLayers}");
            }

            var scene = EditorSceneManager.OpenScene(ScenePath);
            if (!scene.IsValid() || !scene.isLoaded)
            {
                Fail($"Scene did not load: {ScenePath}");
            }

            RequireComponent<PrototypePlayerController>("Pablo Valera Prototype Controller");
            RequireComponent<PrototypeVehicleController>("Prototype Sedan");
            RequireComponent<PrototypeCameraRig>("Prototype Camera Rig");
            RequireComponent<PrototypeCursorController>("Prototype Cursor Controller");
            RequireComponent<PrototypeDebugHud>("Prototype Debug HUD");
            RequireComponent<PrototypePlayerHud>("Prototype Player HUD");
            RequireComponent<PrototypeRunMetrics>("Phase 1 Run Metrics");
            RequireComponent<PrototypeWorldState>("Prototype World State");
            RequireComponent<PrototypeMissionSpine>("Pierwszy Front Mission Spine");
            RequireComponent<PrototypeObjectiveMarker>("Prototype Objective Marker");
            RequireComponent<PrototypePressureZone>("Pressure patrol marker");
            RequireComponent<PrototypePressureChoiceController>("Pressure patrol marker");
            RequireComponent<PrototypePressureScenePlayback>("Pressure patrol marker");
            RequireComponent<Light>("Warm presentation fill light");
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
            RequireComponent<PrototypeReadableProp>("Barrio street identity prop");
            RequireComponent<PrototypeReadableProp>("Safe return readable prop");
            RequireComponent<PrototypeReadableProp>("Rios checkpoint readable prop");
            RequireComponent<PrototypeReadableProp>("Police roadblock readable prop");
            RequireComponent<PrototypeReadableProp>("El Respiro readable prop");

            RequireObject("Narrow asphalt route");
            RequireObject("Tight corner block");
            RequireObject("Motor proof low step");
            RequireObject("Motor proof high wall");
            RequireObject("Motor proof steep slope");
            RequireObject("Tight camera recovery wall");
            RequireObject("Static civilian car obstacle");
            RequireObject("Safe return marker");
            RequireNonBlockingDressing("Barrio Hondo overhead street sign");
            RequireNonBlockingDressing("Safe return alley arch");
            RequireNonBlockingDressing("Safe return painted arrow");
            RequireNonBlockingDressing("Laundry line north");
            RequireNonBlockingDressing("Witness balcony cluster");
            RequireNonBlockingDressing("Rios checkpoint desk");
            RequireNonBlockingDressing("Rios checkpoint awning");
            RequireNonBlockingDressing("Rios checkpoint stool");
            RequireNonBlockingDressing("Rios checkpoint papers");
            RequireNonBlockingDressing("Police roadblock barricade left");
            RequireNonBlockingDressing("Police roadblock barricade right");
            RequireNonBlockingDressing("Police roadblock cone left");
            RequireNonBlockingDressing("Police roadblock cone right");
            RequireNonBlockingDressing("El Respiro workshop sign");
            RequireNonBlockingDressing("El Respiro shutter slat 0");
            RequireNonBlockingDressing("El Respiro shutter slat 1");
            RequireNonBlockingDressing("El Respiro shutter slat 2");
            RequireNonBlockingDressing("El Respiro door lamp");
            RequireNonBlockingDressing("Rooftop water tank");
            RequireNonBlockingDressing("Barrio crate stack");
            RequireNonBlockingDressing("Left sunlit plaster facade");
            RequireNonBlockingDressing("Right faded teal facade");
            RequireNonBlockingDressing("Market awning strip");
            RequireNonBlockingDressing("Workshop plaster return");
            RequireNonBlockingDressing("Pressure road dust band");
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

            RequireLayer("Ground", PrototypeLayers.WorldStatic);
            RequireLayer("Pablo Valera Prototype Controller", PrototypeLayers.Player);
            RequireLayer("Prototype Sedan", PrototypeLayers.Vehicle);
            RequireLayer("Workshop shutter interactable", PrototypeLayers.Interactable);
            RequireLayer("Pressure patrol marker", PrototypeLayers.SensorTrigger);
            RequireLayer("Route checkpoint 0: Start on foot", PrototypeLayers.RouteTrigger);
            RequireLayer("Motor proof low step", PrototypeLayers.WorldStatic);
            RequireLayer("Motor proof high wall", PrototypeLayers.WorldStatic);
            RequireLayer("Motor proof steep slope", PrototypeLayers.WorldStatic);
            RequireLayer("Tight camera recovery wall", PrototypeLayers.WorldStatic);

            Debug.Log("Phase 1 scene validation passed.");
        }

        private static void RequireObject(string objectName)
        {
            if (GameObject.Find(objectName) == null)
            {
                Fail($"Missing required object: {objectName}");
            }
        }

        private static void RequireNonBlockingDressing(string objectName)
        {
            var target = GameObject.Find(objectName);
            if (target == null)
            {
                Fail($"Missing required dressing object: {objectName}");
            }

            if (target.layer != PrototypeLayers.CameraIgnore)
            {
                Fail($"{objectName} is on layer {LayerMask.LayerToName(target.layer)}, expected {LayerMask.LayerToName(PrototypeLayers.CameraIgnore)}.");
            }

            var collider = target.GetComponent<Collider>();
            if (collider != null && collider.enabled && !collider.isTrigger)
            {
                Fail($"{objectName} has a blocking collider. Believability dressing must not affect camera, player, vehicle, or exit checks.");
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

        private static void RequireLayer(string objectName, int expectedLayer)
        {
            var target = GameObject.Find(objectName);
            if (target == null)
            {
                Fail($"Missing required object: {objectName}");
            }

            if (target.layer != expectedLayer)
            {
                Fail($"{objectName} is on layer {LayerMask.LayerToName(target.layer)}, expected {LayerMask.LayerToName(expectedLayer)}.");
            }
        }

        private static void Fail(string message)
        {
            Debug.LogError(message);
            throw new System.InvalidOperationException(message);
        }
    }
}
