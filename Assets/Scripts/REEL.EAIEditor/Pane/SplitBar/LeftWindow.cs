using UnityEngine;

namespace REEL.D2EEditor
{
    public class LeftWindow : Window
    {
        public SplitBar splitBar;

        protected override void Awake()
        {
            base.Awake();

            splitBar.SubscritbeOnDrag(OnSplitBarDrag);
        }

        private void OnSplitBarDrag(RectTransform rect)
        {
            float sizeX = rect.anchoredPosition.x - refRect.anchoredPosition.x;
            refRect.sizeDelta = new Vector2(sizeX, refRect.sizeDelta.y);
        }
    }
}
