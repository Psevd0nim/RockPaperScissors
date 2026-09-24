using System;
using UnityEngine;

namespace MyProject
{
    public class MenuLevel_UI_Manager : UI_Manager
    {
        public event Action OnPlayPressed;
        public event Action OnVsBotPressed;

        public RoomCode_UI RoomCode_UI => _roomCode_UI;

        [SerializeField] private RoomCode_UI _roomCode_UI;
        [SerializeField] private Button_UI _playButton;
        [SerializeField] private Button_UI _vsBotButton;

        public override void Init(AudioManager audioManager)
        {
            base.Init(audioManager);
            _playButton.OnPressed += AfterPlayButtonPressed;
            _vsBotButton.OnPressed += AfterVsBotButtonPressed;
        }

        private void AfterPlayButtonPressed()
        {
            OnPlayPressed?.Invoke();
        }

        private void AfterVsBotButtonPressed()
        {
            OnVsBotPressed?.Invoke();
        }

        public void DisablePlayButton()
        {
            _playButton.SetInteractableStatus(false);
        }

        private void OnDestroy()
        {
            _playButton.OnPressed -= AfterPlayButtonPressed;
            _vsBotButton.OnPressed -= AfterVsBotButtonPressed;
        }
    }
}
