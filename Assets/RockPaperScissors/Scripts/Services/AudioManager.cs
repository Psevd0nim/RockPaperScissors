using UnityEngine;

namespace MyProject
{
    public class AudioManager : MonoBehaviour, IService
    {
        public bool AudioEnable { get; private set; }

        [SerializeField] private AudioSource _audioSource;
        [SerializeField] private AudioSource _backgroundAudioSource;
        [SerializeField] private SoundConfig _soundConfig;

        private void Start()
        {
            AudioEnable = PlayerPrefs.GetInt(Constants.SoundVolumeKey, 1) == 1;
            SetAudioStatus(AudioEnable);
        }

        public void PlaySomeSound(SoundType type)
        {
            SoundData soundData = _soundConfig.GetSoundDataByType(type);
            _audioSource.PlayOneShot(soundData.sound, soundData.volume);
        }

        public void SetAudioStatus(bool status)
        {
            AudioEnable = status;
            AudioListener.volume = AudioEnable ? 1 : 0;
        }

        public void SetBackgroundStatus(bool status)
        {
            _backgroundAudioSource.enabled = status;
        }
    }
}