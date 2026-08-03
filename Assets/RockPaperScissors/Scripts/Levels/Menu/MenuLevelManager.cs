using UnityEngine;

namespace MyProject
{
    public class MenuLevelManager : LevelManager
    {
        [SerializeField] private MenuLevel_UI_Manager _menuLevel_UI_Manager;

        public override void Init(AppServices appServices)
        {
            _menuLevel_UI_Manager.Init(appServices.AudioManager);
        }

        public override void StartLevel()
        {
            
        }
    }
}
