using UnityEngine;

namespace ValleDePlata.Prototype
{
    public sealed class PrototypeWorldReactionMarker : MonoBehaviour
    {
        [SerializeField] private PrototypeWorldEvent reactsTo = PrototypeWorldEvent.PublicViolenceCommitted;
        [SerializeField] private string reactionMessage = "Street reaction changed";
        [SerializeField] private Renderer[] renderers;
        [SerializeField] private Color idleColor = new(0.42f, 0.42f, 0.36f);
        [SerializeField] private Color reactedColor = new(0.9f, 0.22f, 0.12f);

        private PrototypeWorldState connectedState;

        public bool Reacted { get; private set; }
        public string ReactionMessage => reactionMessage;

        public void Configure(PrototypeWorldEvent eventToReactTo, string message, Color idle, Color reacted)
        {
            reactsTo = eventToReactTo;
            reactionMessage = message;
            idleColor = idle;
            reactedColor = reacted;
            CacheRenderers();
            ApplyColor(idleColor);
        }

        private void Awake()
        {
            CacheRenderers();
            ApplyColor(idleColor);
        }

        private void Start()
        {
            Connect(PrototypeWorldState.Active);
        }

        private void OnDisable()
        {
            if (connectedState != null)
            {
                connectedState.Changed -= OnWorldStateChanged;
                connectedState = null;
            }
        }

        private void Connect(PrototypeWorldState state)
        {
            if (state == null || connectedState == state)
            {
                return;
            }

            if (connectedState != null)
            {
                connectedState.Changed -= OnWorldStateChanged;
            }

            connectedState = state;
            connectedState.Changed += OnWorldStateChanged;
            ApplyFromState(state);
        }

        private void OnWorldStateChanged(PrototypeWorldState state)
        {
            ApplyFromState(state);
        }

        private void ApplyFromState(PrototypeWorldState state)
        {
            if (state == null)
            {
                return;
            }

            if (state.LastEvent == PrototypeWorldEvent.None)
            {
                ResetReaction();
                return;
            }

            if (state.LastEvent != reactsTo)
            {
                return;
            }

            Reacted = true;
            ApplyColor(reactedColor);
            PrototypeDebugState.WorldReaction = reactionMessage;
        }

        private void ResetReaction()
        {
            Reacted = false;
            ApplyColor(idleColor);
            PrototypeDebugState.WorldReaction = "World reaction: none";
        }

        private void CacheRenderers()
        {
            if (renderers == null || renderers.Length == 0)
            {
                renderers = GetComponentsInChildren<Renderer>();
            }
        }

        private void ApplyColor(Color color)
        {
            foreach (var targetRenderer in renderers)
            {
                if (targetRenderer != null)
                {
                    targetRenderer.material.color = color;
                }
            }
        }
    }
}
