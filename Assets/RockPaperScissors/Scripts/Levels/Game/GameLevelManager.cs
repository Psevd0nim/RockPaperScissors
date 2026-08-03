using UnityEngine;

namespace MyProject
{
    public class GameLevelManager : LevelManager
    {
        [SerializeField] private Game_UI_Manager _uiManager;

        public override void Init(AppServices appServices)
        {
            
        }

        public override void StartLevel()
        {
            _uiManager.SetTransitionStatus(true);
        }
    }
}
