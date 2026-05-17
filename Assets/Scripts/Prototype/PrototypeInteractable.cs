using UnityEngine;

namespace ValleDePlata.Prototype
{
    public sealed class PrototypeInteractable : MonoBehaviour
    {
        [SerializeField] private string prompt = "Inspect workshop shutter";
        [SerializeField] private string usedMessage = "Workshop contact noted";
        [SerializeField] private Renderer[] highlightRenderers;
        [SerializeField] private Color idleColor = new(0.7f, 0.62f, 0.46f);
        [SerializeField] private Color usedColor = new(0.35f, 0.62f, 0.52f);

        private bool used;

        public string Prompt => used ? usedMessage : prompt;

        public void Interact()
        {
            used = true;
            PrototypeDebugState.Interaction = usedMessage;
            ApplyColor(usedColor);
        }

        private void Awake()
        {
            if (highlightRenderers == null || highlightRenderers.Length == 0)
            {
                highlightRenderers = GetComponentsInChildren<Renderer>();
            }

            ApplyColor(idleColor);
        }

        private void ApplyColor(Color color)
        {
            foreach (var targetRenderer in highlightRenderers)
            {
                if (targetRenderer != null)
                {
                    targetRenderer.material.color = color;
                }
            }
        }
    }
}
