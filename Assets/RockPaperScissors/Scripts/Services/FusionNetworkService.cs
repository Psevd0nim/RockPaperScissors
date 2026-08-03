using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Fusion;
using Fusion.Sockets;
using UnityEngine;

namespace MyProject
{
    public class FusionNetworkService : INetworkRunnerCallbacks
    {
        private const int MaxPlayers = 2;

        public event Action<NetworkConnectionStatus> ConnectionStatusChanged;
        public event Action PlayersChanged;

        public NetworkConnectionStatus Status { get; private set; } = NetworkConnectionStatus.Disconnected;
        public IReadOnlyList<PlayerRef> Players => _players;
        public string SessionName { get; private set; } = string.Empty;
        public string LastError { get; private set; } = string.Empty;

        private readonly List<PlayerRef> _players = new List<PlayerRef>();

        private NetworkRunner _runner;
        private GameObject _runnerObject;

        public async Task JoinOrCreateSessionAsync(string sessionName)
        {
            if (Status == NetworkConnectionStatus.Connecting || Status == NetworkConnectionStatus.Connected)
                return;

            sessionName = sessionName?.Trim();
            if (string.IsNullOrEmpty(sessionName))
                throw new ArgumentException("Session name cannot be empty.", nameof(sessionName));

            LastError = string.Empty;
            SessionName = sessionName;
            SetStatus(NetworkConnectionStatus.Connecting);
            CreateRunner();

            StartGameResult result;

            try
            {
                result = await _runner.StartGame(new StartGameArgs
                {
                    GameMode = GameMode.Shared,
                    SessionName = sessionName,
                    PlayerCount = MaxPlayers
                });
            }
            catch (Exception exception)
            {
                LastError = exception.Message;
                ClearRunner();
                SetStatus(NetworkConnectionStatus.Failed);
                throw;
            }

            if (result.Ok)
            {
                SetStatus(NetworkConnectionStatus.Connected);
                return;
            }

            LastError = string.IsNullOrEmpty(result.ErrorMessage)
                ? result.ShutdownReason.ToString()
                : result.ErrorMessage;

            ClearRunner();
            SetStatus(NetworkConnectionStatus.Failed);
        }

        public async Task DisconnectAsync()
        {
            if (_runner == null)
                return;

            SetStatus(NetworkConnectionStatus.Disconnecting);
            NetworkRunner runner = _runner;
            await runner.Shutdown();

            if (_runner == runner)
            {
                ClearRunner();
                SetStatus(NetworkConnectionStatus.Disconnected);
            }
        }

        public bool IsLocalPlayer(PlayerRef player)
        {
            return _runner != null && _runner.IsRunning && _runner.LocalPlayer == player;
        }

        public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
        {
            if (!_players.Contains(player))
            {
                _players.Add(player);
                _players.Sort((first, second) => first.PlayerId.CompareTo(second.PlayerId));
            }

            PlayersChanged?.Invoke();
        }

        public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
        {
            _players.Remove(player);
            PlayersChanged?.Invoke();
        }

        public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
        {
            if (runner != _runner)
                return;

            ClearRunner();
            SetStatus(NetworkConnectionStatus.Disconnected);
        }

        public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason)
        {
            LastError = reason.ToString();
        }

        public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
        {
            LastError = reason.ToString();
        }

        private void CreateRunner()
        {
            ClearRunner();

            _runnerObject = new GameObject("[Fusion] NetworkRunner");
            UnityEngine.Object.DontDestroyOnLoad(_runnerObject);

            _runner = _runnerObject.AddComponent<NetworkRunner>();
            _runner.ProvideInput = false;
            _runner.AddCallbacks(this);
        }

        private void ClearRunner()
        {
            if (_runner != null)
                _runner.RemoveCallbacks(this);

            if (_runnerObject != null)
                UnityEngine.Object.Destroy(_runnerObject);

            _runner = null;
            _runnerObject = null;
            _players.Clear();
            PlayersChanged?.Invoke();
        }

        private void SetStatus(NetworkConnectionStatus status)
        {
            Status = status;
            ConnectionStatusChanged?.Invoke(status);
        }

        public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
        public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
        public void OnInput(NetworkRunner runner, NetworkInput input) { }
        public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) { }
        public void OnConnectedToServer(NetworkRunner runner) { }
        public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token) { }
        public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList) { }
        public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) { }
        public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken) { }
        public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data) { }
        public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ReadOnlySpan<byte> data) { }
        public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress) { }
        public void OnSceneLoadDone(NetworkRunner runner) { }
        public void OnSceneLoadStart(NetworkRunner runner) { }
    }

    public class NetworkTestService 
    {
        private NetworkRunner _networkRunner;

        public void StartGame()
        {
            StartGameArgs startGameArgs = new StartGameArgs();
            startGameArgs.GameMode = GameMode.Shared;
            _networkRunner.StartGame(startGameArgs);
        }
    }
}