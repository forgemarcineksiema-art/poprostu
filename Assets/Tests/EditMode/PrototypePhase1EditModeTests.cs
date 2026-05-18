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
            RequireComponent<PrototypeCharacterMotor>("Pablo Valera Prototype Controller");
            RequireComponent<PrototypeVehicleController>("Prototype Sedan");
            RequireComponent<PrototypeCameraRig>("Prototype Camera Rig");
            RequireComponent<PrototypeDebugHud>("Prototype Debug HUD");
            RequireComponent<PrototypeRunMetrics>("Phase 1 Run Metrics");
            RequireComponent<PrototypeWorldState>("Prototype World State");
            RequireComponent<PrototypeMissionSpine>("Pierwszy Front Mission Spine");
            RequireComponent<PrototypeObjectiveMarker>("Prototype Objective Marker");
            RequireComponent<PrototypePressureZone>("Pressure patrol marker");
            RequireComponent<PrototypePressureChoiceController>("Pressure patrol marker");
            RequireComponent<PrototypePressureScenePlayback>("Pressure patrol marker");
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
            RequireObject("Motor proof low step");
            RequireObject("Motor proof high wall");
            RequireObject("Motor proof steep slope");
            RequireObject("Tight camera recovery wall");
            RequireObject("Safe return marker");
            RequireObject("Fallback Exit Point");
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
        public void Phase5MissionSpinePublishesPlayableObjectivePrompts()
        {
            var worldObject = new GameObject("Mission Objective World State Test");
            var missionObject = new GameObject("Mission Objective Spine Test");
            var world = worldObject.AddComponent<PrototypeWorldState>();
            var mission = missionObject.AddComponent<PrototypeMissionSpine>();

            mission.AttachWorldState(world);

            Assert.That(mission.ObjectivePrompt, Is.EqualTo("Objective: collect dirty cash at El Respiro"));
            Assert.That(mission.IsPhase5Resolved, Is.False);
            Assert.That(PrototypeDebugState.Mission, Does.Contain("collect dirty cash"));

            world.ApplyEvent(PrototypeWorldEvent.DirtyCashPickedUp);

            Assert.That(mission.Stage, Is.EqualTo(PrototypeMissionStage.CarryingRisk));
            Assert.That(mission.ObjectivePrompt, Is.EqualTo("Objective: secure El Respiro or risk losing the cash"));
            Assert.That(mission.IsPhase5Resolved, Is.False);

            world.ApplyEvent(PrototypeWorldEvent.FrontTakenUnderWatch);

            Assert.That(mission.Stage, Is.EqualTo(PrototypeMissionStage.FrontSecured));
            Assert.That(mission.ObjectivePrompt, Is.EqualTo("Objective complete: exit through Safe return"));
            Assert.That(mission.IsPhase5Resolved, Is.True);
            Assert.That(PrototypeDebugState.Mission, Does.Contain("Phase 5 resolved"));

            Object.DestroyImmediate(missionObject);
            Object.DestroyImmediate(worldObject);
        }

        [Test]
        public void Phase2RPressureBeatPublishesObjectiveBranchesFromWorldState()
        {
            var worldObject = new GameObject("Phase2R Pressure World State Test");
            var missionObject = new GameObject("Phase2R Pressure Mission Spine Test");
            var world = worldObject.AddComponent<PrototypeWorldState>();
            var mission = missionObject.AddComponent<PrototypeMissionSpine>();

            mission.AttachWorldState(world);
            world.ApplyEvent(PrototypeWorldEvent.PublicViolenceCommitted);

            Assert.That(mission.Stage, Is.EqualTo(PrototypeMissionStage.ActionPressure));
            Assert.That(mission.ObjectivePrompt, Is.EqualTo("Objective: contain street pressure before patrol locks the route"));

            world.ApplyEvent(PrototypeWorldEvent.BribeAccepted);
            Assert.That(mission.Stage, Is.EqualTo(PrototypeMissionStage.PressureContained));
            Assert.That(mission.ObjectivePrompt, Is.EqualTo("Objective: pressure contained, continue to El Respiro"));

            world.ResetState();
            world.ApplyEvent(PrototypeWorldEvent.PublicViolenceCommitted);
            world.ApplyEvent(PrototypeWorldEvent.PressureCrackdownTriggered);

            Assert.That(mission.Stage, Is.EqualTo(PrototypeMissionStage.PressureFailure));
            Assert.That(mission.ObjectivePrompt, Is.EqualTo("Objective changed: escape the patrol pressure"));

            Object.DestroyImmediate(missionObject);
            Object.DestroyImmediate(worldObject);
        }

        [Test]
        public void Phase2RPlayablePressureChoiceOnlyCracksDownWhenPressureIsUncontained()
        {
            var worldObject = new GameObject("Phase2R Pressure Choice World State Test");
            var choiceObject = new GameObject("Phase2R Pressure Choice Test");
            var world = worldObject.AddComponent<PrototypeWorldState>();
            var choice = choiceObject.AddComponent<PrototypePressureChoiceController>();

            choice.AttachWorldState(world);
            world.ApplyEvent(PrototypeWorldEvent.PublicViolenceCommitted);

            Assert.That(choice.ResolvePressureEntry(), Is.True);
            Assert.That(choice.LastResolution, Is.EqualTo(PrototypePressureChoiceResolution.Crackdown));
            Assert.That(world.LastEvent, Is.EqualTo(PrototypeWorldEvent.PressureCrackdownTriggered));
            Assert.That(world.StatePressure, Is.EqualTo(PressureLevel.High));

            world.ResetState();
            world.ApplyEvent(PrototypeWorldEvent.PublicViolenceCommitted);
            world.ApplyEvent(PrototypeWorldEvent.BribeAccepted);

            Assert.That(choice.ResolvePressureEntry(), Is.False);
            Assert.That(choice.LastResolution, Is.EqualTo(PrototypePressureChoiceResolution.Contained));
            Assert.That(world.LastEvent, Is.EqualTo(PrototypeWorldEvent.BribeAccepted));
            Assert.That(world.StatePressure, Is.EqualTo(PressureLevel.Low));

            Object.DestroyImmediate(choiceObject);
            Object.DestroyImmediate(worldObject);
        }

        [Test]
        public void Phase5MissionEventsRejectOutOfOrderTransitions()
        {
            var worldObject = new GameObject("Mission Transition World State Test");
            var world = worldObject.AddComponent<PrototypeWorldState>();

            world.ResetState();

            Assert.That(world.ApplyEvent(PrototypeWorldEvent.FrontTakenUnderWatch), Is.False);
            Assert.That(world.FrontControl, Is.EqualTo(FrontControl.Rival));
            Assert.That(world.DirtyCash, Is.EqualTo(DirtyCashState.None));
            Assert.That(world.LastEvent, Is.EqualTo(PrototypeWorldEvent.None));

            Assert.That(world.ApplyEvent(PrototypeWorldEvent.DirtyCashSeized), Is.False);
            Assert.That(world.DirtyCash, Is.EqualTo(DirtyCashState.None));
            Assert.That(world.LastEvent, Is.EqualTo(PrototypeWorldEvent.None));

            Assert.That(world.ApplyEvent(PrototypeWorldEvent.DirtyCashPickedUp), Is.True);
            Assert.That(world.ApplyEvent(PrototypeWorldEvent.FrontTakenUnderWatch), Is.True);
            Assert.That(world.FrontControl, Is.EqualTo(FrontControl.PabloWatched));
            Assert.That(world.DirtyCash, Is.EqualTo(DirtyCashState.Hidden));

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
        public void CameraRigPlanarDirectionsFollowYaw()
        {
            var cameraObject = new GameObject("Camera Direction Test");
            var cameraRig = cameraObject.AddComponent<PrototypeCameraRig>();

            cameraRig.SetYawForTests(90f);

            AssertVectorApproximately(cameraRig.PlanarForward, Vector3.right, 0.001f);
            AssertVectorApproximately(cameraRig.PlanarRight, Vector3.back, 0.001f);
            Assert.That(cameraRig.Yaw, Is.EqualTo(90f).Within(0.001f));

            Object.DestroyImmediate(cameraObject);
        }

        [Test]
        public void GamepadLookYawIsFrameRateIndependent()
        {
            var thirtyFpsYaw = 0f;
            for (var i = 0; i < 30; i++)
            {
                thirtyFpsYaw += PrototypeCameraRig.CalculateYawDelta(Vector2.zero, Vector2.right, 0.12f, 150f, 1f / 30f);
            }

            var oneTwentyFpsYaw = 0f;
            for (var i = 0; i < 120; i++)
            {
                oneTwentyFpsYaw += PrototypeCameraRig.CalculateYawDelta(Vector2.zero, Vector2.right, 0.12f, 150f, 1f / 120f);
            }

            Assert.That(oneTwentyFpsYaw, Is.EqualTo(thirtyFpsYaw).Within(0.001f));
            Assert.That(thirtyFpsYaw, Is.EqualTo(150f).Within(0.001f));
        }

        [Test]
        public void MouseLookStaysRawWhileGamepadPitchScalesWithTime()
        {
            var mouseAtThirtyFps = PrototypeCameraRig.CalculateYawDelta(Vector2.right * 10f, Vector2.zero, 0.12f, 150f, 1f / 30f);
            var mouseAtOneTwentyFps = PrototypeCameraRig.CalculateYawDelta(Vector2.right * 10f, Vector2.zero, 0.12f, 150f, 1f / 120f);
            Assert.That(mouseAtOneTwentyFps, Is.EqualTo(mouseAtThirtyFps).Within(0.001f));

            var thirtyFpsPitch = 0f;
            for (var i = 0; i < 30; i++)
            {
                thirtyFpsPitch += PrototypeCameraRig.CalculatePitchDelta(Vector2.zero, Vector2.up, 0.12f, 120f, 1f / 30f);
            }

            var oneTwentyFpsPitch = 0f;
            for (var i = 0; i < 120; i++)
            {
                oneTwentyFpsPitch += PrototypeCameraRig.CalculatePitchDelta(Vector2.zero, Vector2.up, 0.12f, 120f, 1f / 120f);
            }

            Assert.That(oneTwentyFpsPitch, Is.EqualTo(thirtyFpsPitch).Within(0.001f));
            Assert.That(thirtyFpsPitch, Is.EqualTo(120f).Within(0.001f));
        }

        [Test]
        public void CameraRecenterWaitsForDelayThenMovesTowardPivotYaw()
        {
            var onFoot = PrototypeCameraRig.ResolveProfile(PrototypeCameraMode.OnFootFree);

            var beforeDelay = PrototypeCameraRig.CalculateRecenterYaw(90f, 0f, onFoot.RecenterDelay - 0.01f, onFoot, 0.2f);
            var afterDelay = PrototypeCameraRig.CalculateRecenterYaw(90f, 0f, onFoot.RecenterDelay + 0.01f, onFoot, 0.1f);

            Assert.That(beforeDelay, Is.EqualTo(90f).Within(0.001f));
            Assert.That(Mathf.Abs(Mathf.DeltaAngle(afterDelay, 0f)), Is.LessThan(Mathf.Abs(Mathf.DeltaAngle(90f, 0f))));
            Assert.That(Mathf.Abs(Mathf.DeltaAngle(90f, afterDelay)), Is.LessThanOrEqualTo(onFoot.RecenterSpeed * 0.1f + 0.001f));
        }

        [Test]
        public void CameraTightSpaceRecoveryHoldsBrieflyAfterCollisionClears()
        {
            Assert.That(
                PrototypeCameraRig.ResolveModeWithTightSpace(PrototypeCameraMode.OnFootFree, 0.2f, 0f),
                Is.EqualTo(PrototypeCameraMode.TightSpaceRecovery));
            Assert.That(
                PrototypeCameraRig.ResolveModeWithTightSpace(PrototypeCameraMode.OnFootFree, 0f, 0.2f),
                Is.EqualTo(PrototypeCameraMode.TightSpaceRecovery));
            Assert.That(
                PrototypeCameraRig.ResolveModeWithTightSpace(PrototypeCameraMode.OnFootFree, 0f, 0f),
                Is.EqualTo(PrototypeCameraMode.OnFootFree));
        }

        [Test]
        public void PlayerMovementUsesCameraPlanarAxes()
        {
            var desiredMove = PrototypePlayerController.BuildCameraRelativeMove(
                Vector2.up,
                Vector3.right,
                Vector3.back);

            AssertVectorApproximately(desiredMove, Vector3.right, 0.001f);
        }

        [Test]
        public void PlayerSideMovementDoesNotSpiralWithBodyRotation()
        {
            var cameraForward = Vector3.forward;
            var cameraRight = Vector3.right;
            var firstMove = PrototypePlayerController.BuildCameraRelativeMove(Vector2.left, cameraForward, cameraRight);
            var rotatedBodyWouldHaveChangedLocalRight = Quaternion.Euler(0f, -90f, 0f) * Vector3.right;
            var secondMove = PrototypePlayerController.BuildCameraRelativeMove(Vector2.left, cameraForward, cameraRight);

            AssertVectorApproximately(firstMove, Vector3.left, 0.001f);
            AssertVectorApproximately(secondMove, Vector3.left, 0.001f);
            Assert.That(rotatedBodyWouldHaveChangedLocalRight, Is.Not.EqualTo(cameraRight));
        }

        [Test]
        public void VehicleExitUsesFallbackAndBlocksWhenBothSidesAreOccupied()
        {
            var vehicleObject = new GameObject("Vehicle Exit Safety Test");
            vehicleObject.AddComponent<BoxCollider>();
            vehicleObject.AddComponent<Rigidbody>();
            var vehicle = vehicleObject.AddComponent<PrototypeVehicleController>();
            vehicleObject.transform.position = Vector3.zero;
            vehicleObject.transform.rotation = Quaternion.identity;

            var leftExit = new GameObject("Left Exit").transform;
            leftExit.SetParent(vehicleObject.transform);
            leftExit.localPosition = new Vector3(-1.8f, 0.2f, 0f);

            var rightExit = new GameObject("Right Exit").transform;
            rightExit.SetParent(vehicleObject.transform);
            rightExit.localPosition = new Vector3(1.8f, 0.2f, 0f);

            vehicle.SetExitPointsForTests(leftExit, rightExit);

            var leftBlocker = GameObject.CreatePrimitive(PrimitiveType.Cube);
            leftBlocker.name = "Left Exit Blocker";
            leftBlocker.transform.position = leftExit.position;
            leftBlocker.transform.localScale = Vector3.one;

            Assert.That(vehicle.TryResolveExitPose(out var fallbackPosition, out _), Is.True);
            AssertVectorApproximately(fallbackPosition, rightExit.position, 0.001f);

            var rightBlocker = GameObject.CreatePrimitive(PrimitiveType.Cube);
            rightBlocker.name = "Right Exit Blocker";
            rightBlocker.transform.position = rightExit.position;
            rightBlocker.transform.localScale = Vector3.one;

            Assert.That(vehicle.TryResolveExitPose(out _, out _), Is.False);

            Object.DestroyImmediate(rightBlocker);
            Object.DestroyImmediate(leftBlocker);
            Object.DestroyImmediate(vehicleObject);
        }

        [Test]
        public void VehicleDriveIntentBrakesBeforeReverse()
        {
            var brakeIntent = PrototypeVehicleController.ResolveDriveIntent(-1f, 4f, 0.35f);
            Assert.That(brakeIntent.Throttle, Is.EqualTo(0f));
            Assert.That(brakeIntent.Brake, Is.GreaterThan(0f));
            Assert.That(brakeIntent.Reverse, Is.EqualTo(0f));

            var reverseIntent = PrototypeVehicleController.ResolveDriveIntent(-1f, 0.1f, 0.35f);
            Assert.That(reverseIntent.Throttle, Is.EqualTo(0f));
            Assert.That(reverseIntent.Brake, Is.EqualTo(0f));
            Assert.That(reverseIntent.Reverse, Is.GreaterThan(0f));
        }

        [Test]
        public void FoundationLayersAreConfiguredAndExposeRuntimeMasks()
        {
            Assert.That(PrototypeLayers.AreConfigured(out var missing), Is.True, missing);
            Assert.That(PrototypeLayers.CameraCollisionMask, Is.EqualTo(PrototypeLayers.WorldCollisionMask));
            Assert.That((PrototypeLayers.InteractionQueryMask & (1 << PrototypeLayers.Interactable)) != 0, Is.True);
            Assert.That((PrototypeLayers.InteractionQueryMask & (1 << PrototypeLayers.Vehicle)) != 0, Is.True);
            Assert.That((PrototypeLayers.ExitBlockMask & (1 << PrototypeLayers.RouteTrigger)) == 0, Is.True);
            Assert.That((PrototypeLayers.ExitBlockMask & (1 << PrototypeLayers.SensorTrigger)) == 0, Is.True);
        }

        [Test]
        public void CameraProfilesCoverFoundationLockModes()
        {
            var onFoot = PrototypeCameraRig.ResolveProfile(PrototypeCameraMode.OnFootFree);
            var interaction = PrototypeCameraRig.ResolveProfile(PrototypeCameraMode.OnFootInteractionFocus);
            var driving = PrototypeCameraRig.ResolveProfile(PrototypeCameraMode.DrivingChase);
            var tightSpace = PrototypeCameraRig.ResolveProfile(PrototypeCameraMode.TightSpaceRecovery);

            Assert.That(driving.Distance, Is.GreaterThan(onFoot.Distance));
            Assert.That(interaction.ShoulderBias, Is.GreaterThan(onFoot.ShoulderBias));
            Assert.That(tightSpace.CollisionRestoreSpeed, Is.GreaterThan(onFoot.CollisionRestoreSpeed));
            Assert.That(driving.RecenterDelay, Is.LessThan(onFoot.RecenterDelay));
        }

        [Test]
        public void CharacterMotorVelocityAndSlopeRulesAreDeterministic()
        {
            var current = Vector3.zero;
            var target = PrototypeCharacterMotor.CalculateTargetHorizontalVelocity(
                Vector3.forward,
                current,
                4.2f,
                18f,
                22f,
                0.5f);

            AssertVectorApproximately(target, Vector3.forward * 4.2f, 0.001f);
            Assert.That(PrototypeCharacterMotor.IsSlopeWalkable(Vector3.up, 50f), Is.True);
            Assert.That(PrototypeCharacterMotor.IsSlopeWalkable(Quaternion.Euler(65f, 0f, 0f) * Vector3.up, 50f), Is.False);
        }

        [Test]
        public void CharacterMotorExposesRealFeelTuningDefaults()
        {
            var playerObject = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            var motor = playerObject.AddComponent<PrototypeCharacterMotor>();
            var serialized = new SerializedObject(motor);

            var stepHeight = serialized.FindProperty("stepHeight");
            var groundSnapDistance = serialized.FindProperty("groundSnapDistance");
            var slopeLimit = serialized.FindProperty("slopeLimit");

            Assert.That(stepHeight, Is.Not.Null, "Motor needs an explicit step height before it can be tuned against authored street geometry.");
            Assert.That(stepHeight.floatValue, Is.EqualTo(0.38f).Within(0.001f));
            Assert.That(groundSnapDistance, Is.Not.Null, "Motor needs ground snap as a first-class tuning parameter.");
            Assert.That(groundSnapDistance.floatValue, Is.EqualTo(0.28f).Within(0.001f));
            Assert.That(slopeLimit.floatValue, Is.EqualTo(50f).Within(0.001f));

            Object.DestroyImmediate(playerObject);
        }

        [Test]
        public void InteractionTargetingPrefersVisibleHigherPriorityCandidate()
        {
            var origin = Vector3.zero;
            var visibleVehicle = new PrototypeInteractionCandidate(
                new GameObject("Visible Vehicle Candidate").transform,
                PrototypeInteractionKind.Vehicle,
                "enter",
                10,
                false);
            var blockedInteractable = new PrototypeInteractionCandidate(
                new GameObject("Blocked Interactable Candidate").transform,
                PrototypeInteractionKind.Interactable,
                "use",
                20,
                true);
            visibleVehicle.Transform.position = new Vector3(2f, 0f, 0f);
            blockedInteractable.Transform.position = new Vector3(1f, 0f, 0f);

            var selected = PrototypeInteractionTargeting.SelectBest(
                origin,
                new[] { blockedInteractable, visibleVehicle },
                out var target);

            Assert.That(selected, Is.True);
            Assert.That(target.Kind, Is.EqualTo(PrototypeInteractionKind.Vehicle));
            Assert.That(target.Blocked, Is.False);

            Object.DestroyImmediate(visibleVehicle.Transform.gameObject);
            Object.DestroyImmediate(blockedInteractable.Transform.gameObject);
        }

        [Test]
        public void SliceDefinitionProvidesPhase1RouteData()
        {
            var definition = ScriptableObject.CreateInstance<PrototypeSliceDefinition>();
            definition.ConfigurePhase1Defaults();

            Assert.That(definition.Validate(out var error), Is.True, error);
            Assert.That(definition.RouteCheckpoints.Length, Is.EqualTo(5));
            Assert.That(definition.RouteCheckpoints[0].Label, Is.EqualTo("Start on foot"));
            Assert.That(definition.RouteCheckpoints[^1].Label, Is.EqualTo("Safe return"));

            Object.DestroyImmediate(definition);
        }

        [Test]
        public void WheelVehicleSpikeUsesSameDriveIntentAsArcadeBaseline()
        {
            var baseline = PrototypeVehicleController.ResolveDriveIntent(-1f, 3f, 0.35f);
            var spike = PrototypeWheelVehicleController.ResolveDriveIntent(-1f, 3f, 0.35f);

            Assert.That(spike.Throttle, Is.EqualTo(baseline.Throttle));
            Assert.That(spike.Brake, Is.EqualTo(baseline.Brake));
            Assert.That(spike.Reverse, Is.EqualTo(baseline.Reverse));
        }

        [Test]
        public void VehicleComparisonDecisionKeepsArcadeWhenWheelSpikeIsNotViable()
        {
            var arcade = new PrototypeVehicleProbeMetrics(
                PrototypeVehicleCandidateKind.ArcadeRigidbodyBaseline,
                28f,
                12f,
                4f,
                3f,
                42f,
                55f,
                2f,
                true,
                true);
            var wheel = new PrototypeVehicleProbeMetrics(
                PrototypeVehicleCandidateKind.WheelColliderSpike,
                2f,
                1f,
                0f,
                0f,
                5f,
                0f,
                0f,
                true,
                false);

            var decision = PrototypeVehicleComparison.Decide(arcade, wheel);

            Assert.That(decision, Is.EqualTo(PrototypeVehicleDecision.KeepArcadeRigidbodyBaseline));
            Assert.That(PrototypeVehicleComparison.BuildReport(arcade, wheel, decision), Does.Contain("Decision: KeepArcadeRigidbodyBaseline"));
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

        private static void AssertVectorApproximately(Vector3 actual, Vector3 expected, float tolerance)
        {
            Assert.That(Vector3.Distance(actual, expected), Is.LessThanOrEqualTo(tolerance), $"Expected {expected}, got {actual}.");
        }
    }
}
