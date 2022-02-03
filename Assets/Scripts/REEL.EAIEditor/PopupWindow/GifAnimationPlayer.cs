using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace REEL.D2EEditor
{
    public class GifAnimationPlayer : MonoBehaviour
    {
        public Sprite[] sprites;
        public float playRate = 0.3f;
        public Image image;

        private bool isPause = false;
        int currentIndex = 0;
        int frameCount = 0;

        private void OnEnable()
        {
            if (image == null)
            {
                image = GetComponent<Image>();
            }

            frameCount = sprites.Length;

            isPause = false;
            currentIndex = 0;
            waitTime = Time.time + playRate;

            //Debug.Log($"frameCount: {frameCount} / playRate: {playRate} / wairTime: {waitTime}");
        }

        float waitTime = 0f;
        private void Update()
        {
            if (isPause)
            {
                return;
            }

            if (Time.time < waitTime)
            {
                return;
            }

            waitTime = Time.time + playRate;
            image.sprite = sprites[currentIndex++];

            if (currentIndex >= sprites.Length)
            {
                isPause = true;
                currentIndex = 0;
                Invoke("RePlay", 2f);
            }
        }

        private void RePlay()
        {
            isPause = false;
        }
    }
}