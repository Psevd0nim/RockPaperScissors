namespace MyProject
{
    public class AppServices
    {
        public ISceneNavigator SceneNavigator { get; }
        public InputService InputService { get; }
        public GameFactory GameFactory { get; }
        public SaveAndLoadData SaveAndLoadData { get; }
        public GameData GameData { get; }
        public AudioManager AudioManager { get; }

        public AppServices(
            ISceneNavigator sceneNavigator,
            InputService inputService,
            GameFactory gameFactory,
            SaveAndLoadData saveAndLoadData,
            GameData gameData,
            AudioManager audioManager)
        {
            SceneNavigator = sceneNavigator;
            InputService = inputService;
            GameFactory = gameFactory;
            SaveAndLoadData = saveAndLoadData;
            GameData = gameData;
            AudioManager = audioManager;
        }
    }

    public class GameData
    {
        
    }
}
