namespace ValleDePlata.Prototype
{
    public readonly struct PrototypeAvatarRigDecisionReport
    {
        public PrototypeAvatarRigDecisionReport(
            PrototypeAvatarRigDecision decision,
            string reason,
            bool shouldUseUnityAiForNextAssetPass,
            string unityAiScope)
        {
            Decision = decision;
            Reason = reason;
            ShouldUseUnityAiForNextAssetPass = shouldUseUnityAiForNextAssetPass;
            UnityAiScope = unityAiScope;
        }

        public PrototypeAvatarRigDecision Decision { get; }
        public string Reason { get; }
        public bool ShouldUseUnityAiForNextAssetPass { get; }
        public string UnityAiScope { get; }
    }

    public static class PrototypeAvatarRigDecisionPolicy
    {
        private const string HumanoidNativeUnityAiScope =
            "Create or export a Humanoid-native Pablo Valera source asset. Do not edit gameplay scripts, scenes, packages, project settings, input, camera, motor, vehicle, mission, route, HUD, or Unity AI packages.";

        public static PrototypeAvatarRigDecisionReport Decide(PrototypeAvatarReadinessReport readiness)
        {
            if (readiness.HasValidHumanoidAvatar && readiness.AnimationClipCount > 0 && !readiness.UsesPlaceholderAnimationOnly)
            {
                return new PrototypeAvatarRigDecisionReport(
                    PrototypeAvatarRigDecision.ReadyForHumanoidLocomotion,
                    "Avatar has a valid Humanoid Avatar and non-placeholder animation clips.",
                    false,
                    "Unity AI is not needed for the next rig step; continue with controlled animation integration.");
            }

            if (readiness.SkinnedMeshRendererCount > 0 && readiness.HasAnimator && readiness.HasAnimatorController)
            {
                return new PrototypeAvatarRigDecisionReport(
                    PrototypeAvatarRigDecision.KeepVisualRequestHumanoidSource,
                    "Pablo V2 is a Generic rig candidate: Animator exists, but there is no valid Humanoid Avatar, so it can stay as a visual candidate while the next asset pass requests a Humanoid-native source.",
                    true,
                    HumanoidNativeUnityAiScope);
            }

            return new PrototypeAvatarRigDecisionReport(
                PrototypeAvatarRigDecision.RejectPlayableAvatar,
                "Avatar is missing the skinned mesh, Animator, or controller needed for a playable third-person character foundation.",
                true,
                HumanoidNativeUnityAiScope);
        }

        public static string BuildReport(PrototypeAvatarDefinition definition, PrototypeAvatarReadinessReport readiness)
        {
            var decision = Decide(readiness);
            return
                $"Character: {definition?.DisplayName ?? readiness.PrefabName}\n" +
                $"Decision: {decision.Decision}\n" +
                $"Reason: {decision.Reason}\n" +
                $"Readiness: {readiness}\n" +
                $"Unity AI next asset pass: {(decision.ShouldUseUnityAiForNextAssetPass ? "yes" : "no")}\n" +
                $"Unity AI scope: {decision.UnityAiScope}";
        }
    }
}
