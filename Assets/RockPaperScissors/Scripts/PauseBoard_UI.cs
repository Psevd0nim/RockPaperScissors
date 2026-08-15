using System;
using UnityEngine;

namespace MyProject
{
    public class PauseBoard_UI : MonoBehaviour
    {
        public event Action ResumePressed;
        public event Action MenuPressed;

        [SerializeField] private Button_UI _resumeButton;
        [SerializeField] private Button_UI _menuButton;

        public void Init()
        {
            _resumeButton.OnPressed += OnResumePressed;
            _menuButton.OnPressed += OnMenuPressed;
        }

        private void OnResumePressed()
        {
            ResumePressed?.Invoke();
        }

        private void OnMenuPressed()
        {
            MenuPressed?.Invoke();
        }

        private void OnDestroy()
        {
            _resumeButton.OnPressed -= OnResumePressed;
            _menuButton.OnPressed -= OnMenuPressed;
        }
    }
}
