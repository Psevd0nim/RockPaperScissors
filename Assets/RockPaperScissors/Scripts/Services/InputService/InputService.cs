using UnityEngine;

namespace MyProject
{
    public abstract class InputService : IInputService
    {
        public abstract Vector2 GetInputPosition();
        public abstract InputType GetCurrentKeyWasPressedThisFrame();
        public abstract InputType GetCurrentKeyWasReleasedThisFrame();
        public abstract bool LeftMouseOrSameWasPressedThisFrame();
        public abstract bool LeftMouseOrSameWasReleasedThisFrame();

        public virtual void UpdateSomethingIfNeed()
        {
            
        }
    }
}