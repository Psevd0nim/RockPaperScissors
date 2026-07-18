using UnityEngine;

namespace MyProject
{
    public interface IInputService : IService
    {
        public abstract Vector2 GetInputPosition();
        public abstract InputType GetCurrentKeyWasPressedThisFrame();
        public abstract InputType GetCurrentKeyWasReleasedThisFrame();
        public abstract bool LeftMouseOrSameWasPressedThisFrame();
        public abstract bool LeftMouseOrSameWasReleasedThisFrame();
        public abstract void UpdateSomethingIfNeed();
    }
}