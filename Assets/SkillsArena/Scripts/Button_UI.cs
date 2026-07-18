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
    }
}
