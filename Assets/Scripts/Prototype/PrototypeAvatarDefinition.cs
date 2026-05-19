using UnityEngine;

namespace ValleDePlata.Prototype
{
    public enum PrototypeAvatarAssemblyMode
    {
        FullBodyPlaceholder,
        ModularSlots
    }

    public enum PrototypeAvatarSlot
    {
        FullBody,
        Body,
        Head,
        Hair,
        Jacket,
        Shirt,
        Pants,
        Shoes,
        Accessory
    }

    [CreateAssetMenu(fileName = "PrototypeAvatarDefinition", menuName = "Valle de Plata/Prototype Avatar Definition")]
    public sealed class PrototypeAvatarDefinition : ScriptableObject
    {
        [SerializeField] private string characterId = "pablo-valera";
        [SerializeField] private string displayName = "Pablo Valera";
        [SerializeField] private PrototypeAvatarAssemblyMode assemblyMode = PrototypeAvatarAssemblyMode.FullBodyPlaceholder;
        [SerializeField] private bool isFinalIdentityLocked;
        [SerializeField] private GameObject fullBodyPrefab;
        [SerializeField] private Vector3 visualRootLocalPosition = new(0f, 0.88f, 0f);
        [SerializeField] private Vector3 visualRootLocalEuler;
        [SerializeField] private float visualRootLocalScale = 1f;
        [SerializeField] private Vector3 fullBodyLocalPosition;
        [SerializeField] private Vector3 fullBodyLocalEuler;
        [SerializeField] private float fullBodyLocalScale = 1.75f;
        [SerializeField] private bool hideVisualWhileDriving = true;
        [SerializeField] private string authoringNotes = "Unity AI generated full-body placeholder. Not final Pablo identity.";

        public string CharacterId => characterId;
        public string DisplayName => displayName;
        public PrototypeAvatarAssemblyMode AssemblyMode => assemblyMode;
        public bool IsFinalIdentityLocked => isFinalIdentityLocked;
        public GameObject FullBodyPrefab => fullBodyPrefab;
        public bool HideVisualWhileDriving => hideVisualWhileDriving;

        public void ConfigurePrototypePlaceholder(GameObject prefab)
        {
            characterId = "pablo-valera";
            displayName = "Pablo Valera";
            assemblyMode = PrototypeAvatarAssemblyMode.FullBodyPlaceholder;
            isFinalIdentityLocked = false;
            fullBodyPrefab = prefab;
            visualRootLocalPosition = new Vector3(0f, 0.88f, 0f);
            visualRootLocalEuler = Vector3.zero;
            visualRootLocalScale = 1f;
            fullBodyLocalPosition = Vector3.zero;
            fullBodyLocalEuler = Vector3.zero;
            fullBodyLocalScale = 1.75f;
            hideVisualWhileDriving = true;
            authoringNotes = "Unity AI generated full-body placeholder. Keep Pablo exchangeable until modular avatar direction is locked.";
        }

        public bool Validate(out string error)
        {
            if (string.IsNullOrWhiteSpace(characterId))
            {
                error = "Avatar definition needs a stable character id.";
                return false;
            }

            if (string.IsNullOrWhiteSpace(displayName))
            {
                error = "Avatar definition needs a display name.";
                return false;
            }

            if (fullBodyPrefab == null)
            {
                error = "Full-body placeholder prefab is missing.";
                return false;
            }

            if (visualRootLocalScale <= 0.01f || fullBodyLocalScale <= 0.01f)
            {
                error = "Avatar visual scales must be positive.";
                return false;
            }

            error = string.Empty;
            return true;
        }

        public bool UsesSlot(PrototypeAvatarSlot slot)
        {
            return assemblyMode == PrototypeAvatarAssemblyMode.FullBodyPlaceholder
                ? slot == PrototypeAvatarSlot.FullBody
                : slot != PrototypeAvatarSlot.FullBody;
        }

        public void ApplyVisualRootTransform(Transform visualRoot)
        {
            if (visualRoot == null)
            {
                return;
            }

            visualRoot.localPosition = visualRootLocalPosition;
            visualRoot.localRotation = Quaternion.Euler(visualRootLocalEuler);
            visualRoot.localScale = Vector3.one * visualRootLocalScale;
        }

        public void ApplyFullBodyTransform(Transform fullBodyRoot)
        {
            if (fullBodyRoot == null)
            {
                return;
            }

            fullBodyRoot.localPosition = fullBodyLocalPosition;
            fullBodyRoot.localRotation = Quaternion.Euler(fullBodyLocalEuler);
            fullBodyRoot.localScale = Vector3.one * fullBodyLocalScale;
        }
    }
}
