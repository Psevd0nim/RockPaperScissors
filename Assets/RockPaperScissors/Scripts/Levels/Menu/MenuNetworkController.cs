using System;
using System.Text;
using System.Threading.Tasks;
using Fusion;
using UnityEngine;

namespace MyProject
{
    public class MenuNetworkController : IDisposable
    {
        private readonly MenuLevel_UI_Manager _view;
        private readonly FusionNetworkService _networkService;

        private bool _networkOperationInProgress;

        public MenuNetworkController(MenuLevel_UI_Manager view, FusionNetworkService networkService)
        {
            _view = view;
            _networkService = networkService;
        }

        public void Init()
        {
            _view.ConnectRequested += Connect;
            _view.DisconnectRequested += Disconnect;
            _networkService.ConnectionStatusChanged += RefreshConnectionStatus;
            _networkService.PlayersChanged += RefreshPlayers;

            RefreshConnectionStatus(_networkService.Status);
            RefreshPlayers();
        }

        public void Dispose()
        {
            _view.ConnectRequested -= Connect;
            _view.DisconnectRequested -= Disconnect;
            _networkService.ConnectionStatusChanged -= RefreshConnectionStatus;
            _networkService.PlayersChanged -= RefreshPlayers;
        }

        private async void Connect(string sessionName)
        {
            await RunNetworkOperation(() => _networkService.JoinOrCreateSessionAsync(sessionName));
        }

        private async void Disconnect()
        {
            await RunNetworkOperation(_networkService.DisconnectAsync);
        }

        private async Task RunNetworkOperation(Func<Task> operation)
        {
            if (_networkOperationInProgress)
                return;

            _networkOperationInProgress = true;

            try
            {
                await operation();
            }
            catch (Exception exception)
            {
                Debug.LogException(exception);
            }
            finally
            {
                _networkOperationInProgress = false;
            }
        }

        private void RefreshConnectionStatus(NetworkConnectionStatus status)
        {
            string statusText = status switch
            {
                NetworkConnectionStatus.Disconnected => "Disconnected",
                NetworkConnectionStatus.Connecting => "Connecting...",
                NetworkConnectionStatus.Connected => $"Connected: {_networkService.SessionName}",
                NetworkConnectionStatus.Disconnecting => "Disconnecting...",
                NetworkConnectionStatus.Failed => $"Connection failed\n{_networkService.LastError}",
                _ => status.ToString()
            };

            _view.ShowConnectionStatus(statusText);
        }

        private void RefreshPlayers()
        {
            if (_networkService.Players.Count == 0)
            {
                _view.ShowPlayers("Players: 0");
                return;
            }

            StringBuilder text = new StringBuilder($"Players: {_networkService.Players.Count}");

            foreach (PlayerRef player in _networkService.Players)
            {
                string localLabel = _networkService.IsLocalPlayer(player) ? " (you)" : string.Empty;
                text.Append($"\nPlayer {player.PlayerId}{localLabel}");
            }

            _view.ShowPlayers(text.ToString());
        }
    }
}
