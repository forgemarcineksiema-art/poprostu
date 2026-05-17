using System.Collections;
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
            Assert.That(player, Is.Not.Null);
            Assert.That(vehicle, Is.Not.Null);

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
        }

        [UnityTest]
        public IEnumerator RouteCheckpointsCompleteUnderVehicleTriggerContact()
        {
            SceneManager.LoadScene("Phase1_FeelPrototype");
            yield return null;

            var player = Object.FindAnyObjectByType<PrototypePlayerController>();
            var vehicle = Object.FindAnyObjectByType<PrototypeVehicleController>();
            var route = Object.FindAnyObjectByType<PrototypeRouteProgress>();
            var checkpoints = Object.FindObjectsByType<PrototypeRouteCheckpoint>(FindObjectsSortMode.None)
                .OrderBy(checkpoint => checkpoint.CheckpointIndex)
                .ToArray();

            Assert.That(player, Is.Not.Null);
            Assert.That(vehicle, Is.Not.Null);
            Assert.That(route, Is.Not.Null);
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
        }
    }
}
