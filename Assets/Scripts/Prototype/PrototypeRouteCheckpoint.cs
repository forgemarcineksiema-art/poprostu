using UnityEngine;

namespace ValleDePlata.Prototype
{
    public sealed class PrototypeRouteCheckpoint : MonoBehaviour
    {
        [SerializeField] private PrototypeRouteProgress routeProgress;
        [SerializeField] private int checkpointIndex;
        [SerializeField] private string label = "Checkpoint";

        public int CheckpointIndex => checkpointIndex;
        public string Label => label;

        public void Configure(PrototypeRouteProgress progress, int index, string checkpointLabel)
        {
            routeProgress = progress;
            checkpointIndex = index;
            label = checkpointLabel;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (routeProgress == null)
            {
                return;
            }

            if (other.GetComponentInParent<PrototypePlayerController>() != null
                || other.GetComponentInParent<PrototypeVehicleController>() != null)
            {
                routeProgress.RegisterCheckpoint(checkpointIndex, label);
            }
        }
    }
}
