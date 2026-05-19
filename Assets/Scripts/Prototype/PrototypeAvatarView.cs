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
    }
}
