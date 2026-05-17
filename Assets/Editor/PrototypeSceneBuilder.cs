using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using ValleDePlata.Prototype;

namespace ValleDePlata.Editor
{
    public static class PrototypeSceneBuilder
    {
        private const string ScenePath = "Assets/Scenes/Phase1_FeelPrototype.unity";
        private const string MaterialFolder = "Assets/PrototypeMaterials";

        [MenuItem("Valle de Plata/Build Phase 1 Feel Prototype Scene")]
        public static void BuildPhase1Scene()
        {
            EnsureFolder(MaterialFolder);

            var concrete = CreateMaterial("Prototype_Concrete", new Color(0.46f, 0.45f, 0.4f));
            var asphalt = CreateMaterial("Prototype_Asphalt", new Color(0.13f, 0.13f, 0.12f));
            var sunBleachedWall = CreateMaterial("Prototype_SunBleachedWall", new Color(0.72f, 0.64f, 0.49f));
            var rust = CreateMaterial("Prototype_Rust", new Color(0.55f, 0.22f, 0.13f));
            var patrolBlue = CreateMaterial("Prototype_PatrolBlue", new Color(0.08f, 0.16f, 0.28f));
            var routeGreen = CreateMaterial("Prototype_RouteGreen", new Color(0.18f, 0.42f, 0.32f));

            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            SceneManager.SetActiveScene(scene);

            RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Trilight;
            RenderSettings.ambientSkyColor = new Color(0.75f, 0.68f, 0.55f);
            RenderSettings.ambientEquatorColor = new Color(0.46f, 0.4f, 0.32f);
            RenderSettings.ambientGroundColor = new Color(0.18f, 0.16f, 0.14f);

            CreateLight();
            CreateEnvironment(concrete, asphalt, sunBleachedWall, rust, patrolBlue, routeGreen);
            var player = CreatePlayer();
            CreateVehicle();
            CreateCamera(player);
            CreateDebugHud();

            EditorSceneManager.SaveScene(scene, ScenePath);
            AddSceneToBuildSettings(ScenePath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        private static void CreateLight()
        {
            var lightObject = new GameObject("Hot afternoon sun");
            var light = lightObject.AddComponent<Light>();
            light.type = LightType.Directional;
            light.intensity = 1.35f;
            lightObject.transform.rotation = Quaternion.Euler(48f, -35f, 0f);
        }

        private static void CreateEnvironment(Material concrete, Material asphalt, Material wall, Material rust, Material patrolBlue, Material routeGreen)
        {
            CreateCube("Ground", new Vector3(0f, -0.05f, 12f), new Vector3(42f, 0.1f, 72f), concrete);
            CreateCube("Narrow asphalt route", new Vector3(0f, 0f, 12f), new Vector3(7.4f, 0.05f, 62f), asphalt);
            CreateCube("Left barrio wall", new Vector3(-4.9f, 1.25f, 8f), new Vector3(0.7f, 2.5f, 48f), wall);
            CreateCube("Right barrio wall", new Vector3(4.9f, 1.25f, 6f), new Vector3(0.7f, 2.5f, 44f), wall);
            CreateCube("Tight corner block", new Vector3(2.1f, 1.6f, 28f), new Vector3(7f, 3.2f, 0.8f), wall);
            CreateCube("Return lane blocker", new Vector3(-2.2f, 0.6f, 40f), new Vector3(2.2f, 1.2f, 1.2f), rust);
            CreateCube("Workshop shutter interactable", new Vector3(3.85f, 1.15f, 49f), new Vector3(0.35f, 2.3f, 4.3f), rust).AddComponent<PrototypeInteractable>();
            CreateCube("Static civilian car obstacle", new Vector3(-2.35f, 0.45f, 18f), new Vector3(2f, 0.9f, 4f), rust);

            var patrol = CreateCube("Pressure patrol marker", new Vector3(0f, 0.55f, 34f), new Vector3(2.2f, 1.1f, 2.2f), patrolBlue);
            patrol.GetComponent<Collider>().isTrigger = true;
            patrol.AddComponent<PrototypePressureZone>();

            CreateCube("Safe return marker", new Vector3(0f, 0.04f, -8f), new Vector3(6f, 0.08f, 2.2f), routeGreen);
        }

        private static PrototypePlayerController CreatePlayer()
        {
            var playerObject = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            playerObject.name = "Pablo Valera Prototype Controller";
            playerObject.transform.position = new Vector3(0f, 1f, -12f);
            Object.DestroyImmediate(playerObject.GetComponent<CapsuleCollider>());

            var character = playerObject.AddComponent<CharacterController>();
            character.height = 1.85f;
            character.radius = 0.36f;
            character.center = new Vector3(0f, 0.92f, 0f);

            var controller = playerObject.AddComponent<PrototypePlayerController>();
            var pivot = new GameObject("Camera Pivot").transform;
            pivot.SetParent(playerObject.transform);
            pivot.localPosition = new Vector3(0f, 1.45f, 0f);
            SetObjectReference(controller, "cameraPivot", pivot);

            return controller;
        }

        private static void CreateVehicle()
        {
            var vehicle = CreateCube("Prototype Sedan", new Vector3(-2.4f, 0.55f, -4f), new Vector3(1.9f, 1.1f, 4.1f), CreateMaterial("Prototype_CarGreen", new Color(0.17f, 0.3f, 0.22f)));
            var body = vehicle.AddComponent<Rigidbody>();
            body.mass = 1150f;
            body.linearDamping = 0.08f;
            body.angularDamping = 0.75f;

            var controller = vehicle.AddComponent<PrototypeVehicleController>();

            var cameraPivot = new GameObject("Vehicle Camera Pivot").transform;
            cameraPivot.SetParent(vehicle.transform);
            cameraPivot.localPosition = new Vector3(0f, 1.35f, -0.6f);
            SetObjectReference(controller, "cameraPivot", cameraPivot);

            var exitPoint = new GameObject("Exit Point").transform;
            exitPoint.SetParent(vehicle.transform);
            exitPoint.localPosition = new Vector3(-1.8f, 0.2f, -0.6f);
            SetObjectReference(controller, "exitPoint", exitPoint);
        }

        private static void CreateCamera(PrototypePlayerController player)
        {
            var rig = new GameObject("Prototype Camera Rig");
            rig.transform.position = new Vector3(0f, 3f, -18f);
            var camera = new GameObject("Main Camera");
            camera.tag = "MainCamera";
            camera.transform.SetParent(rig.transform);
            var cameraComponent = camera.AddComponent<Camera>();
            cameraComponent.fieldOfView = 62f;
            camera.AddComponent<AudioListener>();
            var cameraRig = rig.AddComponent<PrototypeCameraRig>();
            SetObjectReference(cameraRig, "player", player);
            SetObjectReference(cameraRig, "targetCamera", cameraComponent);
        }

        private static void CreateDebugHud()
        {
            var debug = new GameObject("Prototype Debug HUD");
            debug.AddComponent<PrototypeDebugHud>();
        }

        private static GameObject CreateCube(string name, Vector3 position, Vector3 scale, Material material)
        {
            var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.name = name;
            cube.transform.position = position;
            cube.transform.localScale = scale;
            cube.GetComponent<Renderer>().sharedMaterial = material;
            return cube;
        }

        private static Material CreateMaterial(string name, Color color)
        {
            var path = $"{MaterialFolder}/{name}.mat";
            var material = AssetDatabase.LoadAssetAtPath<Material>(path);
            if (material == null)
            {
                material = new Material(Shader.Find("Universal Render Pipeline/Lit"));
                AssetDatabase.CreateAsset(material, path);
            }

            material.color = color;
            EditorUtility.SetDirty(material);
            return material;
        }

        private static void SetObjectReference(Object target, string propertyName, Object value)
        {
            var serializedObject = new SerializedObject(target);
            serializedObject.FindProperty(propertyName).objectReferenceValue = value;
            serializedObject.ApplyModifiedPropertiesWithoutUndo();
        }

        private static void AddSceneToBuildSettings(string scenePath)
        {
            var scenes = EditorBuildSettings.scenes;
            foreach (var buildScene in scenes)
            {
                if (buildScene.path == scenePath)
                {
                    return;
                }
            }

            var next = new EditorBuildSettingsScene[scenes.Length + 1];
            scenes.CopyTo(next, 0);
            next[^1] = new EditorBuildSettingsScene(scenePath, true);
            EditorBuildSettings.scenes = next;
        }

        private static void EnsureFolder(string path)
        {
            if (AssetDatabase.IsValidFolder(path))
            {
                return;
            }

            var parent = Path.GetDirectoryName(path)?.Replace("\\", "/");
            var folder = Path.GetFileName(path);
            AssetDatabase.CreateFolder(string.IsNullOrEmpty(parent) ? "Assets" : parent, folder);
        }
    }
}
