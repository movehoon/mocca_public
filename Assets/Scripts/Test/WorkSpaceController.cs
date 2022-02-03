using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace REEL.Test
{
    public static class RectTransformExtension
    {
        public static void SetPivot(this RectTransform rectTransform, Vector2 pivot)
        {
            if (rectTransform == null) return;

            Vector2 size = rectTransform.rect.size;
            Vector2 deltaPivot = rectTransform.pivot - pivot;
            Vector3 deltaPosition = new Vector3(deltaPivot.x * size.x, deltaPivot.y * size.y);
            rectTransform.pivot = pivot;
            rectTransform.localPosition -= deltaPosition;
        }

        private static RectTransform refRect;
        public static RectTransform GetRectTransform(this Transform transform)
        {
            if (refRect == null)
            {
                refRect = transform.GetComponent<RectTransform>();
            }

            return refRect;
        }
    }

    public class WorkSpaceController : MonoBehaviour, IScrollHandler, IDragHandler
    {
        [System.Serializable]
        public class Points
        {
            public Transform topLeft;
            public Transform topRight;
            public Transform bottomLeft;
            public Transform bottomRight;
        }

        [Header("Pane Parameters")]
        public Transform content;

        public float moveOffset = 10f;
        public float moveThreshold = 10f;

        public float scaleOffset = 0.25f;
        public float scaleMin = 0.25f;
        public float scaleMax = 10f;

        [Header("Workspace Points")]
        public Points workspacePoints;

        [Header("Pane Points")]
        public Points panePoints;

        public void OnDrag(PointerEventData eventData)
        {
            // Check only right button drag.
            if (eventData.button != PointerEventData.InputButton.Right)
            {
                return;
            }

            // Horizontal.
            if (eventData.delta.x > moveThreshold)
            {
                content.position += Vector3.right * moveOffset * Time.deltaTime;
            }
            else if (eventData.delta.x < -moveThreshold)
            {
                content.position -= Vector3.right * moveOffset * Time.deltaTime;
            }

            // Vertical.
            if (eventData.delta.y > moveThreshold)
            {
                content.position += Vector3.up * moveOffset * Time.deltaTime;
            }
            else if (eventData.delta.y < -moveThreshold)
            {
                content.position -= Vector3.up * moveOffset * Time.deltaTime;
            }
        }
        
        public void OnScroll(PointerEventData eventData)
        {
            RectTransform contentRect = content.GetComponent<RectTransform>();
            float scrollY = eventData.scrollDelta.y;
            Vector3 scale = content.localScale;
            Vector2 pivot;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(contentRect, Input.mousePosition, null, out pivot);
            pivot = Rect.PointToNormalized(contentRect.rect, pivot);

            if (scrollY > 0f)
            {
                if (scale.x == scaleMax)
                {
                    return;
                }

                scale += new Vector3(scaleOffset, scaleOffset, scaleOffset);
            }
            else
            {
                if (scale.x == scaleMin)
                {
                    return;
                }

                scale -= new Vector3(scaleOffset, scaleOffset, scaleOffset);
            }

            contentRect.SetPivot(pivot);
            ClampScale(ref scale);
            content.localScale = scale;
        }

        private void ClampScale(ref Vector3 scale)
        {
            scale.x = Mathf.Clamp(scale.x, scaleMin, scaleMax);
            scale.y = Mathf.Clamp(scale.y, scaleMin, scaleMax);
            scale.z = Mathf.Clamp(scale.z, scaleMin, scaleMax);
        }
        
        //private void CheckSize()
        //{
        //    // Scale 값이 아니라, Width/Height의 크기를 키우는 방식으로 변경해야함.

        //    //Vector3 scale = content.localScale;
        //    Vector2 xyMargin = Vector2.zero;
        //    if (panePoints.topLeft.position.x > workspacePoints.topLeft.position.x)
        //    {
        //        float xMargin = Mathf.Abs(panePoints.topLeft.position.x - workspacePoints.topLeft.position.x);
        //        xyMargin.x = xMargin * 2f;
        //    }

        //    if (panePoints.topRight.position.x < workspacePoints.topRight.position.x)
        //    {
        //        float xMargin = Mathf.Abs(workspacePoints.topRight.position.x - panePoints.topRight.position.x);
        //        xyMargin.x = xMargin * 2f;
        //    }

        //    if (panePoints.bottomLeft.position.y > workspacePoints.bottomLeft.position.y)
        //    {
        //        float yMargin = Mathf.Abs(panePoints.bottomLeft.position.y - workspacePoints.bottomLeft.position.y);
        //        xyMargin.y = yMargin * 2f;
        //    }

        //    if (panePoints.topLeft.position.y < workspacePoints.topLeft.position.y)
        //    {
        //        float yMargin = Mathf.Abs(workspacePoints.topLeft.position.y - panePoints.topLeft.position.y);
        //        xyMargin.y = yMargin * 2f;
        //    }

        //    content.GetRectTransform().sizeDelta += xyMargin;
        //}
    }
}