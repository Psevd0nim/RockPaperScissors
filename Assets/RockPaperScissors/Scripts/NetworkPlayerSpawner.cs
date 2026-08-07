using Fusion;
using UnityEngine;

namespace MyProject
{
    public class NetworkPlayerSpawner : SimulationBehaviour, IPlayerJoined
    {
        [SerializeField] private NetworkObject _playerEntityPrefab;

        void IPlayerJoined.PlayerJoined(PlayerRef player)
        {
            SpawnLocalPlayerEntity(player);
        }

        public void SpawnEntityForExistingLocalPlayer()
        {
            SpawnLocalPlayerEntity(Runner.LocalPlayer);
        }

        public bool TryGetNetworkPlayerEntity(PlayerRef player, out NetworkPlayerEntity playerEntity)
        {
            playerEntity = null;

            if (Runner == null)
                return false;

            NetworkObject playerObject;

            if (Runner.TryGetPlayerObject(player, out playerObject) == false)
                return false;

            return playerObject.TryGetComponent(out playerEntity);
        }

        private void SpawnLocalPlayerEntity(PlayerRef player)
        {
            if (player != Runner.LocalPlayer)
                return;

            if (Runner.TryGetPlayerObject(player, out _))
                return;

            NetworkObject playerObject = Runner.Spawn(_playerEntityPrefab);
            Runner.SetPlayerObject(player, playerObject);
        }
    }
}
