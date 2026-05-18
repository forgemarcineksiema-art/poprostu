using UnityEngine;

namespace ValleDePlata.Prototype
{
    public enum PrototypeRouteOutcome
    {
        InProgress,
        Complete,
        PressureContained,
        PressureBlocked,
        PressureFailureEscape
    }

    public sealed class PrototypeRouteProgress : MonoBehaviour
    {
        [SerializeField] private int checkpointCount;
        [SerializeField] private PrototypeWorldState worldState;

        private int nextCheckpointIndex;
        private bool pressureEscapeRegistered;

        public int NextCheckpointIndex => nextCheckpointIndex;
        public bool IsComplete => checkpointCount > 0 && nextCheckpointIndex >= checkpointCount;
        public PrototypeRouteOutcome Outcome { get; private set; } = PrototypeRouteOutcome.InProgress;

        public void AttachWorldState(PrototypeWorldState state)
        {
            worldState = state;
        }

        public void RegisterCheckpoint(int checkpointIndex, string label)
        {
            if (IsPressureFailureActive())
            {
                RegisterPressureFailureCheckpoint(label);
                return;
            }

            if (checkpointIndex != nextCheckpointIndex)
            {
                return;
            }

            nextCheckpointIndex++;
            PrototypeDebugState.LastCheckpoint = label;
            Outcome = ResolveNormalOutcome();
            PrototypeDebugState.Route = IsComplete
                ? "Complete"
                : $"{nextCheckpointIndex}/{checkpointCount}";
            PrototypeRunMetrics.Active?.RecordCheckpoint(label, IsComplete);
            PrototypeRunMetrics.Active?.RecordRouteOutcome(Outcome);
        }

        public void Configure(int totalCheckpoints)
        {
            checkpointCount = Mathf.Max(0, totalCheckpoints);
            nextCheckpointIndex = 0;
            Outcome = PrototypeRouteOutcome.InProgress;
            pressureEscapeRegistered = false;
            PrototypeDebugState.Route = checkpointCount > 0 ? $"0/{checkpointCount}" : "No route";
            PrototypeDebugState.LastCheckpoint = "None";
        }

        private void Awake()
        {
            if (checkpointCount > 0)
            {
                PrototypeDebugState.Route = $"0/{checkpointCount}";
            }
        }

        private PrototypeWorldState ResolveWorldState()
        {
            if (worldState == null)
            {
                worldState = PrototypeWorldState.Active;
            }

            return worldState;
        }

        private bool IsPressureFailureActive()
        {
            return ResolveWorldState()?.LastEvent == PrototypeWorldEvent.PressureCrackdownTriggered;
        }

        private void RegisterPressureFailureCheckpoint(string label)
        {
            if (IsSafeReturn(label))
            {
                RegisterPressureEscape(label);
                return;
            }

            Outcome = PrototypeRouteOutcome.PressureBlocked;
            PrototypeDebugState.Route = "Pressure blocked: return through Safe return";
            PrototypeRunMetrics.Active?.RecordRouteOutcome(Outcome);
        }

        private void RegisterPressureEscape(string label)
        {
            if (pressureEscapeRegistered)
            {
                return;
            }

            pressureEscapeRegistered = true;
            Outcome = PrototypeRouteOutcome.PressureFailureEscape;
            PrototypeDebugState.LastCheckpoint = string.IsNullOrWhiteSpace(label) ? "Safe return" : label;
            PrototypeDebugState.Route = "Pressure escape: Safe return reached";
            PrototypeRunMetrics.Active?.RecordCheckpoint(PrototypeDebugState.LastCheckpoint, false);
            PrototypeRunMetrics.Active?.RecordRouteOutcome(Outcome);
        }

        private PrototypeRouteOutcome ResolveNormalOutcome()
        {
            if (!IsComplete)
            {
                return PrototypeRouteOutcome.InProgress;
            }

            return ResolveWorldState()?.LastEvent == PrototypeWorldEvent.BribeAccepted
                ? PrototypeRouteOutcome.PressureContained
                : PrototypeRouteOutcome.Complete;
        }

        private static bool IsSafeReturn(string label)
        {
            return !string.IsNullOrWhiteSpace(label)
                && label.Contains("Safe return", System.StringComparison.OrdinalIgnoreCase);
        }
    }
}
