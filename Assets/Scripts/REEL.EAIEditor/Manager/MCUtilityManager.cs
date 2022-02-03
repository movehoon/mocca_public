using System;
using System.Collections;
using System.IO;
using System.Text;
using UnityEngine;

namespace REEL.D2EEditor
{
    public class MCUtilityManager : MonoBehaviour
    {
        public readonly string screenshotSaveFolderBase = "MoccaScreenshot";
        public readonly string screenshotSaveFileBase = "MCScreenshot_";
        public TMPro.TextMeshProUGUI currentTimeText;
        
        private static int screenshotNumber = 1;
        private StringBuilder stringBuilder;

        private void OnEnable()
        {
            InvokeRepeating("UpdateCurrenTime", 0.0f, 0.05f);
        }

        private void UpdateCurrenTime()
        {
            if (currentTimeText != null)
            {
                currentTimeText.text = DateTime.Now.ToString("d") + "\n" + DateTime.Now.ToString("T");
            }
        }

        public void CaptureScreenShot()
        {
            string path = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.Desktop), 
                screenshotSaveFolderBase
            );

            if (Directory.Exists(path) == false)
            {
                Directory.CreateDirectory(path);
            }

            path = Path.Combine(path, DateTime.Now.ToString("d"));
            if (Directory.Exists(path) == false)
            {
                Directory.CreateDirectory(path);
            }

            path = ScreenshotPath;
            ScreenCapture.CaptureScreenshot(path);

            StartCoroutine(ShowCaptureResultMessage(path));
        }

        WaitForSeconds pointSecond = new WaitForSeconds(0.5f);
        private IEnumerator ShowCaptureResultMessage(string path)
        {
            yield return pointSecond;

            string message = LocalizationManager.ConvertText("[ID_CAPTURESCREENSHOT]");
            message = message.Replace("{path}", path);
            MessageBox.Show(message);
        }

        private string ScreenshotPath
        {
            get
            {
                if (stringBuilder == null)
                {
                    stringBuilder = new StringBuilder();
                }

                stringBuilder.Clear();
                string path = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                    screenshotSaveFolderBase,
                    DateTime.Now.ToString("d")
                );
                stringBuilder.Append(path);
                stringBuilder.Append("/" + screenshotSaveFileBase);
                stringBuilder.Append(DateTime.Now.ToString("HH_mm_ss"));
                stringBuilder.Append(".png");

                return stringBuilder.ToString();
            }
        }
    }
}