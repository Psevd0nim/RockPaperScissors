using System.Collections;
using UnityEngine;

namespace MyProject
{
    public class RpsRoundController : MonoBehaviour
    {
        private MenuLevel_UI_Manager _menuUI;
        private NetworkPlayerEntity _localPlayerEntity;
        private NetworkPlayerEntity _opponentPlayerEntity;
        private Coroutine _showResultCoroutine;
        private bool _roundInProgress;
        private bool _waitingForChoicesReset;

        public void Init(MenuLevel_UI_Manager menuUI)
        {
            _menuUI = menuUI;
            _menuUI.ChoiceSelected += SelectChoice;
        }

        public void StartGame(NetworkPlayerEntity localPlayerEntity, NetworkPlayerEntity opponentPlayerEntity, int localPlayerId, int opponentPlayerId)
        {
            StopGame(false);

            _localPlayerEntity = localPlayerEntity;
            _opponentPlayerEntity = opponentPlayerEntity;

            _localPlayerEntity.EntityChanged += PlayerEntityChanged;
            _opponentPlayerEntity.EntityChanged += PlayerEntityChanged;

            _menuUI.ShowGame(localPlayerId, opponentPlayerId);
            PlayerEntityChanged();
        }

        public void StopGame(bool resetMatch)
        {
            if (_showResultCoroutine != null)
            {
                StopCoroutine(_showResultCoroutine);
                _showResultCoroutine = null;
            }

            if (_localPlayerEntity != null)
                _localPlayerEntity.EntityChanged -= PlayerEntityChanged;

            if (_opponentPlayerEntity != null)
                _opponentPlayerEntity.EntityChanged -= PlayerEntityChanged;

            if (resetMatch && _localPlayerEntity != null)
                _localPlayerEntity.ResetMatch();

            _localPlayerEntity = null;
            _opponentPlayerEntity = null;
            _roundInProgress = false;
            _waitingForChoicesReset = false;
        }

        private void SelectChoice(RpsChoice choice)
        {
            if (_localPlayerEntity == null)
                return;

            if (_localPlayerEntity.CanSelectChoice == false)
                return;

            _localPlayerEntity.SelectChoice(choice);
            _menuUI.ShowLocalChoice(_localPlayerEntity.Choice);
        }

        private void PlayerEntityChanged()
        {
            if (_localPlayerEntity == null || _opponentPlayerEntity == null)
                return;

            _menuUI.UpdateScores(_localPlayerEntity.Score, _opponentPlayerEntity.Score);

            if (_waitingForChoicesReset)
            {
                StartNextRoundWhenChoicesAreReset();
                return;
            }

            if (_roundInProgress)
                return;

            bool bothPlayersSelected = _localPlayerEntity.Choice != RpsChoice.None && _opponentPlayerEntity.Choice != RpsChoice.None;

            if (bothPlayersSelected == false)
                return;

            _roundInProgress = true;
            _showResultCoroutine = StartCoroutine(ShowRoundResult());
        }

        private IEnumerator ShowRoundResult()
        {
            RpsRoundResult result = GetRoundResult(_localPlayerEntity.Choice, _opponentPlayerEntity.Choice);

            if (result == RpsRoundResult.Win)
                _localPlayerEntity.AddPoint();

            _menuUI.ShowRound(_localPlayerEntity.Choice, _opponentPlayerEntity.Choice, result);

            _menuUI.UpdateScores(_localPlayerEntity.Score, _opponentPlayerEntity.Score);

            yield return new WaitForSeconds(2f);

            _waitingForChoicesReset = true;
            _showResultCoroutine = null;
            _localPlayerEntity.ResetChoice();
            StartNextRoundWhenChoicesAreReset();
        }

        private void StartNextRoundWhenChoicesAreReset()
        {
            bool bothChoicesAreReset = _localPlayerEntity.Choice == RpsChoice.None && _opponentPlayerEntity.Choice == RpsChoice.None;

            if (bothChoicesAreReset == false)
                return;

            _waitingForChoicesReset = false;
            _roundInProgress = false;
            _menuUI.PrepareNextRound();
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

        private void OnDestroy()
        {
            if (_menuUI != null)
                _menuUI.ChoiceSelected -= SelectChoice;

            StopGame(false);
        }
    }
}
