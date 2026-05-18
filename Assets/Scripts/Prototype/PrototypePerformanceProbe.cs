using UnityEngine;

namespace ValleDePlata.Prototype
{
    public sealed class PrototypePerformanceProbe : MonoBehaviour
    {
        private void Update()
        {
            PrototypeRunMetrics.Active?.RecordFrameTime(Time.unscaledDeltaTime);
        }
    }
}
