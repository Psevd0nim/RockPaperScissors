using UnityEngine;
using UnityEngine.InputSystem;

namespace MyProject
{
    public class DesktopInput : InputService
    {
        private Mouse _currentMouse;
        private Keyboard _currentKeyboard;

        public DesktopInput()
        {
            _currentMouse = Mouse.current;
            _currentKeyboard = Keyboard.current;
        }

        public override Vector2 GetInputPosition()
        {
            Vector2 currentMousePosOnScreen = _currentMouse.position.value;
            Vector2 mousePosInWorld = Camera.main.ScreenToWorldPoint(currentMousePosOnScreen);
            return mousePosInWorld;
        }

        public override InputType GetCurrentKeyWasPressedThisFrame()
        {
            InputType inputType = InputType.None;
            return inputType;
        }

        public override InputType GetCurrentKeyWasReleasedThisFrame()
        {
            InputType inputType = InputType.None;
            return inputType;
        }

        public override bool LeftMouseOrSameWasPressedThisFrame()
        {
            return _currentMouse.leftButton.wasPressedThisFrame;
        }

        public override bool LeftMouseOrSameWasReleasedThisFrame()
        {
            return _currentMouse.leftButton.wasReleasedThisFrame;
        }
    }
}