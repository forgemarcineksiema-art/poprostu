using UnityEngine;

namespace ValleDePlata.Prototype
{
    public sealed class PrototypeDebugHud : MonoBehaviour
    {
        [SerializeField] private bool visible = true;

        private readonly GUIStyle style = new();

        private void Awake()
        {
            style.fontSize = 18;
            style.normal.textColor = Color.white;
            style.padding = new RectOffset(12, 12, 10, 10);
        }

        private void OnGUI()
        {
            if (!visible)
            {
                return;
            }

            var text =
                "Phase 1 Feel Prototype\n" +
                "WASD / Left Stick: move or drive\n" +
                "Mouse / Right Stick: camera\n" +
                "Shift: sprint | Space/LT: handbrake\n" +
                "E / South Button: interact\n\n" +
                $"Mode: {PrototypeDebugState.Mode}\n" +
                $"Speed: {PrototypeDebugState.Speed:0.0}\n" +
                $"Focus: {PrototypeDebugState.Focus}\n" +
                $"Interaction: {PrototypeDebugState.Interaction}\n" +
                $"Pressure: {PrototypeDebugState.Pressure}\n" +
                $"Route: {PrototypeDebugState.Route}\n" +
                $"Checkpoint: {PrototypeDebugState.LastCheckpoint}\n" +
                $"Metrics: {PrototypeDebugState.Metrics}";

            GUI.Box(new Rect(16, 16, 680, 315), GUIContent.none);
            GUI.Label(new Rect(20, 20, 670, 310), text, style);
        }
    }
}
