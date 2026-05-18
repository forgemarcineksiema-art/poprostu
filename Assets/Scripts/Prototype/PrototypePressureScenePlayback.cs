using UnityEngine;

namespace ValleDePlata.Prototype
{
    public enum PrototypePressurePlaybackState
    {
        Quiet,
        PressureRising,
        Contained,
        Crackdown
    }

    public sealed class PrototypePressureScenePlayback : MonoBehaviour
    {
        [SerializeField] private PrototypeWorldState worldState;
        [SerializeField] private Transform patrolMarker;
        [SerializeField] private Transform roadblockMarker;
        [SerializeField] private Vector3 pressurePatrolPosition = new(0.6f, 0.55f, 29f);
        [SerializeField] private Vector3 crackdownPatrolPosition = new(0f, 0.55f, 34f);
        [SerializeField] private Vector3 roadblockOpenOffset = new(5.2f, 0f, 0f);

        private Vector3 patrolIdlePosition;
        private Vector3 roadblockClosedPosition;
        private Collider roadblockCollider;

        public PrototypePressurePlaybackState State { get; private set; } = PrototypePressurePlaybackState.Quiet;

        public void ConfigureForTests(
            PrototypeWorldState state,
            Transform patrol,
            Transform roadblock,
            Vector3 pressurePosition,
            Vector3 crackdownPosition,
            Vector3 openOffset)
        {
            worldState = state;
            patrolMarker = patrol;
            roadblockMarker = roadblock;
            pressurePatrolPosition = pressurePosition;
            crackdownPatrolPosition = crackdownPosition;
            roadblockOpenOffset = openOffset;
            CaptureInitialPose();
            Connect(worldState);
        }

        private void Awake()
        {
            CaptureInitialPose();
        }

        private void Start()
        {
            Connect(worldState != null ? worldState : PrototypeWorldState.Active);
        }

        private void OnDisable()
        {
            if (worldState != null)
            {
                worldState.Changed -= OnWorldStateChanged;
            }
        }

        private void CaptureInitialPose()
        {
            if (patrolMarker == null)
            {
                patrolMarker = transform;
            }

            if (patrolMarker != null)
            {
                patrolIdlePosition = patrolMarker.position;
            }

            if (roadblockMarker != null)
            {
                roadblockClosedPosition = roadblockMarker.position;
                roadblockCollider = roadblockMarker.GetComponent<Collider>();
            }
        }

        private void Connect(PrototypeWorldState state)
        {
            if (state == null || worldState == state)
            {
                if (state != null)
                {
                    ApplyFromState(state);
                }

                return;
            }

            if (worldState != null)
            {
                worldState.Changed -= OnWorldStateChanged;
            }

            worldState = state;
            worldState.Changed += OnWorldStateChanged;
            ApplyFromState(worldState);
        }

        private void OnWorldStateChanged(PrototypeWorldState state)
        {
            ApplyFromState(state);
        }

        private void ApplyFromState(PrototypeWorldState state)
        {
            if (state == null)
            {
                return;
            }

            switch (state.LastEvent)
            {
                case PrototypeWorldEvent.PublicViolenceCommitted:
                    ApplyPressureRising();
                    break;
                case PrototypeWorldEvent.BribeAccepted:
                    ApplyContained();
                    break;
                case PrototypeWorldEvent.PressureCrackdownTriggered:
                    ApplyCrackdown();
                    break;
                case PrototypeWorldEvent.None:
                    ApplyQuiet();
                    break;
            }
        }

        private void ApplyQuiet()
        {
            State = PrototypePressurePlaybackState.Quiet;
            if (patrolMarker != null)
            {
                patrolMarker.position = patrolIdlePosition;
            }

            CloseRoadblock();
        }

        private void ApplyPressureRising()
        {
            State = PrototypePressurePlaybackState.PressureRising;
            if (patrolMarker != null)
            {
                patrolMarker.position = pressurePatrolPosition;
            }

            CloseRoadblock();
        }

        private void ApplyContained()
        {
            State = PrototypePressurePlaybackState.Contained;
            if (roadblockMarker != null)
            {
                roadblockMarker.position = roadblockClosedPosition + roadblockOpenOffset;
            }

            if (roadblockCollider != null)
            {
                roadblockCollider.enabled = false;
            }
        }

        private void ApplyCrackdown()
        {
            State = PrototypePressurePlaybackState.Crackdown;
            if (patrolMarker != null)
            {
                patrolMarker.position = crackdownPatrolPosition;
            }

            CloseRoadblock();
        }

        private void CloseRoadblock()
        {
            if (roadblockMarker != null)
            {
                roadblockMarker.position = roadblockClosedPosition;
            }

            if (roadblockCollider != null)
            {
                roadblockCollider.enabled = true;
            }
        }
    }
}
