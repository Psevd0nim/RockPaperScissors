using UnityEngine;
using UnityEngine.InputSystem;

namespace MyProject
{
    public class MobileInput : InputService
    {
        private Touchscreen _touchscreen;
        private Vector2 _startPos;
        private float _minMagnitude = 2;

        public MobileInput()
        {
            _touchscreen = Touchscreen.current;
        }

        public override void UpdateSomethingIfNeed()
        {
            if (LeftMouseOrSameWasPressedThisFrame())
            {
                _startPos = GetInputPosition();
            }
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

        public override Vector2 GetInputPosition()
        {
            Vector2 currentMousePosOnScreen = _touchscreen.primaryTouch.value.position;
            Vector2 touchPosInWorld = Camera.main.ScreenToWorldPoint(currentMousePosOnScreen);
            return touchPosInWorld;
        }

        public override bool LeftMouseOrSameWasPressedThisFrame()
        {
            return _touchscreen.primaryTouch.press.wasPressedThisFrame;
        }

        public override bool LeftMouseOrSameWasReleasedThisFrame()
        {
            return _touchscreen.primaryTouch.press.wasReleasedThisFrame;
        }
    }
}