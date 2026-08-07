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
        public event Action PlayersChanged;

        public List<PlayerRef> Players => _players;
        public PlayerRef LocalPlayer => _runner != null ? _runner.LocalPlayer : PlayerRef.None;

        private readonly List<PlayerRef> _players = new List<PlayerRef>();

        private NetworkRunner _runner;

        public async Task<StartGameResult> StartGameAsync()
        {
            GameObject runnerObject = new GameObject("NetworkRunner");
            _runner = runnerObject.AddComponent<NetworkRunner>();
            _runner.AddCallbacks(this);

            StartGameArgs args = new StartGameArgs
            {
                GameMode = GameMode.Shared,
                SessionName = "Test Room",
                PlayerCount = 2
            };

            Task<StartGameResult> operation = _runner.StartGame(args);
            StartGameResult startGameResult = await operation;

            return startGameResult;
        }

        public bool RegisterGlobal(SimulationBehaviour simulationBehaviour)
        {
            if (_runner == null || _runner.IsRunning == false)
                return false;

            _runner.AddGlobal(simulationBehaviour);
            return true;
        }

        public void UnregisterGlobal(SimulationBehaviour simulationBehaviour)
        {
            if (_runner == null || _runner.IsRunning == false)
                return;

            _runner.RemoveGlobal(simulationBehaviour);
        }

        public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
        {
            if (_players.Contains(player) == false)
                _players.Add(player);

            PlayersChanged?.Invoke();

            Debug.Log($"OnPlayerJoined: {player}");
        }

        public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
        {
            _players.Remove(player);
            PlayersChanged?.Invoke();

            Debug.Log($"OnPlayerLeft: {player}");
        }

#region UnrealisedCallbacks
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

        public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
        {
        }

#endregion
    }
}
