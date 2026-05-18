using System;
using UnityEngine;

namespace ValleDePlata.Prototype
{
    public enum FrontControl
    {
        Rival,
        Pablo,
        PabloWatched,
        Burned
    }

    public enum DirtyCashState
    {
        None,
        Loose,
        Carried,
        Hidden,
        Laundered,
        Seized
    }

    public enum PressureLevel
    {
        Low,
        Medium,
        High
    }

    public enum SocialLevel
    {
        Low,
        Neutral,
        High
    }

    public enum LieutenantTrust
    {
        Humiliated,
        Professional,
        Trusted
    }

    public enum RuleStyle
    {
        None,
        Favor,
        Bribe,
        Threat,
        ShowOfForce
    }

    public enum PrototypeWorldEvent
    {
        None,
        PublicViolenceCommitted,
        BribeAccepted,
        MateoProtected,
        MateoHumiliated
    }

    public sealed class PrototypeWorldState : MonoBehaviour
    {
        [SerializeField] private string districtId = "BarrioHondo";
        [SerializeField] private string frontId = "ElRespiroWorkshop";

        public static PrototypeWorldState Active { get; private set; }

        public event Action<PrototypeWorldState> Changed;

        public string DistrictId => districtId;
        public string FrontId => frontId;
        public FrontControl FrontControl { get; private set; }
        public DirtyCashState DirtyCash { get; private set; }
        public PressureLevel StatePressure { get; private set; }
        public SocialLevel PeopleLove { get; private set; }
        public SocialLevel Fear { get; private set; }
        public LieutenantTrust LieutenantTrust { get; private set; }
        public RuleStyle RuleStyleDecision { get; private set; }
        public PrototypeWorldEvent LastEvent { get; private set; }

        public void ResetState()
        {
            FrontControl = FrontControl.Rival;
            DirtyCash = DirtyCashState.None;
            StatePressure = PressureLevel.Low;
            PeopleLove = SocialLevel.Neutral;
            Fear = SocialLevel.Low;
            LieutenantTrust = LieutenantTrust.Professional;
            RuleStyleDecision = RuleStyle.None;
            LastEvent = PrototypeWorldEvent.None;
            UpdateDebugState();
            Changed?.Invoke(this);
        }

        public void ApplyEvent(PrototypeWorldEvent worldEvent)
        {
            if (worldEvent == PrototypeWorldEvent.None)
            {
                return;
            }

            switch (worldEvent)
            {
                case PrototypeWorldEvent.PublicViolenceCommitted:
                    Fear = SocialLevel.High;
                    PeopleLove = SocialLevel.Low;
                    StatePressure = RaisePressure(StatePressure);
                    RuleStyleDecision = RuleStyle.ShowOfForce;
                    break;
                case PrototypeWorldEvent.BribeAccepted:
                    StatePressure = LowerPressure(StatePressure);
                    DirtyCash = DirtyCashState.Hidden;
                    RuleStyleDecision = RuleStyle.Bribe;
                    break;
                case PrototypeWorldEvent.MateoProtected:
                    LieutenantTrust = LieutenantTrust.Trusted;
                    break;
                case PrototypeWorldEvent.MateoHumiliated:
                    LieutenantTrust = LieutenantTrust.Humiliated;
                    break;
            }

            LastEvent = worldEvent;
            UpdateDebugState();
            Changed?.Invoke(this);
        }

        public string BuildDebugSummary()
        {
            return
                $"District: {DistrictId} | Front: {FrontId}\n" +
                $"Control: {FrontControl} | DirtyCash: {DirtyCash} | StatePressure: {StatePressure}\n" +
                $"PeopleLove: {PeopleLove} | Fear: {Fear} | LieutenantTrust: {LieutenantTrust}\n" +
                $"RuleStyle: {RuleStyleDecision} | LastEvent: {LastEvent}";
        }

        private void Awake()
        {
            Active = this;
            ResetState();
        }

        private void OnEnable()
        {
            Active = this;
            UpdateDebugState();
        }

        private void OnDisable()
        {
            if (Active == this)
            {
                Active = null;
            }
        }

        private void UpdateDebugState()
        {
            PrototypeDebugState.World = BuildDebugSummary();
        }

        private static PressureLevel RaisePressure(PressureLevel current)
        {
            return current switch
            {
                PressureLevel.Low => PressureLevel.Medium,
                _ => PressureLevel.High
            };
        }

        private static PressureLevel LowerPressure(PressureLevel current)
        {
            return current switch
            {
                PressureLevel.High => PressureLevel.Medium,
                _ => PressureLevel.Low
            };
        }
    }
}
