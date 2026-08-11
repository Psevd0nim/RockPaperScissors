using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MyProject
{
    public class RPSRoundInfo_UI : MonoBehaviour
    {
        public event Action<RPSElementType> ChoiceSelected;

        [SerializeField] private RPSConfig _rpsConfig;
        [SerializeField] private List<RPSButton_UI> _choiceButtons;
        [SerializeField] private TextMeshProUGUI _localPlayerText;
        [SerializeField] private TextMeshProUGUI _opponentPlayerText;
        [SerializeField] private Image _localChoiceImage;
        [SerializeField] private Image _opponentChoiceImage;
        [SerializeField] private TextMeshProUGUI _roundResultText;

        private string _localPlayerName;
        private string _opponentPlayerName;

        public void Init()
        {
            foreach (RPSButton_UI choiceButton in _choiceButtons)
            {
                choiceButton.Init(_rpsConfig.GetSpriteByType(choiceButton.ElementType));
                choiceButton.ElementSelected += OnElementSelected;
            }
        }

        public void ShowWaitingForOpponent(string localPlayerName)
        {
            _localPlayerName = GetDisplayedName(localPlayerName);
            _opponentPlayerName = null;

            gameObject.SetActive(true);
            _localPlayerText.text = $"YOU\n{_localPlayerName}\nScore: 0";
            _opponentPlayerText.text = "OPPONENT\nWaiting for opponent...";
            HideRoundControls();
        }

        public void ShowPreparingPlayers(string localPlayerName, string opponentPlayerName)
        {
            _localPlayerName = GetDisplayedName(localPlayerName);
            _opponentPlayerName = GetDisplayedName(opponentPlayerName);

            gameObject.SetActive(true);
            _localPlayerText.text = $"YOU\n{_localPlayerName}\nScore: 0";
            _opponentPlayerText.text = $"OPPONENT\n{_opponentPlayerName}\nPreparing...";
            HideRoundControls();
        }

        public void Show(string localPlayerName, string opponentPlayerName)
        {
            _localPlayerName = GetDisplayedName(localPlayerName);
            _opponentPlayerName = GetDisplayedName(opponentPlayerName);

            gameObject.SetActive(true);
            _localPlayerText.text = $"YOU\n{_localPlayerName}\nScore: 0";
            _opponentPlayerText.text = $"OPPONENT\n{_opponentPlayerName}\nScore: 0";

            PrepareNextRound();
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        public void ShowLocalChoice(RPSElementType elementType)
        {
            ShowElement(_localChoiceImage, elementType);
            SetChoiceButtonsActive(false);
        }

        public void ShowRound(RPSElementType localElement, RPSElementType opponentElement, RpsRoundResult result)
        {
            ShowElement(_localChoiceImage, localElement);
            ShowElement(_opponentChoiceImage, opponentElement);

            _roundResultText.text = result.ToString();
            _roundResultText.gameObject.SetActive(true);
        }

        public void UpdateScores(int localScore, int opponentScore)
        {
            _localPlayerText.text = $"YOU\n{_localPlayerName}\nScore: {localScore}";
            _opponentPlayerText.text = $"OPPONENT\n{_opponentPlayerName}\nScore: {opponentScore}";
        }

        public void PrepareNextRound()
        {
            _localChoiceImage.gameObject.SetActive(false);
            _opponentChoiceImage.gameObject.SetActive(false);
            _roundResultText.gameObject.SetActive(false);
            SetChoiceButtonsActive(true);
        }

        private void OnElementSelected(RPSElementType elementType)
        {
            ChoiceSelected?.Invoke(elementType);
        }

        private void ShowElement(Image elementImage, RPSElementType elementType)
        {
            elementImage.sprite = _rpsConfig.GetSpriteByType(elementType);
            elementImage.gameObject.SetActive(true);
        }

        private void SetChoiceButtonsActive(bool isActive)
        {
            foreach (RPSButton_UI choiceButton in _choiceButtons)
                choiceButton.gameObject.SetActive(isActive);
        }

        private void HideRoundControls()
        {
            _localChoiceImage.gameObject.SetActive(false);
            _opponentChoiceImage.gameObject.SetActive(false);
            _roundResultText.gameObject.SetActive(false);
            SetChoiceButtonsActive(false);
        }

        private string GetDisplayedName(string playerName)
        {
            return string.IsNullOrWhiteSpace(playerName) ? "Player" : playerName;
        }

        private void OnDestroy()
        {
            foreach (RPSButton_UI choiceButton in _choiceButtons)
                choiceButton.ElementSelected -= OnElementSelected;
        }
    }
}
