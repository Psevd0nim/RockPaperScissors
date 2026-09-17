using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MyProject
{
    public class PlayerName : MonoBehaviour
    {
        public TMP_InputField inputField;
        public GameObject editIcon;

        private void Awake()
        {
            inputField.text = PlayerPrefs.GetString("PlayerName", "Player123");
            inputField.onValueChanged.AddListener(SaveChangedValue);
            inputField.onSelect.AddListener((value) => editIcon.SetActive(true));
            inputField.onDeselect.AddListener((value) => editIcon.SetActive(false));
            editIcon.SetActive(false);
        }

        private void SaveChangedValue(string value)
        {
            PlayerPrefs.SetString("PlayerName", value);
        }
    }
}
