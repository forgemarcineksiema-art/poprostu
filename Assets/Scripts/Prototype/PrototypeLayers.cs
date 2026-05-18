using System.Text;
using UnityEngine;

namespace ValleDePlata.Prototype
{
    public static class PrototypeLayers
    {
        public const string WorldStaticName = "WorldStatic";
        public const string WorldDynamicName = "WorldDynamic";
        public const string PlayerName = "Player";
        public const string VehicleName = "Vehicle";
        public const string InteractableName = "Interactable";
        public const string RouteTriggerName = "RouteTrigger";
        public const string SensorTriggerName = "SensorTrigger";
        public const string CameraIgnoreName = "CameraIgnore";
        public const string NpcName = "NPC";

        public static int WorldStatic => LayerMask.NameToLayer(WorldStaticName);
        public static int WorldDynamic => LayerMask.NameToLayer(WorldDynamicName);
        public static int Player => LayerMask.NameToLayer(PlayerName);
        public static int Vehicle => LayerMask.NameToLayer(VehicleName);
        public static int Interactable => LayerMask.NameToLayer(InteractableName);
        public static int RouteTrigger => LayerMask.NameToLayer(RouteTriggerName);
        public static int SensorTrigger => LayerMask.NameToLayer(SensorTriggerName);
        public static int CameraIgnore => LayerMask.NameToLayer(CameraIgnoreName);
        public static int Npc => LayerMask.NameToLayer(NpcName);

        public static int WorldCollisionMask => BuildMask(WorldStatic, WorldDynamic);
        public static int CameraCollisionMask => WorldCollisionMask;
        public static int InteractionQueryMask => BuildMask(Interactable, Vehicle);
        public static int ExitBlockMask => BuildMask(0, WorldStatic, WorldDynamic, Vehicle, Npc);

        public static bool AreConfigured(out string missing)
        {
            var builder = new StringBuilder();
            AppendMissingLayer(builder, WorldStaticName, WorldStatic);
            AppendMissingLayer(builder, WorldDynamicName, WorldDynamic);
            AppendMissingLayer(builder, PlayerName, Player);
            AppendMissingLayer(builder, VehicleName, Vehicle);
            AppendMissingLayer(builder, InteractableName, Interactable);
            AppendMissingLayer(builder, RouteTriggerName, RouteTrigger);
            AppendMissingLayer(builder, SensorTriggerName, SensorTrigger);
            AppendMissingLayer(builder, CameraIgnoreName, CameraIgnore);
            AppendMissingLayer(builder, NpcName, Npc);
            missing = builder.ToString();
            return missing.Length == 0;
        }

        public static int BuildMask(params int[] layers)
        {
            var mask = 0;
            foreach (var layer in layers)
            {
                if (layer >= 0)
                {
                    mask |= 1 << layer;
                }
            }

            return mask;
        }

        public static void SetLayerRecursively(GameObject target, int layer)
        {
            if (target == null || layer < 0)
            {
                return;
            }

            target.layer = layer;
            foreach (Transform child in target.transform)
            {
                SetLayerRecursively(child.gameObject, layer);
            }
        }

        private static void AppendMissingLayer(StringBuilder builder, string layerName, int layer)
        {
            if (layer >= 0)
            {
                return;
            }

            if (builder.Length > 0)
            {
                builder.Append(", ");
            }

            builder.Append(layerName);
        }
    }
}
