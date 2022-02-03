using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace REEL.D2EEditor
{
    [System.Serializable]
    public class DragInfo
    {
        public Vector2 topLeft = Vector2.zero;
        public Vector2 bottomRight = Vector2.zero;
        public RectTransform topLeftMarker;
        public RectTransform bottomRightMarker;

        public void Reset()
        {
            topLeft = Vector2.zero;
            bottomRight = Vector2.zero;
        }

        public override string ToString()
        {
            return "topLeft: " + topLeft.ToString() + " , bottomRight " + bottomRight.ToString();
        }
    }

    public class LineArea : MonoBehaviour
    {
        public RectTransform canvasRectTransform;
        public DragPane pane;
        public Image dragArea;
        public RectTransform dragLine;
        public RectTransform topLeftMarker;
        public RectTransform bottomRightMarker;

        private RectTransform rectTransform;

        [SerializeField]
        private DragInfo dragInfo = new DragInfo();

        private void Awake()
        {
            rectTransform = dragArea.GetComponent<RectTransform>();

            pane.SubscribeDragEvent(OnDrag);
            pane.SubscribePointerUpEvent(OnPointerUp);
            pane.SubscribeBeginDragEvent(OnBeginDrag);
        }

        public void OnBeginDrag(PointerEventData data)
        {
			//드래그 시작시 툴팁 나타나지 않도록 처리
			MCTooltipManager.Instance.CanShowPopup(false);


			Vector2 mouseLocalPoint = Vector2.zero;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(dragLine, data.position, data.pressEventCamera, out mouseLocalPoint);

            rectTransform.anchoredPosition = dragInfo.topLeft = mouseLocalPoint;
        }

        void SelectLogic()
        {

        }

        void SetAllEnabled()
        {
            if (!dragArea.gameObject.activeSelf)
            {
                dragArea.gameObject.SetActive(true);
            }
        }

        void SetAllDisabled()
        {
            if (dragArea.gameObject.activeSelf)
            {
                dragArea.gameObject.SetActive(false);
            }
        }

        static Vector3[] corners = new Vector3[4];
        public void OnDrag(PointerEventData data)
        {
            SetAllEnabled();

            RectTransform paneRect = pane.GetComponent<RectTransform>();

            Vector2 mouseLocalPoint = Vector2.zero;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(paneRect, data.position, data.pressEventCamera, out mouseLocalPoint);

            Vector3 rectWorldPos = rectTransform.position;
            Vector3 rectLocalPos = RectTransformUtility.WorldToScreenPoint(data.pressEventCamera, rectWorldPos);
            Vector2 rectLocalPoint = Vector2.zero;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(paneRect, rectLocalPos, data.pressEventCamera, out rectLocalPoint);

            float width = (mouseLocalPoint.x - rectLocalPoint.x);
            float height = (mouseLocalPoint.y - rectLocalPoint.y);

            //dragInfo.topLeft.x = width > 0f ? rectLocalPoint.x : mouseLocalPoint.x;
            //dragInfo.topLeft.y = height > 0f ? mouseLocalPoint.y : rectLocalPoint.y;
            //dragInfo.bottomRight.x = width > 0f ? mouseLocalPoint.x : rectLocalPoint.x;
            //dragInfo.bottomRight.y = height > 0f ? rectLocalPoint.y : mouseLocalPoint.y;

            //dragInfo.topLeft = corners[1];
            //dragInfo.bottomRight = corners[3];

            // Setting Markers.
            //topLeftMarker.anchoredPosition = dragInfo.topLeft;
            //bottomRightMarker.anchoredPosition = dragInfo.bottomRight;
            
            rectTransform.GetWorldCorners(corners);
            topLeftMarker.position = corners[1];
            bottomRightMarker.position = corners[3];
            dragInfo.topLeftMarker = topLeftMarker;
            dragInfo.bottomRightMarker = bottomRightMarker;

            rectTransform.pivot = new Vector2(width > 0f ? 0f : 1f, height > 0f ? 0f : 1f);
            rectTransform.sizeDelta = new Vector2(Mathf.Abs(width), Mathf.Abs(height));

            MCNodeDragManager.Instance.SetBlockSelectionWithDragArea(dragInfo);

            MCCommentManager.Instance.SetCommentSelectionWithDragArea(dragInfo);
        }

        public  void OnPointerUp(PointerEventData data)
        {
			//드래그 끝나면 툴팁 가능하도록 처리
			MCTooltipManager.Instance.CanShowPopup(true);

			SelectLogic();
            SetAllDisabled();
            dragInfo.Reset();
        }
    }
}