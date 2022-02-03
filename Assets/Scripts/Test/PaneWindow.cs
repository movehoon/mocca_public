using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace REEL.Test
{
	public class PaneWindow : MonoBehaviour
	{
        public enum AnchorPosition
        {
            TopLeft, TopRight, BottomLeft, BottomRight
        }

        public enum SplitBarType
        {
            Horizontal, Vertical
        }

        public UISplitBar splitBar;

        public AnchorPosition anchorPosition = AnchorPosition.TopLeft;
        public SplitBarType splitBarType = SplitBarType.Horizontal;

        private RectTransform rectTransform;

        private void Awake()
        {
            if (splitBar)
            {
                if (splitBarType == SplitBarType.Vertical)
                    splitBar.SubscribeOnPositionUpdate(UpdateWidth);
                else if (splitBarType == SplitBarType.Horizontal)
                    splitBar.SubscribeOnPositionUpdate(UpdateHeight);
            }

            rectTransform = GetComponent<RectTransform>();
        }

        void UpdateHeight(Vector2 position)
        {
            switch (anchorPosition)
            {
                case AnchorPosition.BottomLeft:
                case AnchorPosition.BottomRight:
                    {
                        float height = position.y;
                        rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, height);
                    }
                    break;
                case AnchorPosition.TopLeft:
                case AnchorPosition.TopRight:
                    {
                        float height = 1080f - position.y;
                        rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, height);
                    }
                    break;
                default: break;
            }
        }

        void UpdateWidth(Vector2 position)
        {
            switch (anchorPosition)
            {
                case AnchorPosition.TopLeft:
                case AnchorPosition.BottomLeft:
                    {
                        float width = position.x;
                        rectTransform.sizeDelta = new Vector2(width, rectTransform.sizeDelta.y);
                    }
                break;
                case AnchorPosition.TopRight:
                case AnchorPosition.BottomRight:
                    {
                        float width = 1920f - position.x;
                        rectTransform.sizeDelta = new Vector2(width, rectTransform.sizeDelta.y);
                    }
                break;
                default: break;
            }
        }
    }
}