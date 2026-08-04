using System;
using TMPro;
using UnityEngine;

namespace MyProject
{
    public class MenuLevel_UI_Manager : UI_Manager
    {
        public event Action<string> ConnectRequested;
        public event Action DisconnectRequested;

        public event Action OnPlayPressed;

        [SerializeField] private Button_UI _playButton;

        [Header("Network")]
        [SerializeField] private TMP_InputField _sessionNameInput;
        [SerializeField] private TMP_Text _connectionStatusText;
        [SerializeField] private TMP_Text _playersText;
        [SerializeField] private Button_UI _connectButton;
        [SerializeField] private Button_UI _disconnectButton;

        public override void Init(AudioManager audioManager)
        {
            base.Init(audioManager);

            if (_connectButton != null)
                _connectButton.OnPressed += RequestConnect;

            if (_disconnectButton != null)
                _disconnectButton.OnPressed += RequestDisconnect;

            _playButton.OnPressed += AfterPlayButtonPressed;
        }

        private void AfterPlayButtonPressed()
        {
            OnPlayPressed?.Invoke();
        }

        public void ShowConnectionStatus(string status)
        {
            if (_connectionStatusText != null)
                _connectionStatusText.text = status;
        }

        public void ShowPlayers(string players)
        {
            if (_playersText != null)
                _playersText.text = players;
        }

        private void RequestConnect()
        {
            string sessionName = _sessionNameInput == null ? "rps-room" : _sessionNameInput.text;
            ConnectRequested?.Invoke(sessionName);
        }

        private void RequestDisconnect()
        {
            DisconnectRequested?.Invoke();
        }

        private void OnDestroy()
        {
            if (_connectButton != null)
                _connectButton.OnPressed -= RequestConnect;

            if (_disconnectButton != null)
                _disconnectButton.OnPressed -= RequestDisconnect;
        }
    }
}
