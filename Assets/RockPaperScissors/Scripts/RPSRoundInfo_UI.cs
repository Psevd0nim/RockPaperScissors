using System;
using System.Collections.Generic;
using System.ComponentModel;
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
        [SerializeField] private PlayerInfo_UI _localPlayerInfo;
        [SerializeField] private PlayerInfo_UI _opponentPlayerInfo;
        [SerializeField] private Image _localSelectedElementImage;
        [SerializeField] private Image _opponentSelectedElementImage;
        [SerializeField] private TextMeshProUGUI _roundResultText;
        [SerializeField] private GameObject _rpsButtonsParent;

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
            _localPlayerInfo.SetNickname(_localPlayerName);
        }

        public void ShowWaitingForOpponent()
        {
            _opponentPlayerName = null;

            gameObject.SetActive(true);
            _opponentPlayerInfo.SetWaiting();
            HideRoundControls();
        }

        public void Show(string localPlayerName, string opponentPlayerName)
        {
            _localPlayerName = GetDisplayedName(localPlayerName);
            _opponentPlayerName = GetDisplayedName(opponentPlayerName);

            gameObject.SetActive(true);
            _localPlayerInfo.SetNickname(_localPlayerName);
            _opponentPlayerInfo.SetNickname(_opponentPlayerName);

            ShowElementSelection();
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        public void ShowLocalSelectedElement(RPSElementType elementType)
        {
            _localPlayerInfo.SetSelectedElement(_rpsConfig.GetSpriteByType(elementType));
            SetElementButtonsActive(false);
        }

        public void ShowRound(RPSElementType localElement, RPSElementType opponentElement, RpsRoundResult result)
        {
            _localPlayerInfo.SetSelectedElement(_rpsConfig.GetSpriteByType(localElement));
            _opponentPlayerInfo.SetSelectedElement(_rpsConfig.GetSpriteByType(opponentElement));

            _roundResultText.text = result.ToString();
            _roundResultText.gameObject.SetActive(true);
        }

        public void UpdateScores(int localScore, int opponentScore)
        {
            _localPlayerInfo.UpdateScoreText(localScore);
            _opponentPlayerInfo.UpdateScoreText(opponentScore);
        }

        public void ShowElementSelection()
        {
            _localPlayerInfo.HideSelectedElement();
            _opponentPlayerInfo.HideSelectedElement();
            _roundResultText.gameObject.SetActive(false);
            SetElementButtonsActive(true);
        }

        private void OnElementSelected(RPSElementType elementType)
        {
            ElementSelected?.Invoke(elementType);
        }

        private void SetElementButtonsActive(bool isActive)
        {
            _rpsButtonsParent.gameObject.SetActive(isActive);
        }

        private void HideRoundControls()
        {
            _localPlayerInfo.HideSelectedElement();
            _opponentPlayerInfo.HideSelectedElement();
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
