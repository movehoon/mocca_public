using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace REEL.D2EEditor
{
    public class AutoHideWithDelay : MonoBehaviour
    {
        public float delayTimeInSecond = 2f;

        private void OnEnable()
        {
            Invoke("Hide", delayTimeInSecond);
        }

        private void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}