﻿using GPF;
using GPFEditor.Settings;
using UnityEditor;
using UnityEngine;
using System;
using UnityEditor.Callbacks;

public static class BuildWebBackend
{
    [MenuItem("BUILD/Test")]
    static void Build()
    {
        try
        {
            var userSettings = GPFSettings.userSettings;

            var uri = System.Environment.GetEnvironmentVariable("BACKEND_URI");

            userSettings.deployEnv.backendType = DeployEnv.BACKEND_TYPE.SIMULATED;
            //userSettings.deployEnv.CustomURI = uri;

            string[] scenes = { "Assets/React.unity" };
            var report = BuildPipeline.BuildPlayer(scenes, "Build/web", BuildTarget.WebGL, BuildOptions.None);
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
