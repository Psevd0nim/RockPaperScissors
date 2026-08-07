using System.Collections;
using TMPro;
using UnityEngine;

namespace MyProject
{
    public class ConnectingIndicator : MonoBehaviour
    {
        [SerializeField] private TMP_Text _connectingText;
        [SerializeField] private float _dotChangeDelay = 0.4f;

        private Coroutine _animationCoroutine;

        public void Show()
        {
            gameObject.SetActive(true);
            _animationCoroutine = StartCoroutine(AnimateText());
        }

        public void Hide()
        {
            if (_animationCoroutine != null)
            {
                StopCoroutine(_animationCoroutine);
                _animationCoroutine = null;
            }

            gameObject.SetActive(false);
        }

        private IEnumerator AnimateText()
        {
            int dotsCount = 0;

            while (true)
            {
                _connectingText.text = "Connecting" + new string('.', dotsCount);
                dotsCount++;

                if (dotsCount > 3)
                    dotsCount = 0;

                yield return new WaitForSeconds(_dotChangeDelay);
            }
        }
    }
}
