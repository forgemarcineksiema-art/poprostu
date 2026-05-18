using System;
using UnityEngine;

namespace ValleDePlata.Prototype
{
    [Serializable]
    public struct PrototypeRouteCheckpointDefinition
    {
        [SerializeField] private string label;
        [SerializeField] private Vector3 position;
        [SerializeField] private Vector3 scale;

        public PrototypeRouteCheckpointDefinition(string label, Vector3 position, Vector3 scale)
        {
            this.label = label;
            this.position = position;
            this.scale = scale;
        }

        public string Label => label;
        public Vector3 Position => position;
        public Vector3 Scale => scale;
    }

    [CreateAssetMenu(menuName = "Valle de Plata/Prototype Slice Definition")]
    public sealed class PrototypeSliceDefinition : ScriptableObject
    {
        [SerializeField] private PrototypeRouteCheckpointDefinition[] routeCheckpoints;

        public PrototypeRouteCheckpointDefinition[] RouteCheckpoints => routeCheckpoints;

        public void ConfigurePhase1Defaults()
        {
            routeCheckpoints = new[]
            {
                new PrototypeRouteCheckpointDefinition("Start on foot", new Vector3(0f, 0.25f, -10f), new Vector3(3.4f, 0.5f, 1.4f)),
                new PrototypeRouteCheckpointDefinition("Enter vehicle lane", new Vector3(-2.4f, 0.25f, -4f), new Vector3(3.4f, 0.5f, 1.4f)),
                new PrototypeRouteCheckpointDefinition("Patrol pressure turn", new Vector3(0f, 0.25f, 34f), new Vector3(3.4f, 0.5f, 1.4f)),
                new PrototypeRouteCheckpointDefinition("Workshop interaction stop", new Vector3(2.5f, 0.25f, 49f), new Vector3(3.4f, 0.5f, 1.4f)),
                new PrototypeRouteCheckpointDefinition("Safe return", new Vector3(0f, 0.25f, -8f), new Vector3(3.4f, 0.5f, 1.4f))
            };
        }

        public bool Validate(out string error)
        {
            if (routeCheckpoints == null || routeCheckpoints.Length == 0)
            {
                error = "Slice definition has no route checkpoints.";
                return false;
            }

            for (var i = 0; i < routeCheckpoints.Length; i++)
            {
                if (string.IsNullOrWhiteSpace(routeCheckpoints[i].Label))
                {
                    error = $"Route checkpoint {i} is missing a label.";
                    return false;
                }
            }

            error = string.Empty;
            return true;
        }
    }
}
