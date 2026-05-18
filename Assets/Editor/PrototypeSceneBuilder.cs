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
            CreateRoute(routeGreen);
            CreateWorldState();
            CreateMissionSpine();
            CreateCamera(player);
            CreateRunMetrics();
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
            CreatePublicViolenceMicrotest(rust, patrolBlue, routeGreen);
            CreateBribeMicrotest(rust, patrolBlue, routeGreen);
            CreateMateoMicrotest(rust, patrolBlue, routeGreen);
            CreateFrontPrototypeMicrotest(rust, patrolBlue, routeGreen);

            var patrol = CreateCube("Pressure patrol marker", new Vector3(0f, 0.55f, 34f), new Vector3(2.2f, 1.1f, 2.2f), patrolBlue);
            patrol.GetComponent<Collider>().isTrigger = true;
            patrol.AddComponent<PrototypePressureZone>();

            CreateCube("Safe return marker", new Vector3(0f, 0.04f, -8f), new Vector3(6f, 0.08f, 2.2f), routeGreen);
        }

        private static void CreatePublicViolenceMicrotest(Material rust, Material patrolBlue, Material routeGreen)
        {
            var target = CreateCube("Public violence test target", new Vector3(-3.1f, 0.8f, 10f), new Vector3(0.7f, 1.6f, 0.7f), rust);
            var interactable = target.AddComponent<PrototypeInteractable>();
            interactable.Configure(
                "Use public violence",
                "Street saw the violence",
                PrototypeWorldEvent.PublicViolenceCommitted);

            AddReactionMarker(
                PrototypeWorldEvent.PublicViolenceCommitted,
                "Civilian panic marker",
                new Vector3(-4.1f, 0.75f, 12f),
                new Vector3(0.6f, 1.5f, 0.6f),
                routeGreen,
                "Civilians scatter from Pablo's show of force",
                new Color(0.18f, 0.42f, 0.32f),
                new Color(0.95f, 0.7f, 0.18f));
            AddReactionMarker(
                PrototypeWorldEvent.PublicViolenceCommitted,
                "Shop shutter closes marker",
                new Vector3(4.55f, 1.05f, 13.5f),
                new Vector3(0.25f, 2.1f, 2.6f),
                rust,
                "Shop closes after the public violence",
                new Color(0.55f, 0.22f, 0.13f),
                new Color(0.12f, 0.08f, 0.06f));
            AddReactionMarker(
                PrototypeWorldEvent.PublicViolenceCommitted,
                "Police pressure moves closer marker",
                new Vector3(1.6f, 0.55f, 18f),
                new Vector3(1.6f, 1.1f, 1.6f),
                patrolBlue,
                "Patrol shifts closer after witnesses talk",
                new Color(0.08f, 0.16f, 0.28f),
                new Color(0.08f, 0.32f, 0.62f));
        }

        private static void CreateBribeMicrotest(Material rust, Material patrolBlue, Material routeGreen)
        {
            var officer = CreateCube("Rios bribe test officer", new Vector3(3.15f, 0.85f, 22f), new Vector3(0.75f, 1.7f, 0.75f), patrolBlue);
            officer.AddComponent<PrototypeInteractable>().Configure(
                "Pay Rios bribe",
                "Rios lets Pablo pass but remembers",
                PrototypeWorldEvent.BribeAccepted);

            AddReactionMarker(
                PrototypeWorldEvent.BribeAccepted,
                "Bribe roadblock opens marker",
                new Vector3(0f, 0.6f, 24.5f),
                new Vector3(4f, 1.2f, 0.35f),
                patrolBlue,
                "Roadblock opens after the bribe",
                new Color(0.08f, 0.16f, 0.28f),
                new Color(0.18f, 0.42f, 0.32f));
            AddReactionMarker(
                PrototypeWorldEvent.BribeAccepted,
                "Rios leverage marker",
                new Vector3(4.2f, 1.15f, 23f),
                new Vector3(0.35f, 2.3f, 1.7f),
                rust,
                "Rios now has leverage over Pablo",
                new Color(0.55f, 0.22f, 0.13f),
                new Color(0.7f, 0.54f, 0.18f));
            AddReactionMarker(
                PrototypeWorldEvent.BribeAccepted,
                "Risk cargo hidden marker",
                new Vector3(-1.7f, 0.45f, 23f),
                new Vector3(1.4f, 0.9f, 1.4f),
                routeGreen,
                "Risk cargo stays hidden after the bribe",
                new Color(0.18f, 0.42f, 0.32f),
                new Color(0.1f, 0.18f, 0.14f));
        }

        private static void CreateMateoMicrotest(Material rust, Material patrolBlue, Material routeGreen)
        {
            var trusted = CreateCube("Mateo protected test contact", new Vector3(-3.2f, 0.85f, 31f), new Vector3(0.75f, 1.7f, 0.75f), routeGreen);
            trusted.AddComponent<PrototypeInteractable>().Configure(
                "Protect Mateo",
                "Mateo warns early",
                PrototypeWorldEvent.MateoProtected);

            var humiliated = CreateCube("Mateo humiliated test contact", new Vector3(3.2f, 0.85f, 31f), new Vector3(0.75f, 1.7f, 0.75f), rust);
            humiliated.AddComponent<PrototypeInteractable>().Configure(
                "Humiliate Mateo",
                "Mateo warns too late",
                PrototypeWorldEvent.MateoHumiliated);

            AddReactionMarker(
                PrototypeWorldEvent.MateoProtected,
                "Mateo early warning marker",
                new Vector3(-2.2f, 0.45f, 36f),
                new Vector3(1.8f, 0.9f, 1.8f),
                routeGreen,
                "Mateo gives the warning before Pablo reaches the patrol",
                new Color(0.18f, 0.42f, 0.32f),
                new Color(0.35f, 0.75f, 0.48f));
            AddReactionMarker(
                PrototypeWorldEvent.MateoHumiliated,
                "Mateo late warning marker",
                new Vector3(2.2f, 0.45f, 36f),
                new Vector3(1.8f, 0.9f, 1.8f),
                patrolBlue,
                "Mateo withholds the warning until the patrol is close",
                new Color(0.08f, 0.16f, 0.28f),
                new Color(0.62f, 0.12f, 0.1f));
        }

        private static void CreateFrontPrototypeMicrotest(Material rust, Material patrolBlue, Material routeGreen)
        {
            var cash = CreateCube("El Respiro dirty cash pickup", new Vector3(2.8f, 0.35f, 45f), new Vector3(0.9f, 0.7f, 0.9f), routeGreen);
            cash.AddComponent<PrototypeInteractable>().Configure(
                "Pick up dirty cash",
                "Dirty cash is now Pablo's risk",
                PrototypeWorldEvent.DirtyCashPickedUp);

            var front = CreateCube("El Respiro front takeover", new Vector3(3.55f, 1.3f, 47f), new Vector3(0.45f, 2.6f, 2.6f), rust);
            front.AddComponent<PrototypeInteractable>().Configure(
                "Secure El Respiro front",
                "El Respiro works, but under watch",
                PrototypeWorldEvent.FrontTakenUnderWatch);

            AddReactionMarker(
                PrototypeWorldEvent.DirtyCashPickedUp,
                "Dirty cash carried marker",
                new Vector3(1.3f, 0.45f, 45f),
                new Vector3(1.2f, 0.9f, 1.2f),
                routeGreen,
                "Dirty cash is physically exposed until hidden",
                new Color(0.18f, 0.42f, 0.32f),
                new Color(0.86f, 0.62f, 0.2f));
            AddReactionMarker(
                PrototypeWorldEvent.FrontTakenUnderWatch,
                "El Respiro Pablo watched marker",
                new Vector3(4.4f, 1.4f, 47f),
                new Vector3(0.3f, 2.8f, 2.8f),
                patrolBlue,
                "El Respiro changes hands with police eyes nearby",
                new Color(0.08f, 0.16f, 0.28f),
                new Color(0.22f, 0.56f, 0.38f));
            AddReactionMarker(
                PrototypeWorldEvent.FrontTakenUnderWatch,
                "Barrio reaction to front marker",
                new Vector3(-3.7f, 0.85f, 46.5f),
                new Vector3(0.7f, 1.7f, 0.7f),
                routeGreen,
                "Barrio notices El Respiro now belongs to Pablo",
                new Color(0.18f, 0.42f, 0.32f),
                new Color(0.42f, 0.66f, 0.82f));

            var seizure = CreateCube("Dirty cash seizure failstate", new Vector3(-2.7f, 0.45f, 43.5f), new Vector3(1.1f, 0.9f, 1.1f), patrolBlue);
            seizure.AddComponent<PrototypeInteractable>().Configure(
                "Lose dirty cash",
                "Dirty cash seized, operation continues wounded",
                PrototypeWorldEvent.DirtyCashSeized);

            AddReactionMarker(
                PrototypeWorldEvent.DirtyCashSeized,
                "Seized cash partial failure marker",
                new Vector3(-4.2f, 0.7f, 43.5f),
                new Vector3(0.8f, 1.4f, 0.8f),
                patrolBlue,
                "Dirty cash is seized without restarting the slice",
                new Color(0.08f, 0.16f, 0.28f),
                new Color(0.72f, 0.12f, 0.1f));
        }

        private static void AddReactionMarker(
            PrototypeWorldEvent reactsTo,
            string name,
            Vector3 position,
            Vector3 scale,
            Material material,
            string message,
            Color idleColor,
            Color reactedColor)
        {
            var marker = CreateCube(name, position, scale, material);
            marker.AddComponent<PrototypeWorldReactionMarker>().Configure(
                reactsTo,
                message,
                idleColor,
                reactedColor);
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

        private static void CreateRoute(Material routeGreen)
        {
            var routeObject = new GameObject("Phase 1 Route Progress");
            var route = routeObject.AddComponent<PrototypeRouteProgress>();
            route.Configure(5);

            CreateCheckpoint(route, 0, "Start on foot", new Vector3(0f, 0.25f, -10f), routeGreen);
            CreateCheckpoint(route, 1, "Enter vehicle lane", new Vector3(-2.4f, 0.25f, -4f), routeGreen);
            CreateCheckpoint(route, 2, "Patrol pressure turn", new Vector3(0f, 0.25f, 34f), routeGreen);
            CreateCheckpoint(route, 3, "Workshop interaction stop", new Vector3(2.5f, 0.25f, 49f), routeGreen);
            CreateCheckpoint(route, 4, "Safe return", new Vector3(0f, 0.25f, -8f), routeGreen);
        }

        private static void CreateCheckpoint(PrototypeRouteProgress route, int index, string label, Vector3 position, Material material)
        {
            var checkpoint = CreateCube($"Route checkpoint {index}: {label}", position, new Vector3(3.4f, 0.5f, 1.4f), material);
            checkpoint.GetComponent<Collider>().isTrigger = true;
            var routeCheckpoint = checkpoint.AddComponent<PrototypeRouteCheckpoint>();
            routeCheckpoint.Configure(route, index, label);
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

        private static void CreateWorldState()
        {
            var worldState = new GameObject("Prototype World State");
            worldState.AddComponent<PrototypeWorldState>();
        }

        private static void CreateMissionSpine()
        {
            var mission = new GameObject("Pierwszy Front Mission Spine");
            mission.AddComponent<PrototypeMissionSpine>();
        }

        private static void CreateRunMetrics()
        {
            var metrics = new GameObject("Phase 1 Run Metrics");
            metrics.AddComponent<PrototypeRunMetrics>();
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
