using UnityEngine;
using UnityEngine.SceneManagement;

namespace MyProject
{
    public class GameBootstrapper : MonoBehaviour, ICoroutineRunner
    {
        private static GameBootstrapper _instance;
        private GameStateMachine _gameStateMachine;

        [SerializeField] private AudioManager _audioManager;

        private void Awake()
        {
            if (_instance != null)
            {
                Destroy(gameObject);
                return;
            }
            _instance = this;
            DontDestroyOnLoad(gameObject);

            _gameStateMachine = new GameStateMachine(this, _audioManager);
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
