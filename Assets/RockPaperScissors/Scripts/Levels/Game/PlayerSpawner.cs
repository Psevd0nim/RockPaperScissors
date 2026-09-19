using Fusion;
using UnityEngine;

namespace MyProject
{
    public class PlayerSpawner : SimulationBehaviour, IPlayerJoined
    {
        [SerializeField] private PrefabsConfig _prefabsConfig;

        public void PlayerJoined(PlayerRef player)
        {
            if(player != Runner.LocalPlayer)
                return;
            NetworkPlayerEntity localPlayerEntity = Runner.Spawn(_prefabsConfig.playerPrefab);
            Runner.SetPlayerObject(Runner.LocalPlayer, localPlayerEntity.Object);
        }
    }
}
