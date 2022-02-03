using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace REEL.D2EEditor
{
    public class BottomVerticalSetter : SizeSetterBase
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

            float sizeY = Mathf.Abs(-Screen.height / localScaleY - rect.anchoredPosition.y + splitBar.refRect.sizeDelta.y);
            //float sizeY = Mathf.Abs(-Screen.height / 2f - rect.anchoredPosition.y);
            //Debug.Log(Screen.height + " : " + 2f + " : " + rect.anchoredPosition.y + " : " + sizeY + " : " + (sizeY * localScaleY));
            //sizeY *= localScaleY;
            refRect.sizeDelta = new Vector2(refRect.sizeDelta.x, sizeY);
        }
    }
}