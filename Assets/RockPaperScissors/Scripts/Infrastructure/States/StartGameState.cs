using UnityEngine;

namespace MyProject
{
    public class StartGameState : IDefaultState
    {
        private GameStateMachine _stateMachine;

        public StartGameState(GameStateMachine stateMachine)
        {
            _stateMachine = stateMachine;
        }

        public void Enter()
        {
            LevelManager levelManager = Object.FindAnyObjectByType<LevelManager>();
            if (levelManager == null)
            {
                throw new System.Exception("LevelManager not found on loaded scene.");
            }

            levelManager.OnExitLevel += ChangeScene;
            levelManager.Init(_stateMachine.AppServices);
            levelManager.StartLevel();
        }

        private void ChangeScene(LevelManager levelManager, string levelName, float time)
        {
            levelManager.OnExitLevel -= ChangeScene;
            _stateMachine.Enter<LoadLevelState, string, float>(levelName, time);
        }

        public void Exit()
        {
            Debug.Log($"Out from {GetType()}");
        }
    }
}
