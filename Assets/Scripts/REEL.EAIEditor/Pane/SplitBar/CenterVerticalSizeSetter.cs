using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace REEL.D2EEditor
{
    public class CenterVerticalSizeSetter : SizeSetterBase
    {
        public SplitBar splitBar;
        public float offsetY;

        protected override void OnEnable()
        {
            base.OnEnable();

            splitBar.SubscritbeOnDrag(OnSplitBarDrag);
        }

        private void OnSplitBarDrag(RectTransform rect)
        {
            UpdateLocalScale();

            float sizeY = Mathf.Abs(rect.anchoredPosition.y + rect.sizeDelta.y - refRect.anchoredPosition.y - offsetY);
            //sizeY *= localScaleY;
            refRect.sizeDelta = new Vector2(refRect.sizeDelta.x, sizeY);
        }
    }
}