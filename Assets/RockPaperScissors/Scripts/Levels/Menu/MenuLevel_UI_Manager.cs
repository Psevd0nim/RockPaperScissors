using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MyProject
{
    public class MenuLevel_UI_Manager : UI_Manager
    {
        public event Action OnPlayPressed;
        public event Action<RpsChoice> ChoiceSelected;

        [SerializeField] private Button_UI _playButton;
        [SerializeField] private TMP_Text _countPlayersText;
        [SerializeField] private TMP_Text _waitingForPlayerText;
        [SerializeField] private GameObject _gameUIRoot;
        [SerializeField] private TMP_Text _localPlayerText;
        [SerializeField] private TMP_Text _opponentPlayerText;
        [SerializeField] private Button_UI _rockButton;
        [SerializeField] private Button_UI _paperButton;
        [SerializeField] private Button_UI _scissorsButton;
        [SerializeField] private Image _localChoiceImage;
        [SerializeField] private Image _opponentChoiceImage;
        [SerializeField] private TMP_Text _roundResultText;
        [SerializeField] private Sprite _rockSprite;
        [SerializeField] private Sprite _paperSprite;
        [SerializeField] private Sprite _scissorsSprite;

        private int _localPlayerId;
        private int _opponentPlayerId;

        public override void Init(AudioManager audioManager)
        {
            base.Init(audioManager);
            _playButton.OnPressed += AfterPlayButtonPressed;
            _rockButton.OnPressed += AfterRockButtonPressed;
            _paperButton.OnPressed += AfterPaperButtonPressed;
            _scissorsButton.OnPressed += AfterScissorsButtonPressed;
        }

        private void AfterPlayButtonPressed()
        {
            OnPlayPressed?.Invoke();
        }

        private void AfterRockButtonPressed()
        {
            ChoiceSelected?.Invoke(RpsChoice.Rock);
        }

        private void AfterPaperButtonPressed()
        {
            ChoiceSelected?.Invoke(RpsChoice.Paper);
        }

        private void AfterScissorsButtonPressed()
        {
            ChoiceSelected?.Invoke(RpsChoice.Scissors);
        }

        public void ShowPlayersCount(int playersCount)
        {
            _countPlayersText.text = $"Players: {playersCount}";
        }

        public void HidePlayers()
        {
            _waitingForPlayerText.gameObject.SetActive(false);
            _gameUIRoot.SetActive(false);
        }

        public void ShowWaitingForPlayer()
        {
            _waitingForPlayerText.gameObject.SetActive(true);
            _gameUIRoot.SetActive(false);
        }

        public void ShowGame(int localPlayerId, int opponentPlayerId)
        {
            _localPlayerId = localPlayerId;
            _opponentPlayerId = opponentPlayerId;

            _waitingForPlayerText.gameObject.SetActive(false);
            _gameUIRoot.SetActive(true);

            _localPlayerText.text = $"YOU\nPlayer {localPlayerId}\nScore: 0";
            _opponentPlayerText.text = $"OPPONENT\nPlayer {opponentPlayerId}\nScore: 0";

            ShowChoiceButtons();
            _localChoiceImage.gameObject.SetActive(false);
            _opponentChoiceImage.gameObject.SetActive(false);
            _roundResultText.gameObject.SetActive(false);
        }

        public void ShowLocalChoice(RpsChoice choice)
        {
            _localChoiceImage.sprite = GetChoiceSprite(choice);
            _localChoiceImage.gameObject.SetActive(true);

            _rockButton.gameObject.SetActive(false);
            _paperButton.gameObject.SetActive(false);
            _scissorsButton.gameObject.SetActive(false);
        }

        public void ShowRound(RpsChoice localChoice, RpsChoice opponentChoice, RpsRoundResult result)
        {
            _localChoiceImage.sprite = GetChoiceSprite(localChoice);
            _localChoiceImage.gameObject.SetActive(true);

            _opponentChoiceImage.sprite = GetChoiceSprite(opponentChoice);
            _opponentChoiceImage.gameObject.SetActive(true);

            _roundResultText.text = result.ToString();
            _roundResultText.gameObject.SetActive(true);
        }

        public void UpdateScores(int localScore, int opponentScore)
        {
            _localPlayerText.text = $"YOU\nPlayer {_localPlayerId}\nScore: {localScore}";
            _opponentPlayerText.text = $"OPPONENT\nPlayer {_opponentPlayerId}\nScore: {opponentScore}";
        }

        public void PrepareNextRound()
        {
            _localChoiceImage.gameObject.SetActive(false);
            _opponentChoiceImage.gameObject.SetActive(false);
            _roundResultText.gameObject.SetActive(false);
            ShowChoiceButtons();
        }

        private void ShowChoiceButtons()
        {
            _rockButton.gameObject.SetActive(true);
            _paperButton.gameObject.SetActive(true);
            _scissorsButton.gameObject.SetActive(true);
        }

        private Sprite GetChoiceSprite(RpsChoice choice)
        {
            switch (choice)
            {
                case RpsChoice.Rock:
                    return _rockSprite;
                case RpsChoice.Paper:
                    return _paperSprite;
                case RpsChoice.Scissors:
                    return _scissorsSprite;
                default:
                    return null;
            }
        }

        public void HidePlayButton()
        {
            _playButton.gameObject.SetActive(false);
        }

        public void ShowPlayButton()
        {
            _playButton.gameObject.SetActive(true);
        }

        private void OnDestroy()
        {
            _playButton.OnPressed -= AfterPlayButtonPressed;
            _rockButton.OnPressed -= AfterRockButtonPressed;
            _paperButton.OnPressed -= AfterPaperButtonPressed;
            _scissorsButton.OnPressed -= AfterScissorsButtonPressed;
        }
    }
}
