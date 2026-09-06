using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

public static class BuildScript
{
    public static void PerformBuild()
    {
        string[] args = Environment.GetCommandLineArgs();
        string targetPlatformStr = GetCustomArg(args, "-customBuildTarget") ?? "StandaloneWindows64";
        string targetPath = GetCustomArg(args, "-customBuildPath");

        BuildTarget target;
        if (!Enum.TryParse(targetPlatformStr, true, out target))
        {
            target = BuildTarget.StandaloneWindows64;
        }

        if (string.IsNullOrEmpty(targetPath))
        {
            string ext = target switch
            {
                BuildTarget.StandaloneWindows64 => ".exe",
                BuildTarget.StandaloneWindows => ".exe",
                BuildTarget.StandaloneLinux64 => ".x86_64",
                BuildTarget.Android => ".apk",
                BuildTarget.StandaloneOSX => ".app",
                _ => ""
            };
            targetPath = Path.Combine("Build", targetPlatformStr, "HelloWorld" + ext);
        }

        string dir = Path.GetDirectoryName(targetPath);
        if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }

        string[] scenes = EditorBuildSettings.scenes
            .Where(s => s.enabled)
            .Select(s => s.path)
            .ToArray();

        if (scenes.Length == 0)
        {
            scenes = new[] { "Assets/Scenes/MainScene.unity" };
        }

        BuildPlayerOptions options = new BuildPlayerOptions
        {
            scenes = scenes,
            locationPathName = targetPath,
            target = target,
            options = BuildOptions.None
        };

        BuildReport report = BuildPipeline.BuildPlayer(options);
        BuildSummary summary = report.summary;
        Debug.Log("[BuildScript] Target: " + target + ", Result: " + summary.result + ", Size: " + summary.totalSize + " bytes");

        EditorApplication.Exit(summary.result == BuildResult.Succeeded ? 0 : 1);
    }

    private static string GetCustomArg(string[] args, string name)
    {
        for (int i = 0; i < args.Length - 1; i++)
        {
            if (string.Equals(args[i], name, StringComparison.OrdinalIgnoreCase))
                return args[i + 1];
        }
        return null;
    }
}
