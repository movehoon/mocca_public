using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckSize : MonoBehaviour
{
    public Sprite sprite;

    private void Awake()
    {
        StartCoroutine(TestLoop());
    }

    IEnumerator TestLoop()
    {
        for (int ix = 0; ix < 10; ++ix)
        {
            yield return new WaitForSeconds(2f);

            Debug.Log(ix);
        }
    }
}