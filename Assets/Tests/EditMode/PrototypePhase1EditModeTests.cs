using System.IO;
using System.Reflection;
using System.Linq;
using NUnit.Framework;
using UnityEditor;
using UnityEditor.Animations;
using UnityEditor.SceneManagement;
using UnityEngine;
using ValleDePlata.Prototype;

namespace ValleDePlata.Tests
{
    public sealed class PrototypePhase1EditModeTests
    {
        private const string ScenePath = "Assets/Scenes/Phase1_FeelPrototype.unity";
        private const string ValleDePlataStreetKitPath = "Assets/Models/Environment/ValleDePlataStreetKit";
        private const string MaleCrimeDramaPrefabPath = "Assets/Models/Characters/MaleCrimeDrama.prefab";
        private const string PabloValeraV2PrefabPath = "Assets/Models/Characters/PabloValera_V2/PabloValera_V2.prefab";
        private const string PabloValeraV2GlbPath = "Assets/Models/Characters/PabloValera_V2/PabloValera_V2_Assets/selected.glb";
        private const string PabloValeraV2AnimatorPath = "Assets/Models/Characters/PabloValera_V2/Animation/PabloValera_V2_Animator.controller";
        private const string PabloHumanoidCandidatePrefabPath = "Assets/Models/Characters/PabloValera_HumanoidCandidate/PabloValera_HumanoidCandidate.prefab";
        private const string PabloHumanoidCandidateAvatarPath = "Assets/Models/Characters/PabloValera_HumanoidCandidate/PabloValera_HumanoidAvatar.asset";
        private const string PabloHumanoidCandidateAnimationsPath = "Assets/Models/Characters/PabloValera_HumanoidCandidate/Animations";
        private const string PabloHumanoidRuntimeAnimatorPath = "Assets/Models/Characters/PabloValera_HumanoidCandidate/Animations/PabloValera_Humanoid_Runtime.controller";
        private const string PabloHumanoidLowerBodyMaskPath = "Assets/Models/Characters/PabloValera_HumanoidCandidate/Animations/PabloValera_LocomotionLowerBody.mask";
        private const string PabloAvatarPass05ReportPath = "docs/prototype_reports/character_avatar_pass_0_5.md";

        private static readonly string[] PabloRuntimeLocomotionStates =
        {
            "Idle",
            "Walk",
            "Run",
            "Sprint"
        };

        private static readonly string[] PabloUpperBodyCurveFragments =
        {
            "Spine",
            "Chest",
            "UpperChest",
            "Neck",
            "Head",
            "Eye",
            "Jaw",
            "Shoulder",
            "Arm",
            "Forearm",
            "Hand"
        };

        private static readonly string[] ValleDePlataStreetKitStructuralPrefabs =
        {
            "VDP_Corner_Alley_01",
            "VDP_Facade_Plaster_01",
            "VDP_Facade_Shop_01",
            "VDP_Road_Narrow_01",
            "VDP_Rooftop_Parapet_01",
            "VDP_Shutter_Workshop_01",
            "VDP_Sidewalk_Curb_01",
            "VDP_Stairs_01",
            "VDP_Wall_Concrete_01"
        };

        private static readonly string[] ValleDePlataStreetKitDressingPrefabs =
        {
            "VDP_Awning_Market_01",
            "VDP_Balcony_01",
            "VDP_Lamp_Street_01",
            "VDP_Planter_Veg_01",
            "VDP_Pole_Cable_01",
            "VDP_Prop_Street_01",
            "VDP_Sign_Faded_01"
        };

        [Test]
        public void Phase1SceneContainsRequiredFeelPrototypeObjects()
        {
            EditorSceneManager.OpenScene(ScenePath);

            RequireComponent<PrototypePlayerController>("Pablo Valera Prototype Controller");
            RequireComponent<PrototypeCharacterMotor>("Pablo Valera Prototype Controller");
            RequireComponent<PrototypeCharacterPresentation>("Pablo Valera Prototype Controller");
            RequireComponent<PrototypeVehicleController>("Prototype Sedan");
            RequireComponent<PrototypeCameraRig>("Prototype Camera Rig");
            RequireComponent<PrototypeDebugHud>("Prototype Debug HUD");
            RequireComponent<PrototypeRunMetrics>("Phase 1 Run Metrics");
            RequireComponent<PrototypeWorldState>("Prototype World State");
            RequireComponent<PrototypeMissionSpine>("Pierwszy Front Mission Spine");
            RequireComponent<PrototypeObjectiveMarker>("Prototype Objective Marker");
            RequireComponent<PrototypePressureZone>("Pressure patrol marker");
            RequireComponent<PrototypePressureChoiceController>("Pressure patrol marker");
            RequireComponent<PrototypePressureScenePlayback>("Pressure patrol marker");
            RequireComponent<PrototypeInteractable>("Workshop shutter interactable");
            RequireComponent<PrototypeInteractable>("Public violence test target");
            RequireComponent<PrototypeWorldReactionMarker>("Civilian panic marker");
            RequireComponent<PrototypeWorldReactionMarker>("Shop shutter closes marker");
            RequireComponent<PrototypeWorldReactionMarker>("Police pressure moves closer marker");
            RequireComponent<PrototypeInteractable>("Rios bribe test officer");
            RequireComponent<PrototypeWorldReactionMarker>("Bribe roadblock opens marker");
            RequireComponent<PrototypeWorldReactionMarker>("Rios leverage marker");
            RequireComponent<PrototypeWorldReactionMarker>("Risk cargo hidden marker");
            RequireComponent<PrototypeInteractable>("Mateo protected test contact");
            RequireComponent<PrototypeInteractable>("Mateo humiliated test contact");
            RequireComponent<PrototypeWorldReactionMarker>("Mateo early warning marker");
            RequireComponent<PrototypeWorldReactionMarker>("Mateo late warning marker");
            RequireComponent<PrototypeInteractable>("El Respiro dirty cash pickup");
            RequireComponent<PrototypeInteractable>("El Respiro front takeover");
            RequireComponent<PrototypeWorldReactionMarker>("Dirty cash carried marker");
            RequireComponent<PrototypeWorldReactionMarker>("El Respiro Pablo watched marker");
            RequireComponent<PrototypeWorldReactionMarker>("Barrio reaction to front marker");
            RequireComponent<PrototypeInteractable>("Dirty cash seizure failstate");
            RequireComponent<PrototypeWorldReactionMarker>("Seized cash partial failure marker");
            RequireComponent<PrototypeRouteProgress>("Phase 1 Route Progress");
            RequireObject("Narrow asphalt route");
            RequireObject("Tight corner block");
            RequireObject("Motor proof low step");
            RequireObject("Motor proof high wall");
            RequireObject("Motor proof steep slope");
            RequireObject("Tight camera recovery wall");
            RequireObject("Safe return marker");
            RequireObject("Fallback Exit Point");
            RequireRouteCheckpoint(0, "Start on foot");
            RequireRouteCheckpoint(1, "Enter vehicle lane");
            RequireRouteCheckpoint(2, "Patrol pressure turn");
            RequireRouteCheckpoint(3, "Workshop interaction stop");
            RequireRouteCheckpoint(4, "Safe return");

            Assert.That(Camera.main, Is.Not.Null);
        }

        [Test]
        public void PlayerUsesCuratedAiCharacterVisualWithoutGameplayColliders()
        {
            EditorSceneManager.OpenScene(ScenePath);

            var player = RequireObject("Pablo Valera Prototype Controller");
            var rootRenderer = player.GetComponent<Renderer>();
            Assert.That(rootRenderer == null || rootRenderer.enabled == false, Is.True, "The old capsule must become invisible once the generated character model is mounted.");

            var presentation = player.GetComponent<PrototypeCharacterPresentation>();
            Assert.That(presentation, Is.Not.Null, "The player needs a presentation layer separate from the kinematic motor.");

            var visual = player.transform.Find("Pablo Character Visual");
            Assert.That(visual, Is.Not.Null, "The generated character model must be mounted as a visual child, not replace the motor root.");
            Assert.That(visual.gameObject.layer, Is.EqualTo(PrototypeLayers.Player));
            Assert.That(presentation.VisualRoot, Is.EqualTo(visual));

            var meshInstance = visual.Find("PabloValera_HumanoidCandidate Visual Mesh");
            Assert.That(meshInstance, Is.Not.Null, "The Humanoid Unity AI candidate should be the visible player mesh.");
            Assert.That(meshInstance.GetComponentsInChildren<Renderer>(true).Length, Is.GreaterThan(0));
            Assert.That(meshInstance.GetComponentsInChildren<SkinnedMeshRenderer>(true).Length, Is.GreaterThan(0), "The active Pablo visual should be skinned to the Humanoid rig.");
            var animator = meshInstance.GetComponentInChildren<Animator>(true);
            Assert.That(animator, Is.Not.Null);
            Assert.That(animator.avatar, Is.Not.Null);
            Assert.That(animator.avatar.isValid, Is.True);
            Assert.That(animator.avatar.isHuman, Is.True);
            Assert.That(animator.runtimeAnimatorController, Is.EqualTo(AssetDatabase.LoadAssetAtPath<AnimatorController>(PabloHumanoidRuntimeAnimatorPath)));

            foreach (var collider in visual.GetComponentsInChildren<Collider>(true))
            {
                Assert.That(collider.enabled == false || collider.isTrigger, Is.True, $"Visual collider {collider.name} must not affect player movement, vehicle exits, or camera collision.");
            }
        }

        [Test]
        public void CuratedAiCharacterAssetIsLightweightStaticVisual()
        {
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(MaleCrimeDramaPrefabPath);
            Assert.That(prefab, Is.Not.Null, "Missing curated Unity AI character prefab.");
            Assert.That(prefab.GetComponentsInChildren<Renderer>(true).Length, Is.GreaterThan(0));
            Assert.That(prefab.GetComponentsInChildren<SkinnedMeshRenderer>(true).Length, Is.EqualTo(0), "This generated drop is a static mesh and should not be treated as a rigged character yet.");
            Assert.That(prefab.GetComponentsInChildren<Collider>(true).Length, Is.EqualTo(0), "Generated visual assets must not bring gameplay colliders.");

            var glb = new FileInfo("Assets/Models/Characters/MaleCrimeDrama_Assets/selected.glb");
            Assert.That(glb.Exists, Is.True);
            Assert.That(glb.Length, Is.LessThan(2_000_000), "Prototype character visual should stay lightweight until the real rigged model pass.");
        }

        [Test]
        public void PabloValeraV2GeneratedAssetIsSkinnedRigCandidate()
        {
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(PabloValeraV2PrefabPath);
            Assert.That(prefab, Is.Not.Null, "Missing Pablo Valera V2 generated prefab.");
            Assert.That(prefab.GetComponentsInChildren<Renderer>(true).Length, Is.GreaterThan(0));
            Assert.That(prefab.GetComponentsInChildren<SkinnedMeshRenderer>(true).Length, Is.GreaterThan(0), "Pablo V2 should be a skinned mesh, not another static placeholder.");
            Assert.That(prefab.GetComponentsInChildren<Collider>(true).Length, Is.EqualTo(0), "Generated playable visuals must not bring gameplay colliders.");
            Assert.That(prefab.GetComponentsInChildren<Rigidbody>(true).Length, Is.EqualTo(0), "Generated playable visuals must not bring physics bodies.");
            Assert.That(prefab.GetComponentsInChildren<Transform>(true).Length, Is.GreaterThanOrEqualTo(35), "The rig candidate should preserve the generated skeleton hierarchy.");

            var glb = new FileInfo(PabloValeraV2GlbPath);
            Assert.That(glb.Exists, Is.True);
            Assert.That(glb.Length, Is.GreaterThan(10_000_000), "Pablo V2 should be the richer generated character, not the previous tiny placeholder.");
            Assert.That(glb.Length, Is.LessThan(90_000_000), "Keep generated character assets below GitHub's hard file limit until an LFS policy is added.");
        }

        [Test]
        public void PabloAvatarDefinitionUsesHumanoidCandidateAsRuntimeVisual()
        {
            var definitionType = System.Type.GetType("ValleDePlata.Prototype.PrototypeAvatarDefinition, ValleDePlata.Prototype");
            Assert.That(definitionType, Is.Not.Null, "PrototypeAvatarDefinition type is missing.");

            var definition = AssetDatabase.LoadAssetAtPath<ScriptableObject>("Assets/Settings/PabloPrototypeAvatar.asset");
            Assert.That(definition, Is.Not.Null, "Pablo needs an avatar definition asset so the generated model is not hard-coded as the final character.");
            Assert.That(definition.GetType(), Is.EqualTo(definitionType));

            var serialized = new SerializedObject(definition);
            Assert.That(serialized.FindProperty("characterId").stringValue, Is.EqualTo("pablo-valera"));
            Assert.That(serialized.FindProperty("displayName").stringValue, Is.EqualTo("Pablo Valera"));
            Assert.That(serialized.FindProperty("isFinalIdentityLocked").boolValue, Is.False, "The Humanoid candidate is animation-ready source, not final locked identity.");
            Assert.That(serialized.FindProperty("fullBodyPrefab").objectReferenceValue, Is.EqualTo(AssetDatabase.LoadAssetAtPath<GameObject>(PabloHumanoidCandidatePrefabPath)));
            Assert.That(serialized.FindProperty("runtimeAnimatorController").objectReferenceValue, Is.EqualTo(AssetDatabase.LoadAssetAtPath<AnimatorController>(PabloHumanoidRuntimeAnimatorPath)));
            var fullBodyInstanceName = serialized.FindProperty("fullBodyInstanceName");
            Assert.That(fullBodyInstanceName, Is.Not.Null, "Avatar definition should own the scene instance name so old generated meshes can be replaced safely.");
            Assert.That(fullBodyInstanceName.stringValue, Is.EqualTo("PabloValera_HumanoidCandidate Visual Mesh"));
            Assert.That(serialized.FindProperty("fullBodyLocalScale").floatValue, Is.EqualTo(1.8f).Within(0.001f));

            var validate = definitionType.GetMethod("Validate", BindingFlags.Public | BindingFlags.Instance);
            Assert.That(validate, Is.Not.Null);
            var args = new object[] { null };
            Assert.That((bool)validate.Invoke(definition, args), Is.True, args[0] as string);
        }

        [Test]
        public void Phase1PlayerUsesAvatarViewBackedByDefinition()
        {
            EditorSceneManager.OpenScene(ScenePath);

            var presentation = RequireComponent<PrototypeCharacterPresentation>("Pablo Valera Prototype Controller");
            var visual = RequireObject("Pablo Character Visual");
            Assert.That(HasComponentNamed(visual, "PrototypeAvatarView"), Is.True, "The player visual root needs an avatar view so the mesh can be swapped later.");

            var avatarView = visual.GetComponents<MonoBehaviour>().First(component => component != null && component.GetType().Name == "PrototypeAvatarView");
            var viewObject = new SerializedObject(avatarView);
            var definition = viewObject.FindProperty("avatarDefinition").objectReferenceValue;
            var fullBodyRoot = viewObject.FindProperty("fullBodyRoot").objectReferenceValue as Transform;

            Assert.That(definition, Is.Not.Null);
            Assert.That(AssetDatabase.GetAssetPath(definition), Is.EqualTo("Assets/Settings/PabloPrototypeAvatar.asset"));
            Assert.That(fullBodyRoot, Is.Not.Null);
            Assert.That(fullBodyRoot.name, Is.EqualTo("PabloValera_HumanoidCandidate Visual Mesh"));
            Assert.That(fullBodyRoot.localScale, Is.EqualTo(Vector3.one * 1.8f));
            Assert.That(fullBodyRoot.GetComponentsInChildren<SkinnedMeshRenderer>(true).Length, Is.GreaterThan(0));
            var animator = fullBodyRoot.GetComponentInChildren<Animator>(true);
            Assert.That(animator, Is.Not.Null);
            Assert.That(animator.avatar, Is.Not.Null);
            Assert.That(animator.avatar.isValid, Is.True);
            Assert.That(animator.avatar.isHuman, Is.True);
            Assert.That(animator.runtimeAnimatorController, Is.EqualTo(AssetDatabase.LoadAssetAtPath<AnimatorController>(PabloHumanoidRuntimeAnimatorPath)));

            var presentationObject = new SerializedObject(presentation);
            Assert.That(presentationObject.FindProperty("avatarView").objectReferenceValue, Is.EqualTo(avatarView));

            foreach (var collider in visual.GetComponentsInChildren<Collider>(true))
            {
                Assert.That(collider.enabled == false || collider.isTrigger, Is.True, $"Avatar visual collider {collider.name} must not affect gameplay physics.");
            }
        }

        [Test]
        public void PabloAvatarDefinitionDocumentsRuntimeReadinessAndCustomizationPlan()
        {
            var definition = AssetDatabase.LoadAssetAtPath<ScriptableObject>("Assets/Settings/PabloPrototypeAvatar.asset");
            Assert.That(definition, Is.Not.Null);

            var serialized = new SerializedObject(definition);
            var runtimeReadiness = serialized.FindProperty("runtimeReadiness");
            var rigReadiness = serialized.FindProperty("rigReadiness");
            var animationReadiness = serialized.FindProperty("animationReadiness");
            var rigDecision = serialized.FindProperty("rigDecision");
            var supportsRuntimeCustomization = serialized.FindProperty("supportsRuntimeCustomization");
            var plannedSlots = serialized.FindProperty("plannedCustomizationSlots");

            Assert.That(runtimeReadiness, Is.Not.Null, "Avatar definition must say whether a generated mesh is playable, rigged, or custom-ready.");
            Assert.That(rigReadiness, Is.Not.Null, "Avatar definition must explicitly track rig readiness before animation work begins.");
            Assert.That(animationReadiness, Is.Not.Null, "Avatar definition must distinguish placeholder Animator setup from real locomotion animation.");
            Assert.That(rigDecision, Is.Not.Null, "Avatar definition must record the 0.5 rig decision so future asset generation does not guess.");
            Assert.That(supportsRuntimeCustomization, Is.Not.Null, "Avatar definition must not imply future customization until slots are real.");
            Assert.That(plannedSlots, Is.Not.Null, "Avatar definition should name planned slots even while the current mesh is full-body.");

            Assert.That(runtimeReadiness.enumValueIndex, Is.EqualTo(2), "Pablo's active runtime visual should now be the rigged Humanoid candidate.");
            Assert.That(rigReadiness.enumValueIndex, Is.EqualTo(2), "The active candidate needs a validated Humanoid Avatar before runtime animation wiring.");
            Assert.That(animationReadiness.enumValueIndex, Is.EqualTo(2), "Runtime locomotion should be driven by the game-owned Animator bridge.");
            Assert.That(rigDecision.enumValueIndex, Is.EqualTo(2), "The Humanoid source should be accepted for controlled locomotion integration.");
            Assert.That(supportsRuntimeCustomization.boolValue, Is.False);
            Assert.That(plannedSlots.arraySize, Is.GreaterThanOrEqualTo(6), "We need the customization direction recorded before replacing the placeholder.");

            var summaryMethod = definition.GetType().GetMethod("BuildAuthoringSummary", BindingFlags.Public | BindingFlags.Instance);
            Assert.That(summaryMethod, Is.Not.Null, "Avatar definition needs a concise authoring summary for future Unity AI/model passes.");
            var summary = summaryMethod.Invoke(definition, null) as string;
            Assert.That(summary, Does.Contain("runtime Humanoid"));
            Assert.That(summary, Does.Contain("Animator bridge"));
            Assert.That(summary, Does.Contain("customization slots"));
        }

        [Test]
        public void PabloHumanoidRuntimeAnimatorControllerHasBridgeContract()
        {
            var controller = AssetDatabase.LoadAssetAtPath<AnimatorController>(PabloHumanoidRuntimeAnimatorPath);
            Assert.That(controller, Is.Not.Null, "Runtime integration owns a controller separate from the generated source prefab.");

            AssertAnimatorParameter(controller, "Speed", AnimatorControllerParameterType.Float);
            AssertAnimatorParameter(controller, "IsSprinting", AnimatorControllerParameterType.Bool);
            AssertAnimatorParameter(controller, "Grounded", AnimatorControllerParameterType.Bool);

            var states = controller.layers[0].stateMachine.states.Select(state => state.state).ToArray();
            CollectionAssert.AreEquivalent(new[] { "Idle", "Walk", "Run", "Sprint" }, states.Select(state => state.name).ToArray());
            foreach (var state in states)
            {
                var clip = state.motion as AnimationClip;
                Assert.That(clip, Is.Not.Null, $"{state.name} should use a runtime-owned Humanoid locomotion clip.");
                Assert.That(clip!.name, Is.EqualTo($"PabloValera_Runtime_{state.name}"));
                Assert.That(AnimationUtility.GetCurveBindings(clip).Length, Is.GreaterThan(0), $"{clip.name} must contain real animation curves.");
            }
        }

        [Test]
        public void PabloHumanoidRuntimeAnimatorMasksUnapprovedUpperBodySourceMotion()
        {
            var controller = AssetDatabase.LoadAssetAtPath<AnimatorController>(PabloHumanoidRuntimeAnimatorPath);
            var mask = AssetDatabase.LoadAssetAtPath<AvatarMask>(PabloHumanoidLowerBodyMaskPath);
            Assert.That(controller, Is.Not.Null);
            Assert.That(mask, Is.Not.Null, "Runtime locomotion should quarantine AI-generated upper-body motion until visual QA accepts it.");

            var layer = controller.layers[0];
            Assert.That(layer.avatarMask, Is.EqualTo(mask), "Pablo runtime locomotion must use the lower-body safety mask.");
            Assert.That(mask.GetHumanoidBodyPartActive(AvatarMaskBodyPart.LeftLeg), Is.True);
            Assert.That(mask.GetHumanoidBodyPartActive(AvatarMaskBodyPart.RightLeg), Is.True);
            Assert.That(mask.GetHumanoidBodyPartActive(AvatarMaskBodyPart.Body), Is.True);
            Assert.That(mask.GetHumanoidBodyPartActive(AvatarMaskBodyPart.LeftArm), Is.False);
            Assert.That(mask.GetHumanoidBodyPartActive(AvatarMaskBodyPart.RightArm), Is.False);
            Assert.That(mask.GetHumanoidBodyPartActive(AvatarMaskBodyPart.LeftFingers), Is.False);
            Assert.That(mask.GetHumanoidBodyPartActive(AvatarMaskBodyPart.RightFingers), Is.False);
            Assert.That(mask.GetHumanoidBodyPartActive(AvatarMaskBodyPart.Head), Is.False);
        }

        [Test]
        public void PabloHumanoidRuntimeAnimatorUsesUpperBodySanitizedClips()
        {
            var controller = AssetDatabase.LoadAssetAtPath<AnimatorController>(PabloHumanoidRuntimeAnimatorPath);
            Assert.That(controller, Is.Not.Null);

            var states = controller.layers[0].stateMachine.states.Select(state => state.state).ToArray();
            foreach (var stateName in PabloRuntimeLocomotionStates)
            {
                var state = states.FirstOrDefault(candidate => candidate.name == stateName);
                Assert.That(state, Is.Not.Null, $"Missing runtime locomotion state {stateName}.");

                var clip = state.motion as AnimationClip;
                Assert.That(clip, Is.Not.Null, $"{stateName} needs a runtime-owned clip, not a null state.");
                var clipPath = AssetDatabase.GetAssetPath(clip);
                Assert.That(clipPath, Does.EndWith($"PabloValera_Runtime_{stateName}.anim"), $"{stateName} must not bind directly to the Unity AI full-body source clip.");
                Assert.That(AnimationUtility.GetCurveBindings(clip).Any(IsPabloUpperBodyCurve), Is.False, $"{clip.name} still contains upper-body curves that pull Pablo's arms/head into the broken pose.");
                Assert.That(AnimationUtility.GetCurveBindings(clip).Any(binding => binding.propertyName.Contains("Leg") || binding.propertyName.Contains("Foot")), Is.True, $"{clip.name} should keep lower-body locomotion curves.");
            }
        }

        [Test]
        public void PabloValeraV2AnimatorControllerHasExpectedPlaceholderContract()
        {
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(PabloValeraV2PrefabPath);
            var controller = AssetDatabase.LoadAssetAtPath<AnimatorController>(PabloValeraV2AnimatorPath);
            Assert.That(prefab, Is.Not.Null);
            Assert.That(controller, Is.Not.Null, "Unity AI should prepare an Animator Controller asset for Pablo V2.");

            var animator = prefab.GetComponentInChildren<Animator>(true);
            Assert.That(animator, Is.Not.Null, "Pablo V2 prefab should carry an Animator component for future runtime locomotion.");
            Assert.That(animator.runtimeAnimatorController, Is.EqualTo(controller));
            Assert.That(animator.avatar, Is.Null, "The GLB importer kept this as Generic; do not pretend Humanoid retargeting is ready.");
            Assert.That(animator.applyRootMotion, Is.False);

            AssertAnimatorParameter(controller, "Speed", AnimatorControllerParameterType.Float);
            AssertAnimatorParameter(controller, "IsSprinting", AnimatorControllerParameterType.Bool);
            AssertAnimatorParameter(controller, "Grounded", AnimatorControllerParameterType.Bool);

            var expectedStates = new[] { "Idle", "Walk", "Run", "Sprint" };
            var states = controller.layers[0].stateMachine.states.Select(state => state.state).ToArray();
            CollectionAssert.AreEquivalent(expectedStates, states.Select(state => state.name).ToArray());
            foreach (var state in states)
            {
                var clip = state.motion as AnimationClip;
                Assert.That(clip, Is.Not.Null, $"{state.name} should use a safe placeholder clip until real locomotion exists.");
                Assert.That(clip!.name, Is.EqualTo($"Placeholder_{state.name}"));
                Assert.That(AnimationUtility.GetCurveBindings(clip).Length, Is.EqualTo(0), $"{clip.name} must not deform the mesh.");
                Assert.That(AnimationUtility.GetObjectReferenceCurveBindings(clip).Length, Is.EqualTo(0), $"{clip.name} must not swap assets.");
            }
        }

        [Test]
        public void AvatarReadinessAuditClassifiesHumanoidCandidateAsRuntimeReady()
        {
            var analyzerType = System.Type.GetType("ValleDePlata.Prototype.PrototypeAvatarReadiness, ValleDePlata.Prototype");
            Assert.That(analyzerType, Is.Not.Null, "PrototypeAvatarReadiness type is missing.");

            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(PabloHumanoidCandidatePrefabPath);
            var definition = AssetDatabase.LoadAssetAtPath<ScriptableObject>("Assets/Settings/PabloPrototypeAvatar.asset");
            Assert.That(prefab, Is.Not.Null);
            Assert.That(definition, Is.Not.Null);

            var analyze = analyzerType.GetMethod("AnalyzePrefab", BindingFlags.Public | BindingFlags.Static);
            Assert.That(analyze, Is.Not.Null, "Avatar readiness needs a prefab analyzer so generated assets are curated, not trusted blindly.");

            var report = analyze.Invoke(null, new object[] { prefab, definition });
            Assert.That((int)report.GetType().GetProperty("RendererCount")!.GetValue(report), Is.GreaterThan(0));
            Assert.That((int)report.GetType().GetProperty("SkinnedMeshRendererCount")!.GetValue(report), Is.GreaterThan(0));
            var skeletonCount = report.GetType().GetProperty("SkeletonTransformCount");
            Assert.That(skeletonCount, Is.Not.Null, "Readiness report should expose skeleton transform count for AI-rigged candidates.");
            Assert.That((int)skeletonCount!.GetValue(report), Is.GreaterThanOrEqualTo(35));
            var hasAnimatorController = report.GetType().GetProperty("HasAnimatorController");
            Assert.That(hasAnimatorController, Is.Not.Null, "Readiness report should know whether the prefab has an Animator Controller.");
            Assert.That((bool)hasAnimatorController!.GetValue(report), Is.True);
            var clipCount = report.GetType().GetProperty("AnimationClipCount");
            Assert.That(clipCount, Is.Not.Null, "Readiness report should count placeholder clips separately from real animation readiness.");
            Assert.That((int)clipCount!.GetValue(report), Is.EqualTo(4));
            var placeholderOnly = report.GetType().GetProperty("UsesPlaceholderAnimationOnly");
            Assert.That(placeholderOnly, Is.Not.Null, "Readiness report should flag Unity AI placeholder clips.");
            Assert.That((bool)placeholderOnly!.GetValue(report), Is.False);
            Assert.That((int)report.GetType().GetProperty("GameplayColliderCount")!.GetValue(report), Is.EqualTo(0));
            var isSkinnedCandidate = report.GetType().GetProperty("IsSkinnedRigCandidate");
            Assert.That(isSkinnedCandidate, Is.Not.Null, "Readiness report should distinguish skinned candidates from static placeholders.");
            Assert.That((bool)isSkinnedCandidate!.GetValue(report), Is.False);
            Assert.That((bool)report.GetType().GetProperty("RequiresRiggingBeforeAnimation")!.GetValue(report), Is.False);
            Assert.That((bool)report.GetType().GetProperty("SupportsRuntimeCustomization")!.GetValue(report), Is.False);
            Assert.That(report.ToString(), Does.Contain("runtime Humanoid"));
        }

        [Test]
        public void AvatarRigDecisionAcceptsHumanoidCandidateForRuntimeLocomotion()
        {
            var analyzerType = System.Type.GetType("ValleDePlata.Prototype.PrototypeAvatarReadiness, ValleDePlata.Prototype");
            var decisionPolicyType = System.Type.GetType("ValleDePlata.Prototype.PrototypeAvatarRigDecisionPolicy, ValleDePlata.Prototype");
            Assert.That(analyzerType, Is.Not.Null);
            Assert.That(decisionPolicyType, Is.Not.Null, "0.5 needs a policy object that turns the audit into a production decision.");

            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(PabloHumanoidCandidatePrefabPath);
            var definition = AssetDatabase.LoadAssetAtPath<ScriptableObject>("Assets/Settings/PabloPrototypeAvatar.asset");
            var analyze = analyzerType!.GetMethod("AnalyzePrefab", BindingFlags.Public | BindingFlags.Static);
            var report = analyze!.Invoke(null, new object[] { prefab, definition });

            Assert.That(report.GetType().GetProperty("HasAnimatorAvatar"), Is.Not.Null, "Readiness should distinguish Animator component from a real Avatar.");
            Assert.That((bool)report.GetType().GetProperty("HasAnimatorAvatar")!.GetValue(report), Is.True);
            Assert.That((bool)report.GetType().GetProperty("HasValidHumanoidAvatar")!.GetValue(report), Is.True);

            var decide = decisionPolicyType!.GetMethod("Decide", BindingFlags.Public | BindingFlags.Static);
            Assert.That(decide, Is.Not.Null);
            var decision = decide!.Invoke(null, new[] { report });
            Assert.That(decision, Is.Not.Null);

            var decisionName = decision!.GetType().GetProperty("Decision")!.GetValue(decision)!.ToString();
            var reason = decision.GetType().GetProperty("Reason")!.GetValue(decision) as string;
            var unityAiScope = decision.GetType().GetProperty("UnityAiScope")!.GetValue(decision) as string;
            var shouldUseUnityAi = (bool)decision.GetType().GetProperty("ShouldUseUnityAiForNextAssetPass")!.GetValue(decision);

            Assert.That(decisionName, Is.EqualTo("ReadyForHumanoidLocomotion"));
            Assert.That(reason, Does.Contain("valid Humanoid Avatar").And.Contain("non-placeholder animation clips"));
            Assert.That(unityAiScope, Does.Contain("controlled animation integration"));
            Assert.That(shouldUseUnityAi, Is.False);
        }

        [Test]
        public void CharacterAvatarPass05ReportRecordsHumanoidDecisionAndUnityAiBoundary()
        {
            Assert.That(File.Exists(PabloAvatarPass05ReportPath), Is.True, "0.5 needs a written decision report before asking Unity AI for more asset work.");
            var report = File.ReadAllText(PabloAvatarPass05ReportPath);

            Assert.That(report, Does.Contain("# Character/Avatar Pass 0.5"));
            Assert.That(report, Does.Contain("Decision: KeepVisualRequestHumanoidSource"));
            Assert.That(report, Does.Contain("Current status: Pablo V2 is a Generic GLB"));
            Assert.That(report, Does.Contain("Unity AI Assistant"));
            Assert.That(report, Does.Contain("Do not edit gameplay scripts"));
            Assert.That(report, Does.Contain("Humanoid-native"));
        }

        [Test]
        public void PabloHumanoidCandidateIsRealSkinnedHumanoidSourceAsset()
        {
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(PabloHumanoidCandidatePrefabPath);
            var avatar = AssetDatabase.LoadAssetAtPath<Avatar>(PabloHumanoidCandidateAvatarPath);
            Assert.That(prefab, Is.Not.Null, "Unity AI Humanoid candidate prefab is missing.");
            Assert.That(avatar, Is.Not.Null, "Unity AI Humanoid candidate Avatar asset is missing.");

            var animator = prefab.GetComponentInChildren<Animator>(true);
            Assert.That(animator, Is.Not.Null, "Humanoid candidate needs an Animator on the prefab root.");
            Assert.That(animator.avatar, Is.EqualTo(avatar));
            Assert.That(animator.avatar.isValid, Is.True, "Humanoid candidate Avatar must be valid before any runtime integration.");
            Assert.That(animator.avatar.isHuman, Is.True, "Humanoid candidate Avatar must be Human/Humanoid-compatible.");
            Assert.That(animator.runtimeAnimatorController, Is.Null, "The source candidate should not bring a runtime controller; game integration owns controller wiring.");

            var skinned = prefab.GetComponentsInChildren<SkinnedMeshRenderer>(true);
            Assert.That(skinned.Length, Is.GreaterThan(0), "Humanoid candidate must use SkinnedMeshRenderer, not a static mesh with an Avatar nearby.");
            Assert.That(prefab.GetComponentsInChildren<MeshRenderer>(true).Length, Is.EqualTo(0), "Visible body must not be MeshRenderer-based.");
            Assert.That(prefab.GetComponentsInChildren<MeshFilter>(true).Length, Is.EqualTo(0), "Visible body must not be MeshFilter-based.");
            Assert.That(prefab.GetComponentsInChildren<Rigidbody>(true).Length, Is.EqualTo(0), "Generated visual source must not bring gameplay rigidbodies.");
            Assert.That(prefab.GetComponentsInChildren<Collider>(true).Length, Is.EqualTo(0), "Generated visual source must not bring gameplay colliders.");
        }

        [Test]
        public void PabloHumanoidCandidateProvidesRealLocomotionSourceClips()
        {
            var clipGuids = AssetDatabase.FindAssets("t:AnimationClip", new[] { PabloHumanoidCandidateAnimationsPath });
            var clips = clipGuids
                .Select(guid => AssetDatabase.LoadAssetAtPath<AnimationClip>(AssetDatabase.GUIDToAssetPath(guid)))
                .Where(clip => clip != null)
                .Where(clip => PabloRuntimeLocomotionStates.Contains(clip!.name))
                .ToArray();

            CollectionAssert.AreEquivalent(PabloRuntimeLocomotionStates, clips.Select(clip => clip.name));
            foreach (var clip in clips)
            {
                Assert.That(clip!.legacy, Is.False, $"{clip.name} must be a Mecanim clip.");
                Assert.That(AnimationUtility.GetCurveBindings(clip).Length, Is.GreaterThan(0), $"{clip.name} must contain real animation curves.");
                Assert.That(AnimationUtility.GetObjectReferenceCurveBindings(clip).Length, Is.EqualTo(0), $"{clip.name} should not swap assets.");
            }
        }

        [Test]
        public void CharacterPresentationStateTracksIdleWalkSprint()
        {
            Assert.That(PrototypeCharacterPresentation.ResolveLocomotionState(true, 0f, false, 6.4f), Is.EqualTo(PrototypeCharacterLocomotionState.Idle));
            Assert.That(PrototypeCharacterPresentation.ResolveLocomotionState(true, 2.2f, false, 6.4f), Is.EqualTo(PrototypeCharacterLocomotionState.Walk));
            Assert.That(PrototypeCharacterPresentation.ResolveLocomotionState(true, 5.9f, true, 6.4f), Is.EqualTo(PrototypeCharacterLocomotionState.Sprint));
            Assert.That(PrototypeCharacterPresentation.ResolveLocomotionState(false, 5.9f, true, 6.4f), Is.EqualTo(PrototypeCharacterLocomotionState.Hidden));
        }

        [Test]
        public void UnityAiGenerationToolingStaysAvailableForEditorAssetWork()
        {
            var manifest = File.ReadAllText("Packages/manifest.json");
            var packagesLock = File.ReadAllText("Packages/packages-lock.json");

            Assert.That(manifest, Does.Contain("com.unity.ai.assistant"), "Unity AI Assistant should stay installed because we are using it as an editor asset-generation tool.");
            Assert.That(packagesLock, Does.Contain("com.unity.cloud.gltfast"), "The curated GLB model needs Unity's glTF importer dependency resolved in the package lock.");
            Assert.That(manifest, Does.Not.Contain("com.unity.ai.inference"), "The editor Assistant should not force the runtime inference stack into playable builds.");
        }

        [Test]
        public void ValleDePlataStreetKitContainsCuratedPlayablePrefabSet()
        {
            foreach (var prefabName in ValleDePlataStreetKitStructuralPrefabs)
            {
                var prefab = LoadStreetKitPrefab(prefabName);
                Assert.That(prefab.GetComponentsInChildren<Renderer>(true).Length, Is.GreaterThan(0), $"{prefabName} needs visible geometry.");
                Assert.That(prefab.GetComponentsInChildren<MonoBehaviour>(true).Length, Is.EqualTo(0), $"{prefabName} must stay asset-only with no generated gameplay scripts.");
                Assert.That(prefab.GetComponentsInChildren<Rigidbody>(true).Length, Is.EqualTo(0), $"{prefabName} must not bring runtime physics bodies.");
                Assert.That(AllGameObjectsUseLayer(prefab, PrototypeLayers.WorldStatic), Is.True, $"{prefabName} must be on WorldStatic so camera/motor collision masks stay intentional.");

                var blockingColliders = prefab.GetComponentsInChildren<Collider>(true).Where(collider => collider.enabled && !collider.isTrigger).ToArray();
                Assert.That(blockingColliders.Length, Is.GreaterThan(0), $"{prefabName} is structural and needs simple blocking collision.");
                Assert.That(blockingColliders.All(collider => collider is BoxCollider), Is.True, $"{prefabName} should use simple BoxColliders only.");
            }

            foreach (var prefabName in ValleDePlataStreetKitDressingPrefabs)
            {
                var prefab = LoadStreetKitPrefab(prefabName);
                Assert.That(prefab.GetComponentsInChildren<Renderer>(true).Length, Is.GreaterThan(0), $"{prefabName} needs visible geometry.");
                Assert.That(prefab.GetComponentsInChildren<MonoBehaviour>(true).Length, Is.EqualTo(0), $"{prefabName} must stay asset-only with no generated gameplay scripts.");
                Assert.That(prefab.GetComponentsInChildren<Rigidbody>(true).Length, Is.EqualTo(0), $"{prefabName} must not bring runtime physics bodies.");
                Assert.That(AllGameObjectsUseLayer(prefab, PrototypeLayers.CameraIgnore), Is.True, $"{prefabName} is dressing and should not shorten camera distance or block exit checks.");
                Assert.That(prefab.GetComponentsInChildren<Collider>(true).Any(collider => collider.enabled && !collider.isTrigger), Is.False, $"{prefabName} dressing collision must be disabled until intentionally promoted.");
            }
        }

        [Test]
        public void ValleDePlataStreetKitSampleBlockStaysOutOfPhase1RuntimeScene()
        {
            var sample = AssetDatabase.LoadAssetAtPath<GameObject>($"{ValleDePlataStreetKitPath}/VDP_StreetKit_SampleBlock.prefab");
            Assert.That(sample, Is.Not.Null, "The generated kit needs a reusable sample block asset.");
            Assert.That(sample.GetComponentsInChildren<Renderer>(true).Length, Is.GreaterThan(0));

            EditorSceneManager.OpenScene(ScenePath);
            Assert.That(GameObject.Find("VDP_StreetKit_SampleBlock"), Is.Null, "AI preview blocks should stay out of the authored Phase 1 gameplay scene until the scene builder owns placement.");
            Assert.That(GameObject.Find("VDP_Balcony_01"), Is.Null, "Standalone AI preview props should not remain as root objects in the gameplay scene.");
        }

        [Test]
        public void Phase1SceneContainsBelievabilityLandmarksThatDoNotBlockGameplay()
        {
            EditorSceneManager.OpenScene(ScenePath);

            var barrioSign = RequireNonBlockingDressing("Barrio Hondo overhead street sign");
            var safeReturnArch = RequireNonBlockingDressing("Safe return alley arch");
            RequireNonBlockingDressing("Safe return painted arrow");
            RequireNonBlockingDressing("Laundry line north");
            RequireNonBlockingDressing("Witness balcony cluster");
            var riosDesk = RequireNonBlockingDressing("Rios checkpoint desk");
            RequireNonBlockingDressing("Rios checkpoint awning");
            var roadblockLeft = RequireNonBlockingDressing("Police roadblock barricade left");
            var roadblockRight = RequireNonBlockingDressing("Police roadblock barricade right");
            var workshopSign = RequireNonBlockingDressing("El Respiro workshop sign");
            RequireNonBlockingDressing("Rooftop water tank");
            RequireNonBlockingDressing("Barrio crate stack");

            Assert.That(safeReturnArch.transform.position.z, Is.LessThan(-6f));
            Assert.That(barrioSign.transform.position.z, Is.LessThan(-4f));
            Assert.That(riosDesk.transform.position.z, Is.InRange(20f, 24.5f));
            Assert.That(roadblockLeft.transform.position.z, Is.InRange(23f, 26f));
            Assert.That(roadblockRight.transform.position.z, Is.InRange(23f, 26f));
            Assert.That(workshopSign.transform.position.z, Is.GreaterThan(44f));
        }

        [Test]
        public void Phase1SceneContainsPlayerFacingPresentationLayer()
        {
            EditorSceneManager.OpenScene(ScenePath);

            var hud = RequireObject("Prototype Player HUD");
            Assert.That(HasComponentNamed(hud, "PrototypePlayerHud"), Is.True, "Scene needs a player-facing HUD separate from the debug dump.");

            var debugHud = RequireComponent<PrototypeDebugHud>("Prototype Debug HUD");
            var debugVisible = new SerializedObject(debugHud).FindProperty("visible");
            Assert.That(debugVisible, Is.Not.Null);
            Assert.That(debugVisible.boolValue, Is.False, "Debug HUD should not be the default presentation layer for the playable slice.");

            RequireNonBlockingDressing("Left sunlit plaster facade");
            RequireNonBlockingDressing("Right faded teal facade");
            RequireNonBlockingDressing("Market awning strip");
            RequireNonBlockingDressing("Workshop plaster return");
            RequireNonBlockingDressing("Pressure road dust band");

            var fillLight = RequireComponent<Light>("Warm presentation fill light");
            Assert.That(fillLight.intensity, Is.GreaterThanOrEqualTo(0.45f));
            Assert.That(fillLight.shadows, Is.EqualTo(LightShadows.None));
        }

        [Test]
        public void PlayerHudFormatsObjectiveAndPromptWithoutDebugPrefixes()
        {
            var hudType = System.Type.GetType("ValleDePlata.Prototype.PrototypePlayerHud, ValleDePlata.Prototype");
            Assert.That(hudType, Is.Not.Null, "PrototypePlayerHud type is missing.");

            var objectiveMethod = hudType.GetMethod("BuildObjectiveLine", BindingFlags.Public | BindingFlags.Static);
            var promptMethod = hudType.GetMethod("BuildPromptLine", BindingFlags.Public | BindingFlags.Static);
            Assert.That(objectiveMethod, Is.Not.Null);
            Assert.That(promptMethod, Is.Not.Null);

            var objective = (string)objectiveMethod.Invoke(null, new object[] { "Objective: collect dirty cash at El Respiro" });
            var changed = (string)objectiveMethod.Invoke(null, new object[] { "Objective changed: escape the patrol pressure" });
            var prompt = (string)promptMethod.Invoke(null, new object[] { "Pay Rios bribe" });
            var nonePrompt = (string)promptMethod.Invoke(null, new object[] { "None" });

            Assert.That(objective, Is.EqualTo("Collect dirty cash at El Respiro"));
            Assert.That(changed, Is.EqualTo("Escape the patrol pressure"));
            Assert.That(prompt, Is.EqualTo("E / A  Pay Rios bribe"));
            Assert.That(nonePrompt, Is.EqualTo(string.Empty));
        }

        [Test]
        public void Phase1SceneGroupsReadableLandmarksIntoAuthoringProps()
        {
            EditorSceneManager.OpenScene(ScenePath);

            var propType = System.Type.GetType("ValleDePlata.Prototype.PrototypeReadableProp, ValleDePlata.Prototype");
            Assert.That(propType, Is.Not.Null, "PrototypeReadableProp type is missing.");

            RequireReadablePropGroup("Barrio street identity prop", 8);
            RequireReadablePropGroup("Safe return readable prop", 4);
            RequireReadablePropGroup("Rios checkpoint readable prop", 4);
            RequireReadablePropGroup("Police roadblock readable prop", 6);
            RequireReadablePropGroup("El Respiro readable prop", 6);
        }

        [Test]
        public void CursorControllerLocksAndRestoresMouseForPlayModeCamera()
        {
            var controllerType = System.Type.GetType("ValleDePlata.Prototype.PrototypeCursorController, ValleDePlata.Prototype");
            Assert.That(controllerType, Is.Not.Null, "PrototypeCursorController type is missing.");

            var resolveMethod = controllerType.GetMethod("ResolveCursorDecision", BindingFlags.Public | BindingFlags.Static);
            Assert.That(resolveMethod, Is.Not.Null);

            var initialLock = resolveMethod.Invoke(null, new object[] { true, true, false, true, false, CursorLockMode.None });
            AssertCursorDecision(initialLock, CursorLockMode.Locked, false);

            var escapeUnlock = resolveMethod.Invoke(null, new object[] { true, true, true, true, false, CursorLockMode.Locked });
            AssertCursorDecision(escapeUnlock, CursorLockMode.None, true);

            var clickRelock = resolveMethod.Invoke(null, new object[] { true, true, false, true, true, CursorLockMode.None });
            AssertCursorDecision(clickRelock, CursorLockMode.Locked, false);
        }

        [Test]
        public void Phase1SceneContainsCursorControllerForEditorPlaytests()
        {
            EditorSceneManager.OpenScene(ScenePath);

            RequireComponentByName("Prototype Cursor Controller", "PrototypeCursorController");
        }

        [Test]
        public void PerformanceSamplerReportsAverageWorstFrameAndStatus()
        {
            var samplerType = System.Type.GetType("ValleDePlata.Prototype.PrototypePerformanceSampler, ValleDePlata.Prototype");
            Assert.That(samplerType, Is.Not.Null, "PrototypePerformanceSampler type is missing.");

            var sampler = System.Activator.CreateInstance(samplerType);
            var recordMethod = samplerType.GetMethod("RecordFrame", BindingFlags.Public | BindingFlags.Instance);
            var averageFpsProperty = samplerType.GetProperty("AverageFps");
            var worstFrameMsProperty = samplerType.GetProperty("WorstFrameMs");
            var statusProperty = samplerType.GetProperty("Status");
            Assert.That(recordMethod, Is.Not.Null);
            Assert.That(averageFpsProperty, Is.Not.Null);
            Assert.That(worstFrameMsProperty, Is.Not.Null);
            Assert.That(statusProperty, Is.Not.Null);

            recordMethod.Invoke(sampler, new object[] { 1f / 60f });
            recordMethod.Invoke(sampler, new object[] { 1f / 30f });
            recordMethod.Invoke(sampler, new object[] { 1f / 20f });

            var averageFps = (float)averageFpsProperty.GetValue(sampler);
            var worstFrameMs = (float)worstFrameMsProperty.GetValue(sampler);
            var status = (string)statusProperty.GetValue(sampler);

            Assert.That(averageFps, Is.InRange(29f, 31f));
            Assert.That(worstFrameMs, Is.EqualTo(50f).Within(0.1f));
            Assert.That(status, Is.EqualTo("Frame spikes"));
        }

        [Test]
        public void Phase1SceneContainsPerformanceProbeForFeelGate()
        {
            EditorSceneManager.OpenScene(ScenePath);

            RequireComponentByName("Prototype Performance Probe", "PrototypePerformanceProbe");
        }

        [Test]
        public void PlayerHudFormatsPerformanceInStatusLine()
        {
            var hudType = System.Type.GetType("ValleDePlata.Prototype.PrototypePlayerHud, ValleDePlata.Prototype");
            Assert.That(hudType, Is.Not.Null, "PrototypePlayerHud type is missing.");

            var statusMethod = hudType.GetMethod("BuildStatusLine", BindingFlags.Public | BindingFlags.Static);
            Assert.That(statusMethod, Is.Not.Null);

            var status = (string)statusMethod.Invoke(null, new object[] { "Mode: Driving", "Pressure: Patrol", "FPS 58 | worst 24ms" });

            Assert.That(status, Is.EqualTo("Mode: Driving | Pressure: Patrol | FPS 58 | worst 24ms"));
        }

        [Test]
        public void Phase2EventsChangeWorldState()
        {
            var worldObject = new GameObject("World State Test");
            var world = worldObject.AddComponent<PrototypeWorldState>();

            world.ResetState();
            world.ApplyEvent(PrototypeWorldEvent.PublicViolenceCommitted);

            Assert.That(world.Fear, Is.EqualTo(SocialLevel.High));
            Assert.That(world.PeopleLove, Is.EqualTo(SocialLevel.Low));
            Assert.That(world.StatePressure, Is.EqualTo(PressureLevel.Medium));
            Assert.That(world.RuleStyleDecision, Is.EqualTo(RuleStyle.ShowOfForce));
            Assert.That(world.LastEvent, Is.EqualTo(PrototypeWorldEvent.PublicViolenceCommitted));
            Assert.That(PrototypeDebugState.World, Does.Contain("LastEvent: PublicViolenceCommitted"));

            world.ResetState();
            world.ApplyEvent(PrototypeWorldEvent.PublicViolenceCommitted);
            world.ApplyEvent(PrototypeWorldEvent.BribeAccepted);

            Assert.That(world.StatePressure, Is.EqualTo(PressureLevel.Low));
            Assert.That(world.DirtyCash, Is.EqualTo(DirtyCashState.Hidden));
            Assert.That(world.RuleStyleDecision, Is.EqualTo(RuleStyle.Bribe));
            Assert.That(world.LastEvent, Is.EqualTo(PrototypeWorldEvent.BribeAccepted));

            world.ResetState();
            world.ApplyEvent(PrototypeWorldEvent.MateoProtected);
            Assert.That(world.LieutenantTrust, Is.EqualTo(LieutenantTrust.Trusted));
            Assert.That(world.LastEvent, Is.EqualTo(PrototypeWorldEvent.MateoProtected));

            world.ResetState();
            world.ApplyEvent(PrototypeWorldEvent.MateoHumiliated);
            Assert.That(world.LieutenantTrust, Is.EqualTo(LieutenantTrust.Humiliated));
            Assert.That(world.LastEvent, Is.EqualTo(PrototypeWorldEvent.MateoHumiliated));

            Object.DestroyImmediate(worldObject);
        }

        [Test]
        public void Phase3WorldStateSnapshotRoundTripsThroughFile()
        {
            var worldObject = new GameObject("World State Snapshot Test");
            var world = worldObject.AddComponent<PrototypeWorldState>();
            var snapshotPath = Path.Combine(Path.GetTempPath(), "valle_de_plata_phase3_world_state_test.json");

            try
            {
                world.ResetState();
                world.ApplyEvent(PrototypeWorldEvent.PublicViolenceCommitted);
                world.ApplyEvent(PrototypeWorldEvent.BribeAccepted);
                world.SaveSnapshot(snapshotPath);

                Assert.That(File.Exists(snapshotPath), Is.True);
                Assert.That(File.ReadAllText(snapshotPath), Does.Contain("lastEvent"));

                world.ResetState();
                Assert.That(world.LastEvent, Is.EqualTo(PrototypeWorldEvent.None));

                world.LoadSnapshot(snapshotPath);

                Assert.That(world.DistrictId, Is.EqualTo("BarrioHondo"));
                Assert.That(world.FrontId, Is.EqualTo("ElRespiroWorkshop"));
                Assert.That(world.LastEvent, Is.EqualTo(PrototypeWorldEvent.BribeAccepted));
                Assert.That(world.DirtyCash, Is.EqualTo(DirtyCashState.Hidden));
                Assert.That(world.StatePressure, Is.EqualTo(PressureLevel.Low));
                Assert.That(world.RuleStyleDecision, Is.EqualTo(RuleStyle.Bribe));
                Assert.That(PrototypeDebugState.World, Does.Contain("LastEvent: BribeAccepted"));
            }
            finally
            {
                if (File.Exists(snapshotPath))
                {
                    File.Delete(snapshotPath);
                }

                Object.DestroyImmediate(worldObject);
            }
        }

        [Test]
        public void Phase4FrontEventsGiveDirtyCashAndMateoAStateCost()
        {
            var worldObject = new GameObject("Front State Test");
            var world = worldObject.AddComponent<PrototypeWorldState>();

            world.ResetState();
            world.ApplyEvent(PrototypeWorldEvent.DirtyCashPickedUp);

            Assert.That(world.DirtyCash, Is.EqualTo(DirtyCashState.Carried));
            Assert.That(world.StatePressure, Is.EqualTo(PressureLevel.Medium));
            Assert.That(world.LastEvent, Is.EqualTo(PrototypeWorldEvent.DirtyCashPickedUp));

            world.ApplyEvent(PrototypeWorldEvent.FrontTakenUnderWatch);

            Assert.That(world.FrontControl, Is.EqualTo(FrontControl.PabloWatched));
            Assert.That(world.DirtyCash, Is.EqualTo(DirtyCashState.Hidden));
            Assert.That(world.RuleStyleDecision, Is.EqualTo(RuleStyle.Favor));
            Assert.That(world.StatePressure, Is.EqualTo(PressureLevel.High));

            world.ResetState();
            world.ApplyEvent(PrototypeWorldEvent.MateoProtected);
            world.ApplyEvent(PrototypeWorldEvent.DirtyCashPickedUp);
            world.ApplyEvent(PrototypeWorldEvent.FrontTakenUnderWatch);

            Assert.That(world.LieutenantTrust, Is.EqualTo(LieutenantTrust.Trusted));
            Assert.That(world.FrontControl, Is.EqualTo(FrontControl.PabloWatched));
            Assert.That(world.StatePressure, Is.EqualTo(PressureLevel.Low));

            Object.DestroyImmediate(worldObject);
        }

        [Test]
        public void Phase5MissionSpineTracksSuccessAndPartialFailure()
        {
            var worldObject = new GameObject("Mission World State Test");
            var missionObject = new GameObject("Mission Spine Test");
            var world = worldObject.AddComponent<PrototypeWorldState>();
            var mission = missionObject.AddComponent<PrototypeMissionSpine>();

            mission.AttachWorldState(world);
            Assert.That(mission.Stage, Is.EqualTo(PrototypeMissionStage.FindingFront));

            world.ApplyEvent(PrototypeWorldEvent.DirtyCashPickedUp);
            Assert.That(mission.Stage, Is.EqualTo(PrototypeMissionStage.CarryingRisk));

            world.ApplyEvent(PrototypeWorldEvent.FrontTakenUnderWatch);
            Assert.That(mission.Stage, Is.EqualTo(PrototypeMissionStage.FrontSecured));
            Assert.That(PrototypeDebugState.Mission, Does.Contain("secured"));

            world.ResetState();
            world.ApplyEvent(PrototypeWorldEvent.DirtyCashPickedUp);
            world.ApplyEvent(PrototypeWorldEvent.DirtyCashSeized);

            Assert.That(world.DirtyCash, Is.EqualTo(DirtyCashState.Seized));
            Assert.That(world.StatePressure, Is.EqualTo(PressureLevel.High));
            Assert.That(mission.Stage, Is.EqualTo(PrototypeMissionStage.PartialFailure));
            Assert.That(PrototypeDebugState.Mission, Does.Contain("partial failure"));

            Object.DestroyImmediate(missionObject);
            Object.DestroyImmediate(worldObject);
        }

        [Test]
        public void Phase5MissionSpinePublishesPlayableObjectivePrompts()
        {
            var worldObject = new GameObject("Mission Objective World State Test");
            var missionObject = new GameObject("Mission Objective Spine Test");
            var world = worldObject.AddComponent<PrototypeWorldState>();
            var mission = missionObject.AddComponent<PrototypeMissionSpine>();

            mission.AttachWorldState(world);

            Assert.That(mission.ObjectivePrompt, Is.EqualTo("Objective: collect dirty cash at El Respiro"));
            Assert.That(mission.IsPhase5Resolved, Is.False);
            Assert.That(PrototypeDebugState.Mission, Does.Contain("collect dirty cash"));

            world.ApplyEvent(PrototypeWorldEvent.DirtyCashPickedUp);

            Assert.That(mission.Stage, Is.EqualTo(PrototypeMissionStage.CarryingRisk));
            Assert.That(mission.ObjectivePrompt, Is.EqualTo("Objective: secure El Respiro or risk losing the cash"));
            Assert.That(mission.IsPhase5Resolved, Is.False);

            world.ApplyEvent(PrototypeWorldEvent.FrontTakenUnderWatch);

            Assert.That(mission.Stage, Is.EqualTo(PrototypeMissionStage.FrontSecured));
            Assert.That(mission.ObjectivePrompt, Is.EqualTo("Objective complete: exit through Safe return"));
            Assert.That(mission.IsPhase5Resolved, Is.True);
            Assert.That(PrototypeDebugState.Mission, Does.Contain("Phase 5 resolved"));

            Object.DestroyImmediate(missionObject);
            Object.DestroyImmediate(worldObject);
        }

        [Test]
        public void Phase2RPressureBeatPublishesObjectiveBranchesFromWorldState()
        {
            var worldObject = new GameObject("Phase2R Pressure World State Test");
            var missionObject = new GameObject("Phase2R Pressure Mission Spine Test");
            var world = worldObject.AddComponent<PrototypeWorldState>();
            var mission = missionObject.AddComponent<PrototypeMissionSpine>();

            mission.AttachWorldState(world);
            world.ApplyEvent(PrototypeWorldEvent.PublicViolenceCommitted);

            Assert.That(mission.Stage, Is.EqualTo(PrototypeMissionStage.ActionPressure));
            Assert.That(mission.ObjectivePrompt, Is.EqualTo("Objective: contain street pressure before patrol locks the route"));

            world.ApplyEvent(PrototypeWorldEvent.BribeAccepted);
            Assert.That(mission.Stage, Is.EqualTo(PrototypeMissionStage.PressureContained));
            Assert.That(mission.ObjectivePrompt, Is.EqualTo("Objective: pressure contained, continue to El Respiro"));

            world.ResetState();
            world.ApplyEvent(PrototypeWorldEvent.PublicViolenceCommitted);
            world.ApplyEvent(PrototypeWorldEvent.PressureCrackdownTriggered);

            Assert.That(mission.Stage, Is.EqualTo(PrototypeMissionStage.PressureFailure));
            Assert.That(mission.ObjectivePrompt, Is.EqualTo("Objective changed: escape the patrol pressure"));

            Object.DestroyImmediate(missionObject);
            Object.DestroyImmediate(worldObject);
        }

        [Test]
        public void Phase2RPlayablePressureChoiceOnlyCracksDownWhenPressureIsUncontained()
        {
            var worldObject = new GameObject("Phase2R Pressure Choice World State Test");
            var choiceObject = new GameObject("Phase2R Pressure Choice Test");
            var world = worldObject.AddComponent<PrototypeWorldState>();
            var choice = choiceObject.AddComponent<PrototypePressureChoiceController>();

            choice.AttachWorldState(world);
            world.ApplyEvent(PrototypeWorldEvent.PublicViolenceCommitted);

            Assert.That(choice.ResolvePressureEntry(), Is.True);
            Assert.That(choice.LastResolution, Is.EqualTo(PrototypePressureChoiceResolution.Crackdown));
            Assert.That(world.LastEvent, Is.EqualTo(PrototypeWorldEvent.PressureCrackdownTriggered));
            Assert.That(world.StatePressure, Is.EqualTo(PressureLevel.High));

            world.ResetState();
            world.ApplyEvent(PrototypeWorldEvent.PublicViolenceCommitted);
            world.ApplyEvent(PrototypeWorldEvent.BribeAccepted);

            Assert.That(choice.ResolvePressureEntry(), Is.False);
            Assert.That(choice.LastResolution, Is.EqualTo(PrototypePressureChoiceResolution.Contained));
            Assert.That(world.LastEvent, Is.EqualTo(PrototypeWorldEvent.BribeAccepted));
            Assert.That(world.StatePressure, Is.EqualTo(PressureLevel.Low));

            Object.DestroyImmediate(choiceObject);
            Object.DestroyImmediate(worldObject);
        }

        [Test]
        public void Phase5MissionEventsRejectOutOfOrderTransitions()
        {
            var worldObject = new GameObject("Mission Transition World State Test");
            var world = worldObject.AddComponent<PrototypeWorldState>();

            world.ResetState();

            Assert.That(world.ApplyEvent(PrototypeWorldEvent.FrontTakenUnderWatch), Is.False);
            Assert.That(world.FrontControl, Is.EqualTo(FrontControl.Rival));
            Assert.That(world.DirtyCash, Is.EqualTo(DirtyCashState.None));
            Assert.That(world.LastEvent, Is.EqualTo(PrototypeWorldEvent.None));

            Assert.That(world.ApplyEvent(PrototypeWorldEvent.DirtyCashSeized), Is.False);
            Assert.That(world.DirtyCash, Is.EqualTo(DirtyCashState.None));
            Assert.That(world.LastEvent, Is.EqualTo(PrototypeWorldEvent.None));

            Assert.That(world.ApplyEvent(PrototypeWorldEvent.DirtyCashPickedUp), Is.True);
            Assert.That(world.ApplyEvent(PrototypeWorldEvent.FrontTakenUnderWatch), Is.True);
            Assert.That(world.FrontControl, Is.EqualTo(FrontControl.PabloWatched));
            Assert.That(world.DirtyCash, Is.EqualTo(DirtyCashState.Hidden));

            Object.DestroyImmediate(worldObject);
        }

        [Test]
        public void VehicleEnterExitKeepsPlayerRecoverable()
        {
            EditorSceneManager.OpenScene(ScenePath);
            var player = RequireComponent<PrototypePlayerController>("Pablo Valera Prototype Controller");
            var vehicle = RequireComponent<PrototypeVehicleController>("Prototype Sedan");

            player.EnterVehicle(vehicle);

            Assert.That(vehicle.HasDriver, Is.True);
            Assert.That(player.gameObject.activeSelf, Is.False);

            vehicle.ExitDriver();

            Assert.That(vehicle.HasDriver, Is.False);
            Assert.That(player.gameObject.activeSelf, Is.True);
            Assert.That(player.IsDriving, Is.False);
        }

        [Test]
        public void CameraRigPlanarDirectionsFollowYaw()
        {
            var cameraObject = new GameObject("Camera Direction Test");
            var cameraRig = cameraObject.AddComponent<PrototypeCameraRig>();

            cameraRig.SetYawForTests(90f);

            AssertVectorApproximately(cameraRig.PlanarForward, Vector3.right, 0.001f);
            AssertVectorApproximately(cameraRig.PlanarRight, Vector3.back, 0.001f);
            Assert.That(cameraRig.Yaw, Is.EqualTo(90f).Within(0.001f));

            Object.DestroyImmediate(cameraObject);
        }

        [Test]
        public void GamepadLookYawIsFrameRateIndependent()
        {
            var thirtyFpsYaw = 0f;
            for (var i = 0; i < 30; i++)
            {
                thirtyFpsYaw += PrototypeCameraRig.CalculateYawDelta(Vector2.zero, Vector2.right, 0.12f, 150f, 1f / 30f);
            }

            var oneTwentyFpsYaw = 0f;
            for (var i = 0; i < 120; i++)
            {
                oneTwentyFpsYaw += PrototypeCameraRig.CalculateYawDelta(Vector2.zero, Vector2.right, 0.12f, 150f, 1f / 120f);
            }

            Assert.That(oneTwentyFpsYaw, Is.EqualTo(thirtyFpsYaw).Within(0.001f));
            Assert.That(thirtyFpsYaw, Is.EqualTo(150f).Within(0.001f));
        }

        [Test]
        public void MouseLookStaysRawWhileGamepadPitchScalesWithTime()
        {
            var mouseAtThirtyFps = PrototypeCameraRig.CalculateYawDelta(Vector2.right * 10f, Vector2.zero, 0.12f, 150f, 1f / 30f);
            var mouseAtOneTwentyFps = PrototypeCameraRig.CalculateYawDelta(Vector2.right * 10f, Vector2.zero, 0.12f, 150f, 1f / 120f);
            Assert.That(mouseAtOneTwentyFps, Is.EqualTo(mouseAtThirtyFps).Within(0.001f));

            var thirtyFpsPitch = 0f;
            for (var i = 0; i < 30; i++)
            {
                thirtyFpsPitch += PrototypeCameraRig.CalculatePitchDelta(Vector2.zero, Vector2.up, 0.12f, 120f, 1f / 30f);
            }

            var oneTwentyFpsPitch = 0f;
            for (var i = 0; i < 120; i++)
            {
                oneTwentyFpsPitch += PrototypeCameraRig.CalculatePitchDelta(Vector2.zero, Vector2.up, 0.12f, 120f, 1f / 120f);
            }

            Assert.That(oneTwentyFpsPitch, Is.EqualTo(thirtyFpsPitch).Within(0.001f));
            Assert.That(thirtyFpsPitch, Is.EqualTo(120f).Within(0.001f));
        }

        [Test]
        public void CameraRecenterWaitsForDelayThenMovesTowardPivotYaw()
        {
            var onFoot = PrototypeCameraRig.ResolveProfile(PrototypeCameraMode.OnFootFree);

            var beforeDelay = PrototypeCameraRig.CalculateRecenterYaw(90f, 0f, onFoot.RecenterDelay - 0.01f, onFoot, 0.2f);
            var afterDelay = PrototypeCameraRig.CalculateRecenterYaw(90f, 0f, onFoot.RecenterDelay + 0.01f, onFoot, 0.1f);

            Assert.That(beforeDelay, Is.EqualTo(90f).Within(0.001f));
            Assert.That(Mathf.Abs(Mathf.DeltaAngle(afterDelay, 0f)), Is.LessThan(Mathf.Abs(Mathf.DeltaAngle(90f, 0f))));
            Assert.That(Mathf.Abs(Mathf.DeltaAngle(90f, afterDelay)), Is.LessThanOrEqualTo(onFoot.RecenterSpeed * 0.1f + 0.001f));
        }

        [Test]
        public void CameraTightSpaceRecoveryHoldsBrieflyAfterCollisionClears()
        {
            Assert.That(
                PrototypeCameraRig.ResolveModeWithTightSpace(PrototypeCameraMode.OnFootFree, 0.2f, 0f),
                Is.EqualTo(PrototypeCameraMode.TightSpaceRecovery));
            Assert.That(
                PrototypeCameraRig.ResolveModeWithTightSpace(PrototypeCameraMode.OnFootFree, 0f, 0.2f),
                Is.EqualTo(PrototypeCameraMode.TightSpaceRecovery));
            Assert.That(
                PrototypeCameraRig.ResolveModeWithTightSpace(PrototypeCameraMode.OnFootFree, 0f, 0f),
                Is.EqualTo(PrototypeCameraMode.OnFootFree));
        }

        [Test]
        public void PlayerMovementUsesCameraPlanarAxes()
        {
            var desiredMove = PrototypePlayerController.BuildCameraRelativeMove(
                Vector2.up,
                Vector3.right,
                Vector3.back);

            AssertVectorApproximately(desiredMove, Vector3.right, 0.001f);
        }

        [Test]
        public void PlayerSideMovementDoesNotSpiralWithBodyRotation()
        {
            var cameraForward = Vector3.forward;
            var cameraRight = Vector3.right;
            var firstMove = PrototypePlayerController.BuildCameraRelativeMove(Vector2.left, cameraForward, cameraRight);
            var rotatedBodyWouldHaveChangedLocalRight = Quaternion.Euler(0f, -90f, 0f) * Vector3.right;
            var secondMove = PrototypePlayerController.BuildCameraRelativeMove(Vector2.left, cameraForward, cameraRight);

            AssertVectorApproximately(firstMove, Vector3.left, 0.001f);
            AssertVectorApproximately(secondMove, Vector3.left, 0.001f);
            Assert.That(rotatedBodyWouldHaveChangedLocalRight, Is.Not.EqualTo(cameraRight));
        }

        [Test]
        public void VehicleExitUsesFallbackAndBlocksWhenBothSidesAreOccupied()
        {
            var vehicleObject = new GameObject("Vehicle Exit Safety Test");
            vehicleObject.AddComponent<BoxCollider>();
            vehicleObject.AddComponent<Rigidbody>();
            var vehicle = vehicleObject.AddComponent<PrototypeVehicleController>();
            vehicleObject.transform.position = Vector3.zero;
            vehicleObject.transform.rotation = Quaternion.identity;

            var leftExit = new GameObject("Left Exit").transform;
            leftExit.SetParent(vehicleObject.transform);
            leftExit.localPosition = new Vector3(-1.8f, 0.2f, 0f);

            var rightExit = new GameObject("Right Exit").transform;
            rightExit.SetParent(vehicleObject.transform);
            rightExit.localPosition = new Vector3(1.8f, 0.2f, 0f);

            vehicle.SetExitPointsForTests(leftExit, rightExit);

            var leftBlocker = GameObject.CreatePrimitive(PrimitiveType.Cube);
            leftBlocker.name = "Left Exit Blocker";
            leftBlocker.transform.position = leftExit.position;
            leftBlocker.transform.localScale = Vector3.one;

            Assert.That(vehicle.TryResolveExitPose(out var fallbackPosition, out _), Is.True);
            AssertVectorApproximately(fallbackPosition, rightExit.position, 0.001f);

            var rightBlocker = GameObject.CreatePrimitive(PrimitiveType.Cube);
            rightBlocker.name = "Right Exit Blocker";
            rightBlocker.transform.position = rightExit.position;
            rightBlocker.transform.localScale = Vector3.one;

            Assert.That(vehicle.TryResolveExitPose(out _, out _), Is.False);

            Object.DestroyImmediate(rightBlocker);
            Object.DestroyImmediate(leftBlocker);
            Object.DestroyImmediate(vehicleObject);
        }

        [Test]
        public void VehicleDriveIntentBrakesBeforeReverse()
        {
            var brakeIntent = PrototypeVehicleController.ResolveDriveIntent(-1f, 4f, 0.35f);
            Assert.That(brakeIntent.Throttle, Is.EqualTo(0f));
            Assert.That(brakeIntent.Brake, Is.GreaterThan(0f));
            Assert.That(brakeIntent.Reverse, Is.EqualTo(0f));

            var reverseIntent = PrototypeVehicleController.ResolveDriveIntent(-1f, 0.1f, 0.35f);
            Assert.That(reverseIntent.Throttle, Is.EqualTo(0f));
            Assert.That(reverseIntent.Brake, Is.EqualTo(0f));
            Assert.That(reverseIntent.Reverse, Is.GreaterThan(0f));
        }

        [Test]
        public void FoundationLayersAreConfiguredAndExposeRuntimeMasks()
        {
            Assert.That(PrototypeLayers.AreConfigured(out var missing), Is.True, missing);
            Assert.That(PrototypeLayers.CameraCollisionMask, Is.EqualTo(PrototypeLayers.WorldCollisionMask));
            Assert.That((PrototypeLayers.InteractionQueryMask & (1 << PrototypeLayers.Interactable)) != 0, Is.True);
            Assert.That((PrototypeLayers.InteractionQueryMask & (1 << PrototypeLayers.Vehicle)) != 0, Is.True);
            Assert.That((PrototypeLayers.ExitBlockMask & (1 << PrototypeLayers.RouteTrigger)) == 0, Is.True);
            Assert.That((PrototypeLayers.ExitBlockMask & (1 << PrototypeLayers.SensorTrigger)) == 0, Is.True);
        }

        [Test]
        public void CameraProfilesCoverFoundationLockModes()
        {
            var onFoot = PrototypeCameraRig.ResolveProfile(PrototypeCameraMode.OnFootFree);
            var interaction = PrototypeCameraRig.ResolveProfile(PrototypeCameraMode.OnFootInteractionFocus);
            var driving = PrototypeCameraRig.ResolveProfile(PrototypeCameraMode.DrivingChase);
            var tightSpace = PrototypeCameraRig.ResolveProfile(PrototypeCameraMode.TightSpaceRecovery);

            Assert.That(driving.Distance, Is.GreaterThan(onFoot.Distance));
            Assert.That(interaction.ShoulderBias, Is.GreaterThan(onFoot.ShoulderBias));
            Assert.That(tightSpace.CollisionRestoreSpeed, Is.GreaterThan(onFoot.CollisionRestoreSpeed));
            Assert.That(driving.RecenterDelay, Is.LessThan(onFoot.RecenterDelay));
        }

        [Test]
        public void CharacterMotorVelocityAndSlopeRulesAreDeterministic()
        {
            var current = Vector3.zero;
            var target = PrototypeCharacterMotor.CalculateTargetHorizontalVelocity(
                Vector3.forward,
                current,
                4.2f,
                18f,
                22f,
                0.5f);

            AssertVectorApproximately(target, Vector3.forward * 4.2f, 0.001f);
            Assert.That(PrototypeCharacterMotor.IsSlopeWalkable(Vector3.up, 50f), Is.True);
            Assert.That(PrototypeCharacterMotor.IsSlopeWalkable(Quaternion.Euler(65f, 0f, 0f) * Vector3.up, 50f), Is.False);
        }

        [Test]
        public void CharacterMotorExposesRealFeelTuningDefaults()
        {
            var playerObject = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            var motor = playerObject.AddComponent<PrototypeCharacterMotor>();
            var serialized = new SerializedObject(motor);

            var stepHeight = serialized.FindProperty("stepHeight");
            var groundSnapDistance = serialized.FindProperty("groundSnapDistance");
            var slopeLimit = serialized.FindProperty("slopeLimit");

            Assert.That(stepHeight, Is.Not.Null, "Motor needs an explicit step height before it can be tuned against authored street geometry.");
            Assert.That(stepHeight.floatValue, Is.EqualTo(0.38f).Within(0.001f));
            Assert.That(groundSnapDistance, Is.Not.Null, "Motor needs ground snap as a first-class tuning parameter.");
            Assert.That(groundSnapDistance.floatValue, Is.EqualTo(0.28f).Within(0.001f));
            Assert.That(slopeLimit.floatValue, Is.EqualTo(50f).Within(0.001f));

            Object.DestroyImmediate(playerObject);
        }

        [Test]
        public void InteractionTargetingPrefersVisibleHigherPriorityCandidate()
        {
            var origin = Vector3.zero;
            var visibleVehicle = new PrototypeInteractionCandidate(
                new GameObject("Visible Vehicle Candidate").transform,
                PrototypeInteractionKind.Vehicle,
                "enter",
                10,
                false);
            var blockedInteractable = new PrototypeInteractionCandidate(
                new GameObject("Blocked Interactable Candidate").transform,
                PrototypeInteractionKind.Interactable,
                "use",
                20,
                true);
            visibleVehicle.Transform.position = new Vector3(2f, 0f, 0f);
            blockedInteractable.Transform.position = new Vector3(1f, 0f, 0f);

            var selected = PrototypeInteractionTargeting.SelectBest(
                origin,
                new[] { blockedInteractable, visibleVehicle },
                out var target);

            Assert.That(selected, Is.True);
            Assert.That(target.Kind, Is.EqualTo(PrototypeInteractionKind.Vehicle));
            Assert.That(target.Blocked, Is.False);

            Object.DestroyImmediate(visibleVehicle.Transform.gameObject);
            Object.DestroyImmediate(blockedInteractable.Transform.gameObject);
        }

        [Test]
        public void SliceDefinitionProvidesPhase1RouteData()
        {
            var definition = ScriptableObject.CreateInstance<PrototypeSliceDefinition>();
            definition.ConfigurePhase1Defaults();

            Assert.That(definition.Validate(out var error), Is.True, error);
            Assert.That(definition.RouteCheckpoints.Length, Is.EqualTo(5));
            Assert.That(definition.RouteCheckpoints[0].Label, Is.EqualTo("Start on foot"));
            Assert.That(definition.RouteCheckpoints[^1].Label, Is.EqualTo("Safe return"));

            Object.DestroyImmediate(definition);
        }

        [Test]
        public void WheelVehicleSpikeUsesSameDriveIntentAsArcadeBaseline()
        {
            var baseline = PrototypeVehicleController.ResolveDriveIntent(-1f, 3f, 0.35f);
            var spike = PrototypeWheelVehicleController.ResolveDriveIntent(-1f, 3f, 0.35f);

            Assert.That(spike.Throttle, Is.EqualTo(baseline.Throttle));
            Assert.That(spike.Brake, Is.EqualTo(baseline.Brake));
            Assert.That(spike.Reverse, Is.EqualTo(baseline.Reverse));
        }

        [Test]
        public void VehicleComparisonDecisionKeepsArcadeWhenWheelSpikeIsNotViable()
        {
            var arcade = new PrototypeVehicleProbeMetrics(
                PrototypeVehicleCandidateKind.ArcadeRigidbodyBaseline,
                28f,
                12f,
                4f,
                3f,
                42f,
                55f,
                2f,
                true,
                true);
            var wheel = new PrototypeVehicleProbeMetrics(
                PrototypeVehicleCandidateKind.WheelColliderSpike,
                2f,
                1f,
                0f,
                0f,
                5f,
                0f,
                0f,
                true,
                false);

            var decision = PrototypeVehicleComparison.Decide(arcade, wheel);

            Assert.That(decision, Is.EqualTo(PrototypeVehicleDecision.KeepArcadeRigidbodyBaseline));
            Assert.That(PrototypeVehicleComparison.BuildReport(arcade, wheel, decision), Does.Contain("Decision: KeepArcadeRigidbodyBaseline"));
        }

        [Test]
        public void Phase1SceneIsInBuildSettings()
        {
            var found = false;
            foreach (var scene in EditorBuildSettings.scenes)
            {
                if (scene.path == ScenePath && scene.enabled)
                {
                    found = true;
                    break;
                }
            }

            Assert.That(found, Is.True);
        }

        [Test]
        public void RouteProgressAdvancesInCheckpointOrder()
        {
            EditorSceneManager.OpenScene(ScenePath);
            var route = RequireComponent<PrototypeRouteProgress>("Phase 1 Route Progress");
            var first = RequireComponent<PrototypeRouteCheckpoint>("Route checkpoint 0: Start on foot");
            var second = RequireComponent<PrototypeRouteCheckpoint>("Route checkpoint 1: Enter vehicle lane");

            route.Configure(5);
            route.RegisterCheckpoint(second.CheckpointIndex, second.Label);
            Assert.That(route.NextCheckpointIndex, Is.EqualTo(0), "Route should ignore out-of-order checkpoints.");

            route.RegisterCheckpoint(first.CheckpointIndex, first.Label);
            Assert.That(route.NextCheckpointIndex, Is.EqualTo(1));
            Assert.That(PrototypeDebugState.LastCheckpoint, Is.EqualTo("Start on foot"));
        }

        [Test]
        public void RouteProgressContainedPressureCanCompleteNormalRoute()
        {
            var worldObject = new GameObject("Contained Route Gate World State Test");
            var routeObject = new GameObject("Contained Route Gate Test");
            var world = worldObject.AddComponent<PrototypeWorldState>();
            var route = routeObject.AddComponent<PrototypeRouteProgress>();

            route.AttachWorldState(world);
            route.Configure(5);
            world.ApplyEvent(PrototypeWorldEvent.PublicViolenceCommitted);
            world.ApplyEvent(PrototypeWorldEvent.BribeAccepted);

            route.RegisterCheckpoint(0, "Start on foot");
            route.RegisterCheckpoint(1, "Enter vehicle lane");
            route.RegisterCheckpoint(2, "Patrol pressure turn");
            route.RegisterCheckpoint(3, "Workshop interaction stop");
            route.RegisterCheckpoint(4, "Safe return");

            Assert.That(route.IsComplete, Is.True);
            Assert.That(route.Outcome, Is.EqualTo(PrototypeRouteOutcome.PressureContained));
            Assert.That(PrototypeDebugState.Route, Is.EqualTo("Complete"));

            Object.DestroyImmediate(routeObject);
            Object.DestroyImmediate(worldObject);
        }

        [Test]
        public void RouteProgressCrackdownBlocksForwardRouteButAllowsSafeReturnEscape()
        {
            var worldObject = new GameObject("Crackdown Route Gate World State Test");
            var routeObject = new GameObject("Crackdown Route Gate Test");
            var world = worldObject.AddComponent<PrototypeWorldState>();
            var route = routeObject.AddComponent<PrototypeRouteProgress>();

            route.AttachWorldState(world);
            route.Configure(5);

            route.RegisterCheckpoint(0, "Start on foot");
            route.RegisterCheckpoint(1, "Enter vehicle lane");
            route.RegisterCheckpoint(2, "Patrol pressure turn");
            world.ApplyEvent(PrototypeWorldEvent.PublicViolenceCommitted);
            world.ApplyEvent(PrototypeWorldEvent.PressureCrackdownTriggered);

            route.RegisterCheckpoint(3, "Workshop interaction stop");

            Assert.That(route.NextCheckpointIndex, Is.EqualTo(3));
            Assert.That(route.IsComplete, Is.False);
            Assert.That(route.Outcome, Is.EqualTo(PrototypeRouteOutcome.PressureBlocked));
            Assert.That(PrototypeDebugState.Route, Does.Contain("blocked"));

            route.RegisterCheckpoint(4, "Safe return");

            Assert.That(route.IsComplete, Is.False);
            Assert.That(route.Outcome, Is.EqualTo(PrototypeRouteOutcome.PressureFailureEscape));
            Assert.That(PrototypeDebugState.Route, Does.Contain("Pressure escape"));
            Assert.That(PrototypeDebugState.LastCheckpoint, Is.EqualTo("Safe return"));

            Object.DestroyImmediate(routeObject);
            Object.DestroyImmediate(worldObject);
        }

        [Test]
        public void RouteProgressKeepsPressureEscapeWhenOverlappingStartTriggerFiresAfterSafeReturn()
        {
            var worldObject = new GameObject("Crackdown Route Overlap World State Test");
            var routeObject = new GameObject("Crackdown Route Overlap Test");
            var world = worldObject.AddComponent<PrototypeWorldState>();
            var route = routeObject.AddComponent<PrototypeRouteProgress>();

            route.AttachWorldState(world);
            route.Configure(5);
            world.ApplyEvent(PrototypeWorldEvent.PublicViolenceCommitted);
            world.ApplyEvent(PrototypeWorldEvent.PressureCrackdownTriggered);

            route.RegisterCheckpoint(3, "Workshop interaction stop");
            route.RegisterCheckpoint(4, "Safe return");
            route.RegisterCheckpoint(0, "Start on foot");

            Assert.That(route.Outcome, Is.EqualTo(PrototypeRouteOutcome.PressureFailureEscape));
            Assert.That(PrototypeDebugState.Route, Does.Contain("Pressure escape"));

            Object.DestroyImmediate(routeObject);
            Object.DestroyImmediate(worldObject);
        }

        private static GameObject RequireObject(string objectName)
        {
            var target = GameObject.Find(objectName);
            Assert.That(target, Is.Not.Null, $"Missing required object: {objectName}");
            return target;
        }

        private static GameObject LoadStreetKitPrefab(string prefabName)
        {
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>($"{ValleDePlataStreetKitPath}/Prefabs/{prefabName}.prefab");
            Assert.That(prefab, Is.Not.Null, $"Missing generated street kit prefab {prefabName}.");
            return prefab;
        }

        private static bool AllGameObjectsUseLayer(GameObject root, int expectedLayer)
        {
            return root.GetComponentsInChildren<Transform>(true).All(transform => transform.gameObject.layer == expectedLayer);
        }

        private static bool HasComponentNamed(GameObject target, string componentTypeName)
        {
            foreach (var component in target.GetComponents<MonoBehaviour>())
            {
                if (component != null && component.GetType().Name == componentTypeName)
                {
                    return true;
                }
            }

            return false;
        }

        private static void AssertAnimatorParameter(AnimatorController controller, string name, AnimatorControllerParameterType type)
        {
            var parameter = controller.parameters.FirstOrDefault(parameter => parameter.name == name);
            Assert.That(parameter, Is.Not.Null, $"Animator parameter {name} is missing.");
            Assert.That(parameter.type, Is.EqualTo(type), $"Animator parameter {name} has the wrong type.");
        }

        private static bool IsPabloUpperBodyCurve(EditorCurveBinding binding)
        {
            return PabloUpperBodyCurveFragments.Any(fragment => binding.propertyName.Contains(fragment));
        }

        private static GameObject RequireNonBlockingDressing(string objectName)
        {
            var target = GameObject.Find(objectName);
            Assert.That(target, Is.Not.Null, $"Missing required dressing object: {objectName}");
            Assert.That(target.layer, Is.EqualTo(PrototypeLayers.CameraIgnore), $"{objectName} should be on CameraIgnore so it cannot shorten the camera.");
            var collider = target.GetComponent<Collider>();
            Assert.That(collider == null || collider.enabled == false || collider.isTrigger, Is.True, $"{objectName} should not block the player, vehicle, or exit checks.");
            return target;
        }

        private static GameObject RequireReadablePropGroup(string objectName, int minimumRenderers)
        {
            var target = RequireObject(objectName);
            Assert.That(target.layer, Is.EqualTo(PrototypeLayers.CameraIgnore), $"{objectName} should stay out of camera collision.");
            Assert.That(HasComponentNamed(target, "PrototypeReadableProp"), Is.True, $"{objectName} needs readable prop metadata.");
            Assert.That(target.GetComponentsInChildren<Renderer>().Length, Is.GreaterThanOrEqualTo(minimumRenderers));

            foreach (var collider in target.GetComponentsInChildren<Collider>())
            {
                Assert.That(collider.enabled == false || collider.isTrigger, Is.True, $"{objectName} contains a blocking collider on {collider.name}.");
            }

            return target;
        }

        private static void AssertCursorDecision(object decision, CursorLockMode expectedMode, bool expectedVisible)
        {
            Assert.That(decision, Is.Not.Null);
            var decisionType = decision.GetType();
            var lockState = (CursorLockMode)decisionType.GetProperty("LockState")?.GetValue(decision);
            var visible = (bool)decisionType.GetProperty("Visible")?.GetValue(decision);
            Assert.That(lockState, Is.EqualTo(expectedMode));
            Assert.That(visible, Is.EqualTo(expectedVisible));
        }

        private static Component RequireComponentByName(string objectName, string componentName)
        {
            var target = RequireObject(objectName);
            var component = target.GetComponents<Component>().FirstOrDefault(component => component != null && component.GetType().Name == componentName);
            Assert.That(component, Is.Not.Null, $"{objectName} is missing component {componentName}.");
            return component;
        }

        private static T RequireComponent<T>(string objectName) where T : Component
        {
            var target = GameObject.Find(objectName);
            Assert.That(target, Is.Not.Null, $"Missing required object: {objectName}");
            var component = target.GetComponent<T>();
            Assert.That(component, Is.Not.Null, $"{objectName} is missing component {typeof(T).Name}.");
            return component;
        }

        private static void RequireRouteCheckpoint(int index, string label)
        {
            var checkpoint = RequireComponent<PrototypeRouteCheckpoint>($"Route checkpoint {index}: {label}");
            Assert.That(checkpoint.CheckpointIndex, Is.EqualTo(index));
            Assert.That(checkpoint.Label, Is.EqualTo(label));
        }

        private static void AssertVectorApproximately(Vector3 actual, Vector3 expected, float tolerance)
        {
            Assert.That(Vector3.Distance(actual, expected), Is.LessThanOrEqualTo(tolerance), $"Expected {expected}, got {actual}.");
        }
    }
}
