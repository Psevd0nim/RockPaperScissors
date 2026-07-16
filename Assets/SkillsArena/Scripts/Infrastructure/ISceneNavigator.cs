using System;

namespace SkillsArena
{
    public interface ISceneNavigator
    {
        void LoadScene(string sceneName, float delay = 0f, Action onLoaded = null);
    }
}
