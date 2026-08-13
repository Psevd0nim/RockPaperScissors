using System;
using TMPro;
using UnityEngine;

namespace MyProject
{
    public class Game_UI_Manager : UI_Manager
    {
        public event Action<RPSElementType> ChoiceSelected;

        [SerializeField] private ConnectingIndicator _connectingIndicator;
        [SerializeField] private TextMeshProUGUI _countPlayersText;
        [SerializeField] private TextMeshProUGUI _statusText;
        [SerializeField] private RPSRoundInfo_UI _rpsRoundInfoUI;

        public override void Init(AudioManager audioManager)
        {
            base.Init(audioManager);
            _rpsRoundInfoUI.Init();
            _rpsRoundInfoUI.ChoiceSelected += OnChoiceSelected;
        }

        public void ShowPlayersCount(int playersCount)
        {
            _countPlayersText.text = $"Players: {playersCount}";
        }

        public void ShowConnectingIndicator()
        {
            _connectingIndicator.Show();
        }

        public void HideConnectingIndicator()
        {
            _connectingIndicator.Hide();
        }

        public void ShowLocalPlayer(string localPlayerName)
        {
            _rpsRoundInfoUI.ShowLocalPlayer(localPlayerName);
        }

        public void ShowWaitingForOpponent()
        {
            _statusText.gameObject.SetActive(false);
            _rpsRoundInfoUI.ShowWaitingForOpponent();
        }

        public void ShowPreparingPlayers(string localPlayerName, string opponentPlayerName)
        {
            _statusText.gameObject.SetActive(false);
            _rpsRoundInfoUI.ShowPreparingPlayers(localPlayerName, opponentPlayerName);
        }

        public void ShowConnectionFailed()
        {
            _statusText.text = "Connection failed";
            _statusText.gameObject.SetActive(true);
            _rpsRoundInfoUI.Hide();
        }

        public void ShowGame(string localPlayerName, string opponentPlayerName)
        {
            _statusText.gameObject.SetActive(false);
            _rpsRoundInfoUI.Show(localPlayerName, opponentPlayerName);
        }

        public void ShowLocalChoice(RPSElementType elementType)
        {
            _rpsRoundInfoUI.ShowLocalChoice(elementType);
        }

        public void ShowRound(RPSElementType localElement, RPSElementType opponentElement, RpsRoundResult result)
        {
            _rpsRoundInfoUI.ShowRound(localElement, opponentElement, result);
        }

        public void UpdateScores(int localScore, int opponentScore)
        {
            _rpsRoundInfoUI.UpdateScores(localScore, opponentScore);
        }

        public void PrepareNextRound()
        {
            _rpsRoundInfoUI.PrepareNextRound();
        }

        private void OnChoiceSelected(RPSElementType elementType)
        {
            ChoiceSelected?.Invoke(elementType);
        }

        private void OnDestroy()
        {
            if (_rpsRoundInfoUI != null)
                _rpsRoundInfoUI.ChoiceSelected -= OnChoiceSelected;
        }
    }
}
