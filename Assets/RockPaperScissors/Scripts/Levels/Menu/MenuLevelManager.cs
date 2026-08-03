using UnityEngine;

namespace MyProject
{
    public class MenuLevelManager : LevelManager
    {
        [SerializeField] private MenuLevel_UI_Manager _menuLevel_UI_Manager;

        private MenuNetworkController _networkController;

        public override void Init(AppServices appServices)
        {
            _menuLevel_UI_Manager.Init(appServices.AudioManager);

            _networkController = new MenuNetworkController(_menuLevel_UI_Manager, appServices.NetworkService);
            _networkController.Init();
        }

        public override void StartLevel()
        {
            
        }

        private void OnDestroy()
        {
            _networkController?.Dispose();
        }
    }
}
