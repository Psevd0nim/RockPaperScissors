using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MyProject
{
    public class RPSRoundInfo_UI : MonoBehaviour
    {
        public event Action<RPSElementType> ElementSelected;

        [SerializeField] private RPSConfig _rpsConfig;
        [SerializeField] private List<RPSButton_UI> _elementButtons;
        [SerializeField] private TextMeshProUGUI _localPlayerText;
        [SerializeField] private TextMeshProUGUI _opponentPlayerText;
        [SerializeField] private Image _localSelectedElementImage;
        [SerializeField] private Image _opponentSelectedElementImage;
        [SerializeField] private TextMeshProUGUI _roundResultText;

        private string _localPlayerName;
        private string _opponentPlayerName;

        public void Init()
        {
            foreach (RPSButton_UI elementButton in _elementButtons)
            {
                elementButton.Init(_rpsConfig.GetSpriteByType(elementButton.ElementType));
                elementButton.ElementSelected += OnElementSelected;
            }
        }

        public void ShowLocalPlayer(string localPlayerName)
        {
            _localPlayerName = GetDisplayedName(localPlayerName);

            gameObject.SetActive(true);
            _localPlayerText.text = $"YOU\n{_localPlayerName}\nScore: 0";
        }

        public void ShowWaitingForOpponent()
        {
            _opponentPlayerName = null;

            gameObject.SetActive(true);
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

            ShowElementSelection();
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        public void ShowLocalSelectedElement(RPSElementType elementType)
        {
            ShowElement(_localSelectedElementImage, elementType);
            SetElementButtonsActive(false);
        }

        public void ShowRound(RPSElementType localElement, RPSElementType opponentElement, RpsRoundResult result)
        {
            ShowElement(_localSelectedElementImage, localElement);
            ShowElement(_opponentSelectedElementImage, opponentElement);

            _roundResultText.text = result.ToString();
            _roundResultText.gameObject.SetActive(true);
        }

        public void UpdateScores(int localScore, int opponentScore)
        {
            _localPlayerText.text = $"YOU\n{_localPlayerName}\nScore: {localScore}";
            _opponentPlayerText.text = $"OPPONENT\n{_opponentPlayerName}\nScore: {opponentScore}";
        }

        public void ShowElementSelection()
        {
            _localSelectedElementImage.gameObject.SetActive(false);
            _opponentSelectedElementImage.gameObject.SetActive(false);
            _roundResultText.gameObject.SetActive(false);
            SetElementButtonsActive(true);
        }

        private void OnElementSelected(RPSElementType elementType)
        {
            ElementSelected?.Invoke(elementType);
        }

        private void ShowElement(Image elementImage, RPSElementType elementType)
        {
            elementImage.sprite = _rpsConfig.GetSpriteByType(elementType);
            elementImage.gameObject.SetActive(true);
        }

        private void SetElementButtonsActive(bool isActive)
        {
            foreach (RPSButton_UI elementButton in _elementButtons)
                elementButton.gameObject.SetActive(isActive);
        }

        private void HideRoundControls()
        {
            _localSelectedElementImage.gameObject.SetActive(false);
            _opponentSelectedElementImage.gameObject.SetActive(false);
            _roundResultText.gameObject.SetActive(false);
            SetElementButtonsActive(false);
        }

        private string GetDisplayedName(string playerName)
        {
            return string.IsNullOrWhiteSpace(playerName) ? "Player" : playerName;
        }

        private void OnDestroy()
        {
            foreach (RPSButton_UI elementButton in _elementButtons)
                elementButton.ElementSelected -= OnElementSelected;
        }
    }
}
