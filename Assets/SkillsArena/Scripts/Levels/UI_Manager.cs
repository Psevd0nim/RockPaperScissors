using UnityEngine;

namespace MyProject
{
    public abstract class UI_Manager : MonoBehaviour
    {
        [SerializeField] private SoundButton _soundButton;
        private protected AudioManager _audioManager;

        public virtual void Init(AudioManager audioManager)
        {
            _audioManager = audioManager;
            _soundButton.OnPressed += AfterSoundButtonPressed;
            _soundButton.SetActive(_audioManager.AudioActive);
        }

        private void AfterSoundButtonPressed()
        {
            _audioManager.SetAudioStatus(!_audioManager.AudioActive);
            _soundButton.SetActive(_audioManager.AudioActive);
            _audioManager.PlaySomeSound(SoundType.ClickButton);
        }
    }
} 