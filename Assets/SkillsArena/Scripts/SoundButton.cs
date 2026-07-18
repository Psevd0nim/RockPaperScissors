using UnityEngine;
using UnityEngine.UI;

namespace MyProject
{
    public class SoundButton : Button_UI
    {
        [SerializeField] private Image _mainView;
        [SerializeField] private Sprite _enableSprite, _disableSprite;
        [SerializeField] private Color _enableColor, _disableColor;
        

        public void SetActive(bool active)
        {
            _mainView.sprite = active? _enableSprite : _disableSprite;
            _mainView.color = active? _enableColor : _disableColor;
        }
    }
}
