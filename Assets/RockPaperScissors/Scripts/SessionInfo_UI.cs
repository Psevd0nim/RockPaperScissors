using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MyProject
{
    public class SessionInfo_UI : MonoBehaviour
    {
        [SerializeField] private Image _sessionIndicator;
        [SerializeField] private Color _sessionActiveColor = Color.green;
        [SerializeField] private Color _sessionInactiveColor = Color.red;
        [SerializeField] private TextMeshProUGUI _roomCodeText;
        [SerializeField] private Button_UI _copyButton;

        private void Awake()
        {
            _copyButton.OnPressed += CopyRoomCodeToClipboard;
            SetSessionStatus(false);
        }

        public void SetSessionStatus(bool isActive)
        {
            if (isActive)
            {
                _sessionIndicator.color = _sessionActiveColor;
                _roomCodeText.enabled = true;
                _copyButton.SetInteractableStatus(true);

            }
            else
            {
                _roomCodeText.text = string.Empty;
                _sessionIndicator.color = _sessionInactiveColor;
                _roomCodeText.enabled = false;
                _copyButton.SetInteractableStatus(false);
            }
        }

        public void SetRoomCode(string roomCode)
        {
            _roomCodeText.text = roomCode;
        }

        private void CopyRoomCodeToClipboard()
        {
            GUIUtility.systemCopyBuffer = _roomCodeText.text;
        }
    }
}
