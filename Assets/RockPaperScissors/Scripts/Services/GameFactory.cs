using UnityEngine;

namespace MyProject
{
    public class GameFactory : IService
    {
        private PrefabsConfig _prefabsConfig;

        public GameFactory()
        {
            _prefabsConfig = Resources.Load<PrefabsConfig>(Constants.PrefabsConfigPath);
        }
    }
}