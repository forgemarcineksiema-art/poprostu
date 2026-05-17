using System.IO;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace ValleDePlata.Editor
{
    public static class PrototypeBuildPipeline
    {
        private const string ScenePath = "Assets/Scenes/Phase1_FeelPrototype.unity";
        private const string BuildPath = "Builds/Phase1/ValleDePlataPhase1.exe";

        public static void BuildPhase1Windows()
        {
            var outputDirectory = Path.GetDirectoryName(BuildPath);
            if (!string.IsNullOrEmpty(outputDirectory))
            {
                Directory.CreateDirectory(outputDirectory);
            }

            var options = new BuildPlayerOptions
            {
                scenes = new[] { ScenePath },
                locationPathName = BuildPath,
                target = BuildTarget.StandaloneWindows64,
                options = BuildOptions.Development,
            };

            var report = BuildPipeline.BuildPlayer(options);
            var summary = report.summary;
            Debug.Log($"Phase 1 build result: {summary.result}, size: {summary.totalSize}, time: {summary.totalTime}");

            if (summary.result != BuildResult.Succeeded)
            {
                throw new System.InvalidOperationException($"Phase 1 build failed: {summary.result}");
            }
        }
    }
}
