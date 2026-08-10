using System.Collections;
using UnityEngine;

namespace MyProject
{
    public class RpsRoundManager : MonoBehaviour
    {
        public bool IsMatchActive => _localPlayerEntity != null;

        private Game_UI_Manager _gameUI;
        private NetworkPlayerEntity _localPlayerEntity;
        private NetworkPlayerEntity _opponentPlayerEntity;

        public void Init(Game_UI_Manager gameUI)
        {
            _gameUI = gameUI;
            _gameUI.ChoiceSelected += SelectChoice;
        }

        public void StartMatch(NetworkPlayerEntity localPlayerEntity, NetworkPlayerEntity opponentPlayerEntity, int localPlayerId, int opponentPlayerId)
        {
            if (IsMatchActive)
                return;

            _localPlayerEntity = localPlayerEntity;
            _opponentPlayerEntity = opponentPlayerEntity;

            _localPlayerEntity.ChoiceChanged += PlayerChoiceChanged;
            _opponentPlayerEntity.ChoiceChanged += PlayerChoiceChanged;
            _localPlayerEntity.ScoreChanged += UpdateScores;
            _opponentPlayerEntity.ScoreChanged += UpdateScores;

            _gameUI.ShowGame(localPlayerId, opponentPlayerId);
            UpdateScores();
        }

        public void EndMatch()
        {
            if (IsMatchActive == false)
                return;

            StopAllCoroutines();

            _localPlayerEntity.ChoiceChanged -= PlayerChoiceChanged;
            _localPlayerEntity.ScoreChanged -= UpdateScores;

            if (_opponentPlayerEntity != null)
            {
                _opponentPlayerEntity.ChoiceChanged -= PlayerChoiceChanged;
                _opponentPlayerEntity.ScoreChanged -= UpdateScores;
            }

            _localPlayerEntity.Reset();

            _localPlayerEntity = null;
            _opponentPlayerEntity = null;
        }

        private void SelectChoice(RPSElementType elementType)
        {
            if (IsMatchActive == false)
                return;

            _localPlayerEntity.SelectChoice(elementType);
            _gameUI.ShowLocalChoice(elementType);
        }

        private void PlayerChoiceChanged()
        {
            bool bothPlayersSelected = _localPlayerEntity.Choice != RPSElementType.None && _opponentPlayerEntity.Choice != RPSElementType.None;
            bool bothChoicesAreReset = _localPlayerEntity.Choice == RPSElementType.None && _opponentPlayerEntity.Choice == RPSElementType.None;

            if (bothPlayersSelected)
                StartCoroutine(ShowRoundResult());
            else if (bothChoicesAreReset)
                _gameUI.PrepareNextRound();
        }

        private IEnumerator ShowRoundResult()
        {
            RpsRoundResult result = GetRoundResult(_localPlayerEntity.Choice, _opponentPlayerEntity.Choice);

            if (result == RpsRoundResult.Win)
                _localPlayerEntity.AddPoint();

            _gameUI.ShowRound(_localPlayerEntity.Choice, _opponentPlayerEntity.Choice, result);

            yield return new WaitForSeconds(2f);

            _localPlayerEntity.ResetChoice();
        }

        private void UpdateScores()
        {
            _gameUI.UpdateScores(_localPlayerEntity.Score, _opponentPlayerEntity.Score);
        }

        private RpsRoundResult GetRoundResult(RPSElementType localElement, RPSElementType opponentElement)
        {
            if (localElement == opponentElement)
                return RpsRoundResult.Draw;

            bool localPlayerWon =
                localElement == RPSElementType.Rock && opponentElement == RPSElementType.Scissors
                || localElement == RPSElementType.Paper && opponentElement == RPSElementType.Rock
                || localElement == RPSElementType.Scissors && opponentElement == RPSElementType.Paper;

            return localPlayerWon ? RpsRoundResult.Win : RpsRoundResult.Lose;
        }

        private void OnDestroy()
        {
            if (_gameUI != null)
                _gameUI.ChoiceSelected -= SelectChoice;
        }
    }
}
