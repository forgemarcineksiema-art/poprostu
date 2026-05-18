using System;
using UnityEngine;

namespace ValleDePlata.Prototype
{
    public sealed class PrototypePlayerHud : MonoBehaviour
    {
        [SerializeField] private bool visible = true;
        [SerializeField] private PrototypeObjectiveMarker objectiveMarker;

        private readonly GUIStyle objectiveStyle = new();
        private readonly GUIStyle promptStyle = new();
        private readonly GUIStyle statusStyle = new();

        public void AttachObjectiveMarker(PrototypeObjectiveMarker marker)
        {
            objectiveMarker = marker;
        }

        public static string BuildObjectiveLine(string objective)
        {
            if (string.IsNullOrWhiteSpace(objective))
            {
                return "Objective unavailable";
            }

            var trimmed = objective.Trim();
            trimmed = StripPrefix(trimmed, "Objective complete:");
            trimmed = StripPrefix(trimmed, "Objective changed:");
            trimmed = StripPrefix(trimmed, "Objective:");
            return CapitalizeFirst(trimmed.Trim());
        }

        public static string BuildPromptLine(string prompt)
        {
            if (string.IsNullOrWhiteSpace(prompt)
                || prompt.Equals("None", StringComparison.OrdinalIgnoreCase))
            {
                return string.Empty;
            }

            if (prompt.Equals("Interaction blocked", StringComparison.OrdinalIgnoreCase)
                || prompt.Equals("Exit blocked", StringComparison.OrdinalIgnoreCase))
            {
                return "Blocked";
            }

            if (prompt.Contains("exit car", StringComparison.OrdinalIgnoreCase))
            {
                return "E / A  Exit car";
            }

            return $"E / A  {CapitalizeFirst(prompt.Trim())}";
        }

        private void Awake()
        {
            objectiveStyle.fontSize = 22;
            objectiveStyle.fontStyle = FontStyle.Bold;
            objectiveStyle.alignment = TextAnchor.MiddleCenter;
            objectiveStyle.normal.textColor = Color.white;

            promptStyle.fontSize = 20;
            promptStyle.fontStyle = FontStyle.Bold;
            promptStyle.alignment = TextAnchor.MiddleCenter;
            promptStyle.normal.textColor = new Color(0.98f, 0.92f, 0.78f);

            statusStyle.fontSize = 14;
            statusStyle.alignment = TextAnchor.MiddleLeft;
            statusStyle.normal.textColor = new Color(0.86f, 0.88f, 0.82f);
        }

        private void OnGUI()
        {
            if (!visible)
            {
                return;
            }

            if (objectiveMarker == null)
            {
                objectiveMarker = FindAnyObjectByType<PrototypeObjectiveMarker>();
            }

            var objective = BuildObjectiveLine(objectiveMarker != null ? objectiveMarker.CurrentObjective : string.Empty);
            DrawCenteredPanel(18f, Mathf.Min(720f, Screen.width - 32f), 46f, objective, objectiveStyle, new Color(0.04f, 0.05f, 0.045f, 0.72f));

            var prompt = BuildPromptLine(PrototypeDebugState.Interaction);
            if (!string.IsNullOrEmpty(prompt))
            {
                DrawCenteredPanel(Screen.height - 86f, Mathf.Min(520f, Screen.width - 32f), 44f, prompt, promptStyle, new Color(0.08f, 0.07f, 0.045f, 0.78f));
            }

            var status = $"{PrototypeDebugState.Mode} | {PrototypeDebugState.Pressure}";
            GUI.Label(new Rect(20f, Screen.height - 36f, 420f, 24f), status, statusStyle);
        }

        private static void DrawCenteredPanel(float y, float width, float height, string text, GUIStyle style, Color color)
        {
            var rect = new Rect((Screen.width - width) * 0.5f, y, width, height);
            var previousColor = GUI.color;
            GUI.color = color;
            GUI.DrawTexture(rect, Texture2D.whiteTexture);
            GUI.color = previousColor;
            GUI.Label(rect, text, style);
        }

        private static string StripPrefix(string value, string prefix)
        {
            return value.StartsWith(prefix, StringComparison.OrdinalIgnoreCase)
                ? value[prefix.Length..]
                : value;
        }

        private static string CapitalizeFirst(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return string.Empty;
            }

            value = value.Trim();
            return value.Length == 1
                ? value.ToUpperInvariant()
                : char.ToUpperInvariant(value[0]) + value[1..];
        }
    }
}
