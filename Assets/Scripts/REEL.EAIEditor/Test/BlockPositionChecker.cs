using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace REEL.Test
{
    public class BlockPositionChecker : MonoBehaviour
    {
        float interval = 0.3f;
        WaitForSeconds wait;

        new Transform transform;

        //private void Update()
        //{
        //    if (Input.GetMouseButtonDown(0))
        //    {
        //        Debug.Log("MousePos: " + Input.mousePosition);
        //    }
        //}

        private void OnEnable()
        {
            transform = GetComponent<Transform>();
            wait = new WaitForSeconds(interval);

            StartCoroutine(Logger());
        }

        IEnumerator Logger()
        {
            while(true)
            {
                yield return wait;
                Debug.Log(transform.position);
            }
        }
    }
}