using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace REEL.D2EEditor
{
	public class CameraSimulator : MonoBehaviour
	{
        public string url = "http://localhost:8000/";
        public RawImage image;

        private void OnEnable()
        {
#if !UNITY_ANDROID
            if (image == null)
            {
                image = GetComponent<RawImage>();
            }
#endif

            InvokeRepeating("Download", 0f, 0.2f);
        }

        private void OnDisable()
        {
            CancelInvoke();
        }

        void Download()
        {
            StartCoroutine(DownloadImage());
        }

        private IEnumerator DownloadImage()
        {
            WWW www = new WWW(url);
            yield return www;

            byte[] bytes = www.bytes;

            Texture2D texture = new Texture2D(800, 450);
            texture.LoadImage(bytes);

            image.texture = texture;
        }
	}
}