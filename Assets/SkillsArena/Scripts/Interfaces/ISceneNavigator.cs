using System;

namespace MyProject
{
    public interface ISceneNavigator
    {
        void LoadScene(string sceneName, float delay = 0f, Action onLoaded = null);
    }
}
