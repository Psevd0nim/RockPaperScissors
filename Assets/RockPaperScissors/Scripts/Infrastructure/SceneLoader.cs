using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MyProject
{
    public class SceneLoader : ISceneNavigator
    {
        private ICoroutineRunner _coroutineRunner;

        public SceneLoader(ICoroutineRunner coroutineRunner)
        {
            _coroutineRunner = coroutineRunner;
        }

        public void LoadScene(string sceneName, float delay = 0f, Action onLoaded = null)
        {
            _coroutineRunner.StartCoroutine(LoadSceneAsync(sceneName, delay, onLoaded));
        }

        private IEnumerator LoadSceneAsync(string sceneName, float delay, Action onLoaded)
        {
            if (delay > 0f)
            {
                yield return new WaitForSeconds(delay);
            }

            AsyncOperation loadSceneOperation = SceneManager.LoadSceneAsync(sceneName);
            while (!loadSceneOperation.isDone)
            {
                yield return null;
            }
            onLoaded?.Invoke();
        }
    }
}
