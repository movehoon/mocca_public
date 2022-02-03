using System.Collections;
using System.Collections.Generic;
using Unity.VectorGraphics;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace REEL.D2EEditor
{
	public class TabUI : MonoBehaviour
	{
        [SerializeField] private Sprite selectedSprite = null;
        [SerializeField] private Sprite unselectedSprite = null;
        //[SerializeField] private Text nameText = null;
        [SerializeField] private TMPro.TextMeshProUGUI nameText = null;
        [SerializeField] private RectTransform showBoxImageRect = null;
        [SerializeField] private TMPro.TextMeshProUGUI tabNamePopupText = null;

        private Image tabImage;
        private SVGImage SVG_TabImage;
        private RectTransform refRT;

        [SerializeField] private bool isSelected = false;
        [SerializeField] private string tabName = string.Empty;

        public UnityEvent tabSelectedEvent;
        public UnityEvent tabUnselectedEvent;

        Sprite TabImage
        {
            get
            {
                if (SVG_TabImage != null) return SVG_TabImage.sprite;
                if (tabImage != null) return tabImage.sprite;

                return null;
            }

            set
            {
                if (SVG_TabImage != null) SVG_TabImage.sprite = value;
                if (tabImage != null) tabImage.sprite = value;
            }
        }

        private void Awake()
        {
            tabImage = GetComponent<Image>();
            SVG_TabImage = GetComponent<SVGImage>();
            //nameText = GetComponentInChildren<Text>(true);
            if (nameText == null)
            {
                nameText = GetComponentInChildren<TMPro.TextMeshProUGUI>(true);
            }
            
            refRT = GetComponent<RectTransform>();
            ChangeState(false);

            if (tabSelectedEvent == null)
            {
                tabSelectedEvent = new UnityEvent();
            }

            if (tabUnselectedEvent == null)
            {
                tabUnselectedEvent = new UnityEvent();
            }
        }

        public void ChangeState(bool isSelected)
        {
            //if (isSelected) tabImage.sprite = selectedSprite;
            if (isSelected == true)
            {
                TabImage = selectedSprite;
            }
            else
            {
                //tabImage.sprite = unselectedSprite;
                TabImage = unselectedSprite;
            }

            this.isSelected = isSelected;
            if (this.isSelected == true)
            {
                tabSelectedEvent?.Invoke();
            }
            if (this.isSelected == false)
            {
                tabUnselectedEvent?.Invoke();
            }
        }

        public string TabName
        {
            get { return tabName; }
            set
            {
                tabName = value;
                if (nameText)
                {
                    nameText.text = value;
                }
                if (tabNamePopupText)
                {
                    tabNamePopupText.text = value;
                }
            }
        }

        float margin = 20f;
        float minSizeX = 200f;
        public void OnShowPopupTextName()
        {
            if (tabNamePopupText)
            {
                showBoxImageRect.GetComponent<Image>().sprite = isSelected ? selectedSprite : unselectedSprite;

                var size = showBoxImageRect.sizeDelta;
                size.x = tabNamePopupText.preferredWidth + margin;
                size.x = size.x <= minSizeX ? minSizeX : size.x;
                showBoxImageRect.sizeDelta = size;
            }
        }

        public Vector2 TabSize { get { return refRT.sizeDelta; } }
        public bool IsSelected {  get { return isSelected; } }
    }
}