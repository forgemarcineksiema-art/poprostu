using UnityEngine;

namespace ValleDePlata.Prototype
{
    public enum PrototypePressureChoiceResolution
    {
        None,
        Contained,
        Crackdown
    }

    public sealed class PrototypePressureChoiceController : MonoBehaviour
    {
        [SerializeField] private PrototypeWorldState worldState;
        [SerializeField] private string quietMessage = "Pressure choice inactive";
        [SerializeField] private string containedMessage = "Patrol lets Pablo through after the bribe";
        [SerializeField] private string crackdownMessage = "Patrol pressure locks onto Pablo";

        public PrototypePressureChoiceResolution LastResolution { get; private set; } = PrototypePressureChoiceResolution.None;

        public void AttachWorldState(PrototypeWorldState state)
        {
            worldState = state;
        }

        public bool ResolvePressureEntry()
        {
            if (worldState == null)
            {
                worldState = PrototypeWorldState.Active;
            }

            var resolution = ResolvePressureEntry(worldState);
            if (resolution == PrototypePressureChoiceResolution.None)
            {
                LastResolution = PrototypePressureChoiceResolution.None;
                PrototypeDebugState.Pressure = quietMessage;
                return false;
            }

            if (resolution == PrototypePressureChoiceResolution.Contained)
            {
                LastResolution = PrototypePressureChoiceResolution.Contained;
                PrototypeDebugState.Pressure = containedMessage;
                return false;
            }

            if (worldState.LastEvent == PrototypeWorldEvent.PressureCrackdownTriggered)
            {
                LastResolution = PrototypePressureChoiceResolution.Crackdown;
                PrototypeDebugState.Pressure = crackdownMessage;
                return false;
            }

            var applied = worldState.ApplyEvent(PrototypeWorldEvent.PressureCrackdownTriggered);
            LastResolution = applied
                ? PrototypePressureChoiceResolution.Crackdown
                : PrototypePressureChoiceResolution.None;
            PrototypeDebugState.Pressure = applied ? crackdownMessage : quietMessage;
            return applied;
        }

        public bool ResolvePressureEntry(Collider entrant)
        {
            if (!IsValidEntrant(entrant))
            {
                return false;
            }

            return ResolvePressureEntry();
        }

        public static PrototypePressureChoiceResolution ResolvePressureEntry(PrototypeWorldState state)
        {
            if (state == null)
            {
                return PrototypePressureChoiceResolution.None;
            }

            return state.LastEvent switch
            {
                PrototypeWorldEvent.BribeAccepted => PrototypePressureChoiceResolution.Contained,
                PrototypeWorldEvent.PressureCrackdownTriggered => PrototypePressureChoiceResolution.Crackdown,
                PrototypeWorldEvent.PublicViolenceCommitted when state.StatePressure != PressureLevel.Low => PrototypePressureChoiceResolution.Crackdown,
                _ => PrototypePressureChoiceResolution.None
            };
        }

        private void Start()
        {
            if (worldState == null)
            {
                worldState = PrototypeWorldState.Active;
            }
        }

        private static bool IsValidEntrant(Collider entrant)
        {
            return entrant != null
                && (entrant.GetComponentInParent<PrototypePlayerController>() != null
                    || entrant.GetComponentInParent<PrototypeVehicleController>() != null);
        }
    }
}
