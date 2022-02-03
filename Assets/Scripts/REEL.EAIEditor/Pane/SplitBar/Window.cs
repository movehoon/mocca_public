using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace REEL.D2EEditor
{
    public class Window : MonoBehaviour
    {
        public enum Type
        {
            Fixed, Expandable
        }

        public Type windowType = Type.Fixed;

        public RectTransform topLeft;
        public RectTransform bottomRight;

        protected RectTransform refRect;
        protected RectTransform root;

        protected virtual void Awake()
        {
            refRect = GetComponent<RectTransform>();
            root = transform.parent.GetComponent<RectTransform>();
        }
    }
}