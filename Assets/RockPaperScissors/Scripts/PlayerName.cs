using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MyProject
{
    public class PlayerName : MonoBehaviour
    {
        public event Action Test;

        public TMP_InputField inputField;
        public Image view;

        private void Awake()
        {
            inputField.onValueChanged.AddListener(SaveChangedValue);
            inputField.text = PlayerPrefs.GetString("PlayerName", "Player123");
        }

        private void SaveChangedValue(string value)
        {
            PlayerPrefs.SetString("PlayerName", value);
        }
    }
}
