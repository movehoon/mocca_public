using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace REEL.D2EEditor
{
    public class DragPane : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        public RectTransform canvasRectTransform = null;
        public Transform topLeftMarker = null;
        public Transform bottomRightMarker = null;

        private event Action<PointerEventData> dragEvent = null;
        private event Action<PointerEventData> pointerUpEvent = null;
        private event Action<PointerEventData> beginDragEvent = null;

        private RectTransform rectTransform = null;

        private Camera mainCam = null;

        void OnEnable()
        {
            rectTransform = GetComponent<RectTransform>();
            mainCam = Camera.main;
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            //if (eventData.button != PointerEventData.InputButton.Left)
            if (!CanDrag(eventData))
            {
                return;
            }

            beginDragEvent?.Invoke(eventData);
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (!CanDrag(eventData))
            {
                return;
            }

            float minX = topLeftMarker.position.x;
            float maxX = bottomRightMarker.position.x;
            float minY = bottomRightMarker.position.y;
            float maxY = topLeftMarker.position.y;
            //Debug.Log(topLeftMarker.position + " , " + bottomRightMarker.position + " , " + eventData.position);

            float x = Mathf.Clamp(eventData.position.x, minX, maxX);
            float y = Mathf.Clamp(eventData.position.y, minY, maxY);
            eventData.position = new Vector2(x, y);

            dragEvent?.Invoke(eventData);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (!CanDrag(eventData))
            {
                return;
            }

            pointerUpEvent?.Invoke(eventData);
        }

        public void SubscribeDragEvent(Action<PointerEventData> dragDelegate)
        {
            dragEvent += dragDelegate;
        }

        public void SubscribePointerUpEvent(Action<PointerEventData> pointerUpDelegate)
        {
            pointerUpEvent += pointerUpDelegate;
        }

        public void SubscribeBeginDragEvent(Action<PointerEventData> beginDragDelegate)
        {
            beginDragEvent += beginDragDelegate;
        }

        private bool CanDrag(PointerEventData eventData)
        {
            return eventData.button == PointerEventData.InputButton.Left
                && !MCPlayStateManager.Instance.IsSimulation;
        }
    }
}