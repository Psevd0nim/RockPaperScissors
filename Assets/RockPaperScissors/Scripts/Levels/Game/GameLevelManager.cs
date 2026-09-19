using System;
using UnityEngine;

namespace MyProject
{
    public class GameLevelManager : LevelManager
    {
        [SerializeField] private Game_UI_Manager _gameUIManager;
        [SerializeField] private NetworkGameManager _networkGameManager;
        [SerializeField] private RpsMatchManager _rpsMatchManager;
        [SerializeField] private Factory _factory;
        [SerializeField] private bool _enableOfflineMode;

        private FusionNetworkService _networkService;
        private bool _isExiting;
        private bool _isOfflineMode;

        public override void Init(AppServices appServices)
        {
            _gameUIManager.Init(appServices.AudioManager);
            _rpsMatchManager.Init(_gameUIManager);
            _networkService = appServices.NetworkService;
            _networkGameManager.Init(_networkService, _gameUIManager, _rpsMatchManager);

            _gameUIManager.MenuPressed += ExitToMenu;

            if (_enableOfflineMode)
                _isOfflineMode = true;
            else
                _isOfflineMode = PlayerPrefs.GetInt(Constants.OfflineModeKey, 0) == 1;
        }

        public override void StartLevel()
        {
            _gameUIManager.OpenTransition();
            if(_isOfflineMode)
            {
                NetworkPlayerEntity localPlayerEntity = _factory.CreateNetworkPlayerEntity();
                localPlayerEntity.SetNickname(PlayerPrefs.GetString("PlayerName", "Player123"));

                NetworkPlayerEntity opponentPlayerEntity = _factory.CreateNetworkPlayerEntity();
                opponentPlayerEntity.SetNickname("Bot");

                _rpsMatchManager.StartMatch(localPlayerEntity, opponentPlayerEntity, isOfflineMode: true);
            }
            else
                _networkGameManager.StartNetworkSession();
        }

        private async void ExitToMenu()
        {
            if (_isExiting)
                return;

            _isExiting = true;
            _gameUIManager.CloseTransition();

            await _networkGameManager.ShutdownNetworkSession();

            OnExitLevel?.Invoke(this, Constants.MenuSceneName, 1.2f);
        }

        private void OnDestroy()
        {
            _gameUIManager.MenuPressed -= ExitToMenu;
        }
    }
}
