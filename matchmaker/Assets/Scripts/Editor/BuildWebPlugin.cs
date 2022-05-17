using GPF;
using GPFEditor.Settings;
using UnityEditor;
using UnityEngine;
using System;
using UnityEditor.Callbacks;
using System.IO;

public static class BuildWebPlugin
{
    const string buildPath = "Build/web";
    const string localPath = "../gpf-react/gpf-libs/local";
    const string remotePath = "../gpf-react/gpf-libs/remote";

    [MenuItem("BUILD/Local")]
    static void Local()
    {
        Build(DeployEnv.BACKEND_TYPE.SIMULATED, localPath, "");
    }

    [MenuItem("BUILD/Remote")]
    static void RemoteFromEditor()
    {
        Build(DeployEnv.BACKEND_TYPE.CUSTOM_URL, remotePath, "wss://gpf-react.gameplumbers.com:8282");
    }

    static void Remote()
    {
        var uri = System.Environment.GetEnvironmentVariable("BACKEND_URI");

        Build(DeployEnv.BACKEND_TYPE.CUSTOM_URL, remotePath, uri);
    }

    static void Build(DeployEnv.BACKEND_TYPE backend, string outputDirectory, string uri)
    {
        try
        {
            var userSettings = GPFSettings.userSettings;

            userSettings.deployEnv.backendType = backend;
            userSettings.deployEnv.CustomURI = uri;

            string[] scenes = { "Assets/React.unity" };
            var report = BuildPipeline.BuildPlayer(scenes, buildPath, BuildTarget.WebGL, BuildOptions.None);

            if (report.summary.result != UnityEditor.Build.Reporting.BuildResult.Succeeded)
            {
                throw new Exception("BuildPlayer failure: " + report.summary);
            }

            if (!Directory.Exists(outputDirectory))
                Directory.CreateDirectory(outputDirectory);

            if (Directory.Exists(outputDirectory))
                Directory.Delete(outputDirectory, true);

            Directory.Move($"{buildPath}/Build", outputDirectory);
        }
        catch (Exception e)
        {
            Debug.LogException(e);
            if (Application.isBatchMode)
                EditorApplication.Exit(1);
        }
    }

    [PostProcessBuild(9900)]
    public static void OnPostprocessBuild(BuildTarget target, string pathToBuiltProject)
    {
        if (Application.isBatchMode)
            EditorApplication.Exit(0);
    }
}
