using UnityEngine;
using UnityEngine.SceneManagement;

namespace MyProject
{
    public class GameBootstrapper : MonoBehaviour, ICoroutineRunner
    {
        private static GameBootstrapper _instance;
        private GameStateMachine _gameStateMachine;

        [SerializeField] private AudioManager _audioManager;
        [SerializeField] private bool _startWithCurrentScene;

        private void Awake()
        {
            if (_instance != null)
            {
                Destroy(gameObject);
                return;
            }
            _instance = this;
            DontDestroyOnLoad(gameObject);
            
            CreateInfrastructure();
        }

        private void CreateInfrastructure()
        {
            _gameStateMachine = new GameStateMachine(this, _audioManager);
            _gameStateMachine.Enter<BootstrapState>();
        }

        private void Start()
        {
            Application.targetFrameRate = 144;
            StartLoadGame();
        }

        private void StartLoadGame()
        {
            string targetSceneName = Constants.MenuSceneName;
            if (_startWithCurrentScene)
            {
#if UNITY_EDITOR
                targetSceneName = SceneManager.GetActiveScene().name;
#endif
            }
            _gameStateMachine.Enter<LoadLevelState, string, float>(targetSceneName, 0f);
        }
    }
}
