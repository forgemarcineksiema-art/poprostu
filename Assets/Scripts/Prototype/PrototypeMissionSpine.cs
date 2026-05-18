using UnityEngine;

namespace ValleDePlata.Prototype
{
    public enum PrototypeMissionStage
    {
        FindingFront,
        ActionPressure,
        PressureContained,
        PressureFailure,
        CarryingRisk,
        FrontSecured,
        PartialFailure
    }

    public sealed class PrototypeMissionSpine : MonoBehaviour
    {
        [SerializeField] private PrototypeWorldState worldState;

        public PrototypeMissionStage Stage { get; private set; } = PrototypeMissionStage.FindingFront;
        public string ObjectivePrompt { get; private set; } = "Objective: collect dirty cash at El Respiro";
        public bool IsPhase5Resolved => Stage is PrototypeMissionStage.FrontSecured or PrototypeMissionStage.PartialFailure;

        public void AttachWorldState(PrototypeWorldState state)
        {
            if (worldState == state)
            {
                ApplyState(worldState);
                return;
            }

            if (worldState != null)
            {
                worldState.Changed -= OnWorldStateChanged;
            }

            worldState = state;
            if (worldState != null)
            {
                worldState.Changed += OnWorldStateChanged;
            }

            ApplyState(worldState);
        }

        private void Awake()
        {
            UpdateDebugState();
        }

        private void Start()
        {
            AttachWorldState(worldState != null ? worldState : PrototypeWorldState.Active);
        }

        private void OnDisable()
        {
            if (worldState != null)
            {
                worldState.Changed -= OnWorldStateChanged;
            }
        }

        private void OnWorldStateChanged(PrototypeWorldState state)
        {
            ApplyState(state);
        }

        private void ApplyState(PrototypeWorldState state)
        {
            if (state == null)
            {
                Stage = PrototypeMissionStage.FindingFront;
                UpdateDebugState();
                return;
            }

            if (state.DirtyCash == DirtyCashState.Seized)
            {
                Stage = PrototypeMissionStage.PartialFailure;
            }
            else if (state.LastEvent == PrototypeWorldEvent.PressureCrackdownTriggered)
            {
                Stage = PrototypeMissionStage.PressureFailure;
            }
            else if (state.LastEvent == PrototypeWorldEvent.BribeAccepted)
            {
                Stage = PrototypeMissionStage.PressureContained;
            }
            else if (state.LastEvent == PrototypeWorldEvent.PublicViolenceCommitted)
            {
                Stage = PrototypeMissionStage.ActionPressure;
            }
            else if (state.FrontControl == FrontControl.PabloWatched || state.FrontControl == FrontControl.Pablo)
            {
                Stage = PrototypeMissionStage.FrontSecured;
            }
            else if (state.DirtyCash == DirtyCashState.Carried)
            {
                Stage = PrototypeMissionStage.CarryingRisk;
            }
            else
            {
                Stage = PrototypeMissionStage.FindingFront;
            }

            UpdateDebugState();
        }

        private void UpdateDebugState()
        {
            ObjectivePrompt = Stage switch
            {
                PrototypeMissionStage.ActionPressure => "Objective: contain street pressure before patrol locks the route",
                PrototypeMissionStage.PressureContained => "Objective: pressure contained, continue to El Respiro",
                PrototypeMissionStage.PressureFailure => "Objective changed: escape the patrol pressure",
                PrototypeMissionStage.CarryingRisk => "Objective: secure El Respiro or risk losing the cash",
                PrototypeMissionStage.FrontSecured => "Objective complete: exit through Safe return",
                PrototypeMissionStage.PartialFailure => "Objective changed: leave through Safe return without the cash",
                _ => "Objective: collect dirty cash at El Respiro"
            };

            var stateLine = Stage switch
            {
                PrototypeMissionStage.ActionPressure => "Mission: street pressure rising after public violence",
                PrototypeMissionStage.PressureContained => "Mission: pressure contained for now",
                PrototypeMissionStage.PressureFailure => "Mission: patrol pressure has locked onto Pablo",
                PrototypeMissionStage.CarryingRisk => "Mission: dirty cash is exposed, secure El Respiro",
                PrototypeMissionStage.FrontSecured => "Mission: El Respiro secured under watch",
                PrototypeMissionStage.PartialFailure => "Mission: partial failure, dirty cash seized",
                _ => "Mission: find leverage at El Respiro"
            };

            PrototypeDebugState.Mission = IsPhase5Resolved
                ? $"{stateLine}\n{ObjectivePrompt}\nPhase 5 resolved"
                : $"{stateLine}\n{ObjectivePrompt}";
        }
    }
}
