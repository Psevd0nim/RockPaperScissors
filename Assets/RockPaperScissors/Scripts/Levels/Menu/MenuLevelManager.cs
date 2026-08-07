using System.Collections;
using Fusion;
using UnityEngine;

namespace MyProject
{
    public class MenuLevelManager : LevelManager
    {
        [SerializeField] private MenuLevel_UI_Manager _menuLevel_UI_Manager;
        [SerializeField] private NetworkPlayerSpawner _networkPlayerSpawner;
        [SerializeField] private RpsRoundController _rpsRoundController;
        [SerializeField] private ConnectingIndicator _connectingIndicator;

        private FusionNetworkService _fusionNetworkService;

        private bool _gameCompetitionStarted;

        public override void Init(AppServices appServices)
        {
            _menuLevel_UI_Manager.Init(appServices.AudioManager);
            _menuLevel_UI_Manager.OnPlayPressed += AfterPlayPressed;

            _rpsRoundController.Init(_menuLevel_UI_Manager);

            _fusionNetworkService = appServices.NetworkService;
        }

        public override void StartLevel()
        {
        }

        private void AfterPlayPressed()
        {
            TryStartGameSession();
        }

        private async void TryStartGameSession()
        {
            PrepareForConnection();

            StartGameResult startGameResult = await _fusionNetworkService.StartGameSessionAsync();

            Debug.Log($"StartGameResult.Ok: {startGameResult.Ok}");
            Debug.Log($"StartGameResult.ShutdownReason: {startGameResult.ShutdownReason}");
            Debug.Log($"StartGameResult.ErrorMessage: \"{startGameResult.ErrorMessage}\"");

            if (startGameResult.Ok == false)
            {
                ResetAfterFailedConnection();
                return;
            }

            CompleteConnection();
        }

        private void PrepareForConnection()
        {
            _menuLevel_UI_Manager.HidePlayButton();
            _connectingIndicator.Show();
        }

        private void ResetAfterFailedConnection()
        {
            _connectingIndicator.Hide();
            _menuLevel_UI_Manager.ShowPlayButton();
        }

        private void CompleteConnection()
        {
            _connectingIndicator.Hide();
            _fusionNetworkService.RegisterGlobal(_networkPlayerSpawner);
            _networkPlayerSpawner.SpawnEntityForExistingLocalPlayer();

            _fusionNetworkService.PlayersChanged += AfterTotalPlayersChanged;
            AfterTotalPlayersChanged();
        }

        private void AfterTotalPlayersChanged()
        {
            int playersCount = _fusionNetworkService.Players.Count;
            _menuLevel_UI_Manager.ShowPlayersCount(playersCount);

            switch (playersCount)
            {
                case 1:
                    _menuLevel_UI_Manager.ShowWaitingForPlayer();
                    if (_gameCompetitionStarted)
                    {
                        _rpsRoundController.ResetGame();
                        _gameCompetitionStarted = false;
                    }
                    break;
                case 2:
                    StartGameCompetitionForTwoPlayers();
                    break;
            }
        }

        private void StartGameCompetitionForTwoPlayers()
        {
            PlayerRef localPlayer = _fusionNetworkService.LocalPlayer;
            PlayerRef opponentPlayer = FindOpponent(localPlayer);

            StartCoroutine(WaitForPlayerEntitiesAndStartGame(localPlayer, opponentPlayer));
        }

        private PlayerRef FindOpponent(PlayerRef localPlayer)
        {
            foreach (PlayerRef player in _fusionNetworkService.Players)
            {
                if (player != localPlayer)
                    return player;
            }

            return PlayerRef.None;
        }

        private IEnumerator WaitForPlayerEntitiesAndStartGame(PlayerRef localPlayer, PlayerRef opponentPlayer)
        {
            NetworkPlayerEntity localPlayerEntity = null;
            NetworkPlayerEntity opponentPlayerEntity = null;

            while (localPlayerEntity == null || opponentPlayerEntity == null)
            {
                _networkPlayerSpawner.TryGetNetworkPlayerEntity(localPlayer, out localPlayerEntity);
                _networkPlayerSpawner.TryGetNetworkPlayerEntity(opponentPlayer, out opponentPlayerEntity);

                yield return null;
            }

            _rpsRoundController.StartGame(localPlayerEntity, opponentPlayerEntity, localPlayer.PlayerId, opponentPlayer.PlayerId);
            _gameCompetitionStarted = true;
        }
    }
}
