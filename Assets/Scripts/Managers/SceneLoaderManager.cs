using System.Collections;
using Shared;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Managers
{
    public class SceneLoaderManager : Singleton<SceneLoaderManager>
    {
        [SerializeField] private GameObject loadingScreen;
        [SerializeField] private Slider progressBar;


        private void Start()
        {
            loadingScreen.SetActive(false);
        }

        public void LoadLevel(string sceneName)
        {
            // Activate the loading screen
            if (loadingScreen)
            {
                loadingScreen.SetActive(true);
            }


            StartCoroutine(LoadLevelAsync(sceneName));
        }


        public void ExitGame()
        {
            Application.Quit();
        }


        private IEnumerator LoadLevelAsync(string sceneName)
        {
            // Start loading the scene
            AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);
            if (operation != null)
            {
                operation.allowSceneActivation = false; // Control when the scene activates

                while (!operation.isDone)
                {
                    // Update progress bar and text
                    float progress = Mathf.Clamp01(operation.progress / 0.9f);
                    if (progressBar)
                    {
                        progressBar.value = progress;
                    }

                    // Activate the scene when loading is almost done
                    if (operation.progress >= 0.9f)
                    {
                        operation.allowSceneActivation = true;
                    }

                    yield return null;
                }
            }

            // Deactivate the loading screen after the scene loads
            if (loadingScreen)
            {
                loadingScreen.SetActive(false);
            }
        }
    }
}