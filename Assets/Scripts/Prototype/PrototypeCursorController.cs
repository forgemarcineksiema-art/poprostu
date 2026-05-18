using UnityEngine;
using UnityEngine.InputSystem;

namespace ValleDePlata.Prototype
{
    public readonly struct PrototypeCursorDecision
    {
        public PrototypeCursorDecision(CursorLockMode lockState, bool visible)
        {
            LockState = lockState;
            Visible = visible;
        }

        public CursorLockMode LockState { get; }
        public bool Visible { get; }
    }

    public sealed class PrototypeCursorController : MonoBehaviour
    {
        [SerializeField] private bool lockCursorOnPlay = true;
        [SerializeField] private bool hideCursorWhenLocked = true;
        [SerializeField] private bool escapeUnlocksCursor = true;
        [SerializeField] private bool clickRelocksCursor = true;

        private bool unlockedByEscape;

        private void OnEnable()
        {
            if (Application.isPlaying && lockCursorOnPlay)
            {
                unlockedByEscape = false;
                ApplyDecision(ResolveCursorDecision(lockCursorOnPlay, hideCursorWhenLocked, false, clickRelocksCursor, false, Cursor.lockState));
            }
        }

        private void OnDisable()
        {
            if (Application.isPlaying)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
        }

        private void OnApplicationFocus(bool hasFocus)
        {
            if (!Application.isPlaying)
            {
                return;
            }

            if (hasFocus && lockCursorOnPlay)
            {
                ApplyDecision(ResolveCursorDecision(lockCursorOnPlay, hideCursorWhenLocked, false, clickRelocksCursor, false, Cursor.lockState));
                return;
            }

            if (!hasFocus)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
        }

        private void Update()
        {
            if (!Application.isFocused)
            {
                return;
            }

            var escapePressed = escapeUnlocksCursor && Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame;
            var clickPressed = Mouse.current != null
                && (Mouse.current.leftButton.wasPressedThisFrame || Mouse.current.rightButton.wasPressedThisFrame);

            if (escapePressed)
            {
                unlockedByEscape = true;
                ApplyDecision(ResolveCursorDecision(lockCursorOnPlay, hideCursorWhenLocked, true, clickRelocksCursor, false, Cursor.lockState));
                return;
            }

            if (unlockedByEscape)
            {
                if (clickRelocksCursor && clickPressed)
                {
                    unlockedByEscape = false;
                    ApplyDecision(ResolveCursorDecision(lockCursorOnPlay, hideCursorWhenLocked, false, clickRelocksCursor, true, Cursor.lockState));
                    return;
                }

                ApplyDecision(new PrototypeCursorDecision(CursorLockMode.None, true));
                return;
            }

            ApplyDecision(ResolveCursorDecision(lockCursorOnPlay, hideCursorWhenLocked, escapePressed, clickRelocksCursor, clickPressed, Cursor.lockState));
        }

        public static PrototypeCursorDecision ResolveCursorDecision(
            bool lockCursorOnPlay,
            bool hideCursorWhenLocked,
            bool escapePressed,
            bool clickRelocksCursor,
            bool clickPressed,
            CursorLockMode currentLockState)
        {
            if (!lockCursorOnPlay || escapePressed)
            {
                return new PrototypeCursorDecision(CursorLockMode.None, true);
            }

            if (currentLockState == CursorLockMode.Locked || currentLockState == CursorLockMode.None || (clickRelocksCursor && clickPressed))
            {
                return new PrototypeCursorDecision(CursorLockMode.Locked, !hideCursorWhenLocked);
            }

            return new PrototypeCursorDecision(CursorLockMode.None, true);
        }

        private static void ApplyDecision(PrototypeCursorDecision decision)
        {
            Cursor.lockState = decision.LockState;
            Cursor.visible = decision.Visible;
        }
    }
}
