using Fusion;
using UnityEngine;

namespace MyProject
{
    public class Factory : MonoBehaviour
    {
        [SerializeField] private PrefabsConfig _prefabsConfig;

        public NetworkPlayerEntity SpawnLocalPlayer(NetworkRunner runner)
        {
            NetworkPlayerEntity player = runner.Spawn(_prefabsConfig.playerPrefab);

            player.SetNickname(PlayerPrefs.GetString("PlayerName", "Player123"));

            runner.SetPlayerObject(runner.LocalPlayer, player.Object);

            return player;
        }

        public NetworkPlayerEntity SpawnBot(NetworkRunner runner)
        {
            NetworkPlayerEntity bot = runner.Spawn(_prefabsConfig.playerPrefab);

            bot.SetNickname("Bot");

            return bot;
        }
    }
}