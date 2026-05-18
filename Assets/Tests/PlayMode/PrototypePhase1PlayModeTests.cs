using System.Collections;
using System.IO;
using System.Linq;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using ValleDePlata.Prototype;

namespace ValleDePlata.Tests
{
    public sealed class PrototypePhase1PlayModeTests
    {
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
        }

        [UnityTest]
        public IEnumerator PublicViolenceMicrotestChangesWorldAndVisibleMarkers()
        {
            SceneManager.LoadScene("Phase1_FeelPrototype");
            yield return null;

            var world = Object.FindAnyObjectByType<PrototypeWorldState>();
            var violenceTarget = GameObject.Find("Public violence test target")?.GetComponent<PrototypeInteractable>();
            var reactionMarkers = Object.FindObjectsByType<PrototypeWorldReactionMarker>(FindObjectsSortMode.None);

            Assert.That(world, Is.Not.Null);
            Assert.That(violenceTarget, Is.Not.Null);
            Assert.That(reactionMarkers.Length, Is.GreaterThanOrEqualTo(3));

            violenceTarget.Interact();
            yield return null;

            Assert.That(world.LastEvent, Is.EqualTo(PrototypeWorldEvent.PublicViolenceCommitted));
            Assert.That(world.Fear, Is.EqualTo(SocialLevel.High));
            Assert.That(world.PeopleLove, Is.EqualTo(SocialLevel.Low));
            Assert.That(world.StatePressure, Is.EqualTo(PressureLevel.Medium));
            Assert.That(world.RuleStyleDecision, Is.EqualTo(RuleStyle.ShowOfForce));
            Assert.That(PrototypeDebugState.World, Does.Contain("StatePressure: Medium"));
            Assert.That(PrototypeDebugState.WorldReaction, Does.Contain("after"));
            Assert.That(reactionMarkers.Count(marker => marker.Reacted), Is.GreaterThanOrEqualTo(3));
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
    }
}
