using UnityEngine;

namespace MyProject
{
    public class GameLevelManager : LevelManager
    {
        [SerializeField] private Game_UI_Manager _gameUIManager;
        [SerializeField] private NetworkGameManager _networkGameManager;
        [SerializeField] private RpsRoundManager _rpsRoundManager;

        public override void Init(AppServices appServices)
        {
            _gameUIManager.Init(appServices.AudioManager);
            _rpsRoundManager.Init(_gameUIManager);
            _networkGameManager.Init(appServices.NetworkService);
            _networkGameManager.StateChanged += OnNetworkGameStateChanged;
        }

        public override void StartLevel()
        {
            _gameUIManager.OpenTransition();
            _networkGameManager.StartNetworkSession();
        }

        private void OnNetworkGameStateChanged(NetworkGameState state)
        {
            _gameUIManager.ShowPlayersCount(_networkGameManager.PlayersCount);

            switch (state)
            {
                case NetworkGameState.Connecting:
                    _gameUIManager.ShowConnectingIndicator();
                    break;
                case NetworkGameState.WaitingForOpponent:
                    _gameUIManager.HideConnectingIndicator();

                    if (_rpsRoundManager.IsMatchActive)
                        _rpsRoundManager.EndMatch();

                    _gameUIManager.ShowLocalPlayer(_networkGameManager.LocalPlayerEntity?.Nickname);
                    _gameUIManager.ShowWaitingForOpponent();
                    break;
                case NetworkGameState.ReadyToPlay:
                    _gameUIManager.HideConnectingIndicator();
                    _rpsRoundManager.StartMatch(
                        _networkGameManager.LocalPlayerEntity,
                        _networkGameManager.OpponentPlayerEntity);
                    break;
                case NetworkGameState.ConnectionFailed:
                    _gameUIManager.HideConnectingIndicator();
                    _gameUIManager.ShowConnectionFailed();
                    break;
            }
        }

        private void OnDestroy()
        {
            if (_networkGameManager != null)
                _networkGameManager.StateChanged -= OnNetworkGameStateChanged;
        }
    }
}
