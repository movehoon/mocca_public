using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace REEL.D2EEditor
{
    public class LeftSizeSetter : SizeSetterBase
    {
        public SplitBar splitBar;

        protected override void OnEnable()
        {
            base.OnEnable();

            splitBar.SubscritbeOnDrag(OnSplitBarDrag);
        }

        private void OnSplitBarDrag(RectTransform rect)
        {
            UpdateLocalScale();

            float sizeX = rect.anchoredPosition.x - refRect.anchoredPosition.x;
            //sizeX += splitBar.refRect.sizeDelta.x * 0.5f;
            //sizeX *= localScaleX;
            refRect.sizeDelta = new Vector2(sizeX, refRect.sizeDelta.y);
        }
    }
}