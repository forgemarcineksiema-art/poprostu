using System.Collections.Generic;
using UnityEngine;

namespace ValleDePlata.Prototype
{
    public enum PrototypeInteractionKind
    {
        None,
        Interactable,
        Vehicle
    }

    public readonly struct PrototypeInteractionCandidate
    {
        public PrototypeInteractionCandidate(
            Transform transform,
            PrototypeInteractionKind kind,
            string prompt,
            int priority,
            bool blocked)
        {
            Transform = transform;
            Kind = kind;
            Prompt = prompt;
            Priority = priority;
            Blocked = blocked;
            Component = null;
        }

        public PrototypeInteractionCandidate(
            Component component,
            PrototypeInteractionKind kind,
            string prompt,
            int priority,
            bool blocked)
        {
            Component = component;
            Transform = component != null ? component.transform : null;
            Kind = kind;
            Prompt = prompt;
            Priority = priority;
            Blocked = blocked;
        }

        public Component Component { get; }
        public Transform Transform { get; }
        public PrototypeInteractionKind Kind { get; }
        public string Prompt { get; }
        public int Priority { get; }
        public bool Blocked { get; }
    }

    public static class PrototypeInteractionTargeting
    {
        public static bool TryFindBest(
            Vector3 origin,
            Vector3 facing,
            float radius,
            int queryMask,
            int occlusionMask,
            out PrototypeInteractionCandidate target)
        {
            var hits = Physics.OverlapSphere(origin, radius, queryMask, QueryTriggerInteraction.Collide);
            var candidates = new List<PrototypeInteractionCandidate>(hits.Length);
            foreach (var hit in hits)
            {
                var vehicle = hit.GetComponentInParent<PrototypeVehicleController>();
                if (vehicle != null)
                {
                    candidates.Add(new PrototypeInteractionCandidate(
                        vehicle,
                        PrototypeInteractionKind.Vehicle,
                        "E / South Button: enter car",
                        100,
                        IsBlocked(origin, vehicle.transform, occlusionMask)));
                    continue;
                }

                var interactable = hit.GetComponentInParent<PrototypeInteractable>();
                if (interactable != null)
                {
                    candidates.Add(new PrototypeInteractionCandidate(
                        interactable,
                        PrototypeInteractionKind.Interactable,
                        "E / South Button: " + interactable.Prompt,
                        50,
                        IsBlocked(origin, interactable.transform, occlusionMask)));
                }
            }

            return SelectBest(origin, candidates, out target);
        }

        public static bool SelectBest(
            Vector3 origin,
            IReadOnlyList<PrototypeInteractionCandidate> candidates,
            out PrototypeInteractionCandidate target)
        {
            target = default;
            var hasTarget = false;
            var bestVisibleRank = int.MinValue;
            var bestPriority = int.MinValue;
            var bestDistance = float.MaxValue;

            foreach (var candidate in candidates)
            {
                if (candidate.Transform == null || candidate.Kind == PrototypeInteractionKind.None)
                {
                    continue;
                }

                var visibleRank = candidate.Blocked ? 0 : 1;
                var distance = Vector3.SqrMagnitude(candidate.Transform.position - origin);
                var isBetter = !hasTarget
                    || visibleRank > bestVisibleRank
                    || (visibleRank == bestVisibleRank && candidate.Priority > bestPriority)
                    || (visibleRank == bestVisibleRank && candidate.Priority == bestPriority && distance < bestDistance);

                if (!isBetter)
                {
                    continue;
                }

                target = candidate;
                hasTarget = true;
                bestVisibleRank = visibleRank;
                bestPriority = candidate.Priority;
                bestDistance = distance;
            }

            return hasTarget;
        }

        private static bool IsBlocked(Vector3 origin, Transform target, int occlusionMask)
        {
            if (target == null || occlusionMask == 0)
            {
                return false;
            }

            var start = origin + Vector3.up * 1.35f;
            var end = target.position + Vector3.up * 0.8f;
            if (!Physics.Linecast(start, end, out var hit, occlusionMask, QueryTriggerInteraction.Ignore))
            {
                return false;
            }

            return hit.collider != null && !hit.collider.transform.IsChildOf(target);
        }
    }
}
