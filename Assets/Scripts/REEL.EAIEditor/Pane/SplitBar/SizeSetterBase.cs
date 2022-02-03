using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace REEL.D2EEditor
{
    public class SizeSetterBase : MonoBehaviour
    {
        public RectTransform canvasRect;

        protected RectTransform refRect;
        protected RectTransform root;
        protected float localScaleX;
        protected float localScaleY;

        protected virtual void OnEnable()
        {
            refRect = GetComponent<RectTransform>();
            root = transform.parent.GetComponent<RectTransform>();
            UpdateLocalScale();
        }

        protected void UpdateLocalScale()
        {
            localScaleX = canvasRect.localScale.x;
            localScaleY = canvasRect.localScale.y;
        }
    }
}