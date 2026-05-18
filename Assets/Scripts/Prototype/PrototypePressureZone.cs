using UnityEngine;

namespace ValleDePlata.Prototype
{
    public sealed class PrototypePressureZone : MonoBehaviour
    {
        [SerializeField] private string pressureLabel = "Patrol watching the route";
        [SerializeField] private PrototypePressureChoiceController choiceController;

        private void OnTriggerEnter(Collider other)
        {
            if (other.GetComponentInParent<PrototypePlayerController>() != null
                || other.GetComponentInParent<PrototypeVehicleController>() != null)
            {
                PrototypeDebugState.Pressure = pressureLabel;
                PrototypeRunMetrics.Active?.RecordPressureEnter();
                ResolveChoice(other);
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

        private void ResolveChoice(Collider entrant)
        {
            if (choiceController == null)
            {
                choiceController = GetComponent<PrototypePressureChoiceController>();
            }

            choiceController?.ResolvePressureEntry(entrant);
        }
    }
}
