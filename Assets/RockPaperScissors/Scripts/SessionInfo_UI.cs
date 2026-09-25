using Fusion;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using WebGLCopyAndPaste;

namespace MyProject
{
    public class SessionInfo_UI : MonoBehaviour
    {
        [SerializeField] private Image _sessionIndicator;
        [SerializeField] private Color _sessionActiveColor = Color.green;
        [SerializeField] private Color _sessionInactiveColor = Color.red;
        [SerializeField] private TextMeshProUGUI _roomCodeText;
        [SerializeField] private Button_UI _copyButton;
        [SerializeField] private TextMeshProUGUI _regionText;
        [SerializeField] private TextMeshProUGUI _pingText;
        [SerializeField] private int _frequencyPingUpdate;

        private NetworkRunner _networkRunner;
        private bool _isSessionActive;
        private int _currentIndex;

        private void Awake()
        {
            _copyButton.OnPressed += CopyRoomCodeToClipboard;
            SetSessionStatus(false);
        }

        public void Init(NetworkRunner networkRunner)
        {
            _networkRunner = networkRunner;
            _roomCodeText.text = networkRunner.SessionInfo.Name;
            _regionText.text = networkRunner.SessionInfo.Region.ToUpper();
            SetSessionStatus(true);
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
                //_sessionIndicator.color = _sessionActiveColor;
                _roomCodeText.enabled = true;
                _copyButton.SetInteractableStatus(true);

            }
            else
            {
                _roomCodeText.text = string.Empty;
                _roomCodeText.enabled = false;
                //_sessionIndicator.color = _sessionInactiveColor;
                _regionText.text = string.Empty;
                _pingText.text = string.Empty;
                _copyButton.SetInteractableStatus(false);
            }
        }

        public void UpdatePing()
        {
            if (_currentIndex >= _frequencyPingUpdate)
            {
                _pingText.text = $"{(int)(_networkRunner.GetPlayerRtt(_networkRunner.LocalPlayer) * 1000)} ms";
                _currentIndex = 0;
            }
            else
                _currentIndex++;
        }

        private void CopyRoomCodeToClipboard()
        {
            //GUIUtility.systemCopyBuffer = _roomCodeText.text;
            WebGLCopyAndPasteAPI.CopyToClipboard(_roomCodeText.text);
        }
    }
}
