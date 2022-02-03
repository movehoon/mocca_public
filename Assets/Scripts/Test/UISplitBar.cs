using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace REEL.Test
{
	public class UISplitBar : MonoBehaviour
	{
        public delegate void onPositionUpdate(Vector2 position);
        onPositionUpdate positionUpdate;

        private RectTransform rectTransform;

        public enum DragDirection
        {
            LeftRight, TopDown
        }

        private void Awake()
        {
            rectTransform = GetComponent<RectTransform>();
        }

        public void SubscribeOnPositionUpdate(onPositionUpdate update)
        {
            if (positionUpdate == null)
                positionUpdate = new onPositionUpdate(update);
            else
                positionUpdate += update;
        }

        public void PositionUpdate(Vector2 position)
        {
            rectTransform.position = position;
            if (positionUpdate != null)
                positionUpdate(position);
        }
	}
}