// ============================================================
// RoomBuilder.cs
// Smart Relaxation Room — SE4051 Individual Assignment
// Author: Ishara Kumarage
//
// OVERHAUL VERSION: Generates 26 physical Material assets, 
// sets up project folders, builds the room, adds particles,
// and configures a clean, light-themed top-left UI.
// ============================================================

#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class RoomBuilder : MonoBehaviour
{
    private static List<Renderer> tintableRenderers = new List<Renderer>();

    [MenuItem("Tools/Smart Relaxation Room/Build Full Scene")]
    public static void BuildScene()
    {
        Debug.Log("=== RoomBuilder: Starting Project Overhaul construction ===");
        
        // ── PRE-CLEANUP ──────────────────────────────────────────
        // Destroy existing root objects in the scene to avoid duplicates 
        // (like 5 AudioListeners or overly bright lights from multiple Directional Lights)
        var rootObjects = UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects();
        foreach (var go in rootObjects)
        {
            GameObject.DestroyImmediate(go);
        }

        tintableRenderers.Clear();

        // ── 0. FOLDER SETUP ──────────────────────────────────────
        string[] folders = { "Materials", "Textures", "UI", "Prefabs", "Scenes" };
        foreach (string f in folders)
        {
            if (!AssetDatabase.IsValidFolder("Assets/" + f))
            {
                AssetDatabase.CreateFolder("Assets", f);
            }
        }
        AssetDatabase.SaveAssets();

        // Load existing wood texture if available
        Texture2D woodTex = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Textures/Wood_Albedo.png");

        // ── 1. GENERATE 26 MATERIALS (.mat files) ────────────────
        // (1) Wooden floor
        Material matFloor = GetOrCreateMaterial("Assets/Materials/1_WoodenFloor.mat", Color.white, 0.3f, 0.0f, woodTex);
        // (2) Matte painted wall
        Material matWallMatte = GetOrCreateMaterial("Assets/Materials/2_MatteWall.mat", new Color(0.18f, 0.22f, 0.24f), 0.1f, 0f);
        // (3) Calm mode blue wall
        Material matWallCalm = GetOrCreateMaterial("Assets/Materials/3_WallCalm.mat", new Color(0.4f, 0.6f, 1.0f), 0.1f, 0f);
        // (4) Energy mode orange wall
        Material matWallEnergy = GetOrCreateMaterial("Assets/Materials/4_WallEnergy.mat", new Color(1.0f, 0.65f, 0.2f), 0.1f, 0f);
        // (5) Focus mode white wall
        Material matWallFocus = GetOrCreateMaterial("Assets/Materials/5_WallFocus.mat", new Color(0.95f, 0.97f, 1.0f), 0.1f, 0f);
        // (6) Wooden study table
        Material matWoodTable = GetOrCreateMaterial("Assets/Materials/6_WoodenTable.mat", Color.white, 0.3f, 0f, woodTex);
        // (7) Dark chair fabric
        Material matChairFabric = GetOrCreateMaterial("Assets/Materials/7_DarkChairFabric.mat", new Color(0.1f, 0.1f, 0.1f), 0.1f, 0.0f);
        // (8) Deep teal bean bag
        Material matBeanBag = GetOrCreateMaterial("Assets/Materials/8_TealBeanBag.mat", new Color(0.15f, 0.25f, 0.35f), 0.1f, 0f);
        // (9) Grey circular rug
        Material matRug = GetOrCreateMaterial("Assets/Materials/9_GreyRug.mat", new Color(0.2f, 0.2f, 0.2f), 0f, 0f);
        // (10) Cream lamp shade
        Material matLampShade = GetOrCreateMaterial("Assets/Materials/10_CreamLampShade.mat", new Color(0.9f, 0.85f, 0.7f), 0f, 0f);
        // (11) Warm lamp glow (emissive)
        Material matLampGlow = GetOrCreateMaterial("Assets/Materials/11_WarmLampGlow.mat", new Color(1f, 0.9f, 0.7f), 0f, 0f, null, false, new Color(1f, 0.8f, 0.4f) * 2f);
        // (12) Indoor plant leaves
        Material matLeaves = GetOrCreateMaterial("Assets/Materials/12_PlantLeaves.mat", new Color(0.2f, 0.5f, 0.2f), 0.2f, 0f);
        // (13) Ceramic plant pot
        Material matPot = GetOrCreateMaterial("Assets/Materials/13_CeramicPot.mat", new Color(0.6f, 0.4f, 0.2f), 0.6f, 0f);
        // (14) Dark wooden bookshelf
        Material matBookshelf = GetOrCreateMaterial("Assets/Materials/14_DarkBookshelf.mat", Color.white, 0.3f, 0f, woodTex);
        // (15) Colorful book spines
        Material matBookSpines = GetOrCreateMaterial("Assets/Materials/15_ColorfulBooks.mat", new Color(0.8f, 0.3f, 0.3f), 0.1f, 0f);
        // (16) Dark wooden window frame
        Material matWindowFrame = GetOrCreateMaterial("Assets/Materials/16_DarkWindowFrame.mat", new Color(0.15f, 0.1f, 0.05f), 0.2f, 0f);
        // (17) Slightly transparent glass window
        Material matGlass = GetOrCreateMaterial("Assets/Materials/17_GlassWindow.mat", new Color(0.8f, 0.9f, 1.0f, 0.3f), 0.9f, 0.5f, null, true);
        // (18) Motivation board/poster
        Material matBoard = GetOrCreateMaterial("Assets/Materials/18_MotivationBoard.mat", new Color(0.85f, 0.85f, 0.82f), 0.1f, 0f);
        // (19) Transparent glowing trigger zone (barely visible outline effect)
        Material matTriggerZone = GetOrCreateMaterial("Assets/Materials/19_TriggerZone.mat", new Color(1.0f, 1.0f, 1.0f, 0.05f), 0f, 0f, null, true);
        // (20-26) Handled via UI and Particle systems below.

        AssetDatabase.SaveAssets();

        // ── 2. ROOM PARENT & ARCHITECTURE ────────────────────────
        GameObject room = new GameObject("Room");
        
        string pfxBuild = "Assets/Brick Project Studio/Apartment Kit/_Prefabs/Apt Build Kit/";
        string pfxFurn = "Assets/Brick Project Studio/Apartment Kit/_Prefabs/Furniture/";
        string pfxProp = "Assets/Brick Project Studio/Apartment Kit/_Prefabs/Props/";

        string floorPrefabPath = pfxBuild + "Interiors/Flooring & Ceilings/Int_apt_01_Floor_01.prefab";
        
        // Room dimensions (fixed, reliable)
        float W = 10f, D = 10f, H = 4f;
        float wallThickness = 0.2f;

        // ─── SOLID WALLS (guaranteed sealed room, no gaps) ───
        GameObject wallBack  = CreateCube("WallBack",   room, new Vector3(0, H/2f,  D/2f),  new Vector3(W + wallThickness*2, H, wallThickness), matWallMatte);
        GameObject wallFront = CreateCube("WallFront",  room, new Vector3(0, H/2f, -D/2f),  new Vector3(W + wallThickness*2, H, wallThickness), matWallMatte);
        GameObject wallLeft  = CreateCube("WallLeft",   room, new Vector3(-W/2f, H/2f, 0),   new Vector3(wallThickness, H, D), matWallMatte);
        GameObject wallRight = CreateCube("WallRight",  room, new Vector3( W/2f, H/2f, 0),   new Vector3(wallThickness, H, D), matWallMatte);
        GameObject ceiling   = CreateCube("Ceiling",    room, new Vector3(0, H, 0),           new Vector3(W, wallThickness, D), matWallMatte);
        tintableRenderers.Add(wallBack.GetComponent<Renderer>());
        tintableRenderers.Add(wallFront.GetComponent<Renderer>());
        tintableRenderers.Add(wallLeft.GetComponent<Renderer>());
        tintableRenderers.Add(wallRight.GetComponent<Renderer>());
        tintableRenderers.Add(ceiling.GetComponent<Renderer>());

        // ─── PREFAB FLOOR TILES (beautiful textured floor) ───
        // Measure the actual tile size dynamically
        float tileSize = 3f;
        GameObject dummyFloor = (GameObject)UnityEditor.PrefabUtility.InstantiatePrefab(
            UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>(floorPrefabPath));
        if (dummyFloor != null) {
            Renderer r = dummyFloor.GetComponentInChildren<Renderer>();
            if (r != null) tileSize = Mathf.Max(r.bounds.size.x, r.bounds.size.z);
            Object.DestroyImmediate(dummyFloor);
        }
        if (tileSize < 0.5f) tileSize = 3f; // safety fallback

        GameObject floorParent = new GameObject("Floors");
        floorParent.transform.SetParent(room.transform);
        int floorCountX = Mathf.CeilToInt(W / tileSize);
        int floorCountZ = Mathf.CeilToInt(D / tileSize);
        for (int x = 0; x < floorCountX; x++) {
            for (int z = 0; z < floorCountZ; z++) {
                Vector3 pos = new Vector3(
                    -W/2f + (x * tileSize) + tileSize/2f,
                    0,
                    -D/2f + (z * tileSize) + tileSize/2f);
                GameObject f = SpawnPrefab(floorPrefabPath, floorParent, pos, Vector3.one);
                if (f != null) tintableRenderers.AddRange(f.GetComponentsInChildren<Renderer>());
            }
        }

        // ─── DECORATIVE WALL PANELS (layered on inner wall faces) ───
        string wallPanelPath = pfxBuild + "Interiors/Walls Frames & Doors/Apt_01/Int_Apt_01_Wall_01.prefab";
        // Back wall panels (facing -Z, interior)
        for (float x = -W/2f + 1.5f; x < W/2f; x += 3f) {
            GameObject wp = SpawnPrefab(wallPanelPath, room, new Vector3(x, 0, D/2f - wallThickness/2f), Vector3.one);
            if (wp != null) {
                wp.transform.rotation = Quaternion.Euler(0, 180, 0);
                tintableRenderers.AddRange(wp.GetComponentsInChildren<Renderer>());
            }
        }
        // Left wall panels (facing +X, interior)
        for (float z = -D/2f + 1.5f; z < D/2f; z += 3f) {
            GameObject wp = SpawnPrefab(wallPanelPath, room, new Vector3(-W/2f + wallThickness/2f, 0, z), Vector3.one);
            if (wp != null) {
                wp.transform.rotation = Quaternion.Euler(0, 90, 0);
                tintableRenderers.AddRange(wp.GetComponentsInChildren<Renderer>());
            }
        }

        // ─── WINDOW on Right Wall ───
        SpawnPrefab(pfxBuild + "Interiors/Frames Windows Stairs & Doors/Window_Frame_Apt_01.prefab", room, new Vector3(W/2f - 0.15f, 0f, 0f), Quaternion.Euler(0, -90, 0));
        CreateCube("WindowGlass", room, new Vector3(W/2f - 0.15f, H/2f, 0f), new Vector3(0.05f, 2f, 2.5f), matGlass);
        for(int i=0; i<10; i++) {
            CreateCube("Blind" + i, room, new Vector3(W/2f - 0.18f, 1.2f + (i*0.2f), 0f), new Vector3(0.05f, 0.04f, 2.6f), matWindowFrame);
        }

        // ── 3. FURNITURE & PROPS ─────────────────────────────────

        // Rug
        GameObject rug = SpawnPrefab(pfxProp + "Art/Rug_Apt_01.prefab", room, new Vector3(0f, 0.05f, 0f), Vector3.one * 1.5f);

        // Desk (Computer Table with setup)
        Vector3 deskPos = new Vector3(0f, 0f, 4f);
        GameObject desk = SpawnPrefab(pfxFurn + "Living Room/Table_Computer_01_Setup.prefab", room, deskPos, Vector3.one);

        // Chair
        GameObject chairSeat = SpawnPrefab(pfxFurn + "Living Room/Chair_Apt_01.prefab", room, deskPos + new Vector3(0, 0f, -1.2f), Vector3.one);
        chairSeat.transform.rotation = Quaternion.Euler(0, 180, 0);

        // Bookshelf
        Vector3 shelfPos = new Vector3(-4.5f, 0f, 0f);
        GameObject book1 = SpawnPrefab(pfxFurn + "Living Room/Shelf_Apt_01.prefab", room, shelfPos, Vector3.one);
        book1.transform.rotation = Quaternion.Euler(0, 90, 0);

        // Wall Shelf for decoration
        SpawnPrefab(pfxFurn + "Living Room/WallShelf_Apt_01.prefab", room, new Vector3(-4.8f, 2.2f, -2f), Vector3.one);

        // Bean Bag (Using primitive sphere + matBeanBag)
        GameObject beanBag = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        beanBag.name = "BeanBag";
        beanBag.transform.SetParent(room.transform);
        beanBag.transform.localPosition = new Vector3(2.5f, 0.4f, 0f);
        beanBag.transform.localScale = new Vector3(1.5f, 0.8f, 1.5f);
        beanBag.GetComponent<Renderer>().sharedMaterial = matBeanBag;

        // Motivation Board on back wall
        GameObject board = CreateCube("MotivationBoard", room, new Vector3(0f, 2.5f, 4.85f), new Vector3(2f, 1.5f, 0.1f), matBoard);

        // Lamp (Floor Lamp with real light)
        Vector3 lampPos = new Vector3(-2.5f, 0f, 4f);
        GameObject lampPrefab = SpawnPrefab(pfxProp + "Lighting/Lamp_Floor_Apt_01.prefab", room, lampPos, Vector3.one);

        // Lamp Light (attached to the prefab lamp)
        GameObject lampLightObj = new GameObject("LampLight");
        lampLightObj.transform.SetParent(lampPrefab.transform);
        lampLightObj.transform.localPosition = new Vector3(0, 1.8f, 0);
        Light lampL = lampLightObj.AddComponent<Light>();
        lampL.type = LightType.Spot;
        lampL.spotAngle = 100f;
        lampL.color = new Color(1.0f, 0.85f, 0.6f);
        lampL.intensity = 3.0f;
        lampL.range = 8f;
        lampL.shadows = LightShadows.Soft;
        lampL.enabled = false;

        // Table Lamp on desk
        SpawnPrefab(pfxProp + "Lighting/Lamp_Table_Apt_01.prefab", room, deskPos + new Vector3(1f, 0.8f, 0.3f), Vector3.one);

        // Plant (Vase)
        Vector3 plantPos = new Vector3(3.5f, 0f, 4f);
        GameObject pot = SpawnPrefab(pfxProp + "Misc/Vase_Apt_01.prefab", room, plantPos, Vector3.one * 1.5f);

        // Drinking Glass on desk
        SpawnPrefab(pfxProp + "Kitchen/DrinkingGlass_01.prefab", room, deskPos + new Vector3(-0.8f, 0.75f, 0.2f), Vector3.one);

        // Extra Decoration Props
        SpawnPrefab(pfxProp + "Misc/Trash_apt_01.prefab", room, deskPos + new Vector3(-1.2f, 0f, -0.6f), Vector3.one);
        SpawnPrefab(pfxProp + "Misc/WelcomeMat_01.prefab", room, new Vector3(0f, 0.02f, -5.2f), Vector3.one);
        
        GameObject sideTable = SpawnPrefab(pfxFurn + "Living Room/Table_Coffee_01.prefab", room, new Vector3(4.5f, 0f, -2f), Vector3.one);
        sideTable.transform.rotation = Quaternion.Euler(0, 90, 0);
        SpawnPrefab(pfxProp + "Misc/Chess_Board_Base.prefab", room, new Vector3(4.5f, 0.45f, -2f), Vector3.one);
        
        SpawnPrefab(pfxProp + "Sculptures/Sculpture_apt_01_01.prefab", room, new Vector3(-4.5f, 0f, -4f), Vector3.one * 1.5f);
        SpawnPrefab(pfxProp + "Misc/Wall_Mirror_01.prefab", room, new Vector3(-5.9f, 1.5f, -2f), Quaternion.Euler(0, 90, 0));

        CreateCube("FocusZonePad", room, new Vector3(0f, 0.115f, 4f), new Vector3(3f, 0.01f, 2f), matTriggerZone);
        CreateCube("EnergyZonePad", room, new Vector3(-4.5f, 0.115f, 0f), new Vector3(2f, 0.01f, 4f), matTriggerZone);
        CreateCube("RelaxZonePad", room, new Vector3(2.5f, 0.115f, 0f), new Vector3(3f, 0.01f, 3f), matTriggerZone);

        // Directional Light
        GameObject dirLightObj = new GameObject("Directional Light");
        dirLightObj.transform.rotation = Quaternion.Euler(45f, -45f, 0f);
        Light dirLight = dirLightObj.AddComponent<Light>();
        dirLight.type = LightType.Directional;
        dirLight.color = new Color(0.2f, 0.25f, 0.3f);
        dirLight.intensity = 0.2f; // Dimmed default for mood lighting to stand out
        dirLight.shadows = LightShadows.Soft;

        // Particles
        GameObject particlesObj = new GameObject("AmbientParticles");
        particlesObj.transform.SetParent(room.transform);
        particlesObj.transform.localPosition = new Vector3(0, 2f, 0);
        ParticleSystem ps = particlesObj.AddComponent<ParticleSystem>();
        var main = ps.main;
        main.startColor = new Color(0.3f, 0.6f, 1.0f, 0.5f);
        main.startSize = 0.1f;
        main.startLifetime = 10f;
        main.simulationSpace = ParticleSystemSimulationSpace.World;
        main.maxParticles = 80;
        var em = ps.emission;
        em.rateOverTime = 8f;
        var shape = ps.shape;
        shape.shapeType = ParticleSystemShapeType.Box;
        shape.scale = new Vector3(W, H, D);
        var vel = ps.velocityOverLifetime;
        vel.enabled = true;
        vel.x = new ParticleSystem.MinMaxCurve(0f, 0f);
        vel.y = new ParticleSystem.MinMaxCurve(0.1f, 0.3f);
        vel.z = new ParticleSystem.MinMaxCurve(0f, 0f);
        ParticleSystemRenderer psRenderer = ps.GetComponent<ParticleSystemRenderer>();
        Material pMat = new Material(Shader.Find("Particles/Standard Unlit"));
        pMat.SetFloat("_Mode", 2); // Fade mode for particles
        pMat.renderQueue = 3000;
        pMat.SetColor("_Color", new Color(1, 1, 1, 0.5f));
        pMat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        pMat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.One);
        pMat.SetInt("_ZWrite", 0);
        psRenderer.sharedMaterial = pMat;

        // ── 5. PLAYER ─────────────────────────────────────────────
        GameObject player = new GameObject("Player");
        player.tag = "Player";
        player.transform.position = new Vector3(0f, 1.0f, -4f); 

        CharacterController playerCC = player.AddComponent<CharacterController>();
        playerCC.height = 1.8f;
        playerCC.center = new Vector3(0, 0.9f, 0);
        playerCC.radius = 0.3f;
        PlayerMovement pMove = player.AddComponent<PlayerMovement>();

        GameObject camObj = new GameObject("Main Camera");
        camObj.tag = "MainCamera";
        camObj.transform.SetParent(player.transform);
        camObj.transform.localPosition = new Vector3(0, 1.6f, 0);
        Camera cam = camObj.AddComponent<Camera>();
        cam.fieldOfView = 75f;
        camObj.AddComponent<AudioListener>();
        PlayerInteraction pInt = camObj.AddComponent<PlayerInteraction>();

        // ── 6. ENVIRONMENT MANAGER ───────────────────────────────
        GameObject envMgrObj = new GameObject("EnvironmentManager");
        EnvironmentManager envMgr = envMgrObj.AddComponent<EnvironmentManager>();
        AudioSource envAudio = envMgrObj.AddComponent<AudioSource>();
        envMgr.directionalLight = dirLight;
        envMgr.lampLight = lampL;
        envMgr.backgroundAudio = envAudio;
        envMgr.roomRenderers = tintableRenderers.ToArray(); 
        envMgr.ambientParticles = ps;
        
        envMgr.calmAudio = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Audio/calm_mode_music.mp3");
        envMgr.energyAudio = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Audio/energy_mode_music.mp3");
        envMgr.focusAudio = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Audio/focus_mode_music.mp3");

        // ── 7. TRIGGER ZONES ─────────────────────────────────────
        GameObject triggers = new GameObject("TriggerZones");
        CreateTriggerZone("RelaxTrigger",  triggers, new Vector3(2.5f, 1f, 0f), new Vector3(3f, 2f, 3f), ProximityTrigger.TriggerType.Calm,   envMgr);
        CreateTriggerZone("FocusTrigger",  triggers, new Vector3(0f, 1f, 4f), new Vector3(3f, 2f, 2f), ProximityTrigger.TriggerType.Focus,  envMgr);
        CreateTriggerZone("EnergyTrigger", triggers, new Vector3(-4.5f, 1f, 0f), new Vector3(2f, 2f, 4f), ProximityTrigger.TriggerType.Energy, envMgr);
        CreateTriggerZone("LampTrigger",   triggers, new Vector3(-2.5f, 1f, 4f), new Vector3(2f, 2f, 2f), ProximityTrigger.TriggerType.Lamp,   envMgr);
        CreateTriggerZone("PlantTrigger",  triggers, new Vector3(3.5f, 1f, 4f),  new Vector3(2f, 2f, 2f), ProximityTrigger.TriggerType.Plant,  envMgr);

        // ── 8. CANVAS / UI (Top-Left Light Theme) ────────────────
        GameObject canvasObj = new GameObject("Canvas");
        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvasObj.AddComponent<UnityEngine.UI.CanvasScaler>().uiScaleMode = UnityEngine.UI.CanvasScaler.ScaleMode.ScaleWithScreenSize;
        canvasObj.AddComponent<UnityEngine.UI.GraphicRaycaster>();
        
        UIManager uiMgr = canvasObj.AddComponent<UIManager>();
        MoodButtonController btnCtrl = canvasObj.AddComponent<MoodButtonController>();
        btnCtrl.envManager = envMgr;

        // Interaction UI (Crosshair and Prompt)
        TMPro.TextMeshProUGUI crosshair = CreateText(canvasObj, "Crosshair", "•", 24, new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(50, 50), TMPro.TextAlignmentOptions.Center, new Color(1, 1, 1, 0.5f));
        TMPro.TextMeshProUGUI interactPrompt = CreateText(canvasObj, "InteractPrompt", "", 18, new Vector2(0.5f, 0.5f), new Vector2(0, -40), new Vector2(300, 30), TMPro.TextAlignmentOptions.Center, Color.white);
        pInt.promptText = interactPrompt;
        pInt.interactDistance = 4f;
        pInt.interactableLayer = ~0; // All layers

        // Add Interactions to Objects
        // 1. Sit on Chair
        Interactable sitInt = chairSeat.AddComponent<Interactable>();
        sitInt.promptMessage = "[E] Sit on Chair";
        GameObject seatPoint = new GameObject("SeatPoint");
        seatPoint.transform.position = deskPos + new Vector3(0, 1.2f, -0.6f);
        UnityEditor.Events.UnityEventTools.AddObjectPersistentListener<Transform>(sitInt.onInteract, pMove.ToggleSit, seatPoint.transform);
        chairSeat.AddComponent<BoxCollider>();

        // 2. Read Book
        Interactable bookInt = book1.AddComponent<Interactable>();
        bookInt.promptMessage = "[E] Read Book";
        UnityEditor.Events.UnityEventTools.AddStringPersistentListener(bookInt.onInteract, envMgr.ShowFeedback, "\"Continuous improvement is better than delayed perfection.\"");
        book1.AddComponent<BoxCollider>();

        // 3. Toggle Lamp
        Interactable lampInt = lampLightObj.AddComponent<Interactable>();
        lampInt.promptMessage = "[E] Toggle Lamp";
        lampLightObj.AddComponent<BoxCollider>().size = new Vector3(3, 3, 3);
        UnityEditor.Events.UnityEventTools.AddPersistentListener(lampInt.onInteract, envMgr.ToggleLamp);

        // 4. Water Plant
        Interactable plantInt = pot.AddComponent<Interactable>();
        plantInt.promptMessage = "[E] Water Plant";
        pot.AddComponent<BoxCollider>().size = new Vector3(2, 4, 2);
        UnityEditor.Events.UnityEventTools.AddStringPersistentListener(plantInt.onInteract, envMgr.ShowFeedback, "You watered the plant. It looks happy!");

        // (20) Invisible/Modern UI panel
        Color uiBackground = new Color(0f, 0f, 0f, 0f); // Completely invisible panel
        Color textLight = new Color(0.9f, 0.9f, 0.9f); // Light text for contrast
        Color textDark = new Color(0.1f, 0.1f, 0.1f);
        
        // (21-23) Sleek modern buttons
        Color btnBlue = new Color(0.1f, 0.4f, 0.7f, 0.9f);
        Color btnOrange = new Color(0.8f, 0.4f, 0.1f, 0.9f);
        Color btnGreen = new Color(0.2f, 0.6f, 0.3f, 0.9f);
        Color btnPurple = new Color(0.5f, 0.3f, 0.7f, 0.9f);
        Color panelBgDark = new Color(0.05f, 0.05f, 0.05f, 0.8f);

        // Left Panel (Mood Buttons)
        GameObject panelObj = new GameObject("UI_MoodPanel");
        panelObj.transform.SetParent(canvasObj.transform, false);
        UnityEngine.UI.Image panelImg = panelObj.AddComponent<UnityEngine.UI.Image>();
        panelImg.color = panelBgDark;
        RectTransform panelRect = panelObj.GetComponent<RectTransform>();
        panelRect.anchorMin = new Vector2(0, 1);
        panelRect.anchorMax = new Vector2(0, 1);
        panelRect.pivot = new Vector2(0, 1);
        panelRect.anchoredPosition = new Vector2(20, -20);
        panelRect.sizeDelta = new Vector2(160, 150);

        CreateText(panelObj, "MoodHeader", "<b>Mood Selection Buttons</b>", 10, new Vector2(0.5f, 1f), new Vector2(0, -10), new Vector2(150, 15), TMPro.TextAlignmentOptions.Center, textLight);

        // Buttons (Emojis removed for font compatibility)
        UnityEngine.UI.Button stressedBtn = CreateButton(panelObj, "StressedButton", "I am Stressed", btnBlue, Color.white, new Vector2(0, -40));
        UnityEngine.UI.Button tiredBtn    = CreateButton(panelObj, "TiredButton", "I am Tired", btnOrange, Color.white, new Vector2(0, -75));
        UnityEngine.UI.Button unfocusedBtn= CreateButton(panelObj, "UnfocusedButton", "I am Unfocused", btnGreen, Color.white, new Vector2(0, -110));

        // Center Panel (Status & Feedback)
        GameObject statusPanel = new GameObject("UI_StatusPanel");
        statusPanel.transform.SetParent(canvasObj.transform, false);
        UnityEngine.UI.Image statusImg = statusPanel.AddComponent<UnityEngine.UI.Image>();
        statusImg.color = panelBgDark;
        RectTransform statusRect = statusPanel.GetComponent<RectTransform>();
        statusRect.anchorMin = new Vector2(0, 1);
        statusRect.anchorMax = new Vector2(0, 1);
        statusRect.pivot = new Vector2(0, 1);
        statusRect.anchoredPosition = new Vector2(190, -20);
        statusRect.sizeDelta = new Vector2(200, 110);

        CreateText(statusPanel, "StatusHeaderUI", "<b>Status & Feedback UI</b>", 10, new Vector2(0.5f, 1f), new Vector2(0, -10), new Vector2(180, 15), TMPro.TextAlignmentOptions.Center, textLight);
        CreateText(statusPanel, "StatusHeader", "Current Mode:", 10, new Vector2(0.5f, 1f), new Vector2(0, -35), new Vector2(180, 15), TMPro.TextAlignmentOptions.Center, Color.gray);
        TMPro.TextMeshProUGUI statusText = CreateText(statusPanel, "StatusText", "None", 14, new Vector2(0.5f, 1f), new Vector2(0, -50), new Vector2(180, 20), TMPro.TextAlignmentOptions.Center, Color.white);
        TMPro.TextMeshProUGUI feedbackText = CreateText(statusPanel, "FeedbackText", "Select your mood to begin.", 11, new Vector2(0.5f, 1f), new Vector2(0, -75), new Vector2(180, 30), TMPro.TextAlignmentOptions.Center, btnBlue);

        // Right side utilities
        GameObject rightPanel = new GameObject("UI_RightPanel");
        rightPanel.transform.SetParent(canvasObj.transform, false);
        UnityEngine.UI.Image rPanelImg = rightPanel.AddComponent<UnityEngine.UI.Image>();
        rPanelImg.color = panelBgDark;
        RectTransform rPanelRect = rightPanel.GetComponent<RectTransform>();
        rPanelRect.anchorMin = new Vector2(0, 1);
        rPanelRect.anchorMax = new Vector2(0, 1);
        rPanelRect.pivot = new Vector2(0, 1);
        rPanelRect.anchoredPosition = new Vector2(400, -20);
        rPanelRect.sizeDelta = new Vector2(160, 110);

        CreateText(rightPanel, "OtherHeader", "<b>Other UI Buttons</b>", 10, new Vector2(0.5f, 1f), new Vector2(0, -10), new Vector2(150, 15), TMPro.TextAlignmentOptions.Center, textLight);
        UnityEngine.UI.Button reduceBtn = CreateButton(rightPanel, "ReduceEffectsButton", "Reduce Effects", btnPurple, Color.white, new Vector2(0, -40));

        TMPro.TextMeshProUGUI instText = CreateText(rightPanel, "InstructionText", "Use WASD to move\nUse mouse to look\nWalk into a zone.", 9, new Vector2(0.5f, 1f), new Vector2(0, -75), new Vector2(150, 40), TMPro.TextAlignmentOptions.Center, Color.gray);
        TMPro.TextMeshProUGUI escText = CreateText(canvasObj, "EscHintText", "Press ESC to toggle cursor.", 12, new Vector2(1, 0), new Vector2(-10, 10), new Vector2(200, 20), TMPro.TextAlignmentOptions.BottomRight, Color.gray);

        envMgr.statusText = statusText;
        envMgr.feedbackText = feedbackText;
        uiMgr.moodPanel = panelObj;
        uiMgr.instructionText = instText;
        uiMgr.escHintText = escText;
        btnCtrl.stressedButton = stressedBtn;
        btnCtrl.tiredButton = tiredBtn;
        btnCtrl.unfocusedButton = unfocusedBtn;
        btnCtrl.reduceButton = reduceBtn;

        UnityEditor.Events.UnityEventTools.AddPersistentListener(stressedBtn.onClick, btnCtrl.OnStressedClicked);
        UnityEditor.Events.UnityEventTools.AddPersistentListener(tiredBtn.onClick, btnCtrl.OnTiredClicked);
        UnityEditor.Events.UnityEventTools.AddPersistentListener(unfocusedBtn.onClick, btnCtrl.OnUnfocusedClicked);
        UnityEditor.Events.UnityEventTools.AddPersistentListener(reduceBtn.onClick, btnCtrl.OnReduceEffectsClicked);

        GameObject eventSys = new GameObject("EventSystem");
        eventSys.AddComponent<UnityEngine.EventSystems.EventSystem>();
        eventSys.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();

        Debug.Log("=== RoomBuilder: OVERHAUL scene built successfully! ===");
        UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene());
    }

    // ── Helper methods ───────────────────────────────────────────

    private static Material GetOrCreateMaterial(string path, Color albedo, float smoothness, float metallic, Texture2D tex = null, bool isTransparent = false, Color? emission = null)
    {
        Material mat = AssetDatabase.LoadAssetAtPath<Material>(path);
        if (mat == null)
        {
            mat = new Material(Shader.Find("Standard"));
            AssetDatabase.CreateAsset(mat, path);
        }

        if (isTransparent || albedo.a < 1.0f)
        {
            mat.SetFloat("_Mode", 3);
            mat.SetOverrideTag("RenderType", "Transparent");
            mat.renderQueue = 3000;
            mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
            mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            mat.SetInt("_ZWrite", 0);
            mat.DisableKeyword("_ALPHATEST_ON");
            mat.DisableKeyword("_ALPHABLEND_ON");
            mat.EnableKeyword("_ALPHAPREMULTIPLY_ON");
        }
        else
        {
            mat.SetFloat("_Mode", 0);
            mat.SetOverrideTag("RenderType", "Opaque");
            mat.renderQueue = 2000;
            mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
            mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
            mat.SetInt("_ZWrite", 1);
            mat.DisableKeyword("_ALPHATEST_ON");
            mat.DisableKeyword("_ALPHABLEND_ON");
            mat.DisableKeyword("_ALPHAPREMULTIPLY_ON");
        }
        
        mat.color = albedo;
        if (tex != null)
        {
            mat.mainTexture = tex;
        }
        mat.SetFloat("_Smoothness", smoothness);
        mat.SetFloat("_Metallic", metallic);

        if (emission.HasValue)
        {
            mat.EnableKeyword("_EMISSION");
            mat.SetColor("_EmissionColor", emission.Value);
        }
        else
        {
            mat.DisableKeyword("_EMISSION");
            mat.SetColor("_EmissionColor", Color.black);
        }

        EditorUtility.SetDirty(mat);
        return mat;
    }

    private static GameObject CreateCube(string name, GameObject parent, Vector3 position, Vector3 scale, Material mat)
    {
        GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Cube);
        obj.name = name;
        obj.transform.SetParent(parent.transform);
        obj.transform.localPosition = position;
        obj.transform.localScale = scale;
        
        if (mat != null)
        {
            Renderer r = obj.GetComponent<Renderer>();
            r.sharedMaterial = mat;
        }
        return obj;
    }

    private static void CreateTriggerZone(string name, GameObject parent, Vector3 position, Vector3 size, ProximityTrigger.TriggerType type, EnvironmentManager envMgr)
    {
        GameObject zone = new GameObject(name);
        zone.transform.SetParent(parent.transform);
        zone.transform.localPosition = position;

        BoxCollider col = zone.AddComponent<BoxCollider>();
        col.size = size;
        col.isTrigger = true;

        Rigidbody rb = zone.AddComponent<Rigidbody>();
        rb.isKinematic = true;
        rb.useGravity = false;

        ProximityTrigger pt = zone.AddComponent<ProximityTrigger>();
        pt.triggerType = type;
        pt.envManager = envMgr;
    }

    private static TMPro.TextMeshProUGUI CreateText(GameObject parent, string name, string text, int size, Vector2 anchor, Vector2 pos, Vector2 sizeDelta, TMPro.TextAlignmentOptions align, Color color)
    {
        GameObject obj = new GameObject(name);
        obj.transform.SetParent(parent.transform, false);
        TMPro.TextMeshProUGUI tmp = obj.AddComponent<TMPro.TextMeshProUGUI>();
        tmp.text = text;
        tmp.fontSize = size;
        tmp.alignment = align;
        tmp.color = color;
        RectTransform rect = tmp.GetComponent<RectTransform>();
        rect.anchorMin = anchor;
        rect.anchorMax = anchor;
        rect.pivot = anchor;
        rect.anchoredPosition = pos;
        rect.sizeDelta = sizeDelta;
        return tmp;
    }

    private static UnityEngine.UI.Button CreateButton(GameObject parent, string name, string text, Color color, Color textColor, Vector2 position)
    {
        GameObject btnObj = new GameObject(name);
        btnObj.transform.SetParent(parent.transform, false);
        
        UnityEngine.UI.Image img = btnObj.AddComponent<UnityEngine.UI.Image>();
        img.color = color;
        
        UnityEngine.UI.Button btn = btnObj.AddComponent<UnityEngine.UI.Button>();
        RectTransform rect = btnObj.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 1);
        rect.anchorMax = new Vector2(0.5f, 1);
        rect.pivot = new Vector2(0.5f, 1);
        rect.anchoredPosition = position;
        rect.sizeDelta = new Vector2(140, 30); // Smaller buttons

        CreateText(btnObj, "Text", text, 12, new Vector2(0.5f,0.5f), Vector2.zero, new Vector2(140,30), TMPro.TextAlignmentOptions.Center, textColor);
        return btn;
    }

    // Helper to spawn a prefab from an asset path
    private static GameObject SpawnPrefab(string prefabPath, GameObject parent, Vector3 localPos, Vector3 localScale)
    {
        var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
        if (prefab == null)
        {
            Debug.LogError($"Prefab not found at {prefabPath}");
            return null;
        }
        var instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
        instance.transform.SetParent(parent.transform, false);
        instance.transform.localPosition = localPos;
        instance.transform.localScale = localScale;
        return instance;
    }

    // Overload with rotation
    private static GameObject SpawnPrefab(string prefabPath, GameObject parent, Vector3 localPos, Quaternion rotation)
    {
        var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
        if (prefab == null)
        {
            Debug.LogError($"Prefab not found at {prefabPath}");
            return null;
        }
        var instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
        instance.transform.SetParent(parent.transform, false);
        instance.transform.localPosition = localPos;
        instance.transform.rotation = rotation;
        return instance;
    }
}
#endif
