using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MyProject
{
    public class PlayerInfo_UI : MonoBehaviour
    {
        [SerializeField] private GameObject _infoParent;
        [SerializeField] private GameObject _waitingTip;
        [SerializeField] private TextMeshProUGUI _nicknameText;
        [SerializeField] private TextMeshProUGUI _scoreText;
        [SerializeField] private GameObject _selectedElementParent;
        [SerializeField] private Image _selectedElementImage;

        private void Awake()
        {
            HideSelectedElement();
        }

        public void SetWaiting()
        {
            _infoParent.SetActive(false);
            _waitingTip.SetActive(true);
        }

        public void SetNickname(string nickname)
        {
            _nicknameText.text = nickname;
            _infoParent.gameObject.SetActive(true);
            _waitingTip.SetActive(false);
        }

        public void UpdateScoreText(int value)
        {
            _scoreText.text = value.ToString();
        }

        public void SetSelectedElement(Sprite view)
        {
            _selectedElementParent.SetActive(true);
            _selectedElementImage.sprite = view;
        }

        public void HideSelectedElement()
        {
            _selectedElementParent.SetActive(false);
        }
    }
}
