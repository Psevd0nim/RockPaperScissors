using System.Collections;
using UnityEngine;

namespace MyProject
{
    public class RpsRoundController : MonoBehaviour
    {
        private MenuLevel_UI_Manager _menuUI;
        private NetworkPlayerEntity _localPlayerEntity;
        private NetworkPlayerEntity _opponentPlayerEntity;

        public void Init(MenuLevel_UI_Manager menuUI)
        {
            _menuUI = menuUI;
            _menuUI.ChoiceSelected += SelectChoice;
        }

        public void StartGame(NetworkPlayerEntity localPlayerEntity, NetworkPlayerEntity opponentPlayerEntity, int localPlayerId, int opponentPlayerId)
        {
            _localPlayerEntity = localPlayerEntity;
            _opponentPlayerEntity = opponentPlayerEntity;

            _localPlayerEntity.ChoiceChanged += PlayerChoiceChanged;
            _opponentPlayerEntity.ChoiceChanged += PlayerChoiceChanged;
            _localPlayerEntity.ScoreChanged += UpdateScores;
            _opponentPlayerEntity.ScoreChanged += UpdateScores;

            _menuUI.ShowGame(localPlayerId, opponentPlayerId);
            UpdateScores();
        }

        public void ResetGame()
        {
            //При каком условии эта проверка может сработать? В рамках текущего кода
            if (_localPlayerEntity == null)
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

        private void SelectChoice(RpsChoice choice)
        {
            _localPlayerEntity.SelectChoice(choice);
            _menuUI.ShowLocalChoice(choice);
        }

        private void PlayerChoiceChanged()
        {
            bool bothPlayersSelected = _localPlayerEntity.Choice != RpsChoice.None && _opponentPlayerEntity.Choice != RpsChoice.None;
            bool bothChoicesAreReset = _localPlayerEntity.Choice == RpsChoice.None && _opponentPlayerEntity.Choice == RpsChoice.None;

            if (bothPlayersSelected)
                StartCoroutine(ShowRoundResult());
            else if (bothChoicesAreReset)
                _menuUI.PrepareNextRound();
        }

        private IEnumerator ShowRoundResult()
        {
            RpsRoundResult result = GetRoundResult(_localPlayerEntity.Choice, _opponentPlayerEntity.Choice);

            if (result == RpsRoundResult.Win)
                _localPlayerEntity.AddPoint();

            _menuUI.ShowRound(_localPlayerEntity.Choice, _opponentPlayerEntity.Choice, result);

            yield return new WaitForSeconds(2f);

            _localPlayerEntity.ResetChoice();
        }

        private void UpdateScores()
        {
            _menuUI.UpdateScores(_localPlayerEntity.Score, _opponentPlayerEntity.Score);
        }

        private RpsRoundResult GetRoundResult(RpsChoice localChoice, RpsChoice opponentChoice)
        {
            if (localChoice == opponentChoice)
                return RpsRoundResult.Draw;

            bool localPlayerWon =
                localChoice == RpsChoice.Rock && opponentChoice == RpsChoice.Scissors
                || localChoice == RpsChoice.Paper && opponentChoice == RpsChoice.Rock
                || localChoice == RpsChoice.Scissors && opponentChoice == RpsChoice.Paper;

            return localPlayerWon ? RpsRoundResult.Win : RpsRoundResult.Lose;
        }
    }
}
