using UnityEngine;

namespace ValleDePlata.Prototype
{
    public sealed class PrototypeRouteProgress : MonoBehaviour
    {
        [SerializeField] private int checkpointCount;

        private int nextCheckpointIndex;

        public int NextCheckpointIndex => nextCheckpointIndex;
        public bool IsComplete => checkpointCount > 0 && nextCheckpointIndex >= checkpointCount;

        public void RegisterCheckpoint(int checkpointIndex, string label)
        {
            if (checkpointIndex != nextCheckpointIndex)
            {
                return;
            }

            nextCheckpointIndex++;
            PrototypeDebugState.LastCheckpoint = label;
            PrototypeDebugState.Route = IsComplete
                ? "Complete"
                : $"{nextCheckpointIndex}/{checkpointCount}";
        }

        public void Configure(int totalCheckpoints)
        {
            checkpointCount = Mathf.Max(0, totalCheckpoints);
            nextCheckpointIndex = 0;
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
    }
}
