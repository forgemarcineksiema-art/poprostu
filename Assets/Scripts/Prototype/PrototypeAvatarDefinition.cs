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
        SkinnedRigCandidate,
        RiggedHumanoidReady,
        ModularCustomizationReady
    }

    public enum PrototypeAvatarRigReadiness
    {
        UnriggedStaticMesh,
        GenericRig,
        HumanoidRig
    }

    public enum PrototypeAvatarAnimationReadiness
    {
        None,
        GenericPlaceholderController,
        RuntimeLocomotionDriven,
        HumanoidRetargetReady
    }

    public enum PrototypeAvatarRigDecision
    {
        Undecided,
        KeepVisualRequestHumanoidSource,
        ReadyForHumanoidLocomotion,
        RejectPlayableAvatar
    }

    public enum PrototypeAvatarVisualAcceptance
    {
        Unreviewed,
        TechnicalPipelineOnly,
        GameplayCandidate,
        FinalIdentityAccepted
    }

    [CreateAssetMenu(fileName = "PrototypeAvatarDefinition", menuName = "Valle de Plata/Prototype Avatar Definition")]
    public sealed class PrototypeAvatarDefinition : ScriptableObject
    {
        [SerializeField] private string characterId = "pablo-valera";
        [SerializeField] private string displayName = "Pablo Valera";
        [SerializeField] private PrototypeAvatarAssemblyMode assemblyMode = PrototypeAvatarAssemblyMode.FullBodyPlaceholder;
        [SerializeField] private PrototypeAvatarRuntimeReadiness runtimeReadiness = PrototypeAvatarRuntimeReadiness.StaticPlayablePlaceholder;
        [SerializeField] private PrototypeAvatarRigReadiness rigReadiness = PrototypeAvatarRigReadiness.UnriggedStaticMesh;
        [SerializeField] private PrototypeAvatarAnimationReadiness animationReadiness = PrototypeAvatarAnimationReadiness.None;
        [SerializeField] private PrototypeAvatarRigDecision rigDecision = PrototypeAvatarRigDecision.Undecided;
        [SerializeField] private PrototypeAvatarVisualAcceptance visualAcceptance = PrototypeAvatarVisualAcceptance.Unreviewed;
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
        [SerializeField] private string fullBodyInstanceName = "MaleCrimeDrama Visual Mesh";
        [SerializeField] private RuntimeAnimatorController runtimeAnimatorController;
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
        public PrototypeAvatarAnimationReadiness AnimationReadiness => animationReadiness;
        public PrototypeAvatarRigDecision RigDecision => rigDecision;
        public PrototypeAvatarVisualAcceptance VisualAcceptance => visualAcceptance;
        public bool IsFinalIdentityLocked => isFinalIdentityLocked;
        public bool RequiresPlayableVisualReplacement => visualAcceptance == PrototypeAvatarVisualAcceptance.TechnicalPipelineOnly
            || rigDecision == PrototypeAvatarRigDecision.RejectPlayableAvatar;
        public bool SupportsRuntimeCustomization => supportsRuntimeCustomization;
        public PrototypeAvatarSlot[] PlannedCustomizationSlots => plannedCustomizationSlots;
        public GameObject FullBodyPrefab => fullBodyPrefab;
        public string FullBodyInstanceName => string.IsNullOrWhiteSpace(fullBodyInstanceName)
            ? "Pablo Avatar Visual Mesh"
            : fullBodyInstanceName;
        public RuntimeAnimatorController RuntimeAnimatorController => runtimeAnimatorController;
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
            animationReadiness = PrototypeAvatarAnimationReadiness.None;
            rigDecision = PrototypeAvatarRigDecision.Undecided;
            visualAcceptance = PrototypeAvatarVisualAcceptance.TechnicalPipelineOnly;
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
            fullBodyInstanceName = "MaleCrimeDrama Visual Mesh";
            runtimeAnimatorController = null;
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

        public void ConfigureSkinnedRigCandidate(GameObject prefab)
        {
            characterId = "pablo-valera";
            displayName = "Pablo Valera";
            assemblyMode = PrototypeAvatarAssemblyMode.FullBodyPlaceholder;
            runtimeReadiness = PrototypeAvatarRuntimeReadiness.SkinnedRigCandidate;
            rigReadiness = PrototypeAvatarRigReadiness.GenericRig;
            animationReadiness = PrototypeAvatarAnimationReadiness.GenericPlaceholderController;
            rigDecision = PrototypeAvatarRigDecision.KeepVisualRequestHumanoidSource;
            visualAcceptance = PrototypeAvatarVisualAcceptance.TechnicalPipelineOnly;
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
            fullBodyInstanceName = "PabloValera_V2 Visual Mesh";
            runtimeAnimatorController = null;
            visualRootLocalPosition = new Vector3(0f, 0.88f, 0f);
            visualRootLocalEuler = Vector3.zero;
            visualRootLocalScale = 1f;
            fullBodyLocalPosition = Vector3.zero;
            fullBodyLocalEuler = Vector3.zero;
            fullBodyLocalScale = 1.8f;
            expectedRuntimeHeightMeters = 1.8f;
            minimumRuntimeHeightMeters = 1.35f;
            maximumRuntimeHeightMeters = 2.25f;
            hideVisualWhileDriving = true;
            authoringNotes = "Unity AI generated Pablo V2 is a skinned rig candidate. It is not final identity, not runtime-customizable, and still needs Humanoid validation plus animation clips.";
        }

        public void ConfigureHumanoidRuntimeCandidate(GameObject prefab, RuntimeAnimatorController controller)
        {
            characterId = "pablo-valera";
            displayName = "Pablo Valera";
            assemblyMode = PrototypeAvatarAssemblyMode.FullBodyPlaceholder;
            runtimeReadiness = PrototypeAvatarRuntimeReadiness.RiggedHumanoidReady;
            rigReadiness = PrototypeAvatarRigReadiness.HumanoidRig;
            animationReadiness = PrototypeAvatarAnimationReadiness.RuntimeLocomotionDriven;
            rigDecision = PrototypeAvatarRigDecision.ReadyForHumanoidLocomotion;
            visualAcceptance = PrototypeAvatarVisualAcceptance.TechnicalPipelineOnly;
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
            fullBodyInstanceName = "PabloValera_HumanoidCandidate Visual Mesh";
            runtimeAnimatorController = controller;
            visualRootLocalPosition = new Vector3(0f, 0.88f, 0f);
            visualRootLocalEuler = Vector3.zero;
            visualRootLocalScale = 1f;
            fullBodyLocalPosition = Vector3.zero;
            fullBodyLocalEuler = Vector3.zero;
            fullBodyLocalScale = 1.8f;
            expectedRuntimeHeightMeters = 1.8f;
            minimumRuntimeHeightMeters = 1.35f;
            maximumRuntimeHeightMeters = 2.25f;
            hideVisualWhileDriving = true;
            authoringNotes = "Unity AI Humanoid candidate remains technical pipeline only: rig/runtime wiring is useful, but visual QA rejected it as Pablo's playable identity because the in-game silhouette, head, shoulders, arms, and legs are not acceptable.";
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
                error = "Full-body avatar prefab is missing.";
                return false;
            }

            if (string.IsNullOrWhiteSpace(fullBodyInstanceName))
            {
                error = "Full-body avatar instance name is missing.";
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

            if (animationReadiness == PrototypeAvatarAnimationReadiness.RuntimeLocomotionDriven
                && runtimeAnimatorController == null)
            {
                error = "Runtime locomotion-driven avatar needs a game-owned Animator Controller.";
                return false;
            }

            if (isFinalIdentityLocked && visualAcceptance != PrototypeAvatarVisualAcceptance.FinalIdentityAccepted)
            {
                error = "Final identity cannot be locked until visual QA accepts the avatar.";
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
            if (runtimeReadiness == PrototypeAvatarRuntimeReadiness.SkinnedRigCandidate)
            {
                return $"{displayName} is a skinned rig candidate using {fullBodyPrefab?.name ?? "no prefab"}; current Animator is a Generic placeholder controller, rig decision is {rigDecision}, and the next pass needs humanoid validation through a Humanoid-native source, real animation clips, and {customizationState}.";
            }

            if (runtimeReadiness == PrototypeAvatarRuntimeReadiness.RiggedHumanoidReady)
            {
                var visualState = RequiresPlayableVisualReplacement
                    ? "visual QA rejected it as Pablo's playable identity; use it as technical pipeline only"
                    : $"visual acceptance is {visualAcceptance}";
                return $"{displayName} is a runtime Humanoid candidate using {fullBodyPrefab?.name ?? "no prefab"}; the Animator bridge is game-owned, rig decision is {rigDecision}, {visualState}, and the model remains exchangeable with {customizationState}.";
            }

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
