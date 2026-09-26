using System.Collections;
using UnityEngine;

namespace MyProject
{
    public class RpsMatchManager : MonoBehaviour
    {
        public bool IsMatchActive { get; private set; }

        private Game_UI_Manager _gameUI;
        private NetworkPlayerEntity _localPlayerEntity;
        private NetworkPlayerEntity _opponentPlayerEntity;

        private bool _isOfflineMode;

        public void Init(Game_UI_Manager gameUI)
        {
            _gameUI = gameUI;
        }

        public void StartMatch(NetworkPlayerEntity localPlayerEntity, NetworkPlayerEntity opponentPlayerEntity, bool isOfflineMode = false)
        {
            _localPlayerEntity = localPlayerEntity;
            _opponentPlayerEntity = opponentPlayerEntity;
            _isOfflineMode = isOfflineMode;
            IsMatchActive = true;

            _gameUI.ElementSelected += SelectElement;
            _localPlayerEntity.SelectedElementChanged += TryStartRoundFight;
            _opponentPlayerEntity.SelectedElementChanged += TryStartRoundFight;
            _localPlayerEntity.ScoreChanged += UpdateScores;
            _opponentPlayerEntity.ScoreChanged += UpdateScores;

            _gameUI.ShowGame(localPlayerEntity.Nickname, opponentPlayerEntity.Nickname);
            UpdateScores();
        }

        public void EndMatch()
        {
            if (IsMatchActive == false)
                return;

            IsMatchActive = false;
            StopAllCoroutines();

            _gameUI.ElementSelected -= SelectElement;
            _localPlayerEntity.SelectedElementChanged -= TryStartRoundFight;
            _localPlayerEntity.ScoreChanged -= UpdateScores;

            if (_opponentPlayerEntity != null)
            {
                _opponentPlayerEntity.SelectedElementChanged -= TryStartRoundFight;
                _opponentPlayerEntity.ScoreChanged -= UpdateScores;
            }

            _localPlayerEntity = null;
            _opponentPlayerEntity = null;
        }

        private void SelectElement(RPSElementType elementType)
        {
            _localPlayerEntity.SetSelectedElement(elementType);
            _gameUI.ShowLocalSelectedElement(elementType);
        }

        private void TryStartRoundFight()
        {
            if (_localPlayerEntity.IsReady)
            {
                if (_opponentPlayerEntity.IsReady)
                    StartCoroutine(ShowRoundResult()); 
                else
                {
                    if (_isOfflineMode)
                        _opponentPlayerEntity.SetSelectedElement((RPSElementType)Random.Range(1, 4));
                    else
                        _gameUI.ShowLocalSelectedElement(_localPlayerEntity.SelectedElement);
                }
            }
        }

        private IEnumerator ShowRoundResult()
        {
            RpsRoundResult result = GetRoundResult(_localPlayerEntity.SelectedElement, _opponentPlayerEntity.SelectedElement);
            switch (result)
            {
                case RpsRoundResult.Win:
                    _localPlayerEntity.AddPoint();
                    break;
                case RpsRoundResult.Lose:
                    if (_isOfflineMode)
                        _opponentPlayerEntity.AddPoint();
                    break;
            }
            UpdateScores();

            _gameUI.ShowRound(_localPlayerEntity.SelectedElement, _opponentPlayerEntity.SelectedElement, result);

            yield return new WaitForSeconds(2f);

            _localPlayerEntity.ResetSelectedElement();

            if (_isOfflineMode)
                _opponentPlayerEntity.ResetSelectedElement();

            _gameUI.ShowElementSelection();
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
            if(_gameUI != null)
                _gameUI.ElementSelected -= SelectElement;
        }
    }
}
