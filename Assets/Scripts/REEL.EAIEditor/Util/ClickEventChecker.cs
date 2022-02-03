using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace REEL.D2EEditor
{
    public class ClickEventChecker : MonoBehaviour, IPointerDownHandler, IPointerClickHandler, IDragHandler
    {
        // 더블 클릭이 인정되는 마우스 포인터 거리 (단위: 스크린 좌표).
        // 첫 클릭과 두 번째 클릭했을 때의 포인터 위치 사이의 거리 측정.
        public float doubleClickAcceptanceDistance = 5f;

        public UnityEvent onClick;
        public UnityEvent onDoubleClick;

        // 싱글/더블 클릭 확인을 위한 변수.
        private float clickedTime = 0f;
        private float clickDelay = 0.25f;
        private int clickCount = 0;

        // 포인터 이동 거리 측정을 위한 변수.
        private Vector2 firstClickPosition = Vector2.zero;

        private void Update()
        {
            if (Utils.IsProjectNullOrOnSimulation)
            {
                ResetValues();

                return;
            }

            if (clickedTime != 0f && (Time.time - clickedTime) > clickDelay)
            {
                if (clickCount == 1)
                {
                    onClick.Invoke();
                }

                else if (clickCount >= 2)
                {
                    float distance = Vector2.Distance(firstClickPosition, Input.mousePosition);
                    if (distance <= doubleClickAcceptanceDistance)
                    {
                        onDoubleClick.Invoke();
                    }
                    // for debug.
                    //else
                    //{
                    //    Utils.LogRed($"Out of acceptance distance: {distance}");
                    //}
                }

                ResetValues();
            }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            // 첫 번째 클릭인 경우, 클릭 위치 저장.
            if (clickCount == 0)
            {
                firstClickPosition = eventData.position;
            }

            ++clickCount;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            clickedTime = Time.time;
        }

        public void OnDrag(PointerEventData eventData)
        {
            ResetValues();
        }

        private void ResetValues()
        {
            clickedTime = 0f;
            clickCount = 0;
            firstClickPosition = Vector2.zero;
        }
    }
}