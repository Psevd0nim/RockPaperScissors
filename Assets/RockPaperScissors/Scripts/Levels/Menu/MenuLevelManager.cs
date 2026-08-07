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

        private FusionNetworkService _fusionNetworkService;
        private Coroutine _prepareGameCoroutine;

        public override void Init(AppServices appServices)
        {
            _menuLevel_UI_Manager.Init(appServices.AudioManager);
            _menuLevel_UI_Manager.OnPlayPressed += AfterPlayPressed;

            _rpsRoundController.Init(_menuLevel_UI_Manager);

            _fusionNetworkService = appServices.NetworkService;
            _fusionNetworkService.PlayersChanged += RefreshPlayers;

            RefreshPlayers();
        }

        public override void StartLevel()
        {
        }

        private void AfterPlayPressed()
        {
            StartGameSession();
        }

        private async void StartGameSession()
        {
            StartGameResult startGameResult = await _fusionNetworkService.StartGameAsync();

            Debug.Log($"StartGameResult.Ok: {startGameResult.Ok}");
            Debug.Log($"StartGameResult.ShutdownReason: {startGameResult.ShutdownReason}");
            Debug.Log($"StartGameResult.ErrorMessage: \"{startGameResult.ErrorMessage}\"");

            if (startGameResult.Ok == false)
                return;

            _fusionNetworkService.RegisterGlobal(_networkPlayerSpawner);
            _networkPlayerSpawner.SpawnEntityForExistingLocalPlayer();
            _menuLevel_UI_Manager.HidePlayButton();
        }

        private void RefreshPlayers()
        {
            StopPreparingGame();

            int playersCount = _fusionNetworkService.Players.Count;
            _menuLevel_UI_Manager.ShowPlayersCount(playersCount);

            if (playersCount == 0)
            {
                _rpsRoundController.StopGame(false);
                _menuLevel_UI_Manager.HidePlayers();
                return;
            }

            if (playersCount == 1)
            {
                _rpsRoundController.StopGame(true);
                _menuLevel_UI_Manager.ShowWaitingForPlayer();
                return;
            }

            PlayerRef localPlayer = _fusionNetworkService.LocalPlayer;
            PlayerRef opponentPlayer = FindOpponent(localPlayer);

            _prepareGameCoroutine = StartCoroutine(PrepareGame(localPlayer, opponentPlayer));
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

        private IEnumerator PrepareGame(PlayerRef localPlayer, PlayerRef opponentPlayer)
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

            _prepareGameCoroutine = null;
        }

        private void StopPreparingGame()
        {
            if (_prepareGameCoroutine == null)
                return;

            StopCoroutine(_prepareGameCoroutine);
            _prepareGameCoroutine = null;
        }

        private void OnDestroy()
        {
            StopPreparingGame();
            _rpsRoundController.StopGame(false);

            _menuLevel_UI_Manager.OnPlayPressed -= AfterPlayPressed;

            if (_fusionNetworkService != null)
            {
                _fusionNetworkService.PlayersChanged -= RefreshPlayers;
                _fusionNetworkService.UnregisterGlobal(_networkPlayerSpawner);
            }
        }
    }
}
