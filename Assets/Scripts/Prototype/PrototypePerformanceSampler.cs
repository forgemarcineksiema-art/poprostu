using System.Globalization;
using UnityEngine;

namespace ValleDePlata.Prototype
{
    public sealed class PrototypePerformanceSampler
    {
        private float totalFrameSeconds;
        private float worstFrameSeconds;

        public int SampleCount { get; private set; }
        public float AverageFps => totalFrameSeconds > 0f ? SampleCount / totalFrameSeconds : 0f;
        public float WorstFrameMs => worstFrameSeconds * 1000f;
        public string Status
        {
            get
            {
                if (SampleCount == 0)
                {
                    return "No samples";
                }

                if (WorstFrameMs >= 45f)
                {
                    return "Frame spikes";
                }

                return AverageFps < 50f ? "Low FPS" : "OK";
            }
        }

        public void Reset()
        {
            totalFrameSeconds = 0f;
            worstFrameSeconds = 0f;
            SampleCount = 0;
        }

        public void RecordFrame(float unscaledDeltaTime)
        {
            if (unscaledDeltaTime <= 0f || float.IsNaN(unscaledDeltaTime) || float.IsInfinity(unscaledDeltaTime))
            {
                return;
            }

            totalFrameSeconds += unscaledDeltaTime;
            worstFrameSeconds = Mathf.Max(worstFrameSeconds, unscaledDeltaTime);
            SampleCount++;
        }

        public string BuildHudLine()
        {
            if (SampleCount == 0)
            {
                return "FPS -- | worst --ms | No samples";
            }

            return string.Format(
                CultureInfo.InvariantCulture,
                "FPS {0:0} | worst {1:0}ms | {2}",
                AverageFps,
                WorstFrameMs,
                Status);
        }
    }
}
