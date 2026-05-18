using UnityEngine;

namespace ValleDePlata.Prototype
{
    public enum PrototypeCharacterLocomotionState
    {
        Hidden,
        Idle,
        Walk,
        Sprint
    }

    public sealed class PrototypeCharacterPresentation : MonoBehaviour
    {
        [SerializeField] private Transform visualRoot;
        [SerializeField] private float walkThreshold = 0.15f;
        [SerializeField] private float sprintSpeedRatio = 0.78f;
        [SerializeField] private float bobAmplitude = 0.025f;
        [SerializeField] private float walkBobFrequency = 7.5f;
        [SerializeField] private float sprintBobFrequency = 10.5f;
        [SerializeField] private float leanDegrees = 3.5f;

        private Vector3 baseLocalPosition;
        private Quaternion baseLocalRotation = Quaternion.identity;
        private float locomotionClock;

        public Transform VisualRoot => visualRoot;
        public PrototypeCharacterLocomotionState CurrentState { get; private set; } = PrototypeCharacterLocomotionState.Idle;

        private void Awake()
        {
            CacheBasePose();
            EnsureVisualDoesNotCollide();
        }

        public void AttachVisual(Transform visual)
        {
            visualRoot = visual;
            CacheBasePose();
            EnsureVisualDoesNotCollide();
        }

        public void SetVisible(bool visible)
        {
            if (visualRoot != null)
            {
                visualRoot.gameObject.SetActive(visible);
            }

            CurrentState = ResolveLocomotionState(visible, 0f, false, 0f, walkThreshold, sprintSpeedRatio);
        }

        public void ApplyLocomotion(float speed, bool sprintHeld, float sprintSpeed, float deltaTime)
        {
            var visible = visualRoot == null || visualRoot.gameObject.activeInHierarchy;
            CurrentState = ResolveLocomotionState(visible, speed, sprintHeld, sprintSpeed, walkThreshold, sprintSpeedRatio);

            if (visualRoot == null)
            {
                return;
            }

            if (CurrentState == PrototypeCharacterLocomotionState.Idle || CurrentState == PrototypeCharacterLocomotionState.Hidden)
            {
                locomotionClock = 0f;
                visualRoot.localPosition = Vector3.Lerp(visualRoot.localPosition, baseLocalPosition, Mathf.Clamp01(deltaTime * 10f));
                visualRoot.localRotation = Quaternion.Slerp(visualRoot.localRotation, baseLocalRotation, Mathf.Clamp01(deltaTime * 10f));
                return;
            }

            var frequency = CurrentState == PrototypeCharacterLocomotionState.Sprint ? sprintBobFrequency : walkBobFrequency;
            var lean = CurrentState == PrototypeCharacterLocomotionState.Sprint ? leanDegrees : leanDegrees * 0.55f;
            locomotionClock += deltaTime * frequency;

            var bob = Mathf.Sin(locomotionClock) * bobAmplitude;
            visualRoot.localPosition = baseLocalPosition + Vector3.up * bob;
            visualRoot.localRotation = baseLocalRotation * Quaternion.Euler(lean, 0f, 0f);
        }

        public static PrototypeCharacterLocomotionState ResolveLocomotionState(
            bool visible,
            float speed,
            bool sprintHeld,
            float sprintSpeed)
        {
            return ResolveLocomotionState(visible, speed, sprintHeld, sprintSpeed, 0.15f, 0.78f);
        }

        public static PrototypeCharacterLocomotionState ResolveLocomotionState(
            bool visible,
            float speed,
            bool sprintHeld,
            float sprintSpeed,
            float walkThreshold,
            float sprintSpeedRatio)
        {
            if (!visible)
            {
                return PrototypeCharacterLocomotionState.Hidden;
            }

            if (speed <= Mathf.Max(0f, walkThreshold))
            {
                return PrototypeCharacterLocomotionState.Idle;
            }

            var sprintThreshold = Mathf.Max(walkThreshold, sprintSpeed * sprintSpeedRatio);
            return sprintHeld && speed >= sprintThreshold
                ? PrototypeCharacterLocomotionState.Sprint
                : PrototypeCharacterLocomotionState.Walk;
        }

        private void CacheBasePose()
        {
            if (visualRoot == null)
            {
                return;
            }

            baseLocalPosition = visualRoot.localPosition;
            baseLocalRotation = visualRoot.localRotation;
        }

        private void EnsureVisualDoesNotCollide()
        {
            if (visualRoot == null)
            {
                return;
            }

            foreach (var collider in visualRoot.GetComponentsInChildren<Collider>(true))
            {
                collider.enabled = false;
            }
        }
    }
}
