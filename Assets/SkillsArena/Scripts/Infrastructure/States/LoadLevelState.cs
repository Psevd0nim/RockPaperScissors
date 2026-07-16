using UnityEngine;
using UnityEngine.SceneManagement;

namespace SkillsArena
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
            Debug.Log($"Out from {GetType()}");
        }

        private void AfterLevelLoaded()
        {
            Debug.Log(SceneManager.GetActiveScene().name);
            _gameStateMachine.Enter<StartGameState>();
        }
    }
}
