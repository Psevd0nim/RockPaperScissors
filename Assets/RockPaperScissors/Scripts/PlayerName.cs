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

        [SerializeField] private GameObject _placeholder;

        private void Awake()
        {
            inputField.text = PlayerPrefs.GetString("PlayerName", "Player123");
            inputField.onSelect.AddListener((value) => editIcon.SetActive(true));
            inputField.onDeselect.AddListener((value) => editIcon.SetActive(false));
            inputField.onEndEdit.AddListener(SaveChangedValue);
            editIcon.SetActive(false);

            inputField.onSelect.AddListener(_ => _placeholder.SetActive(false));
            inputField.onDeselect.AddListener(_ =>
            {
                if (string.IsNullOrEmpty(inputField.text))
                {
                    _placeholder.SetActive(true);
                }
            });
        }

        private void SaveChangedValue(string value)
        {
            PlayerPrefs.SetString("PlayerName", value);
            editIcon.SetActive(false);
        }
    }
}
