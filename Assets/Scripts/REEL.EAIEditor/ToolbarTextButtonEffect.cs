using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

namespace REEL.D2EEditor
{
    public class ToolbarTextButtonEffect : MonoBehaviour
    {
        [Header("텍스트 효과에 필요한 참조 값. Null이면 OnEnable에서 초기화.")]
        public TextMeshProUGUI targetText;
        public EventTrigger trigger;
        public Button button;

        [Header("일반상태/마우스오버 상태 시 폰트 스타일")]
        public FontStyles normalFontStyle = FontStyles.Normal;
        public FontStyles mouseOverFontStyle = FontStyles.Bold;
        public FontStyles mouseDownFontStyle = FontStyles.Bold;

        private void OnEnable()
        {
            if (targetText == null)
            {
                targetText = GetComponent<TextMeshProUGUI>();
            }

            if (button == null)
            {
                button = GetComponent<Button>();
            }

            if (trigger == null)
            {
                trigger = GetComponent<EventTrigger>();
                //EventTrigger.Entry onPointerDown = new EventTrigger.Entry();
                //onPointerDown.eventID = EventTriggerType.PointerDown;
                //onPointerDown.callback.AddListener(OnPointerDown);
                //trigger.triggers.Add(onPointerDown);

                EventTrigger.Entry onPointerEnter = new EventTrigger.Entry();
                onPointerEnter.eventID = EventTriggerType.PointerEnter;
                onPointerEnter.callback.AddListener(OnPointerEnter);
                trigger.triggers.Add(onPointerEnter);

                EventTrigger.Entry onPointerDown = new EventTrigger.Entry();
                onPointerDown.eventID = EventTriggerType.PointerDown;
                onPointerDown.callback.AddListener(OnPointerDown);
                trigger.triggers.Add(onPointerDown);

                EventTrigger.Entry onPointerUp = new EventTrigger.Entry();
                onPointerUp.eventID = EventTriggerType.PointerUp;
                onPointerUp.callback.AddListener(OnPointerUp);
                trigger.triggers.Add(onPointerUp);

                EventTrigger.Entry onPointerExit = new EventTrigger.Entry();
                onPointerExit.eventID = EventTriggerType.PointerExit;
                onPointerExit.callback.AddListener(OnPointerExit);
                trigger.triggers.Add(onPointerExit);
            }
        }

        public void OnPointerEnter(BaseEventData data)
        {
            if (button.interactable == false)
            {
                return;
            }

            targetText.fontStyle = mouseOverFontStyle;
        }

        public void OnPointerDown(BaseEventData data)
        {
            if (button.interactable == false)
            {
                return;
            }

            targetText.fontStyle = mouseDownFontStyle;
        }

        public void OnPointerUp(BaseEventData data)
        {
            if (button.interactable == false)
            {
                return;
            }

            targetText.fontStyle = normalFontStyle;
        }

        public void OnPointerExit(BaseEventData data)
        {
            if (button.interactable == false)
            {
                return;
            }

            targetText.fontStyle = normalFontStyle;
        }
    }
}