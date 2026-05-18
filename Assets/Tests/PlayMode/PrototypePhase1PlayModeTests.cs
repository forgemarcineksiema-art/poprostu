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
    }
}
