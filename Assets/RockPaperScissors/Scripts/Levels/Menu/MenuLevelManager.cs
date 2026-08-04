using Fusion;
using UnityEngine;

namespace MyProject
{
    public class MenuLevelManager : LevelManager
    {
        [SerializeField] private MenuLevel_UI_Manager _menuLevel_UI_Manager;

        private FusionNetworkService _fusionNetworkService;

        public override void Init(AppServices appServices)
        {
            _menuLevel_UI_Manager.Init(appServices.AudioManager);
            _menuLevel_UI_Manager.OnPlayPressed += AfterPlayPressed;

            _fusionNetworkService = new();
        }

        public override void StartLevel()
        {
            
        }

        private async void AfterPlayPressed()
        {
            StartGameResult startGameResult = await _fusionNetworkService.StartGameAsync();

            Debug.Log($"StartGameResult.Ok: {startGameResult.Ok}");
            Debug.Log($"StartGameResult.ShutdownReason: {startGameResult.ShutdownReason}");
            Debug.Log($"StartGameResult.ErrorMessage: \"{startGameResult.ErrorMessage}\"");
        }
    }
}
