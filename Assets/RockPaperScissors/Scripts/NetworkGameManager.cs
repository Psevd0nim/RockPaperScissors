using System;
using System.Collections;
using Fusion;
using UnityEngine;

namespace MyProject
{
    public class NetworkGameManager : NetworkBehaviour
    {
        public event Action<NetworkGameState> StateChanged;

        public NetworkGameState State { get; private set; } = NetworkGameState.Idle;
        public NetworkPlayerEntity LocalPlayerEntity { get; private set; }
        public NetworkPlayerEntity OpponentPlayerEntity { get; private set; }
        public int LocalPlayerId { get; private set; }
        public int OpponentPlayerId { get; private set; }
        public int PlayersCount => _networkService?.Players.Count ?? 0;

        [SerializeField] private NetworkPlayerEntity _playerEntityPrefab;

        private FusionNetworkService _networkService;
        private Coroutine _waitForPlayerEntitiesCoroutine;

        public void Init(FusionNetworkService networkService)
        {
            _networkService = networkService;
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

            CompleteConnection();
        }

        public override void Spawned()
        {
            NetworkPlayerEntity localPlayerEntity = Runner.Spawn(_playerEntityPrefab);
            Runner.SetPlayerObject(Runner.LocalPlayer, localPlayerEntity.Object);
        }

        private void CompleteConnection()
        {
            _networkService.PlayersChanged += OnPlayersChanged;
            OnPlayersChanged();
        }

        private void OnPlayersChanged()
        {
            if (_networkService.Players.Count < 2)
            {
                CancelWaitingForPlayerEntities();
                OpponentPlayerEntity = null;
                OpponentPlayerId = 0;
                ChangeState(NetworkGameState.WaitingForOpponent);
                return;
            }

            TryPreparePlayers();
        }

        private void TryPreparePlayers()
        {
            if (State == NetworkGameState.ReadyToPlay || _waitForPlayerEntitiesCoroutine != null)
                return;

            PlayerRef localPlayer = _networkService.LocalPlayer;
            PlayerRef opponentPlayer = FindOpponent(localPlayer);

            if (opponentPlayer == PlayerRef.None)
                return;

            _waitForPlayerEntitiesCoroutine = StartCoroutine(WaitForPlayerEntities(localPlayer, opponentPlayer));
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

        private IEnumerator WaitForPlayerEntities(PlayerRef localPlayer, PlayerRef opponentPlayer)
        {
            LocalPlayerEntity = null;
            OpponentPlayerEntity = null;

            while (LocalPlayerEntity == null || OpponentPlayerEntity == null)
            {
                _networkService.TryGetNetworkPlayerEntity(localPlayer, out NetworkPlayerEntity localPlayerEntity);
                _networkService.TryGetNetworkPlayerEntity(opponentPlayer, out NetworkPlayerEntity opponentPlayerEntity);

                LocalPlayerEntity = localPlayerEntity;
                OpponentPlayerEntity = opponentPlayerEntity;

                yield return null;
            }

            LocalPlayerId = localPlayer.PlayerId;
            OpponentPlayerId = opponentPlayer.PlayerId;
            _waitForPlayerEntitiesCoroutine = null;
            ChangeState(NetworkGameState.ReadyToPlay);
        }

        private void CancelWaitingForPlayerEntities()
        {
            if (_waitForPlayerEntitiesCoroutine == null)
                return;

            StopCoroutine(_waitForPlayerEntitiesCoroutine);
            _waitForPlayerEntitiesCoroutine = null;
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
            CancelWaitingForPlayerEntities();

            if (_networkService != null)
                _networkService.PlayersChanged -= OnPlayersChanged;
        }
    }
}
