using UnityEngine;

namespace ValleDePlata.Prototype
{
    public enum PrototypeReadablePropKind
    {
        StreetIdentity,
        SafeReturn,
        RiosCheckpoint,
        PoliceRoadblock,
        Workshop
    }

    public sealed class PrototypeReadableProp : MonoBehaviour
    {
        [SerializeField] private PrototypeReadablePropKind kind;
        [SerializeField] private string displayName = "Readable prop";
        [SerializeField] private string gameplayAnchor = "none";

        public PrototypeReadablePropKind Kind => kind;
        public string DisplayName => displayName;
        public string GameplayAnchor => gameplayAnchor;
        public int RendererCount => GetComponentsInChildren<Renderer>(true).Length;

        public void Configure(PrototypeReadablePropKind nextKind, string nextDisplayName, string nextGameplayAnchor)
        {
            kind = nextKind;
            displayName = string.IsNullOrWhiteSpace(nextDisplayName) ? nextKind.ToString() : nextDisplayName;
            gameplayAnchor = string.IsNullOrWhiteSpace(nextGameplayAnchor) ? "none" : nextGameplayAnchor;
        }
    }
}
