using TMPro;
using UnityEngine;

namespace MyProject
{
    public class FPSCounter_UI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _fpsText;

        private int _frameCount;
        private float _elapsedTime;

        private void Awake()
        {
            _fpsText.text = "***";
        }

        private void Update()
        {
            _frameCount++;
            _elapsedTime += Time.unscaledDeltaTime;

            if (_elapsedTime > 0.5f)
            {
                int fps = Mathf.RoundToInt(_frameCount / _elapsedTime);
                _fpsText.text = fps.ToString();

                _frameCount = 0;
                _elapsedTime = 0f;
            }
        }
    }
}
