using System;
using UnityEngine;
using UnityEngine.UI;

namespace MyProject
{
    public class RPSButton_UI : Button_UI
    {
        public event Action<RPSElementType> ElementSelected;

        public RPSElementType ElementType => _elementType;

        [SerializeField] private RPSElementType _elementType;
        [SerializeField] private Image _view;

        public void Init(Sprite sprite)
        {
            Debug.Assert(_elementType != RPSElementType.None, $"RPS element is not selected for {name}.", this);
            _view.sprite = sprite;
        }

        public override void Pressed()
        {
            base.Pressed();
            ElementSelected?.Invoke(_elementType);
        }
    }
}
