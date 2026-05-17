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

        public void RecordSpeed(float speed)
        {
            MaxSpeed = Mathf.Max(MaxSpeed, speed);
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
                $"MaxSpeed: {MaxSpeed.ToString("0.0", CultureInfo.InvariantCulture)}",
                $"LastInteraction: {LastInteraction}",
                $"LastCheckpoint: {LastCheckpoint}");
        }

        public void WriteReport()
        {
            var path = Path.Combine(Application.persistentDataPath, reportFileName);
            File.WriteAllText(path, BuildSummary());
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
                $"cp {CompletedCheckpointCount} | pressure {PressureEntryCount} | int {InteractionCount}";
        }
    }
}
