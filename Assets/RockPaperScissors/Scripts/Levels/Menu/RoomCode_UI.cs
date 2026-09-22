using TMPro;
using UnityEngine;

namespace MyProject
{
    public class RoomCode_UI : MonoBehaviour
    {
        [SerializeField] private TMP_InputField _inputField;

        private void Awake()
        {
            _inputField.onEndEdit.AddListener(SaveInputedValue);
        }

        private void SaveInputedValue(string value)
        {
            PlayerPrefs.SetString(Constants.RoomCodeKey, value);
        }
    }
}
