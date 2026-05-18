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
        private const string SettingsFolder = "Assets/Settings";
        private const string SliceDefinitionPath = "Assets/Settings/Phase1SliceDefinition.asset";

        [MenuItem("Valle de Plata/Build Phase 1 Feel Prototype Scene")]
        public static void BuildPhase1Scene()
        {
            EnsureFolder(MaterialFolder);
            EnsureFolder(SettingsFolder);
            var sliceDefinition = EnsureSliceDefinition();

            var concrete = CreateMaterial("Prototype_Concrete", new Color(0.56f, 0.54f, 0.48f));
            var asphalt = CreateMaterial("Prototype_Asphalt", new Color(0.18f, 0.18f, 0.17f));
            var sunBleachedWall = CreateMaterial("Prototype_SunBleachedWall", new Color(0.78f, 0.68f, 0.5f));
            var rust = CreateMaterial("Prototype_Rust", new Color(0.55f, 0.22f, 0.13f));
            var patrolBlue = CreateMaterial("Prototype_PatrolBlue", new Color(0.08f, 0.16f, 0.28f));
            var routeGreen = CreateMaterial("Prototype_RouteGreen", new Color(0.18f, 0.42f, 0.32f));

            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            SceneManager.SetActiveScene(scene);

            RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Trilight;
            RenderSettings.ambientSkyColor = new Color(0.84f, 0.76f, 0.62f);
            RenderSettings.ambientEquatorColor = new Color(0.58f, 0.48f, 0.36f);
            RenderSettings.ambientGroundColor = new Color(0.25f, 0.21f, 0.17f);

            CreateLight();
            CreatePresentationFillLight();
            CreateEnvironment(concrete, asphalt, sunBleachedWall, rust, patrolBlue, routeGreen);
            var player = CreatePlayer();
            CreateVehicle();
            CreateRoute(routeGreen, sliceDefinition);
            CreateWorldState();
            var mission = CreateMissionSpine();
            var objectiveMarker = CreateObjectiveMarker(mission);
            CreatePlayerHud(objectiveMarker);
            CreateCamera(player);
            CreateRunMetrics();
            CreateCursorController();
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
            light.intensity = 1.58f;
            light.shadows = LightShadows.Soft;
            lightObject.transform.rotation = Quaternion.Euler(48f, -35f, 0f);
        }

        private static void CreatePresentationFillLight()
        {
            var lightObject = new GameObject("Warm presentation fill light");
            var light = lightObject.AddComponent<Light>();
            light.type = LightType.Directional;
            light.intensity = 0.52f;
            light.color = new Color(1f, 0.84f, 0.62f);
            light.shadows = LightShadows.None;
            lightObject.transform.rotation = Quaternion.Euler(32f, 145f, 0f);
        }

        private static void CreateEnvironment(Material concrete, Material asphalt, Material wall, Material rust, Material patrolBlue, Material routeGreen)
        {
            CreateCube("Ground", new Vector3(0f, -0.05f, 12f), new Vector3(42f, 0.1f, 72f), concrete);
            CreateCube("Narrow asphalt route", new Vector3(0f, 0f, 12f), new Vector3(7.4f, 0.05f, 62f), asphalt);
            CreateCube("Left barrio wall", new Vector3(-4.9f, 1.25f, 8f), new Vector3(0.7f, 2.5f, 48f), wall);
            CreateCube("Right barrio wall", new Vector3(4.9f, 1.25f, 6f), new Vector3(0.7f, 2.5f, 44f), wall);
            CreateCube("Tight corner block", new Vector3(2.1f, 1.6f, 28f), new Vector3(7f, 3.2f, 0.8f), wall);
            CreateCube("Return lane blocker", new Vector3(-2.2f, 0.6f, 40f), new Vector3(2.2f, 1.2f, 1.2f), rust);
            CreateFoundationProofGeometry(wall, rust);
            var workshop = CreateCube("Workshop shutter interactable", new Vector3(3.85f, 1.15f, 49f), new Vector3(0.35f, 2.3f, 4.3f), rust);
            workshop.AddComponent<PrototypeInteractable>();
            PrototypeLayers.SetLayerRecursively(workshop, PrototypeLayers.Interactable);
            CreateCube("Static civilian car obstacle", new Vector3(-2.35f, 0.45f, 18f), new Vector3(2f, 0.9f, 4f), rust);
            var pressureMarker = CreatePublicViolenceMicrotest(rust, patrolBlue, routeGreen);
            var roadblock = CreateBribeMicrotest(rust, patrolBlue, routeGreen);
            CreateMateoMicrotest(rust, patrolBlue, routeGreen);
            CreateFrontPrototypeMicrotest(rust, patrolBlue, routeGreen);

            var patrol = CreateCube("Pressure patrol marker", new Vector3(0f, 0.55f, 34f), new Vector3(2.2f, 1.1f, 2.2f), patrolBlue);
            patrol.GetComponent<Collider>().isTrigger = true;
            var pressureZone = patrol.AddComponent<PrototypePressureZone>();
            var pressureChoice = patrol.AddComponent<PrototypePressureChoiceController>();
            SetObjectReference(pressureZone, "choiceController", pressureChoice);
            var playback = patrol.AddComponent<PrototypePressureScenePlayback>();
            SetObjectReference(playback, "patrolMarker", pressureMarker.transform);
            SetObjectReference(playback, "roadblockMarker", roadblock.transform);
            PrototypeLayers.SetLayerRecursively(patrol, PrototypeLayers.SensorTrigger);

            var safeReturn = CreateCube("Safe return marker", new Vector3(0f, 0.04f, -8f), new Vector3(6f, 0.08f, 2.2f), routeGreen);
            PrototypeLayers.SetLayerRecursively(safeReturn, PrototypeLayers.CameraIgnore);

            CreateBelievabilityDressing(wall, rust, patrolBlue, routeGreen);
        }

        private static void CreateBelievabilityDressing(Material wall, Material rust, Material patrolBlue, Material routeGreen)
        {
            var signRed = CreateMaterial("Prototype_DressingSignRed", new Color(0.62f, 0.14f, 0.1f));
            var fadedBlue = CreateMaterial("Prototype_DressingFadedBlue", new Color(0.17f, 0.33f, 0.48f));
            var sunYellow = CreateMaterial("Prototype_DressingSunYellow", new Color(0.86f, 0.62f, 0.18f));
            var clothWhite = CreateMaterial("Prototype_DressingClothWhite", new Color(0.86f, 0.83f, 0.72f));
            var warmWood = CreateMaterial("Prototype_DressingWarmWood", new Color(0.43f, 0.28f, 0.16f));
            var darkMetal = CreateMaterial("Prototype_DressingDarkMetal", new Color(0.1f, 0.11f, 0.12f));

            var streetIdentity = CreateReadablePropGroup(
                "Barrio street identity prop",
                PrototypeReadablePropKind.StreetIdentity,
                "Barrio Hondo street identity",
                "street route",
                new Vector3(0f, 0f, 12f)).transform;
            var safeReturn = CreateReadablePropGroup(
                "Safe return readable prop",
                PrototypeReadablePropKind.SafeReturn,
                "Safe return alley",
                "pressure escape",
                new Vector3(0f, 0f, -8.9f)).transform;
            var riosCheckpoint = CreateReadablePropGroup(
                "Rios checkpoint readable prop",
                PrototypeReadablePropKind.RiosCheckpoint,
                "Rios checkpoint",
                "bribe branch",
                new Vector3(2.6f, 0f, 22.2f)).transform;
            var roadblock = CreateReadablePropGroup(
                "Police roadblock readable prop",
                PrototypeReadablePropKind.PoliceRoadblock,
                "Police roadblock",
                "pressure route gate",
                new Vector3(0f, 0f, 24.5f)).transform;
            var elRespiro = CreateReadablePropGroup(
                "El Respiro readable prop",
                PrototypeReadablePropKind.Workshop,
                "El Respiro workshop",
                "front takeover",
                new Vector3(3.8f, 0f, 48.2f)).transform;

            CreateDressingCube("Barrio Hondo overhead street sign", new Vector3(0f, 3.05f, -7.2f), new Vector3(5.8f, 0.36f, 0.18f), signRed, parent: streetIdentity);
            CreateWorldText(
                "Barrio Hondo overhead sign text",
                "BARRIO HONDO",
                new Vector3(0f, 3.08f, -7.33f),
                new Vector3(0f, 180f, 0f),
                0.16f,
                Color.white,
                streetIdentity);

            CreateDressingCube("Safe return alley arch", new Vector3(0f, 2.15f, -8.9f), new Vector3(6.8f, 0.32f, 0.42f), darkMetal, parent: safeReturn);
            CreateDressingCube("Safe return alley arch left pillar", new Vector3(-3.25f, 1.05f, -8.9f), new Vector3(0.24f, 2.1f, 0.42f), darkMetal, parent: safeReturn);
            CreateDressingCube("Safe return alley arch right pillar", new Vector3(3.25f, 1.05f, -8.9f), new Vector3(0.24f, 2.1f, 0.42f), darkMetal, parent: safeReturn);
            CreateDressingCube("Safe return painted arrow", new Vector3(0f, 0.08f, -8.95f), new Vector3(1.8f, 0.025f, 0.7f), sunYellow, parent: safeReturn);

            CreateDressingCube("Laundry line north", new Vector3(-4.55f, 2.75f, 6.5f), new Vector3(0.05f, 0.05f, 13.5f), darkMetal, parent: streetIdentity);
            CreateDressingCube("Laundry cloth red", new Vector3(-4.52f, 2.35f, 3.7f), new Vector3(0.05f, 0.72f, 0.55f), rust, parent: streetIdentity);
            CreateDressingCube("Laundry cloth white", new Vector3(-4.52f, 2.28f, 6.1f), new Vector3(0.05f, 0.62f, 0.75f), clothWhite, parent: streetIdentity);
            CreateDressingCube("Laundry cloth blue", new Vector3(-4.52f, 2.32f, 9.2f), new Vector3(0.05f, 0.68f, 0.65f), fadedBlue, parent: streetIdentity);

            CreateDressingCube("Witness balcony cluster", new Vector3(-5.32f, 2.25f, 12.2f), new Vector3(0.42f, 0.22f, 3.2f), warmWood, parent: streetIdentity);
            CreateDressingCube("Witness balcony rail", new Vector3(-5.05f, 2.55f, 12.2f), new Vector3(0.12f, 0.55f, 3.4f), darkMetal, parent: streetIdentity);
            CreateDressingCube("Witness balcony shade", new Vector3(-5.2f, 3.02f, 12.2f), new Vector3(0.52f, 0.18f, 3.5f), fadedBlue, parent: streetIdentity);

            CreateDressingCube("Rios checkpoint desk", new Vector3(2.35f, 0.48f, 22.1f), new Vector3(1.35f, 0.72f, 0.82f), warmWood, parent: riosCheckpoint);
            CreateDressingCube("Rios checkpoint awning", new Vector3(3.05f, 1.95f, 22.25f), new Vector3(2.35f, 0.16f, 2.2f), sunYellow, parent: riosCheckpoint);
            CreateDressingCube("Rios checkpoint stool", new Vector3(1.62f, 0.28f, 22.6f), new Vector3(0.42f, 0.56f, 0.42f), warmWood, parent: riosCheckpoint);
            CreateDressingCube("Rios checkpoint papers", new Vector3(2.32f, 0.86f, 21.92f), new Vector3(0.62f, 0.035f, 0.42f), clothWhite, parent: riosCheckpoint);
            CreateWorldText(
                "Rios checkpoint placard",
                "RIOS",
                new Vector3(2.35f, 0.95f, 21.63f),
                new Vector3(0f, 180f, 0f),
                0.1f,
                Color.white,
                riosCheckpoint);

            CreateDressingCube("Police roadblock barricade left", new Vector3(-1.35f, 0.55f, 24.5f), new Vector3(1.85f, 0.32f, 0.28f), patrolBlue, parent: roadblock);
            CreateDressingCube("Police roadblock barricade right", new Vector3(1.35f, 0.55f, 24.5f), new Vector3(1.85f, 0.32f, 0.28f), patrolBlue, parent: roadblock);
            CreateDressingCube("Police roadblock warning stripe left", new Vector3(-1.35f, 0.76f, 24.32f), new Vector3(1.45f, 0.09f, 0.06f), sunYellow, parent: roadblock);
            CreateDressingCube("Police roadblock warning stripe right", new Vector3(1.35f, 0.76f, 24.32f), new Vector3(1.45f, 0.09f, 0.06f), sunYellow, parent: roadblock);
            CreateDressingCube("Police roadblock cone left", new Vector3(-2.7f, 0.32f, 24.35f), new Vector3(0.38f, 0.64f, 0.38f), rust, PrimitiveType.Cylinder, roadblock);
            CreateDressingCube("Police roadblock cone right", new Vector3(2.7f, 0.32f, 24.35f), new Vector3(0.38f, 0.64f, 0.38f), rust, PrimitiveType.Cylinder, roadblock);

            CreateDressingCube("Rooftop water tank", new Vector3(-5.18f, 3.35f, 29f), new Vector3(1.1f, 0.95f, 1.1f), darkMetal, PrimitiveType.Cylinder, streetIdentity);
            CreateDressingCube("Rooftop water tank base", new Vector3(-5.18f, 2.82f, 29f), new Vector3(1.2f, 0.16f, 1.2f), wall, parent: streetIdentity);

            CreateDressingCube("Barrio crate stack", new Vector3(-3.65f, 0.42f, 43f), new Vector3(1f, 0.84f, 0.85f), warmWood, parent: streetIdentity);
            CreateDressingCube("Barrio crate stack top", new Vector3(-3.18f, 1.08f, 43.2f), new Vector3(0.72f, 0.48f, 0.62f), warmWood, parent: streetIdentity);

            CreateDressingCube("El Respiro workshop sign", new Vector3(3.58f, 2.65f, 48.2f), new Vector3(0.24f, 0.72f, 3.65f), signRed, parent: elRespiro);
            CreateWorldText(
                "El Respiro workshop sign text",
                "EL RESPIRO",
                new Vector3(3.43f, 2.66f, 48.2f),
                new Vector3(0f, -90f, 0f),
                0.12f,
                Color.white,
                elRespiro);
            CreateDressingCube("El Respiro shutter slat 0", new Vector3(3.37f, 1.65f, 46.65f), new Vector3(0.12f, 0.08f, 2.55f), darkMetal, parent: elRespiro);
            CreateDressingCube("El Respiro shutter slat 1", new Vector3(3.37f, 1.25f, 46.65f), new Vector3(0.12f, 0.08f, 2.55f), darkMetal, parent: elRespiro);
            CreateDressingCube("El Respiro shutter slat 2", new Vector3(3.37f, 0.85f, 46.65f), new Vector3(0.12f, 0.08f, 2.55f), darkMetal, parent: elRespiro);
            CreateDressingCube("El Respiro door lamp", new Vector3(3.34f, 2.05f, 45.05f), new Vector3(0.24f, 0.24f, 0.24f), sunYellow, PrimitiveType.Sphere, elRespiro);

            CreatePresentationFacades(elRespiro);
        }

        private static void CreatePresentationFacades(Transform workshopParent)
        {
            var plasterWarm = CreateMaterial("Prototype_FacadeWarmPlaster", new Color(0.83f, 0.63f, 0.38f));
            var tealPaint = CreateMaterial("Prototype_FacadeFadedTeal", new Color(0.23f, 0.52f, 0.55f));
            var awningRed = CreateMaterial("Prototype_FacadeAwningRed", new Color(0.72f, 0.19f, 0.13f));
            var dust = CreateMaterial("Prototype_RoadDust", new Color(0.68f, 0.54f, 0.35f));

            CreateDressingCube("Left sunlit plaster facade", new Vector3(-5.31f, 1.74f, 2.5f), new Vector3(0.12f, 1.28f, 18f), plasterWarm);
            CreateDressingCube("Right faded teal facade", new Vector3(5.31f, 1.62f, 18f), new Vector3(0.12f, 1.15f, 18f), tealPaint);
            CreateDressingCube("Market awning strip", new Vector3(4.38f, 2.38f, 13.5f), new Vector3(1.25f, 0.16f, 4.6f), awningRed);
            CreateDressingCube("Workshop plaster return", new Vector3(4.18f, 1.82f, 47.3f), new Vector3(0.16f, 1.15f, 5.8f), plasterWarm, parent: workshopParent);
            CreateDressingCube("Pressure road dust band", new Vector3(0f, 0.085f, 31f), new Vector3(6.1f, 0.02f, 13.5f), dust);
        }

        private static void CreateFoundationProofGeometry(Material wall, Material rust)
        {
            CreateCube("Motor proof low step", new Vector3(-8f, 0.15f, -10.5f), new Vector3(1.6f, 0.3f, 0.55f), wall);
            CreateCube("Motor proof high wall", new Vector3(-8f, 0.75f, -9.3f), new Vector3(1.8f, 1.5f, 0.25f), wall);

            var slope = CreateCube("Motor proof steep slope", new Vector3(-6.6f, 0.35f, -9.7f), new Vector3(1.8f, 0.22f, 2.2f), rust);
            slope.transform.rotation = Quaternion.Euler(0f, 0f, -58f);

            CreateCube("Tight camera recovery wall", new Vector3(8f, 1.7f, -13.5f), new Vector3(0.45f, 3.4f, 4.4f), wall);
        }

        private static GameObject CreatePublicViolenceMicrotest(Material rust, Material patrolBlue, Material routeGreen)
        {
            var target = CreateCube("Public violence test target", new Vector3(-3.1f, 0.8f, 10f), new Vector3(0.7f, 1.6f, 0.7f), rust);
            PrototypeLayers.SetLayerRecursively(target, PrototypeLayers.Interactable);
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
                "Civilians scatter after Pablo's show of force",
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
            var pressureMarker = AddReactionMarker(
                PrototypeWorldEvent.PublicViolenceCommitted,
                "Police pressure moves closer marker",
                new Vector3(1.6f, 0.55f, 18f),
                new Vector3(1.6f, 1.1f, 1.6f),
                patrolBlue,
                "Patrol shifts closer after witnesses talk",
                new Color(0.08f, 0.16f, 0.28f),
                new Color(0.08f, 0.32f, 0.62f));
            return pressureMarker;
        }

        private static GameObject CreateBribeMicrotest(Material rust, Material patrolBlue, Material routeGreen)
        {
            var officer = CreateCube("Rios bribe test officer", new Vector3(3.15f, 0.85f, 22f), new Vector3(0.75f, 1.7f, 0.75f), patrolBlue);
            PrototypeLayers.SetLayerRecursively(officer, PrototypeLayers.Interactable);
            officer.AddComponent<PrototypeInteractable>().Configure(
                "Pay Rios bribe",
                "Rios lets Pablo pass but remembers",
                PrototypeWorldEvent.BribeAccepted);

            var roadblock = AddReactionMarker(
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
            return roadblock;
        }

        private static void CreateMateoMicrotest(Material rust, Material patrolBlue, Material routeGreen)
        {
            var trusted = CreateCube("Mateo protected test contact", new Vector3(-3.2f, 0.85f, 31f), new Vector3(0.75f, 1.7f, 0.75f), routeGreen);
            PrototypeLayers.SetLayerRecursively(trusted, PrototypeLayers.Interactable);
            trusted.AddComponent<PrototypeInteractable>().Configure(
                "Protect Mateo",
                "Mateo warns early",
                PrototypeWorldEvent.MateoProtected);

            var humiliated = CreateCube("Mateo humiliated test contact", new Vector3(3.2f, 0.85f, 31f), new Vector3(0.75f, 1.7f, 0.75f), rust);
            PrototypeLayers.SetLayerRecursively(humiliated, PrototypeLayers.Interactable);
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
            PrototypeLayers.SetLayerRecursively(cash, PrototypeLayers.Interactable);
            cash.AddComponent<PrototypeInteractable>().Configure(
                "Pick up dirty cash",
                "Dirty cash is now Pablo's risk",
                PrototypeWorldEvent.DirtyCashPickedUp);

            var front = CreateCube("El Respiro front takeover", new Vector3(3.55f, 1.3f, 47f), new Vector3(0.45f, 2.6f, 2.6f), rust);
            PrototypeLayers.SetLayerRecursively(front, PrototypeLayers.Interactable);
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
            PrototypeLayers.SetLayerRecursively(seizure, PrototypeLayers.Interactable);
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

        private static GameObject AddReactionMarker(
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
            PrototypeLayers.SetLayerRecursively(marker, PrototypeLayers.CameraIgnore);
            marker.AddComponent<PrototypeWorldReactionMarker>().Configure(
                reactsTo,
                message,
                idleColor,
                reactedColor);
            return marker;
        }

        private static PrototypePlayerController CreatePlayer()
        {
            var playerObject = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            playerObject.name = "Pablo Valera Prototype Controller";
            playerObject.transform.position = new Vector3(0f, 1f, -12f);
            PrototypeLayers.SetLayerRecursively(playerObject, PrototypeLayers.Player);

            playerObject.AddComponent<PrototypeCharacterMotor>();

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
            PrototypeLayers.SetLayerRecursively(vehicle, PrototypeLayers.Vehicle);
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

            var fallbackExitPoint = new GameObject("Fallback Exit Point").transform;
            fallbackExitPoint.SetParent(vehicle.transform);
            fallbackExitPoint.localPosition = new Vector3(1.8f, 0.2f, -0.6f);
            SetObjectReference(controller, "fallbackExitPoint", fallbackExitPoint);
        }

        private static void CreateRoute(Material routeGreen, PrototypeSliceDefinition sliceDefinition)
        {
            var routeObject = new GameObject("Phase 1 Route Progress");
            var route = routeObject.AddComponent<PrototypeRouteProgress>();
            var checkpoints = sliceDefinition.RouteCheckpoints;
            route.Configure(checkpoints.Length);

            for (var index = 0; index < checkpoints.Length; index++)
            {
                CreateCheckpoint(route, index, checkpoints[index].Label, checkpoints[index].Position, checkpoints[index].Scale, routeGreen);
            }
        }

        private static void CreateCheckpoint(PrototypeRouteProgress route, int index, string label, Vector3 position, Vector3 scale, Material material)
        {
            var checkpoint = CreateCube($"Route checkpoint {index}: {label}", position, scale, material);
            checkpoint.GetComponent<Collider>().isTrigger = true;
            PrototypeLayers.SetLayerRecursively(checkpoint, PrototypeLayers.RouteTrigger);
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
            SetLayerMask(cameraRig, "collisionMask", PrototypeLayers.CameraCollisionMask);
        }

        private static void CreateDebugHud()
        {
            var debug = new GameObject("Prototype Debug HUD");
            var debugHud = debug.AddComponent<PrototypeDebugHud>();
            SetBool(debugHud, "visible", false);
        }

        private static void CreateCursorController()
        {
            var cursor = new GameObject("Prototype Cursor Controller");
            cursor.AddComponent<PrototypeCursorController>();
        }

        private static void CreateWorldState()
        {
            var worldState = new GameObject("Prototype World State");
            worldState.AddComponent<PrototypeWorldState>();
        }

        private static PrototypeMissionSpine CreateMissionSpine()
        {
            var mission = new GameObject("Pierwszy Front Mission Spine");
            return mission.AddComponent<PrototypeMissionSpine>();
        }

        private static PrototypeObjectiveMarker CreateObjectiveMarker(PrototypeMissionSpine mission)
        {
            var marker = new GameObject("Prototype Objective Marker");
            var objectiveMarker = marker.AddComponent<PrototypeObjectiveMarker>();
            SetObjectReference(objectiveMarker, "missionSpine", mission);
            return objectiveMarker;
        }

        private static void CreatePlayerHud(PrototypeObjectiveMarker objectiveMarker)
        {
            var hud = new GameObject("Prototype Player HUD");
            var playerHud = hud.AddComponent<PrototypePlayerHud>();
            SetObjectReference(playerHud, "objectiveMarker", objectiveMarker);
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
            PrototypeLayers.SetLayerRecursively(cube, PrototypeLayers.WorldStatic);
            return cube;
        }

        private static PrototypeReadableProp CreateReadablePropGroup(
            string name,
            PrototypeReadablePropKind kind,
            string displayName,
            string gameplayAnchor,
            Vector3 position)
        {
            var group = new GameObject(name);
            group.transform.position = position;
            PrototypeLayers.SetLayerRecursively(group, PrototypeLayers.CameraIgnore);

            var readableProp = group.AddComponent<PrototypeReadableProp>();
            readableProp.Configure(kind, displayName, gameplayAnchor);
            return readableProp;
        }

        private static GameObject CreateDressingCube(
            string name,
            Vector3 position,
            Vector3 scale,
            Material material,
            PrimitiveType primitiveType = PrimitiveType.Cube,
            Transform parent = null)
        {
            var dressing = GameObject.CreatePrimitive(primitiveType);
            dressing.name = name;
            dressing.transform.position = position;
            dressing.transform.localScale = scale;
            if (parent != null)
            {
                dressing.transform.SetParent(parent, true);
            }

            dressing.GetComponent<Renderer>().sharedMaterial = material;
            var collider = dressing.GetComponent<Collider>();
            if (collider != null)
            {
                collider.enabled = false;
            }

            PrototypeLayers.SetLayerRecursively(dressing, PrototypeLayers.CameraIgnore);
            return dressing;
        }

        private static GameObject CreateWorldText(
            string name,
            string text,
            Vector3 position,
            Vector3 rotation,
            float characterSize,
            Color color,
            Transform parent = null)
        {
            var textObject = new GameObject(name);
            textObject.transform.position = position;
            textObject.transform.rotation = Quaternion.Euler(rotation);
            textObject.transform.localScale = new Vector3(-1f, 1f, 1f);
            if (parent != null)
            {
                textObject.transform.SetParent(parent, true);
            }

            var mesh = textObject.AddComponent<TextMesh>();
            mesh.text = text;
            mesh.anchor = TextAnchor.MiddleCenter;
            mesh.alignment = TextAlignment.Center;
            mesh.characterSize = characterSize;
            mesh.fontSize = 72;
            mesh.color = color;
            PrototypeLayers.SetLayerRecursively(textObject, PrototypeLayers.CameraIgnore);
            return textObject;
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

        private static void SetLayerMask(Object target, string propertyName, int value)
        {
            var serializedObject = new SerializedObject(target);
            serializedObject.FindProperty(propertyName).intValue = value;
            serializedObject.ApplyModifiedPropertiesWithoutUndo();
        }

        private static void SetBool(Object target, string propertyName, bool value)
        {
            var serializedObject = new SerializedObject(target);
            serializedObject.FindProperty(propertyName).boolValue = value;
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

        private static PrototypeSliceDefinition EnsureSliceDefinition()
        {
            var definition = AssetDatabase.LoadAssetAtPath<PrototypeSliceDefinition>(SliceDefinitionPath);
            if (definition == null)
            {
                definition = ScriptableObject.CreateInstance<PrototypeSliceDefinition>();
                definition.ConfigurePhase1Defaults();
                AssetDatabase.CreateAsset(definition, SliceDefinitionPath);
            }
            else if (!definition.Validate(out _))
            {
                definition.ConfigurePhase1Defaults();
                EditorUtility.SetDirty(definition);
            }

            return definition;
        }
    }
}
