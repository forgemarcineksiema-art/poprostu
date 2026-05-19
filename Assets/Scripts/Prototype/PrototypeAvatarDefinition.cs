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

    public enum PrototypeAvatarRuntimeReadiness
    {
        StaticPlayablePlaceholder,
        RiggedHumanoidReady,
        ModularCustomizationReady
    }

    public enum PrototypeAvatarRigReadiness
    {
        UnriggedStaticMesh,
        GenericRig,
        HumanoidRig
    }

    [CreateAssetMenu(fileName = "PrototypeAvatarDefinition", menuName = "Valle de Plata/Prototype Avatar Definition")]
    public sealed class PrototypeAvatarDefinition : ScriptableObject
    {
        [SerializeField] private string characterId = "pablo-valera";
        [SerializeField] private string displayName = "Pablo Valera";
        [SerializeField] private PrototypeAvatarAssemblyMode assemblyMode = PrototypeAvatarAssemblyMode.FullBodyPlaceholder;
        [SerializeField] private PrototypeAvatarRuntimeReadiness runtimeReadiness = PrototypeAvatarRuntimeReadiness.StaticPlayablePlaceholder;
        [SerializeField] private PrototypeAvatarRigReadiness rigReadiness = PrototypeAvatarRigReadiness.UnriggedStaticMesh;
        [SerializeField] private bool isFinalIdentityLocked;
        [SerializeField] private bool supportsRuntimeCustomization;
        [SerializeField] private PrototypeAvatarSlot[] plannedCustomizationSlots =
        {
            PrototypeAvatarSlot.Body,
            PrototypeAvatarSlot.Head,
            PrototypeAvatarSlot.Hair,
            PrototypeAvatarSlot.Jacket,
            PrototypeAvatarSlot.Shirt,
            PrototypeAvatarSlot.Pants,
            PrototypeAvatarSlot.Shoes,
            PrototypeAvatarSlot.Accessory
        };

        [SerializeField] private GameObject fullBodyPrefab;
        [SerializeField] private Vector3 visualRootLocalPosition = new(0f, 0.88f, 0f);
        [SerializeField] private Vector3 visualRootLocalEuler;
        [SerializeField] private float visualRootLocalScale = 1f;
        [SerializeField] private Vector3 fullBodyLocalPosition;
        [SerializeField] private Vector3 fullBodyLocalEuler;
        [SerializeField] private float fullBodyLocalScale = 1.75f;
        [SerializeField] private float expectedRuntimeHeightMeters = 1.8f;
        [SerializeField] private float minimumRuntimeHeightMeters = 1.35f;
        [SerializeField] private float maximumRuntimeHeightMeters = 2.25f;
        [SerializeField] private bool hideVisualWhileDriving = true;
        [SerializeField] private string authoringNotes = "Unity AI generated full-body placeholder. Not final Pablo identity.";

        public string CharacterId => characterId;
        public string DisplayName => displayName;
        public PrototypeAvatarAssemblyMode AssemblyMode => assemblyMode;
        public PrototypeAvatarRuntimeReadiness RuntimeReadiness => runtimeReadiness;
        public PrototypeAvatarRigReadiness RigReadiness => rigReadiness;
        public bool IsFinalIdentityLocked => isFinalIdentityLocked;
        public bool SupportsRuntimeCustomization => supportsRuntimeCustomization;
        public PrototypeAvatarSlot[] PlannedCustomizationSlots => plannedCustomizationSlots;
        public GameObject FullBodyPrefab => fullBodyPrefab;
        public float ExpectedRuntimeHeightMeters => expectedRuntimeHeightMeters;
        public float MinimumRuntimeHeightMeters => minimumRuntimeHeightMeters;
        public float MaximumRuntimeHeightMeters => maximumRuntimeHeightMeters;
        public bool HideVisualWhileDriving => hideVisualWhileDriving;

        public void ConfigurePrototypePlaceholder(GameObject prefab)
        {
            characterId = "pablo-valera";
            displayName = "Pablo Valera";
            assemblyMode = PrototypeAvatarAssemblyMode.FullBodyPlaceholder;
            runtimeReadiness = PrototypeAvatarRuntimeReadiness.StaticPlayablePlaceholder;
            rigReadiness = PrototypeAvatarRigReadiness.UnriggedStaticMesh;
            isFinalIdentityLocked = false;
            supportsRuntimeCustomization = false;
            plannedCustomizationSlots = new[]
            {
                PrototypeAvatarSlot.Body,
                PrototypeAvatarSlot.Head,
                PrototypeAvatarSlot.Hair,
                PrototypeAvatarSlot.Jacket,
                PrototypeAvatarSlot.Shirt,
                PrototypeAvatarSlot.Pants,
                PrototypeAvatarSlot.Shoes,
                PrototypeAvatarSlot.Accessory
            };

            fullBodyPrefab = prefab;
            visualRootLocalPosition = new Vector3(0f, 0.88f, 0f);
            visualRootLocalEuler = Vector3.zero;
            visualRootLocalScale = 1f;
            fullBodyLocalPosition = Vector3.zero;
            fullBodyLocalEuler = Vector3.zero;
            fullBodyLocalScale = 1.75f;
            expectedRuntimeHeightMeters = 1.8f;
            minimumRuntimeHeightMeters = 1.35f;
            maximumRuntimeHeightMeters = 2.25f;
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

            if (minimumRuntimeHeightMeters <= 0f || maximumRuntimeHeightMeters <= minimumRuntimeHeightMeters)
            {
                error = "Avatar runtime height bounds are invalid.";
                return false;
            }

            if (expectedRuntimeHeightMeters < minimumRuntimeHeightMeters || expectedRuntimeHeightMeters > maximumRuntimeHeightMeters)
            {
                error = "Avatar expected runtime height must fit inside runtime height bounds.";
                return false;
            }

            if (plannedCustomizationSlots == null || plannedCustomizationSlots.Length == 0)
            {
                error = "Avatar definition needs planned customization slots even before runtime customization is enabled.";
                return false;
            }

            error = string.Empty;
            return true;
        }

        public string BuildAuthoringSummary()
        {
            var customizationState = supportsRuntimeCustomization
                ? "runtime customization enabled"
                : "customization slots planned but not runtime-enabled";
            return $"{displayName} is a static full-body placeholder using {fullBodyPrefab?.name ?? "no prefab"}; next model pass needs a rigged humanoid before animation and {customizationState}.";
        }

        public bool IsHeightWithinRuntimeBounds(float heightMeters)
        {
            return heightMeters >= minimumRuntimeHeightMeters && heightMeters <= maximumRuntimeHeightMeters;
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
