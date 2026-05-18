using System.Text;
using UnityEngine;

namespace ValleDePlata.Prototype
{
    public enum PrototypeVehicleCandidateKind
    {
        ArcadeRigidbodyBaseline,
        WheelColliderSpike
    }

    public enum PrototypeVehicleDecision
    {
        KeepArcadeRigidbodyBaseline,
        PromoteWheelColliderSpike,
        DeferForRaycastVehicleSpike
    }

    public readonly struct PrototypeVehicleProbeMetrics
    {
        public PrototypeVehicleProbeMetrics(
            PrototypeVehicleCandidateKind candidate,
            float distanceMeters,
            float maxSpeedMetersPerSecond,
            float brakeSpeedDrop,
            float reverseDistanceMeters,
            float yawDegrees,
            float handbrakeYawDegrees,
            float collisionRecoveryMeters,
            bool stayedUpright,
            bool completedProbe)
        {
            Candidate = candidate;
            DistanceMeters = distanceMeters;
            MaxSpeedMetersPerSecond = maxSpeedMetersPerSecond;
            BrakeSpeedDrop = brakeSpeedDrop;
            ReverseDistanceMeters = reverseDistanceMeters;
            YawDegrees = yawDegrees;
            HandbrakeYawDegrees = handbrakeYawDegrees;
            CollisionRecoveryMeters = collisionRecoveryMeters;
            StayedUpright = stayedUpright;
            CompletedProbe = completedProbe;
        }

        public PrototypeVehicleCandidateKind Candidate { get; }
        public float DistanceMeters { get; }
        public float MaxSpeedMetersPerSecond { get; }
        public float BrakeSpeedDrop { get; }
        public float ReverseDistanceMeters { get; }
        public float YawDegrees { get; }
        public float HandbrakeYawDegrees { get; }
        public float CollisionRecoveryMeters { get; }
        public bool StayedUpright { get; }
        public bool CompletedProbe { get; }
        public bool IsViable => CompletedProbe && StayedUpright && DistanceMeters >= 4f && MaxSpeedMetersPerSecond >= 1f;

        public float Score =>
            (IsViable ? 25f : 0f)
            + Mathf.Clamp(DistanceMeters, 0f, 40f)
            + Mathf.Clamp(MaxSpeedMetersPerSecond * 2f, 0f, 30f)
            + Mathf.Clamp(BrakeSpeedDrop * 2f, 0f, 20f)
            + Mathf.Clamp(ReverseDistanceMeters * 2f, 0f, 12f)
            + Mathf.Clamp(YawDegrees * 0.2f, 0f, 15f)
            + Mathf.Clamp(HandbrakeYawDegrees * 0.2f, 0f, 15f)
            + Mathf.Clamp(CollisionRecoveryMeters * 2f, 0f, 10f);
    }

    public static class PrototypeVehicleComparison
    {
        public static PrototypeVehicleProbeMetrics RunArcadeProbe(PrototypeVehicleController vehicle, float fixedDeltaTime)
        {
            if (vehicle == null)
            {
                return Empty(PrototypeVehicleCandidateKind.ArcadeRigidbodyBaseline);
            }

            return RunProbe(
                PrototypeVehicleCandidateKind.ArcadeRigidbodyBaseline,
                vehicle.transform,
                vehicle.GetComponent<Rigidbody>(),
                fixedDeltaTime,
                (move, handbrake) => vehicle.ApplyDriveInput(move, handbrake));
        }

        public static PrototypeVehicleProbeMetrics RunWheelProbe(PrototypeWheelVehicleController vehicle, float fixedDeltaTime)
        {
            if (vehicle == null)
            {
                return Empty(PrototypeVehicleCandidateKind.WheelColliderSpike);
            }

            return RunProbe(
                PrototypeVehicleCandidateKind.WheelColliderSpike,
                vehicle.transform,
                vehicle.GetComponent<Rigidbody>(),
                fixedDeltaTime,
                (move, _) => vehicle.ApplyDriveInput(move));
        }

        public static PrototypeVehicleDecision Decide(PrototypeVehicleProbeMetrics arcade, PrototypeVehicleProbeMetrics wheel)
        {
            if (!arcade.IsViable && !wheel.IsViable)
            {
                return PrototypeVehicleDecision.DeferForRaycastVehicleSpike;
            }

            if (!wheel.IsViable || arcade.Score >= wheel.Score * 0.9f)
            {
                return PrototypeVehicleDecision.KeepArcadeRigidbodyBaseline;
            }

            return wheel.Score > arcade.Score * 1.15f
                ? PrototypeVehicleDecision.PromoteWheelColliderSpike
                : PrototypeVehicleDecision.KeepArcadeRigidbodyBaseline;
        }

        public static string BuildReport(
            PrototypeVehicleProbeMetrics arcade,
            PrototypeVehicleProbeMetrics wheel,
            PrototypeVehicleDecision decision)
        {
            var builder = new StringBuilder();
            builder.AppendLine("Foundation Lock 1.7 Vehicle A/B");
            AppendMetrics(builder, arcade);
            AppendMetrics(builder, wheel);
            builder.AppendLine($"Decision: {decision}");
            builder.AppendLine("Runtime migration: none in this pass");
            return builder.ToString();
        }

        private static PrototypeVehicleProbeMetrics RunProbe(
            PrototypeVehicleCandidateKind candidate,
            Transform vehicleTransform,
            Rigidbody body,
            float fixedDeltaTime,
            System.Action<Vector2, bool> applyInput)
        {
            if (vehicleTransform == null || body == null || applyInput == null)
            {
                return Empty(candidate);
            }

            var previousAutoSimulation = Physics.simulationMode;
            Physics.simulationMode = SimulationMode.Script;
            var previousFixedDeltaTime = Time.fixedDeltaTime;
            Time.fixedDeltaTime = Mathf.Max(0.001f, fixedDeltaTime);

            try
            {
                body.linearVelocity = Vector3.zero;
                body.angularVelocity = Vector3.zero;
                Physics.SyncTransforms();

                var startPosition = vehicleTransform.position;
                var startRotation = vehicleTransform.rotation;
                var maxSpeed = 0f;
                var maxYaw = 0f;
                var maxHandbrakeYaw = 0f;

                SimulatePhase(80, Vector2.up, false, applyInput, body, vehicleTransform, startRotation, ref maxSpeed, ref maxYaw);

                var speedBeforeBrake = body.linearVelocity.magnitude;
                var minBrakeSpeed = speedBeforeBrake;
                for (var i = 0; i < 35; i++)
                {
                    applyInput(Vector2.down, false);
                    Physics.Simulate(Time.fixedDeltaTime);
                    minBrakeSpeed = Mathf.Min(minBrakeSpeed, body.linearVelocity.magnitude);
                    maxSpeed = Mathf.Max(maxSpeed, body.linearVelocity.magnitude);
                }

                var reverseStart = vehicleTransform.position;
                SimulatePhase(45, Vector2.down, false, applyInput, body, vehicleTransform, startRotation, ref maxSpeed, ref maxYaw);
                var reverseDistance = Vector3.Project(reverseStart - vehicleTransform.position, startRotation * Vector3.forward).magnitude;

                SimulatePhase(60, new Vector2(0.85f, 1f), false, applyInput, body, vehicleTransform, startRotation, ref maxSpeed, ref maxYaw);

                var handbrakeStartRotation = vehicleTransform.rotation;
                SimulatePhase(45, new Vector2(0.9f, 1f), true, applyInput, body, vehicleTransform, handbrakeStartRotation, ref maxSpeed, ref maxHandbrakeYaw);

                var recoveryStart = vehicleTransform.position;
                SimulatePhase(40, Vector2.up, false, applyInput, body, vehicleTransform, startRotation, ref maxSpeed, ref maxYaw);

                var distance = Vector3.Distance(
                    new Vector3(startPosition.x, 0f, startPosition.z),
                    new Vector3(vehicleTransform.position.x, 0f, vehicleTransform.position.z));
                var collisionRecovery = Vector3.Distance(
                    new Vector3(recoveryStart.x, 0f, recoveryStart.z),
                    new Vector3(vehicleTransform.position.x, 0f, vehicleTransform.position.z));
                var stayedUpright = Vector3.Dot(vehicleTransform.up, Vector3.up) > 0.65f;

                return new PrototypeVehicleProbeMetrics(
                    candidate,
                    distance,
                    maxSpeed,
                    Mathf.Max(0f, speedBeforeBrake - minBrakeSpeed),
                    reverseDistance,
                    maxYaw,
                    maxHandbrakeYaw,
                    collisionRecovery,
                    stayedUpright,
                    true);
            }
            finally
            {
                Time.fixedDeltaTime = previousFixedDeltaTime;
                Physics.simulationMode = previousAutoSimulation;
                Physics.SyncTransforms();
            }
        }

        private static void SimulatePhase(
            int frames,
            Vector2 move,
            bool handbrake,
            System.Action<Vector2, bool> applyInput,
            Rigidbody body,
            Transform vehicleTransform,
            Quaternion referenceRotation,
            ref float maxSpeed,
            ref float maxYawDegrees)
        {
            for (var i = 0; i < frames; i++)
            {
                applyInput(move, handbrake);
                Physics.Simulate(Time.fixedDeltaTime);
                maxSpeed = Mathf.Max(maxSpeed, body.linearVelocity.magnitude);
                maxYawDegrees = Mathf.Max(maxYawDegrees, Quaternion.Angle(referenceRotation, vehicleTransform.rotation));
            }
        }

        private static PrototypeVehicleProbeMetrics Empty(PrototypeVehicleCandidateKind candidate)
        {
            return new PrototypeVehicleProbeMetrics(candidate, 0f, 0f, 0f, 0f, 0f, 0f, 0f, false, false);
        }

        private static void AppendMetrics(StringBuilder builder, PrototypeVehicleProbeMetrics metrics)
        {
            builder.AppendLine($"Candidate: {metrics.Candidate}");
            builder.AppendLine($"  Completed: {metrics.CompletedProbe}");
            builder.AppendLine($"  Viable: {metrics.IsViable}");
            builder.AppendLine($"  DistanceMeters: {metrics.DistanceMeters:0.00}");
            builder.AppendLine($"  MaxSpeedMps: {metrics.MaxSpeedMetersPerSecond:0.00}");
            builder.AppendLine($"  BrakeSpeedDrop: {metrics.BrakeSpeedDrop:0.00}");
            builder.AppendLine($"  ReverseDistanceMeters: {metrics.ReverseDistanceMeters:0.00}");
            builder.AppendLine($"  YawDegrees: {metrics.YawDegrees:0.00}");
            builder.AppendLine($"  HandbrakeYawDegrees: {metrics.HandbrakeYawDegrees:0.00}");
            builder.AppendLine($"  CollisionRecoveryMeters: {metrics.CollisionRecoveryMeters:0.00}");
            builder.AppendLine($"  StayedUpright: {metrics.StayedUpright}");
            builder.AppendLine($"  Score: {metrics.Score:0.00}");
        }
    }
}
