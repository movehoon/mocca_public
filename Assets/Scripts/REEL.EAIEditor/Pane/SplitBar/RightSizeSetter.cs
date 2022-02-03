using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace REEL.D2EEditor
{
    public class RightSizeSetter : SizeSetterBase
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

            float sizeX = Screen.width / root.localScale.x - rect.anchoredPosition.x - 20f;
            //sizeX -= rect.sizeDelta.x + 10f;
            //sizeX -= rect.sizeDelta.x;
            //sizeX *= localScaleX;
            refRect.sizeDelta = new Vector2(sizeX, refRect.sizeDelta.y);
        }
    }
}