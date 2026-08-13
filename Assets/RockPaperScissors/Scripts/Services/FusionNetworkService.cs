using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Fusion;
using Fusion.Sockets;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MyProject
{
    public class FusionNetworkService : INetworkRunnerCallbacks
    {
        public event Action PlayersChanged;
        public event Action PlayerEntitiesChanged;

        public List<PlayerRef> Players => _players;
        public PlayerRef LocalPlayer => _runner != null ? _runner.LocalPlayer : PlayerRef.None;

        private readonly List<PlayerRef> _players = new List<PlayerRef>();

        private NetworkRunner _runner;
        private readonly PlayerRegistry _playerRegistry;

        public FusionNetworkService()
        {
            _playerRegistry = PlayerRegistry.Instance;
            _playerRegistry.OnPlayerEntitiesChanged += AfterPlayerEntitiesChanged;
        }

        public async Task<StartGameResult> StartGameSessionAsync()
        {
            GameObject runnerObject = new GameObject("NetworkRunner");
            _runner = runnerObject.AddComponent<NetworkRunner>();
            _runner.AddCallbacks(this);

            SceneRef activeScene = SceneRef.FromIndex(SceneManager.GetActiveScene().buildIndex);
            NetworkSceneInfo sceneInfo = new NetworkSceneInfo();
            sceneInfo.AddSceneRef(activeScene);

            StartGameArgs args = new StartGameArgs
            {
                GameMode = GameMode.Shared,
                SessionName = "Test Room",
                PlayerCount = 2,
                Scene = sceneInfo
            };

            Task<StartGameResult> operation = _runner.StartGame(args);
            StartGameResult startGameResult = await operation;

            return startGameResult;
        }

        public bool TryGetNetworkPlayerEntity(PlayerRef player, out NetworkPlayerEntity playerEntity)
        {
            playerEntity = null;

            if (_runner.TryGetPlayerObject(player, out NetworkObject playerObject) == false)
                return false;

            return playerObject.TryGetComponent(out playerEntity);
        }

        public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
        {
            if (_players.Contains(player))
                return;

            _players.Add(player);
            NotifyPlayersChanged();

            Debug.Log($"OnPlayerJoined: {player}");
        }

        public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
        {
            _players.Remove(player);
            NotifyPlayersChanged();

            Debug.Log($"OnPlayerLeft: {player}");
        }

        private void AfterPlayerEntitiesChanged()
        {
            if (IsSessionRunning() == false)
                return;

            PlayerEntitiesChanged?.Invoke();
        }

        private void NotifyPlayersChanged()
        {
            if (IsSessionRunning() == false)
                return;

            PlayersChanged?.Invoke();
        }

        private bool IsSessionRunning()
        {
            return _runner != null && _runner.IsRunning;
        }

        public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
        {
            _players.Clear();
            _runner = null;
        }

        #region UnusedCallbacks
        public void OnConnectedToServer(NetworkRunner runner)
        {
        }

        public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
        {
        }

        public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token)
        {
        }

        public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data)
        {
        }

        public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason)
        {
        }

        public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken)
        {
        }

        public void OnInput(NetworkRunner runner, NetworkInput input)
        {
        }

        public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input)
        {
        }

        public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
        {
        }

        public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
        {
        }

        public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress)
        {
        }

        public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ReadOnlySpan<byte> data)
        {
        }

        public void OnSceneLoadDone(NetworkRunner runner)
        {
        }

        public void OnSceneLoadStart(NetworkRunner runner)
        {
        }

        public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
        {
        }

        #endregion
    }

    public class PlayerRegistry
    {
        public event Action OnPlayerEntitiesChanged;

        public static PlayerRegistry Instance => _instance ?? (_instance = new PlayerRegistry());

        public List<NetworkPlayerEntity> PlayerEntities => _playerEntities;

        private static PlayerRegistry _instance;
        private readonly List<NetworkPlayerEntity> _playerEntities = new List<NetworkPlayerEntity>();

        private PlayerRegistry()
        {

        }

        public void AddPlayerEntity(NetworkPlayerEntity playerEntity)
        {
            if (_playerEntities.Contains(playerEntity))
                return;

            _playerEntities.Add(playerEntity);
            OnPlayerEntitiesChanged?.Invoke();
        }

        public void RemovePlayerEntity(NetworkPlayerEntity playerEntity)
        {
            if (_playerEntities.Remove(playerEntity) == false)
                return;

            OnPlayerEntitiesChanged?.Invoke();
        }
    }
}
