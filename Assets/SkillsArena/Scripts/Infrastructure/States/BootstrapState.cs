using UnityEngine;

namespace SkillsArena
{
    public class BootstrapState : IDefaultState
    {
        private ICoroutineRunner _coroutineRunner;
        private ServiceLocator _serviceLocator;
        private GameStateMachine _gameStateMachine;

        public BootstrapState(ICoroutineRunner coroutineRunner, ServiceLocator serviceLocator, GameStateMachine gameStateMachine)
        {
            _coroutineRunner = coroutineRunner;
            _serviceLocator = serviceLocator;
            _gameStateMachine = gameStateMachine;
        }

        public void Enter()
        {
            AppServices appServices = CreateAppServices();
            RegisterServices(appServices);
            _gameStateMachine.SetAppServices(appServices);
        }

        private AppServices CreateAppServices()
        {
            SceneLoader sceneLoader = new SceneLoader(_coroutineRunner);
            InputService inputService = Application.isMobilePlatform ? new MobileInput() : new DesktopInput();
            GameFactory gameFactory = new GameFactory();
            SaveAndLoadData saveAndLoadData = new SaveAndLoadData();
            GameData gameData = saveAndLoadData.LoadGameData();

            return new AppServices(sceneLoader, inputService, gameFactory, saveAndLoadData, gameData);
        }

        private void RegisterServices(AppServices appServices)
        {
            _serviceLocator.RegisterService(appServices.InputService);
            _serviceLocator.RegisterService(appServices.GameFactory);
            _serviceLocator.RegisterService(appServices.SaveAndLoadData);
            _serviceLocator.RegisterService(appServices.GameData);
        }

        public void Exit()
        {
            Debug.Log($"Out from {GetType()}");
        }
    }
}
