using UnityEngine;
using UnityEngine.SceneManagement;

namespace MyProject
{
    public class LoadLevelState : IPayloadedState<string, float>
    {
        private GameStateMachine _gameStateMachine;

        private bool _isFirstLoad = true;

        public LoadLevelState(GameStateMachine gameStateMachine)
        {
            _gameStateMachine = gameStateMachine;
        }

        public void Enter(string name, float time)
        {
            if (name == SceneManager.GetActiveScene().name && _isFirstLoad)
            {
                AfterLevelLoaded();
                _isFirstLoad = false;
            }
            else
                _gameStateMachine.AppServices.SceneNavigator.LoadScene(name, time, AfterLevelLoaded);
        }

        public void Exit()
        {
            
        }

        private void AfterLevelLoaded()
        {
            _gameStateMachine.Enter<StartGameState>();
        }
    }
}
