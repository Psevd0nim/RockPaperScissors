using UnityEngine;

namespace MyProject
{
    public class BootstrapState : IDefaultState
    {
        private ICoroutineRunner _coroutineRunner;
        private GameStateMachine _gameStateMachine;
        private AudioManager _audioManager;

        public BootstrapState(GameStateMachine gameStateMachine, ICoroutineRunner coroutineRunner, AudioManager audioManager)
        {
            _coroutineRunner = coroutineRunner;
            _gameStateMachine = gameStateMachine;
            _audioManager = audioManager;
        }

        public void Enter()
        {
            AppServices appServices = CreateAppServices();
            _gameStateMachine.SetAppServices(appServices);
        }

        private AppServices CreateAppServices()
        {
            SceneLoader sceneLoader = new SceneLoader(_coroutineRunner);
            InputService inputService = new InputService();
            GameFactory gameFactory = new GameFactory();
            SaveAndLoadData saveAndLoadData = new SaveAndLoadData();
            GameData gameData = saveAndLoadData.LoadGameData();
            FusionNetworkService networkService = new FusionNetworkService();

            return new AppServices(
                sceneLoader,
                inputService,
                gameFactory,
                saveAndLoadData,
                gameData,
                _audioManager,
                networkService);
        }

        public void Exit()
        {
            Debug.Log($"Out from {GetType()}");
        }
    }
}
