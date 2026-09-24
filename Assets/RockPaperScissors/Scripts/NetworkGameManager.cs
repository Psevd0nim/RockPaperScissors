using System;
using System.Threading.Tasks;
using Fusion;
using UnityEngine;

namespace MyProject
{
    public enum NetworkGameState
    {
        NotStarted,
        Connecting,
        WaitingForOpponent,
        ReadyToPlay,
        ConnectionFailed
    }

    public class NetworkGameManager : MonoBehaviour
    {
        public NetworkGameState State { get; private set; } = NetworkGameState.NotStarted;
        public NetworkPlayerEntity LocalPlayerEntity { get; private set; }
        public NetworkPlayerEntity OpponentPlayerEntity { get; private set; }
        public int PlayersCount => _networkService.Players.Count;

        private FusionNetworkService _networkService;
        private Game_UI_Manager _gameUIManager;
        private RpsMatchManager _rpsMatchManager;
        private GameMode _gameMode;
        private Factory _factory;

        public void Init(FusionNetworkService networkService, Game_UI_Manager gameUIManager, RpsMatchManager rpsRoundManager, Factory factory)
        {
            _networkService = networkService;
            _gameUIManager = gameUIManager;
            _rpsMatchManager = rpsRoundManager;
            _factory = factory;
            _networkService.PlayersChanged += TryUpdateMatchState;
            _networkService.PlayerEntitiesChanged += TryUpdateMatchState;
        }

        public async void StartNetworkSession(GameMode gameMode)
        {
            _gameMode = gameMode;

            ChangeState(NetworkGameState.Connecting);

            string roomCode = PlayerPrefs.GetString(Constants.RoomCodeKey, string.Empty);
            if (roomCode == string.Empty)
                roomCode = "123";

            StartGameResult startGameResult = await _networkService.StartGameSessionAsync(gameMode, roomCode);

            if (startGameResult.Ok == false)
            {
                Debug.Log($"StartGameResult.ShutdownReason: {startGameResult.ShutdownReason}");
                Debug.Log($"StartGameResult.ErrorMessage: \"{startGameResult.ErrorMessage}\"");

                ChangeState(NetworkGameState.ConnectionFailed);
                return;
            }

            if(gameMode != GameMode.Single)
                _gameUIManager.SessionInfoUI.Init(_networkService.Runner);
            _networkService.OnSceneLoaded += TrySpawnPlayers;
        }

        private void TrySpawnPlayers()
        {
            NetworkRunner runner = _networkService.Runner;

            LocalPlayerEntity = _factory.SpawnLocalPlayer(runner);

            if (_gameMode == GameMode.Single)
            {
                OpponentPlayerEntity = _factory.SpawnBot(runner);
            }

            TryUpdateMatchState();
        }

        public async Task ShutdownNetworkSession()
        {
            _gameUIManager.SessionInfoUI.SetSessionStatus(false);
            try
            {
                await _networkService.ShutdownGameSessionAsync();
            }
            catch (Exception exception)
            {
                Debug.LogException(exception);
            }
        }

        private void NetworkGameStateChanged(NetworkGameState state)
        {
            if (_gameMode == GameMode.Single)
                _gameUIManager.HidePlayersCount();
            else
                _gameUIManager.ShowPlayersCount(PlayersCount);

            switch (state)
            {
                case NetworkGameState.Connecting:
                    _gameUIManager.ShowConnectingIndicator();
                    break;
                case NetworkGameState.WaitingForOpponent:
                    _gameUIManager.HideConnectingIndicator();

                    if (_rpsMatchManager.IsMatchActive)
                        _rpsMatchManager.EndMatch();

                    _gameUIManager.ShowLocalPlayer(LocalPlayerEntity.Nickname);
                    _gameUIManager.ShowWaitingForOpponent();
                    break;
                case NetworkGameState.ReadyToPlay:
                    _gameUIManager.HideConnectingIndicator();
                    _rpsMatchManager.StartMatch(LocalPlayerEntity, OpponentPlayerEntity, isOfflineMode: _gameMode == GameMode.Single);
                    break;
                case NetworkGameState.ConnectionFailed:
                    _gameUIManager.HideConnectingIndicator();
                    _gameUIManager.ShowConnectionFailed();
                    break;
            }
        }

        private void TryUpdateMatchState()
        {
            if (LocalPlayerEntity == null)
                return;

            if (_gameMode == GameMode.Single)
            {
                if (OpponentPlayerEntity == null)
                    return;

                ChangeState(NetworkGameState.ReadyToPlay);
                return;
            }

            if (TryFindOpponent(out PlayerRef opponentPlayer) == false)
            {
                OpponentPlayerEntity = null;
                ChangeState(NetworkGameState.WaitingForOpponent);
                return;
            }

            if (_networkService.TryGetNetworkPlayerEntity(opponentPlayer, out NetworkPlayerEntity opponentPlayerEntity))
            {
                OpponentPlayerEntity = opponentPlayerEntity;
                ChangeState(NetworkGameState.ReadyToPlay);
            }
        }

        private bool TryFindOpponent(out PlayerRef opponentPlayer)
        {
            foreach (PlayerRef player in _networkService.Players)
            {
                if (player != _networkService.LocalPlayer)
                {
                    opponentPlayer = player;
                    return true;
                }
            }

            opponentPlayer = PlayerRef.None;
            return false;
        }

        private void ChangeState(NetworkGameState state)
        {
            if (State == state)
                return;

            State = state;
            NetworkGameStateChanged(State);
        }

        private void OnDestroy()
        {
            _networkService.PlayersChanged -= TryUpdateMatchState;
            _networkService.PlayerEntitiesChanged -= TryUpdateMatchState;
            _networkService.OnSceneLoaded -= TrySpawnPlayers;
        }
    }
}
