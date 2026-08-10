using System;
using UnityEngine;

namespace MyProject
{
    public class Button_UI : MonoBehaviour
    {
        public event Action OnPressed;

        public virtual void Pressed()
        {
            OnPressed?.Invoke();
        }

        public void SetInteractableStatus(bool isInteractable)
        {
            TryGetComponent(out CanvasGroup canvasGroup);
            if (canvasGroup == null)
            {
                canvasGroup = gameObject.AddComponent<CanvasGroup>();
            }
            canvasGroup.alpha = isInteractable ? 1f : 0.5f;
            canvasGroup.interactable = isInteractable;
        }
    }
}
