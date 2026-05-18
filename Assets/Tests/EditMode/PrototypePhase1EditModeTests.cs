using System.IO;
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
            RequireObject("Safe return marker");
            RequireRouteCheckpoint(0, "Start on foot");
            RequireRouteCheckpoint(1, "Enter vehicle lane");
            RequireRouteCheckpoint(2, "Patrol pressure turn");
            RequireRouteCheckpoint(3, "Workshop interaction stop");
            RequireRouteCheckpoint(4, "Safe return");

            Assert.That(Camera.main, Is.Not.Null);
        }

        [Test]
        public void Phase2EventsChangeWorldState()
        {
            var worldObject = new GameObject("World State Test");
            var world = worldObject.AddComponent<PrototypeWorldState>();

            world.ResetState();
            world.ApplyEvent(PrototypeWorldEvent.PublicViolenceCommitted);

            Assert.That(world.Fear, Is.EqualTo(SocialLevel.High));
            Assert.That(world.PeopleLove, Is.EqualTo(SocialLevel.Low));
            Assert.That(world.StatePressure, Is.EqualTo(PressureLevel.Medium));
            Assert.That(world.RuleStyleDecision, Is.EqualTo(RuleStyle.ShowOfForce));
            Assert.That(world.LastEvent, Is.EqualTo(PrototypeWorldEvent.PublicViolenceCommitted));
            Assert.That(PrototypeDebugState.World, Does.Contain("LastEvent: PublicViolenceCommitted"));

            world.ResetState();
            world.ApplyEvent(PrototypeWorldEvent.PublicViolenceCommitted);
            world.ApplyEvent(PrototypeWorldEvent.BribeAccepted);

            Assert.That(world.StatePressure, Is.EqualTo(PressureLevel.Low));
            Assert.That(world.DirtyCash, Is.EqualTo(DirtyCashState.Hidden));
            Assert.That(world.RuleStyleDecision, Is.EqualTo(RuleStyle.Bribe));
            Assert.That(world.LastEvent, Is.EqualTo(PrototypeWorldEvent.BribeAccepted));

            world.ResetState();
            world.ApplyEvent(PrototypeWorldEvent.MateoProtected);
            Assert.That(world.LieutenantTrust, Is.EqualTo(LieutenantTrust.Trusted));
            Assert.That(world.LastEvent, Is.EqualTo(PrototypeWorldEvent.MateoProtected));

            world.ResetState();
            world.ApplyEvent(PrototypeWorldEvent.MateoHumiliated);
            Assert.That(world.LieutenantTrust, Is.EqualTo(LieutenantTrust.Humiliated));
            Assert.That(world.LastEvent, Is.EqualTo(PrototypeWorldEvent.MateoHumiliated));

            Object.DestroyImmediate(worldObject);
        }

        [Test]
        public void Phase3WorldStateSnapshotRoundTripsThroughFile()
        {
            var worldObject = new GameObject("World State Snapshot Test");
            var world = worldObject.AddComponent<PrototypeWorldState>();
            var snapshotPath = Path.Combine(Path.GetTempPath(), "valle_de_plata_phase3_world_state_test.json");

            try
            {
                world.ResetState();
                world.ApplyEvent(PrototypeWorldEvent.PublicViolenceCommitted);
                world.ApplyEvent(PrototypeWorldEvent.BribeAccepted);
                world.SaveSnapshot(snapshotPath);

                Assert.That(File.Exists(snapshotPath), Is.True);
                Assert.That(File.ReadAllText(snapshotPath), Does.Contain("lastEvent"));

                world.ResetState();
                Assert.That(world.LastEvent, Is.EqualTo(PrototypeWorldEvent.None));

                world.LoadSnapshot(snapshotPath);

                Assert.That(world.DistrictId, Is.EqualTo("BarrioHondo"));
                Assert.That(world.FrontId, Is.EqualTo("ElRespiroWorkshop"));
                Assert.That(world.LastEvent, Is.EqualTo(PrototypeWorldEvent.BribeAccepted));
                Assert.That(world.DirtyCash, Is.EqualTo(DirtyCashState.Hidden));
                Assert.That(world.StatePressure, Is.EqualTo(PressureLevel.Low));
                Assert.That(world.RuleStyleDecision, Is.EqualTo(RuleStyle.Bribe));
                Assert.That(PrototypeDebugState.World, Does.Contain("LastEvent: BribeAccepted"));
            }
            finally
            {
                if (File.Exists(snapshotPath))
                {
                    File.Delete(snapshotPath);
                }

                Object.DestroyImmediate(worldObject);
            }
        }

        [Test]
        public void Phase4FrontEventsGiveDirtyCashAndMateoAStateCost()
        {
            var worldObject = new GameObject("Front State Test");
            var world = worldObject.AddComponent<PrototypeWorldState>();

            world.ResetState();
            world.ApplyEvent(PrototypeWorldEvent.DirtyCashPickedUp);

            Assert.That(world.DirtyCash, Is.EqualTo(DirtyCashState.Carried));
            Assert.That(world.StatePressure, Is.EqualTo(PressureLevel.Medium));
            Assert.That(world.LastEvent, Is.EqualTo(PrototypeWorldEvent.DirtyCashPickedUp));

            world.ApplyEvent(PrototypeWorldEvent.FrontTakenUnderWatch);

            Assert.That(world.FrontControl, Is.EqualTo(FrontControl.PabloWatched));
            Assert.That(world.DirtyCash, Is.EqualTo(DirtyCashState.Hidden));
            Assert.That(world.RuleStyleDecision, Is.EqualTo(RuleStyle.Favor));
            Assert.That(world.StatePressure, Is.EqualTo(PressureLevel.High));

            world.ResetState();
            world.ApplyEvent(PrototypeWorldEvent.MateoProtected);
            world.ApplyEvent(PrototypeWorldEvent.DirtyCashPickedUp);
            world.ApplyEvent(PrototypeWorldEvent.FrontTakenUnderWatch);

            Assert.That(world.LieutenantTrust, Is.EqualTo(LieutenantTrust.Trusted));
            Assert.That(world.FrontControl, Is.EqualTo(FrontControl.PabloWatched));
            Assert.That(world.StatePressure, Is.EqualTo(PressureLevel.Low));

            Object.DestroyImmediate(worldObject);
        }

        [Test]
        public void Phase5MissionSpineTracksSuccessAndPartialFailure()
        {
            var worldObject = new GameObject("Mission World State Test");
            var missionObject = new GameObject("Mission Spine Test");
            var world = worldObject.AddComponent<PrototypeWorldState>();
            var mission = missionObject.AddComponent<PrototypeMissionSpine>();

            mission.AttachWorldState(world);
            Assert.That(mission.Stage, Is.EqualTo(PrototypeMissionStage.FindingFront));

            world.ApplyEvent(PrototypeWorldEvent.DirtyCashPickedUp);
            Assert.That(mission.Stage, Is.EqualTo(PrototypeMissionStage.CarryingRisk));

            world.ApplyEvent(PrototypeWorldEvent.FrontTakenUnderWatch);
            Assert.That(mission.Stage, Is.EqualTo(PrototypeMissionStage.FrontSecured));
            Assert.That(PrototypeDebugState.Mission, Does.Contain("secured"));

            world.ResetState();
            world.ApplyEvent(PrototypeWorldEvent.DirtyCashPickedUp);
            world.ApplyEvent(PrototypeWorldEvent.DirtyCashSeized);

            Assert.That(world.DirtyCash, Is.EqualTo(DirtyCashState.Seized));
            Assert.That(world.StatePressure, Is.EqualTo(PressureLevel.High));
            Assert.That(mission.Stage, Is.EqualTo(PrototypeMissionStage.PartialFailure));
            Assert.That(PrototypeDebugState.Mission, Does.Contain("partial failure"));

            Object.DestroyImmediate(missionObject);
            Object.DestroyImmediate(worldObject);
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
