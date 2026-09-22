using Fusion.Menu;
using UnityEngine;

namespace MyProject
{
    public class MenuLevelManager : LevelManager
    {
        [SerializeField] private MenuLevel_UI_Manager _menuLevel_UI_Manager;
        [SerializeField] private FusionMenuPartyCodeGenerator _menuPartyCodeGenerator;

        public override void Init(AppServices appServices)
        {
            _menuLevel_UI_Manager.Init(appServices.AudioManager);
            _menuLevel_UI_Manager.OnPlayPressed += AfterPlayPressed;
            _menuLevel_UI_Manager.OnVsBotPressed += AfterVsBotPressed;
        }

        public override void StartLevel()
        {
            _menuLevel_UI_Manager.OpenTransition();
        }

        private void AfterPlayPressed()
        {
            string roomCode = PlayerPrefs.GetString(Constants.RoomCodeKey, string.Empty);
            if (roomCode == string.Empty)
                PlayerPrefs.SetString(Constants.RoomCodeKey, _menuPartyCodeGenerator.Create());

            _menuLevel_UI_Manager.DisablePlayButton();
            _menuLevel_UI_Manager.CloseTransition();
            OnExitLevel?.Invoke(this, Constants.GameSceneName, 1.2f);
        }

        private void AfterVsBotPressed()
        {
            PlayerPrefs.SetInt(Constants.OfflineModeKey, 1);
            _menuLevel_UI_Manager.CloseTransition();
            OnExitLevel?.Invoke(this, Constants.GameSceneName, 1.2f);
        }

        private void OnDestroy()
        {
            _menuLevel_UI_Manager.OnPlayPressed -= AfterPlayPressed;
            _menuLevel_UI_Manager.OnVsBotPressed -= AfterVsBotPressed;
        }
    }
}
