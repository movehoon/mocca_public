using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace REEL.D2EEditor
{
    [ExecuteInEditMode]
    public class SplitBar : MonoBehaviour, IDragHandler
    {
        public enum Type
        {
            Horizontal, Vertical
        }

        public Type type = Type.Horizontal;
        public float topOffset;

        public float slideMinX;
        public float slideMaxX;
        public float slideMinY;
        public float slideMaxY;

        public SplitBar oppositeMin = null;
        public SplitBar oppositeMax = null;

        [NonSerialized] public RectTransform refRect;
        [NonSerialized] public RectTransform root;

        private Action<RectTransform> OnDragEvent;

		int lastScreenWidth = Screen.width;
		int lastScreenHeight = Screen.height;

        public bool isDebug = false;
        private bool prevDebugFlag = false;

        private Image image = null;

        private Color normalColor = new Color(0.2f, 0.2f, 0.2f, 0.0f);
        private Color debugColor = Color.red;

        private void OnEnable()
        {
            if (image == null)
            {
                image = GetComponent<Image>();
            }

            refRect = GetComponent<RectTransform>();
            root = transform.parent.GetComponent<RectTransform>();
            prevDebugFlag = isDebug;
        }

		public void Update()
		{
			if( lastScreenWidth != Screen.width || lastScreenHeight != Screen.height)
			{
				lastScreenWidth = Screen.width;
				lastScreenHeight = Screen.height;

				//Invoke("ResizeWindow", 0.1f);
			}

            if (isDebug != prevDebugFlag)
            {
                image.color = isDebug ? debugColor : normalColor;
                prevDebugFlag = isDebug;
            }
		}

		private void ResizeWindow()
		{
			//Debug.Log("ResizeWindow");
			//OnDragEvent?.Invoke(refRect);
		}

		public void OnDrag(PointerEventData eventData)
        {
            Vector2 pos = eventData.position / root.localScale;
            float minX = oppositeMin == null ? slideMinX : oppositeMin.refRect.anchoredPosition.x + slideMinX;
            float maxX = oppositeMax == null ?
                (Screen.width / root.localScale.x - refRect.sizeDelta.x) - slideMaxX
                : oppositeMax.refRect.anchoredPosition.x - slideMaxX;
            pos.x = Mathf.Clamp(pos.x, minX, maxX);
            pos.y -= Screen.height / root.localScale.y;
            pos.y = Mathf.Clamp(pos.y, -Screen.height / root.localScale.y + slideMinY, topOffset - slideMaxY);

            if (type == Type.Horizontal)
            {
                pos.y = refRect.anchoredPosition.y;
                refRect.anchoredPosition = pos;
            }
            else
            {
                pos.x = refRect.anchoredPosition.x;
                refRect.anchoredPosition = pos;
            }

            OnDragEvent?.Invoke(refRect);
        }

        public void SubscritbeOnDrag(Action<RectTransform> onDrag)
        {
            OnDragEvent += onDrag;
        }
    }
}