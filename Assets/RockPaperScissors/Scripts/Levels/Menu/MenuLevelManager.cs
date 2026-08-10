using UnityEngine;

namespace MyProject
{
    public class MenuLevelManager : LevelManager
    {
        [SerializeField] private MenuLevel_UI_Manager _menuLevel_UI_Manager;

        public override void Init(AppServices appServices)
        {
            _menuLevel_UI_Manager.Init(appServices.AudioManager);
            _menuLevel_UI_Manager.OnPlayPressed += AfterPlayPressed;
        }

        public override void StartLevel()
        {
            _menuLevel_UI_Manager.OpenTransition();
        }

        private void AfterPlayPressed()
        {
            _menuLevel_UI_Manager.DisablePlayButton();
            _menuLevel_UI_Manager.CloseTransition();
            OnExitLevel?.Invoke(this, Constants.GameSceneName, 1.2f);
        }
    }
}
