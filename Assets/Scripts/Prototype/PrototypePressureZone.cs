using UnityEngine;

namespace ValleDePlata.Prototype
{
    public sealed class PrototypePressureZone : MonoBehaviour
    {
        [SerializeField] private string pressureLabel = "Patrol watching the route";

        private void OnTriggerEnter(Collider other)
        {
            if (other.GetComponentInParent<PrototypePlayerController>() != null
                || other.GetComponentInParent<PrototypeVehicleController>() != null)
            {
                PrototypeDebugState.Pressure = pressureLabel;
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.GetComponentInParent<PrototypePlayerController>() != null
                || other.GetComponentInParent<PrototypeVehicleController>() != null)
            {
                PrototypeDebugState.Pressure = "Quiet";
            }
        }
    }
}
