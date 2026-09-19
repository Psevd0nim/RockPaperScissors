using UnityEngine;

namespace MyProject
{
    public class Factory : MonoBehaviour
    {
        [SerializeField] private PrefabsConfig _prefabsConfig;

        [SerializeField] private NetworkPlayerEntity _playerPrefab;

        public NetworkPlayerEntity CreateNetworkPlayerEntity()
        {
            return Instantiate(_prefabsConfig.playerPrefab);
        }
    }
}