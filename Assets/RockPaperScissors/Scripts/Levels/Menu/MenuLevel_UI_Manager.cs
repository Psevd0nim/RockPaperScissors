using System;
using UnityEngine;

namespace MyProject
{
    public class MenuLevel_UI_Manager : UI_Manager
    {
        public event Action OnPlayPressed;

        [SerializeField] private Button_UI _playButton;

        public override void Init(AudioManager audioManager)
        {
            base.Init(audioManager);
            _playButton.OnPressed += AfterPlayButtonPressed;
        }

        private void AfterPlayButtonPressed()
        {
            OnPlayPressed?.Invoke();
        }

        public void DisablePlayButton()
        {
            //_playButton.gameObject.SetActive(false);
            _playButton.SetInteractableStatus(false);
        }

        private void OnDestroy()
        {
            _playButton.OnPressed -= AfterPlayButtonPressed;
        }
    }
}
