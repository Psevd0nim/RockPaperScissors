using Newtonsoft.Json;
using UnityEngine;

namespace MyProject
{
    public class SaveAndLoadData : IService
    {
        private GameData _gameData;

        public void SaveGameData()
        {
            string jsonData = JsonConvert.SerializeObject(_gameData);
            PlayerPrefs.SetString(Constants.SaveDataKey, jsonData);
        }

        public GameData LoadGameData()
        {
            string jsonData = PlayerPrefs.GetString(Constants.SaveDataKey, JsonConvert.SerializeObject(new GameData()));
            _gameData = JsonConvert.DeserializeObject<GameData>(jsonData);
            return _gameData;
        }
    }
}