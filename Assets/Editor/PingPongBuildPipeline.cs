#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

public static class PingPongBuildPipeline
{
    const string Output = "Build/Windows/Ping-Pong_Game.exe";

    [MenuItem("Tools/Ping Pong/3 - Build Windows x64")]
    public static void BuildWindows()
    {
        PingPongProjectBuilder.BuildAll();
        Directory.CreateDirectory("Build/Windows");

        BuildPlayerOptions options = new BuildPlayerOptions
        {
            scenes = new[]
            {
                PingPongProjectBuilder.SceneDir + "/Menu.unity",
                PingPongProjectBuilder.SceneDir + "/Game.unity"
            },
            locationPathName = Output,
            target = BuildTarget.StandaloneWindows64,
            options = BuildOptions.None
        };

        BuildReport report = BuildPipeline.BuildPlayer(options);
        BuildSummary summary = report.summary;

        if (summary.result == BuildResult.Succeeded)
            Debug.Log($"PING-PONG_GAME BUILD OK: {Output} ({summary.totalSize / 1024 / 1024} MB)");
        else
            throw new System.Exception("Windows build failed: " + summary.result);
    }

    public static void BuildWindowsCommandLine()
    {
        BuildWindows();
        EditorApplication.Exit(0);
    }
}
#endif
