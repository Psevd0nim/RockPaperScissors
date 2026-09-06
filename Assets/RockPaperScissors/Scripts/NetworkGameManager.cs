using System;
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

    public class NetworkGameManager : NetworkBehaviour
    {
        public event Action<NetworkGameState> StateChanged;

        public NetworkGameState State { get; private set; } = NetworkGameState.NotStarted;
        public NetworkPlayerEntity LocalPlayerEntity { get; private set; }
        public NetworkPlayerEntity OpponentPlayerEntity { get; private set; }
        public int PlayersCount => _networkService.Players.Count;

        [SerializeField] private NetworkPlayerEntity _playerEntityPrefab;

        private FusionNetworkService _networkService;

        public void Init(FusionNetworkService networkService)
        {
            _networkService = networkService;
            _networkService.PlayersChanged += TryUpdateMatchState;
            _networkService.PlayerEntitiesChanged += TryUpdateMatchState;
        }

        public async void StartNetworkSession()
        {
            ChangeState(NetworkGameState.Connecting);

            StartGameResult startGameResult = await _networkService.StartGameSessionAsync();

            if (startGameResult.Ok == false)
            {
                Debug.Log($"StartGameResult.ShutdownReason: {startGameResult.ShutdownReason}");
                Debug.Log($"StartGameResult.ErrorMessage: \"{startGameResult.ErrorMessage}\"");

                ChangeState(NetworkGameState.ConnectionFailed);
            }
        }

        public override void Spawned()
        {
            NetworkPlayerEntity localPlayerEntity = Runner.Spawn(_playerEntityPrefab);
            Runner.SetPlayerObject(Runner.LocalPlayer, localPlayerEntity.Object);

            LocalPlayerEntity = localPlayerEntity;

            TryUpdateMatchState();
        }

        private void TryUpdateMatchState()
        {
            if (LocalPlayerEntity == null)
                return;

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
            StateChanged?.Invoke(State);
        }

        private void OnDestroy()
        {
            _networkService.PlayersChanged -= TryUpdateMatchState;
            _networkService.PlayerEntitiesChanged -= TryUpdateMatchState;
        }
    }
}
