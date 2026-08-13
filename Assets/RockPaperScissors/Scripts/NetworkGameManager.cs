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
        public int PlayersCount => _networkService?.Players.Count ?? 0;

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

            Debug.Log($"StartGameResult.Ok: {startGameResult.Ok}");
            Debug.Log($"StartGameResult.ShutdownReason: {startGameResult.ShutdownReason}");
            Debug.Log($"StartGameResult.ErrorMessage: \"{startGameResult.ErrorMessage}\"");

            if (startGameResult.Ok == false)
            {
                ChangeState(NetworkGameState.ConnectionFailed);
                return;
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

            if (_networkService.Players.Count < 2)
            {
                OpponentPlayerEntity = null;
                ChangeState(NetworkGameState.WaitingForOpponent);
                return;
            }

            PlayerRef opponentPlayer = FindOpponent(Runner.LocalPlayer);
            if (_networkService.TryGetNetworkPlayerEntity(opponentPlayer, out NetworkPlayerEntity opponentPlayerEntity))
            {
                OpponentPlayerEntity = opponentPlayerEntity;
                ChangeState(NetworkGameState.ReadyToPlay);
            }
            else
            {
                Debug.Log("OpponentPlayerRef connected but no NetworkPlayerEntity found for it.");
            }
        }

        private PlayerRef FindOpponent(PlayerRef localPlayer)
        {
            foreach (PlayerRef player in _networkService.Players)
            {
                if (player != localPlayer)
                    return player;
            }

            return PlayerRef.None;
        }

        private void ChangeState(NetworkGameState state)
        {
            /*if (State == state)
                return;*/

            State = state;
            StateChanged?.Invoke(State);
        }

        private void OnDestroy()
        {
            if (_networkService == null)
                return;

            _networkService.PlayersChanged -= TryUpdateMatchState;
            _networkService.PlayerEntitiesChanged -= TryUpdateMatchState;
        }
    }
}
