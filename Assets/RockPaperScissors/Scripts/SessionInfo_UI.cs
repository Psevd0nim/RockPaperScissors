using Fusion;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Unity.Collections.Unicode;

namespace MyProject
{
    public class SessionInfo_UI : MonoBehaviour
    {
        [SerializeField] private Image _sessionIndicator;
        [SerializeField] private Color _sessionActiveColor = Color.green;
        [SerializeField] private Color _sessionInactiveColor = Color.red;
        [SerializeField] private TextMeshProUGUI _roomCodeText;
        [SerializeField] private Button_UI _copyButton;
        [SerializeField] private TextMeshProUGUI _pingText;

        private NetworkRunner _networkRunner;
        private bool _isSessionActive;

        private void Awake()
        {
            _copyButton.OnPressed += CopyRoomCodeToClipboard;
            SetSessionStatus(false);
        }

        public void Init(NetworkRunner networkRunner)
        {
            _networkRunner = networkRunner;
        }

        private void Update()
        {
            if(_networkRunner != null && _isSessionActive)
            {
                UpdatePing();
            }
        }

        public void SetSessionStatus(bool isActive)
        {
            _isSessionActive = isActive;
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

        public void UpdatePing()
        {
            _pingText.text = $"{(int)(_networkRunner.GetPlayerRtt(_networkRunner.LocalPlayer) * 1000)}";
        }

        private void CopyRoomCodeToClipboard()
        {
            GUIUtility.systemCopyBuffer = _roomCodeText.text;
        }
    }
}
