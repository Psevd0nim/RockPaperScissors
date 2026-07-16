using System;
using UnityEngine;

namespace SkillsArena
{
    public abstract class LevelManager : MonoBehaviour
    {
        public Action<LevelManager, string, float> OnExitLevel;

        public abstract void Init(AppServices appServices);

        public abstract void StartLevel();
    }
}
