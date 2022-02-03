using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace REEL.Test
{
	public class GridHeightSetter : MonoBehaviour
	{
        RectTransform rectTransform;

        private void Awake()
        {
            rectTransform = GetComponent<RectTransform>();

            int childCount = transform.childCount;

            rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, 60f * childCount);
            rectTransform.position = new Vector2(rectTransform.position.x, -rectTransform.sizeDelta.y * 0.5f);
        }
    }
}