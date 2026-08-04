using System.Threading.Tasks;
using Fusion;
using UnityEngine;

namespace MyProject
{
    public class FusionNetworkService
    {
        private NetworkRunner _runner;

        public async Task<StartGameResult> StartGameAsync()
        {
            GameObject runnerObject = new GameObject("NetworkRunner");
            _runner = runnerObject.AddComponent<NetworkRunner>();

            StartGameArgs args = new StartGameArgs
            {
                GameMode = GameMode.Shared,
                SessionName = "Test Room",
                PlayerCount = 2
            };

            Task<StartGameResult> operation = _runner.StartGame(args);
            StartGameResult startGameResult = await operation;

            return startGameResult;
        }
    }
}
