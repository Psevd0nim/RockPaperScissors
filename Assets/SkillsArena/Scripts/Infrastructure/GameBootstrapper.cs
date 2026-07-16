using UnityEngine;
using UnityEngine.SceneManagement;

namespace SkillsArena
{
    public class GameBootstrapper : MonoBehaviour, ICoroutineRunner
    {
        private static GameBootstrapper _instance;
        private GameStateMachine _gameStateMachine;

        private void Awake()
        {
            if (_instance != null)
            {
                Destroy(gameObject);
                return;
            }
            _instance = this;
            DontDestroyOnLoad(gameObject);

            _gameStateMachine = new GameStateMachine(this, ServiceLocator.Instance);
            _gameStateMachine.Enter<BootstrapState>();
        }

        private void Start()
        {
            Application.targetFrameRate = 144;

            string targetSceneName = Constants.MenuSceneName;
            
            #if UNITY_EDITOR
                targetSceneName = SceneManager.GetActiveScene().name;
            #endif

            _gameStateMachine.Enter<LoadLevelState, string, float>(targetSceneName, 0f);
        }
    }
}
