using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace REEL.D2EEditor
{
    public class CenterHorizontalSizeSetter : SizeSetterBase
    {
        public SplitBar leftSplitBar;
        public SplitBar rightSplitBar;

        protected override void OnEnable()
        {
            base.OnEnable();

            if (leftSplitBar) leftSplitBar.SubscritbeOnDrag(OnLeftSplitBarDrag);
            if (rightSplitBar) rightSplitBar.SubscritbeOnDrag(OnRightSplitBarDrag);
        }

        private void OnLeftSplitBarDrag(RectTransform rect)
        {
            UpdateLocalScale();

            Vector2 pos = leftSplitBar.refRect.anchoredPosition;
            pos.y = refRect.anchoredPosition.y;
            pos.x += leftSplitBar.refRect.sizeDelta.x;
            refRect.anchoredPosition = pos;
            float sizeX = rightSplitBar != null ?
                rightSplitBar.refRect.anchoredPosition.x - refRect.anchoredPosition.x
                : Screen.width - refRect.anchoredPosition.x;
            //sizeX *= localScaleX;
            refRect.sizeDelta = new Vector2(sizeX, refRect.sizeDelta.y);
        }

        private void OnRightSplitBarDrag(RectTransform rect)
        {
            UpdateLocalScale();

            float sizeX = rect.anchoredPosition.x - refRect.anchoredPosition.x;
            //sizeX *= localScaleX;
            refRect.sizeDelta = new Vector2(sizeX, refRect.sizeDelta.y);
        }
    }
}