using System;
using System.IO;
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
        MateoHumiliated,
        DirtyCashPickedUp,
        FrontTakenUnderWatch,
        DirtyCashSeized,
        PressureCrackdownTriggered
    }

    [Serializable]
    public sealed class PrototypeWorldStateSnapshot
    {
        public string districtId = "BarrioHondo";
        public string frontId = "ElRespiroWorkshop";
        public FrontControl frontControl = FrontControl.Rival;
        public DirtyCashState dirtyCash = DirtyCashState.None;
        public PressureLevel statePressure = PressureLevel.Low;
        public SocialLevel peopleLove = SocialLevel.Neutral;
        public SocialLevel fear = SocialLevel.Low;
        public LieutenantTrust lieutenantTrust = LieutenantTrust.Professional;
        public RuleStyle ruleStyleDecision = RuleStyle.None;
        public PrototypeWorldEvent lastEvent = PrototypeWorldEvent.None;
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

        public PrototypeWorldStateSnapshot CaptureSnapshot()
        {
            return new PrototypeWorldStateSnapshot
            {
                districtId = DistrictId,
                frontId = FrontId,
                frontControl = FrontControl,
                dirtyCash = DirtyCash,
                statePressure = StatePressure,
                peopleLove = PeopleLove,
                fear = Fear,
                lieutenantTrust = LieutenantTrust,
                ruleStyleDecision = RuleStyleDecision,
                lastEvent = LastEvent
            };
        }

        public void ApplySnapshot(PrototypeWorldStateSnapshot snapshot)
        {
            if (snapshot == null)
            {
                throw new ArgumentNullException(nameof(snapshot));
            }

            districtId = string.IsNullOrWhiteSpace(snapshot.districtId) ? "BarrioHondo" : snapshot.districtId;
            frontId = string.IsNullOrWhiteSpace(snapshot.frontId) ? "ElRespiroWorkshop" : snapshot.frontId;
            FrontControl = snapshot.frontControl;
            DirtyCash = snapshot.dirtyCash;
            StatePressure = snapshot.statePressure;
            PeopleLove = snapshot.peopleLove;
            Fear = snapshot.fear;
            LieutenantTrust = snapshot.lieutenantTrust;
            RuleStyleDecision = snapshot.ruleStyleDecision;
            LastEvent = snapshot.lastEvent;
            UpdateDebugState();
            Changed?.Invoke(this);
        }

        public string CaptureJson(bool prettyPrint = false)
        {
            return JsonUtility.ToJson(CaptureSnapshot(), prettyPrint);
        }

        public void ApplyJson(string json)
        {
            if (string.IsNullOrWhiteSpace(json))
            {
                throw new ArgumentException("World state JSON is empty.", nameof(json));
            }

            ApplySnapshot(JsonUtility.FromJson<PrototypeWorldStateSnapshot>(json));
        }

        public void SaveSnapshot(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                throw new ArgumentException("Snapshot path is empty.", nameof(path));
            }

            File.WriteAllText(path, CaptureJson(true));
        }

        public void LoadSnapshot(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                throw new ArgumentException("Snapshot path is empty.", nameof(path));
            }

            ApplyJson(File.ReadAllText(path));
        }

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

        public bool ApplyEvent(PrototypeWorldEvent worldEvent)
        {
            if (worldEvent == PrototypeWorldEvent.None)
            {
                return false;
            }

            if (!CanApplyEvent(worldEvent))
            {
                PrototypeDebugState.WorldReaction = $"World event blocked: {worldEvent}";
                return false;
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
                case PrototypeWorldEvent.DirtyCashPickedUp:
                    DirtyCash = DirtyCashState.Carried;
                    StatePressure = RaisePressure(StatePressure);
                    break;
                case PrototypeWorldEvent.FrontTakenUnderWatch:
                    FrontControl = FrontControl.PabloWatched;
                    DirtyCash = DirtyCashState.Hidden;
                    RuleStyleDecision = RuleStyle.Favor;
                    StatePressure = LieutenantTrust == LieutenantTrust.Trusted
                        ? LowerPressure(StatePressure)
                        : RaisePressure(StatePressure);
                    break;
                case PrototypeWorldEvent.DirtyCashSeized:
                    DirtyCash = DirtyCashState.Seized;
                    StatePressure = PressureLevel.High;
                    break;
                case PrototypeWorldEvent.PressureCrackdownTriggered:
                    Fear = SocialLevel.High;
                    PeopleLove = SocialLevel.Low;
                    StatePressure = PressureLevel.High;
                    RuleStyleDecision = RuleStyle.ShowOfForce;
                    break;
            }

            LastEvent = worldEvent;
            UpdateDebugState();
            Changed?.Invoke(this);
            return true;
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

        private bool CanApplyEvent(PrototypeWorldEvent worldEvent)
        {
            return worldEvent switch
            {
                PrototypeWorldEvent.FrontTakenUnderWatch => DirtyCash == DirtyCashState.Carried,
                PrototypeWorldEvent.DirtyCashSeized => DirtyCash == DirtyCashState.Carried,
                PrototypeWorldEvent.DirtyCashPickedUp => DirtyCash is DirtyCashState.None or DirtyCashState.Loose,
                PrototypeWorldEvent.PressureCrackdownTriggered => StatePressure != PressureLevel.Low,
                _ => true
            };
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
