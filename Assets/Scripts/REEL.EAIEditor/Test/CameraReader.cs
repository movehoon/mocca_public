using UnityEngine;
using UnityEngine.UI;

namespace REEL.D2EEditor
{
	public class CameraReader : MonoBehaviour
	{
        public RawImage image;
        private WebCamTexture webcamTexture;

        private void OnEnable()
        {
            InitDevice();

            if (image == null)
            {
                image = GetComponent<RawImage>();
            }
        }

        void InitDevice()
        {
            WebCamDevice[] devices = WebCamTexture.devices;
            if (devices.Length > 0)
            {
                webcamTexture = new WebCamTexture(devices[0].name);
                webcamTexture.Play();
            }
        }

        private void Update()
        {
            UpdateImage();
        }

        void UpdateImage()
        {
            if (webcamTexture == null)
            {
                return;
            }

            image.texture = webcamTexture;
        }


    }
}