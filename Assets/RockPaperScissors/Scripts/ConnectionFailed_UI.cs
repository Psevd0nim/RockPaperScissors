using UnityEngine;

namespace MyProject
{
    public class ConnectionFailed_UI : MonoBehaviour
    {
        public Button_UI RetryButton => _retryButton;
        public Button_UI MenuButton => _menuButton;

        [SerializeField] private Button_UI _menuButton;
        [SerializeField] private Button_UI _retryButton;
    }
}
