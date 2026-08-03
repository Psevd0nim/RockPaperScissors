using System;
using UnityEngine;

namespace MyProject
{
    public abstract class LevelManager : MonoBehaviour
    {
        public Action<LevelManager, string, float> OnExitLevel;
        
        public abstract void Init(AppServices appServices);

        public abstract void StartLevel();
    }
}
