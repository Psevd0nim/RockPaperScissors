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

        [SerializeField] private PlayerSpawner _playerSpawner;

        private FusionNetworkService _networkService;
        private Game_UI_Manager _gameUIManager;
        private RpsMatchManager _rpsMatchManager;

        public void Init(FusionNetworkService networkService, Game_UI_Manager gameUIManager, RpsMatchManager rpsRoundManager)
        {
            _networkService = networkService;
            _gameUIManager = gameUIManager;
            _rpsMatchManager = rpsRoundManager;
            _networkService.PlayersChanged += TryUpdateMatchState;
            _networkService.PlayerEntitiesChanged += TryUpdateMatchState;
        }

        public async void StartNetworkSession()
        {
            ChangeState(NetworkGameState.Connecting);

            StartGameResult startGameResult = await _networkService.StartGameSessionAsync();
            _networkService.Runner.AddGlobal(_playerSpawner);

            if (startGameResult.Ok == false)
            {
                Debug.Log($"StartGameResult.ShutdownReason: {startGameResult.ShutdownReason}");
                Debug.Log($"StartGameResult.ErrorMessage: \"{startGameResult.ErrorMessage}\"");

                ChangeState(NetworkGameState.ConnectionFailed);
            }
        }

        public async Task ShutdownNetworkSession()
        {
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
                    _rpsMatchManager.StartMatch(LocalPlayerEntity, OpponentPlayerEntity);
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
            {
                if (_networkService.TryGetNetworkPlayerEntity(_networkService.LocalPlayer, out NetworkPlayerEntity localPlayerEntity))
                {
                    LocalPlayerEntity = localPlayerEntity;
                }
                else
                {
                    return;
                }
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
        }
    }
}
