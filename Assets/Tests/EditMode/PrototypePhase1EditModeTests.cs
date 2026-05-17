using NUnit.Framework;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using ValleDePlata.Prototype;

namespace ValleDePlata.Tests
{
    public sealed class PrototypePhase1EditModeTests
    {
        private const string ScenePath = "Assets/Scenes/Phase1_FeelPrototype.unity";

        [Test]
        public void Phase1SceneContainsRequiredFeelPrototypeObjects()
        {
            EditorSceneManager.OpenScene(ScenePath);

            RequireComponent<PrototypePlayerController>("Pablo Valera Prototype Controller");
            RequireComponent<PrototypeVehicleController>("Prototype Sedan");
            RequireComponent<PrototypeCameraRig>("Prototype Camera Rig");
            RequireComponent<PrototypeDebugHud>("Prototype Debug HUD");
            RequireComponent<PrototypePressureZone>("Pressure patrol marker");
            RequireComponent<PrototypeInteractable>("Workshop shutter interactable");
            RequireComponent<PrototypeRouteProgress>("Phase 1 Route Progress");
            RequireObject("Narrow asphalt route");
            RequireObject("Tight corner block");
            RequireObject("Safe return marker");
            RequireRouteCheckpoint(0, "Start on foot");
            RequireRouteCheckpoint(1, "Enter vehicle lane");
            RequireRouteCheckpoint(2, "Patrol pressure turn");
            RequireRouteCheckpoint(3, "Workshop interaction stop");
            RequireRouteCheckpoint(4, "Safe return");

            Assert.That(Camera.main, Is.Not.Null);
        }

        [Test]
        public void VehicleEnterExitKeepsPlayerRecoverable()
        {
            EditorSceneManager.OpenScene(ScenePath);
            var player = RequireComponent<PrototypePlayerController>("Pablo Valera Prototype Controller");
            var vehicle = RequireComponent<PrototypeVehicleController>("Prototype Sedan");

            player.EnterVehicle(vehicle);

            Assert.That(vehicle.HasDriver, Is.True);
            Assert.That(player.gameObject.activeSelf, Is.False);

            vehicle.ExitDriver();

            Assert.That(vehicle.HasDriver, Is.False);
            Assert.That(player.gameObject.activeSelf, Is.True);
            Assert.That(player.IsDriving, Is.False);
        }

        [Test]
        public void Phase1SceneIsInBuildSettings()
        {
            var found = false;
            foreach (var scene in EditorBuildSettings.scenes)
            {
                if (scene.path == ScenePath && scene.enabled)
                {
                    found = true;
                    break;
                }
            }

            Assert.That(found, Is.True);
        }

        [Test]
        public void RouteProgressAdvancesInCheckpointOrder()
        {
            EditorSceneManager.OpenScene(ScenePath);
            var route = RequireComponent<PrototypeRouteProgress>("Phase 1 Route Progress");
            var first = RequireComponent<PrototypeRouteCheckpoint>("Route checkpoint 0: Start on foot");
            var second = RequireComponent<PrototypeRouteCheckpoint>("Route checkpoint 1: Enter vehicle lane");

            route.Configure(5);
            route.RegisterCheckpoint(second.CheckpointIndex, second.Label);
            Assert.That(route.NextCheckpointIndex, Is.EqualTo(0), "Route should ignore out-of-order checkpoints.");

            route.RegisterCheckpoint(first.CheckpointIndex, first.Label);
            Assert.That(route.NextCheckpointIndex, Is.EqualTo(1));
            Assert.That(PrototypeDebugState.LastCheckpoint, Is.EqualTo("Start on foot"));
        }

        private static void RequireObject(string objectName)
        {
            Assert.That(GameObject.Find(objectName), Is.Not.Null, $"Missing required object: {objectName}");
        }

        private static T RequireComponent<T>(string objectName) where T : Component
        {
            var target = GameObject.Find(objectName);
            Assert.That(target, Is.Not.Null, $"Missing required object: {objectName}");
            var component = target.GetComponent<T>();
            Assert.That(component, Is.Not.Null, $"{objectName} is missing component {typeof(T).Name}.");
            return component;
        }

        private static void RequireRouteCheckpoint(int index, string label)
        {
            var checkpoint = RequireComponent<PrototypeRouteCheckpoint>($"Route checkpoint {index}: {label}");
            Assert.That(checkpoint.CheckpointIndex, Is.EqualTo(index));
            Assert.That(checkpoint.Label, Is.EqualTo(label));
        }
    }
}
