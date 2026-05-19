using UnityEngine;

namespace ValleDePlata.Prototype
{
    public sealed class PrototypeAvatarView : MonoBehaviour
    {
        [SerializeField] private PrototypeAvatarDefinition avatarDefinition;
        [SerializeField] private Transform fullBodyRoot;

        public PrototypeAvatarDefinition AvatarDefinition => avatarDefinition;
        public Transform FullBodyRoot => fullBodyRoot;

        private void Awake()
        {
            ApplyDefinition();
            EnsureNonGameplayVisual();
        }

        public void Configure(PrototypeAvatarDefinition definition, Transform fullBody)
        {
            avatarDefinition = definition;
            fullBodyRoot = fullBody;
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
        }

        public void SetVisible(bool visible)
        {
            gameObject.SetActive(visible);
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
    }
}
