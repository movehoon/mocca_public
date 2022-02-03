using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace REEL.D2EEditor
{
    public static class TMPProUGUIRectHelper
    {
        private static RectTransform refRect;

        public static RectTransform RefRect(this TMP_Text tmpText)
        {
            if (refRect == null)
            {
                refRect = tmpText.GetComponent<RectTransform>();
            }

            return refRect;
        }
    }

    public class MCTooltipWindow : MonoBehaviour
    {
        [Header("툴팁 생성할 때 보여줄 UI 참조")]
        public Image iconImage;
        public TMP_Text nodeNameText;
        public TMP_Text inputText;
        public TMP_Text outputText;
        public TMP_Text descText;

        [Header("화면 비율 맞추기 위해")]
        public Canvas mainCanvas;

        private RectTransform refRT;
        private Vector2 windowSize;

        private float baseHeight = 33.33f;
        private float descTextBaseHeight = 0f;

        private Vector2 anchorOriginValue = new Vector2(0f, 1f);
        private Vector2 pivotOriginValue = new Vector2(0.5f, 1f);
        private Vector3 screenBottomLeftToWorld;

        private Camera mainCamera;

        private void Awake()
        {
            if (refRT == null)
            {
                refRT = GetComponent<RectTransform>();
                windowSize = refRT.rect.size;
                descTextBaseHeight = descText.RefRect().rect.size.y;
            }

            if (mainCamera == null)
            {
                mainCamera = Camera.main;
            }
        }

        public void SetIcon(Sprite icon)
        {
            iconImage.sprite = icon;
        }

        public void SetInputText(string input)
        {
            inputText.text = input;
        }

        public void SetNodeNameText(string name)
        {
            nodeNameText.text = name;
        }

        public void SetOutputText(string output)
        {
            outputText.text = output;
        }

        public void SetDescText(string desc)
        {
            descText.text = desc;
        }

        private float widgetOffset = 10f;
        public void ShowWindow(Vector3 position)
        {
            if (mainCamera == null)
            {
                mainCamera = Camera.main;
            }

            if (refRT == null)
            {
                refRT = GetComponent<RectTransform>();
                windowSize = refRT.rect.size;

                screenBottomLeftToWorld = mainCamera.ScreenToWorldPoint(new Vector2(0f, 0f));
            }

            refRT.anchorMin = anchorOriginValue;
            refRT.anchorMax = anchorOriginValue;
            refRT.pivot = pivotOriginValue;

            //float xPos = position.x + (windowSize.x * mainCanvas.transform.localScale.x * 0.5f);
            //float yPos = position.y - (windowSize.y * mainCanvas.transform.localScale.y * 0.5f);
            float xPos = position.x + (windowSize.x * mainCanvas.transform.localScale.x * 0.75f);
            float yPos = position.y;

            refRT.position = new Vector3(xPos, yPos, 0f);
            gameObject.SetActive(true);

            // test.
            Vector2 size = inputText.rectTransform.sizeDelta;
            size.y = inputText.preferredHeight + widgetOffset > size.y ?
                inputText.preferredHeight + widgetOffset : size.y;
            inputText.rectTransform.sizeDelta = size;

            size = outputText.rectTransform.sizeDelta;
            size.y = outputText.preferredHeight + widgetOffset > size.y ?
                outputText.preferredHeight + widgetOffset : size.y;
            outputText.rectTransform.sizeDelta = size;
            Vector2 widgetPosition = outputText.rectTransform.anchoredPosition;
            widgetPosition.y = -inputText.rectTransform.sizeDelta.y;
            outputText.rectTransform.anchoredPosition = widgetPosition;

            // Resize Height of the Tooltip Window.
            Vector2 descRectSize = descText.RefRect().sizeDelta;
            descRectSize.y = descText.preferredHeight + widgetOffset > descText.RefRect().sizeDelta.y ?
                     descText.preferredHeight : descText.RefRect().sizeDelta.y;
            descText.RefRect().sizeDelta = descRectSize;
            widgetPosition = descText.rectTransform.anchoredPosition;
            widgetPosition.y = outputText.rectTransform.anchoredPosition.y -
                outputText.rectTransform.sizeDelta.y;
            descText.rectTransform.anchoredPosition = widgetPosition;

            float totalHeight = inputText.rectTransform.sizeDelta.y +
                outputText.rectTransform.sizeDelta.y +
                descText.rectTransform.sizeDelta.y + 10f;

            //size = refRT.sizeDelta;
            //size.y = (baseHeight * 2f) + descRectSize.y + 10f;
            //size.y = Mathf.Max(size.y, windowSize.y);
            //refRT.sizeDelta = size;

            size = refRT.sizeDelta;
            size.y = totalHeight;
            size.y = Mathf.Max(size.y, windowSize.y);
            refRT.sizeDelta = size;

            // tooltip 창의 corner.
            Vector3[] corners = new Vector3[4];
            refRT.GetWorldCorners(corners);

            if (corners[0].y < screenBottomLeftToWorld.y)
            {
                refRT.anchorMin = Vector2.zero;
                refRT.anchorMax = Vector2.zero;
                refRT.pivot = new Vector2(0.5f, 0f);

                refRT.position = new Vector3(xPos, yPos, 0f);
            }
            //else if (corners[3].x > tooltipWorkspaceBottomRightLimit.transform.position.x)
            //{
            //    refRT.anchorMin = new Vector2(1f, 0f);
            //    refRT.anchorMax = new Vector2(1f, 0f);

            //    refRT.pivot = new Vector2(1f, 0f);

            //    //refRT.position = new Vector3(xPos, yPos, 0f);
            //    refRT.position = tooltipWorkspaceBottomRightLimit.transform.position;
            //}

            //Utils.LogRed($"baseHeight: {baseHeight}");
            //Utils.LogRed($"descTextHeight: {descRectSize.y}");
        }

        public void CloseWindow()
        {
            gameObject.SetActive(false);

            ResetInfomation();
        }

        private void ResetInfomation()
        {
            iconImage.sprite = null;
            nodeNameText.text = string.Empty;
            inputText.text = string.Empty;
            outputText.text = string.Empty;
            descText.text = string.Empty;

            Vector2 size = descText.RefRect().sizeDelta;
            size.y = descTextBaseHeight;
            descText.RefRect().sizeDelta = size;

            size = inputText.rectTransform.sizeDelta;
            size.y = baseHeight;
            inputText.rectTransform.sizeDelta = size;

            size = outputText.rectTransform.sizeDelta;
            size.y = baseHeight;
            outputText.rectTransform.sizeDelta = size;
        }
    }
}