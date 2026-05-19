using UnityEngine;

namespace ValleDePlata.Prototype
{
    public readonly struct PrototypeAvatarReadinessReport
    {
        public PrototypeAvatarReadinessReport(
            string prefabName,
            PrototypeAvatarRuntimeReadiness runtimeReadiness,
            PrototypeAvatarRigReadiness rigReadiness,
            int rendererCount,
            int skinnedMeshRendererCount,
            int skeletonTransformCount,
            int gameplayColliderCount,
            int rigidbodyCount,
            bool hasAnimator,
            bool hasAnimatorController,
            bool hasAnimatorAvatar,
            bool hasValidAvatar,
            bool hasHumanAvatar,
            bool hasValidHumanoidAvatar,
            int animationClipCount,
            bool usesPlaceholderAnimationOnly,
            bool supportsRuntimeCustomization,
            float estimatedHeightMeters)
        {
            PrefabName = prefabName;
            RuntimeReadiness = runtimeReadiness;
            RigReadiness = rigReadiness;
            RendererCount = rendererCount;
            SkinnedMeshRendererCount = skinnedMeshRendererCount;
            SkeletonTransformCount = skeletonTransformCount;
            GameplayColliderCount = gameplayColliderCount;
            RigidbodyCount = rigidbodyCount;
            HasAnimator = hasAnimator;
            HasAnimatorController = hasAnimatorController;
            HasAnimatorAvatar = hasAnimatorAvatar;
            HasValidAvatar = hasValidAvatar;
            HasHumanAvatar = hasHumanAvatar;
            HasValidHumanoidAvatar = hasValidHumanoidAvatar;
            AnimationClipCount = animationClipCount;
            UsesPlaceholderAnimationOnly = usesPlaceholderAnimationOnly;
            SupportsRuntimeCustomization = supportsRuntimeCustomization;
            EstimatedHeightMeters = estimatedHeightMeters;
        }

        public string PrefabName { get; }
        public PrototypeAvatarRuntimeReadiness RuntimeReadiness { get; }
        public PrototypeAvatarRigReadiness RigReadiness { get; }
        public int RendererCount { get; }
        public int SkinnedMeshRendererCount { get; }
        public int SkeletonTransformCount { get; }
        public int GameplayColliderCount { get; }
        public int RigidbodyCount { get; }
        public bool HasAnimator { get; }
        public bool HasAnimatorController { get; }
        public bool HasAnimatorAvatar { get; }
        public bool HasValidAvatar { get; }
        public bool HasHumanAvatar { get; }
        public bool HasValidHumanoidAvatar { get; }
        public int AnimationClipCount { get; }
        public bool UsesPlaceholderAnimationOnly { get; }
        public bool SupportsRuntimeCustomization { get; }
        public float EstimatedHeightMeters { get; }
        public bool IsPlayablePlaceholder => RuntimeReadiness == PrototypeAvatarRuntimeReadiness.StaticPlayablePlaceholder
            && RendererCount > 0
            && GameplayColliderCount == 0
            && RigidbodyCount == 0;
        public bool IsSkinnedRigCandidate => RuntimeReadiness == PrototypeAvatarRuntimeReadiness.SkinnedRigCandidate
            && SkinnedMeshRendererCount > 0
            && SkeletonTransformCount >= 20
            && GameplayColliderCount == 0
            && RigidbodyCount == 0;
        public bool RequiresRiggingBeforeAnimation => !HasValidHumanoidAvatar
            || SkinnedMeshRendererCount == 0
            || !HasAnimator
            || UsesPlaceholderAnimationOnly;

        public override string ToString()
        {
            var readiness = RuntimeReadiness switch
            {
                PrototypeAvatarRuntimeReadiness.StaticPlayablePlaceholder => "playable static placeholder",
                PrototypeAvatarRuntimeReadiness.SkinnedRigCandidate => "skinned rig candidate",
                _ => RuntimeReadiness.ToString()
            };
            var animation = HasAnimatorController
                ? $"animatorClips={AnimationClipCount}, placeholderOnly={UsesPlaceholderAnimationOnly}"
                : "animatorClips=0";
            var avatar = HasAnimatorAvatar
                ? $"avatarValid={HasValidAvatar}, avatarHuman={HasHumanAvatar}"
                : "avatar=none";
            return $"{PrefabName}: {readiness}, renderers={RendererCount}, skinned={SkinnedMeshRendererCount}, skeletonTransforms={SkeletonTransformCount}, colliders={GameplayColliderCount}, rigidbodies={RigidbodyCount}, {avatar}, {animation}, height={EstimatedHeightMeters:0.00}m.";
        }
    }

    public static class PrototypeAvatarReadiness
    {
        public static PrototypeAvatarReadinessReport AnalyzePrefab(GameObject prefab, PrototypeAvatarDefinition definition)
        {
            if (prefab == null)
            {
                return new PrototypeAvatarReadinessReport(
                    "missing prefab",
                    PrototypeAvatarRuntimeReadiness.StaticPlayablePlaceholder,
                    PrototypeAvatarRigReadiness.UnriggedStaticMesh,
                    0,
                    0,
                    0,
                    0,
                    0,
                    false,
                    false,
                    false,
                    false,
                    false,
                    false,
                    0,
                    false,
                    false,
                    0f);
            }

            return AnalyzeObject(prefab, definition);
        }

        public static PrototypeAvatarReadinessReport AnalyzeObject(GameObject root, PrototypeAvatarDefinition definition)
        {
            var renderers = root != null ? root.GetComponentsInChildren<Renderer>(true) : System.Array.Empty<Renderer>();
            var colliders = root != null ? root.GetComponentsInChildren<Collider>(true) : System.Array.Empty<Collider>();
            var rigidbodies = root != null ? root.GetComponentsInChildren<Rigidbody>(true) : System.Array.Empty<Rigidbody>();
            var transforms = root != null ? root.GetComponentsInChildren<Transform>(true) : System.Array.Empty<Transform>();
            var animator = root != null ? root.GetComponentInChildren<Animator>(true) : null;
            var controller = animator != null ? animator.runtimeAnimatorController : null;
            var avatar = animator != null ? animator.avatar : null;
            var clips = controller != null ? controller.animationClips : System.Array.Empty<AnimationClip>();
            var skinnedMeshRendererCount = 0;
            foreach (var renderer in renderers)
            {
                if (renderer is SkinnedMeshRenderer)
                {
                    skinnedMeshRendererCount++;
                }
            }

            var gameplayColliderCount = 0;
            foreach (var collider in colliders)
            {
                if (collider.enabled && !collider.isTrigger)
                {
                    gameplayColliderCount++;
                }
            }

            return new PrototypeAvatarReadinessReport(
                root != null ? root.name : "missing root",
                definition != null ? definition.RuntimeReadiness : PrototypeAvatarRuntimeReadiness.StaticPlayablePlaceholder,
                definition != null ? definition.RigReadiness : PrototypeAvatarRigReadiness.UnriggedStaticMesh,
                renderers.Length,
                skinnedMeshRendererCount,
                transforms.Length,
                gameplayColliderCount,
                rigidbodies.Length,
                animator != null,
                controller != null,
                avatar != null,
                avatar != null && avatar.isValid,
                avatar != null && avatar.isHuman,
                avatar != null && avatar.isValid && avatar.isHuman,
                clips.Length,
                UsesOnlyPlaceholderClips(clips),
                definition != null && definition.SupportsRuntimeCustomization,
                EstimateHeightMeters(renderers));
        }

        private static bool UsesOnlyPlaceholderClips(AnimationClip[] clips)
        {
            if (clips == null || clips.Length == 0)
            {
                return false;
            }

            foreach (var clip in clips)
            {
                if (clip == null || !clip.name.StartsWith("Placeholder_", System.StringComparison.Ordinal))
                {
                    return false;
                }
            }

            return true;
        }

        public static float EstimateHeightMeters(Renderer[] renderers)
        {
            if (renderers == null || renderers.Length == 0)
            {
                return 0f;
            }

            var bounds = renderers[0].bounds;
            for (var i = 1; i < renderers.Length; i++)
            {
                bounds.Encapsulate(renderers[i].bounds);
            }

            return bounds.size.y;
        }
    }
}
