using System.Collections;
using System.IO;
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using ValleDePlata.Prototype;

namespace ValleDePlata.Tests
{
    public sealed class PrototypePhase1PlayModeTests
    {
        [TearDown]
        public void CleanupTransientFoundationObjects()
        {
            var transientNames = new[]
            {
                "Motor Step Test Player",
                "Motor Step Test Ground",
                "Motor Step Test Low Step",
                "Motor Step Test High Wall",
                "Motor Wall Test Player",
                "Motor Wall Test Ground",
                "Motor Wall Test Wall",
                "Vehicle AB Test Ground",
                "Vehicle AB Test Arcade",
                "Vehicle AB Test Wheel",
                "Vehicle AB Test Wall"
            };

            foreach (var transientName in transientNames)
            {
                var target = GameObject.Find(transientName);
                if (target != null)
                {
                    Object.DestroyImmediate(target);
                }
            }
        }

        [UnityTest]
        public IEnumerator PrototypeVehicleMovesUnderScriptedDriveInput()
        {
            SceneManager.LoadScene("Phase1_FeelPrototype");
            yield return null;

            var player = Object.FindAnyObjectByType<PrototypePlayerController>();
            var vehicle = Object.FindAnyObjectByType<PrototypeVehicleController>();
            var metrics = Object.FindAnyObjectByType<PrototypeRunMetrics>();
            Assert.That(player, Is.Not.Null);
            Assert.That(vehicle, Is.Not.Null);
            Assert.That(metrics, Is.Not.Null);

            player.EnterVehicle(vehicle);
            var start = vehicle.transform.position;

            for (var i = 0; i < 90; i++)
            {
                vehicle.ApplyDriveInput(Vector2.up, false);
                yield return new WaitForFixedUpdate();
            }

            var moved = Vector3.Distance(start, vehicle.transform.position);
            Assert.That(moved, Is.GreaterThan(2.5f));
            Assert.That(vehicle.HasDriver, Is.True);
            Assert.That(metrics.VehicleEntryCount, Is.EqualTo(1));
            Assert.That(metrics.MaxSpeed, Is.GreaterThan(0f));
        }

        [UnityTest]
        public IEnumerator RouteCheckpointsCompleteUnderVehicleTriggerContact()
        {
            SceneManager.LoadScene("Phase1_FeelPrototype");
            yield return null;

            var player = Object.FindAnyObjectByType<PrototypePlayerController>();
            var vehicle = Object.FindAnyObjectByType<PrototypeVehicleController>();
            var route = Object.FindAnyObjectByType<PrototypeRouteProgress>();
            var metrics = Object.FindAnyObjectByType<PrototypeRunMetrics>();
            var checkpoints = Object.FindObjectsByType<PrototypeRouteCheckpoint>(FindObjectsSortMode.None)
                .OrderBy(checkpoint => checkpoint.CheckpointIndex)
                .ToArray();

            Assert.That(player, Is.Not.Null);
            Assert.That(vehicle, Is.Not.Null);
            Assert.That(route, Is.Not.Null);
            Assert.That(metrics, Is.Not.Null);
            Assert.That(checkpoints.Length, Is.EqualTo(5));

            player.EnterVehicle(vehicle);
            var body = vehicle.GetComponent<Rigidbody>();

            foreach (var checkpoint in checkpoints)
            {
                body.linearVelocity = Vector3.zero;
                body.angularVelocity = Vector3.zero;
                body.position = checkpoint.transform.position;
                Physics.SyncTransforms();
                yield return new WaitForFixedUpdate();
            }

            Assert.That(route.IsComplete, Is.True);
            Assert.That(PrototypeDebugState.Route, Is.EqualTo("Complete"));
            Assert.That(PrototypeDebugState.LastCheckpoint, Is.EqualTo("Safe return"));
            Assert.That(metrics.CompletedCheckpointCount, Is.EqualTo(5));
            Assert.That(metrics.RouteCompleted, Is.True);
            Assert.That(metrics.LastCheckpoint, Is.EqualTo("Safe return"));
        }

        [UnityTest]
        public IEnumerator MetricsRecordInteractionAndWriteSummary()
        {
            SceneManager.LoadScene("Phase1_FeelPrototype");
            yield return null;

            var metrics = Object.FindAnyObjectByType<PrototypeRunMetrics>();
            var interactable = GameObject.Find("Workshop shutter interactable")?.GetComponent<PrototypeInteractable>();

            Assert.That(metrics, Is.Not.Null);
            Assert.That(interactable, Is.Not.Null);

            interactable.Interact();

            Assert.That(metrics.InteractionCount, Is.EqualTo(1));
            Assert.That(metrics.LastInteraction, Is.EqualTo("Inspect workshop shutter"));

            var summary = metrics.BuildSummary();
            Assert.That(summary, Does.Contain("Phase 1 Feel Prototype Run"));
            Assert.That(summary, Does.Contain("Interactions: 1"));
            Assert.That(summary, Does.Contain("ManualFeelGate: Required"));
        }

        [UnityTest]
        public IEnumerator MetricsCoverageGateRequiresAllPhase1Beats()
        {
            SceneManager.LoadScene("Phase1_FeelPrototype");
            yield return null;

            var metrics = Object.FindAnyObjectByType<PrototypeRunMetrics>();
            Assert.That(metrics, Is.Not.Null);

            metrics.ResetRun();
            Assert.That(metrics.HasRouteCoverage, Is.False);
            Assert.That(metrics.BuildMissingCoverageSummary(), Does.Contain("enter car"));
            Assert.That(metrics.BuildMissingCoverageSummary(), Does.Contain("safe return"));

            metrics.RecordVehicleEnter();
            metrics.RecordSpeed(4f);
            metrics.RecordPressureEnter();
            metrics.RecordInteraction("Inspect workshop shutter");
            metrics.RecordCheckpoint("Safe return", true);

            Assert.That(metrics.HasRouteCoverage, Is.False, "Coverage must still require exiting the vehicle.");
            Assert.That(metrics.BuildMissingCoverageSummary(), Does.Contain("exit car"));

            metrics.RecordVehicleExit();

            Assert.That(metrics.HasRouteCoverage, Is.True);
            Assert.That(metrics.BuildMissingCoverageSummary(), Is.EqualTo("none"));
            Assert.That(metrics.BuildSummary(), Does.Contain("CoverageComplete: True"));
        }

        [UnityTest]
        public IEnumerator MetricsReportPreservesCompleteCoverageWhenCurrentRunIsIncomplete()
        {
            SceneManager.LoadScene("Phase1_FeelPrototype");
            yield return null;

            var metrics = Object.FindAnyObjectByType<PrototypeRunMetrics>();
            Assert.That(metrics, Is.Not.Null);

            var reportFileName = "phase1_metrics_preserve_test.txt";
            var reportPath = Path.Combine(Application.persistentDataPath, reportFileName);
            File.WriteAllText(reportPath, "CoverageComplete: True\nCoverageStatus: Coverage complete\nSentinel: complete");

            typeof(PrototypeRunMetrics)
                .GetField("reportFileName", BindingFlags.Instance | BindingFlags.NonPublic)
                ?.SetValue(metrics, reportFileName);

            metrics.ResetRun();
            metrics.RecordVehicleEnter();
            metrics.WriteReport();

            var report = File.ReadAllText(reportPath);
            Assert.That(report, Does.Contain("CoverageComplete: True"));
            Assert.That(report, Does.Contain("CoverageStatus: Coverage complete"));
            Assert.That(report, Does.Contain("Sentinel: complete"));

            File.Delete(reportPath);
            File.Delete(reportPath + ".incomplete");
        }

        [UnityTest]
        public IEnumerator SceneBeatsCanProduceCompleteCoverage()
        {
            SceneManager.LoadScene("Phase1_FeelPrototype");
            yield return null;

            var player = Object.FindAnyObjectByType<PrototypePlayerController>();
            var vehicle = Object.FindAnyObjectByType<PrototypeVehicleController>();
            var route = Object.FindAnyObjectByType<PrototypeRouteProgress>();
            var metrics = Object.FindAnyObjectByType<PrototypeRunMetrics>();
            var interactable = GameObject.Find("Workshop shutter interactable")?.GetComponent<PrototypeInteractable>();
            var checkpoints = Object.FindObjectsByType<PrototypeRouteCheckpoint>(FindObjectsSortMode.None)
                .OrderBy(checkpoint => checkpoint.CheckpointIndex)
                .ToArray();

            Assert.That(player, Is.Not.Null);
            Assert.That(vehicle, Is.Not.Null);
            Assert.That(route, Is.Not.Null);
            Assert.That(metrics, Is.Not.Null);
            Assert.That(interactable, Is.Not.Null);
            Assert.That(checkpoints.Length, Is.EqualTo(5));

            metrics.ResetRun();
            route.Configure(checkpoints.Length);
            player.EnterVehicle(vehicle);

            for (var i = 0; i < 30; i++)
            {
                vehicle.ApplyDriveInput(Vector2.up, false);
                yield return new WaitForFixedUpdate();
            }

            var body = vehicle.GetComponent<Rigidbody>();
            foreach (var checkpoint in checkpoints)
            {
                body.linearVelocity = Vector3.zero;
                body.angularVelocity = Vector3.zero;
                body.position = checkpoint.transform.position;
                Physics.SyncTransforms();
                yield return new WaitForFixedUpdate();
            }

            vehicle.ExitDriver();
            interactable.Interact();

            Assert.That(route.IsComplete, Is.True);
            Assert.That(metrics.VehicleEntryCount, Is.EqualTo(1));
            Assert.That(metrics.VehicleExitCount, Is.EqualTo(1));
            Assert.That(metrics.PressureEntryCount, Is.GreaterThanOrEqualTo(1));
            Assert.That(metrics.InteractionCount, Is.EqualTo(1));
            Assert.That(metrics.HasRouteCoverage, Is.True);
            Assert.That(metrics.CoverageStatus, Is.EqualTo("Coverage complete"));
            Assert.That(metrics.BuildSummary(), Does.Contain("ManualFeelGate: Required"));

            metrics.WriteReport();
            Assert.That(File.ReadAllText(Path.Combine(Application.persistentDataPath, "phase1_latest_run.txt")), Does.Contain("CoverageComplete: True"));
        }

        [UnityTest]
        public IEnumerator OnFootMovementFollowsCameraYaw()
        {
            SceneManager.LoadScene("Phase1_FeelPrototype");
            yield return null;

            var player = Object.FindAnyObjectByType<PrototypePlayerController>();
            var cameraRig = Object.FindAnyObjectByType<PrototypeCameraRig>();
            Assert.That(player, Is.Not.Null);
            Assert.That(cameraRig, Is.Not.Null);

            cameraRig.SetYawForTests(90f);
            var start = player.transform.position;
            player.ApplyMovementForTests(Vector2.up, false, 0.5f);

            var delta = player.transform.position - start;
            Assert.That(delta.x, Is.GreaterThan(1.2f));
            Assert.That(Mathf.Abs(delta.z), Is.LessThan(0.35f));
        }

        [UnityTest]
        public IEnumerator CameraCollisionIgnoresPrototypeMarkersButHitsWorld()
        {
            var rigObject = new GameObject("Camera Collision Rig Test");
            var rig = rigObject.AddComponent<PrototypeCameraRig>();
            var pivot = Vector3.zero;
            var desired = new Vector3(0f, 0f, -6f);

            var marker = GameObject.CreatePrimitive(PrimitiveType.Cube);
            marker.name = "Ignored Marker";
            marker.transform.position = new Vector3(0f, 0f, -2f);
            marker.transform.localScale = Vector3.one;
            marker.AddComponent<PrototypeWorldReactionMarker>();

            var wall = GameObject.CreatePrimitive(PrimitiveType.Cube);
            wall.name = "World Wall";
            wall.transform.position = new Vector3(0f, 0f, -4f);
            wall.transform.localScale = new Vector3(4f, 4f, 0.5f);

            Physics.SyncTransforms();
            var corrected = rig.ResolveCollisionForTests(pivot, desired);

            Assert.That(Vector3.Distance(pivot, corrected), Is.GreaterThan(3f));
            Assert.That(Vector3.Distance(pivot, corrected), Is.LessThan(5.5f));

            Object.Destroy(marker);
            Object.Destroy(wall);
            Object.Destroy(rigObject);
            yield return null;
        }

        [UnityTest]
        public IEnumerator VehicleBrakesBeforeReversing()
        {
            SceneManager.LoadScene("Phase1_FeelPrototype");
            yield return null;

            var player = Object.FindAnyObjectByType<PrototypePlayerController>();
            var vehicle = Object.FindAnyObjectByType<PrototypeVehicleController>();
            Assert.That(player, Is.Not.Null);
            Assert.That(vehicle, Is.Not.Null);

            player.EnterVehicle(vehicle);
            var body = vehicle.GetComponent<Rigidbody>();

            for (var i = 0; i < 60; i++)
            {
                vehicle.ApplyDriveInput(Vector2.up, false);
                yield return new WaitForFixedUpdate();
            }

            var forwardSpeed = Vector3.Dot(body.linearVelocity, vehicle.transform.forward);
            Assert.That(forwardSpeed, Is.GreaterThan(1f));

            for (var i = 0; i < 20; i++)
            {
                vehicle.ApplyDriveInput(Vector2.down, false);
                yield return new WaitForFixedUpdate();
            }

            var brakingSpeed = Vector3.Dot(body.linearVelocity, vehicle.transform.forward);
            Assert.That(brakingSpeed, Is.GreaterThanOrEqualTo(0f));
            Assert.That(brakingSpeed, Is.LessThan(forwardSpeed));
        }

        [UnityTest]
        public IEnumerator VehicleABSpikeProducesComparableMetricsAndDecisionReport()
        {
            var ground = GameObject.CreatePrimitive(PrimitiveType.Cube);
            ground.name = "Vehicle AB Test Ground";
            ground.layer = PrototypeLayers.WorldStatic;
            ground.transform.position = new Vector3(0f, -0.05f, 18f);
            ground.transform.localScale = new Vector3(28f, 0.1f, 60f);

            var wall = GameObject.CreatePrimitive(PrimitiveType.Cube);
            wall.name = "Vehicle AB Test Wall";
            wall.layer = PrototypeLayers.WorldStatic;
            wall.transform.position = new Vector3(0f, 0.9f, 30f);
            wall.transform.localScale = new Vector3(8f, 1.8f, 0.35f);

            var arcade = CreateArcadeProbeVehicle(new Vector3(-4f, 0.55f, 0f));
            var wheel = CreateWheelProbeVehicle(new Vector3(4f, 0.55f, 0f));
            Physics.SyncTransforms();

            var arcadeMetrics = PrototypeVehicleComparison.RunArcadeProbe(arcade, 1f / 50f);
            var wheelMetrics = PrototypeVehicleComparison.RunWheelProbe(wheel, 1f / 50f);
            var decision = PrototypeVehicleComparison.Decide(arcadeMetrics, wheelMetrics);
            var report = PrototypeVehicleComparison.BuildReport(arcadeMetrics, wheelMetrics, decision);
            TestContext.WriteLine(report);

            Assert.That(arcadeMetrics.Candidate, Is.EqualTo(PrototypeVehicleCandidateKind.ArcadeRigidbodyBaseline));
            Assert.That(wheelMetrics.Candidate, Is.EqualTo(PrototypeVehicleCandidateKind.WheelColliderSpike));
            Assert.That(arcadeMetrics.CompletedProbe, Is.True);
            Assert.That(arcadeMetrics.DistanceMeters, Is.GreaterThan(5f));
            Assert.That(wheelMetrics.CompletedProbe, Is.True, "The spike must run to completion even if it loses the decision.");
            Assert.That(report, Does.Contain("ArcadeRigidbodyBaseline"));
            Assert.That(report, Does.Contain("WheelColliderSpike"));
            Assert.That(report, Does.Contain("Decision:"));
            Assert.That(decision, Is.EqualTo(PrototypeVehicleDecision.KeepArcadeRigidbodyBaseline));

            Object.Destroy(arcade.gameObject);
            Object.Destroy(wheel.gameObject);
            Object.Destroy(ground);
            Object.Destroy(wall);
            yield return null;
        }

        [UnityTest]
        public IEnumerator CharacterMotorStopsAgainstWorldWall()
        {
            var playerObject = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            playerObject.name = "Motor Wall Test Player";
            playerObject.layer = PrototypeLayers.Player;
            playerObject.transform.position = new Vector3(0f, 1.05f, 0f);
            var motor = playerObject.AddComponent<PrototypeCharacterMotor>();

            var ground = GameObject.CreatePrimitive(PrimitiveType.Cube);
            ground.name = "Motor Wall Test Ground";
            ground.layer = PrototypeLayers.WorldStatic;
            ground.transform.position = new Vector3(0f, -0.05f, 1.5f);
            ground.transform.localScale = new Vector3(5f, 0.1f, 5f);

            var wall = GameObject.CreatePrimitive(PrimitiveType.Cube);
            wall.name = "Motor Wall Test Wall";
            wall.layer = PrototypeLayers.WorldStatic;
            wall.transform.position = new Vector3(0f, 1f, 1.2f);
            wall.transform.localScale = new Vector3(3f, 2f, 0.2f);

            Physics.SyncTransforms();
            motor.Move(Vector3.forward, 6f, 30f, 30f, -22f, 14f, 0.5f);
            yield return null;

            Assert.That(playerObject.transform.position.z, Is.LessThan(0.85f));
            Assert.That(motor.IsGrounded, Is.True);

            Object.Destroy(playerObject);
            Object.Destroy(ground);
            Object.Destroy(wall);
        }

        private static PrototypeVehicleController CreateArcadeProbeVehicle(Vector3 position)
        {
            var vehicleObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
            vehicleObject.name = "Vehicle AB Test Arcade";
            vehicleObject.layer = PrototypeLayers.Vehicle;
            vehicleObject.transform.position = position;
            vehicleObject.transform.localScale = new Vector3(1.9f, 1.1f, 4.1f);
            var body = vehicleObject.AddComponent<Rigidbody>();
            body.mass = 1150f;
            body.linearDamping = 0.08f;
            body.angularDamping = 0.75f;
            return vehicleObject.AddComponent<PrototypeVehicleController>();
        }

        private static PrototypeWheelVehicleController CreateWheelProbeVehicle(Vector3 position)
        {
            var vehicleObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
            vehicleObject.name = "Vehicle AB Test Wheel";
            vehicleObject.layer = PrototypeLayers.Vehicle;
            vehicleObject.transform.position = position;
            vehicleObject.transform.localScale = new Vector3(1.9f, 0.8f, 4.1f);
            var body = vehicleObject.AddComponent<Rigidbody>();
            body.mass = 1150f;
            body.linearDamping = 0.08f;
            body.angularDamping = 0.75f;
            body.centerOfMass = new Vector3(0f, -0.35f, 0f);

            var controller = vehicleObject.AddComponent<PrototypeWheelVehicleController>();
            var wheels = new[]
            {
                CreateWheel(vehicleObject.transform, "Front Left Wheel", new Vector3(-0.75f, -0.45f, 1.25f)),
                CreateWheel(vehicleObject.transform, "Front Right Wheel", new Vector3(0.75f, -0.45f, 1.25f)),
                CreateWheel(vehicleObject.transform, "Rear Left Wheel", new Vector3(-0.75f, -0.45f, -1.25f)),
                CreateWheel(vehicleObject.transform, "Rear Right Wheel", new Vector3(0.75f, -0.45f, -1.25f))
            };
            controller.ConfigureForTests(new[] { wheels[2], wheels[3] }, new[] { wheels[0], wheels[1] });
            return controller;
        }

        private static WheelCollider CreateWheel(Transform parent, string name, Vector3 localPosition)
        {
            var wheelObject = new GameObject(name);
            wheelObject.transform.SetParent(parent);
            wheelObject.transform.localPosition = localPosition;
            var wheel = wheelObject.AddComponent<WheelCollider>();
            wheel.radius = 0.32f;
            wheel.suspensionDistance = 0.25f;
            wheel.mass = 25f;
            wheel.forceAppPointDistance = 0.15f;
            var spring = wheel.suspensionSpring;
            spring.spring = 26000f;
            spring.damper = 4500f;
            spring.targetPosition = 0.5f;
            wheel.suspensionSpring = spring;
            return wheel;
        }

        [UnityTest]
        public IEnumerator CharacterMotorClimbsLowStepThenStopsAtHighWall()
        {
            var playerObject = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            playerObject.name = "Motor Step Test Player";
            playerObject.layer = PrototypeLayers.Player;
            playerObject.transform.position = new Vector3(0f, 0.08f, 0f);
            var motor = playerObject.AddComponent<PrototypeCharacterMotor>();

            var ground = GameObject.CreatePrimitive(PrimitiveType.Cube);
            ground.name = "Motor Step Test Ground";
            ground.layer = PrototypeLayers.WorldStatic;
            ground.transform.position = new Vector3(0f, -0.05f, 1.8f);
            ground.transform.localScale = new Vector3(4f, 0.1f, 5.5f);

            var lowStep = GameObject.CreatePrimitive(PrimitiveType.Cube);
            lowStep.name = "Motor Step Test Low Step";
            lowStep.layer = PrototypeLayers.WorldStatic;
            lowStep.transform.position = new Vector3(0f, 0.15f, 1.45f);
            lowStep.transform.localScale = new Vector3(1.6f, 0.3f, 1.35f);

            var highWall = GameObject.CreatePrimitive(PrimitiveType.Cube);
            highWall.name = "Motor Step Test High Wall";
            highWall.layer = PrototypeLayers.WorldStatic;
            highWall.transform.position = new Vector3(0f, 0.75f, 2.35f);
            highWall.transform.localScale = new Vector3(1.8f, 1.5f, 0.25f);

            Physics.SyncTransforms();
            for (var i = 0; i < 120; i++)
            {
                motor.Move(Vector3.forward, 3f, 30f, 30f, -22f, 14f, 1f / 60f);
                yield return null;
            }

            Assert.That(playerObject.transform.position.y, Is.GreaterThan(0.22f), "The motor should step onto low street curbs instead of treating them as full walls.");
            Assert.That(playerObject.transform.position.z, Is.GreaterThan(1.35f));
            Assert.That(playerObject.transform.position.z, Is.LessThan(2.12f), "The same motor must still reject a real wall after climbing the curb.");
            Assert.That(motor.IsGrounded, Is.True);

            Object.Destroy(playerObject);
            Object.Destroy(ground);
            Object.Destroy(lowStep);
            Object.Destroy(highWall);
        }

        [UnityTest]
        public IEnumerator CameraInteractionFocusBlendsProfileInsteadOfSnapping()
        {
            SceneManager.LoadScene("Phase1_FeelPrototype");
            yield return null;

            var player = Object.FindAnyObjectByType<PrototypePlayerController>();
            var cameraRig = Object.FindAnyObjectByType<PrototypeCameraRig>();
            var target = GameObject.Find("Public violence test target");
            Assert.That(player, Is.Not.Null);
            Assert.That(cameraRig, Is.Not.Null);
            Assert.That(target, Is.Not.Null);

            var freeDistance = PrototypeCameraRig.ResolveProfile(PrototypeCameraMode.OnFootFree).Distance;
            var interactionDistance = PrototypeCameraRig.ResolveProfile(PrototypeCameraMode.OnFootInteractionFocus).Distance;
            Assert.That(cameraRig.CurrentProfile.Distance, Is.EqualTo(freeDistance).Within(0.05f));

            player.transform.position = target.transform.position + new Vector3(0.9f, -0.7f, 0f);
            Physics.SyncTransforms();
            yield return null;

            Assert.That(cameraRig.CurrentMode, Is.EqualTo(PrototypeCameraMode.OnFootInteractionFocus));
            Assert.That(cameraRig.CurrentProfile.Distance, Is.LessThan(freeDistance));
            Assert.That(cameraRig.CurrentProfile.Distance, Is.GreaterThan(interactionDistance + 0.05f), "Interaction focus should blend in, not snap the camera distance in one frame.");
        }

        [UnityTest]
        public IEnumerator ObjectiveMarkerFollowsMissionSpineFromWorldState()
        {
            var worldObject = new GameObject("Objective Marker World Test");
            var missionObject = new GameObject("Objective Marker Mission Test");
            var markerObject = new GameObject("Objective Marker Test");
            var world = worldObject.AddComponent<PrototypeWorldState>();
            var mission = missionObject.AddComponent<PrototypeMissionSpine>();
            var marker = markerObject.AddComponent<PrototypeObjectiveMarker>();

            mission.AttachWorldState(world);
            marker.AttachMissionSpine(mission);
            marker.Refresh();

            Assert.That(marker.CurrentObjective, Is.EqualTo("Objective: collect dirty cash at El Respiro"));

            world.ApplyEvent(PrototypeWorldEvent.DirtyCashPickedUp);
            marker.Refresh();

            Assert.That(marker.CurrentObjective, Is.EqualTo("Objective: secure El Respiro or risk losing the cash"));

            Object.Destroy(markerObject);
            Object.Destroy(missionObject);
            Object.Destroy(worldObject);
            yield return null;
        }

        [UnityTest]
        public IEnumerator SceneObjectsUseFoundationLayers()
        {
            SceneManager.LoadScene("Phase1_FeelPrototype");
            yield return null;

            Assert.That(GameObject.Find("Ground").layer, Is.EqualTo(PrototypeLayers.WorldStatic));
            Assert.That(GameObject.Find("Pablo Valera Prototype Controller").layer, Is.EqualTo(PrototypeLayers.Player));
            Assert.That(GameObject.Find("Prototype Sedan").layer, Is.EqualTo(PrototypeLayers.Vehicle));
            Assert.That(GameObject.Find("Route checkpoint 0: Start on foot").layer, Is.EqualTo(PrototypeLayers.RouteTrigger));
            Assert.That(GameObject.Find("Pressure patrol marker").layer, Is.EqualTo(PrototypeLayers.SensorTrigger));
            Assert.That(GameObject.Find("Workshop shutter interactable").layer, Is.EqualTo(PrototypeLayers.Interactable));
            Assert.That(GameObject.Find("Motor proof low step").layer, Is.EqualTo(PrototypeLayers.WorldStatic));
            Assert.That(GameObject.Find("Tight camera recovery wall").layer, Is.EqualTo(PrototypeLayers.WorldStatic));
        }

        [UnityTest]
        public IEnumerator PublicViolenceMicrotestChangesWorldAndVisibleMarkers()
        {
            SceneManager.LoadScene("Phase1_FeelPrototype");
            yield return null;

            var world = Object.FindAnyObjectByType<PrototypeWorldState>();
            var mission = Object.FindAnyObjectByType<PrototypeMissionSpine>();
            var marker = Object.FindAnyObjectByType<PrototypeObjectiveMarker>();
            var violenceTarget = GameObject.Find("Public violence test target")?.GetComponent<PrototypeInteractable>();
            var reactionMarkers = Object.FindObjectsByType<PrototypeWorldReactionMarker>(FindObjectsSortMode.None);

            Assert.That(world, Is.Not.Null);
            Assert.That(mission, Is.Not.Null);
            Assert.That(marker, Is.Not.Null);
            Assert.That(violenceTarget, Is.Not.Null);
            Assert.That(reactionMarkers.Length, Is.GreaterThanOrEqualTo(3));

            violenceTarget.Interact();
            marker.Refresh();
            yield return null;

            Assert.That(world.LastEvent, Is.EqualTo(PrototypeWorldEvent.PublicViolenceCommitted));
            Assert.That(world.Fear, Is.EqualTo(SocialLevel.High));
            Assert.That(world.PeopleLove, Is.EqualTo(SocialLevel.Low));
            Assert.That(world.StatePressure, Is.EqualTo(PressureLevel.Medium));
            Assert.That(world.RuleStyleDecision, Is.EqualTo(RuleStyle.ShowOfForce));
            Assert.That(mission.Stage, Is.EqualTo(PrototypeMissionStage.ActionPressure));
            Assert.That(marker.CurrentObjective, Is.EqualTo("Objective: contain street pressure before patrol locks the route"));
            Assert.That(PrototypeDebugState.World, Does.Contain("StatePressure: Medium"));
            Assert.That(PrototypeDebugState.WorldReaction, Does.Contain("after"));
            Assert.That(reactionMarkers.Count(marker => marker.Reacted), Is.GreaterThanOrEqualTo(3));
        }

        [UnityTest]
        public IEnumerator Phase2RPressureBeatHasRuntimeSuccessAndFailureBranches()
        {
            SceneManager.LoadScene("Phase1_FeelPrototype");
            yield return null;

            var world = Object.FindAnyObjectByType<PrototypeWorldState>();
            var mission = Object.FindAnyObjectByType<PrototypeMissionSpine>();
            var marker = Object.FindAnyObjectByType<PrototypeObjectiveMarker>();
            var violenceTarget = GameObject.Find("Public violence test target")?.GetComponent<PrototypeInteractable>();
            var bribeOfficer = GameObject.Find("Rios bribe test officer")?.GetComponent<PrototypeInteractable>();

            Assert.That(world, Is.Not.Null);
            Assert.That(mission, Is.Not.Null);
            Assert.That(marker, Is.Not.Null);
            Assert.That(violenceTarget, Is.Not.Null);
            Assert.That(bribeOfficer, Is.Not.Null);

            violenceTarget.Interact();
            bribeOfficer.Interact();
            marker.Refresh();
            yield return null;

            Assert.That(world.LastEvent, Is.EqualTo(PrototypeWorldEvent.BribeAccepted));
            Assert.That(world.StatePressure, Is.EqualTo(PressureLevel.Low));
            Assert.That(mission.Stage, Is.EqualTo(PrototypeMissionStage.PressureContained));
            Assert.That(marker.CurrentObjective, Is.EqualTo("Objective: pressure contained, continue to El Respiro"));

            world.ResetState();
            world.ApplyEvent(PrototypeWorldEvent.PublicViolenceCommitted);
            world.ApplyEvent(PrototypeWorldEvent.PressureCrackdownTriggered);
            marker.Refresh();
            yield return null;

            Assert.That(world.LastEvent, Is.EqualTo(PrototypeWorldEvent.PressureCrackdownTriggered));
            Assert.That(world.StatePressure, Is.EqualTo(PressureLevel.High));
            Assert.That(mission.Stage, Is.EqualTo(PrototypeMissionStage.PressureFailure));
            Assert.That(marker.CurrentObjective, Is.EqualTo("Objective changed: escape the patrol pressure"));
        }

        [UnityTest]
        public IEnumerator Phase2RPressureScenePlaybackMovesPatrolAndOpensRoadblock()
        {
            SceneManager.LoadScene("Phase1_FeelPrototype");
            yield return null;

            var world = Object.FindAnyObjectByType<PrototypeWorldState>();
            var playback = Object.FindAnyObjectByType<PrototypePressureScenePlayback>();
            var patrolMarker = GameObject.Find("Police pressure moves closer marker");
            var roadblock = GameObject.Find("Bribe roadblock opens marker");
            var roadblockCollider = roadblock?.GetComponent<Collider>();

            Assert.That(world, Is.Not.Null);
            Assert.That(playback, Is.Not.Null);
            Assert.That(patrolMarker, Is.Not.Null);
            Assert.That(roadblock, Is.Not.Null);
            Assert.That(roadblockCollider, Is.Not.Null);

            var patrolStart = patrolMarker.transform.position;
            var roadblockStart = roadblock.transform.position;

            world.ApplyEvent(PrototypeWorldEvent.PublicViolenceCommitted);
            yield return null;

            Assert.That(playback.State, Is.EqualTo(PrototypePressurePlaybackState.PressureRising));
            Assert.That(Vector3.Distance(patrolStart, patrolMarker.transform.position), Is.GreaterThan(1.5f));
            Assert.That(roadblockCollider.enabled, Is.True);

            world.ApplyEvent(PrototypeWorldEvent.BribeAccepted);
            yield return null;

            Assert.That(playback.State, Is.EqualTo(PrototypePressurePlaybackState.Contained));
            Assert.That(Vector3.Distance(roadblockStart, roadblock.transform.position), Is.GreaterThan(2.5f));
            Assert.That(roadblockCollider.enabled, Is.False);

            world.ApplyEvent(PrototypeWorldEvent.PublicViolenceCommitted);
            world.ApplyEvent(PrototypeWorldEvent.PressureCrackdownTriggered);
            yield return null;

            Assert.That(playback.State, Is.EqualTo(PrototypePressurePlaybackState.Crackdown));
            Assert.That(roadblockCollider.enabled, Is.True);
            Assert.That(Vector3.Distance(roadblockStart, roadblock.transform.position), Is.LessThan(0.05f));
        }

        [UnityTest]
        public IEnumerator Phase2RPlayablePressureChoiceBribeLetsVehicleThroughPressureZone()
        {
            SceneManager.LoadScene("Phase1_FeelPrototype");
            yield return null;

            var world = Object.FindAnyObjectByType<PrototypeWorldState>();
            var mission = Object.FindAnyObjectByType<PrototypeMissionSpine>();
            var marker = Object.FindAnyObjectByType<PrototypeObjectiveMarker>();
            var vehicle = Object.FindAnyObjectByType<PrototypeVehicleController>();
            var choice = Object.FindAnyObjectByType<PrototypePressureChoiceController>();
            var pressureZone = GameObject.Find("Pressure patrol marker");
            var violenceTarget = GameObject.Find("Public violence test target")?.GetComponent<PrototypeInteractable>();
            var bribeOfficer = GameObject.Find("Rios bribe test officer")?.GetComponent<PrototypeInteractable>();

            Assert.That(world, Is.Not.Null);
            Assert.That(mission, Is.Not.Null);
            Assert.That(marker, Is.Not.Null);
            Assert.That(vehicle, Is.Not.Null);
            Assert.That(choice, Is.Not.Null);
            Assert.That(pressureZone, Is.Not.Null);
            Assert.That(violenceTarget, Is.Not.Null);
            Assert.That(bribeOfficer, Is.Not.Null);

            violenceTarget.Interact();
            bribeOfficer.Interact();
            yield return MoveVehicleIntoPressureZone(vehicle, pressureZone.transform);
            marker.Refresh();

            Assert.That(world.LastEvent, Is.EqualTo(PrototypeWorldEvent.BribeAccepted));
            Assert.That(world.StatePressure, Is.EqualTo(PressureLevel.Low));
            Assert.That(mission.Stage, Is.EqualTo(PrototypeMissionStage.PressureContained));
            Assert.That(marker.CurrentObjective, Is.EqualTo("Objective: pressure contained, continue to El Respiro"));
            Assert.That(choice.LastResolution, Is.EqualTo(PrototypePressureChoiceResolution.Contained));
        }

        [UnityTest]
        public IEnumerator Phase2RPlayablePressureChoiceUncontainedZoneEntryTriggersCrackdown()
        {
            SceneManager.LoadScene("Phase1_FeelPrototype");
            yield return null;

            var world = Object.FindAnyObjectByType<PrototypeWorldState>();
            var mission = Object.FindAnyObjectByType<PrototypeMissionSpine>();
            var marker = Object.FindAnyObjectByType<PrototypeObjectiveMarker>();
            var vehicle = Object.FindAnyObjectByType<PrototypeVehicleController>();
            var choice = Object.FindAnyObjectByType<PrototypePressureChoiceController>();
            var playback = Object.FindAnyObjectByType<PrototypePressureScenePlayback>();
            var pressureZone = GameObject.Find("Pressure patrol marker");
            var violenceTarget = GameObject.Find("Public violence test target")?.GetComponent<PrototypeInteractable>();

            Assert.That(world, Is.Not.Null);
            Assert.That(mission, Is.Not.Null);
            Assert.That(marker, Is.Not.Null);
            Assert.That(vehicle, Is.Not.Null);
            Assert.That(choice, Is.Not.Null);
            Assert.That(playback, Is.Not.Null);
            Assert.That(pressureZone, Is.Not.Null);
            Assert.That(violenceTarget, Is.Not.Null);

            violenceTarget.Interact();
            yield return MoveVehicleIntoPressureZone(vehicle, pressureZone.transform);
            marker.Refresh();

            Assert.That(world.LastEvent, Is.EqualTo(PrototypeWorldEvent.PressureCrackdownTriggered));
            Assert.That(world.StatePressure, Is.EqualTo(PressureLevel.High));
            Assert.That(mission.Stage, Is.EqualTo(PrototypeMissionStage.PressureFailure));
            Assert.That(marker.CurrentObjective, Is.EqualTo("Objective changed: escape the patrol pressure"));
            Assert.That(choice.LastResolution, Is.EqualTo(PrototypePressureChoiceResolution.Crackdown));
            Assert.That(playback.State, Is.EqualTo(PrototypePressurePlaybackState.Crackdown));
        }

        [UnityTest]
        public IEnumerator Phase2RContainedPressureRouteCompletesNormalCoverage()
        {
            SceneManager.LoadScene("Phase1_FeelPrototype");
            yield return null;

            var player = Object.FindAnyObjectByType<PrototypePlayerController>();
            var vehicle = Object.FindAnyObjectByType<PrototypeVehicleController>();
            var route = Object.FindAnyObjectByType<PrototypeRouteProgress>();
            var metrics = Object.FindAnyObjectByType<PrototypeRunMetrics>();
            var violenceTarget = GameObject.Find("Public violence test target")?.GetComponent<PrototypeInteractable>();
            var bribeOfficer = GameObject.Find("Rios bribe test officer")?.GetComponent<PrototypeInteractable>();
            var checkpoints = Object.FindObjectsByType<PrototypeRouteCheckpoint>(FindObjectsSortMode.None)
                .OrderBy(checkpoint => checkpoint.CheckpointIndex)
                .ToArray();

            Assert.That(player, Is.Not.Null);
            Assert.That(vehicle, Is.Not.Null);
            Assert.That(route, Is.Not.Null);
            Assert.That(metrics, Is.Not.Null);
            Assert.That(violenceTarget, Is.Not.Null);
            Assert.That(bribeOfficer, Is.Not.Null);
            Assert.That(checkpoints.Length, Is.EqualTo(5));

            metrics.ResetRun();
            route.Configure(checkpoints.Length);
            player.EnterVehicle(vehicle);
            metrics.RecordSpeed(4f);
            violenceTarget.Interact();
            bribeOfficer.Interact();

            foreach (var checkpoint in checkpoints)
            {
                yield return MoveVehicleToTransform(vehicle, checkpoint.transform);
            }

            vehicle.ExitDriver();

            Assert.That(route.IsComplete, Is.True);
            Assert.That(route.Outcome, Is.EqualTo(PrototypeRouteOutcome.PressureContained));
            Assert.That(metrics.RouteCompleted, Is.True);
            Assert.That(metrics.RouteOutcome, Is.EqualTo(PrototypeRouteOutcome.PressureContained));
            Assert.That(metrics.HasRouteCoverage, Is.True);
        }

        [UnityTest]
        public IEnumerator Phase2RPressureFailureBlocksNormalRouteAndSafeReturnBecomesEscape()
        {
            SceneManager.LoadScene("Phase1_FeelPrototype");
            yield return null;

            var vehicle = Object.FindAnyObjectByType<PrototypeVehicleController>();
            var route = Object.FindAnyObjectByType<PrototypeRouteProgress>();
            var metrics = Object.FindAnyObjectByType<PrototypeRunMetrics>();
            var violenceTarget = GameObject.Find("Public violence test target")?.GetComponent<PrototypeInteractable>();
            var checkpoints = Object.FindObjectsByType<PrototypeRouteCheckpoint>(FindObjectsSortMode.None)
                .OrderBy(checkpoint => checkpoint.CheckpointIndex)
                .ToArray();

            Assert.That(vehicle, Is.Not.Null);
            Assert.That(route, Is.Not.Null);
            Assert.That(metrics, Is.Not.Null);
            Assert.That(violenceTarget, Is.Not.Null);
            Assert.That(checkpoints.Length, Is.EqualTo(5));

            metrics.ResetRun();
            route.Configure(checkpoints.Length);

            yield return MoveVehicleToTransform(vehicle, checkpoints[0].transform);
            yield return MoveVehicleToTransform(vehicle, checkpoints[1].transform);
            violenceTarget.Interact();
            yield return MoveVehicleToTransform(vehicle, checkpoints[2].transform);

            Assert.That(Object.FindAnyObjectByType<PrototypeWorldState>().LastEvent, Is.EqualTo(PrototypeWorldEvent.PressureCrackdownTriggered));

            yield return MoveVehicleToTransform(vehicle, checkpoints[3].transform);

            Assert.That(route.IsComplete, Is.False);
            Assert.That(route.Outcome, Is.EqualTo(PrototypeRouteOutcome.PressureBlocked));
            Assert.That(metrics.RouteCompleted, Is.False);

            yield return MoveVehicleToTransform(vehicle, checkpoints[4].transform);

            Assert.That(route.IsComplete, Is.False);
            Assert.That(route.Outcome, Is.EqualTo(PrototypeRouteOutcome.PressureFailureEscape));
            Assert.That(metrics.RouteOutcome, Is.EqualTo(PrototypeRouteOutcome.PressureFailureEscape));
            Assert.That(metrics.HasRouteCoverage, Is.False);
            Assert.That(metrics.BuildSummary(), Does.Contain("RouteOutcome: PressureFailureEscape"));
            Assert.That(PrototypeDebugState.Route, Does.Contain("Pressure escape"));
        }

        [UnityTest]
        public IEnumerator BribeMicrotestReducesPressureAndLeavesVisibleLeverage()
        {
            SceneManager.LoadScene("Phase1_FeelPrototype");
            yield return null;

            var world = Object.FindAnyObjectByType<PrototypeWorldState>();
            var bribeOfficer = GameObject.Find("Rios bribe test officer")?.GetComponent<PrototypeInteractable>();
            var bribeMarkers = Object.FindObjectsByType<PrototypeWorldReactionMarker>(FindObjectsSortMode.None)
                .Where(marker => marker.ReactionMessage.Contains("bribe") || marker.ReactionMessage.Contains("Rios") || marker.ReactionMessage.Contains("Risk cargo"))
                .ToArray();

            Assert.That(world, Is.Not.Null);
            Assert.That(bribeOfficer, Is.Not.Null);
            Assert.That(bribeMarkers.Length, Is.EqualTo(3));

            world.ApplyEvent(PrototypeWorldEvent.PublicViolenceCommitted);
            bribeOfficer.Interact();
            yield return null;

            Assert.That(world.LastEvent, Is.EqualTo(PrototypeWorldEvent.BribeAccepted));
            Assert.That(world.StatePressure, Is.EqualTo(PressureLevel.Low));
            Assert.That(world.DirtyCash, Is.EqualTo(DirtyCashState.Hidden));
            Assert.That(world.RuleStyleDecision, Is.EqualTo(RuleStyle.Bribe));
            Assert.That(PrototypeDebugState.World, Does.Contain("DirtyCash: Hidden"));
            Assert.That(bribeMarkers.Count(marker => marker.Reacted), Is.EqualTo(3));
        }

        [UnityTest]
        public IEnumerator MateoMicrotestBranchesTrustIntoDifferentVisibleWarnings()
        {
            SceneManager.LoadScene("Phase1_FeelPrototype");
            yield return null;

            var world = Object.FindAnyObjectByType<PrototypeWorldState>();
            var protectedContact = GameObject.Find("Mateo protected test contact")?.GetComponent<PrototypeInteractable>();
            var earlyWarning = GameObject.Find("Mateo early warning marker")?.GetComponent<PrototypeWorldReactionMarker>();

            Assert.That(world, Is.Not.Null);
            Assert.That(protectedContact, Is.Not.Null);
            Assert.That(earlyWarning, Is.Not.Null);

            protectedContact.Interact();
            yield return null;

            Assert.That(world.LastEvent, Is.EqualTo(PrototypeWorldEvent.MateoProtected));
            Assert.That(world.LieutenantTrust, Is.EqualTo(LieutenantTrust.Trusted));
            Assert.That(earlyWarning.Reacted, Is.True);

            world.ResetState();
            var humiliatedContact = GameObject.Find("Mateo humiliated test contact")?.GetComponent<PrototypeInteractable>();
            var lateWarning = GameObject.Find("Mateo late warning marker")?.GetComponent<PrototypeWorldReactionMarker>();

            Assert.That(humiliatedContact, Is.Not.Null);
            Assert.That(lateWarning, Is.Not.Null);

            humiliatedContact.Interact();
            yield return null;

            Assert.That(world.LastEvent, Is.EqualTo(PrototypeWorldEvent.MateoHumiliated));
            Assert.That(world.LieutenantTrust, Is.EqualTo(LieutenantTrust.Humiliated));
            Assert.That(lateWarning.Reacted, Is.True);
        }

        [UnityTest]
        public IEnumerator Phase3LoadRestoresWorldStateAndVisibleReactions()
        {
            SceneManager.LoadScene("Phase1_FeelPrototype");
            yield return null;

            var world = Object.FindAnyObjectByType<PrototypeWorldState>();
            var bribeOfficer = GameObject.Find("Rios bribe test officer")?.GetComponent<PrototypeInteractable>();
            var bribeMarkers = Object.FindObjectsByType<PrototypeWorldReactionMarker>(FindObjectsSortMode.None)
                .Where(marker => marker.ReactionMessage.Contains("bribe") || marker.ReactionMessage.Contains("Rios") || marker.ReactionMessage.Contains("Risk cargo"))
                .ToArray();
            var snapshotPath = Path.Combine(Application.temporaryCachePath, "phase3_world_state_runtime_snapshot.json");

            Assert.That(world, Is.Not.Null);
            Assert.That(bribeOfficer, Is.Not.Null);
            Assert.That(bribeMarkers.Length, Is.EqualTo(3));

            world.ApplyEvent(PrototypeWorldEvent.PublicViolenceCommitted);
            bribeOfficer.Interact();
            yield return null;

            world.SaveSnapshot(snapshotPath);
            Assert.That(File.Exists(snapshotPath), Is.True);

            world.ResetState();
            yield return null;

            Assert.That(world.LastEvent, Is.EqualTo(PrototypeWorldEvent.None));
            Assert.That(bribeMarkers.Count(marker => marker.Reacted), Is.EqualTo(0));

            world.LoadSnapshot(snapshotPath);
            yield return null;

            Assert.That(world.LastEvent, Is.EqualTo(PrototypeWorldEvent.BribeAccepted));
            Assert.That(world.DirtyCash, Is.EqualTo(DirtyCashState.Hidden));
            Assert.That(world.StatePressure, Is.EqualTo(PressureLevel.Low));
            Assert.That(world.RuleStyleDecision, Is.EqualTo(RuleStyle.Bribe));
            Assert.That(PrototypeDebugState.World, Does.Contain("LastEvent: BribeAccepted"));
            Assert.That(bribeMarkers.Count(marker => marker.Reacted), Is.EqualTo(3));

            if (File.Exists(snapshotPath))
            {
                File.Delete(snapshotPath);
            }
        }

        [UnityTest]
        public IEnumerator Phase4FrontPrototypeChangesWorldAndVisibleMarkers()
        {
            SceneManager.LoadScene("Phase1_FeelPrototype");
            yield return null;

            var world = Object.FindAnyObjectByType<PrototypeWorldState>();
            var cashPickup = GameObject.Find("El Respiro dirty cash pickup")?.GetComponent<PrototypeInteractable>();
            var frontTakeover = GameObject.Find("El Respiro front takeover")?.GetComponent<PrototypeInteractable>();
            var carriedMarker = GameObject.Find("Dirty cash carried marker")?.GetComponent<PrototypeWorldReactionMarker>();
            var frontMarkers = Object.FindObjectsByType<PrototypeWorldReactionMarker>(FindObjectsSortMode.None)
                .Where(marker => marker.ReactionMessage.Contains("El Respiro") || marker.ReactionMessage.Contains("Barrio notices"))
                .ToArray();

            Assert.That(world, Is.Not.Null);
            Assert.That(cashPickup, Is.Not.Null);
            Assert.That(frontTakeover, Is.Not.Null);
            Assert.That(carriedMarker, Is.Not.Null);
            Assert.That(frontMarkers.Length, Is.EqualTo(2));

            cashPickup.Interact();
            yield return null;

            Assert.That(world.DirtyCash, Is.EqualTo(DirtyCashState.Carried));
            Assert.That(world.StatePressure, Is.EqualTo(PressureLevel.Medium));
            Assert.That(carriedMarker.Reacted, Is.True);

            frontTakeover.Interact();
            yield return null;

            Assert.That(world.FrontControl, Is.EqualTo(FrontControl.PabloWatched));
            Assert.That(world.DirtyCash, Is.EqualTo(DirtyCashState.Hidden));
            Assert.That(world.RuleStyleDecision, Is.EqualTo(RuleStyle.Favor));
            Assert.That(world.StatePressure, Is.EqualTo(PressureLevel.High));
            Assert.That(frontMarkers.Count(marker => marker.Reacted), Is.EqualTo(2));
        }

        [UnityTest]
        public IEnumerator Phase4TrustedMateoReducesFrontTakeoverPressure()
        {
            SceneManager.LoadScene("Phase1_FeelPrototype");
            yield return null;

            var world = Object.FindAnyObjectByType<PrototypeWorldState>();
            var mateo = GameObject.Find("Mateo protected test contact")?.GetComponent<PrototypeInteractable>();
            var cashPickup = GameObject.Find("El Respiro dirty cash pickup")?.GetComponent<PrototypeInteractable>();
            var frontTakeover = GameObject.Find("El Respiro front takeover")?.GetComponent<PrototypeInteractable>();

            Assert.That(world, Is.Not.Null);
            Assert.That(mateo, Is.Not.Null);
            Assert.That(cashPickup, Is.Not.Null);
            Assert.That(frontTakeover, Is.Not.Null);

            mateo.Interact();
            cashPickup.Interact();
            frontTakeover.Interact();
            yield return null;

            Assert.That(world.LieutenantTrust, Is.EqualTo(LieutenantTrust.Trusted));
            Assert.That(world.FrontControl, Is.EqualTo(FrontControl.PabloWatched));
            Assert.That(world.DirtyCash, Is.EqualTo(DirtyCashState.Hidden));
            Assert.That(world.StatePressure, Is.EqualTo(PressureLevel.Low));
        }

        [UnityTest]
        public IEnumerator Phase5MissionSpineTracksRuntimeSuccessPath()
        {
            SceneManager.LoadScene("Phase1_FeelPrototype");
            yield return null;

            var mission = Object.FindAnyObjectByType<PrototypeMissionSpine>();
            var cashPickup = GameObject.Find("El Respiro dirty cash pickup")?.GetComponent<PrototypeInteractable>();
            var frontTakeover = GameObject.Find("El Respiro front takeover")?.GetComponent<PrototypeInteractable>();

            Assert.That(mission, Is.Not.Null);
            Assert.That(cashPickup, Is.Not.Null);
            Assert.That(frontTakeover, Is.Not.Null);
            Assert.That(mission.Stage, Is.EqualTo(PrototypeMissionStage.FindingFront));

            cashPickup.Interact();
            yield return null;

            Assert.That(mission.Stage, Is.EqualTo(PrototypeMissionStage.CarryingRisk));
            Assert.That(PrototypeDebugState.Mission, Does.Contain("dirty cash is exposed"));

            frontTakeover.Interact();
            yield return null;

            Assert.That(mission.Stage, Is.EqualTo(PrototypeMissionStage.FrontSecured));
            Assert.That(PrototypeDebugState.Mission, Does.Contain("secured"));
        }

        [UnityTest]
        public IEnumerator Phase5MissionSpineAllowsPartialFailureWithoutRestart()
        {
            SceneManager.LoadScene("Phase1_FeelPrototype");
            yield return null;

            var world = Object.FindAnyObjectByType<PrototypeWorldState>();
            var mission = Object.FindAnyObjectByType<PrototypeMissionSpine>();
            var cashPickup = GameObject.Find("El Respiro dirty cash pickup")?.GetComponent<PrototypeInteractable>();
            var seizure = GameObject.Find("Dirty cash seizure failstate")?.GetComponent<PrototypeInteractable>();
            var failureMarker = GameObject.Find("Seized cash partial failure marker")?.GetComponent<PrototypeWorldReactionMarker>();

            Assert.That(world, Is.Not.Null);
            Assert.That(mission, Is.Not.Null);
            Assert.That(cashPickup, Is.Not.Null);
            Assert.That(seizure, Is.Not.Null);
            Assert.That(failureMarker, Is.Not.Null);

            cashPickup.Interact();
            seizure.Interact();
            yield return null;

            Assert.That(world.DirtyCash, Is.EqualTo(DirtyCashState.Seized));
            Assert.That(world.StatePressure, Is.EqualTo(PressureLevel.High));
            Assert.That(mission.Stage, Is.EqualTo(PrototypeMissionStage.PartialFailure));
            Assert.That(failureMarker.Reacted, Is.True);
            Assert.That(PrototypeDebugState.Mission, Does.Contain("partial failure"));
        }

        private static IEnumerator MoveVehicleIntoPressureZone(PrototypeVehicleController vehicle, Transform pressureZone)
        {
            var body = vehicle.GetComponent<Rigidbody>();
            Assert.That(body, Is.Not.Null);

            body.linearVelocity = Vector3.zero;
            body.angularVelocity = Vector3.zero;
            body.position = pressureZone.position + Vector3.back * 6f;
            vehicle.transform.position = body.position;
            Physics.SyncTransforms();
            yield return new WaitForFixedUpdate();

            body.linearVelocity = Vector3.zero;
            body.angularVelocity = Vector3.zero;
            body.position = pressureZone.position;
            vehicle.transform.position = body.position;
            Physics.SyncTransforms();
            yield return new WaitForFixedUpdate();
            yield return null;
        }

        private static IEnumerator MoveVehicleToTransform(PrototypeVehicleController vehicle, Transform target)
        {
            var body = vehicle.GetComponent<Rigidbody>();
            Assert.That(body, Is.Not.Null);

            body.linearVelocity = Vector3.zero;
            body.angularVelocity = Vector3.zero;
            body.position = target.position + Vector3.left * 12f;
            vehicle.transform.position = body.position;
            Physics.SyncTransforms();
            yield return new WaitForFixedUpdate();

            body.linearVelocity = Vector3.zero;
            body.angularVelocity = Vector3.zero;
            body.position = target.position;
            vehicle.transform.position = body.position;
            Physics.SyncTransforms();
            yield return new WaitForFixedUpdate();
            yield return null;
        }
    }
}
