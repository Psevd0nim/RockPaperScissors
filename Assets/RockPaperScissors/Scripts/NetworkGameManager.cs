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
        public int PlayersCount => _networkService?.Players.Count ?? 0;

        [SerializeField] private NetworkPlayerEntity _playerEntityPrefab;

        private FusionNetworkService _networkService;
        private NetworkRunner _runner;
        private Coroutine _waitForPlayerEntitiesCoroutine;
        private bool _isSpawned;

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
            _runner = Runner;

            NetworkPlayerEntity localPlayerEntity = Runner.Spawn(_playerEntityPrefab);
            Runner.SetPlayerObject(Runner.LocalPlayer, localPlayerEntity.Object);

            LocalPlayerEntity = localPlayerEntity;
            _isSpawned = true;

            NetworkGameState previousState = State;
            OnPlayersChanged();

            if (State == NetworkGameState.WaitingForOpponent && previousState == State)
                StateChanged?.Invoke(State);
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
                CancelPlayerPreparation();
                OpponentPlayerEntity = null;
                ChangeState(NetworkGameState.WaitingForOpponent);
                return;
            }

            TryStartPlayerPreparation();
        }

        private void TryStartPlayerPreparation()
        {
            if (_isSpawned == false || _waitForPlayerEntitiesCoroutine != null)
                return;

            ChangeState(NetworkGameState.PreparingPlayers);
            _waitForPlayerEntitiesCoroutine = _runner.StartCoroutine(WaitForPlayerEntities());
        }

        private IEnumerator WaitForPlayerEntities()
        {
            yield return null;

            PlayerRef localPlayer = _networkService.LocalPlayer;
            PlayerRef opponentPlayer = FindOpponent(localPlayer);

            while (opponentPlayer != PlayerRef.None)
            {
                bool localPlayerFound = _networkService.TryGetNetworkPlayerEntity(
                    localPlayer,
                    out NetworkPlayerEntity localPlayerEntity);
                bool opponentPlayerFound = _networkService.TryGetNetworkPlayerEntity(
                    opponentPlayer,
                    out NetworkPlayerEntity opponentPlayerEntity);

                if (localPlayerFound && opponentPlayerFound)
                {
                    LocalPlayerEntity = localPlayerEntity;
                    OpponentPlayerEntity = opponentPlayerEntity;
                    _waitForPlayerEntitiesCoroutine = null;
                    ChangeState(NetworkGameState.ReadyToPlay);
                    yield break;
                }

                yield return null;
                opponentPlayer = FindOpponent(localPlayer);
            }

            _waitForPlayerEntitiesCoroutine = null;
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

        private void CancelPlayerPreparation()
        {
            if (_waitForPlayerEntitiesCoroutine == null)
                return;

            _runner.StopCoroutine(_waitForPlayerEntitiesCoroutine);
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
            if (_runner != null)
                CancelPlayerPreparation();

            if (_networkService != null)
                _networkService.PlayersChanged -= OnPlayersChanged;
        }
    }
}
