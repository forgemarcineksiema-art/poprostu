using UnityEngine;

namespace ValleDePlata.Prototype
{
    public sealed class PrototypeObjectiveMarker : MonoBehaviour
    {
        [SerializeField] private PrototypeMissionSpine missionSpine;
        [SerializeField] private string currentObjective = "Objective: collect dirty cash at El Respiro";

        public string CurrentObjective => currentObjective;

        public void AttachMissionSpine(PrototypeMissionSpine nextMissionSpine)
        {
            missionSpine = nextMissionSpine;
            Refresh();
        }

        public void Refresh()
        {
            if (missionSpine == null)
            {
                missionSpine = FindAnyObjectByType<PrototypeMissionSpine>();
            }

            currentObjective = missionSpine != null
                ? missionSpine.ObjectivePrompt
                : "Objective: unavailable";
        }

        private void Update()
        {
            Refresh();
        }
    }
}
