using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using System.IO;
using System;

namespace REEL.D2EEditor
{
	public class MCSceneLoader : MonoBehaviour
	{
        public string sceneName;
        public Image progressBar;
        public string bundleIdentifier = "com.reel.d2e";

        //Test.
        //public TMPro.TextMeshProUGUI text;

        private void OnEnable()
        {
            Application.targetFrameRate = 60;

            StartCoroutine("LoadScene");
        }

        private void CheckFireBaseFolder()
        {
            string localAppPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            localAppPath += "\\" + bundleIdentifier;

            if (Directory.Exists(localAppPath) == false)
            {
                Directory.CreateDirectory(localAppPath);
            }
        }

        IEnumerator LoadScene()
        {
            CheckFireBaseFolder();

            yield return new WaitForSeconds(0.5f);

            AsyncOperation loader = SceneManager.LoadSceneAsync(sceneName);
            loader.allowSceneActivation = false;

            float time = 0f;
            while (!loader.isDone)
            {
                yield return null;

                time += Time.deltaTime;

                if (loader.progress < 0.9f)
                {
                    progressBar.fillAmount = Mathf.Lerp(progressBar.fillAmount, loader.progress, time);
                    if (progressBar.fillAmount >= loader.progress)
                    {
                        time = 0f;
                    }
                }
                else
                {
                    progressBar.fillAmount = Mathf.Lerp(progressBar.fillAmount, 1f, time);

                    if (progressBar.fillAmount == 1.0f)
                    {
                        loader.allowSceneActivation = true;
                        yield break;
                    }
                }
            }
        }
    }
}