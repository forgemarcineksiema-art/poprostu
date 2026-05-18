using UnityEngine;

namespace ValleDePlata.Prototype
{
    public enum PrototypeMissionStage
    {
        FindingFront,
        CarryingRisk,
        FrontSecured,
        PartialFailure
    }

    public sealed class PrototypeMissionSpine : MonoBehaviour
    {
        [SerializeField] private PrototypeWorldState worldState;

        public PrototypeMissionStage Stage { get; private set; } = PrototypeMissionStage.FindingFront;

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
            PrototypeDebugState.Mission = Stage switch
            {
                PrototypeMissionStage.CarryingRisk => "Mission: dirty cash is exposed, secure El Respiro",
                PrototypeMissionStage.FrontSecured => "Mission: El Respiro secured under watch",
                PrototypeMissionStage.PartialFailure => "Mission: partial failure, dirty cash seized",
                _ => "Mission: find leverage at El Respiro"
            };
        }
    }
}
