using UnityEngine;

namespace ValleDePlata.Prototype
{
    public sealed class PrototypeAvatarView : MonoBehaviour
    {
        private static readonly int SpeedParameter = Animator.StringToHash("Speed");
        private const string IsSprintingParameter = "IsSprinting";
        private const string GroundedParameter = "Grounded";

        [SerializeField] private PrototypeAvatarDefinition avatarDefinition;
        [SerializeField] private Transform fullBodyRoot;

        private Animator animator;
        private bool hasSpeedParameter;
        private bool hasIsSprintingParameter;
        private bool hasGroundedParameter;

        public PrototypeAvatarDefinition AvatarDefinition => avatarDefinition;
        public Transform FullBodyRoot => fullBodyRoot;

        private void Awake()
        {
            CacheAnimator();
            ApplyDefinition();
            EnsureNonGameplayVisual();
        }

        public void Configure(PrototypeAvatarDefinition definition, Transform fullBody)
        {
            avatarDefinition = definition;
            fullBodyRoot = fullBody;
            CacheAnimator();
            ApplyDefinition();
            EnsureNonGameplayVisual();
        }

        public void ApplyDefinition()
        {
            if (avatarDefinition == null)
            {
                return;
            }

            avatarDefinition.ApplyVisualRootTransform(transform);
            avatarDefinition.ApplyFullBodyTransform(fullBodyRoot);
            ApplyRuntimeAnimatorController();
            CacheAnimator();
        }

        public void SetVisible(bool visible)
        {
            gameObject.SetActive(visible);
        }

        public void ApplyAnimatorLocomotion(float speed, bool sprinting, bool grounded)
        {
            CacheAnimator();
            if (animator == null || animator.runtimeAnimatorController == null)
            {
                return;
            }

            if (hasSpeedParameter)
            {
                animator.SetFloat(SpeedParameter, Mathf.Max(0f, speed));
            }

            if (hasIsSprintingParameter)
            {
                animator.SetBool(IsSprintingParameter, sprinting);
            }

            if (hasGroundedParameter)
            {
                animator.SetBool(GroundedParameter, grounded);
            }
        }

        public float EstimateVisualHeightMeters()
        {
            var root = fullBodyRoot != null ? fullBodyRoot.gameObject : gameObject;
            return PrototypeAvatarReadiness.AnalyzeObject(root, avatarDefinition).EstimatedHeightMeters;
        }

        public PrototypeAvatarReadinessReport BuildReadinessReport()
        {
            var root = fullBodyRoot != null ? fullBodyRoot.gameObject : gameObject;
            return PrototypeAvatarReadiness.AnalyzeObject(root, avatarDefinition);
        }

        public bool ValidateRuntimeVisual(out string issue)
        {
            if (avatarDefinition == null)
            {
                issue = "Avatar view has no avatar definition.";
                return false;
            }

            if (!avatarDefinition.Validate(out issue))
            {
                return false;
            }

            if (fullBodyRoot == null)
            {
                issue = "Avatar view has no full-body root.";
                return false;
            }

            var report = BuildReadinessReport();
            if (report.RendererCount == 0)
            {
                issue = "Avatar full-body root has no renderers.";
                return false;
            }

            if (report.GameplayColliderCount > 0)
            {
                issue = "Avatar visual contains enabled blocking colliders.";
                return false;
            }

            if (report.RigidbodyCount > 0)
            {
                issue = "Avatar visual contains rigidbodies and should stay non-gameplay.";
                return false;
            }

            if (avatarDefinition.AnimationReadiness == PrototypeAvatarAnimationReadiness.GenericPlaceholderController
                && !report.HasAnimatorController)
            {
                issue = "Avatar definition expects a placeholder Animator Controller, but the visual has none.";
                return false;
            }

            if (avatarDefinition.AnimationReadiness == PrototypeAvatarAnimationReadiness.RuntimeLocomotionDriven)
            {
                if (!report.HasValidHumanoidAvatar)
                {
                    issue = "Runtime locomotion-driven avatar needs a valid Humanoid Avatar.";
                    return false;
                }

                if (!report.HasAnimatorController)
                {
                    issue = "Runtime locomotion-driven avatar needs an Animator Controller.";
                    return false;
                }
            }

            if (!avatarDefinition.IsHeightWithinRuntimeBounds(report.EstimatedHeightMeters))
            {
                issue = $"Avatar visual height {report.EstimatedHeightMeters:0.00}m is outside expected runtime bounds.";
                return false;
            }

            if (!AllChildrenUseLayer(transform, PrototypeLayers.Player))
            {
                issue = "Avatar visual hierarchy must stay on the Player layer.";
                return false;
            }

            issue = string.Empty;
            return true;
        }

        public void EnsureNonGameplayVisual()
        {
            PrototypeLayers.SetLayerRecursively(gameObject, PrototypeLayers.Player);

            foreach (var collider in GetComponentsInChildren<Collider>(true))
            {
                collider.enabled = false;
            }

            foreach (var body in GetComponentsInChildren<Rigidbody>(true))
            {
                body.isKinematic = true;
                body.detectCollisions = false;
            }
        }

        private static bool AllChildrenUseLayer(Transform root, int layer)
        {
            if (root == null || layer < 0)
            {
                return true;
            }

            if (root.gameObject.layer != layer)
            {
                return false;
            }

            foreach (Transform child in root)
            {
                if (!AllChildrenUseLayer(child, layer))
                {
                    return false;
                }
            }

            return true;
        }

        private void CacheAnimator()
        {
            animator = fullBodyRoot != null
                ? fullBodyRoot.GetComponentInChildren<Animator>(true)
                : GetComponentInChildren<Animator>(true);

            hasSpeedParameter = HasAnimatorParameter("Speed", AnimatorControllerParameterType.Float);
            hasIsSprintingParameter = HasAnimatorParameter("IsSprinting", AnimatorControllerParameterType.Bool);
            hasGroundedParameter = HasAnimatorParameter("Grounded", AnimatorControllerParameterType.Bool);
        }

        private void ApplyRuntimeAnimatorController()
        {
            if (avatarDefinition == null || avatarDefinition.RuntimeAnimatorController == null)
            {
                return;
            }

            var targetAnimator = fullBodyRoot != null
                ? fullBodyRoot.GetComponentInChildren<Animator>(true)
                : GetComponentInChildren<Animator>(true);
            if (targetAnimator == null)
            {
                return;
            }

            targetAnimator.runtimeAnimatorController = avatarDefinition.RuntimeAnimatorController;
            targetAnimator.applyRootMotion = false;
        }

        private bool HasAnimatorParameter(string parameterName, AnimatorControllerParameterType parameterType)
        {
            if (animator == null || animator.runtimeAnimatorController == null)
            {
                return false;
            }

            foreach (var parameter in animator.parameters)
            {
                if (parameter.name == parameterName && parameter.type == parameterType)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
