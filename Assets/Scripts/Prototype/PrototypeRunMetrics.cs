using System.Globalization;
using System.IO;
using UnityEngine;

namespace ValleDePlata.Prototype
{
    public sealed class PrototypeRunMetrics : MonoBehaviour
    {
        [SerializeField] private string reportFileName = "phase1_latest_run.txt";
        [SerializeField] private bool writeReportOnQuit = true;

        private float startTime;
        private readonly PrototypePerformanceSampler performance = new();

        public static PrototypeRunMetrics Active { get; private set; }

        public float ElapsedSeconds => Mathf.Max(0f, Time.realtimeSinceStartup - startTime);
        public int VehicleEntryCount { get; private set; }
        public int VehicleExitCount { get; private set; }
        public int InteractionCount { get; private set; }
        public int PressureEntryCount { get; private set; }
        public int CompletedCheckpointCount { get; private set; }
        public bool RouteCompleted { get; private set; }
        public float MaxSpeed { get; private set; }
        public string LastInteraction { get; private set; } = "None";
        public string LastCheckpoint { get; private set; } = "None";
        public string LastReportPath { get; private set; } = "Not written";
        public PrototypeRouteOutcome RouteOutcome { get; private set; } = PrototypeRouteOutcome.InProgress;
        public float AverageFps => performance.AverageFps;
        public float WorstFrameMs => performance.WorstFrameMs;
        public string PerformanceStatus => performance.Status;
        public bool HasRouteCoverage =>
            VehicleEntryCount > 0
            && VehicleExitCount > 0
            && InteractionCount > 0
            && PressureEntryCount > 0
            && RouteCompleted
            && MaxSpeed >= 1f;

        public string CoverageStatus => HasRouteCoverage ? "Coverage complete" : $"Missing: {BuildMissingCoverageSummary()}";

        public void ResetRun()
        {
            startTime = Time.realtimeSinceStartup;
            VehicleEntryCount = 0;
            VehicleExitCount = 0;
            InteractionCount = 0;
            PressureEntryCount = 0;
            CompletedCheckpointCount = 0;
            RouteCompleted = false;
            MaxSpeed = 0f;
            LastInteraction = "None";
            LastCheckpoint = "None";
            LastReportPath = "Not written";
            RouteOutcome = PrototypeRouteOutcome.InProgress;
            performance.Reset();
            UpdateDebugState();
        }

        public void RecordVehicleEnter()
        {
            VehicleEntryCount++;
            UpdateDebugState();
        }

        public void RecordVehicleExit()
        {
            VehicleExitCount++;
            UpdateDebugState();
        }

        public void RecordInteraction(string label)
        {
            InteractionCount++;
            LastInteraction = string.IsNullOrWhiteSpace(label) ? "Interaction" : label;
            UpdateDebugState();
        }

        public void RecordPressureEnter()
        {
            PressureEntryCount++;
            UpdateDebugState();
        }

        public void RecordCheckpoint(string label, bool routeComplete)
        {
            CompletedCheckpointCount++;
            LastCheckpoint = string.IsNullOrWhiteSpace(label) ? "Checkpoint" : label;
            RouteCompleted = routeComplete;
            UpdateDebugState();
        }

        public void RecordRouteOutcome(PrototypeRouteOutcome outcome)
        {
            RouteOutcome = outcome;
            UpdateDebugState();
        }

        public void RecordSpeed(float speed)
        {
            MaxSpeed = Mathf.Max(MaxSpeed, speed);
            UpdateDebugState();
        }

        public void RecordFrameTime(float unscaledDeltaTime)
        {
            performance.RecordFrame(unscaledDeltaTime);
            UpdateDebugState();
        }

        public string BuildSummary()
        {
            return string.Join(
                "\n",
                "Phase 1 Feel Prototype Run",
                $"ElapsedSeconds: {ElapsedSeconds.ToString("0.0", CultureInfo.InvariantCulture)}",
                $"VehicleEntries: {VehicleEntryCount}",
                $"VehicleExits: {VehicleExitCount}",
                $"Interactions: {InteractionCount}",
                $"PressureEntries: {PressureEntryCount}",
                $"CompletedCheckpoints: {CompletedCheckpointCount}",
                $"RouteCompleted: {RouteCompleted}",
                $"RouteOutcome: {RouteOutcome}",
                $"MaxSpeed: {MaxSpeed.ToString("0.0", CultureInfo.InvariantCulture)}",
                $"LastInteraction: {LastInteraction}",
                $"LastCheckpoint: {LastCheckpoint}",
                $"AverageFps: {AverageFps.ToString("0.0", CultureInfo.InvariantCulture)}",
                $"WorstFrameMs: {WorstFrameMs.ToString("0.0", CultureInfo.InvariantCulture)}",
                $"PerformanceStatus: {PerformanceStatus}",
                $"CoverageComplete: {HasRouteCoverage}",
                $"CoverageStatus: {CoverageStatus}",
                "ManualFeelGate: Required");
        }

        public string BuildMissingCoverageSummary()
        {
            var missing = string.Empty;
            AppendMissing(ref missing, VehicleEntryCount > 0, "enter car");
            AppendMissing(ref missing, VehicleExitCount > 0, "exit car");
            AppendMissing(ref missing, MaxSpeed >= 1f, "drive");
            AppendMissing(ref missing, PressureEntryCount > 0, "pressure");
            AppendMissing(ref missing, InteractionCount > 0, "interaction");
            AppendMissing(ref missing, RouteCompleted, "safe return");
            return string.IsNullOrEmpty(missing) ? "none" : missing;
        }

        public void WriteReport()
        {
            var path = Path.Combine(Application.persistentDataPath, reportFileName);
            var summary = BuildSummary();
            if (!HasRouteCoverage && ExistingReportHasCompleteCoverage(path))
            {
                File.WriteAllText(path + ".incomplete", summary);
                LastReportPath = path;
                UpdateDebugState();
                Debug.Log($"Phase 1 run metrics preserved complete coverage report at {path}");
                return;
            }

            File.WriteAllText(path, summary);
            LastReportPath = path;
            UpdateDebugState();
            Debug.Log($"Phase 1 run metrics written to {path}");
        }

        private void Awake()
        {
            ResetRun();
        }

        private void OnEnable()
        {
            Active = this;
        }

        private void OnDisable()
        {
            if (Active == this)
            {
                Active = null;
            }
        }

        private void OnApplicationQuit()
        {
            if (writeReportOnQuit)
            {
                WriteReport();
            }
        }

        private void UpdateDebugState()
        {
            PrototypeDebugState.Metrics =
                $"t {ElapsedSeconds:0}s | car {VehicleEntryCount}/{VehicleExitCount} | " +
                $"cp {CompletedCheckpointCount} | pressure {PressureEntryCount} | int {InteractionCount} | " +
                $"{RouteOutcome} | " +
                (HasRouteCoverage ? "coverage OK" : $"missing {BuildMissingCoverageSummary()}");
            PrototypeDebugState.Performance = performance.BuildHudLine();
        }

        private static void AppendMissing(ref string target, bool passed, string label)
        {
            if (passed)
            {
                return;
            }

            if (!string.IsNullOrEmpty(target))
            {
                target += ", ";
            }

            target += label;
        }

        private static bool ExistingReportHasCompleteCoverage(string path)
        {
            if (!File.Exists(path))
            {
                return false;
            }

            var report = File.ReadAllText(path);
            return report.Contains("CoverageComplete: True", System.StringComparison.OrdinalIgnoreCase)
                && report.Contains("CoverageStatus: Coverage complete", System.StringComparison.OrdinalIgnoreCase);
        }
    }
}
