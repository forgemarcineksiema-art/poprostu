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
            int gameplayColliderCount,
            int rigidbodyCount,
            bool hasAnimator,
            bool supportsRuntimeCustomization,
            float estimatedHeightMeters)
        {
            PrefabName = prefabName;
            RuntimeReadiness = runtimeReadiness;
            RigReadiness = rigReadiness;
            RendererCount = rendererCount;
            SkinnedMeshRendererCount = skinnedMeshRendererCount;
            GameplayColliderCount = gameplayColliderCount;
            RigidbodyCount = rigidbodyCount;
            HasAnimator = hasAnimator;
            SupportsRuntimeCustomization = supportsRuntimeCustomization;
            EstimatedHeightMeters = estimatedHeightMeters;
        }

        public string PrefabName { get; }
        public PrototypeAvatarRuntimeReadiness RuntimeReadiness { get; }
        public PrototypeAvatarRigReadiness RigReadiness { get; }
        public int RendererCount { get; }
        public int SkinnedMeshRendererCount { get; }
        public int GameplayColliderCount { get; }
        public int RigidbodyCount { get; }
        public bool HasAnimator { get; }
        public bool SupportsRuntimeCustomization { get; }
        public float EstimatedHeightMeters { get; }
        public bool IsPlayablePlaceholder => RuntimeReadiness == PrototypeAvatarRuntimeReadiness.StaticPlayablePlaceholder
            && RendererCount > 0
            && GameplayColliderCount == 0
            && RigidbodyCount == 0;
        public bool RequiresRiggingBeforeAnimation => RigReadiness != PrototypeAvatarRigReadiness.HumanoidRig
            || SkinnedMeshRendererCount == 0
            || !HasAnimator;

        public override string ToString()
        {
            var readiness = RuntimeReadiness == PrototypeAvatarRuntimeReadiness.StaticPlayablePlaceholder
                ? "playable static placeholder"
                : RuntimeReadiness.ToString();
            return $"{PrefabName}: {readiness}, renderers={RendererCount}, skinned={SkinnedMeshRendererCount}, colliders={GameplayColliderCount}, rigidbodies={RigidbodyCount}, height={EstimatedHeightMeters:0.00}m.";
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
            var hasAnimator = root != null && root.GetComponentInChildren<Animator>(true) != null;
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
                gameplayColliderCount,
                rigidbodies.Length,
                hasAnimator,
                definition != null && definition.SupportsRuntimeCustomization,
                EstimateHeightMeters(renderers));
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
