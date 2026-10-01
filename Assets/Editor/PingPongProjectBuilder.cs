#if UNITY_EDITOR
using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[InitializeOnLoad]
public static class PingPongProjectBuilder
{
    public const string SceneDir = "Assets/_Project/Scenes";
    const string Marker = "Assets/_Project/Settings/.setup_complete_6000_3_16f1";
    const float AssetsPPU = 32f;
    static Font uiFont;

    static PingPongProjectBuilder()
    {
        EditorApplication.delayCall += () =>
        {
            if (!File.Exists(Marker))
            {
                try { BuildAll(); }
                catch (Exception ex) { Debug.LogException(ex); }
            }
        };
    }

    [MenuItem("Tools/Ping Pong/1 - Rebuild Complete Project")]
    public static void BuildAll()
    {
        EnsureFolders();
        AssetDatabase.Refresh();
        ConfigureProjectSettings();
        ConfigureTextureImporters();
        CreatePhysicsMaterial();
        CreateMenuScene();
        CreateGameScene();
        ConfigureBuildSettings();

        File.WriteAllText(Marker,
            "Generated for Unity 6000.3.16f1. Delete this marker and run Tools > Ping Pong > 1 - Rebuild Complete Project to rebuild scenes.");
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        EditorSceneManager.OpenScene(SceneDir + "/Menu.unity");
        Debug.Log("PING-PONG_GAME: complete 2D project generated for Unity 6000.3.16f1. Press Play from Menu.unity.");
    }

    static void EnsureFolders()
    {
        Directory.CreateDirectory(SceneDir);
        Directory.CreateDirectory("Assets/_Project/Settings");
        Directory.CreateDirectory("Assets/_Project/Materials");
        Directory.CreateDirectory("Build/Windows");
    }

    static void ConfigureProjectSettings()
    {
        EditorSettings.serializationMode = SerializationMode.ForceText;
        EditorSettings.externalVersionControl = "Visible Meta Files";

        PlayerSettings.companyName = "Developer-vic1";
        PlayerSettings.productName = "Ping-Pong_Game";
        PlayerSettings.defaultScreenWidth = 1280;
        PlayerSettings.defaultScreenHeight = 720;
        PlayerSettings.fullScreenMode = FullScreenMode.Windowed;
        PlayerSettings.resizableWindow = true;
        PlayerSettings.runInBackground = true;
        PlayerSettings.allowFullscreenSwitch = true;

        Physics2D.gravity = Vector2.zero;
        QualitySettings.vSyncCount = 1;
    }

    static void ConfigureBuildSettings()
    {
        EditorBuildSettings.scenes = new[]
        {
            new EditorBuildSettingsScene(SceneDir + "/Menu.unity", true),
            new EditorBuildSettingsScene(SceneDir + "/Game.unity", true)
        };
    }

    static void ConfigureTextureImporters()
    {
        foreach (string guid in AssetDatabase.FindAssets("t:Texture2D", new[] { "Assets/_Project/Art" }))
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            TextureImporter ti = AssetImporter.GetAtPath(path) as TextureImporter;
            if (ti == null) continue;

            ti.textureType = TextureImporterType.Sprite;
            ti.spriteImportMode = SpriteImportMode.Single;
            ti.spritePixelsPerUnit = AssetsPPU;
            ti.filterMode = FilterMode.Point;
            ti.textureCompression = TextureImporterCompression.Uncompressed;
            ti.mipmapEnabled = false;
            ti.alphaIsTransparency = true;
            ti.wrapMode = TextureWrapMode.Clamp;
            ti.SaveAndReimport();
        }
    }

    static void CreatePhysicsMaterial()
    {
        const string path = "Assets/_Project/Materials/Bouncy.physicsMaterial2D";
        var existing = AssetDatabase.LoadAssetAtPath<PhysicsMaterial2D>(path);
        if (existing != null)
        {
            existing.bounciness = 1f;
            existing.friction = 0f;
            EditorUtility.SetDirty(existing);
            return;
        }

        var mat = new PhysicsMaterial2D("Bouncy") { bounciness = 1f, friction = 0f };
        AssetDatabase.CreateAsset(mat, path);
    }

    static Font UIFont()
    {
        if (uiFont != null) return uiFont;
        uiFont = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        if (uiFont == null) uiFont = Font.CreateDynamicFontFromOSFont("Arial", 32);
        return uiFont;
    }

    static Sprite S(string path) => AssetDatabase.LoadAssetAtPath<Sprite>(path);
    static AudioClip A(string path) => AssetDatabase.LoadAssetAtPath<AudioClip>(path);

    static Camera CreateCamera()
    {
        GameObject go = new GameObject("Main Camera");
        go.tag = "MainCamera";
        Camera cam = go.AddComponent<Camera>();
        cam.orthographic = true;
        cam.orthographicSize = 5.625f;
        cam.transform.position = new Vector3(0f, 0f, -10f);
        cam.backgroundColor = new Color(0.018f, 0.028f, 0.045f);
        cam.allowHDR = false;
        cam.allowMSAA = false;
        go.AddComponent<CameraShake2D>();
        TryAddPixelPerfectCamera(go);
        return cam;
    }

    static void TryAddPixelPerfectCamera(GameObject cameraObject)
    {
        Type type = AppDomain.CurrentDomain.GetAssemblies()
            .Select(a => a.GetType("UnityEngine.U2D.PixelPerfectCamera"))
            .FirstOrDefault(t => t != null);
        if (type == null) return;

        Component component = cameraObject.AddComponent(type);
        type.GetProperty("assetsPPU")?.SetValue(component, (int)AssetsPPU);
        type.GetProperty("refResolutionX")?.SetValue(component, 640);
        type.GetProperty("refResolutionY")?.SetValue(component, 360);
    }

    static GameObject CreateSpriteGO(string name, Sprite sprite, Vector3 pos, Vector3 scale, int order = 0, Color? color = null, Transform parent = null)
    {
        GameObject go = new GameObject(name);
        if (parent != null) go.transform.SetParent(parent, false);
        go.transform.position = pos;
        go.transform.localScale = scale;
        SpriteRenderer sr = go.AddComponent<SpriteRenderer>();
        sr.sprite = sprite;
        sr.sortingOrder = order;
        sr.color = color ?? Color.white;
        return go;
    }

    static GameObject CreateBackground()
    {
        GameObject bg = CreateSpriteGO(
            "Background",
            S("Assets/_Project/Art/Backgrounds/forest_background.png"),
            new Vector3(0f, 0f, 2f),
            new Vector3(0.32f, 0.32f, 1f),
            -30);

        CreateSpriteGO(
            "Vignette",
            S("Assets/_Project/Art/Backgrounds/vignette.png"),
            new Vector3(0f, 0f, 1.5f),
            new Vector3(0.32f, 0.32f, 1f),
            -20);

        return bg;
    }

    static Canvas CreateCanvas(string name = "Canvas")
    {
        GameObject go = new GameObject(name);
        Canvas canvas = go.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.pixelPerfect = true;
        CanvasScaler scaler = go.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        scaler.matchWidthOrHeight = 0.5f;
        go.AddComponent<GraphicRaycaster>();
        return canvas;
    }

    static GameObject CreateText(Transform parent, string name, string text, Vector2 pos, Vector2 size, int fontSize, Color color, bool outline = true)
    {
        GameObject go = new GameObject(name);
        go.transform.SetParent(parent, false);
        RectTransform rt = go.AddComponent<RectTransform>();
        rt.anchorMin = rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.anchoredPosition = pos;
        rt.sizeDelta = size;

        Text t = go.AddComponent<Text>();
        t.font = UIFont();
        t.text = text;
        t.fontSize = fontSize;
        t.color = color;
        t.alignment = TextAnchor.MiddleCenter;
        t.fontStyle = FontStyle.Bold;
        t.horizontalOverflow = HorizontalWrapMode.Wrap;
        t.verticalOverflow = VerticalWrapMode.Overflow;

        if (outline)
        {
            Outline o = go.AddComponent<Outline>();
            o.effectColor = new Color(0.03f, 0.04f, 0.06f, 0.92f);
            o.effectDistance = new Vector2(2f, -2f);
        }
        return go;
    }

    static GameObject CreateImage(Transform parent, string name, Vector2 pos, Vector2 size, Color color, Sprite sprite = null)
    {
        GameObject go = new GameObject(name);
        go.transform.SetParent(parent, false);
        RectTransform rt = go.AddComponent<RectTransform>();
        rt.anchorMin = rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.anchoredPosition = pos;
        rt.sizeDelta = size;
        Image img = go.AddComponent<Image>();
        img.color = color;
        img.sprite = sprite;
        return go;
    }

    static Button CreateButton(Transform parent, string name, string label, Vector2 pos, Vector2 size, bool blue = false)
    {
        Sprite sprite = S(blue ? "Assets/_Project/Art/UI/button_blue.png" : "Assets/_Project/Art/UI/button_gold.png");
        GameObject go = CreateImage(parent, name, pos, size, Color.white, sprite);
        Button button = go.AddComponent<Button>();
        ColorBlock colors = button.colors;
        colors.normalColor = Color.white;
        colors.highlightedColor = new Color(1f, 0.92f, 0.68f, 1f);
        colors.pressedColor = new Color(0.78f, 0.78f, 0.78f, 1f);
        colors.selectedColor = new Color(1f, 0.88f, 0.55f, 1f);
        colors.fadeDuration = 0.08f;
        button.colors = colors;
        CreateText(go.transform, "Label", label, Vector2.zero, size, (int)(size.y * 0.36f), Color.white);
        return button;
    }

    static void EnsureEventSystem()
    {
        if (UnityEngine.Object.FindFirstObjectByType<UnityEngine.EventSystems.EventSystem>() != null) return;
        GameObject go = new GameObject("EventSystem");
        go.AddComponent<UnityEngine.EventSystems.EventSystem>();
        go.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
    }

    static AudioManager CreateAudioManager()
    {
        GameObject go = new GameObject("AudioManager");
        AudioManager am = go.AddComponent<AudioManager>();
        go.AddComponent<AudioSource>();
        go.AddComponent<AudioSource>();
        am.bounceClip = A("Assets/_Project/Audio/SFX/bounce.wav");
        am.wallClip = A("Assets/_Project/Audio/SFX/wall.wav");
        am.goalClip = A("Assets/_Project/Audio/SFX/goal.wav");
        am.clickClip = A("Assets/_Project/Audio/SFX/click.wav");
        am.winClip = A("Assets/_Project/Audio/SFX/win.wav");
        am.ambientLoop = A("Assets/_Project/Audio/Music/forest_ambient.wav");
        return am;
    }

    static void CreateMenuScene()
    {
        Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        CreateCamera();
        CreateBackground();
        CreateAudioManager();
        EnsureEventSystem();
        Canvas canvas = CreateCanvas();

        CreateImage(canvas.transform, "MenuShade", Vector2.zero, new Vector2(1920, 1080), new Color(0.02f, 0.04f, 0.07f, 0.22f), S("Assets/_Project/Art/Sprites/white_pixel.png"));
        CreateText(canvas.transform, "Title", "PING-PONG GAME", new Vector2(0, 300), new Vector2(1250, 160), 94, new Color(1f, 0.79f, 0.27f));
        CreateText(canvas.transform, "Subtitle", "FANTASY DUEL", new Vector2(0, 205), new Vector2(700, 70), 34, new Color(0.85f, 0.93f, 1f));
        CreateText(canvas.transform, "Tagline", "2 JUGADORES • PRIMERO EN LLEGAR A 5", new Vector2(0, 150), new Vector2(900, 50), 24, new Color(0.72f, 0.78f, 0.86f));

        Button play = CreateButton(canvas.transform, "PlayButton", "JUGAR", new Vector2(0, 40), new Vector2(430, 100));
        Button controls = CreateButton(canvas.transform, "ControlsButton", "CONTROLES", new Vector2(0, -85), new Vector2(430, 100), true);
        Button exit = CreateButton(canvas.transform, "ExitButton", "SALIR", new Vector2(0, -210), new Vector2(430, 100));
        CreateText(canvas.transform, "Version", "Unity 6000.3.16f1  •  v1.1.0", new Vector2(0, -475), new Vector2(700, 40), 20, new Color(0.58f, 0.64f, 0.72f), false);

        GameObject controlsPanel = CreateImage(canvas.transform, "ControlsPanel", Vector2.zero, new Vector2(980, 610), Color.white, S("Assets/_Project/Art/UI/panel.png"));
        CreateText(controlsPanel.transform, "Title", "CONTROLES", new Vector2(0, 205), new Vector2(750, 85), 56, new Color(1f, 0.82f, 0.36f));
        CreateText(controlsPanel.transform, "P1", "PLAYER 1\nW  /  S", new Vector2(-230, 45), new Vector2(360, 190), 42, new Color(1f, 0.64f, 0.55f));
        CreateText(controlsPanel.transform, "P2", "PLAYER 2\n↑  /  ↓", new Vector2(230, 45), new Vector2(360, 190), 42, new Color(0.55f, 0.76f, 1f));
        CreateText(controlsPanel.transform, "Pause", "ESC = PAUSA", new Vector2(0, -105), new Vector2(500, 60), 28, Color.white);
        Button back = CreateButton(controlsPanel.transform, "BackButton", "VOLVER", new Vector2(0, -220), new Vector2(350, 82), true);

        MenuController mc = canvas.gameObject.AddComponent<MenuController>();
        mc.controlsPanel = controlsPanel;
        mc.playButton = play;
        mc.controlsButton = controls;
        mc.exitButton = exit;
        mc.controlsBackButton = back;
        controlsPanel.SetActive(false);

        EditorSceneManager.SaveScene(scene, SceneDir + "/Menu.unity");
    }

    static void CreateGameScene()
    {
        Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        CreateCamera();
        CreateBackground();
        CreateAudioManager();
        EnsureEventSystem();

        Transform arena = new GameObject("Arena").transform;
        Sprite stone = S("Assets/_Project/Art/Sprites/stone_tile.png");

        // Visual court shade for contrast.
        CreateSpriteGO("ArenaShade", S("Assets/_Project/Art/Sprites/white_pixel.png"), new Vector3(0, 0, 0.8f), new Vector3(74f, 32f, 1f), -2, new Color(0.02f, 0.04f, 0.06f, 0.34f), arena);

        GameObject top = CreateSpriteGO("TopBorder", stone, new Vector3(0, 4.62f, 0), new Vector3(9.3f, 0.24f, 1), 1, null, arena);
        BoxCollider2D topCollider = top.AddComponent<BoxCollider2D>();
        topCollider.size = Vector2.one;

        GameObject bottom = CreateSpriteGO("BottomBorder", stone, new Vector3(0, -4.62f, 0), new Vector3(9.3f, 0.24f, 1), 1, null, arena);
        BoxCollider2D bottomCollider = bottom.AddComponent<BoxCollider2D>();
        bottomCollider.size = Vector2.one;

        // Side pillars are decorative only; goals stay open.
        CreateSpriteGO("LeftPillar", stone, new Vector3(-8.95f, 0, 0), new Vector3(0.22f, 4.45f, 1), 1, new Color(0.88f, 0.9f, 0.95f, 0.82f), arena);
        CreateSpriteGO("RightPillar", stone, new Vector3(8.95f, 0, 0), new Vector3(0.22f, 4.45f, 1), 1, new Color(0.88f, 0.9f, 0.95f, 0.82f), arena);

        for (int i = -4; i <= 4; i++)
            CreateSpriteGO("CenterDot_" + i, S("Assets/_Project/Art/Sprites/center_dot.png"), new Vector3(0, i * 0.9f, 0), new Vector3(0.5f, 0.5f, 1), 1, new Color(1f, 1f, 1f, 0.7f), arena);

        CreateSpriteGO("LeftGoalGlow", S("Assets/_Project/Art/Sprites/goal_glow.png"), new Vector3(-8.55f, 0, 0), new Vector3(0.85f, 1.52f, 1), 1, new Color(1f, 0.38f, 0.28f, 0.55f), arena);
        CreateSpriteGO("RightGoalGlow", S("Assets/_Project/Art/Sprites/goal_glow.png"), new Vector3(8.55f, 0, 0), new Vector3(0.85f, 1.52f, 1), 1, new Color(0.25f, 0.62f, 1f, 0.55f), arena);

        GameObject p1Glow = CreateSpriteGO("PaddleLeftGlow", S("Assets/_Project/Art/Sprites/paddle_red.png"), new Vector3(-7.65f, 0, 0.1f), new Vector3(0.39f, 0.32f, 1), 2, new Color(1f, 0.18f, 0.10f, 0.22f));
        GameObject p1 = CreateSpriteGO("PaddleLeft", S("Assets/_Project/Art/Sprites/paddle_red.png"), new Vector3(-7.65f, 0, 0), new Vector3(0.32f, 0.27f, 1), 4);
        p1.AddComponent<BoxCollider2D>();
        p1.AddComponent<Rigidbody2D>();
        PaddleController pc1 = p1.AddComponent<PaddleController>();
        pc1.upKey = KeyCode.W; pc1.downKey = KeyCode.S;
        p1Glow.transform.SetParent(p1.transform, true);

        GameObject p2Glow = CreateSpriteGO("PaddleRightGlow", S("Assets/_Project/Art/Sprites/paddle_blue.png"), new Vector3(7.65f, 0, 0.1f), new Vector3(0.39f, 0.32f, 1), 2, new Color(0.14f, 0.55f, 1f, 0.22f));
        GameObject p2 = CreateSpriteGO("PaddleRight", S("Assets/_Project/Art/Sprites/paddle_blue.png"), new Vector3(7.65f, 0, 0), new Vector3(0.32f, 0.27f, 1), 4);
        p2.AddComponent<BoxCollider2D>();
        p2.AddComponent<Rigidbody2D>();
        PaddleController pc2 = p2.AddComponent<PaddleController>();
        pc2.upKey = KeyCode.UpArrow; pc2.downKey = KeyCode.DownArrow;
        p2Glow.transform.SetParent(p2.transform, true);

        GameObject ball = CreateSpriteGO("Ball", S("Assets/_Project/Art/Sprites/ball.png"), Vector3.zero, new Vector3(0.32f, 0.32f, 1), 6);
        GameObject ballGlow = CreateSpriteGO("Glow", S("Assets/_Project/Art/Sprites/ball_glow.png"), Vector3.zero, Vector3.one, 5, new Color(1f, 0.74f, 0.30f, 0.52f), ball.transform);
        ballGlow.transform.localPosition = Vector3.zero;
        ballGlow.transform.localScale = new Vector3(1.45f, 1.45f, 1f);
        CircleCollider2D circle = ball.AddComponent<CircleCollider2D>();
        circle.sharedMaterial = AssetDatabase.LoadAssetAtPath<PhysicsMaterial2D>("Assets/_Project/Materials/Bouncy.physicsMaterial2D");
        Rigidbody2D ballRb = ball.AddComponent<Rigidbody2D>();
        ballRb.gravityScale = 0f;
        ballRb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        ballRb.interpolation = RigidbodyInterpolation2D.Interpolate;
        ballRb.freezeRotation = true;
        BallController ballScript = ball.AddComponent<BallController>();

        GameObject goalLeft = new GameObject("GoalLeft");
        goalLeft.transform.position = new Vector3(-9.5f, 0, 0);
        BoxCollider2D glc = goalLeft.AddComponent<BoxCollider2D>();
        glc.isTrigger = true; glc.size = new Vector2(0.8f, 9.2f);
        goalLeft.AddComponent<GoalTrigger>().scoringPlayer = 2;

        GameObject goalRight = new GameObject("GoalRight");
        goalRight.transform.position = new Vector3(9.5f, 0, 0);
        BoxCollider2D grc = goalRight.AddComponent<BoxCollider2D>();
        grc.isTrigger = true; grc.size = new Vector2(0.8f, 9.2f);
        goalRight.AddComponent<GoalTrigger>().scoringPlayer = 1;

        Canvas canvas = CreateCanvas();
        CreateImage(canvas.transform, "HUDStrip", new Vector2(0, 465), new Vector2(720, 120), Color.white, S("Assets/_Project/Art/UI/hud_strip.png"));
        Text score1 = CreateText(canvas.transform, "ScoreP1", "0", new Vector2(-180, 465), new Vector2(150, 105), 68, new Color(1f, 0.75f, 0.67f)).GetComponent<Text>();
        Text score2 = CreateText(canvas.transform, "ScoreP2", "0", new Vector2(180, 465), new Vector2(150, 105), 68, new Color(0.62f, 0.79f, 1f)).GetComponent<Text>();
        CreateText(canvas.transform, "ScoreDash", "-", new Vector2(0, 465), new Vector2(90, 105), 54, Color.white);
        CreateText(canvas.transform, "P1Label", "PLAYER 1", new Vector2(-180, 525), new Vector2(260, 44), 22, new Color(1f, 0.65f, 0.58f));
        CreateText(canvas.transform, "P2Label", "PLAYER 2", new Vector2(180, 525), new Vector2(260, 44), 22, new Color(0.56f, 0.75f, 1f));
        CreateText(canvas.transform, "ControlsHint", "W/S                         ↑/↓", new Vector2(0, -505), new Vector2(1280, 45), 22, new Color(0.76f, 0.80f, 0.87f), false);
        CreateText(canvas.transform, "PauseHint", "ESC  PAUSA", new Vector2(790, 510), new Vector2(260, 45), 20, new Color(0.82f, 0.86f, 0.92f), false);
        Text countdown = CreateText(canvas.transform, "Countdown", "", new Vector2(0, 25), new Vector2(600, 180), 112, new Color(1f, 0.84f, 0.36f)).GetComponent<Text>();
        Text roundStatus = CreateText(canvas.transform, "RoundStatus", "", new Vector2(0, -105), new Vector2(850, 70), 30, Color.white).GetComponent<Text>();

        GameObject pause = CreateImage(canvas.transform, "PausePanel", Vector2.zero, new Vector2(850, 610), Color.white, S("Assets/_Project/Art/UI/panel.png"));
        CreateText(pause.transform, "PauseTitle", "PAUSA", new Vector2(0, 205), new Vector2(600, 95), 64, new Color(1f, 0.82f, 0.35f));
        Button resume = CreateButton(pause.transform, "ResumeButton", "CONTINUAR", new Vector2(0, 75), new Vector2(420, 88));
        Button restart = CreateButton(pause.transform, "RestartButton", "REINICIAR", new Vector2(0, -45), new Vector2(420, 88), true);
        Button pauseMenu = CreateButton(pause.transform, "MenuButton", "MENÚ", new Vector2(0, -165), new Vector2(420, 88));

        GameObject win = CreateImage(canvas.transform, "WinPanel", Vector2.zero, new Vector2(980, 680), Color.white, S("Assets/_Project/Art/UI/panel.png"));
        CreateImage(win.transform, "Crown", new Vector2(0, 245), new Vector2(128, 96), Color.white, S("Assets/_Project/Art/UI/crown.png"));
        Text winner = CreateText(win.transform, "WinnerText", "PLAYER 1 GANA", new Vector2(0, 145), new Vector2(820, 110), 68, new Color(1f, 0.82f, 0.35f)).GetComponent<Text>();
        Text finalScore = CreateText(win.transform, "FinalScore", "5 - 0", new Vector2(0, 45), new Vector2(500, 100), 60, Color.white).GetComponent<Text>();
        Button rematch = CreateButton(win.transform, "RematchButton", "REVANCHA", new Vector2(0, -85), new Vector2(440, 92));
        Button winMenu = CreateButton(win.transform, "WinMenuButton", "MENÚ", new Vector2(0, -205), new Vector2(440, 92), true);

        GameObject gmObject = new GameObject("GameManager");
        ScoreManager scoreManager = gmObject.AddComponent<ScoreManager>();
        scoreManager.player1Text = score1;
        scoreManager.player2Text = score2;

        GameManager gm = gmObject.AddComponent<GameManager>();
        gm.maxScore = 5;
        gm.ball = ballScript;
        gm.scoreManager = scoreManager;
        gm.countdownText = countdown;
        gm.roundStatusText = roundStatus;
        gm.pausePanel = pause;
        gm.resumeButton = resume;
        gm.restartButton = restart;
        gm.pauseMenuButton = pauseMenu;
        gm.winPanel = win;
        gm.winnerText = winner;
        gm.finalScoreText = finalScore;
        gm.rematchButton = rematch;
        gm.winMenuButton = winMenu;

        pause.SetActive(false);
        win.SetActive(false);

        EditorSceneManager.SaveScene(scene, SceneDir + "/Game.unity");
    }

    [MenuItem("Tools/Ping Pong/2 - Validate Project")]
    public static void ValidateProject()
    {
        string[] required =
        {
            SceneDir + "/Menu.unity",
            SceneDir + "/Game.unity",
            "Assets/_Project/Scripts/Gameplay/BallController.cs",
            "Assets/_Project/Scripts/Gameplay/PaddleController.cs",
            "Assets/_Project/Audio/SFX/bounce.wav",
            "Assets/_Project/Art/Backgrounds/forest_background.png"
        };

        bool ok = true;
        foreach (string path in required)
        {
            if (!File.Exists(path))
            {
                ok = false;
                Debug.LogError("Missing: " + path);
            }
        }

        if (EditorBuildSettings.scenes.Length < 2) { ok = false; Debug.LogError("Build Settings must contain Menu and Game."); }
        Debug.Log(ok ? "PING-PONG_GAME VALIDATION: OK." : "PING-PONG_GAME VALIDATION: errors found. Check Console.");
    }
}
#endif
