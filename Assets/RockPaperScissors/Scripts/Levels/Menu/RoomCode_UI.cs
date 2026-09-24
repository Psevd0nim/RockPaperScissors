using TMPro;
using UnityEngine;

namespace MyProject
{
    public class RoomCode_UI : MonoBehaviour
    {
        public string RoomCode { get; private set; }

        [SerializeField] private TMP_InputField _inputField;
        [SerializeField] private GameObject _placeholder;

        private void Awake()
        {
            _inputField.onEndEdit.AddListener(SaveInputedValue);
            _inputField.onSelect.AddListener(_ => _placeholder.SetActive(false));
            _inputField.onDeselect.AddListener(_ =>
            {
                if(string.IsNullOrEmpty(_inputField.text))
                {
                    _placeholder.SetActive(true);
                }
            });
        }

        private void SaveInputedValue(string value)
        {
            RoomCode = value;
        }
    }
}
