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
        [SerializeField] private TextMeshProUGUI _waitingForPlayerText;
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

        public void ShowWaitingForOpponent()
        {
            _waitingForPlayerText.text = "Waiting for opponent...";
            _waitingForPlayerText.gameObject.SetActive(true);
            _rpsRoundInfoUI.Hide();
        }

        public void ShowConnectionFailed()
        {
            _waitingForPlayerText.text = "Connection failed";
            _waitingForPlayerText.gameObject.SetActive(true);
            _rpsRoundInfoUI.Hide();
        }

        public void ShowGame(int localPlayerId, int opponentPlayerId)
        {
            _waitingForPlayerText.gameObject.SetActive(false);
            _rpsRoundInfoUI.Show(localPlayerId, opponentPlayerId);
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
