using UnityEngine;
using UnityEngine.EventSystems;

namespace REEL.Test
{
    public class BlockDrag : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        private Vector2 offset;
        private RectTransform rectTransform;

        private void Awake()
        {
            rectTransform = GetComponent<RectTransform>();
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            Vector2 currentPos = rectTransform.position;
            offset = eventData.position - currentPos;
        }

        public void OnDrag(PointerEventData eventData)
        {
            rectTransform.position = eventData.position - offset;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            offset = Vector2.zero;
        }
    }
}