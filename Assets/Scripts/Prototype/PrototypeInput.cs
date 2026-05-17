using UnityEngine;
using UnityEngine.InputSystem;

namespace ValleDePlata.Prototype
{
    public static class PrototypeInput
    {
        public static Vector2 Move
        {
            get
            {
                var keyboard = Keyboard.current;
                var gamepad = Gamepad.current;
                var move = Vector2.zero;

                if (keyboard != null)
                {
                    if (keyboard.wKey.isPressed || keyboard.upArrowKey.isPressed)
                    {
                        move.y += 1f;
                    }

                    if (keyboard.sKey.isPressed || keyboard.downArrowKey.isPressed)
                    {
                        move.y -= 1f;
                    }

                    if (keyboard.dKey.isPressed || keyboard.rightArrowKey.isPressed)
                    {
                        move.x += 1f;
                    }

                    if (keyboard.aKey.isPressed || keyboard.leftArrowKey.isPressed)
                    {
                        move.x -= 1f;
                    }
                }

                if (gamepad != null)
                {
                    move += gamepad.leftStick.ReadValue();
                }

                return Vector2.ClampMagnitude(move, 1f);
            }
        }

        public static Vector2 Look
        {
            get
            {
                var mouse = Mouse.current;
                var gamepad = Gamepad.current;
                var look = Vector2.zero;

                if (mouse != null)
                {
                    look += mouse.delta.ReadValue();
                }

                if (gamepad != null)
                {
                    look += gamepad.rightStick.ReadValue() * 18f;
                }

                return look;
            }
        }

        public static bool SprintHeld
        {
            get
            {
                var keyboard = Keyboard.current;
                var gamepad = Gamepad.current;
                return (keyboard != null && keyboard.leftShiftKey.isPressed)
                    || (gamepad != null && gamepad.leftStickButton.isPressed);
            }
        }

        public static bool InteractPressedThisFrame
        {
            get
            {
                var keyboard = Keyboard.current;
                var gamepad = Gamepad.current;
                return (keyboard != null && keyboard.eKey.wasPressedThisFrame)
                    || (gamepad != null && gamepad.buttonSouth.wasPressedThisFrame);
            }
        }

        public static bool HandbrakeHeld
        {
            get
            {
                var keyboard = Keyboard.current;
                var gamepad = Gamepad.current;
                return (keyboard != null && keyboard.spaceKey.isPressed)
                    || (gamepad != null && gamepad.leftTrigger.isPressed);
            }
        }
    }
}
