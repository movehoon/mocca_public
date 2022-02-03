using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace REEL.D2EEditor
{
    public class WorkspaceScrollRect : ScrollRect
    {
        //public override void OnBeginDrag(PointerEventData eventData)
        //{
        //    if (eventData.button == PointerEventData.InputButton.Right)
        //    {
        //        //eventData.button = PointerEventData.InputButton.Left;
        //        base.OnBeginDrag(eventData);
        //    }
        //}

        private bool isDirty = false;

        protected override void LateUpdate()
        {
            base.LateUpdate();

            if (isDirty == true)
            {
                if (horizontalScrollbar != null)
                {
                    horizontalScrollbar.value = 0f;
                }

                if (verticalScrollbar != null)
                {
                    verticalScrollbar.value = 1f;
                }

                isDirty = false;
            }
        }

        public void ResetState()
        {
            //if (horizontalScrollbar != null)
            //{
            //    horizontalScrollbar.value = 0f;
            //}

            //if (verticalScrollbar != null)
            //{
            //    verticalScrollbar.value = 1f;
            //}
            isDirty = true;
        }

        public override void OnDrag(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Right)
            {
                //eventData.button = PointerEventData.InputButton.Left;
                //base.OnDrag(eventData);
                content.localPosition += new Vector3(eventData.delta.x, eventData.delta.y);
                normalizedPosition = new Vector2(
                    Mathf.Clamp(normalizedPosition.x, 0f, 1f),
                    Mathf.Clamp(normalizedPosition.y, 0f, 1f)
                );
            }
        }

        //public override void OnEndDrag(PointerEventData eventData)
        //{
        //    if (eventData.button == PointerEventData.InputButton.Right)
        //    {
        //        //eventData.button = PointerEventData.InputButton.Left;
        //        base.OnEndDrag(eventData);
        //    }
        //}
    }
}