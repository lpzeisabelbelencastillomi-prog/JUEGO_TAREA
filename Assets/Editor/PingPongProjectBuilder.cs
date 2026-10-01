#if UNITY_EDITOR
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
    const string SceneDir = "Assets/_Project/Scenes";
    const string Marker = "Assets/_Project/Settings/.setup_complete";
    static Font uiFont;

    static PingPongProjectBuilder()
    {
        EditorApplication.delayCall += () =>
        {
            if (!File.Exists(Marker))
            {
                try { BuildAll(); }
                catch (System.Exception ex) { Debug.LogException(ex); }
            }
        };
    }

    [MenuItem("Tools/Ping Pong/Rebuild Complete Project")]
    public static void BuildAll()
    {
        Directory.CreateDirectory(SceneDir);
        Directory.CreateDirectory("Assets/_Project/Settings");
        Directory.CreateDirectory("Assets/_Project/Materials");
        AssetDatabase.Refresh();
        ConfigureTextureImporters();
        CreateMaterial();
        CreateMenuScene();
        CreateGameScene();
        EditorBuildSettings.scenes = new[]
        {
            new EditorBuildSettingsScene(SceneDir + "/Menu.unity", true),
            new EditorBuildSettingsScene(SceneDir + "/Game.unity", true)
        };
        PlayerSettings.productName = "Ping-Pong_Game";
        File.WriteAllText(Marker, "Generated automatically. Delete this file and use Tools > Ping Pong > Rebuild Complete Project to regenerate.");
        AssetDatabase.Refresh();
        EditorSceneManager.OpenScene(SceneDir + "/Menu.unity");
        Debug.Log("PING-PONG_GAME: project generated successfully. Open Menu.unity and press Play.");
    }

    static void ConfigureTextureImporters()
    {
        string[] folders = { "Assets/_Project/Art" };
        foreach (string guid in AssetDatabase.FindAssets("t:Texture2D", folders))
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            TextureImporter ti = AssetImporter.GetAtPath(path) as TextureImporter;
            if (ti == null) continue;
            ti.textureType = TextureImporterType.Sprite;
            ti.spriteImportMode = SpriteImportMode.Single;
            ti.spritePixelsPerUnit = path.Contains("forest_background") ? 100f : 64f;
            ti.filterMode = FilterMode.Point;
            ti.textureCompression = TextureImporterCompression.Uncompressed;
            ti.alphaIsTransparency = true;
            ti.SaveAndReimport();
        }
    }

    static void CreateMaterial()
    {
        string path = "Assets/_Project/Materials/Bouncy.physicsMaterial2D";
        if (AssetDatabase.LoadAssetAtPath<PhysicsMaterial2D>(path) != null) return;
        var mat = new PhysicsMaterial2D("Bouncy");
        mat.bounciness = 1f;
        mat.friction = 0f;
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
        cam.orthographicSize = 5.4f;
        cam.transform.position = new Vector3(0, 0, -10);
        cam.backgroundColor = new Color(0.03f,0.05f,0.08f);
        return cam;
    }

    static GameObject CreateBackground()
    {
        var go = new GameObject("Background");
        var sr = go.AddComponent<SpriteRenderer>();
        sr.sprite = S("Assets/_Project/Art/Backgrounds/forest_background.png");
        sr.sortingOrder = -20;
        go.transform.position = new Vector3(0,0,2);
        return go;
    }

    static Canvas CreateCanvas(string name="Canvas")
    {
        GameObject go = new GameObject(name);
        Canvas c = go.AddComponent<Canvas>();
        c.renderMode = RenderMode.ScreenSpaceOverlay;
        CanvasScaler scaler = go.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920,1080);
        scaler.matchWidthOrHeight = 0.5f;
        go.AddComponent<GraphicRaycaster>();
        return c;
    }

    static GameObject CreateText(Transform parent, string name, string text, Vector2 pos, Vector2 size, int fontSize, Color color)
    {
        GameObject go = new GameObject(name);
        go.transform.SetParent(parent,false);
        RectTransform rt = go.AddComponent<RectTransform>();
        rt.anchorMin = rt.anchorMax = new Vector2(0.5f,0.5f);
        rt.anchoredPosition = pos;
        rt.sizeDelta = size;
        Text t = go.AddComponent<Text>();
        t.font = UIFont(); t.text = text; t.fontSize = fontSize; t.color = color;
        t.alignment = TextAnchor.MiddleCenter;
        t.fontStyle = FontStyle.Bold;
        return go;
    }

    static GameObject CreateImage(Transform parent, string name, Vector2 pos, Vector2 size, Color color, Sprite sprite=null)
    {
        GameObject go = new GameObject(name);
        go.transform.SetParent(parent,false);
        RectTransform rt = go.AddComponent<RectTransform>();
        rt.anchorMin = rt.anchorMax = new Vector2(0.5f,0.5f);
        rt.anchoredPosition = pos; rt.sizeDelta = size;
        Image img = go.AddComponent<Image>(); img.color = color; img.sprite = sprite;
        return go;
    }

    static GameObject CreateButton(Transform parent, string name, string label, Vector2 pos, Vector2 size)
    {
        GameObject go = CreateImage(parent,name,pos,size,Color.white,S("Assets/_Project/Art/UI/button_panel.png"));
        Button b = go.AddComponent<Button>();
        ColorBlock cb = b.colors; cb.highlightedColor = new Color(1f,.85f,.55f,1f); cb.pressedColor = new Color(.75f,.65f,.5f,1f); b.colors=cb;
        GameObject text = CreateText(go.transform,"Label",label,Vector2.zero,size,(int)(size.y*0.36f),Color.white);
        return go;
    }

    static void EnsureEventSystem()
    {
        if (Object.FindFirstObjectByType<UnityEngine.EventSystems.EventSystem>() != null) return;
        GameObject go = new GameObject("EventSystem");
        go.AddComponent<UnityEngine.EventSystems.EventSystem>();
        go.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
    }

    static AudioManager CreateAudioManager()
    {
        GameObject go = new GameObject("AudioManager");
        var am = go.AddComponent<AudioManager>();
        go.AddComponent<AudioSource>();
        am.bounceClip=A("Assets/_Project/Audio/SFX/bounce.wav");
        am.wallClip=A("Assets/_Project/Audio/SFX/wall.wav");
        am.goalClip=A("Assets/_Project/Audio/SFX/goal.wav");
        am.clickClip=A("Assets/_Project/Audio/SFX/click.wav");
        am.winClip=A("Assets/_Project/Audio/SFX/win.wav");
        return am;
    }

    static void CreateMenuScene()
    {
        Scene scene=EditorSceneManager.NewScene(NewSceneSetup.EmptyScene,NewSceneMode.Single);
        CreateCamera(); CreateBackground(); CreateAudioManager(); EnsureEventSystem();
        Canvas canvas=CreateCanvas();
        CreateText(canvas.transform,"Title","PING-PONG GAME",new Vector2(0,265),new Vector2(1000,160),92,new Color(1f,.82f,.35f));
        CreateText(canvas.transform,"Subtitle","FANTASY DUEL",new Vector2(0,180),new Vector2(700,70),34,new Color(.9f,.92f,.95f));
        CreateButton(canvas.transform,"PlayButton","JUGAR",new Vector2(0,55),new Vector2(430,105));
        CreateButton(canvas.transform,"ControlsButton","CONTROLES",new Vector2(0,-75),new Vector2(430,105));
        CreateButton(canvas.transform,"ExitButton","SALIR",new Vector2(0,-205),new Vector2(430,105));
        GameObject controls=CreateImage(canvas.transform,"ControlsPanel",Vector2.zero,new Vector2(920,560),Color.white,S("Assets/_Project/Art/UI/panel.png"));
        CreateText(controls.transform,"ControlsTitle","CONTROLES",new Vector2(0,175),new Vector2(700,80),55,new Color(1f,.82f,.35f));
        CreateText(controls.transform,"ControlsText","PLAYER 1   W / S\n\nPLAYER 2   FLECHAS ARRIBA / ABAJO\n\nESC   PAUSA",new Vector2(0,15),new Vector2(780,300),38,Color.white);
        CreateButton(controls.transform,"ControlsBackButton","VOLVER",new Vector2(0,-205),new Vector2(350,90));
        controls.SetActive(false);
        var mc=canvas.gameObject.AddComponent<MenuController>(); mc.controlsPanel=controls;
        EditorSceneManager.SaveScene(scene,SceneDir+"/Menu.unity");
    }

    static GameObject CreateSpriteGO(string name, Sprite sp, Vector3 pos, Vector3 scale, int order=0)
    {
        GameObject go=new GameObject(name); go.transform.position=pos; go.transform.localScale=scale;
        var sr=go.AddComponent<SpriteRenderer>(); sr.sprite=sp; sr.sortingOrder=order;
        return go;
    }

    static void CreateGameScene()
    {
        Scene scene=EditorSceneManager.NewScene(NewSceneSetup.EmptyScene,NewSceneMode.Single);
        CreateCamera(); CreateBackground(); CreateAudioManager(); EnsureEventSystem();
        Sprite stone=S("Assets/_Project/Art/Sprites/stone_tile.png");
        GameObject top=CreateSpriteGO("TopBorder",stone,new Vector3(0,4.55f,0),new Vector3(18f,.65f,1),1);
        var tc=top.AddComponent<BoxCollider2D>(); tc.size=Vector2.one;
        GameObject bot=CreateSpriteGO("BottomBorder",stone,new Vector3(0,-4.55f,0),new Vector3(18f,.65f,1),1);
        var bc=bot.AddComponent<BoxCollider2D>(); bc.size=Vector2.one;
        for(int i=-4;i<=4;i++) CreateSpriteGO("CenterDot_"+i,S("Assets/_Project/Art/Sprites/center_dot.png"),new Vector3(0,i*.9f,0),Vector3.one,1);
        CreateSpriteGO("LeftGoalGlow",S("Assets/_Project/Art/Sprites/goal_glow.png"),new Vector3(-8.7f,0,0),new Vector3(1,1.1f,1),1);
        CreateSpriteGO("RightGoalGlow",S("Assets/_Project/Art/Sprites/goal_glow.png"),new Vector3(8.7f,0,0),new Vector3(1,1.1f,1),1);

        GameObject p1=CreateSpriteGO("PaddleLeft",S("Assets/_Project/Art/Sprites/paddle_red.png"),new Vector3(-7.6f,0,0),new Vector3(.55f,.55f,1),3);
        p1.AddComponent<BoxCollider2D>(); var rb1=p1.AddComponent<Rigidbody2D>(); rb1.bodyType=RigidbodyType2D.Kinematic; rb1.gravityScale=0;
        var pc1=p1.AddComponent<PaddleController>(); pc1.upKey=KeyCode.W; pc1.downKey=KeyCode.S;
        GameObject p2=CreateSpriteGO("PaddleRight",S("Assets/_Project/Art/Sprites/paddle_blue.png"),new Vector3(7.6f,0,0),new Vector3(.55f,.55f,1),3);
        p2.AddComponent<BoxCollider2D>(); var rb2=p2.AddComponent<Rigidbody2D>(); rb2.bodyType=RigidbodyType2D.Kinematic; rb2.gravityScale=0;
        var pc2=p2.AddComponent<PaddleController>(); pc2.upKey=KeyCode.UpArrow; pc2.downKey=KeyCode.DownArrow;

        GameObject ball=CreateSpriteGO("Ball",S("Assets/_Project/Art/Sprites/ball.png"),Vector3.zero,new Vector3(.55f,.55f,1),4);
        var cc=ball.AddComponent<CircleCollider2D>(); cc.sharedMaterial=AssetDatabase.LoadAssetAtPath<PhysicsMaterial2D>("Assets/_Project/Materials/Bouncy.physicsMaterial2D");
        var brb=ball.AddComponent<Rigidbody2D>(); brb.gravityScale=0; brb.collisionDetectionMode=CollisionDetectionMode2D.Continuous; brb.freezeRotation=true;
        var ballScript=ball.AddComponent<BallController>();

        GameObject gl=new GameObject("GoalLeft"); gl.transform.position=new Vector3(-9.3f,0,0); var glc=gl.AddComponent<BoxCollider2D>(); glc.isTrigger=true; glc.size=new Vector2(.8f,9f); var gls=gl.AddComponent<GoalTrigger>(); gls.scoringPlayer=2;
        GameObject gr=new GameObject("GoalRight"); gr.transform.position=new Vector3(9.3f,0,0); var grc=gr.AddComponent<BoxCollider2D>(); grc.isTrigger=true; grc.size=new Vector2(.8f,9f); var grs=gr.AddComponent<GoalTrigger>(); grs.scoringPlayer=1;

        Canvas canvas=CreateCanvas();
        CreateImage(canvas.transform,"ScorePanelP1",new Vector2(-300,430),new Vector2(220,110),Color.white,S("Assets/_Project/Art/UI/score_panel.png"));
        var s1=CreateText(canvas.transform,"ScoreP1","0",new Vector2(-300,430),new Vector2(180,100),62,Color.white).GetComponent<Text>();
        CreateImage(canvas.transform,"ScorePanelP2",new Vector2(300,430),new Vector2(220,110),Color.white,S("Assets/_Project/Art/UI/score_panel.png"));
        var s2=CreateText(canvas.transform,"ScoreP2","0",new Vector2(300,430),new Vector2(180,100),62,Color.white).GetComponent<Text>();
        CreateText(canvas.transform,"Player1Label","PLAYER 1",new Vector2(-300,505),new Vector2(260,50),26,new Color(1f,.75f,.65f));
        CreateText(canvas.transform,"Player2Label","PLAYER 2",new Vector2(300,505),new Vector2(260,50),26,new Color(.65f,.8f,1f));
        CreateText(canvas.transform,"PauseHint","ESC  PAUSA",new Vector2(760,500),new Vector2(300,50),25,new Color(.9f,.9f,.9f));

        GameObject pause=CreateImage(canvas.transform,"PausePanel",Vector2.zero,new Vector2(850,600),Color.white,S("Assets/_Project/Art/UI/panel.png"));
        CreateText(pause.transform,"PauseTitle","PAUSA",new Vector2(0,190),new Vector2(600,100),65,new Color(1f,.82f,.35f));
        CreateButton(pause.transform,"ResumeButton","CONTINUAR",new Vector2(0,70),new Vector2(420,90));
        CreateButton(pause.transform,"RestartButton","REINICIAR",new Vector2(0,-55),new Vector2(420,90));
        CreateButton(pause.transform,"PauseMenuButton","MENU",new Vector2(0,-180),new Vector2(420,90));
        pause.SetActive(false);

        GameObject win=CreateImage(canvas.transform,"WinPanel",Vector2.zero,new Vector2(980,650),Color.white,S("Assets/_Project/Art/UI/panel.png"));
        var winner=CreateText(win.transform,"WinnerText","PLAYER 1 GANA",new Vector2(0,170),new Vector2(800,120),70,new Color(1f,.82f,.35f)).GetComponent<Text>();
        CreateButton(win.transform,"RematchButton","REVANCHA",new Vector2(0,10),new Vector2(440,100));
        CreateButton(win.transform,"WinMenuButton","MENU",new Vector2(0,-135),new Vector2(440,100));
        win.SetActive(false);

        GameObject gmgo=new GameObject("GameManager");
        var sm=gmgo.AddComponent<ScoreManager>(); sm.player1Text=s1; sm.player2Text=s2;
        var gm=gmgo.AddComponent<GameManager>(); gm.ball=ballScript; gm.scoreManager=sm; gm.pausePanel=pause; gm.winPanel=win; gm.winnerText=winner; gm.maxScore=5;

        EditorSceneManager.SaveScene(scene,SceneDir+"/Game.unity");
    }
}
#endif
